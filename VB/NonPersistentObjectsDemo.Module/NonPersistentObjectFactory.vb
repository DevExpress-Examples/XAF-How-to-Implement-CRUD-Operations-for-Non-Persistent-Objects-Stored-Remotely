Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports DevExpress.Data.Filtering

Namespace NonPersistentObjectsDemo.Module

    Public MustInherit Class NonPersistentObjectFactoryBase

        Public MustOverride Function GetObjectByKey(ByVal objectType As Type, ByVal key As Object) As Object

        Public MustOverride Function GetObjects(ByVal objectType As Type, ByVal criteria As CriteriaOperator, ByVal sorting As IList(Of DevExpress.Xpo.SortProperty)) As IEnumerable

        Public Overridable Sub SaveObjects(ByVal toInsert As ICollection, ByVal toUpdate As ICollection, ByVal toDelete As ICollection)
        End Sub
    End Class
End Namespace
