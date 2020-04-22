import UserProfileModel = require("./UserProfileModel");

class UserLogsModel {
    UniqueId: string;
    SystemUser: string;
    SystemServer: string;
    SystemComputer: string;
    AccessNumber: number;
    PrevOperationDate: Date;
    SessionId: string;
    AdvancedScanner: boolean;
    AdvancedViewer: boolean;
    UserMail: string;
    MobilePhone: string;
    DefaultAdaptiveSearchControls: string;
    AdaptiveSearchStatistics: string;
    AdaptiveSearchEvaluated: string;
    PrivacyLevel: number;
    CurrentTenantId: string;
    UserProfile: UserProfileModel;
    EntityId: number;
    EntityShortId: number;
    RegistrationUser: string;
    RegistrationDate: Date;
    LastChangedUser: string;
    LastChangedDate: Date
}

export = UserLogsModel;