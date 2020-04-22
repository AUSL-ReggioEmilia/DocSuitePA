<Serializable()> _
Public Class ProtocolContact
    Inherits AuditableDomainObject(Of ProtocolContactCompositeKey)

#Region " Fields "

    Private _type As String
    Private _protocol As Protocol
    Private _contact As Contact

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

    Public Overridable Property Contact() As Contact
        Get
            Return _contact
        End Get
        Set(ByVal value As Contact)
            _contact = value
            Id.IdContact = value.Id
        End Set
    End Property

    Public Overridable Property Protocol() As Protocol
        Get
            Return _protocol
        End Get
        Set(ByVal value As Protocol)
            _protocol = value
            Id.Year = value.Year
            Id.Number = value.Number
        End Set
    End Property

    Public Overridable Property ComunicationType() As String
        Get
            Return Id.ComunicationType
        End Get
        Set(ByVal value As String)
            Id.ComunicationType = value
        End Set
    End Property

    Public Overridable Property Type() As String
        Get
            Return _type
        End Get
        Set(ByVal value As String)
            _type = value
        End Set
    End Property

    Public Overridable Property UniqueIdProtocol As Guid
#End Region

#Region " Constructor "

    Public Sub New()
        Id = New ProtocolContactCompositeKey()
        UniqueId = Guid.NewGuid()
    End Sub

    Public Sub New(ByVal key As ProtocolContactCompositeKey)
        Id = key
        UniqueId = Guid.NewGuid()
    End Sub

#End Region

End Class

