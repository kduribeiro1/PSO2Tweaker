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

' TODO: Make sure all the "For Each"s are in order
' TODO: Replace all redundant code with functions
' TODO: Replace all string literals for registry keys with constant strings to avoid errors in the future
' TODO: Organize this form by order of member type (variable, function, etc)

Public Class frmMain

    Private Structure SHELLEXECUTEINFOW
        Dim cbSize As Long
        Dim fMask As Long
        Dim hWnd As Long
        <MarshalAs(UnmanagedType.LPTStr)> Public lpVerb As String
        <MarshalAs(UnmanagedType.LPTStr)> Public lpFile As String
        <MarshalAs(UnmanagedType.LPTStr)> Public lpParameters As String
        <MarshalAs(UnmanagedType.LPTStr)> Public lpDirectory As String
        Dim nShow As Long
        Dim hInstApp As Long
        Dim lpIDList As Long
        Dim lpClass As Long
        Dim hKeyClass As Long
        Dim dwHotKey As Long
        Dim hIcon As Long
        Dim hProcess As Long
    End Structure

    Dim DPISetting As String
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

#Region "External Functions"

    Private Declare Function FindWindowByCaption Lib "user32" (ByVal zero As IntPtr, ByVal lpWindowName As String) As IntPtr

    Private Declare Function ShellExecuteExW Lib "shell32" (ByRef lpExecInfo As SHELLEXECUTEINFOW) As Long

    Private Declare Auto Function ShellExecute Lib "shell32" (ByVal hwnd As IntPtr, ByVal lpOperation As String, ByVal lpFile As String, ByVal lpParameters As String, ByVal lpDirectory As String, ByVal nShowCmd As UInteger) As IntPtr

    Private Declare Function ReadProcessMemory Lib "kernel32" (ByVal hProcess As Integer, ByVal lpBaseAddress As Integer, ByVal lpBuffer As String, ByVal nSize As Integer, ByRef lpNumberOfBytesWritten As Integer) As Integer

    Private Declare Function LoadLibrary Lib "kernel32" Alias "LoadLibraryA" (ByVal lpLibFileName As String) As Integer

    Private Declare Function VirtualAllocEx Lib "kernel32" (ByVal hProcess As Integer, ByVal lpAddress As Integer, ByVal dwSize As Integer, ByVal flAllocationType As Integer, ByVal flProtect As Integer) As Integer

    Private Declare Function WriteProcessMemory Lib "kernel32" (ByVal hProcess As Integer, ByVal lpBaseAddress As Integer, ByVal lpBuffer As String, ByVal nSize As Integer, ByRef lpNumberOfBytesWritten As Integer) As Integer

    Private Declare Function GetProcAddress Lib "kernel32" (ByVal hModule As Integer, ByVal lpProcName As String) As Integer

    Private Declare Function GetModuleHandle Lib "Kernel32" Alias "GetModuleHandleA" (ByVal lpModuleName As String) As Integer

    Private Declare Function CreateRemoteThread Lib "kernel32" (ByVal hProcess As Integer, ByVal lpThreadAttributes As Integer, ByVal dwStackSize As Integer, ByVal lpStartAddress As Integer, ByVal lpParameter As Integer, ByVal dwCreationFlags As Integer, ByRef lpThreadId As Integer) As Integer

    Private Declare Function OpenProcess Lib "kernel32" (ByVal dwDesiredAccess As Integer, ByVal bInheritHandle As Integer, ByVal dwProcessId As Integer) As Integer

    Private Declare Function FindWindow Lib "user32" Alias "FindWindowA" (ByVal lpClassName As String, ByVal lpWindowName As String) As Integer

    Private Declare Function CloseHandle Lib "kernel32" Alias "CloseHandleA" (ByVal hObject As Integer) As Integer

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
            If e.KeyCode = Keys.U And SystemUnlock = 1 Then
                SystemUnlock = 2
                lblStatus.Text = "Please enter the remaining commands to access Veda: **"
            End If
            If e.KeyCode = Keys.N And SystemUnlock = 2 Then
                SystemUnlock = 3
                lblStatus.Text = "Please enter the remaining commands to access Veda: ***"
            End If
            If e.KeyCode = Keys.D And SystemUnlock = 3 Then
                SystemUnlock = 4
                lblStatus.Text = "Please enter the remaining commands to access Veda: ****"
            End If
            If e.KeyCode = Keys.A And SystemUnlock = 4 Then
                SystemUnlock = 5
                lblStatus.Text = "Please enter the remaining commands to access Veda: *****"
            End If
            If e.KeyCode = Keys.M And SystemUnlock = 5 Then
                SystemUnlock = 6
                lblStatus.Text = "Please enter the remaining commands to access Veda: ******"
                Application.DoEvents()
                Threading.Thread.Sleep(2000)
                lblStatus.Text = "[ACCESS GRANTED - SYSTEM UNLOCKED]"
                Application.DoEvents()
                Threading.Thread.Sleep(2000)
                VedaUnlocked = True
                frmVEDA.Show()
            End If
            If e.KeyCode = Keys.M Then
                MileyCyrus = 1
                lblStatus.Text = "Please enter the remaining commands: *"
            End If
            If e.KeyCode = Keys.I And MileyCyrus = 1 Then
                MileyCyrus = 2
                lblStatus.Text = "Please enter the remaining commands: **"
            End If
            If e.KeyCode = Keys.L And MileyCyrus = 2 Then
                MileyCyrus = 3
                lblStatus.Text = "Please enter the remaining commands: ***"
            End If
            If e.KeyCode = Keys.E And MileyCyrus = 3 Then
                MileyCyrus = 4
                lblStatus.Text = "Please enter the remaining commands: ****"
            End If
            If e.KeyCode = Keys.Y And MileyCyrus = 4 Then
                MileyCyrus = 5
                lblStatus.Text = "Please enter the remaining commands: *****"
                Application.DoEvents()
                Threading.Thread.Sleep(2000)
                lblStatus.Text = "[ACCESS GRANTED - PSO2 TWERKER]"
                Application.DoEvents()
                Threading.Thread.Sleep(2000)
                Me.Text = ("PSO2 Twerker ver " & My.Application.Info.Version.ToString)
                ButtonItem6.Text = "Twerk it!"
                chkItemTranslation.Text = "Twerk on Robin Thicke"
            End If
            If e.KeyCode = Keys.K Then
                SteamUnlock = 1
                lblStatus.Text = "Please enter the remaining commands to unlock BETA Steam support: *"
            End If
            If e.KeyCode = Keys.U And SteamUnlock = 1 Then
                SteamUnlock = 2
                lblStatus.Text = "Please enter the remaining commands to unlock BETA Steam support: **"
            End If
            If e.KeyCode = Keys.M And SteamUnlock = 2 Then
                SteamUnlock = 3
                lblStatus.Text = "Please enter the remaining commands to unlock BETA Steam support: ***"
            End If
            If e.KeyCode = Keys.I And SteamUnlock = 3 Then
                SteamUnlock = 4
                lblStatus.Text = "Please enter the remaining commands to unlock BETA Steam support: ****"
            End If
            If e.KeyCode = Keys.H And SteamUnlock = 4 Then
                SteamUnlock = 5
                lblStatus.Text = "Please enter the remaining commands to unlock BETA Steam support: *****"
            End If
            If e.KeyCode = Keys.O And SteamUnlock = 5 Then
                SteamUnlock = 6
                lblStatus.Text = "Please enter the remaining commands to unlock BETA Steam support: ******"
                Application.DoEvents()
                Threading.Thread.Sleep(2000)
                lblStatus.Text = "[ACCESS GRANTED - SYSTEM UNLOCKED]"
                Application.DoEvents()
                Threading.Thread.Sleep(2000)
                frmOptions.ButtonX4.Visible = True
                frmOptions.ButtonX5.Visible = True
                frmOptions.LabelX13.Visible = True
                frmOptions.TextBoxX1.Visible = True
                frmOptions.ButtonX3.Visible = True

                frmVEDA.Show()
            End If
        End If
    End Sub

    Public Structure SHELLEXECUTEINFO
        Public cbSize As Integer
        Public fMask As Integer
        Public hwnd As IntPtr
        <MarshalAs(UnmanagedType.LPTStr)> Public lpVerb As String
        <MarshalAs(UnmanagedType.LPTStr)> Public lpFile As String
        <MarshalAs(UnmanagedType.LPTStr)> Public lpParameters As String
        <MarshalAs(UnmanagedType.LPTStr)> Public lpDirectory As String
        Dim nShow As Integer
        Dim hInstApp As IntPtr
        Dim lpIDList As IntPtr
        <MarshalAs(UnmanagedType.LPTStr)> Public lpClass As String
        Public hkeyClass As IntPtr
        Public dwHotKey As Integer
        Public hIcon As IntPtr
        Public hProcess As IntPtr
    End Structure

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim g As Graphics = Me.CreateGraphics
        If g.DpiX.ToString = "120" Then
            DPISetting = "120"
        End If
        If g.DpiX.ToString = "96" Then
            DPISetting = "96"
        End If
        g.Dispose()
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
            If lblDirectory.Text.Contains("\pso2_bin\data\win32") = True Then
                If File.Exists(lblDirectory.Text.Replace("\data\win32", "") & "\pso2.exe") = True Then
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
            If File.Exists(pso2launchpath & "ddraw.dll") Then File.Delete(pso2launchpath & "ddraw.dll")

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
                    If File.Exists(Dir() & "\SEGA\PHANTASYSTARONLINE2\item_name_cache.dat") Then File.Delete(Dir() & "\SEGA\PHANTASYSTARONLINE2\item_name_cache.dat")
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
                    'If Environment.Is64BitOperatingSystem = True Then
                    'Helper.SetRegKey(Of String)("AppInit_DLLs_backup", Helper.GetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion\Windows", "AppInit_DLLs", Nothing))
                    'Helper.SetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion\Windows", "AppInit_DLLs", "Translator.dll", Microsoft.Win32.RegistryValueKind.String)
                    'Helper.SetRegKey(Of Integer)("LoadAppInit_DLLs_backup", Helper.GetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion\Windows", "LoadAppInit_DLLs", Nothing))
                    'Helper.SetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion\Windows", "LoadAppInit_DLLs", 1, Microsoft.Win32.RegistryValueKind.DWord)
                    'End If
                    'If it's 32-bit
                    'If Environment.Is64BitOperatingSystem = False Then
                    'Helper.SetRegKey(Of String)("AppInit_DLLs_backup", Helper.GetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows", "AppInit_DLLs", Nothing))
                    'Helper.SetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows", "AppInit_DLLs", "Translator.dll", Microsoft.Win32.RegistryValueKind.String)
                    'Helper.SetRegKey(Of Integer)("LoadAppInit_DLLs_backup", Helper.GetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows", "LoadAppInit_DLLs", Nothing))
                    'Helper.SetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows", "LoadAppInit_DLLs", 1, Microsoft.Win32.RegistryValueKind.DWord)
                    'End If
                    'End Item Translation stuff
                    SaveToDisk("ddraw.dll", pso2launchpath & "\ddraw.dll")
                    Log("Setting environment variable")
                    Environment.SetEnvironmentVariable("-pso2", "+0x01e3f1e9")
                    Log("Launching PSO2")
                    ShellExecute(Handle, "open", (pso2launchpath & "\pso2.exe"), "+0x33aca2b9 -pso2", "", 0)
                    Me.Hide()
                    Do Until File.Exists(pso2launchpath & "\ddraw.dll") = False
                        procs = Process.GetProcessesByName("pso2")
                        For Each proc As Process In procs
                            If proc.MainWindowTitle = "Phantasy Star Online 2" And proc.MainModule.ToString = "ProcessModule (pso2.exe)" Then
                                If TransOverride = False Then File.Delete(pso2launchpath & "\ddraw.dll")
                            End If
                        Next
                        Thread.Sleep(1000)
                    Loop
                    Me.Close()
                End If
                If CommandLineArgs(i) = "-pso2" Then
                    Log("Detected command argument -pso2")
                    'Fuck SEGA. Fuck them hard.
                    'If Helper.GetRegKey(Of String)("SeenFuckSEGAMessage") = "False" Then MsgBox("SEGA recently updated the pso2.exe file so that it can't be launched from anything but the official launcher. You can still use this to patch, fix, apply patches, and everything you did before. Once you're ready to launch the game, however, the launcher will open the PSO2JP launcher. Simply click the large button to launch the game. Sorry about the inconvience, I'll try to see if I can find a way around it soon! (This message will not appear again.)" & vbCrLf & "- AIDA")
                    'Helper.SetRegKey(Of String)("SeenFuckSEGAMessage", "True")
                    'Log("Checking if PSO2 is running")
                    'If CheckIfRunning("pso2") = "Running" Then Exit Sub
                    'If CheckIfRunning("pso2.exe") = "Running" Then Exit Sub
                    'Try
                    If (Directory.Exists(lblDirectory.Text) = False OrElse lblDirectory.Text = "lblDirectory") Then
                        MsgBox(My.Resources.strPleaseSelectwin32Dir)
                        Button1.RaiseClick()
                        Exit Sub
                    End If
                    DirectoryString = (lblDirectory.Text & "\data\win32")
                    pso2launchpath = DirectoryString.Replace("\data\win32", "")
                    If UseItemTranslation = True Then
                        Dim dir As String
                        dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                        Log("Deleting item cache...")
                        If File.Exists(dir & "\SEGA\PHANTASYSTARONLINE2\item_name_cache.dat") Then File.Delete(dir & "\SEGA\PHANTASYSTARONLINE2\item_name_cache.dat")
                        'Check to see if the keys exist
                        'for x64
                        'HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion\Windows
                        'for x86
                        'HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows
                        'Keys are:
                        'LoadAppInit_DLLs (0 for no, 1 for yes)
                        'AppInit_DLLs (string, pointing to the file)
                        'If it's 64-bit
                        'Fuck this shit~
                        Log("Checking to see if the item translation works with PSO2...")
                        DLWUA("http://162.243.211.123/freedom/working.txt", "working.txt", True)
                        Using oReader As StreamReader = File.OpenText("working.txt")
                            sBuffer = oReader.ReadLine
                            If sBuffer.ToString = "No" Then
                                MsgBox("Sorry, but the item translation is not working with the current version of PSO2! Please remove the -item argument if you wish to launch PSO2 without it.")
                                Exit Sub
                            End If
                        End Using
                        File.Delete("working.txt")
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
                        'DLWUA(DLLink2, (pso2launchpath & "\translation.bin"), True)

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

                        'If Environment.Is64BitOperatingSystem = True Then
                        'Helper.SetRegKey(Of String)("AppInit_DLLs_backup", Helper.GetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion\Windows", "AppInit_DLLs", Nothing))
                        'Helper.SetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion\Windows", "AppInit_DLLs", "Translator.dll", Microsoft.Win32.RegistryValueKind.String)
                        'Helper.SetRegKey(Of Integer)("LoadAppInit_DLLs_backup", Helper.GetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion\Windows", "LoadAppInit_DLLs", Nothing))
                        'Helper.SetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion\Windows", "LoadAppInit_DLLs", 1, Microsoft.Win32.RegistryValueKind.DWord)
                        'End If
                        'If it's 32-bit
                        'If Environment.Is64BitOperatingSystem = False Then
                        'Helper.SetRegKey(Of String)("AppInit_DLLs_backup", Helper.GetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows", "AppInit_DLLs", Nothing))
                        'Helper.SetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows", "AppInit_DLLs", "Translator.dll", Microsoft.Win32.RegistryValueKind.String)
                        'Helper.SetRegKey(Of Integer)("LoadAppInit_DLLs_backup", Helper.GetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows", "LoadAppInit_DLLs", Nothing))
                        'Helper.SetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows", "LoadAppInit_DLLs", 1, Microsoft.Win32.RegistryValueKind.DWord)
                        'End If
                        'End Item Translation stuff
                        SaveToDisk("ddraw.dll", pso2launchpath & "\ddraw.dll")
                    End If
                    'Dim startInfo As ProcessStartInfo
                    'startInfo = New ProcessStartInfo
                    Log("Setting environment variable")
                    Environment.SetEnvironmentVariable("-pso2", "+0x01e3f1e9")
                    'startInfo.EnvironmentVariables("-pso2") = "+0x01e3f1e9"
                    'startInfo.FileName = (pso2launchpath & "\pso2.exe")
                    'startInfo.Arguments = "+0x33aca2b9"
                    'startInfo.UseShellExecute = True
                    'Dim shell As Process
                    'shell = New Process
                    'shell.StartInfo = startInfo
                    'Dim strEmail As String
                    'Dim Handle As IntPtr = 0  'if you use the literal value it will default to Long
                    Log("Launching PSO2")
                    ShellExecute(Handle, "open", (pso2launchpath & "\pso2.exe"), "+0x33aca2b9 -pso2", "", 0)
                    'shell.Start()
                    'Process.Start((pso2launchpath & "\pso2launcher.exe"))
                    'Process.Start((pso2launchpath & "\pso2.exe"), "-pso2")
                    'If File.Exists("launcherlist.txt") = True Then File.Delete("launcherlist.txt")
                    'If File.Exists("patchlist.txt") = True Then File.Delete("patchlist.txt")
                    'If File.Exists("patchlist_old.txt") = True Then File.Delete("patchlist_old.txt")
                    'If File.Exists("version.ver") = True Then File.Delete("version.ver")
                    'If UseItemTranslation = False Then
                    If File.Exists("LanguagePack.rar") Then File.Delete("LanguagePack.rar")
                    If UseItemTranslation = True Then
                        Me.Hide()
                        Do Until File.Exists(pso2launchpath & "\ddraw.dll") = False
                            procs = Process.GetProcessesByName("pso2")
                            For Each proc As Process In procs
                                If proc.MainWindowTitle = "Phantasy Star Online 2" And proc.MainModule.ToString = "ProcessModule (pso2.exe)" Then
                                    If TransOverride = False Then File.Delete(pso2launchpath & "\ddraw.dll")
                                End If
                            Next
                            Thread.Sleep(1000)
                        Loop
                        'Check to see if the keys exist
                        'for x64
                        'HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion\Windows
                        'for x86
                        'HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows
                        'Keys are:
                        'LoadAppInit_DLLs (0 for no, 1 for yes)
                        'AppInit_DLLs (string, pointing to the file)
                        'If it's 64-bit
                        'If Environment.Is64BitOperatingSystem = "True" Then
                        'Helper.SetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion\Windows", "AppInit_DLLs", Helper.GetRegKey(Of String)("AppInit_DLLs_backup"), Microsoft.Win32.RegistryValueKind.String)
                        'Helper.SetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion\Windows", "LoadAppInit_DLLs", Helper.GetRegKey(Of Integer)("LoadAppInit_DLLs_backup"), Microsoft.Win32.RegistryValueKind.DWord)
                        'End If
                        'If it's 32-bit
                        'If Environment.Is64BitOperatingSystem = "False" Then
                        'Helper.SetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows", "AppInit_DLLs", Helper.GetRegKey(Of String)("AppInit_DLLs_backup"), Microsoft.Win32.RegistryValueKind.String)
                        'Helper.SetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows", "LoadAppInit_DLLs", Helper.GetRegKey(Of Integer)("LoadAppInit_DLLs_backup"), Microsoft.Win32.RegistryValueKind.DWord)
                        'End If
                        'End Item Translation stuff
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
            If File.Exists(pso2launchpath & "\ddraw.dll") And TransOverride = False Then File.Delete(pso2launchpath & "\ddraw.dll")
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
            If Helper.GetRegKey(Of String)("Locale") <> "" Then
                Dim Locale As String = Helper.GetRegKey(Of String)("Locale")
                Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo(Locale)
                Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo(Locale)
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
            ButtonItem6.Text = My.Resources.strLaunchPSO2
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
            ButtonItem14.Text = My.Resources.strExit
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

            If Helper.GetRegKey(Of String)("TextBoxBGColor") <> "" Then rtbDebug.BackColor = Color.FromArgb(Helper.GetRegKey(Of String)("TextboxBGColor"))
            If Helper.GetRegKey(Of String)("TextBoxColor") <> "" Then rtbDebug.ForeColor = Color.FromArgb(Helper.GetRegKey(Of String)("TextboxColor"))

            Log("Colors")

            If Helper.GetRegKey(Of String)("Color") <> "" Then
                StyleManager1.ManagerColorTint = Color.FromArgb(Helper.GetRegKey(Of String)("Color"))
                frmPSO2Options.StyleManager1.ManagerColorTint = Color.FromArgb(Helper.GetRegKey(Of String)("Color"))
            End If

            If Helper.GetRegKey(Of String)("FontColor") <> "" Then
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
            'lblDirectory.Text = ""
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
            If File.Exists((Application.StartupPath & "\logfile.txt")) = True Then
                Dim LogInfo As New FileInfo((Application.StartupPath & "\logfile.txt"))
                If LogInfo.Length > 30720 Then
                    'Dim ClearLog As MsgBoxResult = MsgBox(My.Resources.strYourLogFileisHUGE, MsgBoxStyle.YesNo, "Log file")
                    'If ClearLog = MsgBoxResult.Yes Then
                    File.WriteAllText((Application.StartupPath & "\logfile.txt"), "")
                    'End If
                End If
            End If

            Application.DoEvents()
            Application.DoEvents()

            If nodiag = False Then
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
                Log("Folder Read/Write: " & GetFolderAccess(Application.StartupPath))
                Log("Is 7zip available: " & File.Exists(Application.StartupPath & "\7za.exe"))
                Log("----------------------------------------")
            End If

            If nodiag = True Then
                Log("Diagnostic info skipped due to -nodiag flag!")
            End If

            WriteDebugInfoAndOK((My.Resources.strProgramOpeningSuccessfully & My.Application.Info.Version.ToString))
            Application.DoEvents()
            'lblTitleBar.Text = ("             PSO2 Tweaker ver " & My.Application.Info.Version.ToString)
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)

        End Try
        Try
            Dim DirectoryString As String = (lblDirectory.Text & "\data\win32")
            Dim pso2launchpath As String = DirectoryString.Replace("\data\win32", "")
            Dim localVersion As String = My.Application.Info.Version.ToString
            Dim wc As MyWebClient = New MyWebClient()
            wc.timeout = 10000
            wc.Proxy = Nothing
            Dim source As String = String.Empty
            If File.Exists(Application.StartupPath & "\version.xml") Then File.Delete(Application.StartupPath & "\version.xml")
            'WriteDebugInfo("If startup loads forever, right click the progress bar and hit ""Cancel process"".")
            WriteDebugInfo(My.Resources.strCheckingforupdatesPleasewaitamoment)
            'http://arks-layer.com/aida/tweaker/
            'http://socket-hack.com:2312/tweaker/

            source = wc.DownloadString("http://162.243.211.123/freedom/version.xml")

            'MsgBox(source)
            'http://dl.dropboxusercontent.com/u/23005008/PSO2Tweaker/version.xml
            'http://arks-layer.com/aida/tweaker/version.xml
            'MsgBox(source
            If source.Contains("<VersionHistory>") = True Then

                Dim xm As New Xml.XmlDocument
                xm.LoadXml(source)

                Dim XMLNode = xm.SelectSingleNode("//CurrentVersion")
                Dim currentVersion As String = XMLNode.ChildNodes(0).InnerText.Trim
                'Dim currentLink As String = xm.SelectSingleNode("//CurrentVersion").ChildNodes(1).InnerText.Trim

                Log("Checking for the latest version of the program...")
                If localVersion = currentVersion Then
                    WriteDebugInfo((My.Resources.strYouhavethelatestversionoftheprogram & My.Application.Info.Version.ToString))
                Else
                    Dim changelogtotal As String = ""

                    For index As Integer = 2 To 11
                        Dim innerText = XMLNode.ChildNodes(index).InnerText
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

            'If FileLen("version.xml") = 0 Then
            If source.Contains("<VersionHistory>") = False Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToGetUpdateInfo)
            End If
            'End If
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)

        End Try
        Dim t3 As New Threading.Thread(AddressOf IsServerOnline)
        t3.IsBackground = True
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
            If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) = True And My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585")) = True Then
                If GetFileSize(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) = 167479840 And GetFileSize(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585")) = 151540352 Then
                    chkSwapOP.Text = My.Resources.strSwapPCVitaOpenings & " (" & My.Resources.strNotSwapped & ")"
                End If
                If GetFileSize(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) = 151540352 And GetFileSize(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585")) = 167479840 Then
                    chkSwapOP.Text = My.Resources.strSwapPCVitaOpenings & " (" & My.Resources.strSwapped & ")"
                End If
                'chkSwapOP.Text = "Swap PC/Vita Openings (UNKNOWN)"
            End If

            ' TODO: Shouldn't be doing this in this way
            Application.DoEvents()
            If File.Exists("7za.exe") = False Then
                WriteDebugInfo(My.Resources.strDownloading & "7za.exe...")
                Application.DoEvents()
                DLWUA("http://162.243.211.123/freedom/7za.exe", "7za.exe", True)
            End If
            If GetMD5("7za.exe") <> "42BADC1D2F03A8B1E4875740D3D49336" Then
                WriteDebugInfo(My.Resources.strYour7zipiscorrupt)
                Application.DoEvents()
                DLWUA("http://162.243.211.123/freedom/7za.exe", "7za.exe", True)
            End If
            If GetMD5("7za.exe") <> "42BADC1D2F03A8B1E4875740D3D49336" Then
                WriteDebugInfo(My.Resources.strYour7zipiscorrupt)
                Application.DoEvents()
                DLWUA("http://162.243.211.123/freedom/7za.exe", "7za.exe", True)
            End If
            If GetMD5("7za.exe") <> "42BADC1D2F03A8B1E4875740D3D49336" Then
                WriteDebugInfo(My.Resources.strYour7zipiscorrupt)
                Application.DoEvents()
                DLWUA("http://162.243.211.123/freedom/7za.exe", "7za.exe", True)
            End If
            If File.Exists("UnRar.exe") = False Then
                WriteDebugInfo(My.Resources.strDownloading & "UnRar.exe...")
                Application.DoEvents()
                DLWUA("http://162.243.211.123/freedom/UnRAR.exe", "UnRAR.exe", True)
            End If
            If GetMD5("UnRar.exe") <> "0C83C1293723A682577E3D0B21562B79" Then
                WriteDebugInfo(My.Resources.strYourUnrariscorrupt)
                Application.DoEvents()
                DLWUA("http://162.243.211.123/freedom/UnRAR.exe", "UnRAR.exe", True)
            End If
            If GetMD5("UnRar.exe") <> "0C83C1293723A682577E3D0B21562B79" Then
                WriteDebugInfo(My.Resources.strYourUnrariscorrupt)
                Application.DoEvents()
                DLWUA("http://162.243.211.123/freedom/UnRAR.exe", "UnRAR.exe", True)
            End If
            If GetMD5("UnRar.exe") <> "0C83C1293723A682577E3D0B21562B79" Then
                WriteDebugInfo(My.Resources.strYourUnrariscorrupt)
                Application.DoEvents()
                DLWUA("http://162.243.211.123/freedom/UnRAR.exe", "UnRAR.exe", True)
            End If
            If Directory.Exists("TEMPSTORYAIDAFOOL") = True Then
                Directory.Delete("TEMPSTORYAIDAFOOL", True)
            End If
            If Directory.Exists("TEMPPATCHAIDAFOOL") = True Then
                Directory.Delete("TEMPPATCHAIDAFOOL", True)
            End If

            File.Delete("launcherlist.txt")
            File.Delete("patchlist.txt")
            File.Delete("patchlist_old.txt")

            'Added in precede files. Stupid ass SEGA.
            File.Delete("patchlist0.txt")
            File.Delete("patchlist1.txt")
            File.Delete("patchlist2.txt")
            File.Delete("patchlist3.txt")
            File.Delete("patchlist4.txt")
            File.Delete("patchlist5.txt")
            File.Delete("precede.txt")
            File.Delete("ServerConfig.txt")
            File.Delete("SOMEOFTHETHINGS.txt")
            File.Delete("ALLOFTHETHINGS.txt")
            File.Delete("SOMEOFTHEPREPATCHES.txt")
            File.Delete("ALLOFTHEPREPATCHES.txt")
            File.Delete("precede_apply.txt")
            File.Delete("version.ver")
            File.Delete("Story MD5HashList.txt")

            UnlockGUI()
            ButtonItem6.Enabled = False
            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("SidebarEnabled")) Then Helper.SetRegKey(Of String)("SidebarEnabled", "True")
            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("RemoveCensor")) Then Helper.SetRegKey(Of String)("RemoveCensor", "True")
            If Helper.GetRegKey(Of String)("SidebarEnabled") = "True" Then
                WriteDebugInfo(My.Resources.strLoadingSidebar)
                Dim t1 As New Threading.Thread(AddressOf LoadSidebar)
                t1.IsBackground = True
                t1.Start()

                If DPISetting = "96" Then Me.Width = 796
                If DPISetting = "120" Then Me.Width = 1060
                btnAnnouncements.Text = "<"
            End If
            If File.Exists("resume.txt") = True Then
                Dim YesNoResume As MsgBoxResult = MsgBox("It seems that the last patching attempt was interrupted. Would you like to resume patching?", vbYesNo)
                If YesNoResume = MsgBoxResult.Yes Then
                    btnResumePatching.RaiseClick()
                Else
                    File.Delete("resume.txt")
                End If
            End If
            WriteDebugInfo(My.Resources.strCheckingforPSO2Updates)
            Application.DoEvents()
            CheckForPSO2Updates()
            WriteDebugInfoSameLine(My.Resources.strDone)
            'WriteDebugInfo("Current PSO2 Version is: " & Helper.GetRegKey(Of String)("PSO2PatchlistMD5"))
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
            'DEBUGMYSHITDAWG:
            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("UseItemTranslation")) Then
                Helper.SetRegKey(Of String)("UseItemTranslation", "True")
            End If
            UseItemTranslation = Helper.GetRegKey(Of String)("UseItemTranslation")
            If UseItemTranslation = True Then
                chkItemTranslation.Checked = True
                DirectoryString = (lblDirectory.Text & "\data\win32")
                pso2launchpath = DirectoryString.Replace("\data\win32", "")
                'Dim data As String()
                'data = {"Delay:1000", "TranslationPath:translation.bin", "TranslationCachePath:", "LogPath:", "LogLines:500", "KeyToggle:17", "KeyToggleCancel:16", "KeyDisable:112", "KeyDisableTree:114", "KeyDisableToggle:113"}
                'File.WriteAllLines(pso2launchpath & "\translation.cfg", data)
                'http://socket-hack.com:2312/translator_icecream/
                'Download the latest translator.dll and translation.bin
                WriteDebugInfo(My.Resources.strDownloadingItemTranslationFiles)
                ItemDownloadingDone = False
                Dim t4 As New Threading.Thread(AddressOf DownloadItemTranslationFiles)
                t4.IsBackground = True
                t4.Start()
                Do Until ItemDownloadingDone = True
                    Application.DoEvents()
                Loop
                WriteDebugInfoSameLine(My.Resources.strDone)
            End If
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)

        End Try
        'If UseItemTranslation = True Then
        ' Do Until ItemDownloadingDone = True
        ' Application.DoEvents()
        ' Loop
        ' End If
        DLWUA("http://162.243.211.123/freedom/working.txt", "working.txt", True)
        Using oReader As StreamReader = File.OpenText("working.txt")
            Dim sBuffer As String
            sBuffer = oReader.ReadLine
            WriteDebugInfo(My.Resources.strDoesItemPatchWork & sBuffer.ToString)
        End Using
        File.Delete("working.txt")
        File.Delete("version.xml")
        File.Delete("Story MD5HashList.txt")
        File.Delete("PSO2 Tweaker Updater.exe")
        WriteDebugInfo(My.Resources.strAllDoneSystemReady)
        ButtonItem6.Enabled = True
        'btnPSO2Options.RaiseClick()

        'watch.Stop()
        'MessageBox.Show(watch.ElapsedMilliseconds.ToString())

    End Sub
    Public Sub DownloadItemTranslationFiles()
        Dim DirectoryString As String = (lblDirectory.Text & "\data\win32")
        Dim pso2launchpath As String = DirectoryString.Replace("\data\win32", "")
        Dim DLLink1 As String = "http://162.243.211.123/freedom/translator.dll"
        Dim DLLink2 As String = "http://162.243.211.123/freedom/translation.bin"
        Dim client As New WebClient
        'Dim failednumbers As Integer = 0
