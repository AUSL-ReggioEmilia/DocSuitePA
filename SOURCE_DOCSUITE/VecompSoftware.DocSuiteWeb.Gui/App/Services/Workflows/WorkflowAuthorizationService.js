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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/Workflows/WorkflowAuthorizationModelMapper"], function (require, exports, BaseService, WorkflowAuthorizationModelMapper) {
    var WorkflowAuthorizationService = /** @class */ (function (_super) {
        __extends(WorkflowAuthorizationService, _super);
        function WorkflowAuthorizationService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            _this._mapper = new WorkflowAuthorizationModelMapper();
            return _this;
        }
        /**
         * Recupero le WorkflowAuthorizations con l'id dell'attività
         * @param workflowActivityId
         * @param callback
         * @param error
         */
        WorkflowAuthorizationService.prototype.getAuthorizationsByActivity = function (workflowActivityId, callback, error) {
            var _this = this;
            var url = this._configuration.ODATAUrl;
            var data = "WorkflowAuthorizations?$filter=WorkflowActivity/UniqueId eq ".concat(workflowActivityId);
            this.getRequest(url, data, function (response) {
                if (callback) {
                    var workflowAuthorizations = [];
                    if (response) {
                        workflowAuthorizations = _this._mapper.MapCollection(response.value);
                        callback(workflowAuthorizations);
                    }
                }
            }, error);
        };
        /**
         * Verifica se un determinato utente è autorizzato in una specifica workflowactivity
         * @param account
         * @param workflowActivityId
         * @param callback
         * @param error
         */
        WorkflowAuthorizationService.prototype.isUserAuthorized = function (account, workflowActivityId, callback, error) {
            var _this = this;
            var url = this._configuration.ODATAUrl;
            var qs = "$filter=WorkflowActivity/UniqueId eq " + workflowActivityId + " and Account eq '" + account + "'";
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    var workflowAuthorizations = [];
                    if (response) {
                        workflowAuthorizations = _this._mapper.MapCollection(response.value);
                        callback(workflowAuthorizations.length > 0);
                    }
                }
            }, error);
        };
        return WorkflowAuthorizationService;
    }(BaseService));
    return WorkflowAuthorizationService;
});
//# sourceMappingURL=WorkflowAuthorizationService.js.map