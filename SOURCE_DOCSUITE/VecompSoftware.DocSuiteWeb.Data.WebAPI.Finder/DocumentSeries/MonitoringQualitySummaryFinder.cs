using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Entities.Monitors;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.DocumentSeries
{
    public class MonitoringQualitySummaryFinder : BaseWebAPIFinder<Entity.DocumentArchives.DocumentSeries, MonitoringQualitySummaryModel>
    {
        #region [ Constructor ]

        public MonitoringQualitySummaryFinder(TenantModel tenant)
         : this(new List<TenantModel>() { tenant })
        { }

        public MonitoringQualitySummaryFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
        }

        #endregion

        #region [ Properties ]

        public DateTimeOffset? DateFrom { get; set; }
        public DateTimeOffset? DateTo { get; set; }
        public string IdDocumentSeries { get; set; }
        public string RoleName { get; set; }
        public string DocumentSeries { get; set; }
        
        #endregion

        #region [ Methods ]

        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            string FX_GetMonitoringQualitySummary = string.Format(
                CommonDefinition.OData.DocumentSeriesService.FX_GetMonitoringQualitySummary,
                DateFrom.Value.ToString(ODataDateConversion),
                DateTo.Value.ToString(ODataDateConversion),
                IdDocumentSeries
                );
            if (string.IsNullOrEmpty(IdDocumentSeries))
            {
                FX_GetMonitoringQualitySummary = FX_GetMonitoringQualitySummary.Replace(",idDocumentSeries=", "");
            }
            if (!string.IsNullOrEmpty(RoleName))
            {
                FX_GetMonitoringQualitySummary = string.Concat(FX_GetMonitoringQualitySummary, "?$filter=Role eq '", RoleName, "'");
            }
            if (!string.IsNullOrEmpty(DocumentSeries))
            {
                FX_GetMonitoringQualitySummary = string.Concat(FX_GetMonitoringQualitySummary, "?$filter=DocumentSeries eq '", DocumentSeries, "'");
            }
            odataQuery = odataQuery.Function(FX_GetMonitoringQualitySummary);
            return odataQuery;
        }

        public override void ResetDecoration()
        {
            DateFrom = null;
            DateTo = null;
            IdDocumentSeries = null;
            DocumentSeries = string.Empty;
            RoleName = string.Empty;
        }

        #endregion
    }
}
