Imports Newtonsoft.Json

Public Class POLRequestExtendedProperties
    'Informative value of the source of the information
    <JsonProperty("Source")>
    Public Property Source As String

    <JsonProperty("PushResponse")>
    Public Property PushResponse As PRETNoticePushResponse

    <JsonProperty("GetStatus")>
    Public Property GetStatus As PRETNoticeGetStatusResponse

    <JsonProperty("MaxExceptionCounts")>
    Public Const MaxExceptionCounts As Integer = 5

    <JsonProperty>
    Public ReadOnly Property IsFaulted As Boolean
        Get
            Return Not String.IsNullOrEmpty(ExceptionInfo.ExceptionMessage)
        End Get
    End Property

    <JsonProperty("ExceptionInfo")>
    Public Property ExceptionInfo As ExceptionInfo

    <JsonProperty("Metadata")>
    Public Property MetaData As POLRequestMetaData

    Public Sub New()
        Me.ExceptionInfo = New ExceptionInfo()
    End Sub

    Public Sub ResetException()
        Me.ExceptionInfo = New ExceptionInfo()
    End Sub

    Public Shared Function Serialize(ByRef extendedProperties As POLRequestExtendedProperties) As String
        Return JsonConvert.SerializeObject(extendedProperties)
    End Function

    Public Shared Function Deserialize(ByRef extendedProperties As String) As POLRequestExtendedProperties
        Try
            If (extendedProperties IsNot Nothing) Then
                Return JsonConvert.DeserializeObject(Of POLRequestExtendedProperties)(extendedProperties)
            End If
            Return Nothing
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

End Class
