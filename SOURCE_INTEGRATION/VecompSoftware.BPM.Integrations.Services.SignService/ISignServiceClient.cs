using VecompSoftware.BPM.Integrations.Services.SignServices.Enums;
using VecompSoftware.BPM.Integrations.Services.SignServices.Models;

namespace VecompSoftware.BPM.Integrations.Services.SignServices
{
    public interface ISignServiceClient
    {
        byte[] SignDocument(ISignerParameter signerParameter, byte[] document, ProviderType signature = ProviderType.ArubaSign);
    }
}
