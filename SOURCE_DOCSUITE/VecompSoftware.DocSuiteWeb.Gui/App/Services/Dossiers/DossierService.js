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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/Dossiers/DossierSummaryViewModelMapper", "App/Mappers/Dossiers/DossierSummaryContactViewModelMapper", "App/Mappers/Dossiers/DossierSummaryRoleViewModelMapper", "App/Mappers/Dossiers/DossierGridViewModelMapper"], function (require, exports, BaseService, DossierSummaryViewModelMapper, DossierSummaryContactViewModelMapper, DossierSummaryRoleViewModelMapper, DossierGridViewModelMapper) {
    var DossierService = /** @class */ (function (_super) {
        __extends(DossierService, _super);
        /**
         * Costruttore
         */
        function DossierService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        DossierService.prototype.getAllDossiers = function (callback, error) {
            var url = this._configuration.ODATAUrl;
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var viewModelMapper_1 = new DossierSummaryViewModelMapper();
                    var dossiers_1 = [];
                    $.each(response.value, function (i, value) {
                        dossiers_1.push(viewModelMapper_1.Map(value));
                    });
                    callback(dossiers_1);
                }
                ;
            }, error);
        };
        DossierService.prototype.getDossiers = function (skip, top, searchFilter, callback, error) {
            var url = this._configuration.ODATAUrl.
                concat("/DossierService.GetAuthorizedDossiers(skip=", skip.toString(), ",top=", top.toString(), ",year=", !!searchFilter.year ? searchFilter.year.toString() : null, ",number=", !!searchFilter.number ? searchFilter.number.toString() : null, ",subject=\'", searchFilter.subject, "\',note=\'", searchFilter.note, "\',idContainer=", !!searchFilter.idContainer ? searchFilter.idContainer.toString() : null, ",startDateFrom=\'", searchFilter.startDateFrom, "\',startDateTo=\'", searchFilter.startDateTo, "\',endDateFrom=\'", searchFilter.endDateFrom, "\',endDateTo=\'", searchFilter.endDateTo, "\',idMetadataRepository=", searchFilter.idMetadataRepository ? searchFilter.idMetadataRepository.toString() : null, "\,metadataValue=\'", searchFilter.metadataValue, "\')");
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var viewModelMapper_2 = new DossierGridViewModelMapper();
                    var dossiers_2 = [];
                    $.each(response.value, function (i, value) {
                        dossiers_2.push(viewModelMapper_2.Map(value));
                    });
                    callback(dossiers_2);
                }
                ;
            }, error);
        };
        DossierService.prototype.countDossiers = function (searchFilter, callback, error) {
            var url = this._configuration.ODATAUrl.
                concat("/DossierService.CountAuthorizedDossiers(year=", !!searchFilter.year ? searchFilter.year.toString() : null, ",number=", !!searchFilter.number ? searchFilter.number.toString() : null, ",subject=\'", searchFilter.subject, "\',idContainer=", !!searchFilter.idContainer ? searchFilter.idContainer.toString() : null, ",idMetadataRepository=", !!searchFilter.idMetadataRepository ? searchFilter.idMetadataRepository.toString() : null, ",metadataValue='", searchFilter.metadataValue, "')");
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    callback(response.value);
                }
                ;
            }, error);
        };
        /**
        * Recupero un DossierModel per ID
        * @param id
        * @param callback
        * @param error
        */
        DossierService.prototype.getDossier = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl.concat("/DossierService.GetCompleteDossier(uniqueId=", uniqueId, ")");
            var data = "";
            this.getRequest(url, data, function (response) {
                if (callback && response) {
                    var instance = new DossierSummaryViewModelMapper();
                    callback(instance.Map(response.value[0]));
                }
            }, error);
        };
        DossierService.prototype.getDossierContacts = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl.concat("/DossierService.GetDossierContacts(uniqueId=", uniqueId, ")");
            var data = "";
            this.getRequest(url, data, function (response) {
                if (callback && response) {
                    var instance_1 = new DossierSummaryContactViewModelMapper();
                    var dossiercontacts_1 = [];
                    $.each(response.value, function (i, value) {
                        dossiercontacts_1.push(instance_1.Map(value));
                    });
                    callback(dossiercontacts_1);
                }
            }, error);
        };
        DossierService.prototype.getDossierRoles = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl.concat("/DossierService.GetDossierRoles(idDossier=", uniqueId, ")");
            var data = "";
            this.getRequest(url, data, function (response) {
                if (callback && response) {
                    var instance = new DossierSummaryRoleViewModelMapper();
                    callback(instance.MapCollection(response.value));
                }
            }, error);
        };
        DossierService.prototype.isViewableDossier = function (idDossier, callback, error) {
            var url = this._configuration.ODATAUrl.concat("/DossierService.IsViewableDossier(idDossier=", idDossier, ")");
            var data = "";
            this.getRequest(url, data, function (response) {
                if (callback && response) {
                    callback(response.value);
                }
            }, error);
        };
        DossierService.prototype.isManageableDossier = function (idDossier, callback, error) {
            var url = this._configuration.ODATAUrl.concat("/DossierService.IsManageableDossier(idDossier=", idDossier, ")");
            var data = "";
            this.getRequest(url, data, function (response) {
                if (callback && response) {
                    callback(response.value);
                }
            }, error);
        };
        DossierService.prototype.hasRootNode = function (idDossier, callback, error) {
            var url = this._configuration.ODATAUrl.concat("/DossierService.HasRootNode(idDossier=", idDossier, ")");
            var data = "";
            this.getRequest(url, data, function (response) {
                if (callback && response) {
                    callback(response.value);
                }
            }, error);
        };
        /**
     * Inserisce un nuovo Dossier
     * @param model
     * @param callback
     * @param error
     */
        DossierService.prototype.insertDossier = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.postRequest(url, JSON.stringify(model), callback, error);
        };
        /**
    * Controlla diritti inserimento dossiers
    */
        DossierService.prototype.hasInsertRight = function (callback, error) {
            var url = this._configuration.ODATAUrl.concat("/DossierService.HasInsertRight()");
            var data = "";
            this.getRequest(url, data, function (response) {
                if (callback && response) {
                    callback(response.value);
                }
            }, error);
        };
        /**
    * Modifica un Dossier esistente
    * @param model
    * @param callback
    * @param error
    */
        DossierService.prototype.updateDossier = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.putRequest(url, JSON.stringify(model), callback, error);
        };
        /**
    * Controlla diritti modifica del dossier
    */
        DossierService.prototype.hasModifyRight = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl.concat("/DossierService.HasModifyRight(idDossier=", uniqueId, ")");
            var data = "";
            this.getRequest(url, data, function (response) {
                if (callback && response) {
                    callback(response.value);
                }
            }, error);
        };
        DossierService.prototype.countDossiersById = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "/$count?$filter=DossierFolders/any(d: d/Fascicle/Uniqueid eq " + uniqueId + ")";
            url = url.concat(data);
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response);
                }
            }, error);
        };
        return DossierService;
    }(BaseService));
    return DossierService;
});
//# sourceMappingURL=DossierService.js.map