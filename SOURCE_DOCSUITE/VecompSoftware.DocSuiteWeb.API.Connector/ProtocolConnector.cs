using System.IO;
using VecompSoftware.DocSuiteWeb.API.Connector.ProtocolService;
using VecompSoftware.Helpers;

namespace VecompSoftware.DocSuiteWeb.API
{
    public class ProtocolConnector : BaseConnector<ProtocolServiceClient>
    {
        #region [ Constructors ]

        public ProtocolConnector(string address) : base("VecompSoftware.DocSuiteWeb.API.ProtocolService", Path.Combine(address, "ProtocolService.svc")) { }

        #endregion

        #region [ Methods ]

        public static ProtocolConnector For(string address)
        {
            return new ProtocolConnector(address);
        }

        public override bool IsAlive()
        {
            return Client.IsAlive();
        }

        public APIResponse<ProtocolDTO> Insert(IProtocolDTO dto)
        {
            var input = dto.Serialize();
            var result = this.Client.Insert(input);
            return result.DeserializeAsResponse<ProtocolDTO>();
        }

        protected override void CreateClient()
        {
            Client = new ProtocolServiceClient(ConfigurationName, Address);
        }

        #endregion
    }
}
