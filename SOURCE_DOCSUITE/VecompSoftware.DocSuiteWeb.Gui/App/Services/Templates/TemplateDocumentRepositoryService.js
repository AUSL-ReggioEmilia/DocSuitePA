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
define(["require", "exports", "App/Mappers/Templates/TemplateDocumentRepositoryModelMapper", "App/Services/BaseService"], function (require, exports, TemplateDocumentRepositoryModelMapper, BaseService) {
    var TemplateDocumentRepositoryService = /** @class */ (function (_super) {
        __extends(TemplateDocumentRepositoryService, _super);
        /**
         * Costruttore
         */
        function TemplateDocumentRepositoryService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            _this._modelMapper = new TemplateDocumentRepositoryModelMapper();
            return _this;
        }
        /**
         * Inserisce un nuovo TemplateDocument
         * @param model
         * @param callback
         * @param error
         */
        TemplateDocumentRepositoryService.prototype.insertTemplateDocument = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.postRequest(url, JSON.stringify(model), callback, error);
        };
        /**
         * Modifica un TemplateDocument
         * @param model
         * @param callback
         * @param error
         */
        TemplateDocumentRepositoryService.prototype.updateTemplateDocument = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.putRequest(url, JSON.stringify(model), callback, error);
        };
        /**
         * Recupera la lista dei Tag di tutti i Template inseriti
         * @param callback
         * @param error
         */
        TemplateDocumentRepositoryService.prototype.getTags = function (callback, error) {
            var url = this._configuration.ODATAUrl.concat("/TemplateDocumentRepositoryService.GetTags()");
            this.getRequest(url, undefined, callback, error);
        };
        /**
         * Recupera uno specifico Template dato uno specifico ID
         * @param templateId
         * @param callback
         * @param error
         */
        TemplateDocumentRepositoryService.prototype.getTemplateById = function (templateId, callback, error) {
            var _this = this;
            var url = this._configuration.ODATAUrl.concat("?$filter=UniqueId eq ", templateId);
            this.getRequest(url, undefined, function (response) {
                if (callback) {
                    callback(_this._modelMapper.Map(response.value[0]));
                }
            }, error);
        };
        /**
        * Carica i TemplateDocument già esistenti attraverso filtro di ricerca sul nome del template
        * @param name
        * @param model
        * @param callback
        * @param error
        */
        TemplateDocumentRepositoryService.prototype.findTemplateDocument = function (templateFinder, callback, error) {
            var _this = this;
            var url = this._configuration.ODATAUrl;
            var params = '';
            if (templateFinder.hasFilter()) {
                params = params.concat("$filter=");
                var filters = new Array();
                if (!!templateFinder.Name) {
                    filters.push("contains(Name,'".concat(templateFinder.Name, "')"));
                }
                if (templateFinder.Tags.length > 0) {
                    var odataTags_1 = new Array();
                    $.each(templateFinder.Tags, function (index, tag) {
                        odataTags_1.push("contains(QualityTag,'".concat(tag, "')"));
                    });
                    filters.push("(".concat(odataTags_1.join(" or "), ")"));
                }
                if (templateFinder.Status.length > 0) {
                    var odataStatus_1 = new Array();
                    $.each(templateFinder.Status, function (index, status) {
                        odataStatus_1.push("Status eq VecompSoftware.DocSuiteWeb.Entity.Templates.TemplateDocumentationRepositoryType'".concat(status.toString(), "'"));
                    });
                    filters.push("(".concat(odataStatus_1.join(" or "), ")"));
                }
                params = params.concat(filters.join(" and "));
            }
            if (!!params) {
                params = params.concat('&');
            }
            params = params.concat('$orderby=Name asc');
            this.getRequest(url, params, function (response) {
                if (callback) {
                    var tmp_1 = new Array();
                    $.each(response.value, function (index, item) {
                        tmp_1.push(_this._modelMapper.Map(item));
                    });
                    callback(tmp_1);
                }
            }, error);
        };
        /**
         * Elimina un TemplateDocument esistente
         * @param model
         * @param callback
         * @param error
         */
        TemplateDocumentRepositoryService.prototype.deleteTemplateDocument = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.deleteRequest(url, JSON.stringify(model), callback, error);
        };
        return TemplateDocumentRepositoryService;
    }(BaseService));
    return TemplateDocumentRepositoryService;
});
//# sourceMappingURL=TemplateDocumentRepositoryService.js.map