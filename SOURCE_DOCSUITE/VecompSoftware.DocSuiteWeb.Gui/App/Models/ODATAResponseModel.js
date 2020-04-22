define(["require", "exports"], function (require, exports) {
    var ODATAResponseModel = /** @class */ (function () {
        function ODATAResponseModel(model) {
            this.context = model['@odata.context'];
            this.count = model['@odata.count'];
        }
        return ODATAResponseModel;
    }());
    return ODATAResponseModel;
});
//# sourceMappingURL=ODATAResponseModel.js.map