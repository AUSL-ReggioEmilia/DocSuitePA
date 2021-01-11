import BaseService = require("App/Services/BaseService");
import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import ContactSearchFilterDTO = require("App/DTOs/ContactSearchFilterDTO");
import ContactModel = require("App/Models/Commons/ContactModel");
import ContactModelMapper = require("App/Mappers/Commons/ContactModelMapper");

class ContactService extends BaseService {
    private readonly _configuration: ServiceConfiguration;

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getById(idContact: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = `$filter=EntityId eq ${idContact}&$expand=Title,PlaceName`;
        this.getRequest(url, qs, (response: any) => {
            if (callback) {
                if (response.value.length == 0) {
                    callback(null);
                    return;
                }
                let instance: ContactModel = {} as ContactModel;
                let contactMapper: ContactModelMapper = new ContactModelMapper();
                instance = contactMapper.Map(response.value[0]);
                callback(instance);
            }
        }, error);
    }

    findContacts(finder: ContactSearchFilterDTO, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        url = url.concat(`/ContactService.FindContacts(finder=@p0)?@p0=${JSON.stringify(finder)}&$orderby=Description&$top=100`);
        this.getRequest(url, undefined, (response: any) => {
            if (callback) {                
                callback(response.value);
            }
        }, error);
    }

    getContactParents(idContact: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        url = url.concat(`/ContactService.GetContactParents(idContact=${idContact})`);
        this.getRequest(url, undefined, (response: any) => {
            if (callback) {
                callback(response.value);
            }
        }, error);
    }

    getRoleContacts(callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        url = url.concat(`/ContactService.GetRoleContacts()?$orderby=Description`);
        this.getRequest(url, undefined, (response: any) => {
            if (callback) {
                callback(response.value);
            }
        }, error);
    }

    getByParentId(contactParentId: number, top?: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = `${this._configuration.ODATAUrl}/ContactService.GetContactsByParentId(idContact=${contactParentId})`;
        if (top && top > 0) {
            url = `${url}?$top=${top}`;
        }
        this.getRequest(url, undefined, (response: any) => {
            if (callback) {
                if (callback) {
                    callback(response.value);
                }
            }
        }, error);
    }

    /**
     * metodo per l'inserimento di un nuovo contatto
     * @param categoryFascicle
     * @param callback
     * @param error
     */
    insertContact(contact: ContactModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(contact), callback, error)
    }
}

export = ContactService;