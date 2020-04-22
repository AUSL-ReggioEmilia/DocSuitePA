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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/Monitors/TransparentAdministrationMonitorLogGridViewModelMapper"], function (require, exports, BaseService, TransparentAdministrationMonitorLogGridViewModelMapper) {
    var TransparentAdministrationMonitorLogService = /** @class */ (function (_super) {
        __extends(TransparentAdministrationMonitorLogService, _super);
        function TransparentAdministrationMonitorLogService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        TransparentAdministrationMonitorLogService.prototype.getTransparentAdministrationMonitorLogs = function (searchFilter, callback, error) {
            var filters = ",userName='" + searchFilter.username + "'";
            if (searchFilter.container === "") {
                searchFilter.container = "null";
            }
            filters = filters.concat(",idContainer=" + searchFilter.container);
            if (searchFilter.documentType === "") {
                searchFilter.documentType = "null";
            }
            filters = filters.concat(",environment=" + searchFilter.documentType);
            var oDataFilters = "$orderby=RegistrationDate desc";
            if (searchFilter.idRole) {
                oDataFilters = oDataFilters.concat("&$filter=IdRole eq ", searchFilter.idRole.toString());
            }
            var url = this._configuration.ODATAUrl.
                concat("/TransparentAdministrationMonitorLogService.GetByDateInterval(dateFrom='", searchFilter.dateFrom, "',dateTo='", searchFilter.dateTo, "'", filters, ")?", oDataFilters);
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var viewModelmapper_1 = new TransparentAdministrationMonitorLogGridViewModelMapper();
                    var transparentAdministrationMonitorLogs_1 = [];
                    $.each(response.value, function (i, value) {
                        transparentAdministrationMonitorLogs_1.push(viewModelmapper_1.Map(value));
                    });
                    callback(transparentAdministrationMonitorLogs_1);
                }
                ;
            }, error);
        };
        TransparentAdministrationMonitorLogService.prototype.insertTransparentAdministrationMonitorLog = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.postRequest(url, JSON.stringify(model), callback, error);
        };
        TransparentAdministrationMonitorLogService.prototype.updateTransparentAdministrationMonitorLog = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.putRequest(url, JSON.stringify(model), callback, error);
        };
        TransparentAdministrationMonitorLogService.prototype.getLatestMonitorLogByDocumentUnit = function (documentUnitId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var qs = "$filter=DocumentUnit/UniqueId eq ".concat(documentUnitId, "&$expand=DocumentUnit,Role&$orderby=RegistrationDate desc&$top=1");
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    if (response && response.value) {
                        callback(response.value[0]);
                    }
                    else {
                        callback(undefined);
                    }
                }
                ;
            }, error);
        };
        return TransparentAdministrationMonitorLogService;
    }(BaseService));
    return TransparentAdministrationMonitorLogService;
});
//# sourceMappingURL=TransparentAdministrationMonitorLogService.js.map