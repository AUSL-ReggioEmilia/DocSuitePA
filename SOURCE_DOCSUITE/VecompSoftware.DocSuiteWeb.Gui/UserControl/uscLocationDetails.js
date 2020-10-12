define(["require", "exports", "App/Helpers/EnumHelper", "App/Helpers/ServiceConfigurationHelper", "App/Services/Commons/LocationService"], function (require, exports, EnumHelper, ServiceConfigurationHelper, LocationService) {
    var uscLocationDetails = /** @class */ (function () {
        function uscLocationDetails(serviceConfigurations) {
            this._serviceConfigurations = serviceConfigurations;
            this._enumHelper = new EnumHelper();
        }
        uscLocationDetails.prototype.initialize = function () {
            var locationConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, uscLocationDetails.LOCATION_SERVICE);
            this._locationService = new LocationService(locationConfiguration);
            $("#" + this.pnlDetailsId).hide();
            this._lblName = document.getElementById(this.lblNameId);
            this._lblArchiveProtocol = document.getElementById(this.lblArchiveProtocolId);
            this._lblArchiveDossier = document.getElementById(this.lblArchiveDossierId);
            this._lblArchiveAtti = document.getElementById(this.lblArchiveAttiId);
            this.bindLoaded();
        };
        uscLocationDetails.prototype.clearLocationDetails = function () {
            this._lblName.innerText = "";
            this._lblArchiveProtocol.innerText = "";
            this._lblArchiveDossier.innerText = "";
            this._lblArchiveAtti.innerText = "";
        };
        uscLocationDetails.prototype.setLocationDetails = function () {
            var _this = this;
            this._locationService.getLocationDetailsByUniqueId(uscLocationDetails.selectedLocationUniqueId, function (data) {
                _this.locationModel = data;
                _this._lblName.innerText = _this.locationModel.Name + " (" + _this.locationModel.EntityShortId + " - " + _this.locationModel.UniqueId + ")";
                _this._lblArchiveProtocol.innerText = _this.locationModel.ProtocolArchive;
                _this._lblArchiveDossier.innerText = _this.locationModel.DossierArchive;
                _this._lblArchiveAtti.innerText = _this.locationModel.ResolutionArchive;
            });
        };
        uscLocationDetails.prototype.bindLoaded = function () {
            $("#" + this.pnlDetailsId).data(this);
        };
        uscLocationDetails.LOCATION_SERVICE = "Location";
        return uscLocationDetails;
    }());
    return uscLocationDetails;
});
//# sourceMappingURL=uscLocationDetails.js.map