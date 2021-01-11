using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.MassimariScarto;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;
using VecompSoftware.DocSuiteWeb.Entity.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Model.Parameters;

namespace VecompSoftware.DocSuiteWeb.Facade.WebAPI.MassimariScarto
{
    public class MassimarioScartoFacade : FacadeWebAPIBase<MassimarioScarto, MassimarioScartoDao>
    {
        #region [Constructor ]
        public MassimarioScartoFacade(IReadOnlyCollection<TenantModel> model, Tenant currentTenant)
            : base(model.Select(s => new WebAPITenantConfiguration<MassimarioScarto, MassimarioScartoDao>(s)).ToList(), currentTenant)
        {
        }
        #endregion

        #region [ Methods ]
        public string GetFullName(Guid massimarioId)
        {
            WebAPIDto<MassimarioScarto> currentMassimario = GetById(massimarioId);
            WebAPIDto<MassimarioScarto> parentMassimario = null;
            if (!string.IsNullOrEmpty(currentMassimario.Entity.MassimarioScartoParentPath))
            {
                parentMassimario = GetByPath(currentMassimario.Entity.MassimarioScartoParentPath);
            }

            string fullCode = string.Empty;
            fullCode = parentMassimario.Entity != null && parentMassimario.Entity.Code.HasValue ?
                string.Format("{0}.{1}", parentMassimario.Entity.Code, currentMassimario.Entity.Code)
                : currentMassimario.Entity.Code.Value.ToString();

            return string.Format("{0}.{1}({2})", fullCode, currentMassimario.Entity.Name, GetPeriodName(currentMassimario.Entity.ConservationPeriod.Value));
        }

        public WebAPIDto<MassimarioScarto> GetByPath(string path)
        {
            return CurrentTenantConfiguration.Dao.FindByPath(path);
        }

        public string GetPeriodName(short period)
        {
            switch (period)
            {
                case -1:
                    return "Illimitato";
                default:
                    return string.Concat(period, " Ann", period == 1 ? 'o' : 'i');
            }
        }
        #endregion
    }
}
