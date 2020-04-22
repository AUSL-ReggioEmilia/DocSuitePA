using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VecompSoftware.DocSuiteWeb.DTO.WSSeries;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.JeepService.DocSeriesExporter.CsvHelper;

namespace VecompSoftware.JeepService.DocSeriesExporter.Repository
{
    public class FileSystemRepository : BaseRepository, IExportRepository
    {
        #region [ Fields ]         
        private readonly DocSeriesExporterParameters _parameters;
        #endregion

        #region [ Constructor ]
        public FileSystemRepository(DocSeriesExporterParameters parameters)
            : base(parameters)
        {
            this._parameters = parameters;
        }
        #endregion

        #region [ Methods ]

        public SeriesToCsvFields GetHeaderDynamicFields()
        {
            var seriesToCsv = new SeriesToCsvFields();
            foreach (var data in ArchiveAttribute)
            {
                var dict = new KeyValuePair<string, object>(data.Description, null);

                seriesToCsv.DynamicData.Add(dict);
            }
            return seriesToCsv;
        }

        public void StartExport(int id, bool isSubSection = false)
        {
            var streamData = new MemoryStream();
            CsvWriter csvWriter;
            string fullPath;

            csvWriter = new CsvWriter(streamData, ";", new List<string>() { "IdSeries", "IdSubsection" });             
            csvWriter.WriteHeader(GetHeaderDynamicFields());

            foreach (var field in CsvFields)
            {
                csvWriter.WriteLine(field);
            }

            if (isSubSection)
            {
                fullPath = string.Format(@"{1}\Export_Subsection_{0}.csv", id, this._parameters.FileSystemPath);
            }
            else
            {
                fullPath = string.Format(@"{1}\Export_{0}.csv", id, this._parameters.FileSystemPath);
            }            

            File.WriteAllBytes(fullPath, streamData.ToArray());
            streamData.Dispose();
        }
        #endregion
    }
}
