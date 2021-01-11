using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Model.WebAPI.Client;

namespace VecompSoftware.DocSuiteWeb.Model.Parameters
{
    public class TenantModel
    {
        #region [ Fields ]

        private string _domainName { get; set; }

        #endregion

        #region [ Properties ]

        public Guid TenantId { get; set; }
        public string TenantName { get; set; }
        public bool CurrentTenant { get; set; }
        public string DomainAddress { get; set; }
        public string DomainPassword { get; set; }
        public string DomainUser { get; set; }
        public SecurityContextType SecurityContext { get; set; }
        public string SignalRAddress { get; set; }
        public string DSWUrl { get; set; }
        public string ODATAUrl { get; set; }
        public string BiblosWebAPIUrl { get; set; }
        public HttpClientConfiguration WebApiClientConfig { get; set; }
        public HttpClientConfiguration OriginalConfiguration { get; set; }
        public Dictionary<string, TenantEntityConfiguration> Entities { get; set; }
        public Dictionary<string, bool> EnvironmentsEnable { get; set; }

        public string DomainName
        {
            get
            {
                if (string.IsNullOrEmpty(_domainName))
                {
                    string[] res = DomainUser.Split('\\');
                    if (res.Length > 1)
                    {
                        _domainName = res.First();
                    }
                }
                return _domainName;
            }
        }

        #endregion

        #region [ Constructor ]
        public TenantModel()
        {
            Entities = new Dictionary<string, TenantEntityConfiguration>();
            EnvironmentsEnable = new Dictionary<string, bool>();
            SecurityContext = SecurityContextType.Domain;
        }

        #endregion
    }
}
