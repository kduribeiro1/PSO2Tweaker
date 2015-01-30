Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Runtime.InteropServices

Public Class Helper
    Private Shared ReadOnly SizeSuffixes As String() = {"bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"}
    Private Shared ReadOnly HexTable As String() = {"00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "0A", "0B", "0C", "0D", "0E", "0F", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "1A", "1B", "1C", "1D", "1E", "1F", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "2A", "2B", "2C", "2D", "2E", "2F", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "3A", "3B", "3C", "3D", "3E", "3F", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "4A", "4B", "4C", "4D", "4E", "4F", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "5A", "5B", "5C", "5D", "5E", "5F", "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "6A", "6B", "6C", "6D", "6E", "6F", "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "7A", "7B", "7C", "7D", "7E", "7F", "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "8A", "8B", "8C", "8D", "8E", "8F", "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "9A", "9B", "9C", "9D", "9E", "9F", "A0", "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "AA", "AB", "AC", "AD", "AE", "AF", "B0", "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9", "BA", "BB", "BC", "BD", "BE", "BF", "C0", "C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "CA", "CB", "CC", "CD", "CE", "CF", "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "DA", "DB", "DC", "DD", "DE", "DF", "E0", "E1", "E2", "E3", "E4", "E5", "E6", "E7", "E8", "E9", "EA", "EB", "EC", "ED", "EE", "EF", "F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "FA", "FB", "FC", "FD", "FE", "FF"}
    Private Shared ReadOnly MD5Provider As New MD5Provider(&H100000)
    Private Shared FolderDownloads As New Guid("374DE290-123F-4565-9164-39C4925E467B")

    Public Shared ReadOnly DefaltCultureInfo As CultureInfo = New CultureInfo("en")

    Public Shared Function GetFileSize(ByVal MyFilePath As String) As Long
        Dim MyFile As New FileInfo(MyFilePath)
        Return MyFile.Length
    End Function

    Public Shared Sub DeleteDirectory(path As String)
        If Directory.Exists(path) Then Directory.Delete(path, True)
    End Sub

    Public Shared Function CheckLink(ByVal Url As String) As Boolean
        Try
            Dim request As HttpWebRequest = CType(WebRequest.Create(Url), HttpWebRequest)
            request.Timeout = 5000
            request.Method = "HEAD"

            Using response As WebResponse = request.GetResponse()
                Return True
            End Using
        Catch
            Return False
        End Try
    End Function

    Public Shared Function GetDownloadsPath() As String
        Dim path__1 As String
        If Environment.OSVersion.Version.Major >= 6 Then
            Dim pathPtr As IntPtr
            Dim hr As Integer = External.SHGetKnownFolderPath(FolderDownloads, 0, IntPtr.Zero, pathPtr)
            If hr = 0 Then
                path__1 = Marshal.PtrToStringUni(pathPtr)
                Marshal.FreeCoTaskMem(pathPtr)
                Return path__1
            End If
        End If
        path__1 = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.Personal))
        path__1 = Path.Combine(path__1, "Downloads")
        Return path__1
    End Function

    Public Shared Function GetMD5(ByVal path As String) As String
        Try
            Using stream As FileStream = File.Open(path, FileMode.Open)
                Return GetMD5(stream)
            End Using
        Catch
        End Try

        Return ""
    End Function

    Public Shared Function GetMD5(stream As Stream) As String
        Try
            Dim hash = MD5Provider.ComputeHash(stream)
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