Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data

Public Class RoleEventArgs
    Inherits EventArgs

#Region " Properties "
    Public Property Role As Role
    Public Property Cancel As Boolean
    Public Property IsPersistent As Boolean
#End Region

#Region " Constructor "

    Public Sub New(ByVal role As Role)
        _Role = role
    End Sub

#End Region

End Class

