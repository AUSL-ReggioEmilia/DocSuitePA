/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
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
define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Services/Securities/DomainUserService", "Dossiers/DossierBase", "App/Services/Workflows/WorkflowActivityService", "App/Models/Workflows/WorkflowPropertyHelper", "App/Mappers/Workflows/WorkflowRoleModelMapper"], function (require, exports, ServiceConfigurationHelper, DomainUserService, DossierBase, WorkflowActivityService, WorkflowPropertyHelper, WorkflowRoleModelMapper) {
    var uscDossier = /** @class */ (function (_super) {
        __extends(uscDossier, _super);
        /**
        * Costruttore
        * @param webApiConfiguration
        */
        function uscDossier(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, DossierBase.DOSSIER_TYPE_NAME)) || this;
            /**
            * Metodo che nasconde il loading
            */
            _this.hideLoadingPanel = function () {
                _this._loadingPanel.hide(_this.pageId);
            };
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        /**
        * Inizializzazione
        */
        uscDossier.prototype.initialize = function () {
            this._lblDossierSubject = $("#".concat(this.lblDossierSubjectId));
            this._lblStartDate = $("#".concat(this.lblStartDateId));
            this._lblRegistrationUser = $("#".concat(this.lblRegistrationUserId));
            this._lblDossierNote = $("#".concat(this.lblDossierNoteId));
            this._lblYear = $("#".concat(this.lblYearId));
            this._lblNumber = $("#".concat(this.lblNumberId));
            this._lblModifiedUser = $("#".concat(this.lblModifiedUserId));
            this._lblContainer = $("#".concat(this.lblContainerId));
            this._lblWorkflowProposerRole = $("#".concat(this.lblWorkflowProposerRoleId));
            this._lblWorkflowHandlerUser = $("#".concat(this.lblWorkflowHandlerUserId));
            this._ajaxManager = $find(this.ajaxManagerId);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._loadingPanel.show(this.pageId);
            this._rowMetadataRepository = $("#".concat(this.rowMetadataId));
            this._rowMetadataRepository.hide();
            var domainUserConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DomainUserModel");
            this._domainUserService = new DomainUserService(domainUserConfiguration);
            var workflowActivityConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, 'WorkflowActivity');
            this._workflowActivityService = new WorkflowActivityService(workflowActivityConfiguration);
            this.bindLoaded();
        };
        /**
        *------------------------- Events -----------------------------
        */
        /**
        *------------------------- Methods -----------------------------
        */
        /**
        * Carica i dati dello user control
        */
        uscDossier.prototype.loadData = function (dossier) {
            var _this = this;
            if (dossier == null)
                return;
            this._domainUserService.getUser(dossier.RegistrationUser, function (user) {
                _this.setSummaryData(dossier);
                if (user) {
                    _this._lblRegistrationUser.html(user.DisplayName.concat(" ").concat(dossier.RegistrationDate));
                }
            }, function (exception) {
                _this.setSummaryData(dossier);
            });
            if (dossier.LastChangedUser) {
                this._domainUserService.getUser(dossier.LastChangedUser, function (user) {
                    if (user) {
                        _this._lblModifiedUser.html(user.DisplayName.concat(" ").concat(dossier.LastChangedDate));
                    }
                }, function (exception) {
                    console.log("Anomalia nel recupero del LastChangedUser del dossier.");
                });
            }
            if (this.metadataRepositoryEnabled && dossier.JsonMetadata) {
                this._rowMetadataRepository.show();
                var uscDynamicMetadataSummaryClient = $("#".concat(this.uscDynamicMetadataSummaryClientId)).data();
                if (!jQuery.isEmptyObject(uscDynamicMetadataSummaryClient)) {
                    uscDynamicMetadataSummaryClient.loadMetadatas(dossier.JsonMetadata);
                    sessionStorage.setItem("CurrentMetadataValues", dossier.JsonMetadata);
                }
            }
        };
        /**
     * Imposta i dati nel sommario
     * @param dossier
     */
        uscDossier.prototype.setSummaryData = function (dossier) {
            var _this = this;
            this._lblDossierSubject.html(dossier.Subject);
            this._lblDossierNote.html(dossier.Note);
            this._lblYear.html(dossier.Year.toString());
            this._lblNumber.html(dossier.Number);
            this._lblContainer.html(dossier.ContainerName);
            this._lblStartDate.html(dossier.FormattedStartDate);
            this._lblWorkflowHandlerUser.html("");
            this._lblWorkflowProposerRole.html("");
            $("#".concat(this.rowWorkflowProposerId)).hide();
            if (!String.isNullOrEmpty(this.workflowActivityId)) {
                this._workflowActivityService.getWorkflowActivity(this.workflowActivityId, function (data) {
                    if (data == null)
                        return;
                    _this._workflowActivity = data;
                    var subject;
                    var handler;
                    var role;
                    if (_this._workflowActivity.WorkflowAuthorizations) {
                        var authorization = _this._workflowActivity.WorkflowAuthorizations.filter(function (item) { if (item.IsHandler == true)
                            return item; })[0];
                        if (authorization) {
                            handler = authorization.Account;
                            _this._domainUserService.getUser(handler, function (user) {
                                if (user) {
                                    _this._lblWorkflowHandlerUser.html(user.DisplayName);
                                }
                            }, function (exception) {
                                _this._uscNotification = $("#".concat(_this.uscNotificationId)).data();
                                if (!jQuery.isEmptyObject(_this._uscNotification)) {
                                    _this._uscNotification.showNotification(exception);
                                }
                            });
                        }
                    }
                    if (_this._workflowActivity.WorkflowProperties != null) {
                        subject = _this._workflowActivity.WorkflowProperties.filter(function (item) { if (item.Name == WorkflowPropertyHelper.DSW_PROPERTY_SUBJECT)
                            return item; })[0].ValueString;
                        _this._lblDossierNote.html(subject);
                        var mapper = new WorkflowRoleModelMapper();
                        var propertyRole = _this._workflowActivity.WorkflowProperties.filter(function (item) { if (item.Name == WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_ROLE)
                            return item; })[0];
                        role = mapper.Map(JSON.parse(propertyRole.ValueString));
                        _this._lblWorkflowProposerRole.html(role.Name);
                    }
                    $("#".concat(_this.rowWorkflowProposerId)).show();
                }, function (exception) {
                    _this._uscNotification = $("#".concat(_this.uscNotificationId)).data();
                    if (!jQuery.isEmptyObject(_this._uscNotification)) {
                        _this._uscNotification.showNotification(exception);
                    }
                });
            }
            var ajaxModel = {};
            ajaxModel.Value = new Array();
            ajaxModel.Value.push(JSON.stringify(dossier.Roles));
            ajaxModel.Value.push(JSON.stringify(dossier.Contacts));
            ajaxModel.ActionName = "LoadExternalData";
            this._ajaxManager = $find(this.ajaxManagerId);
            this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
        };
        /**
        * Scateno l'evento di "Load Completed" del controllo
        */
        uscDossier.prototype.bindLoaded = function () {
            $("#".concat(this.pageId)).data(this);
            $("#".concat(this.pageId)).triggerHandler(uscDossier.LOADED_EVENT);
        };
        uscDossier.prototype.loadExternalDataCallback = function () {
            this.hideLoadingPanel();
        };
        uscDossier.LOADED_EVENT = "onLoaded";
        uscDossier.DATA_LOADED_EVENT = "onDataLoaded";
        return uscDossier;
    }(DossierBase));
    return uscDossier;
});
//# sourceMappingURL=uscDossier.js.map