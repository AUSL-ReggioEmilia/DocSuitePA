namespace VecompSoftware.BPM.Integrations.Services.BiblosDS
{
    public interface IDocumentClientConfiguration
    {
        string EndPointConfigurationName { get; set; }
        string RemoteAddress { get; set; }
    }
}