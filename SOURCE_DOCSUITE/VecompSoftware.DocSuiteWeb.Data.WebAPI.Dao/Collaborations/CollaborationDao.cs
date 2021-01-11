using System.Linq;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Dao;
using CollaborationEntities = VecompSoftware.DocSuiteWeb.Entity.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Collaborations
{
    public class CollaborationDao : BaseWebAPIDao<CollaborationEntities.Collaboration, CollaborationModel, CollaborationFinder>
    {
        #region [ Constructor ]

        public CollaborationDao(TenantModel tenant)
            : base(tenant.WebApiClientConfig, tenant.OriginalConfiguration, new CollaborationFinder(tenant))
        { }

        #endregion

        #region [ Methods ]

        public CollaborationEntities.Collaboration GetByIncremental(int incremental)
        {
            Finder.ResetDecoration();
            Finder.CollaborationFinderActionType = null;
            Finder.Incremental = incremental;
            return Finder.DoSearch().Select(s => s.Entity).SingleOrDefault();
        }

        #endregion
    }
}
