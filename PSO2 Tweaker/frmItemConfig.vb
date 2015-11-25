Imports System.IO
Imports PSO2_Tweaker.My

Public Class FrmItemConfig
    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        If cmbToggleKey.Text = "Please choose a key" Then
            MsgBox("Please choose a key to toggle the item patch!")
            Exit Sub
        End If
        Dim pso2Launchpath = Program.Pso2RootDir
        If File.Exists(pso2Launchpath & "\translation.cfg") Then File.Delete(pso2Launchpath & "\translation.cfg")


        Dim splitKey As String() = DirectCast(cmbToggleKey.SelectedItem, String).Split("("c)
        Dim data As String()
        Dim delay As Integer = Convert.ToInt32(NUDDelay.Value * 1000)
        Dim numberKey = splitKey(1).Replace(")"c, "")
        lblToggle.Text = "Toggle item patch ON/OFF: Control + " & splitKey(0).Replace("   (", "")

        If chkLogging.Checked Then
            data = {"Delay:" & delay, "TranslationPath:translation.bin", "TranslationCachePath:", "LogPath:itemlog.txt", "LogLines:0", "KeyToggle:17", "KeyToggleCancel:16", "KeyDisable:" & numberKey, "KeyDisableTree:114", "KeyDisableToggle:113"}
        Else
            data = {"Delay:" & delay, "TranslationPath:translation.bin", "TranslationCachePath:", "LogPath:", "LogLines:500", "KeyToggle:17", "KeyToggleCancel:16", "KeyDisable:" & numberKey, "KeyDisableTree:114", "KeyDisableToggle:113"}
        End If

        File.WriteAllLines(pso2Launchpath & "\translation.cfg", data)
        Hide()
    End Sub

    Private Shared Sub chkLogging_CheckedChanged(sender As Object, e As EventArgs) Handles chkLogging.CheckedChanged
        File.Delete(Program.MainForm.lblDirectory.Text.Replace("\data\win32", "") & "\itemlog.txt")
        MsgBox("Please only turn this feature on if you are specifically asked to. If AIDA didn't tell you to turn it on, don't.")
    End Sub

    Private Sub ButtonX7_Click(sender As Object, e As EventArgs) Handles ButtonX7.Click
        cmbToggleKey.SelectedIndex = 0
    End Sub

    Private Sub cmbToggleKey_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbToggleKey.SelectedIndexChanged
        lblToggle.Text = "Toggle item patch ON/OFF: Control + " & (DirectCast(cmbToggleKey.SelectedItem, String).Split("("c)(0)).Replace("   (", "")
    End Sub
End Class