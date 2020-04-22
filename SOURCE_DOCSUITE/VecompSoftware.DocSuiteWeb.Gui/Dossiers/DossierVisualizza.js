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
define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "Dossiers/DossierBase", "App/Services/Dossiers/DossierFolderService", "UserControl/uscDossierFolders", "App/Services/Dossiers/DossierDocumentService", "App/Services/Fascicles/FascicleService", "App/Services/Workflows/WorkflowActivityService", "App/Services/Workflows/WorkflowAuthorizationService", "App/Models/Workflows/WorkflowStatus", "App/Models/Environment", "App/Managers/HandlerWorkflowManager", "../Workflows/StartWorkflow"], function (require, exports, ServiceConfigurationHelper, DossierBase, DossierFolderService, UscDossierFolders, DossierDocumentService, FascicleService, WorkflowActivityService, WorkflowAuthorizationService, WorkflowStatus, Environment, HandlerWorkflowManager, StartWorkflow) {
    var DossierVisualizza = /** @class */ (function (_super) {
        __extends(DossierVisualizza, _super);
        /**
        * Costruttore
        */
        function DossierVisualizza(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, DossierBase.DOSSIER_TYPE_NAME)) || this;
            _this._isManager = false;
            /**
            *------------------------- Events -----------------------------
            */
            /**
           * Evento scatenato al click del pulsante Modifica
           * @method
           * @param sender
           * @param eventArgs
           * @returns
           */
            _this.btnModifica_OnClick = function (sender, eventArgs) {
                _this._loadingPanel.show(_this.splContentId);
                _this.setButtonEnable(false);
                window.location.href = "DossierModifica.aspx?Type=Dossier&IdDossier=".concat(_this.currentDossierId);
            };
            /**
            * Evento al click del pulsante di Avvio Workflow
            * @param sender
            * @param args
            */
            _this.btnWorkflow_OnClick = function (sender, args) {
                args.set_cancel(true);
                sessionStorage.setItem(StartWorkflow.SESSION_KEY_REFERENCE_MODEL, JSON.stringify(_this._DossierModel));
                sessionStorage.setItem(StartWorkflow.SESSION_KEY_REFERENCE_ID, _this.currentDossierId);
                sessionStorage.setItem(StartWorkflow.SESSION_KEY_REFERENCE_TITLE, _this._DossierModel.Year.toString().concat("/", _this._DossierModel.Number.toString()));
                var url = "../Workflows/StartWorkflow.aspx?Type=Dossier".concat("&ManagerID=", _this.radWindowManagerCollegamentiId, "&DSWEnvironment=Dossier&Callback=", window.location.href);
                return _this.openWindow(url, "windowStartWorkflow", 730, 550);
            };
            /**
            * Evento scatenato al click del pulsante Documenti
            * @method
            * @param sender
            * @param eventArgs
            * @returns
            */
            _this.btnDocumenti_OnClicked = function (sender, eventArgs) {
                _this._loadingPanel.show(_this.splContentId);
                _this.setButtonEnable(false);
                window.location.href = "../Viewers/DossierViewer.aspx?Type=Dossier&IdDossier=".concat(_this.currentDossierId);
            };
            /**
            * Evento al click del pulsante di Log
            * @param sender
            * @param args
            */
            _this.btnLog_OnClick = function (sender, args) {
                args.set_cancel(true);
                _this._loadingPanel.show(_this.splContentId);
                _this.setButtonEnable(false);
                window.location.href = "DossierLog.aspx?Type=Dossier&IdDossier=".concat(_this.currentDossierId, "&DossierTitle=", _this._DossierModel.Year.toString(), "/", _this._DossierModel.Number.toString());
            };
            /**
            * Evento al click del pulsante CompletaWorkflow
            * @param sender
            * @param args
            */
            _this.btnCompleteWorkflow_OnClick = function (sender, args) {
                args.set_cancel(true);
                var url = "../Workflows/CompleteWorkflow.aspx?=Dossier&IdDossier=".concat(_this.currentDossierId, "&IdWorkflowActivity=", _this.workflowActivityId);
                return _this.openWindow(url, "windowCompleteWorkflow", 700, 500);
            };
            /**
             * Evento di chiusura della finestra di Workflow
             */
            _this.onWorkflowCloseWindow = function (sender, args) {
                if (args.get_argument()) {
                    var result = {};
                    result = args.get_argument();
                    _this._notificationInfo.show();
                    _this._notificationInfo.set_text(result.ActionName);
                    _this.workflowActivityId = result.Value ? result.Value[0] : null;
                    _this._isFromWorkflow = !String.isNullOrEmpty(_this.workflowActivityId);
                    _this._workflowActivity = null;
                    _this.workflowCallback();
                }
            };
            /**
        * Evento scatenato al click del pulsante Inserti
        * @method
        * @param sender
        * @param eventArgs
        * @returns
        */
            _this.btnInserti_OnClick = function (sender, eventArgs) {
                _this._loadingPanel.show(_this.splContentId);
                _this.setButtonEnable(false);
                window.location.href = "DossierMiscellanea.aspx?Type=Dossier&IdDossier=".concat(_this.currentDossierId);
            };
            /**
            * funzione per discriminare l'autorizzazione al workflow
            */
            _this.filterWorkflowAuthorizationsByAccount = function (arr, criteria) {
                return arr.filter(function (item) {
                    if (item.Account.toLowerCase() == criteria.toLowerCase()) {
                        return item;
                    }
                });
            };
            /*
            * Funzione chiamata al callback di chiusura della finestra di gestione del workflow
            */
            _this.workflowCallback = function () {
                _this.setButtonEnable(false);
                _this.loadData();
                _this.setWorkflowConfiguration();
                _this._loadingPanel.hide(_this.splContentId);
                if (_this.workflowActivityId) {
                    _this._workflowActivityService.getWorkflowActivity(_this.workflowActivityId, function (dataWorkflow) {
                        if (dataWorkflow) {
                            _this._workflowActivity = dataWorkflow;
                        }
                    });
                }
            };
            _this._serviceConfigurations = serviceConfigurations;
            _this._handlerManager = new HandlerWorkflowManager(serviceConfigurations);
            $(document).ready(function () {
            });
            return _this;
        }
        /**
        * ------------------------- Methods -----------------------------
        */
        /**
        * Initialize
        * La visibilità dei pulsanti di WF va riattivata quando si attiverà il modulo dei Wf su Dossier presso i clienti
        */
        DossierVisualizza.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            this._isWorkflowEnabled = this.workflowEnabled && this.workflowEnabled.toLowerCase() == 'true';
            this._ajaxManager = $find(this.ajaxManagerId);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._notificationInfo = $find(this.radNotificationId);
            this._btnDocumenti = $find(this.btnDocumentiId);
            this._windowStartWorkflow = $find(this.windowStartWorkflowId);
            this._btnInserti = $find(this.btnInsertiId);
            this._windowStartWorkflow.add_close((this.onWorkflowCloseWindow));
            this._windowCompleteWorkflow = $find(this.windowCompleteWorkflowId);
            this._windowCompleteWorkflow.add_close((this.onWorkflowCloseWindow));
            this._btnDocumenti.add_clicked(this.btnDocumenti_OnClicked);
            this._btnClose = $find(this.btnCloseId);
            this._btnModifica = $find(this.btnModificaId);
            this._btnWorkflow = $find(this.btnAvviaWorkflowId);
            this._btnCompleteWorkflow = $find(this.btnCompleteWorkflowId);
            this._btnAutorizza = $find(this.btnAutorizzaId);
            this._btnLog = $find(this.btnLogId);
            this._DossierModel = {};
            this._DossierRoles = new Array();
            this._DossierContacts = new Array();
            this._DossierFolders = new Array();
            this.setButtonEnable(false);
            this._btnModifica.add_clicking(this.btnModifica_OnClick);
            this._btnInserti.add_clicking(this.btnInserti_OnClick);
            //this._btnWorkflow.add_clicking(this.btnWorkflow_OnClick);
            //this._btnCompleteWorkflow.add_clicking(this.btnCompleteWorkflow_OnClick);
            this._btnLog.add_clicking(this.btnLog_OnClick);
            //this._isFromWorkflow = !String.isNullOrEmpty(this.workflowActivityId);
            //this._btnCompleteWorkflow.set_visible(this._isFromWorkflow && this._isWorkflowEnabled);
            //this._btnCompleteWorkflow.set_enabled(false);
            //this._btnWorkflow.set_visible(!this._isFromWorkflow && this._isWorkflowEnabled);
            if (!this.logEnabled) {
                this._btnLog.set_visible(false);
            }
            this._fascPage = $find(this.fascPaneId);
            this._dossierPage = $find(this.dossierPageId);
            this._fascPage.collapse(Telerik.Web.UI.SplitterDirection.Backward);
            var dossierFolderConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, DossierBase.DOSSIERFOLDER_TYPE_NAME);
            this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);
            var dossierDocumentConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, DossierBase.DOSSIERDOCUMENT_TYPE_NAME);
            this._dossierDocumentService = new DossierDocumentService(dossierDocumentConfiguration);
            var fascicleConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Fascicle");
            this._fascicleService = new FascicleService(fascicleConfiguration);
            var workflowActivityConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowActivity");
            this._workflowActivityService = new WorkflowActivityService(workflowActivityConfiguration);
            var WorkflowAuthorizationConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowAuthorization");
            this._workflowAuthorizationService = new WorkflowAuthorizationService(WorkflowAuthorizationConfiguration);
            $("#".concat(this.uscDossierFoldersId)).on(UscDossierFolders.FASCICLE_TREE_NODE_CLICK, function (args, data) {
                var url = "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=".concat(data);
                _this._fascPage.set_contentUrl(url);
                _this._dossierPage.collapse(Telerik.Web.UI.SplitterDirection.Forward);
                _this._fascPage.expand(Telerik.Web.UI.SplitterDirection.Backward);
                _this._dossierPage.collapse(Telerik.Web.UI.SplitterDirection.Forward);
                _this._fascPage.expand(Telerik.Web.UI.SplitterDirection.Backward);
            });
            $("#".concat(this.uscDossierFoldersId)).on(UscDossierFolders.ROOT_NODE_CLICK, function (args) {
                _this._fascPage.collapse(Telerik.Web.UI.SplitterDirection.Backward);
                _this._dossierPage.expand(Telerik.Web.UI.SplitterDirection.Forward);
                _this._fascPage.collapse(Telerik.Web.UI.SplitterDirection.Backward);
                _this._dossierPage.expand(Telerik.Web.UI.SplitterDirection.Forward);
            });
            this.service.hasRootNode(this.currentDossierId, function (data) {
                if (data) {
                    _this.service.isManageableDossier(_this.currentDossierId, function (data) {
                        if (data) {
                            _this._isManager = true;
                            if (_this._isWorkflowEnabled) {
                                _this.setWorkflowConfiguration();
                            }
                            else {
                                _this.loadData();
                            }
                        }
                        else {
                            _this.service.isViewableDossier(_this.currentDossierId, function (data) {
                                if (data) {
                                    _this.loadData();
                                    _this._btnModifica.set_visible(false);
                                    //this._btnClose.set_visible(false);
                                    //this._btnWorkflow.set_visible(false);
                                    //this._btnCompleteWorkflow.set_visible(false);
                                    _this._btnLog.set_visible(false);
                                    _this._btnInserti.set_visible(false);
                                }
                                else {
                                    _this.setButtonEnable(false);
                                    $("#".concat(_this.splContentId)).hide();
                                    _this._loadingPanel.hide(_this.splContentId);
                                    _this.hideUscLoadingPanels();
                                    _this.showNotificationMessage(_this.uscNotificationId, "Impossibile visualizzare il Dossier.<br/> Non si dispone dei diritti necessari.");
                                }
                                ;
                            }, function (exception) {
                                _this._loadingPanel.hide(_this.splContentId);
                                _this.hideUscLoadingPanels();
                                _this.showNotificationException(_this.uscNotificationId, exception);
                            });
                        }
                    }, function (exception) {
                        _this._loadingPanel.hide(_this.splContentId);
                        _this.showNotificationException(_this.uscNotificationId, exception);
                    });
                }
                else {
                    _this.setButtonEnable(false);
                    _this.hideUscLoadingPanels();
                    $("#".concat(_this.splContentId)).hide();
                    _this._loadingPanel.hide(_this.splContentId);
                    _this.showNotificationMessage(_this.uscNotificationId, "Impossibile operare sul Dossier selezionato.<br/> Contattare l'assistenza.");
                }
            }, function (exception) {
                _this._loadingPanel.hide(_this.splContentId);
                _this.hideUscLoadingPanels();
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        /**
       * funzione per nascondere il loading panel del pannello dell'alberatura delle cartelle'
       */
        DossierVisualizza.prototype.hideUscLoadingPanels = function () {
            var uscDossierFolders = $("#".concat(this.uscDossierFoldersId)).data();
            if (!jQuery.isEmptyObject(uscDossierFolders)) {
                uscDossierFolders.hideLoadingPanel();
            }
            var uscDossier = $("#".concat(this.uscDossierId)).data();
            if (!jQuery.isEmptyObject(uscDossier)) {
                uscDossier.hideLoadingPanel();
            }
        };
        /*
        * Carico i dati nello USC dei dossier
        */
        DossierVisualizza.prototype.loadUscDossier = function () {
            var _this = this;
            var promise = $.Deferred();
            $.when(this.loadDossier(), this.loadRoles(), this.loadContacts(), this.loadInserts()).done(function () {
                _this._DossierModel.Roles = _this._DossierRoles;
                _this._DossierModel.Contacts = _this._DossierContacts;
                _this._DossierModel.Documents = _this._DossierDocuments;
                _this.loadDossierVisualize();
                promise.resolve();
            }).fail(function (exception) {
                promise.reject(exception);
            });
            return promise.promise();
        };
        /*
        * funzione di caricamento dei dati
        */
        DossierVisualizza.prototype.loadData = function () {
            var _this = this;
            $.when(this.loadUscDossier(), this.loadFolders()).done(function () {
                _this.setButtonEnable(true);
            }).fail(function (exception) {
                _this.showNotificationException(_this.uscNotificationId, exception, "Errore nel caricamento del Dossier.");
            });
        };
        /*
        * Carico il dossier corrente senza navigation properties
        */
        DossierVisualizza.prototype.loadDossier = function () {
            var _this = this;
            var promise = $.Deferred();
            try {
                this.service.getDossier(this.currentDossierId, function (data) {
                    try {
                        if (data == undefined) {
                            promise.resolve();
                            return;
                        }
                        _this._DossierModel = data;
                        promise.resolve();
                    }
                    catch (error) {
                        console.log(JSON.stringify(error));
                        promise.reject(error);
                    }
                }, function (exception) {
                    promise.reject(exception);
                });
            }
            catch (error) {
                console.log(error.stack);
                promise.reject(error);
            }
            return promise.promise();
        };
        /*
        * Carico i settori del Dossier
        */
        DossierVisualizza.prototype.loadRoles = function () {
            var _this = this;
            var promise = $.Deferred();
            try {
                this.service.getDossierRoles(this.currentDossierId, function (data) {
                    try {
                        if (!data) {
                            promise.resolve();
                            return;
                        }
                        //ritorna solo quello attivo
                        _this._DossierRoles = data;
                        promise.resolve();
                    }
                    catch (error) {
                        console.log(JSON.stringify(error));
                        promise.reject(error);
                    }
                }, function (exception) {
                    promise.reject(exception);
                });
            }
            catch (error) {
                console.log(error.stack);
                promise.reject(error);
            }
            return promise.promise();
        };
        /**
        * carico i contatti del Dossier
        */
        DossierVisualizza.prototype.loadContacts = function () {
            var _this = this;
            var promise = $.Deferred();
            try {
                this.service.getDossierContacts(this.currentDossierId, function (data) {
                    try {
                        if (!data) {
                            promise.resolve();
                            return;
                        }
                        _this._DossierContacts = data;
                        promise.resolve();
                    }
                    catch (error) {
                        console.log(JSON.stringify(error));
                        promise.reject(error);
                    }
                }, function (exception) {
                    promise.reject(exception);
                });
            }
            catch (error) {
                console.log(error.stack);
                promise.reject(error);
            }
            return promise.promise();
        };
        /*
        * Imposto la visibilità dei bottoni per l'utilizzo del workflow
        */
        DossierVisualizza.prototype.setWorkflowConfiguration = function () {
            var _this = this;
            if (this.workflowActivityId) {
                $.when(this._handlerManager.manageHandlingWorkflowWithActivity(this.workflowActivityId)).done(function (idActivity) {
                    _this.workflowActivityId = idActivity;
                    _this.setWorkflowButtons();
                }).fail(function (exception) {
                    _this.showNotificationException(_this.uscNotificationId, exception, "Errore nel caricamento delle attività del fusso di lavoro associate al fascicolo.");
                });
            }
            else {
                $.when(this._handlerManager.manageHandlingWorkflow(this.currentDossierId, Environment.Dossier)).done(function (idActivity) {
                    _this.workflowActivityId = idActivity;
                    _this.setWorkflowButtons();
                }).fail(function (exception) {
                    _this.showNotificationException(_this.uscNotificationId, exception, "Errore nel caricamento delle attività del fusso di lavoro associate al fascicolo.");
                });
            }
        };
        /**
         * Imposto i bottoni del workflow a seconda delle mie autorizzazioni
         */
        DossierVisualizza.prototype.setWorkflowButtons = function () {
            var _this = this;
            var isHandlingDocumentEnabled = false;
            var completeWorkflowUserEnabled = false;
            this._workflowActivityService.getWorkflowActivity(this.workflowActivityId, function (workflowActivity) {
                if (workflowActivity) {
                    _this._workflowActivity = workflowActivity;
                    var userAuthorization = _this.filterWorkflowAuthorizationsByAccount(_this._workflowActivity.WorkflowAuthorizations, _this.currentUser);
                    var status_1 = parseInt(WorkflowStatus[_this._workflowActivity.Status]);
                    if (isNaN(status_1)) {
                        status_1 = WorkflowStatus[_this._workflowActivity.Status.toString()];
                    }
                    completeWorkflowUserEnabled = ((userAuthorization.length > 0) && status_1 != WorkflowStatus.Done);
                    //this._btnCompleteWorkflow.set_visible(completeWorkflowUserEnabled);
                    //this._btnCompleteWorkflow.set_enabled(completeWorkflowUserEnabled);
                    //this._btnWorkflow.set_visible(!completeWorkflowUserEnabled)
                }
                _this.loadData();
            }, function (exception) {
                _this._loadingPanel.hide(_this.splContentId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        /**
        * carico le cartelle del Dossier
       */
        DossierVisualizza.prototype.loadFolders = function () {
            var _this = this;
            var promise = $.Deferred();
            try {
                this._dossierFolderService.getChildren(this.currentDossierId, 0, function (data) {
                    try {
                        if (!data) {
                            promise.resolve();
                            return;
                        }
                        _this._DossierFolders = data;
                        _this.loadDossierFoldersPanel();
                        promise.resolve();
                    }
                    catch (error) {
                        console.log(JSON.stringify(error));
                        promise.reject(error);
                    }
                }, function (exception) {
                    promise.reject(exception);
                });
            }
            catch (error) {
                console.log(error.stack);
                promise.reject(error);
            }
            return promise.promise();
        };
        /**
       * carico gli inserti del Dossier
       */
        DossierVisualizza.prototype.loadInserts = function () {
            var _this = this;
            var promise = $.Deferred();
            try {
                this._dossierDocumentService.getDossierDocuments(this.currentDossierId, function (data) {
                    try {
                        if (!data) {
                            promise.resolve();
                            return;
                        }
                        _this._DossierDocuments = data;
                        promise.resolve();
                    }
                    catch (error) {
                        console.log(JSON.stringify(error));
                        promise.reject(error);
                    }
                }, function (exception) {
                    promise.reject(exception);
                });
            }
            catch (error) {
                console.log(error.stack);
                promise.reject(error);
            }
            return promise.promise();
        };
        /**
        * Inizializzo lo user control del sommario di Dossier
        */
        DossierVisualizza.prototype.loadDossierVisualize = function () {
            var uscDossier = $("#".concat(this.uscDossierId)).data();
            if (!jQuery.isEmptyObject(uscDossier)) {
                uscDossier.workflowActivityId = this.workflowActivityId;
                uscDossier.loadData(this._DossierModel);
            }
        };
        /**
        * Inizializzo lo user control delle cartelline
        */
        DossierVisualizza.prototype.loadDossierFoldersPanel = function () {
            var uscDossierFolders = $("#".concat(this.uscDossierFoldersId)).data();
            if (!jQuery.isEmptyObject(uscDossierFolders)) {
                uscDossierFolders.setRootNode(this.currentDossierTitle, this.currentDossierId);
                uscDossierFolders.loadNodes(this._DossierFolders);
                uscDossierFolders.setButtonVisibility(this._isManager);
            }
        };
        DossierVisualizza.prototype.setButtonEnable = function (value) {
            //this._btnWorkflow.set_enabled(value);
            this._btnDocumenti.set_enabled(value);
            this._btnModifica.set_enabled(value);
            //this._btnClose.set_enabled(value);
            //this._btnAutorizza.set_enabled(value);
            this._btnLog.set_enabled(value);
            //this._btnCompleteWorkflow.set_enabled(value);
            this._btnInserti.set_enabled(value);
            if (!this.miscellaneaLocationEnabled) {
                this._btnInserti.set_enabled(false);
                this._btnInserti.set_toolTip("Nessuna configurazione definita per l'inserimento degli inserti.Contattare Assistenza.");
            }
        };
        /**
         * Apre una nuova nuova RadWindow
        * @param url
        * @param name
        * @param width
        * @param height
        */
        DossierVisualizza.prototype.openWindow = function (url, name, width, height) {
            var manager = $find(this.radWindowManagerCollegamentiId);
            var wnd = manager.open(url, name, null);
            wnd.setSize(width, height);
            wnd.set_modal(true);
            wnd.center();
            return false;
        };
        return DossierVisualizza;
    }(DossierBase));
    return DossierVisualizza;
});
//# sourceMappingURL=DossierVisualizza.js.map