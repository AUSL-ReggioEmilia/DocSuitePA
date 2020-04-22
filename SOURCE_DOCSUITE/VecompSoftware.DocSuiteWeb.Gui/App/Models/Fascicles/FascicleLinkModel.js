define(["require", "exports", "App/Models/Fascicles/FascicleModel"], function (require, exports, FascicleModel) {
    var FascicleLinkModel = /** @class */ (function () {
        function FascicleLinkModel(idFascicle) {
            this.FascicleLinked = new FascicleModel();
            this.FascicleLinked.UniqueId = idFascicle;
        }
        return FascicleLinkModel;
    }());
    return FascicleLinkModel;
});
//# sourceMappingURL=FascicleLinkModel.js.map