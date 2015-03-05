Imports System.IO
Imports PSO2_Tweaker.My
Imports System.Net

Public Class FrmDiagnostic

    Private Shared Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim totalString As String = ""

        totalString &= "OS: " & Computer.Info.OSFullName & vbCrLf
        totalString &= "64 Bit OS?: " & Environment.Is64BitOperatingSystem.ToString() & vbCrLf
        totalString &= "Tweaker is located at: " & Environment.CurrentDirectory & vbCrLf
        totalString &= ".NET Version: " & Environment.Version.ToString() & vbCrLf
        totalString &= "System has been on for: " & Mid((Environment.TickCount / 3600000).ToString(), 1, 5) & " hours"
        Helper.PasteBinText(totalString)

    End Sub

    Private Shared Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim totalString As String = ""
        For Each line As String In Helper.GetLines(Program.HostsFilePath)
            If line <> "" Then totalString &= line & vbCrLf
        Next
        If totalString = "" Then totalString = "No modified host entries detected!"

        FrmMain.DownloadFile("http://download.sysinternals.com/files/Handle.zip", "Handle.zip")

        Dim processStartInfo2 As New ProcessStartInfo With
        {
            .FileName = (Program.StartPath & "\7za.exe"),
            .Verb = "runas",
            .Arguments = ("e -y Handle.zip"),
            .WindowStyle = ProcessWindowStyle.Hidden,
            .UseShellExecute = True
        }

        Process.Start(processStartInfo2).WaitForExit()

        Dim start_info As New ProcessStartInfo("cmd.exe", "/c handle %windir%\system32\drivers\etc\hosts")
        start_info.UseShellExecute = False
        start_info.CreateNoWindow = False
        start_info.RedirectStandardOutput = True
        start_info.RedirectStandardError = True

        ' Make the process and set its start information.
        Dim proc As New Process()
        proc.StartInfo = start_info

        ' Start the process.
        proc.Start()

        ' Attach to stdout and stderr.
        Dim std_out As IO.StreamReader = proc.StandardOutput()

        ' Display the results.
        Dim OutputText As String = totalString & vbNewLine & "HOSTS Handle stuff:" & vbNewLine & std_out.ReadToEnd()

        ' Clean up.
        std_out.Close()

        proc.Close()
        Helper.PasteBinText(OutputText)

    End Sub

    Private Shared Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim drInfo As New DirectoryInfo(Program.Pso2WinDir)
        Dim filesInfo As FileInfo() = drInfo.GetFiles("*.*", SearchOption.AllDirectories)
        Dim fileSize As Long = filesInfo.Sum(Function(fileInfo) fileInfo.Length)

        Dim totalString As String = ""
        totalString &= "PSO2 Directory: " & Program.Pso2RootDir & vbCrLf
        totalString &= "Current game version: " & RegKey.GetValue(Of String)(RegKey.Pso2RemoteVersion) & vbCrLf
        totalString &= "Item Translation: " & RegKey.GetValue(Of String)(RegKey.UseItemTranslation) & vbCrLf
        totalString &= "EN Patch version installed: " & RegKey.GetValue(Of String)(RegKey.EnPatchVersion) & vbCrLf
        totalString &= "Large Files version installed: " & RegKey.GetValue(Of String)(RegKey.LargeFilesVersion) & vbCrLf
        totalString &= "Story Patch version installed: " & RegKey.GetValue(Of String)(RegKey.StoryPatchVersion) & vbCrLf
        totalString &= "Size of PSO2 data/win32 folder: ~" & fileSize.ToString().Remove(2, 9) & "GB"
        Helper.PasteBinText(totalString)

    End Sub

    Private Shared Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim drInfo As New DirectoryInfo(Program.Pso2RootDir)
        Dim filesInfo As FileInfo() = drInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly)
        Dim fileSize As Long
        Dim filename As String
        Dim totalString As String = "Listing of pso2_bin files: "

        For Each fileInfo As FileInfo In filesInfo
            filename = fileInfo.Name
            fileSize = fileInfo.Length
            totalString &= filename
            If filename = "GameGuard.des" OrElse filename = "pso2.exe" OrElse filename = "publickey.blob" OrElse filename = "rsainject.dll" OrElse filename = "translation.bin" OrElse filename = "translator.dll" Then totalString &= ": " & fileSize.ToString()

            totalString &= " | "
        Next

        Helper.PasteBinText(totalString)

    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim start_info As New ProcessStartInfo("cmd.exe", "/c ipconfig /flushdns")
        start_info.UseShellExecute = False
        start_info.CreateNoWindow = True
        start_info.RedirectStandardOutput = True
        start_info.RedirectStandardError = True

        ' Make the process and set its start information.
        Dim proc As New Process()
        proc.StartInfo = start_info

        ' Start the process.
        proc.Start()

        ' Attach to stdout and stderr.
        Dim std_out As IO.StreamReader = proc.StandardOutput()

        ' Display the results.
        Helper.PasteBinText(std_out.ReadToEnd())

        ' Clean up.
        std_out.Close()

        proc.Close()

    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Dim start_info As New ProcessStartInfo("cmd.exe", "/c ping -n 5 gs016.pso2gs.net")
        start_info.UseShellExecute = False
        start_info.CreateNoWindow = True
        start_info.RedirectStandardOutput = True
        start_info.RedirectStandardError = True

        ' Make the process and set its start information.
        Dim proc As New Process()
        proc.StartInfo = start_info

        ' Start the process.
        proc.Start()

        ' Attach to stdout and stderr.
        Dim std_out As IO.StreamReader = proc.StandardOutput()

        ' Display the results.
        Helper.PasteBinText(std_out.ReadToEnd())

        ' Clean up.
        std_out.Close()

        proc.Close()
    End Sub

    Private Sub FrmDiagnostic_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim host As IPHostEntry = Dns.GetHostEntry("gs016.pso2gs.net")

        Dim ip As IPAddress() = host.AddressList

        Dim index As Integer
        For index = 0 To ip.Length - 1
            Console.WriteLine(ip(index))
        Next index
        lblPSO2Test.Text = "gs016.pso2gs.net (Ship 2) resolves to: " & ip(0).ToString
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Dim start_info As New ProcessStartInfo("cmd.exe", "/c netsh winsock show catalog")
        start_info.UseShellExecute = False
        start_info.CreateNoWindow = True
        start_info.RedirectStandardOutput = True
        start_info.RedirectStandardError = True

        ' Make the process and set its start information.
        Dim proc As New Process()
        proc.StartInfo = start_info

        ' Start the process.
        proc.Start()

        ' Attach to stdout and stderr.
        Dim std_out As IO.StreamReader = proc.StandardOutput()

        ' Display the results.
        Helper.PasteBinText(std_out.ReadToEnd())

        ' Clean up.
        std_out.Close()

        proc.Close()
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        FrmMain.DownloadFile("http://download.sysinternals.com/files/Autoruns.zip", "autoruns.zip")

        Dim processStartInfo2 As New ProcessStartInfo With
{
    .FileName = (Program.StartPath & "\7za.exe"),
    .Verb = "runas",
    .Arguments = ("e -y autoruns.zip"),
    .WindowStyle = ProcessWindowStyle.Hidden,
.UseShellExecute = True
}
        Process.Start(processStartInfo2).WaitForExit()

        Dim start_info As New ProcessStartInfo("cmd.exe", "/c autorunsc.exe -a * -c -s")
        start_info.UseShellExecute = False
        start_info.CreateNoWindow = False
        start_info.RedirectStandardOutput = True
        start_info.RedirectStandardError = True

        ' Make the process and set its start information.
        Dim proc As New Process()
        proc.StartInfo = start_info

        ' Start the process.
        proc.Start()

        ' Attach to stdout and stderr.
        Dim std_out As IO.StreamReader = proc.StandardOutput()

        ' Display the results.
        Helper.PasteBinText(std_out.ReadToEnd())

        ' Clean up.
        std_out.Close()

        proc.Close()
    End Sub
End Class