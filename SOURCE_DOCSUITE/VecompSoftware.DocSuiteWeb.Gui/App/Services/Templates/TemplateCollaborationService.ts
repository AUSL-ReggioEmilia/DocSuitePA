import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import TemplateCollaborationModel = require('App/Models/Templates/TemplateCollaborationModel');
import TemplateCollaborationStatus = require('App/Models/Templates/TemplateCollaborationStatus');
import UpdateActionType = require("App/Models/UpdateActionType");
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import TemplateCollaborationModelMapper = require('App/Mappers/Templates/TemplateCollaborationModelMapper');
import OdataCountValue = require('App/Helpers/OdataCountValue');
import EnumValue = require('App/Helpers/EnumValue');

class TemplateCollaborationService extends BaseService {
    _configuration: ServiceConfiguration;

    // NOTE: the mapper was introduced at a later stage in the service. Not all returns are mapped
    // Reason: TemplateCollaborationModel was made from interface to class to make use of some getter properties
    //         To use these properties we needed to instantiate the obejct instead of just use it as a 'intellisense' tool

    _mapper: TemplateCollaborationModelMapper;
    /**
     * Costruttore
     */
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
        this._mapper = new TemplateCollaborationModelMapper();
    }

    getById(templateId: string, callback?: (data: TemplateCollaborationModel) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = "$filter=UniqueId eq ".concat(templateId, "&$expand=TemplateCollaborationUsers($expand=Role),TemplateCollaborationDocumentRepositories,Roles");
        this.getRequest(url, qs,
            (response: any) => {
                if (callback) {
                    callback(this._mapper.Map(response.value[0]));
                }
            }, error);
    }

    /**
     * Returns a template collaboration model which has the folder path STRICTLY equal to folder path
     */
    getByTemplateCollaborationPath(templateCollaborationPath: string, callback?: (data: TemplateCollaborationModel) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = `$filter=TemplateCollaborationPath eq '${templateCollaborationPath}'&$expand=TemplateCollaborationUsers($expand=Role),TemplateCollaborationDocumentRepositories,Roles`;
        this.getRequest(url, qs,
            (response: any) => {
                if (callback) {
                    callback(this._mapper.Map(response.value[0]));
                }
            }, error);
    }

    getMultipleByTemplateCollaborationPath(templateCollaborationPath: string[], callback?: (data: TemplateCollaborationModel[]) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;

        templateCollaborationPath = templateCollaborationPath.map(r => `'${r}'`);
        
        let qs: string = `$filter=TemplateCollaborationPath in (${templateCollaborationPath.join(',')})&$expand=TemplateCollaborationUsers($expand=Role),TemplateCollaborationDocumentRepositories,Roles`;
        this.getRequest(url, qs,
            (response: any) => {
                if (callback) {
                    callback(this._mapper.MapCollection(response.value));
                }
            }, error);
    }

    getTemplates(callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = "$filter=Status eq 'Active'";
        this.getRequest(url, qs,
            (response: any) => {
                if (callback) {
                    callback(response.value);
                }
            }, error);
    }

    getFixedTemplates(idParent: string, callback?: (data: TemplateCollaborationModel[]) => any, error?: (exception: ExceptionDTO) => any, status?: TemplateCollaborationStatus | null) {
        let url: string = `${this._configuration.ODATAUrl}/TemplateCollaborationService.GetChildren(idParent=${idParent},status=${status ?? null})`;

        this.getRequest(url, "",
            (response: any) => {
                callback(this._mapper.MapCollection(response.value));
            },
            (exception: ExceptionDTO) => {
                error(exception)
            });
    }

    getDirectDescendands(node: TemplateCollaborationModel, status: TemplateCollaborationStatus | null, skip?: number, callback?: (data: TemplateCollaborationModel[]) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let nextLevel = node.TemplateCollaborationLevel + 1;
        let qs: string = `$filter=startswith(TemplateCollaborationPath, '${node.TemplateCollaborationPath}') and TemplateCollaborationPath ne '${node.TemplateCollaborationPath}' and TemplateCollaborationLevel eq ${nextLevel}`;
        if (status !== null && status !== undefined) {
            qs += ` and ${this.buildNonFixedTemplateStatusFilter(status)}`
        }
        if (skip != null && skip != undefined) {
            qs += `&$skip=${skip}`;
        }
        qs += "&$orderby=Name";

        this.getRequest(url, qs,
            (response: any) => {
                callback(this._mapper.MapCollection(response.value));
            },
            (exception: ExceptionDTO) => {
                error(exception)
            });
    }

    countDirectDescendands(node: TemplateCollaborationModel, status: TemplateCollaborationStatus | null, callback?: (data: number) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let nextLevel = node.TemplateCollaborationLevel + 1;
        let qs: string = `$filter=startswith(TemplateCollaborationPath, '${node.TemplateCollaborationPath}') and TemplateCollaborationPath ne '${node.TemplateCollaborationPath}' and TemplateCollaborationLevel eq ${nextLevel}`;
        if (status !== null && status !== undefined) {
            qs += ` and ${this.buildNonFixedTemplateStatusFilter(status)}`
        }
        qs += "&$count=true&$top=0";

        this.getRequest(url, qs,
            (response: any) => {
                let countValue = new OdataCountValue(response);
                callback(countValue.Count);
            },
            (exception: ExceptionDTO) => {
                error(exception)
            });
    }

    hasChildren(node: TemplateCollaborationModel, status: TemplateCollaborationStatus | null, callback?: (data: boolean) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = `$filter=startswith(TemplateCollaborationPath, '${node.TemplateCollaborationPath}') and TemplateCollaborationPath ne '${node.TemplateCollaborationPath}' and TemplateCollaborationLevel eq ${node.TemplateCollaborationLevel + 1} `;
        if (status !== null && status !== undefined) {
            qs += ` and ${this.buildNonFixedTemplateStatusFilter(status)}`
        }
        qs += ` &$count=true&$top=0`;

        this.getRequest(url, qs,
            (response: any) => {
                let countValue = new OdataCountValue(response);
                callback(countValue.Count > 0);
            },
            (exception: ExceptionDTO) => {
                error(exception)
            });
    }

    getAllParentsOfTemplate(templateId: string, callback?: (data: TemplateCollaborationModel[]) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        url = url.concat(`/TemplateCollaborationService.GetAllParentsOfTemplate(templateId=${templateId})`);
        this.getRequest(url, "",
            (response: any) => {
                callback(this._mapper.MapCollection(response.value));
            },
            (exception: ExceptionDTO) => {
                error(exception)
            });
    }

    private buildFixedTemplateStatusFilter(status: TemplateCollaborationStatus): string {
        let enumValue = new EnumValue(status, TemplateCollaborationStatus);
        return `Status eq '${enumValue.ValueAsString}'`;
    }

    private buildNonFixedTemplateStatusFilter(status: TemplateCollaborationStatus): string {
        let enumValue = new EnumValue(status, TemplateCollaborationStatus);
        return `(RepresentationType eq 'Folder' or (RepresentationType eq 'Template' and Status eq '${enumValue.ValueAsString}'))`;
    }

    /**
     * Inserisce un nuovo Template di collaborazione
     * @param model
     * @param callback
     * @param error
     */
    insertTemplateCollaboration(model: TemplateCollaborationModel, callback?: (data: TemplateCollaborationModel) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(model), data => {
            let mapped = this._mapper.Map(data);
            callback(mapped);
        }, error);
    }

    /**
     * Modifica un Template di collaborazione esistente
     * @param model
     * @param callback
     * @param error
     */
    updateTemplateCollaboration(model: TemplateCollaborationModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.putRequest(url, JSON.stringify(model), callback, error);
    }

    /**
     * Cancellazione di un Template di collaborazione esistente
     * @param model
     * @param callback
     * @param error
     */
    deleteTemplateCollaboration(model: TemplateCollaborationModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.deleteRequest(url, JSON.stringify(model), callback, error);
    }

    /**
     * Pubblica un template di collaborazione
     * @param model
     * @param callback
     * @param error
     */
    publishTemplate(model: TemplateCollaborationModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        model.Status = TemplateCollaborationStatus.Active;
        let url: string = this._configuration.WebAPIUrl.concat("?actionType=", UpdateActionType.TemplateCollaborationPublish.toString());
        this.putRequest(url, JSON.stringify(model), callback, error);
    }

    getRootNode(callback?: (data: TemplateCollaborationModel) => any, error?: (exception: ExceptionDTO) => any) {
        let qs: string = `$filter=TemplateCollaborationLevel eq 0`;

        this.getRequest(this._configuration.ODATAUrl, qs,
            (response: any) => {
                callback(this._mapper.Map(response.value[0]));
            },
            (exception: ExceptionDTO) => {
                error(exception)
            });
    }
}

export = TemplateCollaborationService;