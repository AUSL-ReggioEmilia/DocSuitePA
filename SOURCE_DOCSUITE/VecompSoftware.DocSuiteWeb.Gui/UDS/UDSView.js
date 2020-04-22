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
define(["require", "exports", "../App/Models/DocumentUnits/DocumentUnitModel", "../App/Models/UDS/UDSRelationType", "./UDSViewBase", "../App/Services/DocumentUnits/DocumentUnitService", "../App/Helpers/ServiceConfigurationHelper", "../App/Services/Commons/RoleService"], function (require, exports, DocumentUnitModel, UDSRelationType, UDSViewBase, DocumentUnitService, ServiceConfigurationHelper, RoleService) {
    var UDSView = /** @class */ (function (_super) {
        __extends(UDSView, _super);
        function UDSView(serviceConfigurations) {
            var _this = _super.call(this, serviceConfigurations) || this;
            _this.showWindow = function (sender, args) {
                _this._windowNuovo.show();
            };
            _this._btnLink_onClick = function () {
                var url = "../UDS/UDSLink.aspx?IdUDSRepository=" + _this.currentIdUDSRepository + "&idUDS=" + _this.currentIdUDS + "&fromUDSLink=True";
                var window = _this._radWindowManager.open(url, "UDSLink", null);
                window.setSize(1024, 600);
                window.add_close(_this.closeWindowSearch);
                window.set_modal(true);
                window.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
                window.set_visibleStatusbar(false);
                window.center();
            };
            _this._btnWorkflow_onClick = function (sender, args) {
                args.set_cancel(true);
                _this.setSessionVariables();
                var url = "../Workflows/StartWorkflow.aspx?Type=UDS&ManagerID=" + _this.radWindowManagerId + "&DSWEnvironment=" + _this._documentUnitModel.Environment + "&Callback=" + window.location.href;
                return _this.openWindow(url, "windowStartWorkflow", 730, 550);
            };
            _this.onWorkflowCloseWindow = function (sender, args) {
                if (args.get_argument()) {
                    var result = {};
                    result = args.get_argument();
                    _this._notificationInfo.show();
                    _this._notificationInfo.set_text(result.ActionName);
                }
            };
            _this.closeWindowSearch = function (sender, args) {
                sender.remove_close(_this.closeWindowSearch);
                //get arguments from callback
                if (args.get_argument() !== null) {
                    _this.destinationIdUDS = args.get_argument().split("|")[0];
                    _this.destinationUDSRepositoryId = args.get_argument().split("|")[1];
                    _this._loadingPanel.show(_this.pnlUDSViewId);
                    _this.postStartUDS(); //first POST with the current UDS
                    _this.postRelatedUDS(); //second POST with the UDS related to the first one
                }
            };
            _this._serviceConfigurations = serviceConfigurations;
            return _this;
        }
        UDSView.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._btnLink = $find(this.btnLinkId);
            this._notificationInfo = $find(this.radNotificationInfoId);
            if (this._btnLink) {
                this._btnLink.add_clicked(this._btnLink_onClick);
            }
            this._radWindowManager = $find(this.radWindowManagerId);
            this._btnWorkflow = $find(this.btnWorkflowId);
            this._btnWorkflow.set_visible(this.isWorkflowEnabled);
            if (this.isWorkflowEnabled) {
                this._btnWorkflow.add_clicking(this._btnWorkflow_onClick);
                this._radWindowManager.add_close((this.onWorkflowCloseWindow));
            }
            var documentUnitConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DocumentUnit");
            this._documentUnitService = new DocumentUnitService(documentUnitConfiguration);
            this._documentUnitService.getDocumentUnitById(this.currentIdUDS, function (data) {
                _this._documentUnitModel = data;
                _this.setDocumentUnitRoles();
            });
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
            sessionStorage.setItem("ReferenceModel", JSON.stringify(this._documentUnitModel));
            sessionStorage.setItem("ReferenceId", this._documentUnitModel.UniqueId);
            sessionStorage.setItem("ReferenceTitle", this._documentUnitModel.Title + " - " + this._documentUnitModel.Subject);
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
            //get the Start UDS repository, then make the POST at callback
            this.udsRepositoryService.getUDSRepositoryByID(this.currentIdUDSRepository, function (data) {
                if (!data)
                    return;
                _this._repositoryModel = data[0];
                _this.populateCurrentUDSModel();
                _this.triggerUDSConnection(false);
            }, function (exception) {
                alert("Anomalia nel collegare gli archivi. Contattare l'assistenza.");
            });
        };
        UDSView.prototype.postRelatedUDS = function () {
            var _this = this;
            //get the UDS repository related to the start UDS, then make the POST at callback
            this.udsRepositoryService.getUDSRepositoryByID(this.destinationUDSRepositoryId, function (data) {
                if (!data)
                    return;
                _this._repositoryModel = data[0];
                _this.populateDestinationUDSModel();
                _this.triggerUDSConnection(true);
            }, function (exception) {
                alert("Anomalia nel collegare gli archivi. Contattare l'assistenza.");
            });
        };
        UDSView.prototype.triggerUDSConnection = function (showMessage) {
            this.udsConnectionService.intitializeConnection(this._udsDocumentUnit, function (data) {
                //show the confirmation message after the second POST
                if (showMessage) {
                    alert("Archivi sono stati collegati con successo!");
                    location.reload();
                }
            }, function (exception) {
                alert("Anomalia nel collegare gli archivi. Contattare l'assistenza.");
                location.reload();
            });
        };
        return UDSView;
    }(UDSViewBase));
    return UDSView;
});
//# sourceMappingURL=UDSView.js.map