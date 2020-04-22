<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="UDSInvoiceDelete.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UDSInvoiceDelete"
    MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Annullamento fattura elettronica" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var correlationId = null;
            var dswSignalR;

            function showLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentFlatLoadingPanel = $find("<%= MasterDocSuite.AjaxFlatLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= pnlContentInvoice.ClientID%>";
                var currentUpdatedToolbar = "<%= pnlButtons.ClientID %>";
                currentLoadingPanel.show(currentUpdatedControl);
                currentFlatLoadingPanel.show(currentUpdatedToolbar);
            }

            function hideLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentFlatLoadingPanel = $find("<%= MasterDocSuite.AjaxFlatLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= pnlContentInvoice.ClientID%>";
                var currentUpdatedToolbar = "<%= pnlButtons.ClientID %>";
                currentLoadingPanel.show(currentUpdatedControl);
                currentFlatLoadingPanel.hide(currentUpdatedToolbar);
                currentLoadingPanel.hide(currentUpdatedControl);
            }

            function confirmDelete(sender, args) {
                var validated = Page_ClientValidate('');
                if (validated) {
                    dswSignalR = new DSWSignalR("<%= SignalRServerAddress %>");
                    correlationId = dswSignalR.newGuid();
                    dswSignalR.setup("WorkflowHub", {
                        'correlationId': correlationId
                    });
                    dswSignalR.registerClientMessage("workflowStatusDone", actionHubWorkflowStatusDone);
                    dswSignalR.registerClientMessage("workflowStatusError", actionHubWorkflowStatusError);
                    dswSignalR.registerClientMessage("workflowNotificationInfo", actionHubWorkflowNotificationInfo);
                    dswSignalR.registerClientMessage("workflowNotificationWarning", actionHubWorkflowNotificationWarning);
                    dswSignalR.registerClientMessage("workflowNotificationError", actionHubWorkflowNotificationError);
                    dswSignalR.startConnection(onDoneSignalRConnectionCallback, onErrorSignalRCallback);
                }
            }

            function onDoneSignalRConnectionCallback() {

                var motivation = $find("<%= txtCancelReason.ClientID%>").get_value();

                var serverFunction = "SubscribeStartWorkflow";
                $find("<%= btnSave.ClientID %>").set_enabled(false);
                addItemInfo("Preparazione annullamento della fattura in corso. Attendere prego...");

                var udsIds;
                if (sessionStorage.getItem("InvoiceSelections") !== null) {
                    udsIds = sessionStorage.getItem("InvoiceSelections");
                    udsIds = JSON.parse(udsIds);
                } else {
                    return;
                }

                var workflowReferenceBiblos = [];
                Array.prototype.forEach.call(udsIds, function (udsId) {
                    var obj = { "ArchiveName": motivation, "ArchiveChainId": udsId };
                    workflowReferenceBiblos.push(obj)
                });

                workflowReferenceBiblos = JSON.stringify(workflowReferenceBiblos);

                dswSignalR.sendServerMessages(serverFunction, correlationId, workflowReferenceBiblos, 'workflow_integration', 'AutomaticInvoiceDelete',
                    onDoneSignalRSubscriptionCallback, onErrorSignalRCallback);
            }

            function onErrorSignalRCallback(error) {
                console.log(error);
                addItemError("Impossibile procedere con l'annullamento della fattura. Contattare l'assistenza : Errore di comunicazione con le WebAPI.");
                $find("<%= btnSave.ClientID %>").set_enabled(true);
            }

            function onDoneSignalRSubscriptionCallback() {
                addItemInfo("Annullamento avviato, a breve verranno visualizzate le attività relative allo stato di elaborazione.");
                addItemInfo("Attendere prego...");
                $find("<%= btnSave.ClientID %>").set_enabled(false);
            }

            function actionHubWorkflowStatusDone(model) {
                addItemDone(model);
            }

            function actionHubWorkflowStatusError(model) {
                addItemError(model);
            }

            function actionHubWorkflowNotificationInfo(model) {
                addItemInfo(model);
            }

            function actionHubWorkflowNotificationWarning(model) {
                addItemWarning(model);
            }

            function actionHubWorkflowNotificationError(model) {
                addItemError(model);
            }

            function addItemDone(text) {
                return addItem(text, "../App_Themes/DocSuite2008/imgset16/star.png");
            }

            function addItemInfo(text) {
                return addItem(text, "../App_Themes/DocSuite2008/imgset16/information.png");
            }

            function addItemWarning(text) {
                return addItem(text, "../App_Themes/DocSuite2008/imgset16/StatusSecurityWarning_16x.png");
            }

            function addItemError(text) {
                return addItem(text, "../App_Themes/DocSuite2008/imgset16/StatusSecurityCritical_16x.png");
            }

            function addItem(text, image) {
                var listBox = $find("<%= radListMessages.ClientID %>");
                var item = new Telerik.Web.UI.RadListBoxItem();
                item.set_text(text);
                item.set_imageUrl(image)
                listBox.get_items().add(item);

                listBox.commitChanges();
                return false;
            }

        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel runat="server" ID="pnlContentInvoice" Width="100%">
        <telerik:RadPageLayout runat="server" ID="insertsPageContent" HtmlTag="Div" Width="100%">
            <Rows>
                <telerik:LayoutRow runat="server" ID="motivationDataRow">
                    <Rows>
                        <telerik:LayoutRow RowType="Container" HtmlTag="Div">
                            <Content>
                                <label class="label"><b>Estremi del provvedimento di annullamento della fattura:</b></label>
                            </Content>
                        </telerik:LayoutRow>
                        <telerik:LayoutRow runat="server" HtmlTag="Div">
                            <Content>
                                <telerik:RadTextBox runat="server" ID="txtCancelReason" Rows="3" Width="100%" TextMode="MultiLine" Style="margin-top: 2px;" />
                                <asp:RequiredFieldValidator ControlToValidate="txtCancelReason" ErrorMessage="Motivazione obbligatoria" ID="rfvCancelReason" runat="server" />
                            </Content>
                        </telerik:LayoutRow>
                        <telerik:LayoutRow runat="server" HtmlTag="Div">
                            <Content>
                                <telerik:RadListBox RenderMode="Lightweight" ID="radListMessages" runat="server" Height="98%" Width="98%" SelectionMode="Single" />
                            </Content>
                        </telerik:LayoutRow>
                    </Rows>
                </telerik:LayoutRow>
            </Rows>
        </telerik:RadPageLayout>
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Panel runat="server" ID="pnlButtons">
        <telerik:RadButton ID="btnSave" runat="server" Width="150px" Text="Annulla fattura" AutoPostBack="false" OnClientClicked="confirmDelete"/>
    </asp:Panel>
</asp:Content>
