Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Entity.Tenants

Public Class uscContattiSelRest
    Inherits DocSuite2008BaseControl

#Region "Properties"
    Public ReadOnly Property PanelContent As Panel
        Get
            Return pnlContent
        End Get
    End Property

    Public Property Required As Boolean
        Get
            Return AnyNodeCheck.Enabled
        End Get
        Set(ByVal value As Boolean)
            AnyNodeCheck.Enabled = value
        End Set
    End Property

    Public Property FilterByParentId As Integer?

    Public ReadOnly Property CurrentTenant As Tenant
        Get
            Return CType(Session("CurrentTenant"), Tenant)
        End Get
    End Property

    Public Property AddAllDataButtonVisibility As Boolean

    Public Property RemoveAllDataButtonVisibility As Boolean
    Public Property FilterByTenantEnabled As Boolean = True

    Public Property ConfirmAndNewEnabled As Boolean = True

    Public Property CreateManualContactEnabled As Boolean = True
    Public Property ToolbarVisible As Boolean = True
#End Region
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        tbContactsControl.Visible = ToolbarVisible
        uscContactRest.FilterByParentId = FilterByParentId
        uscContactRest.FilterByTenantEnabled = FilterByTenantEnabled
        uscContactRest.CreateManualContactEnabled = CreateManualContactEnabled
    End Sub

End Class