Public Class POLAccount

#Region " Fields "

    Private _id As Int32
    Private _name As String
    Private _wsRaccomandataUrl As String
    Private _wsLetteraUrl As String
    Private _wsTelegrammaUrl As String
    Private _username As String
    Private _password As String
    Private _customer As String
    Private _X509Certificate As String
    Private _roles As IList(Of Role)
    Private _defaultContact As Contact
    Private _extendedProperties As String

#End Region

#Region " Properties "

    Public Overridable Property Id() As Int32
        Get
            Return _id
        End Get
        Set(ByVal value As Int32)
            _id = value
        End Set
    End Property

    Public Overridable Property Roles() As IList(Of Role)
        Get
            Return _roles
        End Get
        Set(ByVal value As IList(Of Role))
            _roles = value
        End Set
    End Property


    Public Overridable Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property

    Public Overridable Property WsRaccomandataUrl() As String
        Get
            Return _wsRaccomandataUrl
        End Get
        Set(ByVal value As String)
            _wsRaccomandataUrl = value
        End Set
    End Property

    Public Overridable Property WsLetteraUrl() As String
        Get
            Return _wsLetteraUrl
        End Get
        Set(ByVal value As String)
            _wsLetteraUrl = value
        End Set
    End Property

    Public Overridable Property WsTelegrammaUrl() As String
        Get
            Return _wsTelegrammaUrl
        End Get
        Set(ByVal value As String)
            _wsTelegrammaUrl = value
        End Set
    End Property

    Public Overridable Property Username() As String
        Get
            Return _username
        End Get
        Set(ByVal value As String)
            _username = value
        End Set
    End Property

    Public Overridable Property Password() As String
        Get
            Return _password
        End Get
        Set(ByVal value As String)
            _password = value
        End Set
    End Property

    Public Overridable Property Customer() As String
        Get
            Return _customer
        End Get
        Set(ByVal value As String)
            _customer = value
        End Set
    End Property

    Public Overridable Property X509Certificate() As String
        Get
            Return _X509Certificate
        End Get
        Set(ByVal value As String)
            _X509Certificate = value
        End Set
    End Property

    Public Overridable Property DefaultContact() As Contact
        Get
            Return _defaultContact
        End Get
        Set(ByVal value As Contact)
            _defaultContact = value
        End Set
    End Property

    Public Overridable Property ExtendedProperties As String
        Get
            Return _extendedProperties
        End Get
        Set(value As String)
            _extendedProperties = value
        End Set
    End Property
#End Region

#Region " Constructors "

    Public Sub New()
        _roles = New List(Of Role)
    End Sub

#End Region

#Region " Methods "

    Public Overridable Function TryGetExtendedProperties() As POLAccountExtendedProperties
        Try
            If (ExtendedProperties IsNot Nothing) Then
                Return POLAccountExtendedProperties.Deserialize(ExtendedProperties)
            End If
            Return Nothing
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

#End Region
End Class
