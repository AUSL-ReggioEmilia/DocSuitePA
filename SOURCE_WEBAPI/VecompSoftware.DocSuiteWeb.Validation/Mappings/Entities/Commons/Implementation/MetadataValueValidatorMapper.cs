using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons
{
    public class MetadataValueValidatorMapper : BaseMapper<MetadataValue, MetadataValueValidator>, IMetadataValueValidatorMapper
    {
        public MetadataValueValidatorMapper() { }

        public override MetadataValueValidator Map(MetadataValue entity, MetadataValueValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Name = entity.Name;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.PropertyType = entity.PropertyType;
            entityTransformed.ValueString = entity.ValueString;
            entityTransformed.ValueInt = entity.ValueInt;
            entityTransformed.ValueGuid = entity.ValueGuid;
            entityTransformed.ValueDouble = entity.ValueDouble;
            entityTransformed.ValueDate = entity.ValueDate;
            entityTransformed.ValueBoolean = entity.ValueBoolean;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Fascicle = entity.Fascicle;
            entityTransformed.Dossier = entity.Dossier;
            #endregion

            return entityTransformed;

        }
    }
}
