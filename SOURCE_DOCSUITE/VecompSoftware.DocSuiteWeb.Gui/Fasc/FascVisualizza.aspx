<%@ Page AutoEventWireup="false" CodeBehind="FascVisualizza.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.FascVisualizza" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Fascicolo - Visualizzazione" %>

<%@ Register Src="~/UserControl/uscFascicolo.ascx" TagName="uscFascicolo" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ID="cn" runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            var fascVisualizza;
            require(["Fasc/FascVisualizza"], function (FascVisualizza) {
                $(function () { 
                    fascVisualizza = new FascVisualizza(tenantModelConfiguration.serviceConfiguration);
                    fascVisualizza.currentFascicleId = "<%= IdFascicle %>";
                    fascVisualizza.currentUser = <%= CurrentUser %>;
                    fascVisualizza.signalRServerAddress = "<%= SignalRAddress %>";
                    fascVisualizza.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                    fascVisualizza.radWindowManagerCollegamentiId = "<%= RadWindowManagerCollegamenti.ClientID %>";
                    fascVisualizza.radWindowManagerId = "<%= MasterDocSuite.DefaultWindowManager.ClientID %>";
                    fascVisualizza.radGridUDId = "<%= uscFascicolo.GridUD.ClientID %>";
                    fascVisualizza.panelUDId = "<%= uscFascicolo.PanelUD.ClientID %>";
                    fascVisualizza.windowInsertProtocolloId = "<%= windowInsertProtocollo.ClientID %>";
                    fascVisualizza.windowCompleteWorkflowId = "<%= windowCompleteWorkflow.ClientID %>";
                    fascVisualizza.windowStartWorkflowId = "<%= windowStartWorkflow.ClientID %>";
                    fascVisualizza.windowWorkflowInstanceLogId = "<%= windowWorkflowInstanceLog.ClientID%>";
                    fascVisualizza.currentPageId = "<%= Me.ClientID %>";
                    fascVisualizza.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    fascVisualizza.pageContentId = "<%= pageContent.ClientID %>";
                    fascVisualizza.btnInserisciId = "<%= btnInserisci.ClientID %>";
                    fascVisualizza.btnModificaId = "<%= btnModifica.ClientID %>";
                    fascVisualizza.btnCloseId = "<%= btnClose.ClientID %>";
                    fascVisualizza.actionPage = "<%= Action %>";
                    fascVisualizza.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    fascVisualizza.btnDocumentiId = "<%= btnDocumenti.ClientID %>";
                    fascVisualizza.uscFascicoloId = "<%= uscFascicolo.PageContentDiv.ClientID %>";
                    fascVisualizza.radNotificationInfoId = "<%= radNotificationInfo.ClientID %>";
                    fascVisualizza.btnLinkId = "<%= btnLink.ClientID %>";
                    fascVisualizza.btnRemoveId = "<%= btnRemove.ClientID %>";
                    fascVisualizza.btnAutorizzaId = "<%= btnAutorizza.ClientID %>";
                    fascVisualizza.btnSignId = "<%= btnSign.ClientID %>";
                    fascVisualizza.btnWorkflowId = "<%= btnWorkflow.ClientID %>";
                    fascVisualizza.btnCompleteWorkflowId = "<%= btnCompleteWorkflow.ClientID %>";
                    fascVisualizza.workflowActivityId = "<%= IdWorkflowActivity%>";
                    fascVisualizza.pnlButtonsId = "<%= pnlButtons.ClientID%>";
                    fascVisualizza.workflowEnabled = <%= WorkflowEnabled.ToString().ToLower() %>;
                    fascVisualizza.btnWorkflowLogsId = "<%= btnWorkflowLogs.ClientID%>";
                    fascVisualizza.btnFascicleLogId = "<%= btnFascicleLog.ClientID%>";
                    fascVisualizza.btnOpenId = "<%= btnOpen.ClientID%>";
                    fascVisualizza.btnUndoId = "<%= btnUndo.ClientID%>";
                    fascVisualizza.btnSendToRolesId = "<%= btnSendToRoles.ClientID%>";
                    fascVisualizza.buttonLogVisible = <%= ButtonLogVisible.ToString().ToLower() %>;
                    fascVisualizza.uscFascFoldersId = "<%= uscFascicolo.UscFascicleFolder.PageContentDiv.ClientID %>";
                    fascVisualizza.btnMoveId = "<%= btnMove.ClientID %>";
                    fascVisualizza.windowMoveItemsId = "<%= windowMoveItems.ClientID %>";
                    fascVisualizza.btnCopyToFascicleId = "<%= btnCopyToFascicle.ClientID %>";
                    fascVisualizza.windowFascicleSearchId = "<%= windowFascicleSearch.ClientID %>";
                    fascVisualizza.currentTenantAOOId = "<%= CurrentTenant.TenantAOO.UniqueId %>";
                    fascVisualizza.initialize();
                });
            });

            function closeSignWindow(sender, args) {
                fascVisualizza.closeSignWindow(sender, args);
            }

        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerCollegamenti" runat="server">
        <Windows>
            <telerik:RadWindow Height="450" ID="windowInsertProtocollo" runat="server" Title="Aggiungi collegamento" Width="600" />
            <telerik:RadWindow Height="450" ID="windowStartWorkflow" runat="server" Title="Avvia attività" Width="600" />
            <telerik:RadWindow Height="450" ID="windowCompleteWorkflow" runat="server" Title="Stato attività" Width="600" />
            <telerik:RadWindow Height="650" Width="1000" runat="server" Title="Log attività" ID="windowWorkflowInstanceLog" />
            <telerik:RadWindow Height="500" ID="windowMoveItems" runat="server" Title="Sposta documento" Width="650" />
            <telerik:RadWindow Height="500" ID="signWindow" OnClientClose="closeSignWindow" ReloadOnShow="true" runat="server" Title="Firma documento" Width="600px" Behaviors="Maximize,Close,Resize" />
            <telerik:RadWindow Height="400" ID="windowFascicleSearch" runat="server" Title="Cerca fascicolo" Width="650" />
        </Windows>
    </telerik:RadWindowManager>

    <telerik:RadPageLayout runat="server" ID="pageContent" Height="100%" HtmlTag="Div">
        <Rows>

            <telerik:LayoutRow Height="100%" HtmlTag="Div">
                <Content>
                    <uc:uscFascicolo ID="uscFascicolo" runat="server" />
                </Content>
            </telerik:LayoutRow>

        </Rows>
    </telerik:RadPageLayout>

    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>

    <telerik:RadNotification ID="radNotificationInfo" runat="server"
        VisibleOnPageLoad="false" LoadContentOn="PageLoad" Width="300" Height="150" Animation="FlyIn"
        EnableRoundedCorners="true" EnableShadow="true" ContentIcon="info" Title="Informazioni Fascicolo" TitleIcon="none" AutoCloseDelay="4000" Position="Center" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Panel ID="pnlButtons" runat="server" Width="100%">
        <asp:Panel runat="server">
            <telerik:RadButton ID="btnDocumenti" runat="server" Width="150px" Text="Documenti" div-style="display:none" />
            <telerik:RadButton ID="btnSign" runat="server" Width="150px" Text="Firma" div-style="display:none" />
            <telerik:RadButton ID="btnAutorizza" runat="server" Width="150" Text="Autorizza" div-style="display:none" />
            <telerik:RadButton ID="btnModifica" runat="server" Width="150px" Text="Modifica" div-style="display:none" />
            <telerik:RadButton ID="btnWorkflow" runat="server" Width="150px" Text="Avvia attività" div-style="display:none" />
            <telerik:RadButton ID="btnOpen" runat="server" Width="150px" Text="Apri" div-style="display:none" />
            <telerik:RadButton ID="btnClose" runat="server" Width="150px" Text="Chiudi" div-style="display:none" />
            <telerik:RadButton ID="btnUndo" runat="server" Width="150px" Text="Annulla" div-style="display:none" />
        </asp:Panel>
        <asp:Panel runat="server">
            <telerik:RadButton ID="btnInserisci" runat="server" Width="150px" Text="Inserisci" div-style="display:none" />
            <telerik:RadButton ID="btnMove" runat="server" Width="150px" Text="Sposta" div-style="display:none" />
            <telerik:RadButton ID="btnCopyToFascicle" runat="server" Width="150px" Text="Copia in" AutoPostBack="false" div-style="display:none" />
            <telerik:RadButton ID="btnRemove" runat="server" Width="150px" Text="Rimuovi" div-style="display:none" />
            <telerik:RadButton ID="btnSendToRoles" runat="server" Width="150px" Text="Invio settori" />
            <telerik:RadButton ID="btnLink" runat="server" Width="150px" Text="Collegamenti" div-style="display:none" />
            <telerik:RadButton ID="btnCompleteWorkflow" runat="server" Width="150" Text="Stato attività" div-style="display:none" />
            <telerik:RadButton ID="btnWorkflowLogs" runat="server" Width="150" Text="Log attività" div-style="display:none" />
            <telerik:RadButton ID="btnFascicleLog" runat="server" Width="150" Text="Log" div-style="display:none" />
        </asp:Panel>

    </asp:Panel>
</asp:Content>
