using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Integrations.GenericProcesses;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.Helpers.Workflow;
using VecompSoftware.ServiceBus.Receiver.Base;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS.Commands.Models.Integrations.GenericProcesses;

namespace VecompSoftware.ServiceBus.Module.Workflow.Listener.SecureDocument
{
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public class Execution : IListenerExecution<ICommandSecureDocumentRequest>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly BiblosDS.BiblosClient _biblosClient;
        private readonly StampaConforme.StampaConformeClient _stampaConformeClient;
        private readonly IWebAPIClient _webApiClient;
        private static SecureDocumentConfig _secureDocumentConfig;
        protected static IEnumerable<LogCategory> _logCategories = null;
        private const string _biblos_signature_attribute = "Signature";
        private readonly string _path_JsonConfigModule = Path.Combine(Environment.CurrentDirectory, "Module.WF.SecureDocument.Config.json");
        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
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
        public IDictionary<string, object> Properties { get; set; }

        public EvaluationModel RetryPolicyEvaluation { get; set; }

        private DomainUserModel SignerModel { get; set; }
        private string UDDisplayName { get; set; }
        #endregion

        #region [ Constructor ]
        public Execution(ILogger logger, IWebAPIClient webApiClient, BiblosDS.BiblosClient biblosClient, StampaConforme.StampaConformeClient stampaConformeClient)
        {
            _logger = logger;
            _biblosClient = biblosClient;
            _webApiClient = webApiClient;
            _stampaConformeClient = stampaConformeClient;

            if (!File.Exists(_path_JsonConfigModule))
            {
                _logger.WriteWarning(new LogMessage(string.Concat("The file Module.WF.SecureDocument.Config.json doesn't exist in the directory ", Environment.CurrentDirectory)), LogCategories);
                throw new Exception(string.Concat("The file Module.WF.SecureDocument.Config.json doesn't exist in the directory ", Environment.CurrentDirectory));
            }

            _secureDocumentConfig = JsonConvert.DeserializeObject<SecureDocumentConfig>(File.ReadAllText(_path_JsonConfigModule));

            if (string.IsNullOrEmpty(_secureDocumentConfig.SecureDocumentWorkflowName) || string.IsNullOrWhiteSpace(_secureDocumentConfig.SecureDocumentWorkflowName))
            {
                _logger.WriteWarning(new LogMessage("Parameter SecureDocumentWorkflowName not configured"), LogCategories);
                throw new Exception("Parameter SecureDocumentWorkflowName not configured");
            }

        }
        #endregion

