using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace VecompSoftware.JeepService.DocSeriesExporter.CsvHelper
{
    public static class SeriesToCsvFiledsEx
    {
        public static SeriesToCsvFields DeepClone(this SeriesToCsvFields source)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, source);
                stream.Position = 0;
                return (SeriesToCsvFields)formatter.Deserialize(stream);
            }
        }
    }
}
