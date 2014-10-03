Imports Microsoft.Win32
Imports Newtonsoft.Json.Linq
Imports System.IO
Imports System.Management
Imports System.Net
Imports System.Net.Sockets
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Security.AccessControl
Imports System.Security.Principal
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading

' TODO: Should move the new thread calls to use the thread pool instead
' TODO: Replace all redundant code with functions
' TODO: Replace all string literals for registry keys with constant strings to avoid errors in the future
' TODO: Organize this form by order of member type (variable, function, etc)
' TODO: There are a lot of [.Replace("\data\win32", "")], assumeing this is always the PSO2 path we should just do it once and save it

Public Class frmMain
    Const MEM_COMMIT = 4096
    Const PAGE_READWRITE = 4
    Const PROCESS_CREATE_THREAD = &H2
    Const PROCESS_VM_OPERATION = &H8
    Const PROCESS_VM_WRITE = &H20

    Shared FolderDownloads As New Guid("374DE290-123F-4565-9164-39C4925E467B")

    Dim DPISetting As Integer
    Dim ComingFromOldFiles As Boolean = False
    Dim testfile As String = "http://arks-layer.com/Disko Warp x Pump It Up Pro 2 Official Soundtrack Sampler.mp3"
    Dim testfile_Size As Double = 1.91992 'MB
    Dim timer_start As Integer
    Dim time_for_download As Integer
    Dim velocity As Double
    Dim SystemUnlock As Integer
    Dim MileyCyrus As Integer
    Dim SteamUnlock As Integer
    Dim DLS As New MyWebClient
    Dim Cancelled As Boolean
    Dim CancelledFull As Boolean
    Dim UseItemTranslation As Boolean = False
    Dim VedaUnlocked As Boolean = False
    Dim CommandLineArgs As String() = Environment.GetCommandLineArgs()
    Dim Override As Boolean = False
    Dim TransOverride As Boolean = False
    Dim DoneDownloading As Boolean = False
    Dim patching As Boolean = False
    Dim totalsize2 As Integer
    Dim Restartplz As Boolean
    Dim ItemDownloadingDone As Boolean
    Dim nodiag As Boolean = False
    Dim ComingFromPrePatch As Boolean = False
    Dim TargetProcessHandle As Integer
    Dim pfnStartAddr As Integer
    Dim pszLibFileRemote As String
    Dim TargetBufferSize As Integer
    Dim SOMEOFTHETHINGS As Dictionary(Of String, String)
    Dim processes As Process()

#Region "External Functions"

    Private Declare Auto Function ShellExecute Lib "shell32" (ByVal hwnd As IntPtr, ByVal lpOperation As String, ByVal lpFile As String, ByVal lpParameters As String, ByVal lpDirectory As String, ByVal nShowCmd As UInteger) As IntPtr

    Private Declare Function FindWindow Lib "user32" Alias "FindWindowA" (ByVal lpClassName As String, ByVal lpWindowName As String) As Integer

    Private Declare Function FlashWindow Lib "user32" (ByVal hwnd As Integer, ByVal bInvert As Integer) As Integer

    Private Declare Function SHGetKnownFolderPath Lib "shell32" (ByRef id As Guid, flags As Integer, token As IntPtr, ByRef path As IntPtr) As Integer

