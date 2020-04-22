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
define(["require", "exports", "App/Services/BaseService", "App/Models/Templates/TemplateCollaborationStatus", "App/Models/UpdateActionType"], function (require, exports, BaseService, TemplateCollaborationStatus, UpdateActionType) {
    var TemplateCollaborationService = /** @class */ (function (_super) {
        __extends(TemplateCollaborationService, _super);
        /**
         * Costruttore
         */
        function TemplateCollaborationService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        TemplateCollaborationService.prototype.getById = function (templateId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var qs = "$filter=UniqueId eq ".concat(templateId, "&$expand=TemplateCollaborationUsers($expand=Role),TemplateCollaborationDocumentRepositories,Roles");
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    callback(response.value[0]);
                }
            }, error);
        };
        TemplateCollaborationService.prototype.getTemplates = function (callback, error) {
            var url = this._configuration.ODATAUrl;
            var qs = "$filter=Status eq 'Active'";
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    callback(response.value);
                }
            }, error);
        };
        /**
         * Inserisce un nuovo Template di collaborazione
         * @param model
         * @param callback
         * @param error
         */
        TemplateCollaborationService.prototype.insertTemplateCollaboration = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.postRequest(url, JSON.stringify(model), callback, error);
        };
        /**
         * Modifica un Template di collaborazione esistente
         * @param model
         * @param callback
         * @param error
         */
        TemplateCollaborationService.prototype.updateTemplateCollaboration = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.putRequest(url, JSON.stringify(model), callback, error);
        };
        /**
         * Cancellazione di un Template di collaborazione esistente
         * @param model
         * @param callback
         * @param error
         */
        TemplateCollaborationService.prototype.deleteTemplateCollaboration = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.deleteRequest(url, JSON.stringify(model), callback, error);
        };
        /**
         * Pubblica un template di collaborazione
         * @param model
         * @param callback
         * @param error
         */
        TemplateCollaborationService.prototype.publishTemplate = function (model, callback, error) {
            model.Status = TemplateCollaborationStatus.Active;
            var url = this._configuration.WebAPIUrl.concat("?actionType=", UpdateActionType.TemplateCollaborationPublish.toString());
            this.putRequest(url, JSON.stringify(model), callback, error);
        };
        return TemplateCollaborationService;
    }(BaseService));
    return TemplateCollaborationService;
});
//# sourceMappingURL=TemplateCollaborationService.js.map