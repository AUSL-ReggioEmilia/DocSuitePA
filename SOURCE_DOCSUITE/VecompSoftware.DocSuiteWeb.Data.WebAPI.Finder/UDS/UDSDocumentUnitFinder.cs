using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.UDS
{
    public class UDSDocumentUnitFinder : BaseUDSRelationFinder<UDSDocumentUnit, UDSDocumentUnit>
    {
        #region [ Constructor ]
        public UDSDocumentUnitFinder(TenantModel tenant)
            : this(new List<TenantModel>() { tenant })
        {
        }

        public UDSDocumentUnitFinder(IReadOnlyCollection<TenantModel> tenant)
            : base(tenant)
        {
            DocumentUnitTypes = new List<UDSRelationType>();
        }
        #endregion

        #region [ Properties ]
        public ICollection<UDSRelationType> DocumentUnitTypes { get; set; }

        public Guid? IdDocumentUnit { get; set; }

        public bool ExpandRepository { get; set; }
        #endregion

        #region [ Methods ]
        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if (IdUDS.HasValue && IdUDS.Value != Guid.Empty)
            {
                string odataExpression = string.Format("IdUDS eq {0}", IdUDS.Value);
                odataQuery = odataQuery.Filter(odataExpression);
            }

            if (DocumentUnitTypes.Count > 0)
            {
                string odataExpression = string.Join(" or ", DocumentUnitTypes.Select(s => string.Format("RelationType eq VecompSoftware.DocSuiteWeb.Entity.UDS.UDSRelationType'{0}'", (int)s)));
                odataQuery = odataQuery.Filter(string.Concat("(",odataExpression,")"));
            }

            if (IdDocumentUnit.HasValue)
            {
                odataQuery = odataQuery.Filter(string.Format("Relation/UniqueId eq {0}", IdDocumentUnit));
            }

            if (ExpandRelation)
            {
                odataQuery = odataQuery.Expand("Relation");
            }

            if (ExpandRepository)
            {
                odataQuery = odataQuery.Expand("Repository");
            }

            return odataQuery;
        }
        #endregion
    }
}
