Imports System.IO
Imports PSO2_Tweaker.My

Public Class frmPlugins
    Private Sub frmPlugins_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' make a reference to a directory
        Dim di As New IO.DirectoryInfo(Program.Pso2RootDir & "\plugins\")
        Dim diar1 As IO.FileInfo() = di.GetFiles()
        Dim dra As IO.FileInfo

        'list the names of all files in the specified directory
        For Each dra In diar1
            If dra.Name <> "translator.dll" Then ListViewEx1.Items.Add(dra.Name).Checked = True
        Next

        Dim di2 As New IO.DirectoryInfo(Program.Pso2RootDir & "\plugins\disabled\")
        Dim diar2 As IO.FileInfo() = di2.GetFiles()
        Dim dra2 As IO.FileInfo

        'list the names of all files in the specified directory
        For Each dra2 In diar2
            If dra2.Name <> "translator.dll" Then ListViewEx1.Items.Add(dra2.Name).Checked = False
        Next
    End Sub

    Private Sub ListViewEx1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListViewEx1.SelectedIndexChanged
        lblPluginInfo.Text = ""
        If ListViewEx1.FocusedItem.Text = "PSO2TitleTranslator.dll" Then
            lblPluginInfo.Text = "Translates the titles in-game."
        End If
        If ListViewEx1.FocusedItem.Text = "translator.dll" Then
            lblPluginInfo.Text = "Translates the items in-game."
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
                'Do nothing! :D
            Else
                'Disable
                File.Move(Program.Pso2RootDir & "\plugins\" & filename, Path.Combine(Program.Pso2RootDir & "\plugins\disabled\", filename))
            End If
        Next
        MsgBox("Changes saved! Feel free to close the window when done.")
    End Sub
End Class