namespace VecompSoftware.BPM.Integrations.Services.SignServices.Models
{
    public interface ISignFactory
    {
        byte[] SignDocument(ISignerParameter signerParameter, byte[] document);
    }
}
