<%@ Page AutoEventWireup="false" Codebehind="ProtCollegamentiGes.aspx.vb" EnableEventValidation="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtCollegamentiGes" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Protocollo - Collegamenti" %>

<%@ Register Src="~/UserControl/uscProtocolPreview.ascx" TagName="ProtocolPreview" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" ID="rsbContent" EnableViewState="false">
        <style>#divContent {height: 302px!important; }</style>
        <script language="javascript" type="text/javascript">
             //Apre una finestra generale
            function OpenSearchWindow() {
                var oBrowserWnd = GetRadWindow().BrowserWindow;
                setTimeout(function () {
                    var window = oBrowserWnd.radopen("../Prot/ProtRicerca.aspx?Titolo=Selezione Protocollo&Action=Fasc", "wndSearch");
                    window.set_behaviors(Telerik.Web.UI.WindowBehaviors.Maximize + Telerik.Web.UI.WindowBehaviors.Resize + Telerik.Web.UI.WindowBehaviors.Close);
                    window.add_close(OnClientClose);
                    window.setSize(<%=NewPecWindowWidth %>, <%=NewPecWindowHeight %>);
                }, 0);
            }

            // funzione di chiusura della finestra (prima era iniettata nella pagina chiamante: ProtVisualizza)
            OnClientClose = function(sender, args) {
                if (args.get_argument() !== null) {
                    var splitted = args.get_argument().split("|");
                    document.getElementById('<%=txtYear.ClientId %>').value = splitted[0];
                    document.getElementById('<%=txtNumber.ClientId %>').value = splitted[1];
                    document.getElementById('<%= btnSeleziona.ClientId %>').click();
                }
            };
            
            //restituisce un riferimento alla radwindow
            function GetRadWindow()
            {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }	
            function CloseWindow()
            {
                var oWindow = GetRadWindow();
                oWindow.close();    
            }         
        </script>			
   </telerik:RadScriptBlock>   
    <table class="datatable">
        <tr>
            <th colspan="3">
               Protocollo <%=CurrentProtocol.Id.ToString()%>
            </th>
        </tr>
        <tr>
            <td colspan="3">
                <telerik:RadTreeView ID="tvwProtocolLink" Width="100%" runat="server">
                    <Nodes>
                        <telerik:RadTreeNode runat="server" Text="Collegamenti" Expanded="true" EnableViewState="false" />
                    </Nodes>
                </telerik:RadTreeView>
            </td>
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
            <td>
                <asp:Button ID="btnSeleziona" runat="server" Text="Seleziona" />
                <asp:Button ID="btnCerca" runat="server" Text="Cerca" UseSubmitBehavior="false" OnClientClick="OpenSearchWindow();return false;" />
            </td>
        </tr>
        <tr>
            <td style="text-align:center">
                <asp:RegularExpressionValidator ID="vYear" runat="server" ValidationExpression="\d{4}" ControlToValidate="txtYear" ErrorMessage="Errore formato" Display="Dynamic" />
                <asp:RequiredFieldValidator ID="Requiredfieldvalidator2" runat="server" ControlToValidate="txtYear" ErrorMessage="Campo Obbligatorio" Display="Dynamic" />            
            </td>
            <td style="text-align:center">
                <asp:RegularExpressionValidator ID="vNumber" runat="server" ValidationExpression="\d*" ControlToValidate="txtNumber" ErrorMessage="Errore formato" Display="Dynamic" />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtNumber" ErrorMessage="Campo Obbligatorio" Display="Dynamic" />            
            </td>
            <td>
            </td>            
        </tr>
    </table>
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <usc:ProtocolPreview ID="uscProtocolPreview" runat="server" Visible="false" />
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:button id="btnAdd" runat="server" Text="Aggiungi" Enabled="False" Visible="false" UseSubmitBehavior="false" CausesValidation="false" />
</asp:Content>
