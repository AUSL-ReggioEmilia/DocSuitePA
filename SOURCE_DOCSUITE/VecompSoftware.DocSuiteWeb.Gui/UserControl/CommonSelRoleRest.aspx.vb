Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class CommonSelRoleRest
    Inherits CommBasePage

#Region " Properties "
    Protected ReadOnly Property OnlyMyRoles As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean)("OnlyMyRoles", False)
        End Get
    End Property
    Protected ReadOnly Property MultipleRolesEnabled As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean)("MultipleRoles", True)
        End Get
    End Property
    Protected ReadOnly Property EntityType As String
        Get
            Return Request.QueryString.GetValueOrDefault(Of String)("EntityType", String.Empty)
        End Get
    End Property
    Protected ReadOnly Property EntityId As String
        Get
            Return Request.QueryString.GetValueOrDefault(Of String)("EntityId", String.Empty)
        End Get
    End Property
    Protected ReadOnly Property IdTenantAOO As String
        Get
            Return Request.QueryString.GetValueOrDefault(Of String)("IdTenantAOO", CurrentTenant.TenantAOO.UniqueId.ToString())
        End Get
    End Property
#End Region
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False
        SelectAllBtn.Visible = MultipleRolesEnabled
        UnselectAllBtn.Visible = MultipleRolesEnabled
        RolesTreeView.CheckBoxes = MultipleRolesEnabled
    End Sub

End Class