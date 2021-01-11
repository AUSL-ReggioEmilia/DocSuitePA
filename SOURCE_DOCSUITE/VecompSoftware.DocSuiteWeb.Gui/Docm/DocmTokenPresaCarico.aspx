<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.DocmTokenPresaCarico" Codebehind="DocmTokenPresaCarico.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Pratica - Presa in Carico" %>

<%@ Register Src="~/UserControl/uscDocumentToken.ascx" TagName="UscDocumentToken" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscSettoriUtente.ascx" TagName="UscSettoriUtente" TagPrefix="uc" %>
 
    <asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript" language="javascript">
            function ShowLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= docmContainer.ClientID%>";
                currentLoadingPanel.show(currentUpdatedControl);
            }

            function Rifiuta_ClientShow(radWindow) {
                radWindow.center();
            }

            //richiamata quando la finestra 'Motivazione Rifiuto' viene chiusa
            function CloseRifiutaFunction(sender, args) {
                // restituisco il motivo del rifiuto
                if (args.get_argument() !== null) {
                    ShowLoadingPanel()
                    var ajaxManager = $find("<%= AjaxManager.ClientID%>");
                    ajaxManager.ajaxRequest("<%= Me.ClientID %>" + "|" + args.get_argument());
                }
            }
        </script>
    </telerik:RadScriptBlock>
    </asp:Content>
    <asp:Content ContentPlaceHolderID="cphContent" runat="server">
    
    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerToken" runat="server">
        <Windows>
            <telerik:RadWindow Height="200" ID="windowRifiuta" NavigateUrl="../Docm/DocmTokenRifiuta.aspx?Type=Docm" OnClientClose="CloseRifiutaFunction" OnClientShow="Rifiuta_ClientShow" OpenerElementID="<%# btnRifiuta.ClientID %>" runat="server" Title="Pratica Rifiuto Presa in Carico" Width="600" />
        </Windows>
    </telerik:RadWindowManager>
    <asp:panel style="width: 100%" runat="server" ID="docmContainer">
    <table id="tbl" width="100%" border="0">
        <tr>
            <td colspan="2">
                <uc:UscDocumentToken runat="server" ID="uscDocumentToken" />
             </td>
        </tr>
        <asp:Panel ID="pnlRestituzione" Visible="true" runat="server">
            <tr class="tabella">
                <td colspan="2">
                    Restituzione</td>
            </tr>
            <tr>
                <td align="right" width="20%">
                    <strong>Restituzione:</strong>&nbsp;</td>
                <td width="80%">
                    <telerik:RadTextBox ID="txtReasonResponse" runat="server" Width="100%" Enabled="False" MaxLength="255"></telerik:RadTextBox>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel ID="pnlSettoriInAttesa" Visible="true" runat="server">
            <tr>
                <td colspan="2">
                    <table class="datatable">
                        <tr>
                            <th colspan="2">Settori in Attesa</th>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <telerik:RadTreeView runat="server" ID="radTreeSettoriInAttesa">
                                    <Nodes>
                                        <telerik:RadTreeNode runat="server" Text="Settori in attesa" Font-Bold="true" Expanded="true"></telerik:RadTreeNode>
                                    </Nodes>
                                </telerik:RadTreeView>    
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel ID="pnlDistribuzione" Visible="true" runat="server">
            <tr class="tabella">
                <td colspan="2">Distribuzione Pratica</td>
            </tr>
            <tr>
                <td colspan="2">
                    <uc:UscSettoriUtente runat="server" id="uscDistribuzione" Type="Docm" HeaderVisible="false" ReadOnly="true"></uc:UscSettoriUtente>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel ID="pnlRifiuto" Visible="true" runat="server">
            <tr class="tabella">
                <td colspan="2">
                    Motivo del Rifiuto Presa in Carico</td>
            </tr>
            <tr>
                <td align="right" width="20%">
                    <asp:Image ID="imgRifiuta" runat="server" ImageUrl="../Comm/Images/Remove32.gif"></asp:Image>
                </td>
                <td width="80%">
                    <telerik:RadTextBox ID="txtRefusal" runat="server" Width="100%" MaxLength="255"></telerik:RadTextBox>
                </td>
            </tr>
        </asp:Panel>
    </table>
    <br />
    <br />
    <asp:Button ID="btnAccetta" runat="server" Text="Accetta"></asp:Button>
    <asp:Button ID="btnRifiuta" runat="server" Text="Rifiuta"></asp:Button>
    <asp:TextBox ID="txtProgrIncremental" runat="server" Width="16px" AutoPostBack="True"></asp:TextBox>
    </asp:panel>
</asp:Content>
    
