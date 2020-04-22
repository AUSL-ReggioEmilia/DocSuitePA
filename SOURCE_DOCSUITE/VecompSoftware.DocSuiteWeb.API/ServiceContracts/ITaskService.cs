using System.ServiceModel;

namespace VecompSoftware.DocSuiteWeb.API
{
    [ServiceContract]
    public interface ITaskService
    {
        [OperationContract]
        bool IsAlive();

        [OperationContract]
        string CreateTask(string taskHeaderDTO);

        [OperationContract]
        string UpdateStatus(string taskHeaderDTO);
    }
}
