/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
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
define(["require", "exports", "App/Models/DocumentUnits/DocumentUnitModel", "App/Models/UDS/UDSRelationType", "./UDSViewBase", "App/DTOs/ExceptionDTO", "App/Services/DocumentUnits/DocumentUnitService", "App/Helpers/ServiceConfigurationHelper", "App/Services/Commons/RoleService", "App/DTOs/ExceptionStatusCode", "App/Helpers/SessionStorageKeysHelper"], function (require, exports, DocumentUnitModel, UDSRelationType, UDSViewBase, ExceptionDTO, DocumentUnitService, ServiceConfigurationHelper, RoleService, ExceptionStatusCode, SessionStorageKeysHelper) {
    var UDSView = /** @class */ (function (_super) {
        __extends(UDSView, _super);
        function UDSView(serviceConfigurations) {
            var _this = _super.call(this, serviceConfigurations) || this;
            _this.showWindow = function (sender, args) {
                _this._windowNuovo.show();
            };
            _this.btnLink_onClick = function () {
                var url = "../UDS/UDSLink.aspx?IdUDSRepository=" + _this.currentIdUDSRepository + "&idUDS=" + _this.currentIdUDS + "&fromUDSLink=True";
                var window = _this._radWindowManager.open(url, "UDSLink", null);
                window.setSize(1024, 600);
                window.add_beforeClose(_this.beforeCloseWindowSearch);
                window.set_modal(true);
                window.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
                window.set_visibleStatusbar(false);
                window.center();
            };
            _this.btnWorkflow_onClick = function (sender, args) {
                args.set_cancel(true);
                _this.setSessionVariables();
                var url = "../Workflows/StartWorkflow.aspx?Type=UDS&ManagerID=" + _this.radWindowManagerId + "&DSWEnvironment=" + _this._documentUnitModel.Environment + "&Callback=" + window.location.href;
                return _this.openWindow(url, "windowStartWorkflow", 730, 550);
            };
            _this.btnCompleteWorkflow_OnClick = function (sender, args) {
                args.set_cancel(true);
                var url = "../Workflows/CompleteWorkflow.aspx?Type=Fasc&IdDocumentUnit=" + _this._documentUnitModel.UniqueId + "&IdWorkflowActivity=" + _this.workflowActivityId;
                return _this.openWindow(url, "windowCompleteWorkflow", 700, 500);
            };
            _this.onWorkflowCloseWindow = function (sender, args) {
                if (args.get_argument()) {
                    var result = {};
                    result = args.get_argument();
                    _this._notificationInfo.show();
                    _this._notificationInfo.set_text(result.ActionName);
                    _this._loadingPanel.show(_this.pnlUDSViewId);
                    location.reload();
                }
            };
            _this.beforeCloseWindowSearch = function (sender, args) {
                if (args.get_argument()) {
                    args.set_cancel(true);
                    _this.closeWindowSearch(sender, args);
                }
            };
            _this.closeWindowSearch = function (sender, args) {
                sender.remove_close(_this.closeWindowSearch);
                //get arguments from callback
                if (args.get_argument() !== null) {
                    _this.destinationIdUDS = args.get_argument().split("|")[0];
                    _this.destinationUDSRepositoryId = args.get_argument().split("|")[1];
                    _this._loadingPanel.show(_this.pnlUDSViewId);
                    _this.postStartUDS()
                        .done(function () { return _this.postRelatedUDS()
                        .done(function () {
                        alert("Gli archivi sono stati collegati con successo!");
                        location.reload();
                    })
                        .fail(function (exception) { return alert("Anomalia nel collegare gli archivi. Contattare l'assistenza."); }); })
                        .fail(function (exception) { return alert("Anomalia nel collegare gli archivi. Contattare l'assistenza."); });
                }
            };
            _this.hasActiveWorkflowActivityWorkflow = function () {
                return !String.isNullOrEmpty(_this.workflowActivityId);
            };
            _this._serviceConfigurations = serviceConfigurations;
            return _this;
        }
        UDSView.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            this.cleanSessionStorage();
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._notificationInfo = $find(this.radNotificationInfoId);
            this._radWindowManager = $find(this.radWindowManagerId);
            this._btnLink = $find(this.btnLinkId);
            if (this._btnLink) {
                this._btnLink.add_clicked(this.btnLink_onClick);
            }
            this._btnWorkflow = $find(this.btnWorkflowId);
            this._btnWorkflow.set_visible(this.isWorkflowEnabled);
            if (this.isWorkflowEnabled) {
                this._btnWorkflow.add_clicking(this.btnWorkflow_onClick);
                this._radWindowManager.add_close((this.onWorkflowCloseWindow));
            }
            this._btnCompleteWorkflow = $find(this.btnCompleteWorkflowId);
            this._btnCompleteWorkflow.set_visible(this.hasActiveWorkflowActivityWorkflow());
            if (this.hasActiveWorkflowActivityWorkflow()) {
                this._btnCompleteWorkflow.add_clicking(this.btnCompleteWorkflow_OnClick);
            }
            var documentUnitConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DocumentUnit");
            this._documentUnitService = new DocumentUnitService(documentUnitConfiguration);
            this._documentUnitService.getDocumentUnitById(this.currentIdUDS, function (data) {
                _this._documentUnitModel = data;
                _this.setDocumentUnitRoles();
            });
            this._windowCompleteWorkflow = $find(this.windowCompleteWorkflowId);
            this._windowCompleteWorkflow.add_close((this.onWorkflowCloseWindow));
        };
        UDSView.prototype.openWindow = function (url, name, width, height) {
            var manager = $find(this.radWindowManagerId);
            var wnd = manager.open(url, name, null);
            wnd.setSize(width, height);
            wnd.set_modal(true);
            wnd.center();
            return false;
        };
        UDSView.prototype.setSessionVariables = function () {
            sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_MODEL, JSON.stringify(this._documentUnitModel));
            sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_ID, this._documentUnitModel.UniqueId);
            sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_TITLE, this._documentUnitModel.Title + " - " + this._documentUnitModel.Subject);
        };
        UDSView.prototype.setDocumentUnitRoles = function () {
            var _this = this;
            var roleConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Role");
            this._roleService = new RoleService(roleConfiguration);
            if (this._documentUnitModel) {
                this._documentUnitModel.Roles = new Array();
                this._documentUnitModel.DocumentUnitRoles.forEach(function (x) {
                    _this._roleService.getDocumentUnitRoles(x.UniqueIdRole, function (data) {
                        _this._documentUnitModel.Roles.push(data);
                    });
                });
            }
        };
        UDSView.prototype.populateCurrentUDSModel = function () {
            var documentModel = new DocumentUnitModel();
            documentModel.UniqueId = this.destinationIdUDS;
            this._udsDocumentUnit = {
                Environment: this._repositoryModel.DSWEnvironment,
                RelationType: UDSRelationType.UDS,
                IdUDS: this.currentIdUDS,
                Relation: documentModel,
                Repository: this._repositoryModel
            };
        };
        UDSView.prototype.populateDestinationUDSModel = function () {
            var documentModel = new DocumentUnitModel();
            documentModel.UniqueId = this.currentIdUDS;
            this._udsDocumentUnit = {
                Environment: this._repositoryModel.DSWEnvironment,
                RelationType: UDSRelationType.UDS,
                IdUDS: this.destinationIdUDS,
                Relation: documentModel,
                Repository: this._repositoryModel
            };
        };
        UDSView.prototype.postStartUDS = function () {
            var _this = this;
            var promise = $.Deferred();
            //get the Start UDS repository, then make the POST at callback
            this.udsRepositoryService.getUDSRepositoryByID(this.currentIdUDSRepository, function (data) {
                if (!data) {
                    var exc = new ExceptionDTO();
                    exc.statusCode = ExceptionStatusCode.BadRequest;
                    exc.statusText = "Nessun repository trovato con id " + _this.currentIdUDSRepository;
                    return promise.reject(exc);
                }
                _this._repositoryModel = data[0];
                _this.populateCurrentUDSModel();
                _this.triggerUDSConnection()
                    .done(function () { return promise.resolve(); })
                    .fail(function (exception) { return promise.reject(exception); });
            }, function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        UDSView.prototype.postRelatedUDS = function () {
            var _this = this;
            var promise = $.Deferred();
            //get the UDS repository related to the start UDS, then make the POST at callback
            this.udsRepositoryService.getUDSRepositoryByID(this.destinationUDSRepositoryId, function (data) {
                if (!data) {
                    var exc = new ExceptionDTO();
                    exc.statusCode = ExceptionStatusCode.BadRequest;
                    exc.statusText = "Nessun repository trovato con id " + _this.destinationUDSRepositoryId;
                    return promise.reject(exc);
                }
                _this._repositoryModel = data[0];
                _this.populateDestinationUDSModel();
                _this.triggerUDSConnection()
                    .done(function () { return promise.resolve(); })
                    .fail(function (exception) { return promise.reject(exception); });
            }, function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        UDSView.prototype.triggerUDSConnection = function () {
            var promise = $.Deferred();
            this.udsConnectionService.intitializeConnection(this._udsDocumentUnit, function (data) { return promise.resolve(); }, function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        UDSView.prototype.cleanSessionStorage = function () {
            sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_DOCUMENTS_REFERENCE_MODEL);
        };
        return UDSView;
    }(UDSViewBase));
    return UDSView;
});
//# sourceMappingURL=UDSView.js.map