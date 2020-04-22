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
define(["require", "exports", "App/Models/UDS/UDSTypologyModel", "App/Mappers/BaseMapper"], function (require, exports, UDSTypologyModel, BaseMapper) {
    var UDSTypologyModelMapper = /** @class */ (function (_super) {
        __extends(UDSTypologyModelMapper, _super);
        function UDSTypologyModelMapper() {
            return _super.call(this) || this;
        }
        UDSTypologyModelMapper.prototype.Map = function (source) {
            var toMap = new UDSTypologyModel();
            if (!source) {
                return null;
            }
            toMap.Alias = source.Alias;
            toMap.LastChangedDate = source.LastChangeDate;
            toMap.LastChangedUser = source.LastChangedUser;
            toMap.Name = source.Name;
            toMap.RegistrationDate = source.RegistrationDate;
            toMap.RegistrationUser = source.RegistrationUser;
            toMap.Status = source.Status;
            toMap.UniqueId = source.UniqueId;
            return toMap;
        };
        return UDSTypologyModelMapper;
    }(BaseMapper));
    return UDSTypologyModelMapper;
});
//# sourceMappingURL=UDSTypologyModelMapper.js.map