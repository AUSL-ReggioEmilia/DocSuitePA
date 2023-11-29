using System;

namespace VecompSoftware.DocSuiteWeb.Model.Metadata
{
    [Serializable()]
    public class BaseFieldModel
    {
        public string KeyName { get; set; }
        public string Label { get; set; }
        public string DefaultValue { get; set; }
        public bool Required { get; set; }
        public bool ShowInResults { get; set; }
        public bool HiddenField { get; set; }
        public bool ReadOnly { get; set; }
        public string FormatType { get; set; }
        public int Position { get; set; }
    }
}
