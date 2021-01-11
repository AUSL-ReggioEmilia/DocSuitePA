using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace VecompSoftware.DocSuiteWeb.Facade.Common.DocumentSeries
{
    [Serializable]
    public class SeriesToCsvFields
    {
        #region [ Constructor ]
        public SeriesToCsvFields()
        {
            DynamicData = new Dictionary<string, object>();
        }
        #endregion

        #region [ Properties ]
        public int IdSeries { get; set; }

        [CsvExport(FieldDescription = "Anno", FieldIndex = 0)]
        public string Year { get; set; }

        [CsvExport(FieldDescription = "Numero", FieldIndex = 1)]
        public string Number { get; set; }

        [CsvExport(FieldDescription = "Data Registrazione", FieldIndex = 2)]
        public DateTime RegistrationDate { get; set; }

        [CsvExport(FieldDescription = "Oggetto", FieldIndex = 3)]
        public string Object { get; set; }

        [CsvExport(FieldDescription = "Classificazione", FieldIndex = 4)]
        public string Categorry { get; set; }

        [CsvExport(FieldDescription = "Data di Pubblicazione", FieldIndex = 5)]
        public DateTime? PublicationDate { get; set; }

        [CsvExport(FieldIndex = 6)]
        public IDictionary<string, object> DynamicData { get; set; }

        public SeriesToCsvFields DeepClone()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
                stream.Position = 0;
                return (SeriesToCsvFields)formatter.Deserialize(stream);
            }
        }
        #endregion
    }
}
