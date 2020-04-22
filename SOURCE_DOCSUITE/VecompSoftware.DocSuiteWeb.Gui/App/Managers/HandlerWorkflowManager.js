define(["require", "exports", "App/Models/UpdateActionType", "App/Helpers/ServiceConfigurationHelper", "App/Services/Workflows/WorkflowActivityService", "App/Services/Workflows/WorkflowPropertyService", "App/Models/Workflows/WorkflowPropertyHelper", "App/Services/Workflows/WorkflowAuthorizationService"], function (require, exports, UpdateActionType, ServiceConfigurationHelper, WorkflowActivityService, WorkflowPropertyService, WorkflowPropertyHelper, WorkflowAuthorizationService) {
    var HandlerWorkflowManager = /** @class */ (function () {
        function HandlerWorkflowManager(serviceConfigurations) {
            this._serviceConfigurations = serviceConfigurations;
            this.initialize();
        }
        HandlerWorkflowManager.prototype.initialize = function () {
            var workflowActivityConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowActivity");
            this._workflowActivityService = new WorkflowActivityService(workflowActivityConfiguration);
            var workflowPropertyConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowProperty");
            this._workflowPropertyService = new WorkflowPropertyService(workflowPropertyConfiguration);
            var workflowAuthorizationConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowAuthorizations");
            this._workflowAuthorizationService = new WorkflowAuthorizationService(workflowPropertyConfiguration);
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
        /**
         * Gestisco l'attivita' di workflow corrente se non ho gia' l'id
         * @param currentEnvironmentId
         * @param environment
         */
        HandlerWorkflowManager.prototype.manageHandlingWorkflow = function (currentEnvironmentId, environment) {
            var _this = this;
            var promise = $.Deferred();
            var workflowActivity;
            this._workflowActivityService.getActiveActivitiesByReferenceIdAndEnvironment(currentEnvironmentId, environment, function (data) {
                if (data) {
                    workflowActivity = data;
                    _this._workflowActivityService.hasHandler(workflowActivity.UniqueId, function (data) {
                        if (!data) {
                            _this._workflowPropertyService.getPropertiesFromActivity(workflowActivity.UniqueId, function (data) {
                                if (data) {
                                    if (!_this.handlingIsAutomatic(data)) {
                                        promise.resolve(workflowActivity.UniqueId);
                                        return;
                                    }
                                    _this._workflowActivityService.getWorkflowActivity(workflowActivity.UniqueId, function (data) {
                                        if (data) {
                                            workflowActivity = data;
                                            _this._workflowActivityService.updateHandlingWorkflowActivity(workflowActivity, UpdateActionType.HandlingWorkflow, function (data) {
                                                promise.resolve(workflowActivity.UniqueId);
                                            }, function (exception) {
                                                promise.reject(exception);
                                            });
                                        }
                                        promise.resolve(workflowActivity.UniqueId);
                                    }, function (exception) {
                                        promise.reject(exception);
                                    });
                                }
                            }, function (exception) {
                                promise.reject(exception);
                            });
                        }
                        else {
                            promise.resolve(workflowActivity.UniqueId);
                        }
                    }, function (exception) {
                        promise.reject(exception);
                    });
                }
                else {
                    promise.resolve(null);
                }
            }, function (exception) {
                promise.reject(exception);
            });
            return promise.promise();
        };
        /**
         * Gestisco l'attivita' di workflow corrente se ho gia' l'id
         * @param idWorkflowActivityId
         */
        HandlerWorkflowManager.prototype.manageHandlingWorkflowWithActivity = function (idWorkflowActivityId) {
            var _this = this;
            var promise = $.Deferred();
            var workflowActivity;
            this._workflowActivityService.hasHandler(idWorkflowActivityId, function (data) {
                if (!data) {
                    _this._workflowActivityService.getWorkflowActivity(idWorkflowActivityId, function (data) {
                        if (data) {
                            workflowActivity = data;
                            if (!_this.handlingIsAutomatic(workflowActivity.WorkflowProperties)) {
                                promise.resolve(workflowActivity.UniqueId);
                                return;
                            }
                            _this._workflowActivityService.updateHandlingWorkflowActivity(workflowActivity, UpdateActionType.HandlingWorkflow, function (data) {
                                promise.resolve(workflowActivity.UniqueId);
                            }, function (exception) {
                                promise.reject(exception);
                            });
                        }
                    }, function (exception) {
                        promise.reject(exception);
                    });
                }
                else {
                    promise.resolve(idWorkflowActivityId);
                }
            }, function (exception) {
                promise.reject(exception);
            });
            return promise.promise();
        };
        return HandlerWorkflowManager;
    }());
    return HandlerWorkflowManager;
});
//# sourceMappingURL=HandlerWorkflowManager.js.map