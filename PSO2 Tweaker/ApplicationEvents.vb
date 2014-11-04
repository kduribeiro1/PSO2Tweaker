Namespace My

    ' The following events are available for MyApplication:
    ' 
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication
        Private Sub MyApplication_UnhandledException(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.UnhandledExceptionEventArgs) Handles Me.UnhandledException
            If Convert.ToBoolean(RegKey.GetValue(Of String)(RegKey.Pastebin)) Then
                frmMain.Log(e.ToString())
                frmMain.PasteBinUpload()
            End If
        End Sub

        Public Function PasteBinUpload(ByVal code As String) As String
            System.Net.ServicePointManager.Expect100Continue = False
            Dim pr As Integer = 0
            Dim fi As String = "?paste_private=" & 0 & "&paste_format=text" & "&api_dev_key=ddc1e2efaca45d3df87e6b93ceb43c9f" & "&paste_code=" & "test"
            Dim w As New System.Net.WebClient()
            w.Headers.Add("Content-Type", "application/x-www-form-urlencoded")
            Dim pd As Byte() = System.Text.Encoding.ASCII.GetBytes(fi)
            Dim rd As Byte() = w.UploadData("http://pastebin.com/api_public.php", "POST", pd)
            Dim r As String = System.Text.Encoding.ASCII.GetString(rd)
            Return r
        End Function
    End Class
End Namespace

