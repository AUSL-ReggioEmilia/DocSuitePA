using System;
using System.ServiceModel;

namespace VecompSoftware.ServiceContract.BiblosDS.Preservations
{
    [ServiceContract]
    public interface IServiceLegalExtension : IBiblosDSServiceContract
    {
        [OperationContract]
        string GetData(int value);

        [OperationContract]
        bool IsAlive();

        [OperationContract]
        bool VerifyConservation(Guid IdConservazione);

        [OperationContract]
        bool VerifyTaskOfConservation(Guid IdTask);

        [OperationContract]
        bool DoTaskOfConservation(Guid IdTask);

        [OperationContract]
        bool ConfirmSignedConservation(Guid IdConservazione);
    }   
}
