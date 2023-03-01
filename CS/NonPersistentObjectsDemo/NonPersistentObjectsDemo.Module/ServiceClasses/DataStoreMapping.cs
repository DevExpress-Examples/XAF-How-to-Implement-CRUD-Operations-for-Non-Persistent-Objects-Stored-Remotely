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
}
