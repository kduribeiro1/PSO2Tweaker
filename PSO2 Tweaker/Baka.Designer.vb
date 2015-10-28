<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Baka
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Baka))
        Me.lblFool = New System.Windows.Forms.Label()
        Me.btnOkay = New System.Windows.Forms.Button()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblFool
        '
        Me.lblFool.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblFool.Location = New System.Drawing.Point(12, 295)
        Me.lblFool.Name = "lblFool"
        Me.lblFool.Size = New System.Drawing.Size(621, 136)
        Me.lblFool.TabIndex = 1
        Me.lblFool.Text = resources.GetString("lblFool.Text")
        '
        'btnOkay
        '
        Me.btnOkay.Location = New System.Drawing.Point(195, 432)
        Me.btnOkay.Name = "btnOkay"
        Me.btnOkay.Size = New System.Drawing.Size(229, 40)
        Me.btnOkay.TabIndex = 2
        Me.btnOkay.Text = "OK :("
        Me.btnOkay.UseVisualStyleBackColor = True
        '
        'PictureBox1
        '
        Me.PictureBox1.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.PictureBox1.Location = New System.Drawing.Point(66, 1)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(489, 294)
        Me.PictureBox1.TabIndex = 3
        Me.PictureBox1.TabStop = False
        '
        'Baka
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(634, 474)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.btnOkay)
        Me.Controls.Add(Me.lblFool)
        Me.Name = "Baka"
        Me.Text = "Whyyyyyyyy"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblFool As Label
    Friend WithEvents btnOkay As Button
    Friend WithEvents PictureBox1 As PictureBox
End Class
