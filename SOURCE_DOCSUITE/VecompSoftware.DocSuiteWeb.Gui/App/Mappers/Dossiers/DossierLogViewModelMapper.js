define(["require", "exports", "App/Helpers/EnumHelper"], function (require, exports, EnumHelper) {
    var DossierLogViewModelMapper = /** @class */ (function () {
        function DossierLogViewModelMapper() {
            this._enumHelper = new EnumHelper();
        }
        DossierLogViewModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.Computer = source.SystemComputer;
            toMap.Description = source.LogDescription;
            toMap.TypeDescription = this._enumHelper.getLogTypeDescription(source.LogType);
            toMap.LogDate = moment(source.RegistrationDate).format("L").concat(" ").concat(moment(source.RegistrationDate).format("LTS"));
            toMap.LogUser = source.RegistrationUser;
            return toMap;
        };
        return DossierLogViewModelMapper;
    }());
    return DossierLogViewModelMapper;
});
//# sourceMappingURL=DossierLogViewModelMapper.js.map