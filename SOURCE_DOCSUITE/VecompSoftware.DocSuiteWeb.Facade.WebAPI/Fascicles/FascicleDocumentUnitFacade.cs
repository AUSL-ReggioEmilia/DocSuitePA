using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Fascicles;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Model.Parameters;

namespace VecompSoftware.DocSuiteWeb.Facade.WebAPI.Fascicles
{
    public class FascicleDocumentUnitFacade : FacadeWebAPIBase<FascicleDocumentUnit, FascicleDocumentUnitDao>
    {
        #region [Constructor]
        public FascicleDocumentUnitFacade(ICollection<TenantModel> model, Tenant currentTenant)
            : base(model.Select(s => new WebAPITenantConfiguration<FascicleDocumentUnit, FascicleDocumentUnitDao>(s)).ToList(), currentTenant)
        {
        }
        #endregion

        #region [Methods]
        private void GenerateFascicleReference(Fascicle fascicle, Guid uniqueIdProtocol)
        {
            if (fascicle != null && this.CurrentTenantConfiguration.Dao.FindProtocolInFascicle(uniqueIdProtocol, fascicle.UniqueId) == null)
            {
                FascicleDocumentUnit fascicleDocumentUnit = new FascicleDocumentUnit()
                {
                    DocumentUnit = new Entity.DocumentUnits.DocumentUnit() { UniqueId = uniqueIdProtocol },
                    Fascicle = fascicle,
                    ReferenceType = ReferenceType.Reference
                };
                Save(fascicleDocumentUnit);
            }
        }

        public void LinkFascicleReference(Guid currentIdProtocol, Guid linkedIdProtocol)
        {
            GenerateFascicleReference(CurrentTenantConfiguration.Dao.FindFascicleFromDocumentUnit(currentIdProtocol), linkedIdProtocol);
            GenerateFascicleReference(CurrentTenantConfiguration.Dao.FindFascicleFromDocumentUnit(linkedIdProtocol), currentIdProtocol);
        }
        public WebAPIDto<FascicleDocumentUnit> GetFascicolatedIdDocumentUnit(Guid idDocumentUnit)
        {
            return CurrentTenantConfiguration.Dao.FindFascicolatedFascicleDocumentUnit(idDocumentUnit);
        }
        #endregion
    }
}
