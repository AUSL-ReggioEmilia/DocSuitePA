using System.ServiceModel;

namespace VecompSoftware.DocSuiteWeb.API
{
    [ServiceContract]
    public interface IFastMergeService
    {
        [OperationContract]
        bool IsAlive();

        [OperationContract]
        string GetContainers();

        [OperationContract]
        string GetCategories();

        [OperationContract]
        string GetPECMailboxes();

        [OperationContract]
        string GetPOLMailboxes();

        [OperationContract]
        string GetDocumentTypes();

        [OperationContract]
        string GetServiceCategories();

        [OperationContract]
        string InsertProtocol(string protocolDTO, string taskDTO);

        [OperationContract]
        string SendMail(string mailDTO, string taskDTO);

        [OperationContract]
        string SendProtocolMail(string mailDTO, string protocolDTO, string taskDTO);

        [OperationContract]
        string CreateTask(string taskDTO);

        [OperationContract]
        string UpdateStatus(string taskDTO);

        [OperationContract]
        string GetRecentFastMergeCodes();

        [OperationContract]
        string GetProtocolDocument(string protocolDTO);
    }
}
