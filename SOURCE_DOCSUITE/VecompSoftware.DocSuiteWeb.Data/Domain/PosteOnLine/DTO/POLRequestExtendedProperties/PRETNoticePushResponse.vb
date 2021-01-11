Imports Newtonsoft.Json
''' <summary>
''' PRE = Pol Request Extended Properties
''' </summary>
Public NotInheritable Class PRETNoticePushResponse
    <JsonProperty("ReturnCode")>
    Public Property ReturnCode As TNoticeReturnCodeType

    <JsonProperty("Message")>
    Public Property Message As String

    <JsonProperty("TimeStamp")>
    Public Property TimeStamp As DateTime

    <JsonProperty("Reference")>
    Public Property Reference As String

    <JsonProperty("FatalError")>
    Public Property FatalError As TNoticeFatalErrorType
End Class
