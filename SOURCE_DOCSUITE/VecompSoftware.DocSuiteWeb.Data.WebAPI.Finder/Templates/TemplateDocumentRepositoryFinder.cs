using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Templates
{
    public class TemplateDocumentRepositoryFinder : BaseWebAPIFinder<TemplateDocumentRepository, TemplateDocumentRepository>
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        public TemplateDocumentRepositoryFinder(TenantModel tenant)
           : this(new List<TenantModel>() { tenant })
        {
        }

        public TemplateDocumentRepositoryFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {

        }
        #endregion

        #region [ Methods ]
        public override void ResetDecoration()
        {
            UniqueId = null;
        }

        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            return base.DecorateFinder(odataQuery);
        }
        #endregion        
    }
}
