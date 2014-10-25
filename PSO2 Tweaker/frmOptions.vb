Imports System.Threading
Imports System.IO
Imports System.Net

Public Class frmOptions

    Dim DPISetting As String

    Private Declare Auto Function ShellExecute Lib "shell32.dll" (ByVal hwnd As IntPtr, ByVal lpOperation As String, ByVal lpFile As String, ByVal lpParameters As String, ByVal lpDirectory As String, ByVal nShowCmd As UInteger) As IntPtr

    Private Sub frmOptions_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        SetLocale()
        Me.CMBStyle.SelectedIndex = -1
    End Sub

    Private Function GetBackupMode(ByRef Key As String) As String
        Dim Value As String = Helper.GetRegKey(Of String)(Key)
        Select Case Value
            Case "Ask"
                Return "Ask every time"
            Case "Always"
                Return "Always backup"
            Case "Never"
                Return "Never backup"
            Case Else
                Return Nothing
        End Select
    End Function

    Private Sub frmOptions_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            If Not String.IsNullOrEmpty(Helper.GetRegKey(Of String)("Color")) Then ColorPickerButton1.SelectedColor = Color.FromArgb(Helper.GetRegKey(Of Integer)("Color"))
            If Not String.IsNullOrEmpty(Helper.GetRegKey(Of String)("FontColor")) Then ColorPickerButton2.SelectedColor = Color.FromArgb(Helper.GetRegKey(Of Integer)("FontColor"))
            If Not String.IsNullOrEmpty(Helper.GetRegKey(Of String)("TextBoxBGColor")) Then ColorPickerButton4.SelectedColor = Color.FromArgb(Helper.GetRegKey(Of Integer)("TextboxBGColor"))
            If Not String.IsNullOrEmpty(Helper.GetRegKey(Of String)("TextBoxColor")) Then ColorPickerButton3.SelectedColor = Color.FromArgb(Helper.GetRegKey(Of Integer)("TextboxColor"))

            Dim BackupMode As String

            BackupMode = GetBackupMode("Backup")

            If Not String.IsNullOrEmpty(BackupMode) Then
                cmbBackupPreference.Text = BackupMode
            End If

            BackupMode = GetBackupMode("PredownloadedRAR")

            If Not String.IsNullOrEmpty(BackupMode) Then
                cmbPredownload.Text = BackupMode
            End If

            ' Checks if the DPI greater or equal to 120, and sets accordingly.
            ' Otherwise, we'll assume is 96 or lower.
            Using g As Graphics = Me.CreateGraphics
                If g.DpiX >= 120 Then
                    Me.Size = New Size(583, 554)
                Else
                    Me.Size = New Size(440, 451)
                End If
            End Using

            ' Here we pull the locale setting from registry and apply it to the form.
            ' Reads locale from registry and converts from LangCode (e.g "en") to Language (e.g "English")
            Dim Locale As Language = [Enum].Parse(GetType(LangCode), Helper.GetRegKey(Of String)("Locale"))

            ' Defaults to English if there is no locale or an error occurs
            If Locale = Nothing Then
                cmbLanguage.Text = "English"
                Thread.CurrentThread.CurrentUICulture = Helper.DefaltCultureInfo
                Thread.CurrentThread.CurrentCulture = Helper.DefaltCultureInfo
            Else
                cmbLanguage.Text = Locale.ToString() 
                Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo(CType(Locale, LangCode).ToString())
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture
            End If

            LabelX1.Text = My.Resources.strChooseATheme
            LabelX2.Text = My.Resources.strChooseALanguage
            LabelX3.Text = My.Resources.strChooseABackgroundImage

            CheckBoxX1.Checked = Helper.GetRegKey(Of Boolean)("Pastebin")
            CheckBoxX2.Checked = Helper.GetRegKey(Of Boolean)("ENPatchAfterInstall")
            CheckBoxX3.Checked = Helper.GetRegKey(Of Boolean)("LargeFilesAfterInstall")
            CheckBoxX4.Checked = Helper.GetRegKey(Of Boolean)("StoryPatchAfterInstall")
            CheckBoxX5.Checked = Helper.GetRegKey(Of Boolean)("SidebarEnabled")

            chkAutoRemoveCensor.Checked = Helper.GetRegKey(Of Boolean)("RemoveCensor")
            CMBStyle.Text = Helper.GetRegKey(Of String)("Style")

            ComboItem33.Text = "Last installed: " & Helper.GetRegKey(Of String)("StoryPatchVersion")
            ComboItem33.Text = "Latest version: " & Helper.GetRegKey(Of String)("NewStoryVersion")
            ComboItem35.Text = "Last installed: " & Helper.GetRegKey(Of String)("ENPatchVersion")
            ComboItem36.Text = "Latest version: " & Helper.GetRegKey(Of String)("NewENVersion")
            ComboItem40.Text = "Last installed: " & Helper.GetRegKey(Of String)("LargeFilesVersion")
            ComboItem42.Text = "Latest version: " & Helper.GetRegKey(Of String)("NewLargeFilesVersion")
        Catch ex As Exception
            frmMain.Log(ex.Message)
            frmMain.WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub CMBStyle_SelectedValueChanged(sender As Object, e As EventArgs) Handles CMBStyle.SelectedValueChanged
        If Not String.IsNullOrEmpty(CMBStyle.Text) Then

            '┻━┻ ︵ \(Ò_Ó \)
            '(╯°□°）╯︵ /(.□. \)
            '┯━┯ノ(º₋ºノ)

            Select Case CMBStyle.Text
                Case "Blue"
                    StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Office2007Blue
                    Helper.SetRegKey(Of String)("Style", CMBStyle.Text)

                Case "Silver"
                    StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Office2007Silver
                    Helper.SetRegKey(Of String)("Style", CMBStyle.Text)

                Case "Black"
                    StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Office2007Black
                    Helper.SetRegKey(Of String)("Style", CMBStyle.Text)

                Case "Vista Glass"
                    StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Office2007VistaGlass
                    Helper.SetRegKey(Of String)("Style", CMBStyle.Text)

                Case "2010 Silver"
                    StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Office2010Silver
                    Helper.SetRegKey(Of String)("Style", CMBStyle.Text)

                Case "Windows 7 Blue"
                    StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Windows7Blue
                    Helper.SetRegKey(Of String)("Style", CMBStyle.Text)

                Case Else
                    StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Office2007Blue
                    Helper.SetRegKey(Of String)("Style", "Blue")
            End Select
        End If
    End Sub

    Private Sub ComboBoxEx1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxEx1.SelectedIndexChanged
        Helper.SetRegKey(Of String)("PatchServer", ComboBoxEx1.Text)
    End Sub

    Private Sub cmbLanguage_SelectedValueChanged(sender As Object, e As EventArgs) Handles cmbLanguage.SelectedValueChanged
        Dim DownloadClient As New WebClient
        DownloadClient.DownloadFile(New Uri("http://162.243.211.123/freedom/LanguagePack.rar"), "LanguagePack.rar")
        Dim process As System.Diagnostics.Process = Nothing
        Dim processStartInfo As System.Diagnostics.ProcessStartInfo
        processStartInfo = New System.Diagnostics.ProcessStartInfo()
        Dim UnRarLocation As String
        UnRarLocation = (Application.StartupPath & "\unrar.exe")
        UnRarLocation = UnRarLocation.Replace("\\", "\")
        processStartInfo.FileName = UnRarLocation
        processStartInfo.Verb = "runas"
        processStartInfo.Arguments = ("x -inul -o+ LanguagePack.rar")
        processStartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
        processStartInfo.UseShellExecute = True
        process = System.Diagnostics.Process.Start(processStartInfo)

        Do Until process.WaitForExit(1000)
        Loop

        SetLocale()
    End Sub

    Private Sub SetLocale()
        Dim SelectedLocale As String = [Enum].GetName(GetType(LangCode), cmbLanguage.SelectedIndex)

        If String.IsNullOrEmpty(SelectedLocale) Then
            Thread.CurrentThread.CurrentUICulture = Helper.DefaltCultureInfo
            Thread.CurrentThread.CurrentCulture = Helper.DefaltCultureInfo
            Helper.SetRegKey(Of String)("Locale", "en")
        Else
            Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo(SelectedLocale)
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture
            Helper.SetRegKey(Of String)("Locale", SelectedLocale)
        End If
    End Sub

    Private Sub ColorPickerButton1_SelectedColorChanged(sender As Object, e As EventArgs) Handles ColorPickerButton1.SelectedColorChanged
        frmMain.StyleManager1.ManagerColorTint = ColorPickerButton1.SelectedColor
        Me.StyleManager1.ManagerColorTint = ColorPickerButton1.SelectedColor
        frmPSO2Options.StyleManager1.ManagerColorTint = ColorPickerButton1.SelectedColor
        Helper.SetRegKey(Of Integer)("Color", (ColorPickerButton1.SelectedColor.ToArgb))
    End Sub

    Private Sub CheckBoxX1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxX1.CheckedChanged
        If CheckBoxX1.Checked = False Then
            MsgBox("PLEASE BE CAUTIOUS - If you turn this function off, the program will not automatically upload your logfile to pastebin, so you can report the bug to AIDA. This means that you'll need to provide the logfile yourself, or the likelyhood of your issue being resolved is very, very, slim.")
        End If

        Helper.SetRegKey(Of String)("Pastebin", CheckBoxX1.Checked.ToString())
    End Sub

    Private Sub ColorPickerButton2_SelectedColorChanged(sender As Object, e As EventArgs) Handles ColorPickerButton2.SelectedColorChanged
        frmMain.ForeColor = ColorPickerButton2.SelectedColor
        frmPSO2Options.ForeColor = ColorPickerButton2.SelectedColor
        frmPSO2Options.TabItem1.TextColor = ColorPickerButton2.SelectedColor
        frmPSO2Options.TabItem2.TextColor = ColorPickerButton2.SelectedColor
        frmPSO2Options.TabItem3.TextColor = ColorPickerButton2.SelectedColor
        Me.ForeColor = ColorPickerButton2.SelectedColor
        CheckBoxX1.TextColor = ColorPickerButton2.SelectedColor
        CheckBoxX2.TextColor = ColorPickerButton2.SelectedColor
        CheckBoxX3.TextColor = ColorPickerButton2.SelectedColor
        CheckBoxX4.TextColor = ColorPickerButton2.SelectedColor
        CheckBoxX5.TextColor = ColorPickerButton2.SelectedColor
        frmMain.chkRemoveCensor.TextColor = ColorPickerButton2.SelectedColor
        frmMain.chkRemoveNVidia.TextColor = ColorPickerButton2.SelectedColor
        frmMain.chkRemovePC.TextColor = ColorPickerButton2.SelectedColor
        frmMain.chkRemoveSEGA.TextColor = ColorPickerButton2.SelectedColor
        frmMain.chkRemoveVita.TextColor = ColorPickerButton2.SelectedColor
        frmMain.chkRestoreCensor.TextColor = ColorPickerButton2.SelectedColor
        frmMain.chkRestoreNVidia.TextColor = ColorPickerButton2.SelectedColor
        frmMain.chkRestorePC.TextColor = ColorPickerButton2.SelectedColor
        frmMain.chkRestoreSEGA.TextColor = ColorPickerButton2.SelectedColor
        frmMain.chkRestoreVita.TextColor = ColorPickerButton2.SelectedColor
        frmMain.chkSwapOP.TextColor = ColorPickerButton2.SelectedColor

        Helper.SetRegKey(Of Integer)("FontColor", (ColorPickerButton2.SelectedColor.ToArgb))
    End Sub

    Private Sub CheckBoxX2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxX2.CheckedChanged
        Helper.SetRegKey(Of String)("ENPatchAfterInstall", CheckBoxX2.Checked.ToString())
    End Sub

    Private Sub CheckBoxX3_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxX3.CheckedChanged
        Helper.SetRegKey(Of String)("LargeFilesAfterInstall", CheckBoxX3.Checked.ToString())
    End Sub

    Private Sub CheckBoxX4_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxX4.CheckedChanged
        Helper.SetRegKey(Of String)("StoryPatchAfterInstall", CheckBoxX4.Checked.ToString())
    End Sub

    Private Sub cmbBackupPreference_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbBackupPreference.SelectedIndexChanged
        Select Case cmbBackupPreference.SelectedIndex
            Case 0
                Helper.SetRegKey(Of String)("Backup", "Ask")

            Case 1
                Helper.SetRegKey(Of String)("Backup", "Always")

            Case 2
                Helper.SetRegKey(Of String)("Backup", "Never")

            Case Else
                Helper.SetRegKey(Of String)("Backup", "Ask")
        End Select
    End Sub

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click
        Process.Start("http://arks-layer.com/credits.php")
    End Sub

    Private Sub UpdateVersion(key As String, str As String)
        Dim value As String = str.Replace("Latest version: ", "").Replace("Last installed: ", "")
        Helper.SetRegKey(Of String)(key, value)
        MsgBox(value)
    End Sub

    Private Sub cmbENOverride_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbENOverride.SelectedIndexChanged
        UpdateVersion("ENPatchVersion", cmbENOverride.Text)
    End Sub

    Private Sub cmbLargeFilesOverride_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbLargeFilesOverride.SelectedIndexChanged
        UpdateVersion("LargeFilesVersion", cmbLargeFilesOverride.Text)
    End Sub

    Private Sub cmbStoryOverride_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbStoryOverride.SelectedIndexChanged
        UpdateVersion("StoryPatchVersion", cmbStoryOverride.Text)
    End Sub

    Private Sub ColorPickerButton4_SelectedColorChanged(sender As Object, e As EventArgs) Handles ColorPickerButton4.SelectedColorChanged
        frmMain.rtbDebug.BackColor = ColorPickerButton4.SelectedColor
        Helper.SetRegKey(Of Integer)("TextboxBGColor", (ColorPickerButton4.SelectedColor.ToArgb))
    End Sub

    Private Sub ColorPickerButton3_SelectedColorChanged(sender As Object, e As EventArgs) Handles ColorPickerButton3.SelectedColorChanged
        frmMain.rtbDebug.ForeColor = ColorPickerButton3.SelectedColor
        Helper.SetRegKey(Of Integer)("TextboxColor", (ColorPickerButton3.SelectedColor.ToArgb))
    End Sub

    Private Sub btnPSO2Override_Click(sender As Object, e As EventArgs) Handles btnPSO2Override.Click
        Dim YesNo As MsgBoxResult = MsgBox("This will tell the Tweaker you have the latest version of PSO2 installed - Be aware that this cannot be undone, and should only be used if you update the game outside of the Tweaker. Do you want to continue?", vbYesNo)

        If YesNo = vbYes Then
            Dim lines3 = File.ReadAllLines("version.ver")
            Dim RemoteVersion3 As String = lines3(0)
            Helper.SetRegKey(Of String)("PSO2RemoteVersion", RemoteVersion3)
            MsgBox("PSO2 Installed version set to: " & RemoteVersion3)
        End If
    End Sub

    Private Sub ButtonX3_Click(sender As Object, e As EventArgs) Handles ButtonX3.Click
        If Not String.IsNullOrWhiteSpace(TextBoxX1.Text) Then
            Dim UIDString As String = TextBoxX1.Text.Replace("steam://rungameid/", "")
            Helper.SetRegKey(Of String)("SteamUID", UIDString)
            MsgBox(UIDString)
        End If
    End Sub

    Private Sub ButtonX5_Click(sender As Object, e As EventArgs) Handles ButtonX5.Click
        Environment.SetEnvironmentVariable("-pso2", "+0x01e3f1e9")
        ShellExecute(Handle, "open", ("steam://rungameID/" & Helper.GetRegKey(Of String)("SteamUID")), " +0x33aca2b9 -pso2", "", 0)
    End Sub

    Private Sub CheckBoxX5_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxX5.CheckedChanged
        Helper.SetRegKey(Of String)("SidebarEnabled", CheckBoxX5.Checked.ToString())
    End Sub

    Private Sub chkAutoRemoveCensor_CheckedChanged(sender As Object, e As EventArgs) Handles chkAutoRemoveCensor.CheckedChanged
        Helper.SetRegKey(Of String)("RemoveCensor", chkAutoRemoveCensor.Checked.ToString())
    End Sub

    Private Sub cmbPredownload_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbPredownload.SelectedIndexChanged
        If cmbPredownload.SelectedIndex = 0 Then
            Helper.SetRegKey(Of String)("PredownloadedRAR", "Ask")
        ElseIf cmbPredownload.SelectedIndex = 1 Then
            Helper.SetRegKey(Of String)("PredownloadedRAR", "Never")
        End If
    End Sub
End Class
