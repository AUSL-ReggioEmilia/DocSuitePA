using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.DocumentUnits
{
    public class DocumentUnitFinder : BaseWebAPIFinder<DocumentUnit, DocumentUnit>
    {
        #region [ Constructor ]

        public DocumentUnitFinder(TenantModel tenant)
         : this(new List<TenantModel>() { tenant })
        { }

        public DocumentUnitFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
        }

        #endregion

        #region [ Properties ]

        public Guid? IdDocumentUnit { get; set; }

        public bool ExpandContainer { get; set; }

        public bool ExpandChains { get; set; }

        public bool ExpandRoles { get; set; }

        public bool ExpandUDSRepository { get; set; }

        public bool ExpandUsers { get; set; }
        #endregion

        #region [ Methods ]

        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if (IdDocumentUnit.HasValue && IdDocumentUnit.Value != Guid.Empty)
            {
                string odataExpression = string.Format("UniqueId eq {0}", IdDocumentUnit.Value);
                odataQuery = odataQuery.Filter(odataExpression);
            }

            if (ExpandContainer)
            {
                odataQuery = odataQuery.Expand("Container");
            }

            if (ExpandChains)
            {
                odataQuery = odataQuery.Expand("DocumentUnitChains");
            }

            if (ExpandRoles)
            {
                odataQuery = odataQuery.Expand("DocumentUnitRoles");
            }

            if (ExpandUDSRepository)
            {
                odataQuery = odataQuery.Expand("UDSRepository");
            }

            if (ExpandUsers)
            {
                odataQuery = odataQuery.Expand("DocumentUnitUsers");
            }
            return odataQuery;
        }

        public override void ResetDecoration()
        {
            IdDocumentUnit = null;
            ExpandContainer = false;
            ExpandChains = false;
            ExpandUDSRepository = false;
            ExpandRoles = false;
            ExpandUsers = false;
        }

        #endregion

    }
}
