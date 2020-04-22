using System;

namespace VecompSoftware.DocSuiteWeb.Model.Documents
{
    public class AttributeValue
    {
        public Guid Id { get; set; }
        public Guid IdAttribute { get; set; }
        public string AttributeName { get; set; }
        public long? ValueInt { get; set; }
        public float? ValueFloat { get; set; }
        public DateTime? ValueDate { get; set; }
        public string ValueString { get; set; }
        public AttributeValueType ValueType { get; set; }
        public bool IsDocumentAttribute { get; set; }
    }
}
