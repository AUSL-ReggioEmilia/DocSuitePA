Imports Newtonsoft.Json

Public Class ExceptionInfo

    Private _exceptionMessage As String = String.Empty
    Private _exceptionStackTrace As String = String.Empty

    <JsonProperty("ExceptionStackTrace")>
    Public Property ExceptionStackTrace As String
        Get
            Return _exceptionStackTrace
        End Get
        Set(value As String)
            'we don't want the stack trace to be longer then the max value
            If (value.Length > UInt16.MaxValue) Then
                _exceptionStackTrace = value.Substring(0, UInt16.MaxValue)
            Else
                _exceptionStackTrace = value
            End If
        End Set
    End Property

    <JsonProperty("ExceptionMessage")>
    Public Property ExceptionMessage As String
        Get
            Return _exceptionMessage
        End Get
        Set(value As String)
            'we don't want the exception message to be longer then the max value
            If (value.Length > UInt16.MaxValue) Then
                _exceptionMessage = value.Substring(0, UInt16.MaxValue)
            Else
                _exceptionMessage = value
            End If

        End Set
    End Property

    ''' <summary>
    ''' What method was last called
    ''' </summary>
    <JsonProperty("MethodCall")>
    Public MethodCall As String = String.Empty

    ''' <summary>
    ''' Sometimes an exception may be caused due to a server being down.
    ''' RetryPolicies help as much as the timing permits, but they don't have a large time stamp.
    ''' We will have a retry attempt within the jeep service calls
    ''' </summary>
    <JsonProperty("ExceptedCount")>
    Public ExceptedCount As Integer = 0
End Class
