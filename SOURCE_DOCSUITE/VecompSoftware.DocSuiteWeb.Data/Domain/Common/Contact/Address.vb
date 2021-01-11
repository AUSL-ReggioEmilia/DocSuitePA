<Serializable()> _
Public Class Address

#Region "private data"

    Private _placeName As ContactPlaceName
    Private _address As String
    Private _civicNumber As String
    Private _zipCode As String
    Private _cityCode As String
    Private _city As String

#End Region

#Region "Properties"

    Public Overridable Property PlaceName() As ContactPlaceName
        Get
            Return _placeName
        End Get
        Set(ByVal value As ContactPlaceName)
            _placeName = value
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

    Public Overridable Property ZipCode() As String
        Get
            Return _zipCode
        End Get
        Set(ByVal value As String)
            _zipCode = value
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

    Public Overridable Property CityCode() As String
        Get
            Return _cityCode
        End Get
        Set(ByVal value As String)
            _cityCode = value
        End Set
    End Property

    Public Overridable Property Nationality As String

    Public Overridable Property Language As LanguageType?

    Public ReadOnly Property IsValidAddress() As Boolean
        Get
            Return Not (String.IsNullOrWhiteSpace(Address) OrElse String.IsNullOrWhiteSpace(City) _
                OrElse String.IsNullOrWhiteSpace(ZipCode) OrElse (DocSuiteContext.Current.ProtocolEnv.PosteWebValidateCivicNumber AndAlso String.IsNullOrWhiteSpace(CivicNumber)))
        End Get
    End Property

    Public ReadOnly Property ValidateCityCode() As Boolean
        Get
            If Me Is Nothing Then
                Return True
            End If
            If String.IsNullOrEmpty(CityCode) Then
                Return True
            End If
            If CityCode.Length > 2 Then
                Return False
            End If
            Return True
        End Get
    End Property

#End Region

#Region "Ctor/init"
    Public Sub New()
        '_placeName = New ContactPlaceName()
        '_address = ""
        '_civicNumber = ""
        '_zipCode = ""
        '_cityCode = ""
        '_city = ""
    End Sub
#End Region

End Class

