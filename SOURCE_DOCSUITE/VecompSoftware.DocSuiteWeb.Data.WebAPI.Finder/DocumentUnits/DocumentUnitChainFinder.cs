using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Finder;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.WebAPIManager;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.DocumentUnits
{
    public class DocumentUnitChainFinder : BaseWebAPIFinder<DocumentUnitChain, DocumentUnitChain>
    {
        #region [ Constructor ]

        public DocumentUnitChainFinder(TenantModel tenant)
         : this(new List<TenantModel>() { tenant })
        { }

        public DocumentUnitChainFinder(IReadOnlyCollection<TenantModel> tenants) 
            : base(tenants)
        {
        }

        #endregion

        #region [ Properties ]

        public Guid? IdDocumentUnit { get; set; }

        public bool ExpandProperties { get; set; }

        #endregion

        #region [ Methods ]

        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if (IdDocumentUnit.HasValue)
            {
                odataQuery = odataQuery.Filter(string.Format("DocumentUnit/UniqueId eq {0}", IdDocumentUnit.Value));
            }

            if (ExpandProperties)
            {
                odataQuery = odataQuery.Expand("DocumentUnit($expand=UDSRepository)");
            }
            return odataQuery;
        }

        public override void ResetDecoration()
        {
            IdDocumentUnit = Guid.Empty;
        }

        #endregion

    }
}
