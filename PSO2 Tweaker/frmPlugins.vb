Imports System.IO
Imports System.Net
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
        FrmMain.WriteDebugInfo("Forcing plugin update...")
        FrmMain.DownloadFile("http://107.170.16.100/Plugins/PluginMD5HashList.txt", "PluginMD5HashList.txt")
        Using oReader As StreamReader = File.OpenText("PluginMD5HashList.txt")
            Dim strNewDate As String = oReader.ReadLine()
            RegKey.SetValue(Of String)(RegKey.NewPluginVersionTemp, strNewDate)
            RegKey.SetValue(Of String)(RegKey.NewPluginVersion, strNewDate)
            'Update plugins [AIDA]

            Dim missingfiles As New List(Of String)
            Dim numberofChecks As Integer = 0
            Dim truefilename As String
            Dim filename As String()
            Dim FinalExportString As String = ""
            'Move all plugins to the disabled folder instead. [AIDA]

            RegKey.SetValue(Of String)(RegKey.PluginsEnabled, FinalExportString)
            For Each fi As FileInfo In New DirectoryInfo(Program.Pso2RootDir & "\plugins\").GetFiles
                File.Move(fi.FullName, Path.Combine(Program.Pso2RootDir & "\plugins\disabled", fi.Name))
                FinalExportString += fi.Name & ","
            Next
            If FinalExportString.Length > 0 Then FinalExportString = FinalExportString.Remove(FinalExportString.Length - 1, 1)
            RegKey.SetValue(Of String)(RegKey.PluginsEnabled, FinalExportString)

            While Not (oReader.EndOfStream)
                filename = oReader.ReadLine().Split(","c)
                truefilename = filename(0)
                missingfiles.Add(truefilename)
                Application.DoEvents()
            End While

            Helper.WriteDebugInfo("Downloading/Installing updates...")
            Dim totaldownload As Long = missingfiles.Count
            Dim downloaded As Long = 0

            For Each downloadStr As String In missingfiles
                'Download the missing files:
                Application.DoEvents()
                FrmMain.DownloadFile(("http://107.170.16.100/Plugins/" & downloadStr), downloadStr)
                'Delete the existing file FIRST
                If Not File.Exists(downloadStr) Then
                    Helper.WriteDebugInfoAndFailed("File " & downloadStr & " does not exist! Perhaps it wasn't downloaded properly?")
                End If
                If downloadStr = "pso2h.dll" Or downloadStr = "translation_titles.bin" Or downloadStr = "translation.bin" Then
                    If Environment.CurrentDirectory <> Program.Pso2RootDir Then
                        Helper.DeleteFile((Program.Pso2RootDir & "\" & downloadStr))
                        File.Move(downloadStr, (Program.Pso2RootDir & "\" & downloadStr))
                    End If
                Else
                    If downloadStr = "PSO2Proxy.dll" Then
                        'If Not Dns.GetHostEntry("gs001.pso2gs.net").AddressList(0).ToString().Contains("210.189.") Then
                        If Not Dns.GetHostEntry("gs001.pso2gs.net").AddressList(0).ToString().Contains("210.189.") Then
                            Helper.WriteDebugInfo("PSO2Proxy usage detected! Auto-enabling PSO2Proxy plugin...")
                            File.Move(downloadStr, (Program.Pso2RootDir & "\plugins\" & downloadStr))
                        Else
                            Helper.DeleteFile((Program.Pso2RootDir & "\plugins\disabled\" & downloadStr))
                            File.Move(downloadStr, (Program.Pso2RootDir & "\plugins\disabled\" & downloadStr))
                        End If
                    Else
                        Helper.DeleteFile((Program.Pso2RootDir & "\plugins\disabled\" & downloadStr))
                        File.Move(downloadStr, (Program.Pso2RootDir & "\plugins\disabled\" & downloadStr))
                    End If
                End If

                If File.Exists(downloadStr) = True And Environment.CurrentDirectory <> Program.Pso2RootDir Then Helper.DeleteFile(downloadStr)
                Application.DoEvents()
            Next
            'Restore the plugins to their proper folders now
            'If there's enabled plugins, do stuff.
            Dim FileToMove As String = ""
            If RegKey.GetValue(Of String)(RegKey.PluginsEnabled).ToString.Length > 1 Then
                'Check to see if it's more than one file by seeing if there are any commas
                If RegKey.GetValue(Of String)(RegKey.PluginsEnabled).Contains(",") = False Then
                    'It's just one file
                    'MsgBox("One plugin enabled - " & RegKey.GetValue(Of String)(RegKey.PluginsEnabled).ToString)
                    FileToMove = RegKey.GetValue(Of String)(RegKey.PluginsEnabled).ToString
                    If File.Exists(Program.Pso2RootDir & "\plugins\" & FileToMove) = True Then Helper.DeleteFile((Program.Pso2RootDir & "\plugins\" & FileToMove))
                    If File.Exists(Program.Pso2RootDir & "\plugins\disabled\" & FileToMove) Then File.Move((Program.Pso2RootDir & "\plugins\disabled\" & FileToMove), (Program.Pso2RootDir & "\plugins\" & FileToMove))
                Else
                    'It's multiple files
                    Dim EnabledPlugins() As String = RegKey.GetValue(Of String)(RegKey.PluginsEnabled).Split(CType(",", Char()))
                    For Each EnabledFilename In EnabledPlugins
                        'MsgBox(EnabledFilename)
                        If File.Exists(Program.Pso2RootDir & "\plugins\" & EnabledFilename) = True Then Helper.DeleteFile((Program.Pso2RootDir & "\plugins\" & EnabledFilename))
                        If File.Exists(Program.Pso2RootDir & "\plugins\disabled\" & EnabledFilename) Then File.Move((Program.Pso2RootDir & "\plugins\disabled\" & EnabledFilename), (Program.Pso2RootDir & "\plugins\" & EnabledFilename))
                    Next
                End If
            Else
                'MsgBox("No plugins enabled!")
            End If

            Helper.WriteDebugInfoAndOk("Plugins updated successfully.")
            RegKey.SetValue(Of String)(RegKey.PluginVersion, RegKey.GetValue(Of String)(RegKey.NewPluginVersionTemp))
            RegKey.SetValue(Of String)(RegKey.NewPluginVersionTemp, "")
        End Using
        If File.Exists("PluginMD5HashList.txt") = True Then Helper.DeleteFile("PluginMD5HashList.txt")
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

    Private Sub lblPluginInfo_Click(sender As Object, e As EventArgs) Handles lblPluginInfo.Click

    End Sub
End Class