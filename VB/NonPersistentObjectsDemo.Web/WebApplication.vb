Imports System
Imports DevExpress.ExpressApp
Imports System.ComponentModel
Imports DevExpress.ExpressApp.Web
Imports System.Collections.Generic
Imports DevExpress.ExpressApp.Xpo
Imports DevExpress.ExpressApp.Security
Imports DevExpress.ExpressApp.Security.ClientServer

Namespace NonPersistentObjectsDemo.Web
	' For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Web.WebApplication
	Partial Public Class NonPersistentObjectsDemoAspNetApplication
		Inherits WebApplication

		Private module1 As DevExpress.ExpressApp.SystemModule.SystemModule
		Private module2 As DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule
		Private module3 As NonPersistentObjectsDemo.Module.NonPersistentObjectsDemoModule
		Private module4 As NonPersistentObjectsDemo.Module.Web.NonPersistentObjectsDemoAspNetModule
		Private securityModule1 As DevExpress.ExpressApp.Security.SecurityModule
		Private securityStrategyComplex1 As DevExpress.ExpressApp.Security.SecurityStrategyComplex
		Private authenticationStandard1 As DevExpress.ExpressApp.Security.AuthenticationStandard
		Private objectsModule As DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule
		Private cloneObjectModule As DevExpress.ExpressApp.CloneObject.CloneObjectModule
		Private conditionalAppearanceModule As DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule
		Private fileAttachmentsAspNetModule As DevExpress.ExpressApp.FileAttachments.Web.FileAttachmentsAspNetModule
		Private reportsModuleV2 As DevExpress.ExpressApp.ReportsV2.ReportsModuleV2
		Private reportsAspNetModuleV2 As DevExpress.ExpressApp.ReportsV2.Web.ReportsAspNetModuleV2
		Private schedulerModuleBase As DevExpress.ExpressApp.Scheduler.SchedulerModuleBase
		Private schedulerAspNetModule As DevExpress.ExpressApp.Scheduler.Web.SchedulerAspNetModule
		Private scriptRecorderModuleBase As DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderModuleBase
		Private scriptRecorderAspNetModule As DevExpress.ExpressApp.ScriptRecorder.Web.ScriptRecorderAspNetModule
		Private treeListEditorsModuleBase As DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase
		Private treeListEditorsAspNetModule As DevExpress.ExpressApp.TreeListEditors.Web.TreeListEditorsAspNetModule
		Private validationModule As DevExpress.ExpressApp.Validation.ValidationModule
		Private validationAspNetModule As DevExpress.ExpressApp.Validation.Web.ValidationAspNetModule

		#Region "Default XAF configuration options (https:" 'www.devexpress.com/kb=T501418)
		Shared Sub New()
			EnableMultipleBrowserTabsSupport = True
			DevExpress.ExpressApp.Web.Editors.ASPx.ASPxGridListEditor.AllowFilterControlHierarchy = True
			DevExpress.ExpressApp.Web.Editors.ASPx.ASPxGridListEditor.MaxFilterControlHierarchyDepth = 3
			DevExpress.ExpressApp.Web.Editors.ASPx.ASPxCriteriaPropertyEditor.AllowFilterControlHierarchyDefault = True
			DevExpress.ExpressApp.Web.Editors.ASPx.ASPxCriteriaPropertyEditor.MaxHierarchyDepthDefault = 3
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
'INSTANT VB NOTE: The variable connectionString was renamed since Visual Basic does not handle local variables named the same as class members well:
'INSTANT VB NOTE: The variable connection was renamed since Visual Basic does not handle local variables named the same as class members well:
		Private Function GetDataStoreProvider(ByVal connectionString_Conflict As String, ByVal connection_Conflict As System.Data.IDbConnection) As IXpoDataStoreProvider
			Dim application As System.Web.HttpApplicationState = If(System.Web.HttpContext.Current IsNot Nothing, System.Web.HttpContext.Current.Application, Nothing)
			Dim dataStoreProvider As IXpoDataStoreProvider = Nothing
			If application IsNot Nothing AndAlso application("DataStoreProvider") IsNot Nothing Then
				dataStoreProvider = TryCast(application("DataStoreProvider"), IXpoDataStoreProvider)
			Else
				dataStoreProvider = XPObjectSpaceProvider.GetDataStoreProvider(connectionString_Conflict, connection_Conflict, True)
				If application IsNot Nothing Then
					application("DataStoreProvider") = dataStoreProvider
				End If
			End If
			Return dataStoreProvider
		End Function
		Private Sub NonPersistentObjectsDemoAspNetApplication_DatabaseVersionMismatch(ByVal sender As Object, ByVal e As DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs) Handles Me.DatabaseVersionMismatch
#If EASYTEST Then
			e.Updater.Update()
			e.Handled = True
#Else
			If System.Diagnostics.Debugger.IsAttached Then
				e.Updater.Update()
				e.Handled = True
			Else
				Dim message As String = "The application cannot connect to the specified database, " & "because the database doesn't exist, its version is older " & "than that of the application or its schema does not match " & "the ORM data model structure. To avoid this error, use one " & "of the solutions from the https://www.devexpress.com/kb=T367835 KB Article."

				If e.CompatibilityError IsNot Nothing AndAlso e.CompatibilityError.Exception IsNot Nothing Then
					message &= vbCrLf & vbCrLf & "Inner exception: " & e.CompatibilityError.Exception.Message
				End If
				Throw New InvalidOperationException(message)
			End If
