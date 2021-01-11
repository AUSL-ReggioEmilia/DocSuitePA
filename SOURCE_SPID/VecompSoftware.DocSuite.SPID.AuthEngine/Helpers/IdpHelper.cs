using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VecompSoftware.DocSuite.SPID.AuthEngine.Models;

namespace VecompSoftware.DocSuite.SPID.AuthEngine.Helpers
{
    public class IdpHelper
    {
        #region [ Fields ]
        private readonly IHostingEnvironment _hostingEnvironment;

        public const string IDP_ARUBA = "idp_aruba";
        public const string IDP_INTESA = "idp_intesa";
        public const string IDP_INFOCERT = "idp_infocert";
        public const string IDP_NAMIRIAL = "idp_namirial";
        public const string IDP_POSTE = "idp_poste";
        public const string IDP_REGISTER = "idp_register";
        public const string IDP_SIELTE = "idp_sielte";
        public const string IDP_TIM = "idp_tim";
        public const string IDP_FEDERA = "idp_federa";

        private ICollection<IdentityProvider> _idps =>
            JsonConvert.DeserializeObject<ICollection<IdentityProvider>>(File.ReadAllText(Path.Combine(_hostingEnvironment.ContentRootPath, "identityproviders.json")));
        #endregion

        #region [ Constructor ]
        public IdpHelper(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        #endregion

        #region [ Methods ]
        public string GetSingleSignOnUrl(string idp)
        {
            if (_idps.Any(x => x.IdpCode.Equals(idp)))
            {
                return _idps.First(x => x.IdpCode.Equals(idp)).SingleSignOnService;
            }
            throw new Exception(string.Concat("Nessun IDP trovato con codice ", idp));
        }

        public string GetSingleLogoutUrl(string idp)
        {
            if (_idps.Any(x => x.IdpCode.Equals(idp)))
            {
                return _idps.First(x => x.IdpCode.Equals(idp)).SingleLogoutService;
            }
            throw new Exception(string.Concat("Nessun IDP trovato con codice ", idp));
        }

        public string GetIdpNameFromIssuerId(string issuerId)
        {
            if (_idps.Any(x => x.IssuerId.Equals(issuerId)))
            {
                return _idps.First(x => x.IssuerId.Equals(issuerId)).IdpCode;
            }
            throw new Exception(string.Concat("Nessun IDP trovato con EntityId ", issuerId));
        }

        public string GetEntityId(string idp)
        {
            if (_idps.Any(x => x.IdpCode.Equals(idp)))
            {
                return _idps.First(x => x.IdpCode.Equals(idp)).IssuerId;
            }
            throw new Exception(string.Concat("Nessun IDP trovato con codice ", idp));
        }
        #endregion        
    }
}
