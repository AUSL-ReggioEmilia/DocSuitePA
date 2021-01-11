using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.EntityMapper.WebAPI;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;

namespace VecompSoftware.DocSuiteWeb.Facade.WebAPI.Commons
{
    public class RoleUserFacade : FacadeWebAPIBase<RoleUser, RoleUserDao>
    {
        #region [ Fields ]
        private readonly WebAPIDtoMapper<RoleUser> _mapper;
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Contructor ]
        public RoleUserFacade(ICollection<TenantModel> model, Tenant currentTenant)
            : base(model.Select(s => new WebAPITenantConfiguration<RoleUser, RoleUserDao>(s)).ToList(), currentTenant)
        {
            this._mapper = new WebAPIDtoMapper<RoleUser>();
        }
        #endregion
    }
}
