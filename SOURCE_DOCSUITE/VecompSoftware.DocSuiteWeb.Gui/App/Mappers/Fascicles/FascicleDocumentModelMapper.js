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
define(["require", "exports", "App/Mappers/BaseMapper", "App/Mappers/Fascicles/FascicleModelMapper", "App/Helpers/RequireJSHelper"], function (require, exports, BaseMapper, FascicleModelMapper, RequireJSHelper) {
    var FascicleDocumentModelMapper = /** @class */ (function (_super) {
        __extends(FascicleDocumentModelMapper, _super);
        function FascicleDocumentModelMapper() {
            return _super.call(this) || this;
        }
        FascicleDocumentModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            var _fascicleModelMapper;
            _fascicleModelMapper = RequireJSHelper.getModule(FascicleModelMapper, 'App/Mappers/Fascicles/FascicleModelMapper');
            toMap.ChainType = source.ChainType;
            toMap.Fascicle = source.Fascicle ? _fascicleModelMapper.Map(source.Fascicle) : null;
            toMap.IdArchiveChain = source.IdArchiveChain;
            toMap.UniqueId = source.UniqueId;
            return toMap;
        };
        return FascicleDocumentModelMapper;
    }(BaseMapper));
    return FascicleDocumentModelMapper;
});
//# sourceMappingURL=FascicleDocumentModelMapper.js.map