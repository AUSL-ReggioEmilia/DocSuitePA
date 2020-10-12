using System;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.DocumentUnitInterop.Models
{
    public class ExternalIdentifierModel
    {
        public Guid TenantAOOId { get; set; }

        public string TenantAOOName { get; set; }

        public Guid TenantId { get; set; }

        public string TenantName { get; set; }
    }
}
