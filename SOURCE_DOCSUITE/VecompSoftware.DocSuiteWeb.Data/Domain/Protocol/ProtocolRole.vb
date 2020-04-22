<Serializable()>
Public Class ProtocolRole
    Inherits AuditableDomainObject(Of YearNumberIdCompositeKey)

#Region " Fields "

    Private _protocol As Protocol
    Private _role As Role

#End Region

#Region " Properties "

    Public Overridable Property Year() As Short
        Get
            Return Id.Year
        End Get
        Set(ByVal value As Short)
            Id.Year = value
        End Set
    End Property

    Public Overridable Property Number() As Integer
        Get
            Return Id.Number
        End Get
        Set(ByVal value As Integer)
            Id.Number = value
        End Set
    End Property

    Public Overridable Property Role() As Role
        Get
            Return _role
        End Get
        Set(ByVal value As Role)
            _role = value
            Id.Id = value.Id
        End Set
    End Property

    Public Overridable Property Rights As String

    Public Overridable Property Note As String

    Public Overridable Property DistributionType As String

    Public Overridable Property Type As String

    Public Overridable Property NoteType As ProtocolRoleNoteType?

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

    Public Overridable Property UniqueIdProtocol As Guid

    Public Overridable Property Status As ProtocolRoleStatus

#End Region

#Region " Constructor "

    Public Sub New()
        Id = New YearNumberIdCompositeKey()
        UniqueId = Guid.NewGuid()
        Status = ProtocolRoleStatus.ToEvaluate
    End Sub

#End Region

End Class


