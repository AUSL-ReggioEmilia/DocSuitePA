using System.Collections.Generic;
using System.ServiceModel;
using System.IO;
using Vecompsoftware.FileServer.Services.ActionMessages;

namespace Vecompsoftware.FileServer.Services
{
    [ServiceContract]
	public interface IFileRepositoryService
	{
		[OperationContract]
		Stream GetFile(string virtualPath);

		[OperationContract]
		void PutFile(FileUploadMessage msg);

		[OperationContract]
		void DeleteFile(string virtualPath);

		[OperationContract]
		List<StorageFileInfo> ListDirectories(string virtualPath);

        [OperationContract]
        List<StorageFileInfo> ListFiles(string virtualPath, string filter);

	    [OperationContract]
        ActionMessage StartService(string serviceName);

        [OperationContract]
        ActionMessage StopService(string serviceName);

        [OperationContract]
        ActionMessage RestartService(string serviceName);

	    [OperationContract]
	    ServiceStatusMessage ServiceStatus(string serviceFullPath, string serviceName);

        [OperationContract]
        XmlConfigurationMessage LoadXmlConfiguration(string serviceConfigurationFullPath);

        [OperationContract]
        void SaveXmlConfiguration(XmlConfigurationMessage serviceConfiguration);

        [OperationContract]
        ServiceConfigurationMessage LoadServiceConfiguration(string serviceFullPath);

        [OperationContract]
        void SaveServiceConfiguration(ServiceConfigurationMessage serviceConfiguration, bool encrypt);

	    [OperationContract]
	    DictionaryMessage LoadModuleAssemblyProperties(string assemblyFullPath);

        [OperationContract]
        ActionMessage Update(string updatingObjectName, string zipUpdatePackagePath, ZipExtraction zipExtractionMode, string zipExtractTempPath, string extractionDestinationPath, bool doBackup, string backupPath, string executeCommandFullPath);

        [OperationContract]
        ActionMessage ExecuteFile(ActionMessage msg);
	}

    public enum ZipExtraction
    {
        FlatExtraction,
        FullDirectoryExtraction,
        FullInternalDirectionExtraction
    }
}
