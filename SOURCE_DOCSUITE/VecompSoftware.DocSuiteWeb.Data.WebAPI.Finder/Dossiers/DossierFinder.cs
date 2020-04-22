using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Dossiers
{
    public class DossierFinder : BaseWebAPIFinder<Dossier, DossierModel>
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]
        public bool ExpandProperties { get; set; }
        public Guid DossierId { get; set; }
        #endregion

        #region [ Constructor ]

        public DossierFinder(TenantModel tenant)
           : this(new List<TenantModel>() { tenant })
        {
        }

        public DossierFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
            DossierId = new Guid();
        }

        #endregion

        #region [ Methods ]
        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if (ExpandProperties)
            {
                odataQuery = odataQuery.Expand("DossierDocuments");
            }

            if (DossierId !=null && DossierId != Guid.Empty)
            {
                string expression = string.Format("UniqueId eq {0}", DossierId);
                string odataExpression = expression;
                odataQuery = odataQuery.Filter(odataExpression);
            }

            return base.DecorateFinder(odataQuery);
        }

        public override void ResetDecoration()
        {
            ExpandProperties = false;
            DossierId = new Guid();
        }

        #endregion
    }
}
