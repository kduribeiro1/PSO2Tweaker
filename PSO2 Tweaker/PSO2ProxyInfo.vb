Imports System.Runtime.Serialization

<DataContract>
Public Class Pso2ProxyInfo
    <DataMember(Name:="host")>
    Public Host As String

    <DataMember(Name:="version")>
    Public Version As String

    <DataMember(Name:="name")>
    Public Name As String

    <DataMember(Name:="PSO2Proxy Public Server")>
    Public Server As String

    <DataMember(Name:="publickeyurl")>
    Public PublicKeyUrl As String
End Class