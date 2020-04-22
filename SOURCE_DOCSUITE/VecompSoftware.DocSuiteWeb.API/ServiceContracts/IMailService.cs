using System.ServiceModel;

namespace VecompSoftware.DocSuiteWeb.API
{
    [ServiceContract]
    public interface IMailService
    {
        [OperationContract]
        bool IsAlive();

        [OperationContract]
        string Send(string mailDTO);
    }
}
