using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Data.Filtering;

namespace NonPersistentObjectsDemo.Module {

    public abstract class NonPersistentStorageBase {
        public abstract object GetObjectByKey(Type objectType, object key);
        public abstract IEnumerable GetObjects(Type objectType, CriteriaOperator criteria, IList<DevExpress.Xpo.SortProperty> sorting);
        public virtual void SaveObjects(ICollection toInsert, ICollection toUpdate, ICollection toDelete) { }
    }
}
