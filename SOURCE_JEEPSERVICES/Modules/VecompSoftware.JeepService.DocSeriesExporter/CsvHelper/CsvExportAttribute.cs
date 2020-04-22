using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VecompSoftware.JeepService.DocSeriesExporter.CsvHelper
{
    public class CsvExportAttribute : Attribute
    {
        public CsvExportAttribute() { }

        public string FieldDescription { get; set; }
        public int FieldIndex { get; set; }
    }
}
