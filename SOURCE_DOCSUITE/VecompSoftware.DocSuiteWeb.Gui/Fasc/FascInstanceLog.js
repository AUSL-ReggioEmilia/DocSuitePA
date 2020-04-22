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
define(["require", "exports", "Fasc/FascBase", "App/Helpers/ServiceConfigurationHelper", "App/Services/Workflows/WorkflowInstancelogService", "UserControl/uscWorkflowInstanceLog"], function (require, exports, FascBase, ServiceConfigurationHelper, WorkflowInstanceLogService, UscWorkflowInstanceLog) {
    var FascInstanceLog = /** @class */ (function (_super) {
        __extends(FascInstanceLog, _super);
        /**
         * Costruttore
         */
        function FascInstanceLog(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, FascBase.FASCICLE_TYPE_NAME)) || this;
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        /**
         * inizializzazione
         */
        FascInstanceLog.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            var workflowInstanceLogServiceCongiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowInstanceLog");
            this._workflowInstanceLogService = new WorkflowInstanceLogService(workflowInstanceLogServiceCongiguration);
            this.loadData(0);
            $("#".concat(this.uscWorkflowInstanceLogsId)).on(UscWorkflowInstanceLog.ON_PAGE_CHANGE, function (args, data) {
                _this.loadData(data);
            });
        };
        /**
         * carico i dati nello usercontrol
         * @param skip
         */
        FascInstanceLog.prototype.loadData = function (skip) {
            var _this = this;
            this._workflowInstanceLogService.getFascicleInstanceLogs(this.IdFascicle, skip, 30, function (data) {
                if (data) {
                    var uscWorkflowInstanceLog = $("#".concat(_this.uscWorkflowInstanceLogsId)).data();
                    if (!jQuery.isEmptyObject(uscWorkflowInstanceLog)) {
                        uscWorkflowInstanceLog.setGrid(data);
                    }
                }
            });
        };
        return FascInstanceLog;
    }(FascBase));
    return FascInstanceLog;
});
//# sourceMappingURL=FascInstanceLog.js.map