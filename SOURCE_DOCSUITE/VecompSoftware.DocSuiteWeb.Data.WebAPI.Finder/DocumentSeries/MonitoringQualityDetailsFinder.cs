using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Entities.Monitors;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.DocumentSeries
{
    public class MonitoringQualityDetailsFinder : BaseWebAPIFinder<Entity.DocumentArchives.DocumentSeries, MonitoringQualityDetailsModel>
    {
        #region [ Constructor ]

        public MonitoringQualityDetailsFinder(TenantModel tenant)
         : this(new List<TenantModel>() { tenant })
        { }

        public MonitoringQualityDetailsFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
        }

        #endregion

        #region [ Properties ]

        public DateTimeOffset? DateFrom { get; set; }
        public DateTimeOffset? DateTo { get; set; }
        public string IdDocumentSeries { get; set; }
        public string IdRole { get; set; }

        #endregion

        #region [ Methods ]

        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            odataQuery = odataQuery.Function(string.Format(
                CommonDefinition.OData.DocumentSeriesService.FX_GetMonitoringQualityDetails,
                IdDocumentSeries,
                IdRole,
                DateFrom.Value.ToString(ODataDateConversion),
                DateTo.Value.ToString(ODataDateConversion)
                ));
            return odataQuery;
        }

        public override void ResetDecoration()
        {
            DateFrom = null;
            DateTo = null;
            IdDocumentSeries = null;
            IdRole = null;
        }

        #endregion
    }
}
