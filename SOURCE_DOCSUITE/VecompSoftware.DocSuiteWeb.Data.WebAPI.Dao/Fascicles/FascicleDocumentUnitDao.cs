using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Dao;
using FascicleEntities = VecompSoftware.DocSuiteWeb.Entity.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Fascicles
{
    public class FascicleDocumentUnitDao : BaseWebAPIDao<FascicleEntities.FascicleDocumentUnit, FascicleEntities.FascicleDocumentUnit, FascicleDocumentUnitFinder>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public FascicleDocumentUnitDao(TenantModel tenant)
            : base(tenant.WebApiClientConfig, tenant.OriginalConfiguration,new FascicleDocumentUnitFinder(tenant))
        {
        }

        #endregion

        #region [ Methods ]

        public ICollection<FascicleEntities.FascicleDocumentUnit> FindFascicleProtocols(Guid uniqueIdProtocol)
        {
            Finder.ResetDecoration();
            Finder.IdDocumentUnit = uniqueIdProtocol;
            Finder.ExpandFascicle = true;
            Finder.EnablePaging = false;
            return Finder.DoSearch().Select(f => f.Entity).ToList();
        }

        public FascicleEntities.FascicleDocumentUnit FindProtocolInFascicle(Guid uniqueIdProtocol, Guid fascicleId)
        {
            Finder.ResetDecoration();
            Finder.IdDocumentUnit = uniqueIdProtocol;
            Finder.IdFascicle = fascicleId;
            Finder.EnablePaging = false;
            return Finder.DoSearch().Select(f => f.Entity).SingleOrDefault();
        }

        public FascicleEntities.Fascicle FindFascicleFromDocumentUnit(Guid idDocumentUnit)
        {
            Finder.ResetDecoration();
            Finder.IdDocumentUnit = idDocumentUnit;
            Finder.ExpandFascicle = true;
            Finder.ReferenceType = FascicleEntities.ReferenceType.Fascicle;
            Finder.EnablePaging = false;
            return Finder.DoSearch().Select(f => f.Entity.Fascicle).SingleOrDefault();
        }

        public WebAPIDto<FascicleEntities.FascicleDocumentUnit> FindFascicolatedFascicleDocumentUnit(Guid idDocumentUnit)
        {
            Finder.ResetDecoration();
            Finder.IdDocumentUnit = idDocumentUnit;
            Finder.ReferenceType = FascicleEntities.ReferenceType.Fascicle;
            Finder.EnablePaging = false;
            return Finder.DoSearch().SingleOrDefault();
        }
        #endregion
    }
}
