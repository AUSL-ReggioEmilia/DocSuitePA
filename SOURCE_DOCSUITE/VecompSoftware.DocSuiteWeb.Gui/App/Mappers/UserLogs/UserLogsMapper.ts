import UserLogsModel = require("App/Models/UserLogs/UserLogsModel");
import BaseMapper = require('App/Mappers/BaseMapper');
import UserProfileMapper = require("./UserProfileMapper");
import RequireJSHelper = require("../../Helpers/RequireJSHelper");

class UserLogsMapper extends BaseMapper<UserLogsModel>{
    constructor() {
        super();
    }

    public Map(source: any): UserLogsModel {
        let toMap: UserLogsModel = new UserLogsModel();

        if (!source) {
            return null;
        }

        const _userProfileMapper: UserProfileMapper = RequireJSHelper.getModule<UserProfileMapper>(UserProfileMapper, 'App/Mappers/UserLogs/UserProfileMapper');


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
        toMap.UserProfile = source.UserProfile? _userProfileMapper.Map(JSON.parse(source.UserProfile)):null;
        toMap.EntityId = source.EntityId;
        toMap.EntityShortId = source.EntityShortId;
        toMap.RegistrationUser = source.RegistrationUser;
        toMap.RegistrationDate = source.RegistrationDate;
        toMap.LastChangedUser = source.LastChangedUser;
        toMap.LastChangedDate = source.LastChangedDate;

        return toMap;
    }
}

export = UserLogsMapper;