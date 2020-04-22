using System;
using System.IO;
using VecompSoftware.DocSuiteWeb.API.Connector.FastInvoiceProtocolService;
using VecompSoftware.Helpers;

namespace VecompSoftware.DocSuiteWeb.API
{
    public class FastInvoiceProtocolConnector : BaseConnector<FastInvoiceProtocolServiceClient>
    {
        #region Constructor
        public FastInvoiceProtocolConnector(string address) : base("VecompSoftware.DocSuiteWeb.API.FastInvoiceProtocolService", Path.Combine(address, "FastInvoiceProtocolService.svc")) { }
        #endregion

        #region Methods

        public static FastInvoiceProtocolConnector For(string address)
        {
            return new FastInvoiceProtocolConnector(address);
        }

        public override bool IsAlive()
        {
            return Client.IsAlive();
        }

        public APIResponse<AccountingSectionalDTO[]> GetAccountingSectionals()
        {
            var serialized = this.Client.GetAccountingSectionals();
            return serialized.DeserializeAsResponse<AccountingSectionalDTO[]>();
        }

        public APIResponse<ContainerDTO[]> GetContainers()
        {
            var serialized = this.Client.GetContainers();
            return serialized.DeserializeAsResponse<ContainerDTO[]>();
        }

        public APIResponse<ContactDTO[]> GetContacts(string searchCode)
        {
            var serialized = this.Client.GetContacts(searchCode);
            return serialized.DeserializeAsResponse<ContactDTO[]>();
        }

        public APIResponse<CategoryDTO[]> GetCategories()
        {
            var serialized = this.Client.GetCategories();
            return serialized.DeserializeAsResponse<CategoryDTO[]>();
        }

        public APIResponse<MailboxDTO[]> GetPECMailboxes()
        {
            var serialized = this.Client.GetPECMailboxes();
            return serialized.DeserializeAsResponse<MailboxDTO[]>();
        }

        public APIResponse<ProtocolDTO[]> GetProtocolsToSend()
        {
            var serialized = this.Client.GetProtocolsToSend();
            return serialized.DeserializeAsResponse<ProtocolDTO[]>();
        }

        public int CountProtocolReserved(DateTime from, DateTime to)
        {
            return this.Client.CountProtocolReserved(from, to);
        }
        public APIResponse<ProtocolDTO> InsertProtocol(IProtocolDTO protocolDto)
        {
            var inputProtocol = protocolDto.Serialize();
            var result = this.Client.InsertProtocol(inputProtocol);
            return result.DeserializeAsResponse<ProtocolDTO>();
        }

        public APIResponse<MailDTO> SendMail(IMailDTO mailDTO)
        {
            var inputMail = mailDTO.Serialize();
            var result = this.Client.SendMail(inputMail);
            return result.DeserializeAsResponse<MailDTO>();
        }

        public APIResponse<MailDTO> SendProtocolMail(IMailDTO mailDTO, IProtocolDTO protocolDTO)
        {
            var inputMail = mailDTO.Serialize();
            var inputProtocol = protocolDTO.Serialize();
            var result = this.Client.SendProtocolMail(inputMail, inputProtocol);
            return result.DeserializeAsResponse<MailDTO>();
        }

        protected override void CreateClient()
        {
            Client = new FastInvoiceProtocolServiceClient(ConfigurationName, Address);
        }
        #endregion
    }
}
