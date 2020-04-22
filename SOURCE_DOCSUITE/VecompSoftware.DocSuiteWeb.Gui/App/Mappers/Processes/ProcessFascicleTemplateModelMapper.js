define(["require", "exports", "App/Mappers/Processes/ProcessModelMapper", "App/Mappers/Dossiers/DossierFolderModelMapper"], function (require, exports, ProcessModelMapper, DossierFolderModelMapper) {
    var ProcessFascicleTemplateModelMapper = /** @class */ (function () {
        function ProcessFascicleTemplateModelMapper() {
        }
        ProcessFascicleTemplateModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.UniqueId = source.UniqueId;
            toMap.Process = source.Process ? new ProcessModelMapper().Map(source.Process) : null;
            toMap.DossierFolder = source.DossierFolder ? new DossierFolderModelMapper().Map(source.DossierFolder) : null;
            toMap.Name = source.Name;
            toMap.JsonModel = source.JsonModel;
            toMap.StartDate = source.StartDate;
            toMap.EndDate = source.EndDate;
            toMap.RegistrationUser = source.RegistrationUser;
            toMap.RegistrationDate = source.RegistrationDate;
            toMap.LastChangedUser = source.LastChangedUser;
            toMap.LastChangedDate = source.LastChangedDate;
            return toMap;
        };
        return ProcessFascicleTemplateModelMapper;
    }());
    return ProcessFascicleTemplateModelMapper;
});
//# sourceMappingURL=ProcessFascicleTemplateModelMapper.js.map