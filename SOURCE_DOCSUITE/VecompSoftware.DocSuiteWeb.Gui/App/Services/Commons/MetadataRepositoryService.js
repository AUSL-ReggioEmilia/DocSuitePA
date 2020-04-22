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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/Commons/MetadataRepositoryViewModelMapper", "App/Mappers/Commons/MetadataRepositoryModelMapper", "App/Models/ODATAResponseModel"], function (require, exports, BaseService, MetadataRepositoryViewModelMapper, MetadataRepositoryModelMapper, ODATAResponseModel) {
    var MetadataRepositoryService = /** @class */ (function (_super) {
        __extends(MetadataRepositoryService, _super);
        function MetadataRepositoryService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            _this._mapper = new MetadataRepositoryViewModelMapper();
            _this._modelMapper = new MetadataRepositoryModelMapper();
            return _this;
        }
        /**
         * Recupero tutti i MetadataRepository
         * @param callback
         * @param error
         */
        MetadataRepositoryService.prototype.findMetadataRepositories = function (filter, callback, error) {
            var _this = this;
            var url = this._configuration.ODATAUrl;
            var qs = "";
            qs = "$orderby=Name";
            if (filter && filter.length > 0) {
                qs = qs.concat("&$filter=contains(Name, '", filter, "')");
            }
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    var metadataRepositories = [];
                    if (response) {
                        metadataRepositories = _this._mapper.MapCollection(response.value);
                    }
                    callback(metadataRepositories);
                }
            }, error);
        };
        MetadataRepositoryService.prototype.getAvailableMetadataRepositories = function (filter, repositoryRestrictions, top, skip, callback, error) {
            var _this = this;
            var url = this._configuration.ODATAUrl;
            var qs = "$orderby=Name&$count=true&$top=".concat(top, "&$skip=", skip.toString(), "&$filter=Status eq VecompSoftware.DocSuiteWeb.Entity.Commons.MetadataRepositoryStatus'1'");
            if (!!filter) {
                qs = qs.concat(" and contains(Name, '", filter, "')");
            }
            if (repositoryRestrictions && repositoryRestrictions.length > 0) {
                qs = qs.concat(" and (UniqueId eq ", repositoryRestrictions.join(" or UniqueId eq "), ")");
            }
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    var responseModel = new ODATAResponseModel(response);
                    responseModel.value = _this._mapper.MapCollection(response.value);
                    callback(responseModel);
                }
            }, error);
        };
        MetadataRepositoryService.prototype.getById = function (id, callback, error) {
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
        MetadataRepositoryService.prototype.getNameById = function (id, callback, error) {
            var url = this._configuration.ODATAUrl;
            var qs = "$filter=UniqueId eq ".concat(id, "&$select=Name");
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    var result = (response.value && response.value.length > 0) ? response.value[0].Name : null;
                    callback(result);
                }
            }, error);
        };
        /**
         * Inserisco un nuovo metadataRepository
         * @param model
         * @param callback
         * @param error
         */
        MetadataRepositoryService.prototype.insertMetadataRepository = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.postRequest(url, JSON.stringify(model), callback, error);
        };
        MetadataRepositoryService.prototype.getFullModelById = function (id, callback, error) {
            var _this = this;
            var url = this._configuration.ODATAUrl;
            var qs = "$filter=UniqueId eq ".concat(id);
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    var result = _this._modelMapper.Map(response.value[0]);
                    callback(result);
                }
            }, error);
        };
        /**
         * Aggiorno un MetadataRepositoryEsistente
         * @param model
         * @param callback
         * @param error
         */
        MetadataRepositoryService.prototype.updateMetadataRepository = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.putRequest(url, JSON.stringify(model), callback, error);
        };
        return MetadataRepositoryService;
    }(BaseService));
    return MetadataRepositoryService;
});
//# sourceMappingURL=MetadataRepositoryService.js.map