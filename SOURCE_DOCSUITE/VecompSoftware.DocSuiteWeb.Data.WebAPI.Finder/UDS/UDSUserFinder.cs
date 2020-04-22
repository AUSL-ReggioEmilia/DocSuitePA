using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.UDS
{
    public class UDSUserFinder : BaseWebAPIFinder<UDSUser, UDSUser>
    {

        #region [ Constructor ]
        public UDSUserFinder(TenantModel tenant)
            : this(new List<TenantModel>() { tenant })
        {
        }

        public UDSUserFinder(IReadOnlyCollection<TenantModel> tenant)
            : base(tenant)
        {
        }
        #endregion

        #region [ Properties ]

        public Guid? IdUDS { get; set; }
        public string Domain { get; set; }
        public string Username { get; set; }
        public bool? CheckUserAuthorization { get; set; }

        public override void ResetDecoration()
        {
            IdUDS = null;
            Domain = null;
            Username = null;
            CheckUserAuthorization = null;
        }

        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if ((CheckUserAuthorization.HasValue && CheckUserAuthorization.Value) && (IdUDS.HasValue && IdUDS.Value != Guid.Empty) && (!string.IsNullOrEmpty(Domain) && !string.IsNullOrEmpty(Username)))
            {
                odataQuery = odataQuery.Function(string.Format(CommonDefinition.OData.UDSRepositoryService.FX_IsUserAuthorized, IdUDS.Value, Domain, Username));
            }

            if ((!CheckUserAuthorization.HasValue || !CheckUserAuthorization.Value) && (IdUDS.HasValue && IdUDS.Value != Guid.Empty))
            {
                odataQuery = odataQuery.Filter(string.Format("IdUDS eq {0}", IdUDS.Value));
            }

            return odataQuery;
        }
        #endregion
    }
}
