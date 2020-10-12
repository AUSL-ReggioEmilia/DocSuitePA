using System;
using System.Collections.Generic;
using VecompSoftware.WebAPIManager.Finder;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.WebAPIManager;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VecompSoftware.Clients.WebAPI.Http;
using VecompSoftware.Services.Logging;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;
using System.Linq;
using VecompSoftware.DocSuiteWeb.DTO.OData;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Commons
{
    public class RoleUserFromDossierFinder : BaseWebAPIFinder<RoleUser, RoleUserModel>
    {
        #region [ Properties ] 
        public RoleUserFinderModel RoleUserFinderModel
        {
            get; set;
        }

        #endregion

        #region [ Constructor ] 
        public RoleUserFromDossierFinder(TenantModel tenant)
            : this(new List<TenantModel>() { tenant })
        {
        }

        public RoleUserFromDossierFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
        }

        #endregion

        #region [ Methods ]

        /*
         * http://localhost/dsw.webapi/odata/RoleUsers/RoleUserService.GetRoleUsersFromDossier(finder=@p0)?@p0={'IdDossiers':'cd0e6b16-c523-459f-a118-b541288ac3c3'}
         */
        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            odataQuery = odataQuery.Function(string.Format(CommonDefinition.OData.RoleUserService.FX_AllSecretariesFromDossier,
                JsonConvert.SerializeObject(RoleUserFinderModel)));

            return base.DecorateFinder(odataQuery);
        }

        public override void ResetDecoration()
        {
            RoleUserFinderModel = null;
        }

        #endregion
    }
}
