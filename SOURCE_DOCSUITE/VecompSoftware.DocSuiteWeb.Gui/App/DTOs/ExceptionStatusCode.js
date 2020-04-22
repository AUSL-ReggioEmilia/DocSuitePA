define(["require", "exports"], function (require, exports) {
    var ExceptionStatusCode;
    (function (ExceptionStatusCode) {
        ExceptionStatusCode[ExceptionStatusCode["BadRequest"] = 400] = "BadRequest";
        ExceptionStatusCode[ExceptionStatusCode["Unauthorized"] = 401] = "Unauthorized";
        ExceptionStatusCode[ExceptionStatusCode["InternalServerError"] = 500] = "InternalServerError";
    })(ExceptionStatusCode || (ExceptionStatusCode = {}));
    return ExceptionStatusCode;
});
//# sourceMappingURL=ExceptionStatusCode.js.map