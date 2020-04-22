using System;

namespace VecompSoftware.DocSuiteWeb.Model.Metadata
{
    [Serializable()]
    public class BaseFieldModel
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public string DefaultValue { get; set; }
        public bool Required { get; set; }
        public int Position { get; set; }
    }
}
