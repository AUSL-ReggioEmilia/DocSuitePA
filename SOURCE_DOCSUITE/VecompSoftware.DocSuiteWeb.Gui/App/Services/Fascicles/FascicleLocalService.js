define(["require", "exports", "App/Helpers/GuidHelper"], function (require, exports, Guid) {
    var FascicleLocalService = /** @class */ (function () {
        function FascicleLocalService() {
        }
        FascicleLocalService.prototype.insertFascicle = function (model, callback, error) {
            model.UniqueId = Guid.newGuid();
            if (callback) {
                callback(model);
            }
        };
        FascicleLocalService.prototype.updateFascicle = function (model, actiontype, callback, error) {
            throw new Error("Method not implemented.");
        };
        FascicleLocalService.prototype.closeFascicle = function (model, callback, error) {
            throw new Error("Method not implemented.");
        };
        FascicleLocalService.prototype.getFascicle = function (id, callback, error) {
            throw new Error("Method not implemented.");
        };
        FascicleLocalService.prototype.getAvailableFascicles = function (uniqueId, name, topElement, skipElement, callback, error) {
            throw new Error("Method not implemented.");
        };
        FascicleLocalService.prototype.getAssociatedFascicles = function (uniqueId, environment, qs, callback, error) {
            throw new Error("Method not implemented.");
        };
        FascicleLocalService.prototype.getNotLinkedFascicles = function (model, name, topElement, skipElement, callback, error) {
            throw new Error("Method not implemented.");
        };
        FascicleLocalService.prototype.getLinkedFascicles = function (model, qs, callback, error) {
            throw new Error("Method not implemented.");
        };
        FascicleLocalService.prototype.getFascicleByCategory = function (idCategory, name, callback, error) {
            throw new Error("Method not implemented.");
        };
        return FascicleLocalService;
    }());
    return FascicleLocalService;
});
//# sourceMappingURL=FascicleLocalService.js.map