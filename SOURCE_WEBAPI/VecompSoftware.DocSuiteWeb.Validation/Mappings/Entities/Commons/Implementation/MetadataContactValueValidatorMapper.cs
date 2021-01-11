using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons
{
    public class MetadataContactValueValidatorMapper : BaseMapper<MetadataValueContact, MetadataValueContactValidator>, IMetadataContactValueValidatorMapper
    {
        public MetadataContactValueValidatorMapper() { }

        public override MetadataValueContactValidator Map(MetadataValueContact entity, MetadataValueContactValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Name = entity.Name;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.ContactManual = entity.ContactManual;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Fascicle = entity.Fascicle;
            entityTransformed.Contact = entity.Contact;
            entityTransformed.Dossier = entity.Dossier;
            #endregion

            return entityTransformed;

        }
    }
}
