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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/UDS/UDSLogViewModelMapper", "App/Models/ODATAResponseModel"], function (require, exports, BaseService, UDSLogViewModelMapper, ODATAResponseModel) {
    var UDSLogService = /** @class */ (function (_super) {
        __extends(UDSLogService, _super);
        /**
        * Costruttore
        */
        function UDSLogService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        UDSLogService.prototype.getUDSLogs = function (idUDS, skip, top, callback, error) {
            var url = this._configuration.ODATAUrl.concat("?$filter=IdUDS eq ", idUDS, "&$orderby=RegistrationDate desc&$skip=", skip.toString(), "&$top=", top.toString(), "&$count=true");
            this.getRequest(url, undefined, function (response) {
                if (callback) {
                    var mapper_1 = new UDSLogViewModelMapper();
                    var UDSLogs_1 = [];
                    var responseModel = new ODATAResponseModel(response);
                    if (response) {
                        $.each(response.value, function (i, value) {
                            UDSLogs_1.push(mapper_1.Map(value));
                        });
                        responseModel.value = UDSLogs_1;
                        callback(responseModel);
                    }
                }
            }, error);
        };
        UDSLogService.prototype.getUDSLogsByRegistrationUserAndLogType = function (registrationUser, logType, callback, error) {
            var url = this._configuration.ODATAUrl.concat("?$filter=contains(RegistrationUser,'", registrationUser, "') and LogType eq '", logType, "' &$count=true");
            this.getRequest(url, undefined, function (response) {
                if (callback) {
                    var mapper_2 = new UDSLogViewModelMapper();
                    var UDSLogs_2 = [];
                    var responseModel = new ODATAResponseModel(response);
                    if (response) {
                        $.each(response.value, function (i, value) {
                            UDSLogs_2.push(mapper_2.Map(value));
                        });
                        responseModel.value = UDSLogs_2;
                        callback(responseModel);
                    }
                }
            }, error);
        };
        UDSLogService.prototype.getMyUDSLog = function (idUDS, skip, top, callback, error) {
            var url = this._configuration.ODATAUrl.concat("/UDSLogService.GetMyLogs(idUDS=", idUDS, ",skip=", skip.toString(), ",top=", top.toString(), ")?$count=true");
            this.getRequest(url, undefined, function (response) {
                if (callback) {
                    var mapper_3 = new UDSLogViewModelMapper();
                    var UDSLogs_3 = [];
                    var responseModel = new ODATAResponseModel(response);
                    if (response) {
                        $.each(response.value, function (i, value) {
                            UDSLogs_3.push(mapper_3.Map(value));
                        });
                        responseModel.value = UDSLogs_3;
                        callback(responseModel);
                    }
                }
            }, error);
        };
        return UDSLogService;
    }(BaseService));
    return UDSLogService;
});
//# sourceMappingURL=UDSLogService.js.map