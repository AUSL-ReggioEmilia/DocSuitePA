using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.IO;
using System.ServiceProcess;
using System.Xml.Serialization;
using VecompSoftware.Helpers.Zip;
using Vecompsoftware.FileServer.Services;
using Vecompsoftware.FileServer.Services.ActionMessages;

namespace JeepService.JeepService.DashboardService
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single)]
    public class FileRepositoryService : IFileRepositoryService
    {
        #region Events

        public event FileEventHandler FileRequested;
        public event FileEventHandler FileUploaded;
        public event FileEventHandler FileDeleted;

        #endregion

        #region IFileRepositoryService Members

        /// <summary>
        /// Gets a file from the repository
        /// </summary>
        public Stream GetFile(string virtualPath)
        {
            if (!File.Exists(virtualPath))
                throw new FileNotFoundException("File non trovato", Path.GetFileName(virtualPath));

            SendFileRequested(virtualPath);

            return new FileStream(virtualPath, FileMode.Open, FileAccess.Read);
        }

        /// <summary>
        /// Uploads a file into the repository
        /// </summary>
        public void PutFile(FileUploadMessage msg)
        {
            var filePath = msg.VirtualPath;
            var dir = Path.GetDirectoryName(filePath);

            if (dir != null && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using (var outputStream = new FileStream(filePath, FileMode.Create))
            {
                msg.DataStream.CopyTo(outputStream);
            }

            SendFileUploaded(filePath);
        }

        public ActionMessage ExecuteFile(ActionMessage msg)
        {
            var returnActionMessage = new ActionMessage();
            try
            {
                Process.Start(msg.Message);    
            }
            catch(Exception ex)
            {
                returnActionMessage.Message =
                    String.Format("Errore in fase di esecuzione del file {0}. Errore restituito: {1}", msg.Message,
                                  ex.Message);
            }
            return returnActionMessage;
        }


        /// <summary>
        /// Deletes a file from the repository
        /// </summary>
        public void DeleteFile(string virtualPath)
        {
            var filePath = virtualPath;
            if (!File.Exists(filePath)) return;
            SendFileDeleted(virtualPath);
            File.Delete(filePath);
        }

        /// <summary>
        /// Lists directories from the repository at the specified virtual path.
        /// </summary>
        /// <param name="virtualPath">The virtual path. This can be null to list files from the root of
        /// the repository.</param>
        public List<StorageFileInfo> ListDirectories(string virtualPath)
        {
            var dirInfo = new DirectoryInfo(virtualPath);
            var directories = dirInfo.GetDirectories();

            var elements = (from d in directories
                            select
                                new StorageFileInfo
                                    {
                                        Size = 0,
                                        VirtualPath = d.FullName,
                                        StorageFileType = StorageFileInfo.Type.Directory
                                    }).ToList();
            return elements;
        }

        /// <summary>
        /// Lists files from the repository at the specified virtual path.
        /// </summary>
        /// <param name="virtualPath">The virtual path. This can be null to list files from the root of
        /// the repository.</param>
        /// <param name="filter"> </param>
        public List<StorageFileInfo> ListFiles(string virtualPath, string filter)
        {
            var dirInfo = new DirectoryInfo(virtualPath);
            var files = dirInfo.GetFiles(filter, SearchOption.TopDirectoryOnly);

            var elements = (from f in files
                            select new StorageFileInfo
                            {
                                Name = f.Name,
                                Size = f.Length,
                                VirtualPath = f.FullName,
                                StorageFileType = StorageFileInfo.Type.File,
                            }).ToList();
            return elements;
        }

        public ActionMessage StartService(string serviceName)
        {
            // AVVIO IL SERVIZIO
            var result = new ActionMessage();
            try
            {
                var service = new ServiceController(serviceName);
                if (service.Status == ServiceControllerStatus.Running)
                {
                    result.Message = "Il servizio è già attivo!";
                    return result;
                }
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(60000));
                result.Message = "Il servizio è stato avviato correttamente.";
            }
            catch (Exception ex)
            {
                result.Message = String.Format("Sono occorsi degli errori in fase di avvio del servizio: {0}",
                                               ex.Message);
            }
            return result;
        }

        public ActionMessage StopService(string serviceName)
        {
            // FERMO IL SERVIZIO
            var result = new ActionMessage();
            try
            {
                var service = new ServiceController(serviceName);
                if (service.Status == ServiceControllerStatus.Stopped)
                {
                    result.Message = "Il servizio è già fermo!";
                    return result;
                }
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMilliseconds(60000));
                result.Message = "Il servizio è stato fermato correttamente.";
            }
            catch (Exception ex)
            {
                result.Message = String.Format("Sono occorsi degli errori in fase di fermo del servizio: {0}",
                                               ex.Message);
            }
            return result;
        }

        public ActionMessage RestartService(string serviceName)
        {
            // RIAVVIO IL SERVIZIO
            var result = new ActionMessage();
            result.Message += StopService(serviceName).Message;
            result.Message += "\n" + StartService(serviceName).Message;
            return result;
        }

        public ServiceStatusMessage ServiceStatus(string serviceFullPath, string serviceName)
        {

            var services = ServiceController.GetServices();
            var service = services.FirstOrDefault(s => s.ServiceName == serviceName);
            var result = new ServiceStatusMessage
                             {
                                 Exists = File.Exists(serviceFullPath),
                                 Installed = service != null
                             };
            if (result.Exists)
            {
                var serviceAssemblyFileVersioneInfo = FileVersionInfo.GetVersionInfo(serviceFullPath);

                // Versione del servizio
                result.Version = serviceAssemblyFileVersioneInfo.FileVersion;

                if (result.Installed)
                {
                    // Status del servizio
                    result.Status = new ServiceController(serviceName).Status;
                }
                else result.Message = "Il servizio non è installato";
            }
            else result.Message = "Il servizio non esiste";
            return result;
        }

        public XmlConfigurationMessage LoadXmlConfiguration(string serviceConfigurationFullPath)
        {
            var result = new XmlConfigurationMessage();
            if (!String.IsNullOrEmpty(serviceConfigurationFullPath))
            {
                if (!File.Exists(serviceConfigurationFullPath))
                {
                    throw new FileNotFoundException("Impossibile trovare il file di configurazione JeepConfig.xml.", serviceConfigurationFullPath);
                }
                var serializer = new XmlSerializer(typeof(VecompSoftware.JeepService.Common.Configuration));
                using (Stream stream = new FileStream(serviceConfigurationFullPath, FileMode.Open))
                {
                    result.JeepConfig = (VecompSoftware.JeepService.Common.Configuration)serializer.Deserialize(stream);
                    result.JeepConfigPath = serviceConfigurationFullPath;
                }
            }
            return result;
        }

        public ServiceConfigurationMessage LoadServiceConfiguration(string serviceFullPath)
        {
            var result = new ServiceConfigurationMessage();
            if (!String.IsNullOrEmpty(serviceFullPath))
            {
                var config = ConfigurationManager.OpenExeConfiguration(serviceFullPath);
                result.IsProtected = config.GetSection("appSettings").SectionInformation.IsProtected;
                if (result.IsProtected) DecryptConfigSection("appSettings", config);

                config.SaveAs("ConfigurationManager.dat");
                result.JeepServiceConfig = File.ReadAllBytes("ConfigurationManager.dat");
                File.Delete("ConfigurationManager.dat");
                result.JeepServicePath = serviceFullPath;
            }
            return result;
        }

        public void SaveXmlConfiguration(XmlConfigurationMessage serviceConfiguration)
        {
            if (serviceConfiguration.JeepConfig != null) File.WriteAllText(serviceConfiguration.JeepConfigPath, serviceConfiguration.JeepConfig.ToString());
        }

        public void SaveServiceConfiguration(ServiceConfigurationMessage serviceConfiguration, bool encrypt)
        {
            if (serviceConfiguration.JeepServiceConfig == null) return;
            File.WriteAllBytes("ConfigurationManager.dat", serviceConfiguration.JeepServiceConfig);
            var map = new ExeConfigurationFileMap { ExeConfigFilename = "ConfigurationManager.dat" };
            var configuration = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

            if (encrypt)
            {
                EncryptConfigSection("appSettings", configuration);
            }
            else
            {
                DecryptConfigSection("appSettings", configuration);
            }

            configuration.SaveAs(String.Format("{0}.config", serviceConfiguration.JeepServicePath), ConfigurationSaveMode.Full);
            File.Delete("ConfigurationManager.dat");
        }

        private static void EncryptConfigSection(string sectionKey, System.Configuration.Configuration targetConfig)
        {
            var section = targetConfig.GetSection(sectionKey);
            if (section == null || section.SectionInformation.IsProtected || section.ElementInformation.IsLocked)
            {
                // TODO: esplicitare errore e correzione all'operatore
                return;
            }

            section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
            section.SectionInformation.ForceSave = true;
        }

        private static void DecryptConfigSection(string sectionKey, System.Configuration.Configuration targetConfig)
        {
            var section = targetConfig.GetSection(sectionKey);
            if (section == null || !section.SectionInformation.IsProtected || section.ElementInformation.IsLocked)
            {
                // TODO: esplicitare errore e correzione all'operatore
                return;
            }

            section.SectionInformation.UnprotectSection();
            section.SectionInformation.ForceSave = true;
        }

        public DictionaryMessage LoadModuleAssemblyProperties(string assemblyFullPath)
        {
            var result = new DictionaryMessage();
            if (new FileInfo(assemblyFullPath).Exists)
            {
                var moduleAssemblyFileVersionInfo = FileVersionInfo.GetVersionInfo(assemblyFullPath);
                foreach (var property in moduleAssemblyFileVersionInfo.GetType().GetProperties())
                {
                    result.Properties.Add(property.Name, property.GetValue(moduleAssemblyFileVersionInfo, null).ToString());
                }
            }
            else result.Properties["FileVersion"] = "0.0.0.0";
            
            return result;
        }

        public ActionMessage Update(string updatingObjectName, string zipUpdatePackagePath, ZipExtraction zipExtractionMode, string zipExtractTempPath, string extractionDestinationPath, bool doBackup, string backupPath, string executeCommandFullPath)
        {
            var retvalMessage = new ActionMessage();
            //Gestione Backup
            if (doBackup)
            {
                retvalMessage.Message = BackupFiles(new DirectoryInfo(extractionDestinationPath), new DirectoryInfo(Path.Combine(extractionDestinationPath, backupPath)),
                            String.Format("{0}_{1}.zip", DateTime.Now.ToString("yyyyMMdd_HHmmss"), updatingObjectName)).FullName;
            }

            var zipExtractTemp = new DirectoryInfo(Path.Combine(extractionDestinationPath, zipExtractTempPath));

            //Estraggo i dati caricati
            ExtractFiles(zipUpdatePackagePath, zipExtractTemp, zipExtractionMode);

            //Sposto i file dalla directory temporanea alla directory di destinazione
            MoveFiles(zipExtractTemp, new DirectoryInfo(extractionDestinationPath));

            return retvalMessage;
        }

        private static FileInfo BackupFiles(DirectoryInfo pathToBackup, DirectoryInfo destinationDirectory, string destinationBackupName)
        {
            //Creo la directory dove sarà depositato il file di backup
            destinationDirectory.Create();

            //Definisco dove sarà copiato lo zip di backup
            var destinationZipFile = new FileInfo(Path.Combine(destinationDirectory.FullName, destinationBackupName));

            //Effettuo lo zip della cartella
            ZipManager.CompressFolder(pathToBackup, destinationZipFile);

            //Ritorno il FileInfo dello zip
            return destinationZipFile;
        }

        private static void ExtractFiles(string zipToExtractPath, DirectoryInfo extractionDestinationPath, ZipExtraction zipExtractionMode)
        {
            var zipFiles = ZipManager.Extract(new MemoryStream(File.ReadAllBytes(zipToExtractPath)));
            foreach (var zipItem in zipFiles.Where(zipItem => zipItem.Data.Length > 0))
            {
                var names = zipItem.Filename.Split('/');
                // Carico l'effettivo nome file
                var fileName = names.Length > 0 ? names.Last() : names[0];
                var destinationName = Path.Combine(extractionDestinationPath.FullName, fileName);

                // Definisco come gestire il nesting
                switch (zipExtractionMode)
                {
                    // Estrazione piatta di tutti gli elementi
                    case ZipExtraction.FlatExtraction:
                        {
                            // Niente da fare: in fileName ho già il nome e non serve caricare le cartelle
                        }
                        break;
                    case ZipExtraction.FullDirectoryExtraction:
                        {
                            // Devo caricare tutte le directory (se presenti)
                            if (names.Length > 1)
                            {
                                var fileInfo = new FileInfo(Path.Combine(extractionDestinationPath.FullName, zipItem.Filename.Replace('/', '\\')));
                                if (fileInfo.Directory != null)
                                {
                                    // Creo la directory se non esiste
                                    fileInfo.Directory.Create();
                                    // Aggiorno il path
                                    destinationName = Path.Combine(destinationName, fileInfo.Directory.FullName, fileName);
                                }
                            }
                        }
                        break;
                    case ZipExtraction.FullInternalDirectionExtraction:
                        {
                            // Carico tutte le directory meno che la prima
                            // Devo caricare tutte le directory (se presenti)
                            if (names.Length > 2)
                            {
                                var fileInfo = new FileInfo(Path.Combine(extractionDestinationPath.FullName, zipItem.Filename.Substring(zipItem.Filename.IndexOf('/') + 1).Replace('/', '\\')));
                                if (fileInfo.Directory != null)
                                {
                                    // Creo la directory se non esiste
                                    fileInfo.Directory.Create();
                                    // Aggiorno il path
                                    destinationName = Path.Combine(destinationName, fileInfo.Directory.FullName, fileName);
                                }
                            }
                        }
                        break;
                }
                File.WriteAllBytes(destinationName, zipItem.Data);
            }

            // Elimino lo zip di aggiornamento
            File.Delete(zipToExtractPath);
        }

        private static void MoveFiles(DirectoryInfo sourceDirectory, DirectoryInfo destinationDirectory)
        {
            foreach (var directoryInfo in sourceDirectory.GetDirectories())
            {
                // Ricorro la chiamata sulle directory interne
                var destinationInternalDirectory = new DirectoryInfo(Path.Combine(destinationDirectory.FullName, directoryInfo.Name));
                MoveFiles(directoryInfo, destinationInternalDirectory);

                // Elimino la directory che ho spostato
                directoryInfo.Delete();
            }

            // Copio tutti i file della cartella di aggiornamento
            foreach (var f in sourceDirectory.GetFiles())
            {
                var destinationFile = new FileInfo(Path.Combine(destinationDirectory.FullName, f.Name));
                if (destinationFile.Directory == null) continue;
                
                //Creo la directory di destinazione nel caso non esistesse
                destinationFile.Directory.Create();
                File.Copy(f.FullName, destinationFile.FullName, true);
                File.Delete(f.FullName);
            }           
        }

        #endregion

        #region Events

        /// <summary>
        /// Raises the FileRequested event.
        /// </summary>
        protected void SendFileRequested(string vPath)
        {
            if (FileRequested != null)
                FileRequested(this, new FileEventArgs(vPath));
        }

        /// <summary>
        /// Raises the FileUploaded event
        /// </summary>
        protected void SendFileUploaded(string vPath)
        {
            if (FileUploaded != null)
                FileUploaded(this, new FileEventArgs(vPath));
        }

        /// <summary>
        /// Raises the FileDeleted event.
        /// </summary>
        protected void SendFileDeleted(string vPath)
        {
            if (FileDeleted != null)
                FileDeleted(this, new FileEventArgs(vPath));
        }
        #endregion
    }
}
