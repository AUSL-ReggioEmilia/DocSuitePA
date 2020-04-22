<%@ Page Title="Template Collaborazione - Gestione" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="TemplateUserCollGestione.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TemplateUserCollGestione" %>

<%@ Register Src="~/UserControl/uscDocumentUpload.ascx" TagName="DocumentUpload" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagName="ContattiSel" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="Settori" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var templateUserCollGestione;
            require(["User/TemplateUserCollGestione"], function (TemplateUserCollGestione) {
                $(function () {
                    templateUserCollGestione = new TemplateUserCollGestione(tenantModelConfiguration.serviceConfiguration);
                    templateUserCollGestione.btnConfirmId = "<%= btnConfirm.ClientID %>";
                    templateUserCollGestione.btnPublishId = "<%= btnPublish.ClientID %>";
                    templateUserCollGestione.btnConfirmUniqueId = "<%= btnConfirm.UniqueID %>";
                    templateUserCollGestione.btnPublishUniqueId = "<%= btnPublish.UniqueID %>";
                    templateUserCollGestione.pnlMainPanelId = "<%= pnlMainPanel.ClientID %>";
                    templateUserCollGestione.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>";
                    templateUserCollGestione.ajaxFlatLoadingPanelId = "<%= MasterDocSuite.AjaxFlatLoadingPanel.ClientID%>";
                    templateUserCollGestione.ajaxManagerId = "<%= MasterDocSuite.AjaxManager.ClientID %>";
                    templateUserCollGestione.txtNameId = "<%= txtName.ClientID %>";
                    templateUserCollGestione.txtObjectId = "<%= txtObject.ClientID %>";
                    templateUserCollGestione.txtNoteId = "<%= txtNote.ClientID %>";
                    templateUserCollGestione.ddlDocumentTypeId = "<%= ddlDocumentType.ClientID %>";
                    templateUserCollGestione.ddlDocumentTypeUniqueId = "<%= ddlDocumentType.UniqueID %>";
                    templateUserCollGestione.ddlSpecificDocumentTypeId = "<%= ddlSpecificDocumentType.ClientID %>";
                    templateUserCollGestione.rblPriorityId = "<%= rblPriority.ClientID %>";
                    templateUserCollGestione.action = "<%= Action %>";
                    templateUserCollGestione.templateId = "<%= TemplateId %>";
                    templateUserCollGestione.pnlHeaderId = "<%= pnlHeader.ClientID %>";
                    templateUserCollGestione.pnlButtonsId = "<%= pnlButtons.ClientID %>";
                    templateUserCollGestione.selectSignerOrderId = "<%= selectSignerOrder.ClientID %>";
                    templateUserCollGestione.deletableSignerId = "<%= deletableSigner.ClientID %>";
                    templateUserCollGestione.btnDeleteId = "<%= btnDelete.ClientID %>";
                    templateUserCollGestione.radWindowManagerId = "<%= MasterDocSuite.DefaultWindowManager.ClientID %>";
                    templateUserCollGestione.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
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
            <tr>
                <td class="label col-dsw-2">Tipologia Documento:
                </td>
                <td>
                    <telerik:RadDropDownList AutoPostBack="false" ID="ddlDocumentType" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="ddlDocumentType" Display="Dynamic" ErrorMessage="Campo Tipologia Documento Obbligatorio" ID="rfvDocumentType" runat="server" />
                </td>
            </tr>
            <tr id="specificTypeRow" style="display: none;">
                <td class="label col-dsw-2">Seleziona tipologia specifica:
                </td>
                <td>
                    <telerik:RadDropDownList AutoPostBack="false" ID="ddlSpecificDocumentType" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="label col-dsw-2">Priorità:</td>
                <td>
                    <asp:RadioButtonList ID="rblPriority" RepeatDirection="Horizontal" runat="server">
                        <asp:ListItem Selected="True" Text="Normale" Value="N" />
                        <asp:ListItem Text="Bassa" Value="B" />
                        <asp:ListItem Text="Alta" Value="A" />
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr style="visibility:collapse">
                <td class="label col-dsw-2">Firmatari eliminabili:</td>
                <td>
                    <asp:CheckBox runat="server" ID="deletableSigner" />
                </td>
            </tr>
            <tr style="visibility:collapse">
                <td class="label col-dsw-2">Firmatari aggiunti in testa:</td>
                <td>
                    <asp:CheckBox runat="server" ID="selectSignerOrder" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <div class="col-dsw-10" runat="server" id="pnlMainPanel">
        <%--<table class="datatable">
            <tr>
                <th style="text-align: left">Documento
                    <asp:Label ID="lblDocumento" runat="server" />
                </th>
            </tr>
            <tr>
                <td>
                    <usc:DocumentUpload ButtonPreviewEnabled="True" ID="uscDocumento" HeaderVisible="false" runat="server" IsDocumentRequired="false" MultipleDocuments="False" />
                </td>
            </tr>
        </table>
        <table class="datatable">
            <tr>
                <th style="text-align: left">Allegati (parte integrante)</th>
            </tr>
            <tr>
                <td>
                    <usc:DocumentUpload ButtonPreviewEnabled="True" ID="uscAllegati" HeaderVisible="false" IsDocumentRequired="false" runat="server" />
                </td>
            </tr>
        </table>
        <table class="datatable">
            <tr>
                <th style="text-align: left">Annessi (non parte integrante)</th>
            </tr>
            <tr>
                <td>
                    <usc:DocumentUpload ButtonPreviewEnabled="True" ID="uscAnnexed" HeaderVisible="false" IsDocumentRequired="false" runat="server" />
                </td>
            </tr>
        </table>--%>
        <table class="datatable">
            <tr>
                <th style="text-align: left;">Settori proponenti</th>
            </tr>
            <tr>
                <td>
                    <usc:Settori Caption="Segreterie" Checkable="True" HeaderVisible="false" ID="uscAuthorizedRoles" MultiSelect="true" Required="False" runat="server" />
                </td>
            </tr>
        </table>
        <table class="datatable">
            <tr>
                <th colspan="2" style="text-align: left">Destinatari Visione/Firma</th>
            </tr>
            <tr>
                <td class="label col-dsw-2">Destinatari:
                </td>
                <td class="col-dsw-8">
                    <usc:ContattiSel ButtonDeleteVisible="true" ButtonImportVisible="false" ButtonIPAVisible="false" ButtonManualVisible="false" ButtonPropertiesVisible="false"
                        ButtonRoleVisible="true" ButtonSelectOChartVisible="false" ButtonSelectVisible="false" EnableCheck="True" EnableViewState="true" CollaborationType="P"
                        HeaderVisible="false" ID="uscVisioneFirma" IsRequired="false" Multiple="true" runat="server" TreeViewCaption="Destinatari" UseAD="true" />
                </td>
            </tr>
        </table>
        <asp:Panel ID="pnlRestituzioni" runat="server">
            <table class="datatable">
                <tr>
                    <th colspan="4">Destinatari per
                        <asp:Label ID="lblDestinatari" runat="server" />
                    </th>
                </tr>
                <tr>
                    <td class="label col-dsw-1">Segreterie:</td>
                    <td class="col-dsw-4 dsw-vertical-middle">
                        <usc:Settori Caption="Segreterie" Checkable="True" HeaderVisible="false" ID="uscSettoriSegreterie" MultipleRoles="True" MultiSelect="true" Required="False" runat="server" />
                    </td>
                    <td class="label col-dsw-1">
                        <asp:Literal ID="lblOtherRecipients" runat="server" Text="Altri Destinatari:" />
                    </td>
                    <td class="col-dsw-4 dsw-vertical-middle">
                        <usc:ContattiSel ButtonDeleteVisible="true" ButtonImportVisible="false" ButtonIPAVisible="false" ButtonManualVisible="false" ButtonPropertiesVisible="false" ButtonSelectOChartVisible="False" ButtonSelectVisible="false" EnableCheck="true" EnableViewState="true" HeaderVisible="false" ID="uscRestituzioni" IsRequired="false" Multiple="true" runat="server" TreeViewCaption="Destinatari" UseAD="true" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <table class="datatable">
            <tr>
                <th colspan="2">Dati</th>
            </tr>
            <tr>
                <td class="label col-dsw-2">Oggetto:</td>
                <td class="col-dsw-8">
                    <telerik:RadTextBox ID="txtObject" Rows="3" runat="server" TextMode="MultiLine" Width="100%">
                        <ClientEvents OnBlur="changeStrWithValidCharacterHandler"></ClientEvents>
                    </telerik:RadTextBox>
                </td>
            </tr>
            <tr>
                <td class="label col-dsw-2">Note:</td>
                <td class="col-dsw-8">
                    <telerik:RadTextBox runat="server" MaxLength="255" Width="100%" ID="txtNote">
                        <ClientEvents OnBlur="changeStrWithValidCharacterHandler"></ClientEvents>
                    </telerik:RadTextBox>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="pnlButtons">
        <telerik:RadButton runat="server" ID="btnConfirm" Width="150px" AutoPostBack="false" Text="Salva Bozza"></telerik:RadButton>
        <telerik:RadButton runat="server" ID="btnPublish" Width="150px" AutoPostBack="false" Text="Pubblica"></telerik:RadButton>
        <telerik:RadButton runat="server" ID="btnDelete" Width="150px" AutoPostBack="false" Text="Elimina"></telerik:RadButton>
    </asp:Panel>
</asp:Content>
