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
define(["require", "exports", "App/Models/UserLogs/UserLogsModel", "App/Mappers/BaseMapper", "./UserProfileMapper", "../../Helpers/RequireJSHelper"], function (require, exports, UserLogsModel, BaseMapper, UserProfileMapper, RequireJSHelper) {
    var UserLogsMapper = /** @class */ (function (_super) {
        __extends(UserLogsMapper, _super);
        function UserLogsMapper() {
            return _super.call(this) || this;
        }
        UserLogsMapper.prototype.Map = function (source) {
            var toMap = new UserLogsModel();
            if (!source) {
                return null;
            }
            var _userProfileMapper = RequireJSHelper.getModule(UserProfileMapper, 'App/Mappers/UserLogs/UserProfileMapper');
            toMap.UniqueId = source.UniqueId;
            toMap.SystemUser = source.SystemUser;
            toMap.SystemServer = source.SystemServer;
            toMap.SystemComputer = source.SystemComputer;
            toMap.AccessNumber = source.AccessNumber;
            toMap.PrevOperationDate = source.PrevOperationDate;
            toMap.SessionId = source.SessionId;
            toMap.AdvancedScanner = source.AdvancedScanner;
            toMap.AdvancedViewer = source.AdvancedViewer;
            toMap.UserMail = source.UserMail;
            toMap.MobilePhone = source.MobilePhone;
            toMap.DefaultAdaptiveSearchControls = source.DefaultAdaptiveSearchControls;
            toMap.AdaptiveSearchStatistics = source.AdaptiveSearchStatistics;
            toMap.AdaptiveSearchEvaluated = source.AdaptiveSearchEvaluated;
            toMap.PrivacyLevel = source.PrivacyLevel;
            toMap.CurrentTenantId = source.CurrentTenantId;
            toMap.UserProfile = source.UserProfile ? _userProfileMapper.Map(JSON.parse(source.UserProfile)) : null;
            toMap.EntityId = source.EntityId;
            toMap.EntityShortId = source.EntityShortId;
            toMap.RegistrationUser = source.RegistrationUser;
            toMap.RegistrationDate = source.RegistrationDate;
            toMap.LastChangedUser = source.LastChangedUser;
            toMap.LastChangedDate = source.LastChangedDate;
            return toMap;
        };
        return UserLogsMapper;
    }(BaseMapper));
    return UserLogsMapper;
});
//# sourceMappingURL=UserLogsMapper.js.map