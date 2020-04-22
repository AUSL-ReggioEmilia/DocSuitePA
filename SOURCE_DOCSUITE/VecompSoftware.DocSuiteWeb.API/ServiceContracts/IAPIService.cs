using System.ServiceModel;

namespace VecompSoftware.DocSuiteWeb.API
{
    [ServiceContract]
    public interface IAPIService
    {
        [OperationContract]
        bool IsAlive();

        [OperationContract]
        string GetAvailable();
    }
}
