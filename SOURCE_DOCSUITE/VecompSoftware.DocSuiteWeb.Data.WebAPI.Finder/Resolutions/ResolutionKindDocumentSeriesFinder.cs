using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Resolutions
{
    public class ResolutionKindDocumentSeriesFinder : BaseWebAPIFinder<ResolutionKindDocumentSeries, ResolutionKindDocumentSeries>
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]
        public int? IdDocumentSeries { get; set; }
        public Guid? IdResolutionKind { get; set; }
        public bool ExpandProperties { get; set; }
        #endregion

        #region [ Constructor ]
        public ResolutionKindDocumentSeriesFinder(TenantModel tenant)
           : this(new List<TenantModel>() { tenant })
        {
        }

        public ResolutionKindDocumentSeriesFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
            
        }
        #endregion

        #region [ Methods ]
        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {     
            if (ExpandProperties)
            {
                odataQuery = odataQuery.Expand("DocumentSeriesConstraint");
            }

            if (IdDocumentSeries.HasValue)
            {
                odataQuery = odataQuery.Filter(string.Concat("DocumentSeries/EntityId eq ", IdDocumentSeries.Value));
            }

            if (IdResolutionKind.HasValue)
            {
                odataQuery = odataQuery.Filter(string.Concat("ResolutionKind/UniqueId eq ", IdResolutionKind.Value));
            }

            return base.DecorateFinder(odataQuery);
        }

        public override void ResetDecoration()
        {
            IdDocumentSeries = null;
            IdResolutionKind = null;
            ExpandProperties = false;
        }
        #endregion
    }
}
