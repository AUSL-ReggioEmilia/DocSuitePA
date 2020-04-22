using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using VecompSoftware.BPM.Integrations.Modules.FALCK.Data.Entities;
using VecompSoftware.BPM.Integrations.Modules.FALCK.Workflows.Configurations;
using VecompSoftware.BPM.Integrations.Modules.FALCK.Workflows.Models;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Commands.Models.UDS;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.Helpers.UDS;

namespace VecompSoftware.BPM.Integrations.Modules.FALCK.Workflows
{
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class UDSService
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IWebAPIClient _webApiClient;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly ICollection<UDSMappingModel> _udsMappingConfigurations;
        private readonly string _userName;
        private const string UDS_MAPPING_CONFIGURATION_NAME = "uds.mapping.configuration.json";
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(UDSService));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public UDSService(ILogger logger, IWebAPIClient webApiClient)
        {
            _logger = logger;
            _webApiClient = webApiClient;
            _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
            //todo: rimuovere e consolidare in un unico configuratore
            _udsMappingConfigurations = JsonConvert.DeserializeObject<IList<UDSMappingModel>>(File.ReadAllText(Path.Combine(ModuleConfigurationHelper.CurrentModulePath, UDS_MAPPING_CONFIGURATION_NAME)));
            if (WindowsIdentity.GetCurrent() != null)
            {
                _userName = WindowsIdentity.GetCurrent().Name;
            }
        }
        #endregion

        #region [ Methods ]  
        public UDSRepository GetRepository(string name)
        {
            _logger.WriteDebug(new LogMessage(string.Concat("GetRepository -> ", string.Format("$filter=Name eq '{0}'", name))), LogCategories);
            return _webApiClient.GetAsync<UDSRepository>(string.Format("$filter=Name eq '{0}'", name)).Result.FirstOrDefault();
        }

        public Contact CreateContact(Contact contact)
        {
            _logger.WriteDebug(new LogMessage(string.Concat("CreateContact -> ", contact.Description)), LogCategories);
            return _webApiClient.PostAsync(contact).Result;
        }

        public Contact UpdateContact(Contact contact)
        {
            _logger.WriteDebug(new LogMessage(string.Concat("UpdateContact -> ", contact.Description)), LogCategories);
            return _webApiClient.PutAsync(contact).Result;
        }

        public Contact GetContact(string taxCode)
        {
            _logger.WriteDebug(new LogMessage(string.Concat("GetContact -> ", string.Format("$filter=FiscalCode eq '{0}' and IncrementalFather eq {1}", taxCode, _moduleConfiguration.Contact_RootId))), LogCategories);
            return _webApiClient.GetContactAsync(string.Format("$filter=FiscalCode eq '{0}' and IncrementalFather eq {1}", taxCode, _moduleConfiguration.Contact_RootId)).Result.FirstOrDefault();
        }

        public int GetLastContactId(string taxCode)
        {
            _logger.WriteDebug(new LogMessage("GetLastContactId -> $orderby=EntityId desc&$top=1"), LogCategories);
            return _webApiClient.GetContactAsync("$orderby=EntityId desc&$top=1").Result.Single().EntityId;
        }

        public UDSRepository GetRepository(Guid repositoryId)
        {
            return _webApiClient.GetAsync<UDSRepository>(string.Format("$filter=UniqueId eq {0}", repositoryId)).Result.FirstOrDefault();
        }

        public UDSBaseEntity GetUDS(UDSRepository repository, Guid uniqueId)
        {
            _webApiClient.SetUDSEndpoints(typeof(UDSODATAModel).Name, repository.Name);
            string controllerName = Utils.GetWebAPIControllerName(repository.Name);
            _logger.WriteDebug(new LogMessage(string.Concat("GetUDS -> ", controllerName, " - ", string.Format("$filter=UDSId eq {0}&$top=1", uniqueId))), LogCategories);
            UDSODATAModel uds = _webApiClient.GetAsync<UDSODATAModel, UDSODATAModel>(controllerName, string.Format("$filter=UDSId eq {0}&$top=1", uniqueId)).Result;
            return uds.Items.FirstOrDefault();
        }

        public UDSBaseEntity GetUDSByNumber(UDSRepository repository, string docNumber)
        {
            _webApiClient.SetUDSEndpoints(typeof(UDSODATAModel).Name, repository.Name);
            string controllerName = Utils.GetWebAPIControllerName(repository.Name);
            _logger.WriteDebug(new LogMessage(string.Concat("GetUDSByNumber -> ", controllerName, " - ", string.Format("$filter=DocumentNumber eq '{0}'&$top=1", docNumber))), LogCategories);
            UDSODATAModel uds = _webApiClient.GetAsync<UDSODATAModel, UDSODATAModel>(controllerName, string.Format("$filter=DocumentNumber eq '{0}'&$top=1", docNumber)).Result;
            return uds.Items.FirstOrDefault();
        }

        public Guid SendCommandInsertData(Guid idRepository, UDSModel model, UDSRepository udsRepository)
        {
            IdentityContext identity = new IdentityContext(_userName);
            string tenantName = _moduleConfiguration.TenantName;
            Guid tenantId = _moduleConfiguration.TenantId;

            UDSBuildModel commandModel = CreateCommandModel(model, udsRepository.UniqueId, udsRepository.UniqueId);
            CommandInsertUDSData commandInsert = new CommandInsertUDSData(tenantName, tenantId, identity, commandModel);
            Guid commandId = commandInsert.Id;
            commandInsert = _webApiClient.PostAsync(commandInsert).Result;
            return commandId;
        }

        public UDSModel FillUDSModel(WorkflowMetadata entity, UDSRepository repository, Contact contact)
        {
            UDSModel model = UDSModel.LoadXml(repository.ModuleXML);
            model.Model.Subject.Value = entity.DocumentDescription;
            IDictionary<string, object> uds_metadatas = MappingMetadata(entity, repository);
            model.FillMetaData(uds_metadatas);
            if (model.Model.Documents == null)
            {
                model.Model.Documents = new Documents();
            }
            if (model.Model.Documents.Document == null)
            {
                model.Model.Documents.Document = new Document();
            }
            model.Model.Documents.Document.Instances = FillDocumentInstances(entity.WorkflowAttachments, true);
            if (entity.WorkflowAttachments.Any(f => !Convert.ToBoolean(f.IsMainDocument)))
            {
                if (model.Model.Documents == null)
                {
                    model.Model.Documents = new Documents();
                }
                if (model.Model.Documents.DocumentAttachment == null)
                {
                    model.Model.Documents.DocumentAttachment = new Document();
                }
                model.Model.Documents.DocumentAttachment.Instances = FillDocumentInstances(entity.WorkflowAttachments, false);
            }

            Contacts contacts = model.Model.Contacts.Single();
            if (contacts.ContactInstances == null)
            {
                contacts.ContactInstances = new ContactInstance[]
                {
                    new ContactInstance() { IdContact = contact.EntityId }
                };
            }
            return model;
        }

        private UDSBuildModel CreateCommandModel(UDSModel model, Guid udsRepositoryId, Guid udsId)
        {
            UDSBuildModel commandModel = new UDSBuildModel(model.SerializeToXml())
            {
                UDSRepository = new UDSRepositoryModel(udsRepositoryId),
                UniqueId = udsId,
                RegistrationUser = _userName
            };

            return commandModel;
        }

        private DocumentInstance[] FillDocumentInstances(ICollection<WorkflowAttachment> attachments, bool isMainDocument)
        {
            if (attachments.Count == 0)
            {
                return new DocumentInstance[] { };
            }

            IList<DocumentInstance> instances = new List<DocumentInstance>();
            foreach (WorkflowAttachment attachment in attachments.Where(f => Convert.ToBoolean(f.IsMainDocument) == isMainDocument))
            {
                string filePath = Path.Combine(_moduleConfiguration.AttachmentsPath, attachment.InternalFileName);
                instances.Add(new DocumentInstance()
                {
                    DocumentContent = Convert.ToBase64String(File.ReadAllBytes(filePath)),
                    DocumentName = Path.GetFileName(attachment.OriginalFileName)
                });
            }
            return instances.ToArray();
        }

        private IDictionary<string, object> MappingMetadata(WorkflowMetadata workflowMetadata, UDSRepository repository)
        {
            IDictionary<string, object> results = new Dictionary<string, object>
            {
                { "CompanyCode", workflowMetadata.CompanyCode },
                { "CompanyName", workflowMetadata.CompanyName },
                { "DocumentNumber", workflowMetadata.DocumentNumber },
                { "DocumentDescription", workflowMetadata.DocumentDescription },
                { "CreationDate", workflowMetadata.CreationDateTime },
                { "JobCode", workflowMetadata.JobCode },
                { "JobDescription", workflowMetadata.JobDescription },
                { "JobType", workflowMetadata.JobType },
                { "AmountLcy", Convert.ToDouble(workflowMetadata.AmountLCY) },
                { "CurrencyCode", workflowMetadata.CurrencyCode },
                { "Note1", workflowMetadata.Note1 },
                { "Note2", workflowMetadata.Note2 }
            };
            return results;
        }

        private object GetPropertyValue(WorkflowMetadata entity, string propertyName)
        {
            return entity.GetType().GetProperties()
               .Single(pi => pi.Name == propertyName)
               .GetValue(entity, null);
        }
        #endregion
    }
}
