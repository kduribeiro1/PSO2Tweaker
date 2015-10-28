Public Class Baka
    Private Sub lblFool_Click(sender As Object, e As EventArgs) Handles lblFool.Click

    End Sub

    Private Sub Baka_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PictureBox1.Location = New Point(66, 1)
        Dim rn As New Random
        Dim ImageLink As String = ""
        Dim ImageNumber As Integer = rn.Next(0, 9)       'Tells the program to put the random thing inside of the textbox
        Select Case ImageNumber 'Starts the case block
            Case 0 '1st case define
                ImageLink = "http://i.imgur.com/eOFxwm2.gif"
            Case 1 '2nd case define
                ImageLink = "http://i.imgur.com/FfD9SLN.gif"
            Case 2
                ImageLink = "http://i.imgur.com/QFre3BR.gif"
            Case 3
                PictureBox1.Location = New Point(195, -2)
                ImageLink = "http://i.imgur.com/BgNbPP2.gif"
            Case 4
                ImageLink = "http://i.imgur.com/bOc8Ehd.gif"
            Case 5
                ImageLink = "http://i.imgur.com/4UQYYGk.gif"
            Case 6
                ImageLink = "http://i.imgur.com/RBNhKxq.gif"
            Case 7
                ImageLink = "http://i.imgur.com/4j1Jwgr.gif"
            Case 8
                ImageLink = "http://i.imgur.com/4EtldLS.gif"
            Case 9
                ImageLink = "http://i.imgur.com/Vt93JtZ.gif"
        End Select 'End the case block
        PictureBox1.Load(ImageLink)
    End Sub

    Private Sub btnOkay_Click(sender As Object, e As EventArgs) Handles btnOkay.Click
        Me.Hide()
    End Sub
End Class