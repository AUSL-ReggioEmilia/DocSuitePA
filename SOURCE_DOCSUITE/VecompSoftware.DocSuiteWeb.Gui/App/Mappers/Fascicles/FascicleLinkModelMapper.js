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
define(["require", "exports", "App/Mappers/Fascicles/FascicleModelMapper", "App/Mappers/BaseMapper", "App/Helpers/RequireJSHelper"], function (require, exports, FascicleModelMapper, BaseMapper, RequireJSHelper) {
    var FascicleLinkModelMapper = /** @class */ (function (_super) {
        __extends(FascicleLinkModelMapper, _super);
        function FascicleLinkModelMapper() {
            return _super.call(this) || this;
        }
        FascicleLinkModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            var _fascicleModelMapper;
            _fascicleModelMapper = RequireJSHelper.getModule(FascicleModelMapper, 'App/Mappers/Fascicles/FascicleModelMapper');
            toMap.Fascicle = source.Fascicle ? _fascicleModelMapper.Map(source.Fascicle) : null;
            toMap.FascicleLinked = source.FascicleLinked ? _fascicleModelMapper.Map(source.FascicleLinked) : null;
            toMap.FascicleLinkType = source.FascicleLinkType;
            toMap.UniqueId = source.UniqueId;
            return toMap;
        };
        return FascicleLinkModelMapper;
    }(BaseMapper));
    return FascicleLinkModelMapper;
});
//# sourceMappingURL=FascicleLinkModelMapper.js.map