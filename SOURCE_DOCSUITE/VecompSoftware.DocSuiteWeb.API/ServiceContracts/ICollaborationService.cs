using System.ServiceModel;

namespace VecompSoftware.DocSuiteWeb.API
{
    [ServiceContract]
    public interface ICollaborationService
    {
        [OperationContract]
        bool IsAlive();

        [OperationContract]
        string GetCollaborationsToAlert(bool checkExpiredCollaborations);

    }
}
