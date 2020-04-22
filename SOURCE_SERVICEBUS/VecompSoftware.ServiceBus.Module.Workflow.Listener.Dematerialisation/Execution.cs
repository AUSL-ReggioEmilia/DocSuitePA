using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
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

namespace VecompSoftware.ServiceBus.Module.Workflow.Listener.Dematerialisation
{
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public class Execution : IListenerExecution<ICommandDematerialisationRequest>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly BiblosDS.BiblosClient _biblosClient;
        private readonly IWebAPIClient _webApiClient;
        private static DematerialisationConfig _dematerialisationConfig;
        protected static IEnumerable<LogCategory> _logCategories = null;
        protected static DocSuite.Services.PDFGenerator.PDFGeneratorClient _pdfGeneratorClient = null;
        private readonly string _path_JsonConfigModule = Path.Combine(Environment.CurrentDirectory, "Module.WF.Dematerialisation.Config.json");
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

        protected static DocSuite.Services.PDFGenerator.PDFGeneratorClient PDFClient
        {
            get
            {
                if (_pdfGeneratorClient == null)
                {
                    _pdfGeneratorClient = new DocSuite.Services.PDFGenerator.PDFGeneratorClient();
                }
                return _pdfGeneratorClient;
            }
        }
        private DomainUserModel SignerModel { get; set; }
        private string UDDisplayName { get; set; }
        #endregion

        #region [ Constructor ]
        public Execution(ILogger logger, IWebAPIClient webApiClient, BiblosDS.BiblosClient biblosClient)
        {
            _logger = logger;
            _biblosClient = biblosClient;
            _webApiClient = webApiClient;

            if (!File.Exists(_path_JsonConfigModule))
            {
                _logger.WriteWarning(new LogMessage(string.Concat("The file Module.WF.Dematerialisation.Config.json doesn't exist in the directory ", Environment.CurrentDirectory)), LogCategories);
                throw new Exception(string.Concat("The file Module.WF.Dematerialisation.Config.json doesn't exist in the directory ", Environment.CurrentDirectory));
            }

            _dematerialisationConfig = JsonConvert.DeserializeObject<DematerialisationConfig>(File.ReadAllText(_path_JsonConfigModule));

            if (_dematerialisationConfig.IdTemplateDematerialisation == Guid.Empty)
            {
                _logger.WriteWarning(new LogMessage("Parameter IdTemplateDematerialisation not configured"), LogCategories);
                throw new Exception("Parameter IdTemplateDematerialisation not configured");
            }

            if (string.IsNullOrEmpty(_dematerialisationConfig.DematerialisationWorkflowName) || string.IsNullOrWhiteSpace(_dematerialisationConfig.DematerialisationWorkflowName))
            {
                _logger.WriteWarning(new LogMessage("Parameter DematerialisationWorkflowName not configured"), LogCategories);
                throw new Exception("Parameter DematerialisationWorkflowName not configured");
            }

        }
        #endregion

        #region [ Methods ]
        public async Task ExecuteAsync(ICommandDematerialisationRequest command)
        {
            _logger.WriteInfo(new LogMessage(string.Concat(command, " is arrived")), LogCategories);
            try
            {
                _logger.WriteInfo(new LogMessage(string.Concat(" Evaluating: ", command.ContentType.UniqueId)), LogCategories);

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
                //generate PDF
                byte[] documentDematerialisation = await GeneratePDF(userAccount);
                //startworkflow
                WorkflowResult result = await StartWorkflow(command.ContentType.ContentTypeValue, documentDematerialisation, command.TenantId, command.TenantName);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }

        }

        public async Task<WorkflowResult> StartWorkflow(DocumentManagementRequestModel request, byte[] docDematerialization, Guid tenantId, string tenantName)
        {
            try
            {
                CollaborationModel collaborationModel = await CreateCollaborationModel(request, docDematerialization);

                _logger.WriteDebug(new LogMessage("collaboration Model created"), LogCategories);

                WorkflowStart workflow = new WorkflowStart
                {
                    WorkflowName = _dematerialisationConfig.DematerialisationWorkflowName
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
                    ValueString = _dematerialisationConfig.DematerialisationWorkflowName
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
                    ValueString = JsonConvert.SerializeObject(new WorkflowActivityOperation() { Action = WorkflowActivityAction.ToAssignment, Area = WorkflowActivityArea.Collaboration }),
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
        public async Task<byte[]> GeneratePDF(WorkflowAccount signerAccount)
        {
            string xml = string.Empty;
            _logger.WriteInfo(new LogMessage(string.Concat("Compiling form PDF")), LogCategories);

            FormPDF compiledForm = new FormPDF
            {
                DSW_Date = DateTime.Now.ToString("dd/MM/yyyy"),
                DSW_Signer = string.IsNullOrEmpty(signerAccount.DisplayName) ? SignerModel.DisplayName : signerAccount.DisplayName,
                DSW_Title = UDDisplayName,
                DSW_Abstract = string.Empty,
                DSW_Sign = string.Empty
            };

            XmlSerializer xmlS = new XmlSerializer(compiledForm.GetType());
            using (StringWriter strw = new StringWriter())
            {
                xmlS.Serialize(strw, compiledForm);
                xml = strw.ToString();
            }
            _logger.WriteInfo(new LogMessage(string.Concat("Form PDF Compiled")), LogCategories);

            _logger.WriteInfo(new LogMessage(string.Concat("Request GenerateDocumentPDFA to PDFGeneratorService")), LogCategories);
            byte[] serviceResponse = await PDFClient.GenerateDocumentPDFAAsync(xml, _dematerialisationConfig.IdTemplateDematerialisation);

            _logger.WriteInfo(new LogMessage(string.Concat("PDFGeneratorService has generated the compiled PDFA")), LogCategories);
            return serviceResponse;
        }

        private async Task<CollaborationModel> CreateCollaborationModel(DocumentManagementRequestModel request, byte[] docDematerialization)
        {
            try
            {
                CollaborationModel collaboration = new CollaborationModel
                {
                    IdStatus = "N",//nasconde la collaborazione
                    Subject = string.Concat("Richiesta attestazione ", UDDisplayName)
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
                    DocumentContent = docDematerialization,
                    DocumentName = "Attestazione.pdf",
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
