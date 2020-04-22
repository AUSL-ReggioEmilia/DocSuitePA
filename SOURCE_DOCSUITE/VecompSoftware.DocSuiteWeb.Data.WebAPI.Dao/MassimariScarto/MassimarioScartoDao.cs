using System.Linq;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.MassimariScarto;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;
using VecompSoftware.DocSuiteWeb.Model.Entities.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Dao;
using MassimarioScartoEntities = VecompSoftware.DocSuiteWeb.Entity.MassimariScarto;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.MassimariScarto
{
    public class MassimarioScartoDao : BaseWebAPIDao<MassimarioScartoEntities.MassimarioScarto, MassimarioScartoModel, MassimarioScartoFinder>
    {
        #region [ Constructor ]

        public MassimarioScartoDao(TenantModel tenant)
            : base(tenant.WebApiClientConfig, tenant.OriginalConfiguration, new MassimarioScartoFinder(tenant))
        { }

        #endregion

        #region [ Methods ]
        public WebAPIDto<MassimarioScartoEntities.MassimarioScarto> FindByPath(string path)
        {
            Finder.ResetDecoration();
            Finder.EnablePaging = false;
            Finder.Path = path;
            return Finder.DoSearch().SingleOrDefault();
        }
        #endregion
    }
}
