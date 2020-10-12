using System.Xml.Serialization;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Workflows
{
    [XmlRoot("WorkflowMetadata")]
    public class WorkflowMetadataDraftModel
    {
        [XmlElement("Key")]
        public string Key { get; set; }

        [XmlElement("Value")]
        public string Value { get; set; }
    }
}
