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
define(["require", "exports", "App/Mappers/BaseMapper", "./ContactPlaceNameDSWModelMapper"], function (require, exports, BaseMapper, ContactPlaceNameDSWModelMapper) {
    var ContactAddressDSWModelMapper = /** @class */ (function (_super) {
        __extends(ContactAddressDSWModelMapper, _super);
        function ContactAddressDSWModelMapper() {
            return _super.call(this) || this;
        }
        ContactAddressDSWModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.Address = source.Address;
            toMap.CivicNumber = source.CivicNumber;
            toMap.ZipCode = source.ZipCode;
            toMap.City = source.City;
            toMap.CityCode = source.CityCode;
            toMap.PlaceName = new ContactPlaceNameDSWModelMapper().Map(source);
            return toMap;
        };
        return ContactAddressDSWModelMapper;
    }(BaseMapper));
    return ContactAddressDSWModelMapper;
});
//# sourceMappingURL=ContactAddressDSWModelMapper.js.map