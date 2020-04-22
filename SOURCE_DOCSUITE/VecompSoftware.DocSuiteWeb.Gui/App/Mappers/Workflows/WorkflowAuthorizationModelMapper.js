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
define(["require", "exports", "App/Mappers/BaseMapper"], function (require, exports, BaseMapper) {
    var WorkflowAuthorizationModelMapper = /** @class */ (function (_super) {
        __extends(WorkflowAuthorizationModelMapper, _super);
        function WorkflowAuthorizationModelMapper() {
            return _super.call(this) || this;
        }
        WorkflowAuthorizationModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.Account = source.Account;
            toMap.UniqueId = source.UniqueId;
            toMap.IsHandler = source.IsHandler;
            return toMap;
        };
        return WorkflowAuthorizationModelMapper;
    }(BaseMapper));
    return WorkflowAuthorizationModelMapper;
});
//# sourceMappingURL=WorkflowAuthorizationModelMapper.js.map