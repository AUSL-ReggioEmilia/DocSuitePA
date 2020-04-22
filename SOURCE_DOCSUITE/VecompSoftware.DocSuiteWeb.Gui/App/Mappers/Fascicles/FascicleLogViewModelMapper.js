define(["require", "exports", "App/Helpers/EnumHelper"], function (require, exports, EnumHelper) {
    var FascicleLogViewModelMapper = /** @class */ (function () {
        function FascicleLogViewModelMapper() {
            this._enumHelper = new EnumHelper();
        }
        FascicleLogViewModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.Computer = source.SystemComputer;
            toMap.Description = source.LogDescription;
            toMap.TypeDescription = this._enumHelper.getFascicleLogTypeDescription(source.LogType);
            toMap.LogDate = moment(source.RegistrationDate).format("L").concat(" ").concat(moment(source.RegistrationDate).format("LTS"));
            toMap.LogUser = source.RegistrationUser;
            return toMap;
        };
        return FascicleLogViewModelMapper;
    }());
    return FascicleLogViewModelMapper;
});
//# sourceMappingURL=FascicleLogViewModelMapper.js.map