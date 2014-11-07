Imports System.IO

Public Class frmItemConfig
    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim pso2launchpath = frmMain.lblDirectory.Text.Replace("\data\win32", "")
        File.Delete(pso2launchpath & "\translation.cfg")

        Dim SplitKey As String() = DirectCast(cmbToggleKey.SelectedItem, String).Split("("c)
        Dim data As String()
        Dim delay As Integer = Convert.ToInt32(NUDDelay.Value * 1000)
        Dim NumberKey = SplitKey(1).Replace(")"c, "")
        lblToggle.Text = "Toggle item patch ON/OFF: Control + " & SplitKey(0).Replace("   (", "")

        If chkLogging.Checked Then
            data = {"Delay:" & delay, "TranslationPath:translation.bin", "TranslationCachePath:", "LogPath:itemlog.txt", "LogLines:0", "KeyToggle:17", "KeyToggleCancel:16", "KeyDisable:" & NumberKey, "KeyDisableTree:114", "KeyDisableToggle:113"}
        Else
            data = {"Delay:" & delay, "TranslationPath:translation.bin", "TranslationCachePath:", "LogPath:", "LogLines:500", "KeyToggle:17", "KeyToggleCancel:16", "KeyDisable:" & NumberKey, "KeyDisableTree:114", "KeyDisableToggle:113"}
        End If

        File.WriteAllLines(pso2launchpath & "\translation.cfg", data)
        Me.Hide()
    End Sub

    Private Sub chkLogging_CheckedChanged(sender As Object, e As EventArgs) Handles chkLogging.CheckedChanged
        File.Delete(frmMain.lblDirectory.Text.Replace("\data\win32", "") & "\itemlog.txt")
        MsgBox("Please only turn this feature on if you are specifically asked to. If AIDA didn't tell you to turn it on, don't.")
    End Sub

    Private Sub ButtonX7_Click(sender As Object, e As EventArgs) Handles ButtonX7.Click
        cmbToggleKey.SelectedIndex = 0
    End Sub

    Private Sub cmbToggleKey_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbToggleKey.SelectedIndexChanged
        lblToggle.Text = "Toggle item patch ON/OFF: Control + " & (DirectCast(cmbToggleKey.SelectedItem, String).Split("("c)(0)).Replace("   (", "")
    End Sub
End Class