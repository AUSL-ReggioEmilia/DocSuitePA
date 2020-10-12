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
define(["require", "exports", "./FascBase", "App/DTOs/ExceptionDTO", "App/Models/Commons/MetadataRepositoryModel", "App/Helpers/GuidHelper", "App/Models/Workflows/WorkflowActionFascicleModel", "App/Models/InsertActionType", "App/Helpers/PageClassHelper", "App/Helpers/ServiceConfigurationHelper", "App/Models/Fascicles/FascicleType", "App/Helpers/SessionStorageKeysHelper"], function (require, exports, FascBase, ExceptionDTO, MetadataRepositoryModel, Guid, WorkflowActionFascicleModel, InsertActionType, PageClassHelper, ServiceConfigurationHelper, FascicleType, SessionStorageKeysHelper) {
    var FascProcessInserimento = /** @class */ (function (_super) {
        __extends(FascProcessInserimento, _super);
        function FascProcessInserimento(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, FascBase.FASCICLE_TYPE_NAME)) || this;
            _this._btnInsert_OnClick = function (sender, args) {
                if (!Page_IsValid) {
                    args.set_cancel(true);
                    return;
                }
                PageClassHelper.callUserControlFunctionSafe(_this.uscFascicleInsertId)
                    .done(function (instance) {
                    try {
                        instance.getFascicle().done(function (currentFascicleToInsert) {
                            instance.fillMetadataModel().done(function (metadatas) {
                                if (!metadatas) {
                                    _this._btnInsert.set_enabled(true);
                                    return;
                                }
                                _this.finalizeInsert(currentFascicleToInsert, metadatas[0], metadatas[1]);
                            });
                        });
                    }
                    catch (error) {
                        console.error(error.message);
                        var exception = new ExceptionDTO();
                        exception.statusText = "E' avvenuto un errore durante la creazione del fascicolo";
                        _this.showNotificationException(_this.uscNotificationId, exception);
                    }
                });
            };
            return _this;
        }
        FascProcessInserimento.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            this._btnInsert = $find(this.btnInsertId);
            this._btnInsert.add_clicking(this._btnInsert_OnClick);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            if (this.idCategory) {
                this.loadCategory(this.idCategory);
            }
        };
        FascProcessInserimento.prototype.finalizeInsert = function (fascicle, metadataDesigner, metadataValues) {
            var _this = this;
            fascicle.MetadataValues = metadataValues;
            fascicle.MetadataDesigner = metadataDesigner;
            if (sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_METADATA_REPOSITORY)) {
                var metadataRepository = new MetadataRepositoryModel();
                metadataRepository.UniqueId = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_METADATA_REPOSITORY);
                fascicle.MetadataRepository = metadataRepository;
            }
            if (this.idDocumentUnit) {
                fascicle.UniqueId = Guid.newGuid();
                var workflowAction = new WorkflowActionFascicleModel();
                workflowAction.Fascicle = {};
                workflowAction.Fascicle.$type = "VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles.FascicleModel, VecompSoftware.DocSuiteWeb.Model";
                workflowAction.Fascicle.UniqueId = fascicle.UniqueId;
                workflowAction.Referenced = {};
                workflowAction.Referenced.$type = "VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits.DocumentUnitModel, VecompSoftware.DocSuiteWeb.Model";
                workflowAction.Referenced.UniqueId = this.idDocumentUnit;
                fascicle.WorkflowActions.push(workflowAction);
            }
            var insertActionType = null;
            if (fascicle.FascicleType == FascicleType.Procedure) {
                insertActionType = InsertActionType.InsertProcedureFascicle;
            }
            this._loadingPanel.show(this.pnlContentId);
            this.service.insertFascicle(fascicle, insertActionType, function (data) {
                window.location.href = "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=" + data.UniqueId;
            }, function (exception) {
                _this._loadingPanel.hide(_this.pnlContentId);
                _this._btnInsert.set_enabled(true);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        FascProcessInserimento.prototype.loadCategory = function (idCategory) {
            PageClassHelper.callUserControlFunctionSafe(this.uscFascicleInsertId)
                .done(function (instance) {
                instance.loadDefaultCategory(idCategory);
            });
        };
        return FascProcessInserimento;
    }(FascBase));
    return FascProcessInserimento;
});
//# sourceMappingURL=FascProcessInserimento.js.map