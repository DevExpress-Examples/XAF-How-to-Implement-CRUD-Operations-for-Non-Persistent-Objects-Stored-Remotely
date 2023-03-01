using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Xpo.DB;

namespace NonPersistentObjectsDemo.Module {

    public class DataStoreMapping {
        public DBTable Table;
        public Func<object> Create;
        public Action<object, object[], ObjectMap> Load;
        public Action<object, object[]> Save;
        public Action<object, object> SetKey;
        public Func<object, object> GetKey;
        public IEnumerable<Column> RefColumns;
        public struct Column {
            public int Index;
            public Type Type;
        }
    }

    class DataStoreObjectLoader {
        struct PreResult {
            public DataStoreMapping Mapping;
            public Type ObjectType;
            public SelectStatement Statement;
        }
        struct PostResult {
            public DataStoreMapping Mapping;
            public List<object> Objects;
            public SelectStatementResult Result;
        }
        private ObjectMap objectMap;
        private IDataStore dataStore;
        private Dictionary<Type, DataStoreMapping> mappings;
        public DataStoreObjectLoader(Dictionary<Type, DataStoreMapping> mappings, IDataStore dataStore, ObjectMap objectMap) {
            this.mappings = mappings;
            this.dataStore = dataStore;
            this.objectMap = objectMap;
        }
        private DBColumn GetKeyColumn(DataStoreMapping mapping) {
            return mapping.Table.Columns.First(c => c.IsKey);
        }
        private CriteriaOperator BuildByKeyCriteria(Type objectType, object key, string alias) {
            return new BinaryOperator(
                new QueryOperand(GetKeyColumn(mappings[objectType]), alias),
                new OperandValue(key),
                BinaryOperatorType.Equal);
        }
        private SelectStatement PhaseOne(DataStoreMapping mapping, Type objectType, CriteriaOperator dbCriteria, string alias) {
            var statement = new SelectStatement(mapping.Table, alias);
            statement.Condition = dbCriteria;
            foreach(var column in mapping.Table.Columns) {
                statement.Operands.Add(new QueryOperand(column, alias));
            }
            return statement;
        }
        private List<object> PhaseTwo(DataStoreMapping mapping, Type objectType, SelectStatementResult result, List<PreResult> toLoad) {
            List<object> objects = new List<object>();
            int keyColumnIndex = mapping.Table.Columns.IndexOf(GetKeyColumn(mapping));
            List<DataStoreMapping.Column> refColumns = mapping.RefColumns.ToList();
            foreach(var row in result.Rows) {
                var key = row.Values[keyColumnIndex];
                if(key == null)
                    throw new DataException("Key cannot be null.");
                var obj = objectMap.Get(objectType, key);
                if(obj == null) {
                    obj = mapping.Create();
                    objectMap.Add(objectType, key, obj);
                }
                objects.Add(obj);
                foreach(var member in refColumns) {
                    var alias = "T";
                    var dbCriteria = BuildByKeyCriteria(member.Type, row.Values[member.Index], alias);
                    var memberMapping = mappings[member.Type];
                    toLoad.Add(new PreResult() {
                        Mapping = memberMapping,
                        ObjectType = member.Type,
                        Statement = PhaseOne(memberMapping, member.Type, dbCriteria, alias)
                    });
                }
            }
            return objects;
        }
        private void PhaseThree(DataStoreMapping mapping, List<object> objects, SelectStatementResult result) {
            for(int i = 0; i < objects.Count; i++) {
                mapping.Load(objects[i], result.Rows[i].Values, objectMap);
                objectMap.Accept(objects[i]);
            }
        }
        public IList<object> LoadObjects(Type objectType, CriteriaOperator criteria) {
            var mapping = mappings[objectType];
            var alias = "T";
            var dbCriteria = SimpleDataStoreCriteriaVisitor.Transform(criteria, mapping.Table, alias);
            return LoadObjectsCore(objectType, dbCriteria, alias);
        }
        private IList<object> LoadObjectsCore(Type objectType0, CriteriaOperator dbCriteria, string alias) {
            List<object> objects0 = null;
            var preResults = new List<PreResult>();
            var postResults = new List<PostResult>();
            var mapping0 = mappings[objectType0];
            var statement0 = PhaseOne(mapping0, objectType0, dbCriteria, alias);
            preResults.Add(new PreResult() { Mapping = mapping0, ObjectType = objectType0, Statement = statement0 });
            while(preResults.Count > 0) {
                var statements = preResults.Select(p => p.Statement).ToArray();
                var selectedData = dataStore.SelectData(statements);
                var toLoad = new List<PreResult>();
                for(int i = 0; i < selectedData.ResultSet.Length; i++) {
                    var mapping = preResults[i].Mapping;
                    var result = selectedData.ResultSet[i];
                    var objects = PhaseTwo(mapping, preResults[i].ObjectType, result, toLoad);
                    if(objects0 == null) {
                        objects0 = objects;
                    }
                    postResults.Add(new PostResult() { Mapping = mapping, Objects = objects, Result = result });
                }
                preResults = toLoad;
            }
            foreach(var postResult in postResults) {
                PhaseThree(postResult.Mapping, postResult.Objects, postResult.Result);
            }
            return objects0;
        }
        public object LoadObjectByKey(Type objectType, object key) {
            var alias = "T";
            var objects = LoadObjectsCore(objectType, BuildByKeyCriteria(objectType, key, alias), alias);
            if(objects.Count == 1) {
                return objects[0];
            }
            if(objects.Count == 0) {
                return null;
            }
            throw new DataException();
        }
    }

