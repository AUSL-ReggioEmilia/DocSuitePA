using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Services.StampaConforme.Service;

namespace VecompSoftware.BPM.Integrations.Services.StampaConforme
{
    public interface IStampaConformeClient
    {
        Task<byte[]> ConvertToPDFAAsync(byte[] source, string signature);
        Task UploadSecureDocumentAsync(byte[] source, string referenceId);
        Task<byte[]> BuildPDFAsync(byte[] template, BuildValueModel[] buildValueModel, string label);
    }
}
