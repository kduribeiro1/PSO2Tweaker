Imports DevComponents.DotNetBar
Imports Microsoft.Win32
Imports System.IO
Imports System.Management
Imports System.Net
Imports System.Runtime.Serialization.Json
Imports System.Security.Principal
Imports System.Text.RegularExpressions
Imports System.Threading

' TODO: Replace all redundant code with functions
' TODO: Every instance of file downloading that retries ~5 times should be a function. I didn't realize there were so many.

Public Class FrmMain
    ReadOnly _pso2OptionsFrm As frmPSO2Options
    ReadOnly _args As String() = Environment.GetCommandLineArgs()
    ReadOnly _hostsFilePath As String = Environment.SystemDirectory & "\drivers\etc\hosts"
    ReadOnly _optionsFrm As frmOptions
    ReadOnly _startPath As String = Application.StartupPath

    Dim _cancelled As Boolean
    Dim _cancelledFull As Boolean
    Dim _comingFromOldFiles As Boolean = False
    Dim _dpiSetting As Single
    Dim _freedomUrl As String = "http://162.243.211.123/freedom/"
    Dim _itemDownloadingDone As Boolean
    Dim _mileyCyrus As Integer
    Dim _override As Boolean = False
    Dim _restartplz As Boolean
    Dim _someoftheprepatches As Dictionary(Of String, String)
    Dim _someofthethings As Dictionary(Of String, String)
    Dim _systemUnlock As Integer
    Dim _transOverride As Boolean = False
    Dim _useItemTranslation As Boolean = False
    Dim _vedaUnlocked As Boolean = False
    Dim _wayuIsAFailure As Boolean = False
    Dim _nodiag As Boolean = False
    Dim _processes As Process()
    Dim _pso2RootDir As String
    Dim _pso2WinDir As String
    Dim _totalsize2 As Long

    Sub New()
        Dim locale = RegKey.GetValue(Of String)(RegKey.Locale)

        If Not String.IsNullOrEmpty(locale) Then
            Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(locale)
            Thread.CurrentThread.CurrentCulture = New Globalization.CultureInfo(locale)
        End If

        _pso2OptionsFrm = New frmPSO2Options()
        _optionsFrm = New frmOptions()

        InitializeComponent()

        'Yo, fuck this shit. Shit is mad whack, yo.
        SuspendLayout()
        chkRemoveCensor.Text = My.Resources.strRemoveCensorFile
        chkRemovePC.Text = My.Resources.strRemovePCOpening
        chkRemoveVita.Text = My.Resources.strRemoveVitaOpening
        chkRemoveNVidia.Text = My.Resources.strRemoveNVidiaVideo
        chkRemoveSEGA.Text = My.Resources.strRemoveSEGALogoVideo
        chkSwapOP.Text = My.Resources.strSwapPCVitaOpenings
        chkRestoreCensor.Text = My.Resources.strRestoreCensorFile
        chkRestorePC.Text = My.Resources.strRestorePCOpeningVideo
        chkRestoreVita.Text = My.Resources.strRestoreVitaOpeningVideo
        chkRestoreNVidia.Text = My.Resources.strRestoreNVidiaLogo
        chkRestoreSEGA.Text = My.Resources.strRestoreSEGALogoVideo
        lblDirectoryLabel.Text = My.Resources.strCurrentlyselecteddirectory
        lblStatus.Text = My.Resources.strWaitingforacommand
        btnSelectPSODir.Text = My.Resources.strSelectPSO2win32folder
        ButtonInstall.Text = My.Resources.strInstallUpdatePatches
        btnRestoreBackups.Text = "Restore Backup of JP Files"
        btnApplyChanges.Text = My.Resources.strApplySelectedChanges
        CancelDownloadToolStripMenuItem.Text = My.Resources.strCancelCurrentDownload
        ButtonItem5.Text = My.Resources.strCheckforPSO2Updates
        btnLaunchPSO2.Text = My.Resources.strLaunchPSO2
        btnFixPSO2EXEs.Text = My.Resources.strFixPSO2EXEs
        btnFixPermissions.Text = My.Resources.strFixPSO2Permissions
        lblORBLabel.Text = My.Resources.strClickOrb
        rtbDebug.Text = My.Resources.strProgramStarted
        ButtonItem1.Text = "Redownload Original JP Files"
        ButtonItem2.Text = My.Resources.strTroubleshooting
        btnOtherStuff.Text = My.Resources.strOtherTasks
        ButtonItem3.Text = My.Resources.strWebLinks
        btnCheckForStoryUpdates.Text = My.Resources.strCheckForStoryPatchUpdates
        chkAlwaysOnTop.Text = My.Resources.strAlwaysonTop
        chkItemTranslation.Text = My.Resources.strTranslateItems
        btnPSO2Options.Text = My.Resources.strPSO2Options
        btnOptions.Text = My.Resources.strOptions
        btnExit.Text = My.Resources.strExit
        btnAnalyze.Text = My.Resources.strAnalyzeInstallforIssues
        Button2.Text = My.Resources.strCheckforDeletedMissingFiles
        ButtonItem10.Text = My.Resources.strCheckForOldMissingFiles
        btnGameguard.Text = My.Resources.strFixGameguardErrors
        ButtonItem17.Text = My.Resources.strResetPSO2Settings
        btnResumePatching.Text = My.Resources.strResumePatching
        btnTerminate.Text = My.Resources.strTerminate
        ButtonItem7.Text = My.Resources.strLaunchChrome

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
                StyleManager.Style = eStyle.Office2007Blue
        End Select
        ResumeLayout(False)
    End Sub

    Private Sub frmMain_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        Application.Exit()
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
                frmVEDA.Show()
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
                Text = ("PSO2 Twerker ver " & My.Application.Info.Version.ToString())
                btnLaunchPSO2.Text = "Twerk it!"
                chkItemTranslation.Text = "Twerk on Robin Thicke"
            End If
        End If
    End Sub

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
#If DEBUG Then
        btnNewShit.Visible = True
