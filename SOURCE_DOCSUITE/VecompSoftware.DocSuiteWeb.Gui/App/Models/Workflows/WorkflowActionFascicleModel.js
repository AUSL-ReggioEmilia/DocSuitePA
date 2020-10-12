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
define(["require", "exports", "App/Models/Workflows/WorkflowActionModel"], function (require, exports, WorkflowActionModel) {
    var WorkflowActionFascicleModel = /** @class */ (function (_super) {
        __extends(WorkflowActionFascicleModel, _super);
        function WorkflowActionFascicleModel() {
            var _this = _super.call(this) || this;
            _this.$type = "VecompSoftware.DocSuiteWeb.Model.Workflow.Actions.WorkflowActionFascicleModel, VecompSoftware.DocSuiteWeb.Model";
            return _this;
        }
        return WorkflowActionFascicleModel;
    }(WorkflowActionModel));
    return WorkflowActionFascicleModel;
});
//# sourceMappingURL=WorkflowActionFascicleModel.js.map