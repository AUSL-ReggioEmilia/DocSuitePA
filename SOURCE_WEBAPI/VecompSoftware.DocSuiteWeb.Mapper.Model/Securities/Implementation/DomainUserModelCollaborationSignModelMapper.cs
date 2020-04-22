using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Securities;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Securities
{
    public class DomainUserModelCollaborationSignModelMapper : BaseModelMapper<DomainUserModel, CollaborationSignModel>, IDomainUserModelCollaborationSignModelMapper
    {
        public override CollaborationSignModel Map(DomainUserModel entity, CollaborationSignModel entityTransformed)
        {
            #region [ Base ]
            entityTransformed.IsActive = true;
            entityTransformed.IsRequired = false;
            entityTransformed.SignUser = entity.Name;
            entityTransformed.SignName = entity.DisplayName;
            entityTransformed.SignEmail = entity.EmailAddress;
            #endregion

            return entityTransformed;
        }
    }
}