#End If

        ' TODO: Do we really need DPI stuff?
        Using g As Graphics = CreateGraphics()
            If g.DpiX = 120 OrElse g.DpiX = 96 Then
                _dpiSetting = g.DpiX
            End If
        End Using
        'Attach the thread handler here
        'Thread.Cur()

        Try
            btnAnnouncements.Text = ">"
            Dim procs As Process()
            Log("Program started! - Logging enabled!")

            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.PSO2Dir)) Then
                Dim alreadyInstalled As MsgBoxResult = MsgBox("This appears to be the first time you've used the PSO2 Tweaker! Have you installed PSO2 already? If you select no, the PSO2 Tweaker will install it for you.", MsgBoxStyle.YesNo)
                If alreadyInstalled = vbNo Then
                    btnInstallPSO2.RaiseClick()
                End If
            End If

            Log("Attempting to auto-load pso2_bin directory from settings")
            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.PSO2Dir)) Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                SelectPSO2Directory()
            Else
                lblDirectory.Text = RegKey.GetValue(Of String)(RegKey.PSO2Dir)
                Log("Loaded pso2_bin directory from settings")
            End If

            ' This sets up pso2RootDir and pso2WinDir - don't remove it
            If lblDirectory.Text.Contains("\pso2_bin\data\win32") Then
                If File.Exists(lblDirectory.Text.Replace("\data\win32", "") & "\pso2.exe") Then
                    Log("win32 folder selected instead of pso2_bin folder - Fixing!")
                    lblDirectory.Text = lblDirectory.Text.Replace("\data\win32", "")
                    RegKey.SetValue(Of String)(RegKey.PSO2Dir, lblDirectory.Text)
                    Log(lblDirectory.Text & " " & My.Resources.strSetAsYourPSO2)
                End If
            End If

            If Not Directory.Exists(lblDirectory.Text) OrElse lblDirectory.Text = "lblDirectory" Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                SelectPSO2Directory()
            End If

            _pso2RootDir = lblDirectory.Text
            _pso2WinDir = (_pso2RootDir & "\data\win32")

            If Not Directory.Exists(_pso2WinDir) Then
                Directory.CreateDirectory(_pso2WinDir)
                'WriteDebugInfo("Creating win32 directory... Done!")
            End If

            DeleteFile(_pso2RootDir & "\ddraw.dll")

            For i As Integer = 1 To _args.Length - 1
                Select Case _args(i)
                    Case "-nodllcheck"
                        _transOverride = True

                    Case "-fuck_you_misaki_stop_trying_to_decompile_my_shit"
                        Log("Fuck you, Misaki")
                        MsgBox("Why are you trying to decompile my program? Get outta here!")

                    Case "-item"
                        Log("Detected command argument -item")
                        _useItemTranslation = True

                    Case "-wayu"
                        Log("Detected command argument -wayu")
                        _wayuIsAFailure = True

                    Case "-nodiag"
                        Log("Detected command argument -nodiag")
                        Log("Bypassing OS detection to fix compatibility!")
                        _nodiag = True

                    Case "-bypass"
                        Log("Detected command argument -bypass")
                        Log("Emergency bypass mode activated - Please only use this mode if the Tweaker will not start normally!")
                        MsgBox("Emergency bypass mode activated - Please only use this mode if the Tweaker will not start normally!")
                        If Not Directory.Exists(_pso2RootDir) OrElse _pso2RootDir = "lblDirectory" Then
                            MsgBox(My.Resources.strPleaseSelectwin32Dir)
                            SelectPSO2Directory()
                            Exit Sub
                        End If
                        File.WriteAllBytes(_pso2RootDir & "\ddraw.dll", My.Resources.ddraw)
                        Log("Setting environment variable")
                        Environment.SetEnvironmentVariable("-pso2", "+0x01e3f1e9")
                        Log("Launching PSO2")
                        External.ShellExecute(Handle, "open", (_pso2RootDir & "\pso2.exe"), "+0x33aca2b9 -pso2", "", 0)
                        Hide()
                        Do Until File.Exists(_pso2RootDir & "\ddraw.dll") = False
                            procs = Process.GetProcessesByName("pso2")
                            For Each proc As Process In procs
                                If proc.MainWindowTitle = "Phantasy Star Online 2" AndAlso proc.MainModule.ToString() = "ProcessModule (pso2.exe)" Then
                                    If Not _transOverride Then DeleteFile(_pso2RootDir & "\ddraw.dll")
                                End If
                            Next
                            Thread.Sleep(1000)
                        Loop
                        Close()

                    Case "-pso2"
                        Log("Detected command argument -pso2")

                        'Fuck SEGA. Fuck them hard.
                        If Not Directory.Exists(_pso2RootDir) OrElse _pso2RootDir = "lblDirectory" Then
                            MsgBox(My.Resources.strPleaseSelectwin32Dir)
                            SelectPSO2Directory()
                            Exit Sub
                        End If

                        If _useItemTranslation Then
                            'Why did we ever delete the item cache? [AIDA]
                            'Dim dir As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                            'Log("Deleting item cache...")
                            'DeleteFile(dir & "\SEGA\PHANTASYSTARONLINE2\item_name_cache.dat")

                            'Download the latest translator.dll and translation.bin
                            Dim dlLink1 As String = _freedomUrl & "translator.dll"
                            Dim dlLink2 As String = _freedomUrl & "translation.bin"
                            Log(My.Resources.strDownloadingItemTranslationFiles)
                            Dim client As New WebClient

                            ' Try up to 4 times to download the translator DLL.
                            For tries As Integer = 1 To 4
                                Try
                                    client.DownloadFile(dlLink1, (_pso2RootDir & "\translator.dll"))
                                    Exit For
                                Catch ex As Exception
                                    If tries = 4 Then
                                        Log("Failed to download translation files! (" & ex.Message.ToString & "). Try rebooting your computer or making sure PSO2 isn't open.")
                                        Exit For
                                    End If
                                End Try
                            Next

                            ' Try up to 4 times to download the translation strings.
                            For tries As Integer = 1 To 4
                                Try
                                    client.DownloadFile(dlLink2, (_pso2RootDir & "\translation.bin"))
                                    Exit For
                                Catch ex As Exception
                                    If tries = 4 Then
                                        Log("Failed to download translation files! (" & ex.Message.ToString & "). Try rebooting your computer or making sure PSO2 isn't open.")
                                        Exit Try
                                    End If
                                End Try
                            Next

                            File.WriteAllBytes(_pso2RootDir & "\ddraw.dll", My.Resources.ddraw)
                        End If

                        Log("Setting environment variable")
                        Environment.SetEnvironmentVariable("-pso2", "+0x01e3f1e9")

                        Log("Launching PSO2")
                        External.ShellExecute(Handle, "open", (_pso2RootDir & "\pso2.exe"), "+0x33aca2b9 -pso2", "", 0)

                        DeleteFile("LanguagePack.rar")
                        If _useItemTranslation Then
                            Hide()
                            Do Until File.Exists(_pso2RootDir & "\ddraw.dll") = False
                                procs = Process.GetProcessesByName("pso2")
                                For Each proc As Process In procs
                                    If proc.MainWindowTitle = "Phantasy Star Online 2" AndAlso proc.MainModule.ToString() = "ProcessModule (pso2.exe)" Then
                                        If Not _transOverride Then DeleteFile(_pso2RootDir & "\ddraw.dll")
                                    End If
                                Next
                                Thread.Sleep(1000)
                            Loop
                        End If

                        Close()
                End Select
            Next

            'Normal Tweaker startup
            _cancelledFull = False
            If File.Exists(_pso2RootDir & "\ddraw.dll") AndAlso (Not _transOverride) Then DeleteFile(_pso2RootDir & "\ddraw.dll")
            Log("Starting shitstorm...")
            Dim justice As MyWebClient = New MyWebClient() With {.timeout = 10000, .Proxy = Nothing}
            _freedomUrl = justice.DownloadString("http://arks-layer.com/freedom.txt")
            If _freedomUrl.Contains("freedom") = False Then
                Log("Reverting to default freedom...")
                _freedomUrl = "http://162.243.211.123/freedom/"
            End If

            Log("Loading settings...")

            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.PatchServer)) Then RegKey.SetValue(Of String)(RegKey.PatchServer, "Patch Server #1")
            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.SeenFuckSEGAMessage)) Then RegKey.SetValue(Of Boolean)(RegKey.SeenFuckSEGAMessage, False)
            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.Backup)) Then RegKey.SetValue(Of String)(RegKey.Backup, "Always")
            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.PreDownloadedRAR)) Then RegKey.SetValue(Of String)(RegKey.PreDownloadedRAR, "Never")
            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.Pastebin)) Then RegKey.SetValue(Of Boolean)(RegKey.Pastebin, True)
            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.CloseAfterLaunch)) Then RegKey.SetValue(Of Boolean)(RegKey.CloseAfterLaunch, False)
            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.ENPatchAfterInstall)) Then RegKey.SetValue(Of Boolean)(RegKey.ENPatchAfterInstall, False)
            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.LargeFilesAfterInstall)) Then RegKey.SetValue(Of Boolean)(RegKey.LargeFilesAfterInstall, False)
            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.StoryPatchAfterInstall)) Then RegKey.SetValue(Of Boolean)(RegKey.StoryPatchAfterInstall, False)
            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.LatestStoryBase)) Then RegKey.SetValue(Of String)(RegKey.LatestStoryBase, "Unknown")
            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.ProxyEnabled)) Then RegKey.SetValue(Of Boolean)(RegKey.ProxyEnabled, False)
            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.SteamMode)) Then RegKey.SetValue(Of String)(RegKey.SteamMode, "False")
            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.SidebarEnabled)) Then RegKey.SetValue(Of String)(RegKey.SidebarEnabled, "True")

            If RegKey.GetValue(Of String)(RegKey.SidebarEnabled) = "False" Then
                btnAnnouncements.PerformClick()
            End If

            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.UID)) Then RegKey.SetValue(Of Boolean)(RegKey.UID, False)

            If RegKey.GetValue(Of String)(RegKey.UID) = "False" Then
                Dim client As New WebClient()
                Dim uidStr As String = client.DownloadString("http://arks-layer.com/docs/client.php")
                RegKey.SetValue(Of String)(RegKey.Uid, uidStr)
            End If

            Log("Load more settings...")
            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.StoryPatchVersion)) Then RegKey.SetValue(Of String)(RegKey.StoryPatchVersion, "Not Installed")
            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.ENPatchVersion)) Then RegKey.SetValue(Of String)(RegKey.ENPatchVersion, "Not Installed")
            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.LargeFilesVersion)) Then RegKey.SetValue(Of String)(RegKey.LargeFilesVersion, "Not Installed")

            Log("Loading color stuff...")

            Dim regValue As Integer

            regValue = RegKey.GetValue(Of Integer)(RegKey.TextBoxBGColor)
            If regValue = 0 Then RegKey.SetValue(Of UInteger)(RegKey.TextBoxBGColor, 4294967295)
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

            PBMainBar.Text = ""
            Log("Checking if the PSO2 Tweaker is running")

            If CheckIfRunning("PSO2 Tweaker") Then
                Application.ExitThread()
            End If

            Text = ("PSO2 Tweaker ver " & My.Application.Info.Version.ToString())
            Application.DoEvents()
            Show()

            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.SeenDownloadMessage)) Then RegKey.SetValue(Of String)(RegKey.SeenDownloadMessage, "No")

            If _startPath = Helper.GetDownloadsPath() Then
                If RegKey.GetValue(Of String)(RegKey.SeenDownloadMessage) = "No" Then
                    MsgBox("Please be aware - Due to various Windows 7/8 issues, this program might not work correctly while in the ""Downloads"" folder. Please move it to it's own folder, like C:\Tweaker\")
                    RegKey.SetValue(Of String)(RegKey.SeenDownloadMessage, "Yes")
                End If
            End If

            LockGui()

            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.AlwaysOnTop)) Then RegKey.SetValue(Of Boolean)(RegKey.AlwaysOnTop, False)
            Dim isTopMost = Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.AlwaysOnTop))
            TopMost = isTopMost
            chkAlwaysOnTop.Checked = isTopMost

            If File.Exists((_startPath & "\logfile.txt")) AndAlso Helper.GetFileSize((_startPath & "\logfile.txt")) > 30720 Then
                File.WriteAllText((_startPath & "\logfile.txt"), "")
            End If

            Application.DoEvents()

            If _nodiag Then
                Log("Diagnostic info skipped due to -nodiag flag!")
            Else
                Log(vbCrLf)
                Log("----------------------------------------")
                Log(My.Resources.strProgramOpeningRunningDiagnostics)
                Log(My.Resources.strCurrentOSFullName & My.Computer.Info.OSFullName)
                Log(My.Resources.strCurrentOSVersion & My.Computer.Info.OSVersion)
                Log(My.Resources.strIsTheCurrentOS64bit & Environment.Is64BitOperatingSystem)
                Log(My.Resources.strRunDirectory & _startPath)
                Log(My.Resources.strSelectedPSO2win32directory & _pso2RootDir)
                Log(My.Resources.strIsUnrarAvailable & File.Exists(_startPath & "\UnRar.exe"))
                Dim identity = WindowsIdentity.GetCurrent()
                Dim principal = New WindowsPrincipal(identity)
                Dim isElevated As Boolean = principal.IsInRole(WindowsBuiltInRole.Administrator)
                Log("Run as Administrator: " & isElevated)
                Log("Is 7zip available: " & File.Exists(_startPath & "\7za.exe"))
                Log("Is 7zip available: " & File.Exists("7za.exe"))
                Log("----------------------------------------")
            End If

            WriteDebugInfoAndOK((My.Resources.strProgramOpeningSuccessfully & My.Application.Info.Version.ToString()))
            Application.DoEvents()

        Catch ex As Exception
            Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try

        Try
            DeleteFile(_startPath & "\version.xml")
            WriteDebugInfo(My.Resources.strCheckingforupdatesPleasewaitamoment)
            Dim wc As MyWebClient = New MyWebClient() With {.timeout = 10000, .Proxy = Nothing}
            Dim source = wc.DownloadString(_freedomUrl & "version.xml")

            If source.Contains("<VersionHistory>") Then
                Dim xm As New Xml.XmlDocument
                xm.LoadXml(source)

                Dim xmlNode = xm.SelectSingleNode("//CurrentVersion")
                Dim currentVersion As String = xmlNode.ChildNodes(0).InnerText.Trim

                Log("Checking for the latest version of the program...")

                If My.Application.Info.Version.ToString() = currentVersion Then
                    WriteDebugInfo((My.Resources.strYouhavethelatestversionoftheprogram & My.Application.Info.Version.ToString()))
                Else
                    Dim changelogtotal As String = ""

                    For index As Integer = 2 To 11
                        Dim innerText = xmlNode.ChildNodes(index).InnerText.Trim
                        If Not String.IsNullOrWhiteSpace(innerText) Then changelogtotal &= vbCrLf & innerText
                    Next

                    Dim updateyesno As MsgBoxResult = MsgBox(My.Resources.strYouareusinganoutdatedversionoftheprogram & My.Application.Info.Version.ToString() & My.Resources.strAndthelatestis & currentVersion & My.Resources.strWouldyouliketodownloadthenewversion & vbCrLf & vbCrLf & My.Resources.strChanges & vbCrLf & changelogtotal, MsgBoxStyle.YesNo)
                    If updateyesno = MsgBoxResult.Yes Then
                        WriteDebugInfo(My.Resources.strDownloadingUpdate)
                        DLWUA(_freedomUrl & "PSO2%20Tweaker%20Updater.exe", "PSO2 Tweaker Updater.exe")
                        Process.Start(Environment.CurrentDirectory & "\PSO2 Tweaker Updater.exe")
                        Application.DoEvents()
                        Exit Sub
                    End If
                End If
            End If

            If Not source.Contains("<VersionHistory>") Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToGetUpdateInfo)
            End If
        Catch ex As Exception
            Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try

        Try
            Application.DoEvents()

            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.PSO2Dir)) Then
                MsgBox(My.Resources.strPleaseSelectPSO2win32dir)
                SelectPSO2Directory()
            Else
                _pso2RootDir = RegKey.GetValue(Of String)(RegKey.PSO2Dir)
            End If

            If File.Exists((_pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) AndAlso File.Exists((_pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585")) Then
                If Helper.GetFileSize((_pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) = 167479840 AndAlso Helper.GetFileSize((_pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585")) = 151540352 Then
                    chkSwapOP.Text = My.Resources.strSwapPCVitaOpenings & " (" & My.Resources.strNotSwapped & ")"
                ElseIf Helper.GetFileSize((_pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) = 151540352 AndAlso Helper.GetFileSize((_pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585")) = 167479840 Then
                    chkSwapOP.Text = My.Resources.strSwapPCVitaOpenings & " (" & My.Resources.strSwapped & ")"
                End If
            End If

            ' Shouldn't be doing this in this way
            Application.DoEvents()

            If Not File.Exists("7za.exe") Then
                WriteDebugInfo(My.Resources.strDownloading & "7za.exe...")
                Application.DoEvents()
                DLWUA(_freedomUrl & "7za.exe", "7za.exe")
            End If

            For index = 1 To 5
                If Helper.GetMD5("7za.exe") <> "42BADC1D2F03A8B1E4875740D3D49336" Then
                    WriteDebugInfo(My.Resources.strYour7zipiscorrupt)
                    Application.DoEvents()
                    DLWUA(_freedomUrl & "7za.exe", "7za.exe")
                Else
                    Exit For
                End If
            Next

            If Not File.Exists("UnRar.exe") Then
                WriteDebugInfo(My.Resources.strDownloading & "UnRar.exe...")
                Application.DoEvents()
                DLWUA(_freedomUrl & "UnRAR.exe", "UnRAR.exe")
            End If

            For index = 1 To 5
                If Helper.GetMD5("UnRar.exe") <> "0C83C1293723A682577E3D0B21562B79" Then
                    WriteDebugInfo(My.Resources.strYourUnrariscorrupt)
                    Application.DoEvents()
                    DLWUA(_freedomUrl & "UnRAR.exe", "UnRAR.exe")
                Else
                    Exit For
                End If
            Next

            For index = 1 To 3
                If Directory.Exists("TEMPSTORYAIDAFOOL") Then
                    Directory.Delete("TEMPSTORYAIDAFOOL", True)
                Else
                    Exit For
                End If
            Next

            DeleteFile("launcherlist.txt")
            DeleteFile("patchlist.txt")
            DeleteFile("patchlist_old.txt")

            'Added in precede files. Stupid ass SEGA.
            DeleteFile("patchlist0.txt")
            DeleteFile("patchlist1.txt")
            DeleteFile("patchlist2.txt")
            DeleteFile("patchlist3.txt")
            DeleteFile("patchlist4.txt")
            DeleteFile("patchlist5.txt")
            DeleteFile("precede.txt")
            DeleteFile("ServerConfig.txt")
            DeleteFile("SOMEOFTHETHINGS.txt")
            DeleteFile("ALLOFTHETHINGS.txt")
            DeleteFile("SOMEOFTHEPREPATCHES.txt")
            DeleteFile("ALLOFTHEPREPATCHES.txt")
            DeleteFile("precede_apply.txt")
            DeleteFile("version.ver")
            DeleteFile("Story MD5HashList.txt")

            UnlockGUI()
            btnLaunchPSO2.Enabled = False

            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.SidebarEnabled)) Then RegKey.SetValue(Of Boolean)(RegKey.SidebarEnabled, True)
            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.RemoveCensor)) Then RegKey.SetValue(Of Boolean)(RegKey.RemoveCensor, True)

            If RegKey.GetValue(Of String)(RegKey.SidebarEnabled) = "True" Then
                WriteDebugInfo(My.Resources.strLoadingSidebar)
                ThreadPool.QueueUserWorkItem(AddressOf LoadSidebar, Nothing)

                If _dpiSetting = 96 Then Width = 796
                If _dpiSetting = 120 Then Width = 1060
                btnAnnouncements.Text = "<"
            End If

            If File.Exists("resume.txt") Then
                Dim yesNoResume As MsgBoxResult = MsgBox("It seems that the last patching attempt was interrupted. Would you like to resume patching?", vbYesNo)
                If yesNoResume = MsgBoxResult.Yes Then
                    btnResumePatching.RaiseClick()
                Else
                    DeleteFile("resume.txt")
                End If
            End If

            WriteDebugInfo(My.Resources.strCheckingforPSO2Updates)
            Application.DoEvents()

            CheckForPSO2Updates(False)
            WriteDebugInfoSameLine(My.Resources.strDone)
            Application.DoEvents()

            'Check for PSO2 Updates here, download the version.ver thingie
            'Check for PSO2 EN Patch updates here, parse the URL and see if it's different from the saved one
            'Check for EN Story Patch
            WriteDebugInfo(My.Resources.strCheckingforUpdatestopatches)

            'Check for Story Patches (Done! :D)
            Application.DoEvents()
            CheckForStoryUpdates()
            WriteDebugInfo(My.Resources.strCurrentStoryPatchis & RegKey.GetValue(Of String)(RegKey.StoryPatchVersion))
            Application.DoEvents()

            'Check for English Patches (Done! :D)
            CheckForENPatchUpdates()
            WriteDebugInfo(My.Resources.strCurrentENPatchis & RegKey.GetValue(Of String)(RegKey.ENPatchVersion))
            Application.DoEvents()

            'Check for LargeFiles Update (Work-In-Progress!)
            CheckForLargeFilesUpdates()
            WriteDebugInfo(My.Resources.strCurrentLargeFilesis & RegKey.GetValue(Of String)(RegKey.LargeFilesVersion))
            Application.DoEvents()

            WriteDebugInfo(My.Resources.strIfAboveVersions)

            If _wayuIsAFailure Then
                WriteDebugInfo("Skipping downloads for Wayu!")
            Else
                If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.UseItemTranslation)) Then
                    RegKey.SetValue(Of Boolean)(RegKey.UseItemTranslation, True)
                End If

                _useItemTranslation = Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.UseItemTranslation))

                If _useItemTranslation Then
                    chkItemTranslation.Checked = True
                    WriteDebugInfo("Downloading latest item patch files...")
                    _itemDownloadingDone = False
                    ThreadPool.QueueUserWorkItem(AddressOf DownloadItemTranslationFiles, Nothing)

                    Do Until _itemDownloadingDone = True
                        Application.DoEvents()
                        Thread.Sleep(16)
                    Loop
                End If

                Dim hostname As IPHostEntry = Dns.GetHostEntry("gs001.pso2gs.net")
                Dim ip As IPAddress() = hostname.AddressList

                If Not ip(0).ToString().Contains("210.189.") AndAlso Not _itemDownloadingDone Then
                    WriteDebugInfo("PSO2Proxy usage detected! Downloading latest proxy file...")
                    _itemDownloadingDone = False
                    ThreadPool.QueueUserWorkItem(AddressOf DownloadItemTranslationFiles, Nothing)

                    Do Until _itemDownloadingDone = True
                        Application.DoEvents()
                        Thread.Sleep(16)
                    Loop

                    If Not File.Exists(_pso2RootDir & "\translation.cfg") Then
                        File.WriteAllText(_pso2RootDir & "\translation.cfg", "TranslationPath:translation.bin")
                    End If

                    Using reader As New StreamReader(_pso2RootDir & "\translation.cfg")
                        Dim builtFile As New List(Of String)
                        Dim currentLine As String

                        Do
                            currentLine = reader.ReadLine()
                            If (currentLine Is Nothing) Then Exit Do

                            If currentLine.Contains("TranslationPath:") Then currentLine = "TranslationPath:"
                            builtFile.Add(currentLine)
                        Loop

                        reader.Close()
                        File.WriteAllLines(_pso2RootDir & "\translation.cfg", builtFile.ToArray())
                    End Using
                End If
            End If

            WriteDebugInfoSameLine(My.Resources.strDone)

        Catch ex As Exception
            Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try

        DeleteFile("version.xml")
        DeleteFile("Story MD5HashList.txt")
        DeleteFile("PSO2 Tweaker Updater.exe")
        WriteDebugInfo(My.Resources.strAllDoneSystemReady)
        btnLaunchPSO2.Enabled = True
    End Sub

    Private Sub DownloadItemTranslationFiles(state As Object)
        Dim dlLink1 As String = _freedomUrl & "translator.dll"
        Dim dlLink2 As String = _freedomUrl & "translation.bin"
        Dim client As New WebClient

        Try
            client.DownloadFile(dlLink1, (_pso2RootDir & "\translator.dll"))
        Catch ex As Exception
            MsgBox("Failed to download translation files! (" & ex.Message.ToString & "). Try rebooting your computer or making sure PSO2 isn't open.")
        End Try

        RegKey.SetValue(Of String)(RegKey.Dllmd5, Helper.GetMD5(_pso2RootDir & "\translator.dll"))

        Try
            client.DownloadFile(dlLink2, (_pso2RootDir & "\translation.bin"))
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
            Dim timeFormatted As String = DateTime.Now.ToString("G")
            File.AppendAllText((_startPath & "\logfile.txt"), timeFormatted & " " & addThisText & vbCrLf)
        End If
    End Sub

    Private Sub WriteDebugInfoSameLine(ByVal addThisText As String)
        If rtbDebug.InvokeRequired Then
            rtbDebug.Invoke(New Action(Of String)(AddressOf WriteDebugInfoSameLine), Text)
        Else
            rtbDebug.Text &= (" " & addThisText)
            Dim timeFormatted As String = DateTime.Now.ToString("G")
            File.AppendAllText((_startPath & "\logfile.txt"), timeFormatted & " " & addThisText & vbCrLf)
        End If
    End Sub

    Private Sub WriteDebugInfoAndOk(ByVal addThisText As String)
        If rtbDebug.InvokeRequired Then
            rtbDebug.Invoke(New Action(Of String)(AddressOf WriteDebugInfoAndOk), Text)
        Else
            rtbDebug.Text &= (vbCrLf & addThisText)
            rtbDebug.Select(rtbDebug.TextLength, 0)
            rtbDebug.SelectionColor = Color.Green
            rtbDebug.AppendText(" [OK!]")
            rtbDebug.SelectionColor = rtbDebug.ForeColor
            Dim timeFormatted As String = DateTime.Now.ToString("G")
            File.AppendAllText((_startPath & "\logfile.txt"), timeFormatted & " " & (addThisText & " [OK!]") & vbCrLf)
        End If
    End Sub

    Private Sub WriteDebugInfoAndWarning(ByVal addThisText As String)
        If rtbDebug.InvokeRequired Then
            rtbDebug.Invoke(New Action(Of String)(AddressOf WriteDebugInfoAndWarning), Text)
        Else
            rtbDebug.Text &= (vbCrLf & addThisText)
            rtbDebug.Select(rtbDebug.TextLength, 0)
            rtbDebug.SelectionColor = Color.Red
            rtbDebug.AppendText(" [WARNING!]")
            rtbDebug.SelectionColor = rtbDebug.ForeColor
            Dim timeFormatted As String = DateTime.Now.ToString("G")
            File.AppendAllText((_startPath & "\logfile.txt"), timeFormatted & " " & (addThisText & " [WARNING!]") & vbCrLf)
        End If
    End Sub

    Private Sub WriteDebugInfoAndFailed(ByVal addThisText As String)
        If rtbDebug.InvokeRequired Then
            rtbDebug.Invoke(New Action(Of String)(AddressOf WriteDebugInfoAndFailed), Text)
        Else
            If addThisText = "ERROR - Index was outside the bounds of the array." Then Exit Sub
            If addThisText = "ERROR - Object reference not set to an instance of an object." Then Exit Sub
            rtbDebug.Text &= (vbCrLf & addThisText)
            rtbDebug.Select(rtbDebug.TextLength, 0)
            rtbDebug.SelectionColor = Color.Red
            rtbDebug.AppendText(My.Resources.strFAILED)
            rtbDebug.SelectionColor = rtbDebug.ForeColor
            Dim timeFormatted As String = DateTime.Now.ToString("G")
            File.AppendAllText((_startPath & "\logfile.txt"), timeFormatted & " " & (addThisText & " [FAILED!]") & vbCrLf)
            If Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.Pastebin)) Then
                Dim upload As MsgBoxResult = MsgBox(My.Resources.strSomethingWentWrongUpload, vbYesNo)
                If upload = MsgBoxResult.Yes Then
                    PasteBinUpload()
                End If
                If upload = MsgBoxResult.No Then UnlockGui()
            End If
            UnlockGui()
        End If
    End Sub

    Private Sub rtbDebug_LinkClicked(sender As Object, e As LinkClickedEventArgs) Handles rtbDebug.LinkClicked
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
        PBMainBar.Text = (My.Resources.strDownloaded & Helper.SizeSuffix(downloadedBytes) & " / " & Helper.SizeSuffix(totalSize) & " (" & percentage & "%) - " & My.Resources.strRightClickforOptions)
        'Put your progress UI here, you can cancel download by uncommenting the line below
    End Sub

    Private Sub OnFileDownloadCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs) Handles DLS.DownloadFileCompleted
        PBMainBar.Value = 0
        PBMainBar.Text = ""
    End Sub

    Public Sub Dlwua(ByVal address As String, ByVal filename As String)
        ' Fuck this function, there's just to much wrong with it -Matthew

        DLS.Headers("user-agent") = "AQUA_HTTP"
        DLS.Timeout = 10000

        For i As Integer = 1 To 5
            Try
                Application.DoEvents()
                DLS.DownloadFileAsync((New Uri(address)), filename)
                Exit For
            Catch ex As Exception
                If i = 4 Then Thread.Sleep(5000)
                If i = 5 Then Exit Sub
            End Try
        Next

        While DLS.IsBusy = True
            Application.DoEvents()
            If _restartplz Then
                DLS.CancelAsync()
                _restartplz = False
                While DLS.IsBusy = True

                End While
                DLS.DownloadFileAsync((New Uri(address)), filename)
            End If
            If Not Visible Then
                DLS.CancelAsync()
                _cancelled = True
                _cancelledFull = True
                Application.Exit()
            End If
        End While
    End Sub

    Private Sub LockGui()
        Enabled = False
    End Sub

    Private Sub UnlockGui()
        Enabled = True
    End Sub

    Public Sub Log(output As String)
        File.AppendAllText((_startPath & "\logfile.txt"), DateTime.Now.ToString("G") & ": DEBUG - " & output & vbCrLf)
    End Sub

    Public Sub PasteBinUpload()
        Helper.PasteBinUploadFile(_startPath & "\logfile.txt")
    End Sub

    Public Sub MergePatches()
        Dim patchlist As String() = File.ReadAllLines("patchlist.txt")
        Dim patchlistOld As String() = File.ReadAllLines("patchlist_old.txt")

        Dim files = New Dictionary(Of String, String)

        For index As Integer = 0 To (patchlist.Length - 1)
            Dim currentLine = patchlist(index)
            If String.IsNullOrEmpty(currentLine) Then Continue For

            Dim filename = Regex.Split(currentLine, ".pat")
            If String.IsNullOrEmpty(filename(0)) Then Continue For

            Dim key = filename(0).Replace("data/win32/", "")

            If Not files.ContainsKey(key) Then
                files.Add(key, currentLine)
            End If
        Next

        For index As Integer = 0 To (patchlistOld.Length - 1)
            Dim currentLine = patchlistOld(index)
            If String.IsNullOrEmpty(currentLine) Then Continue For

            Dim filename = Regex.Split(currentLine, ".pat")
            If String.IsNullOrEmpty(filename(0)) Then Continue For

            Dim key = filename(0).Replace("data/win32/", "")

            If Not files.ContainsKey(key) Then
                files.Add(key, currentLine)
            End If
        Next

        _someofthethings = files
    End Sub

    Private Sub MergePrePatches()
        Dim patches As New Dictionary(Of String, String)

        For i As Integer = 0 To 5
            Dim patchlist = File.ReadAllLines("patchlist" & i & ".txt")

            For index As Integer = 0 To (patchlist.Length - 1)
                If String.IsNullOrEmpty(patchlist(index)) Then Continue For

                Dim splitLine = patchlist(index).Split(ControlChars.Tab)
                Dim fileName = Path.GetFileNameWithoutExtension(splitLine(0).Replace("data/win32/", ""))
                Dim hash = splitLine(2)

                If (Not String.IsNullOrEmpty(fileName)) AndAlso (Not patches.ContainsKey(hash)) Then
                    patches.Add(hash, fileName)
                End If
            Next
        Next

        _someoftheprepatches = patches
    End Sub

    Private Function CheckIfRunning(processName As String) As Boolean
        _processes = Process.GetProcessesByName(processName)
        Dim currentProcess As Process = Process.GetCurrentProcess()

        Dim x As Integer = If(processName = "PSO2 Tweaker", 1, 0)

        If _processes.Length > x Then
            Dim closeItYesNo As MsgBoxResult = MsgBox("It seems that " & processName.Replace(".exe", "") & " is already running. Would you like to close it?", vbYesNo)

            If closeItYesNo = vbYes Then
                Dim procs As Process() = Process.GetProcessesByName(processName)

                For Each proc As Process In procs
                    If proc.Id <> currentProcess.Id Then proc.Kill()
                Next
            End If

            Return True
        Else
            Return False
        End If
    End Function

    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        DLS.CancelAsync()
        _cancelled = True
        PBMainBar.Value = 0
        PBMainBar.Text = ""
        DeleteFile("launcherlist.txt")
        DeleteFile("patchlist.txt")
        DeleteFile("patchlist_old.txt")
        DeleteFile("version.ver")
        DeleteFile("ALLOFTHETHINGS.txt")
        DeleteFile("SOMEOFTHETHINGS.txt")
        Application.ExitThread()
    End Sub

    Private Sub frmMain_Leave(sender As Object, e As EventArgs) Handles Me.Leave
        Application.Exit()
    End Sub

    Private Sub CheckForStoryUpdates()
        Try
            If RegKey.GetValue(Of String)(RegKey.StoryPatchVersion) = "Not Installed" Then Exit Sub
            Dlwua(_freedomUrl & "patchfiles/Story%20MD5HashList.txt", "Story MD5HashList.txt")
            Dim sBuffer As String
            Dim filename As String()
            Dim truefilename As String
            Dim missingfiles As New List(Of String)
            Dim numberofChecks As Integer
            Dim trueMd5 As String
            Dim updateNeeded As Boolean = False
            numberofChecks = 0
            missingfiles.Clear()
            Using oReader As StreamReader = File.OpenText("Story MD5HashList.txt")
                sBuffer = oReader.ReadLine()
                RegKey.SetValue(Of String)(RegKey.NewVersionTemp, sBuffer)
                RegKey.SetValue(Of String)(RegKey.NewStoryVersion, sBuffer)
                Dim strNewDate As String = sBuffer
                MsgBox(strNewDate)
                If sBuffer <> RegKey.GetValue(Of String)(RegKey.StoryPatchVersion) Then
                    updateNeeded = True
                    'A new story patch update is available - Would you like to download and install it? PLEASE NOTE: This update assumes you've already downloaded and installed the latest RAR file available from http://arks-layer.com, which seems to be: 
                    Dim net As New WebClient()
                    Dim src As String = net.DownloadString("http://arks-layer.com/story.php")
                    ' Create a match using regular exp<b></b>ressions
                    'http://arks-layer.com/Story%20Patch%208-8-2013.rar.torrent
                    Dim m As Match = Regex.Match(src, "<u>.*?</u>")
                    ' Spit out the value plucked from the code
                    txtHTML.Text = m.Value
                    Dim strDownloadMe As String = txtHTML.Text.Replace("<u>", "").Replace("</u>", "")
                    If strDownloadMe <> RegKey.GetValue(Of String)(RegKey.LatestStoryBase) Then
                        Dim mbVisitLink As MsgBoxResult = MsgBox("A new story patch is available! Would you like to download and install it using the new story patch method?", MsgBoxStyle.YesNo)
                        If mbVisitLink = vbYes Then
                            btnStoryPatchNew.RaiseClick()
                            Exit Sub
                        End If
                        If mbVisitLink = vbNo Then Exit Sub
                    End If

                    Dim updateStoryYesNo As MsgBoxResult = MsgBox("A new story patch update is available as of " & strNewDate & " - Would you like to download and install it? PLEASE NOTE: This update assumes you've already downloaded and installed the latest story patch available from http://arks-layer.com (" & strDownloadMe & "), or used the new method to install the story patch.", vbYesNo)
                    If updateStoryYesNo = vbNo Then
                        Exit Sub
                    End If
                End If
                If updateNeeded Then
                    WriteDebugInfo(My.Resources.strBeginningStoryModeUpdate)
                    While Not (oReader.EndOfStream)
                        sBuffer = oReader.ReadLine()
                        filename = sBuffer.Split(","c)
                        truefilename = filename(0)
                        trueMd5 = filename(1)

                        If Not File.Exists((_pso2WinDir & "\" & truefilename)) Then
                            missingfiles.Add(truefilename)
                        ElseIf Helper.GetMd5((_pso2WinDir & "\" & truefilename)) <> trueMd5 Then
                            missingfiles.Add(truefilename)
                        End If

                        numberofChecks += 1
                        lblStatus.Text = (My.Resources.strCurrentlyCheckingFile & numberofChecks & "")
                        Application.DoEvents()
                    End While
                End If
            End Using
            If updateNeeded Then
                Dim totaldownload As Long = missingfiles.Count
                Dim downloaded As Long = 0

                WriteDebugInfo("Downloading/Installing updates using Patch Server #4 (New York)")

                For Each downloadstring As String In missingfiles
                    'Download the missing files:
                    downloaded += 1
                    lblStatus.Text = My.Resources.strUpdating & downloaded & "/" & totaldownload
                    Application.DoEvents()
                    _cancelled = False
                    Dlwua((_freedomUrl & "patchfiles/" & downloadstring & ".7z"), downloadstring & ".7z")
                    If _cancelled Then Exit Sub
                    'Delete the existing file FIRST
                    If Not File.Exists(downloadstring & ".7z") Then
                        WriteDebugInfoAndFailed("File " & (downloadstring & ".7z") & " does not exist! Perhaps it wasn't downloaded properly?")
                    End If
                    DeleteFile((_pso2WinDir & "\" & downloadstring))
                    Dim process As Process
                    Dim processStartInfo As ProcessStartInfo = New ProcessStartInfo()
                    Dim unRarLocation As String = (_startPath & "\7za.exe")
                    processStartInfo.FileName = unRarLocation
                    processStartInfo.Verb = "runas"
                    processStartInfo.Arguments = ("e -y " & downloadstring & ".7z")
                    processStartInfo.WindowStyle = ProcessWindowStyle.Hidden
                    processStartInfo.UseShellExecute = True
                    process = process.Start(processStartInfo)
                    Do Until process.WaitForExit(1000)
                    Loop
                    If Not File.Exists(downloadstring) Then
                        WriteDebugInfoAndFailed("File " & (downloadstring) & " does not exist! Perhaps it wasn't extracted properly?")
                    End If
                    File.Move(downloadstring, (_pso2WinDir & "\" & downloadstring))
                    DeleteFile(downloadstring & ".7z")
                    Application.DoEvents()
                Next
                WriteDebugInfoAndOk(My.Resources.strStoryPatchUpdated)
                RegKey.SetValue(Of String)(RegKey.StoryPatchVersion, RegKey.GetValue(Of String)(RegKey.NewVersionTemp))
                RegKey.SetValue(Of String)(RegKey.NewVersionTemp, "")
                Exit Sub
            End If
            If Not updateNeeded Then
                WriteDebugInfoAndOk("You have the latest story patch updates!")
                Exit Sub
            End If
        Catch ex As Exception
            Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub

    Private Sub CheckForEnPatchUpdates()
        Dim updateNeeded As Boolean
        Try
            If RegKey.GetValue(Of String)(RegKey.EnPatchVersion) = "Not Installed" Then Exit Sub

            Application.DoEvents()
            Dim net As MyWebClient = New MyWebClient() With {.Timeout = 10000}
            Dim strDownloadMe As String = net.DownloadString(_freedomUrl & "patches/enpatch.txt")
            Dim lastfilename As String() = strDownloadMe.Split("/"c)
            Dim strVersion As String = lastfilename(lastfilename.Length - 1).Replace(".rar", "")

            RegKey.SetValue(Of String)(RegKey.NewEnVersion, strVersion)
            If strVersion <> RegKey.GetValue(Of String)(RegKey.EnPatchVersion) Then
                updateNeeded = True
                Dim updateStoryYesNo As MsgBoxResult = MsgBox(My.Resources.strNewENPatch, vbYesNo)
                If updateStoryYesNo = vbNo Then updateNeeded = False
                If updateNeeded Then
                    btnENPatch.RaiseClick()
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub

    Private Sub CheckForLargeFilesUpdates()
        Dim updateNeeded As Boolean
        Try
            If RegKey.GetValue(Of String)(RegKey.LargeFilesVersion) = "Not Installed" Then
                Exit Sub
            End If
            Application.DoEvents()
            Dim net As MyWebClient = New MyWebClient() With {.Timeout = 10000}
            Dim src As String = net.DownloadString(_freedomUrl & "patches/largefiles.txt")
            Dim strDownloadMe As String = src
            Dim lastfilename As String() = strDownloadMe.Split("/"c)
            Dim strVersion As String = lastfilename(lastfilename.Length - 1).Replace(".rar", "")

            RegKey.SetValue(Of String)(RegKey.NewLargeFilesVersion, strVersion)
            If strVersion <> RegKey.GetValue(Of String)(RegKey.LargeFilesVersion) Then
                updateNeeded = True
                Dim updateStoryYesNo As MsgBoxResult = MsgBox(My.Resources.strNewLargeFiles, vbYesNo)
                If updateStoryYesNo = vbNo Then updateNeeded = False
                If updateNeeded Then
                    btnLargeFiles.RaiseClick()
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub

    ' TODO: this function
    Private Sub CheckForPso2Updates(comingFromPrePatch As Boolean)
        Try
            Dim updateNeeded As Boolean
            'Precede file, syntax is Yes/No:<Dateoflastprepatch>
            Dlwua(_freedomUrl & "precede.txt", "precede.txt")
            If comingFromPrePatch Then GoTo StartPrePatch

            Dim firstTimechecking As Boolean = False
            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.Pso2PrecedeVersion)) Then
                Dim precedefile2 As String() = File.ReadAllLines("precede.txt")
                Dim precedeVersion2 As String() = precedefile2(0).Split(":"c)
                RegKey.SetValue(Of String)(RegKey.Pso2PrecedeVersion, precedeVersion2(1))
                firstTimechecking = True
            End If

            Dim precedefile = File.ReadAllLines("precede.txt")
            Dim precedeSplit As String() = precedefile(0).Split(":"c)
            Dim precedeYesNo As String = precedeSplit(0)
            Dim precedeversionstring As String = precedeSplit(1)

            If precedeYesNo = "Yes" Then
                If RegKey.GetValue(Of String)(RegKey.Pso2PrecedeVersion) <> precedeversionstring OrElse firstTimechecking Then
                    Dim downloadPrepatch As MsgBoxResult = MsgBox("New pre-patch data is available to download - Would you like to download it? This is optional, and will let you download some of a large patch now, as opposed to having a larger download all at once when it is released.", MsgBoxStyle.YesNo)
                    If downloadPrepatch = vbYes Then
StartPrePatch:
                        'Download prepatch
                        _cancelledFull = False
                        Dim truefilename As String
                        Dim missingfiles As New List(Of String)
                        Dim numberofChecks As Integer
                        lblStatus.Text = ""
                        WriteDebugInfo("Downloading pre-patch filelist...")
                        Dlwua("http://download.pso2.jp/patch_prod/patches_precede/patchlist0.txt", "patchlist0.txt")
                        Dlwua("http://download.pso2.jp/patch_prod/patches_precede/patchlist1.txt", "patchlist1.txt")
                        Dlwua("http://download.pso2.jp/patch_prod/patches_precede/patchlist2.txt", "patchlist2.txt")
                        Dlwua("http://download.pso2.jp/patch_prod/patches_precede/patchlist3.txt", "patchlist3.txt")
                        Dlwua("http://download.pso2.jp/patch_prod/patches_precede/patchlist4.txt", "patchlist4.txt")
                        Dlwua("http://download.pso2.jp/patch_prod/patches_precede/patchlist5.txt", "patchlist5.txt")
                        WriteDebugInfoSameLine(My.Resources.strDone)
                        MergePrePatches()
                        If Not Directory.Exists(_pso2RootDir & "\_precede\data\win32") Then Directory.CreateDirectory(_pso2RootDir & "\_precede\data\win32") 'create directory

                        WriteDebugInfo("Checking for already existing precede files...")
                        numberofChecks = 0
                        missingfiles.Clear()

                        _someoftheprepatches.Remove("GameGuard.des")
                        _someoftheprepatches.Remove("PSO2JP.ini")
                        _someoftheprepatches.Remove("script/user_default.pso2")
                        _someoftheprepatches.Remove("script/user_intel.pso2")
                        _someoftheprepatches.Remove("")

                        For Each sBuffer In _someoftheprepatches
                            If _cancelledFull Then
                                _someoftheprepatches = Nothing
                                Exit Sub
                            End If

                            truefilename = sBuffer.Value

                            If Not File.Exists(((_pso2RootDir & "\_precede\data\win32") & "\" & truefilename)) Then
                                If _vedaUnlocked Then WriteDebugInfo("DEBUG: The file " & truefilename & " is missing.")
                                missingfiles.Add(truefilename)
                            ElseIf Helper.GetMd5(((_pso2RootDir & "\_precede\data\win32") & "\" & truefilename)) <> sBuffer.Key Then
                                If _vedaUnlocked Then WriteDebugInfo("DEBUG: The file " & truefilename & " must be redownloaded.")
                                missingfiles.Add(truefilename)
                            End If

                            numberofChecks += 1
                            lblStatus.Text = (My.Resources.strCurrentlyCheckingFile & numberofChecks & "")
                            Application.DoEvents()
                        Next

                        _someoftheprepatches = Nothing

                        Dim totaldownload As Long = missingfiles.Count
                        Dim downloaded As Long = 0
                        Dim totaldownloaded As Long = 0

                        For Each downloadstring As String In missingfiles
                            If _cancelledFull Then Exit Sub
                            'Download the missing files:
                            'WHAT THE FUCK IS GOING ON HERE?
                            downloaded += 1
                            totaldownloaded += _totalsize2

                            lblStatus.Text = My.Resources.strDownloading & "" & downloaded & "/" & totaldownload & " (" & Helper.SizeSuffix(totaldownloaded) & ")"

                            Application.DoEvents()
                            _cancelled = False
                            Dlwua(("http://download.pso2.jp/patch_prod/patches_precede/data/win32/" & downloadstring & ".pat"), downloadstring)
                            Dim info7 As New FileInfo(downloadstring)
                            'Dim length2 As Long
                            'If File.Exists(downloadstring) Then length2 = info7.Length
                            If info7.Length = 0 Then
                                Log("File appears to be empty, trying to download from secondary SEGA server")
                                Dlwua(("http://download.pso2.jp/patch_prod/patches_precede/data/win32/" & downloadstring & ".pat"), downloadstring)
                            End If

                            If _cancelled Then Exit Sub

                            DeleteFile(((_pso2RootDir & "\_precede\data\win32") & "\" & downloadstring))
                            File.Move(downloadstring, ((_pso2RootDir & "\_precede\data\win32") & "\" & downloadstring))
                            If _vedaUnlocked Then WriteDebugInfo("DEBUG: Downloaded and installed " & downloadstring & ".")

                            Application.DoEvents()
                        Next

                        If missingfiles.Count = 0 Then WriteDebugInfo("Your precede data is up to date!")
                        If missingfiles.Count <> 0 Then WriteDebugInfo("Precede data downloaded/updated!")
                        Dim precedefile2 As String() = File.ReadAllLines("precede.txt")
                        Dim precedeVersion2 As String() = precedefile2(0).Split(":"c)
                        RegKey.SetValue(Of String)(RegKey.Pso2PrecedeVersion, precedeVersion2(1))
                    End If
                End If
            End If

            'Check whether or not to apply pre-patch shit. Ugh.
            If Directory.Exists(_pso2RootDir & "\_precede\") Then
                Dlwua(_freedomUrl & "precede_apply.txt", "precede_apply.txt")
                Dim prepatchapply = File.ReadAllLines("precede_apply.txt")
                Dim applyPrePatch As String = prepatchapply(0)

                If Directory.Exists(_pso2RootDir & "\_precede\data\win32\") Then
                    If applyPrePatch = "Yes" Then
                        Dim applyPrePatchYesNo As MsgBoxResult = MsgBox("It appears that it's time to install the pre-patch download - Is this okay? If you select no, the pre-patch will not be installed.", vbYesNo)
                        If applyPrePatchYesNo = vbYes Then
                            WriteDebugInfo("Installing prepatch, please wait...")
                            Application.DoEvents()
                            Dim di As New DirectoryInfo(_pso2RootDir & "\_precede\data\win32\")
                            Dim diar1 As FileInfo() = di.GetFiles()

                            'list the names of all files in the specified directory
                            Dim downloadstring As String
                            Dim count As Integer = 0
                            Dim counter = My.Computer.FileSystem.GetFiles(_pso2RootDir & "\_precede\data\win32\")
                            For Each dra As FileInfo In diar1
                                If counter.Count = 0 Then Exit For
                                downloadstring = dra.Name
                                DeleteFile((_pso2WinDir & "\" & downloadstring))
                                File.Move(_pso2RootDir & "\_precede\data\win32\" & downloadstring, (_pso2WinDir & "\" & downloadstring))
                                count += 1
                                lblStatus.Text = "Moved " & count & " files out of " & counter.Count
                                Application.DoEvents()
                            Next
                            WriteDebugInfoSameLine("Done!")
                            Helper.DeleteDirectory(_pso2RootDir & "\_precede")
                        End If
                    End If
                End If
            End If

            If comingFromPrePatch Then Exit Sub
            Dlwua("http://arks-layer.com/vanila/version.txt", "version.ver")
            If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.Pso2RemoteVersion)) Then
                Dim lines2 = File.ReadAllLines("version.ver")
                Dim remoteVersion2 As String = lines2(0)
                RegKey.SetValue(Of String)(RegKey.Pso2RemoteVersion, remoteVersion2)
            End If

            Dim lines = File.ReadAllLines("version.ver")
            Dim remoteVersion As String = lines(0)
            If RegKey.GetValue(Of String)(RegKey.Pso2RemoteVersion) <> remoteVersion Then
                updateNeeded = True

                Dim updateStoryYesNo As MsgBoxResult = MsgBox(My.Resources.strNewPSO2Update, vbYesNo)
                If updateStoryYesNo = vbNo Then updateNeeded = False
            End If

            If updateNeeded Then
                ButtonItem5.RaiseClick()
            End If
        Catch ex As Exception
            Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub

    Private Sub btnApplyChanges_Click(sender As Object, e As EventArgs) Handles btnApplyChanges.Click
        Try
            If Not Directory.Exists(_pso2WinDir) OrElse _pso2RootDir = "lblDirectory" Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                SelectPso2Directory()
                Exit Sub
            End If
            Log("Restoring/Removing files...")
            If chkRemoveCensor.Checked AndAlso chkRestoreCensor.Checked Then
                MsgBox(My.Resources.strYouCannotRemoveRestore)
                Exit Sub
            End If
            If chkRemoveNVidia.Checked AndAlso chkRestoreNVidia.Checked Then
                MsgBox(My.Resources.strYouCannotRemoveRestore)
                Exit Sub
            End If
            If chkRemovePC.Checked AndAlso chkRestorePC.Checked Then
                MsgBox(My.Resources.strYouCannotRemoveRestore)
                Exit Sub
            End If
            If chkRemoveVita.Checked AndAlso chkRestoreVita.Checked Then
                MsgBox(My.Resources.strYouCannotRemoveRestore)
                Exit Sub
            End If
            If chkRemoveSEGA.Checked AndAlso chkRestoreSEGA.Checked Then
                MsgBox(My.Resources.strYouCannotRemoveRestore)
                Exit Sub
            End If
            'Remove censor
            '[AIDA] Resume here
            If chkRemoveCensor.Checked AndAlso File.Exists((_pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c")) Then
                If File.Exists((_pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup")) Then My.Computer.FileSystem.DeleteFile((_pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile((_pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c"), "ffbff2ac5b7a7948961212cefd4d402c.backup")
                WriteDebugInfoAndOk(My.Resources.strRemoving & "Censor...")
            ElseIf chkRemoveCensor.Checked AndAlso (Not File.Exists((_pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c"))) Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRemoveCensor)
            End If
            'Restore Censor
            If chkRestoreCensor.Checked AndAlso File.Exists((_pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup")) Then
                If File.Exists((_pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c")) Then My.Computer.FileSystem.DeleteFile((_pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile((_pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"), "ffbff2ac5b7a7948961212cefd4d402c")
                WriteDebugInfoAndOk(My.Resources.strRestoring & "Censor...")
            ElseIf chkRestoreCensor.Checked AndAlso (Not File.Exists((_pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"))) Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRestoreCensor)
            End If
            'Remove PC Opening Video [Done]
            If chkRemovePC.Checked AndAlso File.Exists((_pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) Then
                If File.Exists((_pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup")) Then My.Computer.FileSystem.DeleteFile((_pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile((_pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927"), "a44fbb2aeb8084c5a5fbe80e219a9927.backup")
                WriteDebugInfoAndOk(My.Resources.strRemoving & "PC Opening Video...")
            ElseIf chkRemovePC.Checked AndAlso (Not File.Exists((_pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927"))) Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRemovePC)
            End If
            'Restore PC Opening Video [Done]
            If chkRestorePC.Checked AndAlso File.Exists((_pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup")) Then
                If File.Exists((_pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) Then My.Computer.FileSystem.DeleteFile((_pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile((_pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup"), "a44fbb2aeb8084c5a5fbe80e219a9927")
                WriteDebugInfoAndOk(My.Resources.strRestoring & "PC Opening Video...")
            ElseIf chkRestorePC.Checked AndAlso (Not File.Exists((_pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup"))) Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRestorePC)
            End If
            'Remove Vita Opening Video [Done]
            If chkRemoveVita.Checked AndAlso File.Exists((_pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585")) Then
                If File.Exists((_pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585.backup")) Then My.Computer.FileSystem.DeleteFile((_pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585.backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile((_pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585"), "a93adc766eb3510f7b5c279551a45585.backup")
                WriteDebugInfoAndOk(My.Resources.strRemoving & "Vita Opening Video...")
            ElseIf chkRemoveVita.Checked AndAlso (Not File.Exists((_pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585"))) Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRemoveVita)
            End If
            'Restore Vita Opening Video [Done]
            If chkRestoreVita.Checked AndAlso File.Exists((_pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585.backup")) Then
                If File.Exists((_pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585")) Then My.Computer.FileSystem.DeleteFile((_pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile((_pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585.backup"), "a93adc766eb3510f7b5c279551a45585")
                WriteDebugInfoAndOk(My.Resources.strRestoring & "Vita Opening Video...")
            ElseIf chkRestoreVita.Checked AndAlso (Not File.Exists((_pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585.backup"))) Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRestoreVita)
            End If
            'Remove NVidia Opening Video [Done]
            If chkRemoveNVidia.Checked AndAlso File.Exists((_pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75")) Then
                If File.Exists((_pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75.backup")) Then My.Computer.FileSystem.DeleteFile((_pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75.backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile((_pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75"), "7f2368d207e104e8ed6086959b742c75.backup")
                WriteDebugInfoAndOk(My.Resources.strRemoving & "NVidia Opening Video...")
            ElseIf chkRemoveNVidia.Checked AndAlso (Not File.Exists((_pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75"))) Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRemoveNVidia)
            End If
            'Restore NVidia Opening Video [Done]
            If chkRestoreNVidia.Checked AndAlso File.Exists((_pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75.backup")) Then
                If File.Exists((_pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75")) Then My.Computer.FileSystem.DeleteFile((_pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile((_pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75.backup"), "7f2368d207e104e8ed6086959b742c75")
                WriteDebugInfoAndOk(My.Resources.strRestoring & "NVidia Opening Video...")
            ElseIf chkRestoreNVidia.Checked AndAlso (Not File.Exists((_pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75.backup"))) Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRestoreNVidia)
            End If
            'Remove SEGA Opening Video [Done]
            If chkRemoveSEGA.Checked AndAlso File.Exists((_pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771")) Then
                If File.Exists((_pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771.backup")) Then My.Computer.FileSystem.DeleteFile((_pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771.backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile((_pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771"), "009bfec69b04a34576012d50e3417771.backup")
                WriteDebugInfoAndOk(My.Resources.strRemoving & "SEGA Opening Video...")
            ElseIf chkRemoveSEGA.Checked AndAlso (Not File.Exists((_pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771"))) Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRemoveSEGA)
            End If
            'Restore SEGA Opening Video [Done]
            If chkRestoreSEGA.Checked AndAlso File.Exists((_pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771.backup")) Then
                If File.Exists((_pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771")) Then My.Computer.FileSystem.DeleteFile((_pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile((_pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771.backup"), "009bfec69b04a34576012d50e3417771")
                WriteDebugInfoAndOk(My.Resources.strRestoring & "SEGA Opening Video...")
            ElseIf chkRestoreSEGA.Checked AndAlso (Not File.Exists((_pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771.backup"))) Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRestoreSEGA)
            End If
            UnlockGui()
            'Swap PC and Vita Openings
            'Restore PC Opening Video [Done]
            If chkSwapOP.Checked Then
                WriteDebugInfo(My.Resources.strSwappingOpenings)
                If File.Exists((_pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup")) Then
                    If File.Exists((_pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) Then My.Computer.FileSystem.DeleteFile((_pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                    My.Computer.FileSystem.RenameFile((_pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup"), "a44fbb2aeb8084c5a5fbe80e219a9927")
                    WriteDebugInfoAndOk(My.Resources.strRestoring & "PC Opening Video...")
                End If
                'Restore Vita Opening Video [Done]
                If File.Exists((_pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585.backup")) Then
                    If File.Exists((_pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585")) Then My.Computer.FileSystem.DeleteFile((_pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                    My.Computer.FileSystem.RenameFile((_pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585.backup"), "a93adc766eb3510f7b5c279551a45585")
                    WriteDebugInfoAndOk(My.Resources.strRestoring & "Vita Opening Video...")
                End If
                'Rename the original files
                My.Computer.FileSystem.RenameFile((_pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927"), "PCOpening")
                My.Computer.FileSystem.RenameFile((_pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585"), "VitaOpening")
                'Rename them back, swapping them~
                My.Computer.FileSystem.RenameFile((_pso2WinDir & "\" & "PCOpening"), "a93adc766eb3510f7b5c279551a45585")
                My.Computer.FileSystem.RenameFile((_pso2WinDir & "\" & "VitaOpening"), "a44fbb2aeb8084c5a5fbe80e219a9927")
            End If
            If File.Exists((_pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) AndAlso File.Exists((_pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585")) Then
                If Helper.GetFileSize((_pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) = 167479840 AndAlso Helper.GetFileSize((_pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585")) = 151540352 Then
                    chkSwapOP.Text = My.Resources.strSwapPCVitaOpenings & "(" & My.Resources.strNotSwapped & ")"
                    WriteDebugInfo(My.Resources.strallDone)
                    UnlockGui()
                    Exit Sub
                End If
                If Helper.GetFileSize((_pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) = 151540352 AndAlso Helper.GetFileSize((_pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585")) = 167479840 Then
                    chkSwapOP.Text = My.Resources.strSwapPCVitaOpenings & "(" & My.Resources.strSwapped & ")"
                    WriteDebugInfo(My.Resources.strallDone)
                    UnlockGui()
                    Exit Sub
                End If
                chkSwapOP.Text = "Swap PC/Vita Openings (UNKNOWN)"
            End If
        Catch ex As Exception
            Log(ex.Message.ToString)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub

    Private Sub btnLaunchPSO2_Click(sender As Object, e As EventArgs) Handles btnLaunchPSO2.Click
        'Fuck SEGA. Stupid jerks.
        Log("Check if PSO2 is running")
        If CheckIfRunning("pso2") Then Exit Sub
        Try
            If Not Directory.Exists(_pso2WinDir) OrElse _pso2RootDir = "lblDirectory" Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                SelectPso2Directory()
                Exit Sub
            End If

            Dim pso2Launchpath As String = _pso2RootDir

            If Not File.Exists(pso2Launchpath & "\pso2.exe") Then
                WriteDebugInfoAndFailed(My.Resources.strCouldNotFindPSO2EXE)
                Exit Sub
            End If

            DLS.CancelAsync()
            _cancelled = True
            PBMainBar.Value = 0
            PBMainBar.Text = ""
            WriteDebugInfo(My.Resources.strLaunchingPSO2)

            'Why did we ever delete this? [AIDA]
            'Dim dir As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            'DeleteFile(dir & "\SEGA\PHANTASYSTARONLINE2\item_name_cache.dat")

            If chkItemTranslation.Checked Then
                If Helper.GetMd5(pso2Launchpath & "\translator.dll") <> RegKey.GetValue(Of String)(RegKey.Dllmd5) Then
                    MsgBox(My.Resources.strTranslationFilesDontMatch)
                    Exit Sub
                End If
            End If

            'End Item Translation stuff
            DeleteFile(pso2Launchpath & "\ddraw.dll")
            File.WriteAllBytes(pso2Launchpath & "\ddraw.dll", My.Resources.ddraw)
            Dim startInfo As ProcessStartInfo = New ProcessStartInfo With {.FileName = (pso2Launchpath & "\pso2.exe"), .Arguments = "+0x33aca2b9", .UseShellExecute = False}
            startInfo.EnvironmentVariables("-pso2") = "+0x01e3f1e9"
            Dim shell As Process = New Process With {.StartInfo = startInfo}

            Try
                shell.Start()
            Catch ex As Exception
                WriteDebugInfo(My.Resources.strItSeemsThereWasAnError)
                Dlwua("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", "pso2.exe")
                Dim directoryString2 As String = _pso2RootDir
                If File.Exists((directoryString2 & "\pso2.exe")) AndAlso _startPath <> directoryString2 Then DeleteFile((directoryString2 & "\pso2.exe"))
                File.Move("pso2.exe", (directoryString2 & "\pso2.exe"))
                WriteDebugInfoSameLine(My.Resources.strDone)
                shell.Start()
            End Try

            Hide()
            Dim hWnd As IntPtr = External.FindWindow("Phantasy Star Online 2", Nothing)

            Do Until hWnd <> IntPtr.Zero
                hWnd = External.FindWindow("Phantasy Star Online 2", Nothing)
                Thread.Sleep(10)
            Loop

            DeleteFile(pso2Launchpath & "\ddraw.dll")
            If RegKey.GetValue(Of String)(RegKey.SteamMode) = "True" Then
                'NotifyIcon1.Visible = True
                'MsgBox("Minimizing to tray!")
                'NotifyIcon1.ShowBalloonTip(3000, "", "PSO2 Tweaker minimized to tray - Close this when you close PSO2.", ToolTipIcon.Info)
                'Exit Sub
                File.Copy(_pso2RootDir & "\pso2.exe", _pso2RootDir & "\pso2.exe_backup", True)
                Do Until FileInUse(_pso2RootDir & "\pso2.exe") = False
                    Thread.Sleep(1000)
                Loop
                File.Copy(_pso2RootDir & "\pso2.exe_backup", _pso2RootDir & "\pso2.exe", True)
                File.Delete(_pso2RootDir & "\pso2.exe_backup")
            End If
            Close()

        Catch ex As Exception
            Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub

    Private Function FileInUse(ByVal sFile As String) As Boolean
        Dim thisFileInUse As Boolean = False
        If File.Exists(sFile) Then
            Try
                File.Delete(sFile)
                ' thisFileInUse = False
            Catch
                thisFileInUse = True
            End Try
        End If
        Return thisFileInUse
    End Function

    Private Sub PB1_Click(sender As Object, e As EventArgs) Handles PBMainBar.Click
        Dim mouseArgs = DirectCast(e, MouseEventArgs)

        If mouseArgs.Button = MouseButtons.Right Then
            If DLS.IsBusy Then
                CancelDownloadToolStripMenuItem.Visible = True
                cmsProgressBar.Show(CType(sender, Control), mouseArgs.Location)
            End If
            If Not DLS.IsBusy Then
                CancelDownloadToolStripMenuItem.Visible = False
                cmsProgressBar.Show(CType(sender, Control), mouseArgs.Location)
            End If
        End If
    End Sub

    Private Sub CancelDownloadToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CancelDownloadToolStripMenuItem.Click
        DLS.CancelAsync()
        WriteDebugInfo(My.Resources.strDownloadwasCancelled)
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
        WriteDebugInfo(My.Resources.strProcessWasCancelled)
        UnlockGui()
    End Sub

    Private Sub SelectPso2Directory()
        Try
            Log("Selecting PSO2 Directory...")
            Dim myFolderBrowser As New FolderBrowserDialog
            ' Description that displays above the dialog box control. 
            If Not String.IsNullOrEmpty(_pso2RootDir) Then myFolderBrowser.SelectedPath = _pso2RootDir
            myFolderBrowser.Description = My.Resources.strSelectPSO2win32folder2
            ' Sets the root folder where the browsing starts from 
            myFolderBrowser.RootFolder = Environment.SpecialFolder.MyComputer
            Dim dlgResult As DialogResult = myFolderBrowser.ShowDialog()
            If dlgResult = DialogResult.Cancel Then
                WriteDebugInfo("pso2_bin folder selection cancelled!")
                Exit Sub
            End If

            If myFolderBrowser.SelectedPath.Contains("\pso2_bin") Then
                If File.Exists(myFolderBrowser.SelectedPath.Replace("\data\win32", "") & "\pso2.exe") Then
                    WriteDebugInfo("win32 folder selected instead of pso2_bin folder - Fixing!")
                    _pso2RootDir = myFolderBrowser.SelectedPath.Replace("\data\win32", "")
                    lblDirectory.Text = _pso2RootDir
                    RegKey.SetValue(Of String)(RegKey.Pso2Dir, _pso2RootDir)
                    WriteDebugInfoAndOk(_pso2RootDir & " " & My.Resources.strSetAsYourPSO2)
                    Exit Sub
                End If
            End If

            RegKey.SetValue(Of String)(RegKey.Pso2Dir, myFolderBrowser.SelectedPath)
            _pso2RootDir = myFolderBrowser.SelectedPath
            lblDirectory.Text = _pso2RootDir
            WriteDebugInfoAndOk(_pso2RootDir & " " & My.Resources.strSetAsYourPSO2)

        Catch ex As Exception
            Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnSelectPSODir.Click
        SelectPso2Directory()
    End Sub

    Private Sub btnLargeFiles_Click(sender As Object, e As EventArgs) Handles btnLargeFiles.Click
        ' The Using statement will dispose "net" as soon as we're done with it.
        Using net As New WebClient()
            ' This parses the sidebar page for compatibility
            ' First it downloads the page and splits it by line
            Dim compat As String() = Regex.Split(net.DownloadString(_freedomUrl & "tweaker.html"), "\r\n|\r|\n")
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
                Dim url As String = net.DownloadString(_freedomUrl & "patches/largefiles.txt")
                DownloadPatch(url, "Large Files", "LargeFiles.rar", RegKey.LargeFilesVersion, My.Resources.strWouldYouLikeToBackupLargeFiles, My.Resources.strWouldYouLikeToUse, "backup\Large Files\")
            Else
                WriteDebugInfo("Download was cancelled due to incompatibility.")
            End If
        End Using
    End Sub

    Private Sub btnStory_Click(sender As Object, e As EventArgs) Handles btnStory.Click
        _cancelledFull = False
        Dim storyLocation As String
        Dim backupyesno As MsgBoxResult
        If Not Directory.Exists(_pso2WinDir) OrElse _pso2RootDir = "lblDirectory" Then
            MsgBox(My.Resources.strPleaseSelectwin32Dir)
            SelectPso2Directory()
            Exit Sub
        End If
        Log("Selecting story patch...")
        Dim downloaded As MsgBoxResult = MsgBox(My.Resources.strHaveyouDownloadedStoryYet, MsgBoxStyle.YesNo)
        If downloaded = MsgBoxResult.Yes Then
            OpenFileDialog1.Title = My.Resources.strPleaseSelectStoryRAR
            OpenFileDialog1.FileName = "PSO2 Story Patch RAR file"
            OpenFileDialog1.Filter = "RAR Archives|*.rar"
            Dim result = OpenFileDialog1.ShowDialog()
            If result = DialogResult.Cancel Then
                Exit Sub
            End If
            storyLocation = OpenFileDialog1.FileName
            If storyLocation = "PSO2 Story Patch RAR file" Then
                Exit Sub
            End If

            Log("Story mode RAR selected as: " & storyLocation)

            Select Case RegKey.GetValue(Of String)(RegKey.Backup)
                Case "Ask"
                    backupyesno = MsgBox(My.Resources.strWouldYouLikeBackupStory, vbYesNo)
                Case "Always"
                    backupyesno = MsgBoxResult.Yes
                Case "Never"
                    backupyesno = MsgBoxResult.No
            End Select

            Log("Extracting story patch...")
            If Directory.Exists("TEMPSTORYAIDAFOOL") Then
                My.Computer.FileSystem.DeleteDirectory("TEMPSTORYAIDAFOOL", FileIO.DeleteDirectoryOption.DeleteAllContents)
                Directory.CreateDirectory("TEMPSTORYAIDAFOOL")
            End If
            If Not Directory.Exists("TEMPSTORYAIDAFOOL") Then
                Directory.CreateDirectory("TEMPSTORYAIDAFOOL")
            End If
            Dim process As Process
            Dim processStartInfo As ProcessStartInfo = New ProcessStartInfo()
            processStartInfo.FileName = (_startPath & "\unrar.exe")
            processStartInfo.Verb = "runas"
            processStartInfo.Arguments = ("e " & """" & storyLocation & """" & " TEMPSTORYAIDAFOOL")
            processStartInfo.WindowStyle = ProcessWindowStyle.Normal
            processStartInfo.UseShellExecute = True
            process = process.Start(processStartInfo)
            WriteDebugInfo(My.Resources.strWaitingforPatch)
            If _cancelledFull Then Exit Sub
            Do Until process.WaitForExit(1000)
            Loop
            If _cancelledFull Then Exit Sub
            If Not Directory.Exists("TEMPSTORYAIDAFOOL") Then
                Directory.CreateDirectory("TEMPSTORYAIDAFOOL")
                WriteDebugInfo("Had to manually make temp update folder - Did the patch not extract right?")
            End If
            Dim di As New DirectoryInfo("TEMPSTORYAIDAFOOL")
            Dim diar1 As FileInfo() = di.GetFiles()
            Dim dra As FileInfo
            WriteDebugInfoAndOk((My.Resources.strExtractingTo & _pso2WinDir))
            Application.DoEvents()
            'list the names of all files in the specified directory
            Dim backupdir As String = (_pso2WinDir & "\" & "backup\Story Patch\")
            If backupyesno = MsgBoxResult.Yes Then
                If Directory.Exists(backupdir) Then
                    My.Computer.FileSystem.DeleteDirectory(backupdir, FileIO.DeleteDirectoryOption.DeleteAllContents)
                    Directory.CreateDirectory(backupdir)
                    WriteDebugInfo(My.Resources.strErasingPreviousBackup)
                End If
                If Not Directory.Exists(backupdir) Then
                    Directory.CreateDirectory(backupdir)
                    WriteDebugInfo(My.Resources.strCreatingBackupDirectory)
                End If
            End If
            Log("Extracted " & diar1.Length & " files from the patch")
            If diar1.Length = 0 Then
                WriteDebugInfo("Patch failed to extract correctly! Installation failed!")
                Exit Sub
            End If
            WriteDebugInfo(My.Resources.strInstallingPatch)
            For Each dra In diar1
                If _cancelledFull Then Exit Sub
                If backupyesno = MsgBoxResult.Yes Then
                    If File.Exists((_pso2WinDir & "\" & dra.ToString())) Then
                        File.Move((_pso2WinDir & "\" & dra.ToString()), (backupdir & "\" & dra.ToString()))
                    End If
                End If
                If backupyesno = MsgBoxResult.No Then
                    If File.Exists((_pso2WinDir & "\" & dra.ToString())) Then
                        DeleteFile((_pso2WinDir & "\" & dra.ToString()))
                    End If
                End If
                File.Move(("TEMPSTORYAIDAFOOL\" & dra.ToString()), (_pso2WinDir & "\" & dra.ToString()))
            Next
            My.Computer.FileSystem.DeleteDirectory("TEMPSTORYAIDAFOOL", FileIO.DeleteDirectoryOption.DeleteAllContents)
            ' TODO: Less redundant pls - SF94
            If backupyesno = MsgBoxResult.No Then
                External.FlashWindow(Handle, True)
                'Story Patch 3-12-2014.rar
                Dim storyPatchFilename As String = OpenFileDialog1.SafeFileName.Replace("Story Patch ", "").Replace(".rar", "").Replace("-", "/")
                RegKey.SetValue(Of String)(RegKey.StoryPatchVersion, storyPatchFilename)
                RegKey.SetValue(Of String)(RegKey.LatestStoryBase, storyPatchFilename)
                WriteDebugInfo(My.Resources.strStoryPatchInstalled)
                CheckForStoryUpdates()
            End If
            If backupyesno = MsgBoxResult.Yes Then
                External.FlashWindow(Handle, True)
                'Story Patch 3-12-2014.rar
                Dim storyPatchFilename As String = OpenFileDialog1.SafeFileName.Replace("Story Patch ", "").Replace(".rar", "").Replace("-", "/")
                RegKey.SetValue(Of String)(RegKey.StoryPatchVersion, storyPatchFilename)
                RegKey.SetValue(Of String)(RegKey.LatestStoryBase, storyPatchFilename)
                WriteDebugInfo((My.Resources.strStoryPatchBackup & backupdir))
                CheckForStoryUpdates()
            End If
            Exit Sub
        End If
        If downloaded = MsgBoxResult.No Then
            WriteDebugInfo(My.Resources.strDownloadStoryPatch)
            Exit Sub
        End If
    End Sub

    Private Sub chkItemTranslation_Click(sender As Object, e As EventArgs) Handles chkItemTranslation.Click
        If Not File.Exists(_pso2RootDir & "\translation.cfg") Then
            File.WriteAllText(_pso2RootDir & "\translation.cfg", "TranslationPath:translation.bin")
        End If
        If chkItemTranslation.Checked Then

            Dim pso2Launchpath As String = _pso2RootDir

            WriteDebugInfoAndOk(My.Resources.strDownloadingLatestItemTranslationFiles)
            'Download translator.dll and translation.bin
            Dim dlLink1 As String = _freedomUrl & "translator.dll"
            Dim dlLink2 As String = _freedomUrl & "translation.bin"
            Dim client As New WebClient

            For index As Integer = 1 To 5
                Try
                    client.DownloadFile(dlLink1, (pso2Launchpath & "\translator.dll"))
                Catch ex As Exception
                    If index = 5 Then
                        WriteDebugInfoAndWarning("Failed to download translation files! (" & ex.Message.ToString & " Stack Trace: " & ex.StackTrace & ")")
                        Exit Try
                    End If
                End Try
            Next

            RegKey.SetValue(Of String)(RegKey.Dllmd5, Helper.GetMd5(pso2Launchpath & "\translator.dll"))

            For index As Integer = 1 To 5
                Try
                    client.DownloadFile(dlLink2, (pso2Launchpath & "\translation.bin"))
                Catch ex As Exception
                    If index = 5 Then
                        WriteDebugInfoAndWarning("Failed to download translation files! (" & ex.Message.ToString & " Stack Trace: " & ex.StackTrace & ")")
                        Exit Try
                    End If
                End Try
            Next

            'Start the shitstorm
            Using reader As New StreamReader(_pso2RootDir & "\translation.cfg")
                Dim builtFile As New List(Of String)
                Dim currentLine As String

                Do
                    currentLine = reader.ReadLine()
                    If (currentLine Is Nothing) Then Exit Do
                    If currentLine.Contains("TranslationPath:") Then currentLine = "TranslationPath:translation.bin"

                    builtFile.Add(currentLine)
                Loop

                reader.Close()
                File.WriteAllLines(_pso2RootDir & "\translation.cfg", builtFile.ToArray())
            End Using

            WriteDebugInfoSameLine(My.Resources.strDone)
        End If

        If Not chkItemTranslation.Checked Then
            WriteDebugInfoAndOk(My.Resources.strDeletingItemCache)
            'Why did we delete this? [AIDA]
            'Dim dir As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            'DeleteFile(dir & "\SEGA\PHANTASYSTARONLINE2\item_name_cache.dat")
            WriteDebugInfoSameLine(My.Resources.strDone)

            Using reader As New StreamReader(_pso2RootDir & "\translation.cfg")
                Dim builtFile As New List(Of String)
                Dim currentLine As String

                Do
                    currentLine = reader.ReadLine()
                    If (currentLine Is Nothing) Then Exit Do

                    If currentLine.Contains("TranslationPath:") Then currentLine = "TranslationPath:"
                    builtFile.Add(currentLine)
                Loop

                reader.Close()
                File.WriteAllLines(_pso2RootDir & "\translation.cfg", builtFile.ToArray())
            End Using
        End If

        _useItemTranslation = chkItemTranslation.Checked
        RegKey.SetValue(Of Boolean)(RegKey.UseItemTranslation, _useItemTranslation)
    End Sub

    Private Sub ButtonItem5_Click(sender As Object, e As EventArgs) Handles ButtonItem5.Click
        _cancelledFull = False

        If Not Directory.Exists(_pso2WinDir) OrElse _pso2RootDir = "lblDirectory" Then
            MsgBox(My.Resources.strPleaseSelectwin32Dir)
            SelectPso2Directory()
            Exit Sub
        End If

        Dim sBuffer As String
        Dim filename As String()
        Dim truefilename As String
        Dim missingfiles As New List(Of String)
        Dim missingfiles2 As New List(Of String)
        Dim numberofChecks As Integer = 0
        Dim md5 As String()
        Dim trueMd5 As String
        Dim totalfilesize As Long = 0
        Dim testfilesize As String()
        lblStatus.Text = ""

        If Directory.Exists((_pso2WinDir & "\backup\English Patch\")) Then
            WriteDebugInfo(My.Resources.strENBackupFound)
            _override = True
            btnRestoreENBackup.RaiseClick()
            _override = False
        End If

        If Directory.Exists((_pso2WinDir & "\backup\Large Files\")) Then
            WriteDebugInfo(My.Resources.strLFBackupFound)
            _override = True
            btnRestoreLargeFilesBackup.RaiseClick()
            _override = False
        End If

        If Directory.Exists((_pso2WinDir & "\backup\Story Patch\")) Then
            WriteDebugInfo(My.Resources.strStoryBackupFound)
            _override = True
            btnRestoreStoryBackup.RaiseClick()
            _override = False
        End If

        ' Why is the UI being disabled here, is there something I'm missing? -Matthew
        LockGui()
        WriteDebugInfo(My.Resources.strDownloadingPatchFile1)

        Application.DoEvents()
        Dlwua("http://download.pso2.jp/patch_prod/patches/launcherlist.txt", "launcherlist.txt")
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strDownloadingPatchFile2)

        Application.DoEvents()
        Dlwua("http://download.pso2.jp/patch_prod/patches/patchlist.txt", "patchlist.txt")
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strDownloadingPatchFile3)

        Application.DoEvents()
        Dlwua("http://download.pso2.jp/patch_prod/patches_old/patchlist.txt", "patchlist_old.txt")
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strDownloadingPatchFile4)

        Application.DoEvents()
        Dim versionclient As New MyWebClient With {.Timeout = 3000}
        versionclient.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")

        WriteDebugInfoSameLine(My.Resources.strDone)
        Application.DoEvents()
        UnlockGui()

        ' TODO: What's going on here?
        Const result As MsgBoxResult = MsgBoxResult.No
        If Not _comingFromOldFiles Then
            TopMost = False
            If chkAlwaysOnTop.Checked Then TopMost = True
        End If

        If result = MsgBoxResult.Yes OrElse _comingFromOldFiles Then
            WriteDebugInfo(My.Resources.strCheckingforNewContent)
            numberofChecks = 0
            missingfiles.Clear()

            Using oReader As StreamReader = File.OpenText("patchlist.txt")
                If _cancelledFull Then Exit Sub
                While Not (oReader.EndOfStream)
                    If _cancelledFull Then Exit Sub
                    sBuffer = oReader.ReadLine()
                    filename = Regex.Split(sBuffer, ".pat")
                    truefilename = filename(0).Replace("data/win32/", "")
                    md5 = filename(1).Split(ControlChars.Tab)
                    trueMd5 = md5(2)
                    If truefilename <> "GameGuard.des" AndAlso truefilename <> "PSO2JP.ini" AndAlso truefilename <> "script/user_default.pso2" AndAlso truefilename <> "script/user_intel.pso2" Then
                        If Not File.Exists((_pso2WinDir & "\" & truefilename)) Then
                            If _vedaUnlocked Then WriteDebugInfo("DEBUG: The file " & truefilename & " is missing.")
                            missingfiles.Add(truefilename)
                        ElseIf Helper.GetMd5((_pso2WinDir & "\" & truefilename)) <> trueMd5 Then
                            If _vedaUnlocked Then WriteDebugInfo("DEBUG: The file " & truefilename & " must be redownloaded.")
                            missingfiles.Add(truefilename)
                        End If
                    End If

                    numberofChecks += 1
                    lblStatus.Text = (My.Resources.strCurrentlyCheckingFile & numberofChecks & "")
                    Application.DoEvents()
                End While
            End Using

            Dim totaldownload As Long = missingfiles.Count
            Dim downloaded As Long = 0
            Dim totaldownloaded As Long = 0
            DeleteFile("resume.txt")

            File.AppendAllLines("resume.txt", missingfiles)

            For Each downloadstring In missingfiles
                If _cancelledFull Then Exit Sub
                'Download the missing files:
                'WHAT THE FUCK IS GOING ON HERE?
                downloaded += 1
                totaldownloaded += _totalsize2
                lblStatus.Text = My.Resources.strDownloading & "" & downloaded & "/" & totaldownload & " (" & Helper.SizeSuffix(totaldownloaded) & ")"

                Application.DoEvents()
                _cancelled = False
                Dlwua(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadstring & ".pat"), downloadstring)
                Dim info7 As New FileInfo(downloadstring)
                If info7.Length = 0 Then
                    Log("File appears to be empty, trying to download from secondary SEGA server")
                    Dlwua(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring & ".pat"), downloadstring)
                End If

                If _cancelled Then Exit Sub
                DeleteFile((_pso2WinDir & "\" & downloadstring))
                File.Move(downloadstring, (_pso2WinDir & "\" & downloadstring))
                If _vedaUnlocked Then WriteDebugInfo("DEBUG: Downloaded and installed " & downloadstring & ".")
                Dim linesList As New List(Of String)(File.ReadAllLines("resume.txt"))

                'Remove the line to delete, e.g.
                linesList.Remove(downloadstring)

                File.WriteAllLines("resume.txt", linesList.ToArray())
                Application.DoEvents()
            Next

            If missingfiles.Count = 0 Then WriteDebugInfo(My.Resources.strYouAppearToBeUpToDate)
            Dim directoryString As String = (_pso2RootDir & "\")
            WriteDebugInfo(My.Resources.strDownloading & "version file...")
            Application.DoEvents()
            _cancelled = False
            Dim versionclient2 As New MyWebClient With {.Timeout = 3000}
            versionclient2.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")

            If _cancelled Then Exit Sub
            Dim directoryString2 As String = _pso2RootDir
            If File.Exists((Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\SEGA\PHANTASYSTARONLINE2\version.ver")) = True Then DeleteFile((Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
            File.Copy("version.ver", (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
            WriteDebugInfoAndOk((My.Resources.strDownloadedandInstalled & "version file"))

            WriteDebugInfo(My.Resources.strDownloading & "pso2launcher.exe...")
            Application.DoEvents()
            Dim procs As Process() = Process.GetProcessesByName("pso2launcher")
            For Each proc As Process In procs
                If proc.MainWindowTitle = "PHANTASY STAR ONLINE 2" AndAlso proc.MainModule.ToString() = "ProcessModule (pso2launcher.exe)" Then proc.Kill()
            Next

            Dlwua("http://download.pso2.jp/patch_prod/patches/pso2launcher.exe.pat", "pso2launcher.exe")
            If _cancelled Then Exit Sub
            If File.Exists((directoryString & "pso2launcher.exe")) AndAlso _startPath <> directoryString2 Then DeleteFile((directoryString & "pso2launcher.exe"))
            File.Move("pso2launcher.exe", (directoryString & "pso2launcher.exe"))
            WriteDebugInfoAndOk((My.Resources.strDownloadedandInstalled & "pso2launcher.exe"))
            WriteDebugInfo(My.Resources.strDownloading & "pso2updater.exe...")
            Application.DoEvents()
            procs = Process.GetProcessesByName("pso2updater")
            For Each proc As Process In procs
                If proc.MainModule.ToString() = "ProcessModule (pso2updater.exe)" Then proc.Kill()
            Next

            Dlwua("http://download.pso2.jp/patch_prod/patches/pso2updater.exe.pat", "pso2updater.exe")
            If _cancelled Then Exit Sub
            If File.Exists((directoryString & "pso2updater.exe")) AndAlso _startPath <> directoryString2 Then DeleteFile((directoryString & "pso2updater.exe"))
            File.Move("pso2updater.exe", (directoryString & "pso2updater.exe"))
            WriteDebugInfoAndOk((My.Resources.strDownloadedandInstalled & "pso2updater.exe"))
            Application.DoEvents()

            WriteDebugInfo(My.Resources.strDownloading & "pso2.exe...")
            procs = Process.GetProcessesByName("pso2")
            For Each proc As Process In procs
                If proc.MainModule.ToString() = "ProcessModule (pso2.exe)" Then proc.Kill()
            Next

            Dlwua("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", "pso2.exe")
            If _cancelled Then Exit Sub

            If File.Exists((directoryString & "pso2.exe")) AndAlso _startPath <> directoryString2 Then DeleteFile((directoryString & "pso2.exe"))
            File.Move("pso2.exe", (directoryString & "pso2.exe"))
            If _cancelledFull Then Exit Sub
            WriteDebugInfoAndOk((My.Resources.strDownloadedandInstalled & "pso2.exe"))
            RegKey.SetValue(Of String)(RegKey.StoryPatchVersion, "Not Installed")
            RegKey.SetValue(Of String)(RegKey.EnPatchVersion, "Not Installed")
            RegKey.SetValue(Of String)(RegKey.LargeFilesVersion, "Not Installed")
            RegKey.SetValue(Of String)(RegKey.Pso2PatchlistMd5, Helper.GetMd5("patchlist.txt"))
            WriteDebugInfo(My.Resources.strGameUpdatedVanilla)
            DeleteFile("resume.txt")
            Dim lines2 = File.ReadAllLines("version.ver")
            Dim remoteVersion2 As String = lines2(0)
            RegKey.SetValue(Of String)(RegKey.Pso2RemoteVersion, remoteVersion2)
            UnlockGui()

            If Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.RemoveCensor)) Then
                If File.Exists((_pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup")) Then My.Computer.FileSystem.DeleteFile((_pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile((_pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c"), "ffbff2ac5b7a7948961212cefd4d402c.backup")
                WriteDebugInfoAndOk(My.Resources.strRemoving & "Censor...")
            End If

            WriteDebugInfoAndOk(My.Resources.strallDone)
            Exit Sub
        End If

        If result = MsgBoxResult.No Then
            _comingFromOldFiles = False
            If _cancelledFull Then Exit Sub
            MergePatches()
            WriteDebugInfo(My.Resources.strCheckingforAllFiles)

            _someofthethings.Remove("GameGuard.des")
            _someofthethings.Remove("PSO2JP.ini")
            _someofthethings.Remove("script/user_default.pso2")
            _someofthethings.Remove("script/user_intel.pso2")
            _someofthethings.Remove("")

            If _someofthethings.ContainsKey("pso2.exe") Then
                _someofthethings.Remove("pso2.exe")
            End If

            Dim dataPath = _pso2RootDir & "\data\win32\"
            Dim length = _someofthethings.Count
            Dim oldmax = PBMainBar.Maximum
            PBMainBar.Maximum = length
            _cancelled = False

            Dim fileLengths = New DirectoryInfo(dataPath).EnumerateFiles().ToDictionary(Function(fileinfo) fileinfo.Name, Function(fileinfo) fileinfo.Length)

            Dim fileNames = fileLengths.Keys

            For Each kvp In _someofthethings

                If _cancelled Then
                    PBMainBar.Text = ""
                    PBMainBar.Value = 0
                    PBMainBar.Maximum = oldmax
                    _someofthethings = Nothing
                    _cancelled = False
                    Exit Sub
                End If

                lblStatus.Text = (My.Resources.strCurrentlyCheckingFile & numberofChecks)
                PBMainBar.Text = numberofChecks & " / " & length
                If (numberofChecks Mod 8) = 0 Then Application.DoEvents()
                numberofChecks += 1
                PBMainBar.Value += 1

                If Not fileNames.Contains(kvp.Key) Then
                    If _vedaUnlocked Then WriteDebugInfo("DEBUG: The file " & (dataPath & kvp.Key) & My.Resources.strIsMissing)
                    testfilesize = kvp.Value.Split(ControlChars.Tab)
                    totalfilesize += Convert.ToInt32(testfilesize(1))
                    missingfiles2.Add(kvp.Key)
                    Continue For
                End If

                testfilesize = kvp.Value.Split(ControlChars.Tab)
                Dim fileSize = Convert.ToInt32(testfilesize(1))

                If fileSize <> fileLengths(kvp.Key) Then
                    If _vedaUnlocked Then WriteDebugInfo("DEBUG: The file " & kvp.Key & " must be redownloaded.")
                    totalfilesize += fileSize
                    missingfiles2.Add(kvp.Key)
                    Continue For
                End If

                Using stream = New FileStream(dataPath & kvp.Key, FileMode.Open)
                    If Helper.GetMd5(stream) <> testfilesize(2) Then
                        If _vedaUnlocked Then WriteDebugInfo("DEBUG: The file " & kvp.Key & " must be redownloaded.")
                        totalfilesize += fileSize
                        missingfiles2.Add(kvp.Key)
                    End If
                End Using
            Next

            PBMainBar.Text = ""
            PBMainBar.Value = 0
            PBMainBar.Maximum = oldmax
            _someofthethings = Nothing

            Dim totaldownload2 As Long = missingfiles2.Count
            Dim downloaded2 As Long = 0
            Dim info As FileInfo
            Dim totaldownloaded As Long = 0
            DeleteFile("resume.txt")
            File.WriteAllLines("resume.txt", missingfiles2.ToArray())

            For Each downloadstring In missingfiles2
                If _cancelledFull Then Exit Sub
                'Download the missing files:
                'WHAT THE FUCK IS GOING ON HERE?
                downloaded2 += 1
                totaldownloaded += _totalsize2

                lblStatus.Text = My.Resources.strDownloading & "" & downloaded2 & "/" & totaldownload2 & " (" & Helper.SizeSuffix(totaldownloaded) & " / " & Helper.SizeSuffix(totalfilesize) & ")"

                Application.DoEvents()
                Dlwua(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadstring & ".pat"), downloadstring)
                Dim info7 As New FileInfo(downloadstring)
                'Dim length2 As Long
                'If File.Exists(downloadstring) Then length2 = info7.Length
                If info7.Length = 0 Then
                    Log("File appears to be empty, trying to download from secondary SEGA server")
                    Dlwua(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring & ".pat"), downloadstring)
                End If
                info = New FileInfo(downloadstring)
                If info.Length = 0 Then
                    DeleteFile(downloadstring)
                    Dlwua(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring & ".pat"), downloadstring)
                End If

                If File.Exists(downloadstring) Then
                    DeleteFile((_pso2WinDir & "\" & downloadstring))
                    File.Move(downloadstring, (_pso2WinDir & "\" & downloadstring))
                    If _vedaUnlocked Then WriteDebugInfo("DEBUG: Downloaded and installed " & downloadstring & ".")
                    Dim linesList As New List(Of String)(File.ReadAllLines("resume.txt"))

                    'Remove the line to delete, e.g.
                    linesList.Remove(downloadstring)

                    File.WriteAllLines("resume.txt", linesList.ToArray())
                    Application.DoEvents()
                Else
                    Application.DoEvents()
                End If
            Next

            If missingfiles.Count = 0 Then WriteDebugInfo(My.Resources.strYouAppearToBeUpToDate)
            Dim directoryString As String = (_pso2RootDir & "\")
            WriteDebugInfo(My.Resources.strDownloading & "version file...")
            Application.DoEvents()
            Dim directoryString2 As String = _pso2RootDir
            Dim versionclient2 As New MyWebClient With {.Timeout = 3000}
            versionclient2.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")

            If File.Exists((Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\SEGA\PHANTASYSTARONLINE2\version.ver")) = True Then DeleteFile((Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
            File.Copy("version.ver", (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
            WriteDebugInfoAndOk((My.Resources.strDownloadedandInstalled & "version file"))

            WriteDebugInfo(My.Resources.strDownloading & "pso2launcher.exe...")
            Application.DoEvents()
            Dlwua("http://download.pso2.jp/patch_prod/patches/pso2launcher.exe.pat", "pso2launcher.exe")
            If File.Exists((directoryString & "pso2launcher.exe")) AndAlso _startPath <> directoryString2 Then DeleteFile((directoryString & "pso2launcher.exe"))
            File.Move("pso2launcher.exe", (directoryString & "pso2launcher.exe"))
            WriteDebugInfoAndOk((My.Resources.strDownloadedandInstalled & "pso2launcher.exe"))
            WriteDebugInfo(My.Resources.strDownloading & "pso2updater.exe...")
            Application.DoEvents()

            WriteDebugInfo(My.Resources.strDownloading & "pso2.exe...")
            Dlwua("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", "pso2.exe")
            If _cancelled Then Exit Sub

            If File.Exists((directoryString & "pso2.exe")) AndAlso _startPath <> directoryString2 Then DeleteFile((directoryString & "pso2.exe"))
            File.Move("pso2.exe", (directoryString & "pso2.exe"))
            If _cancelledFull Then Exit Sub
            WriteDebugInfoAndOk((My.Resources.strDownloadedandInstalled & "pso2.exe"))

            RegKey.SetValue(Of String)(RegKey.StoryPatchVersion, "Not Installed")
            RegKey.SetValue(Of String)(RegKey.EnPatchVersion, "Not Installed")
            RegKey.SetValue(Of String)(RegKey.LargeFilesVersion, "Not Installed")
            RegKey.SetValue(Of String)(RegKey.Pso2PatchlistMd5, Helper.GetMd5("patchlist.txt"))
            WriteDebugInfo(My.Resources.strGameUpdatedVanilla)
            DeleteFile("resume.txt")
            Dim lines3 = File.ReadAllLines("version.ver")
            Dim remoteVersion3 As String = lines3(0)
            RegKey.SetValue(Of String)(RegKey.Pso2RemoteVersion, remoteVersion3)
            UnlockGui()

            If Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.RemoveCensor)) Then
                If File.Exists((_pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup")) Then My.Computer.FileSystem.DeleteFile((_pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile((_pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c"), "ffbff2ac5b7a7948961212cefd4d402c.backup")
                WriteDebugInfoAndOk(My.Resources.strRemoving & "Censor...")
            End If

            WriteDebugInfoAndOk(My.Resources.strallDone)
            Exit Sub
        End If
    End Sub

    Private Sub DeleteFile(path As String)
        Try
            File.Delete(path)
        Catch ex As Exception
            ' Aida put whatever you see fit here plz
            Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub btnRestoreENBackup_Click(sender As Object, e As EventArgs) Handles btnRestoreENBackup.Click
        Try
            If Not Directory.Exists(_pso2WinDir) OrElse _pso2RootDir = "lblDirectory" Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                SelectPso2Directory()
                Exit Sub
            End If
            Dim backupyesno As MsgBoxResult
            If Not _override Then
                backupyesno = MsgBox(My.Resources.strAreYouSureRestoreBackup, vbYesNo)
            End If
            If _override Then backupyesno = MsgBoxResult.Yes
            If backupyesno = MsgBoxResult.Yes Then
                Const backupfolder As String = "backup\English Patch\"
                If Not Directory.Exists((_pso2WinDir & "\" & backupfolder)) Then
                    WriteDebugInfoAndFailed(My.Resources.strCantFindBackupDirectory & (_pso2WinDir & "\" & backupfolder))
                    Exit Sub
                End If
                Dim di As New DirectoryInfo((_pso2WinDir & "\" & backupfolder))
                Dim diar1 As FileInfo() = di.GetFiles()
                WriteDebugInfoAndOk((My.Resources.strRestoringBackupTo & _pso2WinDir))
                Application.DoEvents()
                'list the names of all files in the specified directory
                Dim win32 As String = _pso2WinDir
                For Each dra As FileInfo In diar1
                    If File.Exists(win32 & "\" & dra.ToString()) Then
                        DeleteFile(win32 & "\" & dra.ToString())
                    End If
                    File.Move(((_pso2WinDir & "\" & backupfolder) & "\" & dra.ToString()), (win32 & "\" & dra.ToString()))
                Next
                My.Computer.FileSystem.DeleteDirectory((_pso2WinDir & "\" & backupfolder), FileIO.DeleteDirectoryOption.DeleteAllContents)
                External.FlashWindow(Handle, True)
                WriteDebugInfo(My.Resources.strBackupRestored)
                UnlockGui()
            Else
                Exit Sub
            End If
        Catch ex As Exception
            Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub

    Private Sub ButtonItem4_Click(sender As Object, e As EventArgs) Handles btnRestoreLargeFilesBackup.Click
        Try
            If Not Directory.Exists(_pso2WinDir) OrElse _pso2RootDir = "lblDirectory" Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                SelectPso2Directory()
                Exit Sub
            End If
            Dim backupyesno As MsgBoxResult
            If Not _override Then
                backupyesno = MsgBox(My.Resources.strAreYouSureRestoreBackup, vbYesNo)
            End If
            If _override Then backupyesno = MsgBoxResult.Yes
            If backupyesno = MsgBoxResult.Yes Then
                Const backupfolder As String = "backup\Large Files\"
                If Not Directory.Exists((_pso2WinDir & "\" & backupfolder)) Then
                    WriteDebugInfoAndFailed(My.Resources.strCantFindBackupDirectory & (_pso2WinDir & "\" & backupfolder))
                    Exit Sub
                End If
                Dim di As New DirectoryInfo((_pso2WinDir & "\" & backupfolder))
                Dim diar1 As FileInfo() = di.GetFiles()
                Dim dra As FileInfo
                WriteDebugInfoAndOk((My.Resources.strRestoringBackupTo & _pso2WinDir))
                Application.DoEvents()
                'list the names of all files in the specified directory
                Dim win32 As String = _pso2WinDir
                For Each dra In diar1
                    If File.Exists(win32 & "\" & dra.ToString()) Then
                        DeleteFile(win32 & "\" & dra.ToString())
                    End If
                    File.Move(((_pso2WinDir & "\" & backupfolder) & "\" & dra.ToString()), (win32 & "\" & dra.ToString()))
                Next
                My.Computer.FileSystem.DeleteDirectory((_pso2WinDir & "\" & backupfolder), FileIO.DeleteDirectoryOption.DeleteAllContents)
                WriteDebugInfo(My.Resources.strBackupRestored)
                UnlockGui()
            Else
                Exit Sub
            End If
        Catch ex As Exception
            Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub

    Private Sub btnRestoreJPNames_Click(sender As Object, e As EventArgs) Handles btnRestoreJPNames.Click
        'http://pso2.arghargh200.net/pso2/2013_06_12/ceffe0e2386e8d39f188358303a92a7d
        If File.Exists((_pso2WinDir & "\" & "ceffe0e2386e8d39f188358303a92a7d.backup")) Then
            If File.Exists((_pso2WinDir & "\" & "ceffe0e2386e8d39f188358303a92a7d")) Then My.Computer.FileSystem.DeleteFile((_pso2WinDir & "\" & "ceffe0e2386e8d39f188358303a92a7d"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
            My.Computer.FileSystem.RenameFile((_pso2WinDir & "\" & "ceffe0e2386e8d39f188358303a92a7d.backup"), "ceffe0e2386e8d39f188358303a92a7d")
            WriteDebugInfoAndOk(My.Resources.strRestoring & " JP Names file...")
        Else
            WriteDebugInfoAndOk(My.Resources.strRestoreFailedPleaseReinstall)
        End If
    End Sub

    Private Sub btnRestoreJPETrials_Click(sender As Object, e As EventArgs) Handles btnRestoreJPETrials.Click
        'http://pso2.arghargh200.net/pso2/2013_06_12/057aa975bdd2b372fe092614b0f4399e
        If File.Exists((_pso2WinDir & "\" & "057aa975bdd2b372fe092614b0f4399e.backup")) Then
            If File.Exists((_pso2WinDir & "\" & "057aa975bdd2b372fe092614b0f4399e")) Then My.Computer.FileSystem.DeleteFile((_pso2WinDir & "\" & "057aa975bdd2b372fe092614b0f4399e"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
            My.Computer.FileSystem.RenameFile((_pso2WinDir & "\" & "057aa975bdd2b372fe092614b0f4399e.backup"), "057aa975bdd2b372fe092614b0f4399e")
            WriteDebugInfoAndOk(My.Resources.strRestoring & " JP E-Trials file...")
        Else
            WriteDebugInfoAndOk(My.Resources.strRestoreFailedPleaseReinstall)
        End If
    End Sub

    Private Sub btnAnalyze_Click(sender As Object, e As EventArgs) Handles btnAnalyze.Click
        Dim pso2Launchpath As String = _pso2WinDir.Replace("data\win32", "")
        WriteDebugInfo(My.Resources.strCheckingForNecessaryFiles)
        If Not File.Exists(pso2Launchpath & "Gameguard.DES") Then WriteDebugInfoAndWarning("Missing GameGuard.DES file! " & My.Resources.strPleaseFixGG)
        If Not File.Exists(pso2Launchpath & "pso2.exe") Then WriteDebugInfoAndWarning("Missing pso2.exe file! " & My.Resources.strPleaseFixPSO2EXEs)
        If Not File.Exists(pso2Launchpath & "pso2launcher.exe") Then WriteDebugInfoAndWarning("Missing pso2launcher.exe file! " & My.Resources.strPleaseFixPSO2EXEs)
        If Not File.Exists(pso2Launchpath & "pso2updater.exe") Then WriteDebugInfoAndWarning("Missing pso2updater.exe file! " & My.Resources.strPleaseFixPSO2EXEs)
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strCheckingForFolders)
        If Not Directory.Exists(pso2Launchpath & "\Gameguard\") Then WriteDebugInfoAndWarning("Missing Gameguard folder! " & My.Resources.strPleaseFixGG)
        If Not Directory.Exists(pso2Launchpath & "\data\") Then WriteDebugInfoAndWarning("Missing data folder! " & My.Resources.strPleaseReinstallOrCheck)
        If Not Directory.Exists(pso2Launchpath & "\data\win32\") Then WriteDebugInfoAndWarning("Missing data\win32 folder! " & My.Resources.strPleaseReinstallOrCheck)
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strDoneTesting)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If Not Directory.Exists(_pso2WinDir) OrElse _pso2RootDir = "lblDirectory" Then
            MsgBox(My.Resources.strPleaseSelectwin32Dir)
            SelectPso2Directory()
            Exit Sub
        End If
        Dim sBuffer As String
        Dim filename As String()
        Dim truefilename As String
        Dim missingfiles As New List(Of String)
        Dim sBuffer2 As String
        Dim filename2 As String()
        Dim truefilename2 As String
        Dim missingfiles2 As New List(Of String)
        Dim numberofChecks As Integer
        LockGui()
        WriteDebugInfo(My.Resources.strDownloadingPatchFile1)
        Application.DoEvents()
        Dlwua("http://download.pso2.jp/patch_prod/patches/launcherlist.txt", "launcherlist.txt")
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strDownloadingPatchFile2)
        Application.DoEvents()
        Dlwua("http://download.pso2.jp/patch_prod/patches/patchlist.txt", "patchlist.txt")
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strDownloadingPatchFile3)
        Application.DoEvents()
        Dlwua("http://download.pso2.jp/patch_prod/patches_old/patchlist.txt", "patchlist_old.txt")
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strDownloadingPatchFile4)
        Application.DoEvents()
        Dim versionclient As New MyWebClient With {.Timeout = 3000}
        versionclient.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
        WriteDebugInfoSameLine(My.Resources.strDone)
        Application.DoEvents()
        UnlockGui()
        Log("Opening patch file list...")
        Using oReader As StreamReader = File.OpenText("patchlist.txt")
            If _cancelledFull Then Exit Sub
            If File.Exists((_pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup")) Then
                If File.Exists((_pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c")) Then DeleteFile((_pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c"))
                My.Computer.FileSystem.RenameFile((_pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"), "ffbff2ac5b7a7948961212cefd4d402c")
                WriteDebugInfoAndOk(My.Resources.strRestoring & "Censor...")
            End If
            If File.Exists((_pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup")) Then
                If File.Exists((_pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) Then DeleteFile((_pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927"))
                My.Computer.FileSystem.RenameFile((_pso2WinDir & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup"), "a44fbb2aeb8084c5a5fbe80e219a9927")
                WriteDebugInfoAndOk(My.Resources.strRestoring & "PC Opening...")
            End If
            If File.Exists((_pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75.backup")) Then
                If File.Exists((_pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75")) Then DeleteFile((_pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75"))
                My.Computer.FileSystem.RenameFile((_pso2WinDir & "\" & "7f2368d207e104e8ed6086959b742c75.backup"), "7f2368d207e104e8ed6086959b742c75")
                WriteDebugInfoAndOk(My.Resources.strRestoring & "NVidia Logo...")
            End If
            If File.Exists((_pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771.backup")) Then
                If File.Exists((_pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771")) Then DeleteFile((_pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771"))
                My.Computer.FileSystem.RenameFile((_pso2WinDir & "\" & "009bfec69b04a34576012d50e3417771.backup"), "009bfec69b04a34576012d50e3417771")
                WriteDebugInfoAndOk(My.Resources.strRestoring & "SEGA Logo...")
            End If
            If File.Exists((_pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585.backup")) Then
                If File.Exists((_pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585")) Then DeleteFile((_pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585"))
                My.Computer.FileSystem.RenameFile((_pso2WinDir & "\" & "a93adc766eb3510f7b5c279551a45585.backup"), "a93adc766eb3510f7b5c279551a45585")
                WriteDebugInfoAndOk(My.Resources.strRestoring & "Vita Opening...")
            End If
            WriteDebugInfo(My.Resources.strCheckingFiles)
            While Not (oReader.EndOfStream)
                If _cancelledFull Then Exit Sub
                sBuffer = oReader.ReadLine()
                filename = Regex.Split(sBuffer, ".pat")
                truefilename = filename(0).Replace("data/win32/", "")
                If truefilename <> "GameGuard.des" AndAlso truefilename <> "edition.txt" AndAlso truefilename <> "gameversion.ver" AndAlso truefilename <> "pso2.exe" AndAlso truefilename <> "PSO2JP.ini" AndAlso truefilename <> "script/user_default.pso2" AndAlso truefilename <> "script/user_intel.pso2" Then
                    Dim info7 As New FileInfo((_pso2WinDir & "\" & truefilename))
                    Dim length2 As Long
                    If File.Exists(_pso2WinDir & "\" & truefilename) Then length2 = info7.Length
                    If Not File.Exists((_pso2WinDir & "\" & truefilename)) Then
                        WriteDebugInfo(truefilename & My.Resources.strIsMissing)
                        missingfiles.Add(truefilename)
                    End If
                    info7 = New FileInfo((_pso2WinDir & "\" & truefilename))
                    If File.Exists(_pso2WinDir & "\" & truefilename) Then length2 = info7.Length
                    If Not File.Exists(_pso2WinDir & "\" & truefilename) Then length2 = 1
                    If length2 = 0 Then
                        WriteDebugInfo(truefilename & " has a filesize of 0!")
                        missingfiles.Add(truefilename)
                        DeleteFile(_pso2WinDir & "\" & truefilename)
                    End If
                End If
                numberofChecks += 1
                lblStatus.Text = (My.Resources.strCurrentlyCheckingFile & numberofChecks)
                Application.DoEvents()
            End While
        End Using
        Log("Opening Second patch file...")
        Using oReader As StreamReader = File.OpenText("patchlist_old.txt")
            While Not (oReader.EndOfStream)
                If _cancelledFull Then Exit Sub
                sBuffer2 = oReader.ReadLine()
                filename2 = Regex.Split(sBuffer2, ".pat")
                truefilename2 = filename2(0).Replace("data/win32/", "")
                If truefilename2 <> "GameGuard.des" AndAlso truefilename2 <> "pso2.exe" AndAlso truefilename2 <> "PSO2JP.ini" AndAlso truefilename2 <> "script/user_default.pso2" AndAlso truefilename2 <> "script/user_intel.pso2" Then
                    Dim info7 As New FileInfo((_pso2WinDir & "\" & truefilename2))
                    Dim length2 As Long
                    If File.Exists(_pso2WinDir & "\" & truefilename2) Then length2 = info7.Length
                    If Not File.Exists((_pso2WinDir & "\" & truefilename2)) Then
                        If missingfiles.Contains(truefilename2) Then
                            'Do nothing
                        Else
                            WriteDebugInfo(truefilename2 & My.Resources.strIsMissing)
                            missingfiles2.Add(truefilename2)
                        End If
                    End If
                    info7 = New FileInfo((_pso2WinDir & "\" & truefilename2))
                    If File.Exists(_pso2WinDir & "\" & truefilename2) Then length2 = info7.Length
                    If Not File.Exists(_pso2WinDir & "\" & truefilename2) Then length2 = 1
                    If length2 = 0 Then
                        WriteDebugInfo(truefilename2 & " has a filesize of 0!")
                        missingfiles.Add(truefilename2)
                        DeleteFile(_pso2WinDir & "\" & truefilename2)
                    End If
                End If
                numberofChecks += 1
                lblStatus.Text = (My.Resources.strCurrentlyCheckingFile & numberofChecks)
                Application.DoEvents()
            End While
            If missingfiles.Count = 0 AndAlso missingfiles2.Count = 0 Then
                WriteDebugInfoAndOk(My.Resources.strAllFilesCheckedOut)
                Exit Sub
            End If
        End Using

        Dim result1 As DialogResult = MessageBox.Show(My.Resources.strWouldYouLikeToDownloadInstallMissing, "Download/Install?", MessageBoxButtons.YesNo)

        If result1 = DialogResult.No Then Exit Sub

        If result1 = DialogResult.Yes Then
            Log(My.Resources.strDownloading & My.Resources.strMissingFilesPart1)
            Dim totaldownload As Long = missingfiles.Count
            Dim downloaded As Long = 0
            Dim totaldownloaded As Long = 0
            DeleteFile("resume.txt")

            File.AppendAllLines("resume.txt", missingfiles)

            For Each downloadstring As String In missingfiles
                'Download the missing files:
                _cancelled = False
                downloaded += 1
                totaldownloaded += _totalsize2
                lblStatus.Text = My.Resources.strDownloading & "" & downloaded & "/" & totaldownload & " (" & Helper.SizeSuffix(totaldownloaded) & ")"

                Dlwua(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadstring & ".pat"), downloadstring)
                Dim info7 As New FileInfo(downloadstring)

                If info7.Length = 0 Then
                    Log("File appears to be empty, trying to download from secondary SEGA server")
                    Dlwua(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring & ".pat"), downloadstring)
                End If

                If _cancelled Then Exit Sub
                File.Move(downloadstring, (_pso2WinDir & "\" & downloadstring))
                WriteDebugInfoAndOk((My.Resources.strDownloadedandInstalled & downloadstring & "."))
                Dim linesList As New List(Of String)(File.ReadAllLines("resume.txt"))

                'Remove the line to delete, e.g.
                linesList.Remove(downloadstring)

                File.WriteAllLines("resume.txt", linesList.ToArray())
                If _cancelledFull Then Exit Sub
            Next

            Log(My.Resources.strDownloading & My.Resources.strMissingFilesPart2)

            DeleteFile("resume.txt")

            File.AppendAllLines("resume.txt", missingfiles2)

            Dim totaldownload2 As Long = missingfiles2.Count
            Dim downloaded2 As Long = 0
            Dim totaldownloaded2 As Long = 0

            For Each downloadstring2 In missingfiles2
                If _cancelledFull Then Exit Sub
                'Download the missing files:
                If Not File.Exists((_pso2WinDir & "\" & downloadstring2)) Then
                    _cancelled = False
                    downloaded2 += 1
                    totaldownloaded2 += _totalsize2
                    lblStatus.Text = My.Resources.strDownloading & "" & downloaded2 & "/" & totaldownload2 & " (" & Helper.SizeSuffix(totaldownloaded2) & ")"

                    Dlwua(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadstring2 & ".pat"), downloadstring2)
                    Dim info7 As New FileInfo(downloadstring2)

                    If info7.Length = 0 Then
                        Log("File appears to be empty, trying to download from secondary SEGA server")
                        Dlwua(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring2 & ".pat"), downloadstring2)
                    End If

                    If _cancelled Then Exit Sub
                    File.Move(downloadstring2, (_pso2WinDir & "\" & downloadstring2))
                    WriteDebugInfoAndOk((My.Resources.strDownloadedandInstalled & downloadstring2 & "."))
                    Dim linesList As New List(Of String)(File.ReadAllLines("resume.txt"))

                    'Remove the line to delete, e.g.
                    linesList.Remove(downloadstring2)
                    File.WriteAllLines("resume.txt", linesList.ToArray())
                End If
            Next
        End If
        WriteDebugInfoAndOk(My.Resources.strallDone)
    End Sub

    Private Sub ButtonItem10_Click(sender As Object, e As EventArgs) Handles ButtonItem10.Click
        LockGui()
        WriteDebugInfo(My.Resources.strDownloadingPatchFile1)
        Application.DoEvents()
        Dlwua("http://download.pso2.jp/patch_prod/patches/launcherlist.txt", "launcherlist.txt")
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strDownloadingPatchFile2)
        Application.DoEvents()
        Dlwua("http://download.pso2.jp/patch_prod/patches/patchlist.txt", "patchlist.txt")
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strDownloadingPatchFile3)
        Application.DoEvents()
        Dlwua("http://download.pso2.jp/patch_prod/patches_old/patchlist.txt", "patchlist_old.txt")
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strDownloadingPatchFile4)
        Application.DoEvents()
        Dim versionclient As New MyWebClient With {.Timeout = 3000}
        versionclient.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
        WriteDebugInfoSameLine(My.Resources.strDone)
        Application.DoEvents()
        UnlockGui()
        WriteDebugInfo(My.Resources.strCheckingForMissingOldFiles)
        _comingFromOldFiles = True
        _cancelledFull = False
        ButtonItem5.RaiseClick()
    End Sub

    Private Sub btnGameguard_Click(sender As Object, e As EventArgs) Handles btnGameguard.Click
        Try
            Dim systempath As String
            MsgBox(My.Resources.strPleaseBeAwareGG)
            Dim pso2Launchpath As String = _pso2RootDir

            If Directory.Exists(pso2Launchpath & "\Gameguard\") Then
                WriteDebugInfo("Removing Gameguard Directory...")
                Directory.Delete(pso2Launchpath & "\Gameguard\", True)
                WriteDebugInfoSameLine(My.Resources.strDone)
            End If
            If File.Exists(pso2Launchpath & "\GameGuard.des") Then
                WriteDebugInfo("Removing Gameguard File...")
                DeleteFile(pso2Launchpath & "\GameGuard.des")
                WriteDebugInfoSameLine(My.Resources.strDone)
            End If
            If Environment.Is64BitOperatingSystem Then
                systempath = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86)
                If File.Exists(systempath & "\npptnt2.sys") Then
                    WriteDebugInfo("Removing Hidden Gameguard Files (1 of 3)...")
                    DeleteFile(systempath & "\npptnt2.sys")
                    WriteDebugInfoSameLine(My.Resources.strDone)
                End If
                If File.Exists(systempath & "\nppt9x.vxd") Then
                    WriteDebugInfo("Removing Hidden Gameguard Files (2 of 3)...")
                    DeleteFile(systempath & "\nppt9x.vxd")
                    WriteDebugInfoSameLine(My.Resources.strDone)
                End If
                If File.Exists(systempath & "\GameMon.des") Then
                    WriteDebugInfo("Removing Hidden Gameguard Files (3 of 3)...")
                    DeleteFile(systempath & "\GameMon.des")
                    WriteDebugInfoSameLine(My.Resources.strDone)
                End If
            End If
            If Not Environment.Is64BitOperatingSystem Then
                systempath = Environment.GetFolderPath(Environment.SpecialFolder.System)
                If File.Exists(systempath & "\npptnt2.sys") Then
                    WriteDebugInfo("Removing Hidden Gameguard Files (1 of 3)...")
                    DeleteFile(systempath & "\npptnt2.sys")
                    WriteDebugInfoSameLine(My.Resources.strDone)
                End If
                If File.Exists(systempath & "\nppt9x.vxd") Then
                    WriteDebugInfo("Removing Hidden Gameguard Files (2 of 3)...")
                    DeleteFile(systempath & "\nppt9x.vxd")
                    WriteDebugInfoSameLine(My.Resources.strDone)
                End If
                If File.Exists(systempath & "\GameMon.des") Then
                    WriteDebugInfo("Removing Hidden Gameguard Files (3 of 3)...")
                    DeleteFile(systempath & "\GameMon.des")
                    WriteDebugInfoSameLine(My.Resources.strDone)
                End If
            End If
            WriteDebugInfo("Downloading Latest Gameguard file...")
            Dlwua("http://download.pso2.jp/patch_prod/patches/GameGuard.des.pat", "GameGuard.des")
            WriteDebugInfo("Downloading Latest Gameguard config...")
            Dlwua("http://download.pso2.jp/patch_prod/patches/PSO2JP.ini.pat", pso2Launchpath & "\PSO2JP.ini")
            WriteDebugInfoSameLine(My.Resources.strDone)
            File.Move("GameGuard.des", pso2Launchpath & "\GameGuard.des")

            Dim foundKey As RegistryKey = My.Computer.Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Services\npggsvc", True)

            If foundKey Is Nothing Then
                WriteDebugInfo("No registry keys to delete. This is OK, they should be created the next time Gameguard launches.")
            Else
                WriteDebugInfo("Deleting Gameguard registry keys...")
                foundKey = My.Computer.Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Services", True)
                foundKey.DeleteSubKeyTree("npggsvc")
                WriteDebugInfoSameLine(My.Resources.strDone)
            End If
            WriteDebugInfoAndOk(My.Resources.strGGReset)
        Catch ex As Exception
            Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub btnFixPSO2EXEs_Click(sender As Object, e As EventArgs) Handles btnFixPSO2EXEs.Click
        Try
            If Not Directory.Exists(_pso2WinDir) OrElse _pso2RootDir = "lblDirectory" Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                SelectPso2Directory()
                Exit Sub
            End If
            Dim directoryString As String = (_pso2RootDir & "\")
            _cancelled = False
            WriteDebugInfo(My.Resources.strDownloading & "pso2launcher.exe...")

            Application.DoEvents()
            Dim procs As Process() = Process.GetProcessesByName("pso2launcher")

            For Each proc As Process In procs
                If proc.MainWindowTitle = "PHANTASY STAR ONLINE 2" AndAlso proc.MainModule.ToString() = "ProcessModule (pso2launcher.exe)" Then proc.Kill()
            Next

            Dlwua("http://download.pso2.jp/patch_prod/patches/pso2launcher.exe.pat", "pso2launcher.exe")
            If _cancelled Then Exit Sub

            ' never mind I'll just rewrite it later or something idk
            If File.Exists((directoryString & "pso2launcher.exe")) AndAlso _startPath <> _pso2RootDir Then DeleteFile((directoryString & "pso2launcher.exe"))
            File.Move("pso2launcher.exe", (directoryString & "pso2launcher.exe"))
            WriteDebugInfoAndOk((My.Resources.strDownloadedandInstalled & "pso2launcher.exe"))
            WriteDebugInfo(My.Resources.strDownloading & "pso2updater.exe...")
            Application.DoEvents()
            procs = Process.GetProcessesByName("pso2updater")
            For Each proc As Process In procs
                If proc.MainModule.ToString() = "ProcessModule (pso2updater.exe)" Then proc.Kill()
            Next
            Dlwua("http://download.pso2.jp/patch_prod/patches/pso2updater.exe.pat", "pso2updater.exe")
            If _cancelled Then Exit Sub

            If File.Exists((directoryString & "pso2updater.exe")) AndAlso _startPath <> _pso2RootDir Then DeleteFile((directoryString & "pso2updater.exe"))
            File.Move("pso2updater.exe", (directoryString & "pso2updater.exe"))
            WriteDebugInfoAndOk((My.Resources.strDownloadedandInstalled & "pso2updater.exe"))
            WriteDebugInfo(My.Resources.strDownloading & "pso2.exe...")
            Application.DoEvents()
            procs = Process.GetProcessesByName("pso2")
            For Each proc As Process In procs
                If proc.MainModule.ToString() = "ProcessModule (pso2.exe)" Then proc.Kill()
            Next
            Dlwua("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", "pso2.exe")
            If _cancelled Then Exit Sub

            If File.Exists((directoryString & "pso2.exe")) AndAlso _startPath <> _pso2RootDir Then DeleteFile((directoryString & "pso2.exe"))
            File.Move("pso2.exe", (directoryString & "pso2.exe"))
            WriteDebugInfoAndOk((My.Resources.strDownloadedandInstalled & "pso2.exe"))
            Application.DoEvents()
            WriteDebugInfo(My.Resources.strAllNecessaryFiles)
            UnlockGui()
        Catch ex As Exception
            Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub

    Private Sub btnRestoreStoryBackup_Click(sender As Object, e As EventArgs) Handles btnRestoreStoryBackup.Click
        Try
            If Not Directory.Exists(_pso2WinDir) OrElse _pso2RootDir = "lblDirectory" Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                SelectPso2Directory()
                Exit Sub
            End If
            Dim backupyesno As MsgBoxResult
            If Not _override Then
                backupyesno = MsgBox(My.Resources.strAreYouSureRestoreBackup, vbYesNo)
            End If
            If _override Then backupyesno = MsgBoxResult.Yes
            If backupyesno = MsgBoxResult.Yes Then
                Const backupfolder As String = "backup\Story Patch\"
                If Not Directory.Exists((_pso2WinDir & "\" & backupfolder)) Then
                    WriteDebugInfoAndFailed(My.Resources.strCantFindBackupDirectory & (_pso2WinDir & "\" & backupfolder))
                    Exit Sub
                End If
                Dim di As New DirectoryInfo((_pso2WinDir & "\" & backupfolder))
                Dim diar1 As FileInfo() = di.GetFiles()
                Dim dra As FileInfo
                WriteDebugInfoAndOk((My.Resources.strRestoringBackupTo & _pso2WinDir))
                Application.DoEvents()
                'list the names of all files in the specified directory
                Dim win32 As String = _pso2WinDir
                For Each dra In diar1
                    If File.Exists(win32 & "\" & dra.ToString()) Then
                        DeleteFile(win32 & "\" & dra.ToString())
                    End If
                    File.Move(((_pso2WinDir & "\" & backupfolder) & "\" & dra.ToString()), (win32 & "\" & dra.ToString()))
                Next
                My.Computer.FileSystem.DeleteDirectory((_pso2WinDir & "\" & backupfolder), FileIO.DeleteDirectoryOption.DeleteAllContents)
                WriteDebugInfo(My.Resources.strBackupRestored)
                UnlockGui()
            Else
                Exit Sub
            End If
        Catch ex As Exception
            Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub

    Private Sub btnFixPermissions_Click(sender As Object, e As EventArgs) Handles btnFixPermissions.Click
        Try
            'SystemInformation.UserName
            'cacls.exe "C:\PHANTASYSTARONLINE2\pso2_bin\pso2.exe" /e /g "AIDA":F
            'cacls.exe "C:\PHANTASYSTARONLINE2\pso2_bin\pso2launcher.exe" /e /g "AIDA":F
            'cacls.exe "C:\PHANTASYSTARONLINE2\pso2_bin\pso2download.exe" /e /g "AIDA":F
            'cacls.exe "C:\PHANTASYSTARONLINE2\pso2_bin\pso2updater.exe" /e /g "AIDA":F
            'cacls.exe "C:\PHANTASYSTARONLINE2\pso2_bin\pso2predownload.exe" /e /g "AIDA":F
            MsgBox(My.Resources.strFixPermissionIssuesText)
            Dim directoryString As String = (_pso2RootDir & "\")
            'MsgBox("cacls.exe" & (DirectoryString & "pso2.exe") & " /e /g """ & SystemInformation.UserName & """:F")
            WriteDebugInfo(My.Resources.strFixingPermissionsFor & "pso2.exe...")
            Application.DoEvents()
            Process.Start("cacls.exe", (directoryString & "pso2.exe") & " /e /g """ & SystemInformation.UserName & """:F")
            WriteDebugInfoSameLine(My.Resources.strDone)
            WriteDebugInfo(My.Resources.strFixingPermissionsFor & "pso2launcher.exe...")
            Application.DoEvents()
            Process.Start("cacls.exe", (directoryString & "pso2launcher.exe") & " /e /g """ & SystemInformation.UserName & """:F")
            WriteDebugInfoSameLine(My.Resources.strDone)
            WriteDebugInfo(My.Resources.strFixingPermissionsFor & "pso2download.exe...")
            Application.DoEvents()
            Process.Start("cacls.exe", (directoryString & "pso2download.exe") & " /e /g """ & SystemInformation.UserName & """:F")
            WriteDebugInfoSameLine(My.Resources.strDone)
            WriteDebugInfo(My.Resources.strFixingPermissionsFor & "pso2updater.exe...")
            Application.DoEvents()
            Process.Start("cacls.exe", (directoryString & "pso2updater.exe") & " /e /g """ & SystemInformation.UserName & """:F")
            WriteDebugInfoSameLine(My.Resources.strDone)
            WriteDebugInfo(My.Resources.strFixingPermissionsFor & "pso2predownload.exe...")
            Application.DoEvents()
            Process.Start("cacls.exe", (directoryString & "pso2predownload.exe") & " /e /g """ & SystemInformation.UserName & """:F")
            WriteDebugInfoSameLine(My.Resources.strDone)
            MsgBox(My.Resources.strFixPermissionsDone)
        Catch ex As Exception
            Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub ButtonItem17_Click(sender As Object, e As EventArgs) Handles ButtonItem17.Click
        Dim whatthefuck As MsgBoxResult = MsgBox(My.Resources.strAreYouSureResetPSO2Settings, MsgBoxStyle.YesNo)
        If whatthefuck = MsgBoxResult.Yes Then
            Dim documents As String = (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\")
            Dim usersettingsfile As String = (documents & "SEGA\PHANTASYSTARONLINE2\user.pso2")
            File.WriteAllText(usersettingsfile, txtPSO2DefaultINI.Text)
            WriteDebugInfoAndOk(My.Resources.strPSO2SettingsReset)
        End If
    End Sub

    Private Sub btnTerminate_Click(sender As Object, e As EventArgs) Handles btnTerminate.Click
        WriteDebugInfo(My.Resources.strTerminatePSO2)
        Dim procs As Process() = Process.GetProcessesByName("pso2")
        For Each proc As Process In procs
            If proc.MainModule.ToString() = "ProcessModule (pso2.exe)" Then proc.Kill()
            If proc.MainModule.ToString() = "ProcessModule (GameMon.des)" Then proc.Kill()
            If proc.MainModule.ToString() = "ProcessModule (GameMon64.des)" Then proc.Kill()
        Next
        WriteDebugInfoSameLine(My.Resources.strDone)
    End Sub

    Private Sub btnBumped_Click(sender As Object, e As EventArgs) Handles btnBumped.Click
        Process.Start("http://bumped.org/psublog/")
    End Sub

    Private Sub btnCirno_Click(sender As Object, e As EventArgs) Handles btnCirno.Click
        Process.Start("http://pso2.cirnopedia.info/")
    End Sub

    Private Sub btnArksCash_Click(sender As Object, e As EventArgs) Handles btnArksCash.Click
        Process.Start("http://www.pso-world.com/forums/showthread.php?t=199490")
    End Sub

    Private Sub btnErrors_Click(sender As Object, e As EventArgs) Handles btnErrors.Click
        Process.Start("http://www.pso-world.com/forums/showthread.php?t=204836")
    End Sub

    Private Sub btnOfficialPSO2JP_Click(sender As Object, e As EventArgs) Handles btnOfficialPSO2JP.Click
        Process.Start("http://cyberk.it/pso2mirror/")
    End Sub

    Private Sub btnRegistration_Click(sender As Object, e As EventArgs) Handles btnRegistration.Click
        Process.Start("http://www.pso-world.com/forums/showthread.php?t=210236")
    End Sub

    Private Sub btnTweaker_Click(sender As Object, e As EventArgs) Handles btnTweaker.Click
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
            Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
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
            Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub btnENPatch_Click(sender As Object, e As EventArgs) Handles btnENPatch.Click
        ' Here we parse the text file before passing it to the DownloadPatch function.
        ' The Using statement will dispose "net" as soon as we're done with it.
        Using net As New WebClient()
            ' If we decide not to, we can do away with "url" and just pass net.DownloadString in as the parameter.
            ' Furthermore, we could also parse it from within the function.
            Dim url As String = net.DownloadString(_freedomUrl & "patches/enpatch.txt")
            DownloadPatch(url, "EN Patch", "ENPatch.rar", RegKey.EnPatchVersion, My.Resources.strBackupEN, My.Resources.strPleaseSelectPreDownloadENRAR, "backup\English Patch\")
        End Using
    End Sub

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Close()
    End Sub

    Private Sub btnEXPFULL_Click(sender As Object, e As EventArgs) Handles btnEXPFULL.Click
        Process.Start("http://www.expfull.com/chat")
    End Sub

    Private Sub btnAnnouncements_Click(sender As Object, e As EventArgs) Handles btnAnnouncements.Click
        If _dpiSetting = 96 Then
            If Width = 420 Then
                Width = 796
                btnAnnouncements.Text = "<"
                If RegKey.GetValue(Of String)(RegKey.SidebarEnabled) = "False" Then
                    WriteDebugInfo(My.Resources.strLoadingSidebarPage)
                    ThreadPool.QueueUserWorkItem(AddressOf LoadSidebar, Nothing)
                End If
                Exit Sub
            End If
            If Width = 796 Then
                Width = 420
                btnAnnouncements.Text = ">"
                Exit Sub
            End If
        End If
        If _dpiSetting = 120 Then
            If Width = 560 Then
                Width = 1060
                btnAnnouncements.Text = "<"
                If RegKey.GetValue(Of String)(RegKey.SidebarEnabled) = "False" Then
                    WriteDebugInfo(My.Resources.strLoadingSidebarPage)
                    ThreadPool.QueueUserWorkItem(AddressOf LoadSidebar, Nothing)
                End If
                Exit Sub
            End If
            If Width = 1060 Then
                Width = 560
                btnAnnouncements.Text = ">"
                Exit Sub
            End If
        End If
    End Sub

    Private Sub WebBrowser4_Navigating(sender As Object, e As WebBrowserNavigatingEventArgs) Handles WebBrowser4.Navigating
        If Visible Then
            If e.Url.ToString() <> _freedomUrl & "tweaker.html" Then
                Process.Start(e.Url.ToString())
                Log("Trying to load URL for sidebar: " & e.Url.ToString)
                ThreadPool.QueueUserWorkItem(AddressOf LoadSidebar, Nothing)
            End If
        End If
    End Sub

    Private Sub btnUninstallENPatch_Click(sender As Object, e As EventArgs) Handles btnUninstallENPatch.Click
        UninstallPatch(_freedomUrl & "patches/enpatchfilelist.txt", "enpatchfilelist.txt", "backup\English Patch\", My.Resources.strENPatchUninstalled, RegKey.EnPatchVersion)
    End Sub

    Private Sub btnUninstallLargeFiles_Click(sender As Object, e As EventArgs) Handles btnUninstallLargeFiles.Click
        UninstallPatch(_freedomUrl & "patches/largefilelist.txt", "largefilelist.txt", "backup\Large Files\", My.Resources.strLFUninstalled, RegKey.LargeFilesVersion)
    End Sub

    Private Sub btnUninstallStory_Click(sender As Object, e As EventArgs) Handles btnUninstallStory.Click
        UninstallPatch(_freedomUrl & "patches/storyfilelist.txt", "storyfilelist.txt", "backup\Story Patch\", My.Resources.strStoryPatchUninstalled, RegKey.StoryPatchVersion)
    End Sub

    Private Sub btnRussianPatch_Click(sender As Object, e As EventArgs) Handles btnRussianPatch.Click
        DownloadPatch("http://dl.cyberman.me/pso2/rupatch.rar", "RU Patch", "RUPatch.rar", Nothing,
                      "Would you like to backup your files before applying the patch? This will erase all previous Pre-RU patch backups.",
                      "Please select the pre-downloaded RU Patch RAR file", "backup\Russian Patch\")
    End Sub

    Private Sub tsmRestartDownload_Click(sender As Object, e As EventArgs) Handles tsmRestartDownload.Click
        _restartplz = True
    End Sub

    Private Sub btnResumePatching_Click(sender As Object, e As EventArgs) Handles btnResumePatching.Click
        If Not File.Exists("resume.txt") Then
            WriteDebugInfo(My.Resources.strCannotFindResume)
            Exit Sub
        End If

        _cancelledFull = False

        Try
            Dim sBuffer As String
            Dim missingfiles As New List(Of String)
            missingfiles.Clear()
            Using oReader As StreamReader = File.OpenText("resume.txt")
                WriteDebugInfoAndOk(My.Resources.strFoundIncompleteJob)
                If _cancelledFull Then Exit Sub
                While Not (oReader.EndOfStream)
                    If _cancelledFull Then Exit Sub
                    sBuffer = oReader.ReadLine()
                    missingfiles.Add(sBuffer)
                End While
            End Using

            Dim totaldownload As Long = missingfiles.Count
            Dim downloaded As Long = 0
            Dim totaldownloaded As Long = 0
            For Each downloadstring As String In missingfiles
                If _cancelledFull Then Exit Sub
                'Download the missing files:
                'WHAT THE FUCK IS GOING ON HERE?v3
                downloaded += 1
                totaldownloaded += _totalsize2

                lblStatus.Text = My.Resources.strDownloading & "" & downloaded & "/" & totaldownload & " (" & Helper.SizeSuffix(totaldownloaded) & ")"

                Application.DoEvents()
                _cancelled = False
                Dlwua(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadstring & ".pat"), downloadstring)
                Dim info7 As New FileInfo(downloadstring)
                'Dim length2 As Long
                'If File.Exists(downloadstring) Then length2 = info7.Length
                If info7.Length = 0 Then
                    Log("File appears to be empty, trying to download from secondary SEGA server")
                    Dlwua(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring & ".pat"), downloadstring)
                End If
                If _cancelled Then Exit Sub
                'Delete the existing file FIRST
                DeleteFile((_pso2WinDir & "\" & downloadstring))
                File.Move(downloadstring, (_pso2WinDir & "\" & downloadstring))
                If _vedaUnlocked Then WriteDebugInfo("DEBUG: Downloaded and installed " & downloadstring & ".")
                Dim linesList As New List(Of String)(File.ReadAllLines("resume.txt"))

                'Remove the line to delete, e.g.
                linesList.Remove(downloadstring)
                File.WriteAllLines("resume.txt", linesList.ToArray())
                Application.DoEvents()
            Next
            DeleteFile("resume.txt")
            If missingfiles.Count = 0 Then WriteDebugInfo(My.Resources.strYouAppearToBeUpToDate)
            Dim directoryString As String = (_pso2RootDir & "\")
            WriteDebugInfo(My.Resources.strDownloading & "version file...")
            Application.DoEvents()
            _cancelled = False
            Dim versionclient As New MyWebClient With {.Timeout = 3000}
            versionclient.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
            If _cancelled Then Exit Sub
            Dim directoryString2 As String = _pso2RootDir
            If File.Exists((Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\SEGA\PHANTASYSTARONLINE2\version.ver")) = True Then DeleteFile((Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
            File.Copy("version.ver", (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
            WriteDebugInfoAndOk((My.Resources.strDownloadedandInstalled & "version file"))
            WriteDebugInfo(My.Resources.strDownloading & "pso2launcher.exe...")
            Application.DoEvents()
            Dim procs = Process.GetProcessesByName("pso2launcher")
            For Each proc As Process In procs
                If proc.MainWindowTitle = "PHANTASY STAR ONLINE 2" AndAlso proc.MainModule.ToString() = "ProcessModule (pso2launcher.exe)" Then proc.Kill()
            Next
            Dlwua("http://download.pso2.jp/patch_prod/patches/pso2launcher.exe.pat", "pso2launcher.exe")
            If _cancelled Then Exit Sub
            If File.Exists((directoryString & "pso2launcher.exe")) AndAlso _startPath <> directoryString2 Then DeleteFile((directoryString & "pso2launcher.exe"))
            File.Move("pso2launcher.exe", (directoryString & "pso2launcher.exe"))
            WriteDebugInfoAndOk((My.Resources.strDownloadedandInstalled & "pso2launcher.exe"))
            WriteDebugInfo(My.Resources.strDownloading & "pso2updater.exe...")
            Application.DoEvents()
            procs = Process.GetProcessesByName("pso2updater")
            For Each proc As Process In procs
                If proc.MainModule.ToString() = "ProcessModule (pso2updater.exe)" Then proc.Kill()
            Next
            Dlwua("http://download.pso2.jp/patch_prod/patches/pso2updater.exe.pat", "pso2updater.exe")
            If _cancelled Then Exit Sub
            If File.Exists((directoryString & "pso2updater.exe")) AndAlso _startPath <> directoryString2 Then DeleteFile((directoryString & "pso2updater.exe"))
            File.Move("pso2updater.exe", (directoryString & "pso2updater.exe"))
            WriteDebugInfoAndOk((My.Resources.strDownloadedandInstalled & "pso2updater.exe"))
            Application.DoEvents()
            WriteDebugInfo(My.Resources.strDownloading & "pso2.exe...")
            procs = Process.GetProcessesByName("pso2")

            For Each proc As Process In procs
                If proc.MainModule.ToString() = "ProcessModule (pso2.exe)" Then proc.Kill()
            Next

            Dlwua("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", "pso2.exe")
            If _cancelled Then Exit Sub

            If File.Exists((directoryString & "pso2.exe")) AndAlso _startPath <> directoryString2 Then DeleteFile((directoryString & "pso2.exe"))
            File.Move("pso2.exe", (directoryString & "pso2.exe"))
            If _cancelledFull Then Exit Sub
            WriteDebugInfoAndOk((My.Resources.strDownloadedandInstalled & "pso2.exe"))
            RegKey.SetValue(Of String)(RegKey.StoryPatchVersion, "Not Installed")
            RegKey.SetValue(Of String)(RegKey.EnPatchVersion, "Not Installed")
            RegKey.SetValue(Of String)(RegKey.LargeFilesVersion, "Not Installed")
            Dlwua("http://download.pso2.jp/patch_prod/patches/patchlist.txt", "patchlist.txt")
            WriteDebugInfoSameLine(My.Resources.strDone)
            RegKey.SetValue(Of String)(RegKey.Pso2PatchlistMd5, Helper.GetMd5("patchlist.txt"))
            WriteDebugInfo(My.Resources.strGameUpdatedVanilla)
            DeleteFile("resume.txt")
            Dim lines2 = File.ReadAllLines("version.ver")
            Dim remoteVersion2 As String = lines2(0)
            RegKey.SetValue(Of String)(RegKey.Pso2RemoteVersion, remoteVersion2)
            UnlockGui()

            If Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.RemoveCensor)) Then
                If File.Exists((_pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup")) Then My.Computer.FileSystem.DeleteFile((_pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile((_pso2WinDir & "\" & "ffbff2ac5b7a7948961212cefd4d402c"), "ffbff2ac5b7a7948961212cefd4d402c.backup")
                WriteDebugInfoAndOk(My.Resources.strRemoving & "Censor...")
            End If

            If Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.EnPatchAfterInstall)) Then
                WriteDebugInfo(My.Resources.strAutoInstallingENPatch)
                btnENPatch.RaiseClick()
            End If

            If Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.LargeFilesAfterInstall)) Then
                WriteDebugInfo(My.Resources.strAutoInstallingLF)
                btnLargeFiles.RaiseClick()
            End If

            If Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.StoryPatchAfterInstall)) Then
                WriteDebugInfo(My.Resources.strAutoInstallingStoryPatch)
                btnStory.RaiseClick()
            End If

            WriteDebugInfoAndOk(My.Resources.strallDone)
            Exit Sub

        Catch ex As Exception
            Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            If ex.Message <> "Arithmetic operation resulted in an overflow." Then
                WriteDebugInfo(My.Resources.strERROR & ex.Message)
                Exit Sub
            End If
        End Try
    End Sub

    Private Sub ButtonItem7_Click(sender As Object, e As EventArgs) Handles ButtonItem7.Click
        Const processName As String = "chrome"
        _processes = Process.GetProcessesByName("chrome")
        Dim currentProcess As Process = Process.GetCurrentProcess()

        ' TODO: What is going on with this?
        Const x As Integer = 0

        If _processes.Length > x Then
            Dim closeItYesNo As MsgBoxResult = MsgBox("You need to have all Chrome windows closed before launching in this mode. Would you like to close all open Chrome windows now?", vbYesNo)
            If closeItYesNo = vbYes Then
                Dim procs As Process() = Process.GetProcessesByName(processName)

                For Each proc As Process In procs
                    If proc.Id <> currentProcess.Id Then proc.Kill()
                Next
            Else
                WriteDebugInfoAndWarning("You need to have all Chrome windows closed before launching in this mode. Please close all Chrome windows and try again.")
                Exit Sub
            End If
        End If
        MsgBox(My.Resources.strPleaseBeAwareChrome)
        Process.Start("chrome", "--no-sandbox")
    End Sub

    Private Sub btnClearSACache_Click(sender As Object, e As EventArgs) Handles btnClearSACache.Click
        Dim clearYesNo As MsgBoxResult = MsgBox("This will clear all Symbol Arts from your ""History"" tab. Having 100 pages of Symbol Arts to load can sometimes cause slowdown.", vbYesNo)
        If clearYesNo = vbNo Then
            Exit Sub
        End If
        If clearYesNo = vbYes Then
            WriteDebugInfo("Deleting Symbol Art Cache...")
            Dim documents As String = (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\")
            Dim saCacheFolder As String = (documents & "SEGA\PHANTASYSTARONLINE2\symbolarts\cache")
            For Each foundFile As String In My.Computer.FileSystem.GetFiles(saCacheFolder, FileIO.SearchOption.SearchAllSubDirectories, "*.*")
                My.Computer.FileSystem.DeleteFile(foundFile, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
            Next
            WriteDebugInfoSameLine(My.Resources.strDone)
        End If
    End Sub

    Private Sub btnInstallPSO2_Click(sender As Object, e As EventArgs) Handles btnInstallPSO2.Click
        Dim installFolder As String = ""

        ' TODO: Why is this like this?
        Const installYesNo As MsgBoxResult = vbYes
        If installYesNo = vbNo Then
            WriteDebugInfo("You can view more information about the installer at:" & vbCrLf & "http://arks-layer.com/setup.php")
            Exit Sub
        End If
        If installYesNo = vbYes Then
            MsgBox("This will install Phantasy Star Online EPISODE 2! Please select a folder to install into." & vbCrLf & "A folder called PHANTASYSTARONLINE2 will be created inside the folder you choose." & vbCrLf & "(For example, if you choose the C drive, it will install to C:\PHANTASYSTARONLINE2\)" & vbCrLf & "It is HIGHLY RECOMMENDED that you do NOT install into the Program Files folder, but a normal folder like C:\PHANTASYSTARONLINE\")
SelectInstallFolder:
            Dim myFolderBrowser As New FolderBrowserDialog With {.RootFolder = Environment.SpecialFolder.MyComputer, .Description = "Please select a folder (or drive) to install PSO2 into"}
            Dim dlgResult As DialogResult = myFolderBrowser.ShowDialog()

            If dlgResult = DialogResult.OK Then
                installFolder = myFolderBrowser.SelectedPath
            End If
            If dlgResult = DialogResult.Cancel Then
                WriteDebugInfo("Installation cancelled by user!")
                Exit Sub
            End If
            Dim correctYesNo As MsgBoxResult = MsgBox("You wish to install PSO2 into " & (installFolder & "\PHANTASYSTARONLINE2\. Is this correct?").Replace("\\", "\"), vbYesNoCancel)
            If correctYesNo = vbCancel Then
                WriteDebugInfo("Installation cancelled by user!")
                Exit Sub
            End If
            If correctYesNo = vbNo Then
                GoTo SelectInstallFolder
            End If
            If correctYesNo = vbYes Then
                ' TODO: Change over to the DriveInfo class
                Dim pso2Binfolder As String = installFolder & "\PHANTASYSTARONLINE2\pso2_bin"
                pso2Binfolder = pso2Binfolder.Replace("\\", "\")
                Dim searcher As ManagementObjectSearcher = New ManagementObjectSearcher("SELECT * FROM Win32_LogicalDisk")
                Dim installDrive As String = installFolder.TrimEnd(":"c).Replace("\"c, "")
                For Each MGMT In searcher.Get
                    If Convert.ToString(MGMT("MediaType")) = "12" Then
                        If Convert.ToString(MGMT("DeviceID")) = installDrive Then
                            If Convert.ToInt64(MGMT("Size")) < 26992893636 Then
                                MsgBox("There is not enough space on the selected disk to install PSO2. Please select a different drive. (Requires 16GB of free space)")
                                GoTo SelectInstallFolder
                            End If
                            If Convert.ToInt64(MGMT("FreeSpace")) < 26992893636 Then
                                MsgBox("There is not enough free space on the selected disk to install PSO2. Please free up some space or select a different drive. (Requires 16GB of free space)")
                                GoTo SelectInstallFolder
                            End If
                        End If
                    End If
                Next
                'Dim InstallENPatchesAfter As MsgBoxResult = MsgBox("Would you like the program to automatically install the core EN patch and Large Files EN patch after it's done updating the game?", vbYesNo)
                Dim finalYesNo As MsgBoxResult = MsgBox("The program will now install the necessary files, create the folders, and set up the game. Afterwards, the program will automatically begin patching. Click ""OK"" to start.", MsgBoxStyle.OkCancel)
                If finalYesNo = vbCancel Then
                    WriteDebugInfo("Installation cancelled by user!")
                    Exit Sub
                End If
                If finalYesNo = vbOK Then
                    Office2007StartButton1.Enabled = False
                    'set the pso2Dir to the install patch
                    _pso2RootDir = pso2Binfolder
                    lblDirectory.Text = _pso2RootDir
                    TopMost = True
                    Show()
                    TopMost = False
                    Application.DoEvents()
                    WriteDebugInfo("Downloading DirectX setup...")
                    'Dim client As New MyWebClient With {.timeout = 10000}
                    Try
                        Dlwua("http://arks-layer.com/docs/dxwebsetup.exe", "dxwebsetup.exe")
                        WriteDebugInfoSameLine("Done!")
                        WriteDebugInfo("Checking/Installing DirectX...")
                        'client.DownloadFile("http://arks-layer.com/docs/dxwebsetup.exe", "dxwebsetup.exe")
                        Dim processStartInfo As ProcessStartInfo = New ProcessStartInfo() With {.FileName = "dxwebsetup.exe", .Verb = "runas", .Arguments = "/Q", .UseShellExecute = True}
                        Dim process As Process = process.Start(processStartInfo)
                        Do Until process.WaitForExit(1000)
                        Loop
                        WriteDebugInfoSameLine("Done!")

                    Catch ex As Exception
                        WriteDebugInfo("DirectX installation failed! Please install it later if neccessary!")
                    End Try

                    If File.Exists("dxwebsetup.exe") = True Then File.Delete("dxwebsetup.exe")
                    'Make a data folder, and a win32 folder under that
                    Directory.CreateDirectory(pso2Binfolder & "\data\win32\")
                    'Download required pso2 stuff
                    WriteDebugInfo("Downloading PSO2 required files...")
                    Dlwua("http://download.pso2.jp/patch_prod/patches/pso2launcher.exe.pat", pso2Binfolder & "\pso2launcher.exe")
                    Dlwua("http://download.pso2.jp/patch_prod/patches/pso2updater.exe.pat", pso2Binfolder & "\pso2updater.exe")
                    Dlwua("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", pso2Binfolder & "\pso2.exe")
                    Dlwua("http://download.pso2.jp/patch_prod/patches/PSO2JP.ini.pat", pso2Binfolder & "\PSO2JP.ini")
                    WriteDebugInfoSameLine("Done!")
                    'Download Gameguard.des
                    WriteDebugInfo("Downloading Latest Gameguard file...")
                    Dlwua("http://download.pso2.jp/patch_prod/patches/GameGuard.des.pat", pso2Binfolder & "\GameGuard.des")
                    WriteDebugInfoSameLine(My.Resources.strDone)
                    Application.DoEvents()

                    RegKey.SetValue(Of String)(RegKey.Pso2Dir, _pso2RootDir)
                    WriteDebugInfo(_pso2RootDir & " " & My.Resources.strSetAsYourPSO2)
                    _pso2WinDir = (_pso2RootDir & "\data\win32")
                    If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.StoryPatchVersion)) Then RegKey.SetValue(Of String)(RegKey.StoryPatchVersion, "Not Installed")
                    If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.EnPatchVersion)) Then RegKey.SetValue(Of String)(RegKey.EnPatchVersion, "Not Installed")
                    If String.IsNullOrEmpty(RegKey.GetValue(Of String)(RegKey.LargeFilesVersion)) Then RegKey.SetValue(Of String)(RegKey.LargeFilesVersion, "Not Installed")

                    'Check for PSO2 Updates~
                    ButtonItem5.RaiseClick()

                    MsgBox("PSO2 installed, patched to the latest Japanese version, and ready to play!" & vbCrLf & "Press OK to continue.")
                    Refresh()
                End If
            End If
        End If
    End Sub

    Private Sub btnConfigureItemTranslation_Click(sender As Object, e As EventArgs) Handles btnConfigureItemTranslation.Click
        FrmItemConfig.Show()
    End Sub

    Private Sub btnSymbolEditor_Click(sender As Object, e As EventArgs) Handles btnSymbolEditor.Click
        Process.Start("http://www.pso-world.com/forums/showthread.php?t=215777")
    End Sub

    Private Sub btnRunPSO2Linux_Click(sender As Object, e As EventArgs) Handles btnRunPSO2Linux.Click
        Process.Start("http://www.pso-world.com/forums/showthread.php?t=215642")
    End Sub

    Private Sub LoadSidebar(state As Object)
        Try
            WebBrowser4.Navigate(_freedomUrl & "tweaker.html")
        Catch ex As Exception
            WriteDebugInfo("Web Browser failed: " & ex.Message.ToString)
        End Try
    End Sub

    Private Sub btnDonateToTweaker_Click(sender As Object, e As EventArgs) Handles btnDonateToTweaker.Click
        Process.Start("http://arks-layer.com/donate.php")
    End Sub

    Private Sub btnDonateToBumped_Click(sender As Object, e As EventArgs) Handles btnDonateToBumped.Click
        Process.Start("http://bumped.org/psublog/about/")
    End Sub

    Private Sub btnDonateToCirno_Click(sender As Object, e As EventArgs) Handles btnDonateToCirno.Click
        Process.Start("http://pso2.cirnopedia.info/support.php")
    End Sub

    Private Sub btnDonateToENPatchHost_Click(sender As Object, e As EventArgs) Handles btnDonateToENPatchHost.Click
        Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=UB7UN9MQ7WZ44")
    End Sub

    Private Sub btnResetTweaker_Click(sender As Object, e As EventArgs) Handles btnResetTweaker.Click
        Dim resetyesno As MsgBoxResult = MsgBox("This will erase all of the PSO2 Tweaker's settings, and restart the program. Continue?", vbYesNo)
        If resetyesno = vbYes Then
            My.Computer.Registry.CurrentUser.DeleteSubKeyTree("Software\AIDA", False)
            Log("All settings reset, restarting program!")
            Application.Restart()
        End If
    End Sub

    Private Sub btnPredownloadLobbyVideos_Click(sender As Object, e As EventArgs) Handles btnPredownloadLobbyVideos.Click
        If Not Directory.Exists(_pso2WinDir) OrElse _pso2RootDir = "lblDirectory" Then
            MsgBox(My.Resources.strPleaseSelectwin32Dir)
            SelectPso2Directory()
            Exit Sub
        End If 'Download the missing files:
        _cancelled = False
        Const downloadstring As String = "3fdcad94b7af8c597542cd23e6a87236"
        Dim totaldownloaded As Long = 0
        totaldownloaded += _totalsize2

        lblStatus.Text = My.Resources.strDownloading & " lobby video (" & Helper.SizeSuffix(totaldownloaded) & ")"

        Dlwua(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadstring & ".pat"), downloadstring)
        Dim info7 As New FileInfo(downloadstring)

        If info7.Length = 0 Then
            Log("File appears to be empty, trying to download from secondary SEGA server")
            Dlwua(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring & ".pat"), downloadstring)
        End If
        If _cancelled Then Exit Sub
        File.Move(downloadstring, (_pso2WinDir & "\" & downloadstring))
        WriteDebugInfoAndOk((My.Resources.strDownloadedandInstalled & downloadstring & "."))
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
            Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub btnChooseProxyServer_Click(sender As Object, e As EventArgs) Handles btnChooseProxyServer.Click
        Try
            'JSON should look like { "version": 1, "host": "0.0.0.0", "name": "Super cool proxy", "publickeyurl": "http://url.com" }

            Dim jsonurl As String = InputBox("Please input the URL of the configuration JSON:", "Configuration JSON", "")
            If String.IsNullOrEmpty(jsonurl) Then Exit Sub

            Dim myWebClient As New WebClient()
            WriteDebugInfo("Downloading configuration...")
            myWebClient.DownloadFile(jsonurl, "ServerConfig.txt")

            Dim proxyInfo As Pso2ProxyInfo
            Using stream As FileStream = File.Open("ServerConfig.txt", FileMode.Open)
                Dim serializer As DataContractJsonSerializer = New DataContractJsonSerializer(GetType(Pso2ProxyInfo))
                proxyInfo = DirectCast(serializer.ReadObject(stream), Pso2ProxyInfo)
            End Using

            If Convert.ToInt32(proxyInfo.Version) <> 1 Then
                MsgBox("ERROR - Version is incorrect! Please recheck the JSON.")
                Exit Sub
            End If

            If Not proxyInfo.PublicKeyUrl.Contains("publickey.blob") Then
                MsgBox("ERROR - Public Key URL doesn't point to a public key blob! Please recheck the JSON.")
                Exit Sub
            End If

            For index As Integer = 0 To (proxyInfo.Host.Length - 1)
                If Char.IsLetter(proxyInfo.Host(index)) Then
                    Dim ips = Dns.GetHostAddresses(proxyInfo.Host)
                    proxyInfo.Host = ips(0).ToString()

                    Exit For
                End If
            Next

            WriteDebugInfoSameLine(" Done!")

            Dim builtFile As New List(Of String)
            Dim alreadyModified As Boolean = False

            Using reader As New StreamReader(_hostsFilePath)
                Dim currentLine As String

                Do
                    currentLine = reader.ReadLine()
                    If (currentLine Is Nothing) Then Exit Do

                    Dim splitLine = currentLine.Split(" "c)

                    If splitLine.Length > 1 Then
                        Select Case (splitLine(1))
                            Case "gs001.pso2gs.net"
                                currentLine = proxyInfo.Host & " gs001.pso2gs.net #" & proxyInfo.Name & " Ship 01"
                                alreadyModified = True
                            Case "gs016.pso2gs.net"
                                currentLine = proxyInfo.Host & " gs016.pso2gs.net #" & proxyInfo.Name & " Ship 02"
                                alreadyModified = True
                            Case "gs031.pso2gs.net"
                                currentLine = proxyInfo.Host & " gs031.pso2gs.net #" & proxyInfo.Name & " Ship 03"
                                alreadyModified = True
                            Case "gs046.pso2gs.net"
                                currentLine = proxyInfo.Host & " gs046.pso2gs.net #" & proxyInfo.Name & " Ship 04"
                                alreadyModified = True
                            Case "gs061.pso2gs.net"
                                currentLine = proxyInfo.Host & " gs061.pso2gs.net #" & proxyInfo.Name & " Ship 05"
                                alreadyModified = True
                            Case "gs076.pso2gs.net"
                                currentLine = proxyInfo.Host & " gs076.pso2gs.net #" & proxyInfo.Name & " Ship 06"
                                alreadyModified = True
                            Case "gs091.pso2gs.net"
                                currentLine = proxyInfo.Host & " gs091.pso2gs.net #" & proxyInfo.Name & " Ship 07"
                                alreadyModified = True
                            Case "gs106.pso2gs.net"
                                currentLine = proxyInfo.Host & " gs106.pso2gs.net #" & proxyInfo.Name & " Ship 08"
                                alreadyModified = True
                            Case "gs121.pso2gs.net"
                                currentLine = proxyInfo.Host & " gs121.pso2gs.net #" & proxyInfo.Name & " Ship 09"
                                alreadyModified = True
                            Case "gs136.pso2gs.net"
                                currentLine = proxyInfo.Host & " gs136.pso2gs.net #" & proxyInfo.Name & " Ship 10"
                                alreadyModified = True
                        End Select
                    End If

                    builtFile.Add(currentLine)
                Loop
            End Using

            If alreadyModified Then WriteDebugInfo("Modifying HOSTS file...")

            If Not alreadyModified Then
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
                WriteDebugInfo("Previous modifications not found, creating new entries...")
            End If

            File.WriteAllLines(_hostsFilePath, builtFile.ToArray())
            WriteDebugInfoSameLine(" Done!")

            WriteDebugInfo("Downloading and installing publickey.blob...")
            myWebClient.DownloadFile(proxyInfo.PublicKeyUrl, _startPath & "\publickey.blob")
            If File.Exists(_pso2RootDir & "\publickey.blob") AndAlso _startPath <> _pso2RootDir Then DeleteFile(_pso2RootDir & "\publickey.blob")
            If _startPath <> _pso2RootDir Then File.Move(_startPath & "\publickey.blob", _pso2RootDir & "\publickey.blob")
            WriteDebugInfoSameLine(" Done!")
            WriteDebugInfo("All done! You should now be able to connect to " & proxyInfo.Name & ".")
            RegKey.SetValue(Of Boolean)(RegKey.ProxyEnabled, True)
        Catch ex As Exception
            WriteDebugInfoAndFailed("ERROR - " & ex.Message.ToString)
            If ex.Message.Contains("is denied.") AndAlso ex.Message.Contains("Access to the path") Then MsgBox("It seems you've gotten an error while trying to patch your HOSTS file. Please go to the " & Environment.SystemDirectory & "\drivers\etc\ folder, right click on the hosts file, and make sure ""Read Only"" is not checked. Then try again.")
            Exit Sub
        End Try
    End Sub

    Private Sub btnRevertPSO2ProxyToJP_Click(sender As Object, e As EventArgs) Handles btnRevertPSO2ProxyToJP.Click
        Dim builtFile = New List(Of String)

        Using reader As New StreamReader(_hostsFilePath)
            Dim currentLine As String

            Do
                currentLine = reader.ReadLine()
                If (currentLine Is Nothing) Then Exit Do
                If Not currentLine.Contains("pso2gs.net") Then builtFile.Add(currentLine)
            Loop
        End Using

        WriteDebugInfo("Modifying HOSTS file...")
        File.WriteAllLines(_hostsFilePath, builtFile.ToArray())
        WriteDebugInfoSameLine(" Done!")
        DeleteFile(_pso2RootDir & "\publickey.blob")
        WriteDebugInfoAndOk("All normal JP connection settings restored!")
        RegKey.SetValue(Of Boolean)(RegKey.ProxyEnabled, False)
    End Sub

    Private Sub btnNewShit_Click(sender As Object, e As EventArgs) Handles btnNewShit.Click
        'Still in development because stuff ;w;

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
        If Directory.Exists((_pso2WinDir & "\backup\English Patch\")) Then
            WriteDebugInfo(My.Resources.strENBackupFound)
            _override = True
            btnRestoreENBackup.RaiseClick()
            _override = False
        End If

        If Directory.Exists((_pso2WinDir & "\backup\Large Files\")) Then
            WriteDebugInfo(My.Resources.strLFBackupFound)
            _override = True
            btnRestoreLargeFilesBackup.RaiseClick()
            _override = False
        End If

        Dim count As Integer = 0
        Dim totalfiles = Directory.GetFiles(_pso2RootDir & "\data\win32\")

        If Not File.Exists("old_patchlist.txt") Then
            WriteDebugInfo("Building file list... ")
            Dim di As New DirectoryInfo(_pso2RootDir & "\data\win32\")
            Dim diar1 As FileInfo() = di.GetFiles()

            Using writer As New StreamWriter("old_patchlist.txt", True)
                For Each dra In diar1
                    writer.WriteLine("data/win32/" & dra.Name & ".pat" & vbTab & dra.Length & vbTab & Helper.GetMd5(_pso2RootDir & "\data\win32\" & dra.Name))
                    count += 1
                    lblStatus.Text = "Building first time list of win32 files (" & count & "/" & totalfiles.Length & ")"
                    Application.DoEvents()
                Next
            End Using

            WriteDebugInfoSameLine("Done!")
        End If

        LockGui()
        WriteDebugInfo(My.Resources.strDownloadingPatchFile1)
        Application.DoEvents()
        Dlwua("http://download.pso2.jp/patch_prod/patches/launcherlist.txt", "launcherlist.txt")
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strDownloadingPatchFile2)
        Application.DoEvents()
        Dlwua("http://download.pso2.jp/patch_prod/patches/patchlist.txt", "patchlist.txt")
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strDownloadingPatchFile3)
        Application.DoEvents()
        Dlwua("http://download.pso2.jp/patch_prod/patches_old/patchlist.txt", "patchlist_old.txt")
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strDownloadingPatchFile4)
        Application.DoEvents()
        Dim versionclient As New MyWebClient With {.Timeout = 3000}
        versionclient.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
        WriteDebugInfoSameLine(My.Resources.strDone)
        Application.DoEvents()
        UnlockGui()
        MergePatches()

        'Rewrite this to support the new format

        Dim segaLine As String
        Dim segaFilename As String
        Dim missingfiles As New List(Of String)
        Dim oldarray = File.ReadAllLines("old_patchlist.txt")

        For i As Integer = 0 To _someofthethings.Count
            segaLine = _someofthethings.Values(i)
            If String.IsNullOrEmpty(segaLine) Then Continue For

            segaFilename = segaLine.Remove(segaLine.IndexOf(".pat")).Replace("data/win32/", "")
            lblStatus.Text = "Checking file " & i & " / " & totalfiles.Length
            If missingfiles.Count > 0 Then lblStatus.Text &= " (missing files found: " & missingfiles.Count & ")"
            Application.DoEvents()
            If Not oldarray.Contains(segaLine) Then missingfiles.Add(segaFilename)
        Next
    End Sub

    Private Sub btnStoryPatchNew_Click(sender As Object, e As EventArgs) Handles btnStoryPatchNew.Click
        'Don't forget to make GUI changes!
        Try
            If Not Directory.Exists(_pso2WinDir) OrElse _pso2RootDir = "lblDirectory" Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                SelectPso2Directory()
                Exit Sub
            End If

            Dim win32 As String = _pso2WinDir
            Dim strStoryPatchLatestBase As String
            Dim backupdir As String = (_pso2WinDir & "\" & "backup\Story Patch\")
            Dim net As New WebClient()
            Dim src As String = net.DownloadString("http://arks-layer.com/story.php")

            ' Create a match using regular exp<b></b>ressions
            Dim m As Match = Regex.Match(src, "<u>.*?</u>")

            ' Spit out the value plucked from the code
            txtHTML.Text = m.Value
            Dim strDownloadMe As String = txtHTML.Text.Replace("<u>", "").Replace("</u>", "")
            strStoryPatchLatestBase = strDownloadMe
            strStoryPatchLatestBase = strStoryPatchLatestBase.Replace("/", "-")
            WriteDebugInfoAndOk("Downloading story patch info... ")
            Dlwua(_freedomUrl & "pso2.stripped.db", "pso2.stripped.db")
            WriteDebugInfoAndOk("Downloading Trans-Am tool... ")
            Dlwua(_freedomUrl & "pso2-transam.exe", "pso2-transam.exe")

            'execute pso2-transam stuff with -b flag for backup
            Dim process As Process
            Dim processStartInfo As ProcessStartInfo = New ProcessStartInfo() With {.FileName = "pso2-transam.exe", .Verb = "runas"}
            If Directory.Exists(backupdir) Then
                Dim counter = My.Computer.FileSystem.GetFiles(backupdir)
                If Convert.ToInt64(counter.Count) > 0 Then
                    processStartInfo.Arguments = ("-t story-eng-" & strStoryPatchLatestBase & " pso2.stripped.db " & """" & win32 & """")
                Else
                    Log("[TRANSAM] Creating backup directory")
                    Directory.CreateDirectory(backupdir)
                    WriteDebugInfo(My.Resources.strCreatingBackupDirectory)
                    processStartInfo.Arguments = ("-b " & """" & backupdir & """" & " -t story-eng-" & strStoryPatchLatestBase & " pso2.stripped.db " & """" & win32 & """")
                End If
            End If
            If Not Directory.Exists(backupdir) Then
                Log("[TRANSAM] Creating backup directory")
                Directory.CreateDirectory(backupdir)
                WriteDebugInfo(My.Resources.strCreatingBackupDirectory)
                processStartInfo.Arguments = ("-b " & """" & backupdir & """" & " -t story-eng-" & strStoryPatchLatestBase & " pso2.stripped.db " & """" & win32 & """")
            End If

            processStartInfo.UseShellExecute = False
            Log("[TRANSAM] Starting shitstorm")
            processStartInfo.Arguments = processStartInfo.Arguments.Replace("\", "/")
            Log("TRANSM parameters: " & processStartInfo.Arguments & vbCrLf & "TRANSAM Working Directory: " & processStartInfo.WorkingDirectory)
            'MsgBox("ALL INFO: " & processStartInfo.ToString)
            process = process.Start(processStartInfo)
            Log("[TRANSAM] Program started")

            Do Until process.WaitForExit(1000)
            Loop

            DeleteFile("pso2.stripped.db")
            DeleteFile("pso2-transam.exe")

            External.FlashWindow(Handle, True)
            'Story Patch 3-12-2014.rar
            RegKey.SetValue(Of String)(RegKey.StoryPatchVersion, strStoryPatchLatestBase.Replace("-", "/"))
            RegKey.SetValue(Of String)(RegKey.LatestStoryBase, strStoryPatchLatestBase.Replace("-", "/"))
            WriteDebugInfo(My.Resources.strStoryPatchInstalled)
            CheckForStoryUpdates()
        Catch ex As Exception
            MsgBox("ERROR - " & ex.Message.ToString)
            Exit Sub
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
            If Not Directory.Exists(_pso2WinDir) OrElse _pso2RootDir = "lblDirectory" Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                SelectPso2Directory()
                Exit Sub
            End If

            WriteDebugInfo(My.Resources.strDownloading & patchname & "...")
            Application.DoEvents()
            Dim strDownloadMe As String = url & filename
            _cancelled = False
            Dlwua(strDownloadMe, filename)
            If _cancelled Then Exit Sub
            WriteDebugInfo((My.Resources.strDownloadCompleteDownloaded & strDownloadMe & ")"))

            If File.Exists((_pso2WinDir & "\" & filename)) Then
                If File.Exists((_pso2WinDir & "\" & filename & ".backup")) Then
                    My.Computer.FileSystem.DeleteFile((_pso2WinDir & "\" & filename & ".backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                End If

                My.Computer.FileSystem.RenameFile((_pso2WinDir & "\" & filename), filename & ".backup")
            End If

            Application.DoEvents()
            File.Move(filename, (_pso2WinDir & "\" & filename))
            External.FlashWindow(Handle, True)
            WriteDebugInfo(patchname & " " & My.Resources.strInstalledUpdated)
            UnlockGui()
        Catch ex As Exception
            Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub DownloadPatch(patchUrl As String, patchName As String, patchFile As String, versionStr As String, msgBackup As String, msgSelectArchive As String, backupDir As String)
        _cancelledFull = False
        Try
            If Not Directory.Exists(_pso2WinDir) OrElse _pso2RootDir = "lblDirectory" Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                SelectPso2Directory()
                Exit Sub
            End If

            Dim backupyesno As MsgBoxResult
            Dim predownloadedyesno As MsgBoxResult
            Dim rarLocation As String = ""
            Dim strVersion As String = ""

            ' Check the patch download method preference
            Dim patchPreference As String = RegKey.GetValue(Of String)(RegKey.PreDownloadedRar)
            Select Case patchPreference
                Case "Ask"
                    predownloadedyesno = MsgBox(My.Resources.strWouldYouLikeToUse, vbYesNo)
                Case "Always"
                    predownloadedyesno = MsgBoxResult.Yes
                Case "Never"
                    predownloadedyesno = MsgBoxResult.No
                Case Else
                    predownloadedyesno = MsgBox(My.Resources.strWouldYouLikeToUse, vbYesNo)
            End Select

            ' Check the backup preference
            patchPreference = RegKey.GetValue(Of String)(RegKey.Backup)
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
                WriteDebugInfo(My.Resources.strDownloading & patchName & "...")
                Application.DoEvents()

                ' Might want to switch to a Uri class.
                ' Get the filename from the downloaded Path
                Dim lastfilename As String() = patchUrl.Split("/"c)
                strVersion = lastfilename(lastfilename.Length - 1)
                strVersion = Path.GetFileNameWithoutExtension(strVersion) ' We're using this so that it's not format-specific.

                _cancelled = False

                If Not Helper.CheckLink(patchUrl) Then
                    WriteDebugInfoAndFailed("Failed to contact " & patchName & " website - Patch install/update canceled!")
                    WriteDebugInfo("Please visit http://goo.gl/YzCE7 for more information!")
                    Exit Sub
                End If

                Dlwua(patchUrl, patchFile)
                If _cancelled Then Exit Sub
                WriteDebugInfo((My.Resources.strDownloadCompleteDownloaded & patchUrl & ")"))
            ElseIf predownloadedyesno = MsgBoxResult.Yes Then
                OpenFileDialog1.Title = msgSelectArchive
                OpenFileDialog1.FileName = "PSO2 " & patchName & " RAR file"
                OpenFileDialog1.Filter = "RAR Archives|*.rar|All Files (*.*) |*.*"
                If OpenFileDialog1.ShowDialog() = DialogResult.Cancel Then Exit Sub

                rarLocation = OpenFileDialog1.FileName
                strVersion = OpenFileDialog1.SafeFileName
                strVersion = Path.GetFileNameWithoutExtension(strVersion)
            End If

            Application.DoEvents()

            If Directory.Exists("TEMPPATCHAIDAFOOL") Then
                My.Computer.FileSystem.DeleteDirectory("TEMPPATCHAIDAFOOL", FileIO.DeleteDirectoryOption.DeleteAllContents)
                Directory.CreateDirectory("TEMPPATCHAIDAFOOL")
            End If

            If Not Directory.Exists("TEMPPATCHAIDAFOOL") Then
                Directory.CreateDirectory("TEMPPATCHAIDAFOOL")
            End If

            Dim process As Process
            Dim processStartInfo As ProcessStartInfo = New ProcessStartInfo()
            Dim unRarLocation As String = (_startPath & "\unrar.exe")
            processStartInfo.FileName = unRarLocation
            processStartInfo.Verb = "runas"
            If predownloadedyesno = MsgBoxResult.No Then processStartInfo.Arguments = ("e " & patchFile & " TEMPPATCHAIDAFOOL")
            If predownloadedyesno = MsgBoxResult.Yes Then processStartInfo.Arguments = ("e " & """" & rarLocation & """" & " TEMPPATCHAIDAFOOL")
            processStartInfo.WindowStyle = ProcessWindowStyle.Normal
            processStartInfo.UseShellExecute = True
            process = process.Start(processStartInfo)
            WriteDebugInfo(My.Resources.strWaitingforPatch)
            Do Until process.WaitForExit(1000)
            Loop
            If Not Directory.Exists("TEMPPATCHAIDAFOOL") Then
                Directory.CreateDirectory("TEMPPATCHAIDAFOOL")
                WriteDebugInfo("Had to manually make temp update folder - Did the patch not extract right?")
            End If
            Dim di As New DirectoryInfo("TEMPPATCHAIDAFOOL")
            Dim diar1 As FileInfo() = di.GetFiles()
            Dim dra As FileInfo
            WriteDebugInfoAndOk((My.Resources.strExtractingTo & _pso2WinDir))
            Application.DoEvents()
            If _cancelledFull Then Exit Sub

            Dim backupstr As String = (_pso2WinDir & "\" & backupDir)
            If backupyesno = MsgBoxResult.Yes Then
                If Directory.Exists(backupstr) Then
                    My.Computer.FileSystem.DeleteDirectory(backupstr, FileIO.DeleteDirectoryOption.DeleteAllContents)
                    Directory.CreateDirectory(backupstr)
                    WriteDebugInfo(My.Resources.strErasingPreviousBackup)
                End If
                If Not Directory.Exists(backupstr) Then
                    Directory.CreateDirectory(backupstr)
                    WriteDebugInfo(My.Resources.strCreatingBackupDirectory)
                End If
            End If

            Log("Extracted " & diar1.Length & " files from the patch")
            If diar1.Length = 0 Then
                WriteDebugInfo("Patch failed to extract correctly! Installation failed!")
                Exit Sub
            End If

            WriteDebugInfo(My.Resources.strInstallingPatch)

            For Each dra In diar1
                If _cancelledFull Then Exit Sub

                If backupyesno = MsgBoxResult.Yes Then
                    If File.Exists((_pso2WinDir & "\" & dra.ToString())) Then
                        File.Move((_pso2WinDir & "\" & dra.ToString()), (backupstr & "\" & dra.ToString()))
                    End If
                End If
                If backupyesno = MsgBoxResult.No Then
                    If File.Exists((_pso2WinDir & "\" & dra.ToString())) Then
                        DeleteFile((_pso2WinDir & "\" & dra.ToString()))
                    End If
                End If
                File.Move(("TEMPPATCHAIDAFOOL\" & dra.ToString()), (_pso2WinDir & "\" & dra.ToString()))
            Next

            My.Computer.FileSystem.DeleteDirectory("TEMPPATCHAIDAFOOL", FileIO.DeleteDirectoryOption.DeleteAllContents)
            If backupyesno = MsgBoxResult.No Then
                External.FlashWindow(Handle, True)
                WriteDebugInfo("English patch " & My.Resources.strInstalledUpdated)
                If Not String.IsNullOrEmpty(versionStr) Then RegKey.SetValue(Of String)(versionStr, strVersion)
            End If
            If backupyesno = MsgBoxResult.Yes Then
                External.FlashWindow(Handle, True)
                WriteDebugInfo(("English patch " & My.Resources.strInstalledUpdatedBackup & backupstr))
                If Not String.IsNullOrEmpty(versionStr) Then RegKey.SetValue(Of String)(versionStr, strVersion)
            End If
            DeleteFile(patchName)
            UnlockGui()
        Catch ex As Exception
            Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub UninstallPatch(patchListUrl As String, patchListFile As String, backupDir As String, consoleMsg As String, patchVersionKey As String)
        Try
            If Not Directory.Exists(_pso2WinDir) OrElse _pso2RootDir = "lblDirectory" Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                SelectPso2Directory()
                Exit Sub
            End If

            Dlwua(patchListUrl, patchListFile)

            Dim missingfiles = File.ReadAllLines(patchListFile)

            DeleteFile(patchListFile)

            WriteDebugInfo(My.Resources.strUninstallingPatch)

            For index As Integer = 0 To (missingfiles.Length - 1)
                If _cancelledFull Then Exit Sub

                'Download JP file
                lblStatus.Text = My.Resources.strUninstalling & index & "/" & missingfiles.Length
                Dlwua(("http://download.pso2.jp/patch_prod/patches/data/win32/" & missingfiles(index) & ".pat"), missingfiles(index))
                Dim info7 As New FileInfo(missingfiles(index))
                If info7.Length = 0 Then Dlwua(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & missingfiles(index) & ".pat"), missingfiles(index))

                'Move JP file to win32
                DeleteFile((_pso2WinDir & "\" & missingfiles(index)))
                File.Move(missingfiles(index), (_pso2WinDir & "\" & missingfiles(index)))
            Next

            If My.Computer.FileSystem.DirectoryExists(_pso2WinDir & "\" & backupDir) Then
                My.Computer.FileSystem.DeleteDirectory((_pso2WinDir & "\" & backupDir), FileIO.DeleteDirectoryOption.DeleteAllContents)
            End If

            External.FlashWindow(Handle, True)
            WriteDebugInfo(consoleMsg)
            RegKey.SetValue(Of String)(patchVersionKey, "Not Installed")
            UnlockGui()
        Catch ex As Exception
            Log(ex.Message.ToString & " Stack Trace: " & ex.StackTrace)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub
End Class