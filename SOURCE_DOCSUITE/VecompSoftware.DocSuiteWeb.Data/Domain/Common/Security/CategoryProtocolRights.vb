Imports System.Text

<Serializable()> _
Public Class CategoryProtocolRights

    Private _rights As CategoryProtocolRightType = CategoryProtocolRightType.None

    Public Sub New()
        _rights = CategoryProtocolRightType.None
    End Sub

    Public Sub New(ByVal pRight As CategoryProtocolRightType)
        _rights = pRight
    End Sub

    Public Sub New(ByVal pRights As String)

        If pRights.Length < 5 Then
            Throw New DocSuiteException("CategoryProtocolRights") With {.Descrizione = "La stringa di diritti deve contenere almeno 5 caratteri. pRights= " & pRights}
        End If
        If pRights.Chars(0) = "1" Then _rights = _rights Or CategoryProtocolRightType.InsertFascicle
        If pRights.Chars(1) = "1" Then _rights = _rights Or CategoryProtocolRightType.ModifyFascicle
        If pRights.Chars(2) = "1" Then _rights = _rights Or CategoryProtocolRightType.ViewFascicle
        If pRights.Chars(3) = "1" Then _rights = _rights Or CategoryProtocolRightType.DeleteFascicle
        If pRights.Chars(4) = "1" Then _rights = _rights Or CategoryProtocolRightType.ManageFascicle

    End Sub

    Public Overrides Function ToString() As String

        Dim sb As New StringBuilder()
        sb.Append(If(CanInsertFascicle, "1", "0"))
        sb.Append(If(CanModifyFascicle, "1", "0"))
        sb.Append(If(CanViewFascicle, "1", "0"))
        sb.Append(If(CanDeleteFascicle, "1", "0"))
        sb.Append(If(CanManageFascicle, "1", "0"))
        Return sb.Append("000000000000000").ToString()

    End Function

    Public Overridable Property CanInsertFascicle() As Boolean
        Get
            Return CheckRight(CategoryProtocolRightType.InsertFascicle)
        End Get
        Set(ByVal value As Boolean)
            SetRight(CategoryProtocolRightType.InsertFascicle)
        End Set
    End Property

    Public Overridable Property CanModifyFascicle() As Boolean
        Get
            Return CheckRight(CategoryProtocolRightType.ModifyFascicle)
        End Get
        Set(ByVal value As Boolean)
            SetRight(CategoryProtocolRightType.ModifyFascicle)
        End Set
    End Property

    Public Overridable Property CanViewFascicle() As Boolean
        Get
            Return CheckRight(CategoryProtocolRightType.ViewFascicle)
        End Get
        Set(ByVal value As Boolean)
            SetRight(CategoryProtocolRightType.ViewFascicle)
        End Set
    End Property

    Public Overridable Property CanDeleteFascicle() As Boolean
        Get
            Return CheckRight(CategoryProtocolRightType.DeleteFascicle)
        End Get
        Set(ByVal value As Boolean)
            SetRight(CategoryProtocolRightType.DeleteFascicle)
        End Set
    End Property

    Public Overridable Property CanManageFascicle() As Boolean
        Get
            Return CheckRight(CategoryProtocolRightType.ManageFascicle)
        End Get
        Set(ByVal value As Boolean)
            SetRight(CategoryProtocolRightType.ManageFascicle)
        End Set
    End Property

    Private Function CheckRight(ByVal right As CategoryProtocolRightType) As Boolean
        If _rights = CategoryProtocolRightType.None Then Return False
        If _rights = CategoryProtocolRightType.All Then Return True
        Return _rights = (_rights Or right)
    End Function

    Private Sub SetRight(ByVal right As CategoryProtocolRightType)
        _rights = _rights Or right
    End Sub

    Public Shared Operator Or(ByVal first As CategoryProtocolRights, ByVal second As CategoryProtocolRights) As CategoryProtocolRights
        Dim p As CategoryProtocolRightType
        p = first._rights Or second._rights
        Return New CategoryProtocolRights(p)
    End Operator

End Class

<Flags()> _
Public Enum CategoryProtocolRightType
    None = 0
    InsertFascicle = 1
    ModifyFascicle = InsertFascicle << 1
    ViewFascicle = InsertFascicle << 2
    DeleteFascicle = InsertFascicle << 3
    ManageFascicle = InsertFascicle << 4
    All = (InsertFascicle << 5) - 1
End Enum
