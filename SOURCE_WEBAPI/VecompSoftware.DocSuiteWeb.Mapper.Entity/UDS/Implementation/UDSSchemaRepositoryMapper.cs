using VecompSoftware.DocSuiteWeb.Entity.UDS;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.UDS
{
    public class UDSSchemaRepositoryMapper : BaseEntityMapper<UDSSchemaRepository, UDSSchemaRepository>, IUDSSchemaRepositoryMapper
    {
        public override UDSSchemaRepository Map(UDSSchemaRepository entity, UDSSchemaRepository entityTransformed)
        {
            #region [ Base ]

            entityTransformed.ActiveDate = entity.ActiveDate;
            entityTransformed.ExpiredDate = entity.ExpiredDate;
            entityTransformed.SchemaXML = entity.SchemaXML;
            entityTransformed.Version = entity.Version;

            #endregion

            return entityTransformed;
        }


    }
}
