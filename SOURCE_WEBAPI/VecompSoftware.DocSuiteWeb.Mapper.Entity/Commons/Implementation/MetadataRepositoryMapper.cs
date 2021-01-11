using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Commons
{
    public class MetadataRepositoryMapper : BaseEntityMapper<MetadataRepository, MetadataRepository>, IMetadataRepositoryMapper
    {
        public MetadataRepositoryMapper()
        {
        }

        public override MetadataRepository Map(MetadataRepository entity, MetadataRepository entityTransformed)
        {
            #region [ Base ]
            entityTransformed.JsonMetadata = entity.JsonMetadata;
            entityTransformed.Name = entity.Name;
            entityTransformed.Status = entity.Status;
            entityTransformed.Version = entity.Version;
            entityTransformed.DateFrom = entity.DateFrom;
            entityTransformed.DateTo = entity.DateTo;
            #endregion

            return entityTransformed;
        }
    }
}
