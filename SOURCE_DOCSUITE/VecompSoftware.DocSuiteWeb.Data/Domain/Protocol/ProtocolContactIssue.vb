<Serializable()> _
Public Class ProtocolContactIssue
    Inherits DomainObject(Of YearNumberIdCompositeKey)
    Implements IAuditable

#Region "private data"

    Private _lastChangedUser As String
    Private _lastChangedDate As DateTimeOffset?
    Private _registrationUser As String
    Private _registrationDate As DateTimeOffset
    Private _protocol As Protocol
    Private _contact As Contact
    Private _incremental As Integer

#End Region

#Region "Properties"

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
            Id.Id = value.Id
        End Set
    End Property

    Public Overridable Property Incremental() As Integer
        Get
            Return _incremental
        End Get
        Set(ByVal value As Integer)
            _incremental = value
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

    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate
        Get
            Return _lastChangedDate
        End Get
        Set(ByVal value As DateTimeOffset?)
            _lastChangedDate = value
        End Set
    End Property

    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser
        Get
            Return _lastChangedUser
        End Get
        Set(ByVal value As String)
            _lastChangedUser = value
        End Set
    End Property

    Public Overridable Property RegistrationUser() As String Implements IAuditable.RegistrationUser
        Get
            Return _registrationUser
        End Get
        Set(ByVal value As String)
            _registrationUser = value
        End Set
    End Property

    Public Overridable Property RegistrationDate() As DateTimeOffset Implements IAuditable.RegistrationDate
        Get
            Return _registrationDate
        End Get
        Set(ByVal value As DateTimeOffset)
            _registrationDate = value
        End Set
    End Property

#End Region

#Region "Ctor/init"
    Public Sub New()
        Id = New YearNumberIdCompositeKey()
    End Sub
#End Region

End Class



