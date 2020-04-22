Imports System

<Serializable()> _
Public Class PecMailBoxRoleCompositeKey
    Implements IComparable

#Region "Properties"

    Public Overridable Property RoleId() As Integer

    Public Overridable Property PECMailBoxId() As Short

#End Region

#Region " Constructor "

    Public Sub New()
    End Sub

    Public Sub New(ByVal role As Role, ByVal mailBox As PECMailBox)
        RoleId = role.Id
        PECMailBoxId = mailBox.Id
    End Sub

#End Region

#Region " Methods "

    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        Dim compareTo As PecMailBoxRoleCompositeKey = DirectCast(obj, PecMailBoxRoleCompositeKey)
        Return RoleId.Equals(compareTo.RoleId) And PECMailBoxId.Equals(compareTo.PECMailBoxId)
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return Me.ToString().GetHashCode()
    End Function

    Public Overrides Function ToString() As String
        Return RoleId.ToString() + "/" + PECMailBoxId.ToString()
    End Function

    Public Function CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
        Return Me.ToString().CompareTo(obj.ToString())
    End Function

#End Region

End Class
