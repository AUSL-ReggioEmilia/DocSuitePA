using System;

namespace VecompSoftware.ServiceBus.Module.UDS.Roslyn.Generators
{
    public class Metadata
    {
        public string PropertyName { get; set; }
        public Type PropertyType { get; set; }
        public bool Required { get; set; }
        public bool Nullable { get; set; }
        public Type BiblosPropertyType { get; set; }
    }
}
