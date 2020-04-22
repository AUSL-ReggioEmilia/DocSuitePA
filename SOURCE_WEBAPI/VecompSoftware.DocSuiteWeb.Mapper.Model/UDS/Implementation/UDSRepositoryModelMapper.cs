using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.UDS
{
    public class UDSRepositoryModelMapper : BaseModelMapper<UDSRepository, UDSRepositoryModel>, IUDSRepositoryModelMapper
    {

        #region [ Fields ]

        private readonly IUDSSchemaRepositoryModelMapper _udsSchemaRepositoryModelMapper;

        #endregion


        #region [ Constructor ]

        public UDSRepositoryModelMapper(IUDSSchemaRepositoryModelMapper udsSchemaRepositoryModelMapper)
        {
            _udsSchemaRepositoryModelMapper = udsSchemaRepositoryModelMapper;
        }

        #endregion

        #region [ Methods ]

        public static DocSuiteWeb.Model.Entities.UDS.UDSRepositoryStatus StatusConverter(Entity.UDS.UDSRepositoryStatus status)
        {
            switch (status)
            {
                case Entity.UDS.UDSRepositoryStatus.Draft:
                    return DocSuiteWeb.Model.Entities.UDS.UDSRepositoryStatus.Draft;
                case Entity.UDS.UDSRepositoryStatus.Confirmed:
                    return DocSuiteWeb.Model.Entities.UDS.UDSRepositoryStatus.Confirmed;
                default:
                    return DocSuiteWeb.Model.Entities.UDS.UDSRepositoryStatus.Draft;
            }
        }
        public static Entity.UDS.UDSRepositoryStatus StatusConverter(DocSuiteWeb.Model.Entities.UDS.UDSRepositoryStatus? status)
        {
            if (!status.HasValue)
            {
                return Entity.UDS.UDSRepositoryStatus.Draft;
            }

            switch (status.Value)
            {
                case DocSuiteWeb.Model.Entities.UDS.UDSRepositoryStatus.Draft:
                    return Entity.UDS.UDSRepositoryStatus.Draft;
                case DocSuiteWeb.Model.Entities.UDS.UDSRepositoryStatus.Confirmed:
                    return Entity.UDS.UDSRepositoryStatus.Confirmed;
                default:
                    return Entity.UDS.UDSRepositoryStatus.Draft;
            }
        }

        public override UDSRepositoryModel Map(UDSRepository entity, UDSRepositoryModel entityTransformed)
        {
            #region [ Base ]

            entityTransformed.ActiveDate = entity.ActiveDate;
            entityTransformed.ExpiredDate = entity.ExpiredDate;
            entityTransformed.Id = entity.UniqueId;
            entityTransformed.ModuleXML = entity.ModuleXML;
            entityTransformed.Name = entity.Name;
            entityTransformed.SequenceCurrentYear = entity.SequenceCurrentYear;
            entityTransformed.SequenceCurrentNumber = entity.SequenceCurrentNumber;
            entityTransformed.Status = StatusConverter(entity.Status);
            entityTransformed.Version = entity.Version;
            entityTransformed.Alias = entity.Alias;
            entityTransformed.DSWEnvironment = entity.DSWEnvironment;

            #endregion

            #region [ Navigation Properties ]
            entityTransformed.SchemaRepository = entity.SchemaRepository != null ? _udsSchemaRepositoryModelMapper.Map(entity.SchemaRepository, new UDSSchemaRepositoryModel()) : null;
            #endregion

            return entityTransformed;
        }

        #endregion

    }
}
