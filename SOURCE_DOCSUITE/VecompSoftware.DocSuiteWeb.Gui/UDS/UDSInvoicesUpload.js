/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalR.d.ts" />
define(["require", "exports", "App/Core/WorkflowStorage/WorkflowStorage", "App/Core/WorkflowStorage/MessageWorkflowResumeStatus"], function (require, exports, WorkflowStorage, MessageWorkflowResumeStatus) {
    var UDSInvoicesUpload = /** @class */ (function () {
        function UDSInvoicesUpload(pageClientValidator) {
            var _this = this;
            //internal fields
            this.isPreview = false;
            this.wStorageEnabled = false;
            this.correlationId = null;
            this.correlatedChainId = null;
            /**
             * If the resume status is returning with a "NotResumed" status, we will disconnect all created listeners
             * in this case the onErrorSignalRCallback message will be triggered signaling that the connection ended
             * with this flag we prevent the unwanted behaviour
             */
            this.plannedConnectionStop = false;
            this.initialize = function () {
                try {
                    //initializing a store to keep track of started activities
                    _this.wstorage = new WorkflowStorage();
                    if (_this.wstorage.IsValid) {
                        _this.wStorageEnabled = true;
                        //now that we have the store, let's check if there is pending item in storage
                    }
                }
                catch (err) {
                    _this.wStorageEnabled = false;
                    //disabling the save button if the client does not support local or session storage
                    _this.getBtnSave().set_enabled(false);
                    window.alert("Questa funzionalità non è supportata con l'attuale browser. E' necessario utilizzare un browser moderno come IE10+, Edge o Chrome");
                }
                if (_this.wstorage.HasKey()) {
                    //if we have a correlationId, there is a possibility that webapi has a running workflow and we can attach to it
                    _this.correlationId = _this.wstorage.GetCorrelationId();
                    _this.correlatedChainId = _this.wstorage.GetCorrelatedChainId();
                    _this.resumeWorkflowImport();
                }
            };
            this.getBtnSave = function () { return $find(_this.btnSaveId); };
            this.getBtnPreview = function () { return $find(_this.btnPreviewId); };
            this.getRadListMessages = function () { return $find(_this.radListMessagesId); };
            this.getCurrentLoadingPanel = function () { return $find(_this.currentLoadingPanelId); };
            this.getCurrentFlatLoadingPanel = function () { return $find(_this.currentFlatLoadingPanelId); };
            this.getAjaxManager = function () { return $find(_this.ajaxManagerId); };
            this.getRgvPreviewDocuments = function () { return $find(_this.rgvPreviewDocumentsId); };
            this.enableBtnSave = function () { return _this.getBtnSave().set_enabled(true); };
            this.disableBtnSave = function () { return _this.getBtnSave().set_enabled(false); };
            this.enableBtnPreview = function () { return _this.getBtnPreview().set_enabled(true); };
            this.disableBtnPreview = function () { return _this.getBtnPreview().set_enabled(false); };
            this.showLoadingPanel = function () {
                _this.getCurrentLoadingPanel().show(_this.currentUpdatedControlId);
                _this.getCurrentFlatLoadingPanel().show(_this.currentUpdatedToolbarId);
            };
            this.hideLoadingPanel = function () {
                _this.getCurrentLoadingPanel().show(_this.currentUpdatedControlId);
                _this.getCurrentFlatLoadingPanel().hide(_this.currentUpdatedToolbarId);
                _this.getCurrentLoadingPanel().hide(_this.currentUpdatedControlId);
            };
            this.confirmImport = function (sender, args) {
                if (_this.isPreview) {
                    _this.generateInvoiceZip();
                    return;
                }
                _this.startWorkflowImport(false);
            };
            this.previewImport = function (sender, args) {
                _this.isPreview = true;
                _this.startWorkflowImport(true);
            };
            this.resumeWorkflowImport = function () {
                _this.getBtnSave().set_enabled(false);
                _this.getBtnPreview().set_enabled(false);
                _this.dswSignalR = new DSWSignalR(_this.signalRServerAddress);
                _this.dswSignalR.setup("WorkflowHub", {
                    'correlationId': _this.correlationId
                });
                _this.dswSignalR.registerClientMessage(UDSInvoicesUpload.hubMethodWorkflowStatusDone, _this.actionHubWorkflowStatusDone);
                _this.dswSignalR.registerClientMessage(UDSInvoicesUpload.hubMethodWorkflowStatusError, _this.actionHubWorkflowStatusError);
                _this.dswSignalR.registerClientMessage(UDSInvoicesUpload.hubMethodWorkflowNotificationInfo, _this.actionHubWorkflowNotificationInfo);
                _this.dswSignalR.registerClientMessage(UDSInvoicesUpload.hubMethodWorkflowNotificationWarning, _this.actionHubWorkflowNotificationWarning);
                _this.dswSignalR.registerClientMessage(UDSInvoicesUpload.hubMethodWorkflowNotificationError, _this.actionHubWorkflowNotificationError);
                //connect to resume channel and wait for response
                _this.dswSignalR.registerClientMessage(UDSInvoicesUpload.hubMethodWorkflowResumeStatus, _this.actionHubWorkflowResumeStatus);
                _this.dswSignalR.startConnection(_this.onDoneResumeSignalRConnectionCallback, _this.onErrorSignalRCallback);
            };
            this.startWorkflowImport = function (_isPreview) {
                var validated = _this._pageClientValidate();
                if (validated) {
                    _this.disableBtnSave();
                    _this.disableBtnPreview();
                    _this.dswSignalR = new DSWSignalR(_this.signalRServerAddress);
                    _this.correlatedChainId = $get(_this.hFcorrelatedChainId).value;
                    _this.correlationId = _this.dswSignalR.newGuid();
                    _this.dswSignalR.setup("WorkflowHub", {
                        'correlationId': _this.correlationId
                    });
                    if (_this.isPreview && _this.isPreview === true) {
                        _this.dswSignalR.registerClientMessage(UDSInvoicesUpload.hubMehtodWorkflowNotificationInfoAsModel, _this.actionHubWorkflowNotificationInfoAsModel);
                    }
                    else {
                        _this.dswSignalR.registerClientMessage(UDSInvoicesUpload.hubMethodWorkflowStatusDone, _this.actionHubWorkflowStatusDone);
                        _this.dswSignalR.registerClientMessage(UDSInvoicesUpload.hubMethodWorkflowStatusError, _this.actionHubWorkflowStatusError);
                        _this.dswSignalR.registerClientMessage(UDSInvoicesUpload.hubMethodWorkflowNotificationInfo, _this.actionHubWorkflowNotificationInfo);
                        _this.dswSignalR.registerClientMessage(UDSInvoicesUpload.hubMethodWorkflowNotificationWarning, _this.actionHubWorkflowNotificationWarning);
                        _this.dswSignalR.registerClientMessage(UDSInvoicesUpload.hubMethodWorkflowNotificationError, _this.actionHubWorkflowNotificationError);
                    }
                    _this.dswSignalR.startConnection(_this.onDoneSignalRConnectionCallback, _this.onErrorSignalRCallback);
                }
            };
            this.generateInvoiceZip = function () {
                var list = [];
                var masterTable = _this.getRgvPreviewDocuments().get_masterTableView();
                var inputList = masterTable.get_dataItems();
                for (var i = 0; i < inputList.length; i++) {
                    if (inputList[i]._selected) {
                        list.push({
                            "InvoiceFilename": inputList[i]._dataItem.InvoiceFilename,
                            "InvoiceMetadataFilename": inputList[i]._dataItem.InvoiceMetadataFilename,
                            "Description": "",
                            "Result": "",
                            "Selectable": false
                        });
                    }
                }
                if (list.length === 0) {
                    alert('si prega di selezionare almeno una fattura');
                    return;
                }
                var ajaxModel = {};
                ajaxModel.Value = [];
                ajaxModel.ActionName = "GenerateInvoiceZIP";
                ajaxModel.Value.push(JSON.stringify(list));
                _this.getAjaxManager().ajaxRequest(JSON.stringify(ajaxModel));
            };
            //lamba function to solve scoping of a callback 
            this.onDoneResumeSignalRConnectionCallback = function () {
                if (!_this.isPreview && _this.wStorageEnabled) {
                    //we want to store the correlationId for the started worflow if we are not in preview mode
                    _this.wstorage.Set(_this.correlationId, _this.correlatedChainId);
                }
                var serverFunction = UDSInvoicesUpload.serverFunctionSubscribeResumeWorkflow;
                _this.addItemInfo("Una attività di importazione è già stata avviata ed è in corso. Attendere prego...");
                _this.dswSignalR.sendServerMessage(serverFunction, _this.correlationId, _this.onDoneSignalRSubscriptionCallback, _this.onErrorSignalRCallback);
            };
            this.onDoneSignalRConnectionCallback = function () {
                if (!_this.isPreview && _this.wStorageEnabled) {
                    //we wnat to store the correlationId for the started worflow if we are not in preview mode
                    _this.wstorage.Set(_this.correlationId, _this.correlatedChainId);
                }
                var serverFunction = UDSInvoicesUpload.serverFunctionSubscribeStartWorkflow;
                _this.addItemInfo("Preparazione importazione in corso. Attendere prego...");
                //this object needs to adapt -  SubscribeStartWorkflow now requires a list of objects to deserialize
                var workflowReferenceBiblos = [];
                var obj = { "ArchiveName": _this.currentUserTenantName, "ArchiveChainId": _this.correlatedChainId, "Simulation": _this.isPreview };
                workflowReferenceBiblos.push(obj);
                workflowReferenceBiblos = JSON.stringify(workflowReferenceBiblos);
                _this.dswSignalR.sendServerMessages(serverFunction, _this.correlationId, workflowReferenceBiblos, 'workflow_integration', 'DocumentAdE', _this.onDoneSignalRSubscriptionCallback, _this.onErrorSignalRCallback);
            };
            this.onErrorSignalRCallback = function (error) {
                if (_this.plannedConnectionStop) {
                    _this.plannedConnectionStop = false;
                    return;
                }
                if (_this.wStorageEnabled) {
                    //if we are trying to resume and there is an error, we should remove
                    _this.wstorage.Unset();
                }
                //console.log(error);
                _this.addItemError("Impossibile procedere con la gestione dell'importazione delle fatture. Contattare l'assistenza : Errore di comunicazione con le WebAPI.");
                _this.enableBtnSave();
            };
            this.onDoneSignalRSubscriptionCallback = function (error) {
                _this.addItemInfo("Importazione avviata, a breve verranno visualizzate le attività realtive allo stato di importazione fatture.");
                _this.addItemInfo("Attendere prego...");
            };
            this.actionHubWorkflowStatusDone = function (model) {
                if (!_this.isPreview && _this.wStorageEnabled) {
                    //if the workflow status error is received, it means that workflow has completed and we can remove
                    //correlation id from store
                    _this.wstorage.Unset();
                }
                _this.addItemDone(model);
            };
            this.actionHubWorkflowStatusError = function (model) {
                if (!_this.isPreview && _this.wStorageEnabled) {
                    //if the workflow status error is received, it means that workflow has completed and we can remove
                    //correlation id from store
                    _this.wstorage.Unset();
                }
                _this.addItemError(model);
            };
            this.actionHubWorkflowNotificationInfo = function (model) {
                _this.addItemInfo(model);
            };
            this.actionHubWorkflowNotificationInfoAsModel = function (model) {
                _this.getBtnSave().set_enabled(true);
                _this.addDocumentsToGrid(model);
                _this.ClearMessages();
            };
            this.actionHubWorkflowNotificationWarning = function (model) {
                _this.addItemWarning(model);
            };
            this.actionHubWorkflowNotificationError = function (model) {
                _this.addItemError(model);
            };
            /**
             * When the dialog attempts to attach to an alleged running worflow, the webapi returns a message if resume fails.
             * Failure occurs if the process is already finished.
             * @param model
             */
            this.actionHubWorkflowResumeStatus = function (status) {
                if (status == MessageWorkflowResumeStatus.DidNotResume) {
                    //if the status is 1, resume has failed and we can show the original dialog
                    _this.wstorage.Unset();
                    //set flag to true, because when closing the connection signalR the function onErrorSignalRCallback will be triggered
                    //saying that the connection was closed before receiving the message
                    _this.plannedConnectionStop = true;
                    _this.dswSignalR.stopClient();
                    _this.getBtnSave().set_enabled(true);
                    _this.getBtnPreview().set_enabled(true);
                    //remove any pending messages in the listbox because we are starting from scratch
                    _this.ClearMessages();
                }
            };
            this.addItemDone = function (text) {
                _this.addItem(text, "../App_Themes/DocSuite2008/imgset16/star.png");
            };
            this.addItemInfo = function (text) {
                _this.addItem(text, "../App_Themes/DocSuite2008/imgset16/information.png");
            };
            this.addItemWarning = function (text) {
                _this.addItem(text, "../App_Themes/DocSuite2008/imgset16/StatusSecurityWarning_16x.png");
            };
            this.addItemError = function (text) {
                _this.addItem(text, "../App_Themes/DocSuite2008/imgset16/StatusSecurityCritical_16x.png");
            };
            this.addDocumentsToGrid = function (model) {
                _this.ShowGrid();
                var masterTable = _this.getRgvPreviewDocuments().get_masterTableView();
                masterTable.set_dataSource(JSON.parse(model));
                masterTable.dataBind();
            };
            this.ShowGrid = function () {
                _this.getRgvPreviewDocuments().get_element().style.display = "";
            };
            this.ClearMessages = function () {
                _this.getRadListMessages().get_items().clear();
            };
            this.addItem = function (text, imageUrl) {
                var item = new Telerik.Web.UI.RadListBoxItem();
                item.set_text(text);
                item.set_imageUrl(imageUrl);
                _this.getRadListMessages().get_items().add(item);
                _this.getRadListMessages().commitChanges();
            };
            this.SetChainId = function (id, startWorkflow) {
                _this.isPreview = false;
                $get(_this.hFcorrelatedChainId).value = id;
                if (startWorkflow) {
                    _this.startWorkflowImport(false);
                }
            };
            this.SetIdDocument = function (id) {
                $get(_this.hFcorrelatedIdDocument).value = id;
            };
            /*
             * Specifies if checkboxes are selectable by binding the Selectable attribute from model to the table.
             */
            this.RowBinding = function (sender, args) {
                var masterTable = sender.get_masterTableView();
                var matchedModel = _this.arrayObjectIndexOf(masterTable._dataSource, masterTable._dataItems[masterTable._dataItems.length - 1]._dataItem.InvoiceFilename, "InvoiceFilename");
                masterTable._dataItems[masterTable._dataItems.length - 1]._selectable = matchedModel.Selectable;
                masterTable.get_dataItems()[masterTable._dataItems.length - 1].get_element().getElementsByTagName("INPUT")[0].disabled = !matchedModel.Selectable;
            };
            this._pageClientValidate = pageClientValidator;
        }
        /*
         * Returns array matched by a searchTerm.
         * Used for IE compatibility.
         */
        UDSInvoicesUpload.prototype.arrayObjectIndexOf = function (myArray, searchTerm, property) {
            for (var i = 0, len = myArray.length; i < len; i++) {
                if (myArray[i][property] === searchTerm)
                    return myArray[i];
            }
            return -1;
        };
        UDSInvoicesUpload.hubMethodWorkflowStatusDone = "workflowStatusDone";
        UDSInvoicesUpload.hubMethodWorkflowStatusError = "workflowStatusError";
        UDSInvoicesUpload.hubMethodWorkflowNotificationInfo = "workflowNotificationInfo";
        UDSInvoicesUpload.hubMehtodWorkflowNotificationInfoAsModel = "workflowNotificationInfoAsModel";
        UDSInvoicesUpload.hubMethodWorkflowNotificationWarning = "workflowNotificationWarning";
        UDSInvoicesUpload.hubMethodWorkflowNotificationError = "workflowNotificationError";
        UDSInvoicesUpload.hubMethodWorkflowResumeStatus = "workflowResumeStatus";
        UDSInvoicesUpload.serverFunctionSubscribeResumeWorkflow = "SubscribeResumeWorkflowUncoupled";
        UDSInvoicesUpload.serverFunctionSubscribeStartWorkflow = "SubscribeStartWorkflowUncoupled";
        return UDSInvoicesUpload;
    }());
    return UDSInvoicesUpload;
});
//# sourceMappingURL=UDSInvoicesUpload.js.map