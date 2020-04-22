using System.Linq;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using System;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;

namespace VecompSoftware.DocSuiteWeb.Facade.WebAPI.Fascicles
{
    public class FascicleResolutionFacade : FacadeWebAPIBase<FascicleResolution, FascicleResolutionDao>
    {
        #region [Constructor]
        public FascicleResolutionFacade(ICollection<TenantModel> model)
            : base(model.Select(s => new WebAPITenantConfiguration<FascicleResolution, FascicleResolutionDao>(s)).ToList())
        {
        }
        #endregion

        #region [Methods]
        public WebAPIDto<FascicleResolution> GetFascicolatedIdResolution(Guid uniqueIdResolution)
        {
            return Query(string.Format("$filter=DocumentUnit/UniqueId eq {0} and ReferenceType eq 'Fascicle'", uniqueIdResolution)).FirstOrDefault();
        }
        #endregion
    }
}
