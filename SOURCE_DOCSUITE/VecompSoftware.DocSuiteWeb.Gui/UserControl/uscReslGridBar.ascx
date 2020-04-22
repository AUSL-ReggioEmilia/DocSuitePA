<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscReslGridBar.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscReslGridBar" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
    <script type="text/javascript" language="javascript">
    //richiamata quando la finestra rubrica viene chiusa
        function <%= Me.ID %>_CloseFunction(sender, args) {
            var ajaxManager = $find("<%= AjaxManager.ClientID %>");
            if (args.get_argument() !== null) {                
                ajaxManager.ajaxRequest('wnd|' + args.get_argument());
            }
            else {
                ajaxManager.ajaxRequest('clearsession');
            }
        }

        function <%= Me.ID %>_OpenWindow(name,title,url,width,height) {
            var manager = $find("<%= RadWindowManagerReslGridBar.ClientID %>");
            var wnd = manager.open(url, name);
            wnd.add_close(<%= Me.ID %>_CloseFunction);
            wnd.SetTitle(title);
            wnd.setSize(width, height);
            wnd.center();
            return false;
        }

        function EnableDisableWorkflowButton(enable) {
            var ajaxManager = $find("<%= AjaxManager.ClientID %>");
            ajaxManager.ajaxRequest(enable);
        }

    </script>
</telerik:RadScriptBlock>
<telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerReslGridBar" runat="server">
    <Windows>
        <telerik:RadWindow ID="windowFlussoElenco" ReloadOnShow="false" runat="server" />
    </Windows>
</telerik:RadWindowManager>

<asp:Panel runat="server" ID="pnlGridBarWorkflow">
    <asp:Button runat="server" ID="btnWorkflow" Text="Prossimo Passo" />
</asp:Panel>
<asp:Panel runat="server" ID="pnlGridBar">
    <DocSuite:PromptClickOnceButton id="btnAdd" runat="server" width="120px" text="Aggiungi" visible="False" />
    <DocSuite:PromptClickOnceButton id="btnRemove" runat="server" width="120px" text="Rimuovi" visible="False" />
    <asp:button id="btnStampa" runat="server" width="120px" text="Stampa selezione" visible="False"></asp:button>
    <asp:Button ID="btnDocuments" runat="server" Text="Visualizza documenti" Visible="False" Width="130px" />
	<asp:button id="btnSelectAll" runat="server" width="120px" text="Seleziona tutti" visible="False" CausesValidation="false"></asp:button>
	<asp:button id="btnDeselectAll" runat="server" width="120px" text="Annulla selezione" visible="False" CausesValidation="false"></asp:button>
	<asp:button id="btnSetRead" runat="server" width="120px" text="Segna come letti" visible="False"></asp:button>
    <asp:Button runat="server" ID="btnPubblicaWeb" Text="Pubblicazione Web" width="120px" Visible="False"  />&nbsp;
	<DocSuite:PromptClickOnceButton id="btnValida" runat="server" width="150px" text="Affari Generali parziale" visible="False" DisableAfterClick="True" ConfirmationMessage="Confermi L'Elenco Atti parziale?"/>
</asp:Panel> 
<asp:Panel runat="server" ID="pnlGridBarRegion">
    <asp:Button runat="server" ID="btnShowRegion" Text="Solo Regioni" width="120px"  />
    <asp:Button runat="server" ID="btnShowAll" Text="Tutti" width="120px" />&nbsp;
</asp:Panel>
