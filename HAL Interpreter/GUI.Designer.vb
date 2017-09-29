<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class GUI
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(GUI))
        Me.textbox = New System.Windows.Forms.RichTextBox()
        Me.BtnOpen = New System.Windows.Forms.Button()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.BtnSave = New System.Windows.Forms.Button()
        Me.BtnRun = New System.Windows.Forms.Button()
        Me.BtnSaveAs = New System.Windows.Forms.Button()
        Me.BtnManual = New System.Windows.Forms.Button()
        Me.BtnHelp = New System.Windows.Forms.Button()
        Me.BtnNew = New System.Windows.Forms.Button()
        Me.cbStrictSyntax = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'textbox
        '
        Me.textbox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.textbox.BackColor = System.Drawing.SystemColors.Window
        Me.textbox.Location = New System.Drawing.Point(31, 59)
        Me.textbox.Name = "textbox"
        Me.textbox.Size = New System.Drawing.Size(502, 185)
        Me.textbox.TabIndex = 1
        Me.textbox.Text = ""
        '
        'BtnOpen
        '
        Me.BtnOpen.BackColor = System.Drawing.SystemColors.Control
        Me.BtnOpen.Location = New System.Drawing.Point(132, 12)
        Me.BtnOpen.Name = "BtnOpen"
        Me.BtnOpen.Size = New System.Drawing.Size(75, 23)
        Me.BtnOpen.TabIndex = 2
        Me.BtnOpen.Text = "Open"
        Me.BtnOpen.UseVisualStyleBackColor = False
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'BtnSave
        '
        Me.BtnSave.BackColor = System.Drawing.SystemColors.Control
        Me.BtnSave.Location = New System.Drawing.Point(232, 12)
        Me.BtnSave.Name = "BtnSave"
        Me.BtnSave.Size = New System.Drawing.Size(75, 23)
        Me.BtnSave.TabIndex = 3
        Me.BtnSave.Text = "Save"
        Me.BtnSave.UseVisualStyleBackColor = False
        '
        'BtnRun
        '
        Me.BtnRun.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnRun.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnRun.BackColor = System.Drawing.SystemColors.ControlLight
        Me.BtnRun.Location = New System.Drawing.Point(616, 195)
        Me.BtnRun.Name = "BtnRun"
        Me.BtnRun.Size = New System.Drawing.Size(87, 35)
        Me.BtnRun.TabIndex = 4
        Me.BtnRun.Text = "Run"
        Me.BtnRun.UseVisualStyleBackColor = False
        '
        'BtnSaveAs
        '
        Me.BtnSaveAs.BackColor = System.Drawing.SystemColors.Control
        Me.BtnSaveAs.Location = New System.Drawing.Point(331, 12)
        Me.BtnSaveAs.Name = "BtnSaveAs"
        Me.BtnSaveAs.Size = New System.Drawing.Size(75, 23)
        Me.BtnSaveAs.TabIndex = 5
        Me.BtnSaveAs.Text = "Save As"
        Me.BtnSaveAs.UseVisualStyleBackColor = False
        '
        'BtnManual
        '
        Me.BtnManual.Location = New System.Drawing.Point(435, 12)
        Me.BtnManual.Name = "BtnManual"
        Me.BtnManual.Size = New System.Drawing.Size(75, 23)
        Me.BtnManual.TabIndex = 6
        Me.BtnManual.Text = "Manual"
        Me.BtnManual.UseVisualStyleBackColor = True
        '
        'BtnHelp
        '
        Me.BtnHelp.Location = New System.Drawing.Point(532, 12)
        Me.BtnHelp.Name = "BtnHelp"
        Me.BtnHelp.Size = New System.Drawing.Size(75, 23)
        Me.BtnHelp.TabIndex = 7
        Me.BtnHelp.Text = "Help"
        Me.BtnHelp.UseVisualStyleBackColor = True
        '
        'BtnNew
        '
        Me.BtnNew.Location = New System.Drawing.Point(31, 12)
        Me.BtnNew.Name = "BtnNew"
        Me.BtnNew.Size = New System.Drawing.Size(75, 23)
        Me.BtnNew.TabIndex = 8
        Me.BtnNew.Text = "New"
        Me.BtnNew.UseVisualStyleBackColor = True
        '
        'cbStrictSyntax
        '
        Me.cbStrictSyntax.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cbStrictSyntax.AutoSize = True
        Me.cbStrictSyntax.Location = New System.Drawing.Point(591, 86)
        Me.cbStrictSyntax.Name = "cbStrictSyntax"
        Me.cbStrictSyntax.Size = New System.Drawing.Size(133, 17)
        Me.cbStrictSyntax.TabIndex = 9
        Me.cbStrictSyntax.Text = "Strict Syntax Checking"
        Me.cbStrictSyntax.UseVisualStyleBackColor = True
        '
        'GUI
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ClientSize = New System.Drawing.Size(751, 302)
        Me.Controls.Add(Me.cbStrictSyntax)
        Me.Controls.Add(Me.BtnNew)
        Me.Controls.Add(Me.BtnHelp)
        Me.Controls.Add(Me.BtnManual)
        Me.Controls.Add(Me.BtnSaveAs)
        Me.Controls.Add(Me.BtnRun)
        Me.Controls.Add(Me.BtnSave)
        Me.Controls.Add(Me.BtnOpen)
        Me.Controls.Add(Me.textbox)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "GUI"
        Me.Text = "HAL/S Interpreter"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents textbox As System.Windows.Forms.RichTextBox
    Friend WithEvents BtnOpen As System.Windows.Forms.Button
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents BtnSave As System.Windows.Forms.Button
    Friend WithEvents BtnRun As System.Windows.Forms.Button
    Friend WithEvents BtnSaveAs As System.Windows.Forms.Button
    Friend WithEvents BtnManual As System.Windows.Forms.Button
    Friend WithEvents BtnHelp As System.Windows.Forms.Button
    Friend WithEvents BtnNew As System.Windows.Forms.Button
    Friend WithEvents cbStrictSyntax As System.Windows.Forms.CheckBox
End Class
