using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Dao;
using DocumentUnitEntities = VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.DocumentUnits
{
    public class DocumentUnitChainDao : BaseWebAPIDao<DocumentUnitEntities.DocumentUnitChain, DocumentUnitEntities.DocumentUnitChain, DocumentUnitChainFinder>
    {
        #region [ Constructor ]

        public DocumentUnitChainDao(TenantModel tenant)
            : base(tenant.WebApiClientConfig, tenant.OriginalConfiguration, new DocumentUnitChainFinder(tenant))
        {
        }

        #endregion

        #region [ Methods ]

        #endregion
    }
}
