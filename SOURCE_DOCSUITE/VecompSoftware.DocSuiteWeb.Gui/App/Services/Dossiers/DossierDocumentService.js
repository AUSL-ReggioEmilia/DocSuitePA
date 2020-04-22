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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/Dossiers/DossierSummaryDocumentViewModelMapper"], function (require, exports, BaseService, DossierSummaryDocumentViewModelMapper) {
    var DossierDocumentService = /** @class */ (function (_super) {
        __extends(DossierDocumentService, _super);
        /**
        * Costruttore
        */
        function DossierDocumentService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        DossierDocumentService.prototype.getDossierDocuments = function (idDossier, callback, error) {
            var url = this._configuration.ODATAUrl.concat("?$filter=Dossier/UniqueId eq ", idDossier);
            this.getRequest(url, undefined, function (response) {
                if (callback) {
                    var instance_1 = new DossierSummaryDocumentViewModelMapper();
                    var dossierDocuments_1 = [];
                    if (response) {
                        $.each(response.value, function (i, value) {
                            dossierDocuments_1.push(instance_1.Map(value));
                        });
                        callback(dossierDocuments_1);
                    }
                }
            }, error);
        };
        DossierDocumentService.prototype.insertDossierDocument = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.postRequest(url, JSON.stringify(model), callback, error);
        };
        return DossierDocumentService;
    }(BaseService));
    return DossierDocumentService;
});
//# sourceMappingURL=DossierDocumentService.js.map