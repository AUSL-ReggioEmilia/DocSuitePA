using System.ServiceModel;
using VecompSoftware.Commons.BiblosDS.Objects.Enums;

namespace BiblosDS.WCF.Interface
{
    [ServiceContract(Namespace = "http://Vecomp.StampaConforme.Office.Converter")]
    public interface IStampaConformeConverter
    {
        [OperationContract]
        bool IsAlive();
      
        [OperationContract]
        string GetVersion();

        [OperationContract]
        byte[] Convert(byte[] fileToConvert, string fileExtension);

        /// <summary>
        /// Metodo esteso con parametri di conversione per .msg
        /// </summary>
        /// <param name="fileToConvert"></param>
        /// <param name="fileExtension"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        [OperationContract]
        byte[] ConvertWithParameters(byte[] fileToConvert, string fileExtension, AttachConversionMode mode);        
   }       
}
