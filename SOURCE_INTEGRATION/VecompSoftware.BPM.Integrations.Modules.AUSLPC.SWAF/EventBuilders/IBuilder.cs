using VecompSoftware.DocSuiteWeb.Model.ExternalModels;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLPC.SWAF.EventBuilders
{
    internal interface IBuilder
    {
        DocSuiteEvent Build();
    }
}
