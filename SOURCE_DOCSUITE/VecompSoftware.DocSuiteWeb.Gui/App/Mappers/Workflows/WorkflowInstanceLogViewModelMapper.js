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
define(["require", "exports", "App/Mappers/BaseMapper", "App/Helpers/EnumHelper"], function (require, exports, BaseMapper, EnumHelper) {
    var WorkflowInstanceLogViewModelMapper = /** @class */ (function (_super) {
        __extends(WorkflowInstanceLogViewModelMapper, _super);
        function WorkflowInstanceLogViewModelMapper() {
            var _this = _super.call(this) || this;
            _this._enumHelper = new EnumHelper();
            return _this;
        }
        WorkflowInstanceLogViewModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.Computer = source.SystemComputer;
            toMap.Description = source.LogDescription;
            toMap.TypeDescription = this._enumHelper.getWorkflowInstanceLogDescription(source.LogType);
            toMap.LogDate = moment(source.RegistrationDate).format("L").concat(" ").concat(moment(source.RegistrationDate).format("LTS"));
            toMap.LogUser = source.RegistrationUser;
            return toMap;
        };
        return WorkflowInstanceLogViewModelMapper;
    }(BaseMapper));
    return WorkflowInstanceLogViewModelMapper;
});
//# sourceMappingURL=WorkflowInstanceLogViewModelMapper.js.map