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

Namespace NonPersistentObjectsDemo.Module

	Public Class DataStoreMapping
		Public Table As DBTable
		Public Create As Func(Of Object)
		Public Load As Action(Of Object, Object(), ObjectMap)
		Public Save As Action(Of Object, Object())
		Public SetKey As Action(Of Object, Object)
		Public GetKey As Func(Of Object, Object)
		Public RefColumns As IEnumerable(Of Column)
		Public Structure Column
			Public Index As Integer
			Public Type As Type
		End Structure
	End Class

	Friend Class DataStoreObjectLoader
		Private Structure PreResult
			Public Mapping As DataStoreMapping
			Public ObjectType As Type
			Public Statement As SelectStatement
		End Structure
		Private Structure PostResult
			Public Mapping As DataStoreMapping
			Public Objects As List(Of Object)
			Public Result As SelectStatementResult
		End Structure
		Private objectMap As ObjectMap
		Private dataStore As IDataStore
		Private mappings As Dictionary(Of Type, DataStoreMapping)
		Public Sub New(ByVal mappings As Dictionary(Of Type, DataStoreMapping), ByVal dataStore As IDataStore, ByVal objectMap As ObjectMap)
			Me.mappings = mappings
			Me.dataStore = dataStore
			Me.objectMap = objectMap
		End Sub
		Private Function GetKeyColumn(ByVal mapping As DataStoreMapping) As DBColumn
			Return mapping.Table.Columns.First(Function(c) c.IsKey)
		End Function
		Private Function BuildByKeyCriteria(ByVal objectType As Type, ByVal key As Object, ByVal [alias] As String) As CriteriaOperator
			Return New BinaryOperator(New QueryOperand(GetKeyColumn(mappings(objectType)), [alias]), New OperandValue(key), BinaryOperatorType.Equal)
		End Function
		Private Function PhaseOne(ByVal mapping As DataStoreMapping, ByVal objectType As Type, ByVal dbCriteria As CriteriaOperator, ByVal [alias] As String) As SelectStatement
			Dim statement = New SelectStatement(mapping.Table, [alias])
			statement.Condition = dbCriteria
			For Each column In mapping.Table.Columns
				statement.Operands.Add(New QueryOperand(column, [alias]))
			Next column
			Return statement
		End Function
		Private Function PhaseTwo(ByVal mapping As DataStoreMapping, ByVal objectType As Type, ByVal result As SelectStatementResult, ByVal toLoad As List(Of PreResult)) As List(Of Object)
			Dim objects As New List(Of Object)()
			Dim keyColumnIndex As Integer = mapping.Table.Columns.IndexOf(GetKeyColumn(mapping))
			Dim refColumns As List(Of DataStoreMapping.Column) = mapping.RefColumns.ToList()
			For Each row In result.Rows
				Dim key = row.Values(keyColumnIndex)
				If key Is Nothing Then
					Throw New DataException("Key cannot be null.")
				End If
				Dim obj = objectMap.Get(objectType, key)
				If obj Is Nothing Then
					obj = mapping.Create()
					objectMap.Add(objectType, key, obj)
				End If
				objects.Add(obj)
				For Each member In refColumns
					Dim [alias] = "T"
					Dim dbCriteria = BuildByKeyCriteria(member.Type, row.Values(member.Index), [alias])
					Dim memberMapping = mappings(member.Type)
					toLoad.Add(New PreResult() With {
						.Mapping = memberMapping,
						.ObjectType = member.Type,
						.Statement = PhaseOne(memberMapping, member.Type, dbCriteria, [alias])
					})
				Next member
			Next row
			Return objects
		End Function
		Private Sub PhaseThree(ByVal mapping As DataStoreMapping, ByVal objects As List(Of Object), ByVal result As SelectStatementResult)
			For i As Integer = 0 To objects.Count - 1
				mapping.Load(objects(i), result.Rows(i).Values, objectMap)
			Next i
		End Sub
		Public Function LoadObjects(ByVal objectType As Type, ByVal criteria As CriteriaOperator) As IList(Of Object)
			Dim mapping = mappings(objectType)
			Dim [alias] = "T"
			Dim dbCriteria = SimpleDataStoreCriteriaVisitor.Process(criteria, mapping.Table, [alias])
			Return LoadObjectsCore(objectType, dbCriteria, [alias])
		End Function
		Private Function LoadObjectsCore(ByVal objectType0 As Type, ByVal dbCriteria As CriteriaOperator, ByVal [alias] As String) As IList(Of Object)
			Dim objects0 As List(Of Object) = Nothing
			Dim preResults = New List(Of PreResult)()
			Dim postResults = New List(Of PostResult)()
			Dim mapping0 = mappings(objectType0)
			Dim statement0 = PhaseOne(mapping0, objectType0, dbCriteria, [alias])
			preResults.Add(New PreResult() With {
				.Mapping = mapping0,
				.ObjectType = objectType0,
				.Statement = statement0
			})
			Do While preResults.Count > 0
				Dim statements = preResults.Select(Function(p) p.Statement).ToArray()
				Dim selectedData = dataStore.SelectData(statements)
				Dim toLoad = New List(Of PreResult)()
				For i As Integer = 0 To selectedData.ResultSet.Length - 1
					Dim mapping = preResults(i).Mapping
					Dim result = selectedData.ResultSet(i)
					Dim objects = PhaseTwo(mapping, preResults(i).ObjectType, result, toLoad)
					If objects0 Is Nothing Then
						objects0 = objects
					End If
					postResults.Add(New PostResult() With {
						.Mapping = mapping,
						.Objects = objects,
						.Result = result
					})
				Next i
				preResults = toLoad
			Loop
			For Each postResult In postResults
				PhaseThree(postResult.Mapping, postResult.Objects, postResult.Result)
			Next postResult
			Return objects0
		End Function
		Public Function LoadObjectByKey(ByVal objectType As Type, ByVal key As Object) As Object
			Dim [alias] = "T"
			Dim objects = LoadObjectsCore(objectType, BuildByKeyCriteria(objectType, key, [alias]), [alias])
			If objects.Count = 1 Then
				Return objects(0)
			End If
			If objects.Count = 0 Then
				Return Nothing
			End If
			Throw New DataException()
		End Function
	End Class

	Friend Class DataStoreObjectSaver
		Private dataStore As IDataStore
		Private mappings As Dictionary(Of Type, DataStoreMapping)
		Public Sub New(ByVal mappings As Dictionary(Of Type, DataStoreMapping), ByVal dataStore As IDataStore)
			Me.mappings = mappings
			Me.dataStore = dataStore
		End Sub
		Public Sub SaveObjects(ByVal toInsert As ICollection, ByVal toUpdate As ICollection, ByVal toDelete As ICollection)
			Dim statements = New List(Of ModificationStatement)()
			Dim identityAwaiters = New List(Of Action(Of Object))()
			For Each obj In toDelete
				DeleteObject(obj, statements)
			Next obj
			For Each obj In toInsert
				InsertObject(obj, statements, identityAwaiters)
			Next obj
			For Each obj In toUpdate
				UpdateObject(obj, statements)
			Next obj
			Dim result = dataStore.ModifyData(statements.ToArray())
			For Each identity In result.Identities
				identityAwaiters(identity.Tag - 1).Invoke(identity.Value)
			Next identity
		End Sub
		Private Sub DeleteObject(ByVal obj As Object, ByVal statements As IList(Of ModificationStatement))
			Dim mapping As DataStoreMapping
			If mappings.TryGetValue(obj.GetType(), mapping) Then
				Dim [alias] As String = Nothing
				Dim statement = New DeleteStatement(mapping.Table, [alias])
				SetupUpdateDeleteStatement(statement, obj, mapping, [alias])
				statements.Add(statement)
			End If
		End Sub
		Private Sub InsertObject(ByVal obj As Object, ByVal statements As IList(Of ModificationStatement), ByVal identityAwaiters As List(Of Action(Of Object)))
			Dim mapping As DataStoreMapping
			If mappings.TryGetValue(obj.GetType(), mapping) Then
				Dim statement = New InsertStatement(mapping.Table, "T")
				If mapping.Table.PrimaryKey IsNot Nothing Then
					For Each columnName In mapping.Table.PrimaryKey.Columns
						Dim column = mapping.Table.GetColumn(columnName)
						If column.IsIdentity Then
							identityAwaiters.Add(Sub(v)
								mapping.SetKey(obj, v)
							End Sub)
							statement.IdentityColumn = column.Name
							statement.IdentityColumnType = column.ColumnType
							statement.IdentityParameter = New ParameterValue(identityAwaiters.Count)
							Exit For
						End If
					Next columnName
				End If
				SetupInsertUpdateStatement(statement, obj, mapping)
				statements.Add(statement)
			End If
		End Sub
		Private Sub UpdateObject(ByVal obj As Object, ByVal statements As IList(Of ModificationStatement))
			Dim mapping As DataStoreMapping
			If mappings.TryGetValue(obj.GetType(), mapping) Then
				Dim [alias] As String = Nothing
				Dim statement = New UpdateStatement(mapping.Table, [alias])
				SetupUpdateDeleteStatement(statement, obj, mapping, [alias])
				SetupInsertUpdateStatement(statement, obj, mapping)
				statements.Add(statement)
			End If
		End Sub
		Private Function GetKeyColumn(ByVal mapping As DataStoreMapping) As DBColumn
			Return mapping.Table.Columns.First(Function(c) c.IsKey)
		End Function
		Private Sub SetupUpdateDeleteStatement(ByVal statement As ModificationStatement, ByVal obj As Object, ByVal mapping As DataStoreMapping, ByVal [alias] As String)
			statement.Condition = New BinaryOperator(New QueryOperand(GetKeyColumn(mapping), [alias]), New OperandValue(mapping.GetKey(obj)), BinaryOperatorType.Equal)
		End Sub
		Private Sub SetupInsertUpdateStatement(ByVal statement As ModificationStatement, ByVal obj As Object, ByVal mapping As DataStoreMapping)
			Dim values = New Object(mapping.Table.Columns.Count - 1){}
			mapping.Save(obj, values)
			For i As Integer = 0 To values.Length - 1
				Dim column = mapping.Table.Columns(i)
				If Not column.IsIdentity Then
					statement.Operands.Add(New QueryOperand(column, Nothing))
					statement.Parameters.Add(New OperandValue(values(i)))
				End If
			Next i
		End Sub
	End Class

	Friend Class SimpleDataStoreCriteriaVisitor
		Inherits ClientCriteriaVisitorBase

		Private table As DBTable
		Private [alias] As String
		Public Sub New(ByVal table As DBTable, ByVal [alias] As String)
			Me.table = table
			Me.alias = [alias]
		End Sub
		Protected Overrides Function Visit(ByVal theOperand As OperandProperty) As CriteriaOperator
			Dim column = table.GetColumn(theOperand.PropertyName)
			If column IsNot Nothing Then
				Return New QueryOperand(table.GetColumn(theOperand.PropertyName), [alias])
			Else
				Return Nothing
			End If
		End Function
		Public Shared Function Process(ByVal criteria As CriteriaOperator, ByVal table As DBTable, ByVal [alias] As String) As CriteriaOperator
			Return (New SimpleDataStoreCriteriaVisitor(table, [alias])).Process(criteria)
		End Function
	End Class
End Namespace
