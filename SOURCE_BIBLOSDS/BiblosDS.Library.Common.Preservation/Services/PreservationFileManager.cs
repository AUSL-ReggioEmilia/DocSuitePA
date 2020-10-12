using BiblosDS.Library.Common.Enums;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Objects.Response;
using log4net;
using System;
using System.IO;
using objects = BiblosDS.Library.Common.Objects;

namespace BiblosDS.Library.Common.Preservation.Services
{
    public class PreservationFileManager : IDisposable
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(PreservationFileManager));
        const string VERYFY_DIRECTORY_NAME = "verify";

        DocumentArchive archive;
        Company company;
        string workingDir = "";

        public PreservationFileManager(DocumentArchive archive, Company company)
        {
            this.archive = archive;
            this.company = company;
            workingDir = archive.PathPreservation;
        }

        public void Empty(System.IO.DirectoryInfo directory)
        {
            foreach (System.IO.FileInfo file in directory.GetFiles()) file.Delete();
            foreach (System.IO.DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
        }

        internal void Check(int totalSize)
        {
            if (string.IsNullOrEmpty(workingDir))
            {
                new PreservationError("Nessuna directory definita per la conservazione. Contattare il riferimento tecnico.", PreservationErrorCode.E_SYSTEM_EXCEPTION).ThrowsAsFaultException();
            }
            else
            {
                if (!Directory.Exists(workingDir))
                {
                    Directory.CreateDirectory(workingDir);
                }
            }
        }

        internal string CheckPreservationWritableDirectory(objects.Preservation preservation, bool verifyTask, bool isUnique)
        {
            return verifyTask ?
                    Path.Combine(workingDir, VERYFY_DIRECTORY_NAME, preservation.Archive.Name, GetPreservationName(preservation, isUnique)) :
                    Path.Combine(workingDir, preservation.StartDate.GetValueOrDefault().Year.ToString(), preservation.Archive.Name, GetPreservationName(preservation, isUnique));
        }

        public static string GetPreservationName(Objects.Preservation preservation, bool isUnique)
        {
            string fiscalDocumentType = preservation.Archive.FiscalDocumentType;
            if (string.IsNullOrEmpty(fiscalDocumentType))
            {
                fiscalDocumentType = preservation.Archive != null ? preservation.Archive.Name : string.Empty;
            }

            //Normalizzazione Nome
            fiscalDocumentType = fiscalDocumentType.Replace(" ", string.Empty)
                .Replace(":", string.Empty)
                .Replace(@"\", string.Empty)
                .Replace("/", string.Empty)
                .Replace("*", string.Empty)
                .Replace("?", string.Empty)
                .Replace("\"", string.Empty)
                .Replace("<", string.Empty)
                .Replace(">", string.Empty)
                .Replace("|", string.Empty);

            DateTime startDate = preservation.StartDate ?? DateTime.MinValue;
            DateTime endDate = preservation.EndDate ?? DateTime.MaxValue;

            string preservationName = $"{fiscalDocumentType}_Dal_{startDate:dd-MM-yyyy}_al_{endDate:dd-MM-yyyy}";
            if (isUnique)
            {
                preservationName = $"{preservationName}_{Guid.NewGuid()}";
            }
            return preservationName;
        }

        internal void CheckTemplateFile(Company company)
        {
            File.WriteAllText(Path.Combine(workingDir, "CHIUSURA.txt"), company.TemplateCloseFile);
            File.WriteAllText(Path.Combine(workingDir, "INDICE.txt"), company.TemplateIndexFile);
        }

        public void Dispose()
        {
            try
            {
            }
            catch { }
        }


        internal string CheckADETxtFile(PreservationStorageDevice device)
        {
            return Path.Combine(workingDir, device.MinDate.GetValueOrDefault().Year.ToString(), device.Label + ".txt");
        }

        internal string CheckADEFile(PreservationStorageDevice device)
        {
            return Path.Combine(workingDir, device.MinDate.GetValueOrDefault().Year.ToString(), device.Label + ".xml");
        }

        public string WorkingDir()
        {
            return workingDir;
        }
    }
}
