/// <amd-dependency path="../../core/extensions/string" />
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
define(["require", "exports", "App/Models/ODATAResponseModel", "App/Services/BaseService", "App/Models/UpdateActionType", "App/Models/DeleteActionType", "App/Models/InsertActionType", "App/Models/Fascicles/FascicleType", "App/Mappers/Fascicles/FascicleModelMapper", "../../core/extensions/string"], function (require, exports, ODATAResponseModel, BaseService, UpdateActionType, DeleteActionType, InsertActionType, FascicleType, FascicleModelMapper) {
    var FascicleService = /** @class */ (function (_super) {
        __extends(FascicleService, _super);
        /**
         * Costruttore
         */
        function FascicleService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        /**
         * Inserisce un nuovo Fascicolo
         * @param model
         * @param callback
         * @param error
         */
        FascicleService.prototype.insertFascicle = function (model, actionType, callback, error) {
            var url = this._configuration.WebAPIUrl;
            if (!actionType) {
                switch (model.FascicleType) {
                    case FascicleType.Activity:
                        url = url.concat("?actionType=", InsertActionType.InsertActivityFascicle.toString());
                        break;
                    case FascicleType.Period:
                        url = url.concat("?actionType=", InsertActionType.InsertPeriodicFascicle.toString());
                        break;
                    case FascicleType.Procedure:
                        break;
                }
            }
            else {
                url = url.concat("?actionType=", actionType.toString());
            }
            this.postRequest(url, JSON.stringify(model), callback, error);
        };
        /**
         * Modifica un Fascicolo esistente
         * @param model
         * @param callback
         * @param error
         */
        FascicleService.prototype.updateFascicle = function (model, actionType, callback, error) {
            var url = this._configuration.WebAPIUrl;
            if (model.FascicleType == FascicleType.Activity && actionType !== UpdateActionType.ChangeFascicleType) {
                actionType = UpdateActionType.ActivityFascicleUpdate;
            }
            if (actionType) {
                url = url.concat("?actionType=", actionType.toString());
            }
            this.putRequest(url, JSON.stringify(model), callback, error);
        };
        ///**
        //* Cancella un Fascicolo esistente
        //* @param model
        //* @param callback
        //* @param error
        //*/
        FascicleService.prototype.deleteFascicle = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl.concat("?actionType=", DeleteActionType.CancelFascicle.toString());
            this.deleteRequest(url, JSON.stringify(model), callback, error);
        };
        /**
         * Chiude un Fascicolo
         * @param model
         * @param callback
         * @param error
         */
        FascicleService.prototype.closeFascicle = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            if (model.FascicleType.toString() != FascicleType.Activity.toString()) {
                url = url.concat("?actionType=", UpdateActionType.FascicleClose.toString());
            }
            else {
                url = url.concat("?actionType=", UpdateActionType.ActivityFascicleClose.toString());
            }
            this.putRequest(url, JSON.stringify(model), callback, error);
        };
        /**
         * Recupera un Fascicolo per ID
         * @param id
         * @param callback
         * @param error
         */
        FascicleService.prototype.getFascicle = function (id, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$filter=UniqueId eq " + id + "&$expand=Category($expand=CategoryFascicles($expand=CategoryFascicleRights)),Container,Contacts,FascicleDocumentUnits,FascicleDocuments,MetadataRepository,DossierFolders,FascicleRoles($expand=Role($expand=Father))";
            this.getRequest(url, data, function (response) {
                if (callback) {
                    var mapper = new FascicleModelMapper();
                    callback(mapper.Map(response.value[0]));
                }
            }, error);
        };
        /**
         * Recupera i Fascicoli disponibili per l'associazione di una Document Unit
         * TODO: da implementare in SignalR
         * @param uniqueId
         * @param environment
         * @param qs
         * @param callback
         * @param error
         */
        FascicleService.prototype.getAvailableFascicles = function (uniqueId, name, topElement, skipElement, callback, error) {
            var url = this._configuration.ODATAUrl.concat("/FascicleService.AvailableFascicles(uniqueId=", uniqueId, ")", "?$orderby=Title,FascicleObject");
            var qs = "$count=true&$top=".concat(topElement, "&$skip=", skipElement.toString());
            if (!String.isNullOrEmpty(name)) {
                qs = qs.concat("&$filter=contains(tolower(concat(concat(Title, ' '),FascicleObject)),'", name.toLowerCase().replace(/'/g, "''"), "')");
            }
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    var responseModel = new ODATAResponseModel(response);
                    var instances = new Array();
                    var mapper = new FascicleModelMapper();
                    instances = mapper.MapCollection(response.value);
                    responseModel.value = instances;
                    callback(responseModel);
                }
            }, error);
        };
        /**
         * TODO: da implementare in SignalR
         * @param uniqueId
         * @param environment
         * @param qs
         * @param callback
         * @param error
         */
        FascicleService.prototype.getAssociatedFascicles = function (uniqueId, environment, qs, callback, error) {
            var url = this._configuration.ODATAUrl.concat("/FascicleService.DocumentUnitAssociated(uniqueId=", uniqueId, ")");
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    var instances = new Array();
                    var mapper = new FascicleModelMapper();
                    instances = mapper.MapCollection(response.value);
                    callback(instances);
                }
            }, error);
        };
        /**
         * TODO: da implementare in SignalR
         * @param model
         * @param qs
         * @param callback
         * @param error
         */
        FascicleService.prototype.getNotLinkedFascicles = function (model, name, topElement, skipElement, callback, error) {
            var url = this._configuration.ODATAUrl.concat("/FascicleService.NotLinkedFascicles(idFascicle=" + model.Fascicle.UniqueId + " ,idCategory=" + model.SelectedCategoryId + ")");
            var qs = "$count=true&$top=".concat(topElement, "&$skip=", skipElement.toString());
            if (!String.isNullOrEmpty(name)) {
                qs = qs.concat("&$filter=contains(tolower(concat(concat(Title, ' '),FascicleObject)),'", name.toLowerCase(), "')");
            }
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    var responseModel = new ODATAResponseModel(response);
                    var instances = new Array();
                    var mapper = new FascicleModelMapper();
                    instances = mapper.MapCollection(response.value);
                    responseModel.value = instances;
                    callback(responseModel);
                }
            }, error);
        };
        FascicleService.prototype.getFascicleByCategorySkipTop = function (idCategory, name, topElement, skipElement, callback, error) {
            var url = this._configuration.ODATAUrl.concat("/FascicleService.GetFasciclesByCategory(idCategory=", idCategory.toString(), ",name='", name, "')");
            var qs = "$count=true&$top=".concat(topElement, "&$skip=", skipElement.toString());
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    var responseModel = new ODATAResponseModel(response);
                    var instances = new Array();
                    var mapper = new FascicleModelMapper();
                    instances = mapper.MapCollection(response.value);
                    responseModel.value = instances;
                    callback(responseModel);
                }
            });
        };
        /**
         * TODO: da implementare in SignalR
         * @param model
         * @param qs
         * @param callback
         * @param error
         */
        FascicleService.prototype.getLinkedFascicles = function (model, qs, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$filter=UniqueId eq ".concat(model.UniqueId, '&$expand=FascicleLinks($expand=FascicleLinked($expand=Category))');
            this.getRequest(url, data, function (response) {
                if (callback) {
                    var mapper = new FascicleModelMapper();
                    callback(mapper.Map(response.value[0]));
                }
            }, error);
        };
        FascicleService.prototype.getFascicleByCategory = function (idCategory, name, hasProcess, callback, error) {
            var url = this._configuration.ODATAUrl + "/FascicleService.GetFasciclesByCategory(idCategory=" + idCategory + ", name='" + name + "', hasProcess=" + hasProcess + ")";
            this.getRequest(url, null, function (response) {
                if (callback) {
                    var responseModel = new ODATAResponseModel(response);
                    var instances = new Array();
                    var mapper = new FascicleModelMapper();
                    instances = mapper.MapCollection(response.value);
                    responseModel.value = instances;
                    callback(responseModel);
                }
            });
        };
        FascicleService.prototype.hasInsertRight = function (fascicleType, callback, error) {
            var url = this._configuration.ODATAUrl.concat("/FascicleService.HasInsertRight(fascicleType=VecompSoftware.DocSuiteWeb.Entity.Fascicles.FascicleType'" + FascicleType[fascicleType] + "')");
            this.getRequest(url, null, function (response) {
                if (callback) {
                    if (!response) {
                        callback(undefined);
                        return;
                    }
                    callback(response.value);
                }
            }, error);
        };
        FascicleService.prototype.hasManageableRight = function (idFascicle, callback, error) {
            var url = this._configuration.ODATAUrl.concat("/FascicleService.HasManageableRight(idFascicle=" + idFascicle + ")");
            this.getRequest(url, null, function (response) {
                if (callback) {
                    if (!response) {
                        callback(undefined);
                        return;
                    }
                    callback(response.value);
                }
            }, error);
        };
        FascicleService.prototype.hasViewableRight = function (idFascicle, callback, error) {
            var url = this._configuration.ODATAUrl.concat("/FascicleService.HasViewableRight(idFascicle=" + idFascicle + ")");
            this.getRequest(url, null, function (response) {
                if (callback) {
                    if (!response) {
                        callback(undefined);
                        return;
                    }
                    callback(response.value);
                }
            }, error);
        };
        FascicleService.prototype.isManager = function (idFascicle, callback, error) {
            var url = this._configuration.ODATAUrl.concat("/FascicleService.IsManager(idFascicle=" + idFascicle + ")");
            this.getRequest(url, null, function (response) {
                if (callback) {
                    if (!response) {
                        callback(undefined);
                        return;
                    }
                    callback(response.value);
                }
            }, error);
        };
        FascicleService.prototype.hasFascicolatedDocumentUnits = function (idFascicle, callback, error) {
            var url = this._configuration.ODATAUrl.concat("/FascicleService.HasFascicolatedDocumentUnits(idFascicle=" + idFascicle + ")");
            this.getRequest(url, null, function (response) {
                if (callback) {
                    if (!response) {
                        callback(undefined);
                        return;
                    }
                    callback(response.value);
                }
            }, error);
        };
        FascicleService.prototype.getAuthorizedFasciclesFromDocumentUnit = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl.concat("/FascicleService.AuthorizedFasciclesFromDocumentUnit(uniqueIdDocumentUnit=", uniqueId, ")");
            this.getRequest(url, null, function (response) {
                if (callback) {
                    var instances = new Array();
                    var mapper = new FascicleModelMapper();
                    instances = mapper.MapCollection(response.value);
                    callback(instances);
                }
            }, error);
        };
        FascicleService.prototype.countAuthorizedFasciclesFromDocumentUnit = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl.concat("/FascicleService.CountAuthorizedFasciclesFromDocumentUnit(uniqueIdDocumentUnit=", uniqueId, ")");
            this.getRequest(url, null, function (response) {
                if (callback) {
                    if (!response) {
                        callback(undefined);
                        return;
                    }
                    callback(response.value);
                }
            }, error);
        };
        return FascicleService;
    }(BaseService));
    return FascicleService;
});
//# sourceMappingURL=FascicleService.js.map