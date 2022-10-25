Imports System
Imports DevExpress.ExpressApp
Imports System.ComponentModel
Imports DevExpress.ExpressApp.Web
Imports DevExpress.ExpressApp.Xpo
Imports DevExpress.ExpressApp.Security

Namespace NonPersistentObjectsDemo.Web

    ' For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Web.WebApplication
    Public Partial Class NonPersistentObjectsDemoAspNetApplication
        Inherits WebApplication

        Private module1 As DevExpress.ExpressApp.SystemModule.SystemModule

        Private module2 As DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule

        Private module3 As [Module].NonPersistentObjectsDemoModule

        Private module4 As [Module].Web.NonPersistentObjectsDemoAspNetModule

        Private securityModule1 As SecurityModule

        Private securityStrategyComplex1 As SecurityStrategyComplex

        Private authenticationStandard1 As AuthenticationStandard

        Private objectsModule As Objects.BusinessClassLibraryCustomizationModule

        Private cloneObjectModule As CloneObject.CloneObjectModule

        Private conditionalAppearanceModule As ConditionalAppearance.ConditionalAppearanceModule

        Private fileAttachmentsAspNetModule As FileAttachments.Web.FileAttachmentsAspNetModule

        Private reportsModuleV2 As ReportsV2.ReportsModuleV2

        Private reportsAspNetModuleV2 As ReportsV2.Web.ReportsAspNetModuleV2

        Private schedulerModuleBase As Scheduler.SchedulerModuleBase

        Private schedulerAspNetModule As Scheduler.Web.SchedulerAspNetModule

        Private scriptRecorderModuleBase As ScriptRecorder.ScriptRecorderModuleBase

        Private scriptRecorderAspNetModule As ScriptRecorder.Web.ScriptRecorderAspNetModule

        Private treeListEditorsModuleBase As TreeListEditors.TreeListEditorsModuleBase

        Private treeListEditorsAspNetModule As TreeListEditors.Web.TreeListEditorsAspNetModule

        Private validationModule As Validation.ValidationModule

        Private validationAspNetModule As Validation.Web.ValidationAspNetModule

#Region "Default XAF configuration options (https://www.devexpress.com/kb=T501418)"
        Shared Sub New()
            EnableMultipleBrowserTabsSupport = True
            Editors.ASPx.ASPxGridListEditor.AllowFilterControlHierarchy = True
            Editors.ASPx.ASPxGridListEditor.MaxFilterControlHierarchyDepth = 3
            Editors.ASPx.ASPxCriteriaPropertyEditor.AllowFilterControlHierarchyDefault = True
            Editors.ASPx.ASPxCriteriaPropertyEditor.MaxHierarchyDepthDefault = 3
            DevExpress.Persistent.Base.PasswordCryptographer.EnableRfc2898 = True
            DevExpress.Persistent.Base.PasswordCryptographer.SupportLegacySha512 = False
        End Sub

        Private Sub InitializeDefaults()
            LinkNewObjectToParentImmediately = False
            OptimizedControllersCreation = True
        End Sub

#End Region
        Public Sub New()
            InitializeComponent()
            InitializeDefaults()
        End Sub

        Protected Overrides Function CreateViewUrlManager() As IViewUrlManager
            Return New ViewUrlManager()
        End Function

        Protected Overrides Sub CreateDefaultObjectSpaceProvider(ByVal args As CreateCustomObjectSpaceProviderEventArgs)
            'args.ObjectSpaceProvider = new SecuredObjectSpaceProvider((SecurityStrategyComplex)Security, GetDataStoreProvider(args.ConnectionString, args.Connection), true);
            args.ObjectSpaceProvider = New XPObjectSpaceProvider(GetDataStoreProvider(args.ConnectionString, args.Connection), True)
            args.ObjectSpaceProviders.Add(New NonPersistentObjectSpaceProvider(TypesInfo, Nothing))
        End Sub

        Private Function GetDataStoreProvider(ByVal connectionString As String, ByVal connection As System.Data.IDbConnection) As IXpoDataStoreProvider
            Dim application As System.Web.HttpApplicationState = If(System.Web.HttpContext.Current IsNot Nothing, System.Web.HttpContext.Current.Application, Nothing)
            Dim dataStoreProvider As IXpoDataStoreProvider = Nothing
            If application IsNot Nothing AndAlso application("DataStoreProvider") IsNot Nothing Then
                dataStoreProvider = TryCast(application("DataStoreProvider"), IXpoDataStoreProvider)
            Else
                dataStoreProvider = XPObjectSpaceProvider.GetDataStoreProvider(connectionString, connection, True)
                If application IsNot Nothing Then
                    application("DataStoreProvider") = dataStoreProvider
                End If
            End If

            Return dataStoreProvider
        End Function

        Private Sub NonPersistentObjectsDemoAspNetApplication_DatabaseVersionMismatch(ByVal sender As Object, ByVal e As DatabaseVersionMismatchEventArgs)
#If EASYTEST
            e.Updater.Update();
            e.Handled = true;
