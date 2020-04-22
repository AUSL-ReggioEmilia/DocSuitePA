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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/Workflows/WorkflowInstanceModelMapper"], function (require, exports, BaseService, WorkflowInstanceModelMapper) {
    var WorkflowInstanceService = /** @class */ (function (_super) {
        __extends(WorkflowInstanceService, _super);
        function WorkflowInstanceService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        WorkflowInstanceService.prototype.getWorkflowInstances = function (searchFilter, callback, error) {
            var url = this._configuration.ODATAUrl.concat("?$expand=WorkflowRepository($select=Name),WorkflowActivities($orderby=RegistrationDate)&$orderby=RegistrationDate desc");
            var oDataFilters = "&$filter=1 eq 1";
            if (searchFilter.name) {
                oDataFilters = oDataFilters.concat(" and contains(WorkflowRepository/Name, '" + searchFilter.name + "')");
            }
            if (searchFilter.status) {
                oDataFilters = oDataFilters.concat(" and Status eq '" + searchFilter.status + "'");
            }
            if (searchFilter.name) {
                oDataFilters = oDataFilters.concat(" and contains(WorkflowRepository/Name, '" + searchFilter.name + "')");
            }
            if (searchFilter.activeFrom) {
                oDataFilters = oDataFilters.concat(" and RegistrationDate gt " + searchFilter.activeFrom);
            }
            if (searchFilter.activeTo) {
                oDataFilters = oDataFilters.concat(" and RegistrationDate lt " + searchFilter.activeTo);
            }
            this.getRequest(url.concat(oDataFilters), null, function (response) {
                if (callback && response) {
                    var modelMapper_1 = new WorkflowInstanceModelMapper();
                    var workflowInstances_1 = [];
                    $.each(response.value, function (i, value) {
                        workflowInstances_1.push(modelMapper_1.Map(value));
                    });
                    callback(workflowInstances_1);
                }
                ;
            }, error);
        };
        return WorkflowInstanceService;
    }(BaseService));
    return WorkflowInstanceService;
});
//# sourceMappingURL=WorkflowInstanceService.js.map