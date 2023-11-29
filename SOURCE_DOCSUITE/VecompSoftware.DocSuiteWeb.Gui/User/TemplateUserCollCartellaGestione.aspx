<%@ Page Title="Template Cartella Collaborazione - Gestione" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="TemplateUserCollCartellaGestione.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TemplateUserCollCartellaGestione" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>


<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var templateUserCollGestione;
            require(["User/TemplateUserCollCartellaGestione"], function (TemplateUserCollCartellaGestione) {
                $(function () {
                    templateUserCollGestione = new TemplateUserCollCartellaGestione(tenantModelConfiguration.serviceConfiguration);
                    templateUserCollGestione.btnConfirmId = "<%= btnConfirm.ClientID %>";
                    templateUserCollGestione.btnDeleteId = "<%= btnDelete.ClientID %>";
                    templateUserCollGestione.txtNameId = "<%= txtName.ClientID %>";
                    templateUserCollGestione.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    templateUserCollGestione.action = "<%= Action %>";
                    templateUserCollGestione.templateId = "<%= TemplateId %>";
                    templateUserCollGestione.parentId = "<%= ParentId %>";
                    templateUserCollGestione.pnlHeaderId = "<%= pnlHeader.ClientID %>";
                    templateUserCollGestione.pnlButtonsId = "<%= pnlButtons.ClientID %>";
                    templateUserCollGestione.ajaxManagerId = "<%= MasterDocSuite.AjaxManager.ClientID %>";
                    templateUserCollGestione.radWindowManagerId = "<%= MasterDocSuite.DefaultWindowManager.ClientID %>";
                    templateUserCollGestione.ajaxFlatLoadingPanelId = "<%= MasterDocSuite.AjaxFlatLoadingPanel.ClientID%>";
                    templateUserCollGestione.initialize();
                });
            });

            function changeStrWithValidCharacterHandler(sender, args) {
                templateUserCollGestione.changeStrWithValidCharacterHandler(sender, args);
            }
        </script>
    </telerik:RadScriptBlock>
    <asp:Panel runat="server" ID="pnlHeader">
        <table class="datatable">
            <tr>
                <td class="label col-dsw-2">Nome:
                </td>
                <td>
                    <telerik:RadTextBox runat="server" ID="txtName" Width="400px"></telerik:RadTextBox>
                    <asp:RequiredFieldValidator ControlToValidate="txtName" Display="Dynamic" ErrorMessage="Campo nome obbligatorio" ID="rfvName" runat="server" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="pnlButtons">
        <telerik:RadButton runat="server" ID="btnConfirm" Width="150px" AutoPostBack="false" Text="Salva "></telerik:RadButton>
        <%--Delete folder functionality is not yet implemented in WebApi, but api functionality with tree refresh is implemented
            We will render visible the button when we implement the web api functionality--%>
        <telerik:RadButton runat="server" ID="btnDelete" Enabled="false" Visible="false" Width="150px" AutoPostBack="false" Text="Elimina"></telerik:RadButton>
    </asp:Panel>
</asp:Content>
