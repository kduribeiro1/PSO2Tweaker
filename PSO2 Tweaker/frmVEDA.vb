Imports System.IO

Public Class FrmVeda
    Private Sub WriteDebugInfo(ByVal addThisText As String)
        rtbStatus.Text &= (vbCrLf & addThisText)
        Log(addThisText)
    End Sub

    '    Private Sub WriteDebugInfoSameLine(ByVal AddThisText As String)
    '        rtbStatus.Text &= (" " & AddThisText)
    '        Log(AddThisText)
    '    End Sub

    Private Sub WriteDebugInfoAndOk(ByVal addThisText As String)
        rtbStatus.Text &= (vbCrLf & addThisText)
        rtbStatus.Select(rtbStatus.TextLength, 0)
        rtbStatus.SelectionColor = Color.Green
        rtbStatus.AppendText(" [OK!]")
        rtbStatus.SelectionColor = rtbStatus.ForeColor
        Log((addThisText & " [OK!]"))
    End Sub

    Private Sub WriteDebugInfoAndWarning(ByVal addThisText As String)
        rtbStatus.Text &= (vbCrLf & addThisText)
        rtbStatus.Select(rtbStatus.TextLength, 0)
        rtbStatus.SelectionColor = Color.Red
        rtbStatus.AppendText(" [WARNING!]")
        rtbStatus.SelectionColor = rtbStatus.ForeColor
        Log((addThisText & " [WARNING!]"))
    End Sub

    '    Private Sub WriteDebugInfoAndFAILED(ByVal AddThisText As String)
    '        rtbStatus.Text &= (vbCrLf & AddThisText)
    '        rtbStatus.Select(rtbStatus.TextLength, 0)
    '        rtbStatus.SelectionColor = Color.Red
    '        rtbStatus.AppendText(" [FAILED!]")
    '        rtbStatus.SelectionColor = rtbStatus.ForeColor
    '        Log((AddThisText & " [FAILED!]"))
    '    End Sub

    '    Private Sub ClearDebugInfo()
    '        rtbStatus.Text = ""
    '    End Sub

    Private Sub rtbStatus_LinkClicked(sender As Object, e As LinkClickedEventArgs) Handles rtbStatus.LinkClicked
        Process.Start(e.LinkText)
    End Sub

    Private Sub rtbStatus_TextChanged(sender As Object, e As EventArgs) Handles rtbStatus.TextChanged
        rtbStatus.SelectionStart = rtbStatus.Text.Length
    End Sub

    Private Sub Log(output As String)
        Dim timeFormatted As String
        Dim time As DateTime = DateTime.Now
        timeFormatted = time.ToString("G")
        File.AppendAllText((Application.StartupPath & "\logfile.txt"), timeFormatted & ": " & output & vbCrLf)
    End Sub

    ' TODOL Might need to redo this
    Private Function GetRandom(ByVal min As Integer, ByVal max As Integer) As Integer
        ' by making Generator static, we preserve the same instance '
        ' (i.e., do not create new instances with the same seed over and over) '
        ' between calls '
        Static generator As Random = New Random()
        Return generator.Next(min, max)
    End Function

    '    Public Sub WriteConsole(str As String)
    '        WriteDebugInfo(str)
    '        Application.DoEvents()
    '        Threading.Thread.Sleep(GetRandom(30, 1000))
    '    End Sub

    Private Sub WriteConsoleAndOk(str As String)
        WriteDebugInfoAndOk(str)
        Application.DoEvents()
        Threading.Thread.Sleep(GetRandom(30, 1000))
    End Sub

    '    Public Sub WriteConsoleAndWarning(str As String)
    '        WriteDebugInfoAndWarning(str)
    '        Application.DoEvents()
    '        Threading.Thread.Sleep(GetRandom(30, 1000))
    '    End Sub

    Private Sub frmVEDA_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        rtbStatus.Height = 408
        rtbStatus.Top = 0
        rtbStatus.Width = 394
        Log("WARNING: VEDA SYSTEM ACTIVATED")
        Application.DoEvents()
        WriteConsoleAndOK("Connecting to Veda System...")
        Threading.Thread.Sleep(2000)
        WriteConsoleAndOK("ID and password confirmed...")
        WriteConsoleAndOK("Quantum computing enabled...")
        WriteConsoleAndOK("Checking memory...")
        WriteConsoleAndOK("")
        WriteConsoleAndOK("Recieving system data... 5TB/s...")
        WriteConsoleAndOK("Complete. Connection established.")
        WriteDebugInfoAndWarning("INVALID ACCESS")
        Application.DoEvents()
        Threading.Thread.Sleep(300)
        WriteDebugInfoAndWarning("INVALID ACCESS")
        Application.DoEvents()
        Threading.Thread.Sleep(300)
        WriteDebugInfoAndWarning("INVALID ACCESS")
        Application.DoEvents()
        Threading.Thread.Sleep(300)
        WriteDebugInfoAndWarning("INVALID ACCESS")
        Application.DoEvents()
        Threading.Thread.Sleep(300)
        WriteDebugInfoAndWarning("INVALID ACCESS")
        WriteDebugInfoAndOK("VEDA System Online... Awaiting commands...")
        rtbStatus.Left = 0
        rtbStatus.Top = 228
        rtbStatus.Height = 172
        rtbStatus.SelectionStart = rtbStatus.Text.Length
        btnMD5.Visible = True
        btnClose.Visible = True
        btnShorten.Visible = True
        btnMerge.Visible = True
        btnListDir.Visible = True
        btnAnyDir.Visible = True
        Button1.Visible = True
    End Sub

    Private Sub btnMD5_Click(sender As Object, e As EventArgs) Handles btnMD5.Click
        OpenFileDialog1.Title = "Please select the file you wish to MD5"
        OpenFileDialog1.FileName = "File"
        OpenFileDialog1.ShowDialog()
        WriteDebugInfo("The MD5 of that file is: " & Helper.GetMD5(OpenFileDialog1.FileName))
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        frmMain.Show()
        Close()
    End Sub

    Private Sub btnShorten_Click(sender As Object, e As EventArgs) Handles btnShorten.Click
        WriteDebugInfo("Rewriting patchlist.txt...")

        File.WriteAllText((Application.StartupPath & "\patchlist.txt"), "data/win32/00049841d10e1e00b22f84204f8b092e.pat	48780	63B8467E37089FB14E55A3C53AD6F0B3" & vbCrLf & "data/win32/000db55c3244a18c67ef894d67932515.pat	12292	63CD2645B8AA3759E93511F50DA2BB92" & vbCrLf & "data/win32/0012b70a7c3b65aa27dc4fa0abe34667.pat	9232	02999793F806FB16F16B9C6812C832C4" & vbCrLf & "data/win32/0029ba00649a7aaedbe495806dc8d216.pat	480	A7DCD5A4837096BCEA1B87FC0DF74D3C" & vbCrLf & "data/win32/00374255c7e19de18a1b07fcb5beb7c4.pat	276244	E7119EFC95C78C1C7DD4CFF8A3FFFC9D" & vbCrLf & "data/win32/00800bcb4e790060f5a47b85fbc2acd0.pat	1733412	2EF237FBEFD59623135E49D194CC152C" & vbCrLf & "data/win32/00910c1384a1c2621c2a4925d78fbd72.pat	278504	548AF4093D1058B3096B4F093A505B3E" & vbCrLf & "data/win32/0096b888cd67ee06518e65ac39114421.pat	1632092	A85B4D9AA065356D241EEF15B118C9B9" & vbCrLf & "data/win32/00a66f5118f5faf240e834877066af82.pat	10660	51CEC116FA14E882B78D179906A41C4B" & vbCrLf & "data/win32/00c0fb6c5e4568fbb5c23b24b2800249.pat	428620	93886CFEDA40C0A378A0FF6674C789BA" & vbCrLf & "data/win32/00c7d4f1c307b42b53581e516d4b4ee0.pat	219132	7D961CE8F661999B742C7E07DBEF7700" & vbCrLf & "data/win32/00d8465dfd7f4fb14eba856507ecf5de.pat	1463188	136A02C2443CF8A2A746B005E5D9C5F5" & vbCrLf & "data/win32/00fbd2ef554bf50f252cce091146cf5e.pat	3184	F30714F08AB1D947738910A66584AAB2")
        WriteDebugInfo("Rewriting patchlist_old.txt...")

        File.WriteAllText((Application.StartupPath & "\patchlist_old.txt"), "data/win32/0002c97e93075ec680d89801fa640912.pat	1276	314A14D1A7DF81A7E53EFFA755AF8D8B" & vbCrLf & "data/win32/000db55c3244a18c67ef894d67932515.pat	12936	ED04C3D6B1964E606C5206AE757F6BC2" & vbCrLf & "data/win32/001aa587c5b0a17902c4ca3c78a3939b.pat	90552	6204C5838389A44DA2D770224B47B8BB" & vbCrLf & "data/win32/001bda4b4d7fa8effc3fca479165fd35.pat	128044	B74D735F096526A3270885FF763CB594" & vbCrLf & "data/win32/00264953afee9be4dac1d072395286bc.pat	41320	26F3C949F8C20670F69CBC6E1A395226" & vbCrLf & "data/win32/0029485b9da95a64a4b6e9258a8f3a6d.pat	1628	754ACF2FACB26D16665267CF1F9E403B" & vbCrLf & "data/win32/0029ba00649a7aaedbe495806dc8d216.pat	488	E59BD464A27385BCA00B6BF70984C760" & vbCrLf & "data/win32/00374255c7e19de18a1b07fcb5beb7c4.pat	275824	9FC1B08852BA5932721AEC7EAA1855D2" & vbCrLf & "data/win32/004da68136936fbb9e50719e2e215053.pat	109680	375649583CE966F1B43F25A0426A438F" & vbCrLf & "data/win32/004f934127d3839086e00bb96be01e4d.pat	601780	FCB7EB1C7B1576DF27128302840AF08E" & vbCrLf & "data/win32/005d8b6167ee6d839c4504837d212570.pat	171508	8DD5331BA0DDAA805F0F4BE6D1505B43" & vbCrLf & "data/win32/00647943b0b4d062e0c5662fd0bcea13.pat	297948	D12A2E1D36B07EFB2FCE20E921C49122" & vbCrLf & "data/win32/00712f9c99865041ae0c20b7b7bd3fa5.pat	1480	DA0EFD7F8C0251DC92E676B55C993596" & vbCrLf & "data/win32/0074237bc3482a5b1b306c67d6f7bc32.pat	4252	41751851C10E97941DC0ACB7B10AE155" & vbCrLf & "data/win32/007a9d14c9b2e4fcd489be7ca81f2066.pat	268124	B6C41C5FBEED44AED6698EBDF690552C" & vbCrLf & "data/win32/007d0c0d5629692153c6cad0e1719995.pat	256284	2F35F711E9B4AC5DCCA022739AAFB294" & vbCrLf & "data/win32/00800bcb4e790060f5a47b85fbc2acd0.pat	1613588	98EB53D6A59A16991DD861CF9C649100" & vbCrLf & "data/win32/008192cd12dccf604605fa608405af31.pat	2064020	D1264FC54F0D01EC717FEF064F75E720" & vbCrLf & "data/win32/0083fc758934f5fb297772781f309d6e.pat	140460	F80ECEDB485BA17582C4A493B0E76127" & vbCrLf & "data/win32/00846c59d76dbe933df6facdce4a13f9.pat	1482156	1453A44FBC8FEDB4FFF845028758DDE2" & vbCrLf & "data/win32/008af38bc0996982365319184b4ca3f4.pat	4216	C8AEE56BBD04D5F43537B6B23FB1FFE2" & vbCrLf & "data/win32/008c2ba04196d996f7e23537bdac4815.pat	12376	54EF9BC5F19475DEC2E3F548351EF0BD" & vbCrLf & "data/win32/00910c1384a1c2621c2a4925d78fbd72.pat	278328	08FE204EFDF2555C46F437B9395BD065" & vbCrLf & "data/win32/0093525546395ac4eff17b0d6fc8404e.pat	336	6C77ED8944B11B60BA76CB9F30BD5B2D" & vbCrLf & "data/win32/0094402b6c7d5821c81b46b327a48d68.pat	121984	7E42C3E70B42C0D78350668AB7657403" & vbCrLf & "data/win32/0096b888cd67ee06518e65ac39114421.pat	4314252	EC1656322C35362A3EBEE577AD127CC0" & vbCrLf & "data/win32/009bfec69b04a34576012d50e3417771.pat	2220928	3B5BDDB1F2B48AE4BFA951AC3EC2E042" & vbCrLf & "data/win32/00a05bbb5b6604e3f3573a8f95edf9e1.pat	1316152	0345FEA4DAE62EFC6ABBC1D26374EAB2" & vbCrLf & "data/win32/00a26ee409b6060375b344ef72e80916.pat	15976	101CDCB962C50A0FAC952112A62E36CA" & vbCrLf & "data/win32/00a39405f69f99eb9081e22502f615a4.pat	325420	BFDC533566935BA077FAE81C3C43D697" & vbCrLf & "data/win32/00a59e51b6d0b4a35544a21b43dc22c5.pat	1049132	32BB30022EAE4B1FE2760CB53CECE2B4" & vbCrLf & "data/win32/00ab5dc84f53cd40ea127de53aaaa80f.pat	1279712	1982809C9BE303B2D4DAAB41A9D9169C" & vbCrLf & "data/win32/00b6a7c277910f2f5962e8e27bae2dd6.pat	4192	ACB33D3A1039C1050177F31D45F8C568" & vbCrLf & "data/win32/00c0fb6c5e4568fbb5c23b24b2800249.pat	428336	7F7056BC286708A5456947446F4A1DA9" & vbCrLf & "data/win32/00c1e2892a66dd7d5ab79782a1aa439c.pat	1580	F6878257EBA7A3C4D877BF94E6B9D705" & vbCrLf & "data/win32/00c22840402e80257d64d649253870ce.pat	4032	FE92CD37E4776D703D61E2D3466C5F71" & vbCrLf & "data/win32/00c7d4f1c307b42b53581e516d4b4ee0.pat	218848	CDBF939C8057D779047A692A0CAEAE18" & vbCrLf & "data/win32/00ca5eefeef0bfbd3acefe5452842494.pat	1903468	402936EC6762E0B63D6AF03F5F679302" & vbCrLf & "data/win32/00d3e278342c1c936c3c9dc639c63d7e.pat	4040	7E14867245431A4BDEECEF018BD05A6E" & vbCrLf & "data/win32/00d8465dfd7f4fb14eba856507ecf5de.pat	1463188	136A02C2443CF8A2A746B005E5D9C5F5" & vbCrLf & "data/win32/00d90977b8d75076d873b6f0cc146105.pat	4200	C9EFFB97C889E8480C6F2B635A757A65" & vbCrLf & "data/win32/00db94d5332bc0e6b2af71dfe9a88087.pat	2312	FED171E705BF9AC84D47612EF5349ABD" & vbCrLf & "data/win32/00dd2d231bf4b2bd12f75eeebd62c238.pat	5157048	CBB32F2077566655B8B88505F710C933" & vbCrLf & "data/win32/00e4c63ca715322f8d974447c683327d.pat	524364	B89F181C101D00688466BC44DAA405C4" & vbCrLf & "data/win32/00e55a66ab0b3570053776d40264b70a.pat	1657484	C733EAF258E6A1ADF97D3CCC56F5F772" & vbCrLf & "data/win32/00e5c66ef3b171161094f2b729121590.pat	24976	AC62F52B19621CF6EB50773DAEB3F8EF" & vbCrLf & "data/win32/00e9f475bb03974789b0d049af0c502c.pat	14016	AB1BECC52598648697EE7418B010F202" & vbCrLf & "data/win32/00ec2aa4e5f80f278052d5dc263fa96c.pat	76300	1DE59D042241458A6B953348966E7878" & vbCrLf & "data/win32/00f1c19d6d2054148177601864e4b3de.pat	336	079A8EED8E559262D0D403AD13C7395A" & vbCrLf & "data/win32/00f2b2f28b9f696b2af07eb6527d8d00.pat	11340	C930062AEF75F41471F6F7077B24949E" & vbCrLf & "data/win32/00f58a691779e82a81b0853a25398ec9.pat	1000588	F4ACCA7A91988C748685699F34480C9B" & vbCrLf & "data/win32/00f66f229cbb51476f761af0c3ac17a4.pat	1279712	1982809C9BE303B2D4DAAB41A9D9169C" & vbCrLf & "data/win32/00fb5920bb8820b3afc1e6be6c045b2a.pat	188384	746DC7C8FDCDF2A11C9037B3238C9F33" & vbCrLf & "data/win32/00fbd2ef554bf50f252cce091146cf5e.pat	3184	F30714F08AB1D947738910A66584AAB2")
        WriteDebugInfoAndOK("Ready to test!")
    End Sub

    Private Sub btnMerge_Click(sender As Object, e As EventArgs) Handles btnMerge.Click
        frmMain.MergePatches()
    End Sub

    Private Sub btnListDir_Click(sender As Object, e As EventArgs) Handles btnListDir.Click
        ' make a reference to a directory
        WriteDebugInfo("Listing contents...")
        Dim directoryString As String
        Dim pso2Launchpath As String
        directoryString = FrmMain.lblDirectory.Text
        pso2Launchpath = directoryString.Replace("\data\win32", "")
        Dim di As New DirectoryInfo(pso2Launchpath)
        Dim diar1 As FileInfo() = di.GetFiles()
        Dim dra As FileInfo
        File.Delete("PSO2 Folder Contents.txt")

        'list the names of all files in the specified directory
        For Each dra In diar1
            File.AppendAllText("PSO2 Folder Contents.txt", (dra.ToString() & vbCrLf))
        Next

        frmMain.PasteBinUploadFile("PSO2 Folder Contents.txt")
    End Sub

    Private Sub btnAnyDir_Click(sender As Object, e As EventArgs) Handles btnAnyDir.Click
        ' make a reference to a directory
        WriteDebugInfo("Listing contents...")
        Dim directoryString As String
        Dim myFolderBrowser As New Windows.Forms.FolderBrowserDialog

        ' Description that displays above the dialog box control.
        myFolderBrowser.Description = "Select the folder you'd like to list"

        ' Sets the root folder where the browsing starts from 
        myFolderBrowser.RootFolder = Environment.SpecialFolder.MyComputer

        Dim dlgResult As DialogResult = myFolderBrowser.ShowDialog()

        If dlgResult = Windows.Forms.DialogResult.OK Then
            directoryString = myFolderBrowser.SelectedPath
        Else
            Exit Sub
        End If

        Dim di As New DirectoryInfo(directoryString)
        Dim diar1 As FileInfo() = di.GetFiles()
        Dim dra As FileInfo
        File.Delete("Folder Contents.txt")

        'list the names of all files in the specified directory
        For Each dra In diar1
            File.AppendAllText("Folder Contents.txt", (dra.ToString() & vbCrLf))
        Next

        frmMain.PasteBinUploadFile("Folder Contents.txt")
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' make a reference to a directory
        WriteDebugInfo("Listing contents...")
        Dim directoryString As String
        Dim myFolderBrowser As New Windows.Forms.FolderBrowserDialog

        ' Description that displays above the dialog box control.
        myFolderBrowser.Description = "Select the folder you'd like to make the MD5HashList of."

        ' Sets the root folder where the browsing starts from 
        myFolderBrowser.RootFolder = Environment.SpecialFolder.MyComputer

        Dim dlgResult As DialogResult = myFolderBrowser.ShowDialog()

        If dlgResult = Windows.Forms.DialogResult.OK Then
            directoryString = myFolderBrowser.SelectedPath
        Else
            Exit Sub
        End If

        Dim di As New DirectoryInfo(directoryString)
        Dim diar1 As FileInfo() = di.GetFiles()
        Dim dra As FileInfo

        'Dim strToday As String = Format(Date.Now(), "dd-MM-yyyy")
        'strToday.Replace("/", "-")
        If File.Exists("Story MD5HashList.txt") Then File.Delete("Story MD5HashList.txt")

        'list the names of all files in the specified directory
        For Each dra In diar1
            File.AppendAllText(directoryString & " \Story MD5HashList.txt", (dra.ToString() & "," & Helper.GetMD5(directoryString & " \" & dra.ToString()) & vbCrLf))
        Next
        WriteDebugInfo("Done.")
        Process.Start("explorer.exe " & directoryString)
    End Sub

    Private Sub btnDLWUA_Click(sender As Object, e As EventArgs) Handles btnDLWUA.Click
        frmMain.DLWUA(txtAqua.Text, "testfile")
    End Sub
End Class