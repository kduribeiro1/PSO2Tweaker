Imports System.IO

Public Class FrmDiagnostic

    Private Shared Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim totalString As String = ""

        totalString &= "OS: " & My.Computer.Info.OSFullName & vbCrLf
        totalString &= "64 Bit OS?: " & Environment.Is64BitOperatingSystem.ToString() & vbCrLf
        totalString &= "Tweaker is located at: " & Environment.CurrentDirectory & vbCrLf
        totalString &= ".NET Version: " & Environment.Version.ToString() & vbCrLf
        totalString &= "System has been on for: " & Mid((Environment.TickCount / 3600000).ToString(), 1, 5) & " hours"
        Clipboard.SetText(totalString)
        MsgBox("Copied!")
    End Sub

    Private Shared Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim totalString As String = ""
        For Each line As String In Helper.GetLines(Environment.SystemDirectory & "\drivers\etc\hosts")
            If line <> "" Then totalString &= line & vbCrLf
        Next
        If totalString = "" Then totalString = "No modified host entries detected!"
        Clipboard.SetText(totalString)
        MsgBox("Copied!")
    End Sub

    Private Shared Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim drInfo As New DirectoryInfo(My.Program.Pso2WinDir)
        Dim filesInfo As FileInfo() = drInfo.GetFiles("*.*", SearchOption.AllDirectories)
        Dim fileSize As Long = filesInfo.Sum(Function(fileInfo) fileInfo.Length)

        Dim totalString As String = ""
        totalString &= "PSO2 Directory: " & My.Program.Pso2RootDir & vbCrLf
        totalString &= "Current game version: " & RegKey.GetValue(Of String)(RegKey.Pso2RemoteVersion) & vbCrLf
        totalString &= "Item Translation: " & RegKey.GetValue(Of String)(RegKey.UseItemTranslation) & vbCrLf
        totalString &= "EN Patch version installed: " & RegKey.GetValue(Of String)(RegKey.EnPatchVersion) & vbCrLf
        totalString &= "Large Files version installed: " & RegKey.GetValue(Of String)(RegKey.LargeFilesVersion) & vbCrLf
        totalString &= "Story Patch version installed: " & RegKey.GetValue(Of String)(RegKey.StoryPatchVersion) & vbCrLf
        totalString &= "Size of PSO2 data/win32 folder: ~" & fileSize.ToString().Remove(2, 9) & "GB"
        Clipboard.SetText(totalString)
        MsgBox("Copied!")
    End Sub

    Private Shared Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim drInfo As New DirectoryInfo(My.Program.Pso2RootDir)
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

        Clipboard.SetText(totalString)
        MsgBox("Copied!")
    End Sub
End Class