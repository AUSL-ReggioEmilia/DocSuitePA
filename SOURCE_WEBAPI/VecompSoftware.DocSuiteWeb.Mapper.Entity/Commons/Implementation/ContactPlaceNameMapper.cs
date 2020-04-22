using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Commons
{
    public class ContactPlaceNameMapper : BaseEntityMapper<ContactPlaceName, ContactPlaceName>, IContactPlaceNameMapper
    {
        public ContactPlaceNameMapper()
        {

        }

        public override ContactPlaceName Map(ContactPlaceName entity, ContactPlaceName entityTransformed)
        {
            #region [ Base ]

            entityTransformed.Description = entity.Description;

            #endregion

            return entityTransformed;
        }

    }
}
