using System;

namespace VecompSoftware.DocSuiteWeb.Model.Parameters
{
    public class JsonParameter
    {
        public string Name { get; set; }

        public JsonPropertyType PropertyType { get; set; }

        public long? ValueInt { get; set; }

        public DateTime? ValueDate { get; set; }

        public double? ValueDouble { get; set; }

        public bool? ValueBoolean { get; set; }

        public Guid? ValueGuid { get; set; }

        public string ValueString { get; set; }
    }
}
