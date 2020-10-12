Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class ProcessesTreeView
    Inherits CommBasePage

#Region "Properties"
    Protected ReadOnly Property ShowOnlyHasRight As Boolean
        Get
            Dim _showOnlyHasRight As Boolean = Request.QueryString.GetValueOrDefault("ShowOnlyHasRight", True)
            Return _showOnlyHasRight
        End Get
    End Property
#End Region

#Region "Methods"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ActionsToolbar.Visible = CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupProcessesViewsManageableRight
    End Sub
#End Region
End Class