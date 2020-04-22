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
define(["require", "exports", "App/Mappers/BaseMapper", "App/Mappers/Fascicles/FascicleModelMapper", "App/Mappers/DocumentUnits/DocumentUnitModelMapper", "App/Helpers/RequireJSHelper"], function (require, exports, BaseMapper, FascicleModelMapper, DocumentUnitModelMapper, RequireJSHelper) {
    var FascicleDocumentUnitModelMapper = /** @class */ (function (_super) {
        __extends(FascicleDocumentUnitModelMapper, _super);
        function FascicleDocumentUnitModelMapper() {
            return _super.call(this) || this;
        }
        FascicleDocumentUnitModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            var _fascicleModelMapper;
            _fascicleModelMapper = RequireJSHelper.getModule(FascicleModelMapper, 'App/Mappers/Fascicles/FascicleModelMapper');
            toMap.Fascicle = source.Fascicle ? _fascicleModelMapper.Map(source.Fascicle) : null;
            toMap.UniqueId = source.UniqueId;
            toMap.DocumentUnit = source.DocumentUnit ? new DocumentUnitModelMapper().Map(source.DocumentUnit) : null;
            toMap.ReferenceType = source.ReferenceType;
            return toMap;
        };
        return FascicleDocumentUnitModelMapper;
    }(BaseMapper));
    return FascicleDocumentUnitModelMapper;
});
//# sourceMappingURL=FascicleDocumentUnitModelMapper.js.map