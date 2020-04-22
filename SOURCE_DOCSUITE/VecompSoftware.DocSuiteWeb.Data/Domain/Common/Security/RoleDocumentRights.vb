Public Class RoleDocumentRights

    Private _rights As RoleDocumentRightType = RoleDocumentRightType.None

    Public Sub New()
        _rights = RoleDocumentRightType.None
    End Sub

    Public Sub New(ByVal pRight As RoleDocumentRightType)
        _rights = pRight
    End Sub

    Public Sub New(ByVal pRights As String)

        If pRights.Length < 1 Then
            Throw New DocSuiteException("La stringa di diritti deve contenere almeno 1 carattere.") With {.Descrizione = "pRights= " & pRights}
        End If
        If pRights.Chars(0) = "1" Then _rights = _rights Or RoleDocumentRightType.RoleEnabled
    End Sub

    Public Overrides Function ToString() As String

        Dim sb As New Text.StringBuilder()
        sb.Append(If(IsRoleEnabled, "1", "0"))
        Return sb.ToString().PadRight(10, "0"c)

    End Function

    Public Overridable Property IsRoleEnabled() As Boolean
        Get
            Return CheckRight(RoleDocumentRightType.RoleEnabled)
        End Get
        Set(ByVal value As Boolean)
            SetRight(RoleDocumentRightType.RoleEnabled)
        End Set
    End Property

    Private Function CheckRight(ByVal right As RoleDocumentRightType) As Boolean
        If _rights = RoleDocumentRightType.None Then Return False
        If _rights = RoleDocumentRightType.All Then Return True
        Return _rights = (_rights Or right)
    End Function

    Private Sub SetRight(ByVal right As RoleDocumentRightType)
        _rights = _rights Or right
    End Sub

    Public Shared Operator Or(ByVal first As RoleDocumentRights, ByVal second As RoleDocumentRights) As RoleDocumentRights
        Dim p As RoleDocumentRightType
        p = first._rights Or second._rights
        Return New RoleDocumentRights(p)
    End Operator


End Class


<Flags()> _
Public Enum RoleDocumentRightType
    None = 0
    RoleEnabled = 1
    All = (RoleEnabled << 1) - 1
End Enum