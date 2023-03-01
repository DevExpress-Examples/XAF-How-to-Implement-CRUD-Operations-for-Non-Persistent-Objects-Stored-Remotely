using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using NonPersistentObjectsDemo.Module.ServiceClasses;

namespace NonPersistentObjectsDemo.Module.BusinessObjects {

    [DefaultClassOptions]
    [DefaultListViewOptions(true, NewItemRowPosition.Top)]
    [DefaultProperty("Subject")]
    [DevExpress.ExpressApp.DC.DomainComponent]
    public class Message : NonPersistentObjectBase {
        private int id;
        [Browsable(false)]
        [DevExpress.ExpressApp.Data.Key]
        public int ID {
            get { return id; }
        }
        public void SetKey(int id) {
            this.id = id;
        }
        private Account _Sender;
        public Account Sender {
            get { return _Sender; }
            set { SetPropertyValue(nameof(Sender), ref _Sender, value); }
        }
        private Account _Recepient;
        public Account Recipient {
            get { return _Recepient; }
            set { SetPropertyValue(nameof(Recipient), ref _Recepient, value); }
        }
        private string _Subject;
        public string Subject {
            get { return _Subject; }
            set { SetPropertyValue<string>(nameof(Subject), ref _Subject, value); }
        }
        private string _Body;
        [FieldSize(-1)]
        public string Body {
            get { return _Body; }
            set { SetPropertyValue<string>(nameof(Body), ref _Body, value); }
        }
    }
}
