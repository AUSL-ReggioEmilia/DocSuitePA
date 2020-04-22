<%@ Page Title="Selezione classificazione" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="CommonSelCategoryRest.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonSelCategoryRest" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var commonSelCategoryREST;
            require(["UserControl/CommonSelCategoryRest"], function (CommonSelCategoryRest) {
                $(function () {
                    commonSelCategoryREST = new CommonSelCategoryRest(tenantModelConfiguration.serviceConfiguration);
                    commonSelCategoryREST.treeViewCategoryId = "<%= TreeViewCategory.ClientID %>";
                    commonSelCategoryREST.txtSearchId = "<%= txtSearch.ClientID %>";
                    commonSelCategoryREST.txtSearchCodeId = "<%= txtSearchCode.ClientID %>";
                    commonSelCategoryREST.btnSearchOnlyFascicolableId = "<%= btnSearchOnlyFascicolable.ClientID %>";
                    commonSelCategoryREST.btnSearchId = "<%= btnSearch.ClientID %>";
                    commonSelCategoryREST.btnConfermaId = "<%= btnConferma.ClientID %>";
                    commonSelCategoryREST.btnSearchCodeId = "<%= btnSearchCode.ClientID %>";
                    commonSelCategoryREST.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    commonSelCategoryREST.pnlMainContentId = "<%= pnlMainContent.ClientID %>";
                    commonSelCategoryREST.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    commonSelCategoryREST.rowOnlyFascicolableId = "<%= rowOnlyFascicolable.ClientID %>";
                    commonSelCategoryREST.fascicleBehavioursEnabled = <%= FascicleBehavioursEnabled.ToString().ToLower() %>;
                    commonSelCategoryREST.manager = "<%= Manager %>";
                    commonSelCategoryREST.secretary = "<%= Secretary %>";
                    commonSelCategoryREST.fascicleType = <%= FascicleTypeToPage %>;
                    commonSelCategoryREST.role = <%= RoleToPage %>;
                    commonSelCategoryREST.container = <%= ContainerToPage %>;
                    commonSelCategoryREST.lblDescriptionId = "<%= lblDescription.ClientID %>";
                    commonSelCategoryREST.pnlDescriptionId = "<%= pnlDescription.ClientID %>";
                    commonSelCategoryREST.parentId = <%= ParentIdToPage %>;
                    commonSelCategoryREST.includeParentDescendants = <%= IncludeParentDescendants.ToString().ToLower() %>;
                    commonSelCategoryREST.initialize();
                });
            });

            function treeView_ClientNodeClicked(sender, args) {
                commonSelCategoryREST.treeViewCategory_ClientNodeClicked(sender, args);
            }

            function treeView_ClientNodeExpanding(sender, args) {
                commonSelCategoryREST.treeViewCategory_ClientNodeExpanding(sender, args);
            }
        </script>
    </telerik:RadScriptBlock>

    <asp:Panel runat="server" ID="pnlDescription" CssClass="dsw-panel">
        <div class="dsw-panel-title">
            <asp:Label ID="lblDescription" runat="server"></asp:Label>
        </div>
    </asp:Panel>
    <table id="tblHeader" width="100%" class="dataform">
        <tr>
            <td class="label col-dsw-2">Descrizione:
            </td>
            <td class="col-dsw-8">
                <asp:Panel DefaultButton="btnSearch" ID="pnlSearch" runat="server" Style="display: inline;">
                    <asp:TextBox ID="txtSearch" runat="server" Width="300px" />
                </asp:Panel>
                <asp:Button ID="btnSearch" runat="server" Text="Cerca" ToolTip="Ricerca per Descrizione" />
            </td>
        </tr>
        <tr>
            <td class="label col-dsw-2">Codice:
            </td>
            <td class="col-dsw-8">
                <asp:Panel runat="server" ID="pnlSearchCode" DefaultButton="btnSearchCode" Style="display: inline;">
                    <asp:TextBox ID="txtSearchCode" MaxLength="20" runat="server" Width="150px" />
                </asp:Panel>
                <asp:Button ID="btnSearchCode" runat="server" Text="Cerca e Seleziona" ToolTip="Ricerca per Codice" />
            </td>
        </tr>
        <tr id="rowOnlyFascicolable" runat="server">
            <td class="label col-dsw-2">Visualizza:</td>
            <td class="col-dsw-8">
                <telerik:RadButton Text="Piano Fascicolazione" runat="server" AutoPostBack="false" ToggleType="CheckBox" Checked="false" ID="btnSearchOnlyFascicolable" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
    <asp:Panel runat="server" Height="100%" Width="100%" ID="pnlMainContent">
        <telerik:RadTreeView ID="TreeViewCategory" LoadingStatusPosition="BeforeNodeText" runat="server" OnClientNodeClicked="treeView_ClientNodeClicked"
            OnClientNodeExpanding="treeView_ClientNodeExpanding" ShowLineImages="true" Width="100%" Height="100%">
            <Nodes>
                <telerik:RadTreeNode Expanded="true" Font-Bold="true" runat="server" Selected="true" Text="Classificatore" />
            </Nodes>
        </telerik:RadTreeView>
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Button ID="btnConferma" runat="server" Text="Conferma Selezione"></asp:Button>
</asp:Content>
