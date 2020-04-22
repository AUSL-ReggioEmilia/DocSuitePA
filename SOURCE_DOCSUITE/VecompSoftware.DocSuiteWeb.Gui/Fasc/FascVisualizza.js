/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "Fasc/FascBase", "UserControl/uscFascicolo", "App/Models/Fascicles/FascicleType", "App/Helpers/ServiceConfigurationHelper", "App/Services/DocumentUnits/DocumentUnitService", "App/Models/Environment", "App/Services/Workflows/WorkflowActivityService", "App/Services/UDS/UDSRepositoryService", "App/Services/Commons/ConservationService", "App/Models/UpdateActionType", "App/Models/Workflows/WorkflowStatus", "App/Managers/HandlerWorkflowManager", "App/Models/Fascicles/FascicleReferenceType", "App/Models/DocumentUnits/ChainType", "App/Models/Fascicles/FascicleFolderTypology", "UserControl/uscFascicleFolders", "App/Services/Fascicles/FascicleDocumentService", "Fasc/FascMoveItems", "App/Services/Workflows/WorkflowRepositoryService", "App/Rules/Rights/Entities/Fascicles/FascicleRights", "App/Models/Fascicles/FascicleDocumentUnitModel", "App/Services/Fascicles/FascicleDocumentUnitService", "../Workflows/StartWorkflow"], function (require, exports, FascicleBase, UscFascicolo, FascicleType, ServiceConfigurationHelper, DocumentUnitService, Environment, WorkflowActivityService, UDSRepositoryService, ConservationService, UpdateActionType, WorkflowStatus, HandlerWorkflowManager, FascicleReferenceType, ChainType, FascicleFolderTypology, UscFascicleFolders, FascicleDocumentService, FascMoveItems, WorkflowRepositoryService, FascicleRights, FascicleDocumentUnitModel, FascicleDocumentUnitService, StartWorkflow) {
    var FascVisualizza = /** @class */ (function (_super) {
        __extends(FascVisualizza, _super);
        /**
         * Costruttore
         */
        function FascVisualizza(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME)) || this;
            /**
             *------------------------- Events -----------------------------
             */
            /**
             * Evento al click del pulsante "Modifica"
             * @param sender
             * @param args
             */
            _this.btnEdit_OnClick = function (sender, args) {
                args.set_cancel(true);
                var editUrl = "../Fasc/FascModifica.aspx?Type=Fasc&IdFascicle=".concat(_this.currentFascicleId);
                if (_this.actionPage != "") {
                    editUrl = editUrl.concat("&Action=", _this.actionPage);
                }
                window.location.href = editUrl;
            };
            /**
             * Evento al click del pulsante "Documenti"
             * @param sender
             * @param args
             */
            _this.btnDocuments_OnClick = function (sender, args) {
                args.set_cancel(true);
                var documentUrl = "../Viewers/FascicleViewer.aspx?Type=Fasc&IdFascicle=".concat(_this.currentFascicleId);
                if (_this.actionPage != "") {
                    documentUrl = documentUrl.concat("&Action=", _this.actionPage);
                }
                _this._loadingPanel.show(_this.pageContentId);
                window.location.href = documentUrl;
            };
            /**
             * Evento al click del pulsante "Inserisci"
             * @param sender
             * @param args
             */
            _this.btnInsert_OnClick = function (sender, args) {
                args.set_cancel(true);
                var selectedFolder = _this.getSelectedFascicleFolder();
                if (!selectedFolder) {
                    _this.showWarningMessage(_this.uscNotificationId, "E' necessario selezionare una cartella del fascicolo");
                    return;
                }
                var typology = FascicleFolderTypology[selectedFolder.Typology];
                if (isNaN(typology)) {
                    typology = FascicleFolderTypology[typology.toString()];
                }
                switch (typology) {
                    case FascicleFolderTypology.Fascicle:
                    case FascicleFolderTypology.SubFascicle:
                        {
                            var url = "../Fasc/FascDocumentsInsert.aspx?Type=Fasc&IdFascicle=".concat(_this.currentFascicleId, "&IdFascicleFolder=", selectedFolder.UniqueId);
                            if (selectedFolder.idCategory) {
                                url = url.concat("&IdCategory=", selectedFolder.idCategory.toString());
                            }
                            if (_this.radWindowManagerCollegamentiId) {
                                url = url.concat("&ManagerID=", _this.radWindowManagerCollegamentiId);
                            }
                            _this.openWindow(url, "windowInsertProtocollo", 850, 600);
                        }
                        break;
                }
            };
            /**
             * Evento al click del pulsante di chiusura Fascicolo
             * @param sender
             * @param args
             */
            _this.btnClose_OnClick = function (sender, args) {
                args.set_cancel(true);
                _this._manager.radconfirm("Sei sicuro di voler chiudere il fascicolo?", function (arg) {
                    if (arg) {
                        _this._loadingPanel.show(_this.pageContentId);
                        _this.setButtonEnable(false);
                        _this._fascicleModel.EndDate = moment().toDate();
                        _this.service.closeFascicle(_this._fascicleModel, function (data) {
                            _this._btnClose.set_visible(false);
                            _this._btnEdit.set_visible(false);
                            _this._btnInsert.set_visible(false);
                            _this._btnMove.set_visible(false);
                            _this._btnRemove.set_visible(false);
                            _this._btnLink.set_visible(false);
                            _this._btnAutorizza.set_visible(false);
                            _this._btnSendToRoles.set_visible(false);
                            _this._btnWorkflow.set_visible(false);
                            _this._btnComplete.set_visible(false);
                            _this.setBtnOpenVisibility();
                            _this._btnUndo.set_visible(false);
                            var uscFascicolo = $("#".concat(_this.uscFascicoloId)).data();
                            if (!jQuery.isEmptyObject(uscFascicolo)) {
                                uscFascicolo.loadData(_this._fascicleModel);
                            }
                        }, function (exception) {
                            _this._loadingPanel.hide(_this.pageContentId);
                            _this.setButtonEnable(true);
                            _this.showNotificationException(_this.uscNotificationId, exception);
                        });
                    }
                }, 300, 160);
            };
            /**
            * Evento al click del pulsante "Collegamenti"
            * @param sender
            * @param args
            */
            _this.btnLink_OnClick = function (sender, args) {
                args.set_cancel(true);
                var linkUrl = "../Fasc/FascicleLink.aspx?Type=Fasc&IdFascicle=".concat(_this.currentFascicleId);
                window.location.href = linkUrl;
            };
            /**
        * Evento al click del pulsante "Autorizza"
        * @param sender
        * @param args
        */
            _this.btnAutorizza_OnClick = function (sender, args) {
                args.set_cancel(true);
                _this._loadingPanel.show(_this.pageContentId);
                var linkUrl = "../Fasc/FascAutorizza.aspx?Type=Fasc&IdFascicle=".concat(_this.currentFascicleId);
                window.location.href = linkUrl;
            };
            /**
            * Evento al click del pulsante "Rimuovi"
            * @param sender
            * @param args
            */
            _this.btnRemove_OnClick = function (sender, args) {
                args.set_cancel(true);
                var radGridUD = $find(_this.radGridUDId);
                var dataItems = radGridUD.get_selectedItems();
                if (dataItems.length == 0) {
                    _this.showWarningMessage(_this.uscNotificationId, "Nessun documento selezionato");
                    return;
                }
                _this._manager.radconfirm("Sei sicuro di voler eliminare il documento selezionato dal fascicolo corrente?", function (arg) {
                    if (arg) {
                        _this._loadingPanel.show(_this.pageContentId);
                        var item = dataItems[0];
                        var element = (item.findControl("btnUDLink"));
                        var uniqueId = element.get_element().getAttribute("UniqueId");
                        var UDName = element.get_element().getAttribute("DocumentUnitName");
                        var environment = element.get_element().getAttribute("Environment");
                        switch (Number(environment)) {
                            case Environment.Document:
                                var ajaxRequest = {};
                                ajaxRequest.ActionName = "Delete_Miscellanea_Document";
                                ajaxRequest.Value = new Array();
                                ajaxRequest.Value.push(uniqueId);
                                _this._ajaxManager.ajaxRequest(JSON.stringify(ajaxRequest));
                                break;
                            default:
                                _this._fascicleDocumentUnitService.getByDocumentUnitAndFascicle(uniqueId, _this.currentFascicleId, function (data) {
                                    var fascicleDocumentUnitModel = new FascicleDocumentUnitModel(_this.currentFascicleId);
                                    fascicleDocumentUnitModel.DocumentUnit = data.DocumentUnit;
                                    fascicleDocumentUnitModel.UniqueId = data.UniqueId;
                                    _this.removeFascicleUD(fascicleDocumentUnitModel, _this._fascicleDocumentUnitService);
                                });
                                break;
                        }
                    }
                }, 300, 160);
            };
            /**
            * Evento al click del pulsante "Completa"
            * @param sender
            * @param args
            */
            _this.btnComplete_OnClick = function (sender, args) {
                args.set_cancel(true);
                var url = "../Workflows/CompleteWorkflow.aspx?Type=Fasc&IdFascicle=".concat(_this.currentFascicleId, "&IdWorkflowActivity=", _this.workflowActivityId);
                return _this.openWindow(url, "windowCompleteWorkflow", 700, 500);
            };
            /**
            * Evento al click del pulsante "Inserti"
            * @param sender
            * @param args
            */
            _this.btnInserts_OnClick = function (sender, args) {
                args.set_cancel(true);
                var url = "../Fasc/FascMiscellanea.aspx?Type=Fasc&IdFascicle=".concat(_this.currentFascicleId);
                window.location.href = url;
            };
            /**
             * evento al click del pulsante Log flusso di lavoro
             * @param sender
             * @param args
             */
            _this.btnWorkflowLogs_OnClick = function (sender, args) {
                args.set_cancel(true);
                var url = "../Fasc/FascInstanceLog.aspx?Type=Fasc&IdFascicle=".concat(_this.currentFascicleId, "&ManagerID=", _this.radWindowManagerCollegamentiId);
                return _this.openWindow(url, "windowWorkflowInstanceLog", 1000, 650);
            };
            /**
             * evento al click del pulsante Log
             * @param sender
             * @param args
             */
            _this.btnFascicleLog_OnClick = function (sender, args) {
                args.set_cancel(true);
                var url = "../Fasc/FascicleLog.aspx?IdFascicle=".concat(_this.currentFascicleId);
                window.location.href = url;
            };
            /**
            * evento al click del pulsante Apri Fascicolo
            * @param sender
            * @param args
            */
            _this.btnOpen_OnClick = function (sender, args) {
                _this._loadingPanel.show(_this.pageContentId);
                args.set_cancel(true);
                _this.openFascicleClosed();
            };
            /**
            * evento al click del pulsante Annulla Fascicolo
            * @param sender
            * @param args
            */
            _this.btnUndo_OnClick = function (sender, args) {
                _this._loadingPanel.show(_this.pageContentId);
                args.set_cancel(true);
                _this.undoFascicle();
            };
            /**
             * Evento alla chiusura della finestra di inserimento UD nel fascicolo che effettua una push a signalr per aggiornare i riferimenti per gli altri client collegati
             * @param sender
             * @param args
             */
            _this.sendUDUpdate = function (sender, args) {
                _this._loadingPanel.hide(_this.radGridUDId);
                _this._loadingPanel.show(_this.radGridUDId);
                _this.sendRefreshUDRequest();
                _this.sendRefreshLinkRequest();
            };
            /**
             * Evento da SignalR all'aggiunta di una UD al fascicolo da parte di un client terzo
             */
            _this.onUpdatedUDRequest = function () {
                _this.sendRefreshUDRequest(_this, function () {
                    _this._notificationInfo.show();
                    _this._notificationInfo.set_text("Aggiunto un nuovo documento al Fascicolo");
                    _this.sendRefreshLinkRequest();
                });
            };
            /**
             * Evento per il caricamento della griglia delle UD associate tramite SignalR
             * @param sender
             * @param onDoneCallback
             */
            _this.sendRefreshUDRequest = function (sender, onDoneCallback, afterRemove) {
                if (afterRemove === void 0) { afterRemove = false; }
                var uscFascicolo = $("#".concat(_this.uscFascicoloId)).data();
                var qs = uscFascicolo.getFilterModel();
                var currentIdFascicleFolder;
                var selectedFolder = _this.getSelectedFascicleFolder();
                if (selectedFolder) {
                    currentIdFascicleFolder = selectedFolder.UniqueId;
                }
                _this._documentUnitService.getFascicleDocumentUnits(_this._fascicleModel, qs, currentIdFascicleFolder, function (data) {
                    _this.refreshUD(data, afterRemove);
                }, function (exception) {
                    _this._loadingPanel.hide(_this.pageContentId);
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            };
            /**
             * Aggiorna la griglia delle UD
             * @param sender
             * @param args
             */
            _this.refreshUD = function (models, updateFascicleDocument) {
                if (updateFascicleDocument === void 0) { updateFascicleDocument = false; }
                var radGridUD = $find(_this.radGridUDId);
                var panelUD = $("#".concat(_this.panelUDId));
                var uscFascicolo = $("#".concat(_this.uscFascicoloId)).data();
                if (!jQuery.isEmptyObject(uscFascicolo)) {
                    var selectedFolderId_1 = "";
                    var selectedFolder = _this.getSelectedFascicleFolder();
                    if (selectedFolder) {
                        selectedFolderId_1 = selectedFolder.UniqueId;
                    }
                    _this._fascicleDocumentService.getByFolder(_this._fascicleModel.UniqueId, selectedFolderId_1, function (data) {
                        var insertsArchiveChains = data.filter(function (x) { return x.ChainType.toString() == ChainType[ChainType.Miscellanea]; }).map(function (m) { return m.IdArchiveChain; });
                        if (selectedFolderId_1 != "" && updateFascicleDocument) {
                            _this._fascicleDocumentService.updateFascicleDocument(data[0]);
                        }
                        uscFascicolo.refreshGridUD(models, insertsArchiveChains);
                    }, function (exception) {
                        _this._loadingPanel.hide(_this.pageContentId);
                        $("#".concat(_this.pageContentId)).hide();
                        _this.showNotificationException(_this.uscNotificationId, exception);
                    });
                }
                _this._fascicleRights.HasFascicolatedUD = (models.filter(function (e) { return e.ReferenceType.toString() == FascicleReferenceType[FascicleReferenceType.Fascicle]; }).length > 0);
                _this.setBtnCloseVisibility();
                _this.setBtnOpenVisibility();
                _this.setButtonEnable(true);
                _this._loadingPanel.hide(_this.pageContentId);
            };
            _this.sendRefreshLinkRequest = function (sender, onDoneCallback) {
                _this.service.getLinkedFascicles(_this._fascicleModel, null, function (data) {
                    _this.refreshLink(data);
                }, function (exception) {
                    _this._loadingPanel.hide(_this.pageContentId);
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            };
            _this.refreshLink = function (model) {
                var uscFascicolo = $("#".concat(_this.uscFascicoloId)).data();
                if (!jQuery.isEmptyObject(uscFascicolo)) {
                    uscFascicolo.refreshLinkedFascicles(model);
                }
            };
            _this.btnWorkflow_OnClick = function (sender, args) {
                args.set_cancel(true);
                sessionStorage.setItem(StartWorkflow.SESSION_KEY_REFERENCE_MODEL, JSON.stringify(_this._fascicleModel));
                sessionStorage.setItem(StartWorkflow.SESSION_KEY_REFERENCE_ID, _this.currentFascicleId);
                sessionStorage.setItem(StartWorkflow.SESSION_KEY_REFERENCE_TITLE, _this._fascicleModel.Title);
                var radGridUD = $find(_this.radGridUDId);
                var dataItems = radGridUD.get_selectedItems();
                var element;
                var archiveChainId;
                var archiveDocumentId;
                var documentName;
                var environment;
                var dtos = [];
                for (var _i = 0, dataItems_1 = dataItems; _i < dataItems_1.length; _i++) {
                    var item = dataItems_1[_i];
                    element = (item.findControl("btnUDLink"));
                    environment = +element.get_element().getAttribute("Environment");
                    if (environment != Environment.Document) {
                        continue;
                    }
                    archiveChainId = element.get_element().getAttribute("BiblosChainId");
                    archiveDocumentId = element.get_element().getAttribute("BiblosDocumentId");
                    documentName = element.get_element().getAttribute("BiblosDocumentName");
                    var dto = {
                        ArchiveChainId: archiveChainId,
                        ChainType: ChainType.Miscellanea,
                        ArchiveDocumentId: archiveDocumentId,
                        ArchiveName: "",
                        DocumentName: documentName,
                        ReferenceDocument: null
                    };
                    dtos.push(dto);
                }
                sessionStorage.setItem(StartWorkflow.SESSION_KEY_DOCUMENTS_REFERENCE_MODEL, JSON.stringify(dtos));
                var url = "../Workflows/StartWorkflow.aspx?Type=Fasc".concat("&ManagerID=", _this.radWindowManagerCollegamentiId, "&DSWEnvironment=Fascicle&Callback=", window.location.href);
                return _this.openWindow(url, "windowStartWorkflow", 730, 550);
            };
            _this.btnMove_OnClick = function (sender, args) {
                args.set_cancel(true);
                var radGridUD = $find(_this.radGridUDId);
                var dataItems = radGridUD.get_selectedItems();
                if (dataItems.length == 0) {
                    _this.showWarningMessage(_this.uscNotificationId, "Nessun documento selezionato");
                    return;
                }
                var selectedFolder = _this.getSelectedFascicleFolder();
                var dtos = [];
                var element;
                var uniqueId;
                var environment;
                var UDName;
                var dto;
                for (var _i = 0, dataItems_2 = dataItems; _i < dataItems_2.length; _i++) {
                    var item = dataItems_2[_i];
                    element = (item.findControl("btnUDLink"));
                    uniqueId = element.get_element().getAttribute("UniqueId");
                    UDName = element.get_text();
                    environment = element.get_element().getAttribute("Environment");
                    dto = {};
                    dto.uniqueId = uniqueId;
                    dto.name = UDName;
                    dto.environment = Number(environment);
                    dtos.push(dto);
                }
                sessionStorage.setItem(FascMoveItems.FASC_MOVE_ITEMS_Session_key, JSON.stringify(dtos));
                var url = "FascMoveItems.aspx?Type=Fasc&idFascicle=" + _this.currentFascicleId + "&ItemsType=DocumentType&IdFascicleFolder=" + selectedFolder.UniqueId;
                return _this.openWindow(url, "windowMoveItems", 750, 550);
            };
            /**
             * Evento di chiusura della finestra di Workflow
             */
            _this.onWorkflowCloseWindow = function (sender, args) {
                if (args.get_argument()) {
                    var result = {};
                    result = args.get_argument();
                    if (result && result.ActionName === "redirect" && result.Value && result.Value.length > 0) {
                        _this._loadingPanel.show(_this.pageContentId);
                        window.location.href = result.Value[0];
                        return;
                    }
                    _this._notificationInfo.show();
                    _this._notificationInfo.set_text(result.ActionName);
                    _this.workflowActivityId = result.Value ? result.Value[0] : null;
                    _this._workflowActivity = null;
                    _this.workflowCallback();
                }
            };
            _this.folderNodeClickCallback = function (arg) {
                _this.sendUDUpdate(_this, undefined);
                _this._btnInsert.set_enabled(arg);
                _this._btnMove.set_enabled(arg);
            };
            _this.onMoveCloseWindow = function (sender, args) {
                if (args.get_argument()) {
                    _this.sendUDUpdate(sender, args);
                }
            };
            _this.filterWorkflowAuthorizationsByAccount = function (arr, criteria) {
                return arr.filter(function (item) {
                    if (item.Account.toLowerCase() == criteria.toLowerCase()) {
                        return item;
                    }
                });
            };
            _this.workflowCallback = function () {
                _this._pnlButtons = $("#".concat(_this.pnlButtonsId));
                _this._pnlButtons.hide();
                _this.setButtonEnable(false);
                _this._loadingPanel.show(_this.pageContentId);
                _this.service.getFascicle(_this.currentFascicleId, function (data) {
                    if (data == null)
                        return;
                    _this._fascicleModel = data;
                    //se non ho più l'attività di workflow attiva, allora cerco se ne ho aperta un'altra
                    var wfAction = function () { return $.Deferred().resolve().promise(); };
                    if (!_this.workflowActivityId) {
                        wfAction = function () { return _this.loadActiveWorkflowActivities(); };
                    }
                    wfAction()
                        .done(function () {
                        _this.initializeRights(_this._fascicleModel)
                            .done(function (fascicleRights) {
                            _this._fascicleRights = fascicleRights;
                            _this.initializePageByRigths(fascicleRights);
                        })
                            .fail(function (exception) {
                            _this._loadingPanel.hide(_this.pageContentId);
                            $("#".concat(_this.pageContentId)).hide();
                            _this.showNotificationException(_this.uscNotificationId, exception);
                        });
                    })
                        .fail(function (exception) {
                        _this._loadingPanel.hide(_this.pageContentId);
                        $("#".concat(_this.pageContentId)).hide();
                        _this.showNotificationException(_this.uscNotificationId, exception);
                    });
                }, function (exception) {
                    _this._loadingPanel.hide(_this.pageContentId);
                    $("#".concat(_this.pageContentId)).hide();
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            };
            _this.hasActiveWorkflowActivityWorkflow = function () {
                return !String.isNullOrEmpty(_this.workflowActivityId);
            };
            _this._serviceConfigurations = serviceConfigurations;
            _this._handlerManager = new HandlerWorkflowManager(serviceConfigurations);
            $(document).ready(function () {
            });
            return _this;
        }
        /**
         * Initialize
         */
        FascVisualizza.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            this._windowInsertProtocol = $find(this.windowInsertProtocolloId);
            this._windowInsertProtocol.add_close(this.sendUDUpdate);
            this._windowStartWorkflow = $find(this.windowStartWorkflowId);
            this._windowStartWorkflow.add_close((this.onWorkflowCloseWindow));
            this._windowCompleteWorkflow = $find(this.windowCompleteWorkflowId);
            this._windowCompleteWorkflow.add_close((this.onWorkflowCloseWindow));
            this._windowWorkflowInstanceLogs = $find(this.windowWorkflowInstanceLogId);
            this._windowMoveItems = $find(this.windowMoveItemsId);
            this._windowMoveItems.add_close(this.onMoveCloseWindow);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._ajaxManager = $find(this.ajaxManagerId);
            this._notificationInfo = $find(this.radNotificationInfoId);
            this._btnInsert = $find(this.btnInserisciId);
            this._btnEdit = $find(this.btnModificaId);
            this._btnDocuments = $find(this.btnDocumentiId);
            this._btnClose = $find(this.btnCloseId);
            this._btnLink = $find(this.btnLinkId);
            this._btnRemove = $find(this.btnRemoveId);
            this._btnAutorizza = $find(this.btnAutorizzaId);
            this._btnWorkflow = $find(this.btnWorkflowId);
            this._btnOpen = $find(this.btnOpenId);
            this._manager = $find(this.radWindowManagerId);
            this._btnComplete = $find(this.btnCompleteId);
            this._btnWorkflowLogs = $find(this.btnWorkflowLogsId);
            this._btnFascicleLog = $find(this.btnFascicleLogId);
            this._btnSendToRoles = $find(this.btnSendToRolesId);
            this._btnUndo = $find(this.btnUndoId);
            this._btnMove = $find(this.btnMoveId);
            this._btnEdit.add_clicking(this.btnEdit_OnClick);
            this._btnDocuments.add_clicking(this.btnDocuments_OnClick);
            this._btnClose.add_clicking(this.btnClose_OnClick);
            this._btnInsert.add_clicking(this.btnInsert_OnClick);
            this._btnLink.add_clicking(this.btnLink_OnClick);
            this._btnRemove.add_clicking(this.btnRemove_OnClick);
            this._btnComplete.add_clicking(this.btnComplete_OnClick);
            this._btnAutorizza.add_clicking(this.btnAutorizza_OnClick);
            this._btnWorkflow.add_clicking(this.btnWorkflow_OnClick);
            this._btnWorkflowLogs.add_clicking(this.btnWorkflowLogs_OnClick);
            this._btnFascicleLog.add_clicking(this.btnFascicleLog_OnClick);
            this._btnOpen.add_clicking(this.btnOpen_OnClick);
            this._btnUndo.add_clicking(this.btnUndo_OnClick);
            this._btnMove.add_clicking(this.btnMove_OnClick);
            var documentUnitConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.DOCUMENT_UNIT_TYPE_NAME);
            this._documentUnitService = new DocumentUnitService(documentUnitConfiguration);
            var udsRepositoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.UDSREPOSITORY_TYPE_NAME);
            this._udsRepositoryService = new UDSRepositoryService(udsRepositoryConfiguration);
            var workflowActivityConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowActivity");
            this._workflowActivityService = new WorkflowActivityService(workflowActivityConfiguration);
            var conservationConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Conservation");
            this._conservationService = new ConservationService(conservationConfiguration);
            var fascicleDocumentConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.FASCICLE_DOCUMENT_TYPE_NAME);
            this._fascicleDocumentService = new FascicleDocumentService(fascicleDocumentConfiguration);
            var fascicleDocumentUnitConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.FASCICLE_DOCUMENTUNIT_TYPE_NAME);
            this._fascicleDocumentUnitService = new FascicleDocumentUnitService(fascicleDocumentUnitConfiguration);
            var workflowRepositoriyConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowRepository");
            this._workflowRepositoriyService = new WorkflowRepositoryService(workflowRepositoriyConfiguration);
            this._pnlButtons = $("#".concat(this.pnlButtonsId));
            this._pnlButtons.hide();
            //Bind evento onRebind dello user control uscFascicolo
            $("#".concat(this.uscFascicoloId)).bind(UscFascicolo.REBIND_EVENT, function (args) {
                _this.sendRefreshUDRequest();
            });
            $("#".concat(this.uscFascFoldersId)).bind(UscFascicleFolders.ROOT_NODE_CLICK, function (args) { return _this.folderNodeClickCallback(false); })
                .bind(UscFascicleFolders.FASCICLE_TREE_NODE_CLICK, function (args) { return _this.folderNodeClickCallback(true); })
                .bind(UscFascicleFolders.SUBFASCICLE_TREE_NODE_CLICK, function (args) { return _this.folderNodeClickCallback(true); });
            this.setButtonEnable(false);
            this._loadingPanel.show(this.pageContentId);
            this.service.getFascicle(this.currentFascicleId, function (data) {
                if (data == null)
                    return;
                _this._fascicleModel = data;
                var wfCheckActivityAction;
                if (_this.workflowEnabled && _this.workflowActivityId) {
                    wfCheckActivityAction = function () { return _this._handlerManager.manageHandlingWorkflowWithActivity(_this.workflowActivityId); };
                }
                else {
                    wfCheckActivityAction = function () { return _this._handlerManager.manageHandlingWorkflow(_this.currentFascicleId, Environment.Fascicle); };
                }
                wfCheckActivityAction()
                    .done(function (idActivity) {
                    _this.workflowActivityId = idActivity;
                    _this.initializeRights(_this._fascicleModel)
                        .done(function (fascicleRights) {
                        _this._fascicleRights = fascicleRights;
                        _this.initializePageByRigths(fascicleRights);
                    })
                        .fail(function (exception) {
                        _this._loadingPanel.hide(_this.pageContentId);
                        $("#".concat(_this.pageContentId)).hide();
                        _this.showNotificationException(_this.uscNotificationId, exception, "Errore nel caricamento delle attività del fusso di lavoro associate al fascicolo.");
                    });
                })
                    .fail(function (exception) {
                    _this._loadingPanel.hide(_this.pageContentId);
                    $("#".concat(_this.pageContentId)).hide();
                    _this.showNotificationException(_this.uscNotificationId, exception, "Errore nel caricamento delle attività del fusso di lavoro associate al fascicolo.");
                });
            }, function (exception) {
                _this._loadingPanel.hide(_this.pageContentId);
                $("#".concat(_this.pageContentId)).hide();
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        /**
         *------------------------- Methods -----------------------------
         */
        FascVisualizza.prototype.initializeRights = function (fascicle) {
            var promise = $.Deferred();
            var fascicleRights = {};
            var fascicleRule = new FascicleRights(fascicle, this._serviceConfigurations);
            $.when(fascicleRule.hasViewableRight(), fascicleRule.hasManageableRight(), fascicleRule.isManager(), fascicleRule.isProcedureSecretary(), this.hasAuthorizedWorkflows(), this.hasFascicolatedUD(fascicle.UniqueId))
                .done(function (view, edit, manager, secretary, wf, ud) {
                fascicleRights.IsViewable = view;
                fascicleRights.IsEditable = edit;
                fascicleRights.IsManageable = edit;
                fascicleRights.IsManager = manager;
                fascicleRights.IsSecretary = secretary;
                fascicleRights.HasAuthorizedWorkflows = wf;
                fascicleRights.HasFascicolatedUD = ud;
                promise.resolve(fascicleRights);
            })
                .fail(function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        FascVisualizza.prototype.hasAuthorizedWorkflows = function () {
            var promise = $.Deferred();
            this._workflowRepositoriyService.hasAuthorizedWorkflowRepositories(Environment.Fascicle, false, function (data) { return promise.resolve(data); }, function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        FascVisualizza.prototype.hasFascicolatedUD = function (idFascicle) {
            var promise = $.Deferred();
            this.service.hasFascicolatedDocumentUnits(idFascicle, function (data) { return promise.resolve(data); }, function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        /**
         * Inizializza lo user control del sommario di fascicolo
         */
        FascVisualizza.prototype.loadFascicoloSummary = function () {
            var _this = this;
            var uscFascicolo = $("#".concat(this.uscFascicoloId)).data();
            if (!jQuery.isEmptyObject(uscFascicolo)) {
                uscFascicolo.workflowActivityId = this.workflowActivityId;
                $("#".concat(this.uscFascicoloId)).bind(UscFascicolo.DATA_LOADED_EVENT, function (args) {
                    _this.sendRefreshUDRequest();
                });
                uscFascicolo.loadData(this._fascicleModel);
            }
        };
        /**
         * Metodo di callback inizializzazione
         * @param viewRights
         */
        FascVisualizza.prototype.initializePageByRigths = function (viewRights) {
            var _this = this;
            this._fascicleRights = viewRights;
            if (!viewRights.IsViewable) {
                this.setButtonEnable(false);
                $("#".concat(this.pageContentId)).hide();
                this._loadingPanel.hide(this.pageContentId);
                this.showNotificationMessage(this.uscNotificationId, "Fascicolo n. ".concat(this._fascicleModel.Title, "<br />Impossibile visualizzare il fascicolo. Non si dispone dei diritti necessari."));
                return;
            }
            //Bind evento onLoaded dello user control uscFascicolo
            $("#".concat(this.uscFascicoloId)).bind(UscFascicolo.LOADED_EVENT, function (args) {
                _this.loadFascicoloSummary();
            });
            this.loadFascicoloSummary();
            var procedureFascicleType = FascicleType[FascicleType.Procedure];
            var isProcedureFascicle = this._fascicleModel.FascicleType.toString() == procedureFascicleType;
            var isPeriodicFascicle = this._fascicleModel.FascicleType.toString() == FascicleType[FascicleType.Period];
            if (!($.type(this._fascicleModel.FascicleType) === "string")) {
                this._fascicleModel.FascicleType = FascicleType[this._fascicleModel.FascicleType.toString()];
                isProcedureFascicle = this._fascicleModel.FascicleType.toString() == procedureFascicleType;
                isPeriodicFascicle = this._fascicleModel.FascicleType.toString() == FascicleType[FascicleType.Period];
            }
            var isClosed = this._fascicleModel.EndDate != null;
            this.setBtnCloseVisibility();
            this.setBtnOpenVisibility();
            this._btnInsert.set_visible(!isClosed);
            this._btnMove.set_visible(!isClosed);
            this._btnRemove.set_visible(!isClosed);
            this._btnSendToRoles.set_visible(!isClosed);
            this._btnEdit.set_visible(!isClosed || isPeriodicFascicle);
            this._btnLink.set_visible(!isClosed);
            this._btnAutorizza.set_visible(!isClosed);
            this._btnDocuments.set_visible(viewRights.IsViewable || isPeriodicFascicle);
            var isWorkflowEnabled = this.workflowEnabled && isProcedureFascicle;
            this._btnWorkflow.set_visible(isWorkflowEnabled && !isClosed && viewRights.HasAuthorizedWorkflows && viewRights.IsManageable);
            this._btnComplete.set_visible(!isClosed && this.hasActiveWorkflowActivityWorkflow());
            this._btnWorkflowLogs.set_visible(viewRights.IsManager || viewRights.IsSecretary);
            this._btnFascicleLog.set_visible(viewRights.IsManager || viewRights.IsSecretary);
            this._btnUndo.set_visible(!isClosed && (viewRights.IsManager || viewRights.IsSecretary));
            if (!isClosed) {
                this._btnEdit.set_visible((viewRights.IsEditable || viewRights.IsManager) && !isPeriodicFascicle);
                this._btnLink.set_visible(viewRights.IsManageable);
                this._btnAutorizza.set_visible((viewRights.IsEditable || viewRights.IsManager) && (isProcedureFascicle || isPeriodicFascicle));
                this._btnSendToRoles.set_visible(viewRights.IsViewable && this._fascicleModel.FascicleRoles.length != 0);
                this._btnInsert.set_visible(viewRights.IsManageable || viewRights.IsManager);
                this._btnMove.set_visible(viewRights.IsManageable || viewRights.IsManager);
                this._btnRemove.set_visible(viewRights.IsManageable || viewRights.IsManager);
                var uscFascFolder = $("#".concat(this.uscFascFoldersId)).data();
                if (!jQuery.isEmptyObject(uscFascFolder)) {
                    uscFascFolder.setManageFascicleFolderVisibility(viewRights.IsManageable || viewRights.IsManager);
                }
                if (this.hasActiveWorkflowActivityWorkflow()) {
                    this._workflowActivityService.getWorkflowActivity(this.workflowActivityId, function (dataWorkflow) {
                        if (dataWorkflow) {
                            _this._workflowActivity = dataWorkflow;
                            var completeWorkflowUserEnabled = false;
                            var isHandler = false;
                            var isHandlingDocumentEnabled = false;
                            var isActivityClosed = true;
                            if (_this._workflowActivity != null && _this._workflowActivity.WorkflowAuthorizations != null) {
                                var userAuthorization = _this.filterWorkflowAuthorizationsByAccount(_this._workflowActivity.WorkflowAuthorizations, _this.currentUser);
                                var status_1 = parseInt(WorkflowStatus[_this._workflowActivity.Status]);
                                if (isNaN(status_1)) {
                                    status_1 = WorkflowStatus[_this._workflowActivity.Status.toString()];
                                }
                                isActivityClosed = (status_1 == WorkflowStatus.Done);
                                completeWorkflowUserEnabled = ((userAuthorization.length > 0) && !isActivityClosed);
                                isHandler = userAuthorization.filter(function (item) {
                                    if (item.IsHandler == true) {
                                        return item;
                                    }
                                }).length > 0;
                            }
                            isHandlingDocumentEnabled = (viewRights.IsManageable || (isHandler && completeWorkflowUserEnabled && _this.hasActiveWorkflowActivityWorkflow()));
                            _this._btnComplete.set_visible(_this.hasActiveWorkflowActivityWorkflow() && completeWorkflowUserEnabled && isWorkflowEnabled);
                            _this._btnInsert.set_visible(isHandlingDocumentEnabled);
                            _this._btnMove.set_visible(isHandlingDocumentEnabled);
                            _this._btnRemove.set_visible(isHandlingDocumentEnabled);
                            var uscFascFolder_1 = $("#".concat(_this.uscFascFoldersId)).data();
                            if (!jQuery.isEmptyObject(uscFascFolder_1)) {
                                uscFascFolder_1.setManageFascicleFolderVisibility(isHandlingDocumentEnabled);
                            }
                            _this._btnWorkflow.set_visible(isWorkflowEnabled && !isClosed && viewRights.HasAuthorizedWorkflows && viewRights.IsManageable && isActivityClosed);
                        }
                    }, function (exception) {
                        _this._loadingPanel.hide(_this.pageContentId);
                        $("#".concat(_this.pageContentId)).hide();
                        _this.showNotificationException(_this.uscNotificationId, exception);
                    });
                }
            }
            else {
                var uscFascFolder = $("#".concat(this.uscFascFoldersId)).data();
                if (!jQuery.isEmptyObject(uscFascFolder)) {
                    uscFascFolder.setCloseAttributeFascicleFolder();
                }
            }
            this._pnlButtons.show();
        };
        /**
         * Apre una nuova nuova RadWindow
         * @param url
         * @param name
         * @param width
         * @param height
         */
        FascVisualizza.prototype.openWindow = function (url, name, width, height) {
            var manager = $find(this.radWindowManagerCollegamentiId);
            var wnd = manager.open(url, name, null);
            wnd.setSize(width, height);
            wnd.set_modal(true);
            wnd.center();
            return false;
        };
        FascVisualizza.prototype.removeFascicleUD = function (model, service) {
            var _this = this;
            service.deleteFascicleUD(model, function (data) {
                _this.sendRefreshUDRequest();
            }, function (exception) {
                _this._loadingPanel.hide(_this.pageContentId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        /**
         * Imposta l'attributo enable dei pulsanti
         * @param value
         */
        FascVisualizza.prototype.setButtonEnable = function (value) {
            this._btnEdit.set_enabled(value);
            this._btnInsert.set_enabled(value && this.getSelectedFascicleFolder() != undefined);
            this._btnMove.set_enabled(value && this._btnInsert.get_enabled());
            this._btnRemove.set_enabled(value);
            this._btnDocuments.set_enabled(value);
            this._btnClose.set_enabled(value);
            this._btnLink.set_enabled(value);
            this._btnAutorizza.set_enabled(value);
            this._btnSendToRoles.set_enabled(value);
            this._btnWorkflow.set_enabled(value);
            this._btnComplete.set_enabled(value);
            this._btnWorkflowLogs.set_enabled(value);
            this._btnUndo.set_enabled(value);
        };
        FascVisualizza.prototype.loadActiveWorkflowActivities = function () {
            var _this = this;
            var promise = $.Deferred();
            //se non ho in query string l'id dell'attività, cerco se il fascicolo ha attività da completare        
            if (!this.hasActiveWorkflowActivityWorkflow()) {
                try {
                    (this._workflowActivityService).getActiveActivitiesByReferenceIdAndEnvironment(this.currentFascicleId, Environment.Fascicle, function (data) {
                        if (data == undefined) {
                            promise.resolve();
                            return;
                        }
                        //TODO: per ora prendo la prima, ma dopo dovremo fare una gestione per selezionare quale attività
                        _this._workflowActivity = data;
                        _this.workflowActivityId = data.UniqueId;
                        promise.resolve();
                    }, function (exception) {
                        promise.reject(exception);
                    });
                }
                catch (error) {
                    console.log(error.stack);
                    promise.reject(error);
                }
            }
            else {
                promise.resolve();
            }
            return promise.promise();
        };
        FascVisualizza.prototype.setBtnCloseVisibility = function () {
            var isProcedureFascicle = this._fascicleModel.FascicleType == FascicleType.Procedure;
            var isPeriodicFascicle = this._fascicleModel.FascicleType == FascicleType.Period;
            var isClosed = this._fascicleModel.EndDate != null;
            var isClosable = ((this._fascicleRights.IsManager || this._fascicleRights.IsSecretary) && !isClosed && !isPeriodicFascicle);
            if (isProcedureFascicle) {
                isClosable = isClosable && this._fascicleRights.HasFascicolatedUD;
            }
            this._btnClose.set_visible(isClosable);
        };
        FascVisualizza.prototype.setBtnOpenVisibility = function () {
            var isClosed = this._fascicleModel.EndDate != null;
            var isVisible = ((this._fascicleRights.IsManager || this._fascicleRights.IsSecretary) && isClosed);
            this._btnOpen.set_visible(isVisible);
            this._btnOpen.set_enabled(false);
            if (isVisible) {
                this.getIdConservation();
            }
        };
        FascVisualizza.prototype.getSelectedFascicleFolder = function () {
            var uscFascicolo = $("#".concat(this.uscFascicoloId)).data();
            if (!jQuery.isEmptyObject(uscFascicolo)) {
                var selectedFolder = uscFascicolo.getSelectedFascicleFolder();
                if (selectedFolder && selectedFolder.UniqueId == this._fascicleModel.UniqueId) {
                    return undefined;
                }
                return selectedFolder;
            }
            return undefined;
        };
        FascVisualizza.prototype.getIdConservation = function () {
            var _this = this;
            this._conservationService.getById(this.currentFascicleId, function (data) {
                if (data == null) {
                    _this._btnOpen.set_enabled(true);
                    return;
                }
                _this._btnOpen.set_enabled(false);
                _this._btnOpen.set_toolTip("Il fascicolo è stato conservato. Funzionalità disabilitata");
            }, function (exception) {
                _this.setButtonEnable(false);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        FascVisualizza.prototype.openFascicleClosed = function () {
            var _this = this;
            var fascicle = {};
            fascicle.UniqueId = this.currentFascicleId;
            fascicle.Title = this._fascicleModel.Title;
            this.service.updateFascicle(fascicle, UpdateActionType.OpenFascicleClosed, function (data) {
                if (data == null)
                    return;
                _this._btnOpen.set_enabled(false);
                window.location.href = "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=".concat(fascicle.UniqueId);
            }, function (exception) {
                _this._loadingPanel.hide(_this.currentPageId);
                _this.showNotificationException(_this.uscNotificationId, exception);
                _this._btnOpen.set_enabled(false);
                _this.setButtonEnable(false);
            });
        };
        FascVisualizza.prototype.undoFascicle = function () {
            var _this = this;
            var fascicle = {};
            fascicle.UniqueId = this.currentFascicleId;
            this.service.deleteFascicle(fascicle, function (data) {
                if (data == null)
                    return;
                _this._btnUndo.set_enabled(false);
                window.location.href = "../Fasc/FascRicerca.aspx?Type=Fasc";
            }, function (exception) {
                _this._loadingPanel.hide(_this.currentPageId);
                _this.showNotificationException(_this.uscNotificationId, exception);
                _this.setButtonEnable(false);
            });
        };
        return FascVisualizza;
    }(FascicleBase));
    return FascVisualizza;
});
//# sourceMappingURL=FascVisualizza.js.map