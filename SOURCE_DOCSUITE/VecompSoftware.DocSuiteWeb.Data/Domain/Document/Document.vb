<Serializable()> _
Public Class Document
    Inherits DomainObject(Of YearNumberCompositeKey)
    Implements IAuditable

#Region " Fields "

    Private _location As Location
    Private _container As Container
    Private _role As Role
    Private _status As DocumentTabStatus
    Private _category As Category
    Private _subCategory As Category
    Private _startDate As Date?
    Private _reStartDate As Date?
    Private _endDate As Date?
    Private _expiryDate As Date?
    Private _archiveDate As Date?
    Private _serviceNumber As String
    Private _name As String
    Private _object As String
    Private _manager As String
    Private _note As String
    Private _checkPublication As String
    Private _lastChangedUser As String
    Private _lastChangedDate As DateTimeOffset?
    Private _lastChangedReason As String
    Private _registrationUser As String
    Private _registrationDate As DateTimeOffset
    Private _contacts As IList(Of DocumentContact)
    Private _objects As IList(Of DocumentObject)
    Private _documentTokens As IList(Of DocumentToken)
    Private _documentVersiong As IList(Of DocumentVersioning)
    Private _logs As IList(Of DocumentLog)
    Private _documentFolders As IList(Of DocumentFolder)
    'Conservation
    Private _conservationStatus As Char?
    Private _lastConservationDate As Date?
    Private _hasConservatedDocs As Boolean = False

#End Region

#Region " Properties "

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

    Public Overridable Property Location() As Location
        Get
            Return _location
        End Get
        Set(ByVal value As Location)
            _location = value
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

    Public Overridable Property Role() As Role
        Get
            Return _role
        End Get
        Set(ByVal value As Role)
            _role = value
        End Set
    End Property

    Public Overridable Property Status() As DocumentTabStatus
        Get
            Return _status
        End Get
        Set(ByVal value As DocumentTabStatus)
            _status = value
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

    Public Overridable Property SubCategory() As Category
        Get
            Return _subCategory
        End Get
        Set(ByVal value As Category)
            _subCategory = value
        End Set
    End Property

    Public Overridable Property StartDate() As Date?
        Get
            Return _startDate
        End Get
        Set(ByVal value As Date?)
            _startDate = value
        End Set
    End Property

    Public Overridable Property ReStartDate() As Date?
        Get
            Return _reStartDate
        End Get
        Set(ByVal value As Date?)
            _reStartDate = value
        End Set
    End Property

    Public Overridable Property EndDate() As Date?
        Get
            Return _endDate
        End Get
        Set(ByVal value As Date?)
            _endDate = value
        End Set
    End Property

    Public Overridable Property ExpiryDate() As Date?
        Get
            Return _expiryDate
        End Get
        Set(ByVal value As Date?)
            _expiryDate = value
        End Set
    End Property

    Public Overridable Property ArchiveDate() As Date?
        Get
            Return _archiveDate
        End Get
        Set(ByVal value As Date?)
            _archiveDate = value
        End Set
    End Property

    Public Overridable ReadOnly Property IsExpired() As Boolean
        Get
            If _expiryDate.HasValue Then
                Return (Date.Compare(ExpiryDate.Value, Date.Now()) <= 0)
            Else
                Return False
            End If
        End Get
    End Property

    Public Overridable Property ServiceNumber() As String
        Get
            Return _serviceNumber
        End Get
        Set(ByVal value As String)
            _serviceNumber = value
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

    Public Overridable Property DocumentObject() As String
        Get
            Return _object
        End Get
        Set(ByVal value As String)
            _object = value
        End Set
    End Property

    Public Overridable Property Manager() As String
        Get
            Return _manager
        End Get
        Set(ByVal value As String)
            _manager = value
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

    Public Overridable Property CheckPublication() As String
        Get
            Return _checkPublication
        End Get
        Set(ByVal value As String)
            _checkPublication = value
        End Set
    End Property

    Public Overridable ReadOnly Property ContactDescription() As String
        Get
            Dim contactDesc As String = ""
            If Contacts.Count > 0 Then
                contactDesc = Replace(Contacts(0).Contact.Description, "|", " ")
            End If
            Return contactDesc
        End Get
    End Property

    Public Overridable Property Contacts() As IList(Of DocumentContact)
        Get
            Return _contacts
        End Get
        Protected Set(ByVal value As IList(Of DocumentContact))
            _contacts = value
        End Set
    End Property

    Public Overridable Property Objects() As IList(Of DocumentObject)
        Get
            Return _objects
        End Get
        Protected Set(ByVal value As IList(Of DocumentObject))
            _objects = value
        End Set
    End Property

    Public Overridable Property DocumentTokens() As IList(Of DocumentToken)
        Get
            Return _documentTokens
        End Get
        Protected Set(ByVal value As IList(Of DocumentToken))
            _documentTokens = value
        End Set
    End Property

    Public Overridable Property DocumentVersionings() As IList(Of DocumentVersioning)
        Get
            Return _documentVersiong
        End Get
        Protected Set(ByVal value As IList(Of DocumentVersioning))
            _documentVersiong = value
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

    Public Overridable Property LastChangedReason() As String
        Get
            Return _lastChangedReason
        End Get
        Set(ByVal value As String)
            _lastChangedReason = value
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

    Public Overridable Property RegistrationUser() As String Implements IAuditable.RegistrationUser
        Get
            Return _registrationUser
        End Get
        Set(ByVal value As String)
            _registrationUser = value
        End Set
    End Property

    Public Overridable Property DocumentFolders() As IList(Of DocumentFolder)
        Get
            Return _documentFolders
        End Get
        Set(ByVal value As IList(Of DocumentFolder))
            _documentFolders = value
        End Set
    End Property

    Public Overridable Property DocumentLogs() As IList(Of DocumentLog)
        Get
            Return _logs
        End Get
        Set(ByVal value As IList(Of DocumentLog))
            _logs = value
        End Set
    End Property

    'Conservation
    Public Overridable Property ConservationStatus As Char?
        Get
            Return _conservationStatus.GetValueOrDefault("M"c)
        End Get
        Set(ByVal value As Char?)
            _conservationStatus = value
        End Set
    End Property

    Public Overridable Property LastConservationDate() As Date?
        Get
            Return _lastConservationDate
        End Get
        Set(ByVal value As Date?)
            _lastConservationDate = value
        End Set
    End Property

    Public Overridable Property HasConservatedDocs() As Boolean
        Get
            Return _hasConservatedDocs
        End Get
        Set(ByVal value As Boolean)
            _hasConservatedDocs = value
        End Set
    End Property

#End Region

#Region " Constructors "

    Public Sub New()
        Id = New YearNumberCompositeKey()
        _contacts = New List(Of DocumentContact)
    End Sub

#End Region

#Region " Methods "

    Public Overridable Sub AddContact(ByRef pContact As Contact)
        Dim dc As New DocumentContact()
        dc.Contact = pContact
        dc.Document = Me
        Contacts.Add(dc)
    End Sub

#End Region

End Class
