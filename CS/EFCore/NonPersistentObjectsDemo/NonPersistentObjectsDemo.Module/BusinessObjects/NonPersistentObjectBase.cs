using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

namespace NonPersistentObjectsDemo.Module.BusinessObjects {


    public abstract class NonPersistentObjectBase : INotifyPropertyChanged, IObjectSpaceLink {
        private IObjectSpace objectSpace;
        protected IObjectSpace ObjectSpace { get { return objectSpace; } }
        IObjectSpace IObjectSpaceLink.ObjectSpace {
            get { return objectSpace; }
            set {
                if(objectSpace != value) {
                    OnObjectSpaceChanging();
                    objectSpace = value;
                    OnObjectSpaceChanged();
                }
            }
        }
        protected virtual void OnObjectSpaceChanging() { }
        protected virtual void OnObjectSpaceChanged() { }
        protected IObjectSpace FindPersistentObjectSpace(Type type) {
            return ((NonPersistentObjectSpace)objectSpace).AdditionalObjectSpaces.FirstOrDefault(os => os.IsKnownType(type));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected void SetPropertyValue<T>(string name, ref T field, T value) {
            if(!Equals(field, value)) {
                field = value;
                OnPropertyChanged(name);
            }
        }
        [Browsable(false)]
        public object This { get { return this; } }
    }
}
