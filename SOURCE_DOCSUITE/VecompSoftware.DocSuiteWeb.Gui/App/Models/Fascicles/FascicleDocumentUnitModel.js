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
define(["require", "exports", "App/Models/Fascicles/FascicolableBaseModel"], function (require, exports, FascicolableBaseModel) {
    var FascicleDocumentUnitModel = /** @class */ (function (_super) {
        __extends(FascicleDocumentUnitModel, _super);
        function FascicleDocumentUnitModel(idFascicle) {
            return _super.call(this, idFascicle) || this;
        }
        return FascicleDocumentUnitModel;
    }(FascicolableBaseModel));
    return FascicleDocumentUnitModel;
});
//# sourceMappingURL=FascicleDocumentUnitModel.js.map