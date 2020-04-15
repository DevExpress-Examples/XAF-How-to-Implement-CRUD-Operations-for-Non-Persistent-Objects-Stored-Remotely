Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Data
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports DevExpress.Data.Filtering
Imports DevExpress.Persistent.Base

Namespace NonPersistentObjectsDemo.Module

	Public Class CurrentUserServiceProvider(Of T)
		Private Shared factory As Func(Of T)
		Public Shared Sub AddService(ByVal factory As Func(Of T))
			CurrentUserServiceProvider(Of T).factory = factory
		End Sub
		Public Shared Function GetService() As T
			Dim vm = ValueManager.GetValueManager(Of T)(GetType(CurrentUserServiceProvider(Of T)).FullName)
			Dim result As T
			If vm.CanManageValue Then
				result = vm.Value
				If Equals(result, CType(Nothing, T)) Then
					result = factory.Invoke()
					vm.Value = result
				End If
			Else
				result = factory.Invoke()
			End If
			Return result
		End Function
	End Class

	Public Class GlobalServiceProvider(Of T)
		Private Shared factory As Func(Of T)
		Private Shared instance As T
		Public Shared Sub AddService(ByVal factory As Func(Of T))
			GlobalServiceProvider(Of T).factory = factory
		End Sub
		Public Shared Function GetService() As T
			GetType(T).TypeInitializer.Invoke(Nothing, Nothing)
			If Equals(instance, CType(Nothing, T)) Then
				instance = factory.Invoke()
			End If
			Return instance
		End Function
	End Class
End Namespace
