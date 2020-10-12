<%@ Page Title="Sposta fattura elettronica" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="UDSInvoiceMove.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UDSInvoiceMove" %>

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

            function confirmMove(sender, args) {
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
                var tenantDestination = $find("<%= rlbSelectCompany.ClientID %>").get_selectedItem().get_value();
                var motivation = $find("<%= txtCancelReason.ClientID%>").get_value();

                var serverFunction = "SubscribeStartWorkflow";
                $find("<%= btnSave.ClientID %>").set_enabled(false);

                var udsIds;
                if (sessionStorage.getItem("InvoiceSelections") !== null) {
                    udsIds = sessionStorage.getItem("InvoiceSelections");
                    udsIds = JSON.parse(udsIds);
                } else {
                    return;
                }

                var workflowReferenceBiblos = [];
                Array.prototype.forEach.call(udsIds, function (udsId) {
                    var obj = { "ArchiveName": motivation, "ArchiveChainId": udsId, "DocumentName": tenantDestination };
                    workflowReferenceBiblos.push(obj)
                });

                workflowReferenceBiblos = JSON.stringify(workflowReferenceBiblos);

                addItemInfo("Preparazione spostamento della fattura in corso. Attendere prego...");
                dswSignalR.sendServerMessages(serverFunction, correlationId, workflowReferenceBiblos, 'workflow_integration', 'AutomaticInvoiceMove',
                    onDoneSignalRSubscriptionCallback, onErrorSignalRCallback);
            }

            function onErrorSignalRCallback(error) {
                console.log(error);
                addItemError("Impossibile procedere lo spostamento della fattura. Contattare l'assistenza : Errore di comunicazione con le WebAPI.");
                $find("<%= btnSave.ClientID %>").set_enabled(true);
            }

            function onDoneSignalRSubscriptionCallback() {
                addItemInfo("Spostamento avviato, a breve verranno visualizzate le attività relative allo stato di elaborazione.");
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
                <telerik:LayoutRow HtmlTag="Div">
                    <Content>
                        <label class="label"><b>Seleziona l'azienda dove si desidera spostare la fattura:</b></label>
                    </Content>
                </telerik:LayoutRow>
                <telerik:LayoutRow HtmlTag="Div">
                    <Content>
                        <telerik:RadDropDownList runat="server" ID="rlbSelectCompany" Width="300px" AutoPostBack="false" DefaultMessage="premi qui per selezionare" selected="true" DropDownHeight="200px" />
                        <asp:RequiredFieldValidator ControlToValidate="rlbSelectCompany" ErrorMessage="Azienda obbligatoria" ID="rfvSelectCompany" runat="server" />
                    </Content>
                </telerik:LayoutRow>
                <telerik:LayoutRow HtmlTag="Div">
                    <Content>
                        <label class="label"><b>Estremi del provvedimento di spostamento della fattura:</b></label>
                    </Content>
                </telerik:LayoutRow>
                <telerik:LayoutRow HtmlTag="Div">
                    <Content>
                        <telerik:RadTextBox runat="server" ID="txtCancelReason" Rows="3" Width="100%" TextMode="MultiLine" Style="margin-top: 2px;" />
                        <asp:RequiredFieldValidator ControlToValidate="txtCancelReason" ErrorMessage="Motivazione obbligatoria" ID="rfvCancelReason" runat="server" />
                    </Content>
                </telerik:LayoutRow>
                <telerik:LayoutRow HtmlTag="Div">
                    <Content>
                        <telerik:RadListBox RenderMode="Lightweight" ID="radListMessages" runat="server" Height="98%" Width="98%" SelectionMode="Single" />
                    </Content>
                </telerik:LayoutRow>
            </Rows>
        </telerik:RadPageLayout>
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Panel runat="server" ID="pnlButtons">
        <telerik:RadButton ID="btnSave" runat="server" Width="150px" Text="Sposta fattura" AutoPostBack="false" OnClientClicked="confirmMove" />
    </asp:Panel>
</asp:Content>
