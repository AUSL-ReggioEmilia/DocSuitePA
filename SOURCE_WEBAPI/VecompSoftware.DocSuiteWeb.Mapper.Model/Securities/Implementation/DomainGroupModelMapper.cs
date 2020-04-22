using System.DirectoryServices.AccountManagement;
using VecompSoftware.DocSuiteWeb.Model.Securities;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Securities
{
    public class DomainGroupModelMapper : BaseDomainModelMapper<Principal, DomainGroupModel>, IDomainGroupModelMapper
    {
        public override DomainGroupModel Map(Principal entity, DomainGroupModel entityTransformed)
        {
            return base.Map(entity, entityTransformed);
        }

    }
}
