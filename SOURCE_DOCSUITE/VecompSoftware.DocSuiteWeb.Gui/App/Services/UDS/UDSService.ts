import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import UDSModel = require('App/Models/UDS/UDSModel');
import PageResultModel = require('App/Models/PageResultModel');
import UDSLookupViewModel = require('App/ViewModels/UDS/UDSLookupViewModel');
import UDSModelMapper = require('App/Mappers/UDS/UDSModelMapper');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UDSInvoiceSearchFilterDTO = require('App/DTOs/UDSInvoiceSearchFilterDTO');
import UDSArchiveSearchFilterDTO = require('App/DTOs/UDSArchiveSearchFilterDTO');


class UDSService extends BaseService {
    private _configuration: ServiceConfiguration;

    /**
     * Costruttore
     * @param webApiUrl
     */
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    /**
     * Recupera una UDS per UniqueId
     * @param uniqueId
     * @param callback
     * @param error
     */
    getUDSByUniqueId(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "$filter=UDSId eq ".concat(uniqueId.toString());
        this.getRequest(url, data, (response: any) => {
            if (callback) {
                let modelMapper = new UDSModelMapper();
                callback(modelMapper.Map(response.Items.$values[0]));
            }
        }, error);
    }

    /**
     * Recupera i valori del campo lookup
     * @param uniqueId
     * @param callback
     * @param error
     */
    getLookupValues(propertyName: string, filter: string, top: string, skip: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        let filterParameter: string = filter ? filter : '';
        let data: string = "isSearch=true&$filter=cast(".concat(propertyName, ",'Edm.String') ne '' and contains(cast(", propertyName, ",'Edm.String'),'", filterParameter, "') and _status eq 1 &$count=true&$top=", top, "&$skip=", skip.toString());
        this.getRequest(url, data, (response: any) => {
            if (callback && response) {
                let pageResult: PageResultModel = new PageResultModel(response);
                let results: string[] = pageResult.items.map(item => item[propertyName].toString());
                let udsLookupViewModel: UDSLookupViewModel = <UDSLookupViewModel>{};
                udsLookupViewModel.values = results.map(i => {
                    let date = moment(i);
                    if (date.isValid()) {
                        i = date.format("DD/MM/YYYY");
                    }
                    return i;
                })
                udsLookupViewModel.count = pageResult.count;
                callback(udsLookupViewModel);
            }
        }, error);
    }


