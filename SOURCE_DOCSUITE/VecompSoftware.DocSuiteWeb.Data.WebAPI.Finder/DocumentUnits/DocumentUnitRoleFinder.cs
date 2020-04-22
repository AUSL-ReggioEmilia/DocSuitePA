using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Finder;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.WebAPIManager;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.DocumentUnits
{
    public class DocumentUnitRoleFinder : BaseWebAPIFinder<DocumentUnitRole, DocumentUnitRole>
    {
        #region [ Constructor ]

        public DocumentUnitRoleFinder(TenantModel tenant)
         : this(new List<TenantModel>() { tenant })
        { }

        public DocumentUnitRoleFinder(IReadOnlyCollection<TenantModel> tenants) 
            : base(tenants)
        {
        }

        #endregion

        #region [ Properties ]

        public string UserName { get; set; }

        public string Domain { get; set; }

        public Guid? IdDocumentUnit { get; set; }

        #endregion

        #region [ Methods ]

        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {                                              
            return odataQuery;
        }

        public override void ResetDecoration()
        {
            UserName = string.Empty;
            Domain = string.Empty;
            IdDocumentUnit = Guid.Empty;
        }

        #endregion

    }
}
