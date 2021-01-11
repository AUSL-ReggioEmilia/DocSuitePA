
Imports Newtonsoft.Json

Public Class PRETNoticeGetStatusResponse
    <JsonProperty("ReturnCode")>
    Public Property ReturnCode As TNoticeReturnCodeType
    <JsonProperty("Message")>
    Public Property Message As String
    <JsonProperty("TimeStamp")>
    Public Property TimeStamp As DateTime
    <JsonProperty("FatalErrorType")>
    Public Property FatalErrorType As TNoticeFatalErrorType
    <JsonProperty("UrltNotice")>
    Public Property UrltNotice As String
    <JsonProperty("UrltNotice_xml")>
    Public Property UrltNotice_xml As String
    <JsonProperty("UrlAccept")>
    Public Property UrlAccept As String
    <JsonProperty("UrlCPF")>
    Public Property UrlCPF As String
    <JsonProperty("UrlCPF_xml")>
    Public Property UrlCPF_xml As String
    <JsonProperty("StatusDescription")>
    Public Property StatusDescription As String
    <JsonProperty("StatusCodeType")>
    Public Property StatusCodeType As TNoticeStatusCodeType
End Class
