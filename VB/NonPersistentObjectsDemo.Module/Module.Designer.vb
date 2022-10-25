Namespace NonPersistentObjectsDemo.[Module]

    Partial Class NonPersistentObjectsDemoModule

        ''' <summary> 
        ''' Required designer variable.
        ''' </summary>
        Private components As System.ComponentModel.IContainer = Nothing

        ''' <summary> 
        ''' Clean up any resources being used.
        ''' </summary>
        ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso (Me.components IsNot Nothing) Then
                Me.components.Dispose()
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
            ' NonPersistentObjectsDemoModule
            ' 
            Me.AdditionalExportedTypes.Add(GetType(DevExpress.Persistent.BaseImpl.ModelDifference))
            Me.AdditionalExportedTypes.Add(GetType(DevExpress.Persistent.BaseImpl.ModelDifferenceAspect))
            Me.AdditionalExportedTypes.Add(GetType(DevExpress.Persistent.BaseImpl.BaseObject))
            'this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.FileData));
            'this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.FileAttachmentBase));
            'this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.Event));
            'this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.Resource));
            'this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.HCategory));
            Me.RequiredModuleTypes.Add(GetType(DevExpress.ExpressApp.SystemModule.SystemModule))
            'this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule));
            Me.RequiredModuleTypes.Add(GetType(DevExpress.ExpressApp.CloneObject.CloneObjectModule))
            Me.RequiredModuleTypes.Add(GetType(DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule))
            Me.RequiredModuleTypes.Add(GetType(DevExpress.ExpressApp.ReportsV2.ReportsModuleV2))
            Me.RequiredModuleTypes.Add(GetType(DevExpress.ExpressApp.Scheduler.SchedulerModuleBase))
            Me.RequiredModuleTypes.Add(GetType(DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderModuleBase))
            Me.RequiredModuleTypes.Add(GetType(DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase))
            Me.RequiredModuleTypes.Add(GetType(DevExpress.ExpressApp.Validation.ValidationModule))
        End Sub
#End Region
    End Class
End Namespace
