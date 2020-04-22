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
    var UDSDocumentUnitModelMapper = /** @class */ (function (_super) {
        __extends(UDSDocumentUnitModelMapper, _super);
        function UDSDocumentUnitModelMapper() {
            return _super.call(this) || this;
        }
        UDSDocumentUnitModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.IdUDS = source.IdUDS;
            toMap.Relation = source.Relation;
            return toMap;
        };
        return UDSDocumentUnitModelMapper;
    }(BaseMapper));
    return UDSDocumentUnitModelMapper;
});
//# sourceMappingURL=UDSDocumentUnitModelMapper.js.map