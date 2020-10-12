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
define(["require", "exports", "PEC/PECBase", "App/Helpers/ServiceConfigurationHelper", "App/Models/Fascicles/FascicleModel", "App/Services/Workflows/WorkflowActivityLogService", "App/Models/DocumentUnits/ChainType", "App/Services/Fascicles/FascicleDocumentService", "App/Services/Fascicles/FascicleFolderService"], function (require, exports, PECBase, ServiceConfigurationHelper, FascicleModel, WorkflowActivityLogService, ChainType, FascicleDocumentService, FascicleFolderService) {
    var PECToDocumentUnit = /** @class */ (function (_super) {
        __extends(PECToDocumentUnit, _super);
        function PECToDocumentUnit(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, PECBase.FASCICLE_TYPE_NAME)) || this;
            _this.radioButton_OnClick = function () {
                var selected = $(':radio:checked').val();
                _this._documentListGrid.get_masterTableView().showColumn(1);
                switch (selected) {
                    case "1": {
                        _this.cmdInitAndClone().hide();
                        if (_this.isPecClone) {
                            _this.cmdInitAndClone().show();
                        }
                        _this.pnlTemplateProtocol().hide();
                        if (_this.templateProtocolEnabled) {
                            _this.pnlTemplateProtocol().show();
                        }
                        _this.pnlUDS().hide();
                        _this.pnlFascicle().hide();
                        break;
                    }
                    case "7": {
                        _this.cmdInitAndClone().hide();
                        if (_this.isPecClone) {
                            _this.cmdInitAndClone().show();
                        }
                        _this.pnlTemplateProtocol().hide();
                        _this.pnlUDS().show();
                        _this.pnlFascicle().hide();
                        break;
                    }
                    case "8": {
                        _this._documentListGrid.get_masterTableView().hideColumn(1);
                        _this.cmdInitAndClone().hide();
                        _this.pnlTemplateProtocol().hide();
                        _this.pnlUDS().hide();
                        _this.pnlFascicle().show();
                        break;
                    }
                }
            };
            _this.cmdFascMiscellaneaInsert_Click = function (sender, args) {
                var uscFascicleSearch = $("#" + _this.uscFascicleSearchId).data();
                if (!jQuery.isEmptyObject(uscFascicleSearch)) {
                    var selectedFascicle = uscFascicleSearch.getSelectedFascicle();
                    if (selectedFascicle) {
                        var selectedFascicleFolder = uscFascicleSearch.getSelectedFascicleFolder();
                        var ajaxModel = {};
                        ajaxModel.Value = new Array();
                        ajaxModel.Value.push(selectedFascicle.UniqueId);
                        if (selectedFascicleFolder) {
                            ajaxModel.Value.push(selectedFascicleFolder.UniqueId);
                        }
                        ajaxModel.ActionName = PECToDocumentUnit.INSERT_MISCELLANEA;
                        $find(_this.ajaxManagerId).ajaxRequest(JSON.stringify(ajaxModel));
                    }
                    else {
                        alert("Nessun fascicolo selezionato");
                    }
                }
            };
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        PECToDocumentUnit.prototype.cmdInit = function () {
            return $("#" + this.cmdInitId);
        };
        PECToDocumentUnit.prototype.cmdInitAndClone = function () {
            return $("#" + this.cmdInitAndCloneId);
        };
        PECToDocumentUnit.prototype.pnlTemplateProtocol = function () {
            return $("#" + this.pnlTemplateProtocolId);
        };
        PECToDocumentUnit.prototype.pnlUDS = function () {
            return $("#" + this.pnlUDSSelectId);
        };
        PECToDocumentUnit.prototype.pnlFascicle = function () {
            return $("#" + this.pnlFascicleSelectId);
        };
        PECToDocumentUnit.prototype.pnlButtons = function () {
            return $("#" + this.pnlButtonsId);
        };
        PECToDocumentUnit.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            var serviceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowActivityLog");
            this._service = new WorkflowActivityLogService(serviceConfiguration);
            var fascicleDocumentServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "FascicleDocument");
            this._fascicleDocumentService = new FascicleDocumentService(fascicleDocumentServiceConfiguration);
            var fascicleFolderServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "FascicleFolder");
            this._fascicleFolderService = new FascicleFolderService(fascicleFolderServiceConfiguration);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._documentListGrid = $find(this.documentListGridId);
            this.cmdInitAndClone().hide();
            if (this.isPecClone) {
                this.cmdInitAndClone().show();
            }
            this.pnlTemplateProtocol().hide();
            if (this.templateProtocolEnabled) {
                this.pnlTemplateProtocol().show();
            }
            this._rblDocumentUnit = $("#".concat(this.rblDocumentUnitId));
        };
        PECToDocumentUnit.prototype.insertWorkflowActivity = function (fascicleModel, workflowActivityModel, url) {
            var _this = this;
            var workflowActivityLogModel = {
                UniqueId: "",
                LogDate: new Date(),
                SystemComputer: "",
                LogType: "",
                LogDescription: "",
                Severity: null,
                RegistrationDate: new Date(),
                RegistrationUser: "",
                LastChangedDate: new Date(),
                LastChangedUser: "",
                Entity: workflowActivityModel
            };
            workflowActivityLogModel.LogDescription = "Fascicolo:  " + fascicleModel.Year + "/" + fascicleModel.Number + "=> " + fascicleModel.FascicleObject;
            this._service.insertWorkflowActivityLog(workflowActivityLogModel, function (data) {
                if (data == null)
                    return;
                alert("I documenti sono stati allegati con successo");
                window.location.href = url;
            }, function (exception) {
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        PECToDocumentUnit.prototype.hasSelectedFascicle = function () {
            var uscFascicleSearch = $("#" + this.uscFascicleSearchId).data();
            if (!jQuery.isEmptyObject(uscFascicleSearch)) {
                var selectedFascicle = uscFascicleSearch.getSelectedFascicle();
                return selectedFascicle != null;
            }
        };
        PECToDocumentUnit.prototype.confirmCallback = function (idChain, idFascicle, isNewArchiveChain, errorMessage, idFascicleFolder) {
            var _this = this;
            if (errorMessage) {
                alert(errorMessage);
                this._loadingPanel.hide(this.documentListGridId);
                this.pnlButtons().show();
                return;
            }
            if (isNewArchiveChain) {
                var fascicleDocumentModel_1 = {};
                fascicleDocumentModel_1.ChainType = ChainType.Miscellanea;
                fascicleDocumentModel_1.IdArchiveChain = idChain;
                fascicleDocumentModel_1.Fascicle = new FascicleModel();
                fascicleDocumentModel_1.Fascicle.UniqueId = idFascicle;
                this._fascicleFolderService.getById(idFascicleFolder, function (data) {
                    if (!data) {
                        _this._loadingPanel.hide(_this.pageContentId);
                        _this.showNotificationException(_this.uscNotificationId, null, "E' avvenuto un errore durante il processo di fascicolazione");
                        return;
                    }
                    fascicleDocumentModel_1.FascicleFolder = data;
                    _this._fascicleDocumentService.insertFascicleDocument(fascicleDocumentModel_1, function (data) { return window.location.href = "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=" + idFascicle; }, function (exception) {
                        _this._loadingPanel.hide(_this.pageContentId);
                        _this.showNotificationException(_this.uscNotificationId, exception);
                    });
                }, function (exception) {
                    _this._loadingPanel.hide(_this.pageContentId);
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            }
            else {
                window.location.href = "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=" + idFascicle;
            }
        };
        PECToDocumentUnit.INSERT_MISCELLANEA = "InsertMiscellanea";
        return PECToDocumentUnit;
    }(PECBase));
    return PECToDocumentUnit;
});
//# sourceMappingURL=PECToDocumentUnit.js.map