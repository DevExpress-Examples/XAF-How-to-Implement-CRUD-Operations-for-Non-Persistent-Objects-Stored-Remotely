Namespace NonPersistentObjectsDemo.Module.Win
	Partial Public Class NonPersistentObjectsDemoWindowsFormsModule
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
			' 
			' NonPersistentObjectsDemoWindowsFormsModule
			' 
			Me.RequiredModuleTypes.Add(GetType(NonPersistentObjectsDemo.Module.NonPersistentObjectsDemoModule))
			Me.RequiredModuleTypes.Add(GetType(DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule))
			Me.RequiredModuleTypes.Add(GetType(DevExpress.ExpressApp.FileAttachments.Win.FileAttachmentsWindowsFormsModule))
			Me.RequiredModuleTypes.Add(GetType(DevExpress.ExpressApp.ReportsV2.Win.ReportsWindowsFormsModuleV2))
			Me.RequiredModuleTypes.Add(GetType(DevExpress.ExpressApp.Scheduler.Win.SchedulerWindowsFormsModule))
			Me.RequiredModuleTypes.Add(GetType(DevExpress.ExpressApp.ScriptRecorder.Win.ScriptRecorderWindowsFormsModule))
			Me.RequiredModuleTypes.Add(GetType(DevExpress.ExpressApp.TreeListEditors.Win.TreeListEditorsWindowsFormsModule))
			Me.RequiredModuleTypes.Add(GetType(DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule))
		End Sub

		#End Region
	End Class
End Namespace