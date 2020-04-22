using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.UDS
{
    public class UDSSchemaRepositoryModelMapper : BaseModelMapper<UDSSchemaRepository, UDSSchemaRepositoryModel>, IUDSSchemaRepositoryModelMapper
    {
        public override UDSSchemaRepositoryModel Map(UDSSchemaRepository entity, UDSSchemaRepositoryModel entityTransformed)
        {
            #region [ Base ]

            entityTransformed.ActiveDate = entity.ActiveDate;
            entityTransformed.ExpiredDate = entity.ExpiredDate;
            entityTransformed.SchemaXML = entity.SchemaXML;
            entityTransformed.Version = entity.Version;
            entityTransformed.Id = entity.UniqueId;

            #endregion

            #region [ Navigation Properties ]

            #endregion

            return entityTransformed;
        }

    }
}
