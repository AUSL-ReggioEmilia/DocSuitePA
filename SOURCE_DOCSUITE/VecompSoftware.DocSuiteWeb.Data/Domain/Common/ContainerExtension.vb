Imports Newtonsoft.Json

<Serializable()> _
Public Class ContainerExtension
    Inherits DomainObject(Of ContainerExtensionCompositeKey)
    Implements IAuditable

#Region " Fields "

    Private _container As Container
    Private _incrementalFather As Short
    Private _keyValue As String
    Private _numeratorType As String
    Private _registrationUser As String
    Private _registrationDate As DateTimeOffset
    Private _lastChangedUser As String
    Private _lastChangedDate As DateTimeOffset?
    Private _accountingSectionalNumber As Integer?

#End Region

#Region " Properties "

    <JsonIgnore()> _
    Public Overridable Property Container() As Container
        Get
            Return _container
        End Get
        Set(ByVal value As Container)
            _container = value
            Id.idContainer = value.Id
        End Set
    End Property

    ''' <summary> Boh </summary>
    ''' <remarks> 
    ''' TODO: Trasformare il tipo in <see cref="ContainerExtensionType"/>.
    ''' </remarks>
    Public Overridable Property KeyType() As String
        Get
            Return Id.KeyType
        End Get
        Set(ByVal value As String)
            Id.KeyType = value
        End Set
    End Property

    Public Overridable Property Incremental() As Short
        Get
            Return Id.Incremental
        End Get
        Set(ByVal value As Short)
            Id.Incremental = value
        End Set
    End Property

    Public Overridable Property IncrementalFather() As Short
        Get
            Return _incrementalFather
        End Get
        Set(ByVal value As Short)
            _incrementalFather = value
        End Set
    End Property

    Public Overridable Property KeyValue() As String
        Get
            Return _keyValue
        End Get
        Set(ByVal value As String)
            _keyValue = value
        End Set
    End Property

    Public Overridable Property AccountingSectionalNumber As Integer?
        Get
            Return _accountingSectionalNumber
        End Get
        Set(value As Integer?)
            _accountingSectionalNumber = value
        End Set
    End Property

    Public Overridable Property NumeratorType() As String
        Get
            Return _numeratorType
        End Get
        Set(ByVal value As String)
            _numeratorType = value
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

    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser
        Get
            Return _lastChangedUser
        End Get
        Set(ByVal value As String)
            _lastChangedUser = value
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

#End Region


#Region " Constructor "

    Public Sub New()
        Id = New ContainerExtensionCompositeKey()
    End Sub

#End Region

End Class