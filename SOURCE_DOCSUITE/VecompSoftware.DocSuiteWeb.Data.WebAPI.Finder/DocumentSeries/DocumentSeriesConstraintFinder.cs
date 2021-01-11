using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.DocumentSeries
{
    public class DocumentSeriesConstraintFinder : BaseWebAPIFinder<DocumentSeriesConstraint, DocumentSeriesConstraint>
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]
        public int? IdSeries { get; set; }
        #endregion

        #region [ Constructor ]

        public DocumentSeriesConstraintFinder(TenantModel tenant)
           : this(new List<TenantModel>() { tenant })
        {
        }

        public DocumentSeriesConstraintFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
            
        }

        #endregion

        #region [ Methods ]
        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {            
            if (IdSeries.HasValue)
            {
                odataQuery = odataQuery.Filter(string.Concat("DocumentSeries/EntityId eq ", IdSeries.Value));
            }

            return base.DecorateFinder(odataQuery);
        }

        public override void ResetDecoration()
        {
                        
        }

        #endregion
    }
}
