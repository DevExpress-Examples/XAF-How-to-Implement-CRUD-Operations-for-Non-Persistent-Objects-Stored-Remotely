Imports System
Imports System.Configuration
Imports System.Windows.Forms
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Security
Imports DevExpress.Persistent.Base
Imports DevExpress.XtraEditors

Namespace NonPersistentObjectsDemo.Win

    Friend Module Program

        ''' <summary>
        ''' The main entry point for the application.
        ''' </summary>
        <STAThread>
        Sub Main()
#If EASYTEST
            DevExpress.ExpressApp.Win.EasyTest.EasyTestRemotingRegistration.Register();
#End If
            Call WindowsFormsSettings.LoadApplicationSettings()
            Call Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)
            DevExpress.Utils.ToolTipController.DefaultController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip
            BaseObjectSpace.ThrowExceptionForNotRegisteredEntityType = True
            EditModelPermission.AlwaysGranted = System.Diagnostics.Debugger.IsAttached
            If Tracing.GetFileLocationFromSettings() = DevExpress.Persistent.Base.FileLocation.CurrentUserApplicationDataFolder Then
                Tracing.LocalUserAppDataPath = Application.LocalUserAppDataPath
            End If

            Call Tracing.Initialize()
            Dim winApplication As NonPersistentObjectsDemoWindowsFormsApplication = New NonPersistentObjectsDemoWindowsFormsApplication()
            'SecurityStrategy security = (SecurityStrategy)winApplication.Security;
            'security.RegisterXPOAdapterProviders();
            If ConfigurationManager.ConnectionStrings("ConnectionString") IsNot Nothing Then
                winApplication.ConnectionString = ConfigurationManager.ConnectionStrings("ConnectionString").ConnectionString
            End If

#If EASYTEST
            if(ConfigurationManager.ConnectionStrings["EasyTestConnectionString"] != null) {
                winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["EasyTestConnectionString"].ConnectionString;
            }
#End If
            winApplication.ConnectionString = Xpo.InMemoryDataStoreProvider.ConnectionString
#If DEBUG
            If System.Diagnostics.Debugger.IsAttached AndAlso winApplication.CheckCompatibilityType = CheckCompatibilityType.DatabaseSchema Then
                winApplication.DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways
            End If

#End If
            Try
                winApplication.Setup()
                winApplication.Start()
            Catch e As Exception
                winApplication.StopSplash()
                winApplication.HandleException(e)
            End Try
        End Sub
    End Module
End Namespace
