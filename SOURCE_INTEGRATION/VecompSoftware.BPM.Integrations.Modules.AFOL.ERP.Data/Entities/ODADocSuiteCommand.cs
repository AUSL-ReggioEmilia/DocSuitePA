using VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Data.Entities.Common;

namespace VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Data.Entities
{
    public class ODADocSuiteCommand : DocSuiteCommand
    {
        public string CIG { get; set; }
        public string RDAReference { get; set; }
    }
}
