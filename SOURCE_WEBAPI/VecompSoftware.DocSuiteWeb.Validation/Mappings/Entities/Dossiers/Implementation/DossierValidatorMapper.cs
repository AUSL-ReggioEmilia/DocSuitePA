using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Dossiers
{
    public class DossierValidatorMapper : BaseMapper<Dossier, DossierValidator>, IDossierValidatorMapper
    {
        public DossierValidatorMapper() { }

        public override DossierValidator Map(Dossier entity, DossierValidator entityTransformed)
        {
            #region [Base]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Year = entity.Year;
            entityTransformed.Number = entity.Number;
            entityTransformed.StartDate = entity.StartDate;
            entityTransformed.Subject = entity.Subject;
            entityTransformed.EndDate = entity.EndDate;
            entityTransformed.Note = entity.Note;
            entityTransformed.JsonMetadata = entity.JsonMetadata;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Container = entity.Container;
            entityTransformed.Contacts = entity.Contacts;
            entityTransformed.DossierLogs = entity.DossierLogs;
            entityTransformed.Messages = entity.Messages;
            entityTransformed.DossierDocuments = entity.DossierDocuments;
            entityTransformed.DossierRoles = entity.DossierRoles;
            entityTransformed.DossierComments = entity.DossierComments;
            entityTransformed.DossierFolders = entity.DossierFolders;
            entityTransformed.WorkflowInstances = entity.WorkflowInstances;
            entityTransformed.DossierLinks = entity.DossierLinks;
            entityTransformed.MetadataRepository = entity.MetadataRepository;
            entityTransformed.Processes = entity.Processes;
            #endregion

            return entityTransformed;
        }
    }
}
