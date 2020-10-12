using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Commons
{
    public class ContactModelMapper : BaseModelMapper<Contact, ContactModel>, IContactModelMapper
    {

        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public ContactModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
        }
        #endregion

        public override ContactModel Map(Contact entity, ContactModel entityTransformed)
        {

            entityTransformed.Id = entity.EntityId;
            entityTransformed.EntityId = entity.EntityId;
            entityTransformed.Description = entity.Description;
            entityTransformed.Email = entity.EmailAddress;

            return entityTransformed;
        }

    }
}
