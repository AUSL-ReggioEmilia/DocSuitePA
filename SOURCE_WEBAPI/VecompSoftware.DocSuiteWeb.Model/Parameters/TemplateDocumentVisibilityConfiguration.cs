using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Model.Parameters
{
    public class TemplateDocumentVisibilityConfiguration
    {
        public string Name { get; set; }
        public IDictionary<ChainType, bool> VisibilityChains { get; set; }
    }
}
