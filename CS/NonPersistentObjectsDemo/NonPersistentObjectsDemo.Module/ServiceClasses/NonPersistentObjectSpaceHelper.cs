using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;

namespace NonPersistentObjectsDemo.Module {

    public class NonPersistentObjectSpaceHelper : IDisposable {
        private XafApplication application;
        private Type[] basePersistentTypes;
        public List<Action<NonPersistentObjectSpace>> AdapterCreators { get; }

        public NonPersistentObjectSpaceHelper(XafApplication application, params Type[] basePersistentTypes) {
            this.application = application;
            this.basePersistentTypes = basePersistentTypes;
            this.AdapterCreators = new List<Action<NonPersistentObjectSpace>>();
            application.ObjectSpaceCreated += Application_ObjectSpaceCreated;
            NonPersistentObjectSpace.UseKeyComparisonToDetermineIdentity = true;
            NonPersistentObjectSpace.AutoSetModifiedOnObjectChangeByDefault = true;
        }
        public void Dispose() {
            application.ObjectSpaceCreated -= Application_ObjectSpaceCreated;
        }
        private void Application_ObjectSpaceCreated(Object sender, ObjectSpaceCreatedEventArgs e) {
            if(e.ObjectSpace is NonPersistentObjectSpace) {
                NonPersistentObjectSpace npos = (NonPersistentObjectSpace)e.ObjectSpace;
                if(basePersistentTypes != null) {
                    foreach(var type in basePersistentTypes) {
                        EnsureObjectSpaceForType(npos, type);
                    }
                }
                npos.AutoDisposeAdditionalObjectSpaces = true;
                foreach(var adapterCreator in AdapterCreators) {
                    adapterCreator.Invoke(npos);
                }
            }
        }
        private void EnsureObjectSpaceForType(NonPersistentObjectSpace npos, Type type) {
            if(!npos.IsKnownType(type)) {
                if(!npos.AdditionalObjectSpaces.Any(os => os.IsKnownType(type))) {
                    IObjectSpace persistentObjectSpace = application.CreateObjectSpace(type);
                    npos.AdditionalObjectSpaces.Add(persistentObjectSpace);
                }
            }
        }
    }
}
