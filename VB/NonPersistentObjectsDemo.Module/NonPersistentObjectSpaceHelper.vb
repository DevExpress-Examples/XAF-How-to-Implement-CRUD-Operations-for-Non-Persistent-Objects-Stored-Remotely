Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports DevExpress.ExpressApp
Imports DevExpress.Persistent.Base

Namespace NonPersistentObjectsDemo.Module

	Public Class NonPersistentObjectSpaceHelper
		Implements IDisposable

		Private application As XafApplication
		Private basePersistentTypes() As Type
		Public ReadOnly Property AdapterCreators() As List(Of Action(Of NonPersistentObjectSpace))

		Public Sub New(ByVal application As XafApplication, ParamArray ByVal basePersistentTypes() As Type)
			Me.application = application
			Me.basePersistentTypes = basePersistentTypes
			Me.AdapterCreators = New List(Of Action(Of NonPersistentObjectSpace))()
			AddHandler application.ObjectSpaceCreated, AddressOf Application_ObjectSpaceCreated
			NonPersistentObjectSpace.UseKeyComparisonToDetermineIdentity = True
			NonPersistentObjectSpace.AutoSetModifiedOnObjectChangeByDefault = True
		End Sub
		Public Sub Dispose() Implements IDisposable.Dispose
			RemoveHandler application.ObjectSpaceCreated, AddressOf Application_ObjectSpaceCreated
		End Sub
		Private Sub Application_ObjectSpaceCreated(ByVal sender As Object, ByVal e As ObjectSpaceCreatedEventArgs)
			If TypeOf e.ObjectSpace Is NonPersistentObjectSpace Then
				Dim npos As NonPersistentObjectSpace = CType(e.ObjectSpace, NonPersistentObjectSpace)
				If basePersistentTypes IsNot Nothing Then
					For Each type In basePersistentTypes
						Dim persistentObjectSpace As IObjectSpace = application.CreateObjectSpace(type)
						npos.AdditionalObjectSpaces.Add(persistentObjectSpace)
					Next type
				End If
				npos.AutoDisposeAdditionalObjectSpaces = True
				For Each adapterCreator In AdapterCreators
					adapterCreator.Invoke(npos)
				Next adapterCreator
			End If
		End Sub
	End Class
End Namespace
