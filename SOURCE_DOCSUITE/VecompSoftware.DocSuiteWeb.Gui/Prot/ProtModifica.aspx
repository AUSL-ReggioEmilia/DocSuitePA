<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtModifica" CodeBehind="ProtModifica.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Protocollo - Modifica" %>

<%@ Register Src="../UserControl/uscDocumentUpload.ascx" TagName="uscDocumentUpload" TagPrefix="uc" %>
<%@ Register Src="../UserControl/uscClassificatore.ascx" TagName="uscClassificatore" TagPrefix="uc" %>
<%@ Register Src="../UserControl/uscProtocollo.ascx" TagName="uscProtocollo" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagName="uscContatti" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscContattiSelText.ascx" TagName="uscContattiText" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscProtocolObjectChanger.ascx" TagName="uscObjectChanger" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscServiceCategory.ascx" TagPrefix="usc" TagName="SelServiceCategory" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">

    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript">

            function ShowLoadingPanel(timeout) {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= pnlMainContent.ClientID%>";
                currentLoadingPanel.show(currentUpdatedControl);

                var ajaxFlatLoadingPanel = $find("<%= MasterDocSuite.AjaxFlatLoadingPanel.ClientID%>");
                var pnlButtons = "<%= pnlButtons.ClientID%>";
                ajaxFlatLoadingPanel.show(pnlButtons);
                if (timeout !== undefined && timeout !== "" && Number.isInteger(timeout)) {
                    setTimeout(function () {
                        var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                        var pnlButtons = "<%= pnlButtons.ClientID%>";
                        var currentUpdatedControl = "<%= pnlMainContent.ClientID%>";
                        currentLoadingPanel.hide(currentUpdatedControl);
                        ajaxFlatLoadingPanel.hide(pnlButtons);
                    }, timeout);
                }
            }
            function confirmSetStatus() {
                var protocolStatusConfirmRequest = <%= protocolStatusConfirm%>;
                var cmbProtocolStatus = document.getElementById("<%=cmbProtocolStatus.ClientID %>");
                if (protocolStatusConfirmRequest.length > 0 && cmbProtocolStatus != null) {
                    var selectedValue = cmbProtocolStatus.value;
                    var selectedText = cmbProtocolStatus.options[cmbProtocolStatus.selectedIndex].text;
                    if (protocolStatusConfirmRequest.indexOf(selectedValue) == -1 || confirm("Sei sicuro di voler impostare il valore dello stato di protocollo a '" + selectedText + "'?")) {
                        ShowLoadingPanel();
                        return true;
                    } else {
                        return false;
                    }
                }
                ShowLoadingPanel();
                return true;
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel runat="server" ID="pnlMainContent" Style="height: 100%">
        <%--protocollo--%>
        <uc:uscProtocollo ID="uscProtocollo" runat="server" />
        <%--documenti--%>
        <uc:uscDocumentUpload ID="uscDocumento" IsDocumentRequired="false" ReadOnly="true" runat="server" />
        <uc:uscDocumentUpload ID="uscAllegati" IsDocumentRequired="false" MultipleDocuments="true" ReadOnly="true" runat="server" />
        <uc:uscDocumentUpload ID="uscAnnexes" IsDocumentRequired="false" MultipleDocuments="true" Prefix="" runat="server" ButtonRemoveEnabled="false" />
        <%--contenitore--%>
        <table id="tblEditContenitore" runat="server" class="datatable">
            <tr>
                <th colspan="2">Contenitore</th>
            </tr>
            <tr>
                <td class="label" style="width: 15%;">Contenitore</td>
                <td style="width: 85%;">
                    <telerik:RadComboBox AutoPostBack="false" CausesValidation="false" EnableLoadOnDemand="true" ID="rcbContainer" ItemRequestTimeout="500" MarkFirstMatch="true" runat="server" Visible="false" Width="300px" />
                    <asp:DropDownList AutoPostBack="true" ID="cmbContainer" runat="server" />
                    &nbsp;
                    <asp:RequiredFieldValidator Display="Dynamic" ErrorMessage="Campo contenitore obbligatorio" ID="rfvContainer" runat="server" />
                </td>
            </tr>
        </table>
        <%--fascicoli--%>
        <table id="tblFascicoli" class="datatable" runat="server">
            <tr>
                <th colspan="3">Fascicoli
                    <asp:Label ID="lblFascicoloCount" runat="server">0</asp:Label>
                </th>
            </tr>
            <tr>
                <td>
                    <uc:uscContatti ButtonImportVisible="false" ButtonManualVisible="false" Caption="Fascicoli" HeaderVisible="false" ID="uscFascicoli" IsRequired="false" Multiple="true" MultiSelect="true" runat="server" TreeViewCaption="Fascicolo" />
                </td>
            </tr>
        </table>
        <%--status--%>
        <table id="tblEditProtocolStatus" class="datatable" runat="server">
            <tr>
                <th colspan="2">Stato del protocollo</th>
            </tr>
            <tr>
                <td class="label" style="width: 15%;">Stato</td>
                <td style="width: 85%;">
                    <asp:DropDownList DataTextField="Description" DataValueField="Id" ID="cmbProtocolStatus" runat="server" />
                </td>
            </tr>
        </table>
        <uc:uscClassificatore ID="uscClassificatore" ReadOnly="true" runat="server" />
        <table id="tblEditClassificazione" runat="server" class="datatable">
            <tr>
                <td class="label" style="width: 15%;">SottoClassificazione:
                </td>
                <td style="width: 85%;">
                    <uc:uscClassificatore ID="uscSottoClassificatore" runat="server" SubCategoryMode="true" HeaderVisible="false" Required="false" />
                </td>
            </tr>
        </table>
        <table id="tblEditObject" runat="server" class="datatable" visible="false">
            <tr>
                <th>Oggetto</th>
            </tr>
            <tr>
                <td>
                    <uc:uscObjectChanger ID="uscObject" MultiLine="true" Required="true" RequiredMessage="Campo Oggetto Obbligatorio" runat="server" />
                </td>
            </tr>
        </table>
        <table runat="server" id="tblOther" class="datatable">
            <tr>
                <th colspan="2">Altri</th>
            </tr>
            <tr>
                <td class="label" style="width: 15%;">Note:</td>
                <td style="width: 85%;">
                    <telerik:RadTextBox ID="txtNote" MaxLength="255" runat="server" Width="100%" />
                </td>
            </tr>
            <tr>
                <td class="label" style="width: 15%;">
                    <asp:Label Font-Bold="true" ID="lblSubject" runat="server" /></td>
                <td style="width: 85%;">
                    <uc:uscContattiText ID="uscSubject" MaxLunghezzaTesto="1000" runat="server" TextBoxWidth="300px" ForceAddressBook="True" />
                </td>
            </tr>
            <tr>
                <td class="label" style="width: 15%;">Categoria:</td>
                <td style="width: 85%;">
                    <usc:SelServiceCategory EditMode="true" ID="SelServiceCategory" MaxLength="100" MultiLine="false" Required="false" runat="server" TextBoxWidth="300px" Type="Prot" />
                </td>
            </tr>
            <tr id="rowPubblica" runat="server" visible="false">
                <td class="label" style="width: 15%;">Pubblica su Internet:</td>
                <td style="width: 85%;">
                    <asp:CheckBox ID="ckbPublication" runat="server" />
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Panel runat="server" ID="pnlButtons">
        <asp:Button ID="btnConferma" runat="server" Text="Conferma modifica" OnClientClick="if(!confirmSetStatus()) return false;" />
    </asp:Panel>
</asp:Content>
