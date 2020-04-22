define(["require", "exports"], function (require, exports) {
    var ProviderSignType;
    (function (ProviderSignType) {
        ProviderSignType[ProviderSignType["Smartcard"] = 0] = "Smartcard";
        ProviderSignType[ProviderSignType["ArubaRemote"] = 1] = "ArubaRemote";
        ProviderSignType[ProviderSignType["InfocertRemote"] = 2] = "InfocertRemote";
        ProviderSignType[ProviderSignType["ArubaAutomatic"] = 3] = "ArubaAutomatic";
        ProviderSignType[ProviderSignType["InfocertAutomatic"] = 4] = "InfocertAutomatic";
    })(ProviderSignType || (ProviderSignType = {}));
    return ProviderSignType;
});
//# sourceMappingURL=ProviderSignType.js.map