Imports System.IO
Imports PSO2_Tweaker.My

Public Class frmPlugins
    Private Sub frmPlugins_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ListViewEx1.Clear()
        ' make a reference to a directory
        Dim di As New IO.DirectoryInfo(Program.Pso2RootDir & "\plugins\")
        Dim diar1 As IO.FileInfo() = di.GetFiles()
        Dim dra As IO.FileInfo

        'list the names of all files in the specified directory
        For Each dra In diar1
            ListViewEx1.Items.Add(dra.Name).Checked = True
        Next

        Dim di2 As New IO.DirectoryInfo(Program.Pso2RootDir & "\plugins\disabled\")
        Dim diar2 As IO.FileInfo() = di2.GetFiles()
        Dim dra2 As IO.FileInfo

        'list the names of all files in the specified directory
        For Each dra2 In diar2
            ListViewEx1.Items.Add(dra2.Name).Checked = False
        Next
    End Sub

    Private Sub ListViewEx1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListViewEx1.SelectedIndexChanged
        lblPluginInfo.Text = ""
        btnConfigure.Visible = False
        If ListViewEx1.FocusedItem.Text = "PSO2TitleTranslator.dll" Then
            lblPluginInfo.Text = "Translates the titles in-game." & vbCrLf & "Author: Variant" & vbCrLf & "Support URL: <not implemented>"
            Exit Sub
        End If
        If ListViewEx1.FocusedItem.Text = "translator.dll" Then
            lblPluginInfo.Text = "Translates the items in-game." & vbCrLf & "Author: arcnmx/Variant/Raven123" & vbCrLf & "Support URL: <not implemented>"
            btnConfigure.Visible = True
            Exit Sub
        End If
        If ListViewEx1.FocusedItem.Text = "PSO2DamageDump.dll" Then
            lblPluginInfo.Text = "DPS (Damage Per Second) Parser plugin. Exports damage logs into a damagelogs folder where pso2.exe is, in excel format." & vbCrLf & "Author: Variant" & vbCrLf & "Support URL: <not implemented>"
            Exit Sub
        End If
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        'Move all plugins to the base folder, then sort according to the list.
        For Each fi As FileInfo In New DirectoryInfo(Program.Pso2RootDir & "\plugins\disabled\").GetFiles
            File.Move(fi.FullName, Path.Combine(Program.Pso2RootDir & "\plugins\", fi.Name))
        Next

        For l_index As Integer = 0 To ListViewEx1.Items.Count - 1
            Dim filename As String = CStr(ListViewEx1.Items(l_index).Text)
            If ListViewEx1.Items(l_index).Checked = True Then
                'Enable
                If filename = "translator.dll" Then
                    If Not File.Exists(Program.Pso2RootDir & "\translation.cfg") Then
                        File.WriteAllText(Program.Pso2RootDir & "\translation.cfg", "TranslationPath:translation.bin")
                    End If
                    Helper.WriteDebugInfo(Resources.strDownloadingLatestItemTranslationFiles)

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

                    Program.UseItemTranslation = True
                    RegKey.SetValue(Of Boolean)(RegKey.UseItemTranslation, Program.UseItemTranslation)
                End If
                'Do nothing! :D
            Else
                'Disable

                If filename = "translator.dll" Then
                    Helper.WriteDebugInfoAndOk(Resources.strDeletingItemCache)
                    Helper.WriteDebugInfoSameLine(Resources.strDone)
                    Dim builtFile As New List(Of String)
                    For Each line In Helper.GetLines(Program.Pso2RootDir & "\translation.cfg")
                        If line.Contains("TranslationPath:") Then line = "TranslationPath:"
                        builtFile.Add(line)
                    Next
                    File.WriteAllLines(Program.Pso2RootDir & "\translation.cfg", builtFile.ToArray())
                    Program.UseItemTranslation = False
                    RegKey.SetValue(Of Boolean)(RegKey.UseItemTranslation, Program.UseItemTranslation)
                End If
                File.Move(Program.Pso2RootDir & "\plugins\" & filename, Path.Combine(Program.Pso2RootDir & "\plugins\disabled\", filename))
            End If
        Next
        MsgBox("Changes saved! Feel free to close the window when done.")
    End Sub

    Private Sub btnConfigure_Click(sender As Object, e As EventArgs) Handles btnConfigure.Click
        If ListViewEx1.FocusedItem.Text = "translator.dll" Then FrmItemConfig.Show()
    End Sub
End Class