var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "App/Services/BaseService", "App/Mappers/Commons/LocationViewModelMapper"], function (require, exports, BaseService, LocationViewModelMapper) {
    var LocationService = /** @class */ (function (_super) {
        __extends(LocationService, _super);
        function LocationService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        LocationService.prototype.getLocations = function (callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$orderby=Name";
            this.getRequest(url, data, function (response) {
                if (callback && response) {
                    var viewModelMapper_1 = new LocationViewModelMapper();
                    var locations_1 = [];
                    $.each(response.value, function (i, value) {
                        locations_1.push(viewModelMapper_1.Map(value));
                    });
                    callback(locations_1);
                }
                ;
            }, error);
        };
        LocationService.prototype.getLocationDetailsByUniqueId = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$filter=UniqueId eq " + uniqueId.toString();
            this.getRequest(url, data, function (response) {
                if (callback)
                    callback(response.value[0]);
            }, error);
        };
        LocationService.prototype.filterLocationByNameAndArchive = function (name, archive, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$orderby=Name&$filter=";
            if (name != "" && archive != "") {
                data = data + " contains(Name,'" + name + "') or contains(ProtocolArchive, '" + archive + "') or contains(DossierArchive, '" + archive + "') or contains(ResolutionArchive, '" + archive + "')";
            }
            else if (name != "") {
                data = data + " contains(Name,'" + name + "')";
            }
            else if (archive != "") {
                data = data + " contains(ProtocolArchive, '" + archive + "') or contains(DossierArchive, '" + archive + "') or contains(ResolutionArchive, '" + archive + "')";
            }
            else {
                data = data + " contains(Name,'') or contains(ProtocolArchive, '') or contains(DossierArchive, '') or contains(ResolutionArchive, '')";
            }
            this.getRequest(url, data, function (response) {
                if (callback && response) {
                    var viewModelMapper_2 = new LocationViewModelMapper();
                    var locations_2 = [];
                    $.each(response.value, function (i, value) {
                        locations_2.push(viewModelMapper_2.Map(value));
                    });
                    callback(locations_2);
                }
                ;
            }, error);
        };
        LocationService.prototype.insert = function (location, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.postRequest(url, JSON.stringify(location), callback, error);
        };
        LocationService.prototype.update = function (location, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.putRequest(url, JSON.stringify(location), callback, error);
        };
        return LocationService;
    }(BaseService));
    return LocationService;
});
//# sourceMappingURL=LocationService.js.map