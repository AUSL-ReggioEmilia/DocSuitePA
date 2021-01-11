<Serializable()> _
Public Class WebPublication
    Inherits DomainObject(Of Int32)
    Implements IAuditable


#Region " Fields "

    Private _resolution As Resolution
    Private _externalKey As String
    Private _status As Integer
    Private _location As Location
    Private _idDocument As Integer
    Private _enumDocument As Integer
    Private _descrizione As String

    Private _lastChangedUser As String
    Private _lastChangedDate As DateTimeOffset?

    Private _registrationUser As String
    Private _registrationDate As DateTimeOffset

    Private _isPrivacy As Nullable(Of Boolean)

#End Region

#Region " Properties "

    Public Overridable Property Resolution As Resolution
        Get
            Return _resolution
        End Get
        Set(value As Resolution)
            _resolution = value
        End Set
    End Property

    Public Overridable Property ExternalKey() As String
        Get
            Return _externalKey
        End Get
        Set(ByVal value As String)
            _externalKey = value
        End Set
    End Property

    ''' <summary>
    ''' 0: appena creato
    ''' 1: pubblicato
    ''' 2: revocato
    ''' 3: ritirato
    ''' </summary>
    Public Overridable Property Status() As Integer
        Get
            Return _status
        End Get
        Set(ByVal value As Integer)
            _status = value
        End Set
    End Property

    Public Overridable Property Location() As Location
        Get
            Return _location
        End Get
        Set(value As Location)
            _location = value
        End Set
    End Property

    Public Overridable Property IDDocument() As Integer
        Get
            Return _idDocument
        End Get
        Set(value As Integer)
            _idDocument = value
        End Set
    End Property

    Public Overridable Property EnumDocument() As Integer
        Get
            Return _enumDocument
        End Get
        Set(value As Integer)
            _enumDocument = value
        End Set
    End Property

    Public Overridable Property Descrizione() As String
        Get
            Return _descrizione
        End Get
        Set(value As String)
            _descrizione = value
        End Set
    End Property

    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate
        Get
            Return _lastChangedDate
        End Get
        Set(value As DateTimeOffset?)
            _lastChangedDate = value
        End Set
    End Property

    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser
        Get
            Return _lastChangedUser
        End Get
        Set(value As String)
            _lastChangedUser = value
        End Set
    End Property

    Public Overridable Property RegistrationDate As DateTimeOffset Implements IAuditable.RegistrationDate
        Get
            Return _registrationDate
        End Get
        Set(value As DateTimeOffset)
            _registrationDate = value
        End Set
    End Property

    Public Overridable Property RegistrationUser As String Implements IAuditable.RegistrationUser
        Get
            Return _registrationUser
        End Get
        Set(value As String)
            _registrationUser = value
        End Set
    End Property

    Public Overridable Property IsPrivacy As Nullable(Of Boolean)
        Get
            Return _isPrivacy.GetValueOrDefault(False)
        End Get
        Set(value As Nullable(Of Boolean))
            _isPrivacy = value
        End Set
    End Property

#End Region

End Class
