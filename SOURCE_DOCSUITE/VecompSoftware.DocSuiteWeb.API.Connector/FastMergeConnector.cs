using System.IO;
using VecompSoftware.DocSuiteWeb.API.Connector.FastMergeService;
using VecompSoftware.Helpers;

namespace VecompSoftware.DocSuiteWeb.API
{
    public class FastMergeConnector : BaseConnector<FastMergeServiceClient>
    {
        #region [ Constructors ]

        public FastMergeConnector(string address) : base("VecompSoftware.DocSuiteWeb.API.FastMergeService", Path.Combine(address, "FastMergeService.svc")) { }

        #endregion

        #region [ Methods ]

        public static FastMergeConnector For(string address)
        {
            return new FastMergeConnector(address);
        }

        public override bool IsAlive()
        {
            return Client.IsAlive();
        }

        public APIResponse<ContainerDTO[]> GetContainers()
        {
            var serialized = this.Client.GetContainers();
            return serialized.DeserializeAsResponse<ContainerDTO[]>();
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

        public APIResponse<MailboxDTO[]> GetPOLMailboxes()
        {
            var serialized = this.Client.GetPOLMailboxes();
            return serialized.DeserializeAsResponse<MailboxDTO[]>();
        }

        public APIResponse<TableDocTypeDTO[]> GetDocumentTypes()
        {
            var serialized = this.Client.GetDocumentTypes();
            return serialized.DeserializeAsResponse<TableDocTypeDTO[]>();
        }

        public APIResponse<ServiceCategoryDTO[]> GetServiceCategories()
        {
            var serialized = this.Client.GetServiceCategories();
            return serialized.DeserializeAsResponse<ServiceCategoryDTO[]>();
        }

        public APIResponse<ProtocolDTO> InsertProtocol(IProtocolDTO protocolDTO, ITaskDTO taskDTO)
        {
            var inputProtocol = protocolDTO.Serialize();
            var inputTask = taskDTO.Serialize();
            var result = this.Client.InsertProtocol(inputProtocol, inputTask);
            return result.DeserializeAsResponse<ProtocolDTO>();
        }

        public APIResponse<MailDTO> SendMail(IMailDTO mailDTO, ITaskDTO taskDTO)
        {
            var inputMail = mailDTO.Serialize();
            var inputTask = taskDTO.Serialize();
            var result = this.Client.SendMail(inputMail, inputTask);
            return result.DeserializeAsResponse<MailDTO>();
        }

        public APIResponse<MailDTO> SendProtocolMail(IMailDTO mailDTO, IProtocolDTO protocolDTO, ITaskDTO taskDTO)
        {
            var inputMail = mailDTO.Serialize();
            var inputProtocol = protocolDTO.Serialize();
            var inputTask = taskDTO.Serialize();
            var result = this.Client.SendProtocolMail(inputMail, inputProtocol, inputTask);
            return result.DeserializeAsResponse<MailDTO>();
        }

        public APIResponse<TaskDTO> CreateTask(ITaskDTO dto)
        {
            var input = dto.Serialize();
            var result = this.Client.CreateTask(input);
            return result.DeserializeAsResponse<TaskDTO>();
        }

        public APIResponse<TaskDTO> UpdateStatus(TaskDTO header)
        {
            var input = header.Serialize();
            var result = this.Client.UpdateStatus(input);
            return result.DeserializeAsResponse<TaskDTO>();
        }

        public APIResponse<string[]> GetRecentFastMergeCodes()
        {
            var result = this.Client.GetRecentFastMergeCodes();
            return result.DeserializeAsResponse<string[]>();
        }

        public APIResponse<string> GetProtocolDocument(IProtocolDTO protocolDTO)
        {
            string serialized = protocolDTO.Serialize();
            var result = this.Client.GetProtocolDocument(serialized);
            return result.DeserializeAsResponse<string>();
        }

        protected override void CreateClient()
        {
            Client = new FastMergeServiceClient(ConfigurationName, Address);
        }

        #endregion
    }
}
