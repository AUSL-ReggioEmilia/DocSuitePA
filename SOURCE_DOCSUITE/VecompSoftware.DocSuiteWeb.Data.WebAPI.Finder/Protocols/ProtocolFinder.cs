using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Protocols
{
    public class ProtocolFinder : BaseWebAPIFinder<Protocol, ProtocolModel>
    {
        #region [ Constructor ]

        public ProtocolFinder(TenantModel tenant)
           : this(new List<TenantModel>() { tenant })
        {
        }

        public ProtocolFinder(IReadOnlyCollection<TenantModel> tenants) 
            : base(tenants)
        {
            
        }

        #endregion

        #region [ Properties ]

        public string UserName { get; set; }

        public string Domain { get; set; }

        public DateTimeOffset? DateFrom { get; set; }

        public DateTimeOffset? DateTo { get; set; }

        #endregion

        #region [ Methods ]

        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            return odataQuery.Function(string.Format(CommonDefinition.OData.ProtocolService.FX_GetAuthorizedProtocols,
                UserName, Domain, DateFrom.Value.ToString(ODataDateConversion), DateTo.Value.ToString(ODataDateConversion)));
        }

        public override void ResetDecoration()
        {
            UserName = string.Empty;
            Domain = string.Empty;
            DateFrom = null;
            DateTo = null;
        }

        #endregion

    }
}
