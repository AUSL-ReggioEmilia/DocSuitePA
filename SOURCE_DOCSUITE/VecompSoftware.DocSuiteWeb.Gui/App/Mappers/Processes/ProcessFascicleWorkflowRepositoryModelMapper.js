define(["require", "exports", "App/Mappers/Processes/ProcessModelMapper", "App/Mappers/Dossiers/DossierFolderModelMapper", "App/Mappers/Workflows/WorkflowRepositoryModelMapper"], function (require, exports, ProcessModelMapper, DossierFolderModelMapper, WorkflowRepositoryModelMapper) {
    var ProcessFascicleWorkflowRepositoryModelMapper = /** @class */ (function () {
        function ProcessFascicleWorkflowRepositoryModelMapper() {
        }
        ProcessFascicleWorkflowRepositoryModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.UniqueId = source.UniqueId;
            toMap.Process = source.Process ? new ProcessModelMapper().Map(source.Process) : null;
            toMap.DossierFolder = source.DossierFolder ? new DossierFolderModelMapper().Map(source.DossierFolder) : null;
            toMap.WorkflowRepository = source.WorkflowRepository ? new WorkflowRepositoryModelMapper().Map(source.WorkflowRepository) : null;
            toMap.RegistrationUser = source.RegistrationUser;
            toMap.RegistrationDate = source.RegistrationDate;
            toMap.LastChangedUser = source.LastChangedUser;
            toMap.LastChangedDate = source.LastChangedDate;
            return toMap;
        };
        return ProcessFascicleWorkflowRepositoryModelMapper;
    }());
    return ProcessFascicleWorkflowRepositoryModelMapper;
});
//# sourceMappingURL=ProcessFascicleWorkflowRepositoryModelMapper.js.map