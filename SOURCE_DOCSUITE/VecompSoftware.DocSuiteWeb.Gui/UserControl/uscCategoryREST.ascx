<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscCategoryRest.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscCategoryRest" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        var <%= Me.ClientID %>_uscCategoryRest;
        require(["UserControl/uscCategoryRest"], function (uscCategoryRest) {
            $(function () {
                <%= Me.ClientID %>_uscCategoryRest = new uscCategoryRest(tenantModelConfiguration.serviceConfiguration, <%= ControlConfiguration %>, "<%= Me.ClientID %>");                
                <%= Me.ClientID %>_uscCategoryRest.pnlMainContentId = "<%= pnlMainContent.ClientID%>";
                <%= Me.ClientID %>_uscCategoryRest.actionToolbarId = "<%= actionToolbar.ClientID%>";
                <%= Me.ClientID %>_uscCategoryRest.windowManagerId = "<%= RadWindowManagerCategory.ClientID %>";
                <%= Me.ClientID %>_uscCategoryRest.treeCategoryId = "<%= treeCategory.ClientID %>";
                <%= Me.ClientID %>_uscCategoryRest.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                <%= Me.ClientID %>_uscCategoryRest.anyNodeCheckId = "<%= AnyNodeCheck.ClientID %>";
                <%= Me.ClientID %>_uscCategoryRest.idCategory = <%= IdCategoryToPage %>;
                <%= Me.ClientID %>_uscCategoryRest.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                <%= Me.ClientID %>_uscCategoryRest.windowSelCategoryId = "<%= windowSelCategory.ClientID %>";
                <%= Me.ClientID %>_uscCategoryRest.showProcesses = <%= ShowProcesses.ToString().ToLower() %>;
                <%= Me.ClientID %>_uscCategoryRest.currentTenantAOOId = "<%= CurrentTenant.TenantAOO.UniqueId %>";
                <%= Me.ClientID %>_uscCategoryRest.initialize();
            });
        });
    </script>
</telerik:RadScriptBlock>

<telerik:RadWindowManager EnableViewState="false" ID="RadWindowManagerCategory" runat="server">
    <Windows>
        <telerik:RadWindow ID="windowSelCategory" Height="600" Width="750" runat="server" Title="Selezione classificatore" />
    </Windows>
</telerik:RadWindowManager>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
<telerik:RadPageLayout runat="server" ID="pnlMainContent" CssClass="dsw-panel">
    <Rows>
        <telerik:LayoutRow runat="server" CssClass="dsw-panel-title" ID="rowTitle">
            <Content>Classificazione</Content>
        </telerik:LayoutRow>
        <telerik:LayoutRow runat="server" CssClass="dsw-panel-content">
            <Content>
                <telerik:RadToolBar RenderMode="Lightweight" AutoPostBack="false" CssClass="ToolBarContainer" EnableRoundedCorners="False" EnableShadows="False" ID="actionToolbar" runat="server" Width="100%">
                    <Items>
                        <telerik:RadToolBarButton CausesValidation="False" ImageUrl="~/App_Themes/DocSuite2008/imgset16/drawer_add.png" ToolTip="Selezione classificatore" CommandName="add" />
                        <telerik:RadToolBarButton CausesValidation="False" ImageUrl="~/App_Themes/DocSuite2008/imgset16/drawer_delete.png" ToolTip="Elimina classificazione" CommandName="delete" />
                    </Items>
                </telerik:RadToolBar>
                <telerik:RadTreeView ID="treeCategory" runat="server" Width="100%" />
            </Content>
        </telerik:LayoutRow>
    </Rows>
</telerik:RadPageLayout>

<asp:CustomValidator runat="server" ID="AnyNodeCheck" ControlToValidate="treeCategory" ValidateEmptyText="true" EnableClientScript="true" ClientValidationFunction="anyNodeCheck" Display="Dynamic" ErrorMessage="Campo classificazione obbligatorio" />