define(["require", "exports"], function (require, exports) {
    var DossierRoleStatus;
    (function (DossierRoleStatus) {
        DossierRoleStatus[DossierRoleStatus["Refused"] = -1] = "Refused";
        DossierRoleStatus[DossierRoleStatus["ToEvaluate"] = 0] = "ToEvaluate";
        DossierRoleStatus[DossierRoleStatus["Active"] = 1] = "Active";
        DossierRoleStatus[DossierRoleStatus["NotActive"] = 2] = "NotActive";
    })(DossierRoleStatus || (DossierRoleStatus = {}));
    return DossierRoleStatus;
});
//# sourceMappingURL=DossierRoleStatus.js.map