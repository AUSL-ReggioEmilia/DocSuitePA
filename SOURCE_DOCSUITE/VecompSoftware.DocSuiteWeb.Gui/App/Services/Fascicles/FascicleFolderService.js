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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/Fascicles/FascicleSummaryFolderViewModelMapper"], function (require, exports, BaseService, FascicleSummaryFolderViewModelMapper) {
    var FascicleFolderService = /** @class */ (function (_super) {
        __extends(FascicleFolderService, _super);
        /**
        * Costruttore
        */
        function FascicleFolderService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        FascicleFolderService.prototype.getById = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$filter=UniqueId eq " + uniqueId;
            this.getRequest(url, data, function (response) {
                if (callback) {
                    callback(response.value[0]);
                }
            }, error);
        };
        FascicleFolderService.prototype.getDefaultFascicleFolder = function (idfascicle, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$filter=Fascicle/UniqueId eq " + idfascicle + " and Name eq 'Fascicolo' and FascicleFolderLevel eq 2";
            this.getRequest(url, data, function (response) {
                if (callback) {
                    callback(response.value[0]);
                }
            }, error);
        };
        FascicleFolderService.prototype.getChildren = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl.concat("/FascicleFolderService.GetChildrenByParent(idFascicleFolder=", uniqueId, ")");
            var data = "$orderby=Name asc";
            this.getRequest(url, data, function (response) {
                if (callback) {
                    var mapper = new FascicleSummaryFolderViewModelMapper();
                    var dossierFolders = [];
                    if (response) {
                        dossierFolders = mapper.MapCollection(response.value);
                        callback(dossierFolders);
                    }
                }
            }, error);
        };
        FascicleFolderService.prototype.getByCategoryAndFascicle = function (idFascicle, idCategory, callback, error) {
            var url = this._configuration.ODATAUrl.concat("/FascicleFolderService.GetByCategoryAndFascicle(idFascicle=", idFascicle, ",idCategory=", idCategory.toString(), ")");
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response.value[0]);
                }
            }, error);
        };
        /**
    * Cancellazione di una FascicleFolder esistente
    * @param model
    * @param callback
    * @param error
    */
        FascicleFolderService.prototype.deleteFascicleFolder = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.deleteRequest(url, JSON.stringify(model), callback, error);
        };
        /**
    * Inserisco una nuova FascicleFolder
    */
        FascicleFolderService.prototype.insertFascicleFolder = function (fascicleFolder, insertAction, callback, error) {
            var url = this._configuration.WebAPIUrl;
            if (insertAction) {
                url = url.concat("?actionType=", insertAction.toString());
            }
            this.postRequest(url, JSON.stringify(fascicleFolder), callback, error);
        };
        /**
        * Aggiorno una FascicleFolder
        */
        FascicleFolderService.prototype.updateFascicleFolder = function (fascicleFolder, updateAction, callback, error) {
            var url = this._configuration.WebAPIUrl;
            if (updateAction) {
                url = url.concat("?actionType=", updateAction.toString());
            }
            this.putRequest(url, JSON.stringify(fascicleFolder), callback, error);
        };
        return FascicleFolderService;
    }(BaseService));
    return FascicleFolderService;
});
//# sourceMappingURL=FascicleFolderService.js.map