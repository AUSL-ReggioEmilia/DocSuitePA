Imports Newtonsoft.Json

<Serializable()> _
Public Class ContactTitle
    Inherits DomainObject(Of Int32)
    Implements ISupportBooleanLogicDelete, IAuditable

#Region "private data"

    Private _code As String
    Private _description As String
    Private _isActive As Boolean
    Private _registrationUser As String
    Private _registrationDate As DateTimeOffset
    Private _lastChangedUser As String
    Private _lastChangedDate As DateTimeOffset?

#End Region

#Region "Properties"
    <JsonProperty("ContactTitleId")> _
    Public Overrides Property Id() As Integer
        Get
            Return MyBase.Id
        End Get
        Set(ByVal value As Integer)
            MyBase.Id = value
        End Set
    End Property

    <JsonProperty("ContactTitleCode")> _
    Public Overridable Property Code() As String
        Get
            Return _code
        End Get
        Set(ByVal value As String)
            _code = value
        End Set
    End Property

    <JsonProperty("ContactTitleDescription")> _
    Public Overridable Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

    <JsonProperty("ContactTitleIsActive")>
    Public Overridable Property IsActive() As Boolean Implements ISupportBooleanLogicDelete.IsActive
        Get
            Return _isActive
        End Get
        Set(ByVal value As Boolean)
            _isActive = value
        End Set
    End Property

    <JsonProperty("ContactTitleRegistrationUser")> _
    Public Overridable Property RegistrationUser() As String Implements IAuditable.RegistrationUser
        Get
            Return _registrationUser
        End Get
        Set(ByVal value As String)
            _registrationUser = value
        End Set
    End Property

    <JsonProperty("ContactTitleRegistrationDate")> _
    Public Overridable Property RegistrationDate() As DateTimeOffset Implements IAuditable.RegistrationDate
        Get
            Return _registrationDate
        End Get
        Set(ByVal value As DateTimeOffset)
            _registrationDate = value
        End Set
    End Property

    <JsonProperty("ContactTitleLastChangedUser")> _
    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser
        Get
            Return _lastChangedUser
        End Get
        Set(ByVal value As String)
            _lastChangedUser = value
        End Set
    End Property

    <JsonProperty("ContactTitleLastChangedDate")> _
    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate
        Get
            Return _lastChangedDate
        End Get
        Set(ByVal value As DateTimeOffset?)
            _lastChangedDate = value
        End Set
    End Property

#End Region

#Region " Constructor "
    Public Sub New()
        UniqueId = Guid.NewGuid()
    End Sub
#End Region

End Class

