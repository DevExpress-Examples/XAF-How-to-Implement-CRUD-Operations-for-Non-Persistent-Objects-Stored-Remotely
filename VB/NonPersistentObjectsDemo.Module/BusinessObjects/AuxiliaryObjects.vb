Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports DevExpress.Data.Filtering
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.DC
Imports DevExpress.Persistent.Base
Imports DevExpress.Persistent.Validation

Namespace NonPersistentObjectsDemo.Module.BusinessObjects

	<DefaultClassOptions>
	<DefaultListViewOptions(True, NewItemRowPosition.Top)>
	<DefaultProperty("Subject")>
	<DevExpress.ExpressApp.DC.DomainComponent>
	Public Class Message
		Inherits NonPersistentObjectBase

'INSTANT VB NOTE: The field id was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private id_Conflict As Integer
		<Browsable(False)>
		<DevExpress.ExpressApp.Data.Key>
		Public ReadOnly Property ID() As Integer
			Get
				Return id_Conflict
			End Get
		End Property
'INSTANT VB NOTE: The variable id was renamed since Visual Basic does not handle local variables named the same as class members well:
		Public Sub SetKey(ByVal id_Conflict As Integer)
			Me.id_Conflict = id_Conflict
		End Sub
		Private _Sender As Account
		Public Property Sender() As Account
			Get
				Return _Sender
			End Get
			Set(ByVal value As Account)
				SetPropertyValue(NameOf(Sender), _Sender, value)
			End Set
		End Property
		Private _Recepient As Account
		Public Property Recepient() As Account
			Get
				Return _Recepient
			End Get
			Set(ByVal value As Account)
				SetPropertyValue(NameOf(Recepient), _Recepient, value)
			End Set
		End Property
		Private _Subject As String
		Public Property Subject() As String
			Get
				Return _Subject
			End Get
			Set(ByVal value As String)
				SetPropertyValue(Of String)(NameOf(Subject), _Subject, value)
			End Set
		End Property
		Private _Body As String
		<FieldSize(-1)>
		Public Property Body() As String
			Get
				Return _Body
			End Get
			Set(ByVal value As String)
				SetPropertyValue(Of String)(NameOf(Body), _Body, value)
			End Set
		End Property
	End Class

	<DefaultClassOptions>
	<DefaultListViewOptions(True, NewItemRowPosition.Top)>
	<DefaultProperty("PublicName")>
	<DevExpress.ExpressApp.DC.DomainComponent>
	Public Class Account
		Inherits NonPersistentObjectBase

'INSTANT VB NOTE: The field userName was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private userName_Conflict As String
		'[Browsable(false)]
		<DevExpress.ExpressApp.ConditionalAppearance.Appearance("", Enabled := False, Criteria := "Not IsNewObject(This)")>
		<RuleRequiredField>
		<DevExpress.ExpressApp.Data.Key>
		Public Property UserName() As String
			Get
				Return userName_Conflict
			End Get
			Set(ByVal value As String)
				userName_Conflict = value
			End Set
		End Property
'INSTANT VB NOTE: The variable userName was renamed since Visual Basic does not handle local variables named the same as class members well:
		Public Sub SetKey(ByVal userName_Conflict As String)
			Me.userName_Conflict = userName_Conflict
		End Sub
'INSTANT VB NOTE: The field publicName was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private publicName_Conflict As String
		Public Property PublicName() As String
			Get
				Return publicName_Conflict
			End Get
			Set(ByVal value As String)
				SetPropertyValue(NameOf(PublicName), publicName_Conflict, value)
			End Set
		End Property
	End Class


	Public Class PostOfficeFactory
		Inherits NonPersistentObjectFactoryBase

		Private ReadOnly Property Storage() As PostOfficeClient
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
			Dim mapping As DataStoreMapping
			If Storage.Mappings.TryGetValue(objectType, mapping) Then
				Return WrapLoading(Function()
					Dim loader = New DataStoreObjectLoader(Storage.Mappings, Storage.DataStore, objectMap)
					Return loader.LoadObjectByKey(objectType, key)
				End Function)
			End If
			Throw New NotImplementedException()
		End Function
		Public Overrides Function GetObjects(ByVal objectType As Type, ByVal criteria As CriteriaOperator, ByVal sorting As IList(Of DevExpress.Xpo.SortProperty)) As IEnumerable
			Dim mapping As DataStoreMapping
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
