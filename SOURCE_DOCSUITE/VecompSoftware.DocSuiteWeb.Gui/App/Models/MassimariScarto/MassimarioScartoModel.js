define(["require", "exports"], function (require, exports) {
    var MassimarioScartoModel = /** @class */ (function () {
        function MassimarioScartoModel() {
        }
        MassimarioScartoModel.prototype.isActive = function () {
            return this.EndDate == undefined || moment(this.EndDate).isAfter(moment().toDate());
        };
        MassimarioScartoModel.prototype.getPeriodLabel = function () {
            var label = '';
            switch (this.ConservationPeriod) {
                case -1:
                    label = 'Illimitato';
                    break;
                default:
                    label = this.ConservationPeriod.toString().concat(' Ann', this.ConservationPeriod == 1 ? 'o' : 'i');
                    break;
            }
            return label;
        };
        return MassimarioScartoModel;
    }());
    return MassimarioScartoModel;
});
//# sourceMappingURL=MassimarioScartoModel.js.map