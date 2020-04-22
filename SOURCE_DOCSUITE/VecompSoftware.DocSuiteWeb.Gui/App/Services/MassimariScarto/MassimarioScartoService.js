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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/MassimariScarto/MassimarioScartoModelMapper"], function (require, exports, BaseService, MassimarioScartoModelMapper) {
    var MassimarioScartoService = /** @class */ (function (_super) {
        __extends(MassimarioScartoService, _super);
        /**
         * Costruttore
         */
        function MassimarioScartoService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        MassimarioScartoService.prototype.getMassimariByParent = function (includeCancel, parentId, callback, error) {
            var url = this._configuration.ODATAUrl.concat("/MassimariScartoService.GetAllChildren(parentId=", parentId, ",includeLogicalDelete=", String(includeCancel), ")");
            this.getRequest(url, null, function (response) {
                if (callback) {
                    var instances = new Array();
                    var mapper = new MassimarioScartoModelMapper();
                    instances = mapper.MapCollection(response.value);
                    callback(instances);
                }
            }, error);
        };
        MassimarioScartoService.prototype.getMassimarioById = function (massimarioId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var qs = "$filter=UniqueId eq ".concat(massimarioId);
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    var mapper = new MassimarioScartoModelMapper();
                    callback(mapper.Map(response.value[0]));
                }
            }, error);
        };
        MassimarioScartoService.prototype.findMassimari = function (name, includeCancel, code, callback, error) {
            var codeQs = code != undefined ? ",fullCode='".concat(code, "'") : "";
            var url = this._configuration.ODATAUrl.concat("/MassimariScartoService.GetMassimari(name='", name, "'", codeQs, ",includeLogicalDelete=", String(includeCancel), ")");
            this.getRequest(url, null, function (response) {
                if (callback) {
                    var instances = new Array();
                    var mapper = new MassimarioScartoModelMapper();
                    instances = mapper.MapCollection(response.value);
                    callback(instances);
                }
            }, error);
        };
        MassimarioScartoService.prototype.insertMassimario = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.postRequest(url, JSON.stringify(model), callback, error);
        };
        MassimarioScartoService.prototype.updateMassimario = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.putRequest(url, JSON.stringify(model), callback, error);
        };
        return MassimarioScartoService;
    }(BaseService));
    return MassimarioScartoService;
});
//# sourceMappingURL=MassimarioScartoService.js.map