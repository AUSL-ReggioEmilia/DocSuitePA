using System.IO;
using Newtonsoft.Json;
using VecompSoftware.DocSuiteWeb.API.Connector.APIService;
using VecompSoftware.Helpers;
using VecompSoftware.Helpers.ExtensionMethods;

namespace VecompSoftware.DocSuiteWeb.API
{
    public class APIConnector : BaseConnector<APIServiceClient>
    {

        #region [ Constructors ]

        public APIConnector(string address) : base("VecompSoftware.DocSuiteWeb.API.APIService", Path.Combine(address, "APIService.svc")) { }

        #endregion

        #region [ Methods ]

        public static APIConnector For(string address)
        {
            return new APIConnector(address);
        }

        public override bool IsAlive()
        {
            return Client.IsAlive();
        }

        public IAPIProviderDTO[] GetAvailable()
        {
            var json = Client.GetAvailable().FromBase64();
            return JsonConvert.DeserializeObject<APIProviderDTO[]>(json);
        }

        protected override void CreateClient()
        {
            Client = new APIServiceClient(ConfigurationName, Address);
        }

        #endregion

    }
}
