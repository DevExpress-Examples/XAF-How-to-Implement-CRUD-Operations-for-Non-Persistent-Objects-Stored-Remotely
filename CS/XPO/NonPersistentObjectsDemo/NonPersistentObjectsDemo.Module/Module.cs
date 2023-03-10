using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using NonPersistentObjectsDemo.Module.BusinessObjects;
using NonPersistentObjectsDemo.Module.ServiceClasses;

namespace NonPersistentObjectsDemo.Module;

// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ModuleBase.
public sealed class NonPersistentObjectsDemoModule : ModuleBase {
    public NonPersistentObjectsDemoModule() {
		// 
		// NonPersistentObjectsDemoModule
		// 
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule));
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule));
    }
    public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
        ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
        return new ModuleUpdater[] { updater };
    }
    public override void Setup(XafApplication application) {
        base.Setup(application);
        factory = new PostOfficeFactory();
        NonPersistentObjectSpace.UseKeyComparisonToDetermineIdentity = true;
        NonPersistentObjectSpace.AutoSetModifiedOnObjectChangeByDefault = true;
        application.ObjectSpaceCreated += Application_ObjectSpaceCreated;
        // Manage various aspects of the application UI and behavior at the module level.
    }
    private void Application_ObjectSpaceCreated(object sender, ObjectSpaceCreatedEventArgs e) {
        if(e.ObjectSpace is NonPersistentObjectSpace) {
            NonPersistentObjectSpace npos = (NonPersistentObjectSpace)e.ObjectSpace;

            npos.AutoDisposeAdditionalObjectSpaces = true;
            var types = new Type[] { typeof(Account) };
            var map = new ObjectMap(npos, types);
            new TransientNonPersistentObjectAdapter(npos, map, factory);
        }
    }

    PostOfficeFactory factory;
    public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
        base.CustomizeTypesInfo(typesInfo);
        CalculatedPersistentAliasHelper.CustomizeTypesInfo(typesInfo);
    }
}
