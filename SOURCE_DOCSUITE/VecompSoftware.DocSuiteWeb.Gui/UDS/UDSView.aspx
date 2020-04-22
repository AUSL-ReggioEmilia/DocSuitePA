<%@ Page Title="Sommario UDS" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="UDSView.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UDSView" %>

<%@ Register Src="UserControl/uscUDS.ascx" TagPrefix="usc" TagName="UDS" %>
<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagPrefix="usc" TagName="Contatti" %>
<%@ Register Src="~/UserControl/uscSettori.ascx" TagPrefix="usc" TagName="Settori" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <script type="text/javascript" src="../Scripts/dsw.uds.hub.js"></script>

    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var udsScripts = null;

            function initialize() {
                if (!udsScripts) {
                    udsScripts = new UscUDSScripts();
                }
            }

            var currentLoadingPanel;
            var currentUpdatedControl;
            function showButtonsLoadingPanel() {
                if (currentLoadingPanel !== null) {
                    return false;
                }
                currentLoadingPanel = $find("<%= MasterDocSuite.AjaxFlatLoadingPanel.ClientID%>");
                currentUpdatedControl = "<%= pnlButtons.ClientID%>";
                currentLoadingPanel.show(currentUpdatedControl);
            };

            function hideButtonsLoadingPanel() {
                if (currentLoadingPanel != null) {
                    currentLoadingPanel.hide(currentUpdatedControl);
                }
                currentUpdatedControl = null;
                currentLoadingPanel = null;
            };

            function showLoadingPanel(sender, args) {
                initialize();
                showButtonsLoadingPanel();
                udsScripts.showLoadingPanel();
            }

            function cancelConfirm(e, args) {
                var validated = Page_ClientValidate('Cancel');
                if (validated) {
                    initialize();
                    setButtonsEnableState(false);
                    closeCancelUDSWindow();
                    var udsHub = new DSWUDSHub("<%= SignalRServerAddress %>",
                        $find("<%= udsNotification.ClientID %>"),
                        $find("<%= responseNotificationError.ClientID %>"),
                        $find("<%= AjaxManager.ClientID %>"),
                        $find("<%= btnCancel.ClientID %>"),
                        $get("<%= HFcorrelatedCommandId.ClientID %>"),
                        "<%= btnCancel.UniqueID %>", udsScripts
                    );
                    udsHub.start("Delete", onSuccessCallback, onErrorCallback);
                }
            }

            function onSuccessCallback(model) {
                $find("<%= AjaxManager.ClientID %>").ajaxRequest("udsDeleteCallback|" + model.UniqueId + "|" + model.UDSRepository.Id);
                setButtonsVisibility(false);
                var btnViewDocuments = $find("<%= btnViewDocuments.ClientID %>");
                if (btnViewDocuments != null) {
                    btnViewDocuments.set_visible(true);
                    btnViewDocuments.set_enabled(true);
                };
            }

            function onErrorCallback() {
                $find("<%= btnCancel.ClientID %>").set_enabled(true);
                setButtonsEnableState(true);
            }

            function onError(message) {
                initialize();
                var notification = $find("<%=udsNotification.ClientID %>");
                notification.hide();
                var responseNotificationError = $find("<%=responseNotificationError.ClientID %>");
                responseNotificationError.set_updateInterval(0);
                udsScripts.hideLoadingPanel();
                hideButtonsLoadingPanel();
                responseNotificationError.show();
                responseNotificationError.set_text(message);
                onErrorCallback();
            }

            function onDone(message) {
                initialize();
                var notification = $find("<%=udsNotification.ClientID %>");
                notification.show();
                notification.set_text(message);
                var responseNotificationError = $find("<%=responseNotificationError.ClientID %>");
                udsScripts.hideLoadingPanel();
                hideButtonsLoadingPanel();
                responseNotificationError.hide();
            }

            function setButtonsEnableState(value) {
                var btnEdit = $find("<%= btnEdit.ClientID %>");
                if (btnEdit != null)
                    btnEdit.set_enabled(value);
                var btnCancel = $find("<%= btnCancel.ClientID %>");
                if (btnCancel != null)
                    btnCancel.set_enabled(value);
                var btnViewDocuments = $find("<%= btnViewDocuments.ClientID %>");
                if (btnViewDocuments != null)
                    btnViewDocuments.set_enabled(value);
                var btnToProtocol = $find("<%= btnToProtocol.ClientID %>");
                if (btnToProtocol != null)
                    btnToProtocol.set_enabled(value);
                var btnSendPec = $find("<%= btnSendPec.ClientID %>");
                if (btnSendPec != null)
                    btnSendPec.set_enabled(value);
                var btnMail = $find("<%= btnMail.ClientID %>");
                if (btnMail != null)
                    btnMail.set_enabled(value);
                var btnMailRoles = $find("<%= btnMailRoles.ClientID %>");
                if (btnMailRoles != null)
                    btnMailRoles.set_enabled(value);
                var btnAuthorize = $find("<%= btnAuthorize.ClientID %>");
                if (btnAuthorize != null)
                    btnAuthorize.set_enabled(value);
                var btnLog = $find("<%= btnLog.ClientID %>");
                if (btnLog != null)
                    btnLog.set_enabled(value);
                var btnFascicle = $find("<%= btnFascicle.ClientID %>");
                if (btnFascicle != null)
                    btnFascicle.set_enabled(value);
                var btnDuplica = $find("<%= btnDuplica.ClientID %>");
                if (btnDuplica != null)
                    btnDuplica.set_enabled(value);
                var btnRequestStatement = $find("<%= btnRequestStatement.ClientID %>");
                if (btnRequestStatement != null)
                    btnRequestStatement.set_enabled(value);
                var btnWorkflow = $find("<%= btnWorkflow.ClientID %>");
                if (btnWorkflow != null)
                    btnWorkflow.set_enabled(value);
                var btnCompleteWorkflow = $find("<%= btnCompleteWorkflow.ClientID %>");
                if (btnCompleteWorkflow != null)
                    btnWorkflow.set_enabled(value);
            }

            function setButtonsVisibility(value) {
                var btnEdit = $find("<%= btnEdit.ClientID %>");
                if (btnEdit != null)
                    btnEdit.set_visible(value);
                var btnCancel = $find("<%= btnCancel.ClientID %>");
                if (btnCancel != null)
                    btnCancel.set_visible(value);
                var btnViewDocuments = $find("<%= btnViewDocuments.ClientID %>");
                if (btnViewDocuments != null)
                    btnViewDocuments.set_visible(value);
                var btnToProtocol = $find("<%= btnToProtocol.ClientID %>");
                if (btnToProtocol != null)
                    btnToProtocol.set_visible(value);
                var btnSendPec = $find("<%= btnSendPec.ClientID %>");
                if (btnSendPec != null)
                    btnSendPec.set_visible(value);
                var btnMail = $find("<%= btnMail.ClientID %>");
                if (btnMail != null)
                    btnMail.set_visible(value);
                var btnMailRoles = $find("<%= btnMailRoles.ClientID %>");
                if (btnMailRoles != null)
                    btnMailRoles.set_visible(value);
                var btnAuthorize = $find("<%= btnAuthorize.ClientID %>");
                if (btnAuthorize != null)
                    btnAuthorize.set_visible(value);
                var btnLog = $find("<%= btnLog.ClientID %>");
                if (btnLog != null)
                    btnLog.set_visible(value);
                var btnFascicle = $find("<%= btnFascicle.ClientID %>");
                if (btnFascicle != null)
                    btnFascicle.set_visible(value);
                var btnDuplica = $find("<%= btnDuplica.ClientID %>");
                if (btnDuplica != null)
                    btnDuplica.set_enabled(value);
                var btnRequestStatement = $find("<%= btnRequestStatement.ClientID %>");
                if (btnRequestStatement != null)
                    btnRequestStatement.set_visible(value);
                var btnWorkflow = $find("<%= btnWorkflow.ClientID %>");
                if (btnWorkflow != null)
                    btnWorkflow.set_visible(value);
                var btnCompleteWorkflow = $find("<%= btnCompleteWorkflow.ClientID %>");
                if (btnCompleteWorkflow != null)
                    btnWorkflow.set_visible(value);

            }

            function openDeleteMotivationWindow(e, args) {
                var wnd = $find("<%= windowCancelUDS.ClientID %>");
                wnd.set_destroyOnClose(true);
                wnd.set_reloadOnShow(true);
                wnd.show();
                wnd.center();
            }

            function closeCancelUDSWindow() {
                var wnd = $find("<%= windowCancelUDS.ClientID %>");
                wnd.close();
            }

            function openWindowDuplica() {
                var idUDS = ("<%= CurrentIdUDS.Value %>")
                var idUDSRepository = ("<%= CurrentIdUDSRepository.Value %>")
                var manager = $find("<%=RadWindowManagerUDS.ClientID %>");
                var wnd = manager.open('../UDS/UDSDuplica.aspx?Titolo=Duplicazione Archivio&IdUDS=' + idUDS + '&IdUDSRepository=' + idUDSRepository, 'windowDuplica');
                wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
                wnd.set_visibleStatusbar(false);
                wnd.add_close(closeDuplica);
                wnd.set_modal(true);
                wnd.center();
                return false;
            }

            function closeDuplica(sender, args) {
                sender.remove_close(closeDuplica);

                var idUDS = ("<%= CurrentIdUDS.Value %>")
                var idUDSRepository = ("<%= CurrentIdUDSRepository.Value %>")

                if (args.get_argument() !== null) {
                    $find("<%= AjaxManager.ClientID %>").ajaxRequest("toDuplicate|" + idUDS + "|" + idUDSRepository + "|" + args.get_argument());
                }
            }
            function openWindowRequestStatement() {
                var idUDS = "<%= CurrentIdUDS.Value %>";
                var idUDSRepository = "<%= CurrentIdUDSRepository.Value %>";
                var manager = $find("<%=RadWindowManagerUDS.ClientID %>");
                var wnd = manager.open('../Workflows/RequestStatement.aspx?Titolo=\'Richiesta Attestazione di Conformità\'&IdUDS=' + idUDS + '&IdUDSRepository=' + idUDSRepository + '&ArchiveName=' + '<% =CurrentUDSRepository.Name %>', 'windowRequestStatement');
                wnd.setSize(720, 560);
                wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Maximize + Telerik.Web.UI.WindowBehaviors.Resize + Telerik.Web.UI.WindowBehaviors.Close);
                wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
                wnd.set_visibleStatusbar(false);
                wnd.add_close(closeRequestStatement);
                wnd.set_modal(true);
                wnd.center();
                return false;
            }

            function closeRequestStatement(sender, args) {
                sender.remove_close(closeRequestStatement);

                var idUDS = "<%= CurrentIdUDS.Value %>";
                var idUDSRepository = "<%= CurrentIdUDSRepository.Value %>";

                if (args.get_argument() !== null) {
                    $find("<%= AjaxManager.ClientID %>").ajaxRequest("toDuplicate|" + idUDS + "|" + idUDSRepository + "|" + args.get_argument());
                }
            }

            function goToDuplicateInsert() {
                $get('<%= toDuplicate.ClientID %>').click();
            }
        </script>
    </telerik:RadScriptBlock>
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            var udsView;
            require(["UDS/UDSView"], function (UDSView) {
                $(function () {
                    udsView = new UDSView(tenantModelConfiguration.serviceConfiguration);
                    udsView.ajaxLoadingPanelId = "<%=MasterDocSuite.AjaxDefaultLoadingPanel.ClientId %>";
                    udsView.pnlUDSViewId = "<%= pnlUDSView.ClientID %>";
                    udsView.btnLinkId = "<%= btnLink.ClientID %>";
                    udsView.currentIdUDS = ("<%= CurrentIdUDS.Value %>");
                    udsView.currentIdUDSRepository = ("<%= CurrentIdUDSRepository.Value %>");
                    udsView.radWindowManagerId = ("<%=RadWindowManagerUDS.ClientID %>");
                    udsView.btnWorkflowId = "<%= btnWorkflow.ClientID %>";
                    udsView.btnCompleteWorkflowId = "<%= btnCompleteWorkflow.ClientID %>";
                    udsView.isWorkflowEnabled = JSON.parse("<%= IsWorkflowEnabled.ToString().ToLower() %>");
                    udsView.workflowActivityId = "<%= CurrentUserWorkflowActivityId %>";
                    udsView.radNotificationInfoId = "<%= radNotificationInfo.ClientID %>";
                    udsView.windowCompleteWorkflowId = "<%= windowCompleteWorkflow.ClientID %>";

                    udsView.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>

     <telerik:RadNotification ID="radNotificationInfo" runat="server"
        VisibleOnPageLoad="false" LoadContentOn="PageLoad" Width="300" Height="150" Animation="FlyIn"
        EnableRoundedCorners="true" EnableShadow="true" ContentIcon="info" Title="Informazioni Archivi" TitleIcon="none" AutoCloseDelay="4000" Position="Center" />
</asp:Content>


<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel runat="server" ID="pnlUDSView">
        <telerik:RadWindowManager BackColor="Gray" Behaviors="Close" DestroyOnClose="true" EnableViewState="False" ID="RadWindowManagerUDS" runat="server">
            <Windows>
                <telerik:RadWindow Height="300" ID="windowDuplica" OnClientClose="closeDuplica" runat="server" Title="Archivio  - Duplica" Width="500" />
                <telerik:RadWindow Height="300" ID="windowRequestStatement" OnClientClose="closeRequestStatement" runat="server" Title="Archivio  - Richiedi Attestazione" Width="500" />
                <telerik:RadWindow Height="300" ID="windowUdsSearch" OnClientClose="closeRequestStatement" runat="server" Title="UDS Search" Width="500" />
                <telerik:RadWindow Height="450" ID="windowCompleteWorkflow" runat="server" Title="Stato attività" Width="600" />
            </Windows>
        </telerik:RadWindowManager>

        <telerik:RadWindowManager ID="rwmStartWorkflow" runat="server">
            <Windows>
                <telerik:RadWindow runat="server" Behaviors="Close" DestroyOnClose="true" Height="250px" ShowContentDuringLoad="false"
                    ReloadOnShow="True" ID="windowCancelUDS" Title="Procedura di annullamento archivio" Width="500px">
                    <ContentTemplate>
                        <telerik:RadPageLayout runat="server" GridType="Fluid" HtmlTag="None">
                            <Rows>
                                <telerik:LayoutRow RowType="Container" HtmlTag="Div" CssClass="col-dsw-10">
                                    <Content>
                                        <label>Estremi del provvedimento di annullamento del documento:</label>
                                    </Content>
                                </telerik:LayoutRow>
                                <telerik:LayoutRow RowType="Container" HtmlTag="Div" CssClass="col-dsw-10">
                                    <Content>
                                        <telerik:RadTextBox runat="server" ID="txtCancelReason" Rows="3" Width="100%" TextMode="MultiLine" Style="margin-top: 2px;"></telerik:RadTextBox>
                                        <asp:RequiredFieldValidator ControlToValidate="txtCancelReason" ValidationGroup="Cancel" Display="Dynamic" ErrorMessage="Motivazione obbligatoria" ID="rfvCancelReason" runat="server" />
                                    </Content>
                                </telerik:LayoutRow>
                                <telerik:LayoutRow RowType="Container" HtmlTag="Div" CssClass="col-dsw-10" Style="margin-top: 2px;">
                                    <Content>
                                        <telerik:RadButton runat="server" Text="Conferma" OnClientClicked="cancelConfirm" ValidationGroup="Cancel" ID="btnCancelUDS" AutoPostBack="false" />
                                    </Content>
                                </telerik:LayoutRow>
                            </Rows>
                        </telerik:RadPageLayout>
                    </ContentTemplate>
                </telerik:RadWindow>
            </Windows>
        </telerik:RadWindowManager>

        <asp:HiddenField ID="HFcorrelatedCommandId" runat="server" Value="" />

        <telerik:RadNotification ID="udsNotification" runat="server"
            Width="400" Height="150" Animation="FlyIn" EnableRoundedCorners="true" EnableShadow="true"
            Title="Notifica Archivio" OffsetX="-20" OffsetY="-20" AutoCloseDelay="0"
            TitleIcon="ok" Style="z-index: 100000;" />

        <telerik:RadNotification ID="responseNotificationError" runat="server"
            Width="400" Height="150" Animation="FlyIn" EnableRoundedCorners="true" EnableShadow="true"
            Title="Anomalia in archivio" OffsetX="-20" OffsetY="-20" AutoCloseDelay="0"
            TitleIcon="delete" Style="z-index: 100000;" />

        <telerik:RadPageLayout runat="server" GridType="Fluid" HtmlTag="None">
            <Rows>
                <telerik:LayoutRow RowType="Container" HtmlTag="None" CssClass="col-dsw-10">
                    <Columns>
                        <telerik:CompositeLayoutColumn HtmlTag="None">
                            <Content>
                                <usc:UDS runat="server" ID="uscUDS" />
                            </Content>
                        </telerik:CompositeLayoutColumn>
                    </Columns>
                </telerik:LayoutRow>
            </Rows>
        </telerik:RadPageLayout>
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="pnlButtons">
        <asp:Panel runat="server" ID="pnlActionButtons">
            <telerik:RadButton runat="server" ID="btnViewDocuments" CausesValidation="false" OnClientClicked="showLoadingPanel" Width="150" Text="Documenti" />
            <telerik:RadButton runat="server" ID="btnFascicle" OnClientClicked="showLoadingPanel" Width="150" CausesValidation="false" Text="Fascicolo" />
            <telerik:RadButton runat="server" ID="btnAuthorize" OnClientClicked="showLoadingPanel" Width="150" CausesValidation="false" Text="Autorizza" />
            <telerik:RadButton runat="server" ID="btnEdit" OnClientClicked="showLoadingPanel" Width="150" CausesValidation="false" Text="Modifica" />
            <telerik:RadButton runat="server" ID="btnCancel" OnClientClicked="openDeleteMotivationWindow" Width="150" CausesValidation="false" Text="Annullamento" AutoPostBack="false" />
            <telerik:RadButton runat="server" ID="btnWorkflow" OnClientClicked="showLoadingPanel" Width="150" CausesValidation="false" Text="Avvia attività" />
            <telerik:RadButton runat="server" ID="btnDuplica" OnClientClicked="openWindowDuplica" Width="150" CausesValidation="false" Text="Duplica" AutoPostBack="false" />
            <telerik:RadButton runat="server" ID="btnRequestStatement" OnClientClicked="openWindowRequestStatement" Width="150" CausesValidation="false" Text="Richiesta attestazione" AutoPostBack="false" Visible="false" />
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlFunctionalityButtons">
            <telerik:RadButton runat="server" ID="btnMail" OnClientClicked="showLoadingPanel" Width="150" Text="Mail" CausesValidation="false" />
            <telerik:RadButton runat="server" ID="btnMailRoles" OnClientClicked="showLoadingPanel" Width="150" Text="Invia Settori" CausesValidation="false" />
            <telerik:RadButton runat="server" ID="btnSendPec" OnClientClicked="showLoadingPanel" Width="150" CausesValidation="false" Text="PEC" />
            <telerik:RadButton runat="server" ID="btnToProtocol" CommandArgument="Confermi la procedura automatica di Protocollazione?" OnClientClicking="RadConfirm" CausesValidation="false" OnClientClicked="showLoadingPanel" Width="150" Text="Protocolla" />
            <telerik:RadButton runat="server" ID="btnLink" Width="150" CausesValidation="false" AutoPostBack="false" Text="Collegamento" />
            <telerik:RadButton runat="server" ID="btnCompleteWorkflow" Width="150" Text="Stato attività" div-style="display:none" CausesValidation="false"  AutoPostBack="false"/>
            <telerik:RadButton runat="server" ID="btnLog" OnClientClicked="showLoadingPanel" Width="150" CausesValidation="false" Text="Log" />
        </asp:Panel>
        <asp:Button runat="server" ID="toDuplicate" Text="test" CausesValidation="false" PostBackUrl="~/UDS/UDSInsert.aspx" CssClass="hiddenField" />
    </asp:Panel>
</asp:Content>
