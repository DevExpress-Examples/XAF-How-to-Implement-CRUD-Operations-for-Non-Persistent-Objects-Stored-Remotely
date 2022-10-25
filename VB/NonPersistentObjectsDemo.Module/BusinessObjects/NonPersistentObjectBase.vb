Imports System
Imports System.ComponentModel
Imports System.Linq
Imports DevExpress.ExpressApp

Namespace NonPersistentObjectsDemo.Module.BusinessObjects

    Public MustInherit Class NonPersistentObjectBase
        Implements INotifyPropertyChanged, IObjectSpaceLink

        Private objectSpaceField As IObjectSpace

        Protected ReadOnly Property ObjectSpaceProp As IObjectSpace
            Get
                Return objectSpaceField
            End Get
        End Property

        Private Property ObjectSpace As IObjectSpace Implements IObjectSpaceLink.ObjectSpace
            Get
                Return objectSpaceField
            End Get

            Set(ByVal value As IObjectSpace)
                If objectSpaceField IsNot value Then
                    OnObjectSpaceChanging()
                    objectSpaceField = value
                    OnObjectSpaceChanged()
                End If
            End Set
        End Property

        Protected Overridable Sub OnObjectSpaceChanging()
        End Sub

        Protected Overridable Sub OnObjectSpaceChanged()
        End Sub

        Protected Function FindPersistentObjectSpace(ByVal type As Type) As IObjectSpace
            Return CType(objectSpaceField, NonPersistentObjectSpace).AdditionalObjectSpaces.FirstOrDefault(Function(os) os.IsKnownType(type))
        End Function

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Protected Sub OnPropertyChanged(ByVal propertyName As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub

        Protected Sub SetPropertyValue(Of T)(ByVal name As String, ByRef field As T, ByVal value As T)
            If Not Equals(field, value) Then
                field = value
                OnPropertyChanged(name)
            End If
        End Sub

        <Browsable(False)>
        Public ReadOnly Property This As Object
            Get
                Return Me
            End Get
        End Property
    End Class
End Namespace
