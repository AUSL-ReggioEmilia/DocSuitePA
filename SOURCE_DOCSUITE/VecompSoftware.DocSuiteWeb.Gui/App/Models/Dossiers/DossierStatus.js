define(["require", "exports"], function (require, exports) {
    var DossierStatus;
    (function (DossierStatus) {
        DossierStatus[DossierStatus["Canceled"] = 0] = "Canceled";
        DossierStatus[DossierStatus["Open"] = 1] = "Open";
        DossierStatus[DossierStatus["Closed"] = 2] = "Closed";
    })(DossierStatus || (DossierStatus = {}));
    return DossierStatus;
});
//# sourceMappingURL=DossierStatus.js.map