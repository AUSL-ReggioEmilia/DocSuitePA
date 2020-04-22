using System;
using System.Xml.Serialization;

namespace VecompSoftware.DocSuiteWeb.AVCP.Import
{
    public class RecordTemplateField
    {
        [XmlAttribute()]
        public string name;

        [XmlAttribute()]
        public string type;

        [XmlAttribute()]
        public bool ignore;

        [XmlAttribute()]
        public bool requested;

        [XmlAttribute()]
        public bool isFilename { get; set; }

        [XmlAttribute()]
        public string defaultValue { get; set; }

        [XmlAttribute()]
        public string columnName { get; set; }

        public RecordTemplateField()
        {
            name = String.Empty;
            type = "String";
            ignore = false;
            requested = false;
            isFilename = false;
            defaultValue = String.Empty;
            columnName = string.Empty;
        }
    }

}
