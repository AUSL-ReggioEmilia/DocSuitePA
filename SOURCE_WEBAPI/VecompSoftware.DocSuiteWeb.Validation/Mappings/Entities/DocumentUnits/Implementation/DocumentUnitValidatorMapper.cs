using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.DocumentUnits
{
    public class DocumentUnitValidatorMapper : BaseMapper<DocumentUnit, DocumentUnitValidator>, IDocumentUnitValidatorMapper
    {
        public DocumentUnitValidatorMapper() { }

        public override DocumentUnitValidator Map(DocumentUnit entity, DocumentUnitValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.EntityId = entity.EntityId;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.DocumentUnitName = entity.DocumentUnitName;
            entityTransformed.Year = entity.Year;
            entityTransformed.Number = entity.Number;
            entityTransformed.Subject = entity.Subject;
            entityTransformed.Title = entity.Title;
            entityTransformed.Status = entity.Status;
            entityTransformed.Environment = entity.Environment;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Container = entity.Container;
            entityTransformed.Category = entity.Category;
            entityTransformed.FascicleDocumentUnits = entity.FascicleDocumentUnits;
            entityTransformed.UDSRepository = entity.UDSRepository;
            entityTransformed.Fascicle = entity.Fascicle;
            entityTransformed.DocumentUnitChains = entity.DocumentUnitChains;
            entityTransformed.DocumentUnitRoles = entity.DocumentUnitRoles;
            entityTransformed.DocumentUnitUsers = entity.DocumentUnitUsers;
            entityTransformed.TransparentAdministrationMonitorLogs = entity.TransparentAdministrationMonitorLogs;
            entityTransformed.DocumentUnitFascicleHistoricizedCategories = entity.DocumentUnitFascicleHistoricizedCategories;
            entityTransformed.DocumentUnitFascicleCategories = entity.DocumentUnitFascicleCategories;
            entityTransformed.UDSDocumentUnits = entity.UDSDocumentUnits;
            #endregion

            return entityTransformed;
        }

    }
}
