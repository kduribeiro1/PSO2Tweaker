Imports System.Globalization

Public Class Helper
    Private Shared ReadOnly SizeSuffixes As String() = {"bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"}

    Public Shared DefaltCultureInfo As CultureInfo = New System.Globalization.CultureInfo("en")

    Public Shared Function SizeSuffix(ByVal value As Long) As String
        If value < 0 Then Return "-" & SizeSuffix(-value)
        If value = 0 Then Return "0.0 bytes"

        Dim pow As Long = 1
        Dim index As Integer = 0

        While pow <= value
            pow <<= 10
            index += 1
        End While

        pow >>= 10

        Return String.Format("{0:n2} {1}", value / pow, SizeSuffixes(index - 1))
    End Function

    Public Shared Function GetRegKey(Of T)(ByRef Key As String) As T
        Return My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\AIDA", Key, Nothing)
    End Function

    Public Shared Sub SetRegKey(Of T)(ByRef Key As String, ByRef Value As T)
        My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\AIDA", Key, Value)
    End Sub
End Class
