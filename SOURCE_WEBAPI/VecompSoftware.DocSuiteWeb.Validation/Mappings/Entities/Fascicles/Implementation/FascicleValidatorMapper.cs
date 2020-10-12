using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Fascicles
{
    public class FascicleValidatorMapper : BaseMapper<Fascicle, FascicleValidator>, IFascicleValidatorMapper
    {
        public FascicleValidatorMapper() { }

        public override FascicleValidator Map(Fascicle entity, FascicleValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Year = entity.Year;
            entityTransformed.Number = entity.Number;
            entityTransformed.Conservation = entity.Conservation;
            entityTransformed.StartDate = entity.StartDate;
            entityTransformed.EndDate = entity.EndDate;
            entityTransformed.Title = entity.Title;
            entityTransformed.Name = entity.Name;
            entityTransformed.FascicleObject = entity.FascicleObject;
            entityTransformed.Manager = entity.Manager;
            entityTransformed.Rack = entity.Rack;
            entityTransformed.Note = entity.Note;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.FascicleType = entity.FascicleType;
            entityTransformed.VisibilityType = entity.VisibilityType;
            entityTransformed.Timestamp = entity.Timestamp;
            entityTransformed.MetadataValues = entity.MetadataValues;
            entityTransformed.MetadataDesigner = entity.MetadataDesigner;
            entityTransformed.DSWEnvironment = entity.DSWEnvironment;
            entityTransformed.CustomActions = entity.CustomActions;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Category = entity.Category;
            entityTransformed.Container = entity.Container;
            entityTransformed.FascicleDocumentUnits = entity.FascicleDocumentUnits;
            entityTransformed.Contacts = entity.Contacts;
            entityTransformed.FascicleLogs = entity.FascicleLogs;
            entityTransformed.FascicleLinks = entity.FascicleLinks;
            entityTransformed.DocumentUnits = entity.DocumentUnits;
            entityTransformed.FascicleDocuments = entity.FascicleDocuments;
            entityTransformed.FascicleRoles = entity.FascicleRoles;
            entityTransformed.MetadataRepository = entity.MetadataRepository;
            entityTransformed.DossierFolders = entity.DossierFolders;
            entityTransformed.WorkflowInstances = entity.WorkflowInstances;
            entityTransformed.FascicleFolders = entity.FascicleFolders;
            entityTransformed.FascicleTemplate = entity.FascicleTemplate;
            entityTransformed.DocumentUnitFascicleCategories = entity.DocumentUnitFascicleCategories;
            entityTransformed.MetadataValueContacts = entity.MetadataValueContacts;
            entityTransformed.SourceMetadataValues = entity.SourceMetadataValues;
            #endregion

            return entityTransformed;
        }

    }
}
