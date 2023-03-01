using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace NonPersistentObjectsDemo.Module.BusinessObjects {
    [DefaultClassOptions]
    [DefaultListViewOptions(true, NewItemRowPosition.Top)]
    [DefaultProperty("PublicName")]
    [DevExpress.ExpressApp.DC.DomainComponent]
    public class Account : NonPersistentObjectBase {
        private string _myKey;
        //[Browsable(false)]
        [DevExpress.ExpressApp.ConditionalAppearance.Appearance("", Enabled = false, Criteria = "Not IsNewObject(This)")]
        [RuleRequiredField]
        [DevExpress.ExpressApp.Data.Key]
        public string MyKey {
            get { return _myKey; }
            set { _myKey = value; }
        }
        public void SetKey(string userName) {
            this._myKey = userName;
        }
        private string publicName;
        public string PublicName {
            get { return publicName; }
            set { SetPropertyValue(nameof(PublicName), ref publicName, value); }
        }
    }
}
