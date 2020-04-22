using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.UDS
{
    public class UDSRepositoryValidatorMapper : BaseMapper<UDSRepository, UDSRepositoryValidator>, IUDSRepositoryValidatorMapper
    {
        public UDSRepositoryValidatorMapper() { }

        public override UDSRepositoryValidator Map(UDSRepository entity, UDSRepositoryValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.ActiveDate = entity.ActiveDate;
            entityTransformed.ExpiredDate = entity.ExpiredDate;
            entityTransformed.ModuleXML = entity.ModuleXML;
            entityTransformed.Name = entity.Name;
            entityTransformed.SequenceCurrentNumber = entity.SequenceCurrentNumber;
            entityTransformed.SequenceCurrentYear = entity.SequenceCurrentYear;
            entityTransformed.Status = entity.Status;
            entityTransformed.Version = entity.Version;
            entityTransformed.Alias = entity.Alias;
            entityTransformed.DSWEnvironment = entity.DSWEnvironment;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Container = entity.Container;
            entityTransformed.SchemaRepository = entity.SchemaRepository;
            entityTransformed.DocumentUnits = entity.DocumentUnits;
            entityTransformed.PecMails = entity.PecMails;
            entityTransformed.UDSTypologies = entity.UDSTypologies;
            entityTransformed.UDSLogs = entity.UDSLogs;
            entityTransformed.UDSRoles = entity.UDSRoles;
            entityTransformed.UDSUsers = entity.UDSUsers;
            #endregion

            return entityTransformed;
        }
    }
}
