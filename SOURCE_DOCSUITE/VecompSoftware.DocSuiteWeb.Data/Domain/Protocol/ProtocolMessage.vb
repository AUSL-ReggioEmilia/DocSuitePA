<Serializable()> _
Public Class ProtocolMessage
    Inherits AuditableDomainObject(Of Integer)

#Region " Properties "

    Public Overridable Property Year As Short
    Public Overridable Property Number As Integer
    Public Overridable Property Protocol As Protocol
    Public Overridable Property Message As DSWMessage
    Public Overridable Property UniqueIdProtocol As Guid

#End Region

#Region " Constructor "

    Public Sub New()
        UniqueId = Guid.NewGuid()
    End Sub

    Public Sub New(ByRef protocol As Protocol, ByRef message As DSWMessage)
        Me.Protocol = protocol
        Me.Message = message
        UniqueId = Guid.NewGuid()
        UniqueIdProtocol = protocol.UniqueId
    End Sub

    Public Sub New(ByVal protocolYear As Short, ByVal protocolNumber As Integer, ByRef message As DSWMessage)
        Year = protocolYear
        Number = protocolNumber
        Me.Message = message
        UniqueId = Guid.NewGuid()
    End Sub

#End Region

End Class