    class DataStoreObjectSaver {
        private IDataStore dataStore;
        private Dictionary<Type, DataStoreMapping> mappings;
        public DataStoreObjectSaver(Dictionary<Type, DataStoreMapping> mappings, IDataStore dataStore) {
            this.mappings = mappings;
            this.dataStore = dataStore;
        }
        public void SaveObjects(ICollection toInsert, ICollection toUpdate, ICollection toDelete) {
            var statements = new List<ModificationStatement>();
            var identityAwaiters = new List<Action<object>>();
            foreach(var obj in toDelete) {
                DeleteObject(obj, statements);
            }
            foreach(var obj in toInsert) {
                InsertObject(obj, statements, identityAwaiters);
            }
            foreach(var obj in toUpdate) {
                UpdateObject(obj, statements);
            }
            var result = dataStore.ModifyData(statements.ToArray());
            foreach(var identity in result.Identities) {
                identityAwaiters[identity.Tag - 1].Invoke(identity.Value);
            }
        }
        private void DeleteObject(object obj, IList<ModificationStatement> statements) {
            if(mappings.TryGetValue(obj.GetType(), out var mapping)) {
                string alias = null;
                var statement = new DeleteStatement(mapping.Table, alias);
                SetupUpdateDeleteStatement(statement, obj, mapping, alias);
                statements.Add(statement);
            }
        }
        private void InsertObject(object obj, IList<ModificationStatement> statements, List<Action<object>> identityAwaiters) {
            if(mappings.TryGetValue(obj.GetType(), out var mapping)) {
                var statement = new InsertStatement(mapping.Table, "T");
                if(mapping.Table.PrimaryKey != null) {
                    foreach(var columnName in mapping.Table.PrimaryKey.Columns) {
                        var column = mapping.Table.GetColumn(columnName);
                        if(column.IsIdentity) {
                            identityAwaiters.Add(v => { mapping.SetKey(obj, v); });
                            statement.IdentityColumn = column.Name;
                            statement.IdentityColumnType = column.ColumnType;
                            statement.IdentityParameter = new ParameterValue(identityAwaiters.Count);
                            break;
                        }
                    }
                }
                SetupInsertUpdateStatement(statement, obj, mapping);
                statements.Add(statement);
            }
        }
        private void UpdateObject(object obj, IList<ModificationStatement> statements) {
            if(mappings.TryGetValue(obj.GetType(), out var mapping)) {
                string alias = null;
                var statement = new UpdateStatement(mapping.Table, alias);
                SetupUpdateDeleteStatement(statement, obj, mapping, alias);
                SetupInsertUpdateStatement(statement, obj, mapping);
                statements.Add(statement);
            }
        }
        private DBColumn GetKeyColumn(DataStoreMapping mapping) {
            return mapping.Table.Columns.First(c => c.IsKey);
        }
        private void SetupUpdateDeleteStatement(ModificationStatement statement, object obj, DataStoreMapping mapping, string alias) {
            statement.Condition = new BinaryOperator(
                new QueryOperand(GetKeyColumn(mapping), alias),
                new OperandValue(mapping.GetKey(obj)),
                BinaryOperatorType.Equal);
        }
        private void SetupInsertUpdateStatement(ModificationStatement statement, object obj, DataStoreMapping mapping) {
            var values = new object[mapping.Table.Columns.Count];
            mapping.Save(obj, values);
            for(int i = 0; i < values.Length; i++) {
                var column = mapping.Table.Columns[i];
                if(!column.IsIdentity) {
                    statement.Operands.Add(new QueryOperand(column, null));
                    statement.Parameters.Add(new OperandValue(values[i]));
                }
            }
        }
    }

    class SimpleDataStoreCriteriaVisitor : ClientCriteriaVisitorBase {
        private DBTable table;
        private string alias;
        public SimpleDataStoreCriteriaVisitor(DBTable table, string alias) {
            this.table = table;
            this.alias = alias;
        }
        protected override CriteriaOperator Visit(OperandProperty theOperand) {
            var column = table.GetColumn(theOperand.PropertyName);
            if(column != null) {
                return new QueryOperand(table.GetColumn(theOperand.PropertyName), alias);
            }
            else {
                return null;
            }
        }
        public static CriteriaOperator Transform(CriteriaOperator criteria, DBTable table, string alias) {
            return new SimpleDataStoreCriteriaVisitor(table, alias).Process(criteria);
        }
    }
}
