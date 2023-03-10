using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using DevExpress.XtraRichEdit.Export.WordML;
using NonPersistentObjectsDemo.Module.BusinessObjects;
using NonPersistentObjectsDemo.Module.Stubs;
using System;
using System.Collections;
using System.Linq;

namespace NonPersistentObjectsDemo.Module.ServiceClasses {
    public class PostOfficeFactory : NonPersistentObjectFactoryBase {
        public PostOfficeFactory() {
            Storage = new List<AccountStub>();
            CreateDemoData(Storage);
        }
        static void CreateDemoData(List<AccountStub> list) {
            var idsAccount = new List<string>();
            for(int i = 0; i < 10; i++) {
                var accStub = new AccountStub();
                accStub.MyKey = "key" + i;
                accStub.MyName = "Name" + i;
                list.Add(accStub);
            }

        }
        List<AccountStub> Storage;

        public override object GetObjectByKey(Type objectType, object key) {
            if(key == null) {
                throw new ArgumentNullException(nameof(key));
            }
            var stub = Storage.Where(x => x.MyKey == (string)key).FirstOrDefault();
            if(stub != null) {
                var acc = new Account();
                acc.MyKey = stub.MyKey;
                acc.PublicName = stub.MyName;
                return acc;
            }
            throw new NotImplementedException();
        }
        public override IEnumerable GetObjects(Type objectType, CriteriaOperator criteria, IList<DevExpress.Xpo.SortProperty> sorting) {
            var lst = new List<Account>();
            foreach(var stub in Storage) {
                var acc = new Account();
                acc.MyKey = stub.MyKey;
                acc.PublicName = stub.MyName;
                lst.Add(acc);
            }
            return lst;

        }

        public override void SaveObjects(ICollection toInsert, ICollection toUpdate, ICollection toDelete) {
            foreach(Account obj in toInsert) {
                var stub = new AccountStub();
                stub.MyKey = obj.MyKey;
                stub.MyName = obj.PublicName;
                Storage.Add(stub);
            }
            foreach(Account obj in toUpdate) {
                var stub = Storage.Where(x => x.MyKey == obj.MyKey).FirstOrDefault();
                if(stub != null) {
                    stub.MyName = obj.PublicName;
                }
            }
            foreach(Account obj in toInsert) {
                var stub = Storage.Where(x => x.MyKey == obj.MyKey).FirstOrDefault();
                if(stub != null) {
                    Storage.Remove(stub);
                }
            }
        }
    }
}
