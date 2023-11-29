<%@ Page AutoEventWireup="false" CodeBehind="DocmVisualizza.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DocmVisualizza" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Pratiche - Visualizzazione" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">


    <telerik:RadScriptBlock runat="server" ID="RadScriptBlockTRV">
        <script type="text/javascript">
            function OpenWindow(url, name, width, height, parameters) {
                var wnd = $find(name);
                wnd.setUrl(url + "?" + parameters);
                wnd.add_close(CloseRefreshFolderFunction);
                wnd.setSize(width, height);
                wnd.show();

                return false;
            }

            function CloseRefreshFolderFunction(sender, args) {
                sender.remove_close(CloseRefreshFolderFunction);
                if (args.get_argument() !== null) {
                    $find("<%= AjaxManager.ClientID %>").ajaxRequest("SetIncremental");
                }
            }

            //funzione che imposta la nuova cartella col focus ed esegue il refresh delle cartelle del sommario
            function FolderRefreshIncr(selectedIncremental) {
                $find("<%= AjaxManager.ClientID %>").ajaxRequest("SetIncremental|" + selectedIncremental);
            }

            //funzione che esegue il refresh delle cartelle nel sommario
            function FolderRefresh() {
                $find("<%= AjaxManager.ClientID %>").ajaxRequest("SetIncremental");
            }

            //funzione che esegue il refresh delle cartelle nel sommario
            function FolderRefreshFull() {
                FolderRefresh();
                return false;
            }

            function SetDocumentPane(url) {
                var pane = $find('<%= paneDocument.ClientID%>');
                pane.set_contentUrl(url);
                return false;
            }

            function RefreshFolderAjax() {
                var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                ajaxManager.ajaxRequest("");
            }

        </script>
    </telerik:RadScriptBlock>
    
    <telerik:RadWindow Behaviors="Close" ID="windowDocmSommarioLegenda" runat="server" />
    <telerik:RadWindow Behaviors="Close,Resize,Move" ID="windowDocmGestioneCartella" runat="server" />

    <telerik:RadSplitter Height="100%" ID="splitterPage" ResizeWithBrowserWindow="true" runat="server" Width="100%">
        <telerik:RadPane ID="paneMenu" MinWidth="100" runat="server" Width="25%">
            <telerik:RadToolBar ID="toolbarInfo" runat="server" Width="100%">
                <Items>
                    <telerik:RadToolBarButton CommandName="info" ToolTip="Dati generali della Pratica" ImageUrl="~/Docm/Images/Info16.gif" CommandArgument="DocmInfo.aspx" />
                    <telerik:RadToolBarButton CommandName="workflow" ToolTip="Visualizzazione attività" ImageUrl="~/Docm/Images/RoleWorkflow.gif" CommandArgument="DocmWorkFlow.aspx" />
                    <telerik:RadToolBarButton CommandName="token" ToolTip="Dettaglio attività" ImageUrl="~/Docm/Images/RoleToken.gif" CommandArgument="DocmToken.aspx" />
                    <telerik:RadToolBarButton CommandName="user" ToolTip="Dettaglio assegnazione utenti" ImageUrl="~/Comm/Images/User16.gif" CommandArgument="DocmTokenUser.aspx" />
                    <telerik:RadToolBarButton CommandName="step" ToolTip="Autorizzazioni" ImageUrl="~/Comm/Images/Proprieta16.gif" CommandArgument="DocmInfoStep.aspx" />
                    <telerik:RadToolBarButton CommandName="send" ToolTip="Mail" ImageUrl="~/App_Themes/DocSuite2008/imgset16/mail.png" Visible="False" PostBackUrl="../MailSenders/GenericMailSender.aspx?Type=Docm" />
                    <telerik:RadToolBarButton CommandName="log" ToolTip="Log" ImageUrl="~/Comm/Images/Trova16.gif" CommandArgument="DocmLog.aspx" />
                    <telerik:RadToolBarButton IsSeparator="true" />
                    <telerik:RadToolBarButton CommandName="legenda" ToolTip="Legenda" ImageUrl="~/Docm/Images/Legenda.gif" />
                </Items>
            </telerik:RadToolBar>
            <telerik:RadToolBar ID="toolbarDocument" runat="server" Width="100%">
                <Items>
                    <telerik:RadToolBarButton CommandName="refresh" ToolTip="Aggiorna la Pratica" ImageUrl="~/Comm/Images/Refresh16.gif" />
                    <telerik:RadToolBarButton IsSeparator="true" />
                    <telerik:RadToolBarButton Group="document" CommandName="authorize" ToolTip="Autorizzazioni in Copia Conoscenza" ImageUrl="~/App_Themes/DocSuite2008/imgset16/brick.png" CommandArgument="DocmAutorizza.aspx" />
                    <telerik:RadToolBarButton Group="document" CommandName="modify" ToolTip="Modifica dati della Pratica" ImageUrl="../Comm/Images/DocSuite/Pratica16.gif" CommandArgument="DocmModifica.aspx" />
                    <telerik:RadToolBarButton Group="document" CommandName="publication" ToolTip="Modifica Pubblicazione su Internet" ImageUrl="~/Comm/Images/Explorer16.gif" CommandArgument="DocmModifica.aspx" />
                    <telerik:RadToolBarButton Group="document" CommandName="lock" ToolTip="Chiusura della Pratica" ImageUrl="~/Docm/Images/DocmChiusura.gif" CommandArgument="DocmChiusuraApertura.aspx" />
                    <telerik:RadToolBarButton Group="document" CommandName="cancel" ToolTip="Annullamento della Pratica" ImageUrl="~/Docm/Images/DocmAnnulla.gif" CommandArgument="DocmAnnulla.aspx" />
                    <telerik:RadToolBarButton Group="document" CommandName="reopen" ToolTip="Riapertura della Pratica" ImageUrl="~/Docm/Images/DocmRiapertura.gif" CommandArgument="DocmChiusuraApertura.aspx" />
                </Items>
            </telerik:RadToolBar>
            <telerik:RadToolBar ID="toolbarFolder" runat="server" Width="100%">
                <Items>
                    <telerik:RadToolBarButton CommandName="add" ToolTip="Inserimento Cartella" ImageUrl="~/Comm/Images/FolderClose16.gif" CommandArgument="DocmCartella.aspx" Value="add" />
                    <telerik:RadToolBarButton CommandName="rename" ToolTip="Rinomina Cartella" ImageUrl="~/Comm/Images/FolderRename16.gif" CommandArgument="DocmCartella.aspx" Value="rename" />
                    <telerik:RadToolBarButton CommandName="delete" ToolTip="Cancella Cartella" ImageUrl="~/Comm/Images/FolderDelete16.gif" CommandArgument="DocmCartella.aspx" Value="delete" />
                    <telerik:RadToolBarButton CommandName="checkout" ToolTip="Elenco CheckOut" ImageUrl="~/Comm/images/file/CheckOut16.gif" CommandArgument="DocmVersioningCheckOut.aspx" />
                </Items>
            </telerik:RadToolBar>
            <telerik:RadToolBar ID="toolbarWorkflow" runat="server" Width="100%">
                <Items>
                    <telerik:RadToolBarButton CommandName="request" ToolTip="Richiesta di Presa in Carico" ImageUrl="~/Docm/Images/TokenRichiestaPresa.gif" CommandArgument="DocmTokenRichiestaPresa.aspx" />
                    <telerik:RadToolBarButton CommandName="retrieval" ToolTip="Richiamo della Richiesta di Presa in Carico" ImageUrl="~/Docm/Images/TokenRichiamo.gif" CommandArgument="DocmTokenRichiamo.aspx" />
                    <telerik:RadToolBarButton CommandName="take" ToolTip="Presa in carico da Richiesta" ImageUrl="~/Docm/Images/TokenPresaCarico.gif" CommandArgument="DocmTokenPresaCarico.aspx" />
                    <telerik:RadToolBarButton CommandName="restitution" ToolTip="Richiesta di Restituzione" ImageUrl="~/Docm/Images/TokenRestituzione.gif" CommandArgument="DocmTokenRestituzione.aspx" />
                    <telerik:RadToolBarButton CommandName="assignment" ToolTip="Assegnazione Utenti" ImageUrl="~/Comm/Images/User16.gif" CommandArgument="DocmSettoreUtenti.aspx" />
                </Items>
            </telerik:RadToolBar>
            <div class="Spazio"></div>
            <telerik:RadTreeView EnableViewState="true" ID="tvDocument" runat="server" Width="100%" />
            <div class="Spazio"></div>
            <telerik:RadTreeView EnableViewState="true" ID="tvSettoriCC" LoadingStatusPosition="BeforeNodeText" runat="server" Width="100%" />
            <span id="misteryBox" runat="server">
                <asp:TextBox ID="txtIdRoleRight" runat="server" />
                <asp:TextBox ID="txtIdRoleRightW" runat="server" />
                <asp:TextBox ID="txtIdRoleRightM" runat="server" />
                <asp:TextBox ID="txtPStep" runat="server" />
                <asp:TextBox ID="txtPIdOwner" runat="server" />
                <asp:TextBox ID="txtRRStep" runat="server" />
                <asp:TextBox ID="txtRRIdOwner" runat="server" />
                <asp:TextBox ID="txtPRStep" runat="server" />
                <asp:TextBox ID="txtPRIdOwner" runat="server" />
                <asp:TextBox ID="txtRStep" runat="server" />
                <asp:TextBox ID="txtRIdOwner" runat="server" />
                <asp:TextBox ID="txtRNStep" runat="server" />
                <asp:TextBox ID="txtRNIdOwner" runat="server" />
                <asp:TextBox ID="txtCCStep" runat="server" />
                <asp:TextBox ID="txtCCidOwner" runat="server" />
                <asp:TextBox ID="txtUserStep" runat="server" />
                <asp:TextBox ID="txtUserAccount" runat="server" />
            </span>
        </telerik:RadPane>
        <telerik:RadSplitBar runat="server" ID="RadSplitBar1" />

        <telerik:RadPane runat="server" ID="paneDocument" Width="75%" />
    </telerik:RadSplitter>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="cmdNuovo" runat="server" Text="Nuova Pratica" Visible="false" />
</asp:Content>
