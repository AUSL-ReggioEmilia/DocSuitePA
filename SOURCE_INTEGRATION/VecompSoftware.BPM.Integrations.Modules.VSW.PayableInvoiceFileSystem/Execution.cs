using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.VSW.PayableInvoiceFileSystem.Configuration;
using VecompSoftware.BPM.Integrations.Modules.VSW.PayableInvoiceFileSystem.Helpers;
using VecompSoftware.BPM.Integrations.Modules.VSW.PayableInvoiceFileSystem.Models;
using VecompSoftware.BPM.Integrations.Services.BiblosDS;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.SignServices;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.BPM.Integrations.Services.WebAPI.Models;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS;
using VecompSoftware.Core.Command.CQRS.Commands.Models.Messages;
using VecompSoftware.Core.Command.CQRS.Commands.Models.UDS;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.DocumentGenerator;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.PECMails;
using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.DocSuiteWeb.Model.Workflow.Actions;
using VecompSoftware.Helpers.EInvoice.EInvoice1_2;
using VecompSoftware.Helpers.EInvoice.Models;
using VecompSoftware.Helpers.EInvoice.UDS.Models;
using VecompSoftware.Helpers.PEC.PA;
using VecompSoftware.Helpers.PEC.PA.Models;
using VecompSoftware.Helpers.UDS;
using VecompSoftware.Helpers.Workflow;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS.Events.Entities.PECMails;
using VecompSoftware.Services.Command.CQRS.Events.Models.PECMails;
using ComunicationType = VecompSoftware.DocSuiteWeb.Model.Entities.Commons.ComunicationType;
using Content = VecompSoftware.BPM.Integrations.Services.BiblosDS.DocumentService.Content;
using DSWEnvironmentType = VecompSoftware.DocSuiteWeb.Model.Entities.Commons.DSWEnvironmentType;
using ProtocolRoleNoteType = VecompSoftware.DocSuiteWeb.Model.Entities.Protocols.ProtocolRoleNoteType;
using ProtocolRoleStatus = VecompSoftware.DocSuiteWeb.Model.Entities.Protocols.ProtocolRoleStatus;
using ProtocolTypology = VecompSoftware.DocSuiteWeb.Model.Entities.Protocols.ProtocolTypology;
using UDSEInvoiceHelper = VecompSoftware.Helpers.EInvoice.UDS.EInvoice1_2.EInvoiceHelper;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.PayableInvoiceFileSystem
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
        private readonly ISignServiceClient _signServiceClient;
        private readonly IWebAPIClient _webAPIClient;
        private readonly IList<Guid> _subscriptions = new List<Guid>();
        private bool _needInitializeModule = false;
        private Location _workflowLocation;
        private readonly TimeSpan _threadWaiting = TimeSpan.FromSeconds(5);
        private readonly TimeSpan _threadWaitingFilesystem = TimeSpan.Zero;
        private const string LOOKIN_EXTENSION_P7M = "*.p7m";
        private const string LOOKIN_EXTENSION_XML = "*.xml";
        private const string EXTENSION_P7M = ".p7m";
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
        public Execution(ILogger logger, IServiceBusClient serviceBusClient, IDocumentClient documentClient, IWebAPIClient webAPIClient,
            ISignServiceClient signServiceClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                if (_moduleConfiguration.WaitingEnabled && _moduleConfiguration.WaitingSecondDuration > 0 && _moduleConfiguration.WaitingSecondDuration < 60)
                {
                    _logger.WriteInfo(new LogMessage($"Waiting {_moduleConfiguration.WaitingSecondDuration} seconds enabled"), LogCategories);
                    _threadWaitingFilesystem = TimeSpan.FromSeconds(_moduleConfiguration.WaitingSecondDuration);
                }
                _documentClient = documentClient;
                _serviceBusClient = serviceBusClient;
                _webAPIClient = webAPIClient;
                _signServiceClient = signServiceClient;
                _username = string.Empty;
                if (WindowsIdentity.GetCurrent() != null)
                {
                    _username = WindowsIdentity.GetCurrent().Name;
                }
                _needInitializeModule = true;
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("VSW.PayableInvoiceFileSystem -> Critical error in costruction module"), ex, LogCategories);
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
                WorkflowConfiguration workflowConfiguration = null;
                List<FileInfo> resultFiles;
                List<FileInfo> foundedFiles;
                string workingPath = string.Empty;
                string workingPathP7M = string.Empty;
                string workingPathJson = string.Empty;
                FileInfo jsonMetadata = null;
                string sessionId = string.Format("{0:yyyyMMdd_HHmmss}", DateTime.Now);
                InvoiceMetadata invoiceMetadata;
                #region [ Looking invoces ] 
                foreach (KeyValuePair<string, WorkflowConfiguration> item in _moduleConfiguration.WorkflowConfigurations.Where(f => !string.IsNullOrEmpty(f.Value.FolderLookingInvoice)))
                {
                    try
                    {
                        workflowConfiguration = item.Value;
                        _logger.WriteInfo(new LogMessage($"PayableInvoiceFileSystemWorkflow -> evaluating {item.Value} configuration in folder {workflowConfiguration.FolderLookingInvoice}"), LogCategories);
                        resultFiles = new DirectoryInfo(workflowConfiguration.FolderLookingInvoice).EnumerateFiles(LOOKIN_EXTENSION_XML, SearchOption.AllDirectories).ToList();
                        _logger.WriteInfo(new LogMessage($"PayableInvoiceFileSystemWorkflow -> found {resultFiles.Count()} xml files"), LogCategories);
                        foundedFiles = new DirectoryInfo(workflowConfiguration.FolderLookingInvoice).EnumerateFiles(LOOKIN_EXTENSION_P7M, SearchOption.AllDirectories).ToList();
                        resultFiles.AddRange(foundedFiles);
                        _logger.WriteInfo(new LogMessage($"PayableInvoiceFileSystemWorkflow -> found {foundedFiles.Count()} p7m files"), LogCategories);

                        workingPath = string.Empty;
                        workingPathP7M = string.Empty;
                        foreach (FileInfo fileInfo in resultFiles)
                        {
                            try
                            {
                                if (_moduleConfiguration.WaitingEnabled && _moduleConfiguration.WaitingSecondDuration > 0 && _threadWaitingFilesystem > TimeSpan.Zero)
                                {
                                    _logger.WriteInfo(new LogMessage($"Waiting {_moduleConfiguration.WaitingSecondDuration} seconds ...."), LogCategories);
                                    Task.Delay(_threadWaitingFilesystem).Wait();
                                }
                                _logger.WriteInfo(new LogMessage($"Evaluating {fileInfo.FullName} ...."), LogCategories);
                                jsonMetadata = new FileInfo(Path.Combine(fileInfo.Directory.FullName, $"{Path.GetFileNameWithoutExtension(fileInfo.FullName)}.json"));
                                _logger.WriteInfo(new LogMessage($"Evaluating metadata {jsonMetadata.FullName} ...."), LogCategories);

                                workingPath = Path.Combine(workflowConfiguration.FolderWorkingInvoice, fileInfo.Name);
                                workingPathP7M = $"{workingPath}{EXTENSION_P7M}";
                                workingPathJson = $"{workingPath.Replace(EXTENSION_XML, string.Empty)}.json";
                                File.Move(fileInfo.FullName, workingPath);
                                _logger.WriteInfo(new LogMessage($"{fileInfo.FullName} has been moved to {workingPath}"), LogCategories);
                                if (jsonMetadata.Exists)
                                {
                                    _logger.WriteInfo(new LogMessage($"Metadata {jsonMetadata.FullName} has been founded"), LogCategories);
                                    File.Move(jsonMetadata.FullName, workingPathJson);
                                }
                                WorkflowStartPayableInvoiceAsync(workflowConfiguration, workingPath, workingPathJson).Wait();
                                if (string.IsNullOrEmpty(workflowConfiguration.FolderBackupInvoice))
                                {
                                    if (File.Exists(workingPathP7M))
                                    {
                                        File.Delete(workingPathP7M);
                                    }
                                    if (File.Exists(workingPathJson))
                                    {
                                        File.Delete(workingPathJson);
                                    }
                                    File.Delete(workingPath);
                                }
                                else
                                {
                                    if (!Directory.Exists(Path.Combine(workflowConfiguration.FolderBackupInvoice, sessionId)))
                                    {
                                        Directory.CreateDirectory(Path.Combine(workflowConfiguration.FolderBackupInvoice, sessionId));
                                    }
                                    File.Move(workingPath, Path.Combine(workflowConfiguration.FolderBackupInvoice, sessionId, fileInfo.Name));
                                    _logger.WriteInfo(new LogMessage($"{workingPath} has been moved to {Path.Combine(workflowConfiguration.FolderBackupInvoice, sessionId, fileInfo.Name)}"), LogCategories);
                                    if (File.Exists(workingPathP7M))
                                    {
                                        string destinationBackup = Path.Combine(workflowConfiguration.FolderBackupInvoice, sessionId, $"{fileInfo.Name}{EXTENSION_P7M}");
                                        File.Move(workingPathP7M, destinationBackup);
                                        _logger.WriteInfo(new LogMessage($"{workingPath} has been moved to {destinationBackup}"), LogCategories);
                                    }
                                    if (File.Exists(workingPathJson))
                                    {
                                        string destinationBackup = Path.Combine(workflowConfiguration.FolderBackupInvoice, sessionId, $"{fileInfo.Name.Replace(EXTENSION_XML, string.Empty)}.json");
                                        File.Move(workingPathJson, destinationBackup);
                                        _logger.WriteInfo(new LogMessage($"{workingPath} has been moved to {destinationBackup}"), LogCategories);
                                    }
                                }
                                if (_moduleConfiguration.DeleteLookingDirectoryEnabled && !fileInfo.Directory.FullName.Equals(workflowConfiguration.FolderLookingInvoice) && fileInfo.Directory.Exists &&
                                    !fileInfo.Directory.EnumerateFiles().Any() && !fileInfo.Directory.EnumerateDirectories().Any())
                                {
                                    Directory.Delete(fileInfo.Directory.FullName);
                                    _logger.WriteInfo(new LogMessage($"{fileInfo.Directory.FullName} has been deleted."), LogCategories);
                                }

                                _logger.WriteInfo(new LogMessage($"{fileInfo.FullName} has been successfully completed."), LogCategories);
                            }
                            catch (Exception ex)
                            {
                                _logger.WriteError(new LogMessage($"Error during {fileInfo.FullName} evaluation. This file was skipped"), ex, LogCategories);
                                string path = Path.Combine(workflowConfiguration.FolderRejectedInvoice, sessionId);
                                string jsonFile = Path.Combine(path, $"{fileInfo.Name}.json");
                                _logger.WriteDebug(new LogMessage($"Writing rejected json file {jsonFile} ..."), LogCategories);
                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }
                                try
                                {
                                    File.Move(workingPath, Path.Combine(path, fileInfo.Name));
                                    string rejected = JsonConvert.SerializeObject(new
                                    {
                                        Filename = fileInfo.Name,
                                        RejectedError = ex.InnerException == null ? ex.Message : ex.InnerException.Message,
                                        RejectedDate = DateTimeOffset.UtcNow
                                    });

                                    File.WriteAllText(jsonFile, rejected);
                                }
                                catch (Exception exi)
                                {
                                    _logger.WriteError(new LogMessage($"Error in rejected {fileInfo.FullName} management "), exi, LogCategories);
                                    try
                                    {
                                        File.Move(workingPath, Path.Combine(workflowConfiguration.FolderRejectedInvoice, sessionId, fileInfo.Name));
                                        if (File.Exists(workingPathP7M))
                                        {
                                            File.Move(workingPathP7M, Path.Combine(workflowConfiguration.FolderRejectedInvoice, sessionId, $"{fileInfo.Name}{EXTENSION_P7M}"));
                                        }
                                        if (File.Exists(workingPathJson))
                                        {
                                            File.Move(workingPathJson, Path.Combine(workflowConfiguration.FolderRejectedInvoice, sessionId, $"{fileInfo.Name.Replace(EXTENSION_XML, string.Empty)}.json"));
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        _logger.WriteWarning(new LogMessage($"Error occouring {fileInfo.FullName} in moving file to rejected folder"), ex, LogCategories);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteError(new LogMessage($"Error occoured in {item.Key} evalation. This configuration was skipped and are going to be process next configuration"), ex, LogCategories);
                    }
                }
                #endregion

                #region [ Looking metadatas ] 
                foreach (KeyValuePair<string, WorkflowConfiguration> item in _moduleConfiguration.WorkflowConfigurations.Where(f => !string.IsNullOrEmpty(f.Value.FolderLookingMetadata)))
                {
                    workflowConfiguration = item.Value;
                    resultFiles = new DirectoryInfo(workflowConfiguration.FolderLookingMetadata).EnumerateFiles("*.json", SearchOption.AllDirectories).ToList();
                    _logger.WriteInfo(new LogMessage($"Looking metadata-> found {resultFiles.Count()} json file in path {workflowConfiguration.FolderLookingMetadata}"), LogCategories);
                    foreach (FileInfo fileInfo in resultFiles)
                    {
                        workingPath = string.Empty;
                        try
                        {
                            _logger.WriteInfo(new LogMessage($"Evaluating {fileInfo.FullName} ...."), LogCategories);
                            workingPath = Path.Combine(workflowConfiguration.FolderWorkingMetadata, fileInfo.Name);
                            if (File.Exists(workingPath))
                            {
                                _logger.WriteWarning(new LogMessage($"The metadata {fileInfo.Name} already exists in directory {workflowConfiguration.FolderWorkingMetadata}. The file was moved to rejected folder."), LogCategories);
                                if (File.Exists(Path.Combine(workflowConfiguration.FolderRejectedMetadata, fileInfo.Name)))
                                {
                                    File.Delete(fileInfo.FullName);
                                    continue;
                                }
                                File.Move(fileInfo.FullName, Path.Combine(workflowConfiguration.FolderRejectedMetadata, fileInfo.Name));
                                continue;
                            }
                            File.Move(fileInfo.FullName, workingPath);
                            _logger.WriteInfo(new LogMessage($"{fileInfo.FullName} has been moved to {workingPath}"), LogCategories);
                            try
                            {
                                _logger.WriteDebug(new LogMessage($"Tentative metadata update {workflowConfiguration.InvoiceTypes.First().Value.UDSRepositoryName}"), LogCategories);
                                invoiceMetadata = new InvoiceMetadata(workingPath, workflowConfiguration.InvoiceTypes.First().Value.UDSRepositoryName, JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(workingPath)));
                                UpdateInvoiceMetadataAsync(invoiceMetadata).Wait();
                            }
                            catch (Exception iex)
                            {
                                _logger.WriteWarning(new LogMessage(iex.Message), iex, LogCategories);
                                _logger.WriteDebug(new LogMessage($"Tentative metadata update {workflowConfiguration.InvoiceTypes.Last().Value.UDSRepositoryName}"), LogCategories);
                                invoiceMetadata = new InvoiceMetadata(workingPath, workflowConfiguration.InvoiceTypes.Last().Value.UDSRepositoryName, JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(workingPath)));
                                UpdateInvoiceMetadataAsync(invoiceMetadata).Wait();
                            }
                            if (string.IsNullOrEmpty(workflowConfiguration.FolderBackupMetadata))
                            {
                                File.Delete(workingPath);
                            }
                            else
                            {
                                File.Move(workingPath, Path.Combine(workflowConfiguration.FolderBackupMetadata, fileInfo.Name));
                                _logger.WriteInfo(new LogMessage($"{workingPath} has been moved to {Path.Combine(workflowConfiguration.FolderBackupMetadata, fileInfo.Name)}"), LogCategories);
                            }
                            if (!fileInfo.Directory.FullName.Equals(workflowConfiguration.FolderLookingMetadata, StringComparison.InvariantCultureIgnoreCase))
                            {
                                fileInfo.Directory.Delete();
                                _logger.WriteInfo(new LogMessage($"{fileInfo.Directory.FullName} has been deleted"), LogCategories);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.WriteError(new LogMessage($"Error during {fileInfo.FullName} evaluation. This file was skipped"), ex, LogCategories);
                            _logger.WriteError(new LogMessage($"PayableInvoice {fileInfo.FullName} metadata file has been skipped to invalid reason: {ex.Message}"), LogCategory.NotifyToEmailCategory);
                            try
                            {
                                if (File.Exists(Path.Combine(workflowConfiguration.FolderRejectedMetadata, fileInfo.Name)))
                                {
                                    _logger.WriteWarning(new LogMessage($"The metadata {fileInfo.Name} already exists in directory {workflowConfiguration.FolderRejectedMetadata}. The file was skipped."), LogCategories);
                                    CommandBuildMessage command = new CommandBuildMessage(
                                        tenantName: _moduleConfiguration.TenantName,
                                        tenantId: _moduleConfiguration.TenantId,
                                        tenantAOOId: Guid.Empty,
                                        new IdentityContext(_username),
                                        MessageHelpers.CreateMessageBuildModel(_moduleConfiguration, $"Il file di metadata {fileInfo.Name} risulta già presente nella directory {workflowConfiguration.FolderRejectedMetadata} e non è stato spostato"));

                                    _webAPIClient.SendCommandAsync<CommandBuildMessage>(command).Wait();

                                    if (File.Exists(fileInfo.FullName))
                                    {
                                        File.Delete(fileInfo.FullName);
                                    }
                                    
                                    if (File.Exists(workingPath))
                                    {
                                        File.Delete(workingPath);
                                    }
                                    continue;
                                }
                                File.Move(fileInfo.FullName, Path.Combine(workflowConfiguration.FolderRejectedMetadata, fileInfo.Name));
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    if (File.Exists(workingPath))
                                    {
                                        File.Move(workingPath, Path.Combine(workflowConfiguration.FolderRejectedMetadata, fileInfo.Name));
                                    }                                    
                                }
                                catch (Exception exx)
                                {
                                    _logger.WriteWarning(new LogMessage($"Error occouring {fileInfo.FullName} in moving file to rejected folder"), exx, LogCategories);
                                }
                            }
                        }
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("PayableInvoiceFileSystem -> critical error"), ex, LogCategories);
                throw;
            }
        }

        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> VSW.PayableInvoiceFileSystem"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _subscriptions.Add(_serviceBusClient.StartListening<IEventReceivedReceiptPECMail>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration,
                    _moduleConfiguration.WorkflowStartUpdateReceiptMetadataInvoiceSubscription, WorkflowPECMailReceiptCallback));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCreatePECMail>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration,
                    _moduleConfiguration.WorkflowStartUpdateMetadataInvoiceSubscription, WorkflowCreatePECMailCallback));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCompletePECMailBuild>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicBuilderEvent,
                    _moduleConfiguration.WorkflowPayableInvoicePECMailBuildCompleteSubscription, CompletePECMailBuildCallback));

                int? workflowLocationId = _webAPIClient.GetParameterWorkflowLocationIdAsync().Result;
                if (!workflowLocationId.HasValue)
                {
                    throw new ArgumentNullException("Parameter WorkflowLocationId is not defined");
                }
                _workflowLocation = _webAPIClient.GetLocationAsync(workflowLocationId.Value).Result.Single();

                _needInitializeModule = false;
            }
        }

        internal void CleanSubscriptions()
        {
            foreach (Guid item in _subscriptions)
            {
                _serviceBusClient.CloseListeningAsync(item).Wait();
            }
            _subscriptions.Clear();
            _needInitializeModule = true;
        }

        #region [ WorkflowStartReceivableInvoiceCallback ]
        private async Task WorkflowStartPayableInvoiceAsync(WorkflowConfiguration workflowConfiguration, string invoiceFileName, string metadataFileName)
        {

            FileInfo metadataFile = new FileInfo(metadataFileName);
            _logger.WriteDebug(new LogMessage($"WorkflowStartPayableInvoice -> evaluate {invoiceFileName} and metadata {metadataFileName} exist {metadataFile.Exists}"), LogCategories);

            try
            {

                InvoiceConfiguration invoiceConfiguration = null;
                XMLModelKind xmlModelKind = XMLModelKind.Invalid;
                List<Role> roles = new List<Role>();
                bool isInvoice = false;
                string xmlContent = string.Empty;
                if (invoiceFileName.EndsWith(EXTENSION_P7M))
                {
                    xmlContent = EInvoiceHelper.TryGetInvoiceSignedContent(File.ReadAllBytes(invoiceFileName), (f, ex) => _logger.WriteWarning(new LogMessage(f), ex, LogCategories));
                }
                else
                {
                    xmlContent = File.ReadAllText(invoiceFileName);
                }
                FileInfo invoiceFileInfo = new FileInfo(invoiceFileName);
                IDictionary<string, object> invoice_metadatas = new Dictionary<string, object>();
                IDictionary<string, byte[]> invoiceAttachments = new Dictionary<string, byte[]>();
                invoice_metadatas = EInvoiceHelper.FillPayableInvoiceMetadatas(xmlContent, invoice_metadatas, (a) => _logger.WriteDebug(new LogMessage(a), LogCategories),
                    out InvoiceContactModel invoiceContactModel, out xmlModelKind, out invoiceAttachments);
                isInvoice |= xmlModelKind == XMLModelKind.InvoicePA_V12 || xmlModelKind == XMLModelKind.InvoicePR_V12;
                Guid correlationId = Guid.NewGuid();
                Guid protocolUniqueId = Guid.NewGuid();
                Guid udsID = Guid.NewGuid();

                if (!isInvoice)
                {
                    throw new ArgumentException($"Invoice xml evaluation found unsupport {xmlModelKind} XmlModelKind. This implementation support only B2B/PA Invoices");
                }
                invoiceConfiguration = workflowConfiguration.InvoiceTypes[xmlModelKind];
                DocSuiteWeb.Entity.Commons.Container container = (await _webAPIClient.GetContainerAsync(invoiceConfiguration.ProtocolContainerId)).SingleOrDefault();
                if (container == null)
                {
                    throw new ArgumentException($"Container {invoiceConfiguration.ProtocolContainerId} not found");
                }
                PECMailBox pecMailBox = (await _webAPIClient.GetPECMailBoxAsync(invoiceConfiguration.PECMailBoxId)).SingleOrDefault();
                if (pecMailBox == null)
                {
                    throw new ArgumentException($"PECMailBox {invoiceConfiguration.PECMailBoxId} not found");
                }
                if (invoiceContactModel == null || string.IsNullOrEmpty(invoiceContactModel.Description) || string.IsNullOrEmpty(invoiceContactModel.SDIIdentification) || string.IsNullOrEmpty(invoiceContactModel.Pivacf))
                {
                    throw new ArgumentException($"contact description {invoiceContactModel.Description} is empty or SDIIdentification {invoiceContactModel.SDIIdentification} is empty or Pivacf {invoiceContactModel.Pivacf} is empty");
                }
                int contactParentId = xmlModelKind == XMLModelKind.InvoicePA_V12 ? invoiceConfiguration.ContactPAParent : invoiceConfiguration.ContactB2BParent;
                ICollection<Contact> contacts = await _webAPIClient.GetContactAsync($"$filter=SDIIdentification eq '{invoiceContactModel.SDIIdentification}' and IncrementalFather eq {contactParentId}");
                Contact contact = contacts.FirstOrDefault();
                if (contact == null)
                {
                    _logger.WriteDebug(new LogMessage($"Contact '{invoiceContactModel.SDIIdentification}' not found and it's going to be creating."), LogCategories);
                    contact = await CreateContactAsync(contactParentId, invoiceContactModel);
                }
                foreach (short roleId in invoiceConfiguration.AuthorizationRoles)
                {
                    roles.Add(await _webAPIClient.GetRoleAsync(roleId));
                }
                List<RoleModel> roleModels = roles.Select(role => new RoleModel()
                {
                    IdRole = role.EntityShortId,
                    Name = role.Name,
                    TenantId = role.TenantId,
                    UniqueId = role.UniqueId,
                    RoleLabel = "Autorizzazione",
                    FullIncrementalPath = role.FullIncrementalPath
                }).ToList();

                UDSRepository udsRepository = (await _webAPIClient.GetUDSRepository(invoiceConfiguration.UDSRepositoryName)).First();
                string controllerName = Utils.GetWebAPIControllerName(invoiceConfiguration.UDSRepositoryName);
                Dictionary<int, Guid> documents = new Dictionary<int, Guid>();
                _logger.WriteDebug(new LogMessage($"Finding UDS invoice using {invoiceFileInfo.Name} filename"), LogCategories);
                Dictionary<string, object> uds_metadatas = null;
                try
                {
                    uds_metadatas = await _webAPIClient.GetUDSByInvoiceFilename(controllerName, invoiceFileInfo.Name, true, documents);
                }
                catch (InvalidOperationException)
                {
                    throw new ArgumentException($"Invoice {invoiceFileInfo.Name} exists more than one unique record expected in UDS archive {invoiceConfiguration.UDSRepositoryName}. Please 'Annulla' Invoice Archive and Protocol records before processed again this invoice.");
                }
                if (uds_metadatas != null && uds_metadatas.Any())
                {
                    throw new ArgumentException($"Invoice {invoiceFileInfo.Name} already inserted in UDS archive {invoiceConfiguration.UDSRepositoryName} with UDSId {uds_metadatas[UDSEInvoiceHelper.UDSMetadata_UDSId]}");
                }

                _logger.WriteDebug(new LogMessage($"Preaparing starting workflow with correlationId {correlationId}, protocolUniqueId {protocolUniqueId}, udsID {udsID}"), LogCategories);
                if (invoiceConfiguration.SignInvoiceType == SignInvoiceType.AutomaticAruba && invoiceConfiguration.SignerParameter != null)
                {
                    _logger.WriteDebug(new LogMessage($"Preaparing Aruba ARSS request for delegated user {invoiceConfiguration.SignerParameter.DelegatedUser}"), LogCategories);
                    byte[] signDocument = _signServiceClient.SignDocument(invoiceConfiguration.SignerParameter, File.ReadAllBytes(invoiceFileInfo.FullName));
                    _logger.WriteDebug(new LogMessage($"Aruba ARSS response document {signDocument.Length}"), LogCategories);
                    File.WriteAllBytes($"{invoiceFileInfo.FullName}{EXTENSION_P7M}", signDocument);
                    invoiceFileInfo = new FileInfo($"{invoiceFileInfo.FullName}{EXTENSION_P7M}");
                }
                if (invoiceConfiguration.SignInvoiceType == SignInvoiceType.AlreadySigned && !invoiceFileInfo.Extension.Equals(EXTENSION_P7M, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new ArgumentException($"Invoice configuration {xmlModelKind} must accept only signed document {invoiceFileName}");
                }

                WorkflowReferenceModel workflowReferenceModelUDS = await CreateUDSBuildModelAsync(invoiceFileInfo, contact, udsRepository, roleModels, udsID, protocolUniqueId, correlationId, invoice_metadatas,
                    invoiceAttachments, invoiceConfiguration.WorkflowRepositoryName, metadataFile);
                WorkflowReferenceModel workflowReferenceModelProtocol = await CreateProtocolBuildModelAsync(invoiceFileInfo, container, contact, udsRepository, roleModels, protocolUniqueId, udsID, invoiceConfiguration.ProtocolCategoryId,
                    correlationId, invoice_metadatas, invoiceAttachments, invoiceConfiguration.WorkflowRepositoryName);
                WorkflowReferenceModel workflowReferenceModelPECMail = await CreatePECMailBuildModelAsync(invoiceFileInfo, container, contact, pecMailBox, invoiceConfiguration.MailRecipients, udsRepository, roleModels, protocolUniqueId,
                    udsID, correlationId, invoice_metadatas, invoiceConfiguration.WorkflowRepositoryName);
                WorkflowResult workflowResult = await StartWorkflowAsync(workflowReferenceModelUDS, workflowReferenceModelProtocol, workflowReferenceModelPECMail, workflowConfiguration, 
                    invoiceConfiguration.WorkflowRepositoryName, $"{invoice_metadatas[EInvoiceHelper.Metadata_Denominazione]} - Fattura n° {invoice_metadatas[EInvoiceHelper.Metadata_NumeroFattura]} del {((DateTimeOffset)invoice_metadatas[EInvoiceHelper.Metadata_DataFattura]).LocalDateTime.ToShortDateString()} ({invoiceFileInfo.Name})");
                if (!workflowResult.IsValid || !workflowResult.InstanceId.HasValue)
                {
                    _logger.WriteError(new LogMessage("An error occured in start payable invoice workflow"), LogCategories);
                    throw new Exception(string.Join(", ", workflowResult.Errors));
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("WorkflowStartPayableInvoice -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        private async Task<Contact> CreateContactAsync(int contactFatherId, InvoiceContactModel invoiceContactModel)
        {
            Guid uniqueId = Guid.NewGuid();
            Contact contact = new Contact()
            {
                IdContactType = DocSuiteWeb.Entity.Commons.ContactType.AOO,
                IncrementalFather = contactFatherId,
                Description = invoiceContactModel.Description,
                SearchCode = invoiceContactModel.Pivacf,
                SDIIdentification = invoiceContactModel.SDIIdentification,
                FiscalCode = invoiceContactModel.Pivacf,
                Address = invoiceContactModel.Address,
                City = invoiceContactModel.City,
                ZipCode = invoiceContactModel.CAP,
                CertifiedMail = invoiceContactModel.CertifiedMail,
                IsActive = 1,
                IsLocked = 0,
                IsChanged = 0,
                UniqueId = uniqueId
            };
            contact = await _webAPIClient.PostAsync(contact);
            contact = (await _webAPIClient.GetContactAsync($"$filter=UniqueId eq {uniqueId}")).Single();
            _logger.WriteInfo(new LogMessage($"Contact '{invoiceContactModel.Description}' ({contact.EntityId}) has been create succesfully"), LogCategories);
            return contact;
        }

        private async Task<WorkflowReferenceModel> CreateProtocolBuildModelAsync(FileInfo invoiceFileInfo, DocSuiteWeb.Entity.Commons.Container container, Contact contact,
            UDSRepository uDSRepository, List<RoleModel> roles, Guid protocolUniqueId, Guid udsID, short categoryId, Guid correlationId,
            IDictionary<string, object> invoice_metadatas, IDictionary<string, byte[]> invoiceAttachments, string workflowName)
        {
            WorkflowReferenceModel workflowReferenceModel = new WorkflowReferenceModel
            {
                ReferenceId = correlationId
            };
            ProtocolBuildModel protocolBuildModel = new ProtocolBuildModel
            {
                WorkflowName = workflowName,
                WorkflowAutoComplete = true,
                UniqueId = workflowReferenceModel.ReferenceId,
                Protocol = new ProtocolModel
                {
                    Category = new CategoryModel() { IdCategory = categoryId },
                    Container = new ContainerModel()
                    {
                        IdContainer = container.EntityShortId,
                        Name = container.Name,
                        ProtLocation = new LocationModel()
                        {
                            IdLocation = container.ProtLocation.EntityShortId,
                            ProtocolArchive = container.ProtLocation.ProtocolArchive
                        }
                    }
                }
            };
            if (invoice_metadatas.ContainsKey(EInvoiceHelper.Metadata_IdentificativoSdi))
            {
                protocolBuildModel.Protocol.SDIIdentification = invoice_metadatas[EInvoiceHelper.Metadata_IdentificativoSdi].ToString();
            }
            protocolBuildModel.Protocol.UniqueId = protocolUniqueId;
            protocolBuildModel.Protocol.ProtocolType = new ProtocolTypeModel(ProtocolTypology.Outgoing);
            protocolBuildModel.Protocol.Object = $"{invoice_metadatas[EInvoiceHelper.Metadata_Denominazione]} - Fattura n° {invoice_metadatas[EInvoiceHelper.Metadata_NumeroFattura]} del {((DateTimeOffset)invoice_metadatas[EInvoiceHelper.Metadata_DataFattura]).LocalDateTime.ToShortDateString()}";
            protocolBuildModel.Protocol.DocumentCode = invoiceFileInfo.Name;
            protocolBuildModel.Protocol.Contacts.Add(new ProtocolContactModel()
            {
                ComunicationType = ComunicationType.Recipient,
                IdContact = contact.EntityId,
                Description = contact.Description
            });
            foreach (RoleModel role in roles)
            {
                protocolBuildModel.Protocol.Roles.Add(new ProtocolRoleModel()
                {
                    NoteType = ProtocolRoleNoteType.Accessible,
                    Status = ProtocolRoleStatus.ToEvaluate,
                    Role = role
                });
            }
            ArchiveDocument archiveDocument = await _documentClient.InsertDocumentAsync(new ArchiveDocument()
            {
                Archive = _workflowLocation.ProtocolArchive,
                ContentStream = File.ReadAllBytes(invoiceFileInfo.FullName),
                Name = invoiceFileInfo.Name,
            });
            protocolBuildModel.Protocol.MainDocument = new DocumentModel()
            {
                FileName = invoiceFileInfo.Name,
                DocumentToStoreId = archiveDocument.IdDocument
            };
            List<DocumentModel> invoiceAttachmentFiles = new List<DocumentModel>();
            foreach (KeyValuePair<string, byte[]> item in invoiceAttachments)
            {
                archiveDocument = await _documentClient.InsertDocumentAsync(new ArchiveDocument()
                {
                    Archive = _workflowLocation.ProtocolArchive,
                    ContentStream = item.Value,
                    Name = item.Key,
                });
                invoiceAttachmentFiles.Add(new DocumentModel()
                {
                    FileName = item.Key,
                    DocumentToStoreId = archiveDocument.IdDocument
                });
            }
            protocolBuildModel.Protocol.Attachments = invoiceAttachmentFiles;

            protocolBuildModel.WorkflowActions.Add(new WorkflowActionDocumentUnitLinkModel(
                new DocumentUnitModel() { UniqueId = protocolUniqueId, Environment = (int)DSWEnvironmentType.Protocol },
                new DocumentUnitModel() { UniqueId = udsID, Environment = uDSRepository.DSWEnvironment, IdUDSRepository = uDSRepository.UniqueId }));

            workflowReferenceModel.ReferenceType = DSWEnvironmentType.Build;
            workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(protocolBuildModel, ModuleConfigurationHelper.JsonSerializerSettings);
            return workflowReferenceModel;
        }

        private async Task<WorkflowReferenceModel> CreatePECMailBuildModelAsync(FileInfo invoiceFileInfo, DocSuiteWeb.Entity.Commons.Container container, Contact contact, PECMailBox pecMailBox,
            string mailRecipients, UDSRepository uDSRepository, List<RoleModel> roles, Guid protocolUniqueId, Guid udsID, Guid correlationId,
            IDictionary<string, object> invoice_metadatas, string workflowName)
        {
            WorkflowReferenceModel workflowReferenceModel = new WorkflowReferenceModel
            {
                ReferenceId = correlationId
            };
            PECMailBuildModel pecMailBuildModel = new PECMailBuildModel
            {
                WorkflowName = workflowName,
                WorkflowAutoComplete = true,
                UniqueId = workflowReferenceModel.ReferenceId,
                PECMail = new PECMailModel
                {
                    Direction = DocSuiteWeb.Model.Entities.PECMails.PECMailDirection.Outgoing,
                    InvoiceStatus = DocSuiteWeb.Model.Entities.PECMails.InvoiceStatus.InvoiceLookingMetadata,
                    IsActive = DocSuiteWeb.Model.Entities.PECMails.PECMailActiveType.Active,
                    MailBody = $"{invoice_metadatas[EInvoiceHelper.Metadata_Denominazione]} - Fattura n° {invoice_metadatas[EInvoiceHelper.Metadata_NumeroFattura]} del {((DateTimeOffset)invoice_metadatas[EInvoiceHelper.Metadata_DataFattura]).LocalDateTime.ToShortDateString()}",
                    MailPriority = DocSuiteWeb.Model.Entities.PECMails.PECMailPriority.Normal,
                    MailRecipients = mailRecipients,
                    MailSenders = pecMailBox.MailBoxRecipient,
                    MailSubject = $"Invio Fattura n° {invoice_metadatas[EInvoiceHelper.Metadata_NumeroFattura]} del {((DateTimeOffset)invoice_metadatas[EInvoiceHelper.Metadata_DataFattura]).LocalDateTime.ToShortDateString()}",
                    PECMailBox = new PECMailBoxModel()
                    {
                        PECMailBoxId = pecMailBox.EntityShortId,
                        UniqueId = pecMailBox.UniqueId,
                        MailBoxRecipient = pecMailBox.MailBoxRecipient,
                        Location = new LocationModel()
                        {
                            IdLocation = pecMailBox.Location.EntityShortId,
                            ProtocolArchive = pecMailBox.Location.ProtocolArchive,
                            UniqueId = pecMailBox.Location.UniqueId,
                        }
                    }
                }
            };
            ArchiveDocument archiveDocument = await _documentClient.InsertDocumentAsync(new ArchiveDocument()
            {
                Archive = _workflowLocation.ProtocolArchive,
                ContentStream = File.ReadAllBytes(invoiceFileInfo.FullName),
                Name = invoiceFileInfo.Name,
            });
            pecMailBuildModel.PECMail.Attachments.Add(new DocumentModel()
            {
                FileName = invoiceFileInfo.Name,
                DocumentToStoreId = archiveDocument.IdDocument
            });

            pecMailBuildModel.WorkflowActions.Add(new WorkflowActionDocumentUnitLinkModel(
               new DocumentUnitModel() { UniqueId = protocolUniqueId, Environment = (int)DSWEnvironmentType.Protocol },
               new DocumentUnitModel() { Environment = (int)DSWEnvironmentType.PECMail }));

            workflowReferenceModel.ReferenceType = DSWEnvironmentType.Build;
            workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(pecMailBuildModel, ModuleConfigurationHelper.JsonSerializerSettings);
            return workflowReferenceModel;
        }

        private async Task<WorkflowReferenceModel> CreateUDSBuildModelAsync(FileInfo invoiceFileInfo, Contact contact, UDSRepository uDSRepository, List<RoleModel> roleModels,
            Guid udsID, Guid protocolUniqueId, Guid correlationId, IDictionary<string, object> invoice_metadatas, IDictionary<string, byte[]> invoiceAttachments,
            string workflowName, FileInfo metadataFile)
        {
            PayableInvoiceMetadata payableInvoiceMetadata = new PayableInvoiceMetadata()
            {
                DateVAT = null,
                ProtocolNumberVAT = default(int?),
                SectionalVAT = string.Empty,
                YearVAT = default(int?),
            };
            if (metadataFile.Exists)
            {
                Dictionary<string, string> metadatas = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(metadataFile.FullName));
                string keyValue = string.Empty;
                DateTime dateValue = DateTime.MinValue;
                if (!metadatas.TryGetValue("Anno IVA", out keyValue) || !int.TryParse(keyValue, out int intValue))
                {
                    throw new ArgumentException($"Metadata invoice has invalid Anno IVA : {keyValue}");
                }
                payableInvoiceMetadata.YearVAT = intValue;
                if (!metadatas.TryGetValue("Protocollo IVA", out keyValue) || !int.TryParse(keyValue, out intValue))
                {
                    throw new ArgumentException($"Metadata invoice has invalid Protocollo IVA : {keyValue}");
                }
                payableInvoiceMetadata.ProtocolNumberVAT = intValue;
                if (!metadatas.TryGetValue("Sezionale IVA", out keyValue) || string.IsNullOrEmpty(keyValue))
                {
                    throw new ArgumentException($"Metadata invoice has invalid Sezionale IVA : {keyValue}");
                }
                payableInvoiceMetadata.SectionalVAT = keyValue;
                if (!metadatas.TryGetValue("Data IVA", out keyValue) || !DateTime.TryParse(keyValue, out dateValue))
                {
                    throw new ArgumentException($"Metadata invoice has invalid Data IVA: {keyValue}");
                }
                payableInvoiceMetadata.DateVAT = dateValue;
            }
            WorkflowReferenceModel workflowReferenceModel = new WorkflowReferenceModel
            {
                ReferenceId = correlationId
            };
            UDSModel model = UDSModel.LoadXml(uDSRepository.ModuleXML);
            model.Model.UDSId = udsID.ToString();
            model.Model.Subject.Value = $"{invoice_metadatas[EInvoiceHelper.Metadata_Denominazione]} - Fattura n° {invoice_metadatas[EInvoiceHelper.Metadata_NumeroFattura]} del {((DateTimeOffset)invoice_metadatas[EInvoiceHelper.Metadata_DataFattura]).LocalDateTime.ToShortDateString()}";
            IDictionary<string, object> uds_metadatas = UDSEInvoiceHelper.MappingPayableInvoiceMetadatas(payableInvoiceMetadata, invoice_metadatas);
            model.FillMetaData(uds_metadatas);
            model = UDSEInvoiceHelper.InitDocumentStructures(model);
            ArchiveDocument archiveDocument = await _documentClient.InsertDocumentAsync(new ArchiveDocument()
            {
                Archive = _workflowLocation.ProtocolArchive,
                ContentStream = File.ReadAllBytes(invoiceFileInfo.FullName),
                Name = invoiceFileInfo.Name,
            }); ;
            model.Model.Documents.Document.Instances = UDSEInvoiceHelper.FillDocumentInstances(new List<InvoiceFileModel>()
            {
                new InvoiceFileModel()
                {
                    InvoiceBiblosDocumentId = archiveDocument.IdDocument,
                    InvoiceFilename =  invoiceFileInfo.Name
                }
            });
            List<InvoiceFileModel> invoiceAttachmentFiles = new List<InvoiceFileModel>();
            foreach (KeyValuePair<string, byte[]> item in invoiceAttachments)
            {
                archiveDocument = await _documentClient.InsertDocumentAsync(new ArchiveDocument()
                {
                    Archive = _workflowLocation.ProtocolArchive,
                    ContentStream = item.Value,
                    Name = item.Key,
                });
                invoiceAttachmentFiles.Add(new InvoiceFileModel()
                {
                    InvoiceFilename = item.Key,
                    InvoiceBiblosDocumentId = archiveDocument.IdDocument
                });
            }
            model.Model.Documents.DocumentAttachment.Instances = UDSEInvoiceHelper.FillDocumentInstances(invoiceAttachmentFiles);

            Contacts contacts = model.Model.Contacts.Single();
            if (contacts.ContactInstances == null)
            {
                contacts.ContactInstances = new ContactInstance[]
                {
                    new ContactInstance() { IdContact = contact.EntityId }
                };
            }

            model = UDSEInvoiceHelper.FillAuthorizations(model, roleModels);
            UDSBuildModel udsBuildModel = new UDSBuildModel(model.SerializeToXml())
            {
                WorkflowName = workflowName,
                WorkflowActions = new List<IWorkflowAction>(),
                Documents = new List<UDSDocumentModel>(),
                Roles = new List<RoleModel>(),
                Users = new List<UserModel>(),
                UniqueId = correlationId,
                UDSRepository = new UDSRepositoryModel(uDSRepository.UniqueId)
                {
                    DSWEnvironment = uDSRepository.DSWEnvironment,
                    Name = uDSRepository.Name
                },
                Subject = model.Model.Subject.Value
            };
            udsBuildModel.WorkflowAutoComplete = true;
            udsBuildModel.Roles = roleModels;
            workflowReferenceModel.ReferenceType = DSWEnvironmentType.Build;
            workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(udsBuildModel, ModuleConfigurationHelper.JsonSerializerSettings);
            return workflowReferenceModel;
        }

        private async Task<WorkflowResult> StartWorkflowAsync(WorkflowReferenceModel workflowReferenceModelUDS, WorkflowReferenceModel workflowReferenceModelProtocol, 
            WorkflowReferenceModel workflowReferenceModelPECMail, WorkflowConfiguration workflowConfiguration, string workflowName, string subject)
        {
            WorkflowStart workflowStart = new WorkflowStart
            {
                WorkflowName = workflowName
            };
            workflowStart.Arguments.Add(string.Concat(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, "_0"),
                new WorkflowArgument()
                {
                    Name = string.Concat(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, "_0"),
                    PropertyType = ArgumentType.Json,
                    ValueString = JsonConvert.SerializeObject(workflowReferenceModelUDS)
                });
            workflowStart.Arguments.Add(string.Concat(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, "_1"),
                new WorkflowArgument()
                {
                    Name = string.Concat(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, "_1"),
                    PropertyType = ArgumentType.Json,
                    ValueString = JsonConvert.SerializeObject(workflowReferenceModelProtocol)
                });
            workflowStart.Arguments.Add(string.Concat(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, "_2"),
                  new WorkflowArgument()
                  {
                      Name = string.Concat(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, "_2"),
                      PropertyType = ArgumentType.Json,
                      ValueString = JsonConvert.SerializeObject(workflowReferenceModelPECMail)
                  });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.RelationGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID,
                ValueGuid = _moduleConfiguration.TenantId
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.RelationGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID,
                ValueGuid = workflowConfiguration.TenantAOOId
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME,
                ValueString = _moduleConfiguration.TenantName
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_INSTANCE_SUBJECT, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_INSTANCE_SUBJECT,
                ValueString = subject
            });
            WorkflowResult workflowResult = await _webAPIClient.StartWorkflow(workflowStart);
            _logger.WriteInfo(new LogMessage(string.Concat("Workflow started correctly [IsValid: ", workflowResult.IsValid, "] with instanceId ", workflowResult.InstanceId)), LogCategories);
            return workflowResult;
        }

        private async Task<WorkflowResult> StartCancelInvoiceWorkflowAsync(WorkflowReferenceModel workflowReferenceModelUDS,
            WorkflowReferenceModel workflowReferenceModelProtocol, WorkflowConfiguration workflowConfiguration, string workflowName)
        {
            WorkflowStart workflowStart = new WorkflowStart
            {
                WorkflowName = workflowName
            };
            workflowStart.Arguments.Add(string.Concat(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, "_0"),
                new WorkflowArgument()
                {
                    Name = string.Concat(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, "_0"),
                    PropertyType = ArgumentType.Json,
                    ValueString = JsonConvert.SerializeObject(workflowReferenceModelUDS)
                });
            workflowStart.Arguments.Add(string.Concat(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, "_1"),
                new WorkflowArgument()
                {
                    Name = string.Concat(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, "_1"),
                    PropertyType = ArgumentType.Json,
                    ValueString = JsonConvert.SerializeObject(workflowReferenceModelProtocol)
                });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.RelationGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID,
                ValueGuid = _moduleConfiguration.TenantId
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.RelationGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID,
                ValueGuid = workflowConfiguration.TenantAOOId
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME,
                ValueString = _moduleConfiguration.TenantName
            });
            WorkflowResult workflowResult = await _webAPIClient.StartWorkflow(workflowStart);
            _logger.WriteInfo(new LogMessage(string.Concat("Workflow started correctly [IsValid: ", workflowResult.IsValid, "] with instanceId ", workflowResult.InstanceId)), LogCategories);
            return workflowResult;
        }

        #endregion

        #region [ ServiceBus Callbacks ]

        private async Task WorkflowPECMailReceiptCallback(IEventReceivedReceiptPECMail evt, IDictionary<string, object> properties)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"WorkflowReceiptPECMailCallback -> evaluate event id {evt.Id}"), LogCategories);
                _logger.WriteInfo(new LogMessage($"Notifying PECMailReceipt"), LogCategories);
                Guid protocolUniqueId = Guid.Empty;
                if (!evt.CustomProperties.ContainsKey(CustomPropertyName.PROTOCOL_UNIQUE_ID) ||
                    !Guid.TryParse(evt.CustomProperties[CustomPropertyName.PROTOCOL_UNIQUE_ID].ToString(), out protocolUniqueId) || protocolUniqueId == Guid.Empty)
                {
                    throw new ArgumentNullException($"Undefined {CustomPropertyName.PROTOCOL_UNIQUE_ID} property in event properties");
                }
                if (!evt.CustomProperties.ContainsKey(CustomPropertyName.PROTOCOL_YEAR))
                {
                    throw new ArgumentNullException($"Undefined {CustomPropertyName.PROTOCOL_YEAR} property in event properties");
                }
                if (!evt.CustomProperties.ContainsKey(CustomPropertyName.PROTOCOL_NUMBER))
                {
                    throw new ArgumentNullException($"Undefined {CustomPropertyName.PROTOCOL_NUMBER} property in event properties");
                }
                if (!evt.CustomProperties.ContainsKey(CustomPropertyName.PECMAIL_RECEIPT_TYPE))
                {
                    throw new ArgumentNullException($"Undefined {CustomPropertyName.PECMAIL_RECEIPT_TYPE} property in event properties");
                }
                if (!evt.CustomProperties.ContainsKey(CustomPropertyName.PECMAIL_RECEIPT_DATE))
                {
                    throw new ArgumentNullException($"Undefined {CustomPropertyName.PECMAIL_RECEIPT_DATE} property in event properties");
                }
                string receiptType = evt.CustomProperties[CustomPropertyName.PECMAIL_RECEIPT_TYPE].ToString();
                DateTime receiptDate = (DateTime)evt.CustomProperties[CustomPropertyName.PECMAIL_RECEIPT_DATE];
                UDSDocumentUnit udsDocumentUnit = (await _webAPIClient.GetUDSDocumentUnitFromDocumentUnitId(protocolUniqueId)).SingleOrDefault();

                if (udsDocumentUnit == null || !_moduleConfiguration.WorkflowConfigurations.SelectMany(f => f.Value.InvoiceTypes).Any(f => f.Value.UDSRepositoryName == udsDocumentUnit.Repository.Name))
                {
                    throw new ArgumentNullException($"Wrong message received ProtocolUniqueId {protocolUniqueId} {evt.CustomProperties[CustomPropertyName.PROTOCOL_YEAR]}/{evt.CustomProperties[CustomPropertyName.PROTOCOL_NUMBER]} not related to valid Invoice Environment");
                }

                ICollection<UDSDocumentUnit> udsDocumentUnits = await _webAPIClient.GetUDSDocumentUnits(udsDocumentUnit.IdUDS, false, false);
                string controllerName = Utils.GetWebAPIControllerName(udsDocumentUnit.Repository.Name);
                Dictionary<int, Guid> documents = new Dictionary<int, Guid>();
                IDictionary<string, object> uds_metadatas = await _webAPIClient.GetUDS(controllerName, udsDocumentUnit.IdUDS, documents);
                if (uds_metadatas == null)
                {
                    throw new ArgumentNullException($"Not found UDS invoice  {udsDocumentUnit.IdUDS}/{controllerName}");
                }
                _logger.WriteInfo(new LogMessage($"Updating PECMailReceipt metadatas for UDSId {udsDocumentUnit.IdUDS}"), LogCategories);
                uds_metadatas = UDSEInvoiceHelper.MappingPayablePECMailReceiptMetadatas(receiptDate, receiptType, uds_metadatas);

                ICollection<UDSRole> udsRoles = await _webAPIClient.GetUDSRoles(udsDocumentUnit.IdUDS);
                ICollection<UDSContact> udsContacts = await _webAPIClient.GetUDSContacts(udsDocumentUnit.IdUDS);
                ICollection<UDSMessage> udsMessages = await _webAPIClient.GetUDSMessages(udsDocumentUnit.IdUDS);
                ICollection<UDSPECMail> udsPECMails = await _webAPIClient.GetUDSPECMails(udsDocumentUnit.IdUDS);

                UDSBuildModel udsBuildModel = UDSEInvoiceHelper.PrepareUpdateUDSBuildModel(udsDocumentUnit.Repository, udsDocumentUnit.IdUDS, uds_metadatas, documents,
                    udsRoles, udsContacts, udsMessages, udsPECMails, udsDocumentUnits, null, evt.Identity.User);
                CommandUpdateUDSData commandUpdateUDSData = new CommandUpdateUDSData(evt.Name, evt.TenantId, evt.TenantAOOId, evt.Identity, udsBuildModel);
                await _webAPIClient.SendCommandAsync(commandUpdateUDSData);
                _logger.WriteInfo(new LogMessage($"Updating metadata invoice {commandUpdateUDSData.Id} has been sended"), LogCategories);

            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("WorkflowReceivableInvoicePECMailBuildCompleteCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        private async Task WorkflowCreatePECMailCallback(IEventCreatePECMail evt, IDictionary<string, object> properties)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"WorkflowCreatePECMailCallback -> evaluate event id {evt.Id}"), LogCategories);
                _logger.WriteInfo(new LogMessage($"Notifying CreatePECMail"), LogCategories);

                PECMail pecMail = evt.ContentType.ContentTypeValue;
                if (pecMail == null || pecMail.DocumentUnit == null)
                {
                    throw new ArgumentNullException($"Undefined protocol in PECMail {pecMail?.EntityId}");
                }
                IEnumerable<PECMailAttachment> attachments = pecMail.Attachments
                    .Where(f => f.IDDocument.HasValue && f.IDDocument.Value != Guid.Empty && new FileInfo(f.AttachmentName).Extension.Equals(EXTENSION_XML, StringComparison.InvariantCultureIgnoreCase));
                if (!attachments.Any())
                {
                    throw new ArgumentNullException($"Undefined valid SDI attachment to evaluate in PECMail {pecMail.EntityId}");
                }

                Protocol protocol = (await _webAPIClient.GetProtocolAsync($"$filter=UniqueId eq {pecMail.DocumentUnit.UniqueId}&$expand=AdvancedProtocol")).SingleOrDefault();
                if (protocol == null)
                {
                    throw new ArgumentNullException($"Protocol not found for identification {pecMail.Year}/{pecMail.Number}");
                }
                UDSDocumentUnit udsDocumentUnit = (await _webAPIClient.GetUDSDocumentUnitFromDocumentUnitId(protocol.UniqueId)).SingleOrDefault();

                if (udsDocumentUnit == null || !_moduleConfiguration.WorkflowConfigurations.SelectMany(f => f.Value.InvoiceTypes).Any(f => f.Value.UDSRepositoryName == udsDocumentUnit.Repository.Name))
                {
                    throw new ArgumentNullException($"Wrong message received ProtocolUniqueId {protocol.UniqueId} {protocol.Year}/{protocol.Number} not related to valid Invoice Environment");
                }
                Dictionary<int, Guid> documents = new Dictionary<int, Guid>();
                string controllerName = Utils.GetWebAPIControllerName(udsDocumentUnit.Repository.Name);
                IDictionary<string, object> uds_metadatas = await _webAPIClient.GetUDS(controllerName, udsDocumentUnit.IdUDS, documents);
                if (uds_metadatas == null)
                {
                    throw new ArgumentNullException($"Not found UDS invoice  {udsDocumentUnit.IdUDS}/{controllerName}");
                }

                SDIMessage sdiMessage = new SDIMessage();
                Content content;
                object sdiMessageObj = null;
                foreach (PECMailAttachment item in attachments)
                {
                    content = await _documentClient.GetDocumentContentByIdAsync(item.IDDocument.Value);
                    using (MemoryStream stream = new MemoryStream(content.Blob))
                    {
                        sdiMessage = PAMessageHelper.GetSIDMessage(stream, out sdiMessageObj);
                        if (sdiMessage.Status != MessageStatus.None)
                        {
                            break;
                        }
                    }
                }
                if (sdiMessageObj == null || sdiMessage.Status == MessageStatus.None)
                {
                    throw new ArgumentNullException($"Undefined valid SDI receipttype {sdiMessageObj}/{sdiMessage.Status} in evaluating PECMail {pecMail.EntityId}");
                }
                uds_metadatas = UDSEInvoiceHelper.MappingPayablePECMailSIDMetadatas(sdiMessage, uds_metadatas);

                ICollection<UDSRole> udsRoles = await _webAPIClient.GetUDSRoles(udsDocumentUnit.IdUDS);
                ICollection<UDSContact> udsContacts = await _webAPIClient.GetUDSContacts(udsDocumentUnit.IdUDS);
                ICollection<UDSMessage> udsMessages = await _webAPIClient.GetUDSMessages(udsDocumentUnit.IdUDS);
                ICollection<UDSPECMail> udsPECMails = await _webAPIClient.GetUDSPECMails(udsDocumentUnit.IdUDS);
                ICollection<UDSDocumentUnit> udsDocumentUnits = await _webAPIClient.GetUDSDocumentUnits(udsDocumentUnit.IdUDS, false, false);
                _logger.WriteInfo(new LogMessage($"Updating PEC PA metadatas for UDSId {udsDocumentUnit.IdUDS}"), LogCategories);

                UDSBuildModel udsBuildModel = UDSEInvoiceHelper.PrepareUpdateUDSBuildModel(udsDocumentUnit.Repository, udsDocumentUnit.IdUDS, uds_metadatas, documents, udsRoles,
                    udsContacts, udsMessages, udsPECMails, udsDocumentUnits, null, evt.Identity.User);
                CommandUpdateUDSData commandUpdateUDSData = new CommandUpdateUDSData(evt.Name, evt.TenantId, evt.TenantAOOId, evt.Identity, udsBuildModel);
                await _webAPIClient.SendCommandAsync(commandUpdateUDSData);
                _logger.WriteInfo(new LogMessage($"Updating metadata invoice {commandUpdateUDSData.Id} has been sended"), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("WorkflowCreatePECMailCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        private async Task CompletePECMailBuildCallback(IEventCompletePECMailBuild evt, IDictionary<string, object> properties)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"CompletePECMailBuildCallback -> evaluate event id {evt.Id}"), LogCategories);
                _logger.WriteInfo(new LogMessage($"Notifying CompletePECMailBuild"), LogCategories);
                if (!evt.CustomProperties.ContainsKey(CustomPropertyName.WORKFLOW_NAME))
                {
                    throw new ArgumentNullException($"Undefined {CustomPropertyName.WORKFLOW_NAME} property in event properties");
                }
                WorkflowConfiguration workflowConfiguration = _moduleConfiguration.WorkflowConfigurations.Select(s => s.Value)
                    .FirstOrDefault(x => x.InvoiceTypes != null && x.InvoiceTypes.Count > 0 && x.InvoiceTypes
                    .Any(y => y.Value.WorkflowRepositoryName == evt.CustomProperties[CustomPropertyName.WORKFLOW_NAME].ToString()));
                if (workflowConfiguration == null)
                {
                    throw new ArgumentNullException($"Undefined workflow configuration for workflow: {evt.CustomProperties[CustomPropertyName.WORKFLOW_NAME]}");
                }
                KeyValuePair<XMLModelKind, InvoiceConfiguration> invoiceConfiguration = workflowConfiguration.InvoiceTypes
                    .FirstOrDefault(f => f.Value.WorkflowRepositoryName == evt.CustomProperties[CustomPropertyName.WORKFLOW_NAME].ToString());
                if (invoiceConfiguration.Equals(default(KeyValuePair<XMLModelKind, InvoiceConfiguration>)))
                {
                    throw new ArgumentNullException($"Undefined invoice configuration for workflow: {evt.CustomProperties[CustomPropertyName.WORKFLOW_NAME]}");
                }
                string udsRepositoryName = invoiceConfiguration.Value.UDSRepositoryName;
                if (string.IsNullOrEmpty(udsRepositoryName))
                {
                    throw new ArgumentNullException($"Undefinded UDSRepositoryName property of related {evt.CustomProperties[CustomPropertyName.WORKFLOW_NAME]}");
                }
                string controllerName = Utils.GetWebAPIControllerName(udsRepositoryName);
                WorkflowResult workflowResult = await CompletePECMailBuid(evt, controllerName, udsRepositoryName, workflowConfiguration);
                if (workflowResult == null)
                {
                    _logger.WriteError(new LogMessage("The cancel invoice workflow was not started."), LogCategories);
                }
                else
                {
                    _logger.WriteInfo(new LogMessage($"The process of canceling the invoice and the protocol was successful."), LogCategories);
                    _logger.WriteDebug(new LogMessage($"Attendere l'esito delle attività previste <b>Identificativo richiesta: {workflowResult.InstanceId}</b>"), LogCategories);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("CompletePECMailBuildCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        #endregion

        #region [ UpdateInvoiceMetadataAsync ]
        private async Task UpdateInvoiceMetadataAsync(InvoiceMetadata invoiceMetadata)
        {
            string controllerName = Utils.GetWebAPIControllerName(invoiceMetadata.UDSRepositoryName);
            Dictionary<int, Guid> documents = new Dictionary<int, Guid>();
            _logger.WriteDebug(new LogMessage($"Finding UDS invoice using {invoiceMetadata.InvoiceFileNameXML} filename"), LogCategories);
            IDictionary<string, object> uds_metadatas = await _webAPIClient.GetUDSByInvoiceFilename(controllerName, invoiceMetadata.InvoiceFileNameXML, false, documents);
            if (uds_metadatas == null || !uds_metadatas.Any())
            {
                throw new ArgumentNullException($"Wrong invoice metadata {invoiceMetadata.MetadataFileName} {invoiceMetadata.InvoiceFileNameXML}/{invoiceMetadata.InvoiceFileNameP7M} not related to valid UDSId");
            }

            UDSRepository udsRepository = (await _webAPIClient.GetUDSRepository(invoiceMetadata.UDSRepositoryName)).Last(f => f.Status == DocSuiteWeb.Entity.UDS.UDSRepositoryStatus.Confirmed);
            if (udsRepository == null)
            {
                throw new ArgumentNullException($"udsRepository {invoiceMetadata.UDSRepositoryName} not found");
            }
            WorkflowConfiguration workflowConfiguration = _moduleConfiguration.WorkflowConfigurations.Select(f => f.Value).SingleOrDefault(f => f.InvoiceTypes.Any(x => x.Value.UDSRepositoryName == udsRepository.Name));
            uds_metadatas[UDSEInvoiceHelper.UDSMetadata_AnnoIva] = invoiceMetadata.InvoiceFiscalMetadata.YearVAT;
            uds_metadatas[UDSEInvoiceHelper.UDSMetadata_DataIva] = invoiceMetadata.InvoiceFiscalMetadata.DateVAT;
            uds_metadatas[UDSEInvoiceHelper.UDSMetadata_SezionaleIva] = invoiceMetadata.InvoiceFiscalMetadata.SectionalVAT;
            uds_metadatas[UDSEInvoiceHelper.UDSMetadata_ProtocolloIva] = invoiceMetadata.InvoiceFiscalMetadata.ProtocolNumberVAT;
            Guid idUDS = Guid.Parse(uds_metadatas[UDSEInvoiceHelper.UDSMetadata_UDSId] as string);
            _logger.WriteInfo(new LogMessage($"UDS {idUDS} updating invoice metadata"), LogCategories);
            ICollection<UDSRole> udsRoles = await _webAPIClient.GetUDSRoles(idUDS);
            ICollection<UDSContact> udsContacts = await _webAPIClient.GetUDSContacts(idUDS);
            ICollection<UDSMessage> udsMessages = await _webAPIClient.GetUDSMessages(idUDS);
            ICollection<UDSPECMail> udsPECMails = await _webAPIClient.GetUDSPECMails(idUDS);
            ICollection<UDSDocumentUnit> udsDocumentUnits = await _webAPIClient.GetUDSDocumentUnits(idUDS, false, false);
            UDSBuildModel udsBuildModel = UDSEInvoiceHelper.PrepareUpdateUDSBuildModel(udsRepository, idUDS, uds_metadatas, documents, udsRoles, udsContacts, udsMessages, udsPECMails,
                udsDocumentUnits, null, _username);

            CommandUpdateUDSData commandUpdateUDSData = new CommandUpdateUDSData(_moduleConfiguration.TenantName, _moduleConfiguration.TenantId, workflowConfiguration.TenantAOOId, new IdentityContext(_username), udsBuildModel);
            await _webAPIClient.SendCommandAsync(commandUpdateUDSData);
            _logger.WriteInfo(new LogMessage($"Updating metadata invoice {commandUpdateUDSData.Id} has been sended"), LogCategories);
        }
        #endregion

        private async Task<WorkflowResult> CompletePECMailBuid(IEventCompletePECMailBuild evt, string controllerName, string udsRepositoryName, WorkflowConfiguration workflowConfiguration)
        {
            foreach (DocumentModel attachment in evt.ContentType.ContentTypeValue.PECMail.Attachments)
            {
                string invoiceFilename = attachment.FileName;
                Dictionary<int, Guid> documents = new Dictionary<int, Guid>();
                _logger.WriteInfo(new LogMessage($"Getting the UDS invoice metadata with filename '{invoiceFilename}' and status 'Rifiutata dallo SDI'."), LogCategories);
                Dictionary<string, object> uds_metadatas = await _webAPIClient.GetRejectedUDSByInvoiceFilename(controllerName, invoiceFilename, documents);
                if (uds_metadatas == null || !uds_metadatas.Any())
                {
                    _logger.WriteInfo(new LogMessage($"Invoice metadata with filename '{invoiceFilename}' and status 'Rifiutata dallo SDI' not found."), LogCategories);
                    continue;
                }
                if (!uds_metadatas.ContainsKey(UDSEInvoiceHelper.UDSMetadata_UDSId))
                {
                    throw new ArgumentNullException($"Undefined {UDSEInvoiceHelper.UDSMetadata_UDSId} property in uds metadata");
                }
                Guid udsID = Guid.Parse(uds_metadatas[UDSEInvoiceHelper.UDSMetadata_UDSId].ToString());
                _logger.WriteInfo(new LogMessage($"Getting the UDSDocumentUnit with UDS ID {udsID} and RelationType {DocSuiteWeb.Entity.UDS.UDSRelationType.ProtocolArchived}."), LogCategories);
                ICollection<UDSDocumentUnit> udsDocumentUnits = await _webAPIClient.GetUDSDocumentUnits(udsID, false, true);
                UDSDocumentUnit udsDocumentUnit = udsDocumentUnits.FirstOrDefault(x => x.RelationType == DocSuiteWeb.Entity.UDS.UDSRelationType.ProtocolArchived);
                if (udsDocumentUnit == null)
                {
                    _logger.WriteInfo(new LogMessage($"UDSDocumentUnit with UDS ID {udsID} was not found."), LogCategories);
                }
                if (udsDocumentUnit.Relation == null)
                {
                    _logger.WriteInfo(new LogMessage($"UDSDocumentUnit with UDS ID {udsID} does not have a related DocumentUnit."), LogCategories);
                }
                if (udsDocumentUnit.Repository == null)
                {
                    _logger.WriteInfo(new LogMessage($"UDSDocumentUnit with UDS ID {udsID} does not have a related UDSRepository."), LogCategories);
                }
                if (uds_metadatas != null)
                {
                    if (!evt.CustomProperties.ContainsKey(CustomPropertyName.TENANT_ID))
                    {
                        throw new ArgumentNullException($"Undefined {CustomPropertyName.TENANT_ID} property in event properties");
                    }
                    if (!evt.CustomProperties.ContainsKey(CustomPropertyName.TENANT_NAME))
                    {
                        throw new ArgumentNullException($"Undefined {CustomPropertyName.TENANT_NAME} property in event properties");
                    }
                    if (!evt.CustomProperties.ContainsKey(CustomPropertyName.TENANT_AOO_ID))
                    {
                        throw new ArgumentNullException($"Undefined {CustomPropertyName.TENANT_AOO_ID} property in event properties");
                    }
                    Guid tenantId = Guid.Parse(evt.CustomProperties[CustomPropertyName.TENANT_ID].ToString());
                    string tenantName = evt.CustomProperties[CustomPropertyName.TENANT_NAME].ToString();
                    Guid tenantAOOId = Guid.Parse(evt.CustomProperties[CustomPropertyName.TENANT_AOO_ID].ToString());
                    IIdentityContext identity = new IdentityContext(_username);
                    string cancelMotivation = udsRepositoryName;
                    _logger.WriteInfo(new LogMessage($"Starting the process of canceling with invoice ID: {udsID}"), LogCategories);
                    WorkflowResult workflowResult = await BuildInvoiceDelete(udsDocumentUnit, cancelMotivation, tenantId, tenantAOOId, tenantName, evt.CorrelationId, identity, workflowConfiguration, uds_metadatas);
                    return workflowResult;
                }
            }
            return null;
        }

        private async Task<WorkflowResult> BuildInvoiceDelete(UDSDocumentUnit udsDocumentUnit, string cancelMotivation, Guid tenantId, Guid tenantAOOId,
            string tenantName, Guid? correlationId, IIdentityContext identity, WorkflowConfiguration workflowConfiguration, Dictionary<string, object> uds_metadatas)
        {
            if (workflowConfiguration.TenantAOOId != tenantAOOId)
            {
                throw new ArgumentNullException("documentUnit", $"TenantAOOId {tenantAOOId} not found");
            }

            Dictionary<int, Guid> documents = new Dictionary<int, Guid>();
            _logger.WriteDebug(new LogMessage($"Found {uds_metadatas != null} invoice metadatas {documents.ContainsKey(1)}"), LogCategories);

            await _webAPIClient.PushCorrelatedNotificationAsync($"Preparazione annullamento fattura <b>{uds_metadatas[UDSEInvoiceHelper.UDSMetadata_NumeroFattura]} del {((DateTime)uds_metadatas[UDSEInvoiceHelper.UDSMetadata_DataFattura]).ToShortDateString()}</b>...",
                ModuleConfigurationHelper.MODULE_NAME, tenantId, tenantAOOId, tenantName, correlationId, identity, NotificationType.EventWorkflowNotificationInfo);

            if (udsDocumentUnit == null)
            {
                await _webAPIClient.PushCorrelatedNotificationAsync($"Annullamento di fattura: Non sono stati trovati protocolli da annullare per la fattura <b>{uds_metadatas[UDSEInvoiceHelper.UDSMetadata_NumeroFattura]}</b>.",
                    ModuleConfigurationHelper.MODULE_NAME, tenantId, tenantAOOId, tenantName, correlationId, identity, NotificationType.EventWorkflowStatusError);
                await _webAPIClient.PushCorrelatedNotificationAsync("Procedere con l'annullamento manuale dell'archivio.",
                    ModuleConfigurationHelper.MODULE_NAME, tenantId, tenantAOOId, tenantName, correlationId, identity, NotificationType.EventWorkflowStatusError);

                throw new ArgumentNullException("udsDocumentUnits", $"Protocol related to invoice {udsDocumentUnit.IdUDS} not found");
            }

            if (udsDocumentUnit.Relation == null || udsDocumentUnit.Repository == null)
            {
                await _webAPIClient.PushCorrelatedNotificationAsync($"Annullamento di fattura: La fattura {udsDocumentUnit.IdUDS} non è stata trovata.",
                ModuleConfigurationHelper.MODULE_NAME, tenantId, tenantAOOId, tenantName, correlationId, identity, NotificationType.EventWorkflowStatusError);
                throw new ArgumentNullException("documentUnit", $"Invoice {udsDocumentUnit.IdUDS} not found");
            }

            WorkflowReferenceModel workflowReferenceModelProtocol = new WorkflowReferenceModel();
            WorkflowReferenceModel workflowReferenceModelUDS = new WorkflowReferenceModel();

            ICollection<UDSRole> udsRoles = await _webAPIClient.GetUDSRoles(udsDocumentUnit.IdUDS);
            ICollection<UDSContact> udsContacts = await _webAPIClient.GetUDSContacts(udsDocumentUnit.IdUDS);
            ICollection<UDSMessage> udsMessages = await _webAPIClient.GetUDSMessages(udsDocumentUnit.IdUDS);
            ICollection<UDSPECMail> udsPECMails = await _webAPIClient.GetUDSPECMails(udsDocumentUnit.IdUDS);

            UDSBuildModel udsBuildModel = UDSEInvoiceHelper.PrepareUpdateUDSBuildModel(udsDocumentUnit.Repository, udsDocumentUnit.IdUDS, uds_metadatas, documents, udsRoles, udsContacts, udsMessages, udsPECMails,
                new List<UDSDocumentUnit> { udsDocumentUnit }, null, _username);
            udsBuildModel.WorkflowName = _moduleConfiguration.WorkflowInvoiceDeleteRepositoryName;
            udsBuildModel.WorkflowAutoComplete = true;
            udsBuildModel.WorkflowActions = new List<IWorkflowAction>();
            udsBuildModel.CancelMotivation = cancelMotivation;
            udsBuildModel.RegistrationUser = identity.User;
            workflowReferenceModelUDS.ReferenceType = DSWEnvironmentType.Build;
            workflowReferenceModelUDS.ReferenceModel = JsonConvert.SerializeObject(udsBuildModel, ModuleConfigurationHelper.JsonSerializerSettings);
            workflowReferenceModelUDS.ReferenceId = correlationId.Value;

            await _webAPIClient.PushCorrelatedNotificationAsync($"Preparazione annullamento protocollo <b>{udsDocumentUnit.Relation.Year}/{udsDocumentUnit.Relation.Number:0000000} del {udsDocumentUnit.Relation.RegistrationDate.ToLocalTime().Date.ToShortDateString()}</b>...",
                ModuleConfigurationHelper.MODULE_NAME, tenantId, tenantAOOId, tenantName, correlationId, identity, NotificationType.EventWorkflowNotificationInfo);

            ProtocolBuildModel protocolBuildModel = new ProtocolBuildModel
            {
                WorkflowName = _moduleConfiguration.WorkflowInvoiceDeleteRepositoryName,
                WorkflowAutoComplete = true,
                UniqueId = correlationId.Value,
                Protocol = new ProtocolModel
                {
                    Number = udsDocumentUnit.Relation.Number,
                    Year = udsDocumentUnit.Relation.Year,
                    Object = udsDocumentUnit.Relation.Subject,
                    UniqueId = udsDocumentUnit.Relation.UniqueId,
                    CancelMotivation = cancelMotivation
                }
            };
            workflowReferenceModelProtocol.ReferenceType = DSWEnvironmentType.Build;
            workflowReferenceModelProtocol.ReferenceModel = JsonConvert.SerializeObject(protocolBuildModel, ModuleConfigurationHelper.JsonSerializerSettings);
            workflowReferenceModelProtocol.ReferenceId = correlationId.Value;
            WorkflowResult workflowResult = await StartCancelInvoiceWorkflowAsync(workflowReferenceModelProtocol, workflowReferenceModelUDS, workflowConfiguration, _moduleConfiguration.WorkflowInvoiceDeleteRepositoryName);
            if (!workflowResult.IsValid || !workflowResult.InstanceId.HasValue)
            {
                _logger.WriteError(new LogMessage($"Receivable invoice an error occured in cancel invoice workflow"), LogCategory.NotifyToEmailCategory);

                _logger.WriteError(new LogMessage("An error occured in start cancel invoice workflow"), LogCategories);
                throw new ArgumentException(string.Join(", ", workflowResult.Errors));
            }

            return workflowResult;
        }
        #endregion
    }
}
