import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import DocumentUnitModel = require('App/Models/DocumentUnits/DocumentUnitModel');
import DocumentUnitModelMapper = require('App/Mappers/DocumentUnits/DocumentUnitModelMapper');
import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import FascicleFinderViewModel = require('App/ViewModels/Fascicles/FascicleFinderViewModel');
import UDSRepositoryModel = require('App/Models/UDS/UDSRepositoryModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import Environment = require('App/Models/Environment');

class DocumentUnitService extends BaseService {
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
     * Recupera le Document Units per anno/numero e utente specifico
     * @param year
     * @param number
     * @param username
     * @param domain
     * @param isSecurityEnabled
     * @param callback
     * @param error
     */
    findDocumentUnits(skip: number, top: number, finderModel: FascicleFinderViewModel, fascicleModel: FascicleModel,
        callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {

        let year: string = (finderModel.Year && finderModel.Year > 0) ? finderModel.Year.toString() : null;
        let number: string = (!!finderModel.Number) ? "'".concat(finderModel.Number, "'") : null;
        let documentUnitName: string = (!!finderModel.UdType) ? "'".concat(finderModel.UdType, "'") : null;
        let categoryId: string = (finderModel.CategoryId) ? finderModel.CategoryId.toString() : null;
        let containerId: string = (finderModel.ContainerId) ? finderModel.ContainerId.toString() : null;
        let subject: string = (!!finderModel.Subject) ? "'".concat(finderModel.Subject, "'") : null;
        let includeChildClassification: boolean = finderModel.IncludeChildClassification;

        let url: string = this._configuration.ODATAUrl.concat("/DocumentUnitService.AuthorizedDocumentUnits(skip=", skip.toString(), ",top=", top.toString(), ",idFascicle=", fascicleModel.UniqueId,
            ",year=", year, ",number=", number, ",documentUnitName=", documentUnitName, ",categoryId=", categoryId, ",containerId=", containerId, ",subject=", subject, ",includeChildClassification=", includeChildClassification.toString(), ")");

        this.getRequest(url, null, (response: any) => {
            let instances: Array<DocumentUnitModel> = new Array<DocumentUnitModel>();
            let mapper = new DocumentUnitModelMapper();
            instances = mapper.MapCollection(response.value);
            if (callback) callback(instances);
        }, error);
    }

    /**
   * Recupera il count totale delle document units
   */
    countDocumentUnits(finderModel: FascicleFinderViewModel, fascicleModel: FascicleModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {

        let year: string = (finderModel.Year && finderModel.Year > 0) ? finderModel.Year.toString() : null;
        let number: string = (!!finderModel.Number) ? "'".concat(finderModel.Number, "'") : null;
        let documentUnitName: string = (!!finderModel.UdType) ? "'".concat(finderModel.UdType, "'") : null;
        let categoryId: string = (finderModel.CategoryId) ? finderModel.CategoryId.toString() : null;
        let containerId: string = (finderModel.ContainerId) ? finderModel.ContainerId.toString() : null;
        let subject: string = (!!finderModel.Subject) ? "'".concat(finderModel.Subject, "'") : null;
        let includeChildClassification: boolean = finderModel.IncludeChildClassification;

        let url: string = this._configuration.ODATAUrl.concat("/DocumentUnitService.CountAuthorizedDocumentUnits(idFascicle=", fascicleModel.UniqueId,
            ",year=", year, ",number=", number, ",documentUnitName=", documentUnitName, ",categoryId=", categoryId, ",containerId=", containerId, ",subject=", subject, ",includeChildClassification=", includeChildClassification.toString(), ")");

        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                callback(response.value);
            };
        }, error);
    }

    /**
     * Recupera le Document Units associate al Fascicolo
     * TODO: da implementare in SignalR
     * @param model
     * @param qs
     * @param callback
     * @param error
     */
    getFascicleDocumentUnits(model: FascicleModel, qs: string, idFascicleFolder?: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        if (!idFascicleFolder) {
            idFascicleFolder = null;
        }
        let url: string = this._configuration.ODATAUrl.concat("/DocumentUnitService.FascicleDocumentUnits(idFascicle=", model.UniqueId, ",idFascicleFolder=", idFascicleFolder,")");
        this.getRequest(url, qs,
            (response: any) => {
                if (callback) callback(response.value);
            }, error);
    }

    getDocumentUnitEnvironment(documentUnit: DocumentUnitModel): Environment {
        let env: Environment = (documentUnit.Environment < 100 ? <Environment>documentUnit.Environment : Environment.UDS);
        return env;
    }

    getDocumentUnitById(idDocumentUnit: string, callback?: (data: DocumentUnitModel) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `$filter=UniqueId eq ${idDocumentUnit}&$expand=Category,Container,UDSRepository,DocumentUnitRoles,DocumentUnitChains`;
        this.getRequest(url, data,
            (response: any) => {
                if (callback) {
                    let mapper = new DocumentUnitModelMapper();
                    callback(mapper.Map(response.value[0]));
                }
            }, error);
    }
}

export = DocumentUnitService;