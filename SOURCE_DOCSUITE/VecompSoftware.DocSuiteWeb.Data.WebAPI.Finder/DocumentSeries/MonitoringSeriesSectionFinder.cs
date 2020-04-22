using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Model.Entities.Monitors;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.DocumentSeries
{
    public class MonitoringSeriesSectionFinder : BaseWebAPIFinder<Entity.DocumentArchives.DocumentSeries, MonitoringSeriesSectionModel>
    {
        #region [ Constructor ]

        public MonitoringSeriesSectionFinder(TenantModel tenant)
         : this(new List<TenantModel>() { tenant })
        { }

        public MonitoringSeriesSectionFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
        }

        #endregion

        #region [ Properties ]

        public DateTimeOffset? DateFrom { get; set; }

        public DateTimeOffset? DateTo { get; set; }

        public string FamilyName { get; set; }

        public string SeriesName { get; set; }

        #endregion

        #region [ Methods ]

        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if (DateFrom.HasValue && DateTo.HasValue)
            {
                string sectionFunction = string.Format(
                CommonDefinition.OData.DocumentSeriesService.FX_GetMonitoringSeriesSection, DateFrom.Value.ToString(ODataDateConversion), DateTo.Value.ToString(ODataDateConversion));
                odataQuery = odataQuery.Function(sectionFunction);
            }

            if (!string.IsNullOrEmpty(FamilyName))
            {
                odataQuery = odataQuery.Filter(string.Concat("Family eq '", FamilyName, "'"));
            }

            if (!string.IsNullOrEmpty(SeriesName))
            {
                odataQuery = odataQuery.Filter(string.Concat("Series eq '", SeriesName, "'"));
            }

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
