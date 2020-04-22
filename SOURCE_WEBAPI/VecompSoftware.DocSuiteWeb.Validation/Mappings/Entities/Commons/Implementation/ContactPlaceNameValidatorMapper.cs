using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons
{
    public class ContactPlaceNameValidatorMapper : BaseMapper<ContactPlaceName, ContactPlaceNameValidator>, IContactPlaceNameValidatorMapper
    {
        public ContactPlaceNameValidatorMapper() { }

        public override ContactPlaceNameValidator Map(ContactPlaceName entity, ContactPlaceNameValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.EntityShortId = entity.EntityShortId;
            entityTransformed.Description = entity.Description;
            entityTransformed.UniqueId = entity.UniqueId;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Contacts = entity.Contacts;
            entityTransformed.ProtocolContactManuals = entity.ProtocolContactManuals;
            #endregion

            return entityTransformed;
        }

    }
}
