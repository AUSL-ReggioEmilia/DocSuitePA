using System.ServiceModel;
using VecompSoftware.Commons.BiblosDS.Objects;

namespace BiblosDS.WCF.Interface
{
    [ServiceContract]
    public interface IServiceStampaConforme
    {
        [OperationContract]
        DocumentContent ConvertToFormat(DocumentContent content, string fileName, string extReq, out DocumentContent wmfSigns);

        [OperationContract]
        DocumentContent ConvertToFormatLabeled(DocumentContent content, string fileName, string extReq, string label);    

        [OperationContract]
        bool IsAlive();
    }   
}
