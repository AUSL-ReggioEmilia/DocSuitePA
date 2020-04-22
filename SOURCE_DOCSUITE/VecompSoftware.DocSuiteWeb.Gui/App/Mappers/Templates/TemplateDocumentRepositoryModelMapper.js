define(["require", "exports", "App/Models/Templates/TemplateDocumentRepositoryStatus"], function (require, exports, TemplateDocumentRepositoryStatus) {
    var TemplateDocumentRepositoryModelMapper = /** @class */ (function () {
        function TemplateDocumentRepositoryModelMapper() {
        }
        TemplateDocumentRepositoryModelMapper.prototype.Map = function (source) {
            var toMap = {};
            toMap.UniqueId = source.UniqueId;
            toMap.Status = this.mapStatus(source.Status);
            toMap.Name = source.Name;
            toMap.QualityTag = source.QualityTag;
            toMap.Version = source.Version;
            toMap.Object = source.Object;
            toMap.IdArchiveChain = source.IdArchiveChain;
            toMap.RegistrationUser = source.RegistrationUser;
            toMap.RegistrationDate = source.RegistrationDate;
            toMap.LastChangedUser = source.LastChangedUser;
            toMap.LastChangedDate = source.LastChangedDate;
            return toMap;
        };
        TemplateDocumentRepositoryModelMapper.prototype.mapStatus = function (status) {
            if (typeof (status) == "string") {
                return TemplateDocumentRepositoryStatus[status.toString()];
            }
            return status;
        };
        return TemplateDocumentRepositoryModelMapper;
    }());
    return TemplateDocumentRepositoryModelMapper;
});
//# sourceMappingURL=TemplateDocumentRepositoryModelMapper.js.map