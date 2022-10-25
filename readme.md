<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/255628948/20.1.3%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T884361)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
<!-- default file list -->

# XAF - How to implement CRUD operations for Non-Persistent Objects stored remotely

## Scenario

This example demonstrates a possible implementation of editable non-persistent objects that represent data stored remotely and separately from the main XAF application database. These non-persistent objects can be created, deleted, and modified. Their changes are persisted in the external storage. The `FilterController` is enabled for these objects, so their list view and lookup list view can be filtered. The built-in `IsNewObject` function is used in the Appearance rule criterion. This rule disables the key property editor after an Account object is saved.


## Solution

The following [NonPersistentObjectSpace](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace) members are used in this example.

Non-persistent objects are kept in an object map. In the [ObjectsGetting](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.ObjectsGetting?v=20.1), [ObjectGetting](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.ObjectGetting), and [ObjectByKeyGetting](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.ObjectByKeyGetting) event handlers, non-persistent objects are looked up and added to the object map. In the [Reloaded](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.BaseObjectSpace.Reloaded) event handler, the object map is cleared. So, subsequent object queries trigger the creation of new non-persistent object instances. In the [ObjectReloading](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.ObjectReloading?v=20.1) event handler, the state of an existing object is reloaded from the storage. 

In the [CustomCommitChanges](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.BaseObjectSpace.CustomCommitChanges?v=20.1) event handler, all object changes are processed and passed to the storage in a single atomic operation.

The [NonPersistentObjectSpace\.AutoSetModifiedOnObjectChange](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.AutoSetModifiedOnObjectChange?v=20.1) property is set to `true` to automatically mark non-persistent objects as modified when the `INotifyPropertyChanged.PropertyChanged`event is raised.

We use a simplified implementation of [IDataStore](https://docs.devexpress.com/CoreLibraries/DevExpress.Xpo.DB.IDataStore) as storage for Non-Persistent Object data.


### Common Components

The following classes are used to provide a common functionality for all non-persistent objects used in the demo.

#### NonPersistentObjectBase

The abstract base class for all non-persistent objects used in the application. It provides a common implementation of the `INotifyPropertyChanged` and `IObjectSpaceLink` interfaces and some convenient protected methods.

#### NonPersistentObjectSpaceHelper

This is a helper class that subscribes to the `XafApplication.ObjectSpaceCreated` event and performs a common setup of `NonPersistentObjectSpace`. This usually includes creating and linking additional object spaces, and setting up object adapters. Adapters are registered in the `Application.SetupComplete` event handler in module code.

#### TransientNonPersistentObjectAdapter

The adapter for transient (short-living) Non-Persistent business objects. Such objects exist only while their object space is alive. A new adapter instance is created for each non-persistent object space. It subscribes to object space events to manage a subset of object types in a common manner. It uses a factory to handle specific object types and their storage. It also maintains an identity map (`ObjectMap`) for `NonPersistentObjectSpace`.

#### NonPersistentObjectFactoryBase

Descendants of this class know how to create object instances and transfer data between objects and the storage. It knows nothing about the adapter. It also uses the identity map to avoid creating duplicated objects.

#### DataStoreMapper

This is a set of classes that represents external storage. These classes can be used by a factory to store object data in XPO's IDataStore storage. The classes are:

* `DataStoreObjectLoader`
  
  Loads objects by their keys and collections of objects.

* `DataStoreObjectSaver`
  
  Saves data of modified objects.
  
* `DataStoreMapping`
  
  Describes how objects are mapped to tables and columns.

## Files to Review

* [AuxiliaryObjects.cs](./CS/NonPersistentObjectsDemo.Module/BusinessObjects/AuxiliaryObjects.cs) (VB: [AuxiliaryObjects.vb](./VB/NonPersistentObjectsDemo.Module/BusinessObjects/AuxiliaryObjects.vb))
* [AuxiliaryStorage.cs](./CS/NonPersistentObjectsDemo.Module/BusinessObjects/AuxiliaryStorage.cs) (VB: [AuxiliaryStorage.vb](./VB/NonPersistentObjectsDemo.Module/BusinessObjects/AuxiliaryStorage.vb))
* [NonPersistentObjectBase.cs](./CS/NonPersistentObjectsDemo.Module/BusinessObjects/NonPersistentObjectBase.cs) (VB: [NonPersistentObjectBase.vb](./VB/NonPersistentObjectsDemo.Module/BusinessObjects/NonPersistentObjectBase.vb))
* [Module.cs](./CS/NonPersistentObjectsDemo.Module/Module.cs) (VB: [Module.vb](./VB/NonPersistentObjectsDemo.Module/Module.vb))
* [NonPersistentObjectSpaceHelper.cs](./CS/NonPersistentObjectsDemo.Module/NonPersistentObjectSpaceHelper.cs) (VB: [NonPersistentObjectSpaceHelper.vb](./VB/NonPersistentObjectsDemo.Module/NonPersistentObjectSpaceHelper.vb))
* [TransientNonPersistentObjectAdapter.cs](./CS/NonPersistentObjectsDemo.Module/TransientNonPersistentObjectAdapter.cs) (VB: [TransientNonPersistentObjectAdapter.vb](./VB/NonPersistentObjectsDemo.Module/TransientNonPersistentObjectAdapter.vb))
