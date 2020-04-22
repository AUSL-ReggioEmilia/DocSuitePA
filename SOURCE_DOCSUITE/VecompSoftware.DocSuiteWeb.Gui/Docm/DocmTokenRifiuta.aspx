<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.DocmTokenRifiuta"
    Codebehind="DocmTokenRifiuta.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Pratica - Motivo del Rifiuto Presa in Carico" %>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
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
            	
                function CloseWindow()
                {
                    var oWindow = GetRadWindow();
                    var sRifiuto = document.getElementById('<%= txtRefusal.ClientID%>').value; 
                    oWindow.close(sRifiuto);
                }
        </script>
    </telerik:RadScriptBlock>

    <table id="tbl" cellspacing="1" cellpadding="1" width="100%" border="0" height="100%">
        <tr class="tabella">
            <td colspan="2">
                Motivo del Rifiuto Presa in Carico</td>
        </tr>
        <tr>
            <td>
                <telerik:RadTextBox ID="txtRefusal" runat="server" Width="100%" Height="100%" TextMode="MultiLine" MaxLength="255"></telerik:RadTextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic"
                    ControlToValidate="txtRefusal" ErrorMessage="Campo Motivo del Rifiuto Obbligatorio"></asp:RequiredFieldValidator>
            </td>
        </tr>
    </table>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
        ShowSummary="False" DisplayMode="List"></asp:ValidationSummary>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnConferma" runat="server" Text="Conferma Rifiuto" OnClientClick="CloseWindow();"></asp:Button><br />
</asp:Content>
