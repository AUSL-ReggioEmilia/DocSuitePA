using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.UDS
{
    public class UDSSchemaRepositoryValidatorMapper : BaseMapper<UDSSchemaRepository, UDSSchemaRepositoryValidator>, IUDSSchemaRepositoryValidatorMapper
    {
        public UDSSchemaRepositoryValidatorMapper() { }

        public override UDSSchemaRepositoryValidator Map(UDSSchemaRepository entity, UDSSchemaRepositoryValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.ActiveDate = entity.ActiveDate;
            entityTransformed.ExpiredDate = entity.ExpiredDate;
            entityTransformed.SchemaXML = entity.SchemaXML;
            entityTransformed.Version = entity.Version;
            #endregion

            return entityTransformed;
        }
    }
}
