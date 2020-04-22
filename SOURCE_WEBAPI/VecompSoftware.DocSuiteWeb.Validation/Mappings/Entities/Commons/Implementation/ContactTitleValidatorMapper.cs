using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons
{
    public class ContactTitleValidatorMapper : BaseMapper<ContactTitle, ContactTitleValidator>, IContactTitleValidatorMapper
    {
        public ContactTitleValidatorMapper() { }

        public override ContactTitleValidator Map(ContactTitle entity, ContactTitleValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.EntityId = entity.EntityId;
            entityTransformed.isActive = entity.isActive;
            entityTransformed.Description = entity.Description;
            entityTransformed.Code = entity.Code;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Contacts = entity.Contacts;
            entityTransformed.ProtocolContactManuals = entity.ProtocolContactManuals;
            #endregion

            return entityTransformed;
        }

    }
}
