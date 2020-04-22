using System;
using System.Collections.Generic;
using VecompSoftware.JeepService.DocSeriesExporter.CsvHelper;

namespace VecompSoftware.JeepService.DocSeriesExporter
{
    [Serializable]
    public class SeriesToCsvFields
    {
        #region Constructor

        public SeriesToCsvFields()
        {
            DynamicData = new Dictionary<string, object>();
        }

        #endregion

        #region Properties
        public int IdSeries { get; set; }

        public int? IdSubsection { get; set; }

        [CsvExport(FieldDescription = "Oggetto", FieldIndex = 0)]
        public string Object { get; set; }

        [CsvExport(FieldDescription = "Data di Pubblicazione", FieldIndex = 1)]
        public DateTime? PublicationDate { get; set; }

        [CsvExport(FieldIndex = 2)]
        public IDictionary<string, object> DynamicData { get; set; }
        
        #endregion
    }
}
