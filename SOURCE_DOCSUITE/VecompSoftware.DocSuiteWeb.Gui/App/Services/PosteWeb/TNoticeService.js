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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/PosteWeb/POLRequestModelMapper", "App/Mappers/PosteWeb/POLRequestSummaryMapper"], function (require, exports, BaseService, POLRequestModelMapper, POLRequestSummaryMapper) {
    var TNoticeService = /** @class */ (function (_super) {
        __extends(TNoticeService, _super);
        function TNoticeService(configuration) {
            var _this_1 = _super.call(this) || this;
            _this_1._configuration = configuration;
            return _this_1;
        }
        TNoticeService.prototype.getRequestByRequestId = function (requestId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var query = "/POLRequestService.GetPOLRequest(uniqueId=@p0)?@p0=" + requestId;
            url = "" + url + query;
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var mapper = new POLRequestModelMapper();
                    var _polRequest = mapper.Map(response.value[0]);
                    if (_polRequest === null) {
                        return;
                    }
                    callback(_polRequest);
                }
            }, error);
        };
        TNoticeService.prototype.countRequestsSummariesByDocumentId = function (documentUnitId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "/$count?$filter=DocumentUnit/UniqueId eq ".concat(documentUnitId);
            url = url.concat(data);
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response);
                }
            }, error);
        };
        TNoticeService.prototype.getRequestsSummariesByDocumentId = function (documentUnitId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var query = "/POLRequestService.GetPOLRequestsByDocumentUnitId(documentUnitId=@p0)?@p0=" + documentUnitId;
            url = "" + url + query;
            var _this = this;
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var summaries_1 = [];
                    $.each(response.value, function (i, value) {
                        var mapper = new POLRequestSummaryMapper();
                        var summary = mapper.Map(value);
                        summaries_1.push(summary);
                    });
                    callback(summaries_1);
                }
            }, error);
        };
        return TNoticeService;
    }(BaseService));
    return TNoticeService;
});
//# sourceMappingURL=TNoticeService.js.map