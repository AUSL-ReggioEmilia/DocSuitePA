using System;

namespace VecompSoftware.DocSuiteWeb.Model.Parameters
{
    public class TenantEntityConfiguration
    {
        public bool IsActive { get; set; }
        public TimeSpan? Timeout { get; set; }
        public string ODATAControllerName { get; set; }
    }
}
