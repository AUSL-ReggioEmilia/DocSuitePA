/// <amd-dependency path="../../core/extensions/string" />
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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/Workflows/WorkflowRepositoryModelMapper", "App/Models/ODATAResponseModel", "App/Models/Workflows/WorkflowDSWEnvironmentType", "../../core/extensions/string"], function (require, exports, BaseService, WorkflowRepositoryModelMapper, ODATAResponseModel, WorkflowDSWEnvironmentType) {
    var WorkflowRepositoryService = /** @class */ (function (_super) {
        __extends(WorkflowRepositoryService, _super);
        function WorkflowRepositoryService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        WorkflowRepositoryService.prototype.getById = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl.concat("?$filter=UniqueId eq ", uniqueId, "&$expand=WorkflowRoleMappings($expand=Role),WorkflowEvaluationProperties($orderby=Name)");
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response.value[0]);
                }
            }, error);
        };
        WorkflowRepositoryService.prototype.getByName = function (name, callback, error) {
            var url = this._configuration.ODATAUrl;
            url = url.concat("?$orderby=Name");
            if (!String.isNullOrEmpty(name)) {
                url = url.concat("&$filter=contains(Name, '", name, "')");
            }
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response.value);
                }
            }, error);
        };
        WorkflowRepositoryService.prototype.getByEnvironment = function (environment, filters, anyEnv, documentRequired, callback, error) {
            var url = this._configuration.ODATAUrl;
            url = url.concat("/WorkflowRepositoryService.GetAuthorizedActiveWorkflowRepositories(environment=" + environment + ",anyEnv=" + anyEnv + ",documentRequired=" + documentRequired + ")?$orderby=Name");
            if (!String.isNullOrEmpty(filters)) {
                url = url.concat("&$filter=contains(tolower(Name), '", filters.toLowerCase(), "')");
            }
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response.value);
                }
            }, error);
        };
        WorkflowRepositoryService.prototype.getRepositoryByEnvironment = function (environment, callback, error) {
            var url = this._configuration.ODATAUrl;
            url = url.concat("?$filter=DSWEnvironment eq '" + WorkflowDSWEnvironmentType[environment] + "'");
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var modelMapper_1 = new WorkflowRepositoryModelMapper();
                    var workflowRepositories_1 = [];
                    $.each(response.value, function (i, value) {
                        workflowRepositories_1.push(modelMapper_1.Map(value));
                    });
                    callback(workflowRepositories_1);
                }
            }, error);
        };
        WorkflowRepositoryService.prototype.getByWorkflowActivityId = function (workflowActivityId, callback, error) {
            var url = this._configuration.ODATAUrl;
            url = url.concat("/WorkflowRepositoryService.GetByWorkflowActivityId(workflowActivityId=", workflowActivityId.toString(), ")");
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response.value[0]);
                }
            }, error);
        };
        WorkflowRepositoryService.prototype.getWorkflowRepositories = function (callback, error) {
            var url = this._configuration.ODATAUrl;
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var workflowRepositories = response.value;
                    callback(workflowRepositories);
                }
                ;
            }, error);
        };
        WorkflowRepositoryService.prototype.getAllWorkflowRepositories = function (name, topElement, skipElement, callback, error) {
            var url = this._configuration.ODATAUrl;
            var qs = "$filter=contains(Name,'" + name + "')&$count=true&$top=" + topElement + "&$skip=" + skipElement.toString();
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    var responseModel = new ODATAResponseModel(response);
                    var mapper = new WorkflowRepositoryModelMapper();
                    responseModel.value = mapper.MapCollection(response.value);
                    ;
                    callback(responseModel);
                }
            }, error);
        };
        WorkflowRepositoryService.prototype.hasAuthorizedWorkflowRepositories = function (environment, anyenv, callback, error) {
            var url = this._configuration.ODATAUrl;
            url = url.concat("/WorkflowRepositoryService.HasAuthorizedWorkflowRepositories(environment=", environment.toString(), ",anyEnv=", anyenv.toString(), ")");
            this.getRequest(url, null, function (response) {
                if (callback) {
                    if (!response) {
                        callback(undefined);
                        return;
                    }
                    callback(response.value);
                }
            }, error);
        };
        return WorkflowRepositoryService;
    }(BaseService));
    return WorkflowRepositoryService;
});
//# sourceMappingURL=WorkflowRepositoryService.js.map