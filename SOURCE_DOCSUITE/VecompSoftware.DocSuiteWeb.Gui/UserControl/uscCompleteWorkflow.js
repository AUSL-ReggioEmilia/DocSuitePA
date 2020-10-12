/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />
define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Services/Workflows/WorkflowActivityService", "App/Services/Workflows/WorkflowNotifyService", "App/Models/Workflows/WorkflowActivityStatus", "App/DTOs/ExceptionDTO", "App/Models/Workflows/AcceptanceStatus", "App/Models/Workflows/WorkflowPropertyHelper", "App/Models/UpdateActionType", "App/Managers/HandlerWorkflowManager", "App/Models/Workflows/ArgumentType", "../app/core/extensions/string"], function (require, exports, ServiceConfigurationHelper, WorkflowActivityService, WorkflowNotifyService, WorkflowActivityStatus, ExceptionDTO, AcceptanceStatus, WorkflowPropertyHelper, UpdateActionType, HandlerWorkflowManager, ArgumentType) {
    var uscCompleteWorkflow = /** @class */ (function () {
        /**
         *
        *Costruttore
         * @param serviceConfigurations
         */
        function uscCompleteWorkflow(serviceConfigurations) {
            var _this = this;
            /**
            *---------------------Events---------------------
            */
            this.btnConfirm_OnClick = function (sender, args) {
                args.set_cancel(true);
                if (!Page_IsValid) {
                    return;
                }
                _this._loadingPanel.show(_this.contentId);
                var checkedChoice = Number(_this._rblActivityStatus.find('input:checked').val());
                switch (checkedChoice) {
                    case WorkflowActivityStatus.Done: {
                        _this.notifyWorkflow(AcceptanceStatus.Accepted);
                        break;
                    }
                    case WorkflowActivityStatus.Handling: {
                        _this.updateHandlingWorkflowActivity(UpdateActionType.HandlingWorkflow);
                        break;
                    }
                    case WorkflowActivityStatus.Todo: {
                        _this.updateHandlingWorkflowActivity(UpdateActionType.RelaseHandlingWorkflow);
                        break;
                    }
                    case WorkflowActivityStatus.Rejected: {
                        _this.notifyWorkflow(AcceptanceStatus.Refused);
                        break;
                    }
                }
            };
            this.initializeRclActivityStatus = function () {
                _this._workflowActivityService.isWorkflowActivityHandler(_this.workflowActivityId, function (data) {
                    if (data == null)
                        return;
                    var toCheck = _this._rblActivityStatus.find("input[value=".concat(WorkflowActivityStatus.Handling.toString(), "]"));
                    toCheck.prop('checked', true);
                    if (data === false) {
                        var toDisable = _this._rblActivityStatus.find("input[value!=".concat(WorkflowActivityStatus.Handling.toString(), "]"));
                        toDisable.each(function () {
                            $(this).prop('disabled', true);
                        });
                    }
                    _this._loadingPanel.hide(_this.contentId);
                }, function (exception) {
                    _this.showNotificationException(_this.uscNotificationId, exception, "Anomalia nel recupero dell'informazione isHandler.");
                });
            };
            this.notifyWorkflow = function (response) {
                var _a, _b;
                if (confirm("Conferma il completamento e restituisci al richiedente?")) {
                    //update della proprietà acceptance activity
                    var acceptanceModel = {};
                    acceptanceModel.Status = response;
                    acceptanceModel.Owner = _this.currentUser;
                    acceptanceModel.Executor = _this.currentUser;
                    var txt = $find(_this.txtWfId);
                    acceptanceModel.AcceptanceReason = txt.get_textBoxValue();
                    var proposerRole_1;
                    _this._workflowActivity.WorkflowProperties.forEach(function (item) {
                        if (item.Name === WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_ROLE) {
                            proposerRole_1 = item.ValueString;
                            return;
                        }
                    });
                    var proposerProperty = JSON.parse(proposerRole_1);
                    acceptanceModel.ProposedRole = proposerProperty;
                    acceptanceModel.AcceptanceDate = new Date();
                    _this._loadingPanel.show(_this.contentId);
                    var workflowNotifyModel = {};
                    workflowNotifyModel.WorkflowActivityId = _this.workflowActivityId;
                    workflowNotifyModel.WorkflowName = (_b = (_a = _this._workflowActivity.WorkflowInstance) === null || _a === void 0 ? void 0 : _a.WorkflowRepository) === null || _b === void 0 ? void 0 : _b.Name;
                    workflowNotifyModel.ModuleName = HandlerWorkflowManager.DOCSUITE_MODULE_NAME;
                    workflowNotifyModel.OutputArguments = {};
                    var propAcceptance = {};
                    propAcceptance.Name = WorkflowPropertyHelper.DSW_FIELD_ACCEPTANCE;
                    propAcceptance.PropertyType = ArgumentType.Json;
                    propAcceptance.ValueString = JSON.stringify(acceptanceModel);
                    workflowNotifyModel.OutputArguments[WorkflowPropertyHelper.DSW_FIELD_ACCEPTANCE] = propAcceptance;
                    var propManualComplete = {};
                    propManualComplete.Name = WorkflowPropertyHelper.DSW_ACTION_ACTIVITY_MANUAL_COMPLETE;
                    propManualComplete.PropertyType = ArgumentType.PropertyBoolean;
                    propManualComplete.ValueBoolean = true;
                    workflowNotifyModel.OutputArguments[WorkflowPropertyHelper.DSW_ACTION_ACTIVITY_MANUAL_COMPLETE] = propManualComplete;
                    _this._workflowNotifyService.notifyWorkflow(workflowNotifyModel, function (data) {
                        _this._loadingPanel.hide(_this.contentId);
                        var result = {};
                        result.ActionName = "Stato attività aggiornato";
                        _this.closeWindow(result);
                    }, function (exception) {
                        _this._loadingPanel.hide(_this.contentId);
                        _this._btnConfirm.set_enabled(true);
                        _this.showNotificationException(_this.uscNotificationId, exception);
                    });
                }
                _this._loadingPanel.hide(_this.contentId);
                _this.radioListButtonChanged();
            };
            this.updateHandlingWorkflowActivity = function (activity) {
                var message;
                switch (activity) {
                    case UpdateActionType.HandlingWorkflow: {
                        message = "Attività presa in carico";
                        break;
                    }
                    case UpdateActionType.RelaseHandlingWorkflow: {
                        message = "Stato attività impostato 'da lavorare'";
                        break;
                    }
                }
                if (_this._workflowActivity.WorkflowAuthorizations) {
                    _this._workflowActivityService.updateHandlingWorkflowActivity(_this._workflowActivity, activity, function (data) {
                        var result = {};
                        result.ActionName = message;
                        result.Value = new Array();
                        result.Value.push(_this.workflowActivityId);
                        _this.closeWindow(result);
                    }, function (exception) {
                        _this._loadingPanel.hide(_this.contentId);
                        $("#".concat(_this.contentId)).hide();
                        _this.showNotificationException(_this.uscNotificationId, exception);
                    });
                }
            };
            /**
        * Recupera una RadWindow dalla pagina
        */
            this.getRadWindow = function () {
                var wnd = null;
                if (window.radWindow)
                    wnd = window.radWindow;
                else if (window.frameElement.radWindow)
                    wnd = window.frameElement.radWindow;
                return wnd;
            };
            /**
             * Chiude la RadWindow
            */
            this.closeWindow = function (message) {
                var wnd = _this.getRadWindow();
                wnd.close(message);
            };
            this.radioListButtonChanged = function () {
                var checkedChoice = Number(_this._rblActivityStatus.find('input:checked').val());
                switch (checkedChoice) {
                    case WorkflowActivityStatus.Done:
                    case WorkflowActivityStatus.Rejected: {
                        _this._motivationEnabled = false;
                        var motivationProp = _this._workflowActivity.WorkflowProperties.filter(function (item) {
                            if (item.Name === WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_END_MOTIVATION_REQUIRED) {
                                return item;
                            }
                        });
                        if (motivationProp && motivationProp.length > 0) {
                            _this._motivationEnabled = motivationProp[0].ValueBoolean;
                        }
                        ValidatorEnable(document.getElementById(_this.ctrlTxtWfId), _this._motivationEnabled);
                        $('#'.concat(_this.pnlWorkflowNoteId)).hide();
                        var motivationProperties = _this._workflowActivity.WorkflowProperties.filter(function (item) { return item.Name === WorkflowPropertyHelper.DSW_ACTION_METADATA_MOTIVATION_LABEL; });
                        if (motivationProperties && motivationProperties.length > 0) {
                            _this._lblMotivation.html(motivationProperties[0].ValueString);
                            $('#'.concat(_this.pnlWorkflowNoteId)).show();
                        }
                        break;
                    }
                    case WorkflowActivityStatus.Handling:
                    case WorkflowActivityStatus.Todo: {
                        ValidatorEnable(document.getElementById(_this.ctrlTxtWfId), false);
                        $('#'.concat(_this.pnlWorkflowNoteId)).hide();
                        break;
                    }
                }
            };
            this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
        }
        /**
        *---------------------Methods---------------------
        */
        /**
        *Initialize
        */
        uscCompleteWorkflow.prototype.initialize = function () {
            var _this = this;
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._loadingPanel.show(this.contentId);
            this._rblActivityStatus = $("#".concat(this.rblActivityStatusId));
            this._rblActivityStatus.on('change', this.radioListButtonChanged);
            this._lblMotivation = $("#".concat(this.lblMotivationId));
            this._btnConfirm = $find(this.btnConfirmId);
            this._btnConfirm.add_clicking(this.btnConfirm_OnClick);
            var workflowActivityConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, 'WorkflowActivity');
            this._workflowActivityService = new WorkflowActivityService(workflowActivityConfiguration);
            var workflowNotifyConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, 'WorkflowNotify');
            this._workflowNotifyService = new WorkflowNotifyService(workflowNotifyConfiguration);
            ValidatorEnable(document.getElementById(this.ctrlTxtWfId), false);
            this._workflowActivityService.getWorkflowActivity(this.workflowActivityId, function (data) {
                if (data == null)
                    return;
                _this._workflowActivity = data;
                _this.initializeRclActivityStatus();
                _this.radioListButtonChanged();
            }, function (exception) {
                _this._loadingPanel.hide(_this.contentId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        uscCompleteWorkflow.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
            if (exception && exception instanceof ExceptionDTO) {
                var uscNotification = $("#".concat(uscNotificationId)).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                }
            }
            else {
                this.showNotificationMessage(uscNotificationId, customMessage);
            }
        };
        uscCompleteWorkflow.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        return uscCompleteWorkflow;
    }());
    return uscCompleteWorkflow;
});
//# sourceMappingURL=uscCompleteWorkflow.js.map