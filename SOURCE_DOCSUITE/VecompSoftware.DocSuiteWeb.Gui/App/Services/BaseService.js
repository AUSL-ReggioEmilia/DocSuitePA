define(["require", "exports", "App/Services/HTTPProtocolType", "App/Helpers/ErrorHelper"], function (require, exports, HTTPProtocolType, ErrorHelper) {
    var BaseService = /** @class */ (function () {
        /**
         * Costruttore
         */
        function BaseService() {
        }
        /**
         * Invia una nuova richiesta Ajax
         * @param protocol
         * @param url
         * @param data
         * @param callback
         * @param error
         */
        BaseService.prototype.sendAjaxRequest = function (protocol, url, data, callback, error) {
            $.ajax({
                type: HTTPProtocolType[protocol],
                url: url,
                data: data,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                xhrFields: {
                    withCredentials: true
                },
                success: function (response) {
                    if (callback)
                        callback(response);
                },
                error: function (XMLHttpRequest) {
                    console.log(JSON.stringify(XMLHttpRequest));
                    var errorHelper = new ErrorHelper();
                    var exception = errorHelper.getExceptionModel(XMLHttpRequest);
                    if (error)
                        error(exception);
                }
            });
        };
        /**
         * Invia una nuova richiesta Ajax di tipo GET
         * @param url
         * @param data
         * @param callback
         * @param error
         */
        BaseService.prototype.getRequest = function (url, data, callback, error) {
            this.sendAjaxRequest(HTTPProtocolType.GET, url, data, callback, error);
        };
        /**
         * Invia una nuova richiesta Ajax di tipo POST
         * @param url
         * @param data
         * @param callback
         * @param error
         */
        BaseService.prototype.postRequest = function (url, data, callback, error) {
            this.sendAjaxRequest(HTTPProtocolType.POST, url, data, callback, error);
        };
        /**
         * Invia una nuova richiesta Ajax di tipo PUT
         * @param url
         * @param data
         * @param callback
         * @param error
         */
        BaseService.prototype.putRequest = function (url, data, callback, error) {
            this.sendAjaxRequest(HTTPProtocolType.PUT, url, data, callback, error);
        };
        /**
         * Invia una nuova richiesta Ajax di tipo DELETE
         * @param url
         * @param data
         * @param callback
         * @param error
         */
        BaseService.prototype.deleteRequest = function (url, data, callback, error) {
            this.sendAjaxRequest(HTTPProtocolType.DELETE, url, data, callback, error);
        };
        return BaseService;
    }());
    return BaseService;
});
//# sourceMappingURL=BaseService.js.map