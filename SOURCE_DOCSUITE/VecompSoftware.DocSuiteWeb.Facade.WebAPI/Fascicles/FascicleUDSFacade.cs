using System.Linq;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using System;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;

namespace VecompSoftware.DocSuiteWeb.Facade.WebAPI.Fascicles
{
    public class FascicleUDSFacade : FacadeWebAPIBase<FascicleUDS, FascicleUDSDao>
    {
        #region [Constructor]
        public FascicleUDSFacade(ICollection<TenantModel> model)
            : base(model.Select(s => new WebAPITenantConfiguration<FascicleUDS, FascicleUDSDao>(s)).ToList())
        {
        }
        #endregion

        #region [Methods]
        public WebAPIDto<FascicleUDS> GetFascicolatedIdUDS(Guid idUDS)
        {
            return Query(string.Format("$filter=IdUDS eq {0} and ReferenceType eq 'Fascicle'", idUDS)).FirstOrDefault();
        }
        #endregion
    }
}
