import BaseService = require("../BaseService");
import ServiceConfiguration = require("../ServiceConfiguration");
import ExceptionDTO = require("../../DTOs/ExceptionDTO");
import FascicleDocumentUnitCategoryModelMapper = require("../../Mappers/Fascicles/FascicleDocumentUnitCategoryModelMapper");

class DocumentUnitFascicleCategoriesService extends BaseService {
    private _configuration: ServiceConfiguration;

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getDocumentUnitFascicleCategory(idDocumentUnit: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `$apply=filter(DocumentUnit/UniqueId eq ${idDocumentUnit})/groupby((Category/FullCode,Category/Name))&$orderby=Category/FullCode`

        this.getRequest(url, data,
            (response: any) => {
                if (callback) {
                    let mapper = new FascicleDocumentUnitCategoryModelMapper();
                    callback(mapper.MapCollection(response.value));
                }
            }, error);
    }
}

export = DocumentUnitFascicleCategoriesService;