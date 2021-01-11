<%@ Page Title="Rubrica AUS" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="CommonSelAUSContact.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonSelAUSContact" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var commonSelAUSContact;
            require(["UserControl/CommonSelAUSContact"], function (CommonSelAUSContact) {
                $(function () {
                    commonSelAUSContact = new CommonSelAUSContact(tenantModelConfiguration.serviceConfiguration);
                    commonSelAUSContact.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                    commonSelAUSContact.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    commonSelAUSContact.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    commonSelAUSContact.chkSubjectTypeId = "<%= chkSubjectType.ClientID %>";
                    commonSelAUSContact.txtSearchId = "<%= txtSearch.ClientID %>";
                    commonSelAUSContact.btnSearchId = "<%= btnSearch.ClientID %>";
                    commonSelAUSContact.txtCodeSearchId = "<%= txtCodeSearch.ClientID %>";
                    commonSelAUSContact.btnCodeSearchId = "<%= btnCodeSearch.ClientID %>";
                    commonSelAUSContact.rtvAUSContactsId = "<%= rtvAUSContacts.ClientID %>";
                    commonSelAUSContact.btnConfirmId = "<%= btnConfirm.ClientID %>";
                    commonSelAUSContact.callerId = "<%= CallerID %>";
                    commonSelAUSContact.btnConfirmAndNewId = "<%= btnConfirmAndNew.ClientID %>";
                    commonSelAUSContact.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <table class="datatable">
        <tr>
            <td class="label labelPanel" style="width: 30%;">Denominazione:
            </td>
            <td>
                <telerik:RadTextBox ID="txtSearch" runat="server" Width="200px" />
                <telerik:RadButton ID="btnSearch" runat="server" Text="Ricerca" AutoPostBack="false" />
            </td>
        </tr>
        <tr>
            <td class="label labelPanel" style="width: 30%;">Codice:
            </td>
            <td>
                <telerik:RadTextBox ID="txtCodeSearch" runat="server" Width="200px" />
                <telerik:RadButton ID="btnCodeSearch" runat="server" Text="Ricerca per codice" AutoPostBack="false" />
            </td>
        </tr>
        <tr>
            <td class="label labelPanel" style="width: 30%;">Tipo di soggetto:
            </td>
            <td>
                <asp:RadioButtonList runat="server" ID="chkSubjectType" RepeatDirection="Horizontal">
                    <asp:ListItem runat="server" Text="Persone fisiche" Value="NaturalPersons" Selected="true" />
                    <asp:ListItem runat="server" Text="Operatori economici" Value="EconomicOperators" />
                </asp:RadioButtonList>
            </td>
        </tr>
    </table>
    <telerik:RadTreeView ID="rtvAUSContacts" runat="server">
        <Nodes>
            <telerik:RadTreeNode Text="Contatti" />
        </Nodes>
    </telerik:RadTreeView>
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="pnlButtons">
        <telerik:RadButton runat="server" ID="btnConfirm" AutoPostBack="false" Text="Conferma" Width="150px" />
        <telerik:RadButton runat="server" ID="btnConfirmAndNew" AutoPostBack="false" Text="Conferma e Nuovo" Width="150px" />
    </asp:Panel>
</asp:Content>
