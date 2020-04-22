<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="UtltProtCambia.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Inherits="VecompSoftware.DocSuiteWeb.Gui.UtltProtCambia" %>

<%@ Register Src="../UserControl/uscClassificatore.ascx" TagName="uscClassificatore"
    TagPrefix="uc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
         <script language="javascript" type="text/javascript">
            //restituisce un riferimento alla radwindow
            function GetRadWindow()
            {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }


            function CloseWindow(args) {
                var oWindow = GetRadWindow();
                oWindow.close(args);
            }

             function OnClick(args) {
                 GetRadWindow().BrowserWindow.Esegui(args);
             }

         </script>			
    </telerik:RadScriptBlock>

   
    <asp:Panel ID="pnlModifica" runat="server">
        <br class="Spazio">
        <asp:Panel ID="pnlContenitore" runat="server">
            <table style="border: gray 1px solid;" class="Table">
                <tr class="tabella">
                    <td class="Scuro" align="left" colspan="2">
                        Contenitore del protocollo</td>
                </tr>
                <tr class="Chiaro">
                    <td width="100%">
                        <b>Attuale:&nbsp;</b>
                        <asp:DropDownList ID="ddlOldContainer" runat="server" Enabled="False">
                        </asp:DropDownList></td>
                </tr>
                <tr class="Chiaro">
                    <td width="100%">
                        <b>Nuovo:&nbsp;</b>
                        <asp:DropDownList ID="ddlNewContainer" runat="server">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvContainer" runat="server" Display="Dynamic" ErrorMessage="Campo contenitore obbligatorio"
                            ControlToValidate="ddlNewContainer"></asp:RequiredFieldValidator></td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlCategoria" runat="server">
            <table style="border: gray 1px solid;" cellpadding="3" border="0" width="100%">
                <tr class="tabella">
                    <td class="Scuro" align="left" colspan="2">
                        Classificatore del protocollo</td>
                </tr>
                <tr class="Chiaro">
                    <td width="100%" colspan="2">
                     
                    <telerik:RadTreeView BorderStyle="Solid" BorderWidth="1px" ID="tvwOldCategory" runat="server" Width="100%">
                        <Nodes>
                            <telerik:RadTreeNode Expanded="True" Text="Classificazione Attuale" />
                        </Nodes>
                    </telerik:RadTreeView>
                </tr>
                <tr class="Spazio">
                
                </tr>
                <tr class="Chiaro">
                    <td width="100%">
                        <uc1:uscClassificatore id="UscClassificatore1" runat="server">
                        </uc1:uscClassificatore>
                </tr>
            </table>
        </asp:Panel>
        &nbsp;<br>
        <asp:Button ID="btnConferma" runat="server" Text="Conferma" ></asp:Button><br>
        <asp:ValidationSummary ID="vs" runat="server" DisplayMode="List" ShowSummary="False"
            ShowMessageBox="True"></asp:ValidationSummary>
    </asp:Panel>
    <asp:Table ID="tblRicerca" runat="server" Width="100%">
        <asp:TableRow>
            <asp:TableCell Width="100%" Font-Bold="True" Text="Tutti i Protocolli selezionati sono associati ad un Fascicolo.&lt;BR&gt;Impossibile modificare la Classificazione del protocollo."></asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Content>
