Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Runtime.InteropServices

Public Class Helper
    Private Shared ReadOnly SizeSuffixes As String() = {"bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"}
    Private Shared ReadOnly HexTable As String() = {"00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "0A", "0B", "0C", "0D", "0E", "0F", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "1A", "1B", "1C", "1D", "1E", "1F", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "2A", "2B", "2C", "2D", "2E", "2F", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "3A", "3B", "3C", "3D", "3E", "3F", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "4A", "4B", "4C", "4D", "4E", "4F", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "5A", "5B", "5C", "5D", "5E", "5F", "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "6A", "6B", "6C", "6D", "6E", "6F", "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "7A", "7B", "7C", "7D", "7E", "7F", "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "8A", "8B", "8C", "8D", "8E", "8F", "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "9A", "9B", "9C", "9D", "9E", "9F", "A0", "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "AA", "AB", "AC", "AD", "AE", "AF", "B0", "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9", "BA", "BB", "BC", "BD", "BE", "BF", "C0", "C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "CA", "CB", "CC", "CD", "CE", "CF", "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "DA", "DB", "DC", "DD", "DE", "DF", "E0", "E1", "E2", "E3", "E4", "E5", "E6", "E7", "E8", "E9", "EA", "EB", "EC", "ED", "EE", "EF", "F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "FA", "FB", "FC", "FD", "FE", "FF"}
    Private Shared ReadOnly Md5Provider As New Md5Provider(&H100000)
    Private Shared ReadOnly FolderDownloads As New Guid("374DE290-123F-4565-9164-39C4925E467B")
    Private Shared ReadOnly Generator As Random = New Random()

    Public Shared ReadOnly DefaltCultureInfo As CultureInfo = New CultureInfo("en")

    Public Shared Iterator Function GetLines(path As String) As IEnumerable(Of String)
        Using reader = File.OpenText(path)
            Dim currentLine As String
            Do
                currentLine = reader.ReadLine()
                If (currentLine Is Nothing) Then Exit Do
                Yield currentLine
            Loop

            reader.Close()
        End Using
    End Function

    Public Shared Function CheckIfRunning(processName As String) As Boolean
        Dim currentProcessId = Process.GetCurrentProcess().Id

        If Process.GetProcessesByName(processName).Length > If(processName = "PSO2 Tweaker", 1, 0) Then
            Dim closeItYesNo As MsgBoxResult = MsgBox("It seems that " & processName.Replace(".exe", "") & " is already running. Would you like to close it?", vbYesNo)

            If closeItYesNo = vbYes Then
                For Each proc As Process In Process.GetProcessesByName(processName)
                    If proc.Id <> currentProcessId Then proc.Kill()
                Next
            End If
        Else
            Return False
        End If

        Return True
    End Function

    Public Shared Function GetFileSize(ByVal myFilePath As String) As Long
        Dim myFile As New FileInfo(myFilePath)
        Return myFile.Length
    End Function

    Public Shared Sub DeleteDirectory(path As String)
        If Directory.Exists(path) Then Directory.Delete(path, True)
    End Sub

    Public Shared Function GetRandom(ByVal min As Integer, ByVal max As Integer) As Integer
        Return Generator.Next(min, max)
    End Function

    Public Shared Function IsFileInUse(ByVal path As String) As Boolean
        Try
            Using stream = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None)
                stream.Close()
            End Using
        Catch
            Return True
        End Try

        Return False
    End Function

    Public Shared Sub PasteBinUploadFile(fileToUpload As String)
        ServicePointManager.Expect100Continue = False

        Using client As New WebClient()
            client.Headers.Add("Content-Type", "application/x-www-form-urlencoded")
            Dim data As String = "?api_paste_private=1&api_option=paste&api_paste_name=Error Log report&api_paste_format=text&api_paste_expire_date=N&api_dev_key=ddc1e2efaca45d3df87e6b93ceb43c9f&api_paste_code=" & File.ReadAllText(fileToUpload)
            Dim responce = client.UploadString("http://pastebin.com/api/api_post.php", "POST", data)
            MsgBox(My.Resources.strPleasecopytheURL)
            Process.Start(responce)
        End Using
    End Sub

    Public Shared Function CheckLink(ByVal url As String) As Boolean
        Try
            Dim request As HttpWebRequest = CType(WebRequest.Create(url), HttpWebRequest)
            request.Timeout = 5000
            request.Method = "HEAD"

            Using response As WebResponse = request.GetResponse()
                Return (response IsNot Nothing)
            End Using
        Catch
            Return False
        End Try
    End Function

    Public Shared Function GetDownloadsPath() As String
        Dim path1 As String
        If Environment.OSVersion.Version.Major >= 6 Then
            Dim pathPtr As IntPtr
            Dim hr As Integer = External.ShGetKnownFolderPath(FolderDownloads, 0, IntPtr.Zero, pathPtr)
            If hr = 0 Then
                path1 = Marshal.PtrToStringUni(pathPtr)
                Marshal.FreeCoTaskMem(pathPtr)
                Return path1
            End If
        End If
        path1 = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.Personal))
        path1 = Path.Combine(path1, "Downloads")
        Return path1
    End Function

    Public Shared Function GetMd5(ByVal path As String) As String
        Try
            Using stream As FileStream = File.Open(path, FileMode.Open)
                Return GetMd5(stream)
            End Using
        Catch
        End Try

        Return ""
    End Function

    Public Shared Function GetMd5(stream As Stream) As String
        Try
            Dim hash = Md5Provider.ComputeHash(stream)
            Return String.Join("", HexTable(hash(0)), HexTable(hash(1)), HexTable(hash(2)), HexTable(hash(3)), HexTable(hash(4)), HexTable(hash(5)), HexTable(hash(6)), HexTable(hash(7)), HexTable(hash(8)), HexTable(hash(9)), HexTable(hash(10)), HexTable(hash(11)), HexTable(hash(12)), HexTable(hash(13)), HexTable(hash(14)), HexTable(hash(15)))
        Catch
        End Try

        Return ""
    End Function

    Public Shared Function SizeSuffix(ByVal value As Long) As String
        If value < 0 Then Return "-" & SizeSuffix(-value)
        If value = 0 Then Return "0 bytes"

        Dim pow As ULong = 1
        Dim index As Integer = 0

        While pow <= value
            pow <<= 10
            index += 1
        End While

        pow >>= 10

        Return String.Format("{0:n2} {1}", value / pow, SizeSuffixes(index - 1))
    End Function
End Class