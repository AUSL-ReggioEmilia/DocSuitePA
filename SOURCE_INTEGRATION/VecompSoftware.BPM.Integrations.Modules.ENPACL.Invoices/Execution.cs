using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Security.Principal;
using VecompSoftware.BPM.Integrations.Modules.ENPACL.Invoices.Configuration;
using VecompSoftware.BPM.Integrations.Modules.ENPACL.Invoices.Models;
using VecompSoftware.BPM.Integrations.Modules.VSW.ReceivableInvoice.Models;
using VecompSoftware.BPM.Integrations.Services.BiblosDS;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Events.Models.Integrations.GenericProcesses;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Integrations.GenericProcesses;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.Helpers.Compress;
using VecompSoftware.Helpers.XML;
using VecompSoftware.Services.Command.CQRS.Events.Models.Integrations.GenericProcesses;

namespace VecompSoftware.BPM.Integrations.Modules.ENPACL.Invoices
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly ILogger _logger;
        private readonly IServiceBusClient _serviceBusClient;
        private readonly IDocumentClient _documentClient;
        private readonly IWebAPIClient _webAPIClient;
        private bool _needInitializeModule = false;
        private Location _workflowLocation;
        private const string EXTENSION_JSON = ".json";
        private const string EXTENSION_XML = ".xml";
        private readonly string _username;
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(Execution));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        [ImportingConstructor]
        public Execution(ILogger logger, IServiceBusClient serviceBusClient, IDocumentClient documentClient, IWebAPIClient webAPIClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                _documentClient = documentClient;
                _serviceBusClient = serviceBusClient;
                _webAPIClient = webAPIClient;
                _needInitializeModule = true;
                _username = string.Empty;
                if (WindowsIdentity.GetCurrent() != null)
                {
                    _username = WindowsIdentity.GetCurrent().Name;
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("ENPACL.SFTP.PassiveInvoice -> Critical error in costruction module"), ex, LogCategories);
                throw;
            }
        }
        #endregion

        #region [ Methods ]
        protected override void Execute()
        {
            if (Cancel)
            {
                return;
            }

            InitializeModule();

            try
            {
                _logger.WriteInfo(new LogMessage("SFTP.PassiveInvoice -> Starting execution module ..."), LogCategories);
                ConnectionInfo connectionInfo = new ConnectionInfo(_moduleConfiguration.SFTPAddress, _moduleConfiguration.SFTPPort, _moduleConfiguration.SFTPUsername,
                    new PasswordAuthenticationMethod(_moduleConfiguration.SFTPUsername, _moduleConfiguration.SFTPPassword));
                using (SftpClient sftp = new SftpClient(connectionInfo))
                {
                    _logger.WriteInfo(new LogMessage($"SFTP.PassiveInvoice -> Connecting to SFTP server {_moduleConfiguration.SFTPAddress}:{_moduleConfiguration.SFTPPort} ..."), LogCategories);
                    sftp.Connect();
                    _logger.WriteInfo(new LogMessage("SFTP.PassiveInvoice -> Connected successfully"), LogCategories);

                    #region [ Looking invoices ]
                    _logger.WriteInfo(new LogMessage("SFTP.PassiveInvoice -> Looking for passive invoices ..."), LogCategories);

                    foreach (SftpFile sftpFile in sftp.ListDirectory(_moduleConfiguration.SFTPPath).Where(f => !f.IsDirectory && f.Name.StartsWith("FatturePA_") && !f.Name.EndsWith("_metaData")))
                    {
                        string sessionId = string.Format("{0:yyyyMMdd_HHmmss}", DateTime.Now);
                        try
                        {
                            #region [ Preparing invoice file ]
                            _logger.WriteInfo(new LogMessage($"SFTP.PassiveInvoice -> Invoice found: {sftpFile.Name}"), LogCategories);
                            _logger.WriteInfo(new LogMessage($"SFTP.PassiveInvoice -> Create {sessionId} folder in {_moduleConfiguration.FolderInvoiceWorking}"), LogCategories);
                            string workingFolderPath = Path.Combine(_moduleConfiguration.FolderInvoiceWorking, sessionId);
                            Directory.CreateDirectory(workingFolderPath);
                            string invoiceWorkingFilePath = Path.Combine(workingFolderPath, sftpFile.Name);
                            _logger.WriteInfo(new LogMessage($"SFTP.PassiveInvoice -> Evaluating {sftpFile.Name} ..."), LogCategories);
                            using (Stream fileStream = File.Create(invoiceWorkingFilePath))
                            {
                                sftp.DownloadFile(sftpFile.FullName, fileStream);
                            }
                            _logger.WriteInfo(new LogMessage($"SFTP.PassiveInvoice -> Invoice file {sftpFile.Name} has been moved to {workingFolderPath}"), LogCategories);
                            fatture invoice = XmlConvert.DeserializeSafe<fatture>(File.ReadAllText(invoiceWorkingFilePath));
                            if (invoice == null)
                            {
                                _logger.WriteWarning(new LogMessage($"Error during xml deserialization of {sftpFile.Name} file. This file was skipped"), LogCategories);
                                continue;
                            }

                            #region [ Preparing invoice documents ]
                            _logger.WriteInfo(new LogMessage("SFTP.PassiveInvoice -> Preparing to get invoice documents ..."), LogCategories);
                            foreach (fattureDocumento invoiceDocument in invoice.documento)
                            {
                                _logger.WriteInfo(new LogMessage($"SFTP.PassiveInvoice -> Evaluating invoice document {invoiceDocument.nome_file} ..."), LogCategories);
                                string invoiceDocumentWorkingFilePath = Path.Combine(workingFolderPath, invoiceDocument.nome_file);
                                SftpFile invoiceDocumentSftpFile = sftp.ListDirectory(_moduleConfiguration.SFTPPath).FirstOrDefault(f => f.Name == invoiceDocument.nome_file);
                                using (Stream fileStream = File.Create(invoiceDocumentWorkingFilePath))
                                {
                                    sftp.DownloadFile(invoiceDocumentSftpFile.FullName, fileStream);
                                }
                                _logger.WriteInfo(new LogMessage($"SFTP.PassiveInvoice -> Invoice document file {invoiceDocument.nome_file} has been moved to {workingFolderPath}"), LogCategories);
                            }
                            #endregion

                            #region [ Preparing invoice metadatas ]
                            SftpFile sftpMetadataFile = sftp.ListDirectory(_moduleConfiguration.SFTPPath).FirstOrDefault(f =>
                                !f.IsDirectory &&
                                f.Name.StartsWith("FatturePA_") &&
                                f.Name.EndsWith("_medaDato") &&
                                f.Name.Contains(invoice.documento.FirstOrDefault().codice_ente.ToString()));
                            if (sftpMetadataFile != null)
                            {
                                _logger.WriteInfo(new LogMessage($"SFTP.PassiveInvoice -> Invoice metatada found: {sftpMetadataFile.Name}"), LogCategories);
                                string invoiceMetadataWorkingFilePath = Path.Combine(workingFolderPath, sftpMetadataFile.Name);
                                using (Stream fileStream = File.Create(invoiceMetadataWorkingFilePath))
                                {
                                    sftp.DownloadFile(sftpMetadataFile.FullName, fileStream);
                                }
                                _logger.WriteInfo(new LogMessage($"SFTP.PassiveInvoice -> Invoice metadata file {sftpMetadataFile.Name} has been moved to {workingFolderPath}"), LogCategories);
                            }
                            else
                            {
                                _logger.WriteInfo(new LogMessage("SFTP.PassiveInvoice -> Invoice metatada not found. Creating invoice metadata files manually ..."), LogCategories);
                                foreach (fattureDocumento invoiceDocument in invoice.documento)
                                {
                                    metadatiFattura invoiceMetadata = new metadatiFattura
                                    {
                                        metadato = new metadatiFatturaMetadato[2]
                                        {
                                            new metadatiFatturaMetadato
                                            {
                                                nome = InvoiceMetadataKey.ID_FILE,
                                                valore = invoiceDocument.id_flusso_sdi.ToString()
                                            },
                                            new metadatiFatturaMetadato
                                            {
                                                nome = InvoiceMetadataKey.FILE_NAME,
                                                valore = invoiceDocument.nome_file
                                            }
                                        }
                                    };
                                    _logger.WriteInfo(new LogMessage($"SFTP.PassiveInvoice -> Invoice metadata {invoiceDocument.nome_file} has been created"), LogCategories);
                                    string invoiceMetadataFilePath = $"{invoiceDocument.nome_file}_metaDato{EXTENSION_XML}";
                                    string invoiceMetadataWorkingPath = Path.Combine(workingFolderPath, invoiceMetadataFilePath);
                                    File.WriteAllText(invoiceMetadataWorkingPath, XmlConvert.Serialize(invoiceMetadata));
                                    _logger.WriteInfo(new LogMessage($"SFTP.PassiveInvoice -> Invoice metadata {invoiceDocument.nome_file} has been moved to {workingFolderPath}"), LogCategories);
                                }
                            }
                            #endregion

                            #endregion

                            #region [ Archive in biblos ]
                            _logger.WriteInfo(new LogMessage($"SFTP.PassiveInvoice -> Preparing to create the zip archive of invoice {sftpFile.Name} ..."), LogCategories);
                            ZipCompress zipCompress = new ZipCompress();
                            byte[] zipContent = zipCompress.InMemoryCompress(workingFolderPath);
                            _logger.WriteInfo(new LogMessage("SFTP.PassiveInvoice -> Sending the zip archive to biblos ..."), LogCategories);
                            ArchiveDocument biblosDocument = _documentClient.InsertDocumentAsync(new ArchiveDocument()
                            {
                                Archive = _workflowLocation.ProtocolArchive,
                                ContentStream = zipContent,
                                Name = invoice.documento.FirstOrDefault().codice_ente.ToString()
                            }).Result;
                            _logger.WriteInfo(new LogMessage("SFTP.PassiveInvoice -> Zip archive was sent to biblos successfully"), LogCategories);
                            #endregion

                            #region [ Send to service bus ]
                            _logger.WriteInfo(new LogMessage($"SFTP.PassiveInvoice -> Prepare to create EventDematerialisation request for invoice {sftpFile.Name} ..."), LogCategories);
                            Guid correlationId = Guid.NewGuid();
                            DocumentManagementRequestModel documentManagementRequest = new DocumentManagementRequestModel()
                            {
                                Documents = new List<WorkflowReferenceBiblosModel>()
                                {
                                    new WorkflowReferenceBiblosModel()
                                    {
                                        ArchiveChainId = biblosDocument.IdChain,
                                        ArchiveDocumentId = biblosDocument.IdDocument,
                                        ArchiveName = biblosDocument.Name, //probably needs to change
                                        ChainType = ChainType.Miscellanea,
                                        DocumentName = sftpFile.Name
                                    }
                                },
                                RegistrationUser = _username
                            };
                            IEventDematerialisationRequest evt = new EventDematerialisationRequest(correlationId,
                                _moduleConfiguration.TenantName,
                                _moduleConfiguration.TenantId,
                                _moduleConfiguration.TenantAOOId,
                                new IdentityContext(_username),
                                documentManagementRequest);
                            _serviceBusClient.SendEventAsync(evt, _moduleConfiguration.TopicName, null, _moduleConfiguration.EventName).Wait();
                            _logger.WriteInfo(new LogMessage($"SFTP.PassiveInvoice -> EventDematerialisation request was sent successfully"), LogCategories);
                            #endregion

                            #region [ Ending invoice file processing ]
                            if (string.IsNullOrEmpty(_moduleConfiguration.FolderInvoiceBackup))
                            {
                                Directory.Delete(workingFolderPath, true);
                                _logger.WriteInfo(new LogMessage($"SFTP.PassiveInvoice -> Folder {workingFolderPath} was deleted"), LogCategories);
                            }
                            else
                            {
                                Directory.Move(workingFolderPath, Path.Combine(_moduleConfiguration.FolderInvoiceBackup, sessionId));
                                _logger.WriteInfo(new LogMessage($"SFTP.PassiveInvoice -> {workingFolderPath} has been moved to {Path.Combine(_moduleConfiguration.FolderInvoiceBackup, sessionId)}"), LogCategories);
                            }
                            _logger.WriteInfo(new LogMessage($"SFTP.PassiveInvoice -> The processing of invoice {sftpFile.Name} was ended"), LogCategories);
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            _logger.WriteError(new LogMessage($"Error during {sftpFile.Name} evaluation. This file was skipped"), ex, LogCategories);
                            _logger.WriteError(new LogMessage($"ReceivableInvoice {sftpFile.Name} metadata file has been skipped to invalid reason: {ex.Message}"), LogCategory.NotifyToEmailCategory);
                            try
                            {
                                Directory.Move(Path.Combine(_moduleConfiguration.FolderInvoiceWorking, sessionId),
                                    Path.Combine(_moduleConfiguration.FolderInvoiceError, sessionId));
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    Directory.Move(Path.Combine(_moduleConfiguration.FolderInvoiceWorking, sessionId),
                                        Path.Combine(_moduleConfiguration.FolderInvoiceError, sessionId));
                                }
                                catch (Exception)
                                {
                                    _logger.WriteWarning(new LogMessage($"Error occouring {sftpFile.Name} in moving file to rejected folder"), ex, LogCategories);
                                }
                            }
                        }
                    }
                    _logger.WriteInfo(new LogMessage("SFTP.PassiveInvoice -> Disconnecting from SFTP server  ..."), LogCategories);
                    sftp.Disconnect();
                    _logger.WriteInfo(new LogMessage("SFTP.PassiveInvoice -> Disconnected successfully"), LogCategories);
                    _logger.WriteInfo(new LogMessage("SFTP.PassiveInvoice -> Exiting ..."), LogCategories);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("SFTP.PassiveInvoice -> critical error"), ex, LogCategories);
                throw;
            }
        }

        protected override void OnStop()
        {
            _logger.WriteInfo(new LogMessage("OnStop -> ENPACL.SFTP.PassiveInvoice"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                int? workflowLocationId = _webAPIClient.GetParameterWorkflowLocationIdAsync().Result;
                if (!workflowLocationId.HasValue)
                {
                    throw new ArgumentNullException("Parameter WorkflowLocationId is not defined");
                }
                _workflowLocation = _webAPIClient.GetLocationAsync(workflowLocationId.Value).Result.Single();

                _needInitializeModule = false;
            }
        }
        #endregion
    }
}