#End If
		End Sub
		Private Sub InitializeComponent()
			Me.module1 = New DevExpress.ExpressApp.SystemModule.SystemModule()
			Me.module2 = New DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule()
			Me.module3 = New NonPersistentObjectsDemo.Module.NonPersistentObjectsDemoModule()
			Me.module4 = New NonPersistentObjectsDemo.Module.Web.NonPersistentObjectsDemoAspNetModule()
			Me.securityModule1 = New DevExpress.ExpressApp.Security.SecurityModule()
			Me.securityStrategyComplex1 = New DevExpress.ExpressApp.Security.SecurityStrategyComplex()
			Me.securityStrategyComplex1.SupportNavigationPermissionsForTypes = False
			Me.authenticationStandard1 = New DevExpress.ExpressApp.Security.AuthenticationStandard()
			Me.objectsModule = New DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule()
			Me.cloneObjectModule = New DevExpress.ExpressApp.CloneObject.CloneObjectModule()
			Me.conditionalAppearanceModule = New DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule()
			Me.fileAttachmentsAspNetModule = New DevExpress.ExpressApp.FileAttachments.Web.FileAttachmentsAspNetModule()
			Me.reportsModuleV2 = New DevExpress.ExpressApp.ReportsV2.ReportsModuleV2()
			Me.reportsAspNetModuleV2 = New DevExpress.ExpressApp.ReportsV2.Web.ReportsAspNetModuleV2()
			Me.schedulerModuleBase = New DevExpress.ExpressApp.Scheduler.SchedulerModuleBase()
			Me.schedulerAspNetModule = New DevExpress.ExpressApp.Scheduler.Web.SchedulerAspNetModule()
			Me.scriptRecorderModuleBase = New DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderModuleBase()
			Me.scriptRecorderAspNetModule = New DevExpress.ExpressApp.ScriptRecorder.Web.ScriptRecorderAspNetModule()
			Me.treeListEditorsModuleBase = New DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase()
			Me.treeListEditorsAspNetModule = New DevExpress.ExpressApp.TreeListEditors.Web.TreeListEditorsAspNetModule()
			Me.validationModule = New DevExpress.ExpressApp.Validation.ValidationModule()
			Me.validationAspNetModule = New DevExpress.ExpressApp.Validation.Web.ValidationAspNetModule()
			DirectCast(Me, System.ComponentModel.ISupportInitialize).BeginInit()
			' 
			' securityStrategyComplex1
			' 
			Me.securityStrategyComplex1.Authentication = Me.authenticationStandard1
			Me.securityStrategyComplex1.RoleType = GetType(DevExpress.Persistent.BaseImpl.PermissionPolicy.PermissionPolicyRole)
			Me.securityStrategyComplex1.UserType = GetType(DevExpress.Persistent.BaseImpl.PermissionPolicy.PermissionPolicyUser)
			' 
			' securityModule1
			' 
			Me.securityModule1.UserType = GetType(DevExpress.Persistent.BaseImpl.PermissionPolicy.PermissionPolicyUser)
			' 
			' authenticationStandard1
			' 
			Me.authenticationStandard1.LogonParametersType = GetType(DevExpress.ExpressApp.Security.AuthenticationStandardLogonParameters)
			'
			' reportsModuleV2
			'
			Me.reportsModuleV2.EnableInplaceReports = True
			Me.reportsModuleV2.ReportDataType = GetType(DevExpress.Persistent.BaseImpl.ReportDataV2)
			Me.reportsModuleV2.ShowAdditionalNavigation = False
			Me.reportsAspNetModuleV2.ReportViewerType = DevExpress.ExpressApp.ReportsV2.Web.ReportViewerTypes.HTML5
			Me.reportsModuleV2.ReportStoreMode = DevExpress.ExpressApp.ReportsV2.ReportStoreModes.XML
			' 
			' NonPersistentObjectsDemoAspNetApplication
			' 
			Me.ApplicationName = "NonPersistentObjectsDemo"
			Me.CheckCompatibilityType = DevExpress.ExpressApp.CheckCompatibilityType.DatabaseSchema
			Me.Modules.Add(Me.module1)
			Me.Modules.Add(Me.module2)
			Me.Modules.Add(Me.module3)
			Me.Modules.Add(Me.module4)
			Me.Modules.Add(Me.securityModule1)
			'this.Security = this.securityStrategyComplex1;
			Me.Modules.Add(Me.objectsModule)
			Me.Modules.Add(Me.cloneObjectModule)
			Me.Modules.Add(Me.conditionalAppearanceModule)
			Me.Modules.Add(Me.fileAttachmentsAspNetModule)
			Me.Modules.Add(Me.reportsModuleV2)
			Me.Modules.Add(Me.reportsAspNetModuleV2)
			Me.Modules.Add(Me.schedulerModuleBase)
			Me.Modules.Add(Me.schedulerAspNetModule)
			Me.Modules.Add(Me.scriptRecorderModuleBase)
			Me.Modules.Add(Me.scriptRecorderAspNetModule)
			Me.Modules.Add(Me.treeListEditorsModuleBase)
			Me.Modules.Add(Me.treeListEditorsAspNetModule)
			Me.Modules.Add(Me.validationModule)
			Me.Modules.Add(Me.validationAspNetModule)
'INSTANT VB NOTE: The following InitializeComponent event wireup was converted to a 'Handles' clause:
'ORIGINAL LINE: this.DatabaseVersionMismatch += new System.EventHandler<DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs>(this.NonPersistentObjectsDemoAspNetApplication_DatabaseVersionMismatch);
			DirectCast(Me, System.ComponentModel.ISupportInitialize).EndInit()

		End Sub
	End Class
End Namespace
