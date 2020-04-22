using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.UDS
{
    public class UDSCollaborationFinder : BaseUDSRelationFinder<UDSCollaboration, UDSCollaboration>
    {
        #region [ Constructor ]
        public UDSCollaborationFinder(TenantModel tenant)
            : this(new List<TenantModel>() { tenant })
        {
        }

        public UDSCollaborationFinder(IReadOnlyCollection<TenantModel> tenant)
            : base(tenant)
        {
        }
        #endregion

        #region [ Properties ]
        public int? IdCollaboration { get; set; }
        public bool ExpandRepository { get; set; }
        #endregion

        #region [ Methods ]
        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if (IdCollaboration.HasValue)
            {
                odataQuery = odataQuery.Filter(string.Format("Relation/EntityId eq {0}", IdCollaboration));
            }

            if (ExpandRepository)
            {
                odataQuery = odataQuery.Expand("Repository");
            }
            return base.DecorateFinder(odataQuery);
        }
        #endregion
    }
}
