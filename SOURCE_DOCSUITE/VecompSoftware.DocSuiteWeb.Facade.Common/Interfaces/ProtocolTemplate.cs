using VecompSoftware.DocSuiteWeb.Data;

namespace VecompSoftware.DocSuiteWeb.Facade.Common.Interfaces
{
    public struct ProtocolTemplate
    {
        public ProtocolStatus Status;
        public ProtocolType Type;
        public DocumentType DocumentType;
        public Location Location;
        public Location AttachLocation;
        public Container Container;
        public Category Category;
        public string Note;
    }
}