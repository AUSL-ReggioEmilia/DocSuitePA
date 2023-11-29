<Serializable()> _
Public Class DocumentFolder
    Inherits DomainObject(Of YearNumberIncrCompositeKey)
    Implements IAuditable, ISupportBooleanLogicDelete

#Region " Fields "

    Private _folderName As String
    Private _IncrementalFather As Nullable(Of Short)
    Private _registrationUser As String
    Private _registrationDate As DateTimeOffset
    Private _lastChangedUser As String
    Private _lastChangedDate As DateTimeOffset?
    Private _documentsRequired As Integer?
    Private _childrenFolder As IList(Of DocumentFolder)
    Private _documentObjects As IList(Of DocumentObject)
    Private _expiryDate As Date?
    Private _description As String
    Private _isActive As Boolean

#End Region

#Region " Properties "

    Public Overridable Property Year() As Short
        Get
            Return Id.Year.Value
        End Get
        Set(ByVal value As Short)
            Id.Year = value
        End Set
    End Property
    Public Overridable Property Number() As Integer
        Get
            Return Id.Number.Value
        End Get
        Set(ByVal value As Integer)
            Id.Number = value
        End Set
    End Property
    Public Overridable Property Incremental() As Short
        Get
            Return Id.Incremental.Value
        End Get
        Set(ByVal value As Short)
            Id.Incremental = value
        End Set
    End Property

    Public Overridable Property Role() As Role

    Public Overridable Property FolderName() As String
        Get
            Return _folderName
        End Get
        Set(ByVal value As String)
            _folderName = value
        End Set
    End Property

    Public Overridable Property IncrementalFather() As Nullable(Of Short)
        Get
            Return _IncrementalFather
        End Get
        Set(ByVal value As Nullable(Of Short))
            _IncrementalFather = value
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

    Public Overridable Property DocumentsRequired() As Integer?
        Get
            Return _documentsRequired
        End Get
        Set(ByVal value As Integer?)
            _documentsRequired = value
        End Set
    End Property

    Public Overridable Property Children() As IList(Of DocumentFolder)
        Get
            Return _childrenFolder
        End Get
        Set(ByVal value As IList(Of DocumentFolder))
            _childrenFolder = value
        End Set
    End Property

    Public Overridable ReadOnly Property HasChildren() As Boolean
        Get
            If Children Is Nothing Then
                Return False
            Else
                Return (Children.Count > 0)
            End If
        End Get
    End Property

    Public Overridable Property DocumentObjects() As IList(Of DocumentObject)
        Get
            Return _documentObjects
        End Get
        Set(ByVal value As IList(Of DocumentObject))
            _documentObjects = value
        End Set
    End Property

    Public Overridable ReadOnly Property HasObjects() As Boolean
        Get
            If DocumentObjects Is Nothing Then
                Return False
            Else
                Return (DocumentObjects.Count > 0)
            End If
        End Get
    End Property

    Public Overridable Property ExpiryDate() As Date?
        Get
            Return _expiryDate
        End Get
        Set(ByVal value As Date?)
            _expiryDate = value
        End Set
    End Property

    Public Overridable Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

    Public Overridable Property IsActive() As Boolean Implements ISupportBooleanLogicDelete.IsActive
        Get
            Return _isActive
        End Get
        Set(ByVal value As Boolean)
            _isActive = value
        End Set
    End Property

#End Region

#Region " Constructor "

    Public Sub New()
        Id = New YearNumberIncrCompositeKey()
    End Sub

#End Region

End Class

