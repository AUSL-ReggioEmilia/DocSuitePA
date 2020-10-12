<Serializable()> _
Public Class ProtocolMessage
    Inherits AuditableDomainObject(Of Guid)

#Region " Properties "

    Public Overridable Property IdProtocolMessage As Integer
    Public Overridable Property Year As Short
    Public Overridable Property Number As Integer
    Public Overridable Property Protocol As Protocol
    Public Overridable Property Message As DSWMessage

#End Region

#Region " Constructor "

    Public Sub New()

    End Sub

    Public Sub New(ByRef protocol As Protocol, ByRef message As DSWMessage)
        Me.New()
        Me.Protocol = protocol
        Me.Message = message
        Me.Year = protocol.Year
        Me.Number = protocol.Number
    End Sub

#End Region

End Class
