using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.UDS
{
    public class BaseUDSRelationFinder<T, THeader> : BaseWebAPIFinder<T, THeader> 
        where T: class 
        where THeader: class, new()
    {
        #region [ Constructor ]
        public BaseUDSRelationFinder(TenantModel tenant)
            : this(new List<TenantModel>() { tenant })
        {
        }

        public BaseUDSRelationFinder(IReadOnlyCollection<TenantModel> tenant)
            : base(tenant)
        {
        }
        #endregion

        #region [ Properties ]
        public Guid? IdUDS { get; set; }

        public bool ExpandRelation { get; set; }
        #endregion

        #region [ Methods ]
        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if (IdUDS.HasValue && IdUDS.Value != Guid.Empty)
            {
                string odataExpression = string.Format("IdUDS eq {0}", IdUDS.Value);
                odataQuery = odataQuery.Filter(odataExpression);
            }

            if (ExpandRelation)
            {
                odataQuery = odataQuery.Expand("Relation");
            }

            return odataQuery;
        }

        public override void ResetDecoration()
        {
            
        }
        #endregion
    }
}
