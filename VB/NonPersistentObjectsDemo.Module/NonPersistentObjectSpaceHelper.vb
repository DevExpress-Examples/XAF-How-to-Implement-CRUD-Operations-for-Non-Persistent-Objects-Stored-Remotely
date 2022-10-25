Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports DevExpress.ExpressApp

Namespace NonPersistentObjectsDemo.Module

    Public Class NonPersistentObjectSpaceHelper
        Implements IDisposable

        Private application As XafApplication

        Private basePersistentTypes As Type()

        Public ReadOnly Property AdapterCreators As List(Of Action(Of NonPersistentObjectSpace))

        Public Sub New(ByVal application As XafApplication, ParamArray basePersistentTypes As Type())
            Me.application = application
            Me.basePersistentTypes = basePersistentTypes
            AdapterCreators = New List(Of Action(Of NonPersistentObjectSpace))()
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
                        EnsureObjectSpaceForType(npos, type)
                    Next
                End If

                npos.AutoDisposeAdditionalObjectSpaces = True
                For Each adapterCreator In AdapterCreators
                    adapterCreator.Invoke(npos)
                Next
            End If
        End Sub

        Private Sub EnsureObjectSpaceForType(ByVal npos As NonPersistentObjectSpace, ByVal type As Type)
            If Not npos.IsKnownType(type) Then
                If Not npos.AdditionalObjectSpaces.Any(Function(os) os.IsKnownType(type)) Then
                    Dim persistentObjectSpace As IObjectSpace = application.CreateObjectSpace(type)
                    npos.AdditionalObjectSpaces.Add(persistentObjectSpace)
                End If
            End If
        End Sub
    End Class
End Namespace
