<Serializable()> _
Public Class ProtocolRoleRights

    Private _rights As ProtocolRoleRightType = ProtocolRoleRightType.None

    Public Sub New()
        _rights = ProtocolRoleRightType.None
    End Sub

    Public Sub New(ByVal pRight As ProtocolRoleRightType)
        _rights = pRight
    End Sub

    Public Sub New(ByVal pRights As String)

        If pRights.Length <> 8 Then
            Throw New DocSuiteException("ProtocolRoleRights", "La stringa di diritti deve essere di 8 caratteri. pRights = " & pRights)
        End If
        If pRights.Chars(0) = "1" Then
            _rights = _rights Or ProtocolRoleRightType.RoleModifyRights
        End If

    End Sub

    Public Overrides Function ToString() As String

        Dim sb As New Text.StringBuilder()
        sb.Append(If(IsRoleModify, "1", "0"))
        Return sb.ToString().PadRight(10, "0"c)

    End Function

    Public Overridable Property IsRoleModify() As Boolean
        Get
            Return CheckRight(ProtocolRoleRightType.RoleModifyRights)
        End Get
        Set(ByVal value As Boolean)
            SetRight(ProtocolRoleRightType.RoleModifyRights)
        End Set
    End Property

    Private Function CheckRight(ByVal right As ProtocolRoleRightType) As Boolean
        If _rights = ProtocolRoleRightType.None Then Return False
        If _rights = ProtocolRoleRightType.All Then Return True
        Return _rights = (_rights Or right)
    End Function

    Private Sub SetRight(ByVal right As ProtocolRoleRightType)
        _rights = _rights Or right
    End Sub

    Public Shared Operator Or(ByVal first As ProtocolRoleRights, ByVal second As ProtocolRoleRights) As ProtocolRoleRights
        Dim p As ProtocolRoleRightType
        p = first._rights Or second._rights
        Return New ProtocolRoleRights(p)
    End Operator

End Class

<Flags()> _
Public Enum ProtocolRoleRightType
    None = 0
    RoleModifyRights = 1
    All = (RoleModifyRights << 1) - 1
End Enum