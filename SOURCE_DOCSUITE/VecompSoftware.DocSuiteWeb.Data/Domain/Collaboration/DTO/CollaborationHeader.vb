
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web

<Serializable()> _
Public Class CollaborationHeader

#Region " Fields "

    Private _collaboration As Integer
    Private _documentType As String
    Private _idPriority As String
    Private _idStatus As String
    Private _memorandumDate As DateTime?
    Private _object As String
    Private _note As String
    Private _publicationDate As DateTime?
    Private _registrationUser As String
    Private _registrationDate As DateTimeOffset
    Private _registrationName As String
    Private _lastChangedUser As String
    Private _lastChangedDate As DateTimeOffset?
    Private _protRegistrationDate As DateTimeOffset
    Private _reslProposeDate As DateTime?
    Private _idDocument As Integer?
    Private _documentName As String
    Private _versioningCount As Integer?
    Private _year As Nullable(Of Short)
    Private _number As Integer?
    Private _idResolution As Integer?
    Private _idRole As Short
    Private _account As String
    Private _location As Location
    Private _idDocumentSeriesItem As Integer?
    Private _hasDocumentExtracted As Boolean?

#End Region

#Region " Properties "

    ''' <summary> Identificativo collaborazione </summary>
    Public Property Collaboration() As Integer
        Get
            Return _collaboration
        End Get
        Set(ByVal value As Integer)
            _collaboration = value
        End Set
    End Property

    Public Property DocumentType() As String
        Get
            Return _documentType
        End Get
        Set(ByVal value As String)
            _documentType = value
        End Set
    End Property

    Public Property IdPriority() As String
        Get
            Return _idPriority
        End Get
        Set(ByVal value As String)
            _idPriority = value
        End Set
    End Property

    Public Property IdStatus() As String
        Get
            Return _idStatus
        End Get
        Set(ByVal value As String)
            _idStatus = value
        End Set
    End Property

    Public Property MemorandumDate() As DateTime?
        Get
            Return _memorandumDate
        End Get
        Set(ByVal value As DateTime?)
            _memorandumDate = value
        End Set
    End Property

    Public Property CollaborationObject() As String
        Get
            Return _object
        End Get
        Set(ByVal value As String)
            _object = value
        End Set
    End Property

    Public Property Note() As String
        Get
            Return _note
        End Get
        Set(ByVal value As String)
            _note = value
        End Set
    End Property

    Public Property PublicationDate() As DateTime?
        Get
            Return _publicationDate
        End Get
        Set(ByVal value As DateTime?)
            _publicationDate = value
        End Set
    End Property

    Public Property RegistrationUser() As String
        Get
            Return _registrationUser
        End Get
        Set(ByVal value As String)
            _registrationUser = value
        End Set
    End Property

    Public Property RegistrationDate() As DateTimeOffset
        Get
            Return _registrationDate
        End Get
        Set(ByVal value As DateTimeOffset)
            _registrationDate = value
        End Set
    End Property

    Public Property RegistrationName() As String
        Get
            Return _registrationName
        End Get
        Set(ByVal value As String)
            _registrationName = value
        End Set
    End Property

    Public Property LastChangedUser As String
        Get
            Return _lastChangedUser
        End Get
        Set(ByVal value As String)
            _lastChangedUser = value
        End Set
    End Property

    Public Property LastChangedDate As DateTimeOffset?
        Get
            Return _lastChangedDate
        End Get
        Set(ByVal value As DateTimeOffset?)
            _lastChangedDate = value
        End Set
    End Property

    Public Property ProtocolRegistrationDate() As DateTimeOffset
        Get
            Return _protRegistrationDate
        End Get
        Set(ByVal value As DateTimeOffset)
            _protRegistrationDate = value
        End Set
    End Property

    Public Property ResolutionProposeDate() As DateTime?
        Get
            Return _reslProposeDate
        End Get
        Set(ByVal value As DateTime?)
            _reslProposeDate = value
        End Set
    End Property

    Public Property IdDocument() As Integer?
        Get
            Return _idDocument
        End Get
        Set(ByVal value As Integer?)
            _idDocument = value
        End Set
    End Property

    Public Property DocumentName() As String
        Get
            Return _documentName
        End Get
        Set(ByVal value As String)
            _documentName = value
        End Set
    End Property

    Public Property VersioningCount() As Integer?
        Get
            Return _versioningCount
        End Get
        Set(ByVal value As Integer?)
            _versioningCount = value
        End Set
    End Property

    Public Property Year() As Nullable(Of Short)
        Get
            Return _year
        End Get
        Set(ByVal value As Nullable(Of Short))
            _year = value
        End Set
    End Property

    Public Property Number() As Integer?
        Get
            Return _number
        End Get
        Set(ByVal value As Integer?)
            _number = value
        End Set
    End Property

    Public Property IdResolution() As Integer?
        Get
            Return _idResolution
        End Get
        Set(ByVal value As Integer?)
            _idResolution = value
        End Set
    End Property

    Public Property DestinationFirst() As Boolean?

    Public Property Account() As String
        Get
            Return _account
        End Get
        Set(ByVal value As String)
            _account = value
        End Set
    End Property

    Public Property IdRole() As String
        Get
            Return _idRole
        End Get
        Set(ByVal value As String)
            _idRole = value
        End Set
    End Property

    Public Property Location() As Location
        Get
            Return _location
        End Get
        Set(ByVal value As Location)
            _location = value
        End Set
    End Property

    Public ReadOnly Property DocumentDate() As DateTimeOffset
        Get
            Return _protRegistrationDate
        End Get
    End Property

    Public ReadOnly Property Proposer() As String
        Get
            Return String.Format("{0}{1}{2}", _registrationUser, WebHelper.Br, _registrationDate.DefaultString)
        End Get
    End Property

    Public Property IdDocumentSeriesItem As Integer?
        Get
            Return _idDocumentSeriesItem
        End Get
        Set(ByVal value As Integer?)
            _idDocumentSeriesItem = value
        End Set
    End Property

    Public Property HasDocumentExtracted As Boolean?
        Get
            Return _hasDocumentExtracted
        End Get
        Set(ByVal value As Boolean?)
            _hasDocumentExtracted = value
        End Set
    End Property

    Public Property IsSignedRequired As Boolean?

#End Region

End Class
