using System.Linq;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using System;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;

namespace VecompSoftware.DocSuiteWeb.Facade.WebAPI.Fascicles
{
    public class FascicleDocumentSeriesItemFacade : FacadeWebAPIBase<FascicleDocumentSeriesItem, FascicleDocumentSeriesItemDao>
    {
        #region [Constructor]
        public FascicleDocumentSeriesItemFacade(ICollection<TenantModel> model)
            : base(model.Select(s => new WebAPITenantConfiguration<FascicleDocumentSeriesItem, FascicleDocumentSeriesItemDao>(s)).ToList())
        {
        }
        #endregion

        #region [Methods]
        public WebAPIDto<FascicleDocumentSeriesItem> GetFascicolatedIdDocumentSeriesItem(int idDocumentSeriesItem)
        {
            return Query(string.Format("$filter=DocumentUnit/UniqueId eq {0} and ReferenceType eq 'Fascicle'", idDocumentSeriesItem)).FirstOrDefault();
        }
        #endregion
    }
}
