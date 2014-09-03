<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmVEDA
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.rtbStatus = New System.Windows.Forms.RichTextBox()
        Me.btnMD5 = New System.Windows.Forms.Button()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.btnShorten = New System.Windows.Forms.Button()
        Me.btnMerge = New System.Windows.Forms.Button()
        Me.btnListDir = New System.Windows.Forms.Button()
        Me.btnAnyDir = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.txtAqua = New System.Windows.Forms.TextBox()
        Me.btnDLWUA = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'rtbStatus
        '
        Me.rtbStatus.HideSelection = False
        Me.rtbStatus.Location = New System.Drawing.Point(0, 228)
        Me.rtbStatus.Name = "rtbStatus"
        Me.rtbStatus.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None
        Me.rtbStatus.Size = New System.Drawing.Size(408, 171)
        Me.rtbStatus.TabIndex = 0
        Me.rtbStatus.Text = ""
        '
        'btnMD5
        '
        Me.btnMD5.Location = New System.Drawing.Point(25, 23)
        Me.btnMD5.Name = "btnMD5"
        Me.btnMD5.Size = New System.Drawing.Size(114, 48)
        Me.btnMD5.TabIndex = 1
        Me.btnMD5.Text = "Get MD5 Hash"
        Me.btnMD5.UseVisualStyleBackColor = True
        Me.btnMD5.Visible = False
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(379, 0)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(28, 29)
        Me.btnClose.TabIndex = 2
        Me.btnClose.Text = "X"
        Me.btnClose.UseVisualStyleBackColor = True
        Me.btnClose.Visible = False
        '
        'btnShorten
        '
        Me.btnShorten.Location = New System.Drawing.Point(25, 77)
        Me.btnShorten.Name = "btnShorten"
        Me.btnShorten.Size = New System.Drawing.Size(113, 49)
        Me.btnShorten.TabIndex = 3
        Me.btnShorten.Text = "Shorten Patch Lists"
        Me.btnShorten.UseVisualStyleBackColor = True
        Me.btnShorten.Visible = False
        '
        'btnMerge
        '
        Me.btnMerge.Location = New System.Drawing.Point(26, 132)
        Me.btnMerge.Name = "btnMerge"
        Me.btnMerge.Size = New System.Drawing.Size(113, 49)
        Me.btnMerge.TabIndex = 4
        Me.btnMerge.Text = "Merge Patch Lists"
        Me.btnMerge.UseVisualStyleBackColor = True
        Me.btnMerge.Visible = False
        '
        'btnListDir
        '
        Me.btnListDir.Location = New System.Drawing.Point(145, 22)
        Me.btnListDir.Name = "btnListDir"
        Me.btnListDir.Size = New System.Drawing.Size(113, 49)
        Me.btnListDir.TabIndex = 5
        Me.btnListDir.Text = "List files in PSO2 dir"
        Me.btnListDir.UseVisualStyleBackColor = True
        Me.btnListDir.Visible = False
        '
        'btnAnyDir
        '
        Me.btnAnyDir.Location = New System.Drawing.Point(145, 77)
        Me.btnAnyDir.Name = "btnAnyDir"
        Me.btnAnyDir.Size = New System.Drawing.Size(113, 49)
        Me.btnAnyDir.TabIndex = 6
        Me.btnAnyDir.Text = "List files in ANY dir"
        Me.btnAnyDir.UseVisualStyleBackColor = True
        Me.btnAnyDir.Visible = False
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(144, 133)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(114, 48)
        Me.Button1.TabIndex = 7
        Me.Button1.Text = "Make MD5 Hashlist"
        Me.Button1.UseVisualStyleBackColor = True
        Me.Button1.Visible = False
        '
        'txtAqua
        '
        Me.txtAqua.Location = New System.Drawing.Point(23, 191)
        Me.txtAqua.Name = "txtAqua"
        Me.txtAqua.Size = New System.Drawing.Size(373, 20)
        Me.txtAqua.TabIndex = 8
        Me.txtAqua.Visible = False
        '
        'btnDLWUA
        '
        Me.btnDLWUA.Location = New System.Drawing.Point(277, 134)
        Me.btnDLWUA.Name = "btnDLWUA"
        Me.btnDLWUA.Size = New System.Drawing.Size(102, 46)
        Me.btnDLWUA.TabIndex = 9
        Me.btnDLWUA.Text = "DLWUA"
        Me.btnDLWUA.UseVisualStyleBackColor = True
        Me.btnDLWUA.Visible = False
        '
        'frmVEDA
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(408, 394)
        Me.Controls.Add(Me.btnDLWUA)
        Me.Controls.Add(Me.txtAqua)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.btnAnyDir)
        Me.Controls.Add(Me.btnListDir)
        Me.Controls.Add(Me.btnMerge)
        Me.Controls.Add(Me.btnShorten)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.btnMD5)
        Me.Controls.Add(Me.rtbStatus)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmVEDA"
        Me.Text = "[VEDA Control System]"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents rtbStatus As System.Windows.Forms.RichTextBox
    Friend WithEvents btnMD5 As System.Windows.Forms.Button
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents btnShorten As System.Windows.Forms.Button
    Friend WithEvents btnMerge As System.Windows.Forms.Button
    Friend WithEvents btnListDir As System.Windows.Forms.Button
    Friend WithEvents btnAnyDir As System.Windows.Forms.Button
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents txtAqua As System.Windows.Forms.TextBox
    Friend WithEvents btnDLWUA As System.Windows.Forms.Button
End Class
