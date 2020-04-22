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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/Dossiers/DossierSummaryFolderViewModelMapper", "../../Mappers/Processes/ProcessFascicleTemplateModelMapper", "../../Mappers/Dossiers/DossierFolderModelMapper"], function (require, exports, BaseService, DossierSummaryFolderViewModelMapper, ProcessFascicleTemplateModelMapper, DossierFolderModelMapper) {
    var DossierFolderService = /** @class */ (function (_super) {
        __extends(DossierFolderService, _super);
        /**
        * Costruttore
        */
        function DossierFolderService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        DossierFolderService.prototype.getChildren = function (uniqueId, status, callback, error) {
            var url = this._configuration.ODATAUrl.concat("/DossierFolderService.GetChildrenByParent(idDossierFolder=", uniqueId, ",status=", status.toString(), ")");
            var data = "$orderby=Name asc";
            this.getRequest(url, data, function (response) {
                if (callback) {
                    var mapper = new DossierSummaryFolderViewModelMapper();
                    var dossierFolders = [];
                    if (response) {
                        dossierFolders = mapper.MapCollection(response.value);
                        callback(dossierFolders);
                    }
                }
            }, error);
        };
        /**
        * Inserisco una nuova DossierFolder
        */
        DossierFolderService.prototype.insertDossierFolder = function (dossierFolder, insertAction, callback, error) {
            var url = this._configuration.WebAPIUrl;
            if (insertAction) {
                url = url.concat("?actionType=", insertAction.toString());
            }
            this.postRequest(url, JSON.stringify(dossierFolder), callback, error);
        };
        /**
        * Cancellazione di una DossierFolder esistente
        * @param model
        * @param callback
        * @param error
        */
        DossierFolderService.prototype.deleteDossierFolder = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.deleteRequest(url, JSON.stringify(model), callback, error);
        };
        /**
        * Aggiorno una DossierFolder
        */
        DossierFolderService.prototype.updateDossierFolder = function (dossierFolder, updateAction, callback, error) {
            var url = this._configuration.WebAPIUrl;
            if (updateAction) {
                url = url.concat("?actionType=", updateAction.toString());
            }
            this.putRequest(url, JSON.stringify(dossierFolder), callback, error);
        };
        /*
        *
        */
        DossierFolderService.prototype.getDossierFolder = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$filter=UniqueId eq ".concat(uniqueId, "&$expand=Category,Fascicle,DossierFolderRoles($expand=Role)");
            this.getRequest(url, data, function (response) {
                if (callback) {
                    var instance = new DossierSummaryFolderViewModelMapper();
                    if (response) {
                        callback(instance.Map(response.value[0]));
                    }
                }
            }, error);
        };
        DossierFolderService.prototype.getFullDossierFolder = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$filter=UniqueId eq ".concat(uniqueId, "&$expand=Category,Dossier($expand=MetadataRepository),DossierFolderRoles($expand=Role)");
            this.getRequest(url, data, function (response) {
                if (callback && response) {
                    callback(response.value[0]);
                }
            }, error);
        };
        DossierFolderService.prototype.getFascicleTemplatesByDossierFolderId = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$expand=FascicleTemplates&$filter=UniqueId eq " + uniqueId + "&$select=FascicleTemplates";
            this.getRequest(url, data, function (response) {
                if (callback && response) {
                    var mapper = new ProcessFascicleTemplateModelMapper();
                    var results = [];
                    for (var _i = 0, _a = response.value[0].FascicleTemplates; _i < _a.length; _i++) {
                        var item = _a[_i];
                        results.push(mapper.Map(item));
                    }
                    callback(results);
                }
            }, error);
        };
        DossierFolderService.prototype.getProcessFolders = function (name, idProcess, loadOnlyActive, loadOnlyMy, callback, error) {
            if (name) {
                name = "'" + name + "'";
            }
            var url = this._configuration.ODATAUrl + "/DossierFolderService.GetProcessFolders(name=" + name + ",idProcess=" + idProcess + ",loadOnlyActive=" + loadOnlyActive + ",loadOnlyMy=" + loadOnlyMy + ")";
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    callback(response.value);
                }
            }, error);
        };
        DossierFolderService.prototype.getDossierFolderById = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl + "?$filter=UniqueId eq " + uniqueId + "&$expand=DossierFolderRoles($expand=Role)";
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var mapper = new DossierFolderModelMapper();
                    callback(mapper.MapCollection(response.value));
                }
            }, error);
        };
        return DossierFolderService;
    }(BaseService));
    return DossierFolderService;
});
//# sourceMappingURL=DossierFolderService.js.map