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
define(["require", "exports", "App/Models/UserLogs/UserProfileModel", "App/Mappers/BaseMapper"], function (require, exports, UserProfileModel, BaseMapper) {
    var UserProfileMapper = /** @class */ (function (_super) {
        __extends(UserProfileMapper, _super);
        function UserProfileMapper() {
            return _super.call(this) || this;
        }
        UserProfileMapper.prototype.Map = function (source) {
            var toMap = new UserProfileModel();
            if (!source) {
                return null;
            }
            toMap.DefaultProvider = source.DefaultProvider;
            toMap.ArubaAutomatic = source.Value.ArubaAutomatic;
            toMap.ArubaRemote = source.Value.ArubaRemote;
            toMap.Smartcard = source.Value.Smartcard;
            toMap.InfocertAutomatic = source.Value.InfocertAutomatic;
            toMap.InfocertRemote = source.Value.InfocertRemote;
            return toMap;
        };
        return UserProfileMapper;
    }(BaseMapper));
    return UserProfileMapper;
});
//# sourceMappingURL=UserProfileMapper.js.map