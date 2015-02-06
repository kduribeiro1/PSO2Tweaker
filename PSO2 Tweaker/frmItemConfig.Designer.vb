<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmItemConfig
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
        Me.btnSave = New DevComponents.DotNetBar.ButtonX()
        Me.ButtonX7 = New DevComponents.DotNetBar.ButtonX()
        Me.chkLogging = New System.Windows.Forms.CheckBox()
        Me.NUDDelay = New System.Windows.Forms.NumericUpDown()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.cmbToggleKey = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.lblToggle = New System.Windows.Forms.Label()
        Me.lblToggleCopyEN = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        CType(Me.NUDDelay, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnSave
        '
        Me.btnSave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnSave.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground
        Me.btnSave.Location = New System.Drawing.Point(13, 225)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(155, 42)
        Me.btnSave.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnSave.TabIndex = 4
        Me.btnSave.Text = "Save"
        '
        'ButtonX7
        '
        Me.ButtonX7.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.ButtonX7.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground
        Me.ButtonX7.Location = New System.Drawing.Point(190, 225)
        Me.ButtonX7.Name = "ButtonX7"
        Me.ButtonX7.Size = New System.Drawing.Size(155, 42)
        Me.ButtonX7.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.ButtonX7.TabIndex = 5
        Me.ButtonX7.Text = "Reset to defaults"
        '
        'chkLogging
        '
        Me.chkLogging.AutoSize = True
        Me.chkLogging.Location = New System.Drawing.Point(245, 190)
        Me.chkLogging.Name = "chkLogging"
        Me.chkLogging.Size = New System.Drawing.Size(100, 17)
        Me.chkLogging.TabIndex = 3
        Me.chkLogging.Text = "Enable Logging"
        Me.chkLogging.UseVisualStyleBackColor = True
        '
        'NUDDelay
        '
        Me.NUDDelay.Location = New System.Drawing.Point(184, 190)
        Me.NUDDelay.Maximum = New Decimal(New Integer() {30, 0, 0, 0})
        Me.NUDDelay.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.NUDDelay.Name = "NUDDelay"
        Me.NUDDelay.Size = New System.Drawing.Size(52, 20)
        Me.NUDDelay.TabIndex = 2
        Me.NUDDelay.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(10, 186)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(173, 36)
        Me.Label1.TabIndex = 12
        Me.Label1.Text = "Find/Replace delay:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(1 is the fastest, 30 is the slowest)" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'Label3
        '
        Me.Label3.Location = New System.Drawing.Point(12, 9)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(173, 40)
        Me.Label3.TabIndex = 16
        Me.Label3.Text = "Select the second key to press, which will toggle the item patch ON/OFF:"
        '
        'cmbToggleKey
        '
        Me.cmbToggleKey.FormattingEnabled = True
        Me.cmbToggleKey.Items.AddRange(New Object() {"F1   (112)", "F2   (113)", "F3   (114)", "F4   (115)", "F5   (116)", "F6   (117)", "F7   (118)", "F8   (119)", "F9   (120)", "F10   (121)", "F11   (122)", "F12   (123)", "0   (48)", "1   (49)", "2   (50)", "3   (51)", "4   (52)", "5   (53)", "6   (54)", "7   (55)", "8   (56)", "9   (57)", "A   (65)", "B   (66)", "D   (68)", "E   (69)", "F   (70)", "G   (71)", "H   (72)", "I   (73)", "J   (74)", "K   (75)", "L   (76)", "M   (77)", "N   (78)", "O   (79)", "P   (80)", "Q   (81)", "R   (82)", "S   (83)", "T   (84)", "U   (85)", "W   (87)", "X   (88)", "Y   (89)", "Z   (90)", "Numpad 0   (96)", "Numpad 1   (97)", "Numpad 2   (98)", "Numpad 3   (99)", "Numpad 4   (100)", "Numpad 5   (101)", "Numpad 6   (102)", "Numpad 7   (103)", "Numpad 8   (104)", "Numpad 9   (105)", "Numpad Multiply   (106)", "Numpad Add   (107)", "Numpad Subtract   (109)", "Numpad Decimal Point   (110)", "Numpad Divide   (111)", "Page Up   (33)", "Page Down   (34)", "End   (35)", "Home   (36)", "Left Arrow   (37)", "Up Arrow   (38)", "Right Arrow   (39)", "Down Arrow   (40)", "Insert   (45)", "Delete   (46)", "Semi-Colon   (186)", "Equal Sign   (187)", "Comma   (188)", "Dash   (189)", "Period   (190)", "Forward Slash   (191)", "Grave Accent   (192)", "Open Bracket   (219)", "Back Slash   (220)", "Close Bracket   (221)", "Single Quote   (222)"})
        Me.cmbToggleKey.Location = New System.Drawing.Point(15, 52)
        Me.cmbToggleKey.Name = "cmbToggleKey"
        Me.cmbToggleKey.Size = New System.Drawing.Size(131, 21)
        Me.cmbToggleKey.TabIndex = 1
        Me.cmbToggleKey.Text = "Please choose a key"
        '
        'Label4
        '
        Me.Label4.Location = New System.Drawing.Point(10, 90)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(161, 14)
        Me.Label4.TabIndex = 21
        Me.Label4.Text = "Key Bindings:"
        '
        'lblToggle
        '
        Me.lblToggle.Location = New System.Drawing.Point(10, 120)
        Me.lblToggle.Name = "lblToggle"
        Me.lblToggle.Size = New System.Drawing.Size(311, 14)
        Me.lblToggle.TabIndex = 22
        Me.lblToggle.Text = "Toggle item patch ON/OFF:"
        '
        'lblToggleCopyEN
        '
        Me.lblToggleCopyEN.Location = New System.Drawing.Point(12, 150)
        Me.lblToggleCopyEN.Name = "lblToggleCopyEN"
        Me.lblToggleCopyEN.Size = New System.Drawing.Size(309, 14)
        Me.lblToggleCopyEN.TabIndex = 23
        Me.lblToggleCopyEN.Text = "Copy EN name of item: Control + Shift + C"
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(181, 9)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(173, 40)
        Me.Label2.TabIndex = 24
        Me.Label2.Text = "You can see the prices of items in your shop by holding control when selecting ""C" & _
    "heck prices""."
        '
        'frmItemConfig
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(357, 279)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.lblToggleCopyEN)
        Me.Controls.Add(Me.lblToggle)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.cmbToggleKey)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.NUDDelay)
        Me.Controls.Add(Me.chkLogging)
        Me.Controls.Add(Me.ButtonX7)
        Me.Controls.Add(Me.btnSave)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.KeyPreview = True
        Me.Name = "frmItemConfig"
        Me.Text = "Configure Item Translation"
        CType(Me.NUDDelay, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnSave As DevComponents.DotNetBar.ButtonX
    Friend WithEvents ButtonX7 As DevComponents.DotNetBar.ButtonX
    Friend WithEvents chkLogging As System.Windows.Forms.CheckBox
    Friend WithEvents NUDDelay As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents cmbToggleKey As System.Windows.Forms.ComboBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents lblToggle As System.Windows.Forms.Label
    Friend WithEvents lblToggleCopyEN As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
End Class
