using System;

namespace VecompSoftware.DocSuiteWeb.Facade.Common.DocumentSeries
{
    public class CsvExportAttribute : Attribute
    {
        public CsvExportAttribute() { }
        public string FieldDescription { get; set; }
        public int FieldIndex { get; set; }
    }
}
