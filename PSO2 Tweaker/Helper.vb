Imports System.Globalization
Imports System.Collections.Generic
Imports Microsoft.Win32

Public Class Helper
    Private Shared ReadOnly SizeSuffixes As String() = {"bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"}
    Private Shared RegistryCache As Dictionary(Of String, Object) = New Dictionary(Of String, Object)
    Private Shared RegistrySubKey As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\AIDA", True)

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
        Dim returnValue As T
        If RegistryCache.TryGetValue(Key, returnValue) Then Return returnValue

        returnValue = RegistrySubKey.GetValue(Key, Nothing)

        If returnValue IsNot Nothing Then RegistryCache.Add(Key, returnValue)

        Return returnValue
    End Function

    Public Shared Sub SetRegKey(Of T)(ByRef Key As String, ByRef Value As T)
        RegistryCache(Key) = Value
        RegistrySubKey.SetValue(Key, Value)
    End Sub
End Class
