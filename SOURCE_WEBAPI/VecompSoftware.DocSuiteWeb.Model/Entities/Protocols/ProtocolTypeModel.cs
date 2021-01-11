namespace VecompSoftware.DocSuiteWeb.Model.Entities.Protocols
{
    public class ProtocolTypeModel
    {
        public ProtocolTypeModel() { }

        public ProtocolTypeModel(ProtocolTypology protocolType)
        {
            EntityShortId = (short)protocolType;
        }
        public short EntityShortId { get; set; }

        public string Description { get; set; }

    }
}
