using System.IO;
using System.Xml;
using VecompSoftware.DocSuiteWeb.Data;

namespace VecompSoftware.DocSuiteWeb.Facade.Common.Interfaces
{
    public interface IProtocolImporter
    {
        Protocol Import(XmlDocument xml, FileInfo pdf, ProtocolTemplate template);
        bool ClassificationManaged { get; }
    }
}
