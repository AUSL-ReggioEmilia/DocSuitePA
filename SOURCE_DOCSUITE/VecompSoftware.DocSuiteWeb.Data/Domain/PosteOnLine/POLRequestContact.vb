Imports Newtonsoft.Json
''' <summary> Nominativo </summary>
Public Class POLRequestContact

#Region " Fields "

    Private _id As Guid
    Private _req As POLRequest
    Private _name As String
    Private _phoneNumber As String
    Private _address As String
    Private _civicNumber As String
    Private _city As String
    Private _province As String
    Private _zipCode As String
    Private _registrationDate As DateTimeOffset
    Private _registrationUser As String
    Private _lastChangedDate As DateTimeOffset?
    Private _lastChangedUser As String
    Private _extendedProperties As String

#End Region

#Region " Constructors "

    Public Sub New()
        ' noop
    End Sub

#End Region

#Region " Properties "

    Public Overridable Property Id() As Guid
        Get
            Return _id
        End Get
        Set(ByVal value As Guid)
            _id = value
        End Set
    End Property

    Public Overridable Property Request() As POLRequest
        Get
            Return _req
        End Get
        Set(ByVal value As POLRequest)
            _req = value
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

    Public Overridable Property PhoneNumber() As String
        Get
            Return _phoneNumber
        End Get
        Set(ByVal value As String)
            _phoneNumber = value
        End Set
    End Property

    Public Overridable Property Address() As String
        Get
            Return _address
        End Get
        Set(ByVal value As String)
            _address = value
        End Set
    End Property

    Public Overridable Property CivicNumber() As String
        Get
            Return _civicNumber
        End Get
        Set(ByVal value As String)
            _civicNumber = value
        End Set
    End Property

    Public Overridable Property City() As String
        Get
            Return _city
        End Get
        Set(ByVal value As String)
            _city = value
        End Set
    End Property

    Public Overridable Property Province() As String
        Get
            Return _province
        End Get
        Set(ByVal value As String)
            _province = value
        End Set
    End Property

    Public Overridable Property ZipCode() As String
        Get
            Return _zipCode
        End Get
        Set(ByVal value As String)
            _zipCode = value
        End Set
    End Property

    Public Overridable Property RegistrationDate() As DateTimeOffset
        Get
            Return _registrationDate
        End Get
        Set(ByVal value As DateTimeOffset)
            _registrationDate = value
        End Set
    End Property

    Public Overridable Property RegistrationUser() As String
        Get
            Return _registrationUser
        End Get
        Set(ByVal value As String)
            _registrationUser = value
        End Set
    End Property

    Public Overridable Property LastChangedDate As DateTimeOffset?
        Get
            Return _lastChangedDate
        End Get
        Set(ByVal value As DateTimeOffset?)
            _lastChangedDate = value
        End Set
    End Property

    Public Overridable Property LastChangedUser As String
        Get
            Return _lastChangedUser
        End Get
        Set(ByVal value As String)
            _lastChangedUser = value
        End Set
    End Property

    ''' <summary>
    ''' The content to be a serialized Dictionary(Of string, string)
    ''' </summary>
    Public Overridable Property ExtendedProperties As String
        Get
            Return _extendedProperties
        End Get
        Set(value As String)
            _extendedProperties = value
        End Set
    End Property

#End Region

#Region " Methods "
    Public Overridable Sub TrySetExtendedProperties(ByRef extendedProperties As POLRequestContactExtendedProperties)
        Me.ExtendedProperties = POLRequestContactExtendedProperties.Serialize(extendedProperties)
    End Sub

    Public Overridable Function TryGetExtendedProperties() As POLRequestContactExtendedProperties
        Try
            If ExtendedProperties IsNot Nothing Then
                Return POLRequestContactExtendedProperties.Deserialize(ExtendedProperties)
            End If
            Return Nothing
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

#End Region
End Class
