using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.WebAPI.Client
{
    public interface IHttpClientConfiguration
    {
        ICollection<IBaseAddress> Addresses { get; set; }

        ICollection<IWebApiControllerEndpoint> EndPoints { get; set; }
    }
}