        #region [ Methods ]
        public async Task ExecuteAsync(ICommandSecureDocumentRequest command)
        {
            _logger.WriteInfo(new LogMessage(string.Concat(command, " is arrived")), LogCategories);
            try
            {
                _logger.WriteInfo(new LogMessage(string.Concat(" Evaluating: ", command.ContentType.ContentTypeValue.UniqueId)), LogCategories);

                if (command.ContentType.ContentTypeValue.DocumentUnit == null)
                {
                    _logger.WriteWarning(new LogMessage("Document Unit is not configured"), LogCategories);
                    throw new Exception("Document Unit is not configured");
                }

                if (command.ContentType.ContentTypeValue.DocumentUnit.ReferenceModel == null)
                {
                    _logger.WriteWarning(new LogMessage("Reference Model is not configured"), LogCategories);
                    throw new Exception("Reference Model is not configured");
                }

                _logger.WriteDebug(new LogMessage(string.Concat(" Document Unit uniqueid: ", command.ContentType.ContentTypeValue.DocumentUnit.ReferenceId)), LogCategories);

                WorkflowMapping signer = command.ContentType.ContentTypeValue.Signers?.FirstOrDefault();
                if (signer == null || signer.Account == null || string.IsNullOrEmpty(signer.Account.AccountName))
                {
                    _logger.WriteWarning(new LogMessage("Signers are not configured"), LogCategories);
                    throw new Exception("Signers are not configured");
                }

                WorkflowAccount userAccount = signer.Account;
                _logger.WriteDebug(new LogMessage(string.Concat(" Signer's name is ", userAccount.AccountName)), LogCategories);

                if (string.IsNullOrEmpty(userAccount.DisplayName) || string.IsNullOrEmpty(userAccount.EmailAddress))
                {
                    string[] signerNames = userAccount.AccountName.Split('\\');
                    if (signerNames.Length < 2)
                    {
                        _logger.WriteWarning(new LogMessage(string.Concat("Signer's Account Name is not correctly configured: ", userAccount.AccountName)), LogCategories);
                        throw new Exception("Signer's Account Name is not correctly configured");
                    }

                    SignerModel = await _webApiClient.GetSignerInformationAsync(signerNames.GetValue(1).ToString(), signerNames.GetValue(0).ToString());
                    if (SignerModel == null)
                    {
                        _logger.WriteWarning(new LogMessage(string.Format("Signer {0} not found", userAccount.AccountName)), LogCategories);
                        throw new Exception(string.Format("Signer {0} not found", userAccount.AccountName));
                    }
                }

                if (command.ContentType.ContentTypeValue.Roles == null || !command.ContentType.ContentTypeValue.Roles.Any())
                {
                    _logger.WriteWarning(new LogMessage("Roles are not configured"), LogCategories);
                    throw new Exception("Roles are not configured");
                }

                DocumentUnit documentUnit = null;
                try
                {
                    documentUnit = JsonConvert.DeserializeObject<DocumentUnit>(command.ContentType.ContentTypeValue.DocumentUnit.ReferenceModel);
                }
                catch (Exception)
                {
                    _logger.WriteWarning(new LogMessage("ReferenceModel is not correctly deserialized"), LogCategories);
                    throw new Exception("ReferenceModel is not correctly deserialized");
                }

                string udDisplayName = string.Concat(documentUnit.DocumentUnitName, " ", documentUnit.Title, " con Oggetto: ", documentUnit.Subject);
                UDDisplayName = udDisplayName;
                bool appendSignature = await _webApiClient.GetSecureDocumentSignatureEnabledAsync();
                foreach (WorkflowReferenceBiblosModel document in command.ContentType.ContentTypeValue.Documents)
                {
                    _logger.WriteDebug(new LogMessage(string.Concat("Creating secure document for id ", document.ArchiveDocumentId, "...")), LogCategories);
                    byte[] content = _biblosClient.Document.GetDocumentContentById(document.ArchiveDocumentId.Value).Blob;
                    StampaConforme.StampaConforme.stDoc toSecure = new StampaConforme.StampaConforme.stDoc()
                    {
                        Blob = Convert.ToBase64String(content),
                        FileExtension = Path.GetExtension(document.DocumentName)
                    };

                    string signatureValue = string.Empty;
                    if (appendSignature)
                    {
                        _logger.WriteDebug(new LogMessage("Append signature to document"), LogCategories);
                        BiblosDS.BiblosDS.Document infoDocument = _biblosClient.Document.GetDocumentInfoById(document.ArchiveDocumentId.Value);
                        BiblosDS.BiblosDS.AttributeValue signature = infoDocument.AttributeValues.SingleOrDefault(f => f.Attribute.Name.Equals(_biblos_signature_attribute, StringComparison.InvariantCultureIgnoreCase));
                        if (signature != null && signature.Value != null)
                        {
                            signatureValue = StampaConforme.StampaConformeClient.GetLabel(signature.Value.ToString());
                        }
                    }

                    StampaConforme.StampaConforme.stDoc response = _stampaConformeClient.StampaConforme.CreateSecureDocument(toSecure, document.DocumentName, signatureValue);
                    _logger.WriteDebug(new LogMessage(string.Concat("Secure document for id ", document.ArchiveDocumentId, " created correctly")), LogCategories);
                    byte[] secureDocumentContent = Convert.FromBase64String(response.Blob);
                    DocumentManagementRequestModel toStart = command.ContentType.ContentTypeValue;
                    toStart.Documents = command.ContentType.ContentTypeValue.Documents.Where(x => x.ArchiveDocumentId == document.ArchiveDocumentId).ToList();
                    WorkflowResult result = await StartWorkflow(toStart, secureDocumentContent, string.Concat(document.DocumentName, ".pdf"), response.ReferenceId, command.TenantId, command.TenantName);
                    if (!result.IsValid)
                    {
                        _logger.WriteError(new LogMessage(string.Concat("Error on start workflow for document ", document.ArchiveDocumentId)), LogCategories);
                        foreach (TranslationError error in result.Errors)
                        {
                            _logger.WriteError(new LogMessage(string.Concat("ActivityId: ", error.ActivityId, " - Message: ", error.Message, " - StartLine: ", error.StartLine, " - ExpressionText: ", error.ExpressionText)), LogCategories);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }

        }

        public async Task<WorkflowResult> StartWorkflow(DocumentManagementRequestModel request, byte[] secureDocument, string secureDocumentName, string documentReferenceId, Guid tenantId, string tenantName)
        {
            try
            {
                CollaborationModel collaborationModel = await CreateCollaborationModel(request, secureDocument, secureDocumentName);

                _logger.WriteDebug(new LogMessage("collaboration Model created"), LogCategories);

                WorkflowStart workflow = new WorkflowStart
                {
                    WorkflowName = _secureDocumentConfig.SecureDocumentWorkflowName
                };
                workflow.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_MODEL, new WorkflowArgument()
                {
                    Name = WorkflowPropertyHelper.DSW_PROPERTY_MODEL,
                    PropertyType = ArgumentType.Json,
                    ValueString = JsonConvert.SerializeObject(collaborationModel)
                });

                workflow.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_LABEL, new WorkflowArgument()
                {
                    Name = WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_LABEL,
                    PropertyType = ArgumentType.PropertyString,
                    ValueString = _secureDocumentConfig.SecureDocumentWorkflowName
                });

                workflow.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, new WorkflowArgument()
                {
                    PropertyType = ArgumentType.RelationGuid,
                    Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID,
                    ValueGuid = tenantId
                });

                workflow.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, new WorkflowArgument()
                {
                    PropertyType = ArgumentType.PropertyString,
                    Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME,
                    ValueString = tenantName
                });

                workflow.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, new WorkflowArgument()
                {
                    Name = WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL,
                    PropertyType = ArgumentType.Json,
                    ValueString = JsonConvert.SerializeObject(request)
                });
                workflow.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_MODEL, new WorkflowArgument()
                {
                    Name = WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_MODEL,
                    PropertyType = ArgumentType.Json,
                    ValueString = "[]"
                });
                workflow.Arguments.Add(WorkflowPropertyHelper.DSW_FIELD_COLLABORATION_MANAGED, new WorkflowArgument()
                {
                    Name = WorkflowPropertyHelper.DSW_FIELD_COLLABORATION_MANAGED,
                    PropertyType = ArgumentType.Json,
                    ValueString = JsonConvert.SerializeObject(new WorkflowActivityOperation() { Action = WorkflowActivityAction.ToSecure, Area = WorkflowActivityArea.Collaboration }),
                });
                workflow.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER, new WorkflowArgument()
                {
                    Name = WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER,
                    PropertyType = ArgumentType.PropertyString,
                    ValueString = documentReferenceId
                });

                _logger.WriteInfo(new LogMessage("Send message Start Workflow"), LogCategories);
                WorkflowResult workflowResult = await _webApiClient.StartWorkflowAsync(workflow);

                return workflowResult;
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        private async Task<CollaborationModel> CreateCollaborationModel(DocumentManagementRequestModel request, byte[] secureDocument, string secureDocumentName)
        {
            try
            {
                CollaborationModel collaboration = new CollaborationModel
                {
                    IdStatus = "N",
                    Subject = string.Concat("Richiesta securizzazione documento ", secureDocumentName, " per ", UDDisplayName)
                };
                short signerNumber = 0;
                DomainUserModel sign = null;

                foreach (WorkflowMapping signer in request.Signers)
                {
                    signerNumber += 1;
                    sign = null;
                    if (signer.Account == null)
                    {
                        _logger.WriteWarning(new LogMessage(string.Concat("Signer Account is not correctly configured")), LogCategories);
                        throw new Exception("Signer Account is not correctly configured");
                    }

                    if (string.IsNullOrEmpty(signer.Account.EmailAddress) || string.IsNullOrEmpty(signer.Account.DisplayName))
                    {
                        string[] signerDomainName = signer.Account.AccountName.Split('\\');
                        sign = await _webApiClient.GetSignerInformationAsync(signerDomainName.GetValue(1).ToString(), signerDomainName.GetValue(0).ToString());
                        if (sign == null)
                        {
                            _logger.WriteWarning(new LogMessage(string.Format("Signer {0} not found", signer.Account.AccountName)), LogCategories);
                            throw new Exception("Signer not found");
                        }
                    }

                    collaboration.CollaborationSigns.Add(new CollaborationSignModel()
                    {
                        Incremental = signerNumber,
                        IsActive = true,
                        IsRequired = signer.Account.Required,
                        SignUser = string.IsNullOrEmpty(signer.Account.AccountName) ? sign.Account : signer.Account.AccountName,
                        SignName = string.IsNullOrEmpty(signer.Account.DisplayName) ? sign.DisplayName : signer.Account.DisplayName,
                        SignEmail = string.IsNullOrEmpty(signer.Account.EmailAddress) ? sign.EmailAddress : signer.Account.EmailAddress
                    });
                }

                collaboration.SignCount = 1;

                signerNumber = 0;
                foreach (WorkflowRole role in request.Roles)
                {
                    signerNumber += 1;
                    collaboration.CollaborationUsers.Add(new CollaborationUserModel()
                    {
                        Incremental = signerNumber,
                        DestinationType = CollaborationDestinationType.S.ToString(),
                        DestinationFirst = role.Required,
                        Account = null,
                        IdRole = role.IdRole,
                        DestinationName = role.Name,
                        DestinationEmail = role.EmailAddress
                    });
                }

                collaboration.CollaborationVersionings.Add(new CollaborationVersioningModel()
                {
                    DocumentContent = secureDocument,
                    DocumentName = secureDocumentName,
                    DocumentGroup = "MainDocument",
                    IsActive = true
                });

                return collaboration;

            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }
        #endregion

    }
}
