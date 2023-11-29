<Serializable()> _
Public Class DocumentTokenUser
    Inherits DomainObject(Of YearNumberIncrCompositeKey)
    Implements IAuditable, ISupportBooleanLogicDelete

#Region "private data"
    Private _step As Short
    Private _subStep As Short
    Private _isActive As Boolean
    Private _idRoleDestination As Short
    Private _userRole As String
    Private _userName As String
    Private _account As String
    Private _note As String
    Private _registrationUser As String
    Private _registrationDate As DateTimeOffset
    Private _lastChangedUser As String
    Private _lastChangedDate As DateTimeOffset?
    Private _lastStep As Short
    Private _lastSubStep As Short
    Private _role As Role

#End Region

#Region "Properties"
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

    Public Overridable ReadOnly Property FormatStep() As String
        Get
            Dim s As String = String.Empty
            If _step <> 0 Then
                s = _step
                If _subStep <> 0 Then
                    s &= "." & _subStep
                End If
            End If
            Return s
        End Get
    End Property

    Public Overridable ReadOnly Property LastFormatStep() As String
        Get
            Dim s As String = String.Empty
            If _lastStep <> 0 Then
                s = _lastStep
                If _lastSubStep <> 0 Then
                    s &= "." & _lastSubStep
                End If
            End If
            Return s
        End Get
    End Property

    Public Overridable ReadOnly Property UserDate() As String
        Get
            Return _registrationUser & " " & _registrationDate.ToString()
        End Get
    End Property

    Public Overridable ReadOnly Property LastUserDate() As String
        Get
            Return _lastChangedUser & " " & _lastChangedDate.ToString()
        End Get
    End Property

    Public Overridable Property IsActive() As Boolean Implements ISupportBooleanLogicDelete.IsActive
        Get
            Return _isActive
        End Get
        Set(ByVal value As Boolean)
            _isActive = value
        End Set
    End Property
    Public Overridable Property IdRoleDestination() As Short
        Get
            Return _idRoleDestination
        End Get
        Set(ByVal value As Short)
            _idRoleDestination = value
        End Set
    End Property

    Public Overridable Property UserRole() As String
        Get
            Return _userRole
        End Get
        Set(ByVal value As String)
            _userRole = value
        End Set
    End Property
    Public Overridable Property UserName() As String
        Get
            Return _userName
        End Get
        Set(ByVal value As String)
            _userName = value
        End Set
    End Property
    Public Overridable Property Account() As String
        Get
            Return _account
        End Get
        Set(ByVal value As String)
            _account = value
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
    Public Overridable Property LastStep() As Short
        Get
            Return _lastStep
        End Get
        Set(ByVal value As Short)
            _lastStep = value
        End Set
    End Property
    Public Overridable Property LastSubStep() As Short
        Get
            Return _lastSubStep
        End Get
        Set(ByVal value As Short)
            _lastSubStep = value
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


#End Region

#Region "Ctor/init"
    Public Sub New()
        Id = New YearNumberIncrCompositeKey()
    End Sub
#End Region


End Class

