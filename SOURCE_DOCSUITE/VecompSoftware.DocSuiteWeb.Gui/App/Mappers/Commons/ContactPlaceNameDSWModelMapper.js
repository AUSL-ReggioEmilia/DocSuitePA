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
    var ContactPlaceNameDSWModelMapper = /** @class */ (function (_super) {
        __extends(ContactPlaceNameDSWModelMapper, _super);
        function ContactPlaceNameDSWModelMapper() {
            return _super.call(this) || this;
        }
        ContactPlaceNameDSWModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source || !source.PlaceName) {
                return null;
            }
            toMap.Id = source.PlaceName.EntityShortId;
            toMap.Description = source.PlaceName.Description;
            return toMap;
        };
        return ContactPlaceNameDSWModelMapper;
    }(BaseMapper));
    return ContactPlaceNameDSWModelMapper;
});
//# sourceMappingURL=ContactPlaceNameDSWModelMapper.js.map