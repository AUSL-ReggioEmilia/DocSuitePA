<Serializable()>
Public Class ProtocolRejectedRole
    Inherits AuditableDomainObject(Of Guid)

#Region " Properties "

    Public Overridable Property Year As Short

    Public Overridable Property Number As Integer

    Public Overridable Property Role As Role

    Public Overridable Property Rights As String

    Public Overridable Property Note As String

    Public Overridable Property DistributionType As String

    Public Overridable Property Type As String

    Public Overridable Property NoteType As ProtocolRoleNoteType?

    Public Overridable Property Protocol As Protocol

    Public Overridable Property UniqueIdProtocol As Guid

    Public Overridable Property Status As ProtocolRoleStatus

#End Region

#Region " Constructor "

    Public Sub New()
        Id = Guid.NewGuid()
    End Sub

#End Region

End Class




