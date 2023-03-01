using DevExpress.Data.Filtering;
using NonPersistentObjectsDemo.Module.ServiceClasses;
using System;
using System.Collections;
using System.Linq;

namespace NonPersistentObjectsDemo.Module.BusinessObjects {
    public class PostOfficeFactory : NonPersistentObjectFactoryBase {
        private PostOfficeClient Storage => GlobalServiceProvider<PostOfficeClient>.GetService();
        private bool isLoading = false;
        private ObjectMap objectMap;
        public PostOfficeFactory(ObjectMap objectMap) {
            this.objectMap = objectMap;
        }
        public override object GetObjectByKey(Type objectType, object key) {
            if(key == null) {
                throw new ArgumentNullException(nameof(key));
            }
            if(Storage.Mappings.TryGetValue(objectType, out var mapping)) {
                return WrapLoading(() => {
                    var loader = new DataStoreObjectLoader(Storage.Mappings, Storage.DataStore, objectMap);
                    return loader.LoadObjectByKey(objectType, key);
                });
            }
            throw new NotImplementedException();
        }
        public override IEnumerable GetObjects(Type objectType, CriteriaOperator criteria, IList<DevExpress.Xpo.SortProperty> sorting) {
            if(Storage.Mappings.TryGetValue(objectType, out var mapping)) {
                return WrapLoading(() => {
                    var loader = new DataStoreObjectLoader(Storage.Mappings, Storage.DataStore, objectMap);
                    return loader.LoadObjects(objectType, criteria);
                });
            }
            throw new NotImplementedException();
        }
        private T WrapLoading<T>(Func<T> doer) {
            if(isLoading) {
                throw new InvalidOperationException();
            }
            isLoading = true;
            try {
                return doer.Invoke();
            }
            finally {
                isLoading = false;
            }
        }
        public override void SaveObjects(ICollection toInsert, ICollection toUpdate, ICollection toDelete) {
            var saver = new DataStoreObjectSaver(Storage.Mappings, Storage.DataStore);
            saver.SaveObjects(toInsert, toUpdate, toDelete);
        }
    }
}
