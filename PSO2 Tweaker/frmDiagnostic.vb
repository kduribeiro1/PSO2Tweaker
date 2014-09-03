Imports System.IO

Public Class frmDiagnostic

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim TotalString As String = ""

        TotalString += "OS: " & My.Computer.Info.OSFullName & vbCrLf
        TotalString += "64 Bit OS?: " & Environment.Is64BitOperatingSystem.ToString & vbCrLf
        TotalString += "Tweaker is located at: " & Environment.CurrentDirectory & vbCrLf
        TotalString += ".NET Version: " & Environment.Version.ToString & vbCrLf
        TotalString += "System has been on for: " & Mid((Environment.TickCount / 3600000), 1, 5) & " hours"
        Clipboard.SetText(TotalString)
        MsgBox("Copied!")
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim TotalString As String = ""
        Dim CurrentLine As String = ""
        Using xRead As New System.IO.StreamReader("C:\WINDOWS\system32\drivers\etc\hosts")
            Dim buffer As New List(Of String)
            Do Until xRead.EndOfStream
                CurrentLine = xRead.ReadLine
                '[AIDA] Gotta change this, it'll make lines that have comments at the end be ignored as well.
                If CurrentLine.Contains("#") = False And CurrentLine <> "" Then TotalString += CurrentLine & vbCrLf
            Loop
        End Using
        'Dim TotalString As String = IO.File.ReadAllText("C:\WINDOWS\system32\drivers\etc\hosts")
        If TotalString = "" Then TotalString = "No modified host entries detected!"
        Clipboard.SetText(TotalString)
        MsgBox("Copied!")
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim drInfo As New DirectoryInfo(frmMain.LoadSetting("PSO2Dir") & "\data\win32\")
        Dim filesInfo() As FileInfo = drInfo.GetFiles("*.*", SearchOption.AllDirectories)

        Dim fileSize As Long = 0

        For Each fileInfo As FileInfo In filesInfo
            fileSize += fileInfo.Length
        Next

        Dim totalString As String = ""
        totalString += "PSO2 Directory: " & frmMain.LoadSetting("PSO2Dir") & vbCrLf
        totalString += "Current game version: " & frmMain.LoadSetting("PSO2RemoteVersion") & vbCrLf
        totalString += "Item Translation: " & frmMain.LoadSetting("UseItemTranslation") & vbCrLf
        totalString += "EN Patch version installed: " & frmMain.LoadSetting("ENPatchVersion") & vbCrLf
        totalString += "Large Files version installed: " & frmMain.LoadSetting("LargeFilesVersion") & vbCrLf
        totalString += "Story Patch version installed: " & frmMain.LoadSetting("StoryPatchVersion") & vbCrLf
        totalString += "Size of PSO2 data/win32 folder: ~" & fileSize.ToString.Remove(2, 9) & "GB"
        Clipboard.SetText(totalString)
        MsgBox("Copied!")
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim drInfo As New DirectoryInfo(frmMain.LoadSetting("PSO2Dir"))
        Dim filesInfo() As FileInfo = drInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly)

        Dim fileSize As Long = 0
        Dim filename As String = ""
        Dim TotalString As String = "Listing of pso2_bin files: "
        For Each fileInfo As FileInfo In filesInfo
            fileName = fileInfo.Name
            fileSize = fileInfo.Length
            TotalString += filename
            If filename = "GameGuard.des" Or filename = "pso2.exe" Or filename = "publickey.blob" Or filename = "rsainject.dll" Or filename = "translation.bin" Or filename = "translator.dll" Then TotalString += ": " & fileSize.ToString
            TotalString += " | "
        Next
        Clipboard.SetText(TotalString)
        MsgBox("Copied!")
    End Sub
End Class