import HTTPProtocolType = require('App/Services/HTTPProtocolType');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import ErrorHelper = require('App/Helpers/ErrorHelper');

abstract class BaseService {

    /**
     * Costruttore
     */
    constructor() {
    }

    /**
     * Invia una nuova richiesta Ajax
     * @param protocol
     * @param url
     * @param data
     * @param callback
     * @param error
     */
       private sendAjaxRequest(protocol: HTTPProtocolType, url: string, data?: any, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any) {
        $.ajax({
            type: HTTPProtocolType[protocol],
            url: url,
            data: data,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            xhrFields: {
                withCredentials: true
            },
            success: function (response: any) {
                if (callback) callback(response);
            },
            error: function (XMLHttpRequest: JQueryXHR) {
                console.log(JSON.stringify(XMLHttpRequest));     
                let errorHelper: ErrorHelper = new ErrorHelper();
                let exception: ExceptionDTO = errorHelper.getExceptionModel(XMLHttpRequest);
                if (error) error(exception);
            }
        });
    }

    

    /**
     * Invia una nuova richiesta Ajax di tipo GET
     * @param url
     * @param data
     * @param callback
     * @param error
     */
   
    getRequest(url: string, data?: any, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any) {
        this.sendAjaxRequest(HTTPProtocolType.GET, url, data, callback, error);
    }

    /**
     * Invia una nuova richiesta Ajax di tipo POST
     * @param url
     * @param data
     * @param callback
     * @param error
     */
    postRequest(url: string, data?: any, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any) {
        this.sendAjaxRequest(HTTPProtocolType.POST, url, data, callback, error);
    }

    /**
     * Invia una nuova richiesta Ajax di tipo PUT
     * @param url
     * @param data
     * @param callback
     * @param error
     */
    putRequest(url: string, data?: any, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any) {
        this.sendAjaxRequest(HTTPProtocolType.PUT, url, data, callback, error);
    }

    /**
     * Invia una nuova richiesta Ajax di tipo DELETE
     * @param url
     * @param data
     * @param callback
     * @param error
     */
    deleteRequest(url: string, data?: any, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any) {
        this.sendAjaxRequest(HTTPProtocolType.DELETE, url, data, callback, error);
    }
}

export = BaseService;