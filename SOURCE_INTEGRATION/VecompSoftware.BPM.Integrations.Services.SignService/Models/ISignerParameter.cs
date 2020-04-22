namespace VecompSoftware.BPM.Integrations.Services.SignServices.Models
{
    public interface ISignerParameter
    {
        string ServiceName { get; set; }
        string OTPPassword { get; set; }

    }
}
