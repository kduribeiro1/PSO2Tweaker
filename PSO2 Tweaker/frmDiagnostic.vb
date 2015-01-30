Imports System.IO

Public Class FrmDiagnostic

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim totalString As String = ""

        totalString &= "OS: " & My.Computer.Info.OSFullName & vbCrLf
        totalString &= "64 Bit OS?: " & Environment.Is64BitOperatingSystem.ToString() & vbCrLf
        totalString &= "Tweaker is located at: " & Environment.CurrentDirectory & vbCrLf
        totalString &= ".NET Version: " & Environment.Version.ToString() & vbCrLf
        totalString &= "System has been on for: " & Mid((Environment.TickCount / 3600000).ToString(), 1, 5) & " hours"
        Clipboard.SetText(totalString)
        MsgBox("Copied!")
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim totalString As String = ""
        Dim currentLine As String

        Using xRead As New StreamReader("C:\WINDOWS\system32\drivers\etc\hosts")
            Do Until xRead.EndOfStream
                currentLine = xRead.ReadLine()
                '[AIDA] Changed it, only took a few days! :D.... :(
                If currentLine <> "" Then totalString &= currentLine & vbCrLf
            Loop
        End Using

        If totalString = "" Then totalString = "No modified host entries detected!"
        Clipboard.SetText(totalString)
        MsgBox("Copied!")
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim drInfo As New DirectoryInfo(RegKey.GetValue(Of String)(RegKey.PSO2Dir) & "\data\win32\")
        Dim filesInfo As FileInfo() = drInfo.GetFiles("*.*", SearchOption.AllDirectories)
        Dim fileSize As Long = 0

        For Each fileInfo As FileInfo In filesInfo
            fileSize += fileInfo.Length
        Next

        Dim totalString As String = ""
        totalString &= "PSO2 Directory: " & RegKey.GetValue(Of String)(RegKey.PSO2Dir) & vbCrLf
        totalString &= "Current game version: " & RegKey.GetValue(Of String)(RegKey.PSO2RemoteVersion) & vbCrLf
        totalString &= "Item Translation: " & RegKey.GetValue(Of String)(RegKey.UseItemTranslation) & vbCrLf
        totalString &= "EN Patch version installed: " & RegKey.GetValue(Of String)(RegKey.ENPatchVersion) & vbCrLf
        totalString &= "Large Files version installed: " & RegKey.GetValue(Of String)(RegKey.LargeFilesVersion) & vbCrLf
        totalString &= "Story Patch version installed: " & RegKey.GetValue(Of String)(RegKey.StoryPatchVersion) & vbCrLf
        totalString &= "Size of PSO2 data/win32 folder: ~" & fileSize.ToString().Remove(2, 9) & "GB"
        Clipboard.SetText(totalString)
        MsgBox("Copied!")
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim drInfo As New DirectoryInfo(RegKey.GetValue(Of String)(RegKey.PSO2Dir))
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