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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/Dossiers/DossierLogViewModelMapper", "App/Models/ODATAResponseModel"], function (require, exports, BaseService, DossierLogViewModelMapper, ODATAResponseModel) {
    var DossierLogService = /** @class */ (function (_super) {
        __extends(DossierLogService, _super);
        /**
        * Costruttore
        */
        function DossierLogService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        DossierLogService.prototype.getDossierLogs = function (idDossier, skip, top, callback, error) {
            var url = this._configuration.ODATAUrl.concat("?$filter=Entity/UniqueId eq ", idDossier, " and (LogType ne VecompSoftware.DocSuiteWeb.Entity.Dossiers.DossierLogType'FolderHystory')&$orderby=RegistrationDate&$skip=", skip.toString(), "&$top=", top.toString(), "&$count=true");
            this.getRequest(url, undefined, function (response) {
                if (callback) {
                    var mapper_1 = new DossierLogViewModelMapper();
                    var dossierLogs_1 = [];
                    var responseModel = new ODATAResponseModel(response);
                    if (response) {
                        $.each(response.value, function (i, value) {
                            dossierLogs_1.push(mapper_1.Map(value));
                        });
                        responseModel.value = dossierLogs_1;
                        callback(responseModel);
                    }
                }
            }, error);
        };
        return DossierLogService;
    }(BaseService));
    return DossierLogService;
});
//# sourceMappingURL=DossierLogService.js.map