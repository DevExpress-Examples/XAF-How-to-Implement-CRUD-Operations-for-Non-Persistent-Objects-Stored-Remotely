Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel
Imports DevExpress.Data.Filtering
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.DC
Imports DevExpress.Persistent.Base
Imports DevExpress.Persistent.Validation

Namespace NonPersistentObjectsDemo.Module.BusinessObjects

    <DefaultClassOptions>
    <DefaultListViewOptions(True, NewItemRowPosition.Top)>
    <DefaultProperty("Subject")>
    <DomainComponent>
    Public Class Message
        Inherits NonPersistentObjectBase

        Private idField As Integer

        <Browsable(False)>
        <Data.Key>
        Public ReadOnly Property ID As Integer
            Get
                Return idField
            End Get
        End Property

        Public Sub SetKey(ByVal id As Integer)
            idField = id
        End Sub

        Private _Sender As Account

        Public Property Sender As Account
            Get
                Return _Sender
            End Get

            Set(ByVal value As Account)
                SetPropertyValue(NameOf(Message.Sender), _Sender, value)
            End Set
        End Property

        Private _Recepient As Account

        Public Property Recepient As Account
            Get
                Return _Recepient
            End Get

            Set(ByVal value As Account)
                SetPropertyValue(NameOf(Message.Recepient), _Recepient, value)
            End Set
        End Property

        Private _Subject As String

        Public Property Subject As String
            Get
                Return _Subject
            End Get

            Set(ByVal value As String)
                SetPropertyValue(NameOf(Message.Subject), _Subject, value)
            End Set
        End Property

        Private _Body As String

        <FieldSize(-1)>
        Public Property Body As String
            Get
                Return _Body
            End Get

            Set(ByVal value As String)
                SetPropertyValue(NameOf(Message.Body), _Body, value)
            End Set
        End Property
    End Class

    <DefaultClassOptions>
    <DefaultListViewOptions(True, NewItemRowPosition.Top)>
    <DefaultProperty("PublicName")>
    <DomainComponent>
    Public Class Account
        Inherits NonPersistentObjectBase

        Private userNameField As String

        '[Browsable(false)]
        <ConditionalAppearance.Appearance("", Enabled:=False, Criteria:="Not IsNewObject(This)")>
        <RuleRequiredField>
        <Data.Key>
        Public Property UserName As String
            Get
                Return userNameField
            End Get

            Set(ByVal value As String)
                userNameField = value
            End Set
        End Property

        Public Sub SetKey(ByVal userName As String)
            userNameField = userName
        End Sub

        Private publicNameField As String

        Public Property PublicName As String
            Get
                Return publicNameField
            End Get

            Set(ByVal value As String)
                SetPropertyValue(NameOf(Account.PublicName), publicNameField, value)
            End Set
        End Property
    End Class

    Public Class PostOfficeFactory
        Inherits NonPersistentObjectFactoryBase

        Private ReadOnly Property Storage As PostOfficeClient
            Get
                Return GlobalServiceProvider(Of PostOfficeClient).GetService()
            End Get
        End Property

        Private isLoading As Boolean = False

        Private objectMap As ObjectMap

        Public Sub New(ByVal objectMap As ObjectMap)
            Me.objectMap = objectMap
        End Sub

        Public Overrides Function GetObjectByKey(ByVal objectType As Type, ByVal key As Object) As Object
            If key Is Nothing Then
                Throw New ArgumentNullException(NameOf(key))
            End If

            Dim mapping As DataStoreMapping = Nothing
            If Storage.Mappings.TryGetValue(objectType, mapping) Then
                Return WrapLoading(Function()
                    Dim loader = New DataStoreObjectLoader(Storage.Mappings, Storage.DataStore, objectMap)
                    Return loader.LoadObjectByKey(objectType, key)
                End Function)
            End If

            Throw New NotImplementedException()
        End Function

        Public Overrides Function GetObjects(ByVal objectType As Type, ByVal criteria As CriteriaOperator, ByVal sorting As IList(Of DevExpress.Xpo.SortProperty)) As IEnumerable
            Dim mapping As DataStoreMapping = Nothing
            If Storage.Mappings.TryGetValue(objectType, mapping) Then
                Return WrapLoading(Function()
                    Dim loader = New DataStoreObjectLoader(Storage.Mappings, Storage.DataStore, objectMap)
                    Return loader.LoadObjects(objectType, criteria)
                End Function)
            End If

            Throw New NotImplementedException()
        End Function

        Private Function WrapLoading(Of T)(ByVal doer As Func(Of T)) As T
            If isLoading Then
                Throw New InvalidOperationException()
            End If

            isLoading = True
            Try
                Return doer.Invoke()
            Finally
                isLoading = False
            End Try
        End Function

        Public Overrides Sub SaveObjects(ByVal toInsert As ICollection, ByVal toUpdate As ICollection, ByVal toDelete As ICollection)
            Dim saver = New DataStoreObjectSaver(Storage.Mappings, Storage.DataStore)
            saver.SaveObjects(toInsert, toUpdate, toDelete)
        End Sub
    End Class
End Namespace
