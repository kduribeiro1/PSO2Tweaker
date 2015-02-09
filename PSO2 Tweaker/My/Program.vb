Imports System.Globalization
Imports System.Threading

Namespace My
    Public Class Program
        Public Shared ReadOnly Args As String() = Environment.GetCommandLineArgs()
        Public Shared ReadOnly StartPath As String = Windows.Forms.Application.StartupPath

        Public Shared MainForm As FrmMain
        Public Shared HostsFilePath As String
        Public Shared Pso2RootDir As String
        Public Shared Pso2WinDir As String

        Public Shared Sub Main()
            Dim locale = RegKey.GetValue(Of String)(RegKey.Locale)

            If Not String.IsNullOrEmpty(locale) Then
                Thread.CurrentThread.CurrentUICulture = New CultureInfo(locale)
                Thread.CurrentThread.CurrentCulture = New CultureInfo(locale)
            End If
        End Sub
    End Class
End Namespace
