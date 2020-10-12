define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Services/Workflows/WorkflowActivityService", "App/Models/Workflows/WorkflowPropertyHelper", "App/Services/Workflows/WorkflowNotifyService", "App/Models/Workflows/ArgumentType"], function (require, exports, ServiceConfigurationHelper, WorkflowActivityService, WorkflowPropertyHelper, WorkflowNotifyService, ArgumentType) {
    var HandlerWorkflowManager = /** @class */ (function () {
        function HandlerWorkflowManager(serviceConfigurations) {
            var _this = this;
            this._updateWorkflowActivityAuthorization = function (workflowActivity) {
                var _a, _b;
                var deffered = $.Deferred();
                var workflowNotifyModel = {};
                workflowNotifyModel.WorkflowActivityId = workflowActivity.UniqueId;
                workflowNotifyModel.WorkflowName = (_b = (_a = workflowActivity.WorkflowInstance) === null || _a === void 0 ? void 0 : _a.WorkflowRepository) === null || _b === void 0 ? void 0 : _b.Name;
                workflowNotifyModel.ModuleName = HandlerWorkflowManager.DOCSUITE_MODULE_NAME;
                var dsw_a_ToHandler = {};
                dsw_a_ToHandler.Name = WorkflowPropertyHelper.DSW_ACTION_TO_HANDLER;
                dsw_a_ToHandler.PropertyType = ArgumentType.PropertyBoolean;
                dsw_a_ToHandler.ValueBoolean = true;
                workflowNotifyModel.OutputArguments = {};
                workflowNotifyModel.OutputArguments[WorkflowPropertyHelper.DSW_ACTION_TO_HANDLER] = dsw_a_ToHandler;
                _this._workflowNotifyService.notifyWorkflow(workflowNotifyModel, function (response) {
                    deffered.resolve(workflowActivity.UniqueId);
                }, function (error) {
                    deffered.reject(error);
                });
                return deffered.promise();
            };
            this._serviceConfigurations = serviceConfigurations;
            this.initialize();
        }
        HandlerWorkflowManager.prototype.initialize = function () {
            var workflowActivityConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowActivity");
            this._workflowActivityService = new WorkflowActivityService(workflowActivityConfiguration);
            var workflowNotifyConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowNotify");
            this._workflowNotifyService = new WorkflowNotifyService(workflowNotifyConfiguration);
        };
        /**
         *  ------------------------------- Methods ----------------------------
         */
        /**
         * Verifico se e' prevista la presa in carico automatica dei workflow
         * @param properties
         */
        HandlerWorkflowManager.prototype.handlingIsAutomatic = function (properties) {
            var automaticHandling = false;
            properties.forEach(function (item) {
                if (item.Name === WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_AUTO_HANDLING) {
                    automaticHandling = item.ValueBoolean;
                    return;
                }
            });
            return automaticHandling;
        };
        HandlerWorkflowManager.prototype.manageHandlingWorkflow = function (environmentOrActivityId, environment) {
            var _this = this;
            var promise = $.Deferred();
            var wfAction = function () { return $.Deferred().resolve(environmentOrActivityId).promise(); };
            if (environment) {
                wfAction = function () {
                    var promise = $.Deferred();
                    _this._workflowActivityService.getActiveActivitiesByReferenceIdAndEnvironment(environmentOrActivityId, environment, function (data) {
                        if (!data) {
                            promise.resolve(null);
                            return;
                        }
                        promise.resolve(data.UniqueId);
                    }, function (exception) { return promise.reject(exception); });
                    return promise.promise();
                };
            }
            wfAction()
                .done(function (activityId) {
                if (!activityId) {
                    promise.resolve(null);
                    return;
                }
                _this._workflowActivityService.hasHandler(activityId, function (workflowActivityHasHandler) {
                    if (workflowActivityHasHandler) {
                        promise.resolve(activityId);
                        return;
                    }
                    _this._workflowActivityService.getWorkflowActivityById(activityId, function (workflowActivityData) {
                        if (!workflowActivityData) {
                            var exception = {};
                            exception.statusText = "Errore nel caricamento delle attività del fusso di lavoro associate al fascicolo.";
                            promise.reject(exception);
                            return;
                        }
                        if (workflowActivityData.WorkflowProperties && !_this.handlingIsAutomatic(workflowActivityData.WorkflowProperties)) {
                            promise.resolve(activityId);
                            return;
                        }
                        _this._updateWorkflowActivityAuthorization(workflowActivityData)
                            .done(function (data) { return promise.resolve(activityId); })
                            .fail(function (exception) { return promise.reject(exception); });
                    }, function (exception) { return promise.reject(exception); }, HandlerWorkflowManager.WORKFLOW_ACTIVITY_EXPAND_PROPERTIES);
                }, function (exception) {
                    promise.reject(exception);
                });
            })
                .fail(function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        HandlerWorkflowManager.DOCSUITE_MODULE_NAME = "DocSuite";
        HandlerWorkflowManager.WORKFLOW_ACTIVITY_EXPAND_PROPERTIES = [
            "WorkflowProperties", "WorkflowInstance($expand=WorkflowRepository)"
        ];
        return HandlerWorkflowManager;
    }());
    return HandlerWorkflowManager;
});
//# sourceMappingURL=HandlerWorkflowManager.js.map