#Else
            If System.Diagnostics.Debugger.IsAttached Then
                e.Updater.Update()
                e.Handled = True
            Else
                Dim message As String = "The application cannot connect to the specified database, " & "because the database doesn't exist, its version is older " & "than that of the application or its schema does not match " & "the ORM data model structure. To avoid this error, use one " & "of the solutions from the https://www.devexpress.com/kb=T367835 KB Article."
                If e.CompatibilityError IsNot Nothing AndAlso e.CompatibilityError.Exception IsNot Nothing Then
                    message += Microsoft.VisualBasic.Constants.vbCrLf & Microsoft.VisualBasic.Constants.vbCrLf & "Inner exception: " & e.CompatibilityError.Exception.Message
                End If

                Throw New InvalidOperationException(message)
            End If
#End If
        End Sub

        Private Sub InitializeComponent()
            module1 = New SystemModule.SystemModule()
            module2 = New SystemModule.SystemAspNetModule()
            module3 = New [Module].NonPersistentObjectsDemoModule()
            module4 = New [Module].Web.NonPersistentObjectsDemoAspNetModule()
            securityModule1 = New SecurityModule()
            securityStrategyComplex1 = New SecurityStrategyComplex()
            securityStrategyComplex1.SupportNavigationPermissionsForTypes = False
            authenticationStandard1 = New AuthenticationStandard()
            objectsModule = New Objects.BusinessClassLibraryCustomizationModule()
            cloneObjectModule = New CloneObject.CloneObjectModule()
            conditionalAppearanceModule = New ConditionalAppearance.ConditionalAppearanceModule()
            fileAttachmentsAspNetModule = New FileAttachments.Web.FileAttachmentsAspNetModule()
            reportsModuleV2 = New ReportsV2.ReportsModuleV2()
            reportsAspNetModuleV2 = New ReportsV2.Web.ReportsAspNetModuleV2()
            schedulerModuleBase = New Scheduler.SchedulerModuleBase()
            schedulerAspNetModule = New Scheduler.Web.SchedulerAspNetModule()
            scriptRecorderModuleBase = New ScriptRecorder.ScriptRecorderModuleBase()
            scriptRecorderAspNetModule = New ScriptRecorder.Web.ScriptRecorderAspNetModule()
            treeListEditorsModuleBase = New TreeListEditors.TreeListEditorsModuleBase()
            treeListEditorsAspNetModule = New TreeListEditors.Web.TreeListEditorsAspNetModule()
            validationModule = New Validation.ValidationModule()
            validationAspNetModule = New Validation.Web.ValidationAspNetModule()
            CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
            ' 
            ' securityStrategyComplex1
            ' 
            securityStrategyComplex1.Authentication = authenticationStandard1
            securityStrategyComplex1.RoleType = GetType(DevExpress.Persistent.BaseImpl.PermissionPolicy.PermissionPolicyRole)
            securityStrategyComplex1.UserType = GetType(DevExpress.Persistent.BaseImpl.PermissionPolicy.PermissionPolicyUser)
            ' 
            ' securityModule1
            ' 
            securityModule1.UserType = GetType(DevExpress.Persistent.BaseImpl.PermissionPolicy.PermissionPolicyUser)
            ' 
            ' authenticationStandard1
            ' 
            authenticationStandard1.LogonParametersType = GetType(AuthenticationStandardLogonParameters)
            '
            ' reportsModuleV2
            '
            reportsModuleV2.EnableInplaceReports = True
            reportsModuleV2.ReportDataType = GetType(DevExpress.Persistent.BaseImpl.ReportDataV2)
            reportsModuleV2.ShowAdditionalNavigation = False
            reportsAspNetModuleV2.ReportViewerType = ReportsV2.Web.ReportViewerTypes.HTML5
            reportsModuleV2.ReportStoreMode = ReportsV2.ReportStoreModes.XML
            ' 
            ' NonPersistentObjectsDemoAspNetApplication
            ' 
            ApplicationName = "NonPersistentObjectsDemo"
            CheckCompatibilityType = CheckCompatibilityType.DatabaseSchema
            Modules.Add(module1)
            Modules.Add(module2)
            Modules.Add(module3)
            Modules.Add(module4)
            Modules.Add(securityModule1)
            'this.Security = this.securityStrategyComplex1;
            Modules.Add(objectsModule)
            Modules.Add(cloneObjectModule)
            Modules.Add(conditionalAppearanceModule)
            Modules.Add(fileAttachmentsAspNetModule)
            Modules.Add(reportsModuleV2)
            Modules.Add(reportsAspNetModuleV2)
            Modules.Add(schedulerModuleBase)
            Modules.Add(schedulerAspNetModule)
            Modules.Add(scriptRecorderModuleBase)
            Modules.Add(scriptRecorderAspNetModule)
            Modules.Add(treeListEditorsModuleBase)
            Modules.Add(treeListEditorsAspNetModule)
            Modules.Add(validationModule)
            Modules.Add(validationAspNetModule)
            AddHandler DatabaseVersionMismatch, New EventHandler(Of DatabaseVersionMismatchEventArgs)(AddressOf NonPersistentObjectsDemoAspNetApplication_DatabaseVersionMismatch)
            CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        End Sub
    End Class
End Namespace
