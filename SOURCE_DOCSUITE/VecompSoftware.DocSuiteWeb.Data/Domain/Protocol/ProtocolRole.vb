<Serializable()>
Public Class ProtocolRole
    Inherits AuditableDomainObject(Of Guid)

#Region " Fields "
    Private _protocol As Protocol
#End Region

#Region " Properties "

    Public Overridable Property Year As Short

    Public Overridable Property Number As Integer

    Public Overridable Property Rights As String

    Public Overridable Property Note As String

    Public Overridable Property DistributionType As String

    Public Overridable Property Type As String

    Public Overridable Property NoteType As ProtocolRoleNoteType?

    Public Overridable Property Status As ProtocolRoleStatus

    Public Overridable Property Protocol As Protocol
        Get
            Return _protocol
        End Get
        Set(ByVal value As Protocol)
            _protocol = value
            Year = value.Year
            Number = value.Number
        End Set
    End Property

    Public Overridable Property Role As Role

#End Region

#Region " Constructor "

    Public Sub New()
        Status = ProtocolRoleStatus.ToEvaluate
    End Sub

#End Region

End Class


