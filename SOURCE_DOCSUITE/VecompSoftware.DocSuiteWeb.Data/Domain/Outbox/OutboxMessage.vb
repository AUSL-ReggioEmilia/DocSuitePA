Public Class OutboxMessage
    Inherits DomainObject(Of Guid)
    Public Overridable Property MessageBody As String
    Public Overridable Property MessageTypeName As String
    Public Overridable Property LastError As String
    Public Overridable Property RetryCount As Integer
    Public Overridable Property Status As Status
    Public Overridable Property MessageType As MessageType
    Public Overridable Property CreatedAt As DateTime
    Public Overridable Property LastAttemptDate As DateTime?

    Public Sub New()
        Id = Guid.NewGuid()
        Status = Status.Pending
        CreatedAt = DateTime.UtcNow
    End Sub
End Class