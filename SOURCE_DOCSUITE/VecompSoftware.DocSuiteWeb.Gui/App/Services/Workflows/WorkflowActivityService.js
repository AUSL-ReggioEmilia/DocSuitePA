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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/Workflows/WorkflowActivityModelMapper", "App/Models/Workflows/WorkflowStatus"], function (require, exports, BaseService, WorkflowActivityModelMapper, WorkflowStatus) {
    var WorkflowActivityService = /** @class */ (function (_super) {
        __extends(WorkflowActivityService, _super);
        /**
         * Costruttore
         */
        function WorkflowActivityService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        /**
     * Recupera un'attività per ID
     * @param id
     * @param callback
     * @param error
     */
        WorkflowActivityService.prototype.getWorkflowActivity = function (id, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$filter=UniqueId eq " + id + "&$expand=WorkflowAuthorizations,WorkflowProperties,WorkflowInstance($expand=WorkflowRepository)";
            this.getRequest(url, data, function (response) {
                if (callback) {
                    var instance = {};
                    var mapper = new WorkflowActivityModelMapper();
                    instance = mapper.Map(response.value[0]);
                    callback(instance);
                }
            }, error);
        };
        WorkflowActivityService.prototype.getWorkflowActivityByLogType = function (id, workflowStatus, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$filter=UniqueId eq " + id + "&$expand=WorkflowActivityLogs($filter=LogType eq '" + WorkflowStatus[workflowStatus] + "')";
            this.getRequest(url, data, function (response) {
                if (callback) {
                    var instance = {};
                    var mapper = new WorkflowActivityModelMapper();
                    instance = mapper.Map(response.value[0]);
                    callback(instance);
                }
            }, error);
        };
        WorkflowActivityService.prototype.getWorkflowActivityById = function (id, callback, error, expandProperties) {
            var url = this._configuration.ODATAUrl;
            var odataQuery = "$filter=UniqueId eq " + id;
            if (expandProperties && expandProperties.length) {
                odataQuery = odataQuery + "&$expand=" + (expandProperties.join(','));
            }
            this.getRequest(url, odataQuery, function (response) {
                if (callback) {
                    var instance = {};
                    var mapper = new WorkflowActivityModelMapper();
                    instance = mapper.Map(response.value[0]);
                    callback(instance);
                }
            }, error);
        };
        WorkflowActivityService.prototype.isWorkflowActivityHandler = function (workflowActivityId, callback, error) {
            var url = this._configuration.ODATAUrl;
            url = url.concat("/WorkflowActivityService.IsWorkflowActivityHandler(workflowActivityId=", workflowActivityId, ")");
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response.value);
                }
            }, error);
        };
        WorkflowActivityService.prototype.hasHandler = function (workflowActivityId, callback, error) {
            var url = this._configuration.ODATAUrl;
            url = url.concat("/WorkflowActivityService.HasHandler(workflowActivityId=", workflowActivityId, ")");
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response.value);
                }
            }, error);
        };
        /**
       * Effettuo la presa in carico dell'attività di workflow
       * @param model
       * @param callback
       * @param error
       */
        WorkflowActivityService.prototype.updateHandlingWorkflowActivity = function (workflowActivity, updateAction, callback, error) {
            var url = this._configuration.WebAPIUrl.concat("?actionType=", updateAction.toString());
            this.putRequest(url, JSON.stringify(workflowActivity), callback, error);
        };
        /**
         *  Recupero l'attività di Workflow associata all'entità desiderata
         * @param environmentId
         * @param type
         * @param callback
         * @param error
         */
        WorkflowActivityService.prototype.getActiveActivitiesByReferenceIdAndEnvironment = function (referenceId, type, callback, error) {
            var url = this._configuration.ODATAUrl;
            url = url.concat("/WorkflowActivityService.GetActiveActivitiesByReferenceIdAndEnvironment(referenceId=", referenceId, ",type=VecompSoftware.DocSuiteWeb.Entity.Commons.DSWEnvironmentType'", type.toString(), "')");
            this.getRequest(url, null, function (response) {
                if (callback) {
                    var mapper = new WorkflowActivityModelMapper();
                    var instance = {};
                    //TODO: Va ancora gestito il caso di più istanze presente sullo stesso referenceId nella pagina di visualizzazione
                    //dell'environment specifico (fascVisualizza, DossierVisualizza)
                    instance = mapper.Map(response.value[0]);
                    callback(instance);
                }
            }, error);
        };
        WorkflowActivityService.prototype.getWorkflowActivities = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl.concat("?$expand=WorkflowInstance&$filter=WorkflowInstance/UniqueId eq " + uniqueId + " and Status eq 'Done' &$orderby=Name");
            this.getRequest(url, null, function (response) {
                if (callback) {
                    var modelMapper_1 = new WorkflowActivityModelMapper();
                    var workflowActivities_1 = [];
                    $.each(response.value, function (i, value) {
                        workflowActivities_1.push(modelMapper_1.Map(value));
                    });
                    callback(workflowActivities_1);
                }
            }, error);
        };
        WorkflowActivityService.prototype.getActiveByReferenceDocumentUnitId = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl.concat("?$filter=DocumentUnitReferenced/UniqueId eq " + uniqueId + " and (Status eq 'Todo' or Status eq 'Progress')");
            this.getRequest(url, null, function (response) {
                if (callback) {
                    var modelMapper_2 = new WorkflowActivityModelMapper();
                    var workflowActivities_2 = [];
                    $.each(response.value, function (i, value) {
                        workflowActivities_2.push(modelMapper_2.Map(value));
                    });
                    callback(workflowActivities_2);
                }
            }, error);
        };
        WorkflowActivityService.prototype.countActiveByReferenceDocumentUnitId = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "/$count?$filter=DocumentUnitReferenced/UniqueId eq " + uniqueId + " and (Status eq 'Todo' or Status eq 'Progress')";
            url = url.concat(data);
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response);
                }
            }, error);
        };
        WorkflowActivityService.prototype.getByStatusReferenceDocumentUnitId = function (status, uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl.concat("?$filter=DocumentUnitReferenced/UniqueId eq " + uniqueId + " and Status eq '" + status + "'");
            this.getRequest(url, null, function (response) {
                if (callback) {
                    var modelMapper_3 = new WorkflowActivityModelMapper();
                    var workflowActivities_3 = [];
                    $.each(response.value, function (i, value) {
                        workflowActivities_3.push(modelMapper_3.Map(value));
                    });
                    callback(workflowActivities_3);
                }
            }, error);
        };
        WorkflowActivityService.prototype.countByStatusReferenceDocumentUnitId = function (status, uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "/$count?$filter=DocumentUnitReferenced/UniqueId eq " + uniqueId + " and Status eq '" + status + "'";
            url = url.concat(data);
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response);
                }
            }, error);
        };
        return WorkflowActivityService;
    }(BaseService));
    return WorkflowActivityService;
});
//# sourceMappingURL=WorkflowActivityService.js.map