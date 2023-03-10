using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;

namespace NonPersistentObjectsDemo.Module {

    class TransientNonPersistentObjectAdapter {
        private NonPersistentObjectSpace objectSpace;
        private NonPersistentStorageBase storage;
        private ObjectMap objectMap;
        public TransientNonPersistentObjectAdapter(NonPersistentObjectSpace objectSpace, ObjectMap objectMap, NonPersistentStorageBase _storage) {
            this.objectSpace = objectSpace;
            this.storage = _storage;
            this.objectMap = objectMap;
            objectSpace.ObjectsGetting += ObjectSpace_ObjectsGetting;
            objectSpace.ObjectGetting += ObjectSpace_ObjectGetting;
            objectSpace.ObjectByKeyGetting += ObjectSpace_ObjectByKeyGetting;
            objectSpace.Reloaded += ObjectSpace_Reloaded;
            objectSpace.CustomCommitChanges += ObjectSpace_CustomCommitChanges;
            objectSpace.ObjectReloading += ObjectSpace_ObjectReloading;
        }
        private void ObjectSpace_ObjectReloading(object sender, ObjectGettingEventArgs e) {
            if(e.SourceObject != null && objectMap.IsKnown(e.SourceObject.GetType())) {
                if(IsNewObject(e.SourceObject)) {
                    e.TargetObject = null;
                }
                else {
                    var key = objectSpace.GetKeyValue(e.SourceObject);
                    e.TargetObject = storage.GetObjectByKey(e.SourceObject.GetType(), key);
                }
            }
        }
        private void ObjectSpace_CustomCommitChanges(object sender, HandledEventArgs e) {
            var toSave = objectSpace.GetObjectsToSave(false);
            var toInsert = new List<object>();
            var toUpdate = new List<object>();
            foreach(var obj in toSave) {
                if(objectSpace.IsNewObject(obj)) {
                    toInsert.Add(obj);
                }
                else {
                    toUpdate.Add(obj);
                }
            }
            var toDelete = objectSpace.GetObjectsToDelete(false);
            if(toInsert.Count != 0 || toUpdate.Count != 0 || toDelete.Count != 0) {
                storage.SaveObjects(toInsert, toUpdate, toDelete);
            }
            //e.Handled = false;// !!!
        }
        private void ObjectSpace_Reloaded(object sender, EventArgs e) {
            objectMap.Clear();
        }
        private void ObjectSpace_ObjectByKeyGetting(object sender, ObjectByKeyGettingEventArgs e) {
            if(e.Key != null && objectMap.IsKnown(e.ObjectType)) {
                Object obj = objectMap.Get(e.ObjectType, e.Key);
                if(obj == null) {
                    obj = storage.GetObjectByKey(e.ObjectType, e.Key);
                    if(obj!= null && !objectMap.Contains(obj)) {
                        objectMap.Add(e.ObjectType, e.Key, obj);
                    }
                }
                if(obj!= null) {
                    e.Object = obj;
                }
            }
        }
        private void ObjectSpace_ObjectGetting(object sender, ObjectGettingEventArgs e) {
            if(e.SourceObject != null && objectMap.IsKnown(e.SourceObject.GetType())) {
                var link = (IObjectSpaceLink)e.SourceObject;
                if(objectSpace.Equals(link.ObjectSpace) && (objectMap.Contains(e.SourceObject) || IsNewObject(e.SourceObject))) {
                    e.TargetObject = e.SourceObject;
                }
                else {
                    var key = objectSpace.GetKeyValue(e.SourceObject);
                    e.TargetObject = objectSpace.GetObjectByKey(e.SourceObject.GetType(), key);
                }
            }
        }
        private void ObjectSpace_ObjectsGetting(object sender, ObjectsGettingEventArgs e) {
            if(objectMap.IsKnown(e.ObjectType)) {
                var collection = new DynamicCollection(objectSpace, e.ObjectType, e.Criteria, e.Sorting, e.InTransaction);
                collection.FetchObjects += DynamicCollection_FetchObjects;
                e.Objects = collection;
            }
        }
        private static bool IsNewObject(object obj) {
            var sourceObjectSpace = BaseObjectSpace.FindObjectSpaceByObject(obj);
            return sourceObjectSpace == null ? false : sourceObjectSpace.IsNewObject(obj);
        }
        private IEnumerable GetList(Type objectType, CriteriaOperator criteria, IList<DevExpress.Xpo.SortProperty> sorting) {
            return storage.GetObjects(objectType, criteria, sorting);
        }
        private void DynamicCollection_FetchObjects(object sender, FetchObjectsEventArgs e) {
            e.Objects = GetList(e.ObjectType, e.Criteria, e.Sorting);
            e.ShapeData = true;
        }
    }

    public class ObjectMap {
        private Dictionary<Type, Dictionary<Object, Object>> typeMap;
        private NonPersistentObjectSpace objectSpace;
        public ObjectMap(NonPersistentObjectSpace objectSpace, params Type[] types) {
            this.objectSpace = objectSpace;
            this.typeMap = new Dictionary<Type, Dictionary<object, object>>();
            foreach(var type in types) {
                typeMap.Add(type, new Dictionary<object, object>());
            }
        }
        public bool IsKnown(Type type) {
            return typeMap.ContainsKey(type);
        }
        public bool Contains(Object obj) {
            Dictionary<Object, Object> objectMap;
            if(typeMap.TryGetValue(obj.GetType(), out objectMap)) {
                return objectMap.ContainsValue(obj);
            }
            return false;
        }
        public void Clear() {
            foreach(var kv in typeMap) {
                kv.Value.Clear();
            }
        }
        public T Get<T>(Object key) {
            return (T)Get(typeof(T), key);
        }
        public Object Get(Type type, Object key) {
            Dictionary<Object, Object> objectMap;
            if(typeMap.TryGetValue(type, out objectMap)) {
                Object obj;
                if(objectMap.TryGetValue(key, out obj)) {
                    return obj;
                }
            }
            return null;
        }
        public void Add(Type type, Object key, Object obj) {
            Dictionary<Object, Object> objectMap;
            if(typeMap.TryGetValue(type, out objectMap)) {
                objectMap.Add(key, obj);
            }
        }
        public void Accept(Object obj) {
            objectSpace.GetObject(obj);
        }
    }
}
