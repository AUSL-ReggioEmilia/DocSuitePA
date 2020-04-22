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
define(["require", "exports", "../BaseService"], function (require, exports, BaseService) {
    var BiblosDocumentsService = /** @class */ (function (_super) {
        __extends(BiblosDocumentsService, _super);
        /**
         * Costruttore
         */
        function BiblosDocumentsService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        BiblosDocumentsService.prototype.getDocumentsByChainId = function (id, callback, error) {
            var url = this._configuration.ODATAUrl;
            url = url.concat("/DocumentUnitService.GetDocumentsByArchiveChain(idArchiveChain=", id, ")");
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response.value);
                }
            }, error);
        };
        return BiblosDocumentsService;
    }(BaseService));
    return BiblosDocumentsService;
});
//# sourceMappingURL=BiblosDocumentsService.js.map