#End Region

    Private Sub frmMain_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        Application.Exit()
    End Sub

    Private Sub frmMain_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.Shift Then
            If e.KeyCode = Keys.G Then
                SystemUnlock = 1
                lblStatus.Text = "Please enter the remaining commands to access Veda: *"
            End If
            If e.KeyCode = Keys.U AndAlso SystemUnlock = 1 Then
                SystemUnlock = 2
                lblStatus.Text = "Please enter the remaining commands to access Veda: **"
            End If
            If e.KeyCode = Keys.N AndAlso SystemUnlock = 2 Then
                SystemUnlock = 3
                lblStatus.Text = "Please enter the remaining commands to access Veda: ***"
            End If
            If e.KeyCode = Keys.D AndAlso SystemUnlock = 3 Then
                SystemUnlock = 4
                lblStatus.Text = "Please enter the remaining commands to access Veda: ****"
            End If
            If e.KeyCode = Keys.A AndAlso SystemUnlock = 4 Then
                SystemUnlock = 5
                lblStatus.Text = "Please enter the remaining commands to access Veda: *****"
            End If
            If e.KeyCode = Keys.M AndAlso SystemUnlock = 5 Then
                SystemUnlock = 6
                lblStatus.Text = "Please enter the remaining commands to access Veda: ******"
                Application.DoEvents()
                Thread.Sleep(2000)
                lblStatus.Text = "[ACCESS GRANTED - SYSTEM UNLOCKED]"
                Application.DoEvents()
                Thread.Sleep(2000)
                VedaUnlocked = True
                frmVEDA.Show()
            End If
            If e.KeyCode = Keys.M Then
                MileyCyrus = 1
                lblStatus.Text = "Please enter the remaining commands: *"
            End If
            If e.KeyCode = Keys.I AndAlso MileyCyrus = 1 Then
                MileyCyrus = 2
                lblStatus.Text = "Please enter the remaining commands: **"
            End If
            If e.KeyCode = Keys.L AndAlso MileyCyrus = 2 Then
                MileyCyrus = 3
                lblStatus.Text = "Please enter the remaining commands: ***"
            End If
            If e.KeyCode = Keys.E AndAlso MileyCyrus = 3 Then
                MileyCyrus = 4
                lblStatus.Text = "Please enter the remaining commands: ****"
            End If
            If e.KeyCode = Keys.Y AndAlso MileyCyrus = 4 Then
                MileyCyrus = 5
                lblStatus.Text = "Please enter the remaining commands: *****"
                Application.DoEvents()
                Thread.Sleep(2000)
                lblStatus.Text = "[ACCESS GRANTED - PSO2 TWERKER]"
                Application.DoEvents()
                Thread.Sleep(2000)
                Me.Text = ("PSO2 Twerker ver " & My.Application.Info.Version.ToString)
                btnLaunchPSO2.Text = "Twerk it!"
                chkItemTranslation.Text = "Twerk on Robin Thicke"
            End If
            If e.KeyCode = Keys.K Then
                SteamUnlock = 1
                lblStatus.Text = "Please enter the remaining commands to unlock BETA Steam support: *"
            End If
            If e.KeyCode = Keys.U AndAlso SteamUnlock = 1 Then
                SteamUnlock = 2
                lblStatus.Text = "Please enter the remaining commands to unlock BETA Steam support: **"
            End If
            If e.KeyCode = Keys.M AndAlso SteamUnlock = 2 Then
                SteamUnlock = 3
                lblStatus.Text = "Please enter the remaining commands to unlock BETA Steam support: ***"
            End If
            If e.KeyCode = Keys.I AndAlso SteamUnlock = 3 Then
                SteamUnlock = 4
                lblStatus.Text = "Please enter the remaining commands to unlock BETA Steam support: ****"
            End If
            If e.KeyCode = Keys.H AndAlso SteamUnlock = 4 Then
                SteamUnlock = 5
                lblStatus.Text = "Please enter the remaining commands to unlock BETA Steam support: *****"
            End If
            If e.KeyCode = Keys.O AndAlso SteamUnlock = 5 Then
                SteamUnlock = 6
                lblStatus.Text = "Please enter the remaining commands to unlock BETA Steam support: ******"
                Application.DoEvents()
                Thread.Sleep(2000)
                lblStatus.Text = "[ACCESS GRANTED - SYSTEM UNLOCKED]"
                Application.DoEvents()
                Thread.Sleep(2000)
                frmOptions.ButtonX4.Visible = True
                frmOptions.ButtonX5.Visible = True
                frmOptions.LabelX13.Visible = True
                frmOptions.TextBoxX1.Visible = True
                frmOptions.ButtonX3.Visible = True

                frmVEDA.Show()
            End If
        End If
    End Sub

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        tmrCheckServerStatus.Start()
        ' TODO: Fix this Mike plz
        Using g As Graphics = Me.CreateGraphics
            If g.DpiX = 120 Then
                DPISetting = 120
            End If
            If g.DpiX = 96 Then
                DPISetting = 96
            End If
        End Using

        Try
            btnAnnouncements.Text = ">"
            Dim procs As Process()
            Log("Program started! - Logging enabled!")
            Dim DirectoryString As String
            Dim pso2launchpath As String
            Dim sBuffer As String

            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("PSO2Dir")) Then
                Dim AlreadyInstalled As MsgBoxResult = MsgBox("This appears to be the first time you've used the PSO2 Tweaker! Have you installed PSO2 already? If you select no, the PSO2 Tweaker will install it for you.", MsgBoxStyle.YesNo)
                If AlreadyInstalled = vbNo Then
                    btnInstallPSO2.RaiseClick()
                End If
            End If

            Log("Attempting to auto-load pso2_bin directory from settings")
            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("PSO2Dir")) Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                Button1.RaiseClick()
            Else
                lblDirectory.Text = Helper.GetRegKey(Of String)("PSO2Dir")
                Log("Loaded pso2_bin directory from settings")
            End If

            If lblDirectory.Text.Contains("\pso2_bin\data\win32") Then
                If File.Exists(lblDirectory.Text.Replace("\data\win32", "") & "\pso2.exe") Then
                    Log("win32 folder selected instead of pso2_bin folder - Fixing!")
                    lblDirectory.Text = lblDirectory.Text.Replace("\data\win32", "")
                    Helper.SetRegKey(Of String)("PSO2Dir", lblDirectory.Text)
                    Log(lblDirectory.Text & " " & My.Resources.strSetAsYourPSO2)
                End If
            End If

            If (Directory.Exists(lblDirectory.Text) = False OrElse lblDirectory.Text = "lblDirectory") Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                Button1.RaiseClick()
            End If

            DirectoryString = lblDirectory.Text
            pso2launchpath = DirectoryString.Replace("\data\win32", "")
            DeleteFile(pso2launchpath & "ddraw.dll")

            For i As Integer = 1 To CommandLineArgs.Length - 1
                If CommandLineArgs(i) = "-fuck_you_misaki_stop_trying_to_decompile_my_shit" Then
                    Log("Fuck you, Misaki")
                    MsgBox("Why are you trying to decompile my program? Get outta here!")
                End If

                If CommandLineArgs(i) = "-nodllcheck" Then
                    TransOverride = True
                End If

                If CommandLineArgs(i) = "-steam" Then
                    Log("Detected -steam argument")
                    If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("SteamUID")) Then
                        MsgBox("You need to open the PSO2 Normally and configure the Steam launch URL in the options.")
                    End If

                    Environment.SetEnvironmentVariable("-pso2", "+0x01e3f1e9")
                    ShellExecute(Handle, "open", (pso2launchpath & "\pso2.exe"), "+0x33aca2b9 -pso2", "", 0)

                    Log("Deleting item cache")
                    DeleteFile(Dir() & "\SEGA\PHANTASYSTARONLINE2\item_name_cache.dat")
                    Log("Launching PSO2 with -steam")

                    Me.Close()
                End If

                If CommandLineArgs(i) = "-item" Then
                    Log("Detected command argument -item")
                    UseItemTranslation = True
                End If

                If CommandLineArgs(i) = "-nodiag" Then
                    Log("Detected command argument -nodiag")
                    Log("Bypassing OS detection to fix compatibility!")
                    nodiag = True
                End If

                If CommandLineArgs(i) = "-bypass" Then
                    Log("Detected command argument -bypass")
                    Log("Emergency bypass mode activated - Please only use this mode if the Tweaker will not start normally!")
                    MsgBox("Emergency bypass mode activated - Please only use this mode if the Tweaker will not start normally!")
                    If (Directory.Exists(lblDirectory.Text) = False OrElse lblDirectory.Text = "lblDirectory") Then
                        MsgBox(My.Resources.strPleaseSelectwin32Dir)
                        Button1.RaiseClick()
                        Exit Sub
                    End If
                    DirectoryString = (lblDirectory.Text & "\data\win32")
                    pso2launchpath = DirectoryString.Replace("\data\win32", "")
                    File.WriteAllBytes(pso2launchpath & "\ddraw.dll", My.Resources.ddraw)
                    Log("Setting environment variable")
                    Environment.SetEnvironmentVariable("-pso2", "+0x01e3f1e9")
                    Log("Launching PSO2")
                    ShellExecute(Handle, "open", (pso2launchpath & "\pso2.exe"), "+0x33aca2b9 -pso2", "", 0)
                    Me.Hide()
                    Do Until File.Exists(pso2launchpath & "\ddraw.dll") = False
                        procs = Process.GetProcessesByName("pso2")
                        For Each proc As Process In procs
                            If proc.MainWindowTitle = "Phantasy Star Online 2" AndAlso proc.MainModule.ToString = "ProcessModule (pso2.exe)" Then
                                If Not TransOverride Then DeleteFile(pso2launchpath & "\ddraw.dll")
                            End If
                        Next
                        Thread.Sleep(1000)
                    Loop
                    Me.Close()
                End If

                If CommandLineArgs(i) = "-pso2" Then
                    Log("Detected command argument -pso2")

                    'Fuck SEGA. Fuck them hard.
                    If (Directory.Exists(lblDirectory.Text) = False OrElse lblDirectory.Text = "lblDirectory") Then
                        MsgBox(My.Resources.strPleaseSelectwin32Dir)
                        Button1.RaiseClick()
                        Exit Sub
                    End If

                    DirectoryString = (lblDirectory.Text & "\data\win32")
                    pso2launchpath = DirectoryString.Replace("\data\win32", "")

                    If UseItemTranslation Then
                        Dim dir As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                        Log("Deleting item cache...")
                        DeleteFile(dir & "\SEGA\PHANTASYSTARONLINE2\item_name_cache.dat")
                        Log("Checking to see if the item translation works with PSO2...")
                        DLWUA("http://162.243.211.123/freedom/working.txt", "working.txt", True)

                        Using oReader As StreamReader = File.OpenText("working.txt")
                            sBuffer = oReader.ReadLine
                            If sBuffer.ToString = "No" Then
                                MsgBox("Sorry, but the item translation is not working with the current version of PSO2! Please remove the -item argument if you wish to launch PSO2 without it.")
                                Exit Sub
                            End If
                        End Using

                        DeleteFile("working.txt")
                        DirectoryString = (lblDirectory.Text & "\data\win32")
                        pso2launchpath = DirectoryString.Replace("\data\win32", "")

                        'Download the latest translator.dll and translation.bin
                        Dim DLLink1 As String = "http://162.243.211.123/freedom/translator.dll"
                        Dim DLLink2 As String = "http://162.243.211.123/freedom/translation.bin"
                        Log(My.Resources.strDownloadingItemTranslationFiles)
                        Dim client As New WebClient

                        ' Try up to 4 times to download the translator DLL.
                        For tries As Integer = 1 To 4
                            Try
                                client.DownloadFile(DLLink1, (pso2launchpath & "\translator.dll"))
                                Exit For
                            Catch ex As Exception
                                If tries = 4 Then
                                    Log("Failed to download translation files! (" & ex.Message & ")")
                                    Exit For
                                End If
                            End Try
                        Next

                        ' Try up to 4 times to download the translation strings.
                        For tries As Integer = 1 To 4
                            Try
                                client.DownloadFile(DLLink2, (pso2launchpath & "\translation.bin"))
                                Exit For
                            Catch ex As Exception
                                If tries = 4 Then
                                    Log("Failed to download translation files! (" & ex.Message & ")")
                                    Exit Try
                                End If
                            End Try
                        Next

                        File.WriteAllBytes(pso2launchpath & "\ddraw.dll", My.Resources.ddraw)
                    End If

                    Log("Setting environment variable")
                    Environment.SetEnvironmentVariable("-pso2", "+0x01e3f1e9")

                    Log("Launching PSO2")
                    ShellExecute(Handle, "open", (pso2launchpath & "\pso2.exe"), "+0x33aca2b9 -pso2", "", 0)

                    DeleteFile("LanguagePack.rar")
                    If UseItemTranslation Then
                        Me.Hide()
                        Do Until File.Exists(pso2launchpath & "\ddraw.dll") = False
                            procs = Process.GetProcessesByName("pso2")
                            For Each proc As Process In procs
                                If proc.MainWindowTitle = "Phantasy Star Online 2" AndAlso proc.MainModule.ToString = "ProcessModule (pso2.exe)" Then
                                    If Not TransOverride Then DeleteFile(pso2launchpath & "\ddraw.dll")
                                End If
                            Next
                            Thread.Sleep(1000)
                        Loop
                    End If

                    Me.Close()
                End If
            Next

            'Normal Tweaker startup
            CancelledFull = False

            '[AIDA]For testing only, bitch
            'Button6.PerformClick()
            'Exit Sub
            'frmPSO2Settings.Show()
            'Exit Sub
            'Helper.SetRegKey(Of String)("Locale", "")
            'If Helper.GetRegKey(Of String)("Locale") = "" Then
            'MsgBox("This appears to be the first time you've used the new version of the Arks System - Please select your language and theme in the next window.")
            'btnOptions.RaiseClick()
            'Do While frmOptions.Visible = True
            '
            'THIS SHIT WILL BREAK EVERYTHING AND EAT KITTENS
            'OR MAYBE EAT EVERTHING AND BREAK KITTENS FUCK IF I REMEMBER
            'Loop
            'End If
            'Dim result As Long
            'result = SetWindowLong(rtbDebug.Handle, GWL_EXSTYLE, _
            'WS_EX_TRANSPARENT)
            'GoTo DEBUGMYSHITDAWG

            DirectoryString = (lblDirectory.Text & "\data\win32")
            pso2launchpath = DirectoryString.Replace("\data\win32", "")
            If File.Exists(pso2launchpath & "\ddraw.dll") AndAlso (Not TransOverride) Then DeleteFile(pso2launchpath & "\ddraw.dll")
            Log("Loading settings...")
            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("PatchServer")) Then Helper.SetRegKey(Of String)("PatchServer", "Patch Server #1")
            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("SeenFuckSEGAMessage")) Then Helper.SetRegKey(Of String)("SeenFuckSEGAMessage", "False")
            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("Backup")) Then Helper.SetRegKey(Of String)("Backup", "Always")
            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("PreDownloadedRAR")) Then Helper.SetRegKey(Of String)("PreDownloadedRAR", "Never")
            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("Pastebin")) Then Helper.SetRegKey(Of String)("Pastebin", "True")
            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("CloseAfterLaunch")) Then Helper.SetRegKey(Of String)("CloseAfterLaunch", "False")
            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("ENPatchAfterInstall")) Then Helper.SetRegKey(Of String)("ENPatchAfterInstall", "False")
            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("LargeFilesAfterInstall")) Then Helper.SetRegKey(Of String)("LargeFilesAfterInstall", "False")
            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("StoryPatchAfterInstall")) Then Helper.SetRegKey(Of String)("StoryPatchAfterInstall", "False")
            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("LatestStoryBase")) Then Helper.SetRegKey(Of String)("LatestStoryBase", "Unknown")
            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("ProxyEnabled")) Then Helper.SetRegKey(Of String)("ProxyEnabled", "False")
            If Helper.GetRegKey(Of String)("SidebarEnabled") = "False" Then
                btnAnnouncements.PerformClick()
            End If
            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("UID")) Then Helper.SetRegKey(Of String)("UID", "False")
            If Helper.GetRegKey(Of String)("UID") = "False" Then
                Dim client As New WebClient()
                Dim UIDSTRING As String = client.DownloadString("http://arks-layer.com/docs/client.php")
                Helper.SetRegKey(Of String)("UID", UIDSTRING)
            End If

            Dim locale = Helper.GetRegKey(Of String)("Locale")

            If Not String.IsNullOrEmpty(Helper.GetRegKey(Of String)("Locale")) Then
                Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo(locale)
                Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo(locale)
            End If

            Me.BackgroundImage = Nothing
            frmOptions.BackgroundImage = Nothing

            Log("Initialize stuff for locale...")

            'Yo, fuck this shit. Shit is mad whack, yo.
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
            Label1.Text = My.Resources.strCurrentlyselecteddirectory
            lblStatus.Text = My.Resources.strWaitingforacommand
            Button1.Text = My.Resources.strSelectPSO2win32folder
            ButtonInstall.Text = My.Resources.strInstallUpdatePatches
            btnRestoreBackups.Text = "Restore Backup of JP Files"
            btnApplyChanges.Text = My.Resources.strApplySelectedChanges
            CancelDownloadToolStripMenuItem.Text = My.Resources.strCancelCurrentDownload
            ButtonItem5.Text = My.Resources.strCheckforPSO2Updates
            btnLaunchPSO2.Text = My.Resources.strLaunchPSO2
            Button6.Text = My.Resources.strFixPSO2EXEs
            btnFixPermissions.Text = My.Resources.strFixPSO2Permissions
            LabelItem1.Text = My.Resources.strClickOrb
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
            btnConnection.Text = My.Resources.strDiagnoseConnectionIssues
            btnGameguard.Text = My.Resources.strFixGameguardErrors
            ButtonItem17.Text = My.Resources.strResetPSO2Settings
            btnResumePatching.Text = My.Resources.strResumePatching
            ButtonItem12.Text = My.Resources.strStoryPatchServerTests
            btnTerminate.Text = My.Resources.strTerminate
            ButtonItem7.Text = My.Resources.strLaunchChrome

            Log("Load more settings...")
            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("StoryPatchVersion")) Then Helper.SetRegKey(Of String)("StoryPatchVersion", "Not Installed")
            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("ENPatchVersion")) Then Helper.SetRegKey(Of String)("ENPatchVersion", "Not Installed")
            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("LargeFilesVersion")) Then Helper.SetRegKey(Of String)("LargeFilesVersion", "Not Installed")

            Dim style As String = Helper.GetRegKey(Of String)("Style")

            If Not String.IsNullOrEmpty(style) Then
                Select Case style
                    Case "Blue"
                        StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Office2007Blue
                        frmPSO2Options.StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Office2007Blue

                    Case "Silver"
                        StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Office2007Silver
                        frmPSO2Options.StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Office2007Silver

                    Case "Black"
                        StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Office2007Black
                        frmPSO2Options.StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Office2007Black

                    Case "Vista Glass"
                        StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Office2007VistaGlass
                        frmPSO2Options.StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Office2007VistaGlass

                    Case "2010 Silver"
                        StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Office2010Silver
                        frmPSO2Options.StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Office2010Silver

                    Case "Windows 7 Blue"
                        StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Windows7Blue
                        frmPSO2Options.StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Windows7Blue

                    Case Else
                        StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Office2007Blue
                        frmPSO2Options.StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Office2007Blue
                End Select
            End If

            ' TODO: neeeds some doing

            Log("Loading textbox settings")

            If Not String.IsNullOrEmpty(Helper.GetRegKey(Of String)("TextBoxBGColor")) Then rtbDebug.BackColor = Color.FromArgb(Helper.GetRegKey(Of String)("TextboxBGColor"))
            If Not String.IsNullOrEmpty(Helper.GetRegKey(Of String)("TextBoxColor")) Then rtbDebug.ForeColor = Color.FromArgb(Helper.GetRegKey(Of String)("TextboxColor"))

            Log("Colors")

            If Not String.IsNullOrEmpty(Helper.GetRegKey(Of String)("Color")) Then
                StyleManager1.ManagerColorTint = Color.FromArgb(Helper.GetRegKey(Of String)("Color"))
                frmPSO2Options.StyleManager1.ManagerColorTint = Color.FromArgb(Helper.GetRegKey(Of String)("Color"))
            End If

            If Not String.IsNullOrEmpty(Helper.GetRegKey(Of String)("FontColor")) Then
                Dim Color As System.Drawing.Color = Color.FromArgb(Helper.GetRegKey(Of String)("FontColor"))
                frmPSO2Options.TabControl1.ColorScheme.TabItemSelectedText = Color
                frmPSO2Options.TabItem1.TextColor = Color
                frmPSO2Options.TabItem2.TextColor = Color
                frmPSO2Options.TabItem3.TextColor = Color
                frmPSO2Options.TabItem4.TextColor = Color
                frmPSO2Options.Slider1.TextColor = Color
                frmPSO2Options.CheckBoxX1.TextColor = Color
                Me.ForeColor = Color
                frmOptions.ForeColor = Color
                frmPSO2Options.ForeColor = Color
                frmOptions.CheckBoxX1.TextColor = Color
                frmOptions.CheckBoxX2.TextColor = Color
                frmOptions.CheckBoxX3.TextColor = Color
                frmOptions.CheckBoxX4.TextColor = Color
                frmOptions.chkAutoRemoveCensor.TextColor = Color
                chkRemoveCensor.TextColor = Color
                chkRemoveNVidia.TextColor = Color
                chkRemovePC.TextColor = Color
                chkRemoveSEGA.TextColor = Color
                chkRemoveVita.TextColor = Color
                chkRestoreCensor.TextColor = Color
                chkRestoreNVidia.TextColor = Color
                chkRestorePC.TextColor = Color
                chkRestoreSEGA.TextColor = Color
                chkRestoreVita.TextColor = Color
                chkSwapOP.TextColor = Color
            End If

            PB1.Text = ""
            Log("Checking if the PSO2 Tweaker is running")

            If CheckIfRunning("PSO2 Tweaker") = "Running" Then
                Application.ExitThread()
            End If

            Me.Text = ("PSO2 Tweaker ver " & My.Application.Info.Version.ToString)
            Application.DoEvents()
            Me.Show()

            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("SeenDownloadMessage")) Then Helper.SetRegKey(Of String)("SeenDownloadMessage", "No")
            If Application.StartupPath = GetDownloadsPath() Then
                If Helper.GetRegKey(Of String)("SeenDownloadMessage") = "No" Then
                    MsgBox("Please be aware - Due to various Windows 7/8 issues, this program might not work correctly while in the ""Downloads"" folder. Please move it to it's own folder, like C:\Tweaker\")
                    Helper.SetRegKey(Of String)("SeenDownloadMessage", "Yes")
                End If
            End If

            LockGUI()

            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("AlwaysOnTop")) Then Helper.SetRegKey(Of String)("AlwaysOnTop", "False")
            Me.TopMost = Helper.GetRegKey(Of String)("AlwaysOnTop")
            chkAlwaysOnTop.Checked = Helper.GetRegKey(Of Boolean)("AlwaysOnTop")
            If File.Exists((Application.StartupPath & "\logfile.txt")) Then
                Dim LogInfo As New FileInfo((Application.StartupPath & "\logfile.txt"))
                If LogInfo.Length > 30720 Then
                    File.WriteAllText((Application.StartupPath & "\logfile.txt"), "")
                End If
            End If

            Application.DoEvents()

            If Not nodiag Then
                Log(vbCrLf)
                Log("----------------------------------------")
                Log(My.Resources.strProgramOpeningRunningDiagnostics)
                Log(My.Resources.strCurrentOSFullName & My.Computer.Info.OSFullName)
                Log(My.Resources.strCurrentOSVersion & My.Computer.Info.OSVersion)
                Log(My.Resources.strIsTheCurrentOS64bit & Environment.Is64BitOperatingSystem)
                Log(My.Resources.strRunDirectory & Application.StartupPath)
                Log(My.Resources.strSelectedPSO2win32directory & lblDirectory.Text)
                Log(My.Resources.strIsUnrarAvailable & File.Exists(Application.StartupPath & "\UnRar.exe"))
                Dim identity = WindowsIdentity.GetCurrent()
                Dim principal = New WindowsPrincipal(identity)
                Dim isElevated As Boolean = principal.IsInRole(WindowsBuiltInRole.Administrator)
                Log("Run as Administrator: " & isElevated)
                Log("Is 7zip available: " & File.Exists(Application.StartupPath & "\7za.exe"))
                Log("Is 7zip available: " & File.Exists("7za.exe"))
                Log("----------------------------------------")
            End If

            If nodiag Then
                Log("Diagnostic info skipped due to -nodiag flag!")
            End If

            WriteDebugInfoAndOK((My.Resources.strProgramOpeningSuccessfully & My.Application.Info.Version.ToString))
            Application.DoEvents()
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try

        Try
            Dim DirectoryString As String = (lblDirectory.Text & "\data\win32")
            Dim pso2launchpath As String = DirectoryString.Replace("\data\win32", "")
            Dim localVersion As String = My.Application.Info.Version.ToString
            Dim wc As MyWebClient = New MyWebClient() With {.timeout = 10000, .Proxy = Nothing}
            Dim source As String = String.Empty
            DeleteFile(Application.StartupPath & "\version.xml")
            WriteDebugInfo(My.Resources.strCheckingforupdatesPleasewaitamoment)
            source = wc.DownloadString("http://162.243.211.123/freedom/version.xml")

            If source.Contains("<VersionHistory>") Then

                Dim xm As New Xml.XmlDocument
                xm.LoadXml(source)

                Dim XMLNode = xm.SelectSingleNode("//CurrentVersion")
                Dim currentVersion As String = XMLNode.ChildNodes(0).InnerText.Trim

                Log("Checking for the latest version of the program...")
                If localVersion = currentVersion Then
                    WriteDebugInfo((My.Resources.strYouhavethelatestversionoftheprogram & My.Application.Info.Version.ToString))
                Else
                    Dim changelogtotal As String = ""

                    For index As Integer = 2 To 11
                        Dim innerText = XMLNode.ChildNodes(index).InnerText.Trim
                        If Not String.IsNullOrWhiteSpace(innerText) Then changelogtotal &= vbCrLf & innerText
                    Next

                    Dim updateyesno As MsgBoxResult = MsgBox(My.Resources.strYouareusinganoutdatedversionoftheprogram & My.Application.Info.Version.ToString & My.Resources.strAndthelatestis & currentVersion & My.Resources.strWouldyouliketodownloadthenewversion & vbCrLf & vbCrLf & My.Resources.strChanges & vbCrLf & changelogtotal, MsgBoxStyle.YesNo)
                    If updateyesno = MsgBoxResult.Yes Then
                        WriteDebugInfo(My.Resources.strDownloadingUpdate)
                        DLWUA("http://162.243.211.123/freedom/PSO2%20Tweaker%20Updater.exe", "PSO2 Tweaker Updater.exe", True)
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
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try

        Dim t3 As New Thread(AddressOf IsServerOnline) With {.IsBackground = True}
        t3.Start()

        Try
            Dim DirectoryString As String = (lblDirectory.Text & "\data\win32")
            Dim pso2launchpath As String = DirectoryString.Replace("\data\win32", "")
            Application.DoEvents()

            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("PSO2Dir")) Then
                MsgBox(My.Resources.strPleaseSelectPSO2win32dir)
                Button1.RaiseClick()
            Else
                lblDirectory.Text = Helper.GetRegKey(Of String)("PSO2Dir")
            End If

            If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) AndAlso File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585")) Then
                If GetFileSize(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) = 167479840 AndAlso GetFileSize(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585")) = 151540352 Then
                    chkSwapOP.Text = My.Resources.strSwapPCVitaOpenings & " (" & My.Resources.strNotSwapped & ")"
                End If
                If GetFileSize(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) = 151540352 AndAlso GetFileSize(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585")) = 167479840 Then
                    chkSwapOP.Text = My.Resources.strSwapPCVitaOpenings & " (" & My.Resources.strSwapped & ")"
                End If
            End If

            ' Shouldn't be doing this in this way
            Application.DoEvents()

            If Not File.Exists("7za.exe") Then
                WriteDebugInfo(My.Resources.strDownloading & "7za.exe...")
                Application.DoEvents()
                DLWUA("http://162.243.211.123/freedom/7za.exe", "7za.exe", True)
            End If

            If Helper.GetMD5("7za.exe") <> "42BADC1D2F03A8B1E4875740D3D49336" Then
                WriteDebugInfo(My.Resources.strYour7zipiscorrupt)
                Application.DoEvents()
                DLWUA("http://162.243.211.123/freedom/7za.exe", "7za.exe", True)
            End If

            If Helper.GetMD5("7za.exe") <> "42BADC1D2F03A8B1E4875740D3D49336" Then
                WriteDebugInfo(My.Resources.strYour7zipiscorrupt)
                Application.DoEvents()
                DLWUA("http://162.243.211.123/freedom/7za.exe", "7za.exe", True)
            End If

            If Helper.GetMD5("7za.exe") <> "42BADC1D2F03A8B1E4875740D3D49336" Then
                WriteDebugInfo(My.Resources.strYour7zipiscorrupt)
                Application.DoEvents()
                DLWUA("http://162.243.211.123/freedom/7za.exe", "7za.exe", True)
            End If

            If Not File.Exists("UnRar.exe") Then
                WriteDebugInfo(My.Resources.strDownloading & "UnRar.exe...")
                Application.DoEvents()
                DLWUA("http://162.243.211.123/freedom/UnRAR.exe", "UnRAR.exe", True)
            End If

            If Helper.GetMD5("UnRar.exe") <> "0C83C1293723A682577E3D0B21562B79" Then
                WriteDebugInfo(My.Resources.strYourUnrariscorrupt)
                Application.DoEvents()
                DLWUA("http://162.243.211.123/freedom/UnRAR.exe", "UnRAR.exe", True)
            End If

            If Helper.GetMD5("UnRar.exe") <> "0C83C1293723A682577E3D0B21562B79" Then
                WriteDebugInfo(My.Resources.strYourUnrariscorrupt)
                Application.DoEvents()
                DLWUA("http://162.243.211.123/freedom/UnRAR.exe", "UnRAR.exe", True)
            End If

            If Helper.GetMD5("UnRar.exe") <> "0C83C1293723A682577E3D0B21562B79" Then
                WriteDebugInfo(My.Resources.strYourUnrariscorrupt)
                Application.DoEvents()
                DLWUA("http://162.243.211.123/freedom/UnRAR.exe", "UnRAR.exe", True)
            End If

            If Directory.Exists("TEMPSTORYAIDAFOOL") Then
                Directory.Delete("TEMPSTORYAIDAFOOL", True)
            End If

            If Directory.Exists("TEMPPATCHAIDAFOOL") Then
                Directory.Delete("TEMPPATCHAIDAFOOL", True)
            End If

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

            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("SidebarEnabled")) Then Helper.SetRegKey(Of String)("SidebarEnabled", "True")
            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("RemoveCensor")) Then Helper.SetRegKey(Of String)("RemoveCensor", "True")

            If Helper.GetRegKey(Of String)("SidebarEnabled") = "True" Then
                WriteDebugInfo(My.Resources.strLoadingSidebar)
                Dim t1 As New Thread(AddressOf LoadSidebar) With {.IsBackground = True}
                t1.Start()

                If DPISetting = 96 Then Me.Width = 796
                If DPISetting = 120 Then Me.Width = 1060
                btnAnnouncements.Text = "<"
            End If

            If File.Exists("resume.txt") Then
                Dim YesNoResume As MsgBoxResult = MsgBox("It seems that the last patching attempt was interrupted. Would you like to resume patching?", vbYesNo)
                If YesNoResume = MsgBoxResult.Yes Then
                    btnResumePatching.RaiseClick()
                Else
                    DeleteFile("resume.txt")
                End If
            End If

            WriteDebugInfo(My.Resources.strCheckingforPSO2Updates)
            Application.DoEvents()

            CheckForPSO2Updates()
            WriteDebugInfoSameLine(My.Resources.strDone)
            Application.DoEvents()

            'Check for PSO2 Updates here, download the version.ver thingie
            'Check for PSO2 EN Patch updates here, parse the URL and see if it's different from the saved one
            'Check for EN Story Patch
            WriteDebugInfo(My.Resources.strCheckingforUpdatestopatches)

            'Check for Story Patches (Done! :D)
            Application.DoEvents()
            CheckForStoryUpdates()
            WriteDebugInfo(My.Resources.strCurrentStoryPatchis & Helper.GetRegKey(Of String)("StoryPatchVersion"))
            Application.DoEvents()

            'Check for English Patches (Done! :D)
            CheckForENPatchUpdates()
            WriteDebugInfo(My.Resources.strCurrentENPatchis & Helper.GetRegKey(Of String)("ENPatchVersion"))
            Application.DoEvents()

            'Check for LargeFiles Update (Work-In-Progress!)
            CheckForLargeFilesUpdates()
            WriteDebugInfo(My.Resources.strCurrentLargeFilesis & Helper.GetRegKey(Of String)("LargeFilesVersion"))
            Application.DoEvents()

            WriteDebugInfo(My.Resources.strIfAboveVersions)

            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("UseItemTranslation")) Then
                Helper.SetRegKey(Of String)("UseItemTranslation", "True")
            End If

            UseItemTranslation = Helper.GetRegKey(Of String)("UseItemTranslation")

            If UseItemTranslation Then
                chkItemTranslation.Checked = True
                DirectoryString = (lblDirectory.Text & "\data\win32")
                pso2launchpath = DirectoryString.Replace("\data\win32", "")
                WriteDebugInfo(My.Resources.strDownloadingItemTranslationFiles)
                ItemDownloadingDone = False
                Dim t4 As New Thread(AddressOf DownloadItemTranslationFiles) With {.IsBackground = True}
                t4.Start()

                Do Until ItemDownloadingDone = True
                    Application.DoEvents()
                    Thread.Sleep(16)
                Loop

                WriteDebugInfoSameLine(My.Resources.strDone)
            End If
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try

        DLWUA("http://162.243.211.123/freedom/working.txt", "working.txt", True)

        Using oReader As StreamReader = File.OpenText("working.txt")
            Dim sBuffer As String = oReader.ReadLine
            WriteDebugInfo(My.Resources.strDoesItemPatchWork & sBuffer.ToString)
        End Using

        DeleteFile("working.txt")
        DeleteFile("version.xml")
        DeleteFile("Story MD5HashList.txt")
        DeleteFile("PSO2 Tweaker Updater.exe")
        WriteDebugInfo(My.Resources.strAllDoneSystemReady)
        btnLaunchPSO2.Enabled = True
    End Sub

    Public Sub DownloadItemTranslationFiles()
        Dim DirectoryString As String = (lblDirectory.Text & "\data\win32")
        Dim pso2launchpath As String = DirectoryString.Replace("\data\win32", "")
        Dim DLLink1 As String = "http://162.243.211.123/freedom/translator.dll"
        Dim DLLink2 As String = "http://162.243.211.123/freedom/translation.bin"
        Dim client As New WebClient

        Try
            client.DownloadFile(DLLink1, (pso2launchpath & "\translator.dll"))
        Catch ex As Exception
            MsgBox("Failed to download translation files! (" & ex.Message & ")")
        End Try

        Helper.SetRegKey(Of String)("DLLMD5", Helper.GetMD5(pso2launchpath & "\translator.dll"))

        Try
            client.DownloadFile(DLLink2, (pso2launchpath & "\translation.bin"))
        Catch ex As Exception
            MsgBox("Failed to download translation files! (" & ex.Message & ")")
        End Try

        ItemDownloadingDone = True
    End Sub

    Friend Function GetFolderAccess(ByVal Path As String) As Boolean
        Try
            Dim thing As AuthorizationRuleCollection = Directory.GetAccessControl(Path, AccessControlSections.All).GetAccessRules(True, True, GetType(NTAccount))

            For Each rule As FileSystemAccessRule In thing
                If (rule.FileSystemRights And FileSystemRights.Write) = FileSystemRights.Write Then Return True
            Next
        Catch
            Return False
        End Try

        Return True
    End Function

    Private Function GetFileSize(ByVal MyFilePath As String) As Long
        Dim MyFile As New FileInfo(MyFilePath)
        Return MyFile.Length
    End Function

    Public Sub WriteDebugInfo(ByVal AddThisText As String)
        If rtbDebug.InvokeRequired Then
            rtbDebug.Invoke(New Action(Of String)(AddressOf WriteDebugInfo), Text)
        Else
            rtbDebug.Text = rtbDebug.Text & vbCrLf & AddThisText
            Dim TimeFormatted As String = DateTime.Now.ToString("G")
            File.AppendAllText((Application.StartupPath & "\logfile.txt"), TimeFormatted & " " & AddThisText & vbCrLf)
        End If
    End Sub

    Private Sub WriteDebugInfoSameLine(ByVal AddThisText As String)
        If rtbDebug.InvokeRequired Then
            rtbDebug.Invoke(New Action(Of String)(AddressOf WriteDebugInfoSameLine), Text)
        Else
            rtbDebug.Text = rtbDebug.Text & " " & AddThisText
            Dim TimeFormatted As String = DateTime.Now.ToString("G")
            File.AppendAllText((Application.StartupPath & "\logfile.txt"), TimeFormatted & " " & AddThisText & vbCrLf)
        End If
    End Sub

    Private Sub WriteDebugInfoAndOK(ByVal AddThisText As String)
        If rtbDebug.InvokeRequired Then
            rtbDebug.Invoke(New Action(Of String)(AddressOf WriteDebugInfoAndOK), Text)
        Else
            rtbDebug.Text = rtbDebug.Text & vbCrLf & AddThisText
            rtbDebug.Select(rtbDebug.TextLength, 0)
            rtbDebug.SelectionColor = Color.Green
            rtbDebug.AppendText(" [OK!]")
            rtbDebug.SelectionColor = rtbDebug.ForeColor
            Dim TimeFormatted As String = DateTime.Now.ToString("G")
            File.AppendAllText((Application.StartupPath & "\logfile.txt"), TimeFormatted & " " & (AddThisText & " [OK!]") & vbCrLf)
        End If
    End Sub

    Private Sub WriteDebugInfoAndWarning(ByVal AddThisText As String)
        If rtbDebug.InvokeRequired Then
            rtbDebug.Invoke(New Action(Of String)(AddressOf WriteDebugInfoAndWarning), Text)
        Else
            rtbDebug.Text = rtbDebug.Text & vbCrLf & AddThisText
            rtbDebug.Select(rtbDebug.TextLength, 0)
            rtbDebug.SelectionColor = Color.Red
            rtbDebug.AppendText(" [WARNING!]")
            rtbDebug.SelectionColor = rtbDebug.ForeColor
            Dim TimeFormatted As String = DateTime.Now.ToString("G")
            File.AppendAllText((Application.StartupPath & "\logfile.txt"), TimeFormatted & " " & (AddThisText & " [WARNING!]") & vbCrLf)
        End If
    End Sub

    Private Sub WriteDebugInfoAndFAILED(ByVal AddThisText As String)
        If rtbDebug.InvokeRequired Then
            rtbDebug.Invoke(New Action(Of String)(AddressOf WriteDebugInfoAndFAILED), Text)
        Else
            If AddThisText = "ERROR - Index was outside the bounds of the array." Then Exit Sub
            If AddThisText = "ERROR - Object reference not set to an instance of an object." Then Exit Sub
            rtbDebug.Text = rtbDebug.Text & vbCrLf & AddThisText
            rtbDebug.Select(rtbDebug.TextLength, 0)
            rtbDebug.SelectionColor = Color.Red
            rtbDebug.AppendText(My.Resources.strFAILED)
            rtbDebug.SelectionColor = rtbDebug.ForeColor
            Dim TimeFormatted As String = DateTime.Now.ToString("G")
            File.AppendAllText((Application.StartupPath & "\logfile.txt"), TimeFormatted & " " & (AddThisText & " [FAILED!]") & vbCrLf)
            If Helper.GetRegKey(Of String)("Pastebin") Then
                Dim upload As MsgBoxResult = MsgBox(My.Resources.strSomethingWentWrongUpload, vbYesNo)
                If upload = MsgBoxResult.Yes Then
                    PasteBinUpload()
                End If
                If upload = MsgBoxResult.No Then UnlockGUI()
            End If
            UnlockGUI()
        End If
    End Sub

    Private Sub ClearDebugInfo()
        If rtbDebug.InvokeRequired Then
            rtbDebug.Invoke(New Action(Of String)(AddressOf ClearDebugInfo), Text)
        Else
            rtbDebug.Text = ""
        End If
    End Sub

    Private Sub rtbDebug_LinkClicked(sender As Object, e As LinkClickedEventArgs) Handles rtbDebug.LinkClicked
        Process.Start(e.LinkText)
    End Sub

    Private Sub rtbDebug_TextChanged(sender As Object, e As EventArgs) Handles rtbDebug.TextChanged
        rtbDebug.SelectionStart = rtbDebug.Text.Length
    End Sub

    Private Sub OnDownloadProgressChanged(ByVal sender As Object, ByVal e As DownloadProgressChangedEventArgs)
        Dim totalSize As Long = e.TotalBytesToReceive
        totalsize2 = totalSize
        Dim downloadedBytes As Long = e.BytesReceived
        Dim percentage As Integer = e.ProgressPercentage
        PB1.Value = percentage
        PB1.Text = (My.Resources.strDownloaded & Helper.SizeSuffix(downloadedBytes) & " / " & Helper.SizeSuffix(totalSize) & " (" & percentage & "%) - " & My.Resources.strRightClickforOptions)
        'Put your progress UI here, you can cancel download by uncommenting the line below
    End Sub

    Public Sub OnFileDownloadCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs)
        PB1.Value = 0
        PB1.Text = ""
    End Sub

    Public Function DLWUA(ByVal Address As String, ByVal Filename As String, ByVal Overwrite As String) As Boolean

        ' Highly Volital Function -Matthew
        ' TODO: Should be the first thing fixed after refactoring is done

        Overwrite = Application.StartupPath & "\" & Overwrite
        AddHandler DLS.DownloadProgressChanged, AddressOf OnDownloadProgressChanged
        AddHandler DLS.DownloadFileCompleted, AddressOf OnFileDownloadCompleted

        DLS.Headers.Add("user-agent", "AQUA_HTTP")
        DLS.timeout = 10000

        For i As Integer = 1 To 5
            Try
                Application.DoEvents()
                DLS.DownloadFileAsync((New Uri(Address)), Filename)
                Application.DoEvents()
                Exit For
            Catch ex As Exception
                If i = 4 Then Thread.Sleep(5000)
                If i = 5 Then Return False
            End Try
        Next

        While DLS.IsBusy = True
            Application.DoEvents()
            If Restartplz Then
                DLS.CancelAsync()
                Restartplz = False
                While DLS.IsBusy = True

                End While
                DLS.DownloadFileAsync((New Uri(Address)), Filename)
            End If
            If Not Me.Visible Then
                DLS.CancelAsync()
                Cancelled = True
                Application.Exit()
            End If
        End While

        Return True
    End Function

    Private Sub LockGUI()
        Me.Enabled = False
    End Sub

    Private Sub UnlockGUI()
        Me.Enabled = True
    End Sub

    Public Sub Log(ByRef Text As String)
        File.AppendAllText((Application.StartupPath & "\logfile.txt"), DateTime.Now.ToString("G") & ": DEBUG - " & Text & vbCrLf)
    End Sub

    Public Function PasteBinUpload() As String
        ServicePointManager.Expect100Continue = False
        Dim pr As Integer = 0
        Dim fi As String = "?api_paste_private=" & 1 & "&api_option=paste" & "&api_paste_name=Error Log report" & "&api_paste_format=text" & "&api_paste_expire_date=N" & "&api_dev_key=ddc1e2efaca45d3df87e6b93ceb43c9f" & "&api_paste_code=" & File.ReadAllText((Application.StartupPath & "\logfile.txt"))
        Dim w As New WebClient()
        w.Headers.Add("Content-Type", "application/x-www-form-urlencoded")
        Dim pd As Byte() = Encoding.ASCII.GetBytes(fi)
        Dim rd As Byte() = w.UploadData("http://pastebin.com/api/api_post.php", "POST", pd)
        Dim r As String = Encoding.ASCII.GetString(rd)
        MsgBox(My.Resources.strPleasecopytheURL)
        Process.Start(r)
        Return r
    End Function

    Public Function PasteBinUploadFile(ByRef FileToUpload As String) As String
        ServicePointManager.Expect100Continue = False
        Dim pr As Integer = 0
        Dim fi As String = "?api_paste_private=" & 1 & "&api_option=paste" & "&api_paste_name=Error Log report" & "&api_paste_format=text" & "&api_paste_expire_date=N" & "&api_dev_key=ddc1e2efaca45d3df87e6b93ceb43c9f" & "&api_paste_code=" & File.ReadAllText(FileToUpload)
        Dim w As New WebClient()
        w.Headers.Add("Content-Type", "application/x-www-form-urlencoded")
        Dim pd As Byte() = Encoding.ASCII.GetBytes(fi)
        Dim rd As Byte() = w.UploadData("http://pastebin.com/api/api_post.php", "POST", pd)
        Dim r As String = Encoding.ASCII.GetString(rd)
        MsgBox(My.Resources.strPleasecopytheURL)
        Process.Start(r)
        Return r
    End Function

    Public Sub MergePatches()
        Dim strTemp As String() = File.ReadAllLines("patchlist.txt")
        Dim strTemp2 As String() = File.ReadAllLines("patchlist_old.txt")
        Dim lines(strTemp.Length + strTemp2.Length) As String

        Array.Copy(strTemp, lines, strTemp.Length)
        Array.Copy(strTemp2, 0, lines, strTemp.Length, strTemp2.Length)

        Dim files = New Dictionary(Of String, String)

        For index As Integer = 0 To (lines.Length - 1)
            Dim currentLine = lines(index)
            If String.IsNullOrEmpty(currentLine) Then Continue For

            Dim filename = Regex.Split(currentLine, ".pat")
            If String.IsNullOrEmpty(filename(0)) Then Continue For

            Dim key = filename(0).Replace("data/win32/", "")

            If Not files.ContainsKey(key) Then
                files.Add(key, currentLine)
            End If
        Next

        SOMEOFTHETHINGS = files
    End Sub

    Public Sub MergePrePatches()

        Dim filename As String()
        Dim truefilename As String

        Using objWriter As New StreamWriter("ALLOFTHEPREPATCHES.txt")
            For i As Integer = 0 To 5
                objWriter.Write(File.ReadAllText("patchlist" & i & ".txt"))
            Next
            objWriter.WriteLine()
        End Using

        ' TODO: Needs a bit of a redo

        Using sr3 = New StreamReader("ALLOFTHEPREPATCHES.txt")
            Using sw3 = New StreamWriter("SOMEOFTHEPREPATCHES.txt")
                Dim MyArray As New ArrayList
                Dim strLine As String

                Do While sr3.Peek <> -1
                    strLine = sr3.ReadLine()
                    filename = Regex.Split(strLine, ".pat")
                    truefilename = filename(0).Replace("data/win32/", "")

                    If (Not MyArray.Contains(truefilename)) AndAlso (Not String.IsNullOrEmpty(truefilename)) Then
                        MyArray.Add(truefilename)
                        sw3.WriteLine(strLine)
                    End If
                Loop
            End Using
        End Using
    End Sub

    Private Function CheckIfRunning(ByRef ProcessName As String)
        ' TODO: should change what this returns
        processes = Process.GetProcessesByName(ProcessName)
        Dim currentProcess As Process = Process.GetCurrentProcess()

        Dim x As Integer = 0
        If ProcessName = "PSO2 Tweaker" Then x = 1

        If processes.Count > x Then
            Dim CloseItYesNo As MsgBoxResult = MsgBox("It seems that " & ProcessName.Replace(".exe", "") & " is already running. Would you like to close it?", vbYesNo)
            If CloseItYesNo = vbYes Then
                Dim procs As Process() = Process.GetProcessesByName(ProcessName)

                For Each proc As Process In procs
                    If proc.Id <> currentProcess.Id Then proc.Kill()
                Next
                Return "Running, but ending"
            Else
                Return "Running"
            End If
        Else
            Return "Not Running"
        End If
    End Function

    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        DLS.CancelAsync()
        Cancelled = True
        PB1.Value = 0
        PB1.Text = ""
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

    Public Sub CheckForStoryUpdates()
        Try
            If Helper.GetRegKey(Of String)("StoryPatchVersion") = "Not Installed" Then Exit Sub
            DLWUA("http://162.243.211.123/patchfiles/Story%20MD5HashList.txt", "Story MD5HashList.txt", True)
            Dim filedownloader As New WebClient()
            Dim sBuffer As String
            Dim filename As String()
            Dim truefilename As String
            Dim missingfiles As New List(Of String)
            Dim filedownloader2 As New WebClient()
            Dim missingfiles2 As New List(Of String)
            Dim NumberofChecks As Integer
            Dim TrueMD5 As String
            Dim UpdateNeeded As Boolean = False
            NumberofChecks = 0
            missingfiles.Clear()
            Using oReader As StreamReader = File.OpenText("Story MD5HashList.txt")
                sBuffer = oReader.ReadLine
                Helper.SetRegKey(Of String)("NewVersionTemp", sBuffer.ToString)
                Helper.SetRegKey(Of String)("NewStoryVersion", sBuffer.ToString)
                If sBuffer.ToString <> Helper.GetRegKey(Of String)("StoryPatchVersion") Then
                    UpdateNeeded = True
                    'A new story patch update is available - Would you like to download and install it? PLEASE NOTE: This update assumes you've already downloaded and installed the latest RAR file available from http://arks-layer.com, which seems to be: 
                    Dim net As New WebClient()
                    Dim src As String = net.DownloadString("http://arks-layer.com/story.php")
                    ' Create a match using regular exp<b></b>ressions
                    'http://arks-layer.com/Story%20Patch%208-8-2013.rar.torrent
                    Dim m As Match = Regex.Match(src, "<u>.*?</u>")
                    ' Spit out the value plucked from the code
                    txtHTML.Text = m.Value
                    Dim strDownloadME As String = txtHTML.Text.Replace("<u>", "").Replace("</u>", "")
                    If strDownloadME <> Helper.GetRegKey(Of String)("LatestStoryBase") Then
                        Dim MBVisitLink As MsgBoxResult = MsgBox("A new story patch is available! Would you like to download and install it using the new story patch method?", MsgBoxStyle.YesNo)
                        If MBVisitLink = vbYes Then
                            btnStoryPatchNew.RaiseClick()
                            Exit Sub
                        End If
                        If MBVisitLink = vbNo Then Exit Sub
                    End If

                    Dim UpdateStoryYesNo As MsgBoxResult = MsgBox(My.Resources.strStoryModeUpdate & strDownloadME, vbYesNo)
                    If UpdateStoryYesNo = vbNo Then
                        Exit Sub
                    End If
                End If
                If UpdateNeeded Then
                    WriteDebugInfo(My.Resources.strBeginningStoryModeUpdate)
                    While Not (oReader.EndOfStream)
                        sBuffer = oReader.ReadLine
                        filename = sBuffer.Split(","c)
                        truefilename = filename(0)
                        TrueMD5 = filename(1)
                        If Not File.Exists(((lblDirectory.Text & "\data\win32") & "\" & truefilename)) Then
                            missingfiles.Add(truefilename)
                            GoTo NEXTFILE1
                        End If
                        If Helper.GetMD5(((lblDirectory.Text & "\data\win32") & "\" & truefilename)) <> TrueMD5 Then
                            missingfiles.Add(truefilename)
                            GoTo NEXTFILE1
                        End If
NEXTFILE1:
                        NumberofChecks += 1
                        lblStatus.Text = (My.Resources.strCurrentlyCheckingFile & NumberofChecks & "")
                        Application.DoEvents()
                    End While
                End If
            End Using
            If UpdateNeeded Then
                Dim totaldownload As String = missingfiles.Count
                Dim downloaded As Long = 0

                WriteDebugInfo("Downloading/Installing updates using Patch Server #4 (New York)")

                For Each downloadstring In missingfiles
                    'Download the missing files:
                    downloaded += 1
                    lblStatus.Text = My.Resources.strUpdating & downloaded & "/" & totaldownload
                    Application.DoEvents()
                    Cancelled = False
                    DLWUA(("http://162.243.211.123/patchfiles/" & downloadstring & ".7z"), downloadstring & ".7z", True)
                    If Cancelled Then Exit Sub
                    'Delete the existing file FIRST
                    If Not File.Exists(downloadstring & ".7z") Then
                        WriteDebugInfoAndFAILED("File " & (downloadstring & ".7z") & " does not exist! Perhaps it wasn't downloaded properly?")
                    End If
                    DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
                    Dim process As Process = Nothing
                    Dim processStartInfo As ProcessStartInfo = New ProcessStartInfo()
                    Dim UnRarLocation As String = (Application.StartupPath & "\7za.exe")
                    processStartInfo.FileName = UnRarLocation
                    processStartInfo.Verb = "runas"
                    processStartInfo.Arguments = ("e -y " & downloadstring & ".7z")
                    processStartInfo.WindowStyle = ProcessWindowStyle.Hidden
                    processStartInfo.UseShellExecute = True
                    process = process.Start(processStartInfo)
                    Do Until process.WaitForExit(1000)
                    Loop
                    If Not File.Exists(downloadstring) Then
                        WriteDebugInfoAndFAILED("File " & (downloadstring) & " does not exist! Perhaps it wasn't extracted properly?")
                    End If
                    File.Move(downloadstring, ((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
                    DeleteFile(downloadstring & ".7z")
                    Application.DoEvents()
                Next
                WriteDebugInfoAndOK(My.Resources.strStoryPatchUpdated)
                Helper.SetRegKey(Of String)("StoryPatchVersion", Helper.GetRegKey(Of String)("NewVersionTemp"))
                Helper.SetRegKey(Of String)("NewVersionTemp", "")
                Exit Sub
            End If
            If Not UpdateNeeded Then
                WriteDebugInfoAndOK("You have the latest story patch updates!")
                Exit Sub
            End If
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub

    Public Sub CheckForENPatchUpdates()
        Dim UpdateNeeded As Boolean
        Try
            If Helper.GetRegKey(Of String)("ENPatchVersion") = "Not Installed" Then Exit Sub

            Application.DoEvents()
            Dim net As MyWebClient = New MyWebClient() With {.timeout = 10000}
            Dim src As String = net.DownloadString("http://162.243.211.123/patches/enpatch.txt")
            Dim strDownloadME As String = src
            Dim Lastfilename As String() = strDownloadME.Split("/")
            Dim strVersion As String = Lastfilename(Lastfilename.Count - 1).Replace(".rar", "")

            Helper.SetRegKey(Of String)("NewENVersion", strVersion)
            If strVersion <> Helper.GetRegKey(Of String)("ENPatchVersion") Then
                UpdateNeeded = True
                Dim UpdateStoryYesNo As MsgBoxResult = MsgBox(My.Resources.strNewENPatch, vbYesNo)
                If UpdateStoryYesNo = vbNo Then UpdateNeeded = False
                If UpdateNeeded Then
                    btnENPatch.RaiseClick()
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub

    Public Sub CheckForLargeFilesUpdates()
        Dim UpdateNeeded As Boolean
        Try
            If Helper.GetRegKey(Of String)("LargeFilesVersion") = "Not Installed" Then
                Exit Sub
            End If
            Application.DoEvents()
            Dim net As MyWebClient = New MyWebClient() With {.timeout = 10000}
            Dim src As String = net.DownloadString("http://162.243.211.123/patches/largefiles.txt")
            Dim strDownloadME As String = src
            Dim Lastfilename As String() = strDownloadME.Split("/")
            Dim strVersion As String = Lastfilename(Lastfilename.Count - 1).Replace(".rar", "")

            Helper.SetRegKey(Of String)("NewLargeFilesVersion", strVersion)
            If strVersion <> Helper.GetRegKey(Of String)("LargeFilesVersion") Then
                UpdateNeeded = True
                Dim UpdateStoryYesNo As MsgBoxResult = MsgBox(My.Resources.strNewLargeFiles, vbYesNo)
                If UpdateStoryYesNo = vbNo Then UpdateNeeded = False
                If UpdateNeeded Then
                    btnLargeFiles.RaiseClick()
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub

    Private Sub DeleteDirectory(path As String)
        If Directory.Exists(path) Then Directory.Delete(path, True)
    End Sub

    Public Sub CheckForPSO2Updates()
        Try
            Dim filedownloader As New WebClient()
            Dim UpdateNeeded As Boolean
            Dim versionclient As New MyWebClient With {.timeout = 3000}
            versionclient.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
            'Precede file, syntax is Yes/No:<Dateoflastprepatch>
            versionclient.DownloadFile("http://162.243.211.123/freedom/precede.txt", "precede.txt")

            If ComingFromPrePatch Then GoTo StartPrePatch

            Dim FirstTimechecking As Boolean = False
            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("PSO2PrecedeVersion")) Then
                Dim precedefile2 As String() = File.ReadAllLines("precede.txt")
                Dim PrecedeVersion2 As String() = precedefile2(0).Split(":")
                Helper.SetRegKey(Of String)("PSO2PrecedeVersion", PrecedeVersion2(1))
                FirstTimechecking = True
            End If

            Dim precedefile = File.ReadAllLines("precede.txt")
            Dim PrecedeSplit As String() = precedefile(0).Split(":")
            Dim PrecedeYesNo As String = PrecedeSplit(0)
            Dim precedeversionstring As String = PrecedeSplit(1)

            If PrecedeYesNo = "Yes" Then
                If Helper.GetRegKey(Of String)("PSO2PrecedeVersion") <> precedeversionstring Or FirstTimechecking Then
                    Dim DownloadPrepatch As MsgBoxResult = MsgBox("New pre-patch data is available to download - Would you like to download it? This is optional, and will let you download some of a large patch now, as opposed to having a larger download all at once when it is released.", MsgBoxStyle.YesNo)
                    If DownloadPrepatch = vbYes Then
StartPrePatch:
                        'Download prepatch
                        CancelledFull = False
                        Dim sBuffer As String = Nothing
                        Dim filename As String() = Nothing
                        Dim truefilename As String = Nothing
                        Dim missingfiles As New List(Of String)
                        Dim filedownloader2 As New WebClient()
                        Dim missingfiles2 As New List(Of String)
                        Dim NumberofChecks As Integer = 0
                        Dim MD5 As String() = Nothing
                        Dim TrueMD5 As String = Nothing
                        Dim PSO2EXEMD5 As String = "FUCK YOU DUDU"
                        lblStatus.Text = ""
                        WriteDebugInfo("Downloading pre-patch filelist...")
                        DLWUA("http://download.pso2.jp/patch_prod/patches_precede/patchlist0.txt", "patchlist0.txt", True)
                        DLWUA("http://download.pso2.jp/patch_prod/patches_precede/patchlist1.txt", "patchlist1.txt", True)
                        DLWUA("http://download.pso2.jp/patch_prod/patches_precede/patchlist2.txt", "patchlist2.txt", True)
                        DLWUA("http://download.pso2.jp/patch_prod/patches_precede/patchlist3.txt", "patchlist3.txt", True)
                        DLWUA("http://download.pso2.jp/patch_prod/patches_precede/patchlist4.txt", "patchlist4.txt", True)
                        DLWUA("http://download.pso2.jp/patch_prod/patches_precede/patchlist5.txt", "patchlist5.txt", True)
                        WriteDebugInfoSameLine(My.Resources.strDone)
                        MergePrePatches()
                        If Not Directory.Exists(lblDirectory.Text & "\_precede\data\win32") Then Directory.CreateDirectory(lblDirectory.Text & "\_precede\data\win32") 'create directory

                        WriteDebugInfo("Checking for already existing precede files...")
                        NumberofChecks = 0
                        missingfiles.Clear()
                        Using oReader As StreamReader = File.OpenText("SOMEOFTHEPREPATCHES.txt")
                            If CancelledFull Then Exit Sub
                            While Not (oReader.EndOfStream)
                                If CancelledFull Then Exit Sub
                                sBuffer = oReader.ReadLine
                                filename = Regex.Split(sBuffer, ".pat")
                                truefilename = filename(0).Replace("data/win32/", "")
                                MD5 = filename(1).Split(vbTab)
                                TrueMD5 = MD5(2)
                                If truefilename <> "GameGuard.des" AndAlso truefilename <> "PSO2JP.ini" AndAlso truefilename <> "script/user_default.pso2" AndAlso truefilename <> "script/user_intel.pso2" Then
                                    If truefilename = "pso2.exe" Then
                                        PSO2EXEMD5 = TrueMD5
                                        GoTo NextFile1
                                    End If
                                    If Not File.Exists(((lblDirectory.Text & "\_precede\data\win32") & "\" & truefilename)) Then
                                        If VedaUnlocked Then WriteDebugInfo("DEBUG: The file " & truefilename & " is missing.")
                                        missingfiles.Add(truefilename)
                                        GoTo NEXTFILE1
                                    End If
                                    If Helper.GetMD5(((lblDirectory.Text & "\_precede\data\win32") & "\" & truefilename)) <> TrueMD5 Then

                                        If VedaUnlocked Then WriteDebugInfo("DEBUG: The file " & truefilename & " must be redownloaded.")
                                        missingfiles.Add(truefilename)
                                        GoTo NEXTFILE1
                                    End If
                                End If
NEXTFILE1:
                                NumberofChecks += 1
                                lblStatus.Text = (My.Resources.strCurrentlyCheckingFile & NumberofChecks & "")
                                Application.DoEvents()
                            End While
                        End Using
                        Dim totaldownload As String = missingfiles.Count
                        Dim downloaded As Long = 0
                        Dim totaldownloaded As Long = 0
                        patching = True

                        For Each downloadstring In missingfiles
                            If CancelledFull Then Exit Sub
                            'Download the missing files:
                            'WHAT THE FUCK IS GOING ON HERE?
                            downloaded += 1
                            totaldownloaded += totalsize2

                            lblStatus.Text = My.Resources.strDownloading & "" & downloaded & "/" & totaldownload & " (" & Helper.SizeSuffix(totaldownloaded) & ")"

                            Application.DoEvents()
                            Cancelled = False
                            DLWUA(("http://download.pso2.jp/patch_prod/patches_precede/data/win32/" & downloadstring & ".pat"), downloadstring, True)
                            Dim info7 As New FileInfo(downloadstring)
                            Dim length2 As Long
                            If File.Exists(downloadstring) Then length2 = info7.Length
                            If info7.Length = 0 Then
                                Log("File appears to be empty, trying to download from secondary SEGA server")
                                DLWUA(("http://download.pso2.jp/patch_prod/patches_precede/data/win32/" & downloadstring & ".pat"), downloadstring, True)
                            End If

                            If Cancelled Then Exit Sub

                            DeleteFile(((lblDirectory.Text & "\_precede\data\win32") & "\" & downloadstring))
                            File.Move(downloadstring, ((lblDirectory.Text & "\_precede\data\win32") & "\" & downloadstring))
                            If VedaUnlocked Then WriteDebugInfo("DEBUG: Downloaded and installed " & downloadstring & ".")

                            Application.DoEvents()
                        Next
                        patching = False
                        If missingfiles.Count = 0 Then WriteDebugInfo("Your precede data is up to date!")
                        If missingfiles.Count <> 0 Then WriteDebugInfo("Precede data downloaded/updated!")
                        Dim precedefile2 As String() = File.ReadAllLines("precede.txt")
                        Dim PrecedeVersion2 As String() = precedefile2(0).Split(":")
                        Helper.SetRegKey(Of String)("PSO2PrecedeVersion", PrecedeVersion2(1))
                    End If
                End If
            End If

            'Check whether or not to apply pre-patch shit. Ugh.
            If Directory.Exists(lblDirectory.Text & "\_precede\") Then
                versionclient.DownloadFile("http://162.243.211.123/freedom/precede_apply.txt", "precede_apply.txt")
                Dim prepatchapply = File.ReadAllLines("precede_apply.txt")
                Dim ApplyPrePatch As String = prepatchapply(0)
                If Not Directory.Exists(lblDirectory.Text & "\_precede\data\win32\") Then GoTo BackToCheckUpdates2
                If ApplyPrePatch = "Yes" Then
                    Dim ApplyPrePatchYesNo As MsgBoxResult = MsgBox("It appears that it's time to install the pre-patch download - Is this okay? If you select no, the pre-patch download will be deleted.", vbYesNo)
                    If ApplyPrePatchYesNo = vbNo Then
                        WriteDebugInfoAndOK("Deleting pre-patch download...")
                        Directory.Delete(lblDirectory.Text & "\_precede", True)
                        GoTo BackToCheckUpdates2
                    End If
                    If ApplyPrePatchYesNo = vbYes Then
                        WriteDebugInfo("Installing prepatch, please wait...")
                        Application.DoEvents()
                        Dim di As New DirectoryInfo(lblDirectory.Text & "\_precede\data\win32\")
                        Dim diar1 As FileInfo() = di.GetFiles()
                        Dim dra As FileInfo

                        'list the names of all files in the specified directory
                        Dim downloadstring As String = ""
                        Dim count As Integer = 0
                        Dim counter = My.Computer.FileSystem.GetFiles(lblDirectory.Text & "\_precede\data\win32\")
                        For Each dra In diar1
                            If counter.Count = 0 Then Exit For
                            downloadstring = dra.Name
                            DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
                            File.Move(lblDirectory.Text & "\_precede\data\win32\" & downloadstring, ((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
                            count += 1
                            lblStatus.Text = "Moved " & count & " files out of " & counter.Count
                            Application.DoEvents()
                        Next
                        WriteDebugInfoSameLine("Done!")
                        DeleteDirectory(lblDirectory.Text & "\_precede")
                    End If
                End If
            End If
BackToCheckUpdates2:
            If ComingFromPrePatch Then Exit Sub
            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("PSO2RemoteVersion")) Then
                Dim lines2 = File.ReadAllLines("version.ver")
                Dim RemoteVersion2 As String = lines2(0)
                Helper.SetRegKey(Of String)("PSO2RemoteVersion", RemoteVersion2)
            End If

            Dim lines = File.ReadAllLines("version.ver")
            Dim RemoteVersion As String = lines(0)
            If Helper.GetRegKey(Of String)("PSO2RemoteVersion") <> RemoteVersion Then
                UpdateNeeded = True

                Dim UpdateStoryYesNo As MsgBoxResult = MsgBox(My.Resources.strNewPSO2Update, vbYesNo)
                If UpdateStoryYesNo = vbNo Then UpdateNeeded = False
            End If

            If UpdateNeeded Then
                ButtonItem5.RaiseClick()
            End If
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub

    Private Sub btnApplyChanges_Click(sender As Object, e As EventArgs) Handles btnApplyChanges.Click
        Try
            If (Directory.Exists((lblDirectory.Text & "\data\win32")) = False OrElse lblDirectory.Text = "lblDirectory") Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                Button1.RaiseClick()
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
            If chkRemoveCensor.Checked AndAlso File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c")) Then
                If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c"), "ffbff2ac5b7a7948961212cefd4d402c.backup")
                WriteDebugInfoAndOK(My.Resources.strRemoving & "Censor...")
            ElseIf chkRemoveCensor.Checked AndAlso (Not File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c"))) Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRemoveCensor)
            End If
            'Restore Censor
            If chkRestoreCensor.Checked AndAlso File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup")) Then
                If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"), "ffbff2ac5b7a7948961212cefd4d402c")
                WriteDebugInfoAndOK(My.Resources.strRestoring & "Censor...")
            ElseIf chkRestoreCensor.Checked AndAlso (Not File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"))) Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRestoreCensor)
            End If
            'Remove PC Opening Video [Done]
            If chkRemovePC.Checked AndAlso File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) Then
                If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927"), "a44fbb2aeb8084c5a5fbe80e219a9927.backup")
                WriteDebugInfoAndOK(My.Resources.strRemoving & "PC Opening Video...")
            ElseIf chkRemovePC.Checked AndAlso (Not File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927"))) Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRemovePC)
            End If
            'Restore PC Opening Video [Done]
            If chkRestorePC.Checked AndAlso File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup")) Then
                If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup"), "a44fbb2aeb8084c5a5fbe80e219a9927")
                WriteDebugInfoAndOK(My.Resources.strRestoring & "PC Opening Video...")
            ElseIf chkRestorePC.Checked AndAlso (Not File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup"))) Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRestorePC)
            End If
            'Remove Vita Opening Video [Done]
            If chkRemoveVita.Checked AndAlso File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585")) Then
                If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585.backup")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585.backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585"), "a93adc766eb3510f7b5c279551a45585.backup")
                WriteDebugInfoAndOK(My.Resources.strRemoving & "Vita Opening Video...")
            ElseIf chkRemoveVita.Checked AndAlso (Not File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585"))) Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRemoveVita)
            End If
            'Restore Vita Opening Video [Done]
            If chkRestoreVita.Checked AndAlso File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585.backup")) Then
                If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585.backup"), "a93adc766eb3510f7b5c279551a45585")
                WriteDebugInfoAndOK(My.Resources.strRestoring & "Vita Opening Video...")
            ElseIf chkRestoreVita.Checked AndAlso (Not File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585.backup"))) Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRestoreVita)
            End If
            'Remove NVidia Opening Video [Done]
            If chkRemoveNVidia.Checked AndAlso File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "7f2368d207e104e8ed6086959b742c75")) Then
                If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "7f2368d207e104e8ed6086959b742c75.backup")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "7f2368d207e104e8ed6086959b742c75.backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "7f2368d207e104e8ed6086959b742c75"), "7f2368d207e104e8ed6086959b742c75.backup")
                WriteDebugInfoAndOK(My.Resources.strRemoving & "NVidia Opening Video...")
            ElseIf chkRemoveNVidia.Checked AndAlso (Not File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "7f2368d207e104e8ed6086959b742c75"))) Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRemoveNVidia)
            End If
            'Restore NVidia Opening Video [Done]
            If chkRestoreNVidia.Checked AndAlso File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "7f2368d207e104e8ed6086959b742c75.backup")) Then
                If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "7f2368d207e104e8ed6086959b742c75")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "7f2368d207e104e8ed6086959b742c75"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "7f2368d207e104e8ed6086959b742c75.backup"), "7f2368d207e104e8ed6086959b742c75")
                WriteDebugInfoAndOK(My.Resources.strRestoring & "NVidia Opening Video...")
            ElseIf chkRestoreNVidia.Checked AndAlso (Not File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "7f2368d207e104e8ed6086959b742c75.backup"))) Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRestoreNVidia)
            End If
            'Remove SEGA Opening Video [Done]
            If chkRemoveSEGA.Checked AndAlso File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "009bfec69b04a34576012d50e3417771")) Then
                If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "009bfec69b04a34576012d50e3417771.backup")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "009bfec69b04a34576012d50e3417771.backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "009bfec69b04a34576012d50e3417771"), "009bfec69b04a34576012d50e3417771.backup")
                WriteDebugInfoAndOK(My.Resources.strRemoving & "SEGA Opening Video...")
            ElseIf chkRemoveSEGA.Checked AndAlso (Not File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "009bfec69b04a34576012d50e3417771"))) Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRemoveSEGA)
            End If
            'Restore SEGA Opening Video [Done]
            If chkRestoreSEGA.Checked AndAlso File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "009bfec69b04a34576012d50e3417771.backup")) Then
                If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "009bfec69b04a34576012d50e3417771")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "009bfec69b04a34576012d50e3417771"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "009bfec69b04a34576012d50e3417771.backup"), "009bfec69b04a34576012d50e3417771")
                WriteDebugInfoAndOK(My.Resources.strRestoring & "SEGA Opening Video...")
            ElseIf chkRestoreSEGA.Checked AndAlso (Not File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "009bfec69b04a34576012d50e3417771.backup"))) Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRestoreSEGA)
            End If
            UnlockGUI()
            'Swap PC and Vita Openings
            'Restore PC Opening Video [Done]
            If chkSwapOP.Checked Then
                WriteDebugInfo(My.Resources.strSwappingOpenings)
                If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup")) Then
                    If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                    My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup"), "a44fbb2aeb8084c5a5fbe80e219a9927")
                    WriteDebugInfoAndOK(My.Resources.strRestoring & "PC Opening Video...")
                End If
                'Restore Vita Opening Video [Done]
                If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585.backup")) Then
                    If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                    My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585.backup"), "a93adc766eb3510f7b5c279551a45585")
                    WriteDebugInfoAndOK(My.Resources.strRestoring & "Vita Opening Video...")
                End If
                'Rename the original files
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927"), "PCOpening")
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585"), "VitaOpening")
                'Rename them back, swapping them~
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "PCOpening"), "a93adc766eb3510f7b5c279551a45585")
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "VitaOpening"), "a44fbb2aeb8084c5a5fbe80e219a9927")
            End If
            If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) AndAlso File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585")) Then
                If GetFileSize(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) = 167479840 AndAlso GetFileSize(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585")) = 151540352 Then
                    chkSwapOP.Text = My.Resources.strSwapPCVitaOpenings & "(" & My.Resources.strNotSwapped & ")"
                    WriteDebugInfo(My.Resources.strallDone)
                    UnlockGUI()
                    Exit Sub
                End If
                If GetFileSize(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) = 151540352 AndAlso GetFileSize(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585")) = 167479840 Then
                    chkSwapOP.Text = My.Resources.strSwapPCVitaOpenings & "(" & My.Resources.strSwapped & ")"
                    WriteDebugInfo(My.Resources.strallDone)
                    UnlockGUI()
                    Exit Sub
                End If
                chkSwapOP.Text = "Swap PC/Vita Openings (UNKNOWN)"
            End If
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub

    Private Sub btnLaunchPSO2_Click(sender As Object, e As EventArgs) Handles btnLaunchPSO2.Click
        'Fuck SEGA. Stupid jerks.
        Log("Check if PSO2 is running")
        If CheckIfRunning("pso2") = "Running" Then Exit Sub
        Try
            If (Directory.Exists((lblDirectory.Text & "\data\win32")) = False OrElse lblDirectory.Text = "lblDirectory") Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                Button1.RaiseClick()
                Exit Sub
            End If

            Dim DirectoryString As String = (lblDirectory.Text & "\data\win32")
            Dim pso2launchpath As String = DirectoryString.Replace("\data\win32", "")

            If Not File.Exists(pso2launchpath & "\pso2.exe") Then
                WriteDebugInfoAndFAILED(My.Resources.strCouldNotFindPSO2EXE)
                Exit Sub
            End If

            DLS.CancelAsync()
            Cancelled = True
            PB1.Value = 0
            PB1.Text = ""
            WriteDebugInfo(My.Resources.strLaunchingPSO2)

            Dim dir As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            DeleteFile(dir & "\SEGA\PHANTASYSTARONLINE2\item_name_cache.dat")

            If chkItemTranslation.Checked Then
                DLWUA("http://162.243.211.123/freedom/working.txt", "working.txt", True)
                Try
                    Dim sBuffer As String
                    Using oReader As StreamReader = File.OpenText("working.txt")
                        sBuffer = oReader.ReadLine
                        If sBuffer.ToString = "No" Then
                            MsgBox(My.Resources.strItemTranslationNotWorking)
                            Exit Sub
                        End If
                    End Using
                Catch ex As Exception
                    Log(ex.Message)
                    WriteDebugInfo(My.Resources.strERROR & ex.Message)
                End Try
                DeleteFile("working.txt")
                If Helper.GetMD5(pso2launchpath & "\translator.dll") <> Helper.GetRegKey(Of String)("DLLMD5") Then
                    MsgBox(My.Resources.strTranslationFilesDontMatch)
                    Exit Sub
                End If
            End If

            'End Item Translation stuff
            DeleteFile(pso2launchpath & "\ddraw.dll")
            File.WriteAllBytes(pso2launchpath & "\ddraw.dll", My.Resources.ddraw)
            Dim startInfo As ProcessStartInfo = New ProcessStartInfo With {.FileName = (pso2launchpath & "\pso2.exe"), .Arguments = "+0x33aca2b9", .UseShellExecute = False}
            startInfo.EnvironmentVariables("-pso2") = "+0x01e3f1e9"
            Dim shell As Process = New Process With {.StartInfo = startInfo}

            Try
                shell.Start()
            Catch ex As Exception
                WriteDebugInfo(My.Resources.strItSeemsThereWasAnError)
                DLWUA("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", "pso2.exe", True)
                Dim DirectoryString2 As String = (lblDirectory.Text & "\data\win32").Replace("\data\win32", "")
                If File.Exists((DirectoryString2 & "\pso2.exe")) AndAlso Application.StartupPath <> DirectoryString2 Then DeleteFile((DirectoryString2 & "\pso2.exe"))
                File.Move("pso2.exe", (DirectoryString2 & "\pso2.exe"))
                WriteDebugInfoSameLine(My.Resources.strDone)
                shell.Start()
            End Try

            Me.Hide()
            Dim hWnd As IntPtr = FindWindow("Phantasy Star Online 2", Nothing)

            Do Until hWnd <> IntPtr.Zero
                hWnd = FindWindow("Phantasy Star Online 2", Nothing)
                Thread.Sleep(10)
            Loop

            DeleteFile(pso2launchpath & "\ddraw.dll")
            Me.Close()

        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub

    Private Sub PB1_Click(sender As Object, e As MouseEventArgs) Handles PB1.Click
        If e.Button = MouseButtons.Right Then
            If DLS.IsBusy Then
                CancelDownloadToolStripMenuItem.Visible = True
                ContextMenuStrip1.Show(CType(sender, Control), e.Location)
            End If
            If Not DLS.IsBusy Then
                CancelDownloadToolStripMenuItem.Visible = False
                ContextMenuStrip1.Show(CType(sender, Control), e.Location)
            End If
        End If
    End Sub

    Private Sub CancelDownloadToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CancelDownloadToolStripMenuItem.Click
        DLS.CancelAsync()
        WriteDebugInfo(My.Resources.strDownloadwasCancelled)
        Cancelled = True
        PB1.Value = 0
        PB1.Text = ""
        lblStatus.Text = ""
        patching = False
    End Sub

    '    Private Function GrabLink(ByRef urlIdentifier As String)
    ' Dim net As New WebClient()
    ' Dim src As String = Net.DownloadString("http://psumods.co.uk/viewtopic.php?f=4&t=206")
    ' Dim regx As New Regex("http://([\w+?\.\w+])+([a-zA-Z0-9\~\!\@\#\$\%\^\&amp;\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?", RegexOptions.IgnoreCase)
    ' Dim mactches As MatchCollection = regx.Matches(src)
    ' Dim returnURL As String = ""
    '     For Each match As Match In mactches
    '         If match.ToString.Contains(urlIdentifier) Then returnURL = match.ToString
    '     Next
    '     If String.IsNullOrEmpty(returnURL) Then
    '         Return "Error! URL not found!"
    '         Exit Function
    '     End If
    '     Return returnURL
    'End Function

    Private Sub seconds_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles seconds.Tick
        Me.timer_start += 1
    End Sub

    Private Sub DocumentCompleted()
        ' TODO: Make function use SizeSuffix and whatever else needs doing
        Me.seconds.Stop()
        Me.time_for_download = timer_start * 10
        Me.velocity = testfile_Size / time_for_download * 1000
        WriteDebugInfoSameLine(" Done!")
        WriteDebugInfo(My.Resources.strYourDownloadSpeedIs & (Format(velocity, "0.0000") & " MB/s"))
        DoneDownloading = True
    End Sub

    Private Sub WebBrowser1_DocumentCompleted(ByVal sender As Object, ByVal e As WebBrowserDocumentCompletedEventArgs) Handles WebBrowser1.DocumentCompleted
        DocumentCompleted()
    End Sub

    Private Sub WebBrowser2_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles WebBrowser2.DocumentCompleted
        DocumentCompleted()
    End Sub

    Private Sub WebBrowser3_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles WebBrowser3.DocumentCompleted
        DocumentCompleted()
    End Sub

    Public Function Ping(ByVal server As String) As String
        'Switch is a class already, for what it is worth
        Dim Response As NetworkInformation.PingReply
        Dim SendPing As New NetworkInformation.Ping

        Try
            Response = SendPing.Send(server)
            If Response.Status = NetworkInformation.IPStatus.Success Then
                Return Response.RoundtripTime.ToString
            End If
        Catch ex As Exception
        End Try

        Return "ERROR"
    End Function

    Private Sub CancelProcessToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CancelProcessToolStripMenuItem.Click
        If DLS.IsBusy Then DLS.CancelAsync()
        Cancelled = True
        PB1.Value = 0
        PB1.Text = ""
        lblStatus.Text = ""
        patching = False
        CancelledFull = True
        WriteDebugInfo(My.Resources.strProcessWasCancelled)
        UnlockGUI()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            Log("Selecting PSO2 Directory...")
            Dim MyFolderBrowser As New FolderBrowserDialog
            ' Description that displays above the dialog box control. 
            If Not String.IsNullOrEmpty(lblDirectory.Text) Then MyFolderBrowser.SelectedPath = lblDirectory.Text
            MyFolderBrowser.Description = My.Resources.strSelectPSO2win32folder2
            ' Sets the root folder where the browsing starts from 
            MyFolderBrowser.RootFolder = Environment.SpecialFolder.MyComputer
            Dim dlgResult As DialogResult = MyFolderBrowser.ShowDialog()
            If dlgResult = DialogResult.Cancel Then
                WriteDebugInfo("pso2_bin folder selection cancelled!")
                Exit Sub
            End If

            If MyFolderBrowser.SelectedPath.Contains("\pso2_bin") Then
                If File.Exists(MyFolderBrowser.SelectedPath.Replace("\data\win32", "") & "\pso2.exe") Then
                    WriteDebugInfo("win32 folder selected instead of pso2_bin folder - Fixing!")
                    lblDirectory.Text = MyFolderBrowser.SelectedPath.Replace("\data\win32", "")
                    Helper.SetRegKey(Of String)("PSO2Dir", lblDirectory.Text)
                    WriteDebugInfoAndOK(lblDirectory.Text & " " & My.Resources.strSetAsYourPSO2)
                    Exit Sub
                End If
            End If

            Helper.SetRegKey(Of String)("PSO2Dir", MyFolderBrowser.SelectedPath)
            lblDirectory.Text = MyFolderBrowser.SelectedPath
            WriteDebugInfoAndOK(lblDirectory.Text & " " & My.Resources.strSetAsYourPSO2)

        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub btnLargeFiles_Click(sender As Object, e As EventArgs) Handles btnLargeFiles.Click
        ' Here we parse the text file before passing it to the DownloadPatch function.
        ' The Using statement will dispose "net" as soon as we're done with it.
        Using net As New WebClient()
            ' TODO: Check the string to make sure it's a valid URL before passing it into the function.
            ' If we decide not to, we can do away with "url" and just pass net.DownloadString in as the parameter.
            ' Furthermore, we could also parse it from within the function.
            Dim url As String = net.DownloadString("http://162.243.211.123/patches/largefiles.txt")
            DownloadPatch(url, "Large Files", "LargeFiles.rar", "LargeFilesVersion", My.Resources.strWouldYouLikeToBackupLargeFiles, My.Resources.strWouldYouLikeToUse, "backupPreLargeFiles")
        End Using
    End Sub

    Private Sub btnStory_Click(sender As Object, e As EventArgs) Handles btnStory.Click
        CancelledFull = False
        Dim StoryLocation As String
        Dim backupyesno As MsgBoxResult
        If (Directory.Exists((lblDirectory.Text & "\data\win32")) = False OrElse lblDirectory.Text = "lblDirectory") Then
            MsgBox(My.Resources.strPleaseSelectwin32Dir)
            Button1.RaiseClick()
            Exit Sub
        End If
        Log("Selecting story patch...")
        Dim Downloaded As MsgBoxResult = MsgBox(My.Resources.strHaveyouDownloadedStoryYet, MsgBoxStyle.YesNo)
        If Downloaded = MsgBoxResult.Yes Then
            OpenFileDialog1.Title = My.Resources.strPleaseSelectStoryRAR
            OpenFileDialog1.FileName = "PSO2 Story Patch RAR file"
            OpenFileDialog1.Filter = "RAR Archives|*.rar"
            Dim result = OpenFileDialog1.ShowDialog()
            If result = DialogResult.Cancel Then
                Exit Sub
            End If
            StoryLocation = OpenFileDialog1.FileName.ToString()
            If StoryLocation = "PSO2 Story Patch RAR file" Then
                Exit Sub
            End If

            Log("Story mode RAR selected as: " & StoryLocation)

            Select Case Helper.GetRegKey(Of String)("Backup")
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
            Dim process As Process = Nothing
            Dim processStartInfo As ProcessStartInfo = New ProcessStartInfo()
            Dim UnRarLocation As String = (Application.StartupPath & "\unrar.exe")
            processStartInfo.FileName = UnRarLocation
            processStartInfo.Verb = "runas"
            processStartInfo.Arguments = ("e " & """" & StoryLocation & """" & " TEMPSTORYAIDAFOOL")
            processStartInfo.WindowStyle = ProcessWindowStyle.Normal
            processStartInfo.UseShellExecute = True
            process = process.Start(processStartInfo)
            WriteDebugInfo(My.Resources.strWaitingforPatch)
            If CancelledFull Then Exit Sub
            Do Until process.WaitForExit(1000)
            Loop
            If CancelledFull Then Exit Sub
            If Not Directory.Exists("TEMPSTORYAIDAFOOL") Then
                Directory.CreateDirectory("TEMPSTORYAIDAFOOL")
                WriteDebugInfo("Had to manually make temp update folder - Did the patch not extract right?")
            End If
            Dim di As New DirectoryInfo("TEMPSTORYAIDAFOOL")
            Dim diar1 As FileInfo() = di.GetFiles()
            Dim dra As FileInfo
            WriteDebugInfoAndOK((My.Resources.strExtractingTo & (lblDirectory.Text & "\data\win32")))
            Application.DoEvents()
            'list the names of all files in the specified directory
            Dim backupdir As String = ((lblDirectory.Text & "\data\win32") & "\" & "backupPreSTORYPatch")
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
            Log("Extracted " & diar1.Count & " files from the patch")
            If diar1.Count = 0 Then
                WriteDebugInfo("Patch failed to extract correctly! Installation failed!")
                Exit Sub
            End If
            WriteDebugInfo(My.Resources.strInstallingPatch)
            For Each dra In diar1
                If CancelledFull Then Exit Sub
                If backupyesno = MsgBoxResult.Yes Then
                    If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString)) Then
                        File.Move(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString), (backupdir & "\" & dra.ToString))
                    End If
                End If
                If backupyesno = MsgBoxResult.No Then
                    If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString)) Then
                        DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
                    End If
                End If
                File.Move(("TEMPSTORYAIDAFOOL\" & dra.ToString), ((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
            Next
            My.Computer.FileSystem.DeleteDirectory("TEMPSTORYAIDAFOOL", FileIO.DeleteDirectoryOption.DeleteAllContents)
            If backupyesno = MsgBoxResult.No Then
                FlashWindow(Me.Handle, 1)
                'Story Patch 3-12-2014.rar
                Dim StoryPatchFilename As String = OpenFileDialog1.SafeFileName.Replace("Story Patch ", "").Replace(".rar", "").Replace("-", "/")
                Helper.SetRegKey(Of String)("StoryPatchVersion", StoryPatchFilename)
                Helper.SetRegKey(Of String)("LatestStoryBase", StoryPatchFilename)
                WriteDebugInfo(My.Resources.strStoryPatchInstalled)
                CheckForStoryUpdates()
            End If
            If backupyesno = MsgBoxResult.Yes Then
                FlashWindow(Me.Handle, 1)
                'Story Patch 3-12-2014.rar
                Dim StoryPatchFilename As String = OpenFileDialog1.SafeFileName.Replace("Story Patch ", "").Replace(".rar", "").Replace("-", "/")
                Helper.SetRegKey(Of String)("StoryPatchVersion", StoryPatchFilename)
                Helper.SetRegKey(Of String)("LatestStoryBase", StoryPatchFilename)
                WriteDebugInfo((My.Resources.strStoryPatchBackup & backupdir))
                CheckForStoryUpdates()
            End If
            Exit Sub
        End If
        If Downloaded = MsgBoxResult.No Then
            WriteDebugInfo(My.Resources.strDownloadStoryPatch)
            Exit Sub
        End If
    End Sub

    Private Sub chkItemTranslation_Click(sender As Object, e As EventArgs) Handles chkItemTranslation.Click
        If Not File.Exists(lblDirectory.Text & "\translation.cfg") Then
            File.WriteAllText(lblDirectory.Text & "\translation.cfg", "TranslationPath:translation.bin")
        End If
        If chkItemTranslation.Checked Then
            Dim DirectoryString As String = (lblDirectory.Text & "\data\win32")
            Dim pso2launchpath As String = DirectoryString.Replace("\data\win32", "")

            WriteDebugInfoAndOK(My.Resources.strDownloadingLatestItemTranslationFiles)
            'Download translator.dll and translation.bin
            Dim DLLink1 As String = "http://162.243.211.123/freedom/translator.dll"
            Dim DLLink2 As String = "http://162.243.211.123/freedom/translation.bin"
            Dim client As New WebClient
            Dim failednumbers As Integer = 0
DOWNLOADDLL2:
            Try
                client.DownloadFile(DLLink1, (pso2launchpath & "\translator.dll"))
            Catch ex As Exception
                failednumbers += 1
                If failednumbers = 4 Then
                    WriteDebugInfoAndWarning("Failed to download translation files! (" & ex.Message & ")")
                    Exit Try
                End If
                GoTo DOWNLOADDLL2
            End Try
            Helper.SetRegKey(Of String)("DLLMD5", Helper.GetMD5(pso2launchpath & "\translator.dll"))
            failednumbers = 0
DOWNLOADBIN2:
            Try
                client.DownloadFile(DLLink2, (pso2launchpath & "\translation.bin"))
            Catch ex As Exception
                failednumbers += 1
                If failednumbers = 4 Then
                    WriteDebugInfoAndWarning("Failed to download translation files! (" & ex.Message & ")")
                    Exit Try
                End If
                GoTo DOWNLOADBIN2
            End Try

            'Start the shitstorm
            Dim objReader As New StreamReader(lblDirectory.Text & "\translation.cfg")
            Dim CurrentLine As String = ""
            Dim BuiltFile As String = ""
            Do While objReader.Peek() <> -1

                CurrentLine = objReader.ReadLine()

                If CurrentLine.Contains("TranslationPath:") Then CurrentLine = "TranslationPath:translation.bin"

                BuiltFile &= CurrentLine & vbNewLine
            Loop
            objReader.Close()
            File.WriteAllText(lblDirectory.Text & "\translation.cfg", BuiltFile)
            WriteDebugInfoSameLine(My.Resources.strDone)
        End If

        If Not chkItemTranslation.Checked Then
            WriteDebugInfoAndOK(My.Resources.strDeletingItemCache)
            Dim dir As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            DeleteFile(dir & "\SEGA\PHANTASYSTARONLINE2\item_name_cache.dat")
            WriteDebugInfoSameLine(My.Resources.strDone)
            Dim objReader As New StreamReader(lblDirectory.Text & "\translation.cfg")
            Dim CurrentLine As String = ""
            Dim BuiltFile As String = ""

            Do While objReader.Peek() <> -1

                CurrentLine = objReader.ReadLine()

                If CurrentLine.Contains("TranslationPath:") Then CurrentLine = "TranslationPath:"

                BuiltFile &= CurrentLine & vbNewLine
            Loop

            objReader.Close()
            File.WriteAllText(lblDirectory.Text & "\translation.cfg", BuiltFile)
        End If

        UseItemTranslation = chkItemTranslation.Checked
        Helper.SetRegKey(Of String)("UseItemTranslation", UseItemTranslation)
    End Sub

    Private Sub ButtonItem5_Click(sender As Object, e As EventArgs) Handles ButtonItem5.Click
        CancelledFull = False

        If (Directory.Exists((lblDirectory.Text & "\data\win32")) = False OrElse lblDirectory.Text = "lblDirectory") Then
            MsgBox(My.Resources.strPleaseSelectwin32Dir)
            Button1.RaiseClick()
            Exit Sub
        End If

        Dim filedownloader As New WebClient()
        Dim sBuffer As String = Nothing
        Dim filename As String() = Nothing
        Dim truefilename As String = Nothing
        Dim missingfiles As New List(Of String)
        Dim filedownloader2 As New WebClient()
        Dim missingfiles2 As New List(Of String)
        Dim NumberofChecks As Integer = 0
        Dim MD5 As String() = Nothing
        Dim TrueMD5 As String = Nothing
        Dim PSO2EXEMD5 As String = "FUCK YOU DUDU"
        Dim totalfilesize As Long = 0
        Dim testfilesize As String()
        lblStatus.Text = ""

        If Directory.Exists(((lblDirectory.Text & "\data\win32") & "\backupPreENPatch")) Then
            WriteDebugInfo(My.Resources.strENBackupFound)
            Override = True
            btnRestoreENBackup.RaiseClick()
            Override = False
        End If

        If Directory.Exists(((lblDirectory.Text & "\data\win32") & "\backupPreLargeFiles")) Then
            WriteDebugInfo(My.Resources.strLFBackupFound)
            Override = True
            btnRestoreLargeFilesBackup.RaiseClick()
            Override = False
        End If

        If Directory.Exists(((lblDirectory.Text & "\data\win32") & "\backupPreSTORYPatch")) Then
            WriteDebugInfo(My.Resources.strStoryBackupFound)
            Override = True
            btnRestoreStoryBackup.RaiseClick()
            Override = False
        End If

        ' Why is the UI being disabled here, is there something I'm missing? -Matthew
        LockGUI()
        WriteDebugInfo(My.Resources.strDownloadingPatchFile1)

        Application.DoEvents()
        DLWUA("http://download.pso2.jp/patch_prod/patches/launcherlist.txt", "launcherlist.txt", True)
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strDownloadingPatchFile2)

        Application.DoEvents()
        DLWUA("http://download.pso2.jp/patch_prod/patches/patchlist.txt", "patchlist.txt", True)
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strDownloadingPatchFile3)

        Application.DoEvents()
        DLWUA("http://download.pso2.jp/patch_prod/patches_old/patchlist.txt", "patchlist_old.txt", True)
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strDownloadingPatchFile4)

        Application.DoEvents()
        Dim versionclient As New MyWebClient With {.timeout = 3000}
        versionclient.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")

        WriteDebugInfoSameLine(My.Resources.strDone)
        Application.DoEvents()
        UnlockGUI()

        ' Mike: No idea what you were doing here. Hope I didn't break anything.
        Dim result As MsgBoxResult = MsgBoxResult.No
        If Not ComingFromOldFiles Then
            Me.TopMost = False
            If chkAlwaysOnTop.Checked Then Me.TopMost = True
        End If

        If result = MsgBoxResult.Yes Or ComingFromOldFiles Then
            WriteDebugInfo(My.Resources.strCheckingforNewContent)
            NumberofChecks = 0
            missingfiles.Clear()

            Using oReader As StreamReader = File.OpenText("patchlist.txt")
                If CancelledFull Then Exit Sub
                While Not (oReader.EndOfStream)
                    If CancelledFull Then Exit Sub
                    sBuffer = oReader.ReadLine
                    filename = Regex.Split(sBuffer, ".pat")
                    truefilename = filename(0).Replace("data/win32/", "")
                    MD5 = filename(1).Split(vbTab)
                    TrueMD5 = MD5(2)
                    If truefilename <> "GameGuard.des" AndAlso truefilename <> "PSO2JP.ini" AndAlso truefilename <> "script/user_default.pso2" AndAlso truefilename <> "script/user_intel.pso2" Then
                        If truefilename = "pso2.exe" Then
                            PSO2EXEMD5 = TrueMD5
                            GoTo NextFile1
                        End If
                        If Not File.Exists(((lblDirectory.Text & "\data\win32") & "\" & truefilename)) Then
                            If VedaUnlocked Then WriteDebugInfo("DEBUG: The file " & truefilename & " is missing.")
                            missingfiles.Add(truefilename)
                            GoTo NEXTFILE1
                        End If
                        If Helper.GetMD5(((lblDirectory.Text & "\data\win32") & "\" & truefilename)) <> TrueMD5 Then
                            If VedaUnlocked Then WriteDebugInfo("DEBUG: The file " & truefilename & " must be redownloaded.")
                            missingfiles.Add(truefilename)
                            GoTo NEXTFILE1
                        End If
                    End If
NEXTFILE1:
                    NumberofChecks += 1
                    lblStatus.Text = (My.Resources.strCurrentlyCheckingFile & NumberofChecks & "")
                    Application.DoEvents()
                End While
            End Using

            Dim totaldownload As String = missingfiles.Count
            Dim downloaded As Long = 0
            Dim totaldownloaded As Long = 0
            patching = True
            DeleteFile("resume.txt")

            For Each downloadstring In missingfiles
                File.AppendAllText("resume.txt", (downloadstring & vbCrLf))
            Next

            For Each downloadstring In missingfiles
                If CancelledFull Then Exit Sub
                'Download the missing files:
                'WHAT THE FUCK IS GOING ON HERE?
                downloaded += 1
                totaldownloaded += totalsize2


                lblStatus.Text = My.Resources.strDownloading & "" & downloaded & "/" & totaldownload & " (" & Helper.SizeSuffix(totaldownloaded) & ")"

                Application.DoEvents()
                Cancelled = False
                DLWUA(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadstring & ".pat"), downloadstring, True)
                Dim info7 As New FileInfo(downloadstring)
                Dim length2 As Long
                If File.Exists(downloadstring) Then length2 = info7.Length
                If info7.Length = 0 Then
                    Log("File appears to be empty, trying to download from secondary SEGA server")
                    DLWUA(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring & ".pat"), downloadstring, True)
                End If

                If Cancelled Then Exit Sub
                DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
                File.Move(downloadstring, ((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
                If VedaUnlocked Then WriteDebugInfo("DEBUG: Downloaded and installed " & downloadstring & ".")
                Dim linesList As New List(Of String)(File.ReadAllLines("resume.txt"))

                'Remove the line to delete, e.g.
                linesList.Remove(downloadstring)

                File.WriteAllLines("resume.txt", linesList.ToArray())
                Application.DoEvents()
            Next

            patching = False
            If missingfiles.Count = 0 Then WriteDebugInfo(My.Resources.strYouAppearToBeUpToDate)
            Dim filedownloader3 As New WebClient()
            Dim DirectoryString As String = ((lblDirectory.Text & "\data\win32").Replace("\data\win32", "") & "\")
            WriteDebugInfo(My.Resources.strDownloading & "version file...")
            Application.DoEvents()
            Cancelled = False
            Dim versionclient2 As New MyWebClient With {.timeout = 3000}
            versionclient2.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")

            If Cancelled Then Exit Sub
            Dim DirectoryString2 As String = (lblDirectory.Text & "\data\win32").Replace("\data\win32", "")
            DeleteFile((Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
            File.Copy("version.ver", (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
            WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & "version file"))

            WriteDebugInfo(My.Resources.strDownloading & "pso2launcher.exe...")
            Application.DoEvents()
            Dim procs As Process() = Process.GetProcessesByName("pso2launcher")
            For Each proc As Process In procs
                If proc.MainWindowTitle = "PHANTASY STAR ONLINE 2" AndAlso proc.MainModule.ToString = "ProcessModule (pso2launcher.exe)" Then proc.Kill()
            Next
            DLWUA("http://download.pso2.jp/patch_prod/patches/pso2launcher.exe.pat", "pso2launcher.exe", True)
            If Cancelled Then Exit Sub
            If File.Exists((DirectoryString & "pso2launcher.exe")) AndAlso Application.StartupPath <> DirectoryString2 Then DeleteFile((DirectoryString & "pso2launcher.exe"))
            File.Move("pso2launcher.exe", (DirectoryString & "pso2launcher.exe"))
            WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & "pso2launcher.exe"))
            WriteDebugInfo(My.Resources.strDownloading & "pso2updater.exe...")
            Application.DoEvents()
            procs = Process.GetProcessesByName("pso2updater")
            For Each proc As Process In procs
                If proc.MainModule.ToString = "ProcessModule (pso2updater.exe)" Then proc.Kill()
            Next
            DLWUA("http://download.pso2.jp/patch_prod/patches/pso2updater.exe.pat", "pso2updater.exe", True)
            If Cancelled Then Exit Sub
            If File.Exists((DirectoryString & "pso2updater.exe")) AndAlso Application.StartupPath <> DirectoryString2 Then DeleteFile((DirectoryString & "pso2updater.exe"))
            File.Move("pso2updater.exe", (DirectoryString & "pso2updater.exe"))
            WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & "pso2updater.exe"))
            Application.DoEvents()

            WriteDebugInfo(My.Resources.strDownloading & "pso2.exe...")
            procs = Process.GetProcessesByName("pso2")
            For Each proc As Process In procs
                If proc.MainModule.ToString = "ProcessModule (pso2.exe)" Then proc.Kill()
            Next
            DLWUA("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", "pso2.exe", True)
            If Cancelled Then Exit Sub

            If File.Exists((DirectoryString & "pso2.exe")) AndAlso Application.StartupPath <> DirectoryString2 Then DeleteFile((DirectoryString & "pso2.exe"))
            File.Move("pso2.exe", (DirectoryString & "pso2.exe"))
            If CancelledFull Then Exit Sub
            WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & "pso2.exe"))
            Helper.SetRegKey(Of String)("StoryPatchVersion", "Not Installed")
            Helper.SetRegKey(Of String)("ENPatchVersion", "Not Installed")
            Helper.SetRegKey(Of String)("LargeFilesVersion", "Not Installed")
            Helper.SetRegKey(Of String)("PSO2PatchlistMD5", Helper.GetMD5("patchlist.txt"))
            WriteDebugInfo(My.Resources.strGameUpdatedVanilla)
            DeleteFile("resume.txt")
            Dim lines2 = File.ReadAllLines("version.ver")
            Dim RemoteVersion2 As String = lines2(0)
            Helper.SetRegKey(Of String)("PSO2RemoteVersion", RemoteVersion2)
            UnlockGUI()

            If Helper.GetRegKey(Of Boolean)("RemoveCensor") Then
                If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c"), "ffbff2ac5b7a7948961212cefd4d402c.backup")
                WriteDebugInfoAndOK(My.Resources.strRemoving & "Censor...")
            End If

            If Helper.GetRegKey(Of Boolean)("ENPatchAfterInstall") Then
                WriteDebugInfo(My.Resources.strAutoInstallingENPatch)
                btnENPatch.RaiseClick()
            End If

            If Helper.GetRegKey(Of Boolean)("LargeFilesAfterInstall") Then
                WriteDebugInfo(My.Resources.strAutoInstallingLF)
                btnLargeFiles.RaiseClick()
            End If

            If Helper.GetRegKey(Of Boolean)("StoryPatchAfterInstall") Then
                WriteDebugInfo(My.Resources.strAutoInstallingStoryPatch)
                btnStory.RaiseClick()
            End If

            WriteDebugInfoAndOK(My.Resources.strallDone)
            Exit Sub
        End If

        If result = MsgBoxResult.No Then
            ComingFromOldFiles = False
            If CancelledFull Then Exit Sub
            MergePatches()
            WriteDebugInfo(My.Resources.strCheckingforAllFiles)

            SOMEOFTHETHINGS.Remove("GameGuard.des")
            SOMEOFTHETHINGS.Remove("PSO2JP.ini")
            SOMEOFTHETHINGS.Remove("script/user_default.pso2")
            SOMEOFTHETHINGS.Remove("script/user_intel.pso2")
            SOMEOFTHETHINGS.Remove("")

            If SOMEOFTHETHINGS.ContainsKey("pso2.exe") Then
                Dim value = SOMEOFTHETHINGS("pso2.exe")
                PSO2EXEMD5 = value.Substring(value.LastIndexOf(vbTab) + 1)
                SOMEOFTHETHINGS.Remove("pso2.exe")
            End If

            Dim dataPath = lblDirectory.Text & "\data\win32\"
            Dim length = SOMEOFTHETHINGS.Count
            Dim oldmax = PB1.Maximum
            PB1.Maximum = length

            For Each kvp In SOMEOFTHETHINGS
                lblStatus.Text = (My.Resources.strCurrentlyCheckingFile & NumberofChecks)
                PB1.Text = NumberofChecks & " / " & length
                If (NumberofChecks Mod 8) = 0 Then Application.DoEvents()
                NumberofChecks += 1
                PB1.Value += 1

                Dim filePath = dataPath & kvp.Key

                If Not File.Exists(filePath) Then
                    If VedaUnlocked Then WriteDebugInfo("DEBUG: The file " & filePath & My.Resources.strIsMissing)
                    testfilesize = kvp.Value.Split(vbTab)
                    totalfilesize += Convert.ToInt32(testfilesize(1))
                    missingfiles2.Add(kvp.Key)
                    Continue For
                End If

                testfilesize = kvp.Value.Split(vbTab)
                Dim fileSizeZ = Convert.ToInt32(testfilesize(1))

                If GetFileSize(filePath) <> fileSizeZ Then
                    If VedaUnlocked Then WriteDebugInfo("DEBUG: The file " & kvp.Key & " must be redownloaded.")
                    totalfilesize += fileSizeZ
                    missingfiles2.Add(kvp.Key)
                    Continue For
                End If

                TrueMD5 = testfilesize(2)

                If Helper.GetMD5(filePath) <> TrueMD5 Then
                    If VedaUnlocked Then WriteDebugInfo("DEBUG: The file " & kvp.Key & " must be redownloaded.")
                    totalfilesize += fileSizeZ
                    missingfiles2.Add(kvp.Key)
                    Continue For
                End If
            Next

            PB1.Text = ""
            PB1.Value = 0
            PB1.Maximum = oldmax
            SOMEOFTHETHINGS = Nothing
            Dim totaldownload2 As String = missingfiles2.Count
            Dim downloaded2 As Long = 0
            Dim info As FileInfo
            Dim filesize As Long
            Dim totaldownloaded As Long = 0
            patching = True
            DeleteFile("resume.txt")
            For Each downloadstring In missingfiles2
                File.AppendAllText("resume.txt", (downloadstring & vbCrLf))
            Next
            For Each downloadstring In missingfiles2
                If CancelledFull Then Exit Sub
                'Download the missing files:
                'WHAT THE FUCK IS GOING ON HERE?
                downloaded2 += 1
                totaldownloaded += totalsize2

                lblStatus.Text = My.Resources.strDownloading & "" & downloaded2 & "/" & totaldownload2 & " (" & Helper.SizeSuffix(totaldownloaded) & " / " & Helper.SizeSuffix(totalfilesize) & ")"

                Application.DoEvents()
                DLWUA(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadstring & ".pat"), downloadstring, False)
                Dim info7 As New FileInfo(downloadstring)
                Dim length2 As Long
                If File.Exists(downloadstring) Then length2 = info7.Length
                If info7.Length = 0 Then
                    Log("File appears to be empty, trying to download from secondary SEGA server")
                    DLWUA(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring & ".pat"), downloadstring, True)
                End If
                info = New FileInfo(downloadstring)
                filesize = info.Length
                If filesize = 0 Then
                    DeleteFile(downloadstring)
                    DLWUA(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring & ".pat"), downloadstring, False)
                End If

                If File.Exists(downloadstring) Then
                    DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
                    File.Move(downloadstring, ((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
                    If VedaUnlocked Then WriteDebugInfo("DEBUG: Downloaded and installed " & downloadstring & ".")
                    Dim linesList As New List(Of String)(File.ReadAllLines("resume.txt"))

                    'Remove the line to delete, e.g.
                    linesList.Remove(downloadstring)

                    File.WriteAllLines("resume.txt", linesList.ToArray())
                    Application.DoEvents()
                Else
                    Application.DoEvents()
                End If
            Next
            patching = False
            If missingfiles.Count = 0 Then WriteDebugInfo(My.Resources.strYouAppearToBeUpToDate)
            Dim filedownloader3 As New WebClient()
            Dim DirectoryString As String = ((lblDirectory.Text & "\data\win32").Replace("\data\win32", "") & "\")
            WriteDebugInfo(My.Resources.strDownloading & "version file...")
            Application.DoEvents()
            Dim DirectoryString2 As String = (lblDirectory.Text & "\data\win32").Replace("\data\win32", "")
            Dim versionclient2 As New MyWebClient With {.timeout = 3000}
            versionclient2.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")

            DeleteFile((Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
            File.Copy("version.ver", (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
            WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & "version file"))

            WriteDebugInfo(My.Resources.strDownloading & "pso2launcher.exe...")
            Application.DoEvents()
            DLWUA("http://download.pso2.jp/patch_prod/patches/pso2launcher.exe.pat", "pso2launcher.exe", True)
            If File.Exists((DirectoryString & "pso2launcher.exe")) AndAlso Application.StartupPath <> DirectoryString2 Then DeleteFile((DirectoryString & "pso2launcher.exe"))
            File.Move("pso2launcher.exe", (DirectoryString & "pso2launcher.exe"))
            WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & "pso2launcher.exe"))
            WriteDebugInfo(My.Resources.strDownloading & "pso2updater.exe...")
            Application.DoEvents()
            DLWUA("http://download.pso2.jp/patch_prod/patches/pso2updater.exe.pat", "pso2updater.exe", True)
            If File.Exists((DirectoryString & "pso2updater.exe")) AndAlso Application.StartupPath <> DirectoryString2 Then DeleteFile((DirectoryString & "pso2updater.exe"))
            File.Move("pso2updater.exe", (DirectoryString & "pso2updater.exe"))
            WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & "pso2updater.exe"))
            Application.DoEvents()
            Application.DoEvents()

            WriteDebugInfo(My.Resources.strDownloading & "pso2.exe...")
            DLWUA("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", "pso2.exe", True)
            If Cancelled Then Exit Sub

            If File.Exists((DirectoryString & "pso2.exe")) AndAlso Application.StartupPath <> DirectoryString2 Then DeleteFile((DirectoryString & "pso2.exe"))
            File.Move("pso2.exe", (DirectoryString & "pso2.exe"))
            If CancelledFull Then Exit Sub
            WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & "pso2.exe"))

            Helper.SetRegKey(Of String)("StoryPatchVersion", "Not Installed")
            Helper.SetRegKey(Of String)("ENPatchVersion", "Not Installed")
            Helper.SetRegKey(Of String)("LargeFilesVersion", "Not Installed")
            Helper.SetRegKey(Of String)("PSO2PatchlistMD5", Helper.GetMD5("patchlist.txt"))
            WriteDebugInfo(My.Resources.strGameUpdatedVanilla)
            DeleteFile("resume.txt")
            Dim lines3 = File.ReadAllLines("version.ver")
            Dim RemoteVersion3 As String = lines3(0)
            Helper.SetRegKey(Of String)("PSO2RemoteVersion", RemoteVersion3)
            UnlockGUI()
            WriteDebugInfoAndOK(My.Resources.strallDone)
            Exit Sub
        End If
    End Sub

    Private Sub DeleteFile(path As String)
        Try
            File.Delete(path)
        Catch ex As Exception
            ' TODO: Aida put whatever you see fit here plz
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub btnRestoreENBackup_Click(sender As Object, e As EventArgs) Handles btnRestoreENBackup.Click
        Try
            If (Directory.Exists((lblDirectory.Text & "\data\win32")) = False OrElse lblDirectory.Text = "lblDirectory") Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                Button1.RaiseClick()
                Exit Sub
            End If
            Dim backupyesno As MsgBoxResult
            If Not Override Then
                backupyesno = MsgBox(My.Resources.strAreYouSureRestoreBackup, vbYesNo)
            End If
            If Override Then backupyesno = MsgBoxResult.Yes
            If backupyesno = MsgBoxResult.Yes Then
                Dim backupfolder As String = "backupPreENPatch"
                If Not Directory.Exists(((lblDirectory.Text & "\data\win32") & "\" & backupfolder)) Then
                    WriteDebugInfoAndFAILED(My.Resources.strCantFindBackupDirectory & ((lblDirectory.Text & "\data\win32") & "\" & backupfolder))
                    Exit Sub
                End If
                Dim di As New DirectoryInfo(((lblDirectory.Text & "\data\win32") & "\" & backupfolder))
                Dim diar1 As FileInfo() = di.GetFiles()
                WriteDebugInfoAndOK((My.Resources.strRestoringBackupTo & (lblDirectory.Text & "\data\win32")))
                Application.DoEvents()
                'list the names of all files in the specified directory
                Dim win32 As String = (lblDirectory.Text & "\data\win32")
                For Each dra As FileInfo In diar1
                    If File.Exists(win32 & "\" & dra.ToString) Then
                        DeleteFile(win32 & "\" & dra.ToString)
                    End If
                    File.Move((((lblDirectory.Text & "\data\win32") & "\" & backupfolder) & "\" & dra.ToString), (win32 & "\" & dra.ToString))
                Next
                My.Computer.FileSystem.DeleteDirectory(((lblDirectory.Text & "\data\win32") & "\" & backupfolder), FileIO.DeleteDirectoryOption.DeleteAllContents)
                FlashWindow(Me.Handle, 1)
                WriteDebugInfo(My.Resources.strBackupRestored)
                UnlockGUI()
            Else
                Exit Sub
            End If
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub

    Private Sub ButtonItem4_Click(sender As Object, e As EventArgs) Handles btnRestoreLargeFilesBackup.Click
        Try
            If (Directory.Exists((lblDirectory.Text & "\data\win32")) = False OrElse lblDirectory.Text = "lblDirectory") Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                Button1.RaiseClick()
                Exit Sub
            End If
            Dim backupyesno As MsgBoxResult
            If Not Override Then
                backupyesno = MsgBox(My.Resources.strAreYouSureRestoreBackup, vbYesNo)
            End If
            If Override Then backupyesno = MsgBoxResult.Yes
            If backupyesno = MsgBoxResult.Yes Then
                Dim backupfolder As String = "backupPreLargeFiles"
                If Not Directory.Exists(((lblDirectory.Text & "\data\win32") & "\" & backupfolder)) Then
                    WriteDebugInfoAndFAILED(My.Resources.strCantFindBackupDirectory & ((lblDirectory.Text & "\data\win32") & "\" & backupfolder))
                    Exit Sub
                End If
                Dim di As New DirectoryInfo(((lblDirectory.Text & "\data\win32") & "\" & backupfolder))
                Dim diar1 As FileInfo() = di.GetFiles()
                Dim dra As FileInfo
                WriteDebugInfoAndOK((My.Resources.strRestoringBackupTo & (lblDirectory.Text & "\data\win32")))
                Application.DoEvents()
                'list the names of all files in the specified directory
                Dim win32 As String = (lblDirectory.Text & "\data\win32")
                For Each dra In diar1
                    If File.Exists(win32 & "\" & dra.ToString) Then
                        DeleteFile(win32 & "\" & dra.ToString)
                    End If
                    File.Move((((lblDirectory.Text & "\data\win32") & "\" & backupfolder) & "\" & dra.ToString), (win32 & "\" & dra.ToString))
                Next
                My.Computer.FileSystem.DeleteDirectory(((lblDirectory.Text & "\data\win32") & "\" & backupfolder), FileIO.DeleteDirectoryOption.DeleteAllContents)
                WriteDebugInfo(My.Resources.strBackupRestored)
                UnlockGUI()
            Else
                Exit Sub
            End If
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub

    Private Sub btnRestoreJPNames_Click(sender As Object, e As EventArgs) Handles btnRestoreJPNames.Click
        'http://pso2.arghargh200.net/pso2/2013_06_12/ceffe0e2386e8d39f188358303a92a7d
        If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "ceffe0e2386e8d39f188358303a92a7d.backup")) Then
            If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "ceffe0e2386e8d39f188358303a92a7d")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "ceffe0e2386e8d39f188358303a92a7d"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
            My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "ceffe0e2386e8d39f188358303a92a7d.backup"), "ceffe0e2386e8d39f188358303a92a7d")
            WriteDebugInfoAndOK(My.Resources.strRestoring & " JP Names file...")
        Else
            WriteDebugInfoAndOK(My.Resources.strRestoreFailedPleaseReinstall)
        End If
    End Sub

    Private Sub btnRestoreJPETrials_Click(sender As Object, e As EventArgs) Handles btnRestoreJPETrials.Click
        'http://pso2.arghargh200.net/pso2/2013_06_12/057aa975bdd2b372fe092614b0f4399e
        If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "057aa975bdd2b372fe092614b0f4399e.backup")) Then
            If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "057aa975bdd2b372fe092614b0f4399e")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "057aa975bdd2b372fe092614b0f4399e"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
            My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "057aa975bdd2b372fe092614b0f4399e.backup"), "057aa975bdd2b372fe092614b0f4399e")
            WriteDebugInfoAndOK(My.Resources.strRestoring & " JP E-Trials file...")
        Else
            WriteDebugInfoAndOK(My.Resources.strRestoreFailedPleaseReinstall)
        End If
    End Sub

    Private Sub btnAnalyze_Click(sender As Object, e As EventArgs) Handles btnAnalyze.Click
        Dim DirectoryString As String = (lblDirectory.Text & "\data\win32")
        Dim pso2launchpath As String = DirectoryString.Replace("data\win32", "")
        WriteDebugInfo(My.Resources.strCheckingForNecessaryFiles)
        If Not File.Exists(pso2launchpath & "Gameguard.DES") Then WriteDebugInfoAndWarning("Missing GameGuard.DES file! " & My.Resources.strPleaseFixGG)
        If Not File.Exists(pso2launchpath & "pso2.exe") Then WriteDebugInfoAndWarning("Missing pso2.exe file! " & My.Resources.strPleaseFixPSO2EXEs)
        If Not File.Exists(pso2launchpath & "pso2launcher.exe") Then WriteDebugInfoAndWarning("Missing pso2launcher.exe file! " & My.Resources.strPleaseFixPSO2EXEs)
        If Not File.Exists(pso2launchpath & "pso2updater.exe") Then WriteDebugInfoAndWarning("Missing pso2updater.exe file! " & My.Resources.strPleaseFixPSO2EXEs)
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strCheckingForFolders)
        If Not Directory.Exists(pso2launchpath & "\Gameguard\") Then WriteDebugInfoAndWarning("Missing Gameguard folder! " & My.Resources.strPleaseFixGG)
        If Not Directory.Exists(pso2launchpath & "\data\") Then WriteDebugInfoAndWarning("Missing data folder! " & My.Resources.strPleaseReinstallOrCheck)
        If Not Directory.Exists(pso2launchpath & "\data\win32\") Then WriteDebugInfoAndWarning("Missing data\win32 folder! " & My.Resources.strPleaseReinstallOrCheck)
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strDoneTesting)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If (Directory.Exists((lblDirectory.Text & "\data\win32")) = False OrElse lblDirectory.Text = "lblDirectory") Then
            MsgBox(My.Resources.strPleaseSelectwin32Dir)
            Button1.RaiseClick()
            Exit Sub
        End If
        Dim filedownloader As New WebClient()
        Dim sBuffer As String
        Dim filename As String()
        Dim truefilename As String
        Dim missingfiles As New List(Of String)
        Dim filedownloader2 As New WebClient()
        Dim sBuffer2 As String
        Dim filename2 As String()
        Dim truefilename2 As String
        Dim missingfiles2 As New List(Of String)
        Dim missingfiles3 As New List(Of String)
        Dim NumberofChecks As Integer
        LockGUI()
        WriteDebugInfo(My.Resources.strDownloadingPatchFile1)
        Application.DoEvents()
        DLWUA("http://download.pso2.jp/patch_prod/patches/launcherlist.txt", "launcherlist.txt", True)
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strDownloadingPatchFile2)
        Application.DoEvents()
        DLWUA("http://download.pso2.jp/patch_prod/patches/patchlist.txt", "patchlist.txt", True)
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strDownloadingPatchFile3)
        Application.DoEvents()
        DLWUA("http://download.pso2.jp/patch_prod/patches_old/patchlist.txt", "patchlist_old.txt", True)
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strDownloadingPatchFile4)
        Application.DoEvents()
        Dim versionclient As New MyWebClient With {.timeout = 3000}
        versionclient.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
        WriteDebugInfoSameLine(My.Resources.strDone)
        Application.DoEvents()
        UnlockGUI()
        Log("Opening patch file list...")
        Using oReader As StreamReader = File.OpenText("patchlist.txt")
            If CancelledFull Then Exit Sub
            If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup")) Then
                If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c")) Then DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c"))
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"), "ffbff2ac5b7a7948961212cefd4d402c")
                WriteDebugInfoAndOK(My.Resources.strRestoring & "Censor...")
            End If
            If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup")) Then
                If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) Then DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927"))
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup"), "a44fbb2aeb8084c5a5fbe80e219a9927")
                WriteDebugInfoAndOK(My.Resources.strRestoring & "PC Opening...")
            End If
            If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "7f2368d207e104e8ed6086959b742c75.backup")) Then
                If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "7f2368d207e104e8ed6086959b742c75")) Then DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "7f2368d207e104e8ed6086959b742c75"))
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "7f2368d207e104e8ed6086959b742c75.backup"), "7f2368d207e104e8ed6086959b742c75")
                WriteDebugInfoAndOK(My.Resources.strRestoring & "NVidia Logo...")
            End If
            If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "009bfec69b04a34576012d50e3417771.backup")) Then
                If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "009bfec69b04a34576012d50e3417771")) Then DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "009bfec69b04a34576012d50e3417771"))
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "009bfec69b04a34576012d50e3417771.backup"), "009bfec69b04a34576012d50e3417771")
                WriteDebugInfoAndOK(My.Resources.strRestoring & "SEGA Logo...")
            End If
            If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585.backup")) Then
                If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585")) Then DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585"))
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585.backup"), "a93adc766eb3510f7b5c279551a45585")
                WriteDebugInfoAndOK(My.Resources.strRestoring & "Vita Opening...")
            End If
            WriteDebugInfo(My.Resources.strCheckingFiles)
            While Not (oReader.EndOfStream)
                If CancelledFull Then Exit Sub
                sBuffer = oReader.ReadLine
                filename = Regex.Split(sBuffer, ".pat")
                truefilename = filename(0).Replace("data/win32/", "")
                If truefilename <> "GameGuard.des" AndAlso truefilename <> "edition.txt" AndAlso truefilename <> "gameversion.ver" AndAlso truefilename <> "pso2.exe" AndAlso truefilename <> "PSO2JP.ini" AndAlso truefilename <> "script/user_default.pso2" AndAlso truefilename <> "script/user_intel.pso2" Then
                    Dim info7 As New FileInfo(((lblDirectory.Text & "\data\win32") & "\" & truefilename))
                    Dim length2 As Long
                    If File.Exists((lblDirectory.Text & "\data\win32") & "\" & truefilename) Then length2 = info7.Length
                    If Not File.Exists(((lblDirectory.Text & "\data\win32") & "\" & truefilename)) Then
                        WriteDebugInfo(truefilename & My.Resources.strIsMissing)
                        missingfiles.Add(truefilename)
                    End If
                    info7 = New FileInfo(((lblDirectory.Text & "\data\win32") & "\" & truefilename))
                    If File.Exists((lblDirectory.Text & "\data\win32") & "\" & truefilename) Then length2 = info7.Length
                    If Not File.Exists((lblDirectory.Text & "\data\win32") & "\" & truefilename) Then length2 = 1
                    If length2 = 0 Then
                        WriteDebugInfo(truefilename & " has a filesize of 0!")
                        missingfiles.Add(truefilename)
                        DeleteFile((lblDirectory.Text & "\data\win32") & "\" & truefilename)
                    End If
                End If
                NumberofChecks += 1
                lblStatus.Text = (My.Resources.strCurrentlyCheckingFile & NumberofChecks)
                Application.DoEvents()
            End While
        End Using
        Log("Opening Second patch file...")
        Using oReader As StreamReader = File.OpenText("patchlist_old.txt")
            While Not (oReader.EndOfStream)
                If CancelledFull Then Exit Sub
                sBuffer2 = oReader.ReadLine
                filename2 = Regex.Split(sBuffer2, ".pat")
                truefilename2 = filename2(0).Replace("data/win32/", "")
                If truefilename2 <> "GameGuard.des" AndAlso truefilename2 <> "pso2.exe" AndAlso truefilename2 <> "PSO2JP.ini" AndAlso truefilename2 <> "script/user_default.pso2" AndAlso truefilename2 <> "script/user_intel.pso2" Then
                    Dim info7 As New FileInfo(((lblDirectory.Text & "\data\win32") & "\" & truefilename2))
                    Dim length2 As Long
                    If File.Exists((lblDirectory.Text & "\data\win32") & "\" & truefilename2) Then length2 = info7.Length
                    If Not File.Exists(((lblDirectory.Text & "\data\win32") & "\" & truefilename2)) Then
                        If missingfiles.Contains(truefilename2) Then
                            'Do nothing
                        Else
                            WriteDebugInfo(truefilename2 & My.Resources.strIsMissing)
                            missingfiles2.Add(truefilename2)
                        End If
                    End If
                    info7 = New FileInfo(((lblDirectory.Text & "\data\win32") & "\" & truefilename2))
                    If File.Exists((lblDirectory.Text & "\data\win32") & "\" & truefilename2) Then length2 = info7.Length
                    If Not File.Exists((lblDirectory.Text & "\data\win32") & "\" & truefilename2) Then length2 = 1
                    If length2 = 0 Then
                        WriteDebugInfo(truefilename2 & " has a filesize of 0!")
                        missingfiles.Add(truefilename2)
                        DeleteFile((lblDirectory.Text & "\data\win32") & "\" & truefilename2)
                    End If
                End If
                NumberofChecks += 1
                lblStatus.Text = (My.Resources.strCurrentlyCheckingFile & NumberofChecks)
                Application.DoEvents()
            End While
            If missingfiles.Count = 0 AndAlso missingfiles2.Count = 0 Then
                WriteDebugInfoAndOK(My.Resources.strAllFilesCheckedOut)
                Exit Sub
            End If
        End Using

        Dim result1 As DialogResult = MessageBox.Show(My.Resources.strWouldYouLikeToDownloadInstallMissing, "Download/Install?", MessageBoxButtons.YesNo)

        If result1 = DialogResult.No Then Exit Sub

        If result1 = DialogResult.Yes Then
            Log(My.Resources.strDownloading & My.Resources.strMissingFilesPart1)
            Dim totaldownload As String = missingfiles.Count
            Dim downloaded As Long = 0
            Dim totaldownloaded As Long = 0
            DeleteFile("resume.txt")

            For Each downloadstring In missingfiles
                File.AppendAllText("resume.txt", (downloadstring & vbCrLf))
            Next

            For Each downloadstring In missingfiles
                'Download the missing files:
                Cancelled = False
                downloaded += 1
                totaldownloaded += totalsize2


                lblStatus.Text = My.Resources.strDownloading & "" & downloaded & "/" & totaldownload & " (" & Helper.SizeSuffix(totaldownloaded) & ")"

                DLWUA(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadstring & ".pat"), downloadstring, True)
                Dim info7 As New FileInfo(downloadstring)
                Dim length2 As Long
                If File.Exists(downloadstring) Then length2 = info7.Length
                If info7.Length = 0 Then
                    Log("File appears to be empty, trying to download from secondary SEGA server")
                    DLWUA(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring & ".pat"), downloadstring, True)
                End If
                If Cancelled Then Exit Sub
                File.Move(downloadstring, ((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
                WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & downloadstring & "."))
                Dim linesList As New List(Of String)(File.ReadAllLines("resume.txt"))

                'Remove the line to delete, e.g.
                linesList.Remove(downloadstring)

                File.WriteAllLines("resume.txt", linesList.ToArray())
                If CancelledFull Then Exit Sub
            Next

            Log(My.Resources.strDownloading & My.Resources.strMissingFilesPart2)

            DeleteFile("resume.txt")

            For Each downloadstring2 In missingfiles2
                File.AppendAllText("resume.txt", (downloadstring2 & vbCrLf))
            Next

            Dim totaldownload2 As String = missingfiles2.Count
            Dim downloaded2 As Long = 0
            Dim totaldownloaded2 As Long = 0

            For Each downloadstring2 In missingfiles2
                If CancelledFull Then Exit Sub
                'Download the missing files:
                If Not File.Exists(((lblDirectory.Text & "\data\win32") & "\" & downloadstring2)) Then
                    Cancelled = False
                    downloaded2 += 1
                    totaldownloaded2 += totalsize2


                    lblStatus.Text = My.Resources.strDownloading & "" & downloaded2 & "/" & totaldownload2 & " (" & Helper.SizeSuffix(totaldownloaded2) & ")"

                    DLWUA(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadstring2 & ".pat"), downloadstring2, True)
                    Dim info7 As New FileInfo(downloadstring2)
                    Dim length2 As Long
                    If File.Exists(downloadstring2) Then length2 = info7.Length
                    If info7.Length = 0 Then
                        Log("File appears to be empty, trying to download from secondary SEGA server")
                        DLWUA(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring2 & ".pat"), downloadstring2, True)
                    End If
                    If Cancelled Then Exit Sub
                    File.Move(downloadstring2, ((lblDirectory.Text & "\data\win32") & "\" & downloadstring2))
                    WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & downloadstring2 & "."))
                    Dim linesList As New List(Of String)(File.ReadAllLines("resume.txt"))

                    'Remove the line to delete, e.g.
                    linesList.Remove(downloadstring2)

                    File.WriteAllLines("resume.txt", linesList.ToArray())
                End If
            Next
        End If
        WriteDebugInfoAndOK(My.Resources.strallDone)
    End Sub

    Private Sub ButtonItem10_Click(sender As Object, e As EventArgs) Handles ButtonItem10.Click
        LockGUI()
        WriteDebugInfo(My.Resources.strDownloadingPatchFile1)
        Application.DoEvents()
        DLWUA("http://download.pso2.jp/patch_prod/patches/launcherlist.txt", "launcherlist.txt", True)
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strDownloadingPatchFile2)
        Application.DoEvents()
        DLWUA("http://download.pso2.jp/patch_prod/patches/patchlist.txt", "patchlist.txt", True)
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strDownloadingPatchFile3)
        Application.DoEvents()
        DLWUA("http://download.pso2.jp/patch_prod/patches_old/patchlist.txt", "patchlist_old.txt", True)
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strDownloadingPatchFile4)
        Application.DoEvents()
        Dim versionclient As New MyWebClient With {.timeout = 3000}
        versionclient.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
        'DLWUA("http://download.pso2.jp/patch_prod/patches/version.ver", "version.ver", True)
        WriteDebugInfoSameLine(My.Resources.strDone)
        Application.DoEvents()
        UnlockGUI()
        WriteDebugInfo(My.Resources.strCheckingForMissingOldFiles)
        ComingFromOldFiles = True
        CancelledFull = False
        ButtonItem5.RaiseClick()
    End Sub

    Private Sub btnConnection_Click(sender As Object, e As EventArgs) Handles btnConnection.Click
        ' ping download.pso2.jp
        ' ping gs016.pso2gs.net
        ' ping www.google.com
        ' network speed test
        WriteDebugInfo(My.Resources.strCheckingConnection)
        WriteDebugInfo(Ping("download.pso2.jp"))
        WriteDebugInfoSameLine("/" & Ping("download.pso2.jp"))
        WriteDebugInfoSameLine("/" & Ping("download.pso2.jp"))
        WriteDebugInfoSameLine("/" & Ping("download.pso2.jp"))
        WriteDebugInfo(My.Resources.strIfTheAboveNumbers)
        WriteDebugInfo(My.Resources.strTestingDownloadSpeeds)
        WriteDebugInfo(My.Resources.strDownloadingTestFile & testfile & "...")

        Me.timer_start = 0
        Me.WebBrowser1.Navigate(testfile)
        Me.seconds.Start()

        Dim query As ManagementObjectSearcher
        Dim Qc As ManagementObjectCollection
        Dim Oq As ObjectQuery
        Dim Ms As ManagementScope
        Dim Co As ConnectionOptions
        Dim Mo As ManagementObject
        Dim signalStrength As Double

        Try
            Co = New ConnectionOptions
            Ms = New ManagementScope("root\wmi")
            Oq = New ObjectQuery("SELECT * FROM MSNdis_80211_ReceivedSignalStrength Where active=true")
            query = New ManagementObjectSearcher(Ms, Oq)
            Qc = query.Get
            signalStrength = 0

            For Each Mo In query.Get
                signalStrength = Convert.ToDouble(Mo("Ndis80211ReceivedSignalStrength"))
            Next
        Catch exp As Exception
            ' Indicate no signal
            signalStrength = -1
        End Try

        If signalStrength <> -1 Then
            WriteDebugInfo("Current Signal Strength is: " & signalStrength.ToString)
            WriteDebugInfo("Any strength below 60 may be an issue!")
        End If
    End Sub

    Private Sub btnGameguard_Click(sender As Object, e As EventArgs) Handles btnGameguard.Click
        Try
            Dim systempath As String
            MsgBox(My.Resources.strPleaseBeAwareGG)
            Dim DirectoryString As String = (lblDirectory.Text & "\data\win32")
            Dim pso2launchpath As String = DirectoryString.Replace("\data\win32", "")

            If Directory.Exists(pso2launchpath & "\Gameguard\") Then
                WriteDebugInfo("Removing Gameguard Directory...")
                Directory.Delete(pso2launchpath & "\Gameguard\", True)
                WriteDebugInfoSameLine(My.Resources.strDone)
            End If
            If File.Exists(pso2launchpath & "\GameGuard.des") Then
                WriteDebugInfo("Removing Gameguard File...")
                DeleteFile(pso2launchpath & "\GameGuard.des")
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
            DLWUA("http://download.pso2.jp/patch_prod/patches/GameGuard.des.pat", "GameGuard.des", True)
            WriteDebugInfo("Downloading Latest Gameguard config...")
            DLWUA("http://download.pso2.jp/patch_prod/patches/PSO2JP.ini.pat", pso2launchpath & "\PSO2JP.ini", True)
            WriteDebugInfoSameLine(My.Resources.strDone)
            File.Move("GameGuard.des", pso2launchpath & "\GameGuard.des")

            Dim foundKey As RegistryKey = My.Computer.Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Services\npggsvc", True)

            If foundKey Is Nothing Then
                WriteDebugInfo("No registry keys to delete. This is OK, they should be created the next time Gameguard launches.")
            Else
                WriteDebugInfo("Deleting Gameguard registry keys...")
                foundKey = My.Computer.Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Services", True)
                foundKey.DeleteSubKeyTree("npggsvc")
                WriteDebugInfoSameLine(My.Resources.strDone)
            End If
            WriteDebugInfoAndOK(My.Resources.strGGReset)
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Try
            If (Directory.Exists((lblDirectory.Text & "\data\win32")) = False OrElse lblDirectory.Text = "lblDirectory") Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                Button1.RaiseClick()
                Exit Sub
            End If
            Dim filedownloader As New WebClient()
            Dim DirectoryString As String = ((lblDirectory.Text & "\data\win32").Replace("\data\win32", "") & "\")
            Cancelled = False
            WriteDebugInfo(My.Resources.strDownloading & "pso2launcher.exe...")

            Application.DoEvents()
            Dim procs As Process() = Process.GetProcessesByName("pso2launcher")

            For Each proc As Process In procs
                If proc.MainWindowTitle = "PHANTASY STAR ONLINE 2" AndAlso proc.MainModule.ToString = "ProcessModule (pso2launcher.exe)" Then proc.Kill()
            Next

            DLWUA("http://download.pso2.jp/patch_prod/patches/pso2launcher.exe.pat", "pso2launcher.exe", True)
            If Cancelled Then Exit Sub
            Dim DirectoryString2 As String = (lblDirectory.Text & "\data\win32").Replace("\data\win32", "")
            If File.Exists((DirectoryString & "pso2launcher.exe")) AndAlso Application.StartupPath <> DirectoryString2 Then DeleteFile((DirectoryString & "pso2launcher.exe"))
            File.Move("pso2launcher.exe", (DirectoryString & "pso2launcher.exe"))
            WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & "pso2launcher.exe"))
            WriteDebugInfo(My.Resources.strDownloading & "pso2updater.exe...")
            Application.DoEvents()
            procs = Process.GetProcessesByName("pso2updater")
            For Each proc As Process In procs
                If proc.MainModule.ToString = "ProcessModule (pso2updater.exe)" Then proc.Kill()
            Next
            DLWUA("http://download.pso2.jp/patch_prod/patches/pso2updater.exe.pat", "pso2updater.exe", True)
            If Cancelled Then Exit Sub
            DirectoryString2 = (lblDirectory.Text & "\data\win32")
            DirectoryString2 = DirectoryString2.Replace("\data\win32", "")
            If File.Exists((DirectoryString & "pso2updater.exe")) AndAlso Application.StartupPath <> DirectoryString2 Then DeleteFile((DirectoryString & "pso2updater.exe"))
            File.Move("pso2updater.exe", (DirectoryString & "pso2updater.exe"))
            WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & "pso2updater.exe"))
            WriteDebugInfo(My.Resources.strDownloading & "pso2.exe...")
            Application.DoEvents()
            procs = Process.GetProcessesByName("pso2")
            For Each proc As Process In procs
                If proc.MainModule.ToString = "ProcessModule (pso2.exe)" Then proc.Kill()
            Next
            DLWUA("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", "pso2.exe", True)
            If Cancelled Then Exit Sub
            DirectoryString2 = (lblDirectory.Text & "\data\win32")
            DirectoryString2 = DirectoryString2.Replace("\data\win32", "")
            If File.Exists((DirectoryString & "pso2.exe")) AndAlso Application.StartupPath <> DirectoryString2 Then DeleteFile((DirectoryString & "pso2.exe"))
            File.Move("pso2.exe", (DirectoryString & "pso2.exe"))
            WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & "pso2.exe"))
            Application.DoEvents()
            WriteDebugInfo(My.Resources.strAllNecessaryFiles)
            UnlockGUI()
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub

    Private Sub btnRestoreStoryBackup_Click(sender As Object, e As EventArgs) Handles btnRestoreStoryBackup.Click
        Try
            If (Directory.Exists((lblDirectory.Text & "\data\win32")) = False OrElse lblDirectory.Text = "lblDirectory") Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                Button1.RaiseClick()
                Exit Sub
            End If
            Dim backupyesno As MsgBoxResult
            If Not Override Then
                backupyesno = MsgBox(My.Resources.strAreYouSureRestoreBackup, vbYesNo)
            End If
            If Override Then backupyesno = MsgBoxResult.Yes
            If backupyesno = MsgBoxResult.Yes Then
                Dim backupfolder As String = "backupPreSTORYPatch"
                If Not Directory.Exists(((lblDirectory.Text & "\data\win32") & "\" & backupfolder)) Then
                    WriteDebugInfoAndFAILED(My.Resources.strCantFindBackupDirectory & ((lblDirectory.Text & "\data\win32") & "\" & backupfolder))
                    Exit Sub
                End If
                Dim di As New DirectoryInfo(((lblDirectory.Text & "\data\win32") & "\" & backupfolder))
                Dim diar1 As FileInfo() = di.GetFiles()
                Dim dra As FileInfo
                WriteDebugInfoAndOK((My.Resources.strRestoringBackupTo & (lblDirectory.Text & "\data\win32")))
                Application.DoEvents()
                'list the names of all files in the specified directory
                Dim win32 As String = (lblDirectory.Text & "\data\win32")
                For Each dra In diar1
                    If File.Exists(win32 & "\" & dra.ToString) Then
                        DeleteFile(win32 & "\" & dra.ToString)
                    End If
                    File.Move((((lblDirectory.Text & "\data\win32") & "\" & backupfolder) & "\" & dra.ToString), (win32 & "\" & dra.ToString))
                Next
                My.Computer.FileSystem.DeleteDirectory(((lblDirectory.Text & "\data\win32") & "\" & backupfolder), FileIO.DeleteDirectoryOption.DeleteAllContents)
                WriteDebugInfo(My.Resources.strBackupRestored)
                UnlockGUI()
            Else
                Exit Sub
            End If
        Catch ex As Exception
            Log(ex.Message)
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
            Dim DirectoryString As String = ((lblDirectory.Text & "\data\win32").Replace("\data\win32", "") & "\")
            'MsgBox("cacls.exe" & (DirectoryString & "pso2.exe") & " /e /g """ & SystemInformation.UserName & """:F")
            WriteDebugInfo(My.Resources.strFixingPermissionsFor & "pso2.exe...")
            Application.DoEvents()
            Process.Start("cacls.exe", (DirectoryString & "pso2.exe") & " /e /g """ & SystemInformation.UserName & """:F")
            WriteDebugInfoSameLine(My.Resources.strDone)
            WriteDebugInfo(My.Resources.strFixingPermissionsFor & "pso2launcher.exe...")
            Application.DoEvents()
            Process.Start("cacls.exe", (DirectoryString & "pso2launcher.exe") & " /e /g """ & SystemInformation.UserName & """:F")
            WriteDebugInfoSameLine(My.Resources.strDone)
            WriteDebugInfo(My.Resources.strFixingPermissionsFor & "pso2download.exe...")
            Application.DoEvents()
            Process.Start("cacls.exe", (DirectoryString & "pso2download.exe") & " /e /g """ & SystemInformation.UserName & """:F")
            WriteDebugInfoSameLine(My.Resources.strDone)
            WriteDebugInfo(My.Resources.strFixingPermissionsFor & "pso2updater.exe...")
            Application.DoEvents()
            Process.Start("cacls.exe", (DirectoryString & "pso2updater.exe") & " /e /g """ & SystemInformation.UserName & """:F")
            WriteDebugInfoSameLine(My.Resources.strDone)
            WriteDebugInfo(My.Resources.strFixingPermissionsFor & "pso2predownload.exe...")
            Application.DoEvents()
            Process.Start("cacls.exe", (DirectoryString & "pso2predownload.exe") & " /e /g """ & SystemInformation.UserName & """:F")
            WriteDebugInfoSameLine(My.Resources.strDone)
            MsgBox(My.Resources.strFixPermissionsDone)
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub ButtonItem17_Click(sender As Object, e As EventArgs) Handles ButtonItem17.Click
        Dim whatthefuck As MsgBoxResult = MsgBox(My.Resources.strAreYouSureResetPSO2Settings, MsgBoxStyle.YesNo)
        If whatthefuck = MsgBoxResult.Yes Then
            Dim Documents As String = (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\")
            Dim usersettingsfile As String = (Documents & "SEGA\PHANTASYSTARONLINE2\user.pso2")
            File.WriteAllText(usersettingsfile, txtPSO2DefaultINI.Text)
            WriteDebugInfoAndOK(My.Resources.strPSO2SettingsReset)
        End If
    End Sub

    Private Sub ButtonItem12_Click(sender As Object, e As EventArgs) Handles ButtonItem12.Click
        ' ping download.pso2.jp
        ' ping gs016.pso2gs.net
        ' ping www.google.com
        ' network speed test
        WriteDebugInfo("Testing download speeds for Patch Server #1 (Japan)...")
        WriteDebugInfo("Downloading Disko Warp x Pump It Up Pro 2 Official Soundtrack Sampler...")
        Me.timer_start = 0
        Me.WebBrowser1.Navigate("http://socket-hack.com:2312/Disko%20Warp%20x%20Pump%20It%20Up%20Pro%202%20Official%20Soundtrack%20Sampler.mp3")
        Me.seconds.Start()
        Do While WebBrowser1.ReadyState <> WebBrowserReadyState.Complete
            Application.DoEvents()
        Loop
        WriteDebugInfo("Testing download speeds for Patch Server #2 (North America)...")
        WriteDebugInfo("Downloading Disko Warp x Pump It Up Pro 2 Official Soundtrack Sampler...")
        Me.timer_start = 0
        Me.WebBrowser2.Navigate("http://pso2.aeongames.net/Disko%20Warp%20x%20Pump%20It%20Up%20Pro%202%20Official%20Soundtrack%20Sampler(1).mp3")
        Me.seconds.Start()
    End Sub

    Private Sub btnTerminate_Click(sender As Object, e As EventArgs) Handles btnTerminate.Click
        WriteDebugInfo(My.Resources.strTerminatePSO2)
        Dim procs As Process() = Process.GetProcessesByName("pso2")
        For Each proc As Process In procs
            If proc.MainModule.ToString = "ProcessModule (pso2.exe)" Then proc.Kill()
            If proc.MainModule.ToString = "ProcessModule (GameMon.des)" Then proc.Kill()
            If proc.MainModule.ToString = "ProcessModule (GameMon64.des)" Then proc.Kill()
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
        Process.Start("http://pso2.jp")
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
        If Me.Visible Then
            If chkAlwaysOnTop.Checked Then
                frmOptions.TopMost = True
                Me.TopMost = True
                Helper.SetRegKey(Of String)("AlwaysOnTop", "True")
            End If
            If Not chkAlwaysOnTop.Checked Then
                frmOptions.TopMost = False
                Me.TopMost = False
                Helper.SetRegKey(Of String)("AlwaysOnTop", "False")
            End If
        End If
    End Sub

    Private Sub btnPSO2Options_Click(sender As Object, e As EventArgs) Handles btnPSO2Options.Click
        Cursor = Cursors.WaitCursor
        Try
            frmPSO2Options.TopMost = Me.TopMost
            frmPSO2Options.Top += 50
            frmPSO2Options.Left += 50
            frmPSO2Options.ShowDialog()
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub btnOptions_Click(sender As Object, e As EventArgs) Handles btnOptions.Click
        Cursor = Cursors.WaitCursor
        Try
            frmOptions.CMBStyle.SelectedIndex = -1
            frmOptions.TopMost = Me.TopMost
            frmOptions.Bounds = Me.Bounds
            frmOptions.Top += 50
            frmOptions.Left += 50
            frmOptions.ShowDialog()
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub btnENPatch_Click(sender As Object, e As EventArgs) Handles btnENPatch.Click
        ' Here we parse the text file before passing it to the DownloadPatch function.
        ' The Using statement will dispose "net" as soon as we're done with it.
        Using net As New WebClient()
            ' TODO: Check the string to make sure it's a valid URL before passing it into the function.
            ' If we decide not to, we can do away with "url" and just pass net.DownloadString in as the parameter.
            ' Furthermore, we could also parse it from within the function.
            Dim url As String = net.DownloadString("http://162.243.211.123/patches/enpatch.txt")
            DownloadPatch(url, "EN Patch", "ENPatch.rar", "ENPatchVersion", My.Resources.strBackupEN, My.Resources.strPleaseSelectPreDownloadENRAR, "backupPreENPatch")
        End Using
    End Sub

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Me.Close()
    End Sub

    Private Sub btnEXPFULL_Click(sender As Object, e As EventArgs) Handles btnEXPFULL.Click
        Process.Start("http://www.expfull.com/chat")
    End Sub

    Private Sub btnAnnouncements_Click(sender As Object, e As EventArgs) Handles btnAnnouncements.Click
        If DPISetting = 96 Then
            If Me.Width = 420 Then
                Me.Width = 796
                btnAnnouncements.Text = "<"
                If Helper.GetRegKey(Of String)("SidebarEnabled") = "False" Then
                    WriteDebugInfo(My.Resources.strLoadingSidebarPage)
                    Dim t1 As New Thread(AddressOf LoadSidebar) With {.IsBackground = True}
                    t1.Start()
                End If
                Exit Sub
            End If
            If Me.Width = 796 Then
                Me.Width = 420
                btnAnnouncements.Text = ">"
                Exit Sub
            End If
        End If
        If DPISetting = 120 Then
            If Me.Width = 560 Then
                Me.Width = 1060
                btnAnnouncements.Text = "<"
                If Helper.GetRegKey(Of String)("SidebarEnabled") = "False" Then
                    WriteDebugInfo(My.Resources.strLoadingSidebarPage)
                    Dim t1 As New Thread(AddressOf LoadSidebar) With {.IsBackground = True}
                    t1.Start()
                End If
                Exit Sub
            End If
            If Me.Width = 1060 Then
                Me.Width = 560
                btnAnnouncements.Text = ">"
                Exit Sub
            End If
        End If
    End Sub

    Private Sub WebBrowser4_Navigating(sender As Object, e As WebBrowserNavigatingEventArgs) Handles WebBrowser4.Navigating
        If Me.Visible Then
            If e.Url.ToString <> "http://162.243.211.123/freedom/tweaker.html" Then
                Process.Start(e.Url.ToString())
                Dim t1 As New Thread(AddressOf LoadSidebar) With {.IsBackground = True}
                t1.Start()
            End If
        End If
    End Sub
    ' TODO: Uninstall function
    Private Sub btnUninstallENPatch_Click(sender As Object, e As EventArgs) Handles btnUninstallENPatch.Click
        Try
            If (Directory.Exists((lblDirectory.Text & "\data\win32")) = False OrElse lblDirectory.Text = "lblDirectory") Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                Button1.RaiseClick()
                Exit Sub
            End If

            DLWUA("http://162.243.211.123/patches/enpatchfilelist.txt", "enpatchfilelist.txt", True)

            Dim oReader As StreamReader = File.OpenText("enpatchfilelist.txt")
            Dim sBuffer As String = Nothing
            Dim filename As String = Nothing
            Dim missingfiles As New List(Of String)
            Dim NumberofChecks As Integer = 0

            While Not (oReader.EndOfStream)
                sBuffer = oReader.ReadLine
                filename = sBuffer
                missingfiles.Add(filename)
                NumberofChecks += 1
            End While

            oReader.Close()
            DeleteFile("enpatchfilelist.txt")

            WriteDebugInfo(My.Resources.strUninstallingPatch)
            Dim totaldownload As String = missingfiles.Count
            Dim downloaded As Long = 0

            For Each downloadstring In missingfiles
                downloaded += 1
                If CancelledFull Then Exit Sub

                'Download JP file
                lblStatus.Text = My.Resources.strUninstalling & downloaded & "/" & totaldownload
                DLWUA(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadstring & ".pat"), downloadstring, True)
                Dim info7 As New FileInfo(downloadstring)
                If info7.Length = 0 Then DLWUA(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring & ".pat"), downloadstring, True)

                'Move JP file to win32
                DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
                File.Move(downloadstring, ((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
            Next

            If My.Computer.FileSystem.DirectoryExists((lblDirectory.Text & "\data\win32") & "\" & "backupPreENPatch") Then My.Computer.FileSystem.DeleteDirectory(((lblDirectory.Text & "\data\win32") & "\" & "backupPreENPatch"), FileIO.DeleteDirectoryOption.DeleteAllContents)
            FlashWindow(Me.Handle, 1)
            WriteDebugInfo(My.Resources.strENPatchUninstalled)
            Helper.SetRegKey(Of String)("ENPatchVersion", "Not Installed")
            UnlockGUI()
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub
    ' TODO: Uninstall function
    Private Sub btnUninstallLargeFiles_Click(sender As Object, e As EventArgs) Handles btnUninstallLargeFiles.Click
        Try
            If (Directory.Exists((lblDirectory.Text & "\data\win32")) = False OrElse lblDirectory.Text = "lblDirectory") Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                Button1.RaiseClick()
                Exit Sub
            End If

            DLWUA("http://162.243.211.123/patches/largefilelist.txt", "largefilelist.txt", True)

            Dim oReader As StreamReader = File.OpenText("largefilelist.txt")
            Dim sBuffer As String = Nothing
            Dim filename As String = Nothing
            Dim missingfiles As New List(Of String)
            Dim NumberofChecks As Integer = 0

            While Not (oReader.EndOfStream)
                sBuffer = oReader.ReadLine
                filename = sBuffer
                missingfiles.Add(filename)
                NumberofChecks += 1
            End While

            oReader.Close()
            DeleteFile("largefilelist.txt")

            WriteDebugInfo(My.Resources.strUninstallingPatch)
            Dim totaldownload As String = missingfiles.Count
            Dim downloaded As Long = 0
            For Each downloadstring In missingfiles
                downloaded += 1

                If CancelledFull Then Exit Sub
                'Download JP file
                lblStatus.Text = My.Resources.strUninstalling & downloaded & "/" & totaldownload
                DLWUA(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadstring & ".pat"), downloadstring, True)
                Dim info7 As New FileInfo(downloadstring)
                If info7.Length = 0 Then DLWUA(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring & ".pat"), downloadstring, True)

                'Move JP file to win32
                DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
                File.Move(downloadstring, ((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
            Next

            If My.Computer.FileSystem.DirectoryExists((lblDirectory.Text & "\data\win32") & "\" & "backupPreLargeFiles") Then My.Computer.FileSystem.DeleteDirectory(((lblDirectory.Text & "\data\win32") & "\" & "backupPreLargeFiles"), FileIO.DeleteDirectoryOption.DeleteAllContents)
            FlashWindow(Me.Handle, 1)
            WriteDebugInfo(My.Resources.strLFUninstalled)
            Helper.SetRegKey(Of String)("LargeFilesVersion", "Not Installed")
            UnlockGUI()
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub
    ' TODO: Uninstall function
    Private Sub btnUninstallStory_Click(sender As Object, e As EventArgs) Handles btnUninstallStory.Click
        Try
            If (Directory.Exists((lblDirectory.Text & "\data\win32")) = False OrElse lblDirectory.Text = "lblDirectory") Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                Button1.RaiseClick()
                Exit Sub
            End If

            DLWUA("http://162.243.211.123/patches/storyfilelist.txt", "storyfilelist.txt", True)

            Dim oReader As StreamReader = File.OpenText("storyfilelist.txt")
            Dim sBuffer As String = Nothing
            Dim filename As String = Nothing
            Dim missingfiles As New List(Of String)
            Dim NumberofChecks As Integer = 0

            While Not (oReader.EndOfStream)
                sBuffer = oReader.ReadLine
                filename = sBuffer
                missingfiles.Add(filename)
                NumberofChecks += 1
            End While

            oReader.Close()
            DeleteFile("storyfilelist.txt")

            WriteDebugInfo(My.Resources.strUninstallingPatch)
            Dim totaldownload As String = missingfiles.Count
            Dim downloaded As Long = 0
            For Each downloadstring In missingfiles
                downloaded += 1
                If CancelledFull Then Exit Sub

                'Download JP file
                lblStatus.Text = My.Resources.strUninstalling & downloaded & "/" & totaldownload
                DLWUA(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadstring & ".pat"), downloadstring, True)
                Dim info7 As New FileInfo(downloadstring)
                If info7.Length = 0 Then DLWUA(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring & ".pat"), downloadstring, True)
                'Move JP file to win32
                DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
                File.Move(downloadstring, ((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
            Next

            If My.Computer.FileSystem.DirectoryExists((lblDirectory.Text & "\data\win32") & "\" & "backupPreSTORYPatch") Then My.Computer.FileSystem.DeleteDirectory(((lblDirectory.Text & "\data\win32") & "\" & "backupPreSTORYPatch"), FileIO.DeleteDirectoryOption.DeleteAllContents)
            FlashWindow(Me.Handle, 1)
            WriteDebugInfo(My.Resources.strStoryPatchUninstalled)
            Helper.SetRegKey(Of String)("StoryPatchVersion", "Not Installed")
            UnlockGUI()
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub

    Private Sub btnRussianPatch_Click(sender As Object, e As EventArgs) Handles btnRussianPatch.Click
        DownloadPatch("http://dl.cyberman.me/pso2/rupatch.rar", "RU Patch", "RUPatch.rar", Nothing,
                      "Would you like to backup your files before applying the patch? This will erase all previous Pre-RU patch backups.",
                      "Please select the pre-downloaded RU Patch RAR file", "backupPreRUPatch")
    End Sub

    Private Sub tsmRestartDownload_Click(sender As Object, e As EventArgs) Handles tsmRestartDownload.Click
        Restartplz = True
    End Sub

    Private Sub btnResumePatching_Click(sender As Object, e As EventArgs) Handles btnResumePatching.Click
        If Not File.Exists("resume.txt") Then
            WriteDebugInfo(My.Resources.strCannotFindResume)
            Exit Sub
        End If

        CancelledFull = False

        Try
            Dim sBuffer As String = Nothing
            Dim missingfiles As New List(Of String)
            Dim NumberofChecks As Integer = 0
            missingfiles.Clear()
            Using oReader As StreamReader = File.OpenText("resume.txt")
                WriteDebugInfoAndOK(My.Resources.strFoundIncompleteJob)
                If CancelledFull Then Exit Sub
                While Not (oReader.EndOfStream)
                    If CancelledFull Then Exit Sub
                    sBuffer = oReader.ReadLine
                    missingfiles.Add(sBuffer)
                End While
            End Using

            Dim totaldownload As String = missingfiles.Count
            Dim downloaded As Long = 0
            Dim totaldownloaded As Long = 0
            patching = True
            For Each downloadstring In missingfiles
                If CancelledFull Then Exit Sub
                'Download the missing files:
                'WHAT THE FUCK IS GOING ON HERE?v3
                downloaded += 1
                totaldownloaded += totalsize2

                lblStatus.Text = My.Resources.strDownloading & "" & downloaded & "/" & totaldownload & " (" & Helper.SizeSuffix(totaldownloaded) & ")"

                Application.DoEvents()
                Cancelled = False
                DLWUA(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadstring & ".pat"), downloadstring, True)
                Dim info7 As New FileInfo(downloadstring)
                Dim length2 As Long
                If File.Exists(downloadstring) Then length2 = info7.Length
                If info7.Length = 0 Then
                    Log("File appears to be empty, trying to download from secondary SEGA server")
                    DLWUA(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring & ".pat"), downloadstring, True)
                End If
                If Cancelled Then Exit Sub
                'Delete the existing file FIRST
                DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
                File.Move(downloadstring, ((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
                If VedaUnlocked Then WriteDebugInfo("DEBUG: Downloaded and installed " & downloadstring & ".")
                Dim linesList As New List(Of String)(File.ReadAllLines("resume.txt"))

                'Remove the line to delete, e.g.
                linesList.Remove(downloadstring)
                File.WriteAllLines("resume.txt", linesList.ToArray())
                Application.DoEvents()
            Next
            DeleteFile("resume.txt")
            patching = False
            If missingfiles.Count = 0 Then WriteDebugInfo(My.Resources.strYouAppearToBeUpToDate)
            Dim filedownloader3 As New WebClient()
            Dim DirectoryString As String = ((lblDirectory.Text & "\data\win32").Replace("\data\win32", "") & "\")
            WriteDebugInfo(My.Resources.strDownloading & "version file...")
            Application.DoEvents()
            Cancelled = False
            Dim versionclient As New MyWebClient With {.timeout = 3000}
            versionclient.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
            If Cancelled Then Exit Sub
            Dim DirectoryString2 As String = (lblDirectory.Text & "\data\win32").Replace("\data\win32", "")
            DeleteFile((Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
            File.Copy("version.ver", (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
            WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & "version file"))
            WriteDebugInfo(My.Resources.strDownloading & "pso2launcher.exe...")
            Application.DoEvents()
            Dim procs = Process.GetProcessesByName("pso2launcher")
            For Each proc As Process In procs
                If proc.MainWindowTitle = "PHANTASY STAR ONLINE 2" AndAlso proc.MainModule.ToString = "ProcessModule (pso2launcher.exe)" Then proc.Kill()
            Next
            DLWUA("http://download.pso2.jp/patch_prod/patches/pso2launcher.exe.pat", "pso2launcher.exe", True)
            If Cancelled Then Exit Sub
            If File.Exists((DirectoryString & "pso2launcher.exe")) AndAlso Application.StartupPath <> DirectoryString2 Then DeleteFile((DirectoryString & "pso2launcher.exe"))
            File.Move("pso2launcher.exe", (DirectoryString & "pso2launcher.exe"))
            WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & "pso2launcher.exe"))
            WriteDebugInfo(My.Resources.strDownloading & "pso2updater.exe...")
            Application.DoEvents()
            procs = Process.GetProcessesByName("pso2updater")
            For Each proc As Process In procs
                If proc.MainModule.ToString = "ProcessModule (pso2updater.exe)" Then proc.Kill()
            Next
            DLWUA("http://download.pso2.jp/patch_prod/patches/pso2updater.exe.pat", "pso2updater.exe", True)
            If Cancelled Then Exit Sub
            If File.Exists((DirectoryString & "pso2updater.exe")) AndAlso Application.StartupPath <> DirectoryString2 Then DeleteFile((DirectoryString & "pso2updater.exe"))
            File.Move("pso2updater.exe", (DirectoryString & "pso2updater.exe"))
            WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & "pso2updater.exe"))
            Application.DoEvents()
            WriteDebugInfo(My.Resources.strDownloading & "pso2.exe...")
            procs = Process.GetProcessesByName("pso2")

            For Each proc As Process In procs
                If proc.MainModule.ToString = "ProcessModule (pso2.exe)" Then proc.Kill()
            Next

            DLWUA("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", "pso2.exe", True)
            If Cancelled Then Exit Sub

            If File.Exists((DirectoryString & "pso2.exe")) AndAlso Application.StartupPath <> DirectoryString2 Then DeleteFile((DirectoryString & "pso2.exe"))
            File.Move("pso2.exe", (DirectoryString & "pso2.exe"))
            If CancelledFull Then Exit Sub
            WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & "pso2.exe"))
            Helper.SetRegKey(Of String)("StoryPatchVersion", "Not Installed")
            Helper.SetRegKey(Of String)("ENPatchVersion", "Not Installed")
            Helper.SetRegKey(Of String)("LargeFilesVersion", "Not Installed")
            DLWUA("http://download.pso2.jp/patch_prod/patches/patchlist.txt", "patchlist.txt", True)
            WriteDebugInfoSameLine(My.Resources.strDone)
            Helper.SetRegKey(Of String)("PSO2PatchlistMD5", Helper.GetMD5("patchlist.txt"))
            WriteDebugInfo(My.Resources.strGameUpdatedVanilla)
            DeleteFile("resume.txt")
            Dim lines2 = File.ReadAllLines("version.ver")
            Dim RemoteVersion2 As String = lines2(0)
            Helper.SetRegKey(Of String)("PSO2RemoteVersion", RemoteVersion2)
            UnlockGUI()

            If Helper.GetRegKey(Of Boolean)("RemoveCensor") Then
                If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c"), "ffbff2ac5b7a7948961212cefd4d402c.backup")
                WriteDebugInfoAndOK(My.Resources.strRemoving & "Censor...")
            End If

            If Helper.GetRegKey(Of Boolean)("ENPatchAfterInstall") Then
                WriteDebugInfo(My.Resources.strAutoInstallingENPatch)
                btnENPatch.RaiseClick()
            End If

            If Helper.GetRegKey(Of Boolean)("LargeFilesAfterInstall") Then
                WriteDebugInfo(My.Resources.strAutoInstallingLF)
                btnLargeFiles.RaiseClick()
            End If

            If Helper.GetRegKey(Of Boolean)("StoryPatchAfterInstall") Then
                WriteDebugInfo(My.Resources.strAutoInstallingStoryPatch)
                btnStory.RaiseClick()
            End If

            WriteDebugInfoAndOK(My.Resources.strallDone)
            Exit Sub

        Catch ex As Exception
            Log(ex.Message)
            If ex.Message <> "Arithmetic operation resulted in an overflow." Then
                WriteDebugInfo(My.Resources.strERROR & ex.Message)
                Exit Sub
            End If
        End Try
    End Sub

    Private Sub ButtonItem7_Click(sender As Object, e As EventArgs) Handles ButtonItem7.Click
        Dim ProcessName As String = "chrome"
        processes = Process.GetProcessesByName("chrome")
        Dim currentProcess As Process = Process.GetCurrentProcess()
        Dim x As Integer = 0

        If processes.Count > x Then
            Dim CloseItYesNo As MsgBoxResult = MsgBox("You need to have all Chrome windows closed before launching in this mode. Would you like to close all open Chrome windows now?", vbYesNo)
            If CloseItYesNo = vbYes Then
                Dim procs As Process() = Process.GetProcessesByName(ProcessName)

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
        Dim ClearYesNo As MsgBoxResult = MsgBox("This will clear all Symbol Arts from your ""History"" tab. Having 100 pages of Symbol Arts to load can sometimes cause slowdown.", vbYesNo)
        If ClearYesNo = vbNo Then
            Exit Sub
        End If
        If ClearYesNo = vbYes Then
            WriteDebugInfo("Deleting Symbol Art Cache...")
            Dim Documents As String = (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\")
            Dim SACacheFolder As String = (Documents & "SEGA\PHANTASYSTARONLINE2\symbolarts\cache")
            For Each foundFile As String In My.Computer.FileSystem.GetFiles(SACacheFolder, FileIO.SearchOption.SearchAllSubDirectories, "*.*")
                My.Computer.FileSystem.DeleteFile(foundFile, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
            Next
            WriteDebugInfoSameLine(My.Resources.strDone)
        End If
    End Sub

    Public Shared Function GetDownloadsPath() As String
        Dim path__1 As String = Nothing
        If Environment.OSVersion.Version.Major >= 6 Then
            Dim pathPtr As IntPtr
            Dim hr As Integer = SHGetKnownFolderPath(FolderDownloads, 0, IntPtr.Zero, pathPtr)
            If hr = 0 Then
                path__1 = Marshal.PtrToStringUni(pathPtr)
                Marshal.FreeCoTaskMem(pathPtr)
                Return path__1
            End If
        End If
        path__1 = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.Personal))
        path__1 = Path.Combine(path__1, "Downloads")
        Return path__1
    End Function

    Private Sub btnInstallPSO2_Click(sender As Object, e As EventArgs) Handles btnInstallPSO2.Click
        Dim InstallFolder As String = ""
        Dim InstallYesNo As MsgBoxResult = vbYes
        If InstallYesNo = vbNo Then
            WriteDebugInfo("You can view more information about the installer at:" & vbCrLf & "http://arks-layer.com/setup.php")
            Exit Sub
        End If
        If InstallYesNo = vbYes Then
            MsgBox("This will install Phantasy Star Online EPISODE 2! Please select a folder to install into." & vbCrLf & "A folder called PHANTASYSTARONLINE2 will be created inside the folder you choose." & vbCrLf & "(For example, if you choose the C drive, it will install to C:\PHANTASYSTARONLINE2\)" & vbCrLf & "It is HIGHLY RECOMMENDED that you do NOT install into the Program Files folder, but a normal folder like C:\PHANTASYSTARONLINE\")
SelectInstallFolder:
            Dim MyFolderBrowser As New FolderBrowserDialog With {.RootFolder = Environment.SpecialFolder.MyComputer, .Description = "Please select a folder (or drive) to install PSO2 into"}
            Dim dlgResult As DialogResult = MyFolderBrowser.ShowDialog()

            If dlgResult = DialogResult.OK Then
                InstallFolder = MyFolderBrowser.SelectedPath
            End If
            If dlgResult = DialogResult.Cancel Then
                WriteDebugInfo("Installation cancelled by user!")
                Exit Sub
            End If
            Dim CorrectYesNo As MsgBoxResult = MsgBox("You wish to install PSO2 into " & InstallFolder & "\PHANTASYSTARONLINE2\. Is this correct?", vbYesNoCancel)
            If CorrectYesNo = vbCancel Then
                WriteDebugInfo("Installation cancelled by user!")
                Exit Sub
            End If
            If CorrectYesNo = vbNo Then
                GoTo SelectInstallFolder
            End If
            If CorrectYesNo = vbYes Then
                Dim pso2_binfolder As String = InstallFolder & "\PHANTASYSTARONLINE2\pso2_bin"
                Dim MGMT As ManagementObject
                Dim Searcher As ManagementObjectSearcher = New ManagementObjectSearcher("SELECT * FROM Win32_LogicalDisk")
                Dim InstallDrive As String = InstallFolder.TrimEnd(":").Replace("\", "")
                For Each MGMT In Searcher.Get
                    If Convert.ToString(MGMT("MediaType")) = "12" Then
                        If MGMT("DeviceID") = InstallDrive Then
                            If (MGMT("Size")) < 17179869184 Then
                                MsgBox("There is not enough space on the selected disk to install PSO2. Please select a different drive. (Requires 16GB of free space)")
                                GoTo SelectInstallFolder
                            End If
                            If (MGMT("FreeSpace")) < 17179869184 Then
                                MsgBox("There is not enough free space on the selected disk to install PSO2. Please free up some space or select a different drive. (Requires 16GB of free space)")
                                GoTo SelectInstallFolder
                            End If
                        End If
                    End If
                Next
                Dim InstallENPatchesAfter As MsgBoxResult = MsgBox("Would you like the program to automatically install the core EN patch and Large Files EN patch after it's done updating the game?", vbYesNo)
                Dim FinalYesNo As MsgBoxResult = MsgBox("The program will now install the necessary files, create the folders, and set up the game. Afterwards, the program will automatically begin patching. Click ""OK"" to start.", MsgBoxStyle.OkCancel)
                If FinalYesNo = vbCancel Then
                    WriteDebugInfo("Installation cancelled by user!")
                    Exit Sub
                End If
                If FinalYesNo = vbOK Then
                    Office2007StartButton1.Enabled = False
                    Me.TopMost = True
                    Me.Show()
                    Me.TopMost = False
                    Application.DoEvents()
                    WriteDebugInfo("Checking/Installing DirectX...")
                    Dim client As New MyWebClient With {.timeout = 10000}
                    Try
                        client.DownloadFile("http://arks-layer.com/docs/dxwebsetup.exe", "dxwebsetup.exe")
                        Dim processStartInfo As ProcessStartInfo = New ProcessStartInfo() With {.FileName = "dxwebsetup.exe", .Verb = "runas", .Arguments = "/Q", .UseShellExecute = True}
                        Dim process As Process = process.Start(processStartInfo)
                        Do Until process.WaitForExit(1000)
                        Loop
                        WriteDebugInfoSameLine("Done!")

                    Catch ex As Exception
                        WriteDebugInfo("DirectX installation failed! Please install it later if neccessary!")
                    End Try
                    'Make a data folder, and a win32 folder under that
                    Directory.CreateDirectory(pso2_binfolder & "\data\win32\")
                    'Download required pso2 stuff
                    WriteDebugInfo("Downloading PSO2 required files...")
                    DLWUA("http://download.pso2.jp/patch_prod/patches/pso2launcher.exe.pat", pso2_binfolder & "\pso2launcher.exe", True)
                    DLWUA("http://download.pso2.jp/patch_prod/patches/pso2updater.exe.pat", pso2_binfolder & "\pso2updater.exe", True)
                    DLWUA("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", pso2_binfolder & "\pso2.exe", True)
                    DLWUA("http://download.pso2.jp/patch_prod/patches/PSO2JP.ini.pat", pso2_binfolder & "\PSO2JP.ini", True)
                    WriteDebugInfoSameLine("Done!")
                    'Download Gameguard.des
                    WriteDebugInfo("Downloading Latest Gameguard file...")
                    DLWUA("http://download.pso2.jp/patch_prod/patches/GameGuard.des.pat", pso2_binfolder & "\GameGuard.des", True)
                    WriteDebugInfoSameLine(My.Resources.strDone)
                    Application.DoEvents()

                    'set the lbldirectory.text to the install patch
                    lblDirectory.Text = pso2_binfolder
                    Helper.SetRegKey(Of String)("PSO2Dir", lblDirectory.Text)
                    WriteDebugInfo(lblDirectory.Text & " " & My.Resources.strSetAsYourPSO2)
                    If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("StoryPatchVersion")) Then Helper.SetRegKey(Of String)("StoryPatchVersion", "Not Installed")
                    If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("ENPatchVersion")) Then Helper.SetRegKey(Of String)("ENPatchVersion", "Not Installed")
                    If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("LargeFilesVersion")) Then Helper.SetRegKey(Of String)("LargeFilesVersion", "Not Installed")

                    'Check for PSO2 Updates~
                    ButtonItem5.RaiseClick()

                    If InstallENPatchesAfter = vbNo Then
                        MsgBox("PSO2 installed, patched to the latest Japanese version, and ready to play!" & vbCrLf & "Press OK to restart the program.")
                        Application.Restart()
                        Exit Sub
                    End If

                    'Install patches after updating (silently, no damn popups)
                    SilentENpatch()
                    SilentLargeFiles()

                    If Not chkItemTranslation.Checked Then
                        chkItemTranslation.RaiseClick()
                        WriteDebugInfo("Item patch enabled!")
                    End If

                    MsgBox("PSO2 installed and updated. English patches installed, game is ready to play!" & vbCrLf & "Press OK to restart the program.")
                    Application.Restart()
                    Exit Sub
                End If
            End If
        End If
    End Sub

    Public Sub SilentLargeFiles()
        CancelledFull = False
        Try
            Dim strVersion As String
            WriteDebugInfo(My.Resources.strDownloading & "Large Files....")
            Application.DoEvents()
            Dim net As New WebClient()
            Dim src As String
            If CheckLink("http://psumods.co.uk/viewtopic.php?f=4&t=206") <> "OK" Then
                WriteDebugInfoAndFAILED("Failed to contact EN Patch website - Patch install/update canceled!")
                WriteDebugInfo("Please visit http://goo.gl/YzCE7 for more information!")
                Exit Sub
            End If
            src = net.DownloadString("http://psumods.co.uk/viewtopic.php?f=4&t=206")
            ' Create a match using regular exp<b></b>ressions
            'http://pso2.arghargh200.net/pso2/2013_05_22_largefiles.rar
            Dim m As Match = Regex.Match(src, "<br /><a href="".*?.rar")
            ' Spit out the value plucked from the code
            txtHTML.Text = m.NextMatch.ToString
            Dim strDownloadME As String = txtHTML.Text
            Dim Lastfilename As String() = strDownloadME.Split("/")
            strVersion = Lastfilename(Lastfilename.Count - 1)
            strVersion = strVersion.Replace(".rar", "")

            Cancelled = False
            If CheckLink(strDownloadME) <> "OK" Then
                WriteDebugInfoAndFAILED("Failed to contact EN Patch website - Patch install/update canceled!")
                WriteDebugInfo("Please visit http://goo.gl/YzCE7 for more information!")
                Exit Sub
            End If
            DLWUA(strDownloadME, "LargeFiles.rar", True)
            If Cancelled Then Exit Sub
            WriteDebugInfo((My.Resources.strDownloadCompleteDownloaded & strDownloadME & ")"))
            Application.DoEvents()
            If Directory.Exists("TEMPPATCHAIDAFOOL") Then
                My.Computer.FileSystem.DeleteDirectory("TEMPPATCHAIDAFOOL", FileIO.DeleteDirectoryOption.DeleteAllContents)
                Directory.CreateDirectory("TEMPPATCHAIDAFOOL")
            End If
            If Not Directory.Exists("TEMPPATCHAIDAFOOL") Then
                Directory.CreateDirectory("TEMPPATCHAIDAFOOL")
            End If
            Dim UnRarLocation As String = (Application.StartupPath & "\unrar.exe")
            Dim process As Process = process.Start(New ProcessStartInfo() With {.FileName = UnRarLocation, .Verb = "runas", .Arguments = "e LargeFiles.rar TEMPPATCHAIDAFOOL", .WindowStyle = ProcessWindowStyle.Normal, .UseShellExecute = True})
            WriteDebugInfo(My.Resources.strWaitingforPatch)
            If CancelledFull Then Exit Sub
            Do Until process.WaitForExit(1000)
            Loop
            If CancelledFull Then Exit Sub
            If Not Directory.Exists("TEMPPATCHAIDAFOOL") Then
                Directory.CreateDirectory("TEMPPATCHAIDAFOOL")
                WriteDebugInfo("Had to manually make temp update folder - Did the patch not extract right?")
            End If
            Dim di As New DirectoryInfo("TEMPPATCHAIDAFOOL")
            Dim diar1 As FileInfo() = di.GetFiles()
            Dim dra As FileInfo
            WriteDebugInfoAndOK((My.Resources.strExtractingTo & (lblDirectory.Text & "\data\win32")))
            Application.DoEvents()

            'list the names of all files in the specified directory
            Log("Extracted " & diar1.Count & " files from the patch")

            If diar1.Count = 0 Then
                WriteDebugInfo("Patch failed to extract correctly! Installation failed!")
                Exit Sub
            End If

            WriteDebugInfo(My.Resources.strInstallingPatch)

            For Each dra In diar1
                If CancelledFull Then Exit Sub
                DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
                File.Move(("TEMPPATCHAIDAFOOL\" & dra.ToString), ((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
            Next

            My.Computer.FileSystem.DeleteDirectory("TEMPPATCHAIDAFOOL", FileIO.DeleteDirectoryOption.DeleteAllContents)

            FlashWindow(Me.Handle, 1)
            Helper.SetRegKey(Of String)("LargeFilesVersion", strVersion)
            WriteDebugInfo("Large Files installed!")

            DeleteFile("LargeFiles.rar")
            UnlockGUI()
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub

    Public Sub SilentENpatch()
        CancelledFull = False
        Try
            WriteDebugInfo(My.Resources.strDownloading & "EN patch...")
            Application.DoEvents()
            Dim strVersion As String
            Dim net As New WebClient()
            Dim src As String
            If CheckLink("http://psumods.co.uk/viewtopic.php?f=4&t=206") <> "OK" Then
                WriteDebugInfoAndFAILED("Failed to contact EN Patch website - Patch install/update canceled!")
                WriteDebugInfo("Please visit http://goo.gl/YzCE7 for more information!")
                Exit Sub
            End If
            src = net.DownloadString("http://psumods.co.uk/viewtopic.php?f=4&t=206")

            ' Create a match using regular exp<b></b>ressions
            'http://pso2.arghargh200.net/pso2/patch_2013_04_24.rar
            Dim m As Match = Regex.Match(src, "<br /><a href="".*?.rar")

            ' Spit out the value plucked from the code
            txtHTML.Text = m.Value
            Dim strDownloadME As String = txtHTML.Text.Replace("<br /><a href=""", "")
            Dim Lastfilename As String() = strDownloadME.Split("/")
            strVersion = Lastfilename(Lastfilename.Count - 1)
            strVersion = strVersion.Replace(".rar", "")

            Cancelled = False
            If CheckLink(strDownloadME) <> "OK" Then
                WriteDebugInfoAndFAILED("Failed to contact EN Patch website - Patch install/update canceled!")
                WriteDebugInfo("Please visit http://goo.gl/YzCE7 for more information!")
                Exit Sub
            End If
            DLWUA(strDownloadME, "ENPatch.rar", True)
            If Cancelled Then Exit Sub

            WriteDebugInfo((My.Resources.strDownloadCompleteDownloaded & strDownloadME & ")"))
            Application.DoEvents()
            If Directory.Exists("TEMPPATCHAIDAFOOL") Then
                My.Computer.FileSystem.DeleteDirectory("TEMPPATCHAIDAFOOL", FileIO.DeleteDirectoryOption.DeleteAllContents)
                Directory.CreateDirectory("TEMPPATCHAIDAFOOL")
            End If
            If Not Directory.Exists("TEMPPATCHAIDAFOOL") Then
                Directory.CreateDirectory("TEMPPATCHAIDAFOOL")
            End If
            Dim process As Process = Nothing
            Dim processStartInfo As ProcessStartInfo = New ProcessStartInfo()
            Dim UnRarLocation = (Application.StartupPath & "\unrar.exe")
            processStartInfo.FileName = UnRarLocation
            processStartInfo.Verb = "runas"
            processStartInfo.Arguments = ("e ENPatch.rar " & "TEMPPATCHAIDAFOOL")
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
            WriteDebugInfoAndOK((My.Resources.strExtractingTo & (lblDirectory.Text & "\data\win32")))
            Application.DoEvents()
            If CancelledFull Then Exit Sub
            'list the names of all files in the specified directory
            Dim backupdir As String = ((lblDirectory.Text & "\data\win32") & "\" & "backupPreENPatch")

            Log("Extracted " & diar1.Count & " files from the patch")

            If diar1.Count = 0 Then
                WriteDebugInfo("Patch failed to extract correctly! Installation failed!")
                Exit Sub
            End If

            WriteDebugInfo(My.Resources.strInstallingPatch)

            For Each dra In diar1
                If CancelledFull Then Exit Sub
                DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
                File.Move(("TEMPPATCHAIDAFOOL\" & dra.ToString), ((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
            Next

            My.Computer.FileSystem.DeleteDirectory("TEMPPATCHAIDAFOOL", FileIO.DeleteDirectoryOption.DeleteAllContents)

            FlashWindow(Me.Handle, 1)
            WriteDebugInfo("English patch installed!")
            Helper.SetRegKey(Of String)("ENPatchVersion", strVersion)
            DeleteFile("ENPatch.rar")
            UnlockGUI()
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub

    Private Sub btnConfigureItemTranslation_Click(sender As Object, e As EventArgs) Handles btnConfigureItemTranslation.Click
        frmItemConfig.Show()
    End Sub

    Public Function CheckLink(ByVal Url As String) As String
        Dim req As HttpWebRequest = TryCast(WebRequest.Create(Url), HttpWebRequest)
        req.Timeout = 5000
        req.Method = "HEAD"
        Try
            Using rsp As HttpWebResponse = TryCast(req.GetResponse(), HttpWebResponse)
                Return "OK"
            End Using
        Catch ex As WebException
            Dim ReturnString As String = ex.Message.ToString.Replace("The remote server returned an error: ", "")
            Return ReturnString
        End Try
    End Function

    Private Sub btnSymbolEditor_Click(sender As Object, e As EventArgs) Handles btnSymbolEditor.Click
        Process.Start("http://www.pso-world.com/forums/showthread.php?t=215777")
    End Sub

    Private Sub btnRunPSO2Linux_Click(sender As Object, e As EventArgs) Handles btnRunPSO2Linux.Click
        Process.Start("http://www.pso-world.com/forums/showthread.php?t=215642")
    End Sub

    Private Sub btnInstallSpanishPatch_Click(sender As Object, e As EventArgs) Handles btnInstallSpanishPatch.Click
        Using net As New WebClient()
            Dim page As String = net.DownloadString("http://162.243.211.123/pso2patches/espatch.html")
            Dim match As Match = Regex.Match(page, "<b>(.{1,})</b>")

            If (match.Success) Then
                Dim url As String = "http://162.243.211.123/pso2patches/uploads/" & match.Groups(1).Value

                DownloadPatch(url, "ES Patch", "ESPatch.rar", Nothing,
                      "Would you like to backup your files before applying the patch? This will erase all previous Pre-RU patch backups.",
                      "Please select the pre-downloaded ES Patch RAR file", "backupPreESPatch")
            Else
                MessageBox.Show("An error occurred while trying to parse the ES Patch page.", "Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
        End Using
    End Sub

    Private Sub LoadSidebar()
        Try
            WebBrowser4.Navigate("http://162.243.211.123/freedom/tweaker.html")
        Catch ex As Exception
            WriteDebugInfo("Web Browser failed: " & ex.Message)
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

    Public Sub setserverstatus(ByVal serverstatus As String)
        If serverstatus = "ONLINE!" Then
            Label5.ForeColor = Color.Green
            Label5.Text = "ONLINE!"
        End If
        If serverstatus = "OFFLINE" Then
            Label5.ForeColor = Color.Red
            Label5.Text = "OFFLINE"
        End If
    End Sub

    Private Sub IsServerOnline()

        'TODO: CK needs to recode this to actually parse the ship statuses from the packet to be more accurate.

        Dim ip As String = "gs016.pso2gs.net" ' Incase they need to use the proxy
        Dim port As Integer = 12200

        Try
            Using sock As New TcpClient()
                sock.NoDelay = True
                sock.ReceiveTimeout = 1000
                'sock.Connect(ip, port)
                Dim result = sock.BeginConnect(ip, port, Nothing, Nothing)
                result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1))
                If Not sock.Connected Then
                    Throw New Exception("Unable to connect!")
                End If

                Label5.Invoke(New Action(Of String)(AddressOf setserverstatus), "ONLINE!")
                sock.Close()
            End Using
        Catch ex As Exception
            Label5.Invoke(New Action(Of String)(AddressOf setserverstatus), "OFFLINE")
        End Try
    End Sub

    Private Sub tmrCheckServerStatus_Tick(sender As Object, e As EventArgs) Handles tmrCheckServerStatus.Tick
        Dim Oldstatus As String = Label5.Text
        Dim t5 As New Thread(AddressOf IsServerOnline) With {.IsBackground = True}
        t5.Start()
        If Label5.Text <> Oldstatus Then
            MsgBox("The server is now " & Label5.Text & "!")
            If Label5.Text = "ONLINE" Then CheckForPSO2Updates()
        End If
    End Sub

    Private Sub btnResetTweaker_Click(sender As Object, e As EventArgs) Handles btnResetTweaker.Click
        Dim resetyesno As MsgBoxResult = MsgBox("This will erase all of the PSO2 Tweaker's settings, and restart the program. Continue?", vbYesNo)
        If resetyesno = vbYes Then
            My.Computer.Registry.CurrentUser.DeleteSubKey("HKEY_CURRENT_USER\Software\AIDA", False)
            Log("All settings reset, restarting program!")
            Application.Restart()
        End If
    End Sub

    Private Sub btnPredownloadLobbyVideos_Click(sender As Object, e As EventArgs) Handles btnPredownloadLobbyVideos.Click
        If (Directory.Exists((lblDirectory.Text & "\data\win32")) = False OrElse lblDirectory.Text = "lblDirectory") Then
            MsgBox(My.Resources.strPleaseSelectwin32Dir)
            Button1.RaiseClick()
            Exit Sub
        End If 'Download the missing files:
        Cancelled = False
        Dim downloadstring As String = "3fdcad94b7af8c597542cd23e6a87236"
        Dim downloaded As Long = 0
        Dim totaldownloaded As Long = 0
        totaldownloaded += totalsize2

        lblStatus.Text = My.Resources.strDownloading & " lobby video (" & Helper.SizeSuffix(totaldownloaded) & ")"

        DLWUA(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadstring & ".pat"), downloadstring, True)
        Dim info7 As New FileInfo(downloadstring)
        Dim length2 As Long
        If File.Exists(downloadstring) Then length2 = info7.Length
        If info7.Length = 0 Then
            Log("File appears to be empty, trying to download from secondary SEGA server")
            DLWUA(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring & ".pat"), downloadstring, True)
        End If
        If Cancelled Then Exit Sub
        File.Move(downloadstring, ((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
        WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & downloadstring & "."))
    End Sub

    Private Sub btnDownloadPrepatch_Click(sender As Object, e As EventArgs) Handles btnDownloadPrepatch.Click
        ComingFromPrePatch = True
        CheckForPSO2Updates()
    End Sub

    Private Sub btnCopyInfo_Click_1(sender As Object, e As EventArgs) Handles btnCopyInfo.Click
        Try
            frmDiagnostic.TopMost = Me.TopMost
            frmDiagnostic.Top += 50
            frmDiagnostic.Left += 50
            frmDiagnostic.ShowDialog()
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub btnChooseProxyServer_Click(sender As Object, e As EventArgs) Handles btnChooseProxyServer.Click
        Try
            Dim myWebClient As New WebClient()
            'JSON should look like { "version": 1, "host": "0.0.0.0", "name": "Super cool proxy", "publickeyurl": "http://url.com" }
            Dim JSONURL As String = InputBox("Please input the URL of the configuration JSON:", "Configuration JSON", "")
            '[AIDA] am i doin it rite sonicfreak?
            If String.IsNullOrEmpty(JSONURL) Then Exit Sub
            WriteDebugInfo("Downloading configuration...")
            myWebClient.DownloadFile(JSONURL, "ServerConfig.txt")
            Dim JSONCONTENTS As String = File.ReadAllText("ServerConfig.txt")
            Dim json As JObject = JObject.Parse(JSONCONTENTS)
            Dim Version As Integer = json.SelectToken("version")
            Dim Host As String = json.SelectToken("host")
            Dim ProxyName As String = json.SelectToken("name")
            Dim PublickeyUrl As String = json.SelectToken("publickeyurl")
            If Version <> 1 Then
                MsgBox("ERROR - Version is incorrect! Please recheck the JSON.")
                Exit Sub
            End If
            If Not PublickeyUrl.Contains("publickey.blob") Then
                MsgBox("ERROR - Public Key URL doesn't point to a public key blob! Please recheck the JSON.")
                Exit Sub
            End If
            For i = 0 To Host.Length - 1
                If Char.IsLetter(Host.Chars(i)) Then
                    Dim ips As IPAddress()

                    ips = Dns.GetHostAddresses(Host)
                    Host = ips(0).ToString
                    Exit For
                End If
            Next
            WriteDebugInfoSameLine(" Done!")

            Dim FILE_NAME As String = Environment.SystemDirectory & "\drivers\etc\hosts"
            Dim BuiltFile As String = ""
            Dim CurrentLine As String = ""
            Dim AlreadyModified As Boolean = False
            'http://162.243.211.123/test.json
            Dim objReader As New StreamReader(FILE_NAME)

            ' TODO: This isn't right

            Do While objReader.Peek() <> -1
                CurrentLine = objReader.ReadLine()

                If CurrentLine.Contains("gs001.pso2gs.net") Then
                    CurrentLine = Host & " gs001.pso2gs.net #" & ProxyName & " Ship 01"
                    AlreadyModified = True
                End If
                If CurrentLine.Contains("gs016.pso2gs.net") Then
                    CurrentLine = Host & " gs016.pso2gs.net #" & ProxyName & " Ship 02"
                    AlreadyModified = True
                End If
                If CurrentLine.Contains("gs031.pso2gs.net") Then
                    CurrentLine = Host & " gs031.pso2gs.net #" & ProxyName & " Ship 03"
                    AlreadyModified = True
                End If
                If CurrentLine.Contains("gs046.pso2gs.net") Then
                    CurrentLine = Host & " gs046.pso2gs.net #" & ProxyName & " Ship 04"
                    AlreadyModified = True
                End If
                If CurrentLine.Contains("gs061.pso2gs.net") Then
                    CurrentLine = Host & " gs061.pso2gs.net #" & ProxyName & " Ship 05"
                    AlreadyModified = True
                End If
                If CurrentLine.Contains("gs076.pso2gs.net") Then
                    CurrentLine = Host & " gs076.pso2gs.net #" & ProxyName & " Ship 06"
                    AlreadyModified = True
                End If
                If CurrentLine.Contains("gs091.pso2gs.net") Then
                    CurrentLine = Host & " gs091.pso2gs.net #" & ProxyName & " Ship 07"
                    AlreadyModified = True
                End If
                If CurrentLine.Contains("gs106.pso2gs.net") Then
                    CurrentLine = Host & " gs106.pso2gs.net #" & ProxyName & " Ship 08"
                    AlreadyModified = True
                End If
                If CurrentLine.Contains("gs121.pso2gs.net") Then
                    CurrentLine = Host & " gs121.pso2gs.net #" & ProxyName & " Ship 09"
                    AlreadyModified = True
                End If
                If CurrentLine.Contains("gs136.pso2gs.net") Then
                    CurrentLine = Host & " gs136.pso2gs.net #" & ProxyName & " Ship 10"
                    AlreadyModified = True
                End If
                BuiltFile &= CurrentLine & vbNewLine
            Loop
            objReader.Close()

            If AlreadyModified Then WriteDebugInfo("Modifying HOSTS file...")

            If Not AlreadyModified Then
                BuiltFile &= Host & " gs001.pso2gs.net #" & ProxyName & " Ship 01" & vbNewLine
                BuiltFile &= Host & " gs016.pso2gs.net #" & ProxyName & " Ship 02" & vbNewLine
                BuiltFile &= Host & " gs031.pso2gs.net #" & ProxyName & " Ship 03" & vbNewLine
                BuiltFile &= Host & " gs046.pso2gs.net #" & ProxyName & " Ship 04" & vbNewLine
                BuiltFile &= Host & " gs061.pso2gs.net #" & ProxyName & " Ship 05" & vbNewLine
                BuiltFile &= Host & " gs076.pso2gs.net #" & ProxyName & " Ship 06" & vbNewLine
                BuiltFile &= Host & " gs091.pso2gs.net #" & ProxyName & " Ship 07" & vbNewLine
                BuiltFile &= Host & " gs106.pso2gs.net #" & ProxyName & " Ship 08" & vbNewLine
                BuiltFile &= Host & " gs121.pso2gs.net #" & ProxyName & " Ship 09" & vbNewLine
                BuiltFile &= Host & " gs136.pso2gs.net #" & ProxyName & " Ship 10" & vbNewLine
                WriteDebugInfo("Previous modifications not found, creating new entries...")
            End If

            File.WriteAllText(Environment.SystemDirectory & "\drivers\etc\hosts", BuiltFile)
            WriteDebugInfoSameLine(" Done!")

            WriteDebugInfo("Downloading and installing publickey.blob...")
            myWebClient.DownloadFile(PublickeyUrl, Application.StartupPath & "\publickey.blob")
            If File.Exists(lblDirectory.Text & "\publickey.blob") AndAlso Application.StartupPath <> lblDirectory.Text Then DeleteFile(lblDirectory.Text & "\publickey.blob")
            If Application.StartupPath <> lblDirectory.Text Then File.Move(Application.StartupPath & "\publickey.blob", lblDirectory.Text & "\publickey.blob")
            WriteDebugInfoSameLine(" Done!")
            WriteDebugInfo("All done! You should now be able to connect to " & ProxyName & ".")
            Helper.SetRegKey(Of String)("ProxyEnabled", "True")
        Catch ex As Exception
            WriteDebugInfoAndFAILED("ERROR - " & ex.Message)
            If ex.Message.ToString.Contains("is denied.") AndAlso ex.Message.ToString.Contains("Access to the path") Then MsgBox("It seems you've gotten an error while trying to patch your HOSTS file. Please go to the " & Environment.SystemDirectory & "\drivers\etc\ folder, right click on the hosts file, and make sure ""Read Only"" is not checked. Then try again.")
            Exit Sub
        End Try
    End Sub

    Private Sub btnRevertPSO2ProxyToJP_Click(sender As Object, e As EventArgs) Handles btnRevertPSO2ProxyToJP.Click
        Dim hostsFilePath As String = Environment.SystemDirectory & "\drivers\etc\hosts"
        Dim builtFile = New List(Of String)

        Using reader As New StreamReader(hostsFilePath)
            Dim currentLine As String = ""

            Do
                currentLine = reader.ReadLine()
                If (currentLine Is Nothing) Then Exit Do
                If Not currentLine.Contains("pso2gs.net") Then builtFile.Add(currentLine)
            Loop
        End Using

        WriteDebugInfo("Modifying HOSTS file...")
        File.WriteAllLines(hostsFilePath, builtFile.ToArray())
        WriteDebugInfoSameLine(" Done!")
        DeleteFile(lblDirectory.Text & "\publickey.blob")
        WriteDebugInfoAndOK("All normal JP connection settings restored!")
        Helper.SetRegKey(Of String)("ProxyEnabled", "False")
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
        If Directory.Exists(((lblDirectory.Text & "\data\win32") & "\backupPreENPatch")) Then
            WriteDebugInfo(My.Resources.strENBackupFound)
            Override = True
            btnRestoreENBackup.RaiseClick()
            Override = False
        End If

        If Directory.Exists(((lblDirectory.Text & "\data\win32") & "\backupPreLargeFiles")) Then
            WriteDebugInfo(My.Resources.strLFBackupFound)
            Override = True
            btnRestoreLargeFilesBackup.RaiseClick()
            Override = False
        End If

        Dim count As Integer = 0
        Dim totalfiles = My.Computer.FileSystem.GetFiles(lblDirectory.Text & "\data\win32\")

        If Not File.Exists("old_patchlist.txt") Then
            Dim Filename As String = ""
            Dim filesize As Long = 0
            WriteDebugInfo("Building file list... ")
            Dim di As New DirectoryInfo(lblDirectory.Text & "\data\win32\")
            Dim diar1 As FileInfo() = di.GetFiles()
            Dim dra As FileInfo

            For Each dra In diar1
                Filename = dra.Name
                filesize = dra.Length
                File.AppendAllText("old_patchlist.txt", "data/win32/" & Filename & ".pat" & vbTab & filesize & vbTab & Helper.GetMD5(lblDirectory.Text & "\data\win32\" & Filename) & vbNewLine)
                count += 1
                lblStatus.Text = "Building first time list of win32 files (" & count & "/" & totalfiles.Count & ")"
                Application.DoEvents()
            Next

            WriteDebugInfoSameLine("Done!")
        End If

        LockGUI()
        WriteDebugInfo(My.Resources.strDownloadingPatchFile1)
        Application.DoEvents()
        DLWUA("http://download.pso2.jp/patch_prod/patches/launcherlist.txt", "launcherlist.txt", True)
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strDownloadingPatchFile2)
        Application.DoEvents()
        DLWUA("http://download.pso2.jp/patch_prod/patches/patchlist.txt", "patchlist.txt", True)
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strDownloadingPatchFile3)
        Application.DoEvents()
        DLWUA("http://download.pso2.jp/patch_prod/patches_old/patchlist.txt", "patchlist_old.txt", True)
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strDownloadingPatchFile4)
        Application.DoEvents()
        Dim versionclient As New MyWebClient With {.timeout = 3000}
        versionclient.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
        WriteDebugInfoSameLine(My.Resources.strDone)
        Application.DoEvents()
        UnlockGUI()
        MergePatches()

        'Rewrite this to support the new format

        Dim SEGALine As String = ""
        Dim LocalLine As String = ""
        Dim SEGAFilename As String = ""
        Dim missingfiles As New List(Of String)

        Dim oldarray = File.ReadAllLines("old_patchlist.txt")
        Dim Contains As Boolean = False

        For i As Integer = 0 To SOMEOFTHETHINGS.Count
            SEGALine = SOMEOFTHETHINGS.Values(i)
            If String.IsNullOrEmpty(SEGALine) Then Continue For

            SEGAFilename = Regex.Split(SEGALine, ".pat")(0).Replace("data/win32/", "")
            lblStatus.Text = "Checking file " & i & " / " & totalfiles.Count
            If missingfiles.Count > 0 Then lblStatus.Text &= " (missing files found: " & missingfiles.Count & ")"
            Application.DoEvents()
            If Not oldarray.Contains(SEGALine) Then missingfiles.Add(SEGAFilename)
        Next
    End Sub

    Private Sub btnStoryPatchNew_Click(sender As Object, e As EventArgs) Handles btnStoryPatchNew.Click
        'Don't forget to make GUI changes!

        If (Directory.Exists((lblDirectory.Text & "\data\win32")) = False OrElse lblDirectory.Text = "lblDirectory") Then
            MsgBox(My.Resources.strPleaseSelectwin32Dir)
            Button1.RaiseClick()
            Exit Sub
        End If

        Dim win32 As String = lblDirectory.Text & "\data\win32"
        Dim strStoryPatchLatestBase As String = ""
        Dim backupdir As String = ((lblDirectory.Text & "\data\win32") & "\" & "backupPreSTORYPatch")
        Dim net As New WebClient()
        Dim src As String = net.DownloadString("http://arks-layer.com/story.php")

        ' Create a match using regular exp<b></b>ressions
        Dim m As Match = Regex.Match(src, "<u>.*?</u>")

        ' Spit out the value plucked from the code
        txtHTML.Text = m.Value
        Dim strDownloadME As String = txtHTML.Text.Replace("<u>", "").Replace("</u>", "")
        strStoryPatchLatestBase = strDownloadME
        strStoryPatchLatestBase = strStoryPatchLatestBase.Replace("/", "-")

        WriteDebugInfoAndOK("Downloading story patch info... ")
        DLWUA("http://162.243.211.123/freedom/pso2.stripped.db", "pso2.stripped.db", True)
        WriteDebugInfoAndOK("Downloading Trans-Am tool... ")
        DLWUA("http://162.243.211.123/freedom/pso2-transam.exe", "pso2-transam.exe", True)

        'execute pso2-transam stuff with -b flag for backup
        Dim process As Process = Nothing
        Dim processStartInfo As ProcessStartInfo = New ProcessStartInfo() With {.FileName = "pso2-transam.exe", .Verb = "runas"}

        If Directory.Exists(backupdir) Then processStartInfo.Arguments = ("-t story-eng-" & strStoryPatchLatestBase & " pso2.stripped.db " & """" & win32 & """")
        If Not Directory.Exists(backupdir) Then
            Directory.CreateDirectory(backupdir)
            WriteDebugInfo(My.Resources.strCreatingBackupDirectory)
            processStartInfo.Arguments = ("-b " & """" & backupdir & """" & " -t story-eng-" & strStoryPatchLatestBase & " pso2.stripped.db " & """" & win32 & """")
        End If

        processStartInfo.WindowStyle = ProcessWindowStyle.Normal
        processStartInfo.UseShellExecute = True
        process = process.Start(processStartInfo)
        Do Until process.WaitForExit(1000)
        Loop

        DeleteFile("pso2.stripped.db")
        DeleteFile("pso2-transam.exe")

        FlashWindow(Me.Handle, 1)
        'Story Patch 3-12-2014.rar
        Helper.SetRegKey(Of String)("StoryPatchVersion", strStoryPatchLatestBase.Replace("-", "/"))
        Helper.SetRegKey(Of String)("LatestStoryBase", strStoryPatchLatestBase.Replace("-", "/"))
        WriteDebugInfo(My.Resources.strStoryPatchInstalled)
        CheckForStoryUpdates()
    End Sub

    Private Sub btnJPEnemyNames_Click(sender As Object, e As EventArgs) Handles btnJPEnemyNames.Click
        Try
            If (Directory.Exists((lblDirectory.Text & "\data\win32")) = False OrElse lblDirectory.Text = "lblDirectory") Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                Button1.RaiseClick()
                Exit Sub
            End If
            WriteDebugInfo(My.Resources.strDownloading & "JP enemy names file....")
            Application.DoEvents()
            Dim strDownloadME As String = "http://107.170.16.100/patches/ceffe0e2386e8d39f188358303a92a7d"
            Dim strVersion As String = strDownloadME.Replace("http://pso2.arghargh200.net/pso2/", "").Replace("http://107.170.16.100/patchbackups/", "")
            Cancelled = False
            DLWUA(strDownloadME, "ceffe0e2386e8d39f188358303a92a7d", True)
            If Cancelled Then Exit Sub
            WriteDebugInfo((My.Resources.strDownloadCompleteDownloaded & strDownloadME & ")"))
            If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "ceffe0e2386e8d39f188358303a92a7d")) Then
                If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "ceffe0e2386e8d39f188358303a92a7d.backup")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "ceffe0e2386e8d39f188358303a92a7d.backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "ceffe0e2386e8d39f188358303a92a7d"), "ceffe0e2386e8d39f188358303a92a7d.backup")
            End If
            Application.DoEvents()
            File.Move("ceffe0e2386e8d39f188358303a92a7d", ((lblDirectory.Text & "\data\win32") & "\ceffe0e2386e8d39f188358303a92a7d"))
            FlashWindow(Me.Handle, 1)
            WriteDebugInfo("JP enemy names file " & My.Resources.strInstalledUpdated)
            UnlockGUI()
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub

    Private Sub btnJPETrials_Click(sender As Object, e As EventArgs) Handles btnJPETrials.Click
        Try
            If (Directory.Exists((lblDirectory.Text & "\data\win32")) = False OrElse lblDirectory.Text = "lblDirectory") Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                Button1.RaiseClick()
                Exit Sub
            End If
            WriteDebugInfo(My.Resources.strDownloading & "JP E-Trials file....")
            Application.DoEvents()
            Dim strDownloadME As String = "http://107.170.16.100/patches/057aa975bdd2b372fe092614b0f4399e"
            Dim strVersion As String = strDownloadME.Replace("http://pso2.arghargh200.net/pso2/", "").Replace("http://107.170.16.100/patchbackups/", "")
            Cancelled = False
            DLWUA(strDownloadME, "057aa975bdd2b372fe092614b0f4399e", True)
            If Cancelled Then Exit Sub
            WriteDebugInfo((My.Resources.strDownloadCompleteDownloaded & strDownloadME & ")"))
            If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "057aa975bdd2b372fe092614b0f4399e")) Then
                If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & "057aa975bdd2b372fe092614b0f4399e.backup")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "057aa975bdd2b372fe092614b0f4399e.backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "057aa975bdd2b372fe092614b0f4399e"), "057aa975bdd2b372fe092614b0f4399e.backup")
            End If
            Application.DoEvents()
            File.Move("057aa975bdd2b372fe092614b0f4399e", ((lblDirectory.Text & "\data\win32") & "\057aa975bdd2b372fe092614b0f4399e"))
            FlashWindow(Me.Handle, 1)
            WriteDebugInfo("JP E-Trials file " & My.Resources.strInstalledUpdated)
            UnlockGUI()
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub

    Private Sub DownloadPatch(PatchURL As String, PatchName As String, PatchFile As String, VersionString As String, msgBackup As String, msgSelectArchive As String, BackupDir As String)
        CancelledFull = False
        Try
            If (Directory.Exists((lblDirectory.Text & "\data\win32")) = False OrElse lblDirectory.Text = "lblDirectory") Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                Button1.RaiseClick()
                Exit Sub
            End If

            Dim backupyesno As MsgBoxResult
            Dim predownloadedyesno As MsgBoxResult
            Dim RARLocation As String = ""
            Dim strVersion As String = ""

            ' Check the patch download method preference
            Dim PatchPreference As String = Helper.GetRegKey(Of String)("PredownloadedRAR")
            Select Case PatchPreference
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
            PatchPreference = Helper.GetRegKey(Of String)("Backup")
            Select Case PatchPreference
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
                WriteDebugInfo(My.Resources.strDownloading & PatchName & "...")
                Application.DoEvents()

                ' TODO: Switch to a Uri class.
                ' Get the filename from the downloaded Path
                Dim Lastfilename As String() = PatchURL.Split("/"c)
                strVersion = Lastfilename(Lastfilename.Length - 1)
                strVersion = Path.GetFileNameWithoutExtension(strVersion) ' We're using this so that it's not format-specific.

                Cancelled = False

                If CheckLink(PatchURL) <> "OK" Then
                    WriteDebugInfoAndFAILED("Failed to contact " & PatchName & " website - Patch install/update canceled!")
                    WriteDebugInfo("Please visit http://goo.gl/YzCE7 for more information!")
                    Exit Sub
                End If

                DLWUA(PatchURL, PatchFile, True)
                If Cancelled Then Exit Sub
                WriteDebugInfo((My.Resources.strDownloadCompleteDownloaded & PatchURL & ")"))
            ElseIf predownloadedyesno = MsgBoxResult.Yes Then
                OpenFileDialog1.Title = msgSelectArchive
                OpenFileDialog1.FileName = "PSO2 " & PatchName & " RAR file"
                OpenFileDialog1.Filter = "RAR Archives|*.rar|All Files (*.*) |*.*"
                If OpenFileDialog1.ShowDialog() = DialogResult.Cancel Then Exit Sub

                RARLocation = OpenFileDialog1.FileName.ToString()
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

            Dim process As Process = Nothing
            Dim processStartInfo As ProcessStartInfo = New ProcessStartInfo()
            Dim UnRarLocation As String = (Application.StartupPath & "\unrar.exe")
            processStartInfo.FileName = UnRarLocation
            processStartInfo.Verb = "runas"
            If predownloadedyesno = MsgBoxResult.No Then processStartInfo.Arguments = ("e " & PatchFile & " TEMPPATCHAIDAFOOL")
            If predownloadedyesno = MsgBoxResult.Yes Then processStartInfo.Arguments = ("e " & """" & RARLocation & """" & " TEMPPATCHAIDAFOOL")
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
            WriteDebugInfoAndOK((My.Resources.strExtractingTo & (lblDirectory.Text & "\data\win32")))
            Application.DoEvents()
            If CancelledFull Then Exit Sub

            Dim backupstr As String = ((lblDirectory.Text & "\data\win32") & "\" & BackupDir)
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

            Log("Extracted " & diar1.Count & " files from the patch")
            If diar1.Count = 0 Then
                WriteDebugInfo("Patch failed to extract correctly! Installation failed!")
                Exit Sub
            End If

            WriteDebugInfo(My.Resources.strInstallingPatch)

            For Each dra In diar1
                If CancelledFull Then Exit Sub

                If backupyesno = MsgBoxResult.Yes Then
                    If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString)) Then
                        File.Move(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString), (backupstr & "\" & dra.ToString))
                    End If
                End If
                If backupyesno = MsgBoxResult.No Then
                    If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString)) Then
                        DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
                    End If
                End If
                File.Move(("TEMPPATCHAIDAFOOL\" & dra.ToString), ((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
            Next

            My.Computer.FileSystem.DeleteDirectory("TEMPPATCHAIDAFOOL", FileIO.DeleteDirectoryOption.DeleteAllContents)
            If backupyesno = MsgBoxResult.No Then
                FlashWindow(Me.Handle, 1)
                WriteDebugInfo("English patch " & My.Resources.strInstalledUpdated)
                If Not String.IsNullOrEmpty(VersionString) Then Helper.SetRegKey(Of String)(VersionString, strVersion)
            End If
            If backupyesno = MsgBoxResult.Yes Then
                FlashWindow(Me.Handle, 1)
                WriteDebugInfo(("English patch " & My.Resources.strInstalledUpdatedBackup & backupstr))
                If Not String.IsNullOrEmpty(VersionString) Then Helper.SetRegKey(Of String)(VersionString, strVersion)
            End If
            DeleteFile(PatchName)
            UnlockGUI()
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub RibbonControl1_Click(sender As Object, e As EventArgs) Handles RibbonControl1.Click

    End Sub
End Class