using System.ServiceModel;

namespace VecompSoftware.DocSuiteWeb.API
{
    [ServiceContract]
    public interface IProtocolService
    {
        [OperationContract]
        bool IsAlive();

        [OperationContract]
        string Insert(string protocolDTO);
    }
}
