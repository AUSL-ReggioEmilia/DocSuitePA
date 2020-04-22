using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;
using Vecompsoftware.FileServer.Services;
using Vecompsoftware.FileServer.Services.ActionMessages;

namespace VecompSoftware.JeepDashboard.Remote
{
    public class FileRepositoryServiceClient : ClientBase<IFileRepositoryService>, IFileRepositoryService, IDisposable
    {
        public FileRepositoryServiceClient()
            : this("FileRepositoryService", new EndpointAddress(ConfigurationManager.AppSettings["LiveUpdateServiceAddress"]))
        {
        }

        public FileRepositoryServiceClient(string endPointConfigurationName, EndpointAddress endPointRemoteAddress)
            : base(endPointConfigurationName, endPointRemoteAddress)
        {
        }

        #region IFileRepositoryService Members

        public System.IO.Stream GetFile(string virtualPath)
        {
            return Channel.GetFile(virtualPath);
        }

        public void PutFile(FileUploadMessage msg)
        {
            Channel.PutFile(msg);
        }

        public ActionMessage ExecuteFile(ActionMessage msg)
        {
            return Channel.ExecuteFile(msg);
        }

        public void DeleteFile(string virtualPath)
        {
            Channel.DeleteFile(virtualPath);
        }

        public List<StorageFileInfo> ListDirectories(string virtualPath)
        {
            return Channel.ListDirectories(virtualPath);
        }

        public List<StorageFileInfo> ListFiles(string virtualPath, string filter)
        {
            return Channel.ListFiles(virtualPath, filter);
        }

        public ActionMessage StartService(string serviceName)
        {
            return Channel.StartService(serviceName);
        }

        public ActionMessage StopService(string serviceName)
        {
            return Channel.StopService(serviceName);
        }

        public ActionMessage RestartService(string serviceName)
        {
            return Channel.RestartService(serviceName);
        }

        public ServiceStatusMessage ServiceStatus(string serviceFullPath, string serviceName)
        {
            return Channel.ServiceStatus(serviceFullPath, serviceName);
        }

        public XmlConfigurationMessage LoadXmlConfiguration(string serviceConfigurationFullPath)
        {
            return Channel.LoadXmlConfiguration(serviceConfigurationFullPath);
        }

        public void SaveXmlConfiguration(XmlConfigurationMessage serviceConfiguration)
        {
            Channel.SaveXmlConfiguration(serviceConfiguration);
        }

        public ServiceConfigurationMessage LoadServiceConfiguration(string serviceFullPath)
        {
            return Channel.LoadServiceConfiguration(serviceFullPath);
        }

        public void SaveServiceConfiguration(ServiceConfigurationMessage serviceConfiguration, bool encrypt)
        {
            Channel.SaveServiceConfiguration(serviceConfiguration, encrypt);
        }

        public DictionaryMessage LoadModuleAssemblyProperties(string assemblyFullPath)
        {
            return Channel.LoadModuleAssemblyProperties(assemblyFullPath);
        }

        public ActionMessage Update(string updatingObjectName, string zipUpdatePackagePath, ZipExtraction zipExtractionMode, string zipExtractTempPath, string extractionDestinationPath, bool doBackup, string backupPath, string executeCommandFullPath)
        {
            return Channel.Update(updatingObjectName, zipUpdatePackagePath, zipExtractionMode, zipExtractTempPath,
                                  extractionDestinationPath, doBackup, backupPath, executeCommandFullPath);
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (State == CommunicationState.Opened)
                Close();
        }

        #endregion
    }
}
