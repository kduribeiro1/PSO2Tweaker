<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmPlugins
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.ListViewEx1 = New DevComponents.DotNetBar.Controls.ListViewEx()
        Me.btnSave = New DevComponents.DotNetBar.ButtonX()
        Me.lblPluginInfo = New DevComponents.DotNetBar.LabelX()
        Me.SuspendLayout()
        '
        'ListViewEx1
        '
        '
        '
        '
        Me.ListViewEx1.Border.Class = "ListViewBorder"
        Me.ListViewEx1.CheckBoxes = True
        Me.ListViewEx1.Location = New System.Drawing.Point(0, 0)
        Me.ListViewEx1.Name = "ListViewEx1"
        Me.ListViewEx1.Size = New System.Drawing.Size(424, 210)
        Me.ListViewEx1.TabIndex = 0
        Me.ListViewEx1.UseCompatibleStateImageBehavior = False
        Me.ListViewEx1.View = System.Windows.Forms.View.List
        '
        'btnSave
        '
        Me.btnSave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnSave.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground
        Me.btnSave.Location = New System.Drawing.Point(129, 328)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(155, 42)
        Me.btnSave.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnSave.TabIndex = 5
        Me.btnSave.Text = "Save"
        '
        'lblPluginInfo
        '
        '
        '
        '
        Me.lblPluginInfo.BackgroundStyle.Class = ""
        Me.lblPluginInfo.Location = New System.Drawing.Point(3, 216)
        Me.lblPluginInfo.Name = "lblPluginInfo"
        Me.lblPluginInfo.Size = New System.Drawing.Size(418, 112)
        Me.lblPluginInfo.TabIndex = 6
        Me.lblPluginInfo.Text = "Please click on a plugin for more information."
        Me.lblPluginInfo.TextLineAlignment = System.Drawing.StringAlignment.Near
        Me.lblPluginInfo.WordWrap = True
        '
        'frmPlugins
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(424, 382)
        Me.Controls.Add(Me.ListViewEx1)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.lblPluginInfo)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "frmPlugins"
        Me.Text = "Plugin Manager (BETA)"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents ListViewEx1 As DevComponents.DotNetBar.Controls.ListViewEx
    Friend WithEvents btnSave As DevComponents.DotNetBar.ButtonX
    Friend WithEvents lblPluginInfo As DevComponents.DotNetBar.LabelX
End Class
