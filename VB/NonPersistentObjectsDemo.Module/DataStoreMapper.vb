Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Data
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports DevExpress.Data.Filtering
Imports DevExpress.Data.Filtering.Helpers
Imports DevExpress.Xpo.DB

Namespace NonPersistentObjectsDemo.[Module]

    Public Class DataStoreMapping

        Public Table As DevExpress.Xpo.DB.DBTable

        Public Create As System.Func(Of Object)

        Public Load As System.Action(Of Object, Object(), NonPersistentObjectsDemo.[Module].ObjectMap)

        Public Save As System.Action(Of Object, Object())

        Public SetKey As System.Action(Of Object, Object)

        Public GetKey As System.Func(Of Object, Object)

        Public RefColumns As System.Collections.Generic.IEnumerable(Of NonPersistentObjectsDemo.[Module].DataStoreMapping.Column)

        Public Structure Column

            Public Index As Integer

            Public Type As System.Type
        End Structure
    End Class

    Friend Class DataStoreObjectLoader

        Friend Structure PreResult

            Public Mapping As NonPersistentObjectsDemo.[Module].DataStoreMapping

            Public ObjectType As System.Type

            Public Statement As DevExpress.Xpo.DB.SelectStatement
        End Structure

        Friend Structure PostResult

            Public Mapping As NonPersistentObjectsDemo.[Module].DataStoreMapping

            Public Objects As System.Collections.Generic.List(Of Object)

            Public Result As DevExpress.Xpo.DB.SelectStatementResult
        End Structure

        Private objectMap As NonPersistentObjectsDemo.[Module].ObjectMap

        Private dataStore As DevExpress.Xpo.DB.IDataStore

        Private mappings As System.Collections.Generic.Dictionary(Of System.Type, NonPersistentObjectsDemo.[Module].DataStoreMapping)

        Public Sub New(ByVal mappings As System.Collections.Generic.Dictionary(Of System.Type, NonPersistentObjectsDemo.[Module].DataStoreMapping), ByVal dataStore As DevExpress.Xpo.DB.IDataStore, ByVal objectMap As NonPersistentObjectsDemo.[Module].ObjectMap)
            Me.mappings = mappings
            Me.dataStore = dataStore
            Me.objectMap = objectMap
        End Sub

        Private Function GetKeyColumn(ByVal mapping As NonPersistentObjectsDemo.[Module].DataStoreMapping) As DBColumn
            Return mapping.Table.Columns.First(Function(c) c.IsKey)
        End Function

        Private Function BuildByKeyCriteria(ByVal objectType As System.Type, ByVal key As Object, ByVal [alias] As String) As CriteriaOperator
            Return New DevExpress.Data.Filtering.BinaryOperator(New DevExpress.Xpo.DB.QueryOperand(Me.GetKeyColumn(Me.mappings(objectType)), [alias]), New DevExpress.Data.Filtering.OperandValue(key), DevExpress.Data.Filtering.BinaryOperatorType.Equal)
        End Function

        Private Function PhaseOne(ByVal mapping As NonPersistentObjectsDemo.[Module].DataStoreMapping, ByVal objectType As System.Type, ByVal dbCriteria As DevExpress.Data.Filtering.CriteriaOperator, ByVal [alias] As String) As SelectStatement
            Dim statement = New DevExpress.Xpo.DB.SelectStatement(mapping.Table, [alias])
            statement.Condition = dbCriteria
            For Each column In mapping.Table.Columns
                statement.Operands.Add(New DevExpress.Xpo.DB.QueryOperand(column, [alias]))
            Next

            Return statement
        End Function

        Private Function PhaseTwo(ByVal mapping As NonPersistentObjectsDemo.[Module].DataStoreMapping, ByVal objectType As System.Type, ByVal result As DevExpress.Xpo.DB.SelectStatementResult, ByVal toLoad As System.Collections.Generic.List(Of NonPersistentObjectsDemo.[Module].DataStoreObjectLoader.PreResult)) As List(Of Object)
            Dim objects As System.Collections.Generic.List(Of Object) = New System.Collections.Generic.List(Of Object)()
            Dim keyColumnIndex As Integer = mapping.Table.Columns.IndexOf(Me.GetKeyColumn(mapping))
            Dim refColumns As System.Collections.Generic.List(Of NonPersistentObjectsDemo.[Module].DataStoreMapping.Column) = mapping.RefColumns.ToList()
            For Each row In result.Rows
                Dim key = row.Values(keyColumnIndex)
                If key Is Nothing Then Throw New System.Data.DataException("Key cannot be null.")
                Dim obj = Me.objectMap.[Get](objectType, key)
                If obj Is Nothing Then
                    obj = mapping.Create()
                    Me.objectMap.Add(objectType, key, obj)
                End If

                objects.Add(obj)
                For Each member In refColumns
                    Dim [alias] = "T"
                    Dim dbCriteria = Me.BuildByKeyCriteria(member.Type, row.Values(member.Index), [alias])
                    Dim memberMapping = Me.mappings(member.Type)
                    toLoad.Add(New NonPersistentObjectsDemo.[Module].DataStoreObjectLoader.PreResult() With {.Mapping = memberMapping, .ObjectType = member.Type, .Statement = Me.PhaseOne(memberMapping, member.Type, dbCriteria, [alias])})
                Next
            Next

            Return objects
        End Function

        Private Sub PhaseThree(ByVal mapping As NonPersistentObjectsDemo.[Module].DataStoreMapping, ByVal objects As System.Collections.Generic.List(Of Object), ByVal result As DevExpress.Xpo.DB.SelectStatementResult)
            For i As Integer = 0 To objects.Count - 1
                mapping.Load(objects(i), result.Rows(CInt((i))).Values, Me.objectMap)
                Me.objectMap.Accept(objects(i))
            Next
        End Sub

        Public Function LoadObjects(ByVal objectType As System.Type, ByVal criteria As DevExpress.Data.Filtering.CriteriaOperator) As IList(Of Object)
            Dim mapping = Me.mappings(objectType)
            Dim [alias] = "T"
            Dim dbCriteria = NonPersistentObjectsDemo.[Module].SimpleDataStoreCriteriaVisitor.Transform(criteria, mapping.Table, [alias])
            Return Me.LoadObjectsCore(objectType, dbCriteria, [alias])
        End Function

        Private Function LoadObjectsCore(ByVal objectType0 As System.Type, ByVal dbCriteria As DevExpress.Data.Filtering.CriteriaOperator, ByVal [alias] As String) As IList(Of Object)
            Dim objects0 As System.Collections.Generic.List(Of Object) = Nothing
            Dim preResults = New System.Collections.Generic.List(Of NonPersistentObjectsDemo.[Module].DataStoreObjectLoader.PreResult)()
            Dim postResults = New System.Collections.Generic.List(Of NonPersistentObjectsDemo.[Module].DataStoreObjectLoader.PostResult)()
            Dim mapping0 = Me.mappings(objectType0)
            Dim statement0 = Me.PhaseOne(mapping0, objectType0, dbCriteria, [alias])
            preResults.Add(New NonPersistentObjectsDemo.[Module].DataStoreObjectLoader.PreResult() With {.Mapping = mapping0, .ObjectType = objectType0, .Statement = statement0})
            While preResults.Count > 0
                Dim statements = preResults.[Select](Function(p) p.Statement).ToArray()
                Dim selectedData = Me.dataStore.SelectData(statements)
                Dim toLoad = New System.Collections.Generic.List(Of NonPersistentObjectsDemo.[Module].DataStoreObjectLoader.PreResult)()
                For i As Integer = 0 To selectedData.ResultSet.Length - 1
                    Dim mapping = preResults(CInt((i))).Mapping
                    Dim result = selectedData.ResultSet(i)
                    Dim objects = Me.PhaseTwo(mapping, preResults(CInt((i))).ObjectType, result, toLoad)
                    If objects0 Is Nothing Then
                        objects0 = objects
                    End If

                    postResults.Add(New NonPersistentObjectsDemo.[Module].DataStoreObjectLoader.PostResult() With {.Mapping = mapping, .Objects = objects, .Result = result})
                Next

                preResults = toLoad
            End While

            For Each postResult In postResults
                Me.PhaseThree(postResult.Mapping, postResult.Objects, postResult.Result)
            Next

            Return objects0
        End Function

        Public Function LoadObjectByKey(ByVal objectType As System.Type, ByVal key As Object) As Object
            Dim [alias] = "T"
            Dim objects = Me.LoadObjectsCore(objectType, Me.BuildByKeyCriteria(objectType, key, [alias]), [alias])
            If objects.Count = 1 Then
                Return objects(0)
            End If

            If objects.Count = 0 Then
                Return Nothing
            End If

            Throw New System.Data.DataException()
        End Function
    End Class

    Friend Class DataStoreObjectSaver

        Private dataStore As DevExpress.Xpo.DB.IDataStore

        Private mappings As System.Collections.Generic.Dictionary(Of System.Type, NonPersistentObjectsDemo.[Module].DataStoreMapping)

        Public Sub New(ByVal mappings As System.Collections.Generic.Dictionary(Of System.Type, NonPersistentObjectsDemo.[Module].DataStoreMapping), ByVal dataStore As DevExpress.Xpo.DB.IDataStore)
            Me.mappings = mappings
            Me.dataStore = dataStore
        End Sub

        Public Sub SaveObjects(ByVal toInsert As System.Collections.ICollection, ByVal toUpdate As System.Collections.ICollection, ByVal toDelete As System.Collections.ICollection)
            Dim statements = New System.Collections.Generic.List(Of DevExpress.Xpo.DB.ModificationStatement)()
            Dim identityAwaiters = New System.Collections.Generic.List(Of System.Action(Of Object))()
            For Each obj In toDelete
                Me.DeleteObject(obj, statements)
            Next

            For Each obj In toInsert
                Me.InsertObject(obj, statements, identityAwaiters)
            Next

            For Each obj In toUpdate
                Me.UpdateObject(obj, statements)
            Next

            Dim result = Me.dataStore.ModifyData(statements.ToArray())
            For Each identity In result.Identities
                identityAwaiters(CInt((identity.Tag - 1))).Invoke(identity.Value)
            Next
        End Sub

        Private Sub DeleteObject(ByVal obj As Object, ByVal statements As System.Collections.Generic.IList(Of DevExpress.Xpo.DB.ModificationStatement))
            Dim mapping As Global.NonPersistentObjectsDemo.[Module].DataStoreMapping = Nothing
            If Me.mappings.TryGetValue(obj.[GetType](), mapping) Then
                Dim [alias] As String = Nothing
                Dim statement = New DevExpress.Xpo.DB.DeleteStatement(mapping.Table, [alias])
                Me.SetupUpdateDeleteStatement(statement, obj, mapping, [alias])
                statements.Add(statement)
            End If
        End Sub

        Private Sub InsertObject(ByVal obj As Object, ByVal statements As System.Collections.Generic.IList(Of DevExpress.Xpo.DB.ModificationStatement), ByVal identityAwaiters As System.Collections.Generic.List(Of System.Action(Of Object)))
            Dim mapping As Global.NonPersistentObjectsDemo.[Module].DataStoreMapping = Nothing
            If Me.mappings.TryGetValue(obj.[GetType](), mapping) Then
                Dim statement = New DevExpress.Xpo.DB.InsertStatement(mapping.Table, "T")
                If mapping.Table.PrimaryKey IsNot Nothing Then
                    For Each columnName In mapping.Table.PrimaryKey.Columns
                        Dim column = mapping.Table.GetColumn(columnName)
                        If column.IsIdentity Then
                            identityAwaiters.Add(Sub(v) mapping.SetKey(obj, v))
                            statement.IdentityColumn = column.Name
                            statement.IdentityColumnType = column.ColumnType
                            statement.IdentityParameter = New DevExpress.Xpo.DB.ParameterValue(identityAwaiters.Count)
                            Exit For
                        End If
                    Next
                End If

                Me.SetupInsertUpdateStatement(statement, obj, mapping)
                statements.Add(statement)
            End If
        End Sub

        Private Sub UpdateObject(ByVal obj As Object, ByVal statements As System.Collections.Generic.IList(Of DevExpress.Xpo.DB.ModificationStatement))
            Dim mapping As Global.NonPersistentObjectsDemo.[Module].DataStoreMapping = Nothing
            If Me.mappings.TryGetValue(obj.[GetType](), mapping) Then
                Dim [alias] As String = Nothing
                Dim statement = New DevExpress.Xpo.DB.UpdateStatement(mapping.Table, [alias])
                Me.SetupUpdateDeleteStatement(statement, obj, mapping, [alias])
                Me.SetupInsertUpdateStatement(statement, obj, mapping)
                statements.Add(statement)
            End If
        End Sub

        Private Function GetKeyColumn(ByVal mapping As NonPersistentObjectsDemo.[Module].DataStoreMapping) As DBColumn
            Return mapping.Table.Columns.First(Function(c) c.IsKey)
        End Function

        Private Sub SetupUpdateDeleteStatement(ByVal statement As DevExpress.Xpo.DB.ModificationStatement, ByVal obj As Object, ByVal mapping As NonPersistentObjectsDemo.[Module].DataStoreMapping, ByVal [alias] As String)
            statement.Condition = New DevExpress.Data.Filtering.BinaryOperator(New DevExpress.Xpo.DB.QueryOperand(Me.GetKeyColumn(mapping), [alias]), New DevExpress.Data.Filtering.OperandValue(mapping.GetKey(obj)), DevExpress.Data.Filtering.BinaryOperatorType.Equal)
        End Sub

        Private Sub SetupInsertUpdateStatement(ByVal statement As DevExpress.Xpo.DB.ModificationStatement, ByVal obj As Object, ByVal mapping As NonPersistentObjectsDemo.[Module].DataStoreMapping)
            Dim values = New Object(mapping.Table.Columns.Count - 1) {}
            mapping.Save(obj, values)
            For i As Integer = 0 To values.Length - 1
                Dim column = mapping.Table.Columns(i)
                If Not column.IsIdentity Then
                    statement.Operands.Add(New DevExpress.Xpo.DB.QueryOperand(column, Nothing))
                    statement.Parameters.Add(New DevExpress.Data.Filtering.OperandValue(values(i)))
                End If
            Next
        End Sub
    End Class

    Friend Class SimpleDataStoreCriteriaVisitor
        Inherits DevExpress.Data.Filtering.Helpers.ClientCriteriaVisitorBase

        Private table As DevExpress.Xpo.DB.DBTable

        Private [alias] As String

        Public Sub New(ByVal table As DevExpress.Xpo.DB.DBTable, ByVal [alias] As String)
            Me.table = table
            Me.[alias] = [alias]
        End Sub

        Protected Overrides Function Visit(ByVal theOperand As DevExpress.Data.Filtering.OperandProperty) As CriteriaOperator
            Dim column = Me.table.GetColumn(theOperand.PropertyName)
            If column IsNot Nothing Then
                Return New DevExpress.Xpo.DB.QueryOperand(Me.table.GetColumn(theOperand.PropertyName), Me.[alias])
            Else
                Return Nothing
            End If
        End Function

        Public Shared Function Transform(ByVal criteria As DevExpress.Data.Filtering.CriteriaOperator, ByVal table As DevExpress.Xpo.DB.DBTable, ByVal [alias] As String) As CriteriaOperator
            Return New NonPersistentObjectsDemo.[Module].SimpleDataStoreCriteriaVisitor(CType((table), DevExpress.Xpo.DB.DBTable), CStr(([alias]))).Process(criteria)
        End Function
    End Class
End Namespace
