Public Class DocumentHeader

#Region "Fields"
    Private _year As Short
    Private _number As Integer
    Private _container As Container
    Private _containerName As String
    Private _role As Role
    Private _roleName As String
    Private _category As Category
    Private _categoryName As String
    Private _subCategory As Category
    Private _startDate As Date?
    Private _serviceNumber As String
    Private _name As String
    Private _object As String
    Private _manager As String
    Private _note As String
    Private _contactDescription As String
    Private _folderName As String
    Private _folderExpiryDate As Date?
    Private _folderExpiryDescription As String
    Private _folderIncremental As Integer
    Private _status As DocumentTabStatus
    Private _documentObjDescription As String
#End Region

#Region "Porperties"
    Public ReadOnly Property Document() As String
        Get
            Return String.Format("{0}/{1}", _year.ToString, _number.ToString.PadLeft(7, "0"c))
        End Get
    End Property

    Public Overridable Property Year() As Short
        Get
            Return _year
        End Get
        Set(ByVal value As Short)
            _year = value
        End Set
    End Property

    Public Overridable Property Number() As Integer
        Get
            Return _number
        End Get
        Set(ByVal value As Integer)
            _number = value
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

    Public Overridable Property ContainerName() As String
        Get
            Return _containerName
        End Get
        Set(ByVal value As String)
            _containerName = value
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

    Public Overridable Property RoleName() As String
        Get
            Return _roleName
        End Get
        Set(ByVal value As String)
            _roleName = value
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

    Public Overridable Property CategoryName() As String
        Get
            Return _categoryName
        End Get
        Set(ByVal value As String)
            _categoryName = value
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

    Public Overridable Property FolderExpiryDescription() As String
        Get
            Return _folderExpiryDescription
        End Get
        Set(ByVal value As String)
            _folderExpiryDescription = value
        End Set
    End Property

    Public Overridable Property FolderName() As String
        Get
            Return _folderName
        End Get
        Set(ByVal value As String)
            _folderName = value
        End Set
    End Property

    Public Overridable Property FolderIncremental() As Integer
        Get
            Return _folderIncremental
        End Get
        Set(ByVal value As Integer)
            _folderIncremental = value
        End Set
    End Property

    Public Overridable Property FolderExpiryDate() As Date?
        Get
            Return _folderExpiryDate
        End Get
        Set(ByVal value As Date?)
            _folderExpiryDate = value
        End Set
    End Property

    Public Overridable Property ContactDescription() As String
        Get
            If Not String.IsNullOrEmpty(_contactDescription) Then
                _contactDescription = _contactDescription.Replace("|", " ")
            End If
            Return _contactDescription
        End Get
        Set(ByVal value As String)
            _contactDescription = value
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

    Public Overridable Property DocumentObjectDescription() As String
        Get
            Return _documentObjDescription
        End Get
        Set(ByVal value As String)
            _documentObjDescription = value
        End Set
    End Property
#End Region

#Region "Calculated Properties"
    Public Overridable ReadOnly Property CategoryCode() As String
        Get
            Dim _code As String = ""
            If _subcategory IsNot Nothing Then
                _code = _subcategory.FullCode.Replace("0", "")
            Else
                _code = _category.Code.ToString()
            End If
            Return _code.Replace("|", ".")
        End Get
    End Property
#End Region

#Region "WS Fields"
    Private _location As Location
    Private _idDocument As Integer?
    Private _idAttachments As Integer?
#End Region

#Region "WS Properties"
    Public Overridable Property Location() As Location
        Get
            Return _location
        End Get
        Set(ByVal value As Location)
            _location = value
        End Set
    End Property

    Public Overridable Property IdDocument() As Integer?
        Get
            Return _idDocument
        End Get
        Set(ByVal value As Integer?)
            _idDocument = value
        End Set
    End Property

    Public Overridable Property IdAttachments() As Integer?
        Get
            Return _idAttachments
        End Get
        Set(ByVal value As Integer?)
            _idAttachments = value
        End Set
    End Property
#End Region
End Class
