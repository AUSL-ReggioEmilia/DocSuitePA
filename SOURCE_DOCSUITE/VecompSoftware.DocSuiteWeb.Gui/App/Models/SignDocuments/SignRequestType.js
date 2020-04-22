define(["require", "exports"], function (require, exports) {
    var SignRequestType;
    (function (SignRequestType) {
        SignRequestType[SignRequestType["Cades"] = 0] = "Cades";
        SignRequestType[SignRequestType["CadesWithMarcaTemporale"] = 1] = "CadesWithMarcaTemporale";
        SignRequestType[SignRequestType["Pades"] = 2] = "Pades";
        SignRequestType[SignRequestType["PadesWithMarcaTemporale"] = 3] = "PadesWithMarcaTemporale";
        SignRequestType[SignRequestType["XadesEnveloped"] = 4] = "XadesEnveloped";
        SignRequestType[SignRequestType["XadesEnveloping"] = 5] = "XadesEnveloping";
        SignRequestType[SignRequestType["XadesDetached"] = 6] = "XadesDetached";
        SignRequestType[SignRequestType["XadesTEnveloped"] = 7] = "XadesTEnveloped";
        SignRequestType[SignRequestType["XadesTEnveloping"] = 8] = "XadesTEnveloping";
        SignRequestType[SignRequestType["XadesTDetached"] = 9] = "XadesTDetached";
    })(SignRequestType || (SignRequestType = {}));
    return SignRequestType;
});
//# sourceMappingURL=SignRequestType.js.map