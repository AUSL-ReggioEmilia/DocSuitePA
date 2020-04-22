using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Securities;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Securities
{
    public class DomainUserModelCollaborationUserModelMapper : BaseModelMapper<DomainUserModel, CollaborationUserModel>, IDomainUserModelCollaborationUserModelMapper
    {
        public override CollaborationUserModel Map(DomainUserModel entity, CollaborationUserModel entityTransformed)
        {
            #region [ Base ]
            entityTransformed.DestinationFirst = true;
            entityTransformed.DestinationEmail = entity.EmailAddress;
            entityTransformed.DestinationName = entity.DisplayName;
            entityTransformed.DestinationType = "P";
            #endregion

            return entityTransformed;
        }
    }
}
