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
define(["require", "exports", "App/Core/Serialization/Serializable"], function (require, exports, Serializable) {
    var RadTreeNodeViewModel = /** @class */ (function (_super) {
        __extends(RadTreeNodeViewModel, _super);
        function RadTreeNodeViewModel() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        RadTreeNodeViewModel.prototype.initClassMapping = function () {
        };
        return RadTreeNodeViewModel;
    }(Serializable));
    return RadTreeNodeViewModel;
});
//# sourceMappingURL=RadTreeNodeViewModel.js.map