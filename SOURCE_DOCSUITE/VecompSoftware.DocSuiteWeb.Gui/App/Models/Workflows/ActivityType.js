define(["require", "exports"], function (require, exports) {
    var ActivityType;
    (function (ActivityType) {
        ActivityType[ActivityType["Undefined"] = -1] = "Undefined";
        ActivityType[ActivityType["AutomaticActivity"] = 0] = "AutomaticActivity";
        ActivityType[ActivityType["ProtocolCreate"] = 1] = "ProtocolCreate";
        ActivityType[ActivityType["PecToProtocol"] = 2] = "PecToProtocol";
        ActivityType[ActivityType["UDSCreate"] = 3] = "UDSCreate";
        ActivityType[ActivityType["UDSToProtocol"] = 4] = "UDSToProtocol";
        ActivityType[ActivityType["UDSToPEC"] = 5] = "UDSToPEC";
        ActivityType[ActivityType["CollaborationCreate"] = 6] = "CollaborationCreate";
        ActivityType[ActivityType["CollaborationSign"] = 7] = "CollaborationSign";
        ActivityType[ActivityType["CollaborationToProtocol"] = 8] = "CollaborationToProtocol";
        ActivityType[ActivityType["FascicleAssignment"] = 9] = "FascicleAssignment";
        ActivityType[ActivityType["DossierAssignment"] = 10] = "DossierAssignment";
        ActivityType[ActivityType["DematerialisationStatement"] = 11] = "DematerialisationStatement";
        ActivityType[ActivityType["SecureDocumentCreate"] = 12] = "SecureDocumentCreate";
        ActivityType[ActivityType["BuildAchive"] = 13] = "BuildAchive";
        ActivityType[ActivityType["BuildProtocol"] = 14] = "BuildProtocol";
        ActivityType[ActivityType["BuildPECMail"] = 15] = "BuildPECMail";
        ActivityType[ActivityType["BuildMessages"] = 16] = "BuildMessages";
        ActivityType[ActivityType["DocumentUnitIntoFascicle"] = 17] = "DocumentUnitIntoFascicle";
        ActivityType[ActivityType["DocumentUnitLinks"] = 18] = "DocumentUnitLinks";
        ActivityType[ActivityType["GenericActivity"] = 19] = "GenericActivity";
    })(ActivityType || (ActivityType = {}));
    return ActivityType;
});
//# sourceMappingURL=ActivityType.js.map