Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports DevExpress.Xpo.DB

Namespace NonPersistentObjectsDemo.Module.BusinessObjects

	Public Class PostOfficeClient
		Shared Sub New()
			GlobalServiceProvider(Of PostOfficeClient).AddService(Function() New PostOfficeClient())
		End Sub
		Private privateMappings As Dictionary(Of Type, DataStoreMapping)
		Public Property Mappings() As Dictionary(Of Type, DataStoreMapping)
			Get
				Return privateMappings
			End Get
			Private Set(ByVal value As Dictionary(Of Type, DataStoreMapping))
				privateMappings = value
			End Set
		End Property
		Private privateDataStore As IDataStore
		Public Property DataStore() As IDataStore
			Get
				Return privateDataStore
			End Get
			Private Set(ByVal value As IDataStore)
				privateDataStore = value
			End Set
		End Property
		Public Sub New()
			Me.DataStore = New InMemoryDataStore(AutoCreateOption.DatabaseAndSchema, False)
			Me.Mappings = New Dictionary(Of Type, DataStoreMapping)()
			Dim mAccount = New DataStoreMapping()
			mAccount.Table = New DBTable("Accounts")
			mAccount.Table.AddColumn(New DBColumn("UserName", True, Nothing, 255, DBColumnType.String))
			mAccount.Table.AddColumn(New DBColumn("PublicName", False, Nothing, 1024, DBColumnType.String))
			mAccount.Create = Function() New Account()
			mAccount.Load = Sub(obj, values, omap)
				CType(obj, Account).SetKey(CStr(values(0)))
				CType(obj, Account).PublicName = CStr(values(1))
			End Sub
			mAccount.Save = Sub(obj, values)
				values(0) = CType(obj, Account).UserName
				values(1) = CType(obj, Account).PublicName
			End Sub
			mAccount.GetKey = Function(obj) (CType(obj, Account)).UserName
			mAccount.RefColumns = Enumerable.Empty(Of DataStoreMapping.Column)()
			Mappings.Add(GetType(Account), mAccount)
			Dim mMessage = New DataStoreMapping()
			mMessage.Table = New DBTable("Messages")
			Dim mMessageKey = New DBColumn("ID", True, Nothing, 0, DBColumnType.Int32)
			mMessageKey.IsIdentity = True
			mMessage.Table.AddColumn(mMessageKey)
			mMessage.Table.AddColumn(New DBColumn("Subject", False, Nothing, 1024, DBColumnType.String))
			mMessage.Table.AddColumn(New DBColumn("Body", False, Nothing, -1, DBColumnType.String))
			mMessage.Table.AddColumn(New DBColumn("Sender", False, Nothing, 255, DBColumnType.String))
			mMessage.Table.AddColumn(New DBColumn("Recepient", False, Nothing, 255, DBColumnType.String))
			mMessage.Table.PrimaryKey = New DBPrimaryKey(New Object() { mMessageKey })
			mMessage.Create = Function() New Message()
			mMessage.SetKey = Sub(obj, key)
				CType(obj, Message).SetKey(CInt(Math.Truncate(key)))
			End Sub
			mMessage.GetKey = Function(obj) (CType(obj, Message)).ID
			mMessage.Load = Sub(obj, values, omap)
				Dim o = CType(obj, Message)
				o.SetKey(CInt(Math.Truncate(values(0))))
				o.Subject = CStr(values(1))
				o.Body = CStr(values(2))
				o.Sender = GetReference(Of Account)(omap, values(3))
				o.Recepient = GetReference(Of Account)(omap, values(4))
			End Sub
			mMessage.Save = Sub(obj, values)
				Dim o = CType(obj, Message)
				values(0) = o.ID
				values(1) = o.Subject
				values(2) = o.Body
				values(3) = o.Sender?.UserName
				values(4) = o.Recepient?.UserName
			End Sub
			mMessage.RefColumns = New DataStoreMapping.Column() {
				New DataStoreMapping.Column() With {
					.Index = 3,
					.Type = GetType(Account)
				},
				New DataStoreMapping.Column() With {
					.Index = 4,
					.Type = GetType(Account)
				}
			}
			Mappings.Add(GetType(Message), mMessage)
			DataStore.UpdateSchema(False, mAccount.Table, mMessage.Table)
			CreateDemoData(DirectCast(DataStore, InMemoryDataStore))
		End Sub
		Private Shared Function GetReference(Of T)(ByVal map As ObjectMap, ByVal key As Object) As T
			Return If(key Is Nothing, CType(Nothing, T), map.Get(Of T)(key))
		End Function

		#Region "Demo Data"
		Private Shared Sub CreateDemoData(ByVal inMemoryDataStore As InMemoryDataStore)
			Dim ds = New System.Data.DataSet()
			Using ms = New System.IO.MemoryStream()
				Using writer = System.Xml.XmlWriter.Create(ms)
					inMemoryDataStore.WriteXml(writer)
					writer.Flush()
				End Using
				ms.Flush()
				ms.Position = 0
				ds.ReadXml(ms)
			End Using
			Dim gen = New GenHelper()
			Dim idsAccount = New List(Of String)()
			Dim dtAccounts = ds.Tables("Accounts")
			For i As Integer = 0 To 199
				Dim id = gen.MakeTosh(20)
				idsAccount.Add(id)
				dtAccounts.Rows.Add(id, gen.GetFullName())
			Next i
			Dim dtMessages = ds.Tables("Messages")
			For i As Integer = 0 To 4999
				Dim id1 = gen.Next(idsAccount.Count)
				Dim id2 = gen.Next(idsAccount.Count - 1)
				dtMessages.Rows.Add(Nothing, GenHelper.ToTitle(gen.MakeBlah(gen.Next(7))), gen.MakeBlahBlahBlah(5 + gen.Next(100), 7), idsAccount(id1), idsAccount((id1 + id2 + 1) Mod idsAccount.Count))
			Next i
			ds.AcceptChanges()
			Using ms = New System.IO.MemoryStream()
				ds.WriteXml(ms, System.Data.XmlWriteMode.WriteSchema)
				ms.Flush()
				ms.Position = 0
				Using reader = System.Xml.XmlReader.Create(ms)
					inMemoryDataStore.ReadXml(reader)
				End Using
			End Using
		End Sub
		#End Region
	End Class
End Namespace
