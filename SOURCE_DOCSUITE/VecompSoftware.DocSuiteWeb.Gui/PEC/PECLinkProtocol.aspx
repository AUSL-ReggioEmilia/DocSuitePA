<%@ Page Title="Collega a Protocollo" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="PECLinkProtocol.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.PECLinkProtocol" %>

<%@ Register Src="~/PEC/uscPECInfo.ascx" TagPrefix="usc" TagName="uscPECInfo" %>
<%@ Register Src="~/UserControl/uscProtocolPreview.ascx" TagPrefix="usc" TagName="uscProtocolPreview" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <style type="text/css" media="all">
        .validationErrorSize {
            font-size: 11px !important;
        }
    </style>
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script language="javascript" type="text/javascript">

            function OpenSearchWindow() {
                var wnd = window.radopen("../Prot/ProtRicerca.aspx?Titolo=Selezione Protocollo&Action=Fasc", "wndSearch");
                wnd.setSize(700, 550);
                wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
                wnd.set_visibleStatusbar(false);
                wnd.set_modal(true);
                wnd.add_close(OnClientCloseWindow);
                wnd.center();
            }

            function OnClientCloseWindow(sender, args) {
                if (args.get_argument() !== null) {
                    var splitted = args.get_argument().split("|");
                    var year = splitted[0];
                    var number = splitted[1];
                    document.getElementById('<%= txtYear.ClientID%>').value = year;
                    document.getElementById('<%= txtNumber.ClientID%>').value = number;
                    SelectProtocol();
                }
            }            

            function SelectProtocol() {
                if (Page_ClientValidate()) {
                    return PECLinkProtocolSend("selectProtocol");
                }
                return false;
            }

            function PECLinkProtocolSend(val) {
                $find("<%= AjaxManager.ClientID %>").ajaxRequest(val);
            }

            function ShowLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= pnlPageContent.ClientID%>";
                currentLoadingPanel.show(currentUpdatedControl);
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel runat="server" ID="pnlPageContent">
            <usc:uscPECInfo ID="pecInfo" runat="server" />
        <asp:Panel ID="pnlProtocol" runat="server">
            <table class="datatable" width="100%">
                <tr>
                    <th colspan="3">Selezione Protocollo a cui collegare la PEC
                    </th>
                </tr>
                <tr>
                    <td>
                        <b>Anno: </b>
                        <asp:TextBox ID="txtYear" runat="server" Width="60px" MaxLength="4" />
                    </td>
                    <td>
                        <b>Numero: </b>
                        <asp:TextBox ID="txtNumber" runat="server" Width="110px" MaxLength="7" />
                    </td>
                    <td style="text-align: right;">
                        <asp:Button ID="btnSeleziona" Width="100px" runat="server" ValidationGroup="protSearch" CausesValidation="True" OnClientClick="return SelectProtocol();" Text="Seleziona" />
                        <asp:Button ID="btnCerca" runat="server" Width="100px" Text="Cerca" UseSubmitBehavior="false" OnClientClick="OpenSearchWindow();return false;" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:RegularExpressionValidator ValidationGroup="protSearch" ControlToValidate="txtYear" Display="Dynamic" CssClass="validationErrorSize" ErrorMessage="Errore formato" ID="yearValidator" runat="server" ValidationExpression="\d{4}" />
                        <asp:RequiredFieldValidator ValidationGroup="protSearch" ControlToValidate="txtYear" Display="Dynamic" CssClass="validationErrorSize" ErrorMessage="Campo Obbligatorio" ID="requiredYearValidator" runat="server" />
                    </td>
                    <td>
                        <asp:RegularExpressionValidator ValidationGroup="protSearch" ControlToValidate="txtNumber" CssClass="validationErrorSize" Display="Dynamic" ErrorMessage="Errore formato" ID="numberValidator" runat="server" ValidationExpression="\d*" />
                        <asp:RequiredFieldValidator ValidationGroup="protSearch" ControlToValidate="txtNumber" CssClass="validationErrorSize" Display="Dynamic" ErrorMessage="Campo Obbligatorio" ID="requiredNumberValidator" runat="server" />
                    </td>
                </tr>
            </table>
            <usc:uscProtocolPreview ID="protocolPreview" runat="server" Visible="false" />
        </asp:Panel>
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Button runat="server" ID="btnConfirm" Width="150px" Text="Collega" />
    <asp:Button runat="server" ID="btnCancel" Width="150px" OnClientClick="ShowLoadingPanel();" Text="Annulla" />
</asp:Content>
