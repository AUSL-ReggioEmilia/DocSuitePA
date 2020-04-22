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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/Commons/PrivacyLevelModelMapper"], function (require, exports, BaseService, PrivacyLevelModelMapper) {
    var PrivacyLevelService = /** @class */ (function (_super) {
        __extends(PrivacyLevelService, _super);
        function PrivacyLevelService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            _this._mapper = new PrivacyLevelModelMapper();
            return _this;
        }
        /**
         * Recupero tutti i privacyLevel
         * @param callback
         * @param error
         */
        PrivacyLevelService.prototype.findPrivacyLevels = function (filter, callback, error) {
            var _this = this;
            var url = this._configuration.ODATAUrl;
            var qs = "";
            if (filter && filter.length > 0) {
                qs = "$filter=contains(Description,'".concat(filter, "')&");
            }
            qs = qs.concat("$orderby=Level asc");
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    var privacyLevels = [];
                    if (response) {
                        privacyLevels = _this._mapper.MapCollection(response.value);
                    }
                    callback(privacyLevels);
                }
            }, error);
        };
        PrivacyLevelService.prototype.getById = function (id, callback, error) {
            var _this = this;
            var url = this._configuration.ODATAUrl;
            var qs = "$filter=UniqueId eq ".concat(id);
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    var result = _this._mapper.Map(response.value[0]);
                    callback(result);
                }
            }, error);
        };
        /**
         * Inserisco un nuovo privacyLevel
         * @param model
         * @param callback
         * @param error
         */
        PrivacyLevelService.prototype.insertPrivacyLevel = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.postRequest(url, JSON.stringify(model), callback, error);
        };
        /**
         * Aggiorno un privacyLevel
         * @param model
         * @param callback
         * @param error
         */
        PrivacyLevelService.prototype.updatePrivacyLevel = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.putRequest(url, JSON.stringify(model), callback, error);
        };
        return PrivacyLevelService;
    }(BaseService));
    return PrivacyLevelService;
});
//# sourceMappingURL=PrivacyLevelService.js.map