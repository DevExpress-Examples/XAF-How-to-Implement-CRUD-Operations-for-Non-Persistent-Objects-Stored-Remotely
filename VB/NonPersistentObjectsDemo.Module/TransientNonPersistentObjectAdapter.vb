Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports DevExpress.Data.Filtering
Imports DevExpress.ExpressApp

Namespace NonPersistentObjectsDemo.Module

	Friend Class TransientNonPersistentObjectAdapter
		Private objectSpace As NonPersistentObjectSpace
		Private factory As NonPersistentObjectFactoryBase
		Private objectMap As ObjectMap
		Public Sub New(ByVal objectSpace As NonPersistentObjectSpace, ByVal objectMap As ObjectMap, ByVal factory As NonPersistentObjectFactoryBase)
			Me.objectSpace = objectSpace
			Me.factory = factory
			Me.objectMap = objectMap
			AddHandler objectSpace.ObjectsGetting, AddressOf ObjectSpace_ObjectsGetting
			AddHandler objectSpace.ObjectGetting, AddressOf ObjectSpace_ObjectGetting
			AddHandler objectSpace.ObjectByKeyGetting, AddressOf ObjectSpace_ObjectByKeyGetting
			AddHandler objectSpace.Reloaded, AddressOf ObjectSpace_Reloaded
			AddHandler objectSpace.CustomCommitChanges, AddressOf ObjectSpace_CustomCommitChanges
			AddHandler objectSpace.ObjectReloading, AddressOf ObjectSpace_ObjectReloading
		End Sub
		Private Sub ObjectSpace_ObjectReloading(ByVal sender As Object, ByVal e As ObjectGettingEventArgs)
			If e.SourceObject IsNot Nothing AndAlso objectMap.IsKnown(e.SourceObject.GetType()) Then
				If IsNewObject(e.SourceObject) Then
					e.TargetObject = Nothing
				Else
					Dim key = objectSpace.GetKeyValue(e.SourceObject)
					e.TargetObject = factory.GetObjectByKey(e.SourceObject.GetType(), key)
				End If
			End If
		End Sub
		Private Sub ObjectSpace_CustomCommitChanges(ByVal sender As Object, ByVal e As HandledEventArgs)
			Dim toSave = objectSpace.GetObjectsToSave(False)
			Dim toInsert = New List(Of Object)()
			Dim toUpdate = New List(Of Object)()
			For Each obj In toSave
				If objectSpace.IsNewObject(obj) Then
					toInsert.Add(obj)
				Else
					toUpdate.Add(obj)
				End If
			Next obj
			Dim toDelete = objectSpace.GetObjectsToDelete(False)
			If toInsert.Count <> 0 OrElse toUpdate.Count <> 0 OrElse toDelete.Count <> 0 Then
				factory.SaveObjects(toInsert, toUpdate, toDelete)
			End If
			'e.Handled = false;// !!!
		End Sub
		Private Sub ObjectSpace_Reloaded(ByVal sender As Object, ByVal e As EventArgs)
			objectMap.Clear()
		End Sub
		Private Sub ObjectSpace_ObjectByKeyGetting(ByVal sender As Object, ByVal e As ObjectByKeyGettingEventArgs)
			If e.Key IsNot Nothing AndAlso objectMap.IsKnown(e.ObjectType) Then
				Dim obj As Object = objectMap.Get(e.ObjectType, e.Key)
				If obj Is Nothing Then
					obj = factory.GetObjectByKey(e.ObjectType, e.Key)
					If obj IsNot Nothing AndAlso Not objectMap.Contains(obj) Then
						objectMap.Add(e.ObjectType, e.Key, obj)
					End If
				End If
				If obj IsNot Nothing Then
					e.Object = obj
				End If
			End If
		End Sub
		Private Sub ObjectSpace_ObjectGetting(ByVal sender As Object, ByVal e As ObjectGettingEventArgs)
			If e.SourceObject IsNot Nothing AndAlso objectMap.IsKnown(e.SourceObject.GetType()) Then
				If objectMap.Contains(e.SourceObject) OrElse IsNewObject(e.SourceObject) Then
					e.TargetObject = e.SourceObject
				Else
					Dim key = objectSpace.GetKeyValue(e.SourceObject)
					e.TargetObject = objectSpace.GetObjectByKey(e.SourceObject.GetType(), key)
				End If
			End If
		End Sub
		Private Sub ObjectSpace_ObjectsGetting(ByVal sender As Object, ByVal e As ObjectsGettingEventArgs)
			If objectMap.IsKnown(e.ObjectType) Then
				Dim collection = New DynamicCollection(objectSpace, e.ObjectType, e.Criteria, e.Sorting, e.InTransaction)
				AddHandler collection.ObjectsGetting, AddressOf DynamicCollection_ObjectsGetting
				e.Objects = collection
			End If
		End Sub
		Private Shared Function IsNewObject(ByVal obj As Object) As Boolean
			Dim sourceObjectSpace = BaseObjectSpace.FindObjectSpaceByObject(obj)
			Return If(sourceObjectSpace Is Nothing, False, sourceObjectSpace.IsNewObject(obj))
		End Function
		Private Function GetList(ByVal objectType As Type, ByVal criteria As CriteriaOperator, ByVal sorting As IList(Of DevExpress.Xpo.SortProperty)) As IEnumerable
			Return factory.GetObjects(objectType, criteria, sorting)
		End Function
		Private Sub DynamicCollection_ObjectsGetting(ByVal sender As Object, ByVal e As DynamicObjectsGettingEventArgs)
			e.Objects = GetList(e.ObjectType, e.Criteria, e.Sorting)
			e.ShapeRawData = True
		End Sub
	End Class

	Public Class ObjectMap
		Private typeMap As Dictionary(Of Type, Dictionary(Of Object, Object))
		Public Sub New(ParamArray ByVal types() As Type)
			Me.typeMap = New Dictionary(Of Type, Dictionary(Of Object, Object))()
			For Each type In types
				typeMap.Add(type, New Dictionary(Of Object, Object)())
			Next type
		End Sub
		Public Function IsKnown(ByVal type As Type) As Boolean
			Return typeMap.ContainsKey(type)
		End Function
		Public Function Contains(ByVal obj As Object) As Boolean
			Dim objectMap As Dictionary(Of Object, Object) = Nothing
			If typeMap.TryGetValue(obj.GetType(), objectMap) Then
				Return objectMap.ContainsValue(obj)
			End If
			Return False
		End Function
		Public Sub Clear()
			For Each kv In typeMap
				kv.Value.Clear()
			Next kv
		End Sub
		Public Function [Get](Of T)(ByVal key As Object) As T
			Return DirectCast([Get](GetType(T), key), T)
		End Function
		Public Function [Get](ByVal type As Type, ByVal key As Object) As Object
			Dim objectMap As Dictionary(Of Object, Object) = Nothing
			If typeMap.TryGetValue(type, objectMap) Then
				Dim obj As Object = Nothing
				If objectMap.TryGetValue(key, obj) Then
					Return obj
				End If
			End If
			Return Nothing
		End Function
		Public Sub Add(ByVal type As Type, ByVal key As Object, ByVal obj As Object)
			Dim objectMap As Dictionary(Of Object, Object) = Nothing
			If typeMap.TryGetValue(type, objectMap) Then
				objectMap.Add(key, obj)
			End If
		End Sub
	End Class
End Namespace
