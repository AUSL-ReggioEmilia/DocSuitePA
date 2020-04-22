<%@ Page AutoEventWireup="false" CodeBehind="ReslProtocollo.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslProtocollo" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscProtocolPreview.ascx" TagName="ProtocolPreview" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock2">
        <script type="text/javascript" language="javascript">
            function OpenSearchWindow() {
                var winManager = GetRadWindow().get_windowManager();
                var window = winManager.open("../Prot/ProtRicerca.aspx?Titolo=Selezione Protocollo&Action=Resl", "wndSearch");
                window.add_close(OnSearchClientClose);
                window.maximize();
                window.center();
            }

            // funzione di chiusura della finestra (prima era iniettata nella pagina chiamante: ProtVisualizza)
            function OnSearchClientClose(sender, args) {
                sender.remove_close(OnSearchClientClose);
                if (args.get_argument() !== null) {
                    $find("<%= AjaxManager.ClientID %>").ajaxRequest(args.get_argument());
                }
            }

            function CloseWindow(value) {
                var oWindow = GetRadWindow();
                oWindow.close(value);
            }
        </script>
    </telerik:RadScriptBlock>

    <table class="datatable">
        <tr>
            <td>
                <asp:Label Font-Bold="True" ID="lblTitolo" runat="server" />
            </td>
        </tr>
        <tr class="Chiaro">
            <td>
                <table id="pnlAggiungi" runat="server">
                    <tr>
                        <td>
                            <b>Anno:</b>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlYear" runat="server" Width="76px" AutoPostBack="true" />
                        </td>
                        <td>
                            <b>Numero:</b>
                        </td>
                        <td style="white-space: nowrap">
                            <asp:Panel runat="server" DefaultButton="btnSeleziona">
                                <telerik:RadComboBox runat="server" CausesValidation="false"
                                    ID="rcbNumber" AutoPostBack="true" EnableLoadOnDemand="true" MarkFirstMatch="true"
                                    ItemRequestTimeout="500" Visible="true" Width="90px">
                                </telerik:RadComboBox>
                            </asp:Panel>
                        </td>
                        <td>
                            <asp:Button ID="btnSeleziona" runat="server" Text="Seleziona" />
                            <asp:Button ID="btnCerca" OnClientClick="OpenSearchWindow();return false;" runat="server" Text="Cerca" UseSubmitBehavior="false" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <usc:ProtocolPreview runat="server" ID="uscProtPreview" Type="Prot" Visible="false" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button Enabled="False" ID="btnInserimento" runat="server" Text="Conferma" />
    <asp:Button ID="btnModifica" runat="server" Text="Conferma" />
</asp:Content>
