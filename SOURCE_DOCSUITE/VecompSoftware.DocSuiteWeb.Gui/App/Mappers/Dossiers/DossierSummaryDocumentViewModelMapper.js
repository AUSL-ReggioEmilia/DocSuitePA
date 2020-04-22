define(["require", "exports", "App/ViewModels/BaseEntityViewModel"], function (require, exports, BaseEntityViewModel) {
    var DossierSummaryDocumentViewModelMapper = /** @class */ (function () {
        function DossierSummaryDocumentViewModelMapper() {
        }
        DossierSummaryDocumentViewModelMapper.prototype.Map = function (source) {
            var toMap = new BaseEntityViewModel();
            if (!source) {
                return null;
            }
            toMap.UniqueId = source.UniqueId;
            toMap.IdArchiveChain = source.IdArchiveChain;
            return toMap;
        };
        return DossierSummaryDocumentViewModelMapper;
    }());
    return DossierSummaryDocumentViewModelMapper;
});
//# sourceMappingURL=DossierSummaryDocumentViewModelMapper.js.map