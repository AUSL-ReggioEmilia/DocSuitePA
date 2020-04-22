<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscFascicleSelect.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscFascicleSelect" %>

<%@ Register Src="~/UserControl/uscFasciclePreview.ascx" TagName="uscFasciclePreview" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscClassificatore.ascx" TagName="uscCategory" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" ID="rsbContent" EnableViewState="true">
    <script type="text/javascript">
        //Apre una finestra generale
        function <%= Me.ID %>_OpenSearchWindow() {
            var url = "../Fasc/FascRicerca.aspx?Type=Fasc&Titolo=Selezione Fascicolo&Action=Docm";
            var name = "wndSearch";
            var oManager = $find("<%=BasePage.MasterDocSuite.DefaultWindowManager().ClientID%>");
            if (oManager == null) {
                alert("Errore: nessun RadWindowManager trovato per l'apertura della form di ricerca");
            } else {
                var window = oManager.open(url, name);
                window.add_close(top.CloseFascSelect);
                window.setSize(700, 480);
                window.maximize();
            }

            return false;
        }

        top.CloseFascSelect = function (sender, args) {
            if (args.get_argument() !== null) {
                var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                ajaxManager.ajaxRequest("<%= Me.ClientID %>" + "|" + args.get_argument());
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

<telerik:RadAjaxPanel runat="server" ID="ajaxPanel">
    <table runat="server" id="tblSearch" class="datatable">
        <tr>
            <td style="height: 36px; vertical-align: middle;" runat="server" id="tdYear">
                <b>Anno:</b>
                <asp:TextBox ID="txtYear" runat="server" MaxLength="4" Width="56px" />
                <asp:RegularExpressionValidator ControlToValidate="txtYear" Display="Dynamic" ErrorMessage="Errore formato" ID="vYear" runat="server" ValidationExpression="\d{4}" />
            </td>
            <td style="height: 36px; vertical-align: middle;" runat="server" id="tdAdoptionNumber">
                <b>Numero:</b>
                <asp:TextBox ID="txtNumber" MaxLength="7" runat="server" Width="96px" />
                <asp:RegularExpressionValidator ControlToValidate="txtNumber" Display="Dynamic" ErrorMessage="Errore formato" ID="vNumber" runat="server" ValidationExpression="\d*" />
            </td>
            <td style="height: 36px; vertical-align: middle;">
                <asp:Button ID="btnSeleziona" runat="server" Text="Seleziona" />&nbsp;
                <asp:Button ID="btnCerca" runat="server" Text="Cerca" />
            </td>
        </tr>
    </table>
</telerik:RadAjaxPanel>
<usc:uscCategory Caption="Classificazione" ID="uscCategory" Multiple="false" Required="false" runat="server" Type="Prot" />

<telerik:RadScriptBlock runat="server" ID="rsbtable" EnableViewState="false">
    <table style="width: 100%;">
        <tr class="Spazio">
            <td></td>
        </tr>
        <tr>
            <td>
                <usc:uscFasciclePreview ID="uscFascilePreview" runat="server" Visible="false" />
            </td>
        </tr>
    </table>
</telerik:RadScriptBlock>
