Public Class ProtocolConservationHeader

#Region "Fields"
    Private _year As Short
    Private _number As Integer
    Private _container As Container
    Private _category As Category
    Private _registrationDate As Date?
    Private _type As ProtocolType
    Private _protocolObject As String
#End Region

#Region "Properties"
    Public Property Year() As Short
        Get
            Return _year
        End Get
        Set(ByVal value As Short)
            _year = value
        End Set
    End Property

    Public Property Number() As Integer
        Get
            Return _number
        End Get
        Set(ByVal value As Integer)
            _number = value
        End Set
    End Property

    Public Property RegistrationDate() As Date?
        Get
            Return _registrationDate
        End Get
        Set(ByVal value As Date?)
            _registrationDate = value
        End Set
    End Property

    Public Property ProtocolObject() As String
        Get
            Return _protocolObject
        End Get
        Set(ByVal value As String)
            _protocolObject = value
        End Set
    End Property

    Public Property Type() As ProtocolType
        Get
            Return _type
        End Get
        Set(ByVal value As ProtocolType)
            _type = value
        End Set
    End Property

    Public Property Container() As Container
        Get
            Return _container
        End Get
        Set(ByVal value As Container)
            _container = value
        End Set
    End Property

    Public Property Category() As Category
        Get
            Return _category
        End Get
        Set(ByVal value As Category)
            _category = value
        End Set
    End Property
#End Region

End Class
