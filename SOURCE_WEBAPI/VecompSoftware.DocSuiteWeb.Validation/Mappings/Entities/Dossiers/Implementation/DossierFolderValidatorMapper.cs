using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Dossiers
{
    public class DossierFolderValidatorMapper : BaseMapper<DossierFolder, DossierFolderValidator>, IDossierFolderValidatorMapper
    {
        public DossierFolderValidatorMapper() { }

        public override DossierFolderValidator Map(DossierFolder entity, DossierFolderValidator entityTransformed)
        {
            #region [Base]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Name = entity.Name;
            entityTransformed.Status = entity.Status;
            entityTransformed.JsonMetadata = entity.JsonMetadata;
            entityTransformed.DossierFolderLevel = entity.DossierFolderLevel;
            entityTransformed.DossierFolderPath = entity.DossierFolderPath;
            entityTransformed.ParentInsertId = entity.ParentInsertId;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Dossier = entity.Dossier;
            entityTransformed.Category = entity.Category;
            entityTransformed.Fascicle = entity.Fascicle;
            entityTransformed.DossierComments = entity.DossierComments;
            entityTransformed.DossierFolderRoles = entity.DossierFolderRoles;
            entityTransformed.DossierLogs = entity.DossierLogs;
            entityTransformed.FascicleTemplates = entity.FascicleTemplates;
            entityTransformed.FascicleWorkflowRepositories = entity.FascicleWorkflowRepositories; 
            #endregion

            return entityTransformed;
        }
    }
}
