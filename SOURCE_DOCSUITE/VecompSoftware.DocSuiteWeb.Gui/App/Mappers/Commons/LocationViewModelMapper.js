define(["require", "exports"], function (require, exports) {
    var LocationViewModelMapper = /** @class */ (function () {
        function LocationViewModelMapper() {
        }
        LocationViewModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.EntityShortId = source.EntityShortId;
            toMap.Name = source.Name;
            toMap.DocumentServer = source.DocumentServer;
            toMap.ProtocolArchive = source.ProtocolArchive;
            toMap.DossierArchive = source.DossierArchive;
            toMap.ResolutionArchive = source.ResolutionArchive;
            toMap.ConservationArchive = source.ConservationArchive;
            toMap.ConservationServer = source.ConservationServer;
            toMap.UniqueId = source.UniqueId;
            toMap.RegistrationUser = source.RegistrationUser;
            toMap.RegistrationDate = source.RegistrationDate;
            toMap.LastChangetUser = source.LastChangetUser;
            toMap.LastChangedDate = source.LastChangedDate;
            return toMap;
        };
        return LocationViewModelMapper;
    }());
    return LocationViewModelMapper;
});
//# sourceMappingURL=LocationViewModelMapper.js.map