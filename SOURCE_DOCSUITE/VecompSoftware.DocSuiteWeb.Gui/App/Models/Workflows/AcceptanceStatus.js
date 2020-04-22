define(["require", "exports"], function (require, exports) {
    var AcceptanceStatus;
    (function (AcceptanceStatus) {
        AcceptanceStatus[AcceptanceStatus["Invalid"] = 0] = "Invalid";
        AcceptanceStatus[AcceptanceStatus["Accepted"] = 1] = "Accepted";
        AcceptanceStatus[AcceptanceStatus["Refused"] = 2] = "Refused";
    })(AcceptanceStatus || (AcceptanceStatus = {}));
    return AcceptanceStatus;
});
//# sourceMappingURL=AcceptanceStatus.js.map