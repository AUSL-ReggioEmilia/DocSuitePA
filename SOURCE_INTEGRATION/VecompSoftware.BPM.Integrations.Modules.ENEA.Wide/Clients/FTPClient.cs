using FluentFTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.ENEA.Wide.Configurations;
using VecompSoftware.BPM.Integrations.Modules.ENEA.Wide.Models;
using VecompSoftware.BPM.Integrations.Services.BiblosDS;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using BiblosDocument = VecompSoftware.BPM.Integrations.Services.BiblosDS.DocumentService.Document;

namespace VecompSoftware.BPM.Integrations.Modules.ENEA.Wide.Clients
{
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class FTPClient
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDocumentClient _documentClient;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(WideClient));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]

        public FTPClient(IDocumentClient documentClient, ILogger logger)
        {
            _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
            _logger = logger;
            _documentClient = documentClient;
        }

        #endregion

        #region [ Methods ]
        public bool DirectoryExist(string directoryName)
        {
            _logger.WriteDebug(new LogMessage($"Checking FTP folder {directoryName} exists ..."), LogCategories);
            using (FtpClient client = new FtpClient(_moduleConfiguration.FTPUrl,
                new NetworkCredential(_moduleConfiguration.FTPUsername, _moduleConfiguration.FTPPassword)))
            {
                client.Connect();
                return client.DirectoryExists(directoryName);
            }
        }
        public void CreateDirectory(string directoryName)
        {
            _logger.WriteDebug(new LogMessage($"Creating FTP folder {directoryName} ..."), LogCategories);
            using (FtpClient client = new FtpClient(_moduleConfiguration.FTPUrl,
                new NetworkCredential(_moduleConfiguration.FTPUsername, _moduleConfiguration.FTPPassword)))
            {
                client.Connect();
                client.CreateDirectory(directoryName);
                _logger.WriteDebug(new LogMessage($"FTP folder {_moduleConfiguration.FTPUrl}/{directoryName} has been successfully created"), LogCategories);
            }
        }

        public async Task StoreFileAsync(string directoryName, BiblosDocument document)
        {
            string destinationFilename = $"{directoryName}/{document.Name}";
            Services.BiblosDS.DocumentService.Content content = await _documentClient.GetDocumentContentByIdAsync(document.IdDocument).ConfigureAwait(false);
            UploadFile(destinationFilename, content.Blob);
        }

        public void StoreFile(string directoryName, string filename, byte[] content)
        {
            string destinationFilename = $"{directoryName}/{filename}";
            UploadFile(destinationFilename, content);
        }

        private void UploadFile(string destinationFilename, byte[] content)
        {
            _logger.WriteDebug(new LogMessage($"Upload file to FTP {destinationFilename} folder ..."), LogCategories);
            using (FtpClient client = new FtpClient(_moduleConfiguration.FTPUrl,
                new NetworkCredential(_moduleConfiguration.FTPUsername, _moduleConfiguration.FTPPassword)))
            {
                client.Connect();
                if (!client.Upload(content, destinationFilename, FtpExists.Overwrite,
                    progress: (p) => _logger.WriteDebug(new LogMessage($"Uploading {p.Progress} ..."), LogCategories)))
                {
                    throw new InvalidOperationException($"Error occoured during uploading file to FTP {destinationFilename} folder");
                }
                _logger.WriteDebug(new LogMessage($"File {destinationFilename} has been successfully uploaded"), LogCategories);
            }
        }

        #endregion

    }
}
