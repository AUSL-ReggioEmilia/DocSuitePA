using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BiblosDS.Cloud;
using BiblosDS.Library.Common.Services;
using objects = BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Objects.Response;
using BiblosDS.Library.Common.Enums;
using System.IO;
using BiblosDS.Library.Common.Objects;
using System.Text.RegularExpressions;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.ServiceRuntime;
using BiblosDS.Library.Common.Utility;
using log4net;

namespace BiblosDS.Library.Common.Preservation.Services
{
    public class PreservationFileManager : IDisposable
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(PreservationFileManager));
        const string VERYFY_DIRECTORY_NAME = "verify";

        DocumentArchive archive;
        Company company;
        CloudDriveManager manager = null;
        bool isOnAzure = false;
        string workingDir = "";

        public PreservationFileManager(DocumentArchive archive, Company company)
        {
            this.archive = archive;
            this.company = company;
            if (AzureService.IsAvailable)
            {
                isOnAzure = true;
                manager = new CloudDriveManager();
            }
            else
            {
                workingDir = archive.PathPreservation;
            }
        }

        public void Empty(System.IO.DirectoryInfo directory)
        {
            foreach (System.IO.FileInfo file in directory.GetFiles()) file.Delete();
            foreach (System.IO.DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
        }

        internal void Check(int totalSize)
        {
            if (isOnAzure)
            {
                bool delete = false;
                if (RoleEnvironment.GetConfigurationSettingValue("Preservation_SingleVHD").Contains("true"))
                {
                    delete = true;
                    manager.CreateDrive(company.IdCompany.ToString(), string.Format("{0}.vhd", ParseAzureStorageName(archive.PathPreservation.ToStringExt() + company.CompanyName)), totalSize, true);
                }
                else
                    manager.CreateDrive(company.IdCompany.ToString(), string.Format("{0}.vhd", ParseAzureStorageName(archive.PathPreservation.ToStringExt() + company.CompanyName)), CloudDriveManager.DISK_SIZE, false);
                workingDir = manager.Mount();
                try
                {
                    if (delete)
                    {
                        _logger.InfoFormat("Eliminazione files");
                        Empty(new DirectoryInfo(workingDir));
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            }
            else
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
        }

        internal static bool CheckSize(long totalSize, bool isOnAzure)
        {
            if (isOnAzure)
            {
                if (totalSize > (CloudDriveManager.DISK_SIZE * 1024))
                {
                    return false;
                }
            }
            return true;
        }

        internal string CheckPreservationDirectory(objects.Preservation preservation, bool verifyTask)
        {
            if (isOnAzure)
                return string.Format("{0}", manager.VHD_Url);
            else
                return CheckPreservationWritableDirectory(preservation, verifyTask);
        }

        internal string CheckPreservationWritableDirectory(objects.Preservation preservation, bool verifyTask)
        {
            return verifyTask ?
                    Path.Combine(workingDir, VERYFY_DIRECTORY_NAME, preservation.Archive.Name, GetPreservationName(preservation)) :
                    Path.Combine(workingDir, preservation.StartDate.GetValueOrDefault().Year.ToString(), preservation.Archive.Name, GetPreservationName(preservation));
        }

        public static string GetPreservationName(Objects.Preservation preservation)
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

            return $"{fiscalDocumentType}_Dal_{startDate:dd-MM-yyyy}_al_{endDate:dd-MM-yyyy}";
        }

        public static string ParseAzureStorageName(string name)
        {
            var res = Regex.Replace(name, @"[^A-Za-z0-9]+", "_");
            return res.Substring(0, Math.Min(res.Length, 60));
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
                if (isOnAzure)
                    manager.Unmount();
            }
            catch { }
        }

        internal string CheckADEFile()
        {
            throw new NotImplementedException();
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
