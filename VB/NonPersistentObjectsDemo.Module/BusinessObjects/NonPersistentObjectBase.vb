Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.DC
Imports DevExpress.Persistent.Base

Namespace NonPersistentObjectsDemo.Module.BusinessObjects


	Public MustInherit Class NonPersistentObjectBase
		Implements INotifyPropertyChanged, IObjectSpaceLink

'INSTANT VB NOTE: The field objectSpace was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private objectSpace_Conflict As IObjectSpace
		Protected ReadOnly Property ObjectSpace() As IObjectSpace
			Get
				Return objectSpace_Conflict
			End Get
		End Property
		Private Property IObjectSpaceLink_ObjectSpace() As IObjectSpace Implements IObjectSpaceLink.ObjectSpace
			Get
				Return objectSpace_Conflict
			End Get
			Set(ByVal value As IObjectSpace)
				If objectSpace_Conflict IsNot value Then
					OnObjectSpaceChanging()
					objectSpace_Conflict = value
					OnObjectSpaceChanged()
				End If
			End Set
		End Property
		Protected Overridable Sub OnObjectSpaceChanging()
		End Sub
		Protected Overridable Sub OnObjectSpaceChanged()
		End Sub
		Protected Function FindPersistentObjectSpace(ByVal type As Type) As IObjectSpace
			Return DirectCast(objectSpace_Conflict, NonPersistentObjectSpace).AdditionalObjectSpaces.FirstOrDefault(Function(os) os.IsKnownType(type))
		End Function
		Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
		Protected Sub OnPropertyChanged(ByVal propertyName As String)
			PropertyChangedEvent?.Invoke(Me, New PropertyChangedEventArgs(propertyName))
		End Sub
		Protected Sub SetPropertyValue(Of T)(ByVal name As String, ByRef field As T, ByVal value As T)
			If Not Equals(field, value) Then
				field = value
				OnPropertyChanged(name)
			End If
		End Sub
		<Browsable(False)>
		Public ReadOnly Property This() As Object
			Get
				Return Me
			End Get
		End Property
	End Class
End Namespace
