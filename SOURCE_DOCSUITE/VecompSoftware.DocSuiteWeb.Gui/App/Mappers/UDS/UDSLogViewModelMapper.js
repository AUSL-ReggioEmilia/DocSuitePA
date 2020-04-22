define(["require", "exports", "App/Helpers/EnumHelper"], function (require, exports, EnumHelper) {
    var UDSLogViewModelMapper = /** @class */ (function () {
        function UDSLogViewModelMapper() {
            this._enumHelper = new EnumHelper();
        }
        UDSLogViewModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.Computer = source.SystemComputer;
            toMap.Description = source.LogDescription;
            toMap.TypeDescription = this._enumHelper.getUDSLogType(source.LogType);
            toMap.LogDate = moment(source.RegistrationDate).format("L").concat(" ").concat(moment(source.RegistrationDate).format("LTS"));
            toMap.LogUser = source.RegistrationUser;
            toMap.IdUDS = source.IdUDS;
            return toMap;
        };
        return UDSLogViewModelMapper;
    }());
    return UDSLogViewModelMapper;
});
//# sourceMappingURL=UDSLogViewModelMapper.js.map