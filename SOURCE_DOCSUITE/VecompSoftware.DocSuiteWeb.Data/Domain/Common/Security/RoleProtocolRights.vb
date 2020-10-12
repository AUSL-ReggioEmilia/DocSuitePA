<Serializable()> _
Public Class RoleProtocolRights

    Private _rights As RoleProtocolRightType = RoleProtocolRightType.None

    Public Sub New()
        _rights = RoleProtocolRightType.None
    End Sub

    Public Sub New(ByVal pRight As RoleProtocolRightType)
        _rights = pRight
    End Sub

    Public Sub New(ByVal pRights As String)

        If pRights.Length < 3 Then
            Throw New DocSuiteException("La stringa di diritti deve contenere almeno 2 caratteri.")
        End If
        If pRights.Chars(0) = "1" Then
            _rights = _rights Or RoleProtocolRightType.RoleEnabled
        End If
        If pRights.Chars(1) = "1" Then
            _rights = _rights Or RoleProtocolRightType.RoleManager
        End If
        If pRights.Chars(2) = "1" Then
            _rights = _rights Or RoleProtocolRightType.RolePEC
        End If
        If pRights.Chars(3) = "1" Then
            _rights = _rights Or RoleProtocolRightType.RoleProtocolMail
        End If

    End Sub

    Public Overrides Function ToString() As String

        Dim sb As New Text.StringBuilder()
        sb.Append(If(IsRoleEnabled, "1", "0"))
        sb.Append(If(IsRoleManager, "1", "0"))
        sb.Append(If(IsRolePEC, "1", "0"))
        sb.Append(If(IsRoleProtocolMail, "1", "0"))
        Return sb.ToString().PadRight(20, "0"c)

    End Function

    Public Overridable Property IsRoleEnabled() As Boolean
        Get
            Return CheckRight(RoleProtocolRightType.RoleEnabled)
        End Get
        Set(ByVal value As Boolean)
            SetRight(RoleProtocolRightType.RoleEnabled)
        End Set
    End Property

    Public Overridable Property IsRoleManager() As Boolean
        Get
            Return CheckRight(RoleProtocolRightType.RoleManager)
        End Get
        Set(ByVal value As Boolean)
            SetRight(RoleProtocolRightType.RoleManager)
        End Set
    End Property

    Public Overridable Property IsRolePEC As Boolean
        Get
            Return CheckRight(RoleProtocolRightType.RolePEC)
        End Get
        Set(ByVal value As Boolean)
            SetRight(RoleProtocolRightType.RolePEC)
        End Set
    End Property
    Public Overridable Property IsRoleProtocolMail As Boolean
        Get
            Return CheckRight(RoleProtocolRightType.RoleProtocolMail)
        End Get
        Set(ByVal value As Boolean)
            SetRight(RoleProtocolRightType.RoleProtocolMail)
        End Set
    End Property
    Private Function CheckRight(ByVal right As RoleProtocolRightType) As Boolean
        If _rights = RoleProtocolRightType.None Then Return False
        If _rights = RoleProtocolRightType.All Then Return True
        Return _rights = (_rights Or right)
    End Function

    Private Sub SetRight(ByVal right As RoleProtocolRightType)
        _rights = _rights Or right
    End Sub

    Public Shared Operator Or(ByVal first As RoleProtocolRights, ByVal second As RoleProtocolRights) As RoleProtocolRights
        Dim p As RoleProtocolRightType
        p = first._rights Or second._rights
        Return New RoleProtocolRights(p)
    End Operator

End Class

<Flags()> _
Public Enum RoleProtocolRightType
    None = 0
    RoleEnabled = 1
    RoleManager = RoleEnabled << 1
    RolePEC = RoleManager << 1
    RoleProtocolMail = RolePEC << 1
    All = (RolePEC << 4) - 1
End Enum