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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/PECMails/PECMailBoxViewModelMapper", "App/Models/PECMails/InvoiceTypeEnum", "App/Models/ODATAResponseModel"], function (require, exports, BaseService, PECMailBoxViewModelMapper, InvoiceTypeEnum, ODATAResponseModel) {
    var PECMailBoxService = /** @class */ (function (_super) {
        __extends(PECMailBoxService, _super);
        function PECMailBoxService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        PECMailBoxService.prototype.getPECMailBoxes = function (searchFilter, callback, error) {
            var urlPart = this._configuration.ODATAUrl;
            var url = searchFilter === ""
                ? urlPart.concat("?$filter=IncomingServer ne null and OutgoingServer ne null")
                : urlPart.concat("?$filter=indexof(MailBoxRecipient,'" + searchFilter + "') gt -1 and IncomingServer ne null and OutgoingServer ne null");
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var viewModelMapper_1 = new PECMailBoxViewModelMapper();
                    var pecMailBoxes_1 = [];
                    $.each(response.value, function (i, value) {
                        pecMailBoxes_1.push(viewModelMapper_1.Map(value));
                    });
                    callback(pecMailBoxes_1);
                }
                ;
            }, error);
        };
        PECMailBoxService.prototype.getFilteredPECMailBoxes = function (callback, error) {
            var urlPart = this._configuration.ODATAUrl;
            // get pec mail boxes only with invoice type InvoicePA or InvoicePR
            var url = urlPart.concat("?$filter= InvoiceType eq VecompSoftware.DocSuiteWeb.Entity.PECMails.InvoiceType'" + InvoiceTypeEnum[InvoiceTypeEnum.InvoicePA] + "' \n                                          or InvoiceType eq VecompSoftware.DocSuiteWeb.Entity.PECMails.InvoiceType'" + InvoiceTypeEnum[InvoiceTypeEnum.InvoicePR] + "'");
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var viewModelMapper_2 = new PECMailBoxViewModelMapper();
                    var pecMailBoxes_2 = [];
                    $.each(response.value, function (i, value) {
                        pecMailBoxes_2.push(viewModelMapper_2.Map(value));
                    });
                    callback(pecMailBoxes_2);
                }
                ;
            }, error);
        };
        PECMailBoxService.prototype.updatePECMailBox = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.putRequest(url, JSON.stringify(model), callback, error);
        };
        PECMailBoxService.prototype.insertPECMailBox = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.postRequest(url, JSON.stringify(model), callback, error);
        };
        PECMailBoxService.prototype.getPECMailBoxById = function (pecMailBoxId, callback, error) {
            var url = this._configuration.ODATAUrl.concat("?$expand=Location&$filter=EntityShortId eq " + pecMailBoxId);
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var viewModelMapper_3 = new PECMailBoxViewModelMapper();
                    var pecMailBoxes_3 = [];
                    $.each(response.value, function (i, value) {
                        pecMailBoxes_3.push(viewModelMapper_3.Map(value));
                    });
                    callback(pecMailBoxes_3);
                }
                ;
            }, error);
        };
        PECMailBoxService.prototype.getAllPECMailBoxes = function (name, topElement, skipElement, callback, error) {
            var url = this._configuration.ODATAUrl;
            var qs = "$filter=contains(MailBoxRecipient,'" + name + "') and IncomingServer ne null and OutgoingServer ne null&$count=true&$top=" + topElement + "&$skip=" + skipElement.toString();
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    var responseModel = new ODATAResponseModel(response);
                    var mapper = new PECMailBoxViewModelMapper();
                    responseModel.value = mapper.MapCollection(response.value);
                    ;
                    callback(responseModel);
                }
            }, error);
        };
        return PECMailBoxService;
    }(BaseService));
    return PECMailBoxService;
});
//# sourceMappingURL=PECMailBoxService.js.map