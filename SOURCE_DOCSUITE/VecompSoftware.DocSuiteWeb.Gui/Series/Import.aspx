<%@ Page Title="Importazione da EXCEL" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="Import.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.Series.Import" %>

<%@ Register Src="~/UserControl/uscDocumentUpload.ascx" TagPrefix="usc" TagName="UploadDocument" %>
<%@ Register TagPrefix="usc" TagName="SelCategory" Src="~/UserControl/uscClassificatore.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server" EnableViewState="false">
        <script language="javascript" type="text/javascript">

            function getAjaxManager() {
                return $find("<%= RadAjaxManager.GetCurrent(Page).ClientID%>");
            }

            // Metodo per il lancio di una Request
            function AjaxRequest(manager, request) {
                manager.ajaxRequest(request);
            }

            function openConfirmWindow(message) {
                radalert(message, 300, 100, 'Info', alertCallBackFn, "../App_Themes/DocSuite2008/imgset32/information.png");
            }

            function alertCallBackFn(arg) {
                // PlaceHolder per azioni da intraprendere dopo la chiusura dell'avviso di importazione
                // AjaxRequest(getAjaxManager(), "MailSender_Confirm");
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel runat="server" ID="MainPanel">
        <table class="datatable">
            <tr>
                <th colspan="2">Opzioni di importazione</th>
            </tr>
            <tr>
                <td class="label col-dsw-2">
                    <asp:Label ID="DocumentSeries" runat="server" />
                </td>
                <td>
                    <asp:DropDownList AutoPostBack="True" CausesValidation="false" ID="ddlDocumentSeries" runat="server" Visible="True" Width="300px" />
                    <asp:RequiredFieldValidator ControlToValidate="ddlDocumentSeries" Display="Dynamic" ErrorMessage="Campo Tipo Documento Obbligatorio" ID="rfvIdDocType" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="label">Stato:</td>
                <td>
                    <asp:CheckBox runat="server" ID="cbDraft" Text="Mantieni in stato bozza" />
                </td>
            </tr>
            <asp:Panel ID="pnlPublishingDate" runat="server">
                <tr>
                    <td class="label">Data pubblicazione:</td>
                    <td>
                        <telerik:RadDatePicker runat="server" ID="ItemPublishingDate" />
                    </td>
                </tr>
            </asp:Panel>
            <tr>
                <td class="label">Categoria:</td>
                <td>
                    <usc:SelCategory runat="server" ID="ItemCategory" Type="Series" HeaderVisible="False" Multiple="false" />
                </td>
            </tr>
        </table>

        <table class="datatable">
            <tr>
                <th colspan="2">Opzioni Documento</th>
            </tr>
            <tr>
                <td class="label col-dsw-2">Documento da importare:
                </td>
                <td>
                    <usc:UploadDocument AllowedExtensions=".xls" ButtonPreviewEnabled="False" ButtonScannerEnabled="False" HeaderVisible="False" ID="uscDocument" IsDocumentRequired="True" MultipleDocuments="False" runat="server" SignButtonEnabled="false" />
                </td>
            </tr>
            <tr>
                <td class="label">Campi del documento:
                </td>
                <td>
                    <asp:Label runat="server" ID="lblFields" />
                </td>
            </tr>
            <tr>
                <td></td>
                <td>
                    <ul class="warningArea">
                        <li>Verificare che i Campi corrispondano alle intestazioni delle colonne</li>
                        <li>Verificare che le celle dei Campi di tipo data siano nel formato corretto</li>
                        <li>Se il file è troppo grande la pagina potrebbe andare in errore, ma l'importazione avverrà correttamente</li>
                    </ul>
                </td>
            </tr>
        </table>

    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Button runat="server" Text="Importa registrazioni" ID="cmdImport" />
</asp:Content>
