using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Commons
{
    public class ContactTitleMapper : BaseEntityMapper<ContactTitle, ContactTitle>, IContactTitleMapper
    {
        public ContactTitleMapper()
        {

        }

        public override ContactTitle Map(ContactTitle entity, ContactTitle entityTransformed)
        {
            #region [ Base ]
            entityTransformed.isActive = entity.isActive;
            entityTransformed.Description = entity.Description;
            entityTransformed.Code = entity.Code;

            #endregion

            return entityTransformed;
        }

    }
}
