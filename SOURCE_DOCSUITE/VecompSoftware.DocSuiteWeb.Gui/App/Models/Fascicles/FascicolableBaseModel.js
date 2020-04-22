define(["require", "exports", "App/Models/Fascicles/FascicleModel"], function (require, exports, FascicleModel) {
    var FascicolableBaseModel = /** @class */ (function () {
        function FascicolableBaseModel(idFascicle) {
            if (idFascicle != null) {
                this.Fascicle = new FascicleModel();
                this.Fascicle.UniqueId = idFascicle;
            }
        }
        return FascicolableBaseModel;
    }());
    return FascicolableBaseModel;
});
//# sourceMappingURL=FascicolableBaseModel.js.map