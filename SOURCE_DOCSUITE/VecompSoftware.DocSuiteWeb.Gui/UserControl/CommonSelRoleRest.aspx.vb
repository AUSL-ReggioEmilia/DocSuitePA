Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class CommonSelRoleRest
    Inherits CommBasePage

#Region " Properties "
    Protected ReadOnly Property MultiTenantEnabled As Boolean
        Get
            Return DocSuiteContext.Current.ProtocolEnv.MultiTenantEnabled
        End Get
    End Property
    Protected ReadOnly Property MultipleRolesEnabled As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean)("MultipleRoles", True)
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