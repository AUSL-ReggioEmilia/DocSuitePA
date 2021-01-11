using System.IO;
using VecompSoftware.DocSuiteWeb.API.Connector.MailService;
using VecompSoftware.Helpers;

namespace VecompSoftware.DocSuiteWeb.API
{
    public class MailConnector : BaseConnector<MailServiceClient>
    {
        #region [ Constructors ]

        public MailConnector(string address) : base("VecompSoftware.DocSuiteWeb.API.MailService", Path.Combine(address, "MailService.svc")) { }

        #endregion

        
        #region [ Methods ]

        public static MailConnector For(string address)
        {
            return new MailConnector(address);
        }

        public override bool IsAlive()
        {
            return this.Client.IsAlive();
        }

        public APIResponse<MailDTO> Send(IMailDTO dto)
        {
            var input = dto.Serialize();
            var result = this.Client.Send(input);
            return result.DeserializeAsResponse<MailDTO>();
        }

        protected override void CreateClient()
        {
            this.Client = new MailServiceClient(this.ConfigurationName, this.Address);
        }

        #endregion
    }
}
