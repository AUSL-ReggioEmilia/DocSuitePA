<Serializable()> _
Public Class ProtocolContactManual
    Inherits AuditableDomainObject(Of YearNumberIdCompositeKey)

#Region " Fields "

    Private _contact As Contact
    Private _comunicationType As String
    Private _type As String
    Private _idAD As String
    Private _protocol As Protocol

#End Region

#Region " Properties "

    Public Overridable Property ComunicationType() As String
        Get
            Return _comunicationType
        End Get
        Set(ByVal value As String)
            _comunicationType = value
        End Set
    End Property

    Public Overridable Property IdAD() As String
        Get
            Return _idAD
        End Get
        Set(ByVal value As String)
            _idAD = value
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

    Public Overridable Property Contact() As Contact
        Get
            Return _contact
        End Get
        Set(ByVal value As Contact)
            _contact = value
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

    Public Overridable Property UniqueIdProtocol As Guid
    public Overridable Property SDIIdentification As String
#End Region

#Region " Constructor "

    Public Sub New()
        Id = New YearNumberIdCompositeKey()
        UniqueId = Guid.NewGuid()
    End Sub

#End Region

End Class


