using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Entities.Monitors;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.DocumentSeries
{
    public class MonitoringSeriesRoleFinder : BaseWebAPIFinder<Entity.DocumentArchives.DocumentSeries, MonitoringSeriesRoleModel>
    {
        #region [ Constructor ]

        public MonitoringSeriesRoleFinder(TenantModel tenant)
         : this(new List<TenantModel>() { tenant })
        { }

        public MonitoringSeriesRoleFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
        }

        #endregion

        #region [ Properties ]

        public DateTimeOffset? DateFrom { get; set; }
        public DateTimeOffset? DateTo { get; set; }

        #endregion

        #region [ Methods ]

        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            string FX_GetMonitoringSeriesRole = string.Format(
                CommonDefinition.OData.DocumentSeriesService.FX_GetMonitoringSeriesRole,
                DateFrom.Value.ToString(ODataDateConversion),
                DateTo.Value.ToString(ODataDateConversion)
                );
            odataQuery = odataQuery.Function(FX_GetMonitoringSeriesRole);
            return odataQuery;
        }

        public override void ResetDecoration()
        {
            DateFrom = null;
            DateTo = null;
        }

        #endregion
    }
}
