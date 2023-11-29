<%@ Page Title="Selezionare Settori" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="CommonSelRoleRest.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonSelRoleRest" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var commonSelRoleRest;
            require(["UserControl/CommonSelRoleRest"], function (CommonSelRoleRest) {
                $(function () {
                    commonSelRoleRest = new CommonSelRoleRest(tenantModelConfiguration.serviceConfiguration);
                    commonSelRoleRest.pnlMainContentId = "<%= pnlMainContent.ClientID %>";
                    commonSelRoleRest.rolesTreeId = "<%= RolesTreeView.ClientID %>";
                    commonSelRoleRest.onlyMyRoles = <%= OnlyMyRoles.ToString().ToLower() %>;
                    commonSelRoleRest.descriptionSearchBtnId = "<%= btnSearch.ClientID %>";
                    commonSelRoleRest.descriptionFilterInputId = "<%= descriptionFilterInput.ClientID %>";
                    commonSelRoleRest.codeSearchBtnId = "<%= btnSearchCode.ClientID %>";
                    commonSelRoleRest.codeSearchFilterInputId = "<%= codeFilterInput.ClientID %>";
                    commonSelRoleRest.confirmSelectionBtnId = "<%= btnConferma.ClientID %>";
                    commonSelRoleRest.selectAllBtnId = "<%= SelectAllBtn.ClientID %>";
                    commonSelRoleRest.unselectAllBtnId = "<%= UnselectAllBtn.ClientID %>";
                    commonSelRoleRest.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    commonSelRoleRest.multipleRolesEnabled = "<%= MultipleRolesEnabled %>".toLowerCase();
                    commonSelRoleRest.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>";
                    commonSelRoleRest.entityType = "<%= EntityType %>";
                    commonSelRoleRest.entityId = "<%= EntityId %>";
                    commonSelRoleRest.idTenantAOO = "<%= IdTenantAOO %>";
                    commonSelRoleRest.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>

    <table width="100%" class="dataform">
        <tr>
            <td class="label" style="width: 20%">
                Descrizione:
            </td>
            <td>
                <asp:Panel runat="server" ID="pnlCerca" DefaultButton="btnSearch" Style="display: inline;">
                    <asp:TextBox ID="descriptionFilterInput" runat="server" Width="250px" />
                </asp:Panel>
                <telerik:RadButton ID="btnSearch" Text="Cerca" runat="server" ToolTip="Ricerca per Descrizione" AutoPostBack="False" />
            </td>
        </tr>
        <tr>
            <td class="label" style="width: 20%">
                Codice Ricerca:&nbsp;
            </td>
            <td>
                <asp:Panel runat="server" ID="pnlSearchCode" DefaultButton="btnSearchCode" Style="display: inline;">
                    <asp:TextBox ID="codeFilterInput" MaxLength="10" runat="server" Width="280px" />
                </asp:Panel>
                <telerik:RadButton ID="btnSearchCode" Text="Cerca e Seleziona" runat="server" ToolTip="Ricerca per Codice e Seleziona" AutoPostBack="False" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
    <asp:Panel runat="server" Height="100%" Width="100%" ID="pnlMainContent" Style="overflow: hidden;">
        <telerik:RadButton ID="SelectAllBtn" Text="Seleziona tutti" runat="server" AutoPostBack="False" />
        <telerik:RadButton ID="UnselectAllBtn" Text="Deseleziona tutti" runat="server" AutoPostBack="False" />
        <telerik:RadTreeView ID="RolesTreeView" 
                             EnableViewState="false" 
                             runat="server" 
                             Width="100%" Height="100%"/>
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <telerik:RadButton ID="btnConferma" Text="Conferma Selezione" runat="server" AutoPostBack="False"/>
</asp:Content>