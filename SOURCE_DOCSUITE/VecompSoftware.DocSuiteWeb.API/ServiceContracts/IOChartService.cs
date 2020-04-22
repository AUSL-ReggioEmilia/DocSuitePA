using System.ServiceModel;

namespace VecompSoftware.DocSuiteWeb.API
{
    [ServiceContract]
    public interface IOChartService
    {
        [OperationContract]
        bool IsAlive();

        [OperationContract]
        string GetEffective();

        [OperationContract]
        bool Transform(string arrayOrgDeptDTO);
    }
}
