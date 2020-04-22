using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.WebAPI.Client
{
    public class HttpClientConfiguration : IHttpClientConfiguration
    {
        #region [ Constructor ]    

        public HttpClientConfiguration()
        {
            Addresses = new List<IBaseAddress>();
            EndPoints = new List<IWebApiControllerEndpoint>();
        }

        #endregion

        #region [ Properties ]

        public ICollection<IBaseAddress> Addresses { get; set; }

        public ICollection<IWebApiControllerEndpoint> EndPoints { get; set; }

        #endregion
    }
}
