using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using System;
using System.Collections;
using System.Linq;


namespace NonPersistentObjectsDemo.Module {
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
}
