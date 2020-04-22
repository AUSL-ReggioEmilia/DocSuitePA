using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data;
using VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data.Entities;
using VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Invoice.Configurations;
using VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Invoice.Models;
using VecompSoftware.BPM.Integrations.Services.BiblosDS;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.SignServices;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS;
using VecompSoftware.Core.Command.CQRS.Commands.Models.UDS;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.DocumentGenerator;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.PECMails;
using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Model.Metadata;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.DocSuiteWeb.Model.Workflow.Actions;
using VecompSoftware.Helpers.EInvoice.EInvoice1_2;
using VecompSoftware.Helpers.EInvoice.Models;
using VecompSoftware.Helpers.EInvoice.UDS.Models;
using VecompSoftware.Helpers.PEC.PA;
using VecompSoftware.Helpers.PEC.PA.Models;
using VecompSoftware.Helpers.UDS;
using VecompSoftware.Helpers.Workflow;
using VecompSoftware.Helpers.XML;
using VecompSoftware.Services.Command.CQRS.Events.Entities.PECMails;
using VecompSoftware.Services.Command.CQRS.Events.Models.PECMails;
using VecompSoftware.Services.Command.CQRS.Events.Models.Protocols;
using VecompSoftware.Services.Command.CQRS.Events.Models.UDS;
using BiblosDocument = VecompSoftware.BPM.Integrations.Services.BiblosDS.DocumentService.Document;
using ComunicationType = VecompSoftware.DocSuiteWeb.Model.Entities.Commons.ComunicationType;
using Container = VecompSoftware.DocSuiteWeb.Entity.Commons.Container;
using Content = VecompSoftware.BPM.Integrations.Services.BiblosDS.DocumentService.Content;
using DSWEnvironmentType = VecompSoftware.DocSuiteWeb.Model.Entities.Commons.DSWEnvironmentType;
using EntityComunicationType = VecompSoftware.DocSuiteWeb.Entity.Protocols.ComunicationType;
using ProtocolRoleNoteType = VecompSoftware.DocSuiteWeb.Model.Entities.Protocols.ProtocolRoleNoteType;
using ProtocolRoleStatus = VecompSoftware.DocSuiteWeb.Model.Entities.Protocols.ProtocolRoleStatus;
using ProtocolTypology = VecompSoftware.DocSuiteWeb.Model.Entities.Protocols.ProtocolTypology;
using UDSEInvoiceHelper = VecompSoftware.Helpers.EInvoice.UDS.EInvoice1_2.EInvoiceHelper;