    getUDSInvoices(searchFilter: UDSInvoiceSearchFilterDTO, top: number, skip: number, orderby: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let urlPart: string = this._configuration.ODATAUrl;
        urlPart = urlPart.concat("?$count=true&$top=", top.toString(), "&$skip=", skip.toString());
        if (orderby && orderby != "") {
            urlPart = urlPart.concat("&$orderby=", orderby);
        }
        let oDataFilters: string = "_status eq 1";

        if (searchFilter.startDateFromFilter) {
            let propertyName = "DataFattura"
            oDataFilters = this.addToFilter(oDataFilters, propertyName.concat(' ge ', searchFilter.startDateFromFilter));
        }
        if (searchFilter.endDateFromFilter) {
            let propertyName = "DataFattura"
            oDataFilters = this.addToFilter(oDataFilters, propertyName.concat(' le ', searchFilter.endDateFromFilter));
        }


        if (searchFilter.dataIvaFromFilter) {
            let propertyName = "DataIva"
            oDataFilters = this.addToFilter(oDataFilters, propertyName.concat(' ge ', searchFilter.dataIvaFromFilter));
        }
        if (searchFilter.dataIvaToFilter) {
            let propertyName = "DataIva"
            oDataFilters = this.addToFilter(oDataFilters, propertyName.concat(' le ', searchFilter.dataIvaToFilter));
        }


        if (searchFilter.dataReceivedFromFilter) {
            let propertyName = "DataRicezioneSdi"
            oDataFilters = this.addToFilter(oDataFilters, propertyName.concat(' ge ', searchFilter.dataReceivedFromFilter));
        }
        if (searchFilter.dataReceivedToFilter) {
            let propertyName = "DataRicezioneSdi"
            oDataFilters = this.addToFilter(oDataFilters, propertyName.concat(' le ', searchFilter.dataReceivedToFilter));
        }


        if (searchFilter.dataacceptFromFilter) {
            let propertyName = "DataAccettazione"
            oDataFilters = this.addToFilter(oDataFilters, propertyName.concat(' ge ', searchFilter.dataacceptFromFilter));
        }
        if (searchFilter.dataacceptToFilter) {
            let propertyName = "DataAccettazione"
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
        } else {
            oDataFilters = this.addToFilter(oDataFilters, this.getFilterString("IndirizzoPec", searchFilter.pecMailBoxFilter, searchFilter.numerofatturafilterEq, PropertyType.string));
        }
        if (oDataFilters != "") { urlPart = urlPart.concat("&$filter=", oDataFilters); }
        let url: string = urlPart;

        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                var UDSs: any[] = [];
                let responseModel: ODATAResponseModel<any> = new ODATAResponseModel<any>(response);
                $.each(response.Items.$values, function (i, value) {
                    UDSs.push(value);
                });
                responseModel.value = UDSs;
                responseModel.count = response.Count;
                callback(responseModel);
            };
        }, error);
    }

    getOnlyToReadUDSArchives(searchFilter: UDSArchiveSearchFilterDTO, top: number, skip: number, orderby: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let urlPart: string = this._configuration.ODATAUrl;
        urlPart = urlPart.concat("?$count=true&$top=", top.toString(), "&$skip=", skip.toString());
        if (orderby && orderby != "") {
            urlPart = urlPart.concat("&$orderby=", orderby);
        }
        let oDataFilters: string = "_status eq 1";

        if (searchFilter.startDateFromFilter) {
            let propertyName = "RegistrationDate"
            oDataFilters = this.addToFilter(oDataFilters, propertyName.concat(' ge ', searchFilter.startDateFromFilter));
        }
        if (searchFilter.endDateFromFilter) {
            let propertyName = "RegistrationDate"
            oDataFilters = this.addToFilter(oDataFilters, propertyName.concat(' le ', searchFilter.endDateFromFilter));
        }
        if (searchFilter.registrationUserFilterEnabled && searchFilter.registrationUserFilter) {
            oDataFilters = this.addToFilter(oDataFilters, this.getFilterString("RegistrationUser", searchFilter.registrationUserFilter, false, PropertyType.string));
        }

        if (oDataFilters != "") { urlPart = urlPart.concat("&$filter=", oDataFilters); }
        urlPart = urlPart.concat("&applySecurity=1&onlyToRead=1");
        let url: string = urlPart;

        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                var UDSs: any[] = [];
                let responseModel: ODATAResponseModel<any> = new ODATAResponseModel<any>(response);
                $.each(response.Items.$values, function (i, value) {
                    UDSs.push(value);
                });
                responseModel.value = UDSs;
                responseModel.count = response.Count;
                callback(responseModel);
            };
        }, error);
    }

    addToFilter(filterBase: string, filterNew: string): string {
        if (filterNew != "") {
            if (filterBase != "") {
                filterBase = filterBase.concat(" and ");
            }
            filterBase = filterBase.concat(filterNew)
        }
        return filterBase;
    }


    addEmptyValueToFilter(filterBase: string, propertyName: string, propertyValue: string, propertyType: PropertyType): string {

        if (filterBase != "") {
            filterBase = filterBase.concat(" and ");
        }
        if (propertyValue !== "") {
            filterBase = filterBase.concat(" contains(", propertyName, ",", this.setApici(propertyValue, propertyType), ") or ", propertyName, " eq \'\'");
        } else {
            filterBase = filterBase.concat(propertyName, " eq \'\'");
        }

        return filterBase;
    }


    getFilterString(propertyName: string, propertyValue: string, filterEq: boolean, propertyType: PropertyType): string {
        let filter: string = "";
        if (propertyValue !== "") {
            if (filterEq) {
                filter = this.getFilterString_Eq(propertyName, propertyValue, propertyType);
            } else {
                filter = this.getFilterString_Contains(propertyName, propertyValue, propertyType);
            }
        }
        return filter;
    }

    getFilterString_Contains(propertyName: string, propertyValue: string, propertyType: PropertyType): string {
        let filter: string = "";
        return propertyValue !== "" ? filter.concat("contains(", propertyName, ",", this.setApici(propertyValue, propertyType), ")") : "";
    }

    getFilterString_Eq(propertyName: string, propertyValue: string, propertyType: PropertyType): string {
        let filter: string = "";
        return propertyValue !== "" ? filter.concat(propertyName, " eq ", this.setApici(propertyValue, propertyType), "") : "";
    }

    setApici(propertyValue: string, propertyType: PropertyType) {
        let newvalue = propertyValue;
        if (newvalue !== "" && propertyType == PropertyType.string) {
            newvalue = `'${propertyValue.replace("'", "''")}'`;
        } else if (newvalue !== "" && propertyType == PropertyType.numeric) {
            newvalue = `${propertyValue.toString().replace(",", ".")}`;
        }

        return newvalue;
    }
}

export = UDSService;
enum PropertyType {
    string = 0,
    numeric = 1
}