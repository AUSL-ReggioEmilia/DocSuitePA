define(["require", "exports", "App/ViewModels/BaseEntityViewModel"], function (require, exports, BaseEntityViewModel) {
    var DossierSummaryContactViewModelMapper = /** @class */ (function () {
        function DossierSummaryContactViewModelMapper() {
        }
        DossierSummaryContactViewModelMapper.prototype.Map = function (source) {
            var toMap = new BaseEntityViewModel();
            if (!source) {
                return null;
            }
            toMap.UniqueId = source.UniqueId;
            toMap.EntityShortId = source.IdContact;
            toMap.Name = source.Description;
            toMap.Type = source.ContactType;
            return toMap;
        };
        return DossierSummaryContactViewModelMapper;
    }());
    return DossierSummaryContactViewModelMapper;
});
//# sourceMappingURL=DossierSummaryContactViewModelMapper.js.map