namespace VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Invoice
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]
        private readonly TimeSpan _threadWaiting = TimeSpan.FromSeconds(5);
        private static readonly HostIdentify _hostIdentify = new HostIdentify(Environment.MachineName, ModuleConfigurationHelper.MODULE_NAME);
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly ILogger _logger;
        private readonly IServiceBusClient _serviceBusClient;
        private readonly ISignServiceClient _signServiceClient;
        private readonly IWebAPIClient _webAPIClient;
        private readonly IDocumentClient _documentClient;
        private readonly IdentityContext _identityContext = null;
        private readonly IList<Guid> _subscriptions = new List<Guid>();
        private bool _needInitializeModule = false;
        private decimal _databaseTenantId = 0;
        private ERPDbContext _dbContext;
        private readonly MetadataModel _metadataModel;
        private readonly IEnumerable<KeyValuePair<XMLModelKind, InvoiceConfiguration>> _receivableWorkflowConfigurations;
        private readonly IEnumerable<KeyValuePair<XMLModelKind, InvoiceConfiguration>> _payableWorkflowConfigurations;
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
        public Execution(ILogger logger, IServiceBusClient serviceBusClient, IWebAPIClient webAPIClient, IDocumentClient documentClient, ISignServiceClient signServiceClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                _receivableWorkflowConfigurations = _moduleConfiguration.ReceivableWorkflowConfigurations.SelectMany(f => f.Value);
                _payableWorkflowConfigurations = _moduleConfiguration.PayableWorkflowConfigurations.SelectMany(f => f.Value);
                _serviceBusClient = serviceBusClient;
                _webAPIClient = webAPIClient;
                _documentClient = documentClient;
                _signServiceClient = signServiceClient;
                string username = "anonymous";
                _needInitializeModule = true;
                _databaseTenantId = _moduleConfiguration.DatabaseTenantId;
                if (WindowsIdentity.GetCurrent() != null)
                {
                    username = WindowsIdentity.GetCurrent().Name;
                }
                _identityContext = new IdentityContext(username);
                //MetadataRepository metadataRepository = _webAPIClient.GetMetadataRepositoryAsync(string.Concat("$filter=UniqueId eq ", _moduleConfiguration.MetadataRepositoryId)).Result.Single();
                //_metadataModel = JsonConvert.DeserializeObject<MetadataModel>(metadataRepository.JsonMetadata);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("***REMOVED***.ERP.Invoice -> Critical error in costruction module"), ex, LogCategories);
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

            try
            {
                InitializeModule();

                InvoiceConfiguration defaultInvoiceConfiguration = _receivableWorkflowConfigurations.First(f => f.Key == XMLModelKind.InvoicePR_V12).Value;
                List<RoleModel> roleModels = null;
                List<Role> roles = new List<Role>();
                UDSRepository uDSRepository = _webAPIClient.GetUDSRepository(defaultInvoiceConfiguration.UDSRepositoryName).Result.Last(f => f.Status == DocSuiteWeb.Entity.UDS.UDSRepositoryStatus.Confirmed);
                foreach (short roleId in defaultInvoiceConfiguration.AuthorizationRoles)
                {
                    roles.Add(_webAPIClient.GetRoleAsync(roleId).Result);
                }
                roleModels = roles.Select(role => new RoleModel()
                {
                    IdRole = role.EntityShortId,
                    Name = role.Name,
                    TenantId = role.TenantId,
                    UniqueId = role.UniqueId,
                    RoleLabel = "Autorizzazione",
                    FullIncrementalPath = role.FullIncrementalPath
                }).ToList();

                ReceivableInvoice currentReceivableInvoice = null;
                PayableInvoice currentPayableInvoice = null;
                int successfullCount = 0;
                int totalCount = 0;
                foreach (ReceivableInvoice receivableInvoice in _dbContext.ReceivableInvoices.Where(f => f.TenantId == _databaseTenantId && f.ERPUpdatedDate != null && f.WorkflowStatus == 0 && f.RecordOwner == ReceivableInvoice.DOCSUITE_OWNER))
                {
                    totalCount++;
                    currentReceivableInvoice = receivableInvoice;
                    try
                    {
                        _logger.WriteInfo(new LogMessage($"Updating ReceivableInvoice {receivableInvoice.InvoiceNumber}({receivableInvoice.WorkflowId}) metadatas"), LogCategories);
                        UpdateInvoiceMetadataAsync(receivableInvoice).Wait();
                        receivableInvoice.WorkflowStatus = 1;
                        _dbContext.SaveChanges();
                        _logger.WriteInfo(new LogMessage($"Invoice {receivableInvoice.WorkflowId} has been updated"), LogCategories);
                        successfullCount++;
                    }
                    catch (ArgumentException ex)
                    {
                        _logger.WriteError(new LogMessage($"ERP.Invoice -> Metadata invalid on evaluating ReceivableInvoice id: {currentReceivableInvoice.WorkflowId}"), ex, LogCategories);
                        receivableInvoice.WorkflowStatus = 3;
                        _dbContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteError(new LogMessage($"ERP.Invoice -> Error on evaluating ReceivableInvoice id: {currentReceivableInvoice.WorkflowId}"), ex, LogCategories);
                        receivableInvoice.WorkflowStatus = 2;
                        _dbContext.SaveChanges();
                    }

                }
                _logger.WriteInfo(new LogMessage($"DocSuite ReceivableInvoice {successfullCount}/{totalCount} has been successfully evaluated"), LogCategories);

                successfullCount = 0;
                totalCount = 0;
                int protocolNumber = 0;
                DocumentUnit protocolDocumentUnit;
                ProtocolContact protocolContactSender = null;
                ProtocolContactManual protocolContactManualSender = null;
                IDictionary<string, object> uds_metadatas = null;
                ReceivableInvoiceMetadata receivableInvoiceMetadata;
                string companyName;
                string vatId;
                foreach (ReceivableInvoice receivableInvoice in _dbContext.ReceivableInvoices.Where(f => f.TenantId == _databaseTenantId && f.WorkflowProcessed == null && f.RecordOwner == ReceivableInvoice.ERP_OWNER))
                {
                    totalCount++;
                    currentReceivableInvoice = receivableInvoice;
                    protocolContactManualSender = null;
                    protocolContactSender = null;
                    protocolDocumentUnit = null;
                    receivableInvoiceMetadata = null;
                    vatId = companyName = string.Empty;
                    try
                    {
                        _logger.WriteInfo(new LogMessage($"Evaluating ReceivableInvoice {receivableInvoice.InvoiceNumber}({receivableInvoice.WorkflowId}) from ERP referenced to protocol {receivableInvoice.DocSuiteProtocolYear}/{receivableInvoice.DocSuiteProtocolNumber}"), LogCategories);

                        if (string.IsNullOrEmpty(receivableInvoice.InvoiceNumber) || !receivableInvoice.InvoiceDate.HasValue || receivableInvoice.InvoiceDate.Value == DateTime.MinValue ||
                            string.IsNullOrEmpty(receivableInvoice.DocSuiteProtocolNumber) || !int.TryParse(receivableInvoice.DocSuiteProtocolNumber, out protocolNumber) || protocolNumber <= 0 ||
                            !receivableInvoice.DocSuiteProtocolYear.HasValue || !receivableInvoice.DateVAT.HasValue || string.IsNullOrEmpty(receivableInvoice.SectionalVAT) || !receivableInvoice.ProtocolNumberVAT.HasValue ||
                            !receivableInvoice.YearVAT.HasValue)
                        {
                            throw new ArgumentException($"Required columns in ERP database has empty value, check : NUMERO_FATTURA, DATA_FATTURA, NUMERO_PROTOCOLLO, ANNO_PROTOCOLLO, ANNO_IVA, DATA_IVA, REGISTRO_IVA, PROTOCOLLO_IVA");
                        }
                        protocolDocumentUnit = _webAPIClient.GetDocumentUnitAsync(Convert.ToInt16(receivableInvoice.DocSuiteProtocolYear.Value), protocolNumber, expandChains: true).Result;
                        if (protocolDocumentUnit == null)
                        {
                            throw new ArgumentException($"Protocol not found with ERP identification, check columns in database: NUMERO_PROTOCOLLO, ANNO_PROTOCOLLO");
                        }
                        protocolContactSender = _webAPIClient.GetProtocolContactsAsync(protocolDocumentUnit.UniqueId, EntityComunicationType.Sender).Result.FirstOrDefault();
                        if (protocolContactSender == null)
                        {
                            protocolContactManualSender = _webAPIClient.GetProtocolContactManualsAsync(protocolDocumentUnit.UniqueId, EntityComunicationType.Sender).Result.FirstOrDefault();
                        }
                        if (protocolContactSender != null && protocolContactSender.Contact != null)
                        {
                            companyName = protocolContactSender.Contact.Description;
                            vatId = protocolContactSender.Contact.FiscalCode;
                        }
                        //if (protocolContactManualSender != null)
                        //{
                        //    companyName = protocolContactManualSender.Description;
                        //    vatId = protocolContactManualSender.FiscalCode;
                        //}
                        if (protocolDocumentUnit == null)
                        {
                            throw new ArgumentException($"Protocol contact sender {protocolDocumentUnit.Title} has no valid conservation information. Check FiscalCode and Descripion informations.");
                        }
                        uds_metadatas = new Dictionary<string, object>
                        {
                            { UDSEInvoiceHelper.UDSMetadata_DataFattura, new DateTimeOffset(receivableInvoice.InvoiceDate.Value) },
                            { UDSEInvoiceHelper.UDSMetadata_NumeroFattura, receivableInvoice.InvoiceNumber },
                            { UDSEInvoiceHelper.UDSMetadata_Importo, null },
                            { UDSEInvoiceHelper.UDSMetadata_TipoFattura, JsonConvert.SerializeObject(new List<string>(){ "Fattura" }) },
                            { UDSEInvoiceHelper.UDSMetadata_Cig, receivableInvoice.CIG },
                            { UDSEInvoiceHelper.UDSMetadata_Oda, receivableInvoice.ODA },
                            { UDSEInvoiceHelper.UDSMetadata_IdentificativoSdi, string.Empty },
                            { UDSEInvoiceHelper.UDSMetadata_ProgessivoInvioSdi, string.Empty },
                            { UDSEInvoiceHelper.UDSMetadata_HashSdi, string.Empty },
                            { UDSEInvoiceHelper.UDSMetadata_Denominazione, companyName },
                            { UDSEInvoiceHelper.UDSMetadata_Pivacf, vatId },
                            { UDSEInvoiceHelper.UDSMetadata_IndirizzoPec, string.Empty }
                        };
                        uds_metadatas = UDSEInvoiceHelper.MappingReceivableInvoiceMetadatas(uds_metadatas.ToDictionary(f => f.Key, f => f.Value), protocolDocumentUnit.RegistrationDate.DateTime);

                        try
                        {
                            receivableInvoiceMetadata = new ReceivableInvoiceMetadata
                            {
                                DateVAT = receivableInvoice.DateVAT,
                                ProtocolNumberVAT = Convert.ToInt32(receivableInvoice.ProtocolNumberVAT.Value),
                                SectionalVAT = receivableInvoice.SectionalVAT,
                                YearVAT = Convert.ToInt32(receivableInvoice.YearVAT.Value)
                            };
                        }
                        catch (Exception)
                        {
                            throw new ArgumentException("Invoice metadata validation error");
                        }

                        uds_metadatas = UDSEInvoiceHelper.MappingReceivableInvoiceFiscalMetadatas(receivableInvoiceMetadata, uds_metadatas.ToDictionary(f => f.Key, f => f.Value));
                        BuildReceivableInvoiceAsync(protocolDocumentUnit, uds_metadatas, uDSRepository, protocolContactSender?.Contact, roleModels).Wait();
                        receivableInvoice.WorkflowStatus = 1;
                        receivableInvoice.WorkflowProcessed = DateTime.UtcNow;
                        _dbContext.SaveChanges();
                        _logger.WriteInfo(new LogMessage($"Invoice {receivableInvoice.WorkflowId} has been updated"), LogCategories);
                        successfullCount++;
                    }
                    catch (ArgumentException ex)
                    {
                        _logger.WriteError(new LogMessage($"ERP.Invoice -> Metadata invalid on evaluating ReceivableInvoice id: {currentReceivableInvoice.WorkflowId}"), ex, LogCategories);
                        receivableInvoice.WorkflowStatus = 3;
                        _dbContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteError(new LogMessage($"ERP.Invoice -> Error on evaluating ReceivableInvoice id: {currentReceivableInvoice.WorkflowId}"), ex, LogCategories);
                        receivableInvoice.WorkflowStatus = 2;
                        _dbContext.SaveChanges();
                    }

                }
                _logger.WriteInfo(new LogMessage($"ERP ReceivableInvoice {successfullCount}/{totalCount} has been successfully evaluated"), LogCategories);

                successfullCount = 0;
                totalCount = 0;
                foreach (PayableInvoice payableInvoice in _dbContext.PayableInvoices.Where(f => f.TenantId == _databaseTenantId && f.WorkflowStarted == null && (f.WorkflowStatus == 0 || f.WorkflowStatus == null)))
                {
                    totalCount++;
                    currentPayableInvoice = payableInvoice;
                    payableInvoice.WorkflowStarted = DateTime.Now;
                    try
                    {
                        _logger.WriteInfo(new LogMessage($"Evaluating PayableInvoice {payableInvoice.InvoiceNumber} - {payableInvoice.Customer} - {payableInvoice.InvoiceFilename}"), LogCategories);
                        WorkflowStartPayableInvoiceAsync(payableInvoice).Wait();
                        payableInvoice.WorkflowStatus = 1;
                        _dbContext.SaveChanges();
                        _logger.WriteInfo(new LogMessage($"Invoice {payableInvoice.WorkflowId} has been updated"), LogCategories);
                        successfullCount++;
                    }
                    catch (ArgumentException ex)
                    {
                        _logger.WriteError(new LogMessage($"ERP.Invoice -> Invoice {currentPayableInvoice.RequestId} XML has no pass XSD validation {payableInvoice.InvoiceFilename}"), ex, LogCategories);
                        payableInvoice.WorkflowStatus = 3;
                        _dbContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteError(new LogMessage($"ERP.Invoice -> Error on evaluating PayableInvoice id: {currentPayableInvoice.RequestId}"), ex, LogCategories);
                        payableInvoice.WorkflowStatus = 2;
                        _dbContext.SaveChanges();
                    }

                }
                _logger.WriteInfo(new LogMessage($"ERP PayableInvoice {successfullCount}/{totalCount} has been successfully evaluated"), LogCategories);

            }
            catch (Exception ex)
            {
                //non mandare via email
                _logger.WriteError(new LogMessage("VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Invoice -> Critical Error"), ex, LogCategories);
                throw;
            }
        }
        
        protected override void OnStop()
        {
            CleanSubscriptions();
            _dbContext.Dispose();
            _logger.WriteInfo(new LogMessage("OnStop -> ***REMOVED***.ERP.Invoice"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _dbContext = new ERPDbContext(_logger, ModuleConfigurationHelper.JsonSerializerSettings, _moduleConfiguration.ConnectionString);
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCompleteUDSBuild>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicBuilderEvent,
                    _moduleConfiguration.WorkflowReceivableInvoiceUDSBuildCompleteSubscription, WorkflowReceivableInvoiceUDSBuildCompleteCallback));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCompleteUDSBuild>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicBuilderEvent,
                    _moduleConfiguration.WorkflowPayableInvoiceUDSBuildCompleteSubscription, WorkflowPayableInvoiceUDSBuildCompleteCallback));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCompleteProtocolBuild>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicBuilderEvent,
                    _moduleConfiguration.WorkflowPayableInvoiceProtocolBuildCompleteSubscription, WorkflowProtocolBuildCompleteCallback));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCompletePECMailBuild>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicBuilderEvent, 
                    _moduleConfiguration.WorkflowPayableInvoicePECMailBuildCompleteSubscription, WorkflowPECMailBuildCompleteCallback));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventReceivedReceiptPECMail>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration,
                    _moduleConfiguration.WorkflowStartUpdateReceiptMetadataInvoiceSubscription, WorkflowPECMailReceiptCallback));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCreatePECMail>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration,
                    _moduleConfiguration.WorkflowStartUpdateMetadataInvoiceSubscription, WorkflowCreatePECMailCallback));

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
        private async Task WorkflowStartPayableInvoiceAsync(PayableInvoice payableInvoice)
        {
            _logger.WriteDebug(new LogMessage($"WorkflowStartPayableInvoice -> evaluate {payableInvoice.InvoiceFilename} for request {payableInvoice.RequestId}"), LogCategories);

            try
            {
                InvoiceConfiguration invoiceConfiguration = null;
                XMLModelKind xmlModelKind = XMLModelKind.Invalid;
                List<Role> roles = new List<Role>();
                bool isInvoice = false;
                InvoiceContactModel invoiceContactModel = null;
                IDictionary<string, object> invoice_metadatas = new Dictionary<string, object>();
                IDictionary<string, byte[]> invoiceAttachments = new Dictionary<string, byte[]>();
                invoice_metadatas = EInvoiceHelper.FillPayableInvoiceMetadatas(payableInvoice.Invoice, invoice_metadatas, (a) => _logger.WriteDebug(new LogMessage(a), LogCategories),
                    out invoiceContactModel, out xmlModelKind, out invoiceAttachments);

                isInvoice |= xmlModelKind == XMLModelKind.InvoicePA_V12 || xmlModelKind == XMLModelKind.InvoicePR_V12;
                Guid correlationId = Guid.NewGuid();
                Guid protocolUniqueId = Guid.NewGuid();
                Guid udsID = Guid.NewGuid();

                if (!isInvoice || !_payableWorkflowConfigurations.Any(f => f.Key == xmlModelKind))
                {
                    throw new ArgumentException($"Invoice xml evaluation found unsupport {xmlModelKind} XmlModelKind. This implementation support only B2B/PA Invoices");
                }
                invoiceConfiguration = _payableWorkflowConfigurations.Single(f => f.Key == xmlModelKind).Value;
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
                    throw new ArgumentException($"contact description {invoiceContactModel.Description} is empty or SDIIdentification {invoiceContactModel.SDIIdentification} is empty  or Pivacf {invoiceContactModel.Pivacf} is empty");
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
                _logger.WriteDebug(new LogMessage($"Finding UDS invoice using {payableInvoice.InvoiceFilename} filename"), LogCategories);
                Dictionary<string, object> uds_metadatas = null;
                try
                {
                    uds_metadatas = await _webAPIClient.GetUDSByInvoiceFilename(controllerName, payableInvoice.InvoiceFilename, true, documents);
                }
                catch (InvalidOperationException)
                {
                    throw new ArgumentException($"Invoice {payableInvoice.InvoiceFilename} exists more than one unique record expected in UDS archive {invoiceConfiguration.UDSRepositoryName}");
                }
                if (uds_metadatas != null && uds_metadatas.Any())
                {
                    throw new ArgumentException($"Invoice {payableInvoice.InvoiceFilename} already inserted in UDS archive {invoiceConfiguration.UDSRepositoryName} with UDSId {uds_metadatas[UDSEInvoiceHelper.UDSMetadata_UDSId]}");
                }

                _logger.WriteDebug(new LogMessage($"Preaparing starting workflow with correlationId {correlationId}, protocolUniqueId {protocolUniqueId}, udsID {udsID}"), LogCategories);
                InvoiceFileModel invoiceFileModel = new InvoiceFileModel()
                {
                    InvoiceContent = Encoding.Default.GetBytes(payableInvoice.Invoice),
                    InvoiceFilename = payableInvoice.InvoiceFilename
                };
                if (invoiceConfiguration.SignInvoiceType == SignInvoiceType.AutomaticAruba && invoiceConfiguration.SignerParameter != null)
                {

                    _logger.WriteDebug(new LogMessage($"Preaparing Aruba ARSS request for delegated user {invoiceConfiguration.SignerParameter.DelegatedUser}"), LogCategories);
                    byte[] signDocument = _signServiceClient.SignDocument(invoiceConfiguration.SignerParameter, Encoding.Default.GetBytes(payableInvoice.Invoice));
                    _logger.WriteDebug(new LogMessage($"Aruba ARSS response document {signDocument.Length}"), LogCategories);
                    invoiceFileModel = new InvoiceFileModel()
                    {
                        InvoiceContent = signDocument,
                        InvoiceFilename = $"{payableInvoice.InvoiceFilename}.p7m"
                    };
                }
                WorkflowReferenceModel workflowReferenceModelUDS = CreateUDSBuildModel(payableInvoice, invoiceFileModel, contact, udsRepository, roleModels, udsID, protocolUniqueId, correlationId, invoice_metadatas,
                    invoiceAttachments, invoiceConfiguration.WorkflowRepositoryName);
                WorkflowReferenceModel workflowReferenceModelProtocol = CreateProtocolBuildModel(payableInvoice, invoiceFileModel, container, contact, udsRepository, roleModels, protocolUniqueId, udsID, invoiceConfiguration.ProtocolCategoryId,
                    correlationId, invoice_metadatas, invoiceAttachments, invoiceConfiguration.WorkflowRepositoryName);
                WorkflowReferenceModel workflowReferenceModelPECMail = CreatePECMailBuildModel(payableInvoice, invoiceFileModel, container, contact, pecMailBox, invoiceConfiguration.MailRecipients, udsRepository, roleModels, protocolUniqueId,
                    udsID, correlationId, invoice_metadatas, invoiceConfiguration.WorkflowRepositoryName);

                WorkflowResult workflowResult = await StartWorkflowAsync(workflowReferenceModelUDS, workflowReferenceModelProtocol, workflowReferenceModelPECMail, invoiceConfiguration.WorkflowRepositoryName);
                if (!workflowResult.IsValid || !workflowResult.InstanceId.HasValue)
                {
                    _logger.WriteError(new LogMessage("An error occured in start payable invoice workflow"), LogCategories);
                    throw new Exception("VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Invoice");
                }
                payableInvoice.WorkflowId = correlationId.ToString();
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

        private WorkflowReferenceModel CreateProtocolBuildModel(PayableInvoice payableInvoice, InvoiceFileModel invoiceFileModel, Container container, Contact contact,
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
            protocolBuildModel.Protocol.DocumentCode = invoiceFileModel.InvoiceFilename;
            //protocolBuildModel.Protocol.Contacts.Add(new ProtocolContactModel()
            //{
            //    ComunicationType = ComunicationType.Recipient,
            //    IdContact = 206,
            //});
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
            protocolBuildModel.Protocol.MainDocument = new DocumentModel()
            {
                FileName = invoiceFileModel.InvoiceFilename,
                ContentStream = invoiceFileModel.InvoiceContent
            };
            protocolBuildModel.Protocol.Attachments = invoiceAttachments.Select(f => new DocumentModel() { FileName = f.Key, ContentStream = f.Value }).ToList();

            protocolBuildModel.WorkflowActions.Add(new WorkflowActionDocumentUnitLinkModel(
                new DocumentUnitModel() { UniqueId = protocolUniqueId, Environment = (int)DSWEnvironmentType.Protocol },
                new DocumentUnitModel() { UniqueId = udsID, Environment = uDSRepository.DSWEnvironment, IdUDSRepository = uDSRepository.UniqueId }));

            /*protocolBuildModel.WorkflowActions.Add(new WorkflowActionDocumentUnitLinkModel(
               new DocumentUnitModel() { UniqueId = protocolUniqueId, Environment = (int)DSWEnvironmentType.Protocol },
               new DocumentUnitModel() { UniqueId = pecMail.UniqueId, EntityId = pecMail.EntityId, Environment = (int)DSWEnvironmentType.PECMail }));*/
            //protocolBuildModel.WorkflowActions.Add(new WorkflowActionFascicleModel(
            //    new FascicleModel() { UniqueId = Guid.NewGuid() },
            //    new DocumentUnitModel() { UniqueId = protocolUniqueId, Environment = 1 }));

            workflowReferenceModel.ReferenceType = DSWEnvironmentType.Build;
            workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(protocolBuildModel, ModuleConfigurationHelper.JsonSerializerSettings);
            return workflowReferenceModel;
        }

        private WorkflowReferenceModel CreatePECMailBuildModel(PayableInvoice payableInvoice, InvoiceFileModel invoiceFileModel, Container container, Contact contact,
            PECMailBox pecMailBox, string mailRecipients, UDSRepository uDSRepository, List<RoleModel> roles, Guid protocolUniqueId, Guid udsID, Guid correlationId,
            IDictionary<string, object> invoice_metadatas, string workflowName)
        {
            WorkflowReferenceModel workflowReferenceModel = new WorkflowReferenceModel
            {
                ReferenceId = correlationId
            };
            PECMailBuildModel pecMailBuildModel = new PECMailBuildModel
            {
                WorkflowName = workflowName,
                UniqueId = workflowReferenceModel.ReferenceId,
                PECMail = new PECMailModel
                {
                    Direction = DocSuiteWeb.Model.Entities.PECMails.PECMailDirection.Outgoing,
                    InvoiceStatus = DocSuiteWeb.Model.Entities.PECMails.InvoiceStatus.InvoiceLookingMetadata,
                    IsActive = DocSuiteWeb.Model.Entities.PECMails.PECMailActiveType.Active,
                    MailBody = $"Invio Fattura n° {invoice_metadatas[EInvoiceHelper.Metadata_NumeroFattura]} del {((DateTimeOffset)invoice_metadatas[EInvoiceHelper.Metadata_DataFattura]).LocalDateTime.ToShortDateString()}",
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
            pecMailBuildModel.PECMail.Attachments.Add(new DocumentModel()
            {
                FileName = invoiceFileModel.InvoiceFilename,
                ContentStream = invoiceFileModel.InvoiceContent
            });

            pecMailBuildModel.WorkflowActions.Add(new WorkflowActionDocumentUnitLinkModel(
               new DocumentUnitModel() { UniqueId = protocolUniqueId, Environment = (int)DSWEnvironmentType.Protocol },
               new DocumentUnitModel() { Environment = (int)DSWEnvironmentType.PECMail }));
            //protocolBuildModel.WorkflowActions.Add(new WorkflowActionFascicleModel(
            //    new FascicleModel() { UniqueId = Guid.NewGuid() },
            //    new DocumentUnitModel() { UniqueId = protocolUniqueId, Environment = 1 }));

            workflowReferenceModel.ReferenceType = DSWEnvironmentType.Build;
            workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(pecMailBuildModel, ModuleConfigurationHelper.JsonSerializerSettings);
            return workflowReferenceModel;
        }

        private WorkflowReferenceModel CreateUDSBuildModel(PayableInvoice payableInvoice, InvoiceFileModel invoiceFileModel, Contact contact,
            UDSRepository uDSRepository, List<RoleModel> roleModels, Guid udsID, Guid protocolUniqueId, Guid correlationId,
            IDictionary<string, object> invoice_metadatas, IDictionary<string, byte[]> invoiceAttachments, string workflowName)
        {

            WorkflowReferenceModel workflowReferenceModel = new WorkflowReferenceModel
            {
                ReferenceId = correlationId
            };
            UDSModel model = UDSModel.LoadXml(uDSRepository.ModuleXML);
            model.Model.UDSId = udsID.ToString();
            model.Model.Subject.Value = $"{invoice_metadatas[EInvoiceHelper.Metadata_Denominazione]} - Fattura n° {invoice_metadatas[EInvoiceHelper.Metadata_NumeroFattura]} del {((DateTimeOffset)invoice_metadatas[EInvoiceHelper.Metadata_DataFattura]).LocalDateTime.ToShortDateString()}";
            IDictionary<string, object> uds_metadatas = UDSEInvoiceHelper.MappingPayableInvoiceMetadatas(new PayableInvoiceMetadata()
            {
                DateVAT = payableInvoice.DateVAT,
                ProtocolNumberVAT = payableInvoice.ProtocolNumberVAT.HasValue ? (int)payableInvoice.ProtocolNumberVAT.Value : default(int?),
                SectionalVAT = payableInvoice.SectionalVAT,
                YearVAT = payableInvoice.YearVAT.HasValue ? (int)payableInvoice.YearVAT.Value : default(int?),
            }, invoice_metadatas);
            uds_metadatas[UDSEInvoiceHelper.UDSMetadata_StatoFattura] = UDSEInvoiceHelper.UDSInvoiceStatusStored;
            model.FillMetaData(uds_metadatas);
            model = UDSEInvoiceHelper.InitDocumentStructures(model);
            model.Model.Documents.Document.Instances = UDSEInvoiceHelper.FillDocumentInstances(new List<InvoiceFileModel>() { invoiceFileModel });
            model.Model.Documents.DocumentAttachment.Instances = UDSEInvoiceHelper.FillDocumentInstances(invoiceAttachments.Select(f => new InvoiceFileModel() { InvoiceFilename = f.Key, InvoiceContent = f.Value }).ToList());

            Contacts contacts = model.Model.Contacts.Single();
            if (contacts.ContactInstances == null)
            {
                contacts.ContactInstances = new ContactInstance[]
                {
                    new ContactInstance() { IdContact = contact.EntityId }
                };
            }

            model.Model.Authorizations = model.Model.Authorizations ?? new Authorizations();
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
            udsBuildModel.Roles = roleModels;
            workflowReferenceModel.ReferenceType = DSWEnvironmentType.Build;
            workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(udsBuildModel, ModuleConfigurationHelper.JsonSerializerSettings);
            return workflowReferenceModel;
        }


        private async Task<WorkflowResult> StartWorkflowAsync(WorkflowReferenceModel workflowReferenceModelUDS,
            WorkflowReferenceModel workflowReferenceModelProtocol, WorkflowReferenceModel workflowReferenceModelPECMail, string workflowName)
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

        #region [ UpdateInvoiceMetadata ]
        private async Task UpdateInvoiceMetadataAsync(ReceivableInvoice receivableInvoice)
        {
            _logger.WriteDebug(new LogMessage($"Get Invoice UDS using WorkflowId {receivableInvoice.WorkflowId}"), LogCategories);
            Guid idUDS = Guid.Parse(receivableInvoice.WorkflowId);
            ICollection<UDSDocumentUnit> udsDocumentUnits = await _webAPIClient.GetUDSDocumentUnits(idUDS, false, true);
            UDSRepository udsRepository = udsDocumentUnits.First().Repository;
            string controllerName = Utils.GetWebAPIControllerName(udsRepository.Name);
            Dictionary<int, Guid> documents = new Dictionary<int, Guid>();
            _logger.WriteDebug(new LogMessage($"Get UDS {receivableInvoice.WorkflowId} invoice metadatas"), LogCategories);
            IDictionary<string, object> uds_metadatas = await _webAPIClient.GetUDS(controllerName, idUDS, documents);

            if (uds_metadatas == null || !uds_metadatas.Any())
            {
                throw new ArgumentNullException($"Wrong invoice metadata not related to valid UDSId {idUDS}");
            }

            _logger.WriteInfo(new LogMessage($"UDS {idUDS} updating invoice metadata"), LogCategories);
            _logger.WriteDebug(new LogMessage($"metadata YearVAT {receivableInvoice.YearVAT}"), LogCategories);
            _logger.WriteDebug(new LogMessage($"metadata DateVAT {receivableInvoice.DateVAT}"), LogCategories);
            _logger.WriteDebug(new LogMessage($"metadata ProtocolNumberVAT {receivableInvoice.ProtocolNumberVAT}"), LogCategories);
            _logger.WriteDebug(new LogMessage($"metadata SectionalVAT {receivableInvoice.SectionalVAT}"), LogCategories);
            _logger.WriteDebug(new LogMessage($"metadata Has ReverseCharge {!string.IsNullOrEmpty(receivableInvoice.AutoInvoice)} && {!string.IsNullOrEmpty(receivableInvoice.AutoInvoiceFilename)}"), LogCategories);
            ReceivableInvoiceMetadata receivableInvoiceMetadata;
            try
            {
                receivableInvoiceMetadata = new ReceivableInvoiceMetadata
                {
                    DateVAT = receivableInvoice.DateVAT,
                    ProtocolNumberVAT = Convert.ToInt32(receivableInvoice.ProtocolNumberVAT.Value),
                    SectionalVAT = receivableInvoice.SectionalVAT,
                    YearVAT = Convert.ToInt32(receivableInvoice.YearVAT.Value)
                };
            }
            catch (Exception)
            {
                throw new ArgumentException("Invoice metadata validation error");
            }
            uds_metadatas = UDSEInvoiceHelper.MappingReceivableInvoiceFiscalMetadatas(receivableInvoiceMetadata, uds_metadatas);

            ICollection<UDSRole> udsRoles = await _webAPIClient.GetUDSRoles(idUDS);
            ICollection<UDSContact> udsContacts = await _webAPIClient.GetUDSContacts(idUDS);
            ICollection<UDSMessage> udsMessages = await _webAPIClient.GetUDSMessages(idUDS);
            ICollection<UDSPECMail> udsPECMails = await _webAPIClient.GetUDSPECMails(idUDS);
            List<InvoiceFileModel> invoiceFiles = new List<InvoiceFileModel>();
            if (!string.IsNullOrEmpty(receivableInvoice.AutoInvoice) && !string.IsNullOrEmpty(receivableInvoice.AutoInvoiceFilename))
            {
                invoiceFiles.Add(new InvoiceFileModel()
                {
                    InvoiceContent = Encoding.Default.GetBytes(receivableInvoice.AutoInvoice),
                    InvoiceFilename = receivableInvoice.AutoInvoiceFilename
                });
            }
            UDSBuildModel udsBuildModel = UDSEInvoiceHelper.PrepareUpdateUDSBuildModel(udsRepository, idUDS, uds_metadatas, documents, udsRoles, udsContacts,
                udsMessages, udsPECMails, udsDocumentUnits, invoiceFiles, _identityContext.User);
            CommandUpdateUDSData commandUpdateUDSData = new CommandUpdateUDSData(_moduleConfiguration.TenantName, _moduleConfiguration.TenantId, _identityContext, udsBuildModel);
            await _webAPIClient.SendCommandAsync(commandUpdateUDSData);
            _logger.WriteInfo(new LogMessage($"Updating metadata invoice {commandUpdateUDSData.Id} has been sended"), LogCategories);
        }

        private async Task BuildReceivableInvoiceAsync(DocumentUnit protocolDocumentUnit, IDictionary<string, object> uds_metadatas, UDSRepository uDSRepository, Contact contact, List<RoleModel> roleModels)
        {
            Guid udsID = Guid.NewGuid();
            Guid correlationId = Guid.NewGuid();
            Guid protocolUniqueId = protocolDocumentUnit.UniqueId;
            UDSModel model = UDSModel.LoadXml(uDSRepository.ModuleXML);
            model.Model.UDSId = udsID.ToString();
            model.Model.Subject.Value = $"{uds_metadatas[EInvoiceHelper.Metadata_Denominazione]} - Fattura passiva n° {uds_metadatas[EInvoiceHelper.Metadata_NumeroFattura]} del {((DateTimeOffset)uds_metadatas[EInvoiceHelper.Metadata_DataFattura]).LocalDateTime.ToShortDateString()}";
            model.FillMetaData(uds_metadatas);
            model = UDSEInvoiceHelper.InitDocumentStructures(model);

            List<InvoiceFileModel> invoiceFileModels = new List<InvoiceFileModel>();
            DocumentUnitChain documentUnitChain = protocolDocumentUnit.DocumentUnitChains.Single(f => f.ChainType == DocSuiteWeb.Entity.DocumentUnits.ChainType.MainChain);
            BiblosDocument biblosDocument = (await _documentClient.GetDocumentChildrenAsync(documentUnitChain.IdArchiveChain)).First();
            Content documentContent = (await _documentClient.GetDocumentContentByIdAsync(biblosDocument.IdDocument));
            invoiceFileModels.Add(new InvoiceFileModel()
            {
                InvoiceContent = documentContent.Blob,
                InvoiceFilename = biblosDocument.Name
            });
            model.Model.Documents.Document.Instances = UDSEInvoiceHelper.FillDocumentInstances(invoiceFileModels);

            invoiceFileModels.Clear();
            documentUnitChain = protocolDocumentUnit.DocumentUnitChains.Single(f => f.ChainType == DocSuiteWeb.Entity.DocumentUnits.ChainType.AttachmentsChain);
            foreach (BiblosDocument document in (await _documentClient.GetDocumentChildrenAsync(documentUnitChain.IdArchiveChain)))
            {
                documentContent = (await _documentClient.GetDocumentContentByIdAsync(biblosDocument.IdDocument));
                invoiceFileModels.Add(new InvoiceFileModel()
                {
                    InvoiceContent = documentContent.Blob,
                    InvoiceFilename = document.Name
                });
            }

            model.Model.Documents.DocumentAttachment.Instances = UDSEInvoiceHelper.FillDocumentInstances(invoiceFileModels);
            invoiceFileModels.Clear();

            Contacts contacts = model.Model.Contacts.Single();
            if (contacts.ContactInstances == null)
            {
                contacts.ContactInstances = new ContactInstance[]
                {
                    new ContactInstance() { IdContact = contact.EntityId }
                };
            }

            model.Model.Authorizations = model.Model.Authorizations ?? new Authorizations();
            model = UDSEInvoiceHelper.FillAuthorizations(model, roleModels);

            UDSBuildModel udsBuildModel = new UDSBuildModel(model.SerializeToXml())
            {
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
            udsBuildModel.Roles = roleModels;
            udsBuildModel.WorkflowActions.Add(new WorkflowActionDocumentUnitLinkModel(
                new DocumentUnitModel() { UniqueId = protocolUniqueId, Environment = (int)DSWEnvironmentType.Protocol },
                new DocumentUnitModel() { UniqueId = udsID, Environment = uDSRepository.DSWEnvironment, IdUDSRepository = uDSRepository.UniqueId }));
            CommandInsertUDSData commandInsertUDSData = new CommandInsertUDSData(_moduleConfiguration.TenantName, _moduleConfiguration.TenantId, _identityContext, udsBuildModel);

            await _webAPIClient.SendCommandAsync(commandInsertUDSData);
            _logger.WriteInfo(new LogMessage($"Insert invoice command {commandInsertUDSData.Id} has been sended"), LogCategories);
        }

        #endregion

        #region [ WorkflowReceivableInvoiceUDSBuildCompleteCallback ]
        private async Task WorkflowReceivableInvoiceUDSBuildCompleteCallback(IEventCompleteUDSBuild evt)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"WorkflowReceivableInvoiceUDSBuildCompleteCallback -> evaluate event id {evt.Id}"), LogCategories);
                _logger.WriteInfo(new LogMessage($"Notifying UDSBuildComplete for WorkflowInstanceId {evt.CorrelationId}"), LogCategories);

                UDSDocumentUnit protocolDocumentUnit;
                Protocol protocol;
                PECMail pecMail;
                KeyValuePair<string, InvoiceFilePersistance> persistanceInvoiceConfiguration;
                KeyValuePair<XMLModelKind, InvoiceConfiguration> invoiceConfiguration;
                Content invoiceContent;
                BiblosDocument invoiceDocument;
                List<BiblosDocument> invoiceDocuments;
                string xmlContent;
                string controllerName;
                string mailBoxRecipient;
                Guid invoiceDocumentChain;
                Dictionary<int, Guid> documents;
                UDSBuildModel udsBuildModel = evt.ContentType.ContentTypeValue;
                _logger.WriteInfo(new LogMessage($"Notifying UDSBuildComplete for WorkflowActivityId {udsBuildModel.IdWorkflowActivity}"), LogCategories);
                protocolDocumentUnit = await RetryingPolicyActionAsync(async () =>
                {
                    UDSDocumentUnit result = (await _webAPIClient.GetUDSDocumentUnits(udsBuildModel.UniqueId, false, true)).FirstOrDefault(f => f.Relation.Environment == (int)DSWEnvironmentType.Protocol);
                    if (result == null)
                    {
                        throw new ArgumentNullException($"Received a wrong message invalid UDSId {udsBuildModel.UniqueId}: UDS not found");
                    }
                    return result;
                });
                if (!_receivableWorkflowConfigurations.Any(f => f.Value.UDSRepositoryName == protocolDocumentUnit.Repository.Name))
                {
                    throw new ArgumentNullException($"Received a wrong message UDSId {protocolDocumentUnit.IdUDS} {protocolDocumentUnit.Repository.Name}: Not related to valid invoice configuration environment");
                }
                invoiceConfiguration = _receivableWorkflowConfigurations.SingleOrDefault(f => f.Value.UDSRepositoryName == protocolDocumentUnit.Repository.Name);
                _logger.WriteDebug(new LogMessage($"Found protocol {protocolDocumentUnit.Relation.UniqueId} associated to UDS {protocolDocumentUnit.IdUDS}"), LogCategories);

                protocol = (await _webAPIClient.GetProtocolAsync($"$filter=Year eq {protocolDocumentUnit.Relation.Year} and Number eq {protocolDocumentUnit.Relation.Number}&$expand=AdvancedProtocol")).SingleOrDefault();
                if (protocol == null)
                {
                    throw new ArgumentNullException($"protocol not found for identification {protocolDocumentUnit.Relation.Year}/ {protocolDocumentUnit.Relation.Number}");
                }
                _logger.WriteDebug(new LogMessage($"Found protocol {protocol.UniqueId} associated to UDS {protocolDocumentUnit.Relation.UniqueId}"), LogCategories);
                mailBoxRecipient = string.Empty;
                pecMail = (await _webAPIClient.GetPECMailFromProtocol(protocolDocumentUnit.Relation.Year, protocolDocumentUnit.Relation.Number)).FirstOrDefault();
                if (pecMail != null)
                {
                    _logger.WriteDebug(new LogMessage($"Found PECMail {pecMail.UniqueId} associated to protocol {protocolDocumentUnit.Relation.UniqueId}"), LogCategories);
                    mailBoxRecipient = pecMail.PECMailBox.MailBoxRecipient;
                }

                if (invoiceConfiguration.Value.InvoicePersistanceConfigurations.Any(f => f.Key.Equals(mailBoxRecipient, StringComparison.InvariantCultureIgnoreCase)))
                {
                    persistanceInvoiceConfiguration = invoiceConfiguration.Value.InvoicePersistanceConfigurations.Single(f => f.Key.Equals(mailBoxRecipient, StringComparison.InvariantCultureIgnoreCase));
                    _logger.WriteDebug(new LogMessage($"Specific configuration found by MailBoxRecipient {mailBoxRecipient}"), LogCategories);
                }
                else
                {
                    _logger.WriteDebug(new LogMessage($"Specific configuration not found for {mailBoxRecipient}. Using default configuration"), LogCategories);
                    persistanceInvoiceConfiguration = invoiceConfiguration.Value.InvoicePersistanceConfigurations.Single(f => string.IsNullOrEmpty(f.Key));

                }
                if (persistanceInvoiceConfiguration.Value == null || string.IsNullOrEmpty(persistanceInvoiceConfiguration.Value.UDSRepositoryName))
                {
                    throw new ArgumentNullException($"Persistance invoice configuration not found for {mailBoxRecipient} MailBoxRecipient");
                }

                controllerName = Utils.GetWebAPIControllerName(persistanceInvoiceConfiguration.Value.UDSRepositoryName);
                documents = new Dictionary<int, Guid>();
                _logger.WriteDebug(new LogMessage($"Get UDS {protocolDocumentUnit.IdUDS} invoice metadatas"), LogCategories);
                Dictionary<string, object> uds_metadatas = await _webAPIClient.GetUDS(controllerName, protocolDocumentUnit.IdUDS, documents);
                _logger.WriteDebug(new LogMessage($"Found {uds_metadatas != null} invoice metadatas {documents.ContainsKey(1)}"), LogCategories);

                xmlContent = string.Empty;
                invoiceDocumentChain = documents[1];
                _logger.WriteDebug(new LogMessage($"Get InvoiceDocument {invoiceDocumentChain}"), LogCategories);
                invoiceDocuments = await _documentClient.GetDocumentChildrenAsync(invoiceDocumentChain);
                _logger.WriteDebug(new LogMessage($"Found {invoiceDocuments?.Count} documents. Main document has identificaiton {invoiceDocuments?.Single(f => f.IsLatestVersion)?.IdDocument}"), LogCategories);
                invoiceDocument = invoiceDocuments.Single(f => f.IsLatestVersion);
                invoiceContent = await _documentClient.GetDocumentContentByIdAsync(invoiceDocument.IdDocument);
                if (new FileInfo(invoiceDocument.Name).Extension.Equals(".xml", StringComparison.InvariantCultureIgnoreCase))
                {
                    xmlContent = XmlConvert.GetEncoding(invoiceContent.Blob).GetString(invoiceContent.Blob);
                }
                if (new FileInfo(invoiceDocument.Name).Extension.Equals(".p7m", StringComparison.InvariantCultureIgnoreCase))
                {
                    xmlContent = EInvoiceHelper.TryGetInvoiceSignedContent(invoiceContent.Blob, (f, ex) => _logger.WriteWarning(new LogMessage(f), ex, LogCategories));
                }
                using (ERPDbContext dbContext = new ERPDbContext(_logger, ModuleConfigurationHelper.JsonSerializerSettings, _moduleConfiguration.ConnectionString))
                {
                    ReceivableInvoice receivableInvoice = new ReceivableInvoice()
                    {
                        TenantId = _databaseTenantId,
                        DateVAT = null,
                        DocSuiteProtocolNumber = protocolDocumentUnit.Relation.Number.ToString("0000000"),
                        DocSuiteProtocolYear = new decimal(protocolDocumentUnit.Relation.Year),
                        DocSuiteProtocolDate = protocolDocumentUnit.Relation.RegistrationDate.ToLocalTime().DateTime,
                        ERPUpdatedDate = null,
                        Invoice = xmlContent,
                        InvoiceDate = (DateTime)uds_metadatas[UDSEInvoiceHelper.UDSMetadata_DataFattura],
                        InvoiceFilename = invoiceDocument.Name,
                        InvoiceNumber = (string)uds_metadatas[UDSEInvoiceHelper.UDSMetadata_NumeroFattura],
                        PIVACF = (string)uds_metadatas[UDSEInvoiceHelper.UDSMetadata_Pivacf],
                        CIG = uds_metadatas.ContainsKey(UDSEInvoiceHelper.UDSMetadata_Cig) ? (string)uds_metadatas[UDSEInvoiceHelper.UDSMetadata_Cig] : null,
                        ODA = uds_metadatas.ContainsKey(UDSEInvoiceHelper.UDSMetadata_Oda) ? (string)uds_metadatas[UDSEInvoiceHelper.UDSMetadata_Oda] : null,
                        ProtocolNumberVAT = null,
                        SDIDate = uds_metadatas.ContainsKey(UDSEInvoiceHelper.UDSMetadata_DataRicezioneSdi) ? (DateTime?)uds_metadatas[UDSEInvoiceHelper.UDSMetadata_DataRicezioneSdi] : null,
                        SDIIdentification = protocol.AdvancedProtocol.IdentificationSdi,
                        SDIResult = "Consegna",
                        SDIResultDescription = string.Empty,
                        SectionalVAT = null,
                        Supplier = uds_metadatas.ContainsKey(UDSEInvoiceHelper.UDSMetadata_Denominazione) ? (string)uds_metadatas[UDSEInvoiceHelper.UDSMetadata_Denominazione] : null,
                        WorkflowId = protocolDocumentUnit.IdUDS.ToString(),
                        WorkflowProcessed = evt.CreationTime.ToLocalTime().DateTime,
                        WorkflowStatus = 0,
                        YearVAT = null,
                    };
                    dbContext.ReceivableInvoices.Add(receivableInvoice);
                    dbContext.SaveChanges();
                }
                _logger.WriteInfo(new LogMessage($"Invoice {udsBuildModel.UniqueId} has been successfully stored in ERP database"), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("WorkflowReceivableInvoiceUDSBuildCompleteCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }


        private async Task<T> RetryingPolicyActionAsync<T>(Func<Task<T>> func, int step = 1, int retry_tentative = 10)
        {
            _logger.WriteDebug(new LogMessage($"RetryingPolicyAction : tentative {step}/{retry_tentative} in progress..."), LogCategories);
            if (step >= retry_tentative)
            {
                _logger.WriteError(new LogMessage("VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Invoice.RetryingPolicyAction: retry policy expired maximum tentatives"), LogCategories);
                throw new Exception("ReceivableInvoice retry policy expired maximum tentatives");
            }
            try
            {
                return await func();
            }
            catch (Exception ex)
            {
                _logger.WriteWarning(new LogMessage($"SafeActionWithRetryPolicy : tentative {step}/{retry_tentative} faild. Waiting {_threadWaiting} second before retrying action"), ex, LogCategories);
                Task.Delay(_threadWaiting).Wait();
                return await RetryingPolicyActionAsync(func, ++step);
            }
        }

        #endregion

        #region [ WorkflowUDSBuildCompleteCallback ]
        private async Task WorkflowPayableInvoiceUDSBuildCompleteCallback(IEventCompleteUDSBuild evt)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"WorkflowPayableInvoiceUDSBuildCompleteCallback -> evaluate event id {evt.Id}"), LogCategories);
                _logger.WriteInfo(new LogMessage($"Notifying UDSBuildComplete for WorkflowInstanceId {evt.CorrelationId}"), LogCategories);
                WorkflowNotify workflowNotify = null;
                WorkflowResult workflowResult = null;

                UDSBuildModel udsBuildModel = evt.ContentType.ContentTypeValue;
                _logger.WriteInfo(new LogMessage($"Notifying UDSBuildComplete for IdWorkflowActivity {udsBuildModel.IdWorkflowActivity}"), LogCategories);
                workflowNotify = new WorkflowNotify(udsBuildModel.IdWorkflowActivity.Value)
                {
                    WorkflowName = udsBuildModel.WorkflowName,
                    ModuleName = ModuleConfigurationHelper.MODULE_NAME
                };
                workflowResult = await _webAPIClient.WorkflowNotify(workflowNotify);
                _logger.WriteInfo(new LogMessage(string.Concat("Workflow notify correctly [IsValid: ", workflowResult.IsValid, "] with instanceId ", workflowResult.InstanceId)), LogCategories);
                if (!workflowResult.IsValid)
                {
                    _logger.WriteError(new LogMessage("An error occured in notify workflow activity"), LogCategories);
                    throw new Exception("VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Invoice.WorkflowPayableInvoiceUDSBuildCompleteCallback");
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("WorkflowPayableInvoiceUDSBuildCompleteCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        #endregion

        #region [ WorkflowProtocolBuildCompleteCallback ]
        private async Task WorkflowProtocolBuildCompleteCallback(IEventCompleteProtocolBuild evt)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"WorkflowProtocolBuildCompleteCallback -> evaluate event id {evt.Id}"), LogCategories);
                _logger.WriteInfo(new LogMessage($"Notifying ProtocolBuildComplete for WorkflowInstanceId {evt.CorrelationId}"), LogCategories);

                WorkflowNotify workflowNotify = null;
                WorkflowResult workflowResult = null;
                ProtocolBuildModel protocolBuildModel = evt.ContentType.ContentTypeValue;
                using (ERPDbContext dbContext = new ERPDbContext(_logger, ModuleConfigurationHelper.JsonSerializerSettings, _moduleConfiguration.ConnectionString))
                {
                    string key = evt.CorrelationId.ToString();
                    PayableInvoice payableInvoice = dbContext.PayableInvoices.SingleOrDefault(f => f.WorkflowId == key);
                    if (payableInvoice == null)
                    {
                        _logger.WriteError(new LogMessage($"PayableInvoice not found with WorkflowId: {key}"), LogCategories);
                        throw new Exception($"PayableInvoice not found with WorkflowId: {key}");
                    }
                    _logger.WriteInfo(new LogMessage($"Setting Protocol reference {protocolBuildModel.Protocol.Year}/{protocolBuildModel.Protocol.Number.ToString("0000000")} to invoice WorkflowInstanceId {evt.CorrelationId}"), LogCategories);
                    payableInvoice.DocSuiteProtocolYear = protocolBuildModel.Protocol.Year;
                    payableInvoice.DocSuiteProtocolNumber = protocolBuildModel.Protocol.Number.ToString("0000000");
                    dbContext.SaveChanges();
                }

                _logger.WriteInfo(new LogMessage($"Notifying ProtocolBuildComplete for IdWorkflowActivity {protocolBuildModel.IdWorkflowActivity}"), LogCategories);
                workflowNotify = new WorkflowNotify(protocolBuildModel.IdWorkflowActivity.Value)
                {
                    WorkflowName = protocolBuildModel.WorkflowName,
                    ModuleName = ModuleConfigurationHelper.MODULE_NAME
                };
                workflowResult = await _webAPIClient.WorkflowNotify(workflowNotify);
                _logger.WriteInfo(new LogMessage(string.Concat("Workflow notify correctly [IsValid: ", workflowResult.IsValid, "] with instanceId ", workflowResult.InstanceId)), LogCategories);
                if (!workflowResult.IsValid)
                {
                    _logger.WriteError(new LogMessage("An error occured in notify workflow activity"), LogCategories);
                    throw new Exception("VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Invoice.WorkflowProtocolBuildCompleteCallback");
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("WorkflowReceivableInvoiceProtocolBuildCompleteCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        #endregion

        #region [ WorkflowPECMailBuildCompleteCallback ]
        private async Task WorkflowPECMailBuildCompleteCallback(IEventCompletePECMailBuild evt)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"WorkflowPECMailBuildCompleteCallback -> evaluate event id {evt.Id}"), LogCategories);
                _logger.WriteInfo(new LogMessage($"Notifying PECMailBuildComplete for WorkflowInstanceId {evt.CorrelationId}"), LogCategories);
                WorkflowNotify workflowNotify = null;
                WorkflowResult workflowResult = null;
                PECMailBuildModel pecMailBuildModel = evt.ContentType.ContentTypeValue;
                _logger.WriteInfo(new LogMessage($"Notifying PECMailBuildComplete for IdWorkflowActivity {pecMailBuildModel.IdWorkflowActivity}"), LogCategories);
                workflowNotify = new WorkflowNotify(pecMailBuildModel.IdWorkflowActivity.Value)
                {
                    WorkflowName = pecMailBuildModel.WorkflowName,
                    ModuleName = ModuleConfigurationHelper.MODULE_NAME
                };
                workflowResult = await _webAPIClient.WorkflowNotify(workflowNotify);
                _logger.WriteInfo(new LogMessage(string.Concat("Workflow notify correctly [IsValid: ", workflowResult.IsValid, "] with instanceId ", workflowResult.InstanceId)), LogCategories);
                if (!workflowResult.IsValid)
                {
                    _logger.WriteError(new LogMessage("An error occured in notify workflow activity"), LogCategories);
                    throw new Exception("VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Invoice.WorkflowPECMailBuildCompleteCallback");
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("WorkflowReceivableInvoicePECMailBuildCompleteCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }
        #endregion

        #region [ WorkflowPECMailReceiptCallback ]

        private async Task WorkflowPECMailReceiptCallback(IEventReceivedReceiptPECMail evt)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"WorkflowReceiptPECMailCallback -> evaluate event id {evt.Id}"), LogCategories);
                _logger.WriteInfo(new LogMessage($"Notifying PECMailReceipt"), LogCategories);
                Guid protocolUniqueId = Guid.Empty;
                if (!evt.CustomProperties.ContainsKey(CustomPropertyName.PROTOCOL_UNIQUE_ID) ||
                    !Guid.TryParse(evt.CustomProperties[CustomPropertyName.PROTOCOL_UNIQUE_ID].ToString(), out protocolUniqueId))
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

                if (udsDocumentUnit == null || !_payableWorkflowConfigurations.Any(f => f.Value.UDSRepositoryName == udsDocumentUnit.Repository.Name))
                {
                    throw new ArgumentNullException($"Wrong message received ProtocolUniqueId {protocolUniqueId} {evt.CustomProperties[CustomPropertyName.PROTOCOL_YEAR]}/{evt.CustomProperties[CustomPropertyName.PROTOCOL_NUMBER]} not related to valid Invoice Environment");
                }
                using (ERPDbContext dbContext = new ERPDbContext(_logger, ModuleConfigurationHelper.JsonSerializerSettings, _moduleConfiguration.ConnectionString))
                {
                    short year = (short)(long)evt.CustomProperties[CustomPropertyName.PROTOCOL_YEAR];
                    string number = ((int)(long)evt.CustomProperties[CustomPropertyName.PROTOCOL_NUMBER]).ToString("0000000");
                    PayableInvoice payableInvoice = dbContext.PayableInvoices.SingleOrDefault(f => f.DocSuiteProtocolYear == year && f.DocSuiteProtocolNumber == number);
                    if (payableInvoice == null)
                    {
                        _logger.WriteWarning(new LogMessage($"ERP payable invoice not found with protocol reference {year}/{number}"), LogCategories);
                    }
                    else
                    {
                        _logger.WriteInfo(new LogMessage($"Updating invoice SDI metadata [{receiptDate}/{receiptType} ] to ERP request {payableInvoice.RequestId}"), LogCategories);
                        payableInvoice.SDIDate = receiptDate;
                        payableInvoice.SDIResult = receiptType;
                        dbContext.SaveChanges();
                    }
                }

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
                ICollection<UDSDocumentUnit> udsDocumentUnits = await _webAPIClient.GetUDSDocumentUnits(udsDocumentUnit.IdUDS, false, false);

                UDSBuildModel udsBuildModel = UDSEInvoiceHelper.PrepareUpdateUDSBuildModel(udsDocumentUnit.Repository, udsDocumentUnit.IdUDS, uds_metadatas, documents,
                    udsRoles, udsContacts, udsMessages, udsPECMails, udsDocumentUnits, null, evt.Identity.User);
                CommandUpdateUDSData commandUpdateUDSData = new CommandUpdateUDSData(evt.Name, evt.TenantId, evt.Identity, udsBuildModel);
                await _webAPIClient.SendCommandAsync(commandUpdateUDSData);
                _logger.WriteInfo(new LogMessage($"Updating metadata invoice {commandUpdateUDSData.Id} has been sended"), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("WorkflowReceivableInvoicePECMailBuildCompleteCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        #endregion

        #region [ WorkflowCreatePECMailCallback ]

        private async Task WorkflowCreatePECMailCallback(IEventCreatePECMail evt)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"WorkflowCreatePECMailCallback -> evaluate event id {evt.Id}"), LogCategories);
                _logger.WriteInfo(new LogMessage($"Notifying CreatePECMail"), LogCategories);

                PECMail pecMail = evt.ContentType.ContentTypeValue;
                if (!pecMail.Year.HasValue || !pecMail.Number.HasValue)
                {
                    throw new ArgumentNullException($"Undefined protocol year {pecMail.Year} or number {pecMail.Number} in PECMail {pecMail.EntityId}");
                }
                IEnumerable<PECMailAttachment> attachments = pecMail.Attachments
                    .Where(f => f.IDDocument.HasValue && f.IDDocument.Value != Guid.Empty && new FileInfo(f.AttachmentName).Extension.Equals(".xml", StringComparison.InvariantCultureIgnoreCase));
                if (!attachments.Any())
                {
                    throw new ArgumentNullException($"Undefined valid SDI attachment to evaluate in PECMail {pecMail.EntityId}");
                }

                Protocol protocol = (await _webAPIClient.GetProtocolAsync($"$filter=Year eq {pecMail.Year.Value} and Number eq {pecMail.Number.Value}&$expand=AdvancedProtocol")).SingleOrDefault();
                if (protocol == null)
                {
                    throw new ArgumentNullException($"Protocol not found for identification {pecMail.Year}/{pecMail.Number}");
                }
                UDSDocumentUnit udsDocumentUnit = (await _webAPIClient.GetUDSDocumentUnitFromDocumentUnitId(protocol.UniqueId)).SingleOrDefault();

                if (udsDocumentUnit == null || !_payableWorkflowConfigurations.Any(f => f.Value.UDSRepositoryName == udsDocumentUnit.Repository.Name))
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

                using (ERPDbContext dbContext = new ERPDbContext(_logger, ModuleConfigurationHelper.JsonSerializerSettings, _moduleConfiguration.ConnectionString))
                {
                    string number = protocol.Number.ToString("0000000");
                    PayableInvoice payableInvoice = dbContext.PayableInvoices.SingleOrDefault(f => f.DocSuiteProtocolYear == protocol.Year && f.DocSuiteProtocolNumber == number);
                    if (payableInvoice == null)
                    {
                        _logger.WriteWarning(new LogMessage($"ERP payable invoice not found with protocol reference {protocol.Year}/{number}"), LogCategories);
                    }
                    else
                    {
                        _logger.WriteInfo(new LogMessage($"Updating invoice SDI metadata [{pecMail.MailDate}/{sdiMessage.MessageType}/{sdiMessage.LogDescription}] to ERP request {payableInvoice.RequestId}"), LogCategories);
                        payableInvoice.SDIDate = sdiMessage.SDIDate.HasValue ? sdiMessage.SDIDate.Value : pecMail.MailDate.Value;
                        payableInvoice.SDIResult = sdiMessage.GetDescriptionMessageStatus();
                        payableInvoice.SDIResultDescription = sdiMessage.LogDescription;
                        dbContext.SaveChanges();
                    }
                }

                ICollection<UDSRole> udsRoles = await _webAPIClient.GetUDSRoles(udsDocumentUnit.IdUDS);
                ICollection<UDSContact> udsContacts = await _webAPIClient.GetUDSContacts(udsDocumentUnit.IdUDS);
                ICollection<UDSMessage> udsMessages = await _webAPIClient.GetUDSMessages(udsDocumentUnit.IdUDS);
                ICollection<UDSPECMail> udsPECMails = await _webAPIClient.GetUDSPECMails(udsDocumentUnit.IdUDS);
                ICollection<UDSDocumentUnit> udsDocumentUnits = await _webAPIClient.GetUDSDocumentUnits(udsDocumentUnit.IdUDS, false, false);
                _logger.WriteInfo(new LogMessage($"Updating PEC PA metadatas for UDSId {udsDocumentUnit.IdUDS}"), LogCategories);

                UDSBuildModel udsBuildModel = UDSEInvoiceHelper.PrepareUpdateUDSBuildModel(udsDocumentUnit.Repository, udsDocumentUnit.IdUDS, uds_metadatas, documents,
                    udsRoles, udsContacts, udsMessages, udsPECMails, udsDocumentUnits, null, evt.Identity.User);
                CommandUpdateUDSData commandUpdateUDSData = new CommandUpdateUDSData(evt.Name, evt.TenantId, evt.Identity, udsBuildModel);
                await _webAPIClient.SendCommandAsync(commandUpdateUDSData);
                _logger.WriteInfo(new LogMessage($"Updating metadata invoice {commandUpdateUDSData.Id} has been sended"), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("WorkflowCreatePECMailCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        #endregion

        #endregion

    }
}
