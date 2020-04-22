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
define(["require", "exports", "App/Services/BaseService", "../../core/extensions/string"], function (require, exports, BaseService) {
    var WorkflowRoleMappingService = /** @class */ (function (_super) {
        __extends(WorkflowRoleMappingService, _super);
        function WorkflowRoleMappingService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        WorkflowRoleMappingService.prototype.deleteWorkflowRoleMapping = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.deleteRequest(url, JSON.stringify(model), callback, error);
        };
        WorkflowRoleMappingService.prototype.getByName = function (name, idRepository, callback, error) {
            var url = this._configuration.ODATAUrl.concat("?$filter=WorkflowRepository/UniqueId eq ", idRepository);
            if (!String.isNullOrEmpty(name)) {
                url = url.concat(" and contains(MappingTag, '", name, "')");
            }
            url = url.concat("&$apply=groupby((MappingTag,WorkflowRepository/UniqueId))");
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response.value);
                }
            }, error);
        };
        return WorkflowRoleMappingService;
    }(BaseService));
    return WorkflowRoleMappingService;
});
//# sourceMappingURL=WorkflowRoleMappingService.js.map