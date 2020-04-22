using System.Collections.Generic;
using System.IO;

namespace VecompSoftware.DocSuiteWeb.Facade.Common.DocumentSeries
{
    public class FileSystemRepository : BaseRepository, IExportRepository
    {
        #region [ Constructor ]
        public FileSystemRepository()
            : base()
        {
        }
        #endregion

        #region [ Methods ]

        public SeriesToCsvFields GetHeaderDynamicFields()
        {
            var seriesToCsv = new SeriesToCsvFields();

            foreach (var data in ArchiveAttribute)
            {
                var dict = new KeyValuePair<string, object>(data.Name, null);

                seriesToCsv.DynamicData.Add(dict);
            }
            return seriesToCsv;
        }

        public byte[] StartExport(int id)
        {
            var streamData = new MemoryStream();
            try
            {                
                CsvWriter csvWriter;

                csvWriter = new CsvWriter(streamData, ";", new List<string>() { "IdSeries", "IdSubsection" });

                csvWriter.WriteHeader(GetHeaderDynamicFields());

                foreach (var field in CsvFields)
                {
                    csvWriter.WriteLine(field);
                }

                return streamData.ToArray();
            }
            finally
            {
                streamData.Dispose();
            }
        }
        #endregion
    }
}
