<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/255628948/22.2.4%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T884361)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
<!-- default file list -->

# XAF - How to Implement CRUD Operations for Non-Persistent Objects Stored Remotely

This example demonstrates an implementation of editable non-persistent objects that represent data stored remotely and separately from the main XAF application database. You can create, modify, and delete these non-persistent objects. The changes are persisted in the external storage. The built-in [`IsNewObject`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.IsNewObject(System.Object)) function is used in the Appearance rule criterion that disables the key property editor after a non-persistent object is saved.

## Implementation Details

This example uses [NonPersistentObjectSpace](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace) members.

Non-persistent objects are kept in an object map. XAF uses event handlers to manage these objects.

1. To look up non-persistent objects and add them to the object map:
    * [ObjectsGetting](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.ObjectsGetting)
    * [ObjectGetting](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.ObjectGetting)
    * [ObjectByKeyGetting](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.ObjectByKeyGetting)

2. To clear the object map:
   * [Reloaded](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.BaseObjectSpace.Reloaded)

     Subsequent object queries trigger the creation of new non-persistent object instances.

3. To reload the state of an existing object from storage:
   * [ObjectReloading](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.ObjectReloading)

4. To process all object changes and pass them to storage:
   * [CustomCommitChanges](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.BaseObjectSpace.CustomCommitChanges?v=20.1)

The [NonPersistentObjectSpace.AutoSetModifiedOnObjectChange](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.AutoSetModifiedOnObjectChange) property is set to `true` to automatically mark non-persistent objects as modified when XAF raises the `INotifyPropertyChanged.PropertyChanged` event.

`List<T>` objects store non-persistent data.

### Common Components

The following classes are used to provide a common functionality for all non-persistent objects used in the demo.

- `NonPersistentObjectBase`  
  The abstract base class for all non-persistent objects used in the application. It implements `INotifyPropertyChanged` and `IObjectSpaceLink` interfaces, and a few helpful protected methods.
  
- `TransientNonPersistentObjectAdapter`  
  The adapter for transient (short-living) non-persistent business objects. Such objects exist only during the lifespan of their object space. A new adapter instance is created for each non-persistent object space. It subscribes to object space events to manage a subset of object types in a common manner. It also maintains an identity map (`ObjectMap`) for `NonPersistentObjectSpace`.
  
- `NonPersistentStorageBase`  
  Descendants of this class know how to create object instances and transfer data between objects and the storage. They know nothing about the adapter. They also use the identity map to avoid creating duplicated objects.

## Files to Review

* [Account.cs](./CS/EFCore/NonPersistentObjectsDemo/NonPersistentObjectsDemo.Module/BusinessObjects/Account.cs)
* [PostOfficeStorage.cs](./CS/EFCore/NonPersistentObjectsDemo/NonPersistentObjectsDemo.Module/ServiceClasses/PostOfficeStorage.cs)
* [TransientNonPersistentObjectAdapter.cs](./CS/EFCore/NonPersistentObjectsDemo/NonPersistentObjectsDemo.Module/ServiceClasses/TransientNonPersistentObjectAdapter.cs)

## Documentation

* [How to: Perform CRUD Operations with Non-Persistent Objects](https://docs.devexpress.com/eXpressAppFramework/115672/business-model-design-orm/non-persistent-objects/how-to-perform-crud-operations-with-non-persistent-objects)
* [Non-Persistent Objects](https://docs.devexpress.com/eXpressAppFramework/116516/business-model-design-orm/non-persistent-objects)

## More Examples

- [How to implement CRUD operations for Non-Persistent Objects stored remotely in eXpressApp Framework](https://github.com/DevExpress-Examples/XAF_Non-Persistent-Objects-Editing-Demo)
- [How to edit Non-Persistent Objects nested in a Persistent Object](https://github.com/DevExpress-Examples/XAF_Non-Persistent-Objects-Nested-In-Persistent-Objects-Demo)
- [How to: Display a List of Non-Persistent Objects](https://github.com/DevExpress-Examples/XAF_how-to-display-a-list-of-non-persistent-objects-e980)
- [How to filter and sort Non-Persistent Objects](https://github.com/DevExpress-Examples/XAF_Non-Persistent-Objects-Filtering-Demo)
- [How to refresh Non-Persistent Objects and reload nested Persistent Objects](https://github.com/DevExpress-Examples/XAF_Non-Persistent-Objects-Reloading-Demo)
- [How to edit a collection of Persistent Objects linked to a Non-Persistent Object](https://github.com/DevExpress-Examples/XAF_Non-Persistent-Objects-Edit-Linked-Persistent-Objects-Demo)
