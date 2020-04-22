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
    var ContactTypeDSWModelMapper = /** @class */ (function (_super) {
        __extends(ContactTypeDSWModelMapper, _super);
        function ContactTypeDSWModelMapper() {
            return _super.call(this) || this;
        }
        ContactTypeDSWModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            switch (source.IdContactType) {
                case "Administration":
                    toMap.ContactTypeId = "M";
                    break;
                case "AOO":
                    toMap.ContactTypeId = "A";
                    break;
                case "AO":
                    toMap.ContactTypeId = "U";
                    break;
                case "Role":
                    toMap.ContactTypeId = "R";
                    break;
                case "Group":
                    toMap.ContactTypeId = "G";
                    break;
                case "Sector":
                    toMap.ContactTypeId = "S";
                    break;
                case "Citizen":
                    toMap.ContactTypeId = "P";
                    break;
                case "IPA":
                    toMap.ContactTypeId = "I";
                    break;
                default:
                    toMap.ContactTypeId = source.IdContactType;
            }
            return toMap;
        };
        return ContactTypeDSWModelMapper;
    }(BaseMapper));
    return ContactTypeDSWModelMapper;
});
//# sourceMappingURL=ContactTypeDSWModelMapper.js.map