Namespace NonPersistentObjectsDemo.Win
	Partial Public Class NonPersistentObjectsDemoWindowsFormsApplication
		''' <summary> 
		''' Required designer variable.
		''' </summary>
		Private components As System.ComponentModel.IContainer = Nothing

		''' <summary> 
		''' Clean up any resources being used.
		''' </summary>
		''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		Protected Overrides Sub Dispose(ByVal disposing As Boolean)
			If disposing AndAlso (components IsNot Nothing) Then
				components.Dispose()
			End If
			MyBase.Dispose(disposing)
		End Sub

		#Region "Component Designer generated code"

		''' <summary> 
		''' Required method for Designer support - do not modify 
		''' the contents of this method with the code editor.
		''' </summary>
		Private Sub InitializeComponent()
			Me.module1 = New DevExpress.ExpressApp.SystemModule.SystemModule()
			Me.module2 = New DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule()
			Me.module3 = New NonPersistentObjectsDemo.Module.NonPersistentObjectsDemoModule()
			Me.module4 = New NonPersistentObjectsDemo.Module.Win.NonPersistentObjectsDemoWindowsFormsModule()
			Me.securityModule1 = New DevExpress.ExpressApp.Security.SecurityModule()
			Me.securityStrategyComplex1 = New DevExpress.ExpressApp.Security.SecurityStrategyComplex()
			Me.securityStrategyComplex1.SupportNavigationPermissionsForTypes = False
			Me.objectsModule = New DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule()
			Me.cloneObjectModule = New DevExpress.ExpressApp.CloneObject.CloneObjectModule()
			Me.conditionalAppearanceModule = New DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule()
			Me.fileAttachmentsWindowsFormsModule = New DevExpress.ExpressApp.FileAttachments.Win.FileAttachmentsWindowsFormsModule()
			Me.reportsModuleV2 = New DevExpress.ExpressApp.ReportsV2.ReportsModuleV2()
			Me.reportsWindowsFormsModuleV2 = New DevExpress.ExpressApp.ReportsV2.Win.ReportsWindowsFormsModuleV2()
			Me.schedulerModuleBase = New DevExpress.ExpressApp.Scheduler.SchedulerModuleBase()
			Me.schedulerWindowsFormsModule = New DevExpress.ExpressApp.Scheduler.Win.SchedulerWindowsFormsModule()
			Me.scriptRecorderModuleBase = New DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderModuleBase()
			Me.scriptRecorderWindowsFormsModule = New DevExpress.ExpressApp.ScriptRecorder.Win.ScriptRecorderWindowsFormsModule()
			Me.treeListEditorsModuleBase = New DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase()
			Me.treeListEditorsWindowsFormsModule = New DevExpress.ExpressApp.TreeListEditors.Win.TreeListEditorsWindowsFormsModule()
			Me.validationModule = New DevExpress.ExpressApp.Validation.ValidationModule()
			Me.validationWindowsFormsModule = New DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule()
			Me.authenticationStandard1 = New DevExpress.ExpressApp.Security.AuthenticationStandard()
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
			Me.reportsModuleV2.ReportStoreMode = DevExpress.ExpressApp.ReportsV2.ReportStoreModes.XML
			' 
			' NonPersistentObjectsDemoWindowsFormsApplication
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
			Me.Modules.Add(Me.fileAttachmentsWindowsFormsModule)
			Me.Modules.Add(Me.reportsModuleV2)
			Me.Modules.Add(Me.reportsWindowsFormsModuleV2)
			Me.Modules.Add(Me.schedulerModuleBase)
			Me.Modules.Add(Me.schedulerWindowsFormsModule)
			Me.Modules.Add(Me.scriptRecorderModuleBase)
			Me.Modules.Add(Me.scriptRecorderWindowsFormsModule)
			Me.Modules.Add(Me.treeListEditorsModuleBase)
			Me.Modules.Add(Me.treeListEditorsWindowsFormsModule)
			Me.Modules.Add(Me.validationModule)
			Me.Modules.Add(Me.validationWindowsFormsModule)
			Me.UseOldTemplates = False
'INSTANT VB NOTE: The following InitializeComponent event wireup was converted to a 'Handles' clause:
'ORIGINAL LINE: this.DatabaseVersionMismatch += new System.EventHandler<DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs>(this.NonPersistentObjectsDemoWindowsFormsApplication_DatabaseVersionMismatch);
'INSTANT VB NOTE: The following InitializeComponent event wireup was converted to a 'Handles' clause:
'ORIGINAL LINE: this.CustomizeLanguagesList += new System.EventHandler<DevExpress.ExpressApp.CustomizeLanguagesListEventArgs>(this.NonPersistentObjectsDemoWindowsFormsApplication_CustomizeLanguagesList);

			DirectCast(Me, System.ComponentModel.ISupportInitialize).EndInit()

		End Sub

		#End Region

		Private module1 As DevExpress.ExpressApp.SystemModule.SystemModule
		Private module2 As DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule
		Private module3 As NonPersistentObjectsDemo.Module.NonPersistentObjectsDemoModule
		Private module4 As NonPersistentObjectsDemo.Module.Win.NonPersistentObjectsDemoWindowsFormsModule
		Private securityModule1 As DevExpress.ExpressApp.Security.SecurityModule
		Private securityStrategyComplex1 As DevExpress.ExpressApp.Security.SecurityStrategyComplex
		Private authenticationStandard1 As DevExpress.ExpressApp.Security.AuthenticationStandard
		Private objectsModule As DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule
		Private cloneObjectModule As DevExpress.ExpressApp.CloneObject.CloneObjectModule
		Private conditionalAppearanceModule As DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule
		Private fileAttachmentsWindowsFormsModule As DevExpress.ExpressApp.FileAttachments.Win.FileAttachmentsWindowsFormsModule
		Private reportsModuleV2 As DevExpress.ExpressApp.ReportsV2.ReportsModuleV2
		Private reportsWindowsFormsModuleV2 As DevExpress.ExpressApp.ReportsV2.Win.ReportsWindowsFormsModuleV2
		Private schedulerModuleBase As DevExpress.ExpressApp.Scheduler.SchedulerModuleBase
		Private schedulerWindowsFormsModule As DevExpress.ExpressApp.Scheduler.Win.SchedulerWindowsFormsModule
		Private scriptRecorderModuleBase As DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderModuleBase
		Private scriptRecorderWindowsFormsModule As DevExpress.ExpressApp.ScriptRecorder.Win.ScriptRecorderWindowsFormsModule
		Private treeListEditorsModuleBase As DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase
		Private treeListEditorsWindowsFormsModule As DevExpress.ExpressApp.TreeListEditors.Win.TreeListEditorsWindowsFormsModule
		Private validationModule As DevExpress.ExpressApp.Validation.ValidationModule
		Private validationWindowsFormsModule As DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule
	End Class
End Namespace
