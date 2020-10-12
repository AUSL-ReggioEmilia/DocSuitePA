<%@ Page Language="vb" Title="CommonSetiContactSel" AutoEventWireup="false"  MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="CommonSetiContactSel.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonSetiContactSel" %>


<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var commonSetiContactSel;
            require(["UserControl/CommonSetiContactSel"], function (CommonSetiContactSel) {
                $(function () {
                   commonSetiContactSel = new CommonSetiContactSel(tenantModelConfiguration.serviceConfiguration);
                   commonSetiContactSel.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                   commonSetiContactSel.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                   commonSetiContactSel.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                   commonSetiContactSel.txtSearchId = "<%= txtSearch.ClientID %>";
                   commonSetiContactSel.btnSearchId = "<%= btnSearch.ClientID %>";
                   commonSetiContactSel.rtvSetiContactId = "<%= rtvSetiContact.ClientID %>";
                   commonSetiContactSel.btnConfirmId = "<%= btnConfirm.ClientID %>";
                   commonSetiContactSel.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <table class="datatable">
        <tr>
            <td class="label labelPanel" style="width: 30%;">Utente:
            </td>
            <td>
                <telerik:RadTextBox ID="txtSearch" runat="server" Width="200px" />
                <telerik:RadButton ID="btnSearch" runat="server" Text="Ricerca" AutoPostBack="false" />
            </td>
        </tr>
       
       
    </table>
    <telerik:RadTreeView ID="rtvSetiContact" runat="server">
        <Nodes>
            <telerik:RadTreeNode Text="Contatti" />
        </Nodes>
    </telerik:RadTreeView>
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="pnlButtons">
        <telerik:RadButton runat="server" ID="btnConfirm" AutoPostBack="false" Text="Conferma" Width="150px" />
    </asp:Panel>
</asp:Content>
