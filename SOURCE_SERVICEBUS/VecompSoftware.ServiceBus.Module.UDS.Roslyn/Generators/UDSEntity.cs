using System.Collections.Generic;

namespace VecompSoftware.ServiceBus.Module.UDS.Roslyn.Generators
{
    public class UDSEntity
    {
        public UDSEntity()
        {
            MetaData = new List<Metadata>();
        }
        public string TableName { get; set; }
        public string Namespace { get; set; }
        public List<Metadata> MetaData { get; set; }
    }
}
