<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscResolutionBar.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscResolutionBar" %>

<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        function webButtonClicked(button, message) {
            if (confirm(message) == false) {
                return false;
            } else {
                button.disabled = true;
                document.getElementById("WebPubWait").style.display = "inline";
                return true;
            }
        }
    </script>
</telerik:RadScriptBlock>
<asp:Table ID="tblButtons" runat="server" CssClass="noPrint">
    <asp:TableRow runat="server">
        <asp:TableCell runat="server">
            <asp:Panel ID="pnlButtonsDocument" runat="server" CssClass="dsw-display-inline">
                <asp:Button ID="btnProposta" runat="server" Text="Prop" Width="120px" />
                <asp:Button ID="btnQuickDocument" runat="server" Text="Documenti" Width="120px" />
                <asp:Button ID="btnToSeries" runat="server" Text="Amministrazione aperta" Visible="False" Width="240px" />
                <asp:Button ID="btnConfirmSeries" runat="server" Text="Completa amministrazione trasparente" Visible="False" Width="240px" />
                <asp:Button ID="btnDoc4" runat="server" Text="Doc4" Width="120px" />
                <asp:Button ID="btnDoc5" runat="server" Text="Doc5" Width="120px" />
                <asp:Button ID="btnFrontespizio" runat="server" Text="Frontalino" Visible="False" Width="120px" />
                <asp:Button ID="btnUltimaPagina" runat="server" Text="Ultima pagina" Visible="False" Width="120px" />
                <asp:Button ID="btnPrivacyAttachments" runat="server" Text="Allegati riservati" Visible="False" Width="120px" />
                <asp:Button ID="btnPubblicaRevoca" Enabled="false" runat="server" Text="Sost. della pubblicazione" Visible="False" Width="160px" />
                <asp:Button ID="btnPublished" Enabled="false" runat="server" Text="Doc. pubblicato" Visible="False" Width="120px" />
                <asp:Button ID="btnDeleteFrontespizio" runat="server" Text="Elimina frontalino" Visible="False" Width="120px" />
                <asp:Button ID="btnDeleteUltimaPagina" runat="server" Text="Elimina ultima pagina" Visible="False" Width="120px" />
                <asp:Button ID="btnLastPageUpload" runat="server" Text="Carica ultima pagina" Visible="False" Width="140px" />
            </asp:Panel>
            <asp:Panel ID="pnlWebPublication" runat="server" Visible="false" CssClass="dsw-display-inline">
                <asp:Button ID="bntPubblicaInternet" OnClientClick="javascript:if (webButtonClicked(this, 'Confermare la pubblicazione del documento?') == false) return false;" runat="server" Text="Pubblica int." ToolTip="Pubblica internet" Visible="False" Width="120px" />
                <asp:Button ID="bntRitiraInternet" OnClientClick="javascript:if (webButtonClicked(this, 'Confermare il ritiro del documento?') == false) return false;" runat="server" Text="Ritira int." ToolTip="Ritira internet" Width="120px" />
                <img alt="Wait please" id="WebPubWait" src="../Resl/Images/wait.gif" style="display: none" />
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlButtonsDefault" CssClass="dsw-display-inline">
                <asp:Repeater runat="server" ID="btnRolesRepeater">
                    <ItemTemplate>
                        <asp:Button runat="server" ID="btnRole" Width="120px" />
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Button ID="btnMail" PostBackUrl="~/MailSenders/ResolutionMailSender.aspx?Type=Resl" runat="server" Text="Mail" Width="120px" />
                <asp:Button ID="btnMailSettori" PostBackUrl="~/MailSenders/ResolutionMailSender.aspx?selectRoles=true&Type=Resl" runat="server" Text="Invia settori" Width="120px" />
                <asp:Button ID="btnStampa" runat="server" Text="Stampa" Width="120px" />
                <asp:Button CausesValidation="false" ID="btnDuplica" OnClientClick="return OpenWindowDuplica();" runat="server" Text="Duplica" value="Duplica" Width="120px" />
                <asp:Button ID="btnConfirmView" runat="server" Text="Conferma visione" Width="120px"  Visible="False"/>
            </asp:Panel>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow ID="rowBtn" runat="server">
        <asp:TableCell runat="server">
            <asp:Panel ID="pnlButtonsExtra" runat="server">
                <asp:Button ID="btnPratica" runat="server" Text="Pratica" Width="120px" />
                <asp:Button ID="btnAddToPratica" runat="server" Text="Aggiungi a pratica" Visible="false" Width="120px" />
                <asp:Button ID="btnFascicle" runat="server" Text="Fascicolo" Width="120px" />
                <asp:Button ID="btnModifica" runat="server" Text="Modifica" Width="120px" />
                <asp:Button ID="btnFlushAnnexed" runat="server" Text="Svuota annessi" Width="120px" />
                <input causesvalidation="false" id="inputElimina" onclick="OpenWindowElimina();" runat="server" style="width: 120px" type="button" value="Elimina" />
                <asp:Button CausesValidation="false" ID="btnAnnulla" runat="server" Text="Annullamento" Width="120px" />
                <asp:Button ID="btnLog" runat="server" Text="Log" Width="120px" />
            </asp:Panel>
            <asp:Panel ID="pnlToolbarPreView" runat="server">
                <asp:Button ID="btnRegistrazione" runat="server" Text="Registrazione" Width="120px" />
            </asp:Panel>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
