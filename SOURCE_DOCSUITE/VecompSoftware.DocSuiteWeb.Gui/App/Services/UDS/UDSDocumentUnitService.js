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
define(["require", "exports", "../BaseService", "App/Mappers/UDS/UDSDocumentUnitModelMapper"], function (require, exports, BaseService, UDSDocumentUnitModelMapper) {
    var UDSDocumentUnitService = /** @class */ (function (_super) {
        __extends(UDSDocumentUnitService, _super);
        /**
         * Costruttore
         */
        function UDSDocumentUnitService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            _this._mapper = new UDSDocumentUnitModelMapper();
            return _this;
        }
        UDSDocumentUnitService.prototype.getUDSByProtocolId = function (Id, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$expand=Relation($expand=UDSRepository)&$filter=Relation/Uniqueid eq " + Id + " and Relation/Environment eq 1";
            this.getRequest(url, data, function (response) {
                if (callback) {
                    callback(response.value);
                }
            }, error);
        };
        UDSDocumentUnitService.prototype.getProtocolListById = function (IdUDS, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$expand=Relation&$filter=Relation/Environment eq 1 and IdUDS eq ".concat(IdUDS);
            this.getRequest(url, data, function (response) {
                if (callback) {
                    callback(response.value);
                }
            }, error);
        };
        UDSDocumentUnitService.prototype.getUDSListById = function (IdUDS, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$expand=Relation($expand=UDSRepository($select=UniqueId))&$filter=Relation/Environment ge 100 and IdUDS eq ".concat(IdUDS);
            this.getRequest(url, data, function (response) {
                if (callback) {
                    callback(response.value);
                }
            }, error);
        };
        UDSDocumentUnitService.prototype.countProtocolsById = function (IdUDS, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "/$count?$filter=Relation/Environment eq 1 and IdUDS eq ".concat(IdUDS);
            url = url.concat(data);
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response);
                }
            }, error);
        };
        UDSDocumentUnitService.prototype.countUDSByRelationId = function (documentUnitId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "/$count?$filter=Relation/Environment eq 1 and Relation/UniqueId eq ".concat(documentUnitId);
            url = url.concat(data);
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response);
                }
            }, error);
        };
        UDSDocumentUnitService.prototype.countUDSById = function (IdUDS, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "/$count?$filter=Relation/Environment ge 100 and IdUDS eq ".concat(IdUDS);
            url = url.concat(data);
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response);
                }
            }, error);
        };
        UDSDocumentUnitService.prototype.countUDSId = function (IdUDS, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "/$count?$filter=IdUDS eq ".concat(IdUDS);
            url = url.concat(data);
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response);
                }
            }, error);
        };
        return UDSDocumentUnitService;
    }(BaseService));
    return UDSDocumentUnitService;
});
//# sourceMappingURL=UDSDocumentUnitService.js.map