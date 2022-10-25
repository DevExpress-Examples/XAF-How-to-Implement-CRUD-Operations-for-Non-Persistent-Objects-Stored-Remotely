Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports DevExpress.Xpo.DB

Namespace NonPersistentObjectsDemo.[Module].BusinessObjects

    Public Class PostOfficeClient

        Private _Mappings As Dictionary(Of System.Type, NonPersistentObjectsDemo.[Module].DataStoreMapping), _DataStore As IDataStore

        Shared Sub New()
            Call NonPersistentObjectsDemo.[Module].GlobalServiceProvider(Of NonPersistentObjectsDemo.[Module].BusinessObjects.PostOfficeClient).AddService(Function() New NonPersistentObjectsDemo.[Module].BusinessObjects.PostOfficeClient())
        End Sub

        Public Property Mappings As Dictionary(Of System.Type, NonPersistentObjectsDemo.[Module].DataStoreMapping)
            Get
                Return _Mappings
            End Get

            Private Set(ByVal value As Dictionary(Of System.Type, NonPersistentObjectsDemo.[Module].DataStoreMapping))
                _Mappings = value
            End Set
        End Property

        Public Property DataStore As IDataStore
            Get
                Return _DataStore
            End Get

            Private Set(ByVal value As IDataStore)
                _DataStore = value
            End Set
        End Property

        Public Sub New()
            Me.DataStore = New DevExpress.Xpo.DB.InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, False)
            Me.Mappings = New System.Collections.Generic.Dictionary(Of System.Type, NonPersistentObjectsDemo.[Module].DataStoreMapping)()
            Dim mAccount = New NonPersistentObjectsDemo.[Module].DataStoreMapping()
            mAccount.Table = New DevExpress.Xpo.DB.DBTable("Accounts")
            mAccount.Table.AddColumn(New DevExpress.Xpo.DB.DBColumn("UserName", True, Nothing, 255, DevExpress.Xpo.DB.DBColumnType.[String]))
            mAccount.Table.AddColumn(New DevExpress.Xpo.DB.DBColumn("PublicName", False, Nothing, 1024, DevExpress.Xpo.DB.DBColumnType.[String]))
            mAccount.Create = Function() New NonPersistentObjectsDemo.[Module].BusinessObjects.Account()
            mAccount.Load = Sub(obj, values, omap)
                CType(obj, NonPersistentObjectsDemo.[Module].BusinessObjects.Account).SetKey(CStr(values(0)))
                CType(obj, NonPersistentObjectsDemo.[Module].BusinessObjects.Account).PublicName = CStr(values(1))
            End Sub
            mAccount.Save = Sub(obj, values)
                values(0) = CType(obj, NonPersistentObjectsDemo.[Module].BusinessObjects.Account).UserName
                values(1) = CType(obj, NonPersistentObjectsDemo.[Module].BusinessObjects.Account).PublicName
            End Sub
            mAccount.GetKey = Function(obj) CType(obj, NonPersistentObjectsDemo.[Module].BusinessObjects.Account).UserName
            mAccount.RefColumns = System.Linq.Enumerable.Empty(Of NonPersistentObjectsDemo.[Module].DataStoreMapping.Column)()
            Me.Mappings.Add(GetType(NonPersistentObjectsDemo.[Module].BusinessObjects.Account), mAccount)
            Dim mMessage = New NonPersistentObjectsDemo.[Module].DataStoreMapping()
            mMessage.Table = New DevExpress.Xpo.DB.DBTable("Messages")
            Dim mMessageKey = New DevExpress.Xpo.DB.DBColumn("ID", True, Nothing, 0, DevExpress.Xpo.DB.DBColumnType.Int32)
            mMessageKey.IsIdentity = True
            mMessage.Table.AddColumn(mMessageKey)
            mMessage.Table.AddColumn(New DevExpress.Xpo.DB.DBColumn("Subject", False, Nothing, 1024, DevExpress.Xpo.DB.DBColumnType.[String]))
            mMessage.Table.AddColumn(New DevExpress.Xpo.DB.DBColumn("Body", False, Nothing, -1, DevExpress.Xpo.DB.DBColumnType.[String]))
            mMessage.Table.AddColumn(New DevExpress.Xpo.DB.DBColumn("Sender", False, Nothing, 255, DevExpress.Xpo.DB.DBColumnType.[String]))
            mMessage.Table.AddColumn(New DevExpress.Xpo.DB.DBColumn("Recepient", False, Nothing, 255, DevExpress.Xpo.DB.DBColumnType.[String]))
            mMessage.Table.PrimaryKey = New DevExpress.Xpo.DB.DBPrimaryKey(New Object() {mMessageKey})
            mMessage.Create = Function() New NonPersistentObjectsDemo.[Module].BusinessObjects.Message()
            mMessage.SetKey = Sub(obj, key) CType(obj, NonPersistentObjectsDemo.[Module].BusinessObjects.Message).SetKey(CInt(key))
            mMessage.GetKey = Function(obj) CType(obj, NonPersistentObjectsDemo.[Module].BusinessObjects.Message).ID
            mMessage.Load = Sub(obj, values, omap)
                Dim o = CType(obj, NonPersistentObjectsDemo.[Module].BusinessObjects.Message)
                o.SetKey(CInt(values(0)))
                o.Subject = CStr(values(1))
                o.Body = CStr(values(2))
                o.Sender = GetReference(Of NonPersistentObjectsDemo.[Module].BusinessObjects.Account)(omap, values(3))
                o.Recepient = GetReference(Of NonPersistentObjectsDemo.[Module].BusinessObjects.Account)(omap, values(4))
            End Sub
            mMessage.Save = Sub(obj, values)
                Dim o = CType(obj, NonPersistentObjectsDemo.[Module].BusinessObjects.Message)
                values(0) = o.ID
                values(1) = o.Subject
                values(2) = o.Body
                values(3) = o.Sender?.UserName
                values(4) = o.Recepient?.UserName
            End Sub
            mMessage.RefColumns = New NonPersistentObjectsDemo.[Module].DataStoreMapping.Column() {New NonPersistentObjectsDemo.[Module].DataStoreMapping.Column() With {.Index = 3, .Type = GetType(NonPersistentObjectsDemo.[Module].BusinessObjects.Account)}, New NonPersistentObjectsDemo.[Module].DataStoreMapping.Column() With {.Index = 4, .Type = GetType(NonPersistentObjectsDemo.[Module].BusinessObjects.Account)}}
            Me.Mappings.Add(GetType(NonPersistentObjectsDemo.[Module].BusinessObjects.Message), mMessage)
            Me.DataStore.UpdateSchema(False, mAccount.Table, mMessage.Table)
            Call NonPersistentObjectsDemo.[Module].BusinessObjects.PostOfficeClient.CreateDemoData(CType(Me.DataStore, DevExpress.Xpo.DB.InMemoryDataStore))
        End Sub

        Private Shared Function GetReference(Of T)(ByVal map As NonPersistentObjectsDemo.[Module].ObjectMap, ByVal key As Object) As T
            Return If((key Is Nothing), DirectCast(Nothing, T), map.[Get](Of T)(key))
        End Function

#Region "Demo Data"
        Private Shared Sub CreateDemoData(ByVal inMemoryDataStore As DevExpress.Xpo.DB.InMemoryDataStore)
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

            Dim gen = New NonPersistentObjectsDemo.[Module].GenHelper()
            Dim idsAccount = New System.Collections.Generic.List(Of String)()
            Dim dtAccounts = ds.Tables("Accounts")
            For i As Integer = 0 To 200 - 1
                Dim id = gen.MakeTosh(20)
                idsAccount.Add(id)
                dtAccounts.Rows.Add(id, gen.GetFullName())
            Next

            Dim dtMessages = ds.Tables("Messages")
            For i As Integer = 0 To 5000 - 1
                Dim id1 = gen.[Next](idsAccount.Count)
                Dim id2 = gen.[Next](idsAccount.Count - 1)
                dtMessages.Rows.Add(Nothing, NonPersistentObjectsDemo.[Module].GenHelper.ToTitle(gen.MakeBlah(gen.[Next](7))), gen.MakeBlahBlahBlah(5 + gen.[Next](100), 7), idsAccount(id1), idsAccount((id1 + id2 + 1) Mod idsAccount.Count))
            Next

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
