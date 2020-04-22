<%@ Page Title="Importazione contatti da Excel" Language="vb" ValidateRequest="false" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="CommonExcelImportContact.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonExcelImportContact" %>
<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            function ReturnValuesJson(serializedContact, close) {
                if (close == "true") {
                    CloseWindow(serializedContact);
                    return;
                }
                GetRadWindow().BrowserWindow.<%= CallerId%>_CloseManualMulti(serializedContact);
            }

            function CloseWindow(contact) {
                GetRadWindow().close(contact);
            }

            function btnClose_OnClick(sender, args) {
                $find("<%= AjaxManager.ClientID %>").ajaxRequestWithTarget('<%= btnClose.UniqueID %>', '');
                return false;
            }

            function btnIgnore_OnClick(sender, args) {
                $find("<%= AjaxManager.ClientID %>").ajaxRequestWithTarget('<%= btnIgnore.UniqueID %>', '');
                return false;
            }

            function CommonExcelImportContactSend(val) {
                $find("<%= AjaxManager.ClientID %>").ajaxRequest(val);
                return false;
            }

            function pageLoad(sender, eventArgs) {
                if (!eventArgs.get_isPartialLoad()) {
                    CommonExcelImportContactSend("importContacts");
                }
            }

        </script>
    </telerik:RadScriptBlock>
    <style type="text/css">
        .prot td.label { background-color: lightgray !important; }
    </style>
</asp:Content>
<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadAjaxPanel runat="server" ID="ajaxPanel" Height="100%">
        <asp:Table class="datatable" runat="server" ID="errorValidationTable"></asp:Table>
    </telerik:RadAjaxPanel>
   
</asp:Content>
<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <telerik:RadButton ID="btnClose" runat="server" Text="Annulla" AutoPostBack="false" OnClientClicked="btnClose_OnClick"/>
    <telerik:RadButton ID="btnIgnore" runat="server" Text="Ignora e continua" AutoPostBack="false" OnClientClicked="btnIgnore_OnClick"/>
</asp:Content>
