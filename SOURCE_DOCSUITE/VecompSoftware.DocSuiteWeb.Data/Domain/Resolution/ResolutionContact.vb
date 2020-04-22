<Serializable()> _
Public Class ResolutionContact
    Inherits AuditableDomainObject(Of ResolutionContactCompositeKey)

#Region "private data"
    Private _incremental As Nullable(Of Short)
    Private _registrationUser As String
    Private _registrationDate As Date?
    Private _resolution As Resolution
    Private _contact As Contact
#End Region

#Region "Properties"
    Public Overridable Property Incremental() As Nullable(Of Short)
        Get
            Return _incremental
        End Get
        Set(ByVal value As Nullable(Of Short))
            _incremental = value
        End Set
    End Property
    Public Overridable Property Resolution() As Resolution
        Get
            Return _resolution
        End Get
        Set(ByVal value As Resolution)
            _resolution = value
            Id.IdResolution = _resolution.Id
        End Set
    End Property
    Public Overridable Property Contact() As Contact
        Get
            Return _contact
        End Get
        Set(ByVal value As Contact)
            _contact = value
            Id.IdContact = _contact.Id
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

    Public Overridable Property UniqueIdResolution As Guid
#End Region

#Region "Ctor/init"
    Public Sub New()
        Id = New ResolutionContactCompositeKey()
        UniqueId = Guid.NewGuid()
    End Sub
#End Region

End Class

