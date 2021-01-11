using VecompSoftware.DocSuiteWeb.Model.Entities.Messages;
using VecompSoftware.DocSuiteWeb.Model.Securities;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Securities
{
    public class DomainUserModelMessageContactEmailModelMapper : BaseModelMapper<DomainUserModel, MessageContactEmailModel>, IDomainUserModelMessageContactEmailModelMapper
    {
        public override MessageContactEmailModel Map(DomainUserModel entity, MessageContactEmailModel entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Description = entity.DisplayName;
            entityTransformed.Email = entity.EmailAddress;
            entityTransformed.User = entity.Description;
            #endregion

            return entityTransformed;
        }
    }
}