DOWNLOADDLL2:
        Try
            client.DownloadFile(DLLink1, (pso2launchpath & "\translator.dll"))
        Catch ex As Exception
            'failednumbers += 1
            'If failednumbers = 4 Then
            MsgBox("Failed to download translation files! (" & ex.Message & ")")
            Exit Try
            'End If
            'GoTo DOWNLOADDLL2
        End Try
        Helper.SetRegKey(Of String)("DLLMD5", GetMD5(pso2launchpath & "\translator.dll"))
        'failednumbers = 0
        'DLWUA(DLLink2, (pso2launchpath & "\translation.bin"), True)
DOWNLOADBIN2:
        Try
            client.DownloadFile(DLLink2, (pso2launchpath & "\translation.bin"))
        Catch ex As Exception
            'failednumbers += 1
            'If failednumbers = 4 Then
            MsgBox("Failed to download translation files! (" & ex.Message & ")")
            Exit Try
            'End If
            'GoTo DOWNLOADBIN2
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
        Dim FileSize As Long = MyFile.Length
        Return FileSize
    End Function
    Public Sub WriteDebugInfo(ByVal AddThisText As String)
        If rtbDebug.InvokeRequired Then
            rtbDebug.Invoke(New Action(Of String)(AddressOf WriteDebugInfo), Text)
        Else
            rtbDebug.Text = rtbDebug.Text & vbCrLf & AddThisText
            Dim TimeFormatted As String
            Dim time As DateTime = DateTime.Now
            TimeFormatted = time.ToString("G")
            File.AppendAllText((Application.StartupPath & "\logfile.txt"), TimeFormatted & " " & AddThisText & vbCrLf)
        End If
    End Sub
    Private Sub WriteDebugInfoSameLine(ByVal AddThisText As String)
        If rtbDebug.InvokeRequired Then
            rtbDebug.Invoke(New Action(Of String)(AddressOf WriteDebugInfoSameLine), Text)
        Else
            rtbDebug.Text = rtbDebug.Text & " " & AddThisText
            Dim TimeFormatted As String
            Dim time As DateTime = DateTime.Now
            TimeFormatted = time.ToString("G")
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
            Dim TimeFormatted As String
            Dim time As DateTime = DateTime.Now
            TimeFormatted = time.ToString("G")
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
            Dim TimeFormatted As String
            Dim time As DateTime = DateTime.Now
            TimeFormatted = time.ToString("G")
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
            Dim TimeFormatted As String
            Dim time As DateTime = DateTime.Now
            TimeFormatted = time.ToString("G")
            File.AppendAllText((Application.StartupPath & "\logfile.txt"), TimeFormatted & " " & (AddThisText & " [FAILED!]") & vbCrLf)
            If Helper.GetRegKey(Of String)("Pastebin") = True Then
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
    Public Function Split(ByVal input As String, ByVal ParamArray delimiter As String()) As String()
        Return input.Split(delimiter, StringSplitOptions.None)
    End Function
    Private Sub OnDownloadProgressChanged(ByVal sender As Object, ByVal e As DownloadProgressChangedEventArgs)

        Dim totalSize As Long = e.TotalBytesToReceive
        totalsize2 = totalSize
        Dim downloadedBytes As Long = e.BytesReceived
        Dim percentage As Integer = e.ProgressPercentage
        PB1.Value = percentage
        PB1.Text = (My.Resources.strDownloaded & Helper.SizeSuffix(downloadedBytes) & " / " & Helper.SizeSuffix(totalSize) & " (" & percentage & "%) - " & My.Resources.strRightClickforOptions)
        'Put your progress UI here, you can cancel download by uncommenting the line below
        'wc.CancelAsync()

    End Sub
    Public Sub OnFileDownloadCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs)
        PB1.Value = 0
        PB1.Text = ""
    End Sub

    Public Function DLWUA(ByVal Address As String, ByVal Filename As String, ByVal Overwrite As String) As Boolean
        Overwrite = Application.StartupPath & "\" & Overwrite
        'Appeler la fonction avec: DLWUA(URL, Emplacement fichier, append ou overwrite (true ou false))

        'Dim Fichier As Byte()
        AddHandler DLS.DownloadProgressChanged, AddressOf OnDownloadProgressChanged
        AddHandler DLS.DownloadFileCompleted, AddressOf OnFileDownloadCompleted

        DLS.Headers.Add("user-agent", "AQUA_HTTP")
        DLS.timeout = 10000

        ' TODO: Why would you do this T_T -Matthew

        For i As Integer = 1 To 5
            Try
                Application.DoEvents()
                DLS.DownloadFileAsync((New Uri(Address)), Filename)
                Application.DoEvents()
                Exit For
            Catch ex As Exception
                If i = 4 Then Threading.Thread.Sleep(5000)
                If i = 5 Then Return False
            End Try
        Next

        'My.Computer.FileSystem.WriteAllBytes(File, Fichier, Ajout)

        While DLS.IsBusy = True
            Application.DoEvents()
            If Restartplz = True Then
                DLS.CancelAsync()
                Restartplz = False
                While DLS.IsBusy = True

                End While
                DLS.DownloadFileAsync((New Uri(Address)), Filename)
            End If
            If Me.Visible = False Then
                DLS.CancelAsync()
                Cancelled = True
                Application.Exit()
            End If
        End While

        Return True
    End Function
    Public Function GetMD5(ByVal fichier As String) As String

        'Pour faire marcher la fonction il suffit d'ajouter GetMD5("C:\fichier.extension") dans une sub

        If IO.File.Exists(fichier) Then
            Dim st As FileStream = Nothing
            Try
                Dim check As New System.Security.Cryptography.MD5CryptoServiceProvider
                st = File.Open(fichier, FileMode.Open, FileAccess.Read)
                Dim somme As Byte() = check.ComputeHash(st)
                Dim ret As String = ""
                For Each a As Byte In somme
                    If (a < 16) Then
                        ret += "0" + a.ToString("X")
                    Else
                        ret += a.ToString("X")
                    End If
                Next
                Return ret
            Catch ex As Exception
                Exit Try
            Finally
                If st IsNot Nothing Then st.Close()
            End Try
        Else
            Return ""
        End If
        Return ""

    End Function
    Private Sub LockGUI()
        Me.Enabled = False
    End Sub

    Private Sub UnlockGUI()
        Me.Enabled = True
    End Sub
    Public Sub Log(ByRef Text As String)
        Dim TimeFormatted As String
        Dim time As DateTime = DateTime.Now
        TimeFormatted = time.ToString("G")
        File.AppendAllText((Application.StartupPath & "\logfile.txt"), TimeFormatted & ": DEBUG - " & Text & vbCrLf)
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
        Dim filename As String()
        Dim truefilename As String
        Dim sr As StreamReader = File.OpenText("patchlist.txt")
        ' Store it in a string variable for now
        Dim strTemp As String = sr.ReadToEnd
        sr.Close()

        Dim sr2 As StreamReader = File.OpenText("patchlist_old.txt")
        Dim strTemp2 As String = sr2.ReadToEnd
        sr2.Close()

        Dim objWriter As New StreamWriter("ALLOFTHETHINGS.txt")
        objWriter.WriteLine((strTemp & strTemp2))
        objWriter.Close()

        'Process.Start("notepad", "ALLOFTHETHINGS.txt")

        Dim sr3 = New StreamReader("ALLOFTHETHINGS.txt")


        Dim sw3 = New StreamWriter("SOMEOFTHETHINGS.txt")
        Dim MyArray As New ArrayList
        Dim strLine As String

        'Dim objWriter2 As New StreamWriter("CHECKME.txt")
        'objWriter2.WriteLine("")
        'objWriter2.Close()

        Do While sr3.Peek <> -1
            strLine = sr3.ReadLine()
            filename = Regex.Split(strLine, ".pat")
            truefilename = filename(0).Replace("data/win32/", "")
            'MsgBox(truefilename)
            If MyArray.Contains(truefilename) = False Then
                'File.AppendAllText("CHECKME.txt", (truefilename & vbCrLf))
                MyArray.Add(truefilename)
                sw3.WriteLine(strLine)
            End If
        Loop
        sr3.Close()
        sw3.Close()
        'Dim sText As String = File.ReadAllText("SOMEOFTHETHINGS.txt")
        'Dim sNewLines As String() = sText.Split(New Char() {ControlChars.Lf}, StringSplitOptions.RemoveEmptyEntries)
        'File.WriteAllLines("SOMEOFTHETHINGS.txt", sNewLines)
        'MsgBox("Duplicates Removed!")
        'Process.Start("notepad", "SOMEOFTHETHINGS.txt")
    End Sub
    Public Sub MergePrePatches()
        Dim filename As String()
        Dim truefilename As String
        Dim sr As StreamReader = File.OpenText("patchlist0.txt")
        ' Store it in a string variable for now
        Dim strTemp As String = sr.ReadToEnd
        sr.Close()

        Dim sr2 As StreamReader = File.OpenText("patchlist1.txt")
        Dim strTemp2 As String = sr2.ReadToEnd
        sr2.Close()

        Dim sr33 As StreamReader = File.OpenText("patchlist2.txt")
        Dim strTemp3 As String = sr33.ReadToEnd
        sr33.Close()

        Dim sr4 As StreamReader = File.OpenText("patchlist3.txt")
        Dim strTemp4 As String = sr4.ReadToEnd
        sr4.Close()

        Dim sr5 As StreamReader = File.OpenText("patchlist4.txt")
        Dim strTemp5 As String = sr5.ReadToEnd
        sr5.Close()

        Dim sr6 As StreamReader = File.OpenText("patchlist5.txt")
        Dim strTemp6 As String = sr6.ReadToEnd
        sr6.Close()

        Dim objWriter As New StreamWriter("ALLOFTHEPREPATCHES.txt")
        objWriter.WriteLine((strTemp & strTemp2 & strTemp3 & strTemp4 & strTemp5 & strTemp6))
        objWriter.Close()

        'Process.Start("notepad", "ALLOFTHETHINGS.txt")

        Dim sr3 = New StreamReader("ALLOFTHEPREPATCHES.txt")


        Dim sw3 = New StreamWriter("SOMEOFTHEPREPATCHES.txt")
        Dim MyArray As New ArrayList
        Dim strLine As String

        'Dim objWriter2 As New StreamWriter("CHECKME.txt")
        'objWriter2.WriteLine("")
        'objWriter2.Close()

        Do While sr3.Peek <> -1
            strLine = sr3.ReadLine()
            filename = Regex.Split(strLine, ".pat")
            truefilename = filename(0).Replace("data/win32/", "")
            'MsgBox(truefilename)
            If MyArray.Contains(truefilename) = False And truefilename <> "" Then
                'File.AppendAllText("CHECKME.txt", (truefilename & vbCrLf))
                MyArray.Add(truefilename)
                sw3.WriteLine(strLine)
            End If
        Loop
        sr3.Close()
        sw3.Close()
        'Dim sText As String = File.ReadAllText("SOMEOFTHETHINGS.txt")
        'Dim sNewLines As String() = sText.Split(New Char() {ControlChars.Lf}, StringSplitOptions.RemoveEmptyEntries)
        'File.WriteAllLines("SOMEOFTHETHINGS.txt", sNewLines)
        'MsgBox("Duplicates Removed!")
        'Process.Start("notepad", "SOMEOFTHETHINGS.txt")
    End Sub
    Dim p() As Process
    Private Function CheckIfRunning(ByRef ProcessName As String)
        p = Process.GetProcessesByName(ProcessName)
        Dim currentProcess As Process = Process.GetCurrentProcess()

        Dim x As Integer = 0
        If ProcessName = "PSO2 Tweaker" Then x = 1
        'MsgBox(p.Count.tostring)
        If p.Count > x Then
            'MsgBox("It seems that " & ProcessName.Replace(".exe", "") & " is already running. If you don't see it, open the task manager and end task it.")
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
        'btnCancelDownload.Visible = False
        Cancelled = True
        PB1.Value = 0
        PB1.Text = ""
        File.Delete("launcherlist.txt")
        File.Delete("patchlist.txt")
        File.Delete("patchlist_old.txt")
        File.Delete("version.ver")
        File.Delete("ALLOFTHETHINGS.txt")
        File.Delete("SOMEOFTHETHINGS.txt")
        Application.ExitThread()
    End Sub
    Private Sub frmMain_Leave(sender As Object, e As EventArgs) Handles Me.Leave
        Application.Exit()
    End Sub
    Public Sub CheckForStoryUpdates()
        Try
            If Helper.GetRegKey(Of String)("StoryPatchVersion") = "Not Installed" Then Exit Sub
            DLWUA("http://162.243.211.123/patchfiles/Story%20MD5HashList.txt", "Story MD5HashList.txt", True)
            Dim filedownloader As New Net.WebClient()
            Dim sBuffer As String
            Dim filename As String()
            Dim truefilename As String
            Dim missingfiles As New List(Of String)
            Dim filedownloader2 As New Net.WebClient()
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
                'MsgBox(sBuffer.ToString & " is the current version. You have: " & My.Settings.LastCheckedStoryVersion)
                If sBuffer.ToString <> Helper.GetRegKey(Of String)("StoryPatchVersion") Then
                    UpdateNeeded = True
                    'A new story patch update is available - Would you like to download and install it? PLEASE NOTE: This update assumes you've already downloaded and installed the latest RAR file available from http://arks-layer.com, which seems to be: 
                    Dim net As New Net.WebClient()
                    Dim src As String
                    src = net.DownloadString("http://arks-layer.com/story.php")
                    'MsgBox(src.ToString)
                    ' Create a match using regular exp<b></b>ressions
                    'http://arks-layer.com/Story%20Patch%208-8-2013.rar.torrent
                    'Dim m As Match = Regex.Match(src, "Story Patch (.*?).rar.torrent")
                    Dim m As Match = Regex.Match(src, "<u>.*?</u>")
                    'MsgBox(m.Value)
                    ' Spit out the value plucked from the code
                    txtHTML.Text = m.Value
                    Dim strDownloadME As String = txtHTML.Text
                    strDownloadME = strDownloadME.Replace("<u>", "")
                    strDownloadME = strDownloadME.Replace("</u>", "")
                    'MsgBox(strDownloadME)
                    'MsgBox(strDownloadME)
                    'MsgBox(Helper.GetRegKey(Of String)("LatestStoryBase"))
                    If strDownloadME <> Helper.GetRegKey(Of String)("LatestStoryBase") Then
                        Dim MBVisitLink As MsgBoxResult = MsgBox("A new story patch is available! Would you like to download and install it using the new story patch method?", MsgBoxStyle.YesNo)
                        If MBVisitLink = vbYes Then
                            btnStoryPatchNew.RaiseClick()
                            'Process.Start("http://arks-layer.com/story.php")
                            Exit Sub
                        End If
                        If MBVisitLink = vbNo Then Exit Sub
                    End If

                    Dim UpdateStoryYesNo As MsgBoxResult = MsgBox(My.Resources.strStoryModeUpdate & strDownloadME, vbYesNo)
                    If UpdateStoryYesNo = vbNo Then
                        Exit Sub
                    End If
                End If
                If UpdateNeeded = True Then
                    WriteDebugInfo(My.Resources.strBeginningStoryModeUpdate)
                    While Not (oReader.EndOfStream)
                        sBuffer = oReader.ReadLine
                        filename = Regex.Split(sBuffer, ",")
                        truefilename = filename(0)
                        TrueMD5 = filename(1)
                        If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & truefilename)) = False Then
                            missingfiles.Add(truefilename)
                            GoTo NEXTFILE1
                        End If
                        If GetMD5(((lblDirectory.Text & "\data\win32") & "\" & truefilename)) <> TrueMD5 Then
                            'MsgBox("GetMD5 gave me: " & GetMD5(((lblDirectory.Text & "\data\win32") & "\" & truefilename)) & " and the MD5 it should match is " & TrueMD5)
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
            If UpdateNeeded = True Then
                Dim totaldownload As String = missingfiles.Count
                Dim downloaded As Long = 0
                'If totaldownload > 100 Then
                'MsgBox("Unfortunately, in order to keep this service active and speedy, you'll need to download the latest RAR from the http://arks-layer.com website. Install the Story Patch from that RAR, and THEN allow it to update.")
                'Exit Sub
                'End If
                'If Helper.GetRegKey(Of String)("PatchServer") = "Patch Server #1" Then WriteDebugInfo("Downloading/Installing updates using Patch Server #1 (Japan)")
                WriteDebugInfo("Downloading/Installing updates using Patch Server #4 (New York)")
                'If Helper.GetRegKey(Of String)("PatchServer") = "Patch Server #2" Then WriteDebugInfo("Downloading/Installing updates using Patch Server #2 (North America)")
                'If Helper.GetRegKey(Of String)("PatchServer") = "Patch Server #3" Then WriteDebugInfo("Downloading/Installing updates using Patch Server #3 (Germany)")
                For Each downloadstring In missingfiles
                    'Download the missing files:
                    downloaded += 1
                    lblStatus.Text = My.Resources.strUpdating & downloaded & "/" & totaldownload
                    Application.DoEvents()
                    Cancelled = False
                    'MsgBox(Helper.GetRegKey(Of String)("PatchServer"))
                    'MsgBox("http://socket-hack.com:2312/patchfiles/" & downloadstring & ".7z")
                    'If Helper.GetRegKey(Of String)("PatchServer") = "Patch Server #1" Then DLWUA(("http://socket-hack.com:2312/patchfiles/" & downloadstring & ".7z"), downloadstring & ".7z", True)
                    'If Helper.GetRegKey(Of String)("PatchServer") = "Patch Server #2" Then DLWUA(("http://pso2.aeongames.net/patchfiles/" & downloadstring & ".7z"), downloadstring & ".7z", True)
                    'If Helper.GetRegKey(Of String)("PatchServer") = "Patch Server #3" Then DLWUA(("http://cyberkitsune.net/aida/patchfiles/" & downloadstring & ".7z"), downloadstring & ".7z", True)
                    DLWUA(("http://162.243.211.123/patchfiles/" & downloadstring & ".7z"), downloadstring & ".7z", True)
                    If Cancelled = True Then Exit Sub
                    'Delete the existing file FIRST
                    If File.Exists(downloadstring & ".7z") = False Then
                        WriteDebugInfoAndFAILED("File " & (downloadstring & ".7z") & " does not exist! Perhaps it wasn't downloaded properly?")
                    End If
                    If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & downloadstring)) Then File.Delete(((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
                    Dim process As Process = Nothing
                    Dim processStartInfo As ProcessStartInfo
                    processStartInfo = New ProcessStartInfo()
                    Dim UnRarLocation As String
                    UnRarLocation = (Application.StartupPath & "\7za.exe")
                    UnRarLocation = UnRarLocation.Replace("\\", "\")
                    processStartInfo.FileName = UnRarLocation
                    processStartInfo.Verb = "runas"
                    processStartInfo.Arguments = ("e -y " & downloadstring & ".7z")
                    processStartInfo.WindowStyle = ProcessWindowStyle.Hidden
                    processStartInfo.UseShellExecute = True
                    process = process.Start(processStartInfo)
                    Do Until process.WaitForExit(1000)
                    Loop
                    If File.Exists(downloadstring) = False Then
                        WriteDebugInfoAndFAILED("File " & (downloadstring) & " does not exist! Perhaps it wasn't extracted properly?")
                    End If
                    File.Move(downloadstring, ((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
                    File.Delete(downloadstring & ".7z")
                    Application.DoEvents()
                Next
                'If missingfiles.Count = 0 Then WriteDebugInfo("You appear to have the latest story patch files!")
                WriteDebugInfoAndOK(My.Resources.strStoryPatchUpdated)
                Helper.SetRegKey(Of String)("StoryPatchVersion", Helper.GetRegKey(Of String)("NewVersionTemp"))
                Helper.SetRegKey(Of String)("NewVersionTemp", "")
                Exit Sub
            End If
            If UpdateNeeded = False Then
                WriteDebugInfoAndOK("You have the latest story patch updates!")
                Exit Sub
            End If
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub

    Public Class MyWebClient
        Inherits WebClient

        Private _timeout As Integer

        Public Property timeout As Integer

            Get
                timeout = _timeout
            End Get

            Set(ByVal value As Integer)
                _timeout = value
            End Set
        End Property

        Public Sub MyWebClient()
            Me.timeout = 60000
        End Sub

        Protected Overrides Function GetWebRequest(ByVal address As Uri) As WebRequest
            Dim result = MyBase.GetWebRequest(address)
            result.Timeout = Me._timeout
            Return result
        End Function

    End Class
    Public Sub CheckForENPatchUpdates()
        Dim UpdateNeeded As Boolean
        Try
            If Helper.GetRegKey(Of String)("ENPatchVersion") = "Not Installed" Then
                'Dim InstalledYesNo As MsgBoxResult = MsgBox("Do you have the English patch currently installed?", vbYesNo)
                'If InstalledYesNo = vbYes Then
                'Helper.SetRegKey(Of String)("EnPatchVersion", "Installed")
                'End If
                'If InstalledYesNo = MsgBoxResult.No Then Exit Sub
                Exit Sub
            End If
            Application.DoEvents()
            Dim net As MyWebClient = New MyWebClient()
            net.timeout = 10000
            Dim src As String
            'If CheckLink("http://psumods.co.uk/viewtopic.php?f=4&t=206") <> "OK" Then
            'WriteDebugInfoAndFAILED("Failed to contact EN Patch website - Patch install/update canceled!")
            'WriteDebugInfo("Please visit http://goo.gl/YzCE7 for more information!")
            'Exit Sub
            'End If
            src = net.DownloadString("http://162.243.211.123/patches/enpatch.txt")
            ' Create a match using regular exp<b></b>ressions
            'http://pso2.arghargh200.net/pso2/patch_2013_04_24.rar
            'Dim m As Match = Regex.Match(src, "<br /><a href="".*?.rar")
            ' Spit out the value plucked from the code
            'txtHTML.Text = m.Value
            Dim strDownloadME As String = src
            'strDownloadME = strDownloadME.Replace("<br /><a href=""", "")
            'http://dl.dropboxusercontent.com/u/12757141/patch_2013_10_09.rar
            Dim Lastfilename As String() = strDownloadME.Split("/")
            Dim strVersion As String = Lastfilename(Lastfilename.Count - 1)
            'Dim strVersion As String = strDownloadME.Replace("http://pso2.arghargh200.net/pso2/", "")
            'strVersion = strVersion.Replace("http://107.170.16.100/patchbackups/", "")
            'strVersion = strVersion.Replace("http://dl.dropboxusercontent.com/u/12757141/", "")
            strVersion = strVersion.Replace(".rar", "")
            Helper.SetRegKey(Of String)("NewENVersion", strVersion)
            If strVersion <> Helper.GetRegKey(Of String)("ENPatchVersion") Then
                UpdateNeeded = True
                Dim UpdateStoryYesNo As MsgBoxResult = MsgBox(My.Resources.strNewENPatch, vbYesNo)
                If UpdateStoryYesNo = vbNo Then UpdateNeeded = False
                If UpdateNeeded = True Then
                    btnENPatch.RaiseClick()
                    'Helper.SetRegKey(Of String)("ENPatchVersion", Helper.GetRegKey(Of String)("NewVersionTemp"))
                    'Helper.SetRegKey(Of String)("NewVersionTemp", "")
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
                'Dim InstalledYesNo As MsgBoxResult = MsgBox("Do you have the English patch currently installed?", vbYesNo)
                'If InstalledYesNo = vbYes Then
                'Helper.SetRegKey(Of String)("EnPatchVersion", "Installed")
                'End If
                'If InstalledYesNo = MsgBoxResult.No Then Exit Sub
                Exit Sub
            End If
            Application.DoEvents()
            Dim net As MyWebClient = New MyWebClient()
            net.timeout = 10000
            Dim src As String
            'If CheckLink("http://psumods.co.uk/viewtopic.php?f=4&t=206") <> "OK" Then
            ' WriteDebugInfoAndFAILED("Failed to contact EN Patch website - Patch install/update canceled!")
            ' WriteDebugInfo("Please visit http://goo.gl/YzCE7 for more information!")
            'Exit Sub
            'End If
            src = net.DownloadString("http://162.243.211.123/patches/largefiles.txt")
            ' Create a match using regular exp<b></b>ressions
            'http://pso2.arghargh200.net/pso2/2013_05_22_largefiles.rar
            'Dim m As Match = Regex.Match(src, "http://pso2.arghargh200.net/pso2/.*?.rar")
            ' Spit out the value plucked from the code
            'txtHTML.Text = m.NextMatch.ToString
            Dim strDownloadME As String = src
            'Get the current version, save as NewVersionTemp
            Dim Lastfilename As String() = strDownloadME.Split("/")
            Dim strVersion As String = Lastfilename(Lastfilename.Count - 1)
            'Dim strVersion As String = strDownloadME.Replace("http://pso2.arghargh200.net/pso2/", "")
            'strVersion = strVersion.Replace("http://107.170.16.100/patchbackups/", "")
            strVersion = strVersion.Replace(".rar", "")
            Helper.SetRegKey(Of String)("NewLargeFilesVersion", strVersion)
            If strVersion <> Helper.GetRegKey(Of String)("LargeFilesVersion") Then
                UpdateNeeded = True
                Dim UpdateStoryYesNo As MsgBoxResult = MsgBox(My.Resources.strNewLargeFiles, vbYesNo)
                If UpdateStoryYesNo = vbNo Then UpdateNeeded = False
                If UpdateNeeded = True Then
                    btnLargeFiles.RaiseClick()
                    'Helper.SetRegKey(Of String)("ENPatchVersion", Helper.GetRegKey(Of String)("NewVersionTemp"))
                    'Helper.SetRegKey(Of String)("NewVersionTemp", "")
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
        If Directory.Exists(path) Then
            'Delete all files from the Directory
            For Each filepath As String In Directory.GetFiles(path)
                File.Delete(filepath)
            Next
            'Delete all child Directories
            For Each dir As String In Directory.GetDirectories(path)
                DeleteDirectory(dir)
            Next
            'Delete a Directory
            Directory.Delete(path)
        End If
    End Sub
    Public Sub CheckForPSO2Updates()
        Try
            'If Helper.GetRegKey(Of String)("StoryPatchVersion") = "Not Installed" Then Exit Sub
            'DLWUA("http://dl.dropboxusercontent.com/u/23005008/patchfiles/Story MD5HashList.txt", "Story MD5HashList.txt", True)
            Dim filedownloader As New Net.WebClient()
            Dim UpdateNeeded As Boolean
            'If Helper.GetRegKey(Of String)("PSO2PatchlistMD5") = "" Then Helper.SetRegKey(Of String)("PSO2PatchlistMD5", GetMD5("patchlist.txt"))
            'If Helper.GetRegKey(Of String)("PSO2PatchlistMD5") <> GetMD5("patchlist.txt") Then
            Dim versionclient As New MyWebClient
            versionclient.timeout = 3000
            versionclient.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
            'Precede file, syntax is Yes/No:<Dateoflastprepatch>
            versionclient.DownloadFile("http://162.243.211.123/freedom/precede.txt", "precede.txt")
            'DLWUA("http://download.pso2.jp/patch_prod/patches/version.ver", "version.ver", True)
            If ComingFromPrePatch = True Then GoTo StartPrePatch

            Dim FirstTimechecking As Boolean = False
            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("PSO2PrecedeVersion")) Then
                Dim precedefile2 As String() = File.ReadAllLines("precede.txt")
                Dim PrecedeVersion2 As String() = precedefile2(0).Split(":")
                Helper.SetRegKey(Of String)("PSO2PrecedeVersion", PrecedeVersion2(1))
                FirstTimechecking = True
            End If
            'MsgBox("Change version now.")
            Dim precedefile = File.ReadAllLines("precede.txt")
            Dim PrecedeSplit As String() = precedefile(0).Split(":")
            Dim PrecedeYesNo As String = PrecedeSplit(0)
            Dim precedeversionstring As String = PrecedeSplit(1)

            If PrecedeYesNo = "Yes" Then
                'MsgBox("Precede available!")
                'MsgBox(Helper.GetRegKey(Of String)("PSO2PrecedeVersion"))
                If Helper.GetRegKey(Of String)("PSO2PrecedeVersion") <> precedeversionstring Or FirstTimechecking = True Then
                    'MsgBox("Version is different!")
                    Dim DownloadPrepatch As MsgBoxResult = MsgBox("New pre-patch data is available to download - Would you like to download it? This is optional, and will let you download some of a large patch now, as opposed to having a larger download all at once when it is released.", MsgBoxStyle.YesNo)
                    If DownloadPrepatch = vbYes Then
StartPrePatch:
                        'Download prepatch
                        'MsgBox("Yes")
                        CancelledFull = False
                        'Dim filedownloader As New Net.WebClient()
                        Dim sBuffer As String = Nothing
                        Dim filename As String() = Nothing
                        Dim truefilename As String = Nothing
                        Dim missingfiles As New List(Of String)
                        Dim filedownloader2 As New Net.WebClient()
                        Dim missingfiles2 As New List(Of String)
                        Dim NumberofChecks As Integer = 0
                        Dim MD5 As String() = Nothing
                        Dim TrueMD5 As String = Nothing
                        Dim PSO2EXEMD5 As String = "FUCK YOU DUDU"
                        lblStatus.Text = ""
                        'LockGUI()
                        WriteDebugInfo("Downloading pre-patch filelist...")
                        DLWUA("http://download.pso2.jp/patch_prod/patches_precede/patchlist0.txt", "patchlist0.txt", True)
                        DLWUA("http://download.pso2.jp/patch_prod/patches_precede/patchlist1.txt", "patchlist1.txt", True)
                        DLWUA("http://download.pso2.jp/patch_prod/patches_precede/patchlist2.txt", "patchlist2.txt", True)
                        DLWUA("http://download.pso2.jp/patch_prod/patches_precede/patchlist3.txt", "patchlist3.txt", True)
                        DLWUA("http://download.pso2.jp/patch_prod/patches_precede/patchlist4.txt", "patchlist4.txt", True)
                        DLWUA("http://download.pso2.jp/patch_prod/patches_precede/patchlist5.txt", "patchlist5.txt", True)
                        WriteDebugInfoSameLine(My.Resources.strDone)
                        'UnlockGUI()
                        MergePrePatches()
                        If Directory.Exists(lblDirectory.Text & "\_precede\data\win32") = False Then Directory.CreateDirectory(lblDirectory.Text & "\_precede\data\win32") 'create directory
                        'MsgBox("See if directory was created!")

                        WriteDebugInfo("Checking for already existing precede files...")
                        NumberofChecks = 0
                        missingfiles.Clear()
                        Using oReader As StreamReader = File.OpenText("SOMEOFTHEPREPATCHES.txt")
                            If CancelledFull = True Then Exit Sub
                            While Not (oReader.EndOfStream)
                                If CancelledFull = True Then Exit Sub
                                sBuffer = oReader.ReadLine
                                filename = Regex.Split(sBuffer, ".pat")
                                truefilename = filename(0).Replace("data/win32/", "")
                                MD5 = Regex.Split(filename(1), vbTab)
                                TrueMD5 = MD5(2)
                                If truefilename <> "GameGuard.des" And truefilename <> "PSO2JP.ini" And truefilename <> "script/user_default.pso2" And truefilename <> "script/user_intel.pso2" Then
                                    If truefilename = "pso2.exe" Then
                                        PSO2EXEMD5 = TrueMD5
                                        GoTo NextFile1
                                    End If
                                    If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\_precede\data\win32") & "\" & truefilename)) = False Then
                                        'MsgBox((lblDirectory.Text & "\data\win32"))
                                        'MsgBox("\")
                                        'MsgBox(truefilename)
                                        If VedaUnlocked = True Then WriteDebugInfo("DEBUG: The file " & truefilename & " is missing.")
                                        missingfiles.Add(truefilename)
                                        GoTo NEXTFILE1
                                    End If
                                    If GetMD5(((lblDirectory.Text & "\_precede\data\win32") & "\" & truefilename)) <> TrueMD5 Then
                                        'MsgBox((lblDirectory.Text & "\data\win32"))
                                        'MsgBox("\")
                                        'MsgBox(truefilename)
                                        If VedaUnlocked = True Then WriteDebugInfo("DEBUG: The file " & truefilename & " must be redownloaded.")
                                        missingfiles.Add(truefilename)
                                        GoTo NEXTFILE1
                                    End If
                                End If
NEXTFILE1:
                                NumberofChecks += 1
                                lblStatus.Text = (My.Resources.strCurrentlyCheckingFile & NumberofChecks & "")
                                Application.DoEvents()
                                'If missingfiles.Count > 30 Then Exit While
                            End While
                        End Using
                        Dim totaldownload As String = missingfiles.Count
                        Dim downloaded As Long = 0
                        Dim totaldownloaded As Long = 0
                        patching = True

                        For Each downloadstring In missingfiles
                            If CancelledFull = True Then Exit Sub
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
                            If File.Exists(downloadstring) = True Then length2 = info7.Length
                            If info7.Length = 0 Then
                                Log("File appears to be empty, trying to download from secondary SEGA server")
                                DLWUA(("http://download.pso2.jp/patch_prod/patches_precede/data/win32/" & downloadstring & ".pat"), downloadstring, True)
                            End If

                            If Cancelled = True Then Exit Sub
                            'filedownloader.DownloadFile(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadstring & ".pat"), downloadstring)
                            'DLWUA("http://download.pso2.jp/patch_prod/patches/launcherlist.txt", "launcherlist.txt", True)
                            'Delete the existing file FIRST
                            If File.Exists(((lblDirectory.Text & "\_precede\data\win32") & "\" & downloadstring)) Then File.Delete(((lblDirectory.Text & "\_precede\data\win32") & "\" & downloadstring))
                            File.Move(downloadstring, ((lblDirectory.Text & "\_precede\data\win32") & "\" & downloadstring))
                            If VedaUnlocked = True Then WriteDebugInfo("DEBUG: Downloaded and installed " & downloadstring & ".")
                            'Dim linesList As New List(Of String)(File.ReadAllLines("resume.txt"))

                            'Remove the line to delete, e.g.
                            'linesList.Remove(downloadstring)

                            'File.WriteAllLines("resume.txt", linesList.ToArray())
                            'MsgBox("removed this line - " & downloadstring)
                            Application.DoEvents()
                            'MsgBox(downloadstring)
                        Next
                        patching = False
                        If missingfiles.Count = 0 Then WriteDebugInfo("Your precede data is up to date!")
                        If missingfiles.Count <> 0 Then WriteDebugInfo("Precede data downloaded/updated!")
                        Dim precedefile2 As String() = File.ReadAllLines("precede.txt")
                        Dim PrecedeVersion2 As String() = precedefile2(0).Split(":")
                        Helper.SetRegKey(Of String)("PSO2PrecedeVersion", PrecedeVersion2(1))
                        GoTo BackToCheckUpdates
                    End If
                End If
            End If
BackToCheckUpdates:
            'Check whether or not to apply pre-patch shit. Ugh.
            If Directory.Exists(lblDirectory.Text & "\_precede\") = True Then
                versionclient.DownloadFile("http://162.243.211.123/freedom/precede_apply.txt", "precede_apply.txt")
                Dim prepatchapply = File.ReadAllLines("precede_apply.txt")
                Dim ApplyPrePatch As String = prepatchapply(0)
                If Directory.Exists(lblDirectory.Text & "\_precede\data\win32\") = False Then GoTo BackToCheckUpdates2
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
                        Dim di As New IO.DirectoryInfo(lblDirectory.Text & "\_precede\data\win32\")
                        Dim diar1 As IO.FileInfo() = di.GetFiles()
                        Dim dra As IO.FileInfo

                        'list the names of all files in the specified directory
                        Dim downloadstring As String = ""
                        Dim count As Integer = 0
                        Dim counter = My.Computer.FileSystem.GetFiles(lblDirectory.Text & "\_precede\data\win32\")
                        For Each dra In diar1
                            If counter.Count = 0 Then Exit For
                            downloadstring = dra.Name
                            If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & downloadstring)) Then File.Delete(((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
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
            If ComingFromPrePatch = True Then Exit Sub
            If String.IsNullOrEmpty(Helper.GetRegKey(Of String)("PSO2RemoteVersion")) Then
                Dim lines2 = File.ReadAllLines("version.ver")
                Dim RemoteVersion2 As String = lines2(0)
                Helper.SetRegKey(Of String)("PSO2RemoteVersion", RemoteVersion2)
            End If
            'MsgBox("Change version now.")
            Dim lines = File.ReadAllLines("version.ver")
            Dim RemoteVersion As String = lines(0)
            If Helper.GetRegKey(Of String)("PSO2RemoteVersion") <> RemoteVersion Then
                UpdateNeeded = True

                Dim UpdateStoryYesNo As MsgBoxResult = MsgBox(My.Resources.strNewPSO2Update, vbYesNo)
                If UpdateStoryYesNo = vbNo Then UpdateNeeded = False
                'End If
            End If

            If UpdateNeeded = True Then
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
            'LockGUI()
            If (Directory.Exists((lblDirectory.Text & "\data\win32")) = False OrElse lblDirectory.Text = "lblDirectory") Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                Button1.RaiseClick()
                Exit Sub
            End If
            Log("Restoring/Removing files...")
            If chkRemoveCensor.Checked = True And chkRestoreCensor.Checked = True Then
                MsgBox(My.Resources.strYouCannotRemoveRestore)
                Exit Sub
            End If
            If chkRemoveNVidia.Checked = True And chkRestoreNVidia.Checked = True Then
                MsgBox(My.Resources.strYouCannotRemoveRestore)
                Exit Sub
            End If
            If chkRemovePC.Checked = True And chkRestorePC.Checked = True Then
                MsgBox(My.Resources.strYouCannotRemoveRestore)
                Exit Sub
            End If
            If chkRemoveVita.Checked = True And chkRestoreVita.Checked = True Then
                MsgBox(My.Resources.strYouCannotRemoveRestore)
                Exit Sub
            End If
            If chkRemoveSEGA.Checked = True And chkRestoreSEGA.Checked = True Then
                MsgBox(My.Resources.strYouCannotRemoveRestore)
                Exit Sub
            End If
            'Remove censor
            '[AIDA] Resume here
            If chkRemoveCensor.Checked = True And My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c")) = True Then
                If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c"), "ffbff2ac5b7a7948961212cefd4d402c.backup")
                WriteDebugInfoAndOK(My.Resources.strRemoving & "Censor...")
            ElseIf chkRemoveCensor.Checked = True And My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c")) = False Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRemoveCensor)
            End If
            'Restore Censor
            If chkRestoreCensor.Checked = True And My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup")) = True Then
                If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"), "ffbff2ac5b7a7948961212cefd4d402c")
                WriteDebugInfoAndOK(My.Resources.strRestoring & "Censor...")
            ElseIf chkRestoreCensor.Checked = True And My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup")) = False Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRestoreCensor)
            End If
            'Remove PC Opening Video [Done]
            If chkRemovePC.Checked = True And My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) = True Then
                If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927"), "a44fbb2aeb8084c5a5fbe80e219a9927.backup")
                WriteDebugInfoAndOK(My.Resources.strRemoving & "PC Opening Video...")
            ElseIf chkRemovePC.Checked = True And My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) = False Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRemovePC)
            End If
            'Restore PC Opening Video [Done]
            If chkRestorePC.Checked = True And My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup")) = True Then
                If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup"), "a44fbb2aeb8084c5a5fbe80e219a9927")
                WriteDebugInfoAndOK(My.Resources.strRestoring & "PC Opening Video...")
            ElseIf chkRestorePC.Checked = True And My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup")) = False Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRestorePC)
            End If
            'Remove Vita Opening Video [Done]
            If chkRemoveVita.Checked = True And My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585")) = True Then
                If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585.backup")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585.backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585"), "a93adc766eb3510f7b5c279551a45585.backup")
                WriteDebugInfoAndOK(My.Resources.strRemoving & "Vita Opening Video...")
            ElseIf chkRemoveVita.Checked = True And My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585")) = False Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRemoveVita)
            End If
            'Restore Vita Opening Video [Done]
            If chkRestoreVita.Checked = True And My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585.backup")) = True Then
                If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585.backup"), "a93adc766eb3510f7b5c279551a45585")
                WriteDebugInfoAndOK(My.Resources.strRestoring & "Vita Opening Video...")
            ElseIf chkRestoreVita.Checked = True And My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585.backup")) = False Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRestoreVita)
            End If
            'Remove NVidia Opening Video [Done]
            If chkRemoveNVidia.Checked = True And My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "7f2368d207e104e8ed6086959b742c75")) = True Then
                If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "7f2368d207e104e8ed6086959b742c75.backup")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "7f2368d207e104e8ed6086959b742c75.backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "7f2368d207e104e8ed6086959b742c75"), "7f2368d207e104e8ed6086959b742c75.backup")
                WriteDebugInfoAndOK(My.Resources.strRemoving & "NVidia Opening Video...")
            ElseIf chkRemoveNVidia.Checked = True And My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "7f2368d207e104e8ed6086959b742c75")) = False Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRemoveNVidia)
            End If
            'Restore NVidia Opening Video [Done]
            If chkRestoreNVidia.Checked = True And My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "7f2368d207e104e8ed6086959b742c75.backup")) = True Then
                If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "7f2368d207e104e8ed6086959b742c75")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "7f2368d207e104e8ed6086959b742c75"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "7f2368d207e104e8ed6086959b742c75.backup"), "7f2368d207e104e8ed6086959b742c75")
                WriteDebugInfoAndOK(My.Resources.strRestoring & "NVidia Opening Video...")
            ElseIf chkRestoreNVidia.Checked = True And My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "7f2368d207e104e8ed6086959b742c75.backup")) = False Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRestoreNVidia)
            End If
            'Remove SEGA Opening Video [Done]
            If chkRemoveSEGA.Checked = True And My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "009bfec69b04a34576012d50e3417771")) = True Then
                If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "009bfec69b04a34576012d50e3417771.backup")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "009bfec69b04a34576012d50e3417771.backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "009bfec69b04a34576012d50e3417771"), "009bfec69b04a34576012d50e3417771.backup")
                WriteDebugInfoAndOK(My.Resources.strRemoving & "SEGA Opening Video...")
            ElseIf chkRemoveSEGA.Checked = True And My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "009bfec69b04a34576012d50e3417771")) = False Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRemoveSEGA)
            End If
            'Restore SEGA Opening Video [Done]
            If chkRestoreSEGA.Checked = True And My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "009bfec69b04a34576012d50e3417771.backup")) = True Then
                If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "009bfec69b04a34576012d50e3417771")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "009bfec69b04a34576012d50e3417771"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "009bfec69b04a34576012d50e3417771.backup"), "009bfec69b04a34576012d50e3417771")
                WriteDebugInfoAndOK(My.Resources.strRestoring & "SEGA Opening Video...")
            ElseIf chkRestoreSEGA.Checked = True And My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "009bfec69b04a34576012d50e3417771.backup")) = False Then
                WriteDebugInfoAndWarning(My.Resources.strFailedToRestoreSEGA)
            End If
            UnlockGUI()
            'Swap PC and Vita Openings
            'Restore PC Opening Video [Done]
            If chkSwapOP.Checked = True Then
                WriteDebugInfo(My.Resources.strSwappingOpenings)
                If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup")) = True Then
                    If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                    My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup"), "a44fbb2aeb8084c5a5fbe80e219a9927")
                    WriteDebugInfoAndOK(My.Resources.strRestoring & "PC Opening Video...")
                    'ElseIf My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup")) = False Then
                    'WriteDebugInfoAndWarning(My.Resources.strRestoring & "PC Opening Video...")
                End If
                'Restore Vita Opening Video [Done]
                If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585.backup")) = True Then
                    If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                    My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585.backup"), "a93adc766eb3510f7b5c279551a45585")
                    WriteDebugInfoAndOK(My.Resources.strRestoring & "Vita Opening Video...")
                    'ElseIf My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585.backup")) = False Then
                    'WriteDebugInfoAndWarning(My.Resources.strRestoring & "Vita Opening Video...")
                End If
                'Rename the original files
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927"), "PCOpening")
                'WriteDebugInfoAndOK("Renaming PC Opening file...")
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585"), "VitaOpening")
                'WriteDebugInfoAndOK("Renaming Vita Opening file...")
                'Rename them back, swapping them~
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "PCOpening"), "a93adc766eb3510f7b5c279551a45585")
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "VitaOpening"), "a44fbb2aeb8084c5a5fbe80e219a9927")
                'WriteDebugInfoAndOK("Swapping Files...")
            End If
            If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) = True And My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585")) = True Then
                If GetFileSize(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) = 167479840 And GetFileSize(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585")) = 151540352 Then
                    chkSwapOP.Text = My.Resources.strSwapPCVitaOpenings & "(" & My.Resources.strNotSwapped & ")"
                    WriteDebugInfo(My.Resources.strallDone)
                    UnlockGUI()
                    Exit Sub
                End If
                If GetFileSize(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) = 151540352 And GetFileSize(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585")) = 167479840 Then
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

    Private Sub ButtonItem6_Click(sender As Object, e As EventArgs) Handles ButtonItem6.Click
        'Dim t5 As New Threading.Thread(AddressOf Injectstuff)
        't5.IsBackground = True
        't5.Start()
        'Fuck SEGA. Stupid jerks.
        'If Helper.GetRegKey(Of String)("SeenFuckSEGAMessage") = "False" Then MsgBox("SEGA recently updated the pso2.exe file so that it can't be launched from anything but the official launcher. You can still use this to patch, fix, apply patches, and everything you did before. Once you're ready to launch the game, however, the launcher will open the PSO2JP launcher. Simply click the large button to launch the game. Sorry about the inconvience, I'll try to see if I can find a way around it soon! (This message will not appear again.)" & vbCrLf & "- AIDA")
        'Helper.SetRegKey(Of String)("SeenFuckSEGAMessage", "True")
        Log("Check if PSO2 is running")
        If CheckIfRunning("pso2") = "Running" Then Exit Sub
        'If CheckIfRunning("pso2.exe") = "Running" Then Exit Sub
        Try
            If (Directory.Exists((lblDirectory.Text & "\data\win32")) = False OrElse lblDirectory.Text = "lblDirectory") Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                Button1.RaiseClick()
                Exit Sub
            End If
            Dim DirectoryString As String
            Dim pso2launchpath As String
            DirectoryString = (lblDirectory.Text & "\data\win32")
            pso2launchpath = DirectoryString.Replace("\data\win32", "")
            If File.Exists(pso2launchpath & "\pso2.exe") = False Then
                WriteDebugInfoAndFAILED(My.Resources.strCouldNotFindPSO2EXE)
                Exit Sub
            End If
            'Log("Cancel any downloads")
            DLS.CancelAsync()
            'btnCancelDownload.Visible = False
            Cancelled = True
            PB1.Value = 0
            PB1.Text = ""
            'Log("PSO2 Launch patch is " & pso2launchpath)
            'MsgBox(pso2launchpath)
            WriteDebugInfo(My.Resources.strLaunchingPSO2)
            'If UseItemTranslation = True Then
            Dim dir As String
            dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            'Log("Delete item cache")
            If File.Exists(dir & "\SEGA\PHANTASYSTARONLINE2\item_name_cache.dat") Then File.Delete(dir & "\SEGA\PHANTASYSTARONLINE2\item_name_cache.dat")
            'Check to see if the keys exist
            'for x64
            'HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion\Windows
            'for x86
            'HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows
            'Keys are:
            'LoadAppInit_DLLs (0 for no, 1 for yes)
            'AppInit_DLLs (string, pointing to the file)
            'If it's 64-bit
            'Fuck this shit~
            'Log("Make sure the item translation is working atm")
            If chkItemTranslation.Checked = True Then
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
                'Log("Delete the check")
                File.Delete("working.txt")
                If GetMD5(pso2launchpath & "\translator.dll") <> Helper.GetRegKey(Of String)("DLLMD5") Then
                    MsgBox(My.Resources.strTranslationFilesDontMatch)
                    Exit Sub
                End If
            End If
            'If Environment.Is64BitOperatingSystem = True Then
            'Helper.SetRegKey(Of String)("AppInit_DLLs_backup", Helper.GetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion\Windows", "AppInit_DLLs", Nothing))
            'Helper.SetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion\Windows", "AppInit_DLLs", "Translator.dll", Microsoft.Win32.RegistryValueKind.String)
            'Helper.SetRegKey(Of Integer)("LoadAppInit_DLLs_backup", Helper.GetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion\Windows", "LoadAppInit_DLLs", Nothing))
            'Helper.SetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion\Windows", "LoadAppInit_DLLs", 1, Microsoft.Win32.RegistryValueKind.DWord)
            'End If
            'If it's 32-bit
            'If Environment.Is64BitOperatingSystem = False Then
            'Helper.SetRegKey(Of String)("AppInit_DLLs_backup", Helper.GetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows", "AppInit_DLLs", Nothing))
            'Helper.SetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows", "AppInit_DLLs", "Translator.dll", Microsoft.Win32.RegistryValueKind.String)
            'Helper.SetRegKey(Of Integer)("LoadAppInit_DLLs_backup", Helper.GetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows", "LoadAppInit_DLLs", Nothing))
            'Helper.SetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows", "LoadAppInit_DLLs", 1, Microsoft.Win32.RegistryValueKind.DWord)
            'End If
            'End Item Translation stuff
            File.Delete(pso2launchpath & "\ddraw.dll")
            SaveToDisk("ddraw.dll", pso2launchpath & "\ddraw.dll")
            'Log("Start building the process")
            Dim startInfo As ProcessStartInfo
            startInfo = New ProcessStartInfo
            'Log("Set ENV Variable")
            startInfo.EnvironmentVariables("-pso2") = "+0x01e3f1e9"
            'Log("Set filename")
            startInfo.FileName = (pso2launchpath & "\pso2.exe")
            'Log("Set arugments")
            startInfo.Arguments = "+0x33aca2b9"
            startInfo.UseShellExecute = False
            Dim shell As Process
            shell = New Process
            shell.StartInfo = startInfo
            'Log("Launch the game!")
            Try
                shell.Start()
            Catch ex As Exception
                WriteDebugInfo(My.Resources.strItSeemsThereWasAnError)
                DLWUA("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", "pso2.exe", True)
                Dim DirectoryString2 As String
                DirectoryString2 = (lblDirectory.Text & "\data\win32")
                DirectoryString2 = DirectoryString2.Replace("\data\win32", "")
                If File.Exists((DirectoryString2 & "\pso2.exe")) And Application.StartupPath <> DirectoryString2 Then File.Delete((DirectoryString2 & "\pso2.exe"))
                File.Move("pso2.exe", (DirectoryString2 & "\pso2.exe"))
                WriteDebugInfoSameLine(My.Resources.strDone)
                shell.Start()
            End Try
            'Process.Start((pso2launchpath & "\pso2launcher.exe"))
            'Process.Start((pso2launchpath & "\pso2.exe"), "-pso2")
            'If File.Exists("launcherlist.txt") = True Then File.Delete("launcherlist.txt")
            'If File.Exists("patchlist.txt") = True Then File.Delete("patchlist.txt")
            'If File.Exists("patchlist_old.txt") = True Then File.Delete("patchlist_old.txt")
            'If File.Exists("version.ver") = True Then File.Delete("version.ver")
            'If UseItemTranslation = False Then
            'If UseItemTranslation = True Then
            Me.Hide()
            'Do Until File.Exists(pso2launchpath & "\ddraw.dll") = False
            Dim hWnd As IntPtr = FindWindow("Phantasy Star Online 2", Nothing)

            Do Until hWnd <> IntPtr.Zero
                hWnd = FindWindow("Phantasy Star Online 2", Nothing)
                Thread.Sleep(10)
            Loop

            File.Delete(pso2launchpath & "\ddraw.dll")
            'Loop
            'Check to see if the keys exist
            'for x64
            'HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion\Windows
            'for x86
            'HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows
            'Keys are:
            'LoadAppInit_DLLs (0 for no, 1 for yes)
            'AppInit_DLLs (string, pointing to the file)
            'If it's 64-bit
            'If Environment.Is64BitOperatingSystem = "True" Then
            'Helper.SetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion\Windows", "AppInit_DLLs", Helper.GetRegKey(Of String)("AppInit_DLLs_backup"), Microsoft.Win32.RegistryValueKind.String)
            'Helper.SetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion\Windows", "LoadAppInit_DLLs", Helper.GetRegKey(Of Integer)("LoadAppInit_DLLs_backup"), Microsoft.Win32.RegistryValueKind.DWord)
            'End If
            'If it's 32-bit
            'If Environment.Is64BitOperatingSystem = "False" Then
            'Helper.SetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows", "AppInit_DLLs", Helper.GetRegKey(Of String)("AppInit_DLLs_backup"), Microsoft.Win32.RegistryValueKind.String)
            'Helper.SetRegKey("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows", "LoadAppInit_DLLs", Helper.GetRegKey(Of Integer)("LoadAppInit_DLLs_backup"), Microsoft.Win32.RegistryValueKind.DWord)
            'End If
            'End Item Translation stuff
            'End If
            'If Helper.GetRegKey(Of String)("CloseAfterLaunch") = True Then btnExit.RaiseClick()
            Me.Close()
            'End If
            'If UseItemTranslation = True Then
            'Me.WindowState = FormWindowState.Minimized
            'End If
            'Do Until shell.WaitForExit(1000)
            'Loop
            'Me.WindowState = FormWindowState.Normal
            'WriteDebugInfo("Detected PSO2 was closed.")
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub
    Public Sub SaveToDisk(ByVal resourceName As String, ByVal fileName As String)
        ' TODO: WHAT IS THIS THING!~>??

        ' Get a reference to the running application.
        Dim assy As [Assembly] = [Assembly].GetExecutingAssembly()

        ' Loop through each resource, looking for the image name (case-insensitive).
        For Each resource As String In assy.GetManifestResourceNames()
            If resource.ToLower().IndexOf(resourceName.ToLower) <> -1 Then
                ' Get the embedded file from the assembly as a MemoryStream.
                Using resourceStream As Stream = assy.GetManifestResourceStream(resource)
                    If resourceStream IsNot Nothing Then
                        Using reader As New BinaryReader(resourceStream)
                            ' Read the bytes from the input stream.
                            Dim buffer As Byte() = reader.ReadBytes(CInt(resourceStream.Length))
                            Using outputStream As New FileStream(fileName, FileMode.Create)
                                Using writer As New BinaryWriter(outputStream)
                                    ' Write the bytes to the output stream.
                                    writer.Write(buffer)
                                End Using
                            End Using
                        End Using
                    End If
                End Using
                Exit For
            End If
        Next resource
    End Sub
    Private Sub PB1_Click(sender As Object, e As MouseEventArgs) Handles PB1.Click
        If e.Button = Windows.Forms.MouseButtons.Right Then
            If DLS.IsBusy = True Then
                CancelDownloadToolStripMenuItem.Visible = True
                ContextMenuStrip1.Show(CType(sender, Control), e.Location)
            End If
            If DLS.IsBusy = False Then
                CancelDownloadToolStripMenuItem.Visible = False
                ContextMenuStrip1.Show(CType(sender, Control), e.Location)
            End If
        End If
    End Sub

    Private Sub CancelDownloadToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CancelDownloadToolStripMenuItem.Click
        DLS.CancelAsync()
        WriteDebugInfo(My.Resources.strDownloadwasCancelled)
        'btnCancelDownload.Visible = False
        Cancelled = True
        PB1.Value = 0
        PB1.Text = ""
        lblStatus.Text = ""
        patching = False
    End Sub
    Private Function GrabLink(ByRef urlIdentifier As String)
        Dim net As New Net.WebClient()
        Dim src As String
        src = net.DownloadString("http://psumods.co.uk/viewtopic.php?f=4&t=206")
        Dim regx As New Regex("http://([\w+?\.\w+])+([a-zA-Z0-9\~\!\@\#\$\%\^\&amp;\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?", RegexOptions.IgnoreCase)
        Dim mactches As MatchCollection = regx.Matches(src)
        Dim returnURL As String = ""
        For Each match As Match In mactches
            If match.ToString.Contains(urlIdentifier) Then returnURL = match.ToString
        Next
        If String.IsNullOrEmpty(returnURL) Then
            Return "Error! URL not found!"
            Exit Function
        End If
        Return returnURL
    End Function
    Private Sub seconds_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles seconds.Tick
        Me.seconds.Interval = 10
        Me.timer_start += 1
        'Me.Label3.Text = Format(timer_start, "0.000") / 100 & " s"
    End Sub
    Private Sub WebBrowser1_DocumentCompleted(ByVal sender As Object, ByVal e As WebBrowserDocumentCompletedEventArgs) Handles WebBrowser1.DocumentCompleted
        Me.seconds.Stop()
        Me.time_for_download = timer_start * 10
        Me.velocity = testfile_Size / time_for_download * 1000
        WriteDebugInfoSameLine(" Done!")
        WriteDebugInfo(My.Resources.strYourDownloadSpeedIs & (Format(velocity, "0.0000") & " MB/s"))
        DoneDownloading = True
    End Sub

    Public Function Ping(ByVal server As String) As String
        'Switch is a class already, for what it is worth
        Dim Response As Net.NetworkInformation.PingReply
        Dim SendPing As New Net.NetworkInformation.Ping

        Try
            Response = SendPing.Send(server)
            If Response.Status = Net.NetworkInformation.IPStatus.Success Then
                Return Response.RoundtripTime.ToString
            End If
        Catch ex As Exception
        End Try

        Return "ERROR"
    End Function

    Private Sub CancelProcessToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CancelProcessToolStripMenuItem.Click
        If DLS.IsBusy = True Then DLS.CancelAsync()
        'WriteDebugInfo(My.Resources.strDownloadwasCancelled)
        'btnCancelDownload.Visible = False
        Cancelled = True
        PB1.Value = 0
        PB1.Text = ""
        lblStatus.Text = ""
        patching = False
        CancelledFull = True
        WriteDebugInfo(My.Resources.strProcessWasCancelled)
        UnlockGUI()
    End Sub
    Private Sub frmMain_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        'Try
        'If Me.WindowState = FormWindowState.Minimized Then
        'Me.WindowState = FormWindowState.Minimized
        'NotifyIcon1.Visible = True
        'Me.Hide()
        'End If
        'Catch ex As Exception
        'MsgBox(ex.Message)
        'End Try
    End Sub

    Private Sub NotifyIcon1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseDoubleClick
        Try
            Me.Show()
            Me.WindowState = FormWindowState.Normal
            NotifyIcon1.Visible = False
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub WebBrowser2_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles WebBrowser2.DocumentCompleted
        Me.seconds.Stop()
        Me.time_for_download = timer_start * 10
        Me.velocity = testfile_Size / time_for_download * 1000
        WriteDebugInfoSameLine(" Done!")
        WriteDebugInfo(My.Resources.strYourDownloadSpeedIs & (Format(velocity, "0.0000") & " MB/s"))
        DoneDownloading = True
    End Sub

    Private Sub WebBrowser3_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles WebBrowser3.DocumentCompleted
        Me.seconds.Stop()
        Me.time_for_download = timer_start * 10
        Me.velocity = testfile_Size / time_for_download * 1000
        WriteDebugInfoSameLine(" Done!")
        WriteDebugInfo(My.Resources.strYourDownloadSpeedIs & (Format(velocity, "0.0000") & " MB/s"))
        DoneDownloading = True
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Try
            Log("Selecting PSO2 Directory...")
SelectPSO2Folder:
            Dim MyFolderBrowser As New FolderBrowserDialog
            ' Description that displays above the dialog box control. 
            If lblDirectory.Text <> "" Then MyFolderBrowser.SelectedPath = lblDirectory.Text
            MyFolderBrowser.Description = My.Resources.strSelectPSO2win32folder2
            ' Sets the root folder where the browsing starts from 
            MyFolderBrowser.RootFolder = Environment.SpecialFolder.MyComputer
            Dim dlgResult As DialogResult = MyFolderBrowser.ShowDialog()
            If dlgResult = Windows.Forms.DialogResult.Cancel Then
                WriteDebugInfo("pso2_bin folder selection cancelled!")
                Exit Sub
            End If

            If MyFolderBrowser.SelectedPath.Contains("\pso2_bin\data\win32") = True Then
                If File.Exists(MyFolderBrowser.SelectedPath.Replace("\data\win32", "") & "\pso2.exe") = True Then
                    WriteDebugInfo("win32 folder selected instead of pso2_bin folder - Fixing!")
                    lblDirectory.Text = MyFolderBrowser.SelectedPath.Replace("\data\win32", "")
                    Helper.SetRegKey(Of String)("PSO2Dir", lblDirectory.Text)
                    WriteDebugInfoAndOK(lblDirectory.Text & " " & My.Resources.strSetAsYourPSO2)
                    Exit Sub
                End If
            End If
            'If File.Exists(MyFolderBrowser.SelectedPath & "\pso2.exe") = False Then
            ' MsgBox("Are you SURE you selected the pso2_bin folder? The program was unable to locate pso2.exe - Please verify the location and try again.")
            ' GoTo SelectPSO2Folder
            ' End If
            Helper.SetRegKey(Of String)("PSO2Dir", MyFolderBrowser.SelectedPath)
            lblDirectory.Text = MyFolderBrowser.SelectedPath
            'If DirectoryString.Contains("\data\win32") = False Then
            'MsgBox(My.Resources.strAreYouSurewin32 & " Please click ""Select PSO2 win32 directory"" and try again!")
            'Exit Sub
            'End If
            'If DirectoryString.Contains("\data\win32\") = True Then
            'MsgBox(My.Resources.strAreYouSurewin32NotInside & " Please click ""Select PSO2 win32 directory"" and try again!")
            'Exit Sub
            'End If

            WriteDebugInfoAndOK(lblDirectory.Text & " " & My.Resources.strSetAsYourPSO2)
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub btnLargeFiles_Click(sender As Object, e As EventArgs) Handles btnLargeFiles.Click
        DownloadPatch("http://162.243.211.123/patches/largefiles.txt", "Large Files", "LargeFiles.rar", "LargeFilesVersion", My.Resources.strWouldYouLikeToBackupLargeFiles, My.Resources.strWouldYouLikeToUse, "backupPreLargeFiles")
    End Sub

    Private Sub btnStory_Click(sender As Object, e As EventArgs) Handles btnStory.Click
        CancelledFull = False
        Dim StoryLocation As String
        'Try
        Dim backupyesno As MsgBoxResult
        If (Directory.Exists((lblDirectory.Text & "\data\win32")) = False OrElse lblDirectory.Text = "lblDirectory") Then
            MsgBox(My.Resources.strPleaseSelectwin32Dir)
            Button1.RaiseClick()
            Exit Sub
        End If
        'CheckForStoryUpdates()
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
STORYSTUFF:
InstallStory:
            If Helper.GetRegKey(Of String)("Backup") = "Ask" Then backupyesno = MsgBox(My.Resources.strWouldYouLikeBackupStory, vbYesNo)
            If Helper.GetRegKey(Of String)("Backup") = "Always" Then backupyesno = MsgBoxResult.Yes
            If Helper.GetRegKey(Of String)("Backup") = "Never" Then backupyesno = MsgBoxResult.No
            Log("Extracting story patch...")
            If Directory.Exists("TEMPSTORYAIDAFOOL") = True Then
                My.Computer.FileSystem.DeleteDirectory("TEMPSTORYAIDAFOOL", FileIO.DeleteDirectoryOption.DeleteAllContents)
                Directory.CreateDirectory("TEMPSTORYAIDAFOOL")
            End If
            If Directory.Exists("TEMPSTORYAIDAFOOL") = False Then
                Directory.CreateDirectory("TEMPSTORYAIDAFOOL")
            End If
            Dim process As Process = Nothing
            Dim processStartInfo As ProcessStartInfo
            processStartInfo = New ProcessStartInfo()
            'WriteDebugInfo("-------DEBUG LOG FOR CYBERMAN-------")
            'WriteDebugInfo("Does the Directory to extract to exist: " & Directory.Exists("TEMPSTORYAIDAFOOL"))
            Dim UnRarLocation As String
            UnRarLocation = (Application.StartupPath & "\unrar.exe")
            UnRarLocation = UnRarLocation.Replace("\\", "\")
            processStartInfo.FileName = UnRarLocation
            'WriteDebugInfo("Path to Unrar.exe: " & (Environment.CurrentDirectory & "\unrar.exe"))
            'WriteDebugInfo("Unrar.exe exists on that path: " & IO.File.Exists(Environment.CurrentDirectory & "\unrar.exe"))
            processStartInfo.Verb = "runas"
            processStartInfo.Arguments = ("e " & """" & StoryLocation & """" & " TEMPSTORYAIDAFOOL")
            'WriteDebugInfo("Name of the story RAR is: " & StoryLocation)
            processStartInfo.WindowStyle = ProcessWindowStyle.Normal
            processStartInfo.UseShellExecute = True
            process = process.Start(processStartInfo)
            'WriteDebugInfo("Final step - Total: " & process.ToString)
            WriteDebugInfo(My.Resources.strWaitingforPatch)
            If CancelledFull = True Then Exit Sub
            Do Until process.WaitForExit(1000)
            Loop
            If CancelledFull = True Then Exit Sub
            If Directory.Exists("TEMPSTORYAIDAFOOL") = False Then
                Directory.CreateDirectory("TEMPSTORYAIDAFOOL")
                WriteDebugInfo("Had to manually make temp update folder - Did the patch not extract right?")
            End If
            Dim di As New IO.DirectoryInfo("TEMPSTORYAIDAFOOL")
            Dim diar1 As IO.FileInfo() = di.GetFiles()
            Dim dra As IO.FileInfo
            WriteDebugInfoAndOK((My.Resources.strExtractingTo & (lblDirectory.Text & "\data\win32")))
            Application.DoEvents()
            'list the names of all files in the specified directory
            Dim backupdir As String = ((lblDirectory.Text & "\data\win32") & "\" & "backupPreSTORYPatch")
            'WriteDebugInfo("Backup path set to: " & backupdir)
            If backupyesno = MsgBoxResult.Yes Then
                If Directory.Exists(backupdir) = True Then
                    My.Computer.FileSystem.DeleteDirectory(backupdir, FileIO.DeleteDirectoryOption.DeleteAllContents)
                    Directory.CreateDirectory(backupdir)
                    WriteDebugInfo(My.Resources.strErasingPreviousBackup)
                End If
                If Directory.Exists(backupdir) = False Then
                    Directory.CreateDirectory(backupdir)
                    WriteDebugInfo(My.Resources.strCreatingBackupDirectory)
                End If
                'MsgBox(backupdir)
            End If
            Log("Extracted " & diar1.Count & " files from the patch")
            If diar1.Count = 0 Then
                WriteDebugInfo("Patch failed to extract correctly! Installation failed!")
                Exit Sub
            End If
            WriteDebugInfo(My.Resources.strInstallingPatch)
            For Each dra In diar1
                If CancelledFull = True Then Exit Sub
                'ListBox1.Items.Add(dra)
                'MsgBox(dra.ToString)
                'OldFileMD5 = GetMD5(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
                'NewFileMD5 = GetMD5(("TEMPPATCHAIDAFOOL\" & dra.ToString))
                If backupyesno = MsgBoxResult.Yes Then
                    If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString)) = True Then
                        File.Move(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString), (backupdir & "\" & dra.ToString))
                        'MsgBox(("Moving" & ((lblDirectory.Text & "\data\win32") & "\" & dra.ToString) & " to " & (backupdir & "\" & dra.ToString)))
                        'MsgBox(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
                    End If
                End If
                If backupyesno = MsgBoxResult.No Then
                    If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString)) = True Then
                        File.Delete(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
                    End If
                End If
                File.Move(("TEMPSTORYAIDAFOOL\" & dra.ToString), ((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
                'If OldFileMD5 <> NewFileMD5 Then
                'If OldFileMD5 = GetMD5(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString)) Then
                'WriteDebugInfoAndFAILED("Old file " & ((lblDirectory.Text & "\data\win32") & "\" & dra.ToString) & " still exists! File was NOT overwritten!")
                'End If
                'End If
                'MsgBox(("Moving" & (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\TEMPPATCH\" & dra.ToString) & " to " & ((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
            Next
            'Process.Start("7z.exe", ("e ENPatch.rar -y -o" & (lblDirectory.Text & "\data\win32")))
            My.Computer.FileSystem.DeleteDirectory("TEMPSTORYAIDAFOOL", FileIO.DeleteDirectoryOption.DeleteAllContents)
            If backupyesno = MsgBoxResult.No Then
                FlashWindow(Me.Handle, 1)
                'Story Patch 3-12-2014.rar
                Dim StoryPatchFilename As String = OpenFileDialog1.SafeFileName
                StoryPatchFilename = StoryPatchFilename.Replace("Story Patch ", "")
                StoryPatchFilename = StoryPatchFilename.Replace(".rar", "")
                StoryPatchFilename = StoryPatchFilename.Replace("-", "/")
                Helper.SetRegKey(Of String)("StoryPatchVersion", StoryPatchFilename)
                Helper.SetRegKey(Of String)("LatestStoryBase", StoryPatchFilename)
                WriteDebugInfo(My.Resources.strStoryPatchInstalled)
                CheckForStoryUpdates()
            End If
            If backupyesno = MsgBoxResult.Yes Then
                FlashWindow(Me.Handle, 1)
                'Story Patch 3-12-2014.rar
                Dim StoryPatchFilename As String = OpenFileDialog1.SafeFileName
                StoryPatchFilename = StoryPatchFilename.Replace("Story Patch ", "")
                StoryPatchFilename = StoryPatchFilename.Replace(".rar", "")
                StoryPatchFilename = StoryPatchFilename.Replace("-", "/")
                Helper.SetRegKey(Of String)("StoryPatchVersion", StoryPatchFilename)
                Helper.SetRegKey(Of String)("LatestStoryBase", StoryPatchFilename)
                WriteDebugInfo((My.Resources.strStoryPatchBackup & backupdir))
                CheckForStoryUpdates()
            End If
            Exit Sub
        End If
        If Downloaded = MsgBoxResult.No Then
            'Dim DownloadStoryQuestionMark As MsgBoxResult = MsgBox("Would you like the Tweaker to download the story patch for you? If not, you can also use get the torrent link or direct download link by hitting No.", vbYesNo)
            'If DownloadStoryQuestionMark = vbNo Then
            WriteDebugInfo(My.Resources.strDownloadStoryPatch)
            Exit Sub
            'End If
            'If DownloadStoryQuestionMark = vbYes Then
            'WriteDebugInfo("Downloading story patch...")
            'Dim net As New Net.WebClient()
            'Dim src As String
            'src = net.DownloadString("http://arks-layer.com/storypatchlink/")
            'Dim strDownloadME As String = src.ToString
            'strDownloadME = strDownloadME.Replace("<HTML>" & vbCrLf & "<HEAD><meta HTTP-EQUIV=""REFRESH"" content=""0; url=", "")
            'strDownloadME = strDownloadME.Replace("""></HEAD>" & vbCrLf & "</HTML>", "")
            'DLWUA(strDownloadME, "StoryPatch.rar", True)
            'WriteDebugInfoSameLine(My.Resources.strDone)
            'StoryLocation = Application.StartupPath & "/StoryPatch.rar"
            'GoTo STORYSTUFF
            'End If
        End If
        'Catch ex As Exception
        'Log(ex.Message)
        'WriteDebugInfo(My.Resources.strERROR & ex.Message)
        'Exit Sub
        'End Try
    End Sub

    Private Sub chkItemTranslation_Click(sender As Object, e As EventArgs) Handles chkItemTranslation.Click
        If File.Exists(lblDirectory.Text & "\translation.cfg") = False Then
            File.WriteAllText(lblDirectory.Text & "\translation.cfg", "TranslationPath:translation.bin")
        End If
        If chkItemTranslation.Checked = True Then
            'WriteDebugInfoAndOK(My.Resources.strItemTranslationEnabled)
            Dim DirectoryString As String
            Dim pso2launchpath As String
            DirectoryString = (lblDirectory.Text & "\data\win32")
            pso2launchpath = DirectoryString.Replace("\data\win32", "")

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
            Helper.SetRegKey(Of String)("DLLMD5", GetMD5(pso2launchpath & "\translator.dll"))
            failednumbers = 0
            'DLWUA(DLLink2, (pso2launchpath & "\translation.bin"), True)
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

                BuiltFile += CurrentLine & vbNewLine
            Loop
            objReader.Close()
            'MsgBox(BuiltFile)
            File.WriteAllText(lblDirectory.Text & "\translation.cfg", BuiltFile)
            WriteDebugInfoSameLine(My.Resources.strDone)
        End If
        If chkItemTranslation.Checked = False Then
            WriteDebugInfoAndOK(My.Resources.strDeletingItemCache)
            Dim dir As String
            dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            If File.Exists(dir & "\SEGA\PHANTASYSTARONLINE2\item_name_cache.dat") Then File.Delete(dir & "\SEGA\PHANTASYSTARONLINE2\item_name_cache.dat")
            WriteDebugInfoSameLine(My.Resources.strDone)
            Dim objReader As New StreamReader(lblDirectory.Text & "\translation.cfg")
            Dim CurrentLine As String = ""
            Dim BuiltFile As String = ""
            Do While objReader.Peek() <> -1

                CurrentLine = objReader.ReadLine()

                If CurrentLine.Contains("TranslationPath:") Then CurrentLine = "TranslationPath:"

                BuiltFile += CurrentLine & vbNewLine
            Loop
            objReader.Close()

            'MsgBox(BuiltFile)
            File.WriteAllText(lblDirectory.Text & "\translation.cfg", BuiltFile)
        End If

        UseItemTranslation = chkItemTranslation.Checked
        Helper.SetRegKey(Of String)("UseItemTranslation", UseItemTranslation)
    End Sub

    Private Sub ButtonItem5_Click(sender As Object, e As EventArgs) Handles ButtonItem5.Click
        CancelledFull = False
        'Try
        If (Directory.Exists((lblDirectory.Text & "\data\win32")) = False OrElse lblDirectory.Text = "lblDirectory") Then
            MsgBox(My.Resources.strPleaseSelectwin32Dir)
            Button1.RaiseClick()
            Exit Sub
        End If
        Dim filedownloader As New Net.WebClient()
        Dim sBuffer As String = Nothing
        Dim filename As String() = Nothing
        Dim truefilename As String = Nothing
        Dim missingfiles As New List(Of String)
        Dim filedownloader2 As New Net.WebClient()
        Dim missingfiles2 As New List(Of String)
        Dim NumberofChecks As Integer = 0
        Dim MD5 As String() = Nothing
        Dim TrueMD5 As String = Nothing
        Dim PSO2EXEMD5 As String = "FUCK YOU DUDU"
        Dim totalfilesize As Long = 0
        Dim testfilesize As String()
        lblStatus.Text = ""
        If Directory.Exists(((lblDirectory.Text & "\data\win32") & "\backupPreENPatch")) = True Then
            WriteDebugInfo(My.Resources.strENBackupFound)
            Override = True
            btnRestoreENBackup.RaiseClick()
            Override = False
        End If
        If Directory.Exists(((lblDirectory.Text & "\data\win32") & "\backupPreLargeFiles")) = True Then
            WriteDebugInfo(My.Resources.strLFBackupFound)
            Override = True
            btnRestoreLargeFilesBackup.RaiseClick()
            Override = False
        End If
        If Directory.Exists(((lblDirectory.Text & "\data\win32") & "\backupPreSTORYPatch")) = True Then
            WriteDebugInfo(My.Resources.strStoryBackupFound)
            Override = True
            btnRestoreStoryBackup.RaiseClick()
            Override = False
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
        Dim versionclient As New MyWebClient
        versionclient.timeout = 3000
        versionclient.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
        'DLWUA("http://download.pso2.jp/patch_prod/patches/version.ver", "version.ver", True)
        WriteDebugInfoSameLine(My.Resources.strDone)
        Application.DoEvents()
        UnlockGUI()

        ' Mike: No idea what you were doing here. Hope I didn't break anything.
        Dim result As MsgBoxResult = MsgBoxResult.No
        If ComingFromOldFiles = False Then
            Me.TopMost = False
            'result = MsgBox(My.Resources.strDidYouHaveTheLatestVersion, vbYesNo)
            If chkAlwaysOnTop.Checked = True Then Me.TopMost = True
        End If

        If result = MsgBoxResult.Yes Or ComingFromOldFiles Then
            WriteDebugInfo(My.Resources.strCheckingforNewContent)
            NumberofChecks = 0
            missingfiles.Clear()
            Using oReader As StreamReader = File.OpenText("patchlist.txt")
                If CancelledFull = True Then Exit Sub
                While Not (oReader.EndOfStream)
                    If CancelledFull = True Then Exit Sub
                    sBuffer = oReader.ReadLine
                    filename = Regex.Split(sBuffer, ".pat")
                    truefilename = filename(0).Replace("data/win32/", "")
                    MD5 = Regex.Split(filename(1), vbTab)
                    TrueMD5 = MD5(2)
                    If truefilename <> "GameGuard.des" And truefilename <> "PSO2JP.ini" And truefilename <> "script/user_default.pso2" And truefilename <> "script/user_intel.pso2" Then
                        If truefilename = "pso2.exe" Then
                            PSO2EXEMD5 = TrueMD5
                            GoTo NextFile1
                        End If
                        If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & truefilename)) = False Then
                            'MsgBox((lblDirectory.Text & "\data\win32"))
                            'MsgBox("\")
                            'MsgBox(truefilename)
                            If VedaUnlocked = True Then WriteDebugInfo("DEBUG: The file " & truefilename & " is missing.")
                            missingfiles.Add(truefilename)
                            GoTo NEXTFILE1
                        End If
                        If GetMD5(((lblDirectory.Text & "\data\win32") & "\" & truefilename)) <> TrueMD5 Then
                            'MsgBox((lblDirectory.Text & "\data\win32"))
                            'MsgBox("\")
                            'MsgBox(truefilename)
                            If VedaUnlocked = True Then WriteDebugInfo("DEBUG: The file " & truefilename & " must be redownloaded.")
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
            If File.Exists("resume.txt") Then File.Delete("resume.txt")
            For Each downloadstring In missingfiles
                File.AppendAllText("resume.txt", (downloadstring & vbCrLf))
            Next
            For Each downloadstring In missingfiles
                If CancelledFull = True Then Exit Sub
                'Download the missing files:
                'WHAT THE FUCK IS GOING ON HERE?
                downloaded += 1
                totaldownloaded = totaldownloaded + totalsize2

                lblStatus.Text = My.Resources.strDownloading & "" & downloaded & "/" & totaldownload & " (" & Helper.SizeSuffix(totaldownloaded) & ")"

                Application.DoEvents()
                Cancelled = False
                DLWUA(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadstring & ".pat"), downloadstring, True)
                Dim info7 As New FileInfo(downloadstring)
                Dim length2 As Long
                If File.Exists(downloadstring) = True Then length2 = info7.Length
                If info7.Length = 0 Then
                    Log("File appears to be empty, trying to download from secondary SEGA server")
                    DLWUA(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring & ".pat"), downloadstring, True)
                End If

                If Cancelled = True Then Exit Sub
                'filedownloader.DownloadFile(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadstring & ".pat"), downloadstring)
                'DLWUA("http://download.pso2.jp/patch_prod/patches/launcherlist.txt", "launcherlist.txt", True)
                'Delete the existing file FIRST
                If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & downloadstring)) Then File.Delete(((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
                File.Move(downloadstring, ((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
                If VedaUnlocked = True Then WriteDebugInfo("DEBUG: Downloaded and installed " & downloadstring & ".")
                Dim linesList As New List(Of String)(File.ReadAllLines("resume.txt"))

                'Remove the line to delete, e.g.
                linesList.Remove(downloadstring)

                File.WriteAllLines("resume.txt", linesList.ToArray())
                'MsgBox("removed this line - " & downloadstring)
                Application.DoEvents()
                'MsgBox(downloadstring)
            Next
            patching = False
            If missingfiles.Count = 0 Then WriteDebugInfo(My.Resources.strYouAppearToBeUpToDate)
            Dim filedownloader3 As New Net.WebClient()
            Dim DirectoryString As String
            DirectoryString = (lblDirectory.Text & "\data\win32")
            DirectoryString = DirectoryString.Replace("\data\win32", "")
            DirectoryString = (DirectoryString & "\")
            WriteDebugInfo(My.Resources.strDownloading & "version file...")
            Application.DoEvents()
            Cancelled = False
            Dim versionclient2 As New MyWebClient
            versionclient2.timeout = 3000
            versionclient2.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
            'DLWUA("http://download.pso2.jp/patch_prod/patches/version.ver", "version.ver", True)
            If Cancelled = True Then Exit Sub
            Dim DirectoryString2 As String
            DirectoryString2 = (lblDirectory.Text & "\data\win32")
            DirectoryString2 = DirectoryString2.Replace("\data\win32", "")
            If File.Exists((Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\SEGA\PHANTASYSTARONLINE2\version.ver")) Then File.Delete((Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
            File.Copy("version.ver", (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
            WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & "version file"))
            'DLWUA("http://download.pso2.jp/patch_prod/patches/version.ver", "version.ver", True)
            WriteDebugInfo(My.Resources.strDownloading & "pso2launcher.exe...")
            Application.DoEvents()
            Dim procs As Process()
            procs = Process.GetProcessesByName("pso2launcher")
            For Each proc As Process In procs
                If proc.MainWindowTitle = "PHANTASY STAR ONLINE 2" And proc.MainModule.ToString = "ProcessModule (pso2launcher.exe)" Then proc.Kill()
            Next
            DLWUA("http://download.pso2.jp/patch_prod/patches/pso2launcher.exe.pat", "pso2launcher.exe", True)
            If Cancelled = True Then Exit Sub
            If File.Exists((DirectoryString & "pso2launcher.exe")) And Application.StartupPath <> DirectoryString2 Then File.Delete((DirectoryString & "pso2launcher.exe"))
            File.Move("pso2launcher.exe", (DirectoryString & "pso2launcher.exe"))
            WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & "pso2launcher.exe"))
            WriteDebugInfo(My.Resources.strDownloading & "pso2updater.exe...")
            Application.DoEvents()
            procs = Process.GetProcessesByName("pso2updater")
            For Each proc As Process In procs
                If proc.MainModule.ToString = "ProcessModule (pso2updater.exe)" Then proc.Kill()
            Next
            DLWUA("http://download.pso2.jp/patch_prod/patches/pso2updater.exe.pat", "pso2updater.exe", True)
            If Cancelled = True Then Exit Sub
            If File.Exists((DirectoryString & "pso2updater.exe")) And Application.StartupPath <> DirectoryString2 Then File.Delete((DirectoryString & "pso2updater.exe"))
            File.Move("pso2updater.exe", (DirectoryString & "pso2updater.exe"))
            WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & "pso2updater.exe"))
            Application.DoEvents()
            'Dim NumberofDownloads As Integer = 0
            'DLPSO2EXE:
            'If PSO2EXEMD5 <> GetMD5((DirectoryString & "pso2.exe")) Then
            WriteDebugInfo(My.Resources.strDownloading & "pso2.exe...")
            procs = Process.GetProcessesByName("pso2")
            For Each proc As Process In procs
                If proc.MainModule.ToString = "ProcessModule (pso2.exe)" Then proc.Kill()
            Next
            DLWUA("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", "pso2.exe", True)
            If Cancelled = True Then Exit Sub
            'If PSO2EXEMD5 <> GetMD5((DirectoryString & "pso2.exe")) Then
            ' NumberofDownloads += 1
            ' WriteDebugInfoAndWarning("It appears PSO2.EXE did not download correctly... Retrying...")
            ' If NumberofDownloads < 6 Then GoTo DLPSO2EXE
            ' If NumberofDownloads > 6 Then
            ' WriteDebugInfoAndFAILED("Failed to download pso2.exe correctly!")
            ' Exit Sub
            'End If
            If File.Exists((DirectoryString & "pso2.exe")) And Application.StartupPath <> DirectoryString2 Then File.Delete((DirectoryString & "pso2.exe"))
            File.Move("pso2.exe", (DirectoryString & "pso2.exe"))
            If CancelledFull = True Then Exit Sub
            WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & "pso2.exe"))
            Helper.SetRegKey(Of String)("StoryPatchVersion", "Not Installed")
            Helper.SetRegKey(Of String)("ENPatchVersion", "Not Installed")
            Helper.SetRegKey(Of String)("LargeFilesVersion", "Not Installed")
            Helper.SetRegKey(Of String)("PSO2PatchlistMD5", GetMD5("patchlist.txt"))
            WriteDebugInfo(My.Resources.strGameUpdatedVanilla)
            If File.Exists("resume.txt") Then File.Delete("resume.txt")
            Dim lines2 = File.ReadAllLines("version.ver")
            Dim RemoteVersion2 As String = lines2(0)
            Helper.SetRegKey(Of String)("PSO2RemoteVersion", RemoteVersion2)
            UnlockGUI()

            If Helper.GetRegKey(Of String)("RemoveCensor") = True Then
                If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c"), "ffbff2ac5b7a7948961212cefd4d402c.backup")
                WriteDebugInfoAndOK(My.Resources.strRemoving & "Censor...")
            End If
            If Helper.GetRegKey(Of String)("ENPatchAfterInstall") = True Then
                WriteDebugInfo(My.Resources.strAutoInstallingENPatch)
                btnENPatch.RaiseClick()
            End If

            If Helper.GetRegKey(Of String)("LargeFilesAfterInstall") = True Then
                WriteDebugInfo(My.Resources.strAutoInstallingLF)
                btnLargeFiles.RaiseClick()
            End If

            If Helper.GetRegKey(Of String)("StoryPatchAfterInstall") = True Then
                WriteDebugInfo(My.Resources.strAutoInstallingStoryPatch)
                btnStory.RaiseClick()
            End If

            WriteDebugInfoAndOK(My.Resources.strallDone)
            Exit Sub
        End If
        If result = MsgBoxResult.No Then
CHECKFOROLDFILES:
            ComingFromOldFiles = False
            If CancelledFull = True Then Exit Sub
            'Const NewFiles As String = "patchlist.txt"
            'WriteDebugInfoAndOK("Getting ready...")
            MergePatches()
            'MsgBox("Making the program wait~")
            WriteDebugInfo(My.Resources.strCheckingforAllFiles)
            Using oReader As StreamReader = File.OpenText("SOMEOFTHETHINGS.txt")
                If CancelledFull = True Then Exit Sub
                While Not (oReader.EndOfStream)
                    If CancelledFull = True Then Exit Sub
                    sBuffer = oReader.ReadLine
                    If String.IsNullOrEmpty(sBuffer) Then GoTo DOWNLOADFILES
                    'MsgBox(sBuffer.ToString)
                    filename = Regex.Split(sBuffer, ".pat")
                    'MsgBox(filename(1))
                    'MsgBox(testfilesize(1))
                    'MsgBox(totalfilesize.ToString & "before addition")
                    'MsgBox(totalfilesize.ToString & "after adding " & Convert.ToInt32(testfilesize(1)))
                    truefilename = filename(0).Replace("data/win32/", "")
                    MD5 = Regex.Split(filename(1), vbTab)
                    TrueMD5 = MD5(2)
                    If truefilename <> "GameGuard.des" And truefilename <> "PSO2JP.ini" And truefilename <> "script/user_default.pso2" And truefilename <> "script/user_intel.pso2" Then
                        If truefilename = "pso2.exe" Then
                            PSO2EXEMD5 = TrueMD5
                            GoTo NextFile2
                        End If
                        If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & truefilename)) = False Then
                            'MsgBox((lblDirectory.Text & "\data\win32"))
                            'MsgBox("\")
                            'MsgBox(truefilename)
                            If VedaUnlocked = True Then WriteDebugInfo("DEBUG: The file " & truefilename & My.Resources.strIsMissing)
                            testfilesize = Regex.Split(filename(1), "	")
                            totalfilesize += Convert.ToInt32(testfilesize(1))
                            missingfiles2.Add(truefilename)
                            GoTo NEXTFILE2
                        End If
                        Dim TempMD5 As String = GetMD5(((lblDirectory.Text & "\data\win32") & "\" & truefilename))
                        If GetMD5(((lblDirectory.Text & "\data\win32") & "\" & truefilename)) <> TrueMD5 Then
                            'MsgBox((lblDirectory.Text & "\data\win32"))
                            'MsgBox("\")
                            'MsgBox((truefilename & "'s hash should not be " & GetMD5(((lblDirectory.Text & "\data\win32") & "\" & truefilename)) & ". It should be " & TrueMD5))
                            'If NewFiles.Contains(truefilename) = True Then MsgBox("Both files contain " & truefilename)
                            'If NewFiles.Contains(truefilename) = False Then
                            If VedaUnlocked = True Then WriteDebugInfo("DEBUG: The file " & truefilename & " must be redownloaded.")
                            testfilesize = Regex.Split(filename(1), "	")
                            totalfilesize += Convert.ToInt32(testfilesize(1))
                            missingfiles2.Add(truefilename)
                            'End If
                            GoTo NEXTFILE2
                        End If
NEXTFILE2:
                        NumberofChecks += 1
                        lblStatus.Text = (My.Resources.strCurrentlyCheckingFile & NumberofChecks)
                        Application.DoEvents()
                    End If
                End While
            End Using
DOWNLOADFILES:
            Dim totaldownload2 As String = missingfiles2.Count
            Dim downloaded2 As Long = 0
            Dim info As FileInfo
            Dim filesize As Long
            Dim totaldownloaded As Long = 0
            patching = True
            If File.Exists("resume.txt") Then File.Delete("resume.txt")
            For Each downloadstring In missingfiles2
                File.AppendAllText("resume.txt", (downloadstring & vbCrLf))
            Next
            For Each downloadstring In missingfiles2
                If CancelledFull = True Then Exit Sub
                'Download the missing files:
                'WHAT THE FUCK IS GOING ON HERE?
                downloaded2 += 1
                totaldownloaded += totalsize2

                lblStatus.Text = My.Resources.strDownloading & "" & downloaded2 & "/" & totaldownload2 & " (" & Helper.SizeSuffix(totaldownloaded) & " / " & Helper.SizeSuffix(totalfilesize) & ")"

                Application.DoEvents()
                DLWUA(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadstring & ".pat"), downloadstring, False)
                Dim info7 As New FileInfo(downloadstring)
                Dim length2 As Long
                If File.Exists(downloadstring) = True Then length2 = info7.Length
                If info7.Length = 0 Then
                    Log("File appears to be empty, trying to download from secondary SEGA server")
                    DLWUA(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring & ".pat"), downloadstring, True)
                End If
                info = New FileInfo(downloadstring)
                filesize = info.Length
                If filesize = 0 Then
                    IO.File.Delete(downloadstring)
                    DLWUA(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring & ".pat"), downloadstring, False)
                End If
                'filedownloader.DownloadFile(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring & ".pat"), downloadstring)
                If File.Exists(downloadstring) = True Then
                    If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & downloadstring)) Then File.Delete(((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
                    File.Move(downloadstring, ((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
                    If VedaUnlocked = True Then WriteDebugInfo("DEBUG: Downloaded and installed " & downloadstring & ".")
                    Dim linesList As New List(Of String)(File.ReadAllLines("resume.txt"))

                    'Remove the line to delete, e.g.
                    linesList.Remove(downloadstring)

                    File.WriteAllLines("resume.txt", linesList.ToArray())
                    'MsgBox("removed this line - " & downloadstring)
                    Application.DoEvents()
                    'MsgBox(downloadstring)
                Else
                    'WriteDebugInfoAndFAILED("Couldn't download " & downloadstring)
                    Application.DoEvents()
                End If
            Next
            patching = False
            If missingfiles.Count = 0 Then WriteDebugInfo(My.Resources.strYouAppearToBeUpToDate)
            Dim filedownloader3 As New Net.WebClient()
            Dim DirectoryString As String
            DirectoryString = (lblDirectory.Text & "\data\win32")
            DirectoryString = DirectoryString.Replace("\data\win32", "")
            DirectoryString = (DirectoryString & "\")
            WriteDebugInfo(My.Resources.strDownloading & "version file...")
            Application.DoEvents()
            Dim DirectoryString2 As String
            DirectoryString2 = (lblDirectory.Text & "\data\win32")
            DirectoryString2 = DirectoryString2.Replace("\data\win32", "")
            Dim versionclient2 As New MyWebClient
            versionclient2.timeout = 3000
            versionclient2.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
            'DLWUA("http://download.pso2.jp/patch_prod/patches/version.ver", "version.ver", True)
            If File.Exists((Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\SEGA\PHANTASYSTARONLINE2\version.ver")) Then File.Delete((Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
            File.Copy("version.ver", (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
            WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & "version file"))
            'DLWUA("http://download.pso2.jp/patch_prod/patches/version.ver", "version.ver", True)
            WriteDebugInfo(My.Resources.strDownloading & "pso2launcher.exe...")
            Application.DoEvents()
            DLWUA("http://download.pso2.jp/patch_prod/patches/pso2launcher.exe.pat", "pso2launcher.exe", True)
            If File.Exists((DirectoryString & "pso2launcher.exe")) And Application.StartupPath <> DirectoryString2 Then File.Delete((DirectoryString & "pso2launcher.exe"))
            File.Move("pso2launcher.exe", (DirectoryString & "pso2launcher.exe"))
            WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & "pso2launcher.exe"))
            WriteDebugInfo(My.Resources.strDownloading & "pso2updater.exe...")
            Application.DoEvents()
            DLWUA("http://download.pso2.jp/patch_prod/patches/pso2updater.exe.pat", "pso2updater.exe", True)
            If File.Exists((DirectoryString & "pso2updater.exe")) And Application.StartupPath <> DirectoryString2 Then File.Delete((DirectoryString & "pso2updater.exe"))
            File.Move("pso2updater.exe", (DirectoryString & "pso2updater.exe"))
            WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & "pso2updater.exe"))
            Application.DoEvents()
            Application.DoEvents()
            'Dim NumberofDownloads As Integer = 0
            'DLPSO2EXE:
            'If PSO2EXEMD5 <> GetMD5((DirectoryString & "pso2.exe")) Then
            WriteDebugInfo(My.Resources.strDownloading & "pso2.exe...")
            DLWUA("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", "pso2.exe", True)
            If Cancelled = True Then Exit Sub
            'If PSO2EXEMD5 <> GetMD5((DirectoryString & "pso2.exe")) Then
            ' NumberofDownloads += 1
            ' WriteDebugInfoAndWarning("It appears PSO2.EXE did not download correctly... Retrying...")
            ' If NumberofDownloads < 6 Then GoTo DLPSO2EXE
            ' If NumberofDownloads > 6 Then
            ' WriteDebugInfoAndFAILED("Failed to download pso2.exe correctly!")
            ' Exit Sub
            'End If
            If File.Exists((DirectoryString & "pso2.exe")) And Application.StartupPath <> DirectoryString2 Then File.Delete((DirectoryString & "pso2.exe"))
            File.Move("pso2.exe", (DirectoryString & "pso2.exe"))
            If CancelledFull = True Then Exit Sub
            WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & "pso2.exe"))

            Helper.SetRegKey(Of String)("StoryPatchVersion", "Not Installed")
            Helper.SetRegKey(Of String)("ENPatchVersion", "Not Installed")
            Helper.SetRegKey(Of String)("LargeFilesVersion", "Not Installed")
            Helper.SetRegKey(Of String)("PSO2PatchlistMD5", GetMD5("patchlist.txt"))
            WriteDebugInfo(My.Resources.strGameUpdatedVanilla)
            If File.Exists("resume.txt") Then File.Delete("resume.txt")
            Dim lines3 = File.ReadAllLines("version.ver")
            Dim RemoteVersion3 As String = lines3(0)
            Helper.SetRegKey(Of String)("PSO2RemoteVersion", RemoteVersion3)
            UnlockGUI()
            WriteDebugInfoAndOK(My.Resources.strallDone)
            Exit Sub
        End If
        'Catch ex As Exception
        ' Log(ex.Message)
        ' If ex.Message <> "Arithmetic operation resulted in an overflow." Then
        ' WriteDebugInfo(My.Resources.strERROR & ex.Message)
        ' Exit Sub
        ' End If
        'End Try
    End Sub

    Private Sub btnRestoreENBackup_Click(sender As Object, e As EventArgs) Handles btnRestoreENBackup.Click
        Try
            If (Directory.Exists((lblDirectory.Text & "\data\win32")) = False OrElse lblDirectory.Text = "lblDirectory") Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                Button1.RaiseClick()
                Exit Sub
            End If
            Dim backupyesno As MsgBoxResult
            If Override = False Then
                backupyesno = MsgBox(My.Resources.strAreYouSureRestoreBackup, vbYesNo)
            End If
            If Override = True Then backupyesno = MsgBoxResult.Yes
            If backupyesno = MsgBoxResult.Yes Then
                Dim backupfolder As String = "backupPreENPatch"
                If Directory.Exists(((lblDirectory.Text & "\data\win32") & "\" & backupfolder)) = False Then
                    WriteDebugInfoAndFAILED(My.Resources.strCantFindBackupDirectory & ((lblDirectory.Text & "\data\win32") & "\" & backupfolder))
                    Exit Sub
                End If
                Dim di As New IO.DirectoryInfo(((lblDirectory.Text & "\data\win32") & "\" & backupfolder))
                'MsgBox((lblDirectory.Text & "\data\win32") & "\" & "backupPreENPatch")
                Dim diar1 As IO.FileInfo() = di.GetFiles()
                Dim dra As IO.FileInfo
                WriteDebugInfoAndOK((My.Resources.strRestoringBackupTo & (lblDirectory.Text & "\data\win32")))
                Application.DoEvents()
                'list the names of all files in the specified directory
                Dim win32 As String = (lblDirectory.Text & "\data\win32")
                For Each dra In diar1
                    If File.Exists(win32 & "\" & dra.ToString) = True Then
                        File.Delete(win32 & "\" & dra.ToString)
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
            If Override = False Then
                backupyesno = MsgBox(My.Resources.strAreYouSureRestoreBackup, vbYesNo)
            End If
            If Override = True Then backupyesno = MsgBoxResult.Yes
            If backupyesno = MsgBoxResult.Yes Then
                Dim backupfolder As String = "backupPreLargeFiles"
                If Directory.Exists(((lblDirectory.Text & "\data\win32") & "\" & backupfolder)) = False Then
                    WriteDebugInfoAndFAILED(My.Resources.strCantFindBackupDirectory & ((lblDirectory.Text & "\data\win32") & "\" & backupfolder))
                    Exit Sub
                End If
                Dim di As New IO.DirectoryInfo(((lblDirectory.Text & "\data\win32") & "\" & backupfolder))
                'MsgBox((lblDirectory.Text & "\data\win32") & "\" & "backupPreENPatch")
                Dim diar1 As IO.FileInfo() = di.GetFiles()
                Dim dra As IO.FileInfo
                WriteDebugInfoAndOK((My.Resources.strRestoringBackupTo & (lblDirectory.Text & "\data\win32")))
                Application.DoEvents()
                'list the names of all files in the specified directory
                Dim win32 As String = (lblDirectory.Text & "\data\win32")
                For Each dra In diar1
                    If File.Exists(win32 & "\" & dra.ToString) = True Then
                        File.Delete(win32 & "\" & dra.ToString)
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
        If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "ceffe0e2386e8d39f188358303a92a7d.backup")) = True Then
            If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "ceffe0e2386e8d39f188358303a92a7d")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "ceffe0e2386e8d39f188358303a92a7d"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
            My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "ceffe0e2386e8d39f188358303a92a7d.backup"), "ceffe0e2386e8d39f188358303a92a7d")
            WriteDebugInfoAndOK(My.Resources.strRestoring & " JP Names file...")
        Else
            WriteDebugInfoAndOK(My.Resources.strRestoreFailedPleaseReinstall)
        End If
    End Sub

    Private Sub btnRestoreJPETrials_Click(sender As Object, e As EventArgs) Handles btnRestoreJPETrials.Click
        'http://pso2.arghargh200.net/pso2/2013_06_12/057aa975bdd2b372fe092614b0f4399e
        If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "057aa975bdd2b372fe092614b0f4399e.backup")) = True Then
            If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "057aa975bdd2b372fe092614b0f4399e")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "057aa975bdd2b372fe092614b0f4399e"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
            My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "057aa975bdd2b372fe092614b0f4399e.backup"), "057aa975bdd2b372fe092614b0f4399e")
            WriteDebugInfoAndOK(My.Resources.strRestoring & " JP E-Trials file...")
        Else
            WriteDebugInfoAndOK(My.Resources.strRestoreFailedPleaseReinstall)
        End If
    End Sub

    Private Sub btnAnalyze_Click(sender As Object, e As EventArgs) Handles btnAnalyze.Click
        Dim DirectoryString As String
        Dim pso2launchpath As String
        DirectoryString = (lblDirectory.Text & "\data\win32")
        pso2launchpath = DirectoryString.Replace("data\win32", "")
        WriteDebugInfo(My.Resources.strCheckingForNecessaryFiles)
        If File.Exists(pso2launchpath & "Gameguard.DES") = False Then WriteDebugInfoAndWarning("Missing GameGuard.DES file! " & My.Resources.strPleaseFixGG)
        If File.Exists(pso2launchpath & "pso2.exe") = False Then WriteDebugInfoAndWarning("Missing pso2.exe file! " & My.Resources.strPleaseFixPSO2EXEs)
        If File.Exists(pso2launchpath & "pso2launcher.exe") = False Then WriteDebugInfoAndWarning("Missing pso2launcher.exe file! " & My.Resources.strPleaseFixPSO2EXEs)
        If File.Exists(pso2launchpath & "pso2updater.exe") = False Then WriteDebugInfoAndWarning("Missing pso2updater.exe file! " & My.Resources.strPleaseFixPSO2EXEs)
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strCheckingForFolders)
        If Directory.Exists(pso2launchpath & "\Gameguard\") = False Then WriteDebugInfoAndWarning("Missing Gameguard folder! " & My.Resources.strPleaseFixGG)
        If Directory.Exists(pso2launchpath & "\data\") = False Then WriteDebugInfoAndWarning("Missing data folder! " & My.Resources.strPleaseReinstallOrCheck)
        If Directory.Exists(pso2launchpath & "\data\win32\") = False Then WriteDebugInfoAndWarning("Missing data\win32 folder! " & My.Resources.strPleaseReinstallOrCheck)
        WriteDebugInfoSameLine(My.Resources.strDone)
        WriteDebugInfo(My.Resources.strDoneTesting)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        'Try
        If (Directory.Exists((lblDirectory.Text & "\data\win32")) = False OrElse lblDirectory.Text = "lblDirectory") Then
            MsgBox(My.Resources.strPleaseSelectwin32Dir)
            Button1.RaiseClick()
            Exit Sub
        End If
        Dim filedownloader As New Net.WebClient()
        Dim sBuffer As String
        Dim filename As String()
        Dim truefilename As String
        Dim missingfiles As New List(Of String)
        Dim filedownloader2 As New Net.WebClient()
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
        Dim versionclient As New MyWebClient
        versionclient.timeout = 3000
        versionclient.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
        'DLWUA("http://download.pso2.jp/patch_prod/patches/version.ver", "version.ver", True)
        WriteDebugInfoSameLine(My.Resources.strDone)
        Application.DoEvents()
        UnlockGUI()
        Log("Opening patch file list...")
        Using oReader As StreamReader = File.OpenText("patchlist.txt")
            If CancelledFull = True Then Exit Sub
            If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup")) = True Then
                If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c")) = True Then IO.File.Delete(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c"))
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"), "ffbff2ac5b7a7948961212cefd4d402c")
                WriteDebugInfoAndOK(My.Resources.strRestoring & "Censor...")
            End If
            If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup")) = True Then
                If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927")) = True Then IO.File.Delete(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927"))
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "a44fbb2aeb8084c5a5fbe80e219a9927.backup"), "a44fbb2aeb8084c5a5fbe80e219a9927")
                WriteDebugInfoAndOK(My.Resources.strRestoring & "PC Opening...")
            End If
            If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "7f2368d207e104e8ed6086959b742c75.backup")) = True Then
                If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "7f2368d207e104e8ed6086959b742c75")) = True Then IO.File.Delete(((lblDirectory.Text & "\data\win32") & "\" & "7f2368d207e104e8ed6086959b742c75"))
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "7f2368d207e104e8ed6086959b742c75.backup"), "7f2368d207e104e8ed6086959b742c75")
                WriteDebugInfoAndOK(My.Resources.strRestoring & "NVidia Logo...")
            End If
            If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "009bfec69b04a34576012d50e3417771.backup")) = True Then
                If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "009bfec69b04a34576012d50e3417771")) = True Then IO.File.Delete(((lblDirectory.Text & "\data\win32") & "\" & "009bfec69b04a34576012d50e3417771"))
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "009bfec69b04a34576012d50e3417771.backup"), "009bfec69b04a34576012d50e3417771")
                WriteDebugInfoAndOK(My.Resources.strRestoring & "SEGA Logo...")
            End If
            If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585.backup")) = True Then
                If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585")) = True Then IO.File.Delete(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585"))
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "a93adc766eb3510f7b5c279551a45585.backup"), "a93adc766eb3510f7b5c279551a45585")
                WriteDebugInfoAndOK(My.Resources.strRestoring & "Vita Opening...")
            End If
            WriteDebugInfo(My.Resources.strCheckingFiles)
            While Not (oReader.EndOfStream)
                If CancelledFull = True Then Exit Sub
                sBuffer = oReader.ReadLine
                filename = Regex.Split(sBuffer, ".pat")
                truefilename = filename(0).Replace("data/win32/", "")
                If truefilename <> "GameGuard.des" And truefilename <> "edition.txt" And truefilename <> "gameversion.ver" And truefilename <> "pso2.exe" And truefilename <> "PSO2JP.ini" And truefilename <> "script/user_default.pso2" And truefilename <> "script/user_intel.pso2" Then
                    Dim info7 As New FileInfo(((lblDirectory.Text & "\data\win32") & "\" & truefilename))
                    Dim length2 As Long
                    If File.Exists((lblDirectory.Text & "\data\win32") & "\" & truefilename) = True Then length2 = info7.Length
                    If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & truefilename)) = False Then
                        'MsgBox((lblDirectory.Text & "\data\win32"))
                        'MsgBox("\")
                        'MsgBox(truefilename)
                        WriteDebugInfo(truefilename & My.Resources.strIsMissing)
                        missingfiles.Add(truefilename)
                    End If
                    info7 = New FileInfo(((lblDirectory.Text & "\data\win32") & "\" & truefilename))
                    If File.Exists((lblDirectory.Text & "\data\win32") & "\" & truefilename) = True Then length2 = info7.Length
                    If File.Exists((lblDirectory.Text & "\data\win32") & "\" & truefilename) = False Then length2 = 1
                    If length2 = 0 Then
                        WriteDebugInfo(truefilename & " has a filesize of 0!")
                        missingfiles.Add(truefilename)
                        File.Delete((lblDirectory.Text & "\data\win32") & "\" & truefilename)
                    End If
                End If
                NumberofChecks += 1
                'MsgBox(NumberofChecks)
                lblStatus.Text = (My.Resources.strCurrentlyCheckingFile & NumberofChecks)
                Application.DoEvents()
            End While
        End Using
        Log("Opening Second patch file...")
        Using oReader As StreamReader = File.OpenText("patchlist_old.txt")
            While Not (oReader.EndOfStream)
                If CancelledFull = True Then Exit Sub
                sBuffer2 = oReader.ReadLine
                filename2 = Regex.Split(sBuffer2, ".pat")
                truefilename2 = filename2(0).Replace("data/win32/", "")
                If truefilename2 <> "GameGuard.des" And truefilename2 <> "pso2.exe" And truefilename2 <> "PSO2JP.ini" And truefilename2 <> "script/user_default.pso2" And truefilename2 <> "script/user_intel.pso2" Then
                    Dim info7 As New FileInfo(((lblDirectory.Text & "\data\win32") & "\" & truefilename2))
                    Dim length2 As Long
                    If File.Exists((lblDirectory.Text & "\data\win32") & "\" & truefilename2) = True Then length2 = info7.Length
                    If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & truefilename2)) = False Then
                        'MsgBox((lblDirectory.Text & "\data\win32"))
                        'MsgBox("\")
                        'MsgBox(truefilename)
                        If missingfiles.Contains(truefilename2) Then
                            'Do nothing
                        Else
                            WriteDebugInfo(truefilename2 & My.Resources.strIsMissing)
                            missingfiles2.Add(truefilename2)
                        End If
                    End If
                    info7 = New FileInfo(((lblDirectory.Text & "\data\win32") & "\" & truefilename2))
                    If File.Exists((lblDirectory.Text & "\data\win32") & "\" & truefilename2) = True Then length2 = info7.Length
                    If File.Exists((lblDirectory.Text & "\data\win32") & "\" & truefilename2) = False Then length2 = 1
                    If length2 = 0 Then
                        WriteDebugInfo(truefilename2 & " has a filesize of 0!")
                        missingfiles.Add(truefilename2)
                        File.Delete((lblDirectory.Text & "\data\win32") & "\" & truefilename2)
                    End If
                End If
                NumberofChecks += 1
                lblStatus.Text = (My.Resources.strCurrentlyCheckingFile & NumberofChecks)
                Application.DoEvents()
            End While
            If missingfiles.Count = 0 And missingfiles2.Count = 0 Then
                WriteDebugInfoAndOK(My.Resources.strAllFilesCheckedOut)
                Exit Sub
            End If
        End Using

        Dim result1 As DialogResult = MessageBox.Show(My.Resources.strWouldYouLikeToDownloadInstallMissing, "Download/Install?", MessageBoxButtons.YesNo)

        If result1 = Windows.Forms.DialogResult.No Then Exit Sub

        If result1 = Windows.Forms.DialogResult.Yes Then
            Log(My.Resources.strDownloading & My.Resources.strMissingFilesPart1)
            Dim totaldownload As String = missingfiles.Count
            Dim downloaded As Long = 0
            Dim totaldownloaded As Long = 0
            If File.Exists("resume.txt") Then File.Delete("resume.txt")

            For Each downloadstring In missingfiles
                File.AppendAllText("resume.txt", (downloadstring & vbCrLf))
            Next

            For Each downloadstring In missingfiles
                'Download the missing files:
                Cancelled = False
                downloaded += 1
                totaldownloaded = totaldownloaded + totalsize2

                lblStatus.Text = My.Resources.strDownloading & "" & downloaded & "/" & totaldownload & " (" & Helper.SizeSuffix(totaldownloaded) & ")"

                DLWUA(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadstring & ".pat"), downloadstring, True)
                Dim info7 As New FileInfo(downloadstring)
                Dim length2 As Long
                If File.Exists(downloadstring) = True Then length2 = info7.Length
                If info7.Length = 0 Then
                    Log("File appears to be empty, trying to download from secondary SEGA server")
                    DLWUA(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring & ".pat"), downloadstring, True)
                End If
                If Cancelled = True Then Exit Sub
                'filedownloader.DownloadFile(("http://dl.dropboxusercontent.com/u/23005008/win32/" & downloadstring), downloadstring)
                'My.Computer.Network.DownloadFile(("http://patch01.pso2gs.net/patch_prod/patches/data/win32/" & downloadstring & ".pat"), downloadstring, vbNullString, vbNullString, True, 5000, True)
                'filedownloader.DownloadFile(("http://patch01.pso2gs.net/patch_prod/patches/data/win32/" & downloadstring & ".pat"), downloadstring)
                File.Move(downloadstring, ((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
                WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & downloadstring & "."))
                Dim linesList As New List(Of String)(File.ReadAllLines("resume.txt"))

                'Remove the line to delete, e.g.
                linesList.Remove(downloadstring)

                File.WriteAllLines("resume.txt", linesList.ToArray())
                'MsgBox("removed this line - " & downloadstring)
                'MsgBox(downloadstring)
                If CancelledFull = True Then Exit Sub
            Next

            Log(My.Resources.strDownloading & My.Resources.strMissingFilesPart2)

            If File.Exists("resume.txt") Then File.Delete("resume.txt")

            For Each downloadstring2 In missingfiles2
                File.AppendAllText("resume.txt", (downloadstring2 & vbCrLf))
            Next

            Dim totaldownload2 As String = missingfiles2.Count
            Dim downloaded2 As Long = 0
            Dim totaldownloaded2 As Long = 0

            For Each downloadstring2 In missingfiles2
                If CancelledFull = True Then Exit Sub
                'Download the missing files:
                If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & downloadstring2)) = False Then
                    'filedownloader2.DownloadFile(("http://dl.dropboxusercontent.com/u/23005008/win32/" & downloadstring2), downloadstring2)
                    Cancelled = False
                    downloaded2 += 1
                    totaldownloaded2 = totaldownloaded2 + totalsize2

                    lblStatus.Text = My.Resources.strDownloading & "" & downloaded2 & "/" & totaldownload2 & " (" & Helper.SizeSuffix(totaldownloaded2) & ")"

                    DLWUA(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadstring2 & ".pat"), downloadstring2, True)
                    Dim info7 As New FileInfo(downloadstring2)
                    Dim length2 As Long
                    If File.Exists(downloadstring2) = True Then length2 = info7.Length
                    If info7.Length = 0 Then
                        Log("File appears to be empty, trying to download from secondary SEGA server")
                        DLWUA(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring2 & ".pat"), downloadstring2, True)
                    End If
                    If Cancelled = True Then Exit Sub
                    'My.Computer.Network.DownloadFile(("http://patch01.pso2gs.net/patch_prod/patches_old/data/win32/" & downloadstring2 & ".pat"), downloadstring2, vbNullString, vbNullString, True, 5000, True)
                    'filedownloader.DownloadFile(("http://patch01.pso2gs.net/patch_prod/patches_old/data/win32/" & downloadstring2 & ".pat"), downloadstring2)
                    File.Move(downloadstring2, ((lblDirectory.Text & "\data\win32") & "\" & downloadstring2))
                    WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & downloadstring2 & "."))
                    Dim linesList As New List(Of String)(File.ReadAllLines("resume.txt"))

                    'Remove the line to delete, e.g.
                    linesList.Remove(downloadstring2)

                    File.WriteAllLines("resume.txt", linesList.ToArray())
                    'MsgBox("removed this line - " & downloadstring2)
                    'MsgBox(downloadstring)
                End If
            Next
        End If
        WriteDebugInfoAndOK(My.Resources.strallDone)
        'Catch ex As Exception
        ' Log(ex.Message)
        '  WriteDebugInfo(My.Resources.strERROR & ex.Message)
        ' Exit Sub
        ' End Try
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
        Dim versionclient As New MyWebClient
        versionclient.timeout = 3000
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
        'Me.Label2.Text = testfile_Size & " MB"
        WriteDebugInfo(My.Resources.strCheckingConnection)
        WriteDebugInfo(Ping("download.pso2.jp"))
        WriteDebugInfoSameLine("/" & Ping("download.pso2.jp"))
        WriteDebugInfoSameLine("/" & Ping("download.pso2.jp"))
        WriteDebugInfoSameLine("/" & Ping("download.pso2.jp"))
        WriteDebugInfo(My.Resources.strIfTheAboveNumbers)
        'WriteDebugInfo("Checking connection to Ship 02 Server...")
        'WriteDebugInfo(Ping("gs016.pso2gs.net"))
        WriteDebugInfo(My.Resources.strTestingDownloadSpeeds)
        WriteDebugInfo(My.Resources.strDownloadingTestFile & testfile & "...")
        Me.timer_start = 0
        'Me.Label3.Text = Nothing
        'Me.Label4.Text = Nothing
        Me.WebBrowser1.Navigate(testfile)
        Me.seconds.Start()
        'Me.Label1.Text = testfile
        Dim query As Management.ManagementObjectSearcher
        Dim Qc As Management.ManagementObjectCollection
        Dim Oq As Management.ObjectQuery
        Dim Ms As Management.ManagementScope
        Dim Co As Management.ConnectionOptions
        Dim Mo As Management.ManagementObject
        Dim signalStrength As Double
        Try
            Co = New Management.ConnectionOptions
            Ms = New Management.ManagementScope("root\wmi")
            Oq = New Management.ObjectQuery("SELECT * FROM MSNdis_80211_ReceivedSignalStrength Where active=true")
            query = New Management.ManagementObjectSearcher(Ms, Oq)
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
            Dim DirectoryString As String
            Dim pso2launchpath As String
            DirectoryString = (lblDirectory.Text & "\data\win32")
            pso2launchpath = DirectoryString.Replace("\data\win32", "")
            If IO.Directory.Exists(pso2launchpath & "\Gameguard\") Then
                WriteDebugInfo("Removing Gameguard Directory...")
                IO.Directory.Delete(pso2launchpath & "\Gameguard\", True)
                WriteDebugInfoSameLine(My.Resources.strDone)
            End If
            If IO.File.Exists(pso2launchpath & "\GameGuard.des") Then
                WriteDebugInfo("Removing Gameguard File...")
                IO.File.Delete(pso2launchpath & "\GameGuard.des")
                WriteDebugInfoSameLine(My.Resources.strDone)
            End If
            If Environment.Is64BitOperatingSystem = True Then
                systempath = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86)
                If IO.File.Exists(systempath & "\npptnt2.sys") Then
                    WriteDebugInfo("Removing Hidden Gameguard Files (1 of 3)...")
                    IO.File.Delete(systempath & "\npptnt2.sys")
                    WriteDebugInfoSameLine(My.Resources.strDone)
                End If
                If IO.File.Exists(systempath & "\nppt9x.vxd") Then
                    WriteDebugInfo("Removing Hidden Gameguard Files (2 of 3)...")
                    IO.File.Delete(systempath & "\nppt9x.vxd")
                    WriteDebugInfoSameLine(My.Resources.strDone)
                End If
                If IO.File.Exists(systempath & "\GameMon.des") Then
                    WriteDebugInfo("Removing Hidden Gameguard Files (3 of 3)...")
                    IO.File.Delete(systempath & "\GameMon.des")
                    WriteDebugInfoSameLine(My.Resources.strDone)
                End If
            End If
            If Environment.Is64BitOperatingSystem = False Then
                systempath = Environment.GetFolderPath(Environment.SpecialFolder.System)
                If IO.File.Exists(systempath & "\npptnt2.sys") Then
                    WriteDebugInfo("Removing Hidden Gameguard Files (1 of 3)...")
                    IO.File.Delete(systempath & "\npptnt2.sys")
                    WriteDebugInfoSameLine(My.Resources.strDone)
                End If
                If IO.File.Exists(systempath & "\nppt9x.vxd") Then
                    WriteDebugInfo("Removing Hidden Gameguard Files (2 of 3)...")
                    IO.File.Delete(systempath & "\nppt9x.vxd")
                    WriteDebugInfoSameLine(My.Resources.strDone)
                End If
                If IO.File.Exists(systempath & "\GameMon.des") Then
                    WriteDebugInfo("Removing Hidden Gameguard Files (3 of 3)...")
                    IO.File.Delete(systempath & "\GameMon.des")
                    WriteDebugInfoSameLine(My.Resources.strDone)
                End If
            End If
            WriteDebugInfo("Downloading Latest Gameguard file...")
            DLWUA("http://download.pso2.jp/patch_prod/patches/GameGuard.des.pat", "GameGuard.des", True)
            WriteDebugInfo("Downloading Latest Gameguard config...")
            DLWUA("http://download.pso2.jp/patch_prod/patches/PSO2JP.ini.pat", pso2launchpath & "\PSO2JP.ini", True)
            WriteDebugInfoSameLine(My.Resources.strDone)
            IO.File.Move("GameGuard.des", pso2launchpath & "\GameGuard.des")

            ' TODO: look at what this is doing
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
            Dim filedownloader As New Net.WebClient()
            Dim DirectoryString As String
            DirectoryString = (lblDirectory.Text & "\data\win32")
            DirectoryString = DirectoryString.Replace("\data\win32", "")
            DirectoryString = (DirectoryString & "\")
            Cancelled = False
            WriteDebugInfo(My.Resources.strDownloading & "pso2launcher.exe...")

            Application.DoEvents()
            Dim procs As Process() = Process.GetProcessesByName("pso2launcher")

            For Each proc As Process In procs
                If proc.MainWindowTitle = "PHANTASY STAR ONLINE 2" And proc.MainModule.ToString = "ProcessModule (pso2launcher.exe)" Then proc.Kill()
            Next

            DLWUA("http://download.pso2.jp/patch_prod/patches/pso2launcher.exe.pat", "pso2launcher.exe", True)
            If Cancelled = True Then Exit Sub
            'If Application.StartupPath.Contains("\pso2_bin") = False And Application.StartupPath.Contains("\pso2_bin\") = True Or Application.StartupPath.Contains("pso2_bin") = False Then
            Dim DirectoryString2 As String
            DirectoryString2 = (lblDirectory.Text & "\data\win32")
            DirectoryString2 = DirectoryString2.Replace("\data\win32", "")
            'MsgBox(Application.StartupPath & " is supposed to be the same as " & DirectoryString2)
            If File.Exists((DirectoryString & "pso2launcher.exe")) And Application.StartupPath <> DirectoryString2 Then File.Delete((DirectoryString & "pso2launcher.exe"))
            File.Move("pso2launcher.exe", (DirectoryString & "pso2launcher.exe"))
            'End If
            WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & "pso2launcher.exe"))
            WriteDebugInfo(My.Resources.strDownloading & "pso2updater.exe...")
            Application.DoEvents()
            procs = Process.GetProcessesByName("pso2updater")
            For Each proc As Process In procs
                If proc.MainModule.ToString = "ProcessModule (pso2updater.exe)" Then proc.Kill()
            Next
            DLWUA("http://download.pso2.jp/patch_prod/patches/pso2updater.exe.pat", "pso2updater.exe", True)
            If Cancelled = True Then Exit Sub
            DirectoryString2 = (lblDirectory.Text & "\data\win32")
            DirectoryString2 = DirectoryString2.Replace("\data\win32", "")
            If File.Exists((DirectoryString & "pso2updater.exe")) And Application.StartupPath <> DirectoryString2 Then File.Delete((DirectoryString & "pso2updater.exe"))
            File.Move("pso2updater.exe", (DirectoryString & "pso2updater.exe"))
            WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & "pso2updater.exe"))
            WriteDebugInfo(My.Resources.strDownloading & "pso2.exe...")
            Application.DoEvents()
            procs = Process.GetProcessesByName("pso2")
            For Each proc As Process In procs
                If proc.MainModule.ToString = "ProcessModule (pso2.exe)" Then proc.Kill()
            Next
            DLWUA("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", "pso2.exe", True)
            If Cancelled = True Then Exit Sub
            DirectoryString2 = (lblDirectory.Text & "\data\win32")
            DirectoryString2 = DirectoryString2.Replace("\data\win32", "")
            If File.Exists((DirectoryString & "pso2.exe")) And Application.StartupPath <> DirectoryString2 Then File.Delete((DirectoryString & "pso2.exe"))
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
            If Override = False Then
                backupyesno = MsgBox(My.Resources.strAreYouSureRestoreBackup, vbYesNo)
            End If
            If Override = True Then backupyesno = MsgBoxResult.Yes
            If backupyesno = MsgBoxResult.Yes Then
                Dim backupfolder As String = "backupPreSTORYPatch"
                If Directory.Exists(((lblDirectory.Text & "\data\win32") & "\" & backupfolder)) = False Then
                    WriteDebugInfoAndFAILED(My.Resources.strCantFindBackupDirectory & ((lblDirectory.Text & "\data\win32") & "\" & backupfolder))
                    Exit Sub
                End If
                Dim di As New IO.DirectoryInfo(((lblDirectory.Text & "\data\win32") & "\" & backupfolder))
                'MsgBox((lblDirectory.Text & "\data\win32") & "\" & "backupPreENPatch")
                Dim diar1 As IO.FileInfo() = di.GetFiles()
                Dim dra As IO.FileInfo
                WriteDebugInfoAndOK((My.Resources.strRestoringBackupTo & (lblDirectory.Text & "\data\win32")))
                Application.DoEvents()
                'list the names of all files in the specified directory
                Dim win32 As String = (lblDirectory.Text & "\data\win32")
                For Each dra In diar1
                    If File.Exists(win32 & "\" & dra.ToString) = True Then
                        File.Delete(win32 & "\" & dra.ToString)
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
            Dim DirectoryString As String
            DirectoryString = (lblDirectory.Text & "\data\win32")
            DirectoryString = DirectoryString.Replace("\data\win32", "")
            DirectoryString = (DirectoryString & "\")
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
            'If File.Exists(usersettingsfile) Then File.Delete(usersettingsfile)
            WriteDebugInfoAndOK(My.Resources.strPSO2SettingsReset)
        End If
    End Sub

    Private Sub ButtonItem12_Click(sender As Object, e As EventArgs) Handles ButtonItem12.Click
        ' ping download.pso2.jp
        ' ping gs016.pso2gs.net
        ' ping www.google.com
        ' network speed test
        'Me.Label2.Text = testfile_Size & " MB"
        'WriteDebugInfo("Checking connection to Patch Server #1...")
        'WriteDebugInfo(Ping("socket-hack.com:2312/"))
        'WriteDebugInfoSameLine("/" & Ping("socket-hack.com:2312/"))
        'WriteDebugInfoSameLine("/" & Ping("socket-hack.com:2312/"))
        'WriteDebugInfoSameLine("/" & Ping("socket-hack.com:2312/"))
        'WriteDebugInfo("Checking connection to Patch Server #2...")
        'WriteDebugInfo(Ping("pso2.aeongames.net"))
        'WriteDebugInfoSameLine("/" & Ping("pso2.aeongames.net"))
        'WriteDebugInfoSameLine("/" & Ping("pso2.aeongames.net"))
        'WriteDebugInfoSameLine("/" & Ping("pso2.aeongames.net"))
        'WriteDebugInfo("Checking connection to Ship 02 Server...")
        'WriteDebugInfo(Ping("gs016.pso2gs.net"))
        WriteDebugInfo("Testing download speeds for Patch Server #1 (Japan)...")
        WriteDebugInfo("Downloading Disko Warp x Pump It Up Pro 2 Official Soundtrack Sampler...")
        Me.timer_start = 0
        'Me.Label3.Text = Nothing
        'Me.Label4.Text = Nothing
        Me.WebBrowser1.Navigate("http://socket-hack.com:2312/Disko%20Warp%20x%20Pump%20It%20Up%20Pro%202%20Official%20Soundtrack%20Sampler.mp3")
        Me.seconds.Start()
        Do While WebBrowser1.ReadyState <> WebBrowserReadyState.Complete
            Application.DoEvents()
        Loop
        WriteDebugInfo("Testing download speeds for Patch Server #2 (North America)...")
        WriteDebugInfo("Downloading Disko Warp x Pump It Up Pro 2 Official Soundtrack Sampler...")
        Me.timer_start = 0
        'Me.Label3.Text = Nothing
        'Me.Label4.Text = Nothing
        'Me.WebBrowser1.Navigate("http://cyberkitsune.net/aida/Disko%20Warp%20x%20Pump%20It%20Up%20Pro%202%20Official%20Soundtrack%20Sampler(2).mp3")
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
        If Me.Visible = True Then
            If chkAlwaysOnTop.Checked = True Then
                frmOptions.TopMost = True
                Me.TopMost = True
                Helper.SetRegKey(Of String)("AlwaysOnTop", "True")
            End If
            If chkAlwaysOnTop.Checked = False Then
                frmOptions.TopMost = False
                Me.TopMost = False
                Helper.SetRegKey(Of String)("AlwaysOnTop", "False")
            End If
        End If
    End Sub

    Private Sub ButtonItem9_Click(sender As Object, e As EventArgs) Handles btnPSO2Options.Click
        Try
            frmPSO2Options.TopMost = Me.TopMost
            'frmPSO2Options.Bounds = Me.Bounds
            frmPSO2Options.Top = frmPSO2Options.Top + 50
            frmPSO2Options.Left = frmPSO2Options.Left + 50
            'If Me.TopMost = True Then frmOptions.TopMost = True
            frmPSO2Options.ShowDialog()
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub ButtonItem13_Click(sender As Object, e As EventArgs) Handles btnOptions.Click
        Try
            frmOptions.CMBStyle.SelectedIndex = -1
            frmOptions.TopMost = Me.TopMost
            frmOptions.Bounds = Me.Bounds
            frmOptions.Top = frmOptions.Top + 50
            frmOptions.Left = frmOptions.Left + 50
            'If Me.TopMost = True Then frmOptions.TopMost = True
            frmOptions.ShowDialog()
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub btnENPatch_Click(sender As Object, e As EventArgs) Handles btnENPatch.Click
        DownloadPatch("http://162.243.211.123/patches/enpatch.txt", "EN Patch", "ENPatch.rar", "ENPatchVersion", My.Resources.strBackupEN, My.Resources.strPleaseSelectPreDownloadENRAR, "backupPreENPatch")
    End Sub

    Private Sub ButtonItem14_Click(sender As Object, e As EventArgs) Handles ButtonItem14.Click
        Me.Close()
    End Sub

    Private Sub btnEXPFULL_Click(sender As Object, e As EventArgs) Handles btnEXPFULL.Click
        Process.Start("http://www.expfull.com/chat")
    End Sub

    Private Sub btnAnnouncements_Click(sender As Object, e As EventArgs) Handles btnAnnouncements.Click
        If DPISetting = "96" Then
            If Me.Width = 420 Then
                Me.Width = 796
                btnAnnouncements.Text = "<"
                If Helper.GetRegKey(Of String)("SidebarEnabled") = "False" Then
                    WriteDebugInfo(My.Resources.strLoadingSidebarPage)
                    Dim t1 As New Threading.Thread(AddressOf LoadSidebar)
                    t1.IsBackground = True
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
        If DPISetting = "120" Then
            If Me.Width = 560 Then
                Me.Width = 1060
                btnAnnouncements.Text = "<"
                If Helper.GetRegKey(Of String)("SidebarEnabled") = "False" Then
                    WriteDebugInfo(My.Resources.strLoadingSidebarPage)
                    Dim t1 As New Threading.Thread(AddressOf LoadSidebar)
                    t1.IsBackground = True
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

    Private Sub WebBrowser4_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles WebBrowser4.DocumentCompleted

    End Sub

    Private Sub WebBrowser4_Navigating(sender As Object, e As WebBrowserNavigatingEventArgs) Handles WebBrowser4.Navigating
        If Me.Visible = True Then
            If e.Url.ToString <> "http://162.243.211.123/freedom/tweaker.html" Then
                Process.Start(e.Url.ToString())
                Dim t1 As New Threading.Thread(AddressOf LoadSidebar)
                t1.IsBackground = True
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
            File.Delete("enpatchfilelist.txt")

            WriteDebugInfo(My.Resources.strUninstallingPatch)
            Dim totaldownload As String = missingfiles.Count
            Dim downloaded As Long = 0
            For Each downloadstring In missingfiles
                downloaded += 1
                If CancelledFull = True Then Exit Sub
                'ListBox1.Items.Add(dra)
                'MsgBox(dra.ToString)
                'OldFileMD5 = GetMD5(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
                'NewFileMD5 = GetMD5(("TEMPPATCHAIDAFOOL\" & dra.ToString))
                'File.Delete(("TEMPPATCHAIDAFOOL\" & dra.ToString))
                'Download JP file
                lblStatus.Text = My.Resources.strUninstalling & downloaded & "/" & totaldownload
                DLWUA(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadstring & ".pat"), downloadstring, True)
                Dim info7 As New FileInfo(downloadstring)
                If info7.Length = 0 Then DLWUA(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring & ".pat"), downloadstring, True)
                'Move JP file to win32
                If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & downloadstring)) Then File.Delete(((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
                File.Move(downloadstring, ((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
                'If OldFileMD5 <> NewFileMD5 Then
                'If OldFileMD5 = GetMD5(((lblDirectory.Text & "\data\win32") & "\" & downloadstring)) Then
                'WriteDebugInfoAndFAILED("Old file " & ((lblDirectory.Text & "\data\win32") & "\" & downloadstring) & " still exists! File was NOT overwritten!")
                'End If
                'End If
                'MsgBox(("Moving" & (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\TEMPPATCH\" & downloadstring) & " to " & ((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
            Next

            If My.Computer.FileSystem.DirectoryExists((lblDirectory.Text & "\data\win32") & "\" & "backupPreENPatch") = True Then My.Computer.FileSystem.DeleteDirectory(((lblDirectory.Text & "\data\win32") & "\" & "backupPreENPatch"), FileIO.DeleteDirectoryOption.DeleteAllContents)
            FlashWindow(Me.Handle, 1)
            WriteDebugInfo(My.Resources.strENPatchUninstalled)
            Helper.SetRegKey(Of String)("ENPatchVersion", "Not Installed")
            '            File.Delete("ENPatch.rar")
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
            File.Delete("largefilelist.txt")

            WriteDebugInfo(My.Resources.strUninstallingPatch)
            Dim totaldownload As String = missingfiles.Count
            Dim downloaded As Long = 0
            For Each downloadstring In missingfiles
                downloaded += 1
                If CancelledFull = True Then Exit Sub
                'ListBox1.Items.Add(dra)
                'MsgBox(dra.ToString)
                'OldFileMD5 = GetMD5(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
                'NewFileMD5 = GetMD5(("TEMPPATCHAIDAFOOL\" & dra.ToString))
                'File.Delete(("TEMPPATCHAIDAFOOL\" & dra.ToString))
                'Download JP file
                lblStatus.Text = My.Resources.strUninstalling & downloaded & "/" & totaldownload
                DLWUA(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadstring & ".pat"), downloadstring, True)
                Dim info7 As New FileInfo(downloadstring)
                If info7.Length = 0 Then DLWUA(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring & ".pat"), downloadstring, True)
                'Move JP file to win32
                If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & downloadstring)) Then File.Delete(((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
                File.Move(downloadstring, ((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
                'If OldFileMD5 <> NewFileMD5 Then
                'If OldFileMD5 = GetMD5(((lblDirectory.Text & "\data\win32") & "\" & downloadstring)) Then
                'WriteDebugInfoAndFAILED("Old file " & ((lblDirectory.Text & "\data\win32") & "\" & downloadstring) & " still exists! File was NOT overwritten!")
                'End If
                'End If
                'MsgBox(("Moving" & (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\TEMPPATCH\" & downloadstring) & " to " & ((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
            Next

            If My.Computer.FileSystem.DirectoryExists((lblDirectory.Text & "\data\win32") & "\" & "backupPreLargeFiles") = True Then My.Computer.FileSystem.DeleteDirectory(((lblDirectory.Text & "\data\win32") & "\" & "backupPreLargeFiles"), FileIO.DeleteDirectoryOption.DeleteAllContents)
            FlashWindow(Me.Handle, 1)
            WriteDebugInfo(My.Resources.strLFUninstalled)
            Helper.SetRegKey(Of String)("LargeFilesVersion", "Not Installed")
            '            File.Delete("ENPatch.rar")
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
            File.Delete("storyfilelist.txt")

            WriteDebugInfo(My.Resources.strUninstallingPatch)
            Dim totaldownload As String = missingfiles.Count
            Dim downloaded As Long = 0
            For Each downloadstring In missingfiles
                downloaded += 1
                If CancelledFull = True Then Exit Sub
                'ListBox1.Items.Add(dra)
                'MsgBox(dra.ToString)
                'OldFileMD5 = GetMD5(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
                'NewFileMD5 = GetMD5(("TEMPPATCHAIDAFOOL\" & dra.ToString))
                'File.Delete(("TEMPPATCHAIDAFOOL\" & dra.ToString))
                'Download JP file
                lblStatus.Text = My.Resources.strUninstalling & downloaded & "/" & totaldownload
                DLWUA(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadstring & ".pat"), downloadstring, True)
                Dim info7 As New FileInfo(downloadstring)
                If info7.Length = 0 Then DLWUA(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring & ".pat"), downloadstring, True)
                'Move JP file to win32
                If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & downloadstring)) Then File.Delete(((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
                File.Move(downloadstring, ((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
                'If OldFileMD5 <> NewFileMD5 Then
                'If OldFileMD5 = GetMD5(((lblDirectory.Text & "\data\win32") & "\" & downloadstring)) Then
                'WriteDebugInfoAndFAILED("Old file " & ((lblDirectory.Text & "\data\win32") & "\" & downloadstring) & " still exists! File was NOT overwritten!")
                'End If
                'End If
                'MsgBox(("Moving" & (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\TEMPPATCH\" & downloadstring) & " to " & ((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
            Next

            If My.Computer.FileSystem.DirectoryExists((lblDirectory.Text & "\data\win32") & "\" & "backupPreSTORYPatch") = True Then My.Computer.FileSystem.DeleteDirectory(((lblDirectory.Text & "\data\win32") & "\" & "backupPreSTORYPatch"), FileIO.DeleteDirectoryOption.DeleteAllContents)
            FlashWindow(Me.Handle, 1)
            WriteDebugInfo(My.Resources.strStoryPatchUninstalled)
            Helper.SetRegKey(Of String)("StoryPatchVersion", "Not Installed")
            '            File.Delete("ENPatch.rar")
            UnlockGUI()
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub
    Private Sub btnRussianPatch_Click(sender As Object, e As EventArgs) Handles btnRussianPatch.Click
        CancelledFull = False
        Try
            Dim backupyesno As MsgBoxResult
            Dim predownloadedyesno As MsgBoxResult
            Dim RARLocation As String = ""
            Dim strVersion As String = ""
            If (Directory.Exists((lblDirectory.Text & "\data\win32")) = False OrElse lblDirectory.Text = "lblDirectory") Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                Button1.RaiseClick()
                Exit Sub
            End If
            If Helper.GetRegKey(Of String)("PredownloadedRAR") = "Ask" Then predownloadedyesno = MsgBox(My.Resources.strWouldYouLikeToUse, vbYesNo)
            If Helper.GetRegKey(Of String)("PredownloadedRAR") = "Always" Then predownloadedyesno = MsgBoxResult.Yes
            If Helper.GetRegKey(Of String)("PredownloadedRAR") = "Never" Then predownloadedyesno = MsgBoxResult.No
            If Helper.GetRegKey(Of String)("Backup") = "Ask" Then backupyesno = MsgBox("Would you like to backup your files before applying the patch? This will erase all previous Pre-RU patch backups.", vbYesNo)
            If Helper.GetRegKey(Of String)("Backup") = "Always" Then backupyesno = MsgBoxResult.Yes
            If Helper.GetRegKey(Of String)("Backup") = "Never" Then backupyesno = MsgBoxResult.No
            If predownloadedyesno = MsgBoxResult.No Then
                WriteDebugInfo(My.Resources.strDownloading & "RU patch...")
                Application.DoEvents()
                If CheckLink("http://46.150.76.126/pso2/rupatch.rar") <> "OK" Then
                    WriteDebugInfoAndFAILED("Failed to contact RU Patch website - Patch install canceled!")
                    WriteDebugInfo("Please visit http://vk.com/pso2rus for more information!")
                    Exit Sub
                End If
                DLWUA("http://46.150.76.126/pso2/rupatch.rar", "RUPatch.rar", True)
                If Cancelled = True Then Exit Sub
                'My.Computer.Network.DownloadFile(strDownloadME, "ENPatch.rar", vbNullString, vbNullString, True, 5000, True)
                'net.DownloadFile(strDownloadME, "ENPatch.rar")
                WriteDebugInfo(My.Resources.strDownloadCompleteDownloaded & "http://46.150.76.126/pso2/rupatch.rar")
            End If
            If predownloadedyesno = MsgBoxResult.Yes Then
                OpenFileDialog1.Title = "Please select the pre-downloaded RU Patch RAR file"
                OpenFileDialog1.FileName = "PSO2 RU Patch RAR file"
                OpenFileDialog1.Filter = "RAR Archives|*.rar|All Files (*.*) |*.*"
                Dim result = OpenFileDialog1.ShowDialog()
                If result = DialogResult.Cancel Then
                    Exit Sub
                End If
                RARLocation = OpenFileDialog1.FileName.ToString()
                MsgBox(RARLocation)
                strVersion = OpenFileDialog1.SafeFileName
                strVersion = strVersion.Replace(".rar", "")
            End If
            Application.DoEvents()
            If Directory.Exists("TEMPPATCHAIDAFOOL") = True Then
                My.Computer.FileSystem.DeleteDirectory("TEMPPATCHAIDAFOOL", FileIO.DeleteDirectoryOption.DeleteAllContents)
                Directory.CreateDirectory("TEMPPATCHAIDAFOOL")
            End If
            If Directory.Exists("TEMPPATCHAIDAFOOL") = False Then
                Directory.CreateDirectory("TEMPPATCHAIDAFOOL")
            End If
            Dim process As Process = Nothing
            Dim processStartInfo As ProcessStartInfo
            processStartInfo = New ProcessStartInfo()
            Dim UnRarLocation As String
            UnRarLocation = (Application.StartupPath & "\unrar.exe")
            UnRarLocation = UnRarLocation.Replace("\\", "\")
            processStartInfo.FileName = UnRarLocation
            processStartInfo.Verb = "runas"
            If predownloadedyesno = MsgBoxResult.No Then processStartInfo.Arguments = ("e RUPatch.rar " & "TEMPPATCHAIDAFOOL")
            If predownloadedyesno = MsgBoxResult.Yes Then processStartInfo.Arguments = ("e " & """" & RARLocation & """" & " TEMPPATCHAIDAFOOL")
            processStartInfo.WindowStyle = ProcessWindowStyle.Normal
            processStartInfo.UseShellExecute = True
            process = process.Start(processStartInfo)
            WriteDebugInfo(My.Resources.strWaitingforPatch)
            Do Until process.WaitForExit(1000)
            Loop
            If Directory.Exists("TEMPPATCHAIDAFOOL") = False Then
                Directory.CreateDirectory("TEMPPATCHAIDAFOOL")
                WriteDebugInfo("Had to manually make temp update folder - Did the patch not extract right?")
            End If
            Dim di As New IO.DirectoryInfo("TEMPPATCHAIDAFOOL")
            Dim diar1 As IO.FileInfo() = di.GetFiles()
            Dim dra As IO.FileInfo
            WriteDebugInfoAndOK((My.Resources.strExtractingTo & (lblDirectory.Text & "\data\win32")))
            Application.DoEvents()
            If CancelledFull = True Then Exit Sub
            'list the names of all files in the specified directory
            Dim backupdir As String = ((lblDirectory.Text & "\data\win32") & "\" & "backupPreRUPatch")
            If backupyesno = MsgBoxResult.Yes Then
                If Directory.Exists(backupdir) = True Then
                    My.Computer.FileSystem.DeleteDirectory(backupdir, FileIO.DeleteDirectoryOption.DeleteAllContents)
                    Directory.CreateDirectory(backupdir)
                    WriteDebugInfo(My.Resources.strErasingPreviousBackup)
                End If
                If Directory.Exists(backupdir) = False Then
                    Directory.CreateDirectory(backupdir)
                    WriteDebugInfo(My.Resources.strCreatingBackupDirectory)
                End If
                'MsgBox(backupdir)
            End If
            Log("Extracted " & diar1.Count & " files from the patch")
            If diar1.Count = 0 Then
                WriteDebugInfo("Patch failed to extract correctly! Installation failed!")
                Exit Sub
            End If
            WriteDebugInfo(My.Resources.strInstallingPatch)
            For Each dra In diar1
                If CancelledFull = True Then Exit Sub
                'ListBox1.Items.Add(dra)
                'MsgBox(dra.ToString)
                'OldFileMD5 = GetMD5(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
                'NewFileMD5 = GetMD5(("TEMPPATCHAIDAFOOL\" & dra.ToString))
                If backupyesno = MsgBoxResult.Yes Then
                    If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString)) = True Then
                        File.Move(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString), (backupdir & "\" & dra.ToString))
                        'MsgBox(("Moving" & ((lblDirectory.Text & "\data\win32") & "\" & dra.ToString) & " to " & (backupdir & "\" & dra.ToString)))
                        'MsgBox(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
                    End If
                End If
                If backupyesno = MsgBoxResult.No Then
                    If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString)) = True Then
                        File.Delete(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
                    End If
                End If
                File.Move(("TEMPPATCHAIDAFOOL\" & dra.ToString), ((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
                'If OldFileMD5 <> NewFileMD5 Then
                'If OldFileMD5 = GetMD5(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString)) Then
                'WriteDebugInfoAndFAILED("Old file " & ((lblDirectory.Text & "\data\win32") & "\" & dra.ToString) & " still exists! File was NOT overwritten!")
                'End If
                'End If
                'MsgBox(("Moving" & (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\TEMPPATCH\" & dra.ToString) & " to " & ((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
            Next
            'Process.Start("7z.exe", ("e ENPatch.rar -y -o" & (lblDirectory.Text & "\data\win32")))
            My.Computer.FileSystem.DeleteDirectory("TEMPPATCHAIDAFOOL", FileIO.DeleteDirectoryOption.DeleteAllContents)
            If backupyesno = MsgBoxResult.No Then
                FlashWindow(Me.Handle, 1)
                WriteDebugInfo("Russian patch " & My.Resources.strInstalledUpdated)
            End If
            If backupyesno = MsgBoxResult.Yes Then
                FlashWindow(Me.Handle, 1)
                WriteDebugInfo(("Russian patch " & My.Resources.strInstalledUpdatedBackup & backupdir))
            End If
            File.Delete("RUPatch.rar")
            UnlockGUI()
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub

    Private Sub Office2007StartButton1_Click(sender As Object, e As EventArgs) Handles Office2007StartButton1.Click

    End Sub

    Private Sub tsmRestartDownload_Click(sender As Object, e As EventArgs) Handles tsmRestartDownload.Click
        Restartplz = True
    End Sub

    Private Sub lblDownloaded_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub btnResumePatching_Click(sender As Object, e As EventArgs) Handles btnResumePatching.Click
        If File.Exists("resume.txt") = False Then
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
                If CancelledFull = True Then Exit Sub
                While Not (oReader.EndOfStream)
                    If CancelledFull = True Then Exit Sub
                    sBuffer = oReader.ReadLine
                    missingfiles.Add(sBuffer)
                End While
            End Using

            Dim totaldownload As String = missingfiles.Count
            Dim downloaded As Long = 0
            Dim totaldownloaded As Long = 0
            patching = True
            For Each downloadstring In missingfiles
                If CancelledFull = True Then Exit Sub
                'Download the missing files:
                'WHAT THE FUCK IS GOING ON HERE?v3
                downloaded += 1
                totaldownloaded = totaldownloaded + totalsize2

                lblStatus.Text = My.Resources.strDownloading & "" & downloaded & "/" & totaldownload & " (" & Helper.SizeSuffix(totaldownloaded) & ")"

                Application.DoEvents()
                Cancelled = False
                DLWUA(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadstring & ".pat"), downloadstring, True)
                Dim info7 As New FileInfo(downloadstring)
                Dim length2 As Long
                If File.Exists(downloadstring) = True Then length2 = info7.Length
                If info7.Length = 0 Then
                    Log("File appears to be empty, trying to download from secondary SEGA server")
                    DLWUA(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring & ".pat"), downloadstring, True)
                End If
                If Cancelled = True Then Exit Sub
                'filedownloader.DownloadFile(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadstring & ".pat"), downloadstring)
                'DLWUA("http://download.pso2.jp/patch_prod/patches/launcherlist.txt", "launcherlist.txt", True)
                'Delete the existing file FIRST
                If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & downloadstring)) Then File.Delete(((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
                File.Move(downloadstring, ((lblDirectory.Text & "\data\win32") & "\" & downloadstring))
                If VedaUnlocked = True Then WriteDebugInfo("DEBUG: Downloaded and installed " & downloadstring & ".")
                Dim linesList As New List(Of String)(File.ReadAllLines("resume.txt"))

                'Remove the line to delete, e.g.
                linesList.Remove(downloadstring)

                File.WriteAllLines("resume.txt", linesList.ToArray())
                'MsgBox("removed this line - " & downloadstring)
                Application.DoEvents()
                'MsgBox(downloadstring)
            Next
            If File.Exists("resume.txt") Then File.Delete("resume.txt")
            patching = False
            If missingfiles.Count = 0 Then WriteDebugInfo(My.Resources.strYouAppearToBeUpToDate)
            Dim filedownloader3 As New Net.WebClient()
            Dim DirectoryString As String
            DirectoryString = (lblDirectory.Text & "\data\win32")
            DirectoryString = DirectoryString.Replace("\data\win32", "")
            DirectoryString = (DirectoryString & "\")
            WriteDebugInfo(My.Resources.strDownloading & "version file...")
            Application.DoEvents()
            Cancelled = False
            Dim versionclient As New MyWebClient
            versionclient.timeout = 3000
            versionclient.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
            'DLWUA("http://download.pso2.jp/patch_prod/patches/version.ver", "version.ver", True)
            If Cancelled = True Then Exit Sub
            Dim DirectoryString2 As String
            DirectoryString2 = (lblDirectory.Text & "\data\win32")
            DirectoryString2 = DirectoryString2.Replace("\data\win32", "")
            If File.Exists((Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\SEGA\PHANTASYSTARONLINE2\version.ver")) Then File.Delete((Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
            File.Copy("version.ver", (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\SEGA\PHANTASYSTARONLINE2\version.ver"))
            WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & "version file"))
            'DLWUA("http://download.pso2.jp/patch_prod/patches/version.ver", "version.ver", True)
            WriteDebugInfo(My.Resources.strDownloading & "pso2launcher.exe...")
            Application.DoEvents()
            Dim procs As Process()
            procs = Process.GetProcessesByName("pso2launcher")
            For Each proc As Process In procs
                If proc.MainWindowTitle = "PHANTASY STAR ONLINE 2" And proc.MainModule.ToString = "ProcessModule (pso2launcher.exe)" Then proc.Kill()
            Next
            DLWUA("http://download.pso2.jp/patch_prod/patches/pso2launcher.exe.pat", "pso2launcher.exe", True)
            If Cancelled = True Then Exit Sub
            If File.Exists((DirectoryString & "pso2launcher.exe")) And Application.StartupPath <> DirectoryString2 Then File.Delete((DirectoryString & "pso2launcher.exe"))
            File.Move("pso2launcher.exe", (DirectoryString & "pso2launcher.exe"))
            WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & "pso2launcher.exe"))
            WriteDebugInfo(My.Resources.strDownloading & "pso2updater.exe...")
            Application.DoEvents()
            procs = Process.GetProcessesByName("pso2updater")
            For Each proc As Process In procs
                If proc.MainModule.ToString = "ProcessModule (pso2updater.exe)" Then proc.Kill()
            Next
            DLWUA("http://download.pso2.jp/patch_prod/patches/pso2updater.exe.pat", "pso2updater.exe", True)
            If Cancelled = True Then Exit Sub
            If File.Exists((DirectoryString & "pso2updater.exe")) And Application.StartupPath <> DirectoryString2 Then File.Delete((DirectoryString & "pso2updater.exe"))
            File.Move("pso2updater.exe", (DirectoryString & "pso2updater.exe"))
            WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & "pso2updater.exe"))
            Application.DoEvents()
            'Dim NumberofDownloads As Integer = 0
            'DLPSO2EXE:
            'If PSO2EXEMD5 <> GetMD5((DirectoryString & "pso2.exe")) Then
            WriteDebugInfo(My.Resources.strDownloading & "pso2.exe...")
            procs = Process.GetProcessesByName("pso2")
            For Each proc As Process In procs
                If proc.MainModule.ToString = "ProcessModule (pso2.exe)" Then proc.Kill()
            Next
            DLWUA("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", "pso2.exe", True)
            If Cancelled = True Then Exit Sub
            'If PSO2EXEMD5 <> GetMD5((DirectoryString & "pso2.exe")) Then
            ' NumberofDownloads += 1
            ' WriteDebugInfoAndWarning("It appears PSO2.EXE did not download correctly... Retrying...")
            ' If NumberofDownloads < 6 Then GoTo DLPSO2EXE
            ' If NumberofDownloads > 6 Then
            ' WriteDebugInfoAndFAILED("Failed to download pso2.exe correctly!")
            ' Exit Sub
            'End If
            If File.Exists((DirectoryString & "pso2.exe")) And Application.StartupPath <> DirectoryString2 Then File.Delete((DirectoryString & "pso2.exe"))
            File.Move("pso2.exe", (DirectoryString & "pso2.exe"))
            If CancelledFull = True Then Exit Sub
            WriteDebugInfoAndOK((My.Resources.strDownloadedandInstalled & "pso2.exe"))
            Helper.SetRegKey(Of String)("StoryPatchVersion", "Not Installed")
            Helper.SetRegKey(Of String)("ENPatchVersion", "Not Installed")
            Helper.SetRegKey(Of String)("LargeFilesVersion", "Not Installed")
            DLWUA("http://download.pso2.jp/patch_prod/patches/patchlist.txt", "patchlist.txt", True)
            WriteDebugInfoSameLine(My.Resources.strDone)
            Helper.SetRegKey(Of String)("PSO2PatchlistMD5", GetMD5("patchlist.txt"))
            WriteDebugInfo(My.Resources.strGameUpdatedVanilla)
            If File.Exists("resume.txt") Then File.Delete("resume.txt")
            Dim lines2 = File.ReadAllLines("version.ver")
            Dim RemoteVersion2 As String = lines2(0)
            Helper.SetRegKey(Of String)("PSO2RemoteVersion", RemoteVersion2)
            UnlockGUI()

            If Helper.GetRegKey(Of String)("RemoveCensor") = True Then
                If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c.backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "ffbff2ac5b7a7948961212cefd4d402c"), "ffbff2ac5b7a7948961212cefd4d402c.backup")
                WriteDebugInfoAndOK(My.Resources.strRemoving & "Censor...")
            End If

            If Helper.GetRegKey(Of String)("ENPatchAfterInstall") = True Then
                WriteDebugInfo(My.Resources.strAutoInstallingENPatch)
                btnENPatch.RaiseClick()
            End If

            If Helper.GetRegKey(Of String)("LargeFilesAfterInstall") = True Then
                WriteDebugInfo(My.Resources.strAutoInstallingLF)
                btnLargeFiles.RaiseClick()
            End If

            If Helper.GetRegKey(Of String)("StoryPatchAfterInstall") = True Then
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

    Private Sub chkRemoveCensor_CheckedChanged(sender As Object, e As EventArgs) Handles chkRemoveCensor.CheckedChanged

    End Sub

    Private Sub ButtonItem7_Click(sender As Object, e As EventArgs) Handles ButtonItem7.Click
        'MsgBox(My.Resources.strLatestGG)
        Dim ProcessName As String = "chrome"
        p = Process.GetProcessesByName("chrome")
        Dim currentProcess As Process = Process.GetCurrentProcess()

        Dim x As Integer = 0
        'MsgBox(p.Count.tostring)
        If p.Count > x Then
            'MsgBox("It seems that " & ProcessName.Replace(".exe", "") & " is already running. If you don't see it, open the task manager and end task it.")
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

    Private Shared FolderDownloads As New Guid("374DE290-123F-4565-9164-39C4925E467B")

    Private Sub btnInstallPSO2_Click(sender As Object, e As EventArgs) Handles btnInstallPSO2.Click
        Dim InstallFolder As String = ""
        'Dim InstallYesNo As MsgBoxResult = MsgBox("While this installer is great, there is an installer that contains all the latest PSO updates and is available through a torrent at http://arks-layer.com, which could possibly be faster to download than this. Would you like to continue with the installation?", vbYesNo)
        Dim InstallYesNo As MsgBoxResult = vbYes
        If InstallYesNo = vbNo Then
            WriteDebugInfo("You can view more information about the installer at:" & vbCrLf & "http://arks-layer.com/setup.php")
            Exit Sub
        End If
        If InstallYesNo = vbYes Then
            MsgBox("This will install Phantasy Star Online EPISODE 2! Please select a folder to install into." & vbCrLf & "A folder called PHANTASYSTARONLINE2 will be created inside the folder you choose." & vbCrLf & "(For example, if you choose the C drive, it will install to C:\PHANTASYSTARONLINE2\)" & vbCrLf & "It is HIGHLY RECOMMENDED that you do NOT install into the Program Files folder, but a normal folder like C:\PHANTASYSTARONLINE\")
SelectInstallFolder:
            Dim MyFolderBrowser As New FolderBrowserDialog
            MyFolderBrowser.Description = "Please select a folder (or drive) to install PSO2 into"
            ' Sets the root folder where the browsing starts from 
            MyFolderBrowser.RootFolder = Environment.SpecialFolder.MyComputer
            Dim dlgResult As DialogResult = MyFolderBrowser.ShowDialog()

            If dlgResult = Windows.Forms.DialogResult.OK Then
                InstallFolder = MyFolderBrowser.SelectedPath
            End If
            If dlgResult = Windows.Forms.DialogResult.Cancel Then
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
                Dim Searcher As ManagementObjectSearcher
                Searcher = New ManagementObjectSearcher("SELECT * FROM Win32_LogicalDisk")
                Dim InstallDrive As String = InstallFolder.TrimEnd(":")
                InstallDrive = InstallDrive.Replace("\", "")
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
                    Dim client As New MyWebClient
                    client.timeout = 10000
                    Try
                        client.DownloadFile("http://arks-layer.com/docs/dxwebsetup.exe", "dxwebsetup.exe")
                        Dim process As Process
                        Dim processStartInfo As ProcessStartInfo
                        processStartInfo = New ProcessStartInfo()
                        processStartInfo.FileName = "dxwebsetup.exe"
                        processStartInfo.Verb = "runas"
                        processStartInfo.Arguments = "/Q"
                        processStartInfo.UseShellExecute = True
                        process = process.Start(processStartInfo)
                        Do Until process.WaitForExit(1000)
                        Loop
                        WriteDebugInfoSameLine("Done!")

                    Catch ex As Exception
                        WriteDebugInfo("DirectX installation failed! Please install it later if neccessary!")
                    End Try
                    'Make a data folder, and a win32 folder under that
                    IO.Directory.CreateDirectory(pso2_binfolder & "\data\win32\")
                    'Download required pso2 stuff
                    WriteDebugInfo("Downloading PSO2 required files...")
                    DLWUA("http://download.pso2.jp/patch_prod/patches/pso2launcher.exe.pat", pso2_binfolder & "\pso2launcher.exe", True)
                    DLWUA("http://download.pso2.jp/patch_prod/patches/pso2updater.exe.pat", pso2_binfolder & "\pso2updater.exe", True)
                    DLWUA("http://download.pso2.jp/patch_prod/patches/pso2.exe.pat", pso2_binfolder & "\pso2.exe", True)
                    'DLWUA("http://download.pso2.jp/patch_prod/patches/pso2predownload.exe.pat", pso2_binfolder & "\pso2predownload.exe", True)
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
                    If chkItemTranslation.Checked = False Then
                        chkItemTranslation.RaiseClick()
                        WriteDebugInfo("Item patch enabled!")
                    End If
                    'MsgBox("If you wish to install the story patch, please visit http://arks-layer.com/story.php")
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
            ' Unused?
            'Dim RARLocation As String
            Dim strVersion As String
            WriteDebugInfo(My.Resources.strDownloading & "Large Files....")
            Application.DoEvents()
            Dim net As New Net.WebClient()
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
            'strDownloadME = strDownloadME.Replace("<br /><a href=""", "")
            ''http://dl.dropboxusercontent.com/u/12757141/patch_2013_10_09.rar
            'strVersion = strDownloadME.Replace("http://pso2.arghargh200.net/pso2/", "")
            'strVersion = strVersion.Replace("http://dl.dropboxusercontent.com/u/12757141/", "")
            'strVersion = strVersion.Replace("http://107.170.16.100/patchbackups/", "")
            strVersion = strVersion.Replace(".rar", "")
            'MsgBox(strVersion)
            Cancelled = False
            If CheckLink(strDownloadME) <> "OK" Then
                WriteDebugInfoAndFAILED("Failed to contact EN Patch website - Patch install/update canceled!")
                WriteDebugInfo("Please visit http://goo.gl/YzCE7 for more information!")
                Exit Sub
            End If
            DLWUA(strDownloadME, "LargeFiles.rar", True)
            If Cancelled = True Then Exit Sub
            'My.Computer.Network.DownloadFile(strDownloadME, "LargeFiles.rar", vbNullString, vbNullString, True, 5000, True)
            'net.DownloadFile(, "LargeFiles.rar")
            WriteDebugInfo((My.Resources.strDownloadCompleteDownloaded & strDownloadME & ")"))
            Application.DoEvents()
            If Directory.Exists("TEMPPATCHAIDAFOOL") = True Then
                My.Computer.FileSystem.DeleteDirectory("TEMPPATCHAIDAFOOL", FileIO.DeleteDirectoryOption.DeleteAllContents)
                Directory.CreateDirectory("TEMPPATCHAIDAFOOL")
            End If
            If Directory.Exists("TEMPPATCHAIDAFOOL") = False Then
                Directory.CreateDirectory("TEMPPATCHAIDAFOOL")
            End If
            Dim process As Process = Nothing
            Dim processStartInfo As ProcessStartInfo
            processStartInfo = New ProcessStartInfo()
            Dim UnRarLocation As String
            UnRarLocation = (Application.StartupPath & "\unrar.exe")
            UnRarLocation = UnRarLocation.Replace("\\", "\")
            processStartInfo.FileName = UnRarLocation
            processStartInfo.Verb = "runas"
            processStartInfo.Arguments = ("e LargeFiles.rar " & "TEMPPATCHAIDAFOOL")
            processStartInfo.WindowStyle = ProcessWindowStyle.Normal
            processStartInfo.UseShellExecute = True
            process = process.Start(processStartInfo)
            WriteDebugInfo(My.Resources.strWaitingforPatch)
            If CancelledFull = True Then Exit Sub
            Do Until process.WaitForExit(1000)
            Loop
            If CancelledFull = True Then Exit Sub
            If Directory.Exists("TEMPPATCHAIDAFOOL") = False Then
                Directory.CreateDirectory("TEMPPATCHAIDAFOOL")
                WriteDebugInfo("Had to manually make temp update folder - Did the patch not extract right?")
            End If
            Dim di As New IO.DirectoryInfo("TEMPPATCHAIDAFOOL")
            Dim diar1 As IO.FileInfo() = di.GetFiles()
            Dim dra As IO.FileInfo
            WriteDebugInfoAndOK((My.Resources.strExtractingTo & (lblDirectory.Text & "\data\win32")))
            Application.DoEvents()

            'list the names of all files in the specified directory
            ' TODO: Is clone(1)
            Log("Extracted " & diar1.Count & " files from the patch")

            If diar1.Count = 0 Then
                WriteDebugInfo("Patch failed to extract correctly! Installation failed!")
                Exit Sub
            End If

            WriteDebugInfo(My.Resources.strInstallingPatch)


            For Each dra In diar1
                If CancelledFull = True Then Exit Sub
                File.Delete(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
                File.Move(("TEMPPATCHAIDAFOOL\" & dra.ToString), ((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
            Next

            My.Computer.FileSystem.DeleteDirectory("TEMPPATCHAIDAFOOL", FileIO.DeleteDirectoryOption.DeleteAllContents)

            FlashWindow(Me.Handle, 1)
            Helper.SetRegKey(Of String)("LargeFilesVersion", strVersion)
            WriteDebugInfo("Large Files installed!")


            File.Delete("LargeFiles.rar")
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
            Dim net As New Net.WebClient()
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
            Dim strDownloadME As String = txtHTML.Text
            strDownloadME = strDownloadME.Replace("<br /><a href=""", "")
            Dim Lastfilename As String() = strDownloadME.Split("/")
            strVersion = Lastfilename(Lastfilename.Count - 1)
            'http://dl.dropboxusercontent.com/u/12757141/patch_2013_10_09.rar
            'strVersion = strDownloadME.Replace("http://pso2.arghargh200.net/pso2/", "")
            'strVersion = strVersion.Replace("http://dl.dropboxusercontent.com/u/12757141/", "")
            'strVersion = strVersion.Replace("http://107.170.16.100/patchbackups/", "")
            strVersion = strVersion.Replace(".rar", "")
            'If strVersion = Helper.GetRegKey(Of String)("EnPatchVersion") Then
            'Dim InstallYesNo As MsgBoxResult = MsgBox("It seems like you have the latest version already - Would you still like to download and install the EN patch?")
            'If InstallYesNo = vbNo Then Exit Sub
            'End If
            Cancelled = False
            If CheckLink(strDownloadME) <> "OK" Then
                WriteDebugInfoAndFAILED("Failed to contact EN Patch website - Patch install/update canceled!")
                WriteDebugInfo("Please visit http://goo.gl/YzCE7 for more information!")
                Exit Sub
            End If
            DLWUA(strDownloadME, "ENPatch.rar", True)
            If Cancelled = True Then Exit Sub
            'My.Computer.Network.DownloadFile(strDownloadME, "ENPatch.rar", vbNullString, vbNullString, True, 5000, True)
            'net.DownloadFile(strDownloadME, "ENPatch.rar")
            WriteDebugInfo((My.Resources.strDownloadCompleteDownloaded & strDownloadME & ")"))
            Application.DoEvents()
            If Directory.Exists("TEMPPATCHAIDAFOOL") = True Then
                My.Computer.FileSystem.DeleteDirectory("TEMPPATCHAIDAFOOL", FileIO.DeleteDirectoryOption.DeleteAllContents)
                Directory.CreateDirectory("TEMPPATCHAIDAFOOL")
            End If
            If Directory.Exists("TEMPPATCHAIDAFOOL") = False Then
                Directory.CreateDirectory("TEMPPATCHAIDAFOOL")
            End If
            Dim process As Process = Nothing
            Dim processStartInfo As ProcessStartInfo
            processStartInfo = New ProcessStartInfo()
            Dim UnRarLocation As String
            UnRarLocation = (Application.StartupPath & "\unrar.exe")
            UnRarLocation = UnRarLocation.Replace("\\", "\")
            processStartInfo.FileName = UnRarLocation
            processStartInfo.Verb = "runas"
            processStartInfo.Arguments = ("e ENPatch.rar " & "TEMPPATCHAIDAFOOL")
            processStartInfo.WindowStyle = ProcessWindowStyle.Normal
            processStartInfo.UseShellExecute = True
            process = process.Start(processStartInfo)
            WriteDebugInfo(My.Resources.strWaitingforPatch)
            Do Until process.WaitForExit(1000)
            Loop
            If Directory.Exists("TEMPPATCHAIDAFOOL") = False Then
                Directory.CreateDirectory("TEMPPATCHAIDAFOOL")
                WriteDebugInfo("Had to manually make temp update folder - Did the patch not extract right?")
            End If
            Dim di As New IO.DirectoryInfo("TEMPPATCHAIDAFOOL")
            Dim diar1 As IO.FileInfo() = di.GetFiles()
            Dim dra As IO.FileInfo
            WriteDebugInfoAndOK((My.Resources.strExtractingTo & (lblDirectory.Text & "\data\win32")))
            Application.DoEvents()
            If CancelledFull = True Then Exit Sub
            'list the names of all files in the specified directory
            Dim backupdir As String = ((lblDirectory.Text & "\data\win32") & "\" & "backupPreENPatch")

            ' TODO: Is clone(1)
            Log("Extracted " & diar1.Count & " files from the patch")

            If diar1.Count = 0 Then
                WriteDebugInfo("Patch failed to extract correctly! Installation failed!")
                Exit Sub
            End If

            WriteDebugInfo(My.Resources.strInstallingPatch)


            For Each dra In diar1
                If CancelledFull = True Then Exit Sub
                File.Delete(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
                File.Move(("TEMPPATCHAIDAFOOL\" & dra.ToString), ((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
            Next

            My.Computer.FileSystem.DeleteDirectory("TEMPPATCHAIDAFOOL", FileIO.DeleteDirectoryOption.DeleteAllContents)

            FlashWindow(Me.Handle, 1)
            WriteDebugInfo("English patch installed!")
            Helper.SetRegKey(Of String)("ENPatchVersion", strVersion)
            File.Delete("ENPatch.rar")
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
            Dim ReturnString As String
            ReturnString = ex.Message.ToString
            ReturnString = ReturnString.Replace("The remote server returned an error: ", "")
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
        CancelledFull = False
        Try
            Dim backupyesno As MsgBoxResult
            Dim predownloadedyesno As MsgBoxResult
            Dim RARLocation As String = ""
            Dim strVersion As String = ""
            If (Directory.Exists((lblDirectory.Text & "\data\win32")) = False OrElse lblDirectory.Text = "lblDirectory") Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                Button1.RaiseClick()
                Exit Sub
            End If

            ' TODO: fix like the other that was like this
            If Helper.GetRegKey(Of String)("PredownloadedRAR") = "Ask" Then predownloadedyesno = MsgBox(My.Resources.strWouldYouLikeToUse, vbYesNo)
            If Helper.GetRegKey(Of String)("PredownloadedRAR") = "Always" Then predownloadedyesno = MsgBoxResult.Yes
            If Helper.GetRegKey(Of String)("PredownloadedRAR") = "Never" Then predownloadedyesno = MsgBoxResult.No
            If Helper.GetRegKey(Of String)("Backup") = "Ask" Then backupyesno = MsgBox("Would you like to backup your files before applying the patch? This will erase all previous Pre-ES patch backups.", vbYesNo)
            If Helper.GetRegKey(Of String)("Backup") = "Always" Then backupyesno = MsgBoxResult.Yes
            If Helper.GetRegKey(Of String)("Backup") = "Never" Then backupyesno = MsgBoxResult.No
            If predownloadedyesno = MsgBoxResult.No Then
                WriteDebugInfo(My.Resources.strDownloading & "ES patch...")
                Application.DoEvents()
                Dim net As New Net.WebClient()
                Dim src As String
                'http://162.243.211.123/pso2patches/upload.html
                src = net.DownloadString("http://162.243.211.123/pso2patches/espatch.html")
                'MsgBox(src.ToString)
                ' Create a match using regular exp<b></b>ressions
                'http://arks-layer.com/Story%20Patch%208-8-2013.rar.torrent
                'Dim m As Match = Regex.Match(src, "Story Patch (.*?).rar.torrent")
                Dim m As Match = Regex.Match(src, "<b>.*?</b>")
                'MsgBox(m.Value)
                ' Spit out the value plucked from the code
                txtHTML.Text = m.Value
                Dim strDownloadME As String = txtHTML.Text
                strDownloadME = strDownloadME.Replace("<b>", "")
                strDownloadME = strDownloadME.Replace("</b>", "")
                strDownloadME = "http://162.243.211.123/pso2patches/uploads/" & strDownloadME
                'http://162.243.211.123/pso2patches/espatch.html
                'If CheckLink("Link here") <> "OK" Then
                ' WriteDebugInfoAndFAILED("Failed to contact ES Patch website - Patch install canceled!")
                ' WriteDebugInfo("Please visit https://www.facebook.com/pso2es?ref=hl for more information!")
                ' Exit Sub
                'End If
                DLWUA(strDownloadME, "ESPatch.rar", True)
                If Cancelled = True Then Exit Sub
                'My.Computer.Network.DownloadFile(strDownloadME, "ENPatch.rar", vbNullString, vbNullString, True, 5000, True)
                'net.DownloadFile(strDownloadME, "ENPatch.rar")
                WriteDebugInfoSameLine(" Done!")
            End If
            If predownloadedyesno = MsgBoxResult.Yes Then
                OpenFileDialog1.Title = "Please select the pre-downloaded ES Patch RAR file"
                OpenFileDialog1.FileName = "PSO2 ES Patch RAR file"
                OpenFileDialog1.Filter = "RAR Archives|*.rar|All Files (*.*) |*.*"
                Dim result = OpenFileDialog1.ShowDialog()
                If result = DialogResult.Cancel Then
                    Exit Sub
                End If
                RARLocation = OpenFileDialog1.FileName.ToString()
                MsgBox(RARLocation)
                strVersion = OpenFileDialog1.SafeFileName
                strVersion = strVersion.Replace(".rar", "")
            End If
            Application.DoEvents()
            If Directory.Exists("TEMPPATCHAIDAFOOL") = True Then
                My.Computer.FileSystem.DeleteDirectory("TEMPPATCHAIDAFOOL", FileIO.DeleteDirectoryOption.DeleteAllContents)
                Directory.CreateDirectory("TEMPPATCHAIDAFOOL")
            End If
            If Directory.Exists("TEMPPATCHAIDAFOOL") = False Then
                Directory.CreateDirectory("TEMPPATCHAIDAFOOL")
            End If
            Dim process As Process = Nothing
            Dim processStartInfo As ProcessStartInfo
            processStartInfo = New ProcessStartInfo()
            Dim UnRarLocation As String
            UnRarLocation = (Application.StartupPath & "\unrar.exe")
            UnRarLocation = UnRarLocation.Replace("\\", "\")
            processStartInfo.FileName = UnRarLocation
            processStartInfo.Verb = "runas"
            If predownloadedyesno = MsgBoxResult.No Then processStartInfo.Arguments = ("e ESPatch.rar " & "TEMPPATCHAIDAFOOL")
            If predownloadedyesno = MsgBoxResult.Yes Then processStartInfo.Arguments = ("e " & """" & RARLocation & """" & " TEMPPATCHAIDAFOOL")
            processStartInfo.WindowStyle = ProcessWindowStyle.Normal
            processStartInfo.UseShellExecute = True
            process = process.Start(processStartInfo)
            WriteDebugInfo(My.Resources.strWaitingforPatch)
            Do Until process.WaitForExit(1000)
            Loop
            If Directory.Exists("TEMPPATCHAIDAFOOL") = False Then
                Directory.CreateDirectory("TEMPPATCHAIDAFOOL")
                WriteDebugInfo("Had to manually make temp update folder - Did the patch not extract right?")
            End If
            Dim di As New IO.DirectoryInfo("TEMPPATCHAIDAFOOL")
            Dim diar1 As IO.FileInfo() = di.GetFiles()
            Dim dra As IO.FileInfo
            WriteDebugInfoAndOK((My.Resources.strExtractingTo & (lblDirectory.Text & "\data\win32")))
            Application.DoEvents()
            If CancelledFull = True Then Exit Sub
            'list the names of all files in the specified directory
            Dim backupdir As String = ((lblDirectory.Text & "\data\win32") & "\" & "backupPreESPatch")
            If backupyesno = MsgBoxResult.Yes Then
                If Directory.Exists(backupdir) = True Then
                    My.Computer.FileSystem.DeleteDirectory(backupdir, FileIO.DeleteDirectoryOption.DeleteAllContents)
                    Directory.CreateDirectory(backupdir)
                    WriteDebugInfo(My.Resources.strErasingPreviousBackup)
                End If
                If Directory.Exists(backupdir) = False Then
                    Directory.CreateDirectory(backupdir)
                    WriteDebugInfo(My.Resources.strCreatingBackupDirectory)
                End If
                'MsgBox(backupdir)
            End If
            Log("Extracted " & diar1.Count & " files from the patch")
            If diar1.Count = 0 Then
                WriteDebugInfo("Patch failed to extract correctly! Installation failed!")
                Exit Sub
            End If
            WriteDebugInfo(My.Resources.strInstallingPatch)
            For Each dra In diar1
                If CancelledFull = True Then Exit Sub
                'ListBox1.Items.Add(dra)
                'MsgBox(dra.ToString)
                'OldFileMD5 = GetMD5(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
                'NewFileMD5 = GetMD5(("TEMPPATCHAIDAFOOL\" & dra.ToString))
                If backupyesno = MsgBoxResult.Yes Then
                    If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString)) = True Then
                        File.Move(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString), (backupdir & "\" & dra.ToString))
                        'MsgBox(("Moving" & ((lblDirectory.Text & "\data\win32") & "\" & dra.ToString) & " to " & (backupdir & "\" & dra.ToString)))
                        'MsgBox(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
                    End If
                End If
                If backupyesno = MsgBoxResult.No Then
                    If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString)) = True Then
                        File.Delete(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
                    End If
                End If
                File.Move(("TEMPPATCHAIDAFOOL\" & dra.ToString), ((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
                'If OldFileMD5 <> NewFileMD5 Then
                'If OldFileMD5 = GetMD5(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString)) Then
                'WriteDebugInfoAndFAILED("Old file " & ((lblDirectory.Text & "\data\win32") & "\" & dra.ToString) & " still exists! File was NOT overwritten!")
                'End If
                'End If
                'MsgBox(("Moving" & (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\TEMPPATCH\" & dra.ToString) & " to " & ((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
            Next
            'Process.Start("7z.exe", ("e ENPatch.rar -y -o" & (lblDirectory.Text & "\data\win32")))
            My.Computer.FileSystem.DeleteDirectory("TEMPPATCHAIDAFOOL", FileIO.DeleteDirectoryOption.DeleteAllContents)
            If backupyesno = MsgBoxResult.No Then
                FlashWindow(Me.Handle, 1)
                WriteDebugInfo("Spanish patch " & My.Resources.strInstalledUpdated)
                Helper.SetRegKey(Of String)("ENPatchVersion", "Not Installed")
            End If
            If backupyesno = MsgBoxResult.Yes Then
                FlashWindow(Me.Handle, 1)
                WriteDebugInfo(("Spanish patch " & My.Resources.strInstalledUpdatedBackup & backupdir))
                Helper.SetRegKey(Of String)("ENPatchVersion", "Not Installed")
            End If
            File.Delete("ESPatch.rar")
            UnlockGUI()
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
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
        Process.Start("http://arghargh200.net/?page=donators")
    End Sub

    Public Sub setserverstatus(ByVal serverstatus As String)
        If serverstatus = "ONLINE" Then
            Label5.ForeColor = Color.Green
            Label5.Text = "ONLINE"
        End If
        If serverstatus = "OFFLINE" Then
            Label5.ForeColor = Color.Red
            Label5.Text = "OFFLINE"
        End If
    End Sub

    Private Sub IsServerOnline()

        'This isn't working at the moment. Let's just exit the sub for now.
        Exit Sub

        ' The warnings were really bugging me, just uncomment when you want it back -Matthew
        ' TODO: Fix this and uncomment

        'Dim sock As TcpClient
        'Dim ip As String = "210.129.209.16"
        'Dim port As Int32 = 12200

        'Try
        '    sock = New TcpClient()
        '    sock.NoDelay = True
        '    sock.Connect(ip, port)
        '    Dim stream As NetworkStream = sock.GetStream()
        '    ' Receive the TcpServer.response. 
        '    Dim data As [Byte]()
        '    ' Buffer to store the response bytes.
        '    data = New [Byte](256) {}

        '    ' String to store the response ASCII representation. 
        '    Dim responseData As [String] = [String].Empty

        '    ' Read the first batch of the TcpServer response bytes. 
        '    Dim bytes As Int32 = stream.Read(data, 0, data.Length)
        '    responseData = Encoding.ASCII.GetString(data, 0, bytes)
        '    Label5.Invoke(New Action(Of String)(AddressOf setserverstatus), "ONLINE")
        '    stream.Close()
        '    sock.Close()

        'Catch ex As Exception
        '    Label5.Invoke(New Action(Of String)(AddressOf setserverstatus), "OFFLINE")
        'End Try
    End Sub

    Private Sub tmrCheckServerStatus_Tick(sender As Object, e As EventArgs) Handles tmrCheckServerStatus.Tick
        Dim Oldstatus As String = Label5.Text
        Dim t5 As New Threading.Thread(AddressOf IsServerOnline)
        t5.IsBackground = True
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

    Private TargetProcessHandle As Integer
    Private pfnStartAddr As Integer
    Private pszLibFileRemote As String
    Private TargetBufferSize As Integer
    Private Const MEM_COMMIT = 4096
    Private Const PAGE_READWRITE = 4
    Private Const PROCESS_CREATE_THREAD = (&H2)
    Private Const PROCESS_VM_OPERATION = (&H8)
    Private Const PROCESS_VM_WRITE = (&H20)


    Dim ExeName As String = IO.Path.GetFileNameWithoutExtension(Application.ExecutablePath)

    Private Sub Inject()
        On Error GoTo 1 ' If error occurs, app will close without any error messages
        Dim TargetProcess As Process() = Process.GetProcessesByName("pso2")
        TargetProcessHandle = OpenProcess(PROCESS_CREATE_THREAD Or PROCESS_VM_OPERATION Or PROCESS_VM_WRITE, False, TargetProcess(0).Id)
        pszLibFileRemote = OFDSweetFX.FileName()
        pfnStartAddr = GetProcAddress(GetModuleHandle("Kernel32"), "LoadLibraryA")
        TargetBufferSize = 1 + Len(pszLibFileRemote)
        Dim Rtn As Integer
        Dim LoadLibParamAdr As Integer
        LoadLibParamAdr = VirtualAllocEx(TargetProcessHandle, 0, TargetBufferSize, MEM_COMMIT, PAGE_READWRITE)
        Rtn = WriteProcessMemory(TargetProcessHandle, LoadLibParamAdr, pszLibFileRemote, TargetBufferSize, 0)
        CreateRemoteThread(TargetProcessHandle, 0, 0, pfnStartAddr, LoadLibParamAdr, 0, 0)
        CloseHandle(TargetProcessHandle)
        MsgBox("Injected!")
1:      Me.Show()
    End Sub

    Private Sub btnSelectSweetFX_Click(sender As Object, e As EventArgs) Handles btnSelectSweetFX.Click
        OFDSweetFX.Filter = "DLL (*.dll) |*.dll|(*.*) |*.*"
        OFDSweetFX.ShowDialog()
        Dim DllFileName As String = OFDSweetFX.FileName
    End Sub

    Private Sub Injectstuff()
        Dim hWnd As IntPtr = FindWindow("Phantasy Star Online 2", Nothing)
        Do Until hWnd <> IntPtr.Zero
            hWnd = FindWindow("Phantasy Star Online 2", Nothing)
            Thread.Sleep(10)
        Loop
        Dim TargetProcess As Process() = Process.GetProcessesByName("pso2")
        If TargetProcess.Length = 0 Then

        Else
            Call Inject()
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
        totaldownloaded = totaldownloaded + totalsize2

        lblStatus.Text = My.Resources.strDownloading & " lobby video (" & Helper.SizeSuffix(totaldownloaded) & ")"

        DLWUA(("http://download.pso2.jp/patch_prod/patches/data/win32/" & downloadstring & ".pat"), downloadstring, True)
        Dim info7 As New FileInfo(downloadstring)
        Dim length2 As Long
        If File.Exists(downloadstring) = True Then length2 = info7.Length
        If info7.Length = 0 Then
            Log("File appears to be empty, trying to download from secondary SEGA server")
            DLWUA(("http://download.pso2.jp/patch_prod/patches_old/data/win32/" & downloadstring & ".pat"), downloadstring, True)
        End If
        If Cancelled = True Then Exit Sub
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
            'frmPSO2Options.Bounds = Me.Bounds
            frmDiagnostic.Top = frmDiagnostic.Top + 50
            frmDiagnostic.Left = frmDiagnostic.Left + 50
            'If Me.TopMost = True Then frmOptions.TopMost = True
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
            If PublickeyUrl.Contains("publickey.blob") = False Then
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
            'MsgBox(Version & vbCrLf & Host & vbCrLf & ProxyName & vbCrLf & PublickeyUrl)
            WriteDebugInfoSameLine(" Done!")


            Dim FILE_NAME As String = Environment.SystemDirectory & "\drivers\etc\hosts"
            Dim BuiltFile As String = ""
            Dim CurrentLine As String = ""
            Dim AlreadyModified As Boolean = False
            'http://162.243.211.123/test.json
            Dim objReader As New StreamReader(FILE_NAME)

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
                BuiltFile += CurrentLine & vbNewLine
            Loop
            objReader.Close()

            If AlreadyModified = True Then WriteDebugInfo("Modifying HOSTS file...")

            If AlreadyModified = False Then
                BuiltFile += Host & " gs001.pso2gs.net #" & ProxyName & " Ship 01" & vbNewLine
                BuiltFile += Host & " gs016.pso2gs.net #" & ProxyName & " Ship 02" & vbNewLine
                BuiltFile += Host & " gs031.pso2gs.net #" & ProxyName & " Ship 03" & vbNewLine
                BuiltFile += Host & " gs046.pso2gs.net #" & ProxyName & " Ship 04" & vbNewLine
                BuiltFile += Host & " gs061.pso2gs.net #" & ProxyName & " Ship 05" & vbNewLine
                BuiltFile += Host & " gs076.pso2gs.net #" & ProxyName & " Ship 06" & vbNewLine
                BuiltFile += Host & " gs091.pso2gs.net #" & ProxyName & " Ship 07" & vbNewLine
                BuiltFile += Host & " gs106.pso2gs.net #" & ProxyName & " Ship 08" & vbNewLine
                BuiltFile += Host & " gs121.pso2gs.net #" & ProxyName & " Ship 09" & vbNewLine
                BuiltFile += Host & " gs136.pso2gs.net #" & ProxyName & " Ship 10" & vbNewLine
                WriteDebugInfo("Previous modifications not found, creating new entries...")
            End If
            'http://162.243.211.123/test.json
            'MsgBox(BuiltFile)
            File.WriteAllText(Environment.SystemDirectory & "\drivers\etc\hosts", BuiltFile)
            WriteDebugInfoSameLine(" Done!")

            WriteDebugInfo("Downloading and installing publickey.blob...")
            myWebClient.DownloadFile(PublickeyUrl, Application.StartupPath & "\publickey.blob")
            If File.Exists(lblDirectory.Text & "\publickey.blob") = True And Application.StartupPath <> lblDirectory.Text Then File.Delete(lblDirectory.Text & "\publickey.blob")
            If Application.StartupPath <> lblDirectory.Text Then File.Move(Application.StartupPath & "\publickey.blob", lblDirectory.Text & "\publickey.blob")
            WriteDebugInfoSameLine(" Done!")
            WriteDebugInfo("All done! You should now be able to connect to " & ProxyName & ".")
            Helper.SetRegKey(Of String)("ProxyEnabled", "True")
        Catch ex As Exception
            WriteDebugInfoAndFAILED("ERROR - " & ex.Message)
            If ex.Message.ToString.Contains("is denied.") And ex.Message.ToString.Contains("Access to the path") Then MsgBox("It seems you've gotten an error while trying to patch your HOSTS file. Please go to the " & Environment.SystemDirectory & "\drivers\etc\ folder, right click on the hosts file, and make sure ""Read Only"" is not checked. Then try again.")
            Exit Sub
        End Try
    End Sub

    Private Sub btnRevertPSO2ProxyToJP_Click(sender As Object, e As EventArgs) Handles btnRevertPSO2ProxyToJP.Click
        Dim FILE_NAME As String = Environment.SystemDirectory & "\drivers\etc\hosts"
        Dim BuiltFile As String = ""
        Dim CurrentLine As String = ""
        Dim objReader As New StreamReader(FILE_NAME)

        Do While objReader.Peek() <> -1

            CurrentLine = objReader.ReadLine()

            If CurrentLine.Contains("pso2gs.net") Then CurrentLine = ""
            If CurrentLine <> "" Then BuiltFile += CurrentLine & vbNewLine
        Loop

        objReader.Close()

        WriteDebugInfo("Modifying HOSTS file...")
        File.WriteAllText(Environment.SystemDirectory & "\drivers\etc\hosts", BuiltFile)
        WriteDebugInfoSameLine(" Done!")
        File.Delete(lblDirectory.Text & "\publickey.blob")
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
        If Directory.Exists(((lblDirectory.Text & "\data\win32") & "\backupPreENPatch")) = True Then
            WriteDebugInfo(My.Resources.strENBackupFound)
            Override = True
            btnRestoreENBackup.RaiseClick()
            Override = False
        End If
        If Directory.Exists(((lblDirectory.Text & "\data\win32") & "\backupPreLargeFiles")) = True Then
            WriteDebugInfo(My.Resources.strLFBackupFound)
            Override = True
            btnRestoreLargeFilesBackup.RaiseClick()
            Override = False
        End If

        Dim count As Integer = 0
        Dim totalfiles = My.Computer.FileSystem.GetFiles(lblDirectory.Text & "\data\win32\")

        If File.Exists("old_patchlist.txt") = False Then
            Dim Filename As String = ""
            Dim filesize As Long = 0
            WriteDebugInfo("Building file list... ")
            Dim di As New IO.DirectoryInfo(lblDirectory.Text & "\data\win32\")
            Dim diar1 As IO.FileInfo() = di.GetFiles()
            Dim dra As IO.FileInfo
            '            If File.Exists("old_patchlist.txt") Then File.Delete("old_patchlist.txt")


            For Each dra In diar1
                Filename = dra.Name
                filesize = dra.Length
                File.AppendAllText("old_patchlist.txt", "data/win32/" & Filename & ".pat" & vbTab & filesize & vbTab & GetMD5(lblDirectory.Text & "\data\win32\" & Filename) & vbNewLine)
                count += 1
                lblStatus.Text = "Building first time list of win32 files (" & count & "/" & CStr(totalfiles.Count) & ")"
                Application.DoEvents()
            Next
            WriteDebugInfoSameLine("Done!")
        End If
        'MsgBox("Test new built thing")
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
        Dim versionclient As New MyWebClient
        versionclient.timeout = 3000
        versionclient.DownloadFile("http://arks-layer.com/vanila/version.txt", "version.ver")
        WriteDebugInfoSameLine(My.Resources.strDone)
        Application.DoEvents()
        UnlockGUI()
        MergePatches()

        'Rewrite this to support the new format

        Dim SEGALine As String = ""
        Dim LocalLine As String = ""
        Dim SplitSEGALine As String()
        'Dim SplitLocalLine() As String
        Dim SEGAFilename As String = ""
        ' Unused?
        'Dim SEGAFilesize As Long
        Dim missingfiles As New List(Of String)
        Dim sr1 As StreamReader = File.OpenText("SOMEOFTHETHINGS.txt")
        Dim sr2 As StreamReader = New StreamReader("old_patchlist.txt")
        Dim oldarray As New List(Of String)
        Dim Contains As Boolean = False
        count = 0
        Do While sr2.Peek <> -1
            oldarray.Add(sr2.ReadLine)
        Loop
        'MsgBox(sr1.ReadLine)
        Do While sr1.Peek <> -1
            SEGALine = sr1.ReadLine()
            If String.IsNullOrEmpty(sr1.ReadLine) Then
                Continue Do
                ' Mike: Why was this here? It never executed!
                'count -= 1
            End If
            'MsgBox(SEGALine)
            SplitSEGALine = Regex.Split(SEGALine, ".pat")
            SEGAFilename = SplitSEGALine(0).Replace("data/win32/", "")
            'SplitSEGALine = Regex.Split(SplitSEGALine(1), "	")
            'SEGAFilesize = Convert.ToInt32(SplitSEGALine(1))
            'MsgBox("Filename is: " & SEGAFilename & vbNewLine & "Filesize is: " & SEGAFilesize)
            lblStatus.Text = "Checking file " & count & " / " & CStr(totalfiles.Count)
            If missingfiles.Count > 0 Then lblStatus.Text += " (missing files found: " & missingfiles.Count & ")"
            Application.DoEvents()
            'MsgBox(truefilename)
            'MsgBox("Found a mismatched filed! Name: " & SEGAFilename & vbNewLine & "Filesize should be: " & SEGAFilesize & vbNewLine & "but is: " & SplitLocalLine(1).ToString)
            If oldarray.Contains(SEGALine) = False Then missingfiles.Add(SEGAFilename)
            count += 2
        Loop

        sr1.Close()
        sr2.Close()
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

        Dim net As New Net.WebClient()
        Dim src As String
        src = net.DownloadString("http://arks-layer.com/story.php")
        'MsgBox(src.ToString)

        ' Create a match using regular exp<b></b>ressions
        'http://arks-layer.com/Story%20Patch%208-8-2013.rar.torrent
        'Dim m As Match = Regex.Match(src, "Story Patch (.*?).rar.torrent")
        Dim m As Match = Regex.Match(src, "<u>.*?</u>")
        'MsgBox(m.Value)

        ' Spit out the value plucked from the code
        txtHTML.Text = m.Value
        Dim strDownloadME As String = txtHTML.Text
        strDownloadME = strDownloadME.Replace("<u>", "")
        strDownloadME = strDownloadME.Replace("</u>", "")

        strStoryPatchLatestBase = strDownloadME
        strStoryPatchLatestBase = strStoryPatchLatestBase.Replace("/", "-")

        'MsgBox(strStoryPatchLatestBase)

        WriteDebugInfoAndOK("Downloading story patch info... ")
        DLWUA("http://162.243.211.123/freedom/pso2.stripped.db", "pso2.stripped.db", True)
        WriteDebugInfoAndOK("Downloading Trans-Am tool... ")
        DLWUA("http://162.243.211.123/freedom/pso2-transam.exe", "pso2-transam.exe", True)

        'If Directory.Exists(backupdir) = True Then
        ' My.Computer.FileSystem.DeleteDirectory(backupdir, FileIO.DeleteDirectoryOption.DeleteAllContents)
        ' Directory.CreateDirectory(backupdir)
        ' WriteDebugInfo(My.Resources.strErasingPreviousBackup)
        ' End If

        'execute pso2-transam stuff with -b flag for backup
        Dim process As Process = Nothing
        Dim processStartInfo As ProcessStartInfo
        processStartInfo = New ProcessStartInfo()
        processStartInfo.FileName = "pso2-transam.exe"
        processStartInfo.Verb = "runas"

        If Directory.Exists(backupdir) = True Then processStartInfo.Arguments = ("-t story-eng-" & strStoryPatchLatestBase & " pso2.stripped.db " & """" & win32 & """")
        If Directory.Exists(backupdir) = False Then
            Directory.CreateDirectory(backupdir)
            WriteDebugInfo(My.Resources.strCreatingBackupDirectory)
            processStartInfo.Arguments = ("-b " & """" & backupdir & """" & " -t story-eng-" & strStoryPatchLatestBase & " pso2.stripped.db " & """" & win32 & """")
        End If
        'Clipboard.SetText(processStartInfo.Arguments.ToString)
        ' MsgBox("Done!")
        processStartInfo.WindowStyle = ProcessWindowStyle.Normal
        processStartInfo.UseShellExecute = True
        process = process.Start(processStartInfo)
        Do Until process.WaitForExit(1000)
        Loop

        File.Delete("pso2.stripped.db")
        File.Delete("pso2-transam.exe")

        FlashWindow(Me.Handle, 1)
        'Story Patch 3-12-2014.rar
        Helper.SetRegKey(Of String)("StoryPatchVersion", strStoryPatchLatestBase.Replace("-", "/"))
        Helper.SetRegKey(Of String)("LatestStoryBase", strStoryPatchLatestBase.Replace("-", "/"))
        WriteDebugInfo(My.Resources.strStoryPatchInstalled)
        CheckForStoryUpdates()
    End Sub

    Private Sub btnJPEnemyNames_Click(sender As Object, e As EventArgs) Handles btnJPEnemyNames.Click
        Try
            'LockGUI()
            If (Directory.Exists((lblDirectory.Text & "\data\win32")) = False OrElse lblDirectory.Text = "lblDirectory") Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                Button1.RaiseClick()
                Exit Sub
            End If
            'Dim backupyesno As MsgBoxResult = MsgBox(My.Resources.strWouldYouLikeToBackupLargeFiles, vbYesNo)
            WriteDebugInfo(My.Resources.strDownloading & "JP enemy names file....")
            Application.DoEvents()
            'Dim net As New Net.WebClient()
            'Dim src As String
            'src = net.DownloadString("http://psumods.co.uk/viewtopic.php?f=4&t=206")
            ' Create a match using regular exp<b></b>ressions
            'http://pso2.arghargh200.net/pso2/2013_06_12/ceffe0e2386e8d39f188358303a92a7d
            'Dim m As Match = Regex.Match(src, "http://pso2.arghargh200.net/pso2/.*ceffe0e2386e8d39f188358303a92a7d")
            ' Spit out the value plucked from the code
            'txtHTML.Text = m.NextMatch.ToString()
            Dim strDownloadME As String = GrabLink("ceffe0e2386e8d39f188358303a92a7d")
            Dim strVersion As String = strDownloadME.Replace("http://pso2.arghargh200.net/pso2/", "")
            strVersion = strVersion.Replace("http://107.170.16.100/patchbackups/", "")
            'strVersion = strVersion.Replace("/ceffe0e2386e8d39f188358303a92a7d", "")
            'MsgBox(strVersion)
            Cancelled = False
            DLWUA(strDownloadME, "ceffe0e2386e8d39f188358303a92a7d", True)
            If Cancelled = True Then Exit Sub
            'My.Computer.Network.DownloadFile(strDownloadME, "LargeFiles.rar", vbNullString, vbNullString, True, 5000, True)
            'net.DownloadFile(, "LargeFiles.rar")
            WriteDebugInfo((My.Resources.strDownloadCompleteDownloaded & strDownloadME & ")"))
            If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "ceffe0e2386e8d39f188358303a92a7d")) = True Then
                If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "ceffe0e2386e8d39f188358303a92a7d.backup")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "ceffe0e2386e8d39f188358303a92a7d.backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "ceffe0e2386e8d39f188358303a92a7d"), "ceffe0e2386e8d39f188358303a92a7d.backup")
            End If
            Application.DoEvents()
            File.Move("ceffe0e2386e8d39f188358303a92a7d", ((lblDirectory.Text & "\data\win32") & "\ceffe0e2386e8d39f188358303a92a7d"))
            'MsgBox(("Moving" & (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\TEMPPATCH\" & dra.ToString) & " to " & ((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
            'Process.Start("7z.exe", ("e ENPatch.rar -y -o" & (lblDirectory.Text & "\data\win32")))
            FlashWindow(Me.Handle, 1)
            'Helper.SetRegKey(Of String)("LargeFilesVersion", strVersion)
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
            'LockGUI()
            If (Directory.Exists((lblDirectory.Text & "\data\win32")) = False OrElse lblDirectory.Text = "lblDirectory") Then
                MsgBox(My.Resources.strPleaseSelectwin32Dir)
                Button1.RaiseClick()
                Exit Sub
            End If
            'Dim backupyesno As MsgBoxResult = MsgBox(My.Resources.strWouldYouLikeToBackupLargeFiles, vbYesNo)
            WriteDebugInfo(My.Resources.strDownloading & "JP E-Trials file....")
            Application.DoEvents()
            'Dim net As New Net.WebClient()
            'Dim src As String
            'src = net.DownloadString("http://psumods.co.uk/viewtopic.php?f=4&t=206")
            ' Create a match using regular exp<b></b>ressions
            'http://pso2.arghargh200.net/pso2/2013_06_12/057aa975bdd2b372fe092614b0f4399e
            'Dim m As Match = Regex.Match(src, "http://pso2.arghargh200.net/pso2/.*057aa975bdd2b372fe092614b0f4399e")
            ' Spit out the value plucked from the code
            'txtHTML.Text = m.NextMatch.ToString()
            Dim strDownloadME As String = GrabLink("057aa975bdd2b372fe092614b0f4399e")
            Dim strVersion As String = strDownloadME.Replace("http://pso2.arghargh200.net/pso2/", "")
            strVersion = strVersion.Replace("http://107.170.16.100/patchbackups/", "")
            'strVersion = strVersion.Replace("/ceffe0e2386e8d39f188358303a92a7d", "")
            'MsgBox(strVersion)
            Cancelled = False
            DLWUA(strDownloadME, "057aa975bdd2b372fe092614b0f4399e", True)
            If Cancelled = True Then Exit Sub
            'My.Computer.Network.DownloadFile(strDownloadME, "LargeFiles.rar", vbNullString, vbNullString, True, 5000, True)
            'net.DownloadFile(, "LargeFiles.rar")
            WriteDebugInfo((My.Resources.strDownloadCompleteDownloaded & strDownloadME & ")"))
            If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "057aa975bdd2b372fe092614b0f4399e")) = True Then
                If My.Computer.FileSystem.FileExists(((lblDirectory.Text & "\data\win32") & "\" & "057aa975bdd2b372fe092614b0f4399e.backup")) Then My.Computer.FileSystem.DeleteFile(((lblDirectory.Text & "\data\win32") & "\" & "057aa975bdd2b372fe092614b0f4399e.backup"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                My.Computer.FileSystem.RenameFile(((lblDirectory.Text & "\data\win32") & "\" & "057aa975bdd2b372fe092614b0f4399e"), "057aa975bdd2b372fe092614b0f4399e.backup")
            End If
            Application.DoEvents()
            File.Move("057aa975bdd2b372fe092614b0f4399e", ((lblDirectory.Text & "\data\win32") & "\057aa975bdd2b372fe092614b0f4399e"))
            'MsgBox(("Moving" & (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\TEMPPATCH\" & dra.ToString) & " to " & ((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
            'Process.Start("7z.exe", ("e ENPatch.rar -y -o" & (lblDirectory.Text & "\data\win32")))
            FlashWindow(Me.Handle, 1)
            'Helper.SetRegKey(Of String)("LargeFilesVersion", strVersion)
            WriteDebugInfo("JP E-Trials file " & My.Resources.strInstalledUpdated)
            UnlockGUI()
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
            Exit Sub
        End Try
    End Sub

    ' TODO: Do any necessary parsing BEFORE this function as opposed to inside. This'll make it compatible with other language patches.
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
                Dim net As New Net.WebClient()
                Dim src As String = net.DownloadString(PatchURL)
                Dim strDownloadME As String = src

                ' Get the filename from the downloaded Path
                Dim Lastfilename As String() = strDownloadME.Split("/"c)
                strVersion = Lastfilename(Lastfilename.Length - 1)
                strVersion = Path.GetFileNameWithoutExtension(strVersion) ' We're using this so that it's not format-specific.

                Cancelled = False

                If CheckLink(strDownloadME) <> "OK" Then
                    WriteDebugInfoAndFAILED("Failed to contact " & PatchName & " website - Patch install/update canceled!")
                    WriteDebugInfo("Please visit http://goo.gl/YzCE7 for more information!")
                    Exit Sub
                End If

                DLWUA(strDownloadME, PatchFile, True)
                If Cancelled = True Then Exit Sub
                WriteDebugInfo((My.Resources.strDownloadCompleteDownloaded & strDownloadME & ")"))
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

            If Directory.Exists("TEMPPATCHAIDAFOOL") = True Then
                My.Computer.FileSystem.DeleteDirectory("TEMPPATCHAIDAFOOL", FileIO.DeleteDirectoryOption.DeleteAllContents)
                Directory.CreateDirectory("TEMPPATCHAIDAFOOL")
            End If
            If Directory.Exists("TEMPPATCHAIDAFOOL") = False Then
                Directory.CreateDirectory("TEMPPATCHAIDAFOOL")
            End If

            Dim process As Process = Nothing
            Dim processStartInfo As ProcessStartInfo
            processStartInfo = New ProcessStartInfo()
            Dim UnRarLocation As String
            UnRarLocation = (Application.StartupPath & "\unrar.exe")
            UnRarLocation = UnRarLocation.Replace("\\", "\")
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
            If Directory.Exists("TEMPPATCHAIDAFOOL") = False Then
                Directory.CreateDirectory("TEMPPATCHAIDAFOOL")
                WriteDebugInfo("Had to manually make temp update folder - Did the patch not extract right?")
            End If
            Dim di As New IO.DirectoryInfo("TEMPPATCHAIDAFOOL")
            Dim diar1 As IO.FileInfo() = di.GetFiles()
            Dim dra As IO.FileInfo
            WriteDebugInfoAndOK((My.Resources.strExtractingTo & (lblDirectory.Text & "\data\win32")))
            Application.DoEvents()
            If CancelledFull = True Then Exit Sub

            Dim backupstr As String = ((lblDirectory.Text & "\data\win32") & "\" & BackupDir)
            If backupyesno = MsgBoxResult.Yes Then
                If Directory.Exists(backupstr) = True Then
                    My.Computer.FileSystem.DeleteDirectory(backupstr, FileIO.DeleteDirectoryOption.DeleteAllContents)
                    Directory.CreateDirectory(backupstr)
                    WriteDebugInfo(My.Resources.strErasingPreviousBackup)
                End If
                If Directory.Exists(backupstr) = False Then
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
                If CancelledFull = True Then Exit Sub

                If backupyesno = MsgBoxResult.Yes Then
                    If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString)) = True Then
                        File.Move(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString), (backupstr & "\" & dra.ToString))

                    End If
                End If
                If backupyesno = MsgBoxResult.No Then
                    If File.Exists(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString)) = True Then
                        File.Delete(((lblDirectory.Text & "\data\win32") & "\" & dra.ToString))
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
            File.Delete(PatchName)
            UnlockGUI()
        Catch ex As Exception
            Log(ex.Message)
            WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try
    End Sub
End Class