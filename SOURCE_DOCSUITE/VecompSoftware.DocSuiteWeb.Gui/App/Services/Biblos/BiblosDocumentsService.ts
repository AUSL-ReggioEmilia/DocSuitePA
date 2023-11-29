import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import BaseService = require("App/Services/BaseService");

class BiblosDocumentsService extends BaseService {
    _configuration: ServiceConfiguration;

    /**
     * Costruttore
     */

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getDocumentsByChainId(id: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        url = url.concat("/DocumentUnitService.GetDocumentsByArchiveChain(idArchiveChain=", id, ")");

        this.getRequest(url, null, (response: any) => {
            if (callback) {
                callback(response.value);
            }
        }, error);
    }

    getBiblosDocumentContent(id: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        url = url.concat("/DocumentUnitService.GetBiblosDocumentContent(documentId=", id, ")");

        this.getRequest(url, null, (response: any) => {
            if (callback) {
                callback(response.value);
            }
        }, error);
    }
}

export = BiblosDocumentsService;