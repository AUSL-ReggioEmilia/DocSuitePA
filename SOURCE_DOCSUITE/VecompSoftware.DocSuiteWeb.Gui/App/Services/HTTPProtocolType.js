define(["require", "exports"], function (require, exports) {
    var HTTPProtocolType;
    (function (HTTPProtocolType) {
        HTTPProtocolType[HTTPProtocolType["GET"] = 0] = "GET";
        HTTPProtocolType[HTTPProtocolType["POST"] = 1] = "POST";
        HTTPProtocolType[HTTPProtocolType["PUT"] = 2] = "PUT";
        HTTPProtocolType[HTTPProtocolType["DELETE"] = 3] = "DELETE";
    })(HTTPProtocolType || (HTTPProtocolType = {}));
    return HTTPProtocolType;
});
//# sourceMappingURL=HTTPProtocolType.js.map