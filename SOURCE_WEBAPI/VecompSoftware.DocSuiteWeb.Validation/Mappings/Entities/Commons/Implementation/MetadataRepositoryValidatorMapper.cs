using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons
{
    public class MetadataRepositoryValidatorMapper : BaseMapper<MetadataRepository, MetadataRepositoryValidator>, IMetadataRepositoryValidatorMapper
    {
        public MetadataRepositoryValidatorMapper() { }

        public override MetadataRepositoryValidator Map(MetadataRepository entity, MetadataRepositoryValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.JsonMetadata = entity.JsonMetadata;
            entityTransformed.Name = entity.Name;
            entityTransformed.Status = entity.Status;
            entityTransformed.Version = entity.Version;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.DateFrom = entity.DateFrom;
            entityTransformed.DateTo = entity.DateTo;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Fascicles = entity.Fascicles;
            entityTransformed.Categories = entity.Categories;
            entityTransformed.Dossiers = entity.Dossiers;
            #endregion

            return entityTransformed;

        }
    }
}
