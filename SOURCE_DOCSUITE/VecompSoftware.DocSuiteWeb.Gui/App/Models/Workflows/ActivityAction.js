define(["require", "exports"], function (require, exports) {
    var ActivityAction;
    (function (ActivityAction) {
        ActivityAction[ActivityAction["Create"] = 0] = "Create";
        ActivityAction[ActivityAction["ToProtocol"] = 1] = "ToProtocol";
        ActivityAction[ActivityAction["ToPEC"] = 2] = "ToPEC";
        ActivityAction[ActivityAction["ToCollaboration"] = 3] = "ToCollaboration";
        ActivityAction[ActivityAction["ToDesk"] = 4] = "ToDesk";
        ActivityAction[ActivityAction["ToResolution"] = 5] = "ToResolution";
        ActivityAction[ActivityAction["ToSign"] = 6] = "ToSign";
        ActivityAction[ActivityAction["ToAssignment"] = 7] = "ToAssignment";
        ActivityAction[ActivityAction["ToSecure"] = 8] = "ToSecure";
        ActivityAction[ActivityAction["ToFascicle"] = 9] = "ToFascicle";
        ActivityAction[ActivityAction["ToDocumentUnit"] = 10] = "ToDocumentUnit";
        ActivityAction[ActivityAction["ToArchive"] = 11] = "ToArchive";
        ActivityAction[ActivityAction["ToMessage"] = 12] = "ToMessage";
        ActivityAction[ActivityAction["CancelProtocol"] = 13] = "CancelProtocol";
        ActivityAction[ActivityAction["CancelArchive"] = 14] = "CancelArchive";
        ActivityAction[ActivityAction["CancelDocumentUnit"] = 15] = "CancelDocumentUnit";
        ActivityAction[ActivityAction["ToApprove"] = 16] = "ToApprove";
        ActivityAction[ActivityAction["ToShare"] = 17] = "ToShare";
        ActivityAction[ActivityAction["UpdateArchive"] = 18] = "UpdateArchive";
        ActivityAction[ActivityAction["UpdateFascicle"] = 19] = "UpdateFascicle";
        ActivityAction[ActivityAction["ToIntegration"] = 20] = "ToIntegration";
        ActivityAction[ActivityAction["GenerateReport"] = 21] = "GenerateReport";
        ActivityAction[ActivityAction["CopyFascicleContents"] = 22] = "CopyFascicleContents";
    })(ActivityAction || (ActivityAction = {}));
    return ActivityAction;
});
//# sourceMappingURL=ActivityAction.js.map