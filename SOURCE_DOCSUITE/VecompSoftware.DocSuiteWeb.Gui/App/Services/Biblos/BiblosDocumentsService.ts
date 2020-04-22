import ServiceConfiguration = require("../ServiceConfiguration");
import ExceptionDTO = require("../../DTOs/ExceptionDTO");
import BaseService = require("../BaseService");

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
}

export = BiblosDocumentsService;