Imports System.IO
Imports PSO2_Tweaker.My

Public Class frmPlugins
    Private Sub frmPlugins_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ListViewEx1.Clear()
        ' make a reference to a directory
        Dim di As New IO.DirectoryInfo(Program.Pso2RootDir & "\plugins\")
        Dim diar1 As IO.FileInfo() = di.GetFiles()
        Dim dra As IO.FileInfo
        Dim DisplayName As String = ""
        'list the names of all files in the specified directory
        For Each dra In diar1
            DisplayName = dra.Name.Replace("translator.dll", "Item Translation").Replace("PSO2TitleTranslator.dll", "Title Translation").Replace("PSO2DamageDump.dll", "Damage Parser").Replace("PSO2Proxy.dll", "PSO2Proxy Plugin")
            ListViewEx1.Items.Add(DisplayName).Checked = True
        Next

        Dim di2 As New IO.DirectoryInfo(Program.Pso2RootDir & " \plugins\disabled\")
        Dim diar2 As IO.FileInfo() = di2.GetFiles()
        Dim dra2 As IO.FileInfo

        'list the names of all files in the specified directory
        For Each dra2 In diar2
            DisplayName = dra2.Name.Replace("translator.dll", "Item Translation").Replace("PSO2TitleTranslator.dll", "Title Translation").Replace("PSO2DamageDump.dll", "Damage Parser").Replace("PSO2Proxy.dll", "PSO2Proxy Plugin")
            ListViewEx1.Items.Add(DisplayName).Checked = False
        Next


    End Sub

    Private Sub ListViewEx1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListViewEx1.SelectedIndexChanged
        lblPluginInfo.Text = ""
        btnConfigure.Visible = False
        If ListViewEx1.FocusedItem.Text = "Title Translation" Then
            lblPluginInfo.Text =
                "Translates the titles in-game." & vbCrLf &
                "Author: Variant" & vbCrLf &
                "DLL Name: PSO2TitleTranslator.dll" & vbCrLf &
                "Support URL: N/A"
            Exit Sub
        End If
        If ListViewEx1.FocusedItem.Text = "Item Translation" Then
            lblPluginInfo.Text =
                "Translates the items in-game." & vbCrLf &
                "Author: arcnmx/Variant/Raven123" & vbCrLf &
                "DLL Name: translator.dll" & vbCrLf &
                "Support URL: N/A"
            btnConfigure.Visible = True
            Exit Sub
        End If
        If ListViewEx1.FocusedItem.Text = "Damage Parser" Then
            lblPluginInfo.Text =
                "DPS (Damage Per Second) Parser plugin. Exports damage logs into a damagelogs folder where pso2.exe is, in excel format." & vbCrLf &
                "Author: Variant" & vbCrLf &
                "DLL Name: PSO2DamageDump.dll" & vbCrLf &
                "Support URL: N/A"
            Exit Sub
        End If
        If ListViewEx1.FocusedItem.Text = "PSO2Proxy Plugin" Then
            lblPluginInfo.Text =
                "PSO2Proxy plugin. Allows people from SEA and other blocked regions to connect to PSO2JP." & vbCrLf &
                "Author: Variant/Cyberkitsune" & vbCrLf &
                "DLL Name: PSO2Proxy.dll" & vbCrLf &
                "Support URL: http://pso2proxy.cyberkitsune.net"
            Exit Sub
        End If
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        'Move all plugins to the base folder, then sort according to the list.
        Dim FinalExportString As String = ""
        For Each fi As FileInfo In New DirectoryInfo(Program.Pso2RootDir & "\plugins\disabled\").GetFiles
            File.Move(fi.FullName, Path.Combine(Program.Pso2RootDir & "\plugins\", fi.Name))
        Next

        For l_index As Integer = 0 To ListViewEx1.Items.Count - 1
            Dim filename As String = CStr(ListViewEx1.Items(l_index).Text).Replace("Item Translation", "translator.dll").Replace("Title Translation", "PSO2TitleTranslator.dll").Replace("Damage Parser", "PSO2DamageDump.dll").Replace("PSO2Proxy Plugin", "PSO2Proxy.dll")
            If ListViewEx1.Items(l_index).Checked = True Then
                'Enable
                If filename = "translator.dll" Then
                    If Not File.Exists(Program.Pso2RootDir & "\translation.cfg") Then
                        File.WriteAllText(Program.Pso2RootDir & "\translation.cfg", "TranslationPath:translation.bin")
                    End If

                    'Start the shitstorm
                    Dim builtFile As New List(Of String)
                    For Each line In Helper.GetLines(Program.Pso2RootDir & "\translation.cfg")
                        If line.Contains("TranslationPath:") Then line = "TranslationPath:translation.bin"
                        builtFile.Add(line)
                    Next
                    File.WriteAllLines(Program.Pso2RootDir & "\translation.cfg", builtFile.ToArray())

                    Program.UseItemTranslation = True
                    RegKey.SetValue(Of Boolean)(RegKey.UseItemTranslation, Program.UseItemTranslation)
                End If
                'Do nothing! :D
            Else
                'Disable

                If filename = "translator.dll" Then
                    'Helper.WriteDebugInfoAndOk(Resources.strDeletingItemCache)
                    'Helper.WriteDebugInfoSameLine(Resources.strDone)
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
        For Each fi As FileInfo In New DirectoryInfo(Program.Pso2RootDir & "\plugins\").GetFiles
            FinalExportString += fi.Name & ","
        Next
        If FinalExportString.Length > 0 Then FinalExportString = FinalExportString.Remove(FinalExportString.Length - 1, 1)
        RegKey.SetValue(Of String)(RegKey.PluginsEnabled, FinalExportString)
        FrmMain.WriteDebugInfoAndOk("Plugin changes saved!")
        Me.Close()
    End Sub

    Private Sub btnConfigure_Click(sender As Object, e As EventArgs) Handles btnConfigure.Click
        If ListViewEx1.FocusedItem.Text = "Item Translation" Then FrmItemConfig.Show()
    End Sub

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click
        btnSave.Enabled = False
        ButtonX1.Enabled = False
        btnConfigure.Enabled = False
        FrmMain.WriteDebugInfo("Checking for plugin updates...")
        FrmMain.CheckForPluginUpdates()
        btnSave.Enabled = True
        ButtonX1.Enabled = True
        btnConfigure.Enabled = True
    End Sub

    Private Sub ListViewEx1_Click(sender As Object, e As EventArgs) Handles ListViewEx1.Click
        lblPluginInfo.Text = ""
        btnConfigure.Visible = False
        If ListViewEx1.FocusedItem.Text = "Title Translation" Then
            lblPluginInfo.Text =
                "Translates the titles in-game." & vbCrLf &
                "Author: Variant" & vbCrLf &
                "DLL Name: PSO2TitleTranslator.dll" & vbCrLf &
                "Support URL: N/A"
            Exit Sub
        End If
        If ListViewEx1.FocusedItem.Text = "Item Translation" Then
            lblPluginInfo.Text =
                "Translates the items in-game." & vbCrLf &
                "Author: arcnmx/Variant/Raven123" & vbCrLf &
                "DLL Name: translator.dll" & vbCrLf &
                "Support URL: N/A"
            btnConfigure.Visible = True
            Exit Sub
        End If
        If ListViewEx1.FocusedItem.Text = "Damage Parser" Then
            lblPluginInfo.Text =
                "DPS (Damage Per Second) Parser plugin. Exports damage logs into a damagelogs folder where pso2.exe is, in excel format." & vbCrLf &
                "Author: Variant" & vbCrLf &
                "DLL Name: PSO2DamageDump.dll" & vbCrLf &
                "Support URL: N/A"
            Exit Sub
        End If
        If ListViewEx1.FocusedItem.Text = "PSO2Proxy Plugin" Then
            lblPluginInfo.Text =
                "PSO2Proxy plugin. Allows people from SEA and other blocked regions to connect to PSO2JP." & vbCrLf &
                "Author: Variant/Cyberkitsune" & vbCrLf &
                "DLL Name: PSO2Proxy.dll" & vbCrLf &
                "Support URL: http://pso2proxy.cyberkitsune.net"
            Exit Sub
        End If
    End Sub
End Class