<Serializable()> _
Public Class ProtocolTransfert
    Inherits DomainObject(Of YearNumberCompositeKey)
    Implements IAuditable

#Region "Fields"
    Private _category As Category
    Private _type As ProtocolType
    Private _container As Container
    Private _object As String
    Private _note As String
    Private _request As String
    Private _lastChangedUser As String
    Private _lastChangedDate As DateTimeOffset?
    Private _registrationUser As String
    Private _registrationDate As DateTimeOffset
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


    Public Overridable Property Container() As Container
        Get
            Return _container
        End Get
        Set(ByVal value As Container)
            _container = value
        End Set
    End Property

    Public Overridable Property Category() As Category
        Get
            Return _category
        End Get
        Set(ByVal value As Category)
            _category = value
        End Set
    End Property

    Public Overridable Property ProtocolObject() As String
        Get
            Return _object
        End Get
        Set(ByVal value As String)
            _object = value
        End Set
    End Property

    Public Overridable Property Type() As ProtocolType
        Get
            Return _type
        End Get
        Set(ByVal value As ProtocolType)
            _type = value
        End Set
    End Property

    Public Overridable Property Note() As String
        Get
            Return _note
        End Get
        Set(ByVal value As String)
            _note = value
        End Set
    End Property

    Public Overridable Property Request() As String
        Get
            Return _request
        End Get
        Set(ByVal value As String)
            _request = value
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


#Region "New"
    Public Sub New()
        Id = New YearNumberCompositeKey()
    End Sub
#End Region

End Class
