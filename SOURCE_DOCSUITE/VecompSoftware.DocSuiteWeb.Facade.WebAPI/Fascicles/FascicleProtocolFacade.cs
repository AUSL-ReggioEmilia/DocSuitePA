using System.Linq;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using System;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;

namespace VecompSoftware.DocSuiteWeb.Facade.WebAPI.Fascicles
{
    public class FascicleProtocolFacade : FacadeWebAPIBase<FascicleProtocol, FascicleProtocolDao>
    {
        #region [Constructor]
        public FascicleProtocolFacade(ICollection<TenantModel> model)
            : base(model.Select(s => new WebAPITenantConfiguration<FascicleProtocol, FascicleProtocolDao>(s)).ToList())
        {
        }
        #endregion

        #region [Methods]
        private void GenerateFascicleReference(Fascicle fascicle, Guid uniqueIdProtocol)
        {
            if (fascicle != null && this.CurrentTenantConfiguration.Dao.FindProtocolInFascicle(uniqueIdProtocol, fascicle.UniqueId) == null)
            {
                FascicleProtocol fascicleProtocol = new FascicleProtocol()
                {
                    DocumentUnit = new Entity.Protocols.Protocol() { UniqueId = uniqueIdProtocol },
                    Fascicle = fascicle,
                    ReferenceType = ReferenceType.Reference
                };
                Save(fascicleProtocol);
            }
        }

        public void LinkFascicleReference(Guid currentIdProtocol, Guid linkedIdProtocol)
        {
            GenerateFascicleReference(this.CurrentTenantConfiguration.Dao.FindFascicleFromProtocol(currentIdProtocol), linkedIdProtocol);
            GenerateFascicleReference(this.CurrentTenantConfiguration.Dao.FindFascicleFromProtocol(linkedIdProtocol), currentIdProtocol);
        }

        public WebAPIDto<FascicleProtocol> GetFascicolatedUniqueIdProtocol(Guid uniqueIdProtocol)
        {
            return Query(string.Format("$filter=UniqueIdProtocol eq {0} and ReferenceType eq 'Fascicle'", uniqueIdProtocol)).FirstOrDefault();
        }
        #endregion
    }
}
