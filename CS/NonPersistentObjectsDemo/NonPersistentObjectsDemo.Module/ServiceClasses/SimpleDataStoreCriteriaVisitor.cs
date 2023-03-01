using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Xpo.DB;
using System;
using System.Linq;


namespace NonPersistentObjectsDemo.Module {
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
