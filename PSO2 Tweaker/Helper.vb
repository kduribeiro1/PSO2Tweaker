Public Class Helper
    Private Shared ReadOnly SizeSuffixes As String() = {"bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"}

    Public Shared Function SizeSuffix(ByVal value As Long) As String
        If (value = 0) Then
            Return "0 bytes"
        ElseIf (value < 0) Then
            Return String.Concat("-", SizeSuffix(-value))
        End If

        Dim num As Integer = Math.Log(value, 1024)
        Dim num1 As Decimal = value / (1 << (num * 10))

        Return String.Format("{0:n2} {1}", num1, SizeSuffixes(num))
    End Function
End Class
