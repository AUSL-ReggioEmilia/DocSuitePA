using VecompSoftware.DocSuiteWeb.Entity.UDS;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.UDS
{
    public class UDSRepositoryMapper : BaseEntityMapper<UDSRepository, UDSRepository>, IUDSRepositoryMapper
    {
        public override UDSRepository Map(UDSRepository entity, UDSRepository entityTransformed)
        {
            #region [ Base ]

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

            return entityTransformed;
        }

    }
}
