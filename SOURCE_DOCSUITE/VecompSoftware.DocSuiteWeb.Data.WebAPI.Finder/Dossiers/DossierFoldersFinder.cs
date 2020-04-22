using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Dossiers
{
    public class DossierFoldersFinder : BaseWebAPIFinder<DossierFolder, DossierFolder>
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]
        public bool ExpandProperties { get; set; }
        public bool HasFascicles { get; set; }
        public Guid DossierId { get; set; }
        #endregion

        #region [ Constructor ]

        public DossierFoldersFinder(TenantModel tenant)
           : this(new List<TenantModel>() { tenant })
        {
        }

        public DossierFoldersFinder(IReadOnlyCollection<TenantModel> tenants)
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
                odataQuery = odataQuery.Expand("Fascicle($expand=FascicleDocuments($expand=FascicleFolder))");
            }

            if (DossierId != Guid.Empty)
            {
                string expression = string.Format("Dossier/UniqueId eq {0}", DossierId);
                string odataExpression = expression;
                odataQuery = odataQuery.Filter(odataExpression);
            }
            if (HasFascicles)
            {
                string expression = "Fascicle ne null";
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
