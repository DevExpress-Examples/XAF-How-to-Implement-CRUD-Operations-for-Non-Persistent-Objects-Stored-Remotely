using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo.DB;
using NonPersistentObjectsDemo.Module.BusinessObjects;

namespace NonPersistentObjectsDemo.Module.ServiceClasses {

    public class PostOfficeClient {
        //static PostOfficeClient() {
        //    GlobalServiceProvider<PostOfficeClient>.AddService(() => new PostOfficeClient());
        //}
        public Dictionary<Type, DataStoreMapping> Mappings { get; private set; }
        public IDataStore DataStore { get; private set; }
        public PostOfficeClient() {
            DataStore = new InMemoryDataStore(AutoCreateOption.DatabaseAndSchema, false);
            Mappings = new Dictionary<Type, DataStoreMapping>();
            var mAccount = new DataStoreMapping();
            mAccount.Table = new DBTable("Accounts");
            mAccount.Table.AddColumn(new DBColumn("UserName", true, null, 255, DBColumnType.String));
            mAccount.Table.AddColumn(new DBColumn("PublicName", false, null, 1024, DBColumnType.String));
            mAccount.Create = () => new Account();
            mAccount.Load = (obj, values, omap) => {
                ((Account)obj).SetKey((string)values[0]);
                ((Account)obj).PublicName = (string)values[1];
            };
            mAccount.Save = (obj, values) => {
                values[0] = ((Account)obj).UserName;
                values[1] = ((Account)obj).PublicName;
            };
            mAccount.GetKey = (obj) => ((Account)obj).UserName;
            mAccount.RefColumns = Enumerable.Empty<DataStoreMapping.Column>();
            Mappings.Add(typeof(Account), mAccount);
            var mMessage = new DataStoreMapping();
            mMessage.Table = new DBTable("Messages");
            var mMessageKey = new DBColumn("ID", true, null, 0, DBColumnType.Int32);
            mMessageKey.IsIdentity = true;
            mMessage.Table.AddColumn(mMessageKey);
            mMessage.Table.AddColumn(new DBColumn("Subject", false, null, 1024, DBColumnType.String));
            mMessage.Table.AddColumn(new DBColumn("Body", false, null, -1, DBColumnType.String));
            mMessage.Table.AddColumn(new DBColumn("Sender", false, null, 255, DBColumnType.String));
            mMessage.Table.AddColumn(new DBColumn("Recepient", false, null, 255, DBColumnType.String));
            mMessage.Table.PrimaryKey = new DBPrimaryKey(new object[] { mMessageKey });
            mMessage.Create = () => new Message();
            mMessage.SetKey = (obj, key) => {
                ((Message)obj).SetKey((int)key);
            };
            mMessage.GetKey = (obj) => ((Message)obj).ID;
            mMessage.Load = (obj, values, omap) => {
                var o = (Message)obj;
                o.SetKey((int)values[0]);
                o.Subject = (string)values[1];
                o.Body = (string)values[2];
                o.Sender = GetReference<Account>(omap, values[3]);
                o.Recipient = GetReference<Account>(omap, values[4]);
            };
            mMessage.Save = (obj, values) => {
                var o = (Message)obj;
                values[0] = o.ID;
                values[1] = o.Subject;
                values[2] = o.Body;
                values[3] = o.Sender?.UserName;
                values[4] = o.Recipient?.UserName;
            };
            mMessage.RefColumns = new DataStoreMapping.Column[] {
                new DataStoreMapping.Column(){ Index = 3, Type = typeof(Account) },
                new DataStoreMapping.Column(){ Index = 4, Type = typeof(Account) }
            };
            Mappings.Add(typeof(Message), mMessage);
            DataStore.UpdateSchema(false, mAccount.Table, mMessage.Table);
            CreateDemoData((InMemoryDataStore)DataStore);
        }
        private static T GetReference<T>(ObjectMap map, object key) {
            return key == null ? default : map.Get<T>(key);
        }

        #region Demo Data
        static void CreateDemoData(InMemoryDataStore inMemoryDataStore) {
            var ds = new System.Data.DataSet();
            using(var ms = new MemoryStream()) {
                using(var writer = System.Xml.XmlWriter.Create(ms)) {
                    inMemoryDataStore.WriteXml(writer);
                    writer.Flush();
                }
                ms.Flush();
                ms.Position = 0;
                ds.ReadXml(ms);
            }
            var gen = new GenHelper();
            var idsAccount = new List<string>();
            var dtAccounts = ds.Tables["Accounts"];
            for(int i = 0; i < 200; i++) {
                var id = gen.MakeTosh(20);
                idsAccount.Add(id);
                dtAccounts.Rows.Add(id, gen.GetFullName());
            }
            var dtMessages = ds.Tables["Messages"];
            for(int i = 0; i < 5000; i++) {
                var id1 = gen.Next(idsAccount.Count);
                var id2 = gen.Next(idsAccount.Count - 1);
                dtMessages.Rows.Add(null, GenHelper.ToTitle(gen.MakeBlah(gen.Next(7))), gen.MakeBlahBlahBlah(5 + gen.Next(100), 7),
                    idsAccount[id1], idsAccount[(id1 + id2 + 1) % idsAccount.Count]);
            }
            ds.AcceptChanges();
            using(var ms = new MemoryStream()) {
                ds.WriteXml(ms, System.Data.XmlWriteMode.WriteSchema);
                ms.Flush();
                ms.Position = 0;
                using(var reader = System.Xml.XmlReader.Create(ms)) {
                    inMemoryDataStore.ReadXml(reader);
                }
            }
        }
        #endregion
    }
}
