<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="UDSInvoicesUpload.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UDSInvoicesUpload"
    MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Gestione cassetto fiscale Agenzia delle Entrate" %>

<%@ Register Src="~/UserControl/uscDocumentUpload.ascx" TagName="uscDocumentUpload" TagPrefix="uc1" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">

    <telerik:RadScriptBlock runat="server" ID="radScriptBlock">
        <script type="text/javascript">
            var correlationId = null;
            var correlatedChainId = null;
            var isPreview = false;
            var currentUserTenantName = "<%= CurrentUserTenantName %>";
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

            function confirmImport(sender, args) {
                if (isPreview) {
                    generateInvoiceZip();
                    return;
                }
                startWorkflowImport(sender, args, false);
            }

            function previewImport(sender, args) {
                isPreview = true;
                startWorkflowImport(sender, args, true);
            }

            function startWorkflowImport(sender, args, isPreview) {
                var validated = Page_ClientValidate('');
                if (validated) {
                    dswSignalR = new DSWSignalR("<%= SignalRServerAddress %>");
                    correlatedChainId = $get("<%= HFcorrelatedChainId.ClientID %>").value,
                        correlationId = dswSignalR.newGuid();
                    dswSignalR.setup("WorkflowHub", {
                        'correlationId': correlationId
                    });
                    if (isPreview && isPreview === true) {
                        dswSignalR.registerClientMessage("workflowNotificationInfoAsModel", actionHubWorkflowNotificationInfoAsModel);
                    } else {
                        dswSignalR.registerClientMessage("workflowStatusDone", actionHubWorkflowStatusDone);
                        dswSignalR.registerClientMessage("workflowStatusError", actionHubWorkflowStatusError);
                        dswSignalR.registerClientMessage("workflowNotificationInfo", actionHubWorkflowNotificationInfo);
                        dswSignalR.registerClientMessage("workflowNotificationWarning", actionHubWorkflowNotificationWarning);
                        dswSignalR.registerClientMessage("workflowNotificationError", actionHubWorkflowNotificationError);
                    }
                    dswSignalR.startConnection(onDoneSignalRConnectionCallback, onErrorSignalRCallback);
                }
            }

            function generateInvoiceZip() {
                var list = [];
                var masterTable = $find("<%=rgvPreviewDocuments.ClientID %>").get_masterTableView();
                var inputList = masterTable.get_dataItems();

                for (var i = 0; i < inputList.length; i++) {
                    if (inputList[i]._selected) {
                        list.push({
                            "InvoiceFilename": inputList[i]._dataItem.InvoiceFilename,
                            "InvoiceMetadataFilename": inputList[i]._dataItem.InvoiceMetadataFilename,
                            "Description": "", //passing empty values so object can be deserialized
                            "Result": "",
                            "Selectable": false
                        })
                    }
                }

                if (list.length === 0) {
                    alert('si prega di selezionare almeno una fattura');
                    return;
                }

                var ajaxManager = $find("<%= AjaxManager.ClientID%>");
                var ajaxModel = {};
                ajaxModel.Value = [];
                ajaxModel.ActionName = "GenerateInvoiceZIP";
                ajaxModel.Value.push(JSON.stringify(list));
                ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
            }
            
            function onDoneSignalRConnectionCallback() {
                var serverFunction = "SubscribeStartWorkflow";
                $find("<%= btnSave.ClientID %>").set_enabled(false);
                $find("<%= btnPreview.ClientID %>").set_enabled(false);
                addItemInfo("Preparazione importazione in corso. Attendere prego...");

                //this object needs to adapt -  SubscribeStartWorkflow now requires a list of objects to deserialize
                var workflowReferenceBiblos = [];
                var obj = { "ArchiveName": currentUserTenantName, "ArchiveChainId": correlatedChainId, "Simulation": isPreview };
                workflowReferenceBiblos.push(obj);

                workflowReferenceBiblos = JSON.stringify(workflowReferenceBiblos);

                dswSignalR.sendServerMessages(serverFunction, correlationId, workflowReferenceBiblos, 'workflow_integration', 'DocumentAdE',
                    onDoneSignalRSubscriptionCallback, onErrorSignalRCallback);
            }

            function onErrorSignalRCallback(error) {
                console.log(error);
                addItemError("Impossibile procedere con la gestione dell'importazione delle fatture. Contattare l'assistenza : Errore di comunicazione con le WebAPI.");
                $find("<%= btnSave.ClientID %>").set_enabled(true);
            }

            function onDoneSignalRSubscriptionCallback() {
                addItemInfo("Importazione avviata, a breve verranno visualizzate le attività realtive allo stato di importazione fatture.");
                addItemInfo("Attendere prego...");

                if (!isPreview) {
                    $find("<%= btnSave.ClientID %>").set_enabled(false);
                }                
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

            function actionHubWorkflowNotificationInfoAsModel(model) {
                $find("<%= btnSave.ClientID %>").set_enabled(true);
                addDocumentsToGrid(model);
                ClearMessages();
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

            function addDocumentsToGrid(model) {
                ShowGrid();
                var masterTable = $find("<%=rgvPreviewDocuments.ClientID %>").get_masterTableView();
                masterTable.set_dataSource(JSON.parse(model));
                masterTable.dataBind();
            }

            function ShowGrid() {
                $find("<%=rgvPreviewDocuments.ClientID %>").get_element().style.display = "";
            }

            function ClearMessages() {
                var listBox = $find("<%= radListMessages.ClientID %>");
                listBox.get_items().clear();
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

            function SetChainId(id, startWorkflow) {
                isPreview = false;
                $get("<%= HFcorrelatedChainId.ClientID %>").value = id;
                if (startWorkflow) {
                    startWorkflowImport(null, null, false);
                }
            }

            function SetIdDocument(id) {
                $get("<%= HFcorrelatedIdDocument.ClientID %>").value = id;
            }

            /*
             * Specifies if checkboxes are selectable by binding the Selectable attribute from model to the table.
             */
            function RowBinding(sender, args) {
                var masterTable = sender.get_masterTableView();
                var matchedModel = arrayObjectIndexOf(masterTable._dataSource,
                    masterTable._dataItems[masterTable._dataItems.length - 1]._dataItem.InvoiceFilename,
                    "InvoiceFilename"
                );
                masterTable._dataItems[masterTable._dataItems.length - 1]._selectable = matchedModel.Selectable;
                masterTable.get_dataItems()[masterTable._dataItems.length - 1].get_element().getElementsByTagName("INPUT")[0].disabled = !matchedModel.Selectable;
            }

            /*
             * Returns array matched by a searchTerm.
             * Used for IE compatibility.
             */ 
            function arrayObjectIndexOf(myArray, searchTerm, property) {
                for (var i = 0, len = myArray.length; i < len; i++) {
                    if (myArray[i][property] === searchTerm) return myArray[i];
                }
                return -1;
            }

        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel runat="server" ID="pnlContentInvoice" Width="100%">
        <telerik:RadPageLayout runat="server" ID="insertsPageContent" HtmlTag="Div" Width="100%">
            <Rows>
                <telerik:LayoutRow runat="server" ID="InsertsDataRow">
                    <Rows>
                        <telerik:LayoutRow runat="server" HtmlTag="Div">
                            <Content>
                                <asp:HiddenField ID="HFcorrelatedChainId" runat="server" Value="" />
                                <asp:HiddenField ID="HFcorrelatedIdDocument" runat="server" Value="" />
                                <uc1:uscDocumentUpload ID="uscDocumentUpload" Caption="Documento" runat="server" IsDocumentRequired="true" SignButtonEnabled="false" ButtonScannerEnabled="false" ButtonLibrarySharepointEnabled="false" MultipleDocuments="false" UseSessionStorage="true" Type="Prot" AllowZipDocument="true" AllowedExtensions=".zip" />
                            </Content>
                        </telerik:LayoutRow>
                        <telerik:LayoutRow runat="server" HtmlTag="Div" Width="100%">
                            <Content>
                                <telerik:RadGrid runat="server" ID="rgvPreviewDocuments" AllowAutomaticUpdates="True" GridLines="None" AllowPaging="False" Skin="Office2010Blue" AllowMultiRowSelection="true" AllowFilteringByColumn="False">
                                    <ClientSettings>
                                        <Selecting AllowRowSelect ="true" />
                                        <ClientEvents OnRowDataBound="RowBinding"/>
                                    </ClientSettings>
                                    <MasterTableView CommandItemDisplay="None" AutoGenerateColumns="False" DataKeyNames="UniqueId">
                                        <Columns>
                                            <telerik:GridClientSelectColumn>
                                            </telerik:GridClientSelectColumn>
                                            <telerik:GridTemplateColumn Visible="false" DataField="InvoiceFilename">
                                                <ItemTemplate>
                                                    <input type="hidden" value="InvoiceFilename" id="hdnInvoiceFilename"/>                                                    
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn Visible="false" DataField="InvoiceMetadataFilename">
                                                <ItemTemplate>
                                                    <input type="hidden" value="InvoiceMetadataFilename" id="hdnInvoiceMetadataFilename"/>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridBoundColumn DataField="Description" HeaderStyle-Width="80%" HeaderText="Fattura" AllowFiltering="false" UniqueName="colInvoice">
                                            </telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn DataField="Result" HeaderStyle-Width="18%" HeaderText="Esito" AllowFiltering="false" UniqueName="colOutcome">
                                            </telerik:GridBoundColumn>
                                        </Columns>
                                    </MasterTableView>
                                </telerik:RadGrid>
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
        <telerik:RadButton ID="btnPreview" runat="server" Width="150px" Text="Anteprima importazione" AutoPostBack="false" OnClientClicked="previewImport" />
        <telerik:RadButton ID="btnSave" runat="server" Width="150px" Text="Avvia importazione" AutoPostBack="false" OnClientClicked="confirmImport" />
    </asp:Panel>
</asp:Content>
