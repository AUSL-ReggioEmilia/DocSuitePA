<Serializable()> _
Public Class ProtocolLink
    Inherits AuditableDomainObject(Of ProtocolLinkCompositeKey)

#Region "private data"

    Private _protocol As Protocol
    Private _linkType As Integer
    Private _protocolLinked As Protocol
    Private _registrationUser As String
    Private _registrationDate As Date?

#End Region

#Region "Properties"

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

    Public Overridable Property LinkType() As Integer
        Get
            Return _linkType
        End Get
        Set(ByVal value As Integer)
            _linkType = value
        End Set
    End Property

    Public Overridable Property ProtocolLinked() As Protocol
        Get
            Return _protocolLinked
        End Get
        Set(ByVal value As Protocol)
            _protocolLinked = value
            Id.YearSon = value.Year
            Id.NumberSon = value.Number
        End Set
    End Property

    Public Overridable Property UniqueIdProtocolParent As Guid

    Public Overridable Property UniqueIdProtocolSon As Guid

#End Region

#Region "Ctor/init"
    Public Sub New()
        UniqueId = Guid.NewGuid()
        RegistrationDate = DateTimeOffset.UtcNow
        RegistrationUser = DocSuiteContext.Current.User.FullUserName
    End Sub
#End Region

End Class





