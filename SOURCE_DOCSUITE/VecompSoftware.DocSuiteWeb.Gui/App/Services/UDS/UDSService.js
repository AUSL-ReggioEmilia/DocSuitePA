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
define(["require", "exports", "App/Services/BaseService", "App/Models/PageResultModel", "App/Mappers/UDS/UDSModelMapper", "App/Models/ODATAResponseModel"], function (require, exports, BaseService, PageResultModel, UDSModelMapper, ODATAResponseModel) {
    var UDSService = /** @class */ (function (_super) {
        __extends(UDSService, _super);
        /**
         * Costruttore
         * @param webApiUrl
         */
        function UDSService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        /**
         * Recupera una UDS per UniqueId
         * @param uniqueId
         * @param callback
         * @param error
         */
        UDSService.prototype.getUDSByUniqueId = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$filter=UDSId eq ".concat(uniqueId.toString());
            this.getRequest(url, data, function (response) {
                if (callback) {
                    var modelMapper = new UDSModelMapper();
                    callback(modelMapper.Map(response.Items.$values[0]));
                }
            }, error);
        };
        /**
         * Recupera i valori del campo lookup
         * @param uniqueId
         * @param callback
         * @param error
         */
        UDSService.prototype.getLookupValues = function (propertyName, filter, top, skip, callback, error) {
            var url = this._configuration.WebAPIUrl;
            var filterParameter = filter ? filter : '';
            var data = "isSearch=true&$filter=cast(".concat(propertyName, ",'Edm.String') ne '' and contains(cast(", propertyName, ",'Edm.String'),'", filterParameter, "') and _status eq 1 &$count=true&$top=", top, "&$skip=", skip.toString());
            this.getRequest(url, data, function (response) {
                if (callback && response) {
                    var pageResult = new PageResultModel(response);
                    var results = pageResult.items.map(function (item) { return item[propertyName].toString(); });
                    var udsLookupViewModel = {};
                    udsLookupViewModel.values = results.map(function (i) {
                        var date = moment(i);
                        if (date.isValid()) {
                            i = date.format("DD/MM/YYYY");
                        }
                        return i;
                    });
                    udsLookupViewModel.count = pageResult.count;
                    callback(udsLookupViewModel);
                }
            }, error);
        };
        UDSService.prototype.getUDSInvoices = function (searchFilter, top, skip, orderby, callback, error) {
            var urlPart = this._configuration.ODATAUrl;
            urlPart = urlPart.concat("?$count=true&$top=", top.toString(), "&$skip=", skip.toString());
            if (orderby && orderby != "") {
                urlPart = urlPart.concat("&$orderby=", orderby);
            }
            var oDataFilters = "_status eq 1";
            if (searchFilter.startDateFromFilter) {
                var propertyName = "DataFattura";
                oDataFilters = this.addToFilter(oDataFilters, propertyName.concat(' ge ', searchFilter.startDateFromFilter));
            }
            if (searchFilter.endDateFromFilter) {
                var propertyName = "DataFattura";
                oDataFilters = this.addToFilter(oDataFilters, propertyName.concat(' le ', searchFilter.endDateFromFilter));
            }
            if (searchFilter.dataIvaFromFilter) {
                var propertyName = "DataIva";
                oDataFilters = this.addToFilter(oDataFilters, propertyName.concat(' ge ', searchFilter.dataIvaFromFilter));
            }
            if (searchFilter.dataIvaToFilter) {
                var propertyName = "DataIva";
                oDataFilters = this.addToFilter(oDataFilters, propertyName.concat(' le ', searchFilter.dataIvaToFilter));
            }
            if (searchFilter.dataReceivedFromFilter) {
                var propertyName = "DataRicezioneSdi";
                oDataFilters = this.addToFilter(oDataFilters, propertyName.concat(' ge ', searchFilter.dataReceivedFromFilter));
            }
            if (searchFilter.dataReceivedToFilter) {
                var propertyName = "DataRicezioneSdi";
                oDataFilters = this.addToFilter(oDataFilters, propertyName.concat(' le ', searchFilter.dataReceivedToFilter));
            }
            if (searchFilter.dataacceptFromFilter) {
                var propertyName = "DataAccettazione";
                oDataFilters = this.addToFilter(oDataFilters, propertyName.concat(' ge ', searchFilter.dataacceptFromFilter));
            }
            if (searchFilter.dataacceptToFilter) {
                var propertyName = "DataAccettazione";
                oDataFilters = this.addToFilter(oDataFilters, propertyName.concat(' le ', searchFilter.dataacceptToFilter));
            }
            if (searchFilter.numerofatturafilter) {
                oDataFilters = this.addToFilter(oDataFilters, this.getFilterString("NumeroFattura", searchFilter.numerofatturafilter, searchFilter.numerofatturafilterEq, PropertyType.string));
            }
            if (searchFilter.importoFilter) {
                oDataFilters = this.addToFilter(oDataFilters, this.getFilterString("Importo", searchFilter.importoFilter, true, PropertyType.numeric));
            }
            if (searchFilter.pivacfFilter) {
                oDataFilters = this.addToFilter(oDataFilters, this.getFilterString("Pivacf", searchFilter.pivacfFilter, searchFilter.pivacfFilterEq, PropertyType.string));
            }
            if (searchFilter.denomiazioneFilter) {
                oDataFilters = this.addToFilter(oDataFilters, this.getFilterString("denominazione", searchFilter.denomiazioneFilter, searchFilter.denomiazioneFilterEq, PropertyType.string));
            }
            if (searchFilter.annoivaFilter) {
                oDataFilters = this.addToFilter(oDataFilters, this.getFilterString_Eq("AnnoIva", searchFilter.annoivaFilter, PropertyType.numeric));
            }
            if (searchFilter.identificativoSdiFilter) {
                oDataFilters = this.addToFilter(oDataFilters, this.getFilterString("IdentificativoSdi", searchFilter.identificativoSdiFilter, searchFilter.identificativoSdiFilterEq, PropertyType.string));
            }
            if (searchFilter.progressIDSDIFilter) {
                oDataFilters = this.addToFilter(oDataFilters, this.getFilterString("ProgessivoInvioSdi", searchFilter.progressIDSDIFilter, searchFilter.progressIDSDIFilterEq, PropertyType.string));
            }
            if (searchFilter.statofatturaFilter) {
                oDataFilters = this.addToFilter(oDataFilters, this.getFilterString_Eq("StatoFattura", searchFilter.statofatturaFilter, PropertyType.string));
            }
            if (searchFilter.pecMailBoxFilter) {
                oDataFilters = this.addToFilter(oDataFilters, this.getFilterString("IndirizzoPec", searchFilter.pecMailBoxFilter, searchFilter.numerofatturafilterEq, PropertyType.string));
            }
            if (searchFilter.pecMailBoxFilterEq) {
                oDataFilters = this.addEmptyValueToFilter(oDataFilters, "IndirizzoPec", searchFilter.pecMailBoxFilter, PropertyType.string);
            }
            else {
                oDataFilters = this.addToFilter(oDataFilters, this.getFilterString("IndirizzoPec", searchFilter.pecMailBoxFilter, searchFilter.numerofatturafilterEq, PropertyType.string));
            }
            if (oDataFilters != "") {
                urlPart = urlPart.concat("&$filter=", oDataFilters);
            }
            var url = urlPart;
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var UDSs = [];
                    var responseModel = new ODATAResponseModel(response);
                    $.each(response.Items.$values, function (i, value) {
                        UDSs.push(value);
                    });
                    responseModel.value = UDSs;
                    responseModel.count = response.Count;
                    callback(responseModel);
                }
                ;
            }, error);
        };
        UDSService.prototype.getOnlyToReadUDSArchives = function (searchFilter, top, skip, orderby, callback, error) {
            var urlPart = this._configuration.ODATAUrl;
            urlPart = urlPart.concat("?$count=true&$top=", top.toString(), "&$skip=", skip.toString());
            if (orderby && orderby != "") {
                urlPart = urlPart.concat("&$orderby=", orderby);
            }
            var oDataFilters = "_status eq 1";
            if (searchFilter.startDateFromFilter) {
                var propertyName = "RegistrationDate";
                oDataFilters = this.addToFilter(oDataFilters, propertyName.concat(' ge ', searchFilter.startDateFromFilter));
            }
            if (searchFilter.endDateFromFilter) {
                var propertyName = "RegistrationDate";
                oDataFilters = this.addToFilter(oDataFilters, propertyName.concat(' le ', searchFilter.endDateFromFilter));
            }
            if (searchFilter.registrationUserFilterEnabled && searchFilter.registrationUserFilter) {
                oDataFilters = this.addToFilter(oDataFilters, this.getFilterString("RegistrationUser", searchFilter.registrationUserFilter, false, PropertyType.string));
            }
            if (oDataFilters != "") {
                urlPart = urlPart.concat("&$filter=", oDataFilters);
            }
            urlPart = urlPart.concat("&applySecurity=1&onlyToRead=1");
            var url = urlPart;
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var UDSs = [];
                    var responseModel = new ODATAResponseModel(response);
                    $.each(response.Items.$values, function (i, value) {
                        UDSs.push(value);
                    });
                    responseModel.value = UDSs;
                    responseModel.count = response.Count;
                    callback(responseModel);
                }
                ;
            }, error);
        };
        UDSService.prototype.addToFilter = function (filterBase, filterNew) {
            if (filterNew != "") {
                if (filterBase != "") {
                    filterBase = filterBase.concat(" and ");
                }
                filterBase = filterBase.concat(filterNew);
            }
            return filterBase;
        };
        UDSService.prototype.addEmptyValueToFilter = function (filterBase, propertyName, propertyValue, propertyType) {
            if (filterBase != "") {
                filterBase = filterBase.concat(" and ");
            }
            if (propertyValue !== "") {
                filterBase = filterBase.concat(" contains(", propertyName, ",", this.setApici(propertyValue, propertyType), ") or ", propertyName, " eq \'\'");
            }
            else {
                filterBase = filterBase.concat(propertyName, " eq \'\'");
            }
            return filterBase;
        };
        UDSService.prototype.getFilterString = function (propertyName, propertyValue, filterEq, propertyType) {
            var filter = "";
            if (propertyValue !== "") {
                if (filterEq) {
                    filter = this.getFilterString_Eq(propertyName, propertyValue, propertyType);
                }
                else {
                    filter = this.getFilterString_Contains(propertyName, propertyValue, propertyType);
                }
            }
            return filter;
        };
        UDSService.prototype.getFilterString_Contains = function (propertyName, propertyValue, propertyType) {
            var filter = "";
            return propertyValue !== "" ? filter.concat("contains(", propertyName, ",", this.setApici(propertyValue, propertyType), ")") : "";
        };
        UDSService.prototype.getFilterString_Eq = function (propertyName, propertyValue, propertyType) {
            var filter = "";
            return propertyValue !== "" ? filter.concat(propertyName, " eq ", this.setApici(propertyValue, propertyType), "") : "";
        };
        UDSService.prototype.setApici = function (propertyValue, propertyType) {
            var newvalue = propertyValue;
            if (newvalue !== "" && propertyType == PropertyType.string) {
                newvalue = "'" + propertyValue.replace("'", "''") + "'";
            }
            else if (newvalue !== "" && propertyType == PropertyType.numeric) {
                newvalue = "" + propertyValue.toString().replace(",", ".");
            }
            return newvalue;
        };
        return UDSService;
    }(BaseService));
    var PropertyType;
    (function (PropertyType) {
        PropertyType[PropertyType["string"] = 0] = "string";
        PropertyType[PropertyType["numeric"] = 1] = "numeric";
    })(PropertyType || (PropertyType = {}));
    return UDSService;
});
//# sourceMappingURL=UDSService.js.map