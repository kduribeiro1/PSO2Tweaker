Imports DevComponents.DotNetBar
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.Win32
Imports System.ComponentModel
Imports System.IO
Imports System.Net
Imports System.Runtime.Serialization.Json
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Xml
Imports PSO2_Tweaker.My

' TODO: Replace all redundant code with functions
' TODO: Every instance of file downloading that retries ~5 times should be a function. I didn't realize there were so many.

Public Class FrmMain
    Const EnglishPatch = "English Patch"
    Const StoryPatch = "Story Patch"
    Const LargeFiles = "Large Files"

    ReadOnly _pso2OptionsFrm As FrmPso2Options
    ReadOnly _optionsFrm As FrmOptions
    ReadOnly _myDocuments As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)

    Dim _cancelled As Boolean
    Dim _cancelledFull As Boolean
    Dim _dpiSetting As Single
    Dim _itemDownloadingDone As Boolean
    Dim _mileyCyrus As Integer
    Dim _restartplz As Boolean
    Dim _systemUnlock As Integer
    Dim _vedaUnlocked As Boolean = False
    Public SkipDialogs As Boolean = False
    Dim _totalsize2 As Long

    Sub New()
        Try
            _pso2OptionsFrm = New FrmPso2Options()
            _optionsFrm = New FrmOptions()

            InitializeComponent()

            Program.MainForm = Me
            'Yo, fuck this shit. Shit is mad whack, yo.
            SuspendLayout()
            Text = ("PSO2 Tweaker ver " & Application.Info.Version.ToString())
            chkRemoveCensor.Text = Resources.strRemoveCensorFile
            chkRemovePC.Text = Resources.strRemovePCOpening
            chkRemoveVita.Text = Resources.strRemoveVitaOpening
            chkRemoveNVidia.Text = Resources.strRemoveNVidiaVideo
            chkRemoveSEGA.Text = Resources.strRemoveSEGALogoVideo
            chkSwapOP.Text = Resources.strSwapPCVitaOpenings
            chkRestoreCensor.Text = Resources.strRestoreCensorFile
            chkRestorePC.Text = Resources.strRestorePCOpeningVideo
            chkRestoreVita.Text = Resources.strRestoreVitaOpeningVideo
            chkRestoreNVidia.Text = Resources.strRestoreNVidiaLogo
            chkRestoreSEGA.Text = Resources.strRestoreSEGALogoVideo
            lblDirectoryLabel.Text = Resources.strCurrentlyselecteddirectory
            lblStatus.Text = Resources.strWaitingforacommand
            btnSelectPSODir.Text = Resources.strSelectPSO2win32folder
            ButtonInstall.Text = Resources.strInstallUpdatePatches
            btnRestoreBackups.Text = "Restore Backup of JP Files"
            btnApplyChanges.Text = Resources.strApplySelectedChanges
            CancelDownloadToolStripMenuItem.Text = Resources.strCancelCurrentDownload
            BtnUpdatePso2.Text = Resources.strCheckforPSO2Updates
            btnLaunchPSO2.Text = Resources.strLaunchPSO2
            btnFixPSO2EXEs.Text = Resources.strFixPSO2EXEs
            btnFixPermissions.Text = Resources.strFixPSO2Permissions
            lblORBLabel.Text = Resources.strClickOrb
            rtbDebug.Text = Resources.strProgramStarted
            ButtonItem1.Text = "Redownload Original JP Files"
            ButtonItem2.Text = Resources.strTroubleshooting
            btnOtherStuff.Text = Resources.strOtherTasks
            ButtonItem3.Text = Resources.strWebLinks
            btnCheckForStoryUpdates.Text = Resources.strCheckForStoryPatchUpdates
            chkAlwaysOnTop.Text = Resources.strAlwaysonTop
            chkItemTranslation.Text = Resources.strTranslateItems
            btnPSO2Options.Text = Resources.strPSO2Options
            btnOptions.Text = Resources.strOptions
            btnExit.Text = Resources.strExit
            btnAnalyze.Text = Resources.strAnalyzeInstallforIssues
            Button2.Text = Resources.strCheckforDeletedMissingFiles
            ButtonItem10.Text = Resources.strCheckForOldMissingFiles
            btnGameguard.Text = Resources.strFixGameguardErrors
            ButtonItem17.Text = Resources.strResetPSO2Settings
            btnResumePatching.Text = Resources.strResumePatching
            btnTerminate.Text = Resources.strTerminate
            ButtonItem7.Text = Resources.strLaunchChrome

            Helper.Log("Loading color stuff...")

            Select Case RegKey.GetValue(Of String)(RegKey.Style)
                Case "Blue"
                    StyleManager.Style = eStyle.Office2007Blue

                Case "Silver"
                    StyleManager.Style = eStyle.Office2007Silver

                Case "Black"
                    StyleManager.Style = eStyle.Office2007Black

                Case "Vista Glass"
                    StyleManager.Style = eStyle.Office2007VistaGlass

                Case "2010 Silver"
                    StyleManager.Style = eStyle.Office2010Silver

                Case "Windows 7 Blue"
                    StyleManager.Style = eStyle.Windows7Blue

                Case Else
                    StyleManager.Style = eStyle.Office2007Black
            End Select



            Dim regValue As Integer

            regValue = RegKey.GetValue(Of Integer)(RegKey.TextBoxBgColor)
            If regValue = 0 Then RegKey.SetValue(Of UInteger)(RegKey.TextBoxBgColor, 4294967295)
            If regValue <> 0 Then rtbDebug.BackColor = Color.FromArgb(Convert.ToInt32(regValue))

            regValue = RegKey.GetValue(Of Integer)(RegKey.TextBoxColor)
            If regValue <> 0 Then rtbDebug.ForeColor = Color.FromArgb(Convert.ToInt32(regValue))

            regValue = RegKey.GetValue(Of Integer)(RegKey.Color)
            If regValue <> 0 Then StyleManager.ColorTint = Color.FromArgb(Convert.ToInt32(regValue))

            regValue = RegKey.GetValue(Of Integer)(RegKey.FontColor)
            If regValue <> 0 Then
                Dim color As Color = color.FromArgb(Convert.ToInt32(regValue))
                _pso2OptionsFrm.TabControl1.ColorScheme.TabItemSelectedText = color
                _pso2OptionsFrm.TabItem1.TextColor = color
                _pso2OptionsFrm.TabItem2.TextColor = color
                _pso2OptionsFrm.TabItem3.TextColor = color
                _pso2OptionsFrm.TabItem4.TextColor = color
                _pso2OptionsFrm.TabItem7.TextColor = color
                _pso2OptionsFrm.Slider1.TextColor = color
                _pso2OptionsFrm.SBGM.TextColor = color
                _pso2OptionsFrm.CheckBoxX1.TextColor = color
                ForeColor = color
                _optionsFrm.ForeColor = color
                _pso2OptionsFrm.ForeColor = color
                _optionsFrm.CheckBoxX1.TextColor = color
                _optionsFrm.chkAutoRemoveCensor.TextColor = color
                chkRemoveCensor.TextColor = color
                chkRemoveNVidia.TextColor = color
                chkRemovePC.TextColor = color
                chkRemoveSEGA.TextColor = color
                chkRemoveVita.TextColor = color
                chkRestoreCensor.TextColor = color
                chkRestoreNVidia.TextColor = color
                chkRestorePC.TextColor = color
                chkRestoreSEGA.TextColor = color
                chkRestoreVita.TextColor = color
                chkSwapOP.TextColor = color
            End If
        Catch ex As Exception
            Helper.Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            Helper.WriteDebugInfo(Resources.strERROR & ex.Message)
        Finally
            ResumeLayout(False)
        End Try
    End Sub

    Private Shared Sub frmMain_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        Windows.Forms.Application.Exit()
    End Sub

    Private Sub frmMain_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.Shift Then
            If e.KeyCode = Keys.G Then
                _systemUnlock = 1
                lblStatus.Text = "Please enter the remaining commands to access Veda: *"
            End If
            If e.KeyCode = Keys.U AndAlso _systemUnlock = 1 Then
                _systemUnlock = 2
                lblStatus.Text = "Please enter the remaining commands to access Veda: **"
            End If
            If e.KeyCode = Keys.N AndAlso _systemUnlock = 2 Then
                _systemUnlock = 3
                lblStatus.Text = "Please enter the remaining commands to access Veda: ***"
            End If
            If e.KeyCode = Keys.D AndAlso _systemUnlock = 3 Then
                _systemUnlock = 4
                lblStatus.Text = "Please enter the remaining commands to access Veda: ****"
            End If
            If e.KeyCode = Keys.A AndAlso _systemUnlock = 4 Then
                _systemUnlock = 5
                lblStatus.Text = "Please enter the remaining commands to access Veda: *****"
            End If
            If e.KeyCode = Keys.M AndAlso _systemUnlock = 5 Then
                _systemUnlock = 6
                lblStatus.Text = "Please enter the remaining commands to access Veda: ******"
                Application.DoEvents()
                Thread.Sleep(2000)
                lblStatus.Text = "[ACCESS GRANTED - SYSTEM UNLOCKED]"
                Application.DoEvents()
                Thread.Sleep(2000)
                _vedaUnlocked = True
                FrmVeda.Show()
            End If
            If e.KeyCode = Keys.M Then
                _mileyCyrus = 1
                lblStatus.Text = "Please enter the remaining commands: *"
            End If
            If e.KeyCode = Keys.I AndAlso _mileyCyrus = 1 Then
                _mileyCyrus = 2
                lblStatus.Text = "Please enter the remaining commands: **"
            End If
            If e.KeyCode = Keys.L AndAlso _mileyCyrus = 2 Then
                _mileyCyrus = 3
                lblStatus.Text = "Please enter the remaining commands: ***"
            End If
            If e.KeyCode = Keys.E AndAlso _mileyCyrus = 3 Then
                _mileyCyrus = 4
                lblStatus.Text = "Please enter the remaining commands: ****"
            End If
            If e.KeyCode = Keys.Y AndAlso _mileyCyrus = 4 Then
                _mileyCyrus = 5
                lblStatus.Text = "Please enter the remaining commands: *****"
                Application.DoEvents()
                Thread.Sleep(2000)
                lblStatus.Text = "[ACCESS GRANTED - PSO2 TWERKER]"
                Application.DoEvents()
                Thread.Sleep(2000)
                Text = ("PSO2 Twerker ver " & Application.Info.Version.ToString())
                btnLaunchPSO2.Text = "Twerk it!"
                chkItemTranslation.Text = "Twerk on Robin Thicke"
            End If
        End If
    End Sub

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Using g As Graphics = CreateGraphics()
            If g.DpiX = 120 OrElse g.DpiX = 96 Then
                _dpiSetting = g.DpiX
            End If
        End Using

        Try
            If Program.SidebarEnabled Then
                OpenSideBar()
            Else
                ToggleSideBar()
            End If

            While Not Program.IsPso2Installed
                InstallPso2()
                Program.Main()
            End While

            lblDirectory.Text = Program.Pso2RootDir
            PBMainBar.Text = ""
            TopMost = Program.IsMainFormTopMost
            chkAlwaysOnTop.Checked = Program.IsMainFormTopMost
            _cancelledFull = False
            If RegKey.GetValue(Of String)(RegKey.ImageLocation) <> "" Then
                If File.Exists(RegKey.GetValue(Of String)(RegKey.ImageLocation)) Then Me.BackgroundImage = System.Drawing.Image.FromFile(OpenFileDialog1.FileName)
            End If
            Show()

            Helper.WriteDebugInfoAndOk((Resources.strProgramOpeningSuccessfully & Application.Info.Version.ToString()))
        Catch ex As Exception
            Helper.Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            Helper.WriteDebugInfo(Resources.strERROR & ex.Message)
        End Try

        Try
            CheckForTweakerUpdates()
        Catch ex As Exception
            Helper.Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            Helper.WriteDebugInfo(Resources.strERROR & ex.Message)
        End Try

        Try
            Application.DoEvents()

            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.Pso2Dir)) Then
                MsgBox(Resources.strPleaseSelectPSO2win32dir)
                Helper.SelectPso2Directory()
            Else
                Program.Pso2RootDir = RegKey.GetValue(Of String)(RegKey.Pso2Dir)
                lblDirectory.Text = Program.Pso2RootDir
            End If

            If File.Exists((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) AndAlso File.Exists((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585")) Then
                If Helper.GetFileSize((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) = 167479840 AndAlso Helper.GetFileSize((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585")) = 151540352 Then
                    chkSwapOP.Text = Resources.strSwapPCVitaOpenings & " (" & Resources.strNotSwapped & ")"
                ElseIf Helper.GetFileSize((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) = 151540352 AndAlso Helper.GetFileSize((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585")) = 167479840 Then
                    chkSwapOP.Text = Resources.strSwapPCVitaOpenings & " (" & Resources.strSwapped & ")"
                End If
            End If

            ' Shouldn't be doing this in this way
            Application.DoEvents()

            If Not File.Exists("7za.exe") Then
                Helper.WriteDebugInfo(Resources.strDownloading & "7za.exe...")
                Application.DoEvents()
                DownloadFile(Program.FreedomUrl & "7za.exe", "7za.exe")
            End If

            For index = 1 To 5
                If Helper.GetMd5("7za.exe") <> "42BADC1D2F03A8B1E4875740D3D49336" Then
                    Helper.WriteDebugInfo(Resources.strYour7zipiscorrupt)
                    Application.DoEvents()
                    DownloadFile(Program.FreedomUrl & "7za.exe", "7za.exe")
                Else
                    Exit For
                End If
            Next

            If Not File.Exists("UnRar.exe") Then
                Helper.WriteDebugInfo(Resources.strDownloading & "UnRar.exe...")
                Application.DoEvents()
                DownloadFile(Program.FreedomUrl & "UnRAR.exe", "UnRAR.exe")
            End If

            For index = 1 To 5
                If Helper.GetMd5("UnRar.exe") <> "0C83C1293723A682577E3D0B21562B79" Then
                    Helper.WriteDebugInfo(Resources.strYourUnrariscorrupt)
                    Application.DoEvents()
                    DownloadFile(Program.FreedomUrl & "UnRAR.exe", "UnRAR.exe")
                Else
                    Exit For
                End If
            Next

            Helper.DeleteDirectory("TEMPSTORYAIDAFOOL")
            Helper.DeleteFile("launcherlist.txt")
            Helper.DeleteFile("patchlist.txt")
            Helper.DeleteFile("patchlist_old.txt")

            'Added in precede files. Stupid ass SEGA.
            Helper.DeleteFile("patchlist0.txt")
            Helper.DeleteFile("patchlist1.txt")
            Helper.DeleteFile("patchlist2.txt")
            Helper.DeleteFile("patchlist3.txt")
            Helper.DeleteFile("patchlist4.txt")
            Helper.DeleteFile("patchlist5.txt")
            Helper.DeleteFile("precede.txt")
            Helper.DeleteFile("ServerConfig.txt")
            Helper.DeleteFile("precede_apply.txt")
            Helper.DeleteFile("version.ver")
            Helper.DeleteFile("Story MD5HashList.txt")

            UnlockGui()
            btnLaunchPSO2.Enabled = False

            If File.Exists("resume.txt") Then
                Dim yesNoResume As MsgBoxResult = MsgBox("It seems that the last patching attempt was interrupted. Would you like to resume patching?", vbYesNo)
                If yesNoResume = MsgBoxResult.Yes Then
                    ResumePatching()
                Else
                    Helper.DeleteFile("resume.txt")
                End If
            End If

            Helper.WriteDebugInfo(Resources.strCheckingforPSO2Updates)
            Application.DoEvents()

            CheckForPso2Updates(False)
            Helper.WriteDebugInfoSameLine(Resources.strDone)
            Application.DoEvents()

            'Check for PSO2 Updates here, download the version.ver thingie
            'Check for PSO2 EN Patch updates here, parse the URL and see if it's different from the saved one
            'Check for EN Story Patch
            Helper.WriteDebugInfo(Resources.strCheckingforUpdatestopatches)

            'Check for English Patches (Done! :D)
            CheckForEnPatchUpdates()
            Helper.WriteDebugInfo(Resources.strCurrentENPatchis & RegKey.GetValue(Of String)(RegKey.EnPatchVersion))
            Application.DoEvents()

            'Check for LargeFiles Update (Work-In-Progress!)
            CheckForLargeFilesUpdates()
            Helper.WriteDebugInfo(Resources.strCurrentLargeFilesis & RegKey.GetValue(Of String)(RegKey.LargeFilesVersion))
            Application.DoEvents()

            'Check for Story Patches (Done! :D)
            Application.DoEvents()
            CheckForStoryUpdates()
            Helper.WriteDebugInfo(Resources.strCurrentStoryPatchis & RegKey.GetValue(Of String)(RegKey.StoryPatchVersion))
            Application.DoEvents()


            '            Helper.WriteDebugInfo(Resources.strIfAboveVersions)

            If Program.WayuIsAFailure Then
                Helper.WriteDebugInfo("Skipping downloads for Wayu!")
            Else
                If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.UseItemTranslation)) Then
                    RegKey.SetValue(Of Boolean)(RegKey.UseItemTranslation, True)
                End If

                Program.UseItemTranslation = Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.UseItemTranslation))

                If Program.UseItemTranslation Then
                    chkItemTranslation.Checked = True
                    Helper.WriteDebugInfo("Downloading latest item patch files...")
                    _itemDownloadingDone = False
                    ThreadPool.QueueUserWorkItem(AddressOf DownloadItemTranslationFiles, Nothing)

                    Do Until _itemDownloadingDone
                        Application.DoEvents()
                        Thread.Sleep(16)
                    Loop
                End If

                If Not Dns.GetHostEntry("gs001.pso2gs.net").AddressList(0).ToString().Contains("210.189.") AndAlso Not _itemDownloadingDone Then
                    Helper.WriteDebugInfo("PSO2Proxy usage detected! Downloading latest proxy file...")
                    _itemDownloadingDone = False
                    ThreadPool.QueueUserWorkItem(AddressOf DownloadItemTranslationFiles, Nothing)

                    Do Until _itemDownloadingDone
                        Application.DoEvents()
                        Thread.Sleep(16)
                    Loop

                    If Not File.Exists(Program.Pso2RootDir & "\translation.cfg") Then
                        File.WriteAllText(Program.Pso2RootDir & "\translation.cfg", "TranslationPath:translation.bin")
                    End If

                    Dim builtFile As New List(Of String)

                    For Each line In Helper.GetLines(Program.Pso2RootDir & "\translation.cfg")
                        If line.Contains("TranslationPath:") Then line = "TranslationPath:"
                        builtFile.Add(line)
                    Next

                    File.WriteAllLines(Program.Pso2RootDir & "\translation.cfg", builtFile.ToArray())
                End If
            End If

            Helper.WriteDebugInfoSameLine(Resources.strDone)
        Catch ex As Exception
            Helper.Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            Helper.WriteDebugInfo(Resources.strERROR & ex.Message)
        End Try

        Helper.DeleteFile("Story MD5HashList.txt")
        Helper.DeleteFile("PSO2 Tweaker Updater.exe")
        Helper.WriteDebugInfo(Resources.strAllDoneSystemReady)
        btnLaunchPSO2.Enabled = True
    End Sub

    Private Sub CheckForTweakerUpdates()
        Helper.WriteDebugInfo(Resources.strCheckingforupdatesPleasewaitamoment)
        Dim source As String = Program.Client.DownloadString(Program.FreedomUrl & "version.xml")

        If Not String.IsNullOrEmpty(source) AndAlso source.Contains("<VersionHistory>") Then
            Dim xm As New XmlDocument
            xm.LoadXml(source)

            Dim xmlNode = xm.SelectSingleNode("//CurrentVersion")
            Dim currentVersion As String = xmlNode.ChildNodes(0).InnerText.Trim

            Helper.Log("Checking for the latest version of the program...")

            If Application.Info.Version.ToString() = currentVersion Then
                Helper.WriteDebugInfo((Resources.strYouhavethelatestversionoftheprogram & Application.Info.Version.ToString()))
            Else
                Dim changelogtotal As String = ""

                For index As Integer = 2 To 11
                    Dim innerText = xmlNode.ChildNodes(index).InnerText.Trim
                    If Not String.IsNullOrEmpty(innerText) Then changelogtotal &= vbCrLf & innerText
                Next

                Dim updateyesno As MsgBoxResult = MsgBox(Resources.strYouareusinganoutdatedversionoftheprogram & Application.Info.Version.ToString() & Resources.strAndthelatestis & currentVersion & Resources.strWouldyouliketodownloadthenewversion & vbCrLf & vbCrLf & Resources.strChanges & vbCrLf & changelogtotal, MsgBoxStyle.YesNo)
                If updateyesno = MsgBoxResult.Yes Then
                    Helper.WriteDebugInfo(Resources.strDownloadingUpdate)
                    DownloadFile(Program.FreedomUrl & "PSO2%20Tweaker%20Updater.exe", "PSO2 Tweaker Updater.exe")
                    Process.Start(Environment.CurrentDirectory & "\PSO2 Tweaker Updater.exe")
                    Application.DoEvents()
                    Return
                End If
            End If
        Else
            Helper.WriteDebugInfoAndWarning(Resources.strFailedToGetUpdateInfo)
        End If
    End Sub

    Private Shared Function IsPso2WinDirMissing() As Boolean
        If Program.Pso2RootDir = "lblDirectory" OrElse Not Directory.Exists(Program.Pso2WinDir) Then
            MsgBox(Resources.strPleaseSelectwin32Dir)
            Helper.SelectPso2Directory()
            Return True
        End If
        Return False
    End Function

    Private Sub DownloadItemTranslationFiles(state As Object)
        Try
            Program.Client.DownloadFile(Program.FreedomUrl & "translator.dll", (Program.Pso2RootDir & "\translator.dll"))
        Catch ex As Exception
            MsgBox("Failed to download translation files! (" & ex.Message.ToString & "). Try rebooting your computer or making sure PSO2 isn't open.")
        End Try

        RegKey.SetValue(Of String)(RegKey.Dllmd5, Helper.GetMd5(Program.Pso2RootDir & "\translator.dll"))

        Try
            Program.Client.DownloadFile(Program.FreedomUrl & "translation.bin", (Program.Pso2RootDir & "\translation.bin"))
        Catch ex As Exception
            MsgBox("Failed to download translation files! (" & ex.Message.ToString & "). Try rebooting your computer or making sure PSO2 isn't open.")
        End Try

        _itemDownloadingDone = True
    End Sub

    Public Sub WriteDebugInfo(ByVal addThisText As String)
        If rtbDebug.InvokeRequired Then
            rtbDebug.Invoke(New Action(Of String)(AddressOf WriteDebugInfo), Text)
        Else
            rtbDebug.Text &= (vbCrLf & addThisText)
        End If
    End Sub

    Public Sub WriteDebugInfoSameLine(ByVal addThisText As String)
        If rtbDebug.InvokeRequired Then
            rtbDebug.Invoke(New Action(Of String)(AddressOf WriteDebugInfoSameLine), Text)
        Else
            rtbDebug.Text &= (" " & addThisText)
        End If
    End Sub

    Public Sub WriteDebugInfoAndOk(ByVal addThisText As String)
        If rtbDebug.InvokeRequired Then
            rtbDebug.Invoke(New Action(Of String)(AddressOf WriteDebugInfoAndOk), Text)
        Else
            rtbDebug.Text &= (vbCrLf & addThisText)
            rtbDebug.Select(rtbDebug.TextLength, 0)
            rtbDebug.SelectionColor = Color.Green
            rtbDebug.AppendText(" [OK!]")
            rtbDebug.SelectionColor = rtbDebug.ForeColor
        End If
    End Sub

    Public Sub WriteDebugInfoAndWarning(ByVal addThisText As String)
        If rtbDebug.InvokeRequired Then
            rtbDebug.Invoke(New Action(Of String)(AddressOf WriteDebugInfoAndWarning), Text)
        Else
            rtbDebug.Text &= (vbCrLf & addThisText)
            rtbDebug.Select(rtbDebug.TextLength, 0)
            rtbDebug.SelectionColor = Color.Red
            rtbDebug.AppendText(" [WARNING!]")
            rtbDebug.SelectionColor = rtbDebug.ForeColor
        End If
    End Sub

    Public Sub WriteDebugInfoAndFailed(ByVal addThisText As String)
        If rtbDebug.InvokeRequired Then
            rtbDebug.Invoke(New Action(Of String)(AddressOf WriteDebugInfoAndFailed), Text)
        Else
            If addThisText = "ERROR - Index was outside the bounds of the array." Then Return
            If addThisText = "ERROR - Object reference not set to an instance of an object." Then Return
            rtbDebug.Text &= (vbCrLf & addThisText)
            rtbDebug.Select(rtbDebug.TextLength, 0)
            rtbDebug.SelectionColor = Color.Red
            rtbDebug.AppendText(Resources.strFAILED)
            rtbDebug.SelectionColor = rtbDebug.ForeColor
            UnlockGui()
        End If
    End Sub

    Private Shared Sub rtbDebug_LinkClicked(sender As Object, e As LinkClickedEventArgs) Handles rtbDebug.LinkClicked
        Process.Start(e.LinkText)
    End Sub

    Private Sub rtbDebug_TextChanged(sender As Object, e As EventArgs) Handles rtbDebug.TextChanged
        rtbDebug.SelectionStart = rtbDebug.Text.Length
    End Sub

    Private Sub OnDownloadProgressChanged(ByVal sender As Object, ByVal e As DownloadProgressChangedEventArgs) Handles DLS.DownloadProgressChanged
        Dim totalSize As Long = e.TotalBytesToReceive
        _totalsize2 = totalSize
        Dim downloadedBytes As Long = e.BytesReceived
        Dim percentage As Integer = e.ProgressPercentage
        PBMainBar.Value = percentage
        PBMainBar.Text = (Resources.strDownloaded & Helper.SizeSuffix(downloadedBytes) & " / " & Helper.SizeSuffix(totalSize) & " (" & percentage & "%) - " & Resources.strRightClickforOptions)
        'Put your progress UI here, you can cancel download by uncommenting the line below
    End Sub

    Private Sub OnFileDownloadCompleted(ByVal sender As Object, ByVal e As AsyncCompletedEventArgs) Handles DLS.DownloadFileCompleted
        PBMainBar.Value = 0
        PBMainBar.Text = ""
    End Sub

    Public Sub DownloadFile(ByVal address As String, ByVal filename As String)
        DLS.Headers("user-agent") = "AQUA_HTTP"
        DLS.Timeout = 10000

        While DLS.IsBusy
            Application.DoEvents()
            Thread.Sleep(16)
        End While

        DLS.DownloadFileAsync(New Uri(address), filename)

        While DLS.IsBusy
            Application.DoEvents()
            Thread.Sleep(16)

            If _restartplz Then
                DLS.CancelAsync()
                _restartplz = False
                DownloadFile(address, filename)
                Exit While
            End If

            If Not Visible And SkipDialogs = False Then
                DLS.CancelAsync()
                _cancelled = True
                _cancelledFull = True
                Windows.Forms.Application.Exit()
            End If
        End While
    End Sub

    Private Sub LockGui()
        Enabled = False
    End Sub

    Private Sub UnlockGui()
        Enabled = True
    End Sub

    Private Shared Function MergePatches() As Dictionary(Of String, String)
        Dim patchlist As String() = File.ReadAllLines("patchlist.txt")
        Dim patchlistOld As String() = File.ReadAllLines("patchlist_old.txt")

        Dim result = New Dictionary(Of String, String)

        For index As Integer = 0 To (patchlist.Length - 1)
            Dim currentLine = patchlist(index)
            If String.IsNullOrEmpty(currentLine) Then Continue For

            Dim filename = Regex.Split(currentLine, ".pat")
            If String.IsNullOrEmpty(filename(0)) Then Continue For

            Dim key = filename(0).Replace("data/win32/", "")

            If Not result.ContainsKey(key) Then
                result.Add(key, currentLine)
            End If
        Next

        For index As Integer = 0 To (patchlistOld.Length - 1)
            Dim currentLine = patchlistOld(index)
            If String.IsNullOrEmpty(currentLine) Then Continue For

            Dim filename = Regex.Split(currentLine, ".pat")
            If String.IsNullOrEmpty(filename(0)) Then Continue For

            Dim key = filename(0).Replace("data/win32/", "")

            If Not result.ContainsKey(key) Then
                result.Add(key, currentLine)
            End If
        Next

        Return result
    End Function

    Private Shared Function MergePrePatches() As Dictionary(Of String, String)
        Dim result As New Dictionary(Of String, String)

        For i As Integer = 0 To 5
            Dim patchlist = File.ReadAllLines("patchlist" & i & ".txt")
            'Dim patchlist = DlwuaString("http://download.pso2.jp/patch_prod/patches_precede/patchlist" & i & ".txt").Split(ControlChars.Cr)

            For index As Integer = 0 To (patchlist.Length - 1)
                If String.IsNullOrEmpty(patchlist(index)) Then Continue For

                Dim splitLine = patchlist(index).Split(ControlChars.Tab)
                Dim fileName = Path.GetFileNameWithoutExtension(splitLine(0).Replace("data/win32/", ""))
                Dim hash = splitLine(2)

                If (Not String.IsNullOrEmpty(fileName)) AndAlso (Not result.ContainsKey(hash)) Then
                    result.Add(hash, fileName)
                End If
            Next
        Next

        Return result
    End Function

    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        DLS.CancelAsync()
        _cancelled = True
        PBMainBar.Value = 0
        PBMainBar.Text = ""
        Helper.DeleteFile("launcherlist.txt")
        Helper.DeleteFile("patchlist.txt")
        Helper.DeleteFile("patchlist_old.txt")
        Helper.DeleteFile("version.ver")
        Windows.Forms.Application.ExitThread()
    End Sub

    Private Shared Sub frmMain_Leave(sender As Object, e As EventArgs) Handles Me.Leave
        Windows.Forms.Application.Exit()
    End Sub

    Private Sub CheckForStoryUpdates()
        Try
            If RegKey.GetValue(Of String)(RegKey.StoryPatchVersion) = "Not Installed" Then Return
            'We're going to comment this out for the moment, but DO NOT REMOVE IT as it may be used again in the future... [AIDA]
            'DownloadFile(Program.FreedomUrl & "patchfiles/Story%20MD5HashList.txt", "Story MD5HashList.txt")

            'Using oReader As StreamReader = File.OpenText("Story MD5HashList.txt")
            'Dim strNewDate As String = oReader.ReadLine()
            'RegKey.SetValue(Of String)(RegKey.NewVersionTemp, strNewDate)
            'RegKey.SetValue(Of String)(RegKey.NewStoryVersion, strNewDate)
            'If strNewDate <> RegKey.GetValue(Of String)(RegKey.StoryPatchVersion) Then
            'A new story patch update is available - Would you like to download and install it? PLEASE NOTE: This update assumes you've already downloaded and installed the latest RAR file available from http://arks-layer.com, which seems to be: 
            ' Create a match using regular exp<b></b>ressions
            'http://arks-layer.com/Story%20Patch%208-8-2013.rar.torrent
            ' Spit out the value plucked from the code
            txtHTML.Text = Regex.Match(Program.Client.DownloadString("http://arks-layer.com/story.php"), "<u>.*?</u>").Value
            Dim strDownloadMe = txtHTML.Text.Replace("<u>", "").Replace("</u>", "")
            If RegKey.GetValue(Of String)(RegKey.LatestStoryBase) <> strDownloadMe Then
                Dim StoryChangeLog As String = Program.Client.DownloadString("http://arks-layer.com/storyupdates.txt")
                Dim mbVisitLink As MsgBoxResult = MsgBox("A new story patch is available! Would you like to download and install it? Here's a list of changes: " & vbCrLf & StoryChangeLog, MsgBoxStyle.YesNo)
                If mbVisitLink = vbYes Then InstallStoryPatchNew()
                Return
            End If

            'Dim updateStoryYesNo As MsgBoxResult = MsgBox("A new story patch update is available as of " & strNewDate & " - Would you like to download and install it? PLEASE NOTE: This update assumes you've already downloaded and installed the latest story patch available from http://arks-layer.com (" & strDownloadMe & "), or used the new method to install the story patch.", vbYesNo)
            'If updateStoryYesNo = vbNo Then Return

            'Dim missingfiles As New List(Of String)
            'Dim numberofChecks As Integer = 0
            'Dim truefilename As String
            'Dim filename As String()
            'Helper.WriteDebugInfo(Resources.strBeginningStoryModeUpdate)

            'While Not (oReader.EndOfStream)
            ' filename = oReader.ReadLine().Split(","c)
            ' truefilename = filename(0)
            '
            '            If Not File.Exists((Program.Pso2WinDir & "\" & truefilename)) Then
            ' missingfiles.Add(truefilename)
            ' ElseIf Helper.GetMd5((Program.Pso2WinDir & "\" & truefilename)) <> filename(1) Then
            ' missingfiles.Add(truefilename)
            ' End If
            '
            '            numberofChecks += 1
            '            lblStatus.Text = (Resources.strCurrentlyCheckingFile & numberofChecks & "")
            '            Application.DoEvents()
            '            End While

            'Helper.WriteDebugInfo("Downloading/Installing updates using Patch Server #4 (New York)")
            'Dim totaldownload As Long = missingfiles.Count
            'Dim downloaded As Long = 0
            '
            '            For Each downloadStr As String In missingfiles
            ' 'Download the missing files:
            ' downloaded += 1
            ' lblStatus.Text = Resources.strUpdating & downloaded & "/" & totaldownload
            ' Application.DoEvents()
            ' _cancelled = False
            ' DownloadFile((Program.FreedomUrl & "patchfiles/" & downloadStr & ".7z"), downloadStr & ".7z")
            ' If _cancelled Then Return
            ' 'Delete the existing file FIRST
            ' If Not File.Exists(downloadStr & ".7z") Then
            ' Helper.WriteDebugInfoAndFailed("File " & (downloadStr & ".7z") & " does not exist! Perhaps it wasn't downloaded properly?")
            ' End If
            ' Helper.DeleteFile((Program.Pso2WinDir & "\" & downloadStr))
            ' Dim processStartInfo As New ProcessStartInfo With
            ' {
            '     .FileName = (Program.StartPath & "\7za.exe"),
            '     .Verb = "runas",
            '     .Arguments = ("e -y " & downloadStr & ".7z"),
            '     .WindowStyle = ProcessWindowStyle.Hidden,
            ' .UseShellExecute = True
            ' }
            ' Process.Start(processStartInfo).WaitForExit()
            ' If Not File.Exists(downloadStr) Then
            ' Helper.WriteDebugInfoAndFailed("File " & (downloadStr) & " does not exist! Perhaps it wasn't extracted properly?")
            ' End If
            ' File.Move(downloadStr, (Program.Pso2WinDir & "\" & downloadStr))
            ' Helper.DeleteFile(downloadStr & ".7z")
            ' Application.DoEvents()
            ' Next
            ' Helper.WriteDebugInfoAndOk(Resources.strStoryPatchUpdated)
            ' RegKey.SetValue(Of String)(RegKey.StoryPatchVersion, RegKey.GetValue(Of String)(RegKey.NewVersionTemp))
            ' RegKey.SetValue(Of String)(RegKey.NewVersionTemp, "")
            ' Else
            ' Helper.WriteDebugInfoAndOk("You have the latest story patch updates!")
            ' End If
            ' End Using
        Catch ex As Exception
            Helper.Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            Helper.WriteDebugInfo(Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub CheckForEnPatchUpdates()
        Try
            If RegKey.GetValue(Of String)(RegKey.EnPatchVersion) = "Not Installed" Then Return
            Application.DoEvents()
            Dim lastfilename As String() = Program.Client.DownloadString(Program.FreedomUrl & "patches/enpatch.txt").Split("/"c)
            Dim strVersion As String = lastfilename(lastfilename.Length - 1).Replace(".rar", "")
            RegKey.SetValue(Of String)(RegKey.NewEnVersion, strVersion)

            If RegKey.GetValue(Of String)(RegKey.EnPatchVersion) <> strVersion Then
                If MsgBox(Resources.strNewENPatch, vbYesNo) = vbYes Then
                    DownloadEnglishPatch()
                End If
            End If
        Catch ex As Exception
            Helper.Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            Helper.WriteDebugInfo(Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub CheckForLargeFilesUpdates()
        Try
            If RegKey.GetValue(Of String)(RegKey.LargeFilesVersion) = "Not Installed" Then Return

            Application.DoEvents()

            Dim strVersion As String = Program.Client.DownloadString(Program.FreedomUrl & "patches/largefilesTRANSAM.txt").Replace("-", "/")

            RegKey.SetValue(Of String)(RegKey.NewLargeFilesVersion, strVersion)

            If RegKey.GetValue(Of String)(RegKey.LargeFilesVersion) <> strVersion Then
                If MsgBox("An update for the Large Files is available. Would you like to install it via TRANSAM?", vbYesNo) = vbYes Then
                    btnLargeFilesTRANSAM.RaiseClick()
                End If
            End If
        Catch ex As Exception
            Helper.Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            Helper.WriteDebugInfo(Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub CheckForPso2Updates(comingFromPrePatch As Boolean)
        Try
            'Precede file, syntax is Yes/No:<Dateoflastprepatch>
            DownloadFile(Program.FreedomUrl & "precede.txt", "precede.txt")
            Dim precedeSplit As String() = File.ReadAllLines("precede.txt")(0).Split(":"c)
            Dim precedeversionstring As String = precedeSplit(1)

            If comingFromPrePatch Then
                DownloadPrePatch(precedeversionstring)
            Else
                Dim firstTimechecking As Boolean = String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.Pso2PrecedeVersion))
                If firstTimechecking Then RegKey.SetValue(Of String)(RegKey.Pso2PrecedeVersion, precedeversionstring)

                If precedeSplit(0) = "Yes" AndAlso (RegKey.GetValue(Of String)(RegKey.Pso2PrecedeVersion) <> precedeversionstring OrElse firstTimechecking) Then
                    Dim downloadPrepatchResult As MsgBoxResult = MsgBox("New pre-patch data is available to download - Would you like to download it? This is optional, and will let you download some of a large patch now, as opposed to having a larger download all at once when it is released.", MsgBoxStyle.YesNo)
                    If downloadPrepatchResult = vbYes Then DownloadPrePatch(precedeversionstring)
                End If
            End If

            'Check whether or not to apply pre-patch shit. Ugh.
            If Directory.Exists(Program.Pso2RootDir & "\_precede\") AndAlso Directory.Exists(Program.Pso2RootDir & "\_precede\data\win32\") Then
                DownloadFile(Program.FreedomUrl & "precede_apply.txt", "precede_apply.txt")
                If File.ReadAllLines("precede_apply.txt")(0) = "Yes" Then
                    InstallPrePatch()
                End If
            End If

            If Not comingFromPrePatch Then
                DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
                Dim version = File.ReadAllLines("version.ver")(0)
                If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.Pso2RemoteVersion)) Then
                    RegKey.SetValue(Of String)(RegKey.Pso2RemoteVersion, version)
                End If

                If RegKey.GetValue(Of String)(RegKey.Pso2RemoteVersion) <> version Then
                    If MsgBox(Resources.strNewPSO2Update, vbYesNo) = vbYes Then btnNewShit.RaiseClick()
                End If
            End If
        Catch ex As Exception
            Helper.Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            Helper.WriteDebugInfo(Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub InstallPrePatch()
        Dim applyPrePatchYesNo As MsgBoxResult = MsgBox("It appears that it's time to install the pre-patch download - Is this okay? If you select no, the pre-patch will not be installed.", vbYesNo)
        If applyPrePatchYesNo = vbYes Then
            'WriteDebugInfo("Restoring backup of vanilla JP files...")
            '[AIDA] Apply the precede   
            If Directory.Exists(BuildBackupPath(EnglishPatch)) Then RestoreBackup(EnglishPatch)
            If Directory.Exists(BuildBackupPath(LargeFiles)) Then RestoreBackup(LargeFiles)
            If Directory.Exists(BuildBackupPath(StoryPatch)) Then RestoreBackup(StoryPatch)
            Helper.WriteDebugInfo("Installing prepatch, please wait...")
            Application.DoEvents()

            'list the names of all files in the specified directory
            Dim files = New DirectoryInfo(Program.Pso2RootDir & "\_precede\data\win32\").GetFiles()
            Dim counter = files.Length
            Dim count As Integer = 0

            For Each dra As FileInfo In files
                Dim downloadStr As String = dra.Name
                Helper.DeleteFile((Program.Pso2WinDir & "\" & downloadStr))
                File.Move(Program.Pso2RootDir & "\_precede\data\win32\" & downloadStr, (Program.Pso2WinDir & "\" & downloadStr))
                count += 1
                lblStatus.Text = "Moved " & count & " files out of " & counter
                Application.DoEvents()
            Next
            Helper.WriteDebugInfoSameLine("Done!")
            Helper.DeleteDirectory(Program.Pso2RootDir & "\_precede")
        End If
    End Sub

    Private Sub DownloadPrePatch(version As String)
        _cancelledFull = False
        lblStatus.Text = ""
        Helper.WriteDebugInfo("Downloading pre-patch filelist...")
        DownloadFile("http://download.pso2.jp/patch_prod/patches_precede/patchlist0.txt", "patchlist0.txt")
        DownloadFile("http://download.pso2.jp/patch_prod/patches_precede/patchlist1.txt", "patchlist1.txt")
        DownloadFile("http://download.pso2.jp/patch_prod/patches_precede/patchlist2.txt", "patchlist2.txt")
        DownloadFile("http://download.pso2.jp/patch_prod/patches_precede/patchlist3.txt", "patchlist3.txt")
        DownloadFile("http://download.pso2.jp/patch_prod/patches_precede/patchlist4.txt", "patchlist4.txt")
        DownloadFile("http://download.pso2.jp/patch_prod/patches_precede/patchlist5.txt", "patchlist5.txt")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        If Not Directory.Exists(Program.Pso2RootDir & "\_precede\data\win32") Then Directory.CreateDirectory(Program.Pso2RootDir & "\_precede\data\win32")

        Dim mergedPrePatches = MergePrePatches()
        mergedPrePatches.Remove("GameGuard.des")
        mergedPrePatches.Remove("PSO2JP.ini")
        mergedPrePatches.Remove("script/user_default.pso2")
        mergedPrePatches.Remove("script/user_intel.pso2")
        mergedPrePatches.Remove("")

        Helper.WriteDebugInfo("Checking for already existing precede files...")

        Dim missingfiles As New List(Of String)
        Dim truefilename As String
        Dim numberofChecks As Integer = 0

        For Each sBuffer In mergedPrePatches
            If _cancelledFull Then Return

            truefilename = sBuffer.Value

            If Not File.Exists(((Program.Pso2RootDir & "\_precede\data\win32") & "\" & truefilename)) Then
                If _vedaUnlocked Then Helper.WriteDebugInfo("DEBUG: The file " & truefilename & " is missing.")
                missingfiles.Add(truefilename)
            ElseIf Helper.GetMd5(((Program.Pso2RootDir & "\_precede\data\win32") & "\" & truefilename)) <> sBuffer.Key Then
                If _vedaUnlocked Then Helper.WriteDebugInfo("DEBUG: The file " & truefilename & " must be redownloaded.")
                missingfiles.Add(truefilename)
            End If

            numberofChecks += 1
            lblStatus.Text = (Resources.strCurrentlyCheckingFile & numberofChecks & "")
            Application.DoEvents()
        Next

        Dim totaldownload As Long = missingfiles.Count
        Dim downloaded As Long = 0
        Dim totaldownloaded As Long = 0

        For Each downloadStr As String In missingfiles
            If _cancelledFull Then Return
            'Download the missing files:
            'WHAT THE FUCK IS GOING ON HERE?
            downloaded += 1
            totaldownloaded += _totalsize2

            lblStatus.Text = Resources.strDownloading & "" & downloaded & "/" & totaldownload & " (" & Helper.SizeSuffix(totaldownloaded) & ")"

            Application.DoEvents()
            _cancelled = False
            DownloadFile(("http://download.pso2.jp/patch_prod/patches_precede/data/win32/" & downloadStr & ".pat"), downloadStr)
            If New FileInfo(downloadStr).Length = 0 Then
                Helper.Log("File appears to be empty, trying to download from secondary SEGA server")
                DownloadFile(("http://download.pso2.jp/patch_prod/patches_precede/data/win32/" & downloadStr & ".pat"), downloadStr)
            End If

            If _cancelled Then Return

            Helper.DeleteFile(((Program.Pso2RootDir & "\_precede\data\win32") & "\" & downloadStr))
            File.Move(downloadStr, ((Program.Pso2RootDir & "\_precede\data\win32") & "\" & downloadStr))
            If _vedaUnlocked Then Helper.WriteDebugInfo("DEBUG: Downloaded and installed " & downloadStr & ".")
            Application.DoEvents()
        Next

        If missingfiles.Count = 0 Then Helper.WriteDebugInfo("Your precede data is up to date!")
        If missingfiles.Count <> 0 Then
            Helper.WriteDebugInfo("Precede data downloaded/updated!")
            If File.Exists("win32list_DO_NOT_DELETE_ME.txt") Then File.Delete("win32list_DO_NOT_DELETE_ME.txt")
            Helper.WriteDebugInfo("You will need to generate a new win32 list via the new patch method. Sorry!")
        End If
        RegKey.SetValue(Of String)(RegKey.Pso2PrecedeVersion, version)
    End Sub

    Private Sub btnApplyChanges_Click(sender As Object, e As EventArgs) Handles btnApplyChanges.Click
        Try
            If IsPso2WinDirMissing() Then Return
            Helper.Log("Restoring/Removing files...")
            If chkRemoveCensor.Checked AndAlso chkRestoreCensor.Checked Then
                MsgBox(Resources.strYouCannotRemoveRestore)
                Return
            End If
            If chkRemoveNVidia.Checked AndAlso chkRestoreNVidia.Checked Then
                MsgBox(Resources.strYouCannotRemoveRestore)
                Return
            End If
            If chkRemovePC.Checked AndAlso chkRestorePC.Checked Then
                MsgBox(Resources.strYouCannotRemoveRestore)
                Return
            End If
            If chkRemoveVita.Checked AndAlso chkRestoreVita.Checked Then
                MsgBox(Resources.strYouCannotRemoveRestore)
                Return
            End If
            If chkRemoveSEGA.Checked AndAlso chkRestoreSEGA.Checked Then
                MsgBox(Resources.strYouCannotRemoveRestore)
                Return
            End If
            'Remove censor
            '[AIDA] Resume here
            If chkRemoveCensor.Checked AndAlso File.Exists((Program.Pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c")) Then
                If File.Exists((Program.Pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup")) Then Computer.FileSystem.DeleteFile((Program.Pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"), UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
                Computer.FileSystem.RenameFile((Program.Pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c"), "ffbff2ac5b7a7948961212cefd4d402c.backup")
                Helper.WriteDebugInfoAndOk(Resources.strRemoving & "Censor...")
            ElseIf chkRemoveCensor.Checked AndAlso (Not File.Exists((Program.Pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c"))) Then
                Helper.WriteDebugInfoAndWarning(Resources.strFailedToRemoveCensor)
            End If
            'Restore Censor
            If chkRestoreCensor.Checked AndAlso File.Exists((Program.Pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup")) Then
                If File.Exists((Program.Pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c")) Then Computer.FileSystem.DeleteFile((Program.Pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"), UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
                Computer.FileSystem.RenameFile((Program.Pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"), "ffbff2ac5b7a7948961212cefd4d402c")
                Helper.WriteDebugInfoAndOk(Resources.strRestoring & "Censor...")
            ElseIf chkRestoreCensor.Checked AndAlso (Not File.Exists((Program.Pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"))) Then
                Helper.WriteDebugInfoAndWarning(Resources.strFailedToRestoreCensor)
            End If
            'Remove PC Opening Video [Done]
            If chkRemovePC.Checked AndAlso File.Exists((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) Then
                If File.Exists((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup")) Then Computer.FileSystem.DeleteFile((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup"), UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
                Computer.FileSystem.RenameFile((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927"), "a44fbb2aeb8084c5a5fbe80e219a9927.backup")
                Helper.WriteDebugInfoAndOk(Resources.strRemoving & "PC Opening Video...")
            ElseIf chkRemovePC.Checked AndAlso (Not File.Exists((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927"))) Then
                Helper.WriteDebugInfoAndWarning(Resources.strFailedToRemovePC)
            End If
            'Restore PC Opening Video [Done]
            If chkRestorePC.Checked AndAlso File.Exists((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup")) Then
                If File.Exists((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) Then Computer.FileSystem.DeleteFile((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927"), UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
                Computer.FileSystem.RenameFile((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup"), "a44fbb2aeb8084c5a5fbe80e219a9927")
                Helper.WriteDebugInfoAndOk(Resources.strRestoring & "PC Opening Video...")
            ElseIf chkRestorePC.Checked AndAlso (Not File.Exists((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup"))) Then
                Helper.WriteDebugInfoAndWarning(Resources.strFailedToRestorePC)
            End If
            'Remove Vita Opening Video [Done]
            If chkRemoveVita.Checked AndAlso File.Exists((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585")) Then
                If File.Exists((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585.backup")) Then Computer.FileSystem.DeleteFile((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585.backup"), UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
                Computer.FileSystem.RenameFile((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585"), "a93adc766eb3510f7b5c279551a45585.backup")
                Helper.WriteDebugInfoAndOk(Resources.strRemoving & "Vita Opening Video...")
            ElseIf chkRemoveVita.Checked AndAlso (Not File.Exists((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585"))) Then
                Helper.WriteDebugInfoAndWarning(Resources.strFailedToRemoveVita)
            End If
            'Restore Vita Opening Video [Done]
            If chkRestoreVita.Checked AndAlso File.Exists((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585.backup")) Then
                If File.Exists((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585")) Then Computer.FileSystem.DeleteFile((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585"), UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
                Computer.FileSystem.RenameFile((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585.backup"), "a93adc766eb3510f7b5c279551a45585")
                Helper.WriteDebugInfoAndOk(Resources.strRestoring & "Vita Opening Video...")
            ElseIf chkRestoreVita.Checked AndAlso (Not File.Exists((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585.backup"))) Then
                Helper.WriteDebugInfoAndWarning(Resources.strFailedToRestoreVita)
            End If
            'Remove NVidia Opening Video [Done]
            If chkRemoveNVidia.Checked AndAlso File.Exists((Program.Pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75")) Then
                If File.Exists((Program.Pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75.backup")) Then Computer.FileSystem.DeleteFile((Program.Pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75.backup"), UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
                Computer.FileSystem.RenameFile((Program.Pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75"), "7f2368d207e104e8ed6086959b742c75.backup")
                Helper.WriteDebugInfoAndOk(Resources.strRemoving & "NVidia Opening Video...")
            ElseIf chkRemoveNVidia.Checked AndAlso (Not File.Exists((Program.Pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75"))) Then
                Helper.WriteDebugInfoAndWarning(Resources.strFailedToRemoveNVidia)
            End If
            'Restore NVidia Opening Video [Done]
            If chkRestoreNVidia.Checked AndAlso File.Exists((Program.Pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75.backup")) Then
                If File.Exists((Program.Pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75")) Then Computer.FileSystem.DeleteFile((Program.Pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75"), UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
                Computer.FileSystem.RenameFile((Program.Pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75.backup"), "7f2368d207e104e8ed6086959b742c75")
                Helper.WriteDebugInfoAndOk(Resources.strRestoring & "NVidia Opening Video...")
            ElseIf chkRestoreNVidia.Checked AndAlso (Not File.Exists((Program.Pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75.backup"))) Then
                Helper.WriteDebugInfoAndWarning(Resources.strFailedToRestoreNVidia)
            End If
            'Remove SEGA Opening Video [Done]
            If chkRemoveSEGA.Checked AndAlso File.Exists((Program.Pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771")) Then
                If File.Exists((Program.Pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771.backup")) Then Computer.FileSystem.DeleteFile((Program.Pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771.backup"), UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
                Computer.FileSystem.RenameFile((Program.Pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771"), "009bfec69b04a34576012d50e3417771.backup")
                Helper.WriteDebugInfoAndOk(Resources.strRemoving & "SEGA Opening Video...")
            ElseIf chkRemoveSEGA.Checked AndAlso (Not File.Exists((Program.Pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771"))) Then
                Helper.WriteDebugInfoAndWarning(Resources.strFailedToRemoveSEGA)
            End If
            'Restore SEGA Opening Video [Done]
            If chkRestoreSEGA.Checked AndAlso File.Exists((Program.Pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771.backup")) Then
                If File.Exists((Program.Pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771")) Then Computer.FileSystem.DeleteFile((Program.Pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771"), UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
                Computer.FileSystem.RenameFile((Program.Pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771.backup"), "009bfec69b04a34576012d50e3417771")
                Helper.WriteDebugInfoAndOk(Resources.strRestoring & "SEGA Opening Video...")
            ElseIf chkRestoreSEGA.Checked AndAlso (Not File.Exists((Program.Pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771.backup"))) Then
                Helper.WriteDebugInfoAndWarning(Resources.strFailedToRestoreSEGA)
            End If
            UnlockGui()
            'Swap PC and Vita Openings
            'Restore PC Opening Video [Done]
            If chkSwapOP.Checked Then
                Helper.WriteDebugInfo(Resources.strSwappingOpenings)
                If File.Exists((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup")) Then
                    If File.Exists((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) Then Computer.FileSystem.DeleteFile((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927"), UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
                    Computer.FileSystem.RenameFile((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup"), "a44fbb2aeb8084c5a5fbe80e219a9927")
                    Helper.WriteDebugInfoAndOk(Resources.strRestoring & "PC Opening Video...")
                End If
                'Restore Vita Opening Video [Done]
                If File.Exists((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585.backup")) Then
                    If File.Exists((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585")) Then Computer.FileSystem.DeleteFile((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585"), UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
                    Computer.FileSystem.RenameFile((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585.backup"), "a93adc766eb3510f7b5c279551a45585")
                    Helper.WriteDebugInfoAndOk(Resources.strRestoring & "Vita Opening Video...")
                End If
                'Rename the original files
                Computer.FileSystem.RenameFile((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927"), "PCOpening")
                Computer.FileSystem.RenameFile((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585"), "VitaOpening")
                'Rename them back, swapping them~
                Computer.FileSystem.RenameFile((Program.Pso2WinDir & "\" & "PCOpening"), "a93adc766eb3510f7b5c279551a45585")
                Computer.FileSystem.RenameFile((Program.Pso2WinDir & "\" & "VitaOpening"), "a44fbb2aeb8084c5a5fbe80e219a9927")
            End If
            If File.Exists((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) AndAlso File.Exists((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585")) Then
                If Helper.GetFileSize((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) = 167479840 AndAlso Helper.GetFileSize((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585")) = 151540352 Then
                    chkSwapOP.Text = Resources.strSwapPCVitaOpenings & "(" & Resources.strNotSwapped & ")"
                    Helper.WriteDebugInfo(Resources.strallDone)
                    UnlockGui()
                    Return
                End If
                If Helper.GetFileSize((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) = 151540352 AndAlso Helper.GetFileSize((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585")) = 167479840 Then
                    chkSwapOP.Text = Resources.strSwapPCVitaOpenings & "(" & Resources.strSwapped & ")"
                    Helper.WriteDebugInfo(Resources.strallDone)
                    UnlockGui()
                    Return
                End If
                chkSwapOP.Text = "Swap PC/Vita Openings (UNKNOWN)"
            End If
        Catch ex As Exception
            Helper.Log(ex.Message.ToString)
            Helper.WriteDebugInfo(Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub btnLaunchPSO2_Click(sender As Object, e As EventArgs) Handles btnLaunchPSO2.Click
        'Fuck SEGA. Stupid jerks.
        Helper.Log("Check if PSO2 is running")
        If Helper.CheckIfRunning("pso2") Then Return
        Try
            If IsPso2WinDirMissing() Then Return

            If Not File.Exists(Program.Pso2RootDir & "\pso2.exe") Then
                Helper.WriteDebugInfoAndFailed(Resources.strCouldNotFindPSO2EXE)
                Return
            End If

            DLS.CancelAsync()
            _cancelled = True
            If SkipDialogs = False Then PBMainBar.Value = 0
            If SkipDialogs = False Then PBMainBar.Text = ""
            Helper.WriteDebugInfo(Resources.strLaunchingPSO2)

            If chkItemTranslation.Checked AndAlso (Helper.GetMd5(Program.Pso2RootDir & "\translator.dll") <> RegKey.GetValue(Of String)(RegKey.Dllmd5)) Then
                MsgBox(Resources.strTranslationFilesDontMatch)
                Return
            End If

            'End Item Translation stuff
            Helper.DeleteFile(Program.Pso2RootDir & "\ddraw.dll")
            File.WriteAllBytes(Program.Pso2RootDir & "\ddraw.dll", Resources.ddraw)
            Dim startInfo As ProcessStartInfo = New ProcessStartInfo With {.FileName = (Program.Pso2RootDir & "\pso2.exe"), .Arguments = "+0x33aca2b9", .UseShellExecute = False}
            startInfo.EnvironmentVariables("-pso2") = "+0x01e3f1e9"
            Dim shell As Process = New Process With {.StartInfo = startInfo}

            Try
                shell.Start()
            Catch ex As Exception
                Helper.WriteDebugInfo(Resources.strItSeemsThereWasAnError)
                DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", "pso2.exe")
                If File.Exists((Program.Pso2RootDir & "\pso2.exe")) AndAlso Program.StartPath <> Program.Pso2RootDir Then Helper.DeleteFile((Program.Pso2RootDir & "\pso2.exe"))
                File.Move("pso2.exe", (Program.Pso2RootDir & "\pso2.exe"))
                Helper.WriteDebugInfoSameLine(Resources.strDone)
                shell.Start()
            End Try

            Hide()
            Dim hWnd As IntPtr = External.FindWindow("Phantasy Star Online 2", Nothing)

            SkipDialogs = False
            tmrWaitingforPSO2.Enabled = True

            Do While hWnd = IntPtr.Zero
                hWnd = External.FindWindow("Phantasy Star Online 2", Nothing)
                Thread.Sleep(10)
                Application.DoEvents()
                If SkipDialogs = True Then Exit Do
            Loop

            Helper.DeleteFile(Program.Pso2RootDir & "\ddraw.dll")
            tmrWaitingforPSO2.Enabled = False
            If RegKey.GetValue(Of String)(RegKey.SteamMode) = "True" Then
                File.Copy(Program.Pso2RootDir & "\pso2.exe", Program.Pso2RootDir & "\pso2.exe_backup", True)
                Do While Helper.IsFileInUse(Program.Pso2RootDir & "\pso2.exe")
                    Thread.Sleep(1000)
                Loop
                File.Copy(Program.Pso2RootDir & "\pso2.exe_backup", Program.Pso2RootDir & "\pso2.exe", True)
                File.Delete(Program.Pso2RootDir & "\pso2.exe_backup")
            End If
            Close()

        Catch ex As Exception
            Helper.Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            Helper.WriteDebugInfo(Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub PBMainBar_MouseClick(sender As Object, e As MouseEventArgs) Handles PBMainBar.MouseClick
        If e.Button = MouseButtons.Right Then
            CancelDownloadToolStripMenuItem.Visible = DLS.IsBusy
            cmsProgressBar.Show(DirectCast(sender, Control), e.Location)
        End If
    End Sub

    Private Sub CancelDownloadToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CancelDownloadToolStripMenuItem.Click
        DLS.CancelAsync()
        Helper.WriteDebugInfo(Resources.strDownloadwasCancelled)
        _cancelled = True
        PBMainBar.Value = 0
        PBMainBar.Text = ""
        lblStatus.Text = ""
    End Sub

    Private Sub CancelProcessToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CancelProcessToolStripMenuItem.Click
        If DLS.IsBusy Then DLS.CancelAsync()
        _cancelled = True
        PBMainBar.Value = 0
        PBMainBar.Text = ""
        lblStatus.Text = ""
        _cancelledFull = True
        Helper.WriteDebugInfo(Resources.strProcessWasCancelled)
        UnlockGui()
    End Sub

    Private Shared Sub Button1_Click(sender As Object, e As EventArgs) Handles btnSelectPSODir.Click
        Helper.SelectPso2Directory()
    End Sub

    Private Sub btnLargeFiles_Click(sender As Object, e As EventArgs) Handles btnLargeFiles.Click
        DownloadLargeFiles()
    End Sub

    Private Sub DownloadLargeFiles()

        ' The Using statement will dispose "net" as soon as we're done with it.
        ' This parses the sidebar page for compatibility
        ' First it downloads the page and splits it by line
        Dim compat As String() = Regex.Split(Program.Client.DownloadString(Program.FreedomUrl & "tweaker.html"), "\r\n|\r|\n")
        Dim doDownload As Boolean = True

        ' Then for each string in the split page, it does a regex match to grab the compatibility.
        ' This way we can avoid .replace.replace.replace.replace.replace and just get straight to the point;
        ' is it equal to "Compatible"
        For Each str As String In compat
            If Regex.IsMatch(str, "> Large Files: <font color=""[^""]+"">([^<]+)</font><br>") Then
                If Not Regex.Match(str, "> Large Files: <font color=""[^""]+"">([^<]+)</font><br>").Groups(1).Value.StartsWith("Compatible") Then
                    Dim reallyInstall As MsgBoxResult = MsgBox("It looks like the Large Files patch isn't compatible right now. Installing it may break your game, force an endless loading screen, crash the universe and/or destablize space and time. Do you really want to install it?", MsgBoxStyle.YesNo)

                    doDownload = reallyInstall <> MsgBoxResult.No
                End If
            End If
        Next

        If doDownload Then
            ' Here we parse the text file before passing it to the DownloadPatch function.
            Dim url As String = Program.Client.DownloadString(Program.FreedomUrl & "patches/largefiles.txt")
            DownloadPatch(url, LargeFiles, "LargeFiles.rar", RegKey.LargeFilesVersion, Resources.strWouldYouLikeToBackupLargeFiles, Resources.strWouldYouLikeToUse)
        Else
            Helper.WriteDebugInfo("Download was cancelled due to incompatibility.")
        End If
    End Sub

    Private Sub chkItemTranslation_Click(sender As Object, e As EventArgs) Handles chkItemTranslation.Click
        If Not File.Exists(Program.Pso2RootDir & "\translation.cfg") Then
            File.WriteAllText(Program.Pso2RootDir & "\translation.cfg", "TranslationPath:translation.bin")
        End If
        If chkItemTranslation.Checked Then
            Helper.WriteDebugInfoAndOk(Resources.strDownloadingLatestItemTranslationFiles)

            'Download translator.dll and translation.bin
            For index As Integer = 1 To 5
                Try
                    Program.Client.DownloadFile(Program.FreedomUrl & "translator.dll", (Program.Pso2RootDir & "\translator.dll"))
                Catch ex As Exception
                    If index = 5 Then
                        Helper.WriteDebugInfoAndWarning("Failed to download translation files! (" & ex.Message.ToString & " Stack Trace: " & ex.StackTrace & ")")
                    End If
                End Try
            Next

            RegKey.SetValue(Of String)(RegKey.Dllmd5, Helper.GetMd5(Program.Pso2RootDir & "\translator.dll"))

            For index As Integer = 1 To 5
                Try
                    Program.Client.DownloadFile(Program.FreedomUrl & "translation.bin", (Program.Pso2RootDir & "\translation.bin"))
                Catch ex As Exception
                    If index = 5 Then
                        Helper.WriteDebugInfoAndWarning("Failed to download translation files! (" & ex.Message.ToString & " Stack Trace: " & ex.StackTrace & ")")
                    End If
                End Try
            Next

            'Start the shitstorm
            Dim builtFile As New List(Of String)
            For Each line In Helper.GetLines(Program.Pso2RootDir & "\translation.cfg")
                If line.Contains("TranslationPath:") Then line = "TranslationPath:translation.bin"
                builtFile.Add(line)
            Next
            File.WriteAllLines(Program.Pso2RootDir & "\translation.cfg", builtFile.ToArray())
            Helper.WriteDebugInfoSameLine(Resources.strDone)
        Else
            Helper.WriteDebugInfoAndOk(Resources.strDeletingItemCache)
            Helper.WriteDebugInfoSameLine(Resources.strDone)
            Dim builtFile As New List(Of String)
            For Each line In Helper.GetLines(Program.Pso2RootDir & "\translation.cfg")
                If line.Contains("TranslationPath:") Then line = "TranslationPath:"
                builtFile.Add(line)
            Next
            File.WriteAllLines(Program.Pso2RootDir & "\translation.cfg", builtFile.ToArray())
        End If

        Program.UseItemTranslation = chkItemTranslation.Checked
        RegKey.SetValue(Of Boolean)(RegKey.UseItemTranslation, Program.UseItemTranslation)
    End Sub

    Private Sub UpdatePso2(comingFromOldFiles As Boolean)
        _cancelledFull = False
        If IsPso2WinDirMissing() Then Return
        Dim missingfiles As New List(Of String)
        Dim missingfiles2 As New List(Of String)
        Dim numberofChecks As Integer = 0
        Dim totalfilesize As Long = 0
        Dim testfilesize As String()
        lblStatus.Text = ""

        If Directory.Exists(BuildBackupPath(EnglishPatch)) Then
            Helper.WriteDebugInfo(Resources.strENBackupFound)
            RestoreBackup(EnglishPatch)
        End If

        If Directory.Exists(BuildBackupPath(LargeFiles)) Then
            Helper.WriteDebugInfo(Resources.strLFBackupFound)
            RestoreBackup(LargeFiles)
        End If

        If Directory.Exists(BuildBackupPath(StoryPatch)) Then
            Helper.WriteDebugInfo(Resources.strStoryBackupFound)
            RestoreBackup(StoryPatch)
        End If

        ' Why is the UI being disabled here, is there something I'm missing? -Matthew
        LockGui()
        Helper.WriteDebugInfo(Resources.strDownloadingPatchFile1)

        Application.DoEvents()
        DownloadFile("http://download.pso2.jp/patch_prod/patches/launcherlist.txt", "launcherlist.txt")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Helper.WriteDebugInfo(Resources.strDownloadingPatchFile2)

        Application.DoEvents()
        DownloadFile("http://download.pso2.jp/patch_prod/patches/patchlist.txt", "patchlist.txt")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Helper.WriteDebugInfo(Resources.strDownloadingPatchFile3)

        Application.DoEvents()
        DownloadFile("http://download.pso2.jp/patch_prod/patches_old/patchlist.txt", "patchlist_old.txt")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Helper.WriteDebugInfo(Resources.strDownloadingPatchFile4)

        Application.DoEvents()
        Program.Client.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Application.DoEvents()
        UnlockGui()

        'Const result As MsgBoxResult = MsgBoxResult.No
        'If result = MsgBoxResult.Yes OrElse _comingFromOldFiles Then
        If comingFromOldFiles Then
            Helper.WriteDebugInfo(Resources.strCheckingforNewContent)
            numberofChecks = 0

            If _cancelledFull Then Return
            For Each line In Helper.GetLines("patchlist.txt")
                If _cancelledFull Then Return
                Dim filename As String() = Regex.Split(line, ".pat")
                Dim truefilename As String = filename(0).Replace("data/win32/", "")
                Dim trueMd5 As String = filename(1).Split(ControlChars.Tab)(2)
                If truefilename <> "GameGuard.des" AndAlso truefilename <> "edition.txt" AndAlso truefilename <> "gameversion.ver" AndAlso truefilename <> "pso2.exe" AndAlso truefilename <> "PSO2JP.ini" AndAlso truefilename <> "script/user_default.pso2" AndAlso truefilename <> "script/user_intel.pso2" Then
                    If Not File.Exists((Program.Pso2WinDir & "\" & truefilename)) Then
                        If _vedaUnlocked Then Helper.WriteDebugInfo("DEBUG: The file " & truefilename & " is missing.")
                        missingfiles.Add(truefilename)
                    ElseIf Helper.GetMd5((Program.Pso2WinDir & "\" & truefilename)) <> trueMd5 Then
                        If _vedaUnlocked Then Helper.WriteDebugInfo("DEBUG: The file " & truefilename & " must be redownloaded.")
                        missingfiles.Add(truefilename)
                    End If
                End If

                numberofChecks += 1
                lblStatus.Text = (Resources.strCurrentlyCheckingFile & numberofChecks & "")
                Application.DoEvents()
            Next

            Helper.DeleteFile("resume.txt")
            File.AppendAllLines("resume.txt", missingfiles)
            Dim totaldownload As Long = missingfiles.Count
            Dim downloaded As Long = 0
            Dim totaldownloadedthings As Long = 0

            For Each downloadStr In missingfiles
                If _cancelledFull Then Return
                'Download the missing files:
                'WHAT THE FUCK IS GOING ON HERE?
                downloaded += 1
                totaldownloadedthings += _totalsize2
                lblStatus.Text = Resources.strDownloading & "" & downloaded & "/" & totaldownload & " (" & Helper.SizeSuffix(totaldownloadedthings) & ")"

                Application.DoEvents()
                _cancelled = False
                DownloadFile(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadStr & ".pat"), downloadStr)
                If New FileInfo(downloadStr).Length = 0 Then
                    Helper.Log("File appears to be empty, trying to download from secondary SEGA server")
                    DownloadFile(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadStr & ".pat"), downloadStr)
                End If

                If _cancelled Then Return
                Helper.DeleteFile((Program.Pso2WinDir & "\" & downloadStr))
                File.Move(downloadStr, (Program.Pso2WinDir & "\" & downloadStr))
                If _vedaUnlocked Then Helper.WriteDebugInfo("DEBUG: Downloaded and installed " & downloadStr & ".")
                Dim linesList As New List(Of String)(File.ReadAllLines("resume.txt"))

                'Remove the line to delete, e.g.
                linesList.Remove(downloadStr)

                File.WriteAllLines("resume.txt", linesList.ToArray())
                Application.DoEvents()
            Next

            If missingfiles.Count = 0 Then Helper.WriteDebugInfo(Resources.strYouAppearToBeUpToDate)
            Dim directoryStringthing As String = (Program.Pso2RootDir & "\")
            Helper.WriteDebugInfo(Resources.strDownloading & "version file...")
            Application.DoEvents()
            _cancelled = False
            Program.Client.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
            If _cancelled Then Return
            If File.Exists((_myDocuments & "\SEGA\PHANTASYSTARONLINE2\version.ver")) Then Helper.DeleteFile((_myDocuments & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
            File.Copy("version.ver", (_myDocuments & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
            Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & "version file"))

            Helper.WriteDebugInfo(Resources.strDownloading & "pso2launcher.exe...")
            Application.DoEvents()
            For Each proc As Process In Process.GetProcessesByName("pso2launcher")
                If proc.MainWindowTitle = "PHANTASY STAR ONLINE 2" AndAlso proc.MainModule.ToString() = "ProcessModule (pso2launcher.exe)" Then proc.Kill()
            Next

            DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2launcher.exe.pat", "pso2launcher.exe")
            If _cancelled Then Return
            If File.Exists((directoryStringthing & "pso2launcher.exe")) AndAlso Program.StartPath <> Program.Pso2RootDir Then Helper.DeleteFile((directoryStringthing & "pso2launcher.exe"))
            File.Move("pso2launcher.exe", (directoryStringthing & "pso2launcher.exe"))
            Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & "pso2launcher.exe"))
            Helper.WriteDebugInfo(Resources.strDownloading & "pso2updater.exe...")
            Application.DoEvents()
            For Each proc As Process In Process.GetProcessesByName("pso2updater")
                If proc.MainModule.ToString() = "ProcessModule (pso2updater.exe)" Then proc.Kill()
            Next

            DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2updater.exe.pat", "pso2updater.exe")
            If _cancelled Then Return
            If File.Exists((directoryStringthing & "pso2updater.exe")) AndAlso Program.StartPath <> Program.Pso2RootDir Then Helper.DeleteFile((directoryStringthing & "pso2updater.exe"))
            File.Move("pso2updater.exe", (directoryStringthing & "pso2updater.exe"))
            Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & "pso2updater.exe"))
            Application.DoEvents()

            Helper.WriteDebugInfo(Resources.strDownloading & "pso2.exe...")
            For Each proc As Process In Process.GetProcessesByName("pso2")
                If proc.MainModule.ToString() = "ProcessModule (pso2.exe)" Then proc.Kill()
            Next

            DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", "pso2.exe")
            If _cancelled Then Return

            If File.Exists((directoryStringthing & "pso2.exe")) AndAlso Program.StartPath <> Program.Pso2RootDir Then Helper.DeleteFile((directoryStringthing & "pso2.exe"))
            File.Move("pso2.exe", (directoryStringthing & "pso2.exe"))
            If _cancelledFull Then Return
            Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & "pso2.exe"))
            RegKey.SetValue(Of String)(RegKey.StoryPatchVersion, "Not Installed")
            RegKey.SetValue(Of String)(RegKey.EnPatchVersion, "Not Installed")
            RegKey.SetValue(Of String)(RegKey.LargeFilesVersion, "Not Installed")
            RegKey.SetValue(Of String)(RegKey.Pso2PatchlistMd5, Helper.GetMd5("patchlist.txt"))
            Helper.WriteDebugInfo(Resources.strGameUpdatedVanilla)
            Helper.DeleteFile("resume.txt")
            RegKey.SetValue(Of String)(RegKey.Pso2RemoteVersion, File.ReadAllLines("version.ver")(0))
            UnlockGui()

            If Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.RemoveCensor)) Then
                If File.Exists((Program.Pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup")) Then Computer.FileSystem.DeleteFile((Program.Pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"), UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
                Computer.FileSystem.RenameFile((Program.Pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c"), "ffbff2ac5b7a7948961212cefd4d402c.backup")
                Helper.WriteDebugInfoAndOk(Resources.strRemoving & "Censor...")
            End If

            Helper.WriteDebugInfoAndOk(Resources.strallDone)
            Return
        Else
            TopMost = chkAlwaysOnTop.Checked
        End If

        If _cancelledFull Then Return
        Dim mergedPatches = MergePatches()
        Helper.WriteDebugInfo(Resources.strCheckingforAllFiles)

        mergedPatches.Remove("GameGuard.des")
        mergedPatches.Remove("PSO2JP.ini")
        mergedPatches.Remove("script/user_default.pso2")
        mergedPatches.Remove("script/user_intel.pso2")
        mergedPatches.Remove("")

        If mergedPatches.ContainsKey("pso2.exe") Then
            mergedPatches.Remove("pso2.exe")
        End If

        Dim dataPath = Program.Pso2RootDir & "\data\win32\"
        Dim length = mergedPatches.Count
        Dim oldmax = PBMainBar.Maximum
        PBMainBar.Maximum = length
        _cancelled = False

        Dim fileLengths = New DirectoryInfo(dataPath).EnumerateFiles().ToDictionary(Function(fileinfo) fileinfo.Name, Function(fileinfo) fileinfo.Length)
        Dim fileNames = fileLengths.Keys

        For Each kvp In mergedPatches

            If _cancelled Then
                PBMainBar.Text = ""
                PBMainBar.Value = 0
                PBMainBar.Maximum = oldmax
                _cancelled = False
                Return
            End If

            lblStatus.Text = (Resources.strCurrentlyCheckingFile & numberofChecks)
            PBMainBar.Text = numberofChecks & " / " & length
            If (numberofChecks Mod 8) = 0 Then Application.DoEvents()
            numberofChecks += 1
            PBMainBar.Value += 1

            If Not fileNames.Contains(kvp.Key) Then
                If _vedaUnlocked Then Helper.WriteDebugInfo("DEBUG: The file " & (dataPath & kvp.Key) & Resources.strIsMissing)
                testfilesize = kvp.Value.Split(ControlChars.Tab)
                totalfilesize += Convert.ToInt32(testfilesize(1))
                missingfiles2.Add(kvp.Key)
                Continue For
            End If

            testfilesize = kvp.Value.Split(ControlChars.Tab)
            Dim fileSize = Convert.ToInt32(testfilesize(1))

            If fileSize <> fileLengths(kvp.Key) Then
                If _vedaUnlocked Then Helper.WriteDebugInfo("DEBUG: The file " & kvp.Key & " must be redownloaded.")
                totalfilesize += fileSize
                missingfiles2.Add(kvp.Key)
                Continue For
            End If

            Using stream = New FileStream(dataPath & kvp.Key, FileMode.Open)
                If Helper.GetMd5(stream) <> testfilesize(2) Then
                    If _vedaUnlocked Then Helper.WriteDebugInfo("DEBUG: The file " & kvp.Key & " must be redownloaded.")
                    totalfilesize += fileSize
                    missingfiles2.Add(kvp.Key)
                End If
            End Using
        Next

        PBMainBar.Text = ""
        PBMainBar.Value = 0
        PBMainBar.Maximum = oldmax

        Dim totaldownload2 As Long = missingfiles2.Count
        Dim downloaded2 As Long = 0
        Dim totaldownloaded As Long = 0
        Helper.DeleteFile("resume.txt")
        File.WriteAllLines("resume.txt", missingfiles2.ToArray())

        For Each downloadStr In missingfiles2
            If _cancelledFull Then Return
            'Download the missing files:
            'WHAT THE FUCK IS GOING ON HERE?
            downloaded2 += 1
            totaldownloaded += _totalsize2

            lblStatus.Text = Resources.strDownloading & "" & downloaded2 & "/" & totaldownload2 & " (" & Helper.SizeSuffix(totaldownloaded) & " / " & Helper.SizeSuffix(totalfilesize) & ")"

            Application.DoEvents()
            DownloadFile(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadStr & ".pat"), downloadStr)
            If New FileInfo(downloadStr).Length = 0 Then
                Helper.Log("File appears to be empty, trying to download from secondary SEGA server")
                DownloadFile(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadStr & ".pat"), downloadStr)
            End If
            If New FileInfo(downloadStr).Length = 0 Then
                Helper.DeleteFile(downloadStr)
                DownloadFile(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadStr & ".pat"), downloadStr)
            End If

            If File.Exists(downloadStr) Then
                Helper.DeleteFile((Program.Pso2WinDir & "\" & downloadStr))
                File.Move(downloadStr, (Program.Pso2WinDir & "\" & downloadStr))
                If _vedaUnlocked Then Helper.WriteDebugInfo("DEBUG: Downloaded and installed " & downloadStr & ".")
                Dim linesList As New List(Of String)(File.ReadAllLines("resume.txt"))

                'Remove the line to delete, e.g.
                linesList.Remove(downloadStr)

                File.WriteAllLines("resume.txt", linesList.ToArray())
            End If
            Application.DoEvents()
        Next

        If missingfiles.Count = 0 Then Helper.WriteDebugInfo(Resources.strYouAppearToBeUpToDate)
        Dim directoryString As String = (Program.Pso2RootDir & "\")
        Helper.WriteDebugInfo(Resources.strDownloading & "version file...")
        Application.DoEvents()
        Program.Client.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
        If File.Exists((_myDocuments & "\SEGA\PHANTASYSTARONLINE2\version.ver")) Then Helper.DeleteFile((_myDocuments & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
        File.Copy("version.ver", (_myDocuments & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
        Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & "version file"))

        Helper.WriteDebugInfo(Resources.strDownloading & "pso2launcher.exe...")
        Application.DoEvents()
        DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2launcher.exe.pat", "pso2launcher.exe")
        If File.Exists((directoryString & "pso2launcher.exe")) AndAlso Program.StartPath <> Program.Pso2RootDir Then Helper.DeleteFile((directoryString & "pso2launcher.exe"))
        File.Move("pso2launcher.exe", (directoryString & "pso2launcher.exe"))
        Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & "pso2launcher.exe"))
        Helper.WriteDebugInfo(Resources.strDownloading & "pso2updater.exe...")
        Application.DoEvents()

        Helper.WriteDebugInfo(Resources.strDownloading & "pso2.exe...")
        DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", "pso2.exe")
        If _cancelled Then Return

        If File.Exists((directoryString & "pso2.exe")) AndAlso Program.StartPath <> Program.Pso2RootDir Then Helper.DeleteFile((directoryString & "pso2.exe"))
        File.Move("pso2.exe", (directoryString & "pso2.exe"))
        If _cancelledFull Then Return
        Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & "pso2.exe"))

        RegKey.SetValue(Of String)(RegKey.StoryPatchVersion, "Not Installed")
        RegKey.SetValue(Of String)(RegKey.EnPatchVersion, "Not Installed")
        RegKey.SetValue(Of String)(RegKey.LargeFilesVersion, "Not Installed")
        RegKey.SetValue(Of String)(RegKey.Pso2PatchlistMd5, Helper.GetMd5("patchlist.txt"))
        Helper.WriteDebugInfo(Resources.strGameUpdatedVanilla)
        Helper.DeleteFile("resume.txt")
        RegKey.SetValue(Of String)(RegKey.Pso2RemoteVersion, File.ReadAllLines("version.ver")(0))
        UnlockGui()

        If Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.RemoveCensor)) Then
            If File.Exists((Program.Pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup")) Then Computer.FileSystem.DeleteFile((Program.Pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"), UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
            Computer.FileSystem.RenameFile((Program.Pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c"), "ffbff2ac5b7a7948961212cefd4d402c.backup")
            Helper.WriteDebugInfoAndOk(Resources.strRemoving & "Censor...")
        End If

        Helper.WriteDebugInfoAndOk(Resources.strallDone)
    End Sub

    Private Sub BtnUpdatePso2_Click(sender As Object, e As EventArgs) Handles BtnUpdatePso2.Click
        UpdatePso2(False)
    End Sub

    Private Sub btnRestoreENBackup_Click(sender As Object, e As EventArgs) Handles btnRestoreENBackup.Click
        Try
            If IsPso2WinDirMissing() Then Return
            If MsgBox(Resources.strAreYouSureRestoreBackup, vbYesNo) = MsgBoxResult.Yes Then
                RestoreBackup(EnglishPatch)
            End If
        Catch ex As Exception
            Helper.Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            Helper.WriteDebugInfo(Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub btnRestoreLargeFilesBackup_Click(sender As Object, e As EventArgs) Handles btnRestoreLargeFilesBackup.Click
        Try
            If IsPso2WinDirMissing() Then Return
            If MsgBox(Resources.strAreYouSureRestoreBackup, vbYesNo) = MsgBoxResult.Yes Then
                RestoreBackup(LargeFiles)
            End If
        Catch ex As Exception
            Helper.Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            Helper.WriteDebugInfo(Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Shared Sub btnRestoreJPNames_Click(sender As Object, e As EventArgs) Handles btnRestoreJPNames.Click
        'http://pso2.arghargh200.net/pso2/2013_06_12/ceffe0e2386e8d39f188358303a92a7d
        If File.Exists((Program.Pso2WinDir & "\" & "ceffe0e2386e8d39f188358303a92a7d.backup")) Then
            If File.Exists((Program.Pso2WinDir & "\" & "ceffe0e2386e8d39f188358303a92a7d")) Then Computer.FileSystem.DeleteFile((Program.Pso2WinDir & "\" & "ceffe0e2386e8d39f188358303a92a7d"), UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
            Computer.FileSystem.RenameFile((Program.Pso2WinDir & "\" & "ceffe0e2386e8d39f188358303a92a7d.backup"), "ceffe0e2386e8d39f188358303a92a7d")
            Helper.WriteDebugInfoAndOk(Resources.strRestoring & " JP Names file...")
        Else
            Helper.WriteDebugInfoAndOk(Resources.strRestoreFailedPleaseReinstall)
        End If
    End Sub

    Private Shared Sub btnRestoreJPETrials_Click(sender As Object, e As EventArgs) Handles btnRestoreJPETrials.Click
        'http://pso2.arghargh200.net/pso2/2013_06_12/057aa975bdd2b372fe092614b0f4399e
        If File.Exists((Program.Pso2WinDir & "\" & "057aa975bdd2b372fe092614b0f4399e.backup")) Then
            If File.Exists((Program.Pso2WinDir & "\" & "057aa975bdd2b372fe092614b0f4399e")) Then Computer.FileSystem.DeleteFile((Program.Pso2WinDir & "\" & "057aa975bdd2b372fe092614b0f4399e"), UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
            Computer.FileSystem.RenameFile((Program.Pso2WinDir & "\" & "057aa975bdd2b372fe092614b0f4399e.backup"), "057aa975bdd2b372fe092614b0f4399e")
            Helper.WriteDebugInfoAndOk(Resources.strRestoring & " JP E-Trials file...")
        Else
            Helper.WriteDebugInfoAndOk(Resources.strRestoreFailedPleaseReinstall)
        End If
    End Sub

    Private Shared Sub btnAnalyze_Click(sender As Object, e As EventArgs) Handles btnAnalyze.Click
        Dim pso2Launchpath As String = Program.Pso2WinDir.Replace("data\win32", "")
        Helper.WriteDebugInfo(Resources.strCheckingForNecessaryFiles)
        If Not File.Exists(pso2Launchpath & "Gameguard.DES") Then Helper.WriteDebugInfoAndWarning("Missing GameGuard.DES file! " & Resources.strPleaseFixGG)
        If Not File.Exists(pso2Launchpath & "pso2.exe") Then Helper.WriteDebugInfoAndWarning("Missing pso2.exe file! " & Resources.strPleaseFixPSO2EXEs)
        If Not File.Exists(pso2Launchpath & "pso2launcher.exe") Then Helper.WriteDebugInfoAndWarning("Missing pso2launcher.exe file! " & Resources.strPleaseFixPSO2EXEs)
        If Not File.Exists(pso2Launchpath & "pso2updater.exe") Then Helper.WriteDebugInfoAndWarning("Missing pso2updater.exe file! " & Resources.strPleaseFixPSO2EXEs)
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Helper.WriteDebugInfo(Resources.strCheckingForFolders)
        If Not Directory.Exists(pso2Launchpath & "\Gameguard\") Then Helper.WriteDebugInfoAndWarning("Missing Gameguard folder! " & Resources.strPleaseFixGG)
        If Not Directory.Exists(pso2Launchpath & "\data\") Then Helper.WriteDebugInfoAndWarning("Missing data folder! " & Resources.strPleaseReinstallOrCheck)
        If Not Directory.Exists(pso2Launchpath & "\data\win32\") Then Helper.WriteDebugInfoAndWarning("Missing data\win32 folder! " & Resources.strPleaseReinstallOrCheck)
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Helper.WriteDebugInfo(Resources.strDoneTesting)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If IsPso2WinDirMissing() Then Return
        Dim filename As String()
        Dim truefilename As String
        Dim missingfiles As New List(Of String)
        Dim filename2 As String()
        Dim truefilename2 As String
        Dim missingfiles2 As New List(Of String)
        Dim numberofChecks As Integer
        LockGui()
        Helper.WriteDebugInfo(Resources.strDownloadingPatchFile1)
        Application.DoEvents()
        DownloadFile("http://download.pso2.jp/patch_prod/patches/launcherlist.txt", "launcherlist.txt")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Helper.WriteDebugInfo(Resources.strDownloadingPatchFile2)
        Application.DoEvents()
        DownloadFile("http://download.pso2.jp/patch_prod/patches/patchlist.txt", "patchlist.txt")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Helper.WriteDebugInfo(Resources.strDownloadingPatchFile3)
        Application.DoEvents()
        DownloadFile("http://download.pso2.jp/patch_prod/patches_old/patchlist.txt", "patchlist_old.txt")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Helper.WriteDebugInfo(Resources.strDownloadingPatchFile4)
        Application.DoEvents()
        Program.Client.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Application.DoEvents()
        UnlockGui()
        Helper.Log("Opening patch file list...")
        If _cancelledFull Then Return
        If File.Exists((Program.Pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup")) Then
            If File.Exists((Program.Pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c")) Then Helper.DeleteFile((Program.Pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c"))
            Computer.FileSystem.RenameFile((Program.Pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"), "ffbff2ac5b7a7948961212cefd4d402c")
            Helper.WriteDebugInfoAndOk(Resources.strRestoring & "Censor...")
        End If
        If File.Exists((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup")) Then
            If File.Exists((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) Then Helper.DeleteFile((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927"))
            Computer.FileSystem.RenameFile((Program.Pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup"), "a44fbb2aeb8084c5a5fbe80e219a9927")
            Helper.WriteDebugInfoAndOk(Resources.strRestoring & "PC Opening...")
        End If
        If File.Exists((Program.Pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75.backup")) Then
            If File.Exists((Program.Pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75")) Then Helper.DeleteFile((Program.Pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75"))
            Computer.FileSystem.RenameFile((Program.Pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75.backup"), "7f2368d207e104e8ed6086959b742c75")
            Helper.WriteDebugInfoAndOk(Resources.strRestoring & "NVidia Logo...")
        End If
        If File.Exists((Program.Pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771.backup")) Then
            If File.Exists((Program.Pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771")) Then Helper.DeleteFile((Program.Pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771"))
            Computer.FileSystem.RenameFile((Program.Pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771.backup"), "009bfec69b04a34576012d50e3417771")
            Helper.WriteDebugInfoAndOk(Resources.strRestoring & "SEGA Logo...")
        End If
        If File.Exists((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585.backup")) Then
            If File.Exists((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585")) Then Helper.DeleteFile((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585"))
            Computer.FileSystem.RenameFile((Program.Pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585.backup"), "a93adc766eb3510f7b5c279551a45585")
            Helper.WriteDebugInfoAndOk(Resources.strRestoring & "Vita Opening...")
        End If
        Helper.WriteDebugInfo(Resources.strCheckingFiles)
        For Each line In Helper.GetLines("patchlist.txt")
            If _cancelledFull Then Return
            filename = Regex.Split(line, ".pat")
            truefilename = filename(0).Replace("data/win32/", "")
            If truefilename <> "GameGuard.des" AndAlso truefilename <> "edition.txt" AndAlso truefilename <> "gameversion.ver" AndAlso truefilename <> "pso2.exe" AndAlso truefilename <> "PSO2JP.ini" AndAlso truefilename <> "script/user_default.pso2" AndAlso truefilename <> "script/user_intel.pso2" Then
                Dim length2 As Long
                If File.Exists(Program.Pso2WinDir & "\" & truefilename) Then length2 = New FileInfo(Program.Pso2WinDir & "\" & truefilename).Length
                If Not File.Exists((Program.Pso2WinDir & "\" & truefilename)) Then
                    Helper.WriteDebugInfo(truefilename & Resources.strIsMissing)
                    missingfiles.Add(truefilename)
                End If
                If File.Exists(Program.Pso2WinDir & "\" & truefilename) Then length2 = New FileInfo(Program.Pso2WinDir & "\" & truefilename).Length
                If Not File.Exists(Program.Pso2WinDir & "\" & truefilename) Then length2 = 1
                If length2 = 0 Then
                    Helper.WriteDebugInfo(truefilename & " has a filesize of 0!")
                    missingfiles.Add(truefilename)
                    Helper.DeleteFile(Program.Pso2WinDir & "\" & truefilename)
                End If
            End If
            numberofChecks += 1
            lblStatus.Text = (Resources.strCurrentlyCheckingFile & numberofChecks)
            Application.DoEvents()
        Next

        Helper.Log("Opening Second patch file...")
        For Each line In Helper.GetLines("patchlist_old.txt")
            If _cancelledFull Then Return
            filename2 = Regex.Split(line, ".pat")
            truefilename2 = filename2(0).Replace("data/win32/", "")
            If truefilename2 <> "GameGuard.des" AndAlso truefilename2 <> "edition.txt" AndAlso truefilename2 <> "gameversion.ver" AndAlso truefilename2 <> "pso2.exe" AndAlso truefilename2 <> "PSO2JP.ini" AndAlso truefilename2 <> "script/user_default.pso2" AndAlso truefilename2 <> "script/user_intel.pso2" Then
                Dim length2 As Long
                If File.Exists(Program.Pso2WinDir & "\" & truefilename2) Then length2 = New FileInfo(Program.Pso2WinDir & "\" & truefilename2).Length
                If Not File.Exists((Program.Pso2WinDir & "\" & truefilename2)) Then
                    If Not missingfiles.Contains(truefilename2) Then
                        Helper.WriteDebugInfo(truefilename2 & Resources.strIsMissing)
                        missingfiles2.Add(truefilename2)
                    End If
                End If
                If File.Exists(Program.Pso2WinDir & "\" & truefilename2) Then length2 = New FileInfo(Program.Pso2WinDir & "\" & truefilename2).Length
                If Not File.Exists(Program.Pso2WinDir & "\" & truefilename2) Then length2 = 1
                If length2 = 0 Then
                    Helper.WriteDebugInfo(truefilename2 & " has a filesize of 0!")
                    missingfiles.Add(truefilename2)
                    Helper.DeleteFile(Program.Pso2WinDir & "\" & truefilename2)
                End If
            End If
            numberofChecks += 1
            lblStatus.Text = (Resources.strCurrentlyCheckingFile & numberofChecks)
            Application.DoEvents()
        Next

        If missingfiles.Count = 0 AndAlso missingfiles2.Count = 0 Then
            Helper.WriteDebugInfoAndOk(Resources.strAllFilesCheckedOut)
            Return
        End If

        Dim result1 As DialogResult = MessageBox.Show(Resources.strWouldYouLikeToDownloadInstallMissing, "Download/Install?", MessageBoxButtons.YesNo)

        If result1 = DialogResult.Yes Then
            Helper.Log(Resources.strDownloading & Resources.strMissingFilesPart1)
            Dim totaldownload As Long = missingfiles.Count
            Dim downloaded As Long = 0
            Dim totaldownloaded As Long = 0
            Helper.DeleteFile("resume.txt")

            File.AppendAllLines("resume.txt", missingfiles)

            For Each downloadStr As String In missingfiles
                'Download the missing files:
                _cancelled = False
                downloaded += 1
                totaldownloaded += _totalsize2
                lblStatus.Text = Resources.strDownloading & "" & downloaded & "/" & totaldownload & " (" & Helper.SizeSuffix(totaldownloaded) & ")"

                DownloadFile(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadStr & ".pat"), downloadStr)

                If New FileInfo(downloadStr).Length = 0 Then
                    Helper.Log("File appears to be empty, trying to download from secondary SEGA server")
                    DownloadFile(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadStr & ".pat"), downloadStr)
                End If

                If _cancelled Then Return
                File.Move(downloadStr, (Program.Pso2WinDir & "\" & downloadStr))
                Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & downloadStr & "."))
                Dim linesList As New List(Of String)(File.ReadAllLines("resume.txt"))

                'Remove the line to delete, e.g.
                linesList.Remove(downloadStr)

                File.WriteAllLines("resume.txt", linesList.ToArray())
                If _cancelledFull Then Return
            Next

            Helper.Log(Resources.strDownloading & Resources.strMissingFilesPart2)

            Helper.DeleteFile("resume.txt")

            File.AppendAllLines("resume.txt", missingfiles2)

            Dim totaldownload2 As Long = missingfiles2.Count
            Dim downloaded2 As Long = 0
            Dim totaldownloaded2 As Long = 0

            For Each downloadstring2 In missingfiles2
                If _cancelledFull Then Return
                'Download the missing files:
                If Not File.Exists((Program.Pso2WinDir & "\" & downloadstring2)) Then
                    _cancelled = False
                    downloaded2 += 1
                    totaldownloaded2 += _totalsize2
                    lblStatus.Text = Resources.strDownloading & "" & downloaded2 & "/" & totaldownload2 & " (" & Helper.SizeSuffix(totaldownloaded2) & ")"

                    DownloadFile(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadstring2 & ".pat"), downloadstring2)

                    If New FileInfo(downloadstring2).Length = 0 Then
                        Helper.Log("File appears to be empty, trying to download from secondary SEGA server")
                        DownloadFile(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring2 & ".pat"), downloadstring2)
                    End If

                    If _cancelled Then Return
                    File.Move(downloadstring2, (Program.Pso2WinDir & "\" & downloadstring2))
                    Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & downloadstring2 & "."))
                    Dim linesList As New List(Of String)(File.ReadAllLines("resume.txt"))

                    'Remove the line to delete, e.g.
                    linesList.Remove(downloadstring2)
                    File.WriteAllLines("resume.txt", linesList.ToArray())
                End If
            Next
            Helper.WriteDebugInfoAndOk(Resources.strallDone)
        End If
    End Sub

    Private Sub ButtonItem10_Click(sender As Object, e As EventArgs) Handles ButtonItem10.Click
        LockGui()
        Helper.WriteDebugInfo(Resources.strDownloadingPatchFile1)
        Application.DoEvents()
        DownloadFile("http://download.pso2.jp/patch_prod/patches/launcherlist.txt", "launcherlist.txt")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Helper.WriteDebugInfo(Resources.strDownloadingPatchFile2)
        Application.DoEvents()
        DownloadFile("http://download.pso2.jp/patch_prod/patches/patchlist.txt", "patchlist.txt")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Helper.WriteDebugInfo(Resources.strDownloadingPatchFile3)
        Application.DoEvents()
        DownloadFile("http://download.pso2.jp/patch_prod/patches_old/patchlist.txt", "patchlist_old.txt")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Helper.WriteDebugInfo(Resources.strDownloadingPatchFile4)
        Application.DoEvents()
        Program.Client.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Application.DoEvents()
        UnlockGui()
        Helper.WriteDebugInfo(Resources.strCheckingForMissingOldFiles)
        _cancelledFull = False
        UpdatePso2(True)
    End Sub

    Private Sub btnGameguard_Click(sender As Object, e As EventArgs) Handles btnGameguard.Click
        Try
            Dim systempath As String
            If SkipDialogs = False Then MsgBox(Resources.strPleaseBeAwareGG)

            If Directory.Exists(Program.Pso2RootDir & "\Gameguard\") Then
                Helper.WriteDebugInfo("Removing Gameguard Directory...")
                Directory.Delete(Program.Pso2RootDir & "\Gameguard\", True)
                Helper.WriteDebugInfoSameLine(Resources.strDone)
            End If
            If File.Exists(Program.Pso2RootDir & "\GameGuard.des") Then
                Helper.WriteDebugInfo("Removing Gameguard File...")
                Helper.DeleteFile(Program.Pso2RootDir & "\GameGuard.des")
                Helper.WriteDebugInfoSameLine(Resources.strDone)
            End If
            If Environment.Is64BitOperatingSystem Then
                systempath = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86)
                If File.Exists(systempath & "\npptnt2.sys") Then
                    Helper.WriteDebugInfo("Removing Hidden Gameguard Files (1 of 3)...")
                    Helper.DeleteFile(systempath & "\npptnt2.sys")
                    Helper.WriteDebugInfoSameLine(Resources.strDone)
                End If
                If File.Exists(systempath & "\nppt9x.vxd") Then
                    Helper.WriteDebugInfo("Removing Hidden Gameguard Files (2 of 3)...")
                    Helper.DeleteFile(systempath & "\nppt9x.vxd")
                    Helper.WriteDebugInfoSameLine(Resources.strDone)
                End If
                If File.Exists(systempath & "\GameMon.des") Then
                    Helper.WriteDebugInfo("Removing Hidden Gameguard Files (3 of 3)...")
                    Helper.DeleteFile(systempath & "\GameMon.des")
                    Helper.WriteDebugInfoSameLine(Resources.strDone)
                End If
            End If
            If Not Environment.Is64BitOperatingSystem Then
                systempath = Environment.GetFolderPath(Environment.SpecialFolder.System)
                If File.Exists(systempath & "\npptnt2.sys") Then
                    Helper.WriteDebugInfo("Removing Hidden Gameguard Files (1 of 3)...")
                    Helper.DeleteFile(systempath & "\npptnt2.sys")
                    Helper.WriteDebugInfoSameLine(Resources.strDone)
                End If
                If File.Exists(systempath & "\nppt9x.vxd") Then
                    Helper.WriteDebugInfo("Removing Hidden Gameguard Files (2 of 3)...")
                    Helper.DeleteFile(systempath & "\nppt9x.vxd")
                    Helper.WriteDebugInfoSameLine(Resources.strDone)
                End If
                If File.Exists(systempath & "\GameMon.des") Then
                    Helper.WriteDebugInfo("Removing Hidden Gameguard Files (3 of 3)...")
                    Helper.DeleteFile(systempath & "\GameMon.des")
                    Helper.WriteDebugInfoSameLine(Resources.strDone)
                End If
            End If
            Helper.WriteDebugInfo("Downloading Latest Gameguard file...")
            DownloadFile("http://download.pso2.jp/patch_prod/patches/GameGuard.des.pat", Program.Pso2RootDir & "\GameGuard.des")
            Helper.WriteDebugInfo("Downloading Latest Gameguard config...")
            DownloadFile("http://download.pso2.jp/patch_prod/patches/PSO2JP.ini.pat", Program.Pso2RootDir & "\PSO2JP.ini")
            Helper.WriteDebugInfoSameLine(Resources.strDone)
            'File.Move("GameGuard.des", Program.Pso2RootDir & "\GameGuard.des")

            Dim foundKey As RegistryKey = Computer.Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Services\npggsvc", True)

            If foundKey Is Nothing Then
                Helper.WriteDebugInfo("No registry keys to delete. This is OK, they should be created the next time Gameguard launches.")
            Else
                Helper.WriteDebugInfo("Deleting Gameguard registry keys...")
                foundKey = Computer.Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Services", True)
                foundKey.DeleteSubKeyTree("npggsvc")
                Helper.WriteDebugInfoSameLine(Resources.strDone)
            End If
            Helper.WriteDebugInfoAndOk(Resources.strGGReset)
            If SkipDialogs = True Then btnLaunchPSO2.PerformClick()
        Catch ex As Exception
            Helper.Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            Helper.WriteDebugInfo(Resources.strERROR & ex.Message)
            If ex.Message.Contains("Access to the path 'GameMon") Then MsgBox("It looks like Gameguard believes it's open, whether or not it actually is. You'll need to restart your computer to fix this problem. Sorry!")
        End Try
    End Sub

    Private Sub btnFixPSO2EXEs_Click(sender As Object, e As EventArgs) Handles btnFixPSO2EXEs.Click
        Try
            If IsPso2WinDirMissing() Then Return
            Dim directoryString As String = (Program.Pso2RootDir & "\")
            _cancelled = False
            Helper.WriteDebugInfo(Resources.strDownloading & "pso2launcher.exe...")

            Application.DoEvents()
            Dim procs As Process() = Process.GetProcessesByName("pso2launcher")

            For Each proc As Process In procs
                If proc.MainWindowTitle = "PHANTASY STAR ONLINE 2" AndAlso proc.MainModule.ToString() = "ProcessModule (pso2launcher.exe)" Then proc.Kill()
            Next

            DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2launcher.exe.pat", "pso2launcher.exe")
            If _cancelled Then Return

            ' never mind I'll just rewrite it later or something idk
            If File.Exists((directoryString & "pso2launcher.exe")) AndAlso Program.StartPath <> Program.Pso2RootDir Then Helper.DeleteFile((directoryString & "pso2launcher.exe"))
            File.Move("pso2launcher.exe", (directoryString & "pso2launcher.exe"))
            Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & "pso2launcher.exe"))
            Helper.WriteDebugInfo(Resources.strDownloading & "pso2updater.exe...")
            Application.DoEvents()
            procs = Process.GetProcessesByName("pso2updater")
            For Each proc As Process In procs
                If proc.MainModule.ToString() = "ProcessModule (pso2updater.exe)" Then proc.Kill()
            Next
            DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2updater.exe.pat", "pso2updater.exe")
            If _cancelled Then Return

            If File.Exists((directoryString & "pso2updater.exe")) AndAlso Program.StartPath <> Program.Pso2RootDir Then Helper.DeleteFile((directoryString & "pso2updater.exe"))
            File.Move("pso2updater.exe", (directoryString & "pso2updater.exe"))
            Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & "pso2updater.exe"))
            Helper.WriteDebugInfo(Resources.strDownloading & "pso2.exe...")
            Application.DoEvents()
            procs = Process.GetProcessesByName("pso2")
            For Each proc As Process In procs
                If proc.MainModule.ToString() = "ProcessModule (pso2.exe)" Then proc.Kill()
            Next
            DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", "pso2.exe")
            If _cancelled Then Return

            If File.Exists((directoryString & "pso2.exe")) AndAlso Program.StartPath <> Program.Pso2RootDir Then Helper.DeleteFile((directoryString & "pso2.exe"))
            File.Move("pso2.exe", (directoryString & "pso2.exe"))
            Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & "pso2.exe"))
            Application.DoEvents()
            Helper.WriteDebugInfo(Resources.strAllNecessaryFiles)
            UnlockGui()
        Catch ex As Exception
            Helper.Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            Helper.WriteDebugInfo(Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub btnRestoreStoryBackup_Click(sender As Object, e As EventArgs) Handles btnRestoreStoryBackup.Click
        Try
            If IsPso2WinDirMissing() Then Return
            If MsgBox(Resources.strAreYouSureRestoreBackup, vbYesNo) = MsgBoxResult.Yes Then
                RestoreBackup(StoryPatch)
            End If
        Catch ex As Exception
            Helper.Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            Helper.WriteDebugInfo(Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Shared Sub btnFixPermissions_Click(sender As Object, e As EventArgs) Handles btnFixPermissions.Click
        Try
            'SystemInformation.UserName
            'cacls.exe "C:\PHANTASYSTARONLINE2\pso2_bin\pso2.exe" /e /g "AIDA":F
            'cacls.exe "C:\PHANTASYSTARONLINE2\pso2_bin\pso2launcher.exe" /e /g "AIDA":F
            'cacls.exe "C:\PHANTASYSTARONLINE2\pso2_bin\pso2download.exe" /e /g "AIDA":F
            'cacls.exe "C:\PHANTASYSTARONLINE2\pso2_bin\pso2updater.exe" /e /g "AIDA":F
            'cacls.exe "C:\PHANTASYSTARONLINE2\pso2_bin\pso2predownload.exe" /e /g "AIDA":F
            MsgBox(Resources.strFixPermissionIssuesText)
            Dim directoryString As String = (Program.Pso2RootDir & "\")
            Helper.WriteDebugInfo(Resources.strFixingPermissionsFor & "pso2.exe...")
            Application.DoEvents()
            Process.Start("cacls.exe", (directoryString & "pso2.exe") & " /e /g """ & SystemInformation.UserName & """:F")
            Helper.WriteDebugInfoSameLine(Resources.strDone)
            Helper.WriteDebugInfo(Resources.strFixingPermissionsFor & "pso2launcher.exe...")
            Application.DoEvents()
            Process.Start("cacls.exe", (directoryString & "pso2launcher.exe") & " /e /g """ & SystemInformation.UserName & """:F")
            Helper.WriteDebugInfoSameLine(Resources.strDone)
            Helper.WriteDebugInfo(Resources.strFixingPermissionsFor & "pso2download.exe...")
            Application.DoEvents()
            Process.Start("cacls.exe", (directoryString & "pso2download.exe") & " /e /g """ & SystemInformation.UserName & """:F")
            Helper.WriteDebugInfoSameLine(Resources.strDone)
            Helper.WriteDebugInfo(Resources.strFixingPermissionsFor & "pso2updater.exe...")
            Application.DoEvents()
            Process.Start("cacls.exe", (directoryString & "pso2updater.exe") & " /e /g """ & SystemInformation.UserName & """:F")
            Helper.WriteDebugInfoSameLine(Resources.strDone)
            Helper.WriteDebugInfo(Resources.strFixingPermissionsFor & "pso2predownload.exe...")
            Application.DoEvents()
            Process.Start("cacls.exe", (directoryString & "pso2predownload.exe") & " /e /g """ & SystemInformation.UserName & """:F")
            Helper.WriteDebugInfoSameLine(Resources.strDone)
            MsgBox(Resources.strFixPermissionsDone)
        Catch ex As Exception
            Helper.Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            Helper.WriteDebugInfo(Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub ButtonItem17_Click(sender As Object, e As EventArgs) Handles ButtonItem17.Click
        If MsgBox(Resources.strAreYouSureResetPSO2Settings, MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
            File.WriteAllText((_myDocuments & "\" & "SEGA\PHANTASYSTARONLINE2\user.pso2"), txtPSO2DefaultINI.Text)
            Helper.WriteDebugInfoAndOk(Resources.strPSO2SettingsReset)
        End If
    End Sub

    Private Shared Sub btnTerminate_Click(sender As Object, e As EventArgs) Handles btnTerminate.Click
        Helper.WriteDebugInfo(Resources.strTerminatePSO2)
        Dim procs As Process() = Process.GetProcessesByName("pso2")
        For Each proc As Process In procs
            If proc.MainModule.ToString() = "ProcessModule (pso2.exe)" Then proc.Kill()
            If proc.MainModule.ToString() = "ProcessModule (GameMon.des)" Then proc.Kill()
            If proc.MainModule.ToString() = "ProcessModule (GameMon64.des)" Then proc.Kill()
        Next
        Helper.WriteDebugInfoSameLine(Resources.strDone)
    End Sub

    Private Shared Sub btnBumped_Click(sender As Object, e As EventArgs) Handles btnBumped.Click
        Process.Start("http://bumped.org/psublog/")
    End Sub

    Private Shared Sub btnCirno_Click(sender As Object, e As EventArgs) Handles btnCirno.Click
        Process.Start("http://pso2.cirnopedia.info/")
    End Sub

    Private Shared Sub btnArksCash_Click(sender As Object, e As EventArgs) Handles btnArksCash.Click
        Process.Start("http://www.pso-world.com/forums/showthread.php?t=199490")
    End Sub

    Private Shared Sub btnErrors_Click(sender As Object, e As EventArgs) Handles btnErrors.Click
        Process.Start("http://www.pso-world.com/forums/showthread.php?t=204836")
    End Sub

    Private Shared Sub btnOfficialPSO2JP_Click(sender As Object, e As EventArgs) Handles btnOfficialPSO2JP.Click
        Process.Start("http://cyberk.it/pso2mirror/")
    End Sub

    Private Shared Sub btnRegistration_Click(sender As Object, e As EventArgs) Handles btnRegistration.Click
        Process.Start("http://www.pso-world.com/forums/showthread.php?t=210236")
    End Sub

    Private Shared Sub btnTweaker_Click(sender As Object, e As EventArgs) Handles btnTweaker.Click
        Process.Start("http://www.pso-world.com/forums/showthread.php?t=207248")
    End Sub

    Private Sub btnCheckForStoryUpdates_Click(sender As Object, e As EventArgs) Handles btnCheckForStoryUpdates.Click
        CheckForStoryUpdates()
    End Sub

    Private Sub chkAlwaysOnTop_Click(sender As Object, e As EventArgs) Handles chkAlwaysOnTop.Click
        If Visible Then
            If chkAlwaysOnTop.Checked Then
                _optionsFrm.TopMost = True
                TopMost = True
                RegKey.SetValue(Of Boolean)(RegKey.AlwaysOnTop, True)
            Else
                _optionsFrm.TopMost = False
                TopMost = False
                RegKey.SetValue(Of Boolean)(RegKey.AlwaysOnTop, False)
            End If
        End If
    End Sub

    Private Sub btnPSO2Options_Click(sender As Object, e As EventArgs) Handles btnPSO2Options.Click
        Cursor = Cursors.WaitCursor
        Try
            _pso2OptionsFrm.TopMost = TopMost
            _pso2OptionsFrm.Top += 50
            _pso2OptionsFrm.Left += 50
            _pso2OptionsFrm.ShowDialog()
        Catch ex As Exception
            Helper.Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            Helper.WriteDebugInfo(Resources.strERROR & ex.Message)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub btnOptions_Click(sender As Object, e As EventArgs) Handles btnOptions.Click
        Cursor = Cursors.WaitCursor
        Try
            _optionsFrm.CMBStyle.SelectedIndex = -1
            _optionsFrm.TopMost = TopMost
            _optionsFrm.Bounds = Bounds
            _optionsFrm.Top += 50
            _optionsFrm.Left += 50
            _optionsFrm.ShowDialog()
        Catch ex As Exception
            Helper.Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            Helper.WriteDebugInfo(Resources.strERROR & ex.Message)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub btnENPatch_Click(sender As Object, e As EventArgs) Handles btnENPatch.Click
        DownloadEnglishPatch()
    End Sub

    Private Sub DownloadEnglishPatch()
        ' Here we parse the text file before passing it to the DownloadPatch function.
        ' The Using statement will dispose "net" as soon as we're done with it.
        ' If we decide not to, we can do away with "url" and just pass net.DownloadString in as the parameter.
        ' Furthermore, we could also parse it from within the function.
        Dim url As String = Program.Client.DownloadString(Program.FreedomUrl & "patches/enpatch.txt")
        DownloadPatch(url, EnglishPatch, "ENPatch.rar", RegKey.EnPatchVersion, Resources.strBackupEN, Resources.strPleaseSelectPreDownloadENRAR)
    End Sub

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Close()
    End Sub

    Private Shared Sub btnEXPFULL_Click(sender As Object, e As EventArgs) Handles btnEXPFULL.Click
        Process.Start("http://www.expfull.com/chat")
    End Sub

    Private Sub OpenSideBar()
        Helper.WriteDebugInfo(Resources.strLoadingSidebar)
        ThreadPool.QueueUserWorkItem(AddressOf LoadSidebar, Nothing)

        If _dpiSetting = 96 Then Width = 796
        If _dpiSetting = 120 Then Width = 1060
        btnAnnouncements.Text = "<"

        Program.SidebarEnabled = True
        RegKey.SetValue(Of Boolean)(RegKey.SidebarEnabled, True)
    End Sub

    Private Sub ToggleSideBar()
        If _dpiSetting = 96 Then
            If Width = 420 Then
                Width = 796
            ElseIf Width = 796 Then
                Width = 420
            End If
        ElseIf _dpiSetting = 120 Then
            If Width = 560 Then
                Width = 1060
            ElseIf Width = 1060 Then
                Width = 560
            End If
        End If

        Program.SidebarEnabled = (Not Program.SidebarEnabled)
        RegKey.SetValue(Of Boolean)(RegKey.SidebarEnabled, Program.SidebarEnabled)

        If Program.SidebarEnabled Then
            ThreadPool.QueueUserWorkItem(AddressOf LoadSidebar, Nothing)
            Helper.WriteDebugInfo(Resources.strLoadingSidebarPage)
            rtbDebug.Refresh()
            btnAnnouncements.Text = "<"
        Else
            btnAnnouncements.Text = ">"
        End If
    End Sub

    Private Sub btnAnnouncements_Click(sender As Object, e As EventArgs) Handles btnAnnouncements.Click
        ToggleSideBar()
    End Sub

    Private Sub WebBrowser4_Navigating(sender As Object, e As WebBrowserNavigatingEventArgs) Handles WebBrowser4.Navigating
        If Visible Then
            If e.Url.ToString() <> Program.FreedomUrl & "tweaker.html" Then
                Process.Start(e.Url.ToString())
                Helper.Log("Trying to load URL for sidebar: " & e.Url.ToString)
                ThreadPool.QueueUserWorkItem(AddressOf LoadSidebar, Nothing)
            End If
        End If
    End Sub

    Private Sub btnUninstallENPatch_Click(sender As Object, e As EventArgs) Handles btnUninstallENPatch.Click
        UninstallPatch(Program.FreedomUrl & "patches/enpatchfilelist.txt", "enpatchfilelist.txt", EnglishPatch, Resources.strENPatchUninstalled, RegKey.EnPatchVersion)
    End Sub

    Private Sub btnUninstallLargeFiles_Click(sender As Object, e As EventArgs) Handles btnUninstallLargeFiles.Click
        UninstallPatch(Program.FreedomUrl & "patches/largefilelist.txt", "largefilelist.txt", LargeFiles, Resources.strLFUninstalled, RegKey.LargeFilesVersion)
    End Sub

    Private Sub btnUninstallStory_Click(sender As Object, e As EventArgs) Handles btnUninstallStory.Click
        UninstallPatch(Program.FreedomUrl & "patches/storyfilelist.txt", "storyfilelist.txt", StoryPatch, Resources.strStoryPatchUninstalled, RegKey.StoryPatchVersion)
    End Sub

    Private Sub btnRussianPatch_Click(sender As Object, e As EventArgs) Handles btnRussianPatch.Click
        DownloadPatch("http://dl.cyberman.me/pso2/rupatch.rar", "RU Patch", "RUPatch.rar", Nothing,
                      "Would you like to backup your files before applying the patch? This will erase all previous Pre-RU patch backups.",
                      "Please select the pre-downloaded RU Patch RAR file")
    End Sub

    Private Sub tsmRestartDownload_Click(sender As Object, e As EventArgs) Handles tsmRestartDownload.Click
        _restartplz = True
    End Sub

    Private Sub btnResumePatching_Click(sender As Object, e As EventArgs) Handles btnResumePatching.Click
        ResumePatching()
    End Sub

    Private Sub ResumePatching()

        If Not File.Exists("resume.txt") Then
            Helper.WriteDebugInfo(Resources.strCannotFindResume)
            Return
        End If

        _cancelledFull = False

        Try
            Dim missingfiles As New List(Of String)

            Helper.WriteDebugInfoAndOk(Resources.strFoundIncompleteJob)
            If _cancelledFull Then Return
            For Each line In Helper.GetLines("resume.txt")
                If _cancelledFull Then Return
                missingfiles.Add(line)
            Next

            Dim totaldownload As Long = missingfiles.Count
            Dim downloaded As Long = 0
            Dim totaldownloaded As Long = 0
            For Each downloadStr As String In missingfiles
                If _cancelledFull Then Return
                'Download the missing files:
                'WHAT THE FUCK IS GOING ON HERE?v3
                downloaded += 1
                totaldownloaded += _totalsize2

                lblStatus.Text = Resources.strDownloading & "" & downloaded & "/" & totaldownload & " (" & Helper.SizeSuffix(totaldownloaded) & ")"

                Application.DoEvents()
                _cancelled = False
                DownloadFile(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadStr & ".pat"), downloadStr)
                If New FileInfo(downloadStr).Length = 0 Then
                    Helper.Log("File appears to be empty, trying to download from secondary SEGA server")
                    DownloadFile(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadStr & ".pat"), downloadStr)
                End If
                If _cancelled Then Return
                'Delete the existing file FIRST
                Helper.DeleteFile((Program.Pso2WinDir & "\" & downloadStr))
                File.Move(downloadStr, (Program.Pso2WinDir & "\" & downloadStr))
                If _vedaUnlocked Then Helper.WriteDebugInfo("DEBUG: Downloaded and installed " & downloadStr & ".")
                Dim linesList As New List(Of String)(File.ReadAllLines("resume.txt"))

                'Remove the line to delete, e.g.
                linesList.Remove(downloadStr)
                File.WriteAllLines("resume.txt", linesList.ToArray())
                Application.DoEvents()
            Next
            Helper.DeleteFile("resume.txt")
            If missingfiles.Count = 0 Then Helper.WriteDebugInfo(Resources.strYouAppearToBeUpToDate)
            Dim directoryString As String = (Program.Pso2RootDir & "\")
            Helper.WriteDebugInfo(Resources.strDownloading & "version file...")
            Application.DoEvents()
            _cancelled = False
            Program.Client.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
            If _cancelled Then Return
            If File.Exists((_myDocuments & "\SEGA\PHANTASYSTARONLINE2\version.ver")) Then Helper.DeleteFile((_myDocuments & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
            File.Copy("version.ver", (_myDocuments & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
            Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & "version file"))
            Helper.WriteDebugInfo(Resources.strDownloading & "pso2launcher.exe...")
            Application.DoEvents()
            For Each proc As Process In Process.GetProcessesByName("pso2launcher")
                If proc.MainWindowTitle = "PHANTASY STAR ONLINE 2" AndAlso proc.MainModule.ToString() = "ProcessModule (pso2launcher.exe)" Then proc.Kill()
            Next
            DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2launcher.exe.pat", "pso2launcher.exe")
            If _cancelled Then Return
            If File.Exists((directoryString & "pso2launcher.exe")) AndAlso Program.StartPath <> Program.Pso2RootDir Then Helper.DeleteFile((directoryString & "pso2launcher.exe"))
            File.Move("pso2launcher.exe", (directoryString & "pso2launcher.exe"))
            Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & "pso2launcher.exe"))
            Helper.WriteDebugInfo(Resources.strDownloading & "pso2updater.exe...")
            Application.DoEvents()
            For Each proc As Process In Process.GetProcessesByName("pso2updater")
                If proc.MainModule.ToString() = "ProcessModule (pso2updater.exe)" Then proc.Kill()
            Next
            DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2updater.exe.pat", "pso2updater.exe")
            If _cancelled Then Return
            If File.Exists((directoryString & "pso2updater.exe")) AndAlso Program.StartPath <> Program.Pso2RootDir Then Helper.DeleteFile((directoryString & "pso2updater.exe"))
            File.Move("pso2updater.exe", (directoryString & "pso2updater.exe"))
            Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & "pso2updater.exe"))
            Application.DoEvents()
            Helper.WriteDebugInfo(Resources.strDownloading & "pso2.exe...")
            For Each proc As Process In Process.GetProcessesByName("pso2")
                If proc.MainModule.ToString() = "ProcessModule (pso2.exe)" Then proc.Kill()
            Next

            DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", "pso2.exe")
            If _cancelled Then Return

            If File.Exists((directoryString & "pso2.exe")) AndAlso Program.StartPath <> Program.Pso2RootDir Then Helper.DeleteFile((directoryString & "pso2.exe"))
            File.Move("pso2.exe", (directoryString & "pso2.exe"))
            If _cancelledFull Then Return
            Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & "pso2.exe"))
            RegKey.SetValue(Of String)(RegKey.StoryPatchVersion, "Not Installed")
            RegKey.SetValue(Of String)(RegKey.EnPatchVersion, "Not Installed")
            RegKey.SetValue(Of String)(RegKey.LargeFilesVersion, "Not Installed")
            DownloadFile("http://download.pso2.jp/patch_prod/patches/patchlist.txt", "patchlist.txt")
            Helper.WriteDebugInfoSameLine(Resources.strDone)
            RegKey.SetValue(Of String)(RegKey.Pso2PatchlistMd5, Helper.GetMd5("patchlist.txt"))
            Helper.WriteDebugInfo(Resources.strGameUpdatedVanilla)
            Helper.DeleteFile("resume.txt")
            RegKey.SetValue(Of String)(RegKey.Pso2RemoteVersion, File.ReadAllLines("version.ver")(0))
            UnlockGui()

            If Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.RemoveCensor)) Then
                If File.Exists((Program.Pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup")) Then Computer.FileSystem.DeleteFile((Program.Pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"), UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
                Computer.FileSystem.RenameFile((Program.Pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c"), "ffbff2ac5b7a7948961212cefd4d402c.backup")
                Helper.WriteDebugInfoAndOk(Resources.strRemoving & "Censor...")
            End If

            If Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.EnPatchAfterInstall)) Then
                Helper.WriteDebugInfo(Resources.strAutoInstallingENPatch)
                DownloadEnglishPatch()
            End If

            If Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.LargeFilesAfterInstall)) Then
                Helper.WriteDebugInfo(Resources.strAutoInstallingLF)
                DownloadLargeFiles()
            End If

            If Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.StoryPatchAfterInstall)) Then
                Helper.WriteDebugInfo(Resources.strAutoInstallingStoryPatch)
                InstallStoryPatchNew()
            End If

            Helper.WriteDebugInfoAndOk(Resources.strallDone)
        Catch ex As Exception
            Helper.Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            If ex.Message <> "Arithmetic operation resulted in an overflow." Then
                Helper.WriteDebugInfo(Resources.strERROR & ex.Message)
            End If
        End Try
    End Sub

    Private Shared Sub ButtonItem7_Click(sender As Object, e As EventArgs) Handles ButtonItem7.Click
        Const processName As String = "chrome"
        Dim processes = Process.GetProcessesByName("chrome")
        Dim currentProcess As Process = Process.GetCurrentProcess()
        If processes.Length > 0 Then
            Dim closeItYesNo As MsgBoxResult = MsgBox("You need to have all Chrome windows closed before launching in this mode. Would you like to close all open Chrome windows now?", vbYesNo)
            If closeItYesNo = vbYes Then
                For Each proc As Process In Process.GetProcessesByName(processName)
                    If proc.Id <> currentProcess.Id Then proc.Kill()
                Next
            Else
                Helper.WriteDebugInfoAndWarning("You need to have all Chrome windows closed before launching in this mode. Please close all Chrome windows and try again.")
                Return
            End If
        End If
        MsgBox(Resources.strPleaseBeAwareChrome)
        Process.Start("chrome", "--no-sandbox")
    End Sub

    Private Sub btnClearSACache_Click(sender As Object, e As EventArgs) Handles btnClearSACache.Click
        Dim clearYesNo As MsgBoxResult = MsgBox("This will clear all Symbol Arts from your ""History"" tab. Having 100 pages of Symbol Arts to load can sometimes cause slowdown.", vbYesNo)
        If clearYesNo = vbYes Then
            Helper.WriteDebugInfo("Deleting Symbol Art Cache...")
            For Each foundFile As String In Computer.FileSystem.GetFiles((_myDocuments & "\" & "SEGA\PHANTASYSTARONLINE2\symbolarts\cache"), FileIO.SearchOption.SearchAllSubDirectories, "*.*")
                Computer.FileSystem.DeleteFile(foundFile, UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
            Next
            Helper.WriteDebugInfoSameLine(Resources.strDone)
        End If
    End Sub

    Private Sub btnInstallPSO2_Click(sender As Object, e As EventArgs) Handles btnInstallPSO2.Click
        InstallPso2()
    End Sub

    Private Sub InstallPso2()
        Dim installFolder As String = ""
        'Const installYesNo As MsgBoxResult = vbYes
        'If installYesNo = vbNo Then
        '    WriteDebugInfo("You can view more information about the installer at:" & vbCrLf & "http://arks-layer.com/setup.php")
        '    Return
        'End If
        'If installYesNo = vbYes Then
        MsgBox("This will install Phantasy Star Online EPISODE 2! Please select a folder to install into." & vbCrLf & "A folder called PHANTASYSTARONLINE2 will be created inside the folder you choose." & vbCrLf & "(For example, if you choose the C drive, it will install to C:\PHANTASYSTARONLINE2\)" & vbCrLf & "It is HIGHLY RECOMMENDED that you do NOT install into the Program Files folder, but a normal folder like C:\PHANTASYSTARONLINE\")
        Dim installPso2 As Boolean = True
        While installPso2
            Dim myFolderBrowser As New FolderBrowserDialog With {.RootFolder = Environment.SpecialFolder.MyComputer, .Description = "Please select a folder (or drive) to install PSO2 into"}
            Dim dlgResult As DialogResult = myFolderBrowser.ShowDialog()

            If dlgResult = DialogResult.OK Then
                installFolder = myFolderBrowser.SelectedPath
            End If
            If dlgResult = DialogResult.Cancel Then
                Helper.WriteDebugInfo("Installation cancelled by user!")
                Return
            End If
            Dim correctYesNo As MsgBoxResult = MsgBox("You wish to install PSO2 into " & (installFolder & "\PHANTASYSTARONLINE2\. Is this correct?").Replace("\\", "\"), vbYesNoCancel)
            If correctYesNo = vbCancel Then
                Helper.WriteDebugInfo("Installation cancelled by user!")
                Return
            End If
            If correctYesNo = vbNo Then
                Continue While
            End If
            If correctYesNo = vbYes Then
                For Each drive In DriveInfo.GetDrives()
                    If (drive.DriveType = DriveType.Fixed) AndAlso (installFolder(0) = drive.Name(0)) Then
                        If drive.TotalSize < 26992893636 Then
                            MsgBox("There is not enough space on the selected disk to install PSO2. Please select a different drive. (Requires 16GB of free space)")
                            Continue While
                        End If
                        If drive.AvailableFreeSpace < 26992893636 Then
                            MsgBox("There is not enough free space on the selected disk to install PSO2. Please free up some space or select a different drive. (Requires 16GB of free space)")
                            Continue While
                        End If
                    End If
                Next

                Dim finalYesNo As MsgBoxResult = MsgBox("The program will now install the necessary files, create the folders, and set up the game. Afterwards, the program will automatically begin patching. Click ""OK"" to start.", MsgBoxStyle.OkCancel)
                If finalYesNo = vbCancel Then
                    Helper.WriteDebugInfo("Installation cancelled by user!")
                Else
                    Office2007StartButton1.Enabled = False
                    'set the pso2Dir to the install patch
                    Program.Pso2RootDir = (installFolder & "\PHANTASYSTARONLINE2\pso2_bin").Replace("\\", "\")
                    lblDirectory.Text = Program.Pso2RootDir
                    Show()
                    Focus()
                    Application.DoEvents()
                    Helper.WriteDebugInfo("Downloading DirectX setup...")
                    Try
                        DownloadFile("http://arks-layer.com/docs/dxwebsetup.exe", "dxwebsetup.exe")
                        Helper.WriteDebugInfoSameLine("Done!")
                        Helper.WriteDebugInfo("Checking/Installing DirectX...")
                        Dim processStartInfo As ProcessStartInfo = New ProcessStartInfo() With {.FileName = "dxwebsetup.exe", .Verb = "runas", .Arguments = "/Q", .UseShellExecute = True}
                        Process.Start(processStartInfo).WaitForExit()
                        Helper.WriteDebugInfoSameLine("Done!")
                    Catch ex As Exception
                        Helper.WriteDebugInfo("DirectX installation failed! Please install it later if neccessary!")
                    End Try

                    If File.Exists("dxwebsetup.exe") Then File.Delete("dxwebsetup.exe")
                    'Make a data folder, and a win32 folder under that
                    Directory.CreateDirectory(Program.Pso2RootDir & "\data\win32\")
                    'Download required pso2 stuff
                    Helper.WriteDebugInfo("Downloading PSO2 required files...")
                    DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2launcher.exe.pat", Program.Pso2RootDir & "\pso2launcher.exe")
                    DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2updater.exe.pat", Program.Pso2RootDir & "\pso2updater.exe")
                    DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", Program.Pso2RootDir & "\pso2.exe")
                    DownloadFile("http://download.pso2.jp/patch_prod/patches/PSO2JP.ini.pat", Program.Pso2RootDir & "\PSO2JP.ini")
                    Helper.WriteDebugInfoSameLine("Done!")
                    'Download Gameguard.des
                    Helper.WriteDebugInfo("Downloading Latest Gameguard file...")
                    DownloadFile("http://download.pso2.jp/patch_prod/patches/GameGuard.des.pat", Program.Pso2RootDir & "\GameGuard.des")
                    Helper.WriteDebugInfoSameLine(Resources.strDone)
                    Application.DoEvents()

                    RegKey.SetValue(Of String)(RegKey.Pso2Dir, Program.Pso2RootDir)
                    Helper.WriteDebugInfo(Program.Pso2RootDir & " " & Resources.strSetAsYourPSO2)
                    Program.Pso2WinDir = (Program.Pso2RootDir & "\data\win32")
                    If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.StoryPatchVersion)) Then RegKey.SetValue(Of String)(RegKey.StoryPatchVersion, "Not Installed")
                    If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.EnPatchVersion)) Then RegKey.SetValue(Of String)(RegKey.EnPatchVersion, "Not Installed")
                    If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.LargeFilesVersion)) Then RegKey.SetValue(Of String)(RegKey.LargeFilesVersion, "Not Installed")

                    'Check for PSO2 Updates~
                    UpdatePso2(False)

                    MsgBox("PSO2 installed, patched to the latest Japanese version, and ready to play!" & vbCrLf & "Press OK to continue.")
                    Refresh()
                End If
            End If

            installPso2 = False
            Program.IsPso2Installed = True
        End While
        'End If
    End Sub

    Private Sub btnConfigureItemTranslation_Click(sender As Object, e As EventArgs) Handles btnConfigureItemTranslation.Click
        FrmItemConfig.Show()
    End Sub

    Private Shared Sub btnSymbolEditor_Click(sender As Object, e As EventArgs) Handles btnSymbolEditor.Click
        Process.Start("http://www.pso-world.com/forums/showthread.php?t=215777")
    End Sub

    Private Shared Sub btnRunPSO2Linux_Click(sender As Object, e As EventArgs) Handles btnRunPSO2Linux.Click
        Process.Start("http://www.pso-world.com/forums/showthread.php?t=215642")
    End Sub

    Private Sub LoadSidebar(state As Object)
        Try
            WebBrowser4.Navigate(Program.FreedomUrl & "tweaker.html")
        Catch ex As Exception
            Helper.WriteDebugInfo("Web Browser failed: " & ex.Message.ToString)
        End Try
    End Sub

    Private Shared Sub btnDonateToTweaker_Click(sender As Object, e As EventArgs) Handles btnDonateToTweaker.Click
        Process.Start("http://arks-layer.com/donate.php")
    End Sub

    Private Shared Sub btnDonateToBumped_Click(sender As Object, e As EventArgs) Handles btnDonateToBumped.Click
        Process.Start("http://bumped.org/psublog/about/")
    End Sub

    Private Shared Sub btnDonateToCirno_Click(sender As Object, e As EventArgs) Handles btnDonateToCirno.Click
        Process.Start("http://pso2.cirnopedia.info/support.php")
    End Sub

    Private Shared Sub btnDonateToENPatchHost_Click(sender As Object, e As EventArgs) Handles btnDonateToENPatchHost.Click
        Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=UB7UN9MQ7WZ44")
    End Sub

    Private Shared Sub btnResetTweaker_Click(sender As Object, e As EventArgs) Handles btnResetTweaker.Click
        Dim resetyesno As MsgBoxResult = MsgBox("This will erase all of the PSO2 Tweaker's settings, and restart the program. Continue?", vbYesNo)
        If resetyesno = vbYes Then
            Computer.Registry.CurrentUser.DeleteSubKeyTree("Software\AIDA", False)
            Helper.Log("All settings reset, restarting program!")
            Windows.Forms.Application.Restart()
        End If
    End Sub

    Private Sub btnPredownloadLobbyVideos_Click(sender As Object, e As EventArgs) Handles btnPredownloadLobbyVideos.Click
        If IsPso2WinDirMissing() Then Return
        'Download the missing files:
        _cancelled = False
        Const downloadStr As String = "3fdcad94b7af8c597542cd23e6a87236"

        lblStatus.Text = Resources.strDownloading & " lobby video (" & Helper.SizeSuffix(_totalsize2) & ")"

        DownloadFile(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadStr & ".pat"), downloadStr)

        If New FileInfo(downloadStr).Length = 0 Then
            Helper.Log("File appears to be empty, trying to download from secondary SEGA server")
            DownloadFile(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadStr & ".pat"), downloadStr)
        End If
        If _cancelled Then Return
        File.Move(downloadStr, (Program.Pso2WinDir & "\" & downloadStr))
        Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & downloadStr & "."))
    End Sub

    Private Sub btnDownloadPrepatch_Click(sender As Object, e As EventArgs) Handles btnDownloadPrepatch.Click
        CheckForPso2Updates(True)
    End Sub

    Private Sub btnCopyInfo_Click_1(sender As Object, e As EventArgs) Handles btnCopyInfo.Click
        Try
            FrmDiagnostic.TopMost = TopMost
            FrmDiagnostic.Top += 50
            FrmDiagnostic.Left += 50
            FrmDiagnostic.ShowDialog()
        Catch ex As Exception
            Helper.Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            Helper.WriteDebugInfo(Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Shared Sub btnChooseProxyServer_Click(sender As Object, e As EventArgs) Handles btnChooseProxyServer.Click
        Try
            'JSON should look like { "version": 1, "host": "0.0.0.0", "name": "Super cool proxy", "publickeyurl": "http://url.com" }

            Dim jsonurl As String = InputBox("Please input the URL of the configuration JSON:", "Configuration JSON", "")
            If String.IsNullOrEmpty(jsonurl) Then Return

            Helper.WriteDebugInfo("Downloading configuration...")
            Program.Client.DownloadFile(jsonurl, "ServerConfig.txt")

            Dim proxyInfo As Pso2ProxyInfo
            Using stream As FileStream = File.Open("ServerConfig.txt", FileMode.Open)
                Dim serializer As DataContractJsonSerializer = New DataContractJsonSerializer(GetType(Pso2ProxyInfo))
                proxyInfo = DirectCast(serializer.ReadObject(stream), Pso2ProxyInfo)
            End Using

            If Convert.ToInt32(proxyInfo.Version) <> 1 Then
                MsgBox("ERROR - Version is incorrect! Please recheck the JSON.")
                Return
            End If

            If Not proxyInfo.PublicKeyUrl.Contains("publickey.blob") Then
                MsgBox("ERROR - Public Key URL doesn't point to a public key blob! Please recheck the JSON.")
                Return
            End If

            For index As Integer = 0 To (proxyInfo.Host.Length - 1)
                If Char.IsLetter(proxyInfo.Host(index)) Then
                    Dim ips = Dns.GetHostAddresses(proxyInfo.Host)
                    proxyInfo.Host = ips(0).ToString()
                    Exit For
                End If
            Next

            Helper.WriteDebugInfoSameLine(" Done!")

            Dim builtFile As New List(Of String)
            Dim alreadyModified As Boolean = False
            If Not File.Exists(Program.HostsFilePath) Then File.Create(Program.HostsFilePath).Dispose()

            For Each line In Helper.GetLines(Program.HostsFilePath)
                Dim splitLine = line.Split(" "c)

                If splitLine.Length > 1 Then
                    Select Case (splitLine(1))
                        Case "gs001.pso2gs.net"
                            line = proxyInfo.Host & " gs001.pso2gs.net #" & proxyInfo.Name & " Ship 01"
                            alreadyModified = True
                        Case "gs016.pso2gs.net"
                            line = proxyInfo.Host & " gs016.pso2gs.net #" & proxyInfo.Name & " Ship 02"
                            alreadyModified = True
                        Case "gs031.pso2gs.net"
                            line = proxyInfo.Host & " gs031.pso2gs.net #" & proxyInfo.Name & " Ship 03"
                            alreadyModified = True
                        Case "gs046.pso2gs.net"
                            line = proxyInfo.Host & " gs046.pso2gs.net #" & proxyInfo.Name & " Ship 04"
                            alreadyModified = True
                        Case "gs061.pso2gs.net"
                            line = proxyInfo.Host & " gs061.pso2gs.net #" & proxyInfo.Name & " Ship 05"
                            alreadyModified = True
                        Case "gs076.pso2gs.net"
                            line = proxyInfo.Host & " gs076.pso2gs.net #" & proxyInfo.Name & " Ship 06"
                            alreadyModified = True
                        Case "gs091.pso2gs.net"
                            line = proxyInfo.Host & " gs091.pso2gs.net #" & proxyInfo.Name & " Ship 07"
                            alreadyModified = True
                        Case "gs106.pso2gs.net"
                            line = proxyInfo.Host & " gs106.pso2gs.net #" & proxyInfo.Name & " Ship 08"
                            alreadyModified = True
                        Case "gs121.pso2gs.net"
                            line = proxyInfo.Host & " gs121.pso2gs.net #" & proxyInfo.Name & " Ship 09"
                            alreadyModified = True
                        Case "gs136.pso2gs.net"
                            line = proxyInfo.Host & " gs136.pso2gs.net #" & proxyInfo.Name & " Ship 10"
                            alreadyModified = True
                    End Select
                End If

                builtFile.Add(line)
            Next

            If alreadyModified Then
                Helper.WriteDebugInfo("Modifying HOSTS file...")
            Else
                builtFile.Add(proxyInfo.Host & " gs001.pso2gs.net #" & proxyInfo.Name & " Ship 01")
                builtFile.Add(proxyInfo.Host & " gs016.pso2gs.net #" & proxyInfo.Name & " Ship 02")
                builtFile.Add(proxyInfo.Host & " gs031.pso2gs.net #" & proxyInfo.Name & " Ship 03")
                builtFile.Add(proxyInfo.Host & " gs046.pso2gs.net #" & proxyInfo.Name & " Ship 04")
                builtFile.Add(proxyInfo.Host & " gs061.pso2gs.net #" & proxyInfo.Name & " Ship 05")
                builtFile.Add(proxyInfo.Host & " gs076.pso2gs.net #" & proxyInfo.Name & " Ship 06")
                builtFile.Add(proxyInfo.Host & " gs091.pso2gs.net #" & proxyInfo.Name & " Ship 07")
                builtFile.Add(proxyInfo.Host & " gs106.pso2gs.net #" & proxyInfo.Name & " Ship 08")
                builtFile.Add(proxyInfo.Host & " gs121.pso2gs.net #" & proxyInfo.Name & " Ship 09")
                builtFile.Add(proxyInfo.Host & " gs136.pso2gs.net #" & proxyInfo.Name & " Ship 10")
                Helper.WriteDebugInfo("Previous modifications not found, creating new entries...")
            End If

            File.WriteAllLines(Program.HostsFilePath, builtFile.ToArray())
            Helper.WriteDebugInfoSameLine(" Done!")

            Helper.WriteDebugInfo("Downloading and installing publickey.blob...")
            Program.Client.DownloadFile(proxyInfo.PublicKeyUrl, Program.StartPath & "\publickey.blob")
            If File.Exists(Program.Pso2RootDir & "\publickey.blob") AndAlso Program.StartPath <> Program.Pso2RootDir Then Helper.DeleteFile(Program.Pso2RootDir & "\publickey.blob")
            If Program.StartPath <> Program.Pso2RootDir Then File.Move(Program.StartPath & "\publickey.blob", Program.Pso2RootDir & "\publickey.blob")
            Helper.WriteDebugInfoSameLine(" Done!")
            Helper.WriteDebugInfo("All done! You should now be able to connect to " & proxyInfo.Name & ".")
            RegKey.SetValue(Of Boolean)(RegKey.ProxyEnabled, True)
        Catch ex As Exception
            Helper.WriteDebugInfoAndFailed("ERROR - " & ex.Message.ToString)
            If ex.Message.Contains("is denied.") AndAlso ex.Message.Contains("Access to the path") Then
                MsgBox("It seems you've gotten an error while trying to patch your HOSTS file. Please go to the " & Environment.SystemDirectory & "\drivers\etc\ folder, right click on the hosts file, and make sure ""Read Only"" is not checked. Then try again." & vbNewLine & "When you click Okay, the Tweaker will also generate a pastebin of your HOSTS file and what is locking it. Look at the bottom of the pastebin where the 'HOSTS Handle stuff' is.")
                FrmDiagnostic.Button2.PerformClick()
            End If
        End Try
    End Sub

    Private Shared Sub btnRevertPSO2ProxyToJP_Click(sender As Object, e As EventArgs) Handles btnRevertPSO2ProxyToJP.Click
        '[Revert]
        Dim builtFile = New List(Of String)
        If Not File.Exists(Program.HostsFilePath) Then File.Create(Program.HostsFilePath).Dispose()

        For Each line In Helper.GetLines(Program.HostsFilePath)
            If Not line.Contains("pso2gs.net") Then builtFile.Add(line)
        Next

        Helper.WriteDebugInfo("Modifying HOSTS file...")
        File.WriteAllLines(Program.HostsFilePath, builtFile.ToArray())
        Helper.WriteDebugInfoSameLine(" Done!")
        Helper.DeleteFile(Program.Pso2RootDir & "\publickey.blob")
        Helper.WriteDebugInfoAndOk("All normal JP connection settings restored!")
        RegKey.SetValue(Of Boolean)(RegKey.ProxyEnabled, False)
    End Sub

#If DEBUG Then
    Private Sub btnNewShit_Click(sender As Object, e As EventArgs) Handles btnNewShit.Click
        Helper.WriteDebugInfo("Starting TRANS-AM BURST system...")
        'All done!

        'arcnmx: It first creates a new empty patchlist. It loops through each item in
        'p (the new one freshly downloaded).

        'arcnmx: If, searching by filename, it can't find an entry in po (the old currently-installed
        'patchlist) or it finds an entry but the filesize/md5 is different, it puts the entry from p
        'in the new list.

        'arcnmx: Then the new list only contains the files that changed, and need to be downloaded.

        'Have two patchlists (the old merged SOME OF THE THINGS) (B) and the new patchlist downloaded
        'from SEGA (A), merged.

        'Read the first line from A, search until the end of B. If found, and the lines are different,
        'add it to missing files. If it's not there, add to missing files.

        'download all missingfiles

        'if file "currentpatchlist.txt" is not found then build list like SEGA's.

        _cancelledFull = False
        If IsPso2WinDirMissing() Then Return
        lblStatus.Text = ""

        If Directory.Exists(BuildBackupPath(EnglishPatch)) Then
            If File.Exists("win32list_DO_NOT_DELETE_ME.txt") Then File.Delete("win32list_DO_NOT_DELETE_ME.txt")
            Helper.WriteDebugInfo(Resources.strENBackupFound)
            RestoreBackup(EnglishPatch)
        End If

        If Directory.Exists(BuildBackupPath(LargeFiles)) Then
            If File.Exists("win32list_DO_NOT_DELETE_ME.txt") Then File.Delete("win32list_DO_NOT_DELETE_ME.txt")
            Helper.WriteDebugInfo(Resources.strLFBackupFound)
            RestoreBackup(LargeFiles)
        End If

        If Directory.Exists(BuildBackupPath(StoryPatch)) Then
            If File.Exists("win32list_DO_NOT_DELETE_ME.txt") Then File.Delete("win32list_DO_NOT_DELETE_ME.txt")
            Helper.WriteDebugInfo(Resources.strStoryBackupFound)
            RestoreBackup(StoryPatch)
        End If

        Dim totalfiles = Directory.GetFiles(Program.Pso2RootDir & "\data\win32\")

        If File.Exists("win32list_DO_NOT_DELETE_ME.txt_temp") Then File.Delete("win32list_DO_NOT_DELETE_ME.txt_temp")

        If Not File.Exists("win32list_DO_NOT_DELETE_ME.txt") Then
            Helper.WriteDebugInfo("Building file list... ")
            Helper.WriteDebugInfo("This will only be done once, as long as you don't delete the 'win32list_DO_NOT_DELETE_ME.txt' file")
            Dim di As New DirectoryInfo(Program.Pso2RootDir & "\data\win32\")

            Using writer As New StreamWriter("win32list_DO_NOT_DELETE_ME.txt_temp", True)
                Dim count As Integer = 0
                For Each dra In di.GetFiles()
                    If _cancelledFull Then Return
                    writer.WriteLine("data/win32/" & dra.Name & ".pat" & vbTab & dra.Length & vbTab & Helper.GetMd5(Program.Pso2RootDir & "\data\win32\" & dra.Name))
                    count += 1
                    lblStatus.Text = "Building first time list of win32 files (" & count & "/" & totalfiles.Length & ")"
                    Application.DoEvents()
                Next
            End Using

            FileSystem.RenameFile("win32list_DO_NOT_DELETE_ME.txt_temp", "win32list_DO_NOT_DELETE_ME.txt")

            Helper.WriteDebugInfoSameLine("Done!")
        End If

        LockGui()
        Helper.WriteDebugInfo(Resources.strDownloadingPatchFile1)
        Application.DoEvents()
        DownloadFile("http://download.pso2.jp/patch_prod/patches/launcherlist.txt", "launcherlist.txt")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Helper.WriteDebugInfo(Resources.strDownloadingPatchFile2)
        Application.DoEvents()
        DownloadFile("http://download.pso2.jp/patch_prod/patches/patchlist.txt", "patchlist.txt")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Helper.WriteDebugInfo(Resources.strDownloadingPatchFile3)
        Application.DoEvents()
        DownloadFile("http://download.pso2.jp/patch_prod/patches_old/patchlist.txt", "patchlist_old.txt")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Helper.WriteDebugInfo(Resources.strDownloadingPatchFile4)
        Application.DoEvents()
        Program.Client.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
        Helper.WriteDebugInfoSameLine(Resources.strDone)
        Application.DoEvents()
        UnlockGui()
        Dim mergedPatches = MergePatches()

        'Rewrite this to support the new format

        Dim segaLine As String
        Dim segaFilename As String
        Dim missingfiles As New List(Of String)
        Dim oldarray = File.ReadAllLines("win32list_DO_NOT_DELETE_ME.txt")

        For i As Integer = 0 To mergedPatches.Count

            If _cancelledFull Then Return
            segaLine = mergedPatches.Values(i)
            If String.IsNullOrEmpty(segaLine) Then Continue For

            segaFilename = segaLine.Remove(segaLine.IndexOf(".pat", StringComparison.Ordinal)).Replace("data/win32/", "")
            lblStatus.Text = "Checking file " & i + 1 & " / " & mergedPatches.Count
            If missingfiles.Count > 0 Then lblStatus.Text &= " (missing files found: " & missingfiles.Count & ")"
            Application.DoEvents()
            If Not oldarray.Contains(segaLine) And segaLine.Contains("user_default.pso2") = False And segaLine.Contains("edition.txt") = False And segaLine.Contains("GameGuard.des") = False And segaLine.Contains("gameversion.ver") = False And segaLine.Contains("pso2.exe") = False And segaLine.Contains("PSO2JP.ini") = False And segaLine.Contains("script/user_intel.pso2") = False And segaLine.Contains("ffbff2ac5b7a7948961212cefd4d402c") = False Then
                missingfiles.Add(segaFilename)
            End If
        Next


        'Helper.DeleteFile("resume.txt")
        'File.AppendAllLines("resume.txt", missingfiles)
        Dim totaldownload As Long = missingfiles.Count
        Dim downloaded As Long = 0
        Dim totaldownloadedthings As Long = 0

        For Each downloadStr In missingfiles
            If _cancelledFull Then Return
            'Download the missing files:
            'WHAT THE FUCK IS GOING ON HERE?
            downloaded += 1
            totaldownloadedthings += _totalsize2
            lblStatus.Text = Resources.strDownloading & "" & downloaded & "/" & totaldownload & " (" & Helper.SizeSuffix(totaldownloadedthings) & ")"

            Application.DoEvents()
            _cancelled = False
            DownloadFile(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadStr & ".pat"), downloadStr)
            If New FileInfo(downloadStr).Length = 0 Then
                Helper.Log("File appears to be empty, trying to download from secondary SEGA server")
                DownloadFile(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadStr & ".pat"), downloadStr)
            End If

            If _cancelled Then Return
            Helper.DeleteFile((Program.Pso2WinDir & "\" & downloadStr))
            File.Move(downloadStr, (Program.Pso2WinDir & "\" & downloadStr))
            If _vedaUnlocked Then Helper.WriteDebugInfo("DEBUG: Downloaded and installed " & downloadStr & ".")
            'Dim linesList As New List(Of String)(File.ReadAllLines("resume.txt"))

            'Remove the line to delete, e.g.
            'linesList.Remove(downloadStr)

            'File.WriteAllLines("resume.txt", linesList.ToArray())
            Application.DoEvents()
        Next
        If missingfiles.Count = 0 Then Helper.WriteDebugInfo(Resources.strYouAppearToBeUpToDate)
        Dim directoryStringthing As String = (Program.Pso2RootDir & "\")

        Helper.WriteDebugInfo(Resources.strDownloading & "version file...")
        Application.DoEvents()
        _cancelled = False
        Program.Client.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
        If _cancelled Then Return
        If File.Exists((_myDocuments & "\SEGA\PHANTASYSTARONLINE2\version.ver")) Then Helper.DeleteFile((_myDocuments & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
        File.Copy("version.ver", (_myDocuments & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
        Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & "version file"))

        Helper.WriteDebugInfo(Resources.strDownloading & "pso2launcher.exe...")
        Application.DoEvents()
        For Each proc As Process In Process.GetProcessesByName("pso2launcher")
            If proc.MainWindowTitle = "PHANTASY STAR ONLINE 2" AndAlso proc.MainModule.ToString() = "ProcessModule (pso2launcher.exe)" Then proc.Kill()
        Next

        DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2launcher.exe.pat", "pso2launcher.exe")
        If _cancelled Then Return
        If File.Exists((directoryStringthing & "pso2launcher.exe")) AndAlso Program.StartPath <> Program.Pso2RootDir Then Helper.DeleteFile((directoryStringthing & "pso2launcher.exe"))
        File.Move("pso2launcher.exe", (directoryStringthing & "pso2launcher.exe"))
        Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & "pso2launcher.exe"))
        Helper.WriteDebugInfo(Resources.strDownloading & "pso2updater.exe...")
        Application.DoEvents()
        For Each proc As Process In Process.GetProcessesByName("pso2updater")
            If proc.MainModule.ToString() = "ProcessModule (pso2updater.exe)" Then proc.Kill()
        Next

        DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2updater.exe.pat", "pso2updater.exe")
        If _cancelled Then Return
        If File.Exists((directoryStringthing & "pso2updater.exe")) AndAlso Program.StartPath <> Program.Pso2RootDir Then Helper.DeleteFile((directoryStringthing & "pso2updater.exe"))
        File.Move("pso2updater.exe", (directoryStringthing & "pso2updater.exe"))
        Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & "pso2updater.exe"))
        Application.DoEvents()

        Helper.WriteDebugInfo(Resources.strDownloading & "pso2.exe...")
        For Each proc As Process In Process.GetProcessesByName("pso2")
            If proc.MainModule.ToString() = "ProcessModule (pso2.exe)" Then proc.Kill()
        Next

        DownloadFile("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", "pso2.exe")
        If _cancelled Then Return

        If File.Exists((directoryStringthing & "pso2.exe")) AndAlso Program.StartPath <> Program.Pso2RootDir Then Helper.DeleteFile((directoryStringthing & "pso2.exe"))
        File.Move("pso2.exe", (directoryStringthing & "pso2.exe"))
        If _cancelledFull Then Return

        Helper.WriteDebugInfoAndOk((Resources.strDownloadedandInstalled & "pso2.exe"))


        If RegKey.GetValue(Of String)(RegKey.StoryPatchVersion) <> "Not Installed" Then RegKey.SetValue(Of String)(RegKey.StoryPatchVersion, "Not Updated")
        If RegKey.GetValue(Of String)(RegKey.EnPatchVersion) <> "Not Installed" Then RegKey.SetValue(Of String)(RegKey.EnPatchVersion, "Not Updated")
        If RegKey.GetValue(Of String)(RegKey.LargeFilesVersion) <> "Not Installed" Then RegKey.SetValue(Of String)(RegKey.LargeFilesVersion, "Not Updated")

        RegKey.SetValue(Of String)(RegKey.Pso2PatchlistMd5, Helper.GetMd5("patchlist.txt"))
        Helper.WriteDebugInfo("Game updated to the latest version. Don't forget to re-install/update the patches, as some of the files might have been untranslated.")
        'Helper.DeleteFile("resume.txt")
        RegKey.SetValue(Of String)(RegKey.Pso2RemoteVersion, File.ReadAllLines("version.ver")(0))
        UnlockGui()

        If Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.RemoveCensor)) And File.Exists((Program.Pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c")) Then
            If File.Exists((Program.Pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup")) Then Computer.FileSystem.DeleteFile((Program.Pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"), UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
            Computer.FileSystem.RenameFile((Program.Pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c"), "ffbff2ac5b7a7948961212cefd4d402c.backup")
            Helper.WriteDebugInfoAndOk(Resources.strRemoving & "Censor...")
        End If

        Helper.WriteDebugInfo("Updating win32 list...")

        'Write new win32 here
        'Take in the old file, search for a file that was downloaded.
        'Replace that line with the new filesize or make a new entry if the file wasn't found in the old one.
        'SHOULD be much faster than writing a new win32 file
        If File.Exists("new_win32.txt") Then File.Delete("new_win32.txt")
        If File.Exists("missingfiles.txt") Then File.Delete("missingfiles.txt")
        Dim FILE_NAME As String = ""
        Dim Found As Boolean = False
        System.IO.File.WriteAllText("new_win32.txt", System.IO.File.ReadAllText("win32list_DO_NOT_DELETE_ME.txt"))
        For i As Integer = 0 To missingfiles.Count - 1
            Found = False
            File.AppendAllText("missingfiles.txt", missingfiles(i) & vbNewLine)
            For Each match As Match In Regex.Matches(System.IO.File.ReadAllText("new_win32.txt"), "data/win32/" & missingfiles(i).ToString & ".*")
                'MsgBox(match.Value)
                Dim info As New FileInfo(Program.Pso2RootDir & "\data\win32\" & missingfiles(i).ToString)
                System.IO.File.WriteAllText("new_win32.txt", System.IO.File.ReadAllText("new_win32.txt").Replace(match.Value, "data/win32/" & missingfiles(i).ToString & ".pat" & vbTab & info.Length & vbTab & Helper.GetMd5(Program.Pso2RootDir & "\data\win32\" & missingfiles(i).ToString)))
                Found = True
            Next
            If Found = False Then
                Dim info As New FileInfo(Program.Pso2RootDir & "\data\win32\" & missingfiles(i).ToString)
                System.IO.File.WriteAllText("new_win32.txt", System.IO.File.ReadAllText("new_win32.txt") & vbNewLine & "data/win32/" & missingfiles(i).ToString & ".pat" & vbTab & info.Length & vbTab & Helper.GetMd5(Program.Pso2RootDir & "\data\win32\" & missingfiles(i).ToString))
            End If
        Next

        If File.Exists("win32list_DO_NOT_DELETE_ME.txt") Then File.Delete("win32list_DO_NOT_DELETE_ME.txt")
        FileSystem.RenameFile("new_win32.txt", "win32list_DO_NOT_DELETE_ME.txt")
        Helper.WriteDebugInfoSameLine(" Done!")
        Helper.WriteDebugInfoAndOk(Resources.strallDone)

        'MsgBox(missingfiles.Count)
    End Sub
#End If

    Private Sub btnStoryPatchNew_Click(sender As Object, e As EventArgs) Handles btnStoryPatchNew.Click
        InstallStoryPatchNew()
    End Sub

    Private Sub InstallStoryPatchNew()
        'Don't forget to make GUI changes!
        Try
            If IsPso2WinDirMissing() Then Return

            ' Create a match using regular exp<b></b>ressions
            ' Spit out the value plucked from the code
            txtHTML.Text = Regex.Match(Program.Client.DownloadString("http://arks-layer.com/story.php"), "<u>.*?</u>").Value

            Dim backupdir As String = BuildBackupPath(StoryPatch)
            Dim strStoryPatchLatestBase As String = txtHTML.Text.Replace("<u>", "").Replace("</u>", "").Replace("/", "-")
            Helper.WriteDebugInfoAndOk("Downloading story patch info... ")
            DownloadFile(Program.FreedomUrl & "pso2.stripped.db.7z", "pso2.stripped.db.7z")
            Dim processStartInfo2 As New ProcessStartInfo With
            {
                .FileName = (Program.StartPath & "\7za.exe"),
                .Verb = "runas",
                .Arguments = ("e -y pso2.stripped.db.7z"),
                .WindowStyle = ProcessWindowStyle.Hidden,
            .UseShellExecute = True
            }
            Process.Start(processStartInfo2).WaitForExit()
            Dim DBMD5 As String = Helper.GetMd5("pso2.stripped.db")
            Helper.WriteDebugInfoAndOk("Downloading Trans-Am tool... ")
            DownloadFile(Program.FreedomUrl & "pso2-transam.exe", "pso2-transam.exe")

            'execute pso2-transam stuff with -b flag for backup
            Dim processStartInfo As ProcessStartInfo = New ProcessStartInfo() With {.FileName = "pso2-transam.exe", .Verb = "runas"}
            'If Directory.Exists(backupdir) Then
            'Dim counter = Computer.FileSystem.GetFiles(backupdir)
            'If counter.Count > 0 Then
            processStartInfo.Arguments = ("-t story-eng-" & strStoryPatchLatestBase & " pso2.stripped.db " & """" & Program.Pso2WinDir & """")
            'Else
            'Helper.Log("[TRANSAM] Creating backup directory")
            'Directory.CreateDirectory(backupdir)
            'Helper.WriteDebugInfo(Resources.strCreatingBackupDirectory)
            'processStartInfo.Arguments = ("-b " & """" & backupdir & """" & " -t story-eng-" & strStoryPatchLatestBase & " pso2.stripped.db " & """" & Program.Pso2WinDir & """")
            'End If
            'End If

            'We don't need to make backups anymore
            'If Not Directory.Exists(backupdir) Then
            ' Helper.Log("[TRANSAM] Creating backup directory")
            ' Directory.CreateDirectory(backupdir)
            ' Helper.WriteDebugInfo(Resources.strCreatingBackupDirectory)
            ' processStartInfo.Arguments = ("-b " & """" & backupdir & """" & " -t story-eng-" & strStoryPatchLatestBase & " pso2.stripped.db " & """" & Program.Pso2WinDir & """")
            ' End If

            processStartInfo.UseShellExecute = False
            Helper.Log("[TRANSAM] Starting shitstorm")
            processStartInfo.Arguments = processStartInfo.Arguments.Replace("\", "/")
            Helper.Log("TRANSM parameters: " & processStartInfo.Arguments & vbCrLf & "TRANSAM Working Directory: " & processStartInfo.WorkingDirectory)
            Helper.Log("[TRANSAM] Program started")
            If Helper.GetMd5("pso2.stripped.db") <> DBMD5 Then
                MsgBox("ERROR: Files have been modified since download. Aborting...")
                Exit Sub
            End If
            Process.Start(processStartInfo).WaitForExit()
            Helper.DeleteFile("pso2.stripped.db")
            Helper.DeleteFile("pso2-transam.exe")
            External.FlashWindow(Handle, True)
            'Story Patch 3-12-2014.rar
            RegKey.SetValue(Of String)(RegKey.StoryPatchVersion, strStoryPatchLatestBase.Replace("-", "/"))
            RegKey.SetValue(Of String)(RegKey.LatestStoryBase, strStoryPatchLatestBase.Replace("-", "/"))
            Helper.WriteDebugInfo(Resources.strStoryPatchInstalled)
            CheckForStoryUpdates()
        Catch ex As Exception
            MsgBox("ERROR - " & ex.Message.ToString)
        End Try
    End Sub

    Private Sub btnJPEnemyNames_Click(sender As Object, e As EventArgs) Handles btnJPEnemyNames.Click
        RestoreJapaneseNames("ceffe0e2386e8d39f188358303a92a7d", "JP enemy names")
    End Sub

    Private Sub btnJPETrials_Click(sender As Object, e As EventArgs) Handles btnJPETrials.Click
        RestoreJapaneseNames("057aa975bdd2b372fe092614b0f4399e", "JP E-Trials file")
    End Sub

    Private Sub RestoreJapaneseNames(filename As String, patchname As String, Optional url As String = "http://107.170.16.100/patches/")
        Try
            If IsPso2WinDirMissing() Then Return

            Helper.WriteDebugInfo(Resources.strDownloading & patchname & "...")
            Application.DoEvents()
            Dim strDownloadMe As String = url & filename
            _cancelled = False
            DownloadFile(strDownloadMe, filename)
            If _cancelled Then Return
            Helper.WriteDebugInfo((Resources.strDownloadCompleteDownloaded & strDownloadMe & ")"))

            If File.Exists((Program.Pso2WinDir & "\" & filename)) Then
                If File.Exists((Program.Pso2WinDir & "\" & filename & ".backup")) Then
                    Computer.FileSystem.DeleteFile((Program.Pso2WinDir & "\" & filename & ".backup"), UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
                End If

                Computer.FileSystem.RenameFile((Program.Pso2WinDir & "\" & filename), filename & ".backup")
            End If

            Application.DoEvents()
            File.Move(filename, (Program.Pso2WinDir & "\" & filename))
            External.FlashWindow(Handle, True)
            Helper.WriteDebugInfo(patchname & " " & Resources.strInstalledUpdated)
            UnlockGui()
        Catch ex As Exception
            Helper.Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            Helper.WriteDebugInfo(Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub DownloadPatch(patchUrl As String, patchName As String, patchFile As String, versionStr As String, msgBackup As String, msgSelectArchive As String)
        _cancelledFull = False
        Try
            If IsPso2WinDirMissing() Then Return

            Dim backupyesno As MsgBoxResult
            Dim predownloadedyesno As MsgBoxResult
            Dim rarLocation As String = ""
            Dim strVersion As String = ""

            ' Check the patch download method preference
            Dim patchPreference As String = RegKey.GetValue(Of String)(RegKey.PreDownloadedRar)
            Select Case patchPreference
                Case "Ask"
                    predownloadedyesno = MsgBox(Resources.strWouldYouLikeToUse, vbYesNo)
                Case "Always"
                    predownloadedyesno = MsgBoxResult.Yes
                Case "Never"
                    predownloadedyesno = MsgBoxResult.No
                Case Else
                    predownloadedyesno = MsgBox(Resources.strWouldYouLikeToUse, vbYesNo)
            End Select

            ' Check the backup preference
            patchPreference = "Never"
            'patchPreference = RegKey.GetValue(Of String)(RegKey.Backup)
            Select Case patchPreference
                Case "Ask"
                    backupyesno = MsgBox(msgBackup, vbYesNo)
                Case "Always"
                    backupyesno = MsgBoxResult.Yes
                Case "Never"
                    backupyesno = MsgBoxResult.No
                Case Else
                    backupyesno = MsgBox(msgBackup, vbYesNo)
            End Select

            If predownloadedyesno = MsgBoxResult.No Then
                Helper.WriteDebugInfo(Resources.strDownloading & patchName & "...")
                Application.DoEvents()

                ' Might want to switch to a Uri class.
                ' Get the filename from the downloaded Path
                Dim lastfilename As String() = patchUrl.Split("/"c)
                strVersion = lastfilename(lastfilename.Length - 1)
                strVersion = Path.GetFileNameWithoutExtension(strVersion) ' We're using this so that it's not format-specific.

                _cancelled = False

                If Not Helper.CheckLink(patchUrl) Then
                    Helper.WriteDebugInfoAndFailed("Failed to contact " & patchName & " website - Patch install/update canceled!")
                    Helper.WriteDebugInfo("Please visit http://goo.gl/YzCE7 for more information!")
                    Return
                End If

                DownloadFile(patchUrl, patchFile)
                If _cancelled Then Return
                Helper.WriteDebugInfo((Resources.strDownloadCompleteDownloaded & patchUrl & ")"))
            ElseIf predownloadedyesno = MsgBoxResult.Yes Then
                OpenFileDialog1.Title = msgSelectArchive
                OpenFileDialog1.FileName = "PSO2 " & patchName & " RAR file"
                OpenFileDialog1.Filter = "RAR Archives|*.rar|All Files (*.*) |*.*"
                If OpenFileDialog1.ShowDialog() = DialogResult.Cancel Then Return

                rarLocation = OpenFileDialog1.FileName
                strVersion = OpenFileDialog1.SafeFileName
                strVersion = Path.GetFileNameWithoutExtension(strVersion)
            End If

            Application.DoEvents()

            Helper.DeleteDirectory("TEMPPATCHAIDAFOOL")
            Directory.CreateDirectory("TEMPPATCHAIDAFOOL")
            Dim startInfo As New ProcessStartInfo() With {.FileName = (Program.StartPath & "\unrar.exe"), .Verb = "runas", .WindowStyle = ProcessWindowStyle.Normal, .UseShellExecute = True}
            If predownloadedyesno = MsgBoxResult.No Then startInfo.Arguments = ("e " & patchFile & " TEMPPATCHAIDAFOOL")
            If predownloadedyesno = MsgBoxResult.Yes Then startInfo.Arguments = ("e " & """" & rarLocation & """" & " TEMPPATCHAIDAFOOL")

            Helper.WriteDebugInfo(Resources.strWaitingforPatch)
            Process.Start(startInfo).WaitForExit()

            If Not Directory.Exists("TEMPPATCHAIDAFOOL") Then
                Directory.CreateDirectory("TEMPPATCHAIDAFOOL")
                Helper.WriteDebugInfo("Had to manually make temp update folder - Did the patch not extract right?")
            End If
            Dim diar1 As FileInfo() = New DirectoryInfo("TEMPPATCHAIDAFOOL").GetFiles()
            Helper.WriteDebugInfoAndOk((Resources.strExtractingTo & Program.Pso2WinDir))
            Application.DoEvents()
            If _cancelledFull Then Return

            Dim backupPath As String = BuildBackupPath(patchName)
            If backupyesno = MsgBoxResult.Yes Then
                If Directory.Exists(backupPath) Then
                    Directory.Delete(backupPath, True)
                    Directory.CreateDirectory(backupPath)
                    Helper.WriteDebugInfo(Resources.strErasingPreviousBackup)
                End If
                If Not Directory.Exists(backupPath) Then
                    Directory.CreateDirectory(backupPath)
                    Helper.WriteDebugInfo(Resources.strCreatingBackupDirectory)
                End If
            End If

            Helper.Log("Extracted " & diar1.Length & " files from the patch")
            If diar1.Length = 0 Then
                Helper.WriteDebugInfo("Patch failed to extract correctly! Installation failed!")
                Return
            End If

            Helper.WriteDebugInfo(Resources.strInstallingPatch)

            For Each dra As FileInfo In diar1
                If _cancelledFull Then Return
                If backupyesno = MsgBoxResult.Yes Then
                    If File.Exists((Program.Pso2WinDir & "\" & dra.ToString())) Then
                        File.Move((Program.Pso2WinDir & "\" & dra.ToString()), (backupPath & "\" & dra.ToString()))
                    End If
                End If
                If backupyesno = MsgBoxResult.No Then
                    If File.Exists((Program.Pso2WinDir & "\" & dra.ToString())) Then
                        Helper.DeleteFile((Program.Pso2WinDir & "\" & dra.ToString()))
                    End If
                End If
                File.Move(("TEMPPATCHAIDAFOOL\" & dra.ToString()), (Program.Pso2WinDir & "\" & dra.ToString()))
            Next

            Helper.DeleteDirectory("TEMPPATCHAIDAFOOL")
            If backupyesno = MsgBoxResult.No Then
                External.FlashWindow(Handle, True)
                Helper.WriteDebugInfo("English patch " & Resources.strInstalledUpdated)
                If Not String.IsNullOrEmpty(versionStr) Then RegKey.SetValue(Of String)(versionStr, strVersion)
            End If
            If backupyesno = MsgBoxResult.Yes Then
                External.FlashWindow(Handle, True)
                Helper.WriteDebugInfo(("English patch " & Resources.strInstalledUpdatedBackup & backupPath))
                If Not String.IsNullOrEmpty(versionStr) Then RegKey.SetValue(Of String)(versionStr, strVersion)
            End If
            Helper.DeleteFile(patchName)
            UnlockGui()
        Catch ex As Exception
            Helper.Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            Helper.WriteDebugInfo(Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub UninstallPatch(patchListUrl As String, patchListFile As String, patchName As String, consoleMsg As String, patchVersionKey As String)
        Try
            If IsPso2WinDirMissing() Then Return

            DownloadFile(patchListUrl, patchListFile)
            Dim missingfiles = File.ReadAllLines(patchListFile)
            Helper.DeleteFile(patchListFile)
            Helper.WriteDebugInfo(Resources.strUninstallingPatch)

            For index As Integer = 0 To (missingfiles.Length - 1)
                If _cancelledFull Then Return

                'Download JP file
                lblStatus.Text = Resources.strUninstalling & index & "/" & missingfiles.Length
                DownloadFile(("http://download.pso2.jp/patch_prod/patches/data/win32/" & missingfiles(index) & ".pat"), missingfiles(index))
                If New FileInfo(missingfiles(index)).Length = 0 Then DownloadFile(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & missingfiles(index) & ".pat"), missingfiles(index))

                'Move JP file to win32
                Helper.DeleteFile((Program.Pso2WinDir & "\" & missingfiles(index)))
                File.Move(missingfiles(index), (Program.Pso2WinDir & "\" & missingfiles(index)))
            Next

            Helper.DeleteDirectory(BuildBackupPath(patchName))
            External.FlashWindow(Handle, True)
            Helper.WriteDebugInfo(consoleMsg)
            RegKey.SetValue(Of String)(patchVersionKey, "Not Installed")
            UnlockGui()
        Catch ex As Exception
            Helper.Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            Helper.WriteDebugInfo(Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub RestoreBackup(patchName As String)
        Dim backupPath As String = BuildBackupPath(patchName)
        If Directory.Exists(backupPath) = False Then Return

        Dim di As New DirectoryInfo(backupPath)
        Helper.WriteDebugInfoAndOk("Restoring " & patchName & " backup...")
        Application.DoEvents()

        'list the names of all files in the specified directory
        For Each dra As FileInfo In di.GetFiles()
            If File.Exists(Program.Pso2WinDir & "\" & dra.ToString()) Then
                Helper.DeleteFile(Program.Pso2WinDir & "\" & dra.ToString())
            End If
            File.Move(backupPath & "\" & dra.ToString(), Program.Pso2WinDir & "\" & dra.ToString())
        Next

        Helper.DeleteDirectory(backupPath)
        External.FlashWindow(Handle, True)

        Select Case patchName
            Case EnglishPatch
                If Not String.IsNullOrEmpty(RegKey.EnPatchVersion) Then RegKey.SetValue(Of String)(RegKey.EnPatchVersion, "Not Installed")
            Case LargeFiles
                If Not String.IsNullOrEmpty(RegKey.LargeFilesVersion) Then RegKey.SetValue(Of String)(RegKey.LargeFilesVersion, "Not Installed")
            Case StoryPatch
                If Not String.IsNullOrEmpty(RegKey.StoryPatchVersion) Then RegKey.SetValue(Of String)(RegKey.StoryPatchVersion, "Not Installed")
        End Select
        'WriteDebugInfoSameLine(" Done!")
        UnlockGui()
    End Sub

    Private Shared Function BuildBackupPath(ByVal patchName As String) As String
        Return Program.Pso2WinDir & "\backup\" & patchName & "\"
    End Function

    Private Sub tmrWaitingforPSO2_Tick(sender As Object, e As EventArgs) Handles tmrWaitingforPSO2.Tick
        tmrWaitingforPSO2.Enabled = False

        Dim YesNo As MsgBoxResult = MsgBox("PSO2 doesn't appear to have opened yet. This can be caused by Gameguard Issues. Would you like the Tweaker to try to solve the issues and relaunch the game?", vbYesNo)
        If YesNo = MsgBoxResult.No Then
            tmrWaitingforPSO2.Enabled = False
            Exit Sub
        End If
        If YesNo = MsgBoxResult.Yes Then
            SkipDialogs = True
            btnGameguard.RaiseClick()
        End If
    End Sub

    Private Sub btnLargeFilesTRANSAM_Click(sender As Object, e As EventArgs) Handles btnLargeFilesTRANSAM.Click
        'Install Large Files with TRANSAM to cut down on net costs for Agrajag and friends.
        'Need to speak with Agrajag and get some files before I can do this, though.
        Try
            If IsPso2WinDirMissing() Then Return

            ' Create a match using regular exp<b></b>ressions
            ' Spit out the value plucked from the code
            Dim LFDate As String = Program.Client.DownloadString(Program.FreedomUrl & "patches/largefilesTRANSAM.txt")

            Helper.WriteDebugInfoAndOk("Downloading Large Files info... ")
            DownloadFile(Program.FreedomUrl & "patches/lf.stripped.db.7z", "lf.stripped.db.7z")
            Dim processStartInfo2 As New ProcessStartInfo With
            {
                .FileName = (Program.StartPath & "\7za.exe"),
                .Verb = "runas",
                .Arguments = ("e -y lf.stripped.db.7z"),
                .WindowStyle = ProcessWindowStyle.Hidden,
            .UseShellExecute = True
            }
            Process.Start(processStartInfo2).WaitForExit()
            Dim DBMD5 As String = Helper.GetMd5("lf.stripped.db")
            Helper.WriteDebugInfoAndOk("Downloading Trans-Am tool... ")
            DownloadFile(Program.FreedomUrl & "pso2-transam.exe", "pso2-transam.exe")

            'execute pso2-transam stuff with -b flag for backup
            Dim processStartInfo As ProcessStartInfo = New ProcessStartInfo() With {.FileName = "pso2-transam.exe", .Verb = "runas"}
            'If Directory.Exists(backupdir) Then
            'Dim counter = Computer.FileSystem.GetFiles(backupdir)
            'If counter.Count > 0 Then
            processStartInfo.Arguments = ("-t largefiles-" & LFDate & " lf.stripped.db " & """" & Program.Pso2WinDir & """")
            Clipboard.SetText(processStartInfo.Arguments)
            'Else
            'Helper.Log("[TRANSAM] Creating backup directory")
            'Directory.CreateDirectory(backupdir)
            'Helper.WriteDebugInfo(Resources.strCreatingBackupDirectory)
            'processStartInfo.Arguments = ("-b " & """" & backupdir & """" & " -t story-eng-" & strStoryPatchLatestBase & " pso2.stripped.db " & """" & Program.Pso2WinDir & """")
            'End If
            'End If

            'We don't need to make backups anymore
            'If Not Directory.Exists(backupdir) Then
            ' Helper.Log("[TRANSAM] Creating backup directory")
            ' Directory.CreateDirectory(backupdir)
            ' Helper.WriteDebugInfo(Resources.strCreatingBackupDirectory)
            ' processStartInfo.Arguments = ("-b " & """" & backupdir & """" & " -t story-eng-" & strStoryPatchLatestBase & " pso2.stripped.db " & """" & Program.Pso2WinDir & """")
            ' End If

            processStartInfo.UseShellExecute = False
            Helper.Log("[TRANSAM] Starting shitstorm")
            processStartInfo.Arguments = processStartInfo.Arguments.Replace("\", "/")
            Helper.Log("TRANSM parameters: " & processStartInfo.Arguments & vbCrLf & "TRANSAM Working Directory: " & processStartInfo.WorkingDirectory)
            Helper.Log("[TRANSAM] Program started")
            If Helper.GetMd5("lf.stripped.db") <> DBMD5 Then
                MsgBox("ERROR: Files have been modified since download. Aborting...")
                Exit Sub
            End If
            Process.Start(processStartInfo).WaitForExit()
            Helper.DeleteFile("lf.stripped.db")
            Helper.DeleteFile("pso2-transam.exe")
            External.FlashWindow(Handle, True)
            'Story Patch 3-12-2014.rar
            RegKey.SetValue(Of String)(RegKey.LargeFilesVersion, LFDate.Replace("-", "/"))
            Helper.WriteDebugInfo("Large Files Patch installed!")
        Catch ex As Exception
            MsgBox("ERROR - " & ex.Message.ToString)
        End Try
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs)

    End Sub
End Class