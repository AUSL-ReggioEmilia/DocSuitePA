<Serializable()> _
Public Class DocumentToken
    Inherits DomainObject(Of YearNumberIncrCompositeKey)
    Implements IAuditable, ISupportBooleanLogicDelete

#Region " Fields "

    Private _incrementalOrigin As Short
    Private _isActive As Boolean
    Private _response As String
    Private _step As Short
    Private _subStep As Short
    Private _roleSource As Role
    Private _roleDestination As Role
    Private _operationDate As Date?
    Private _expiryDate As Date?
    Private _object As String
    Private _reason As String
    Private _note As String
    Private _reasonResponse As String
    Private _registrationUser As String
    Private _registrationDate As DateTimeOffset
    Private _lastChangedUser As String
    Private _lastChangedDate As DateTimeOffset?
    Private _document As Document
    Private _documentTabToken As DocumentTabToken

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
    Public Overridable Property Incremental() As Short
        Get
            Return Id.Incremental
        End Get
        Set(ByVal value As Short)
            Id.Incremental = value
        End Set
    End Property
    Public Overridable Property IncrementalOrigin() As Short
        Get
            Return _incrementalOrigin
        End Get
        Set(ByVal value As Short)
            _incrementalOrigin = value
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
    Public Overridable Property Response() As String
        Get
            Return _response
        End Get
        Set(ByVal value As String)
            _response = value
        End Set
    End Property
    Public Overridable Property DocStep() As Short
        Get
            Return _step
        End Get
        Set(ByVal value As Short)
            _step = value
        End Set
    End Property
    Public Overridable Property SubStep() As Short
        Get
            Return _subStep
        End Get
        Set(ByVal value As Short)
            _subStep = value
        End Set
    End Property

    Public Overridable ReadOnly Property FullStep() As String
        Get
            Return DocStep & If(SubStep <> 0, "." & SubStep, "")
        End Get
    End Property

    Public Overridable Property RoleSource() As Role
        Get
            Return _roleSource
        End Get
        Set(ByVal value As Role)
            _roleSource = value
        End Set
    End Property
    Public Overridable Property RoleDestination() As Role
        Get
            Return _roleDestination
        End Get
        Set(ByVal value As Role)
            _roleDestination = value
        End Set
    End Property
    Public Overridable Property OperationDate() As Date?
        Get
            Return _operationDate
        End Get
        Set(ByVal value As Date?)
            _operationDate = value
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
    Public Overridable Property DocObject() As String
        Get
            Return _object
        End Get
        Set(ByVal value As String)
            _object = value
        End Set
    End Property
    Public Overridable Property Reason() As String
        Get
            Return _reason
        End Get
        Set(ByVal value As String)
            _reason = value
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
    Public Overridable Property ReasonResponse() As String
        Get
            Return _reasonResponse
        End Get
        Set(ByVal value As String)
            _reasonResponse = value
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

    Public Overridable Property Document() As Document
        Get
            Return _document
        End Get
        Set(ByVal value As Document)
            _document = value
        End Set
    End Property

    Public Overridable Property DocumentTabToken() As DocumentTabToken
        Get
            Return _documentTabToken
        End Get
        Set(ByVal value As DocumentTabToken)
            _documentTabToken = value
        End Set
    End Property

    Public Overridable ReadOnly Property RegistrationUserDateDescription() As String
        Get
            Return String.Format("{0} {1:dd/MM/yyyy HH:mm:ss}", RegistrationUser, RegistrationDate)
        End Get
    End Property
    Public Overridable ReadOnly Property SourceDestinationRoleDescription() As String
        Get
            Return String.Format("{0}<BR>{1}", RoleSource.Name, RoleDestination.Name)
        End Get
    End Property
    Public Overridable ReadOnly Property OperationExpiryDateDescription() As String
        Get
            Return String.Format("{0:dd/MM/yyyy HH:mm:ss}<BR>{1:dd/MM/yyyy}", OperationDate, ExpiryDate)
        End Get
    End Property

    Public Overridable ReadOnly Property StepDescription() As String
        Get
            Return DocStep & If(SubStep = 0, "", "." & SubStep)
        End Get
    End Property

    Public Overridable ReadOnly Property LastChangedUserDateDescription() As String
        Get
            Return String.Format("{0} {1:dd/MM/yyyy HH:mm:ss}", LastChangedUser, LastChangedDate)
        End Get
    End Property

#End Region

#Region " Constructors "

    Public Sub New()
        Id = New YearNumberIncrCompositeKey()
    End Sub

#End Region

End Class

