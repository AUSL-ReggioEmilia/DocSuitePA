<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscProtocolSelect.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscProtocolSelect" %>

<%@ Register Src="~/UserControl/uscProtocolPreview.ascx" TagName="ProtocolPreview" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" ID="rsbContent" EnableViewState="true">
    <script type="text/javascript">
        //Apre una finestra generale
        function <%= Me.ID %>_OpenSearchWindow() {
            var url = "../Prot/ProtRicerca.aspx?Titolo=Selezione Protocollo&Action=Fasc";
            var name = "wndSearch";
            var oManager = $find("<%=BasePage.MasterDocSuite.DefaultWindowManager().ClientID%>");
            if (oManager == null) {
                alert("Errore: nessun RadWindowManager trovato per l'apertura della form di ricerca");
            } else {
                var window = oManager.open(url, name);
                window.add_close(top.CloseProtSelect);
                window.setSize(700, 480);
                window.maximize();
            }

            return false;
        }

        top.CloseProtSelect = function(sender, args) {
            if (args.get_argument() !== null) {
                var splitted = args.get_argument().split("|");
                document.getElementById('<%= txtYear.ClientID %>').value = splitted[0];
                document.getElementById('<%= txtNumber.ClientID %>').value = splitted[1];
                document.getElementById('<%= btnSeleziona.ClientID %>').click();
            }
        };

        //restituisce un riferimento alla radwindow
        function <%= Me.ID %>_GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }

        function <%= Me.ID %>_CloseWindow() {
            var oWindow = <%= Me.ID %>_GetRadWindow();
            oWindow.close();
        }
    </script>

</telerik:RadScriptBlock>
<div class="prot">
    <table runat="server" id="tblSearch" class="datatable">
        <tr>
            <td>
                <b>Anno: </b>
                <asp:TextBox ID="txtYear" runat="server" Width="60px" MaxLength="4" />
                <asp:RegularExpressionValidator ControlToValidate="txtYear" Display="Dynamic" ErrorMessage="Errore formato" ID="vYear" runat="server" ValidationExpression="\d{4}" />
                <asp:RequiredFieldValidator ControlToValidate="txtYear" Display="Dynamic" ErrorMessage="Campo Obbligatorio" ID="Requiredfieldvalidator2" runat="server" />
                <b>Numero: </b>
                <asp:TextBox ID="txtNumber" runat="server" Width="110px" MaxLength="7" />
                <asp:RegularExpressionValidator ControlToValidate="txtNumber" Display="Dynamic" ErrorMessage="Errore formato" ID="vNumber" runat="server" ValidationExpression="\d*" />
                <asp:RequiredFieldValidator ControlToValidate="txtNumber" Display="Dynamic" ErrorMessage="Campo Obbligatorio" ID="RequiredFieldValidator1" runat="server" />
                <asp:Button ID="btnSeleziona" runat="server" Text="Seleziona" />
                <asp:Button ID="btnCerca" runat="server" Text="Cerca" UseSubmitBehavior="false" />
            </td>
        </tr>
    </table>
    <telerik:RadScriptBlock runat="server" ID="rsbtable" EnableViewState="false">
        <table style="width:100%;">
            <tr class="Spazio">
                <td></td>
            </tr>
            <tr>
                <td>
                    <usc:ProtocolPreview id="uscProtocolPreview" runat="server" visible="false" Type="Prot" />        
                </td>
            </tr>
        </table>
    </telerik:RadScriptBlock>
</div>
