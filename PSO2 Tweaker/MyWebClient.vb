Imports System.Net

Public Class MyWebClient
    Inherits WebClient

    Private _timeout As Integer

    Public Property timeout As Integer
        Get
            timeout = _timeout
        End Get

        Set(ByVal value As Integer)
            _timeout = value
        End Set
    End Property

    Public Sub MyWebClient()
        Me.timeout = 60000
    End Sub

    Protected Overrides Function GetWebRequest(ByVal address As Uri) As WebRequest
        Dim result = MyBase.GetWebRequest(address)
        result.Timeout = Me._timeout
        Return result
    End Function

End Class