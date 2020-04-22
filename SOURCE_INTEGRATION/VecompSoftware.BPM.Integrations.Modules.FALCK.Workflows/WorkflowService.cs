using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using VecompSoftware.BPM.Integrations.Modules.FALCK.Workflows.Models;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.Helpers.Workflow;

namespace VecompSoftware.BPM.Integrations.Modules.FALCK.Workflows
{
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class WorkflowService
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IWebAPIClient _webApiClient;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly JsonSerializerSettings _serializerSettings;
        #endregion

        #region [ Start Parameters ]
        private const string COMPANY_CODE = "CompanyCode";
        private const string COMPANY_NAME = "CompanyName";
        private const string DOCUMENT_TYPE = "DocumentType";
        private const string DOCUMENT_NUMBER = "DocumentNumber";
        private const string DOCUMENT_DESCRIPTION = "DocumentDescription";
        private const string REFERENCE_DOC_TYPE = "ReferenceDocType";
        private const string REFERENCE_DOC_NUMBER = "ReferenceDocNumber";
        private const string SOURCE_TYPE = "SourceType";
        private const string SOURCE_NUMBER = "SourceNumber";
        private const string SOURCE_NAME = "SourceName";
        private const string SOURCE_VATE_REG_NUMBER = "SourceVatRegNumber";
        private const string SOURCE_FISCAL_CODE = "SourceFiscalCode";
        private const string SOURCE_EMAIL = "SourceEmail";
        private const string SOURCE_CERTIFIED_EMAIL = "SourceCertifiedEmail";
        private const string SOURCE_LEGAL_FORM = "SourceLegalForm";
        private const string CREATION_USER = "CreationUser";
        private const string CREATION_EMAIL = "CreationEmail";
        private const string CREATION_DISPLAY_NAME = "CreationDisplayName";
        private const string CREATION_DATE = "CreationDate";
        private const string IS_EXTERNAL_PARTNER = "IsExternalPartner";
        private const string JOB_CODE = "JobCode";
        private const string JOB_DESCRIPTION = "JobDescription";
        private const string JOB_TYPE = "JobType";
        private const string JOB_AMOUNT_LCY = "JobDelegateAmountLCY";
        private const string IS_ITEM_SPECIAL_CATEGORY = "IsItemSpecialCategory";
        private const string SPECIAL_CATEGORY_CODE = "SpecialCategoryCode";
        private const string SPECIAL_CATEGORY_DESCRIPTION = "SpecialCategoryDescription";
        private const string IS_ITEM_EMERGENCY = "IsItemEmergency";
        private const string IS_ITEM_STANDARD_WORKFLOW = "IsItemStandardWorkflow";
        private const string AMOUNT_LCY = "AmountLCY";
        private const string AMOUNT_LCY_DISPLAY = "DisplayAmountLCY";
        private const string CURRENCY_CODE = "CurrencyCode";
        private const string IS_INTERNAL_PO = "IsInternalPO";
        private const string POR_RESPONSIBLE_1 = "POResponsibleLevel1";
        private const string POR_RESPONSIBLE_2 = "POResponsibleLevel2";
        private const string POR_RESPONSIBLE_3 = "POResponsibleLevel3";
        private const string POR_RESPONSIBLE_SPECIAL_CATEGORY = "POResponsibleSpecialCategory";
        private const string IS_LEGAL_CHECKED = "IsLegalChecked";
        private const string NOTE1 = "Note1";
        private const string NOTE2 = "Note2";
        private const string MAPPING_TAG = "MappingTag";
        private const string ROLE_SPECIAL_CATEGORY = "RoleSpecialCategory";
        private const string EMAIL_SPECIAL_CATEGORY = "EmailsSpecialCategory";
        private const string WORKFLOW_LABEL = "WorkflowLabel";
        private const string LIMIT_PCLCY = "LimitPCLCY";
        private const string LIMIT_LCY = "LimitLCY";
        private const string LIMIT_EMPOWERED_RESPONSIBLE_LCY = "LimitEmpoweredResponsibleLCY";
        private const string LIMIT_PROCUREMENT_RESPONSIBLE_LCY = "LimitProcurementResponsibleLCY";
        private const string ROLE_COMPANY_CEO = "RoleCompanyCEO";
        private const string EMAIL_COMPANY_CEO = "EmailsCompanyCEO";
        private const string ROLE_COMPANY_PROCUREMENT = "RoleCompanyProcurement";
        private const string EMAIL_COMPANY_PROCUREMENT = "EmailsCompanyProcurement";
        #endregion

        #region [ Workflow Properties ]
        public const string START_WORKFLOW_NAME = "_wfm_e_StartWorkflowName";
        public const string START_WORKFLOW_STATUS = "_wfm_e_StartStatus";
        public const string WORKFLOW_AUTOAPPROVAL = "IsWorkflowAutoApproval";
        public const string WORKFLOW_APPROVED = "IsWorkflowApproved";
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(WorkflowService));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public WorkflowService(ILogger logger, IWebAPIClient webApiClient)
        {
            _logger = logger;
            _webApiClient = webApiClient;
            _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
            _serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            };
        }
        #endregion

        #region [ Methods ]
        public Guid StartWorkflow(TransporterModel transporterModel, string workflowName)
        {
            WorkflowStart start = new WorkflowStart
            {
                WorkflowName = workflowName
            };
            start.Arguments.Add(WorkflowPropertyHelper.DSW_FIELD_UDS_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_FIELD_UDS_ID,
                ValueGuid = transporterModel.UDS.UDSId
            });

            start.Arguments.Add(WorkflowPropertyHelper.DSW_FIELD_UDS_REPOSITORY_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_FIELD_UDS_REPOSITORY_ID,
                ValueGuid = transporterModel.UDS.IdUDSRepository
            });

            start.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_MODEL, new WorkflowArgument()
            {
                PropertyType = ArgumentType.Json,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_MODEL,
                ValueString = JsonConvert.SerializeObject(transporterModel.CollaborationModel, _serializerSettings)
            });

            start.StartParameters.Add(MAPPING_TAG, transporterModel.WorkflowMetadata.GetMappingTag());
            start.StartParameters.Add(COMPANY_CODE, transporterModel.WorkflowMetadata.CompanyCode);
            start.StartParameters.Add(COMPANY_NAME, transporterModel.WorkflowMetadata.CompanyName);
            start.StartParameters.Add(DOCUMENT_TYPE, Convert.ToInt32((int)transporterModel.WorkflowMetadata.DocumentType));
            start.StartParameters.Add(DOCUMENT_NUMBER, transporterModel.WorkflowMetadata.DocumentNumber);
            start.StartParameters.Add(DOCUMENT_DESCRIPTION, transporterModel.WorkflowMetadata.DocumentDescription);
            start.StartParameters.Add(REFERENCE_DOC_TYPE, Convert.ToInt32((int)transporterModel.WorkflowMetadata.ReferenceDocType));
            start.StartParameters.Add(REFERENCE_DOC_NUMBER, transporterModel.WorkflowMetadata.ReferenceDocNumber);
            start.StartParameters.Add(SOURCE_TYPE, Convert.ToInt32((int)transporterModel.WorkflowMetadata.SourceType));
            start.StartParameters.Add(SOURCE_NUMBER, transporterModel.WorkflowMetadata.SourceNumber);
            start.StartParameters.Add(SOURCE_NAME, transporterModel.WorkflowMetadata.SourceName);
            start.StartParameters.Add(SOURCE_VATE_REG_NUMBER, transporterModel.WorkflowMetadata.SourceVatRegNumber);
            start.StartParameters.Add(SOURCE_FISCAL_CODE, transporterModel.WorkflowMetadata.SourceFiscalCode);
            start.StartParameters.Add(SOURCE_EMAIL, transporterModel.WorkflowMetadata.SourceEmail);
            start.StartParameters.Add(SOURCE_CERTIFIED_EMAIL, transporterModel.WorkflowMetadata.SourceCertifiedEmail);
            start.StartParameters.Add(SOURCE_LEGAL_FORM, transporterModel.WorkflowMetadata.SourceLegalForm);
            start.StartParameters.Add(CREATION_USER, transporterModel.WorkflowMetadata.CreationUser);
            start.StartParameters.Add(CREATION_EMAIL, transporterModel.DomainUser.EmailAddress);
            start.StartParameters.Add(CREATION_DISPLAY_NAME, transporterModel.DomainUser.DisplayName);
            start.StartParameters.Add(CREATION_DATE, transporterModel.WorkflowMetadata.CreationDateTime);
            start.StartParameters.Add(IS_EXTERNAL_PARTNER, (transporterModel.WorkflowMetadata.IsExternalPartner == 1));
            start.StartParameters.Add(JOB_CODE, transporterModel.WorkflowMetadata.JobCode);
            start.StartParameters.Add(JOB_DESCRIPTION, transporterModel.WorkflowMetadata.JobDescription);
            start.StartParameters.Add(JOB_TYPE, transporterModel.WorkflowMetadata.JobType);
            start.StartParameters.Add(JOB_AMOUNT_LCY, transporterModel.WorkflowMetadata.JobDelegateAmountLCY);
            start.StartParameters.Add(IS_ITEM_SPECIAL_CATEGORY, transporterModel.WorkflowMetadata.IsItemSpecialCategory == 1);
            start.StartParameters.Add(SPECIAL_CATEGORY_CODE, transporterModel.WorkflowMetadata.SpecialCategoryCode);
            start.StartParameters.Add(SPECIAL_CATEGORY_DESCRIPTION, transporterModel.WorkflowMetadata.SpecialCategoryDescription);
            start.StartParameters.Add(IS_ITEM_EMERGENCY, transporterModel.WorkflowMetadata.IsItemEmergency == 1);
            start.StartParameters.Add(IS_ITEM_STANDARD_WORKFLOW, transporterModel.WorkflowMetadata.IsItemStandardWorkflow == 1);
            start.StartParameters.Add(AMOUNT_LCY, (double)transporterModel.WorkflowMetadata.AmountLCY);
            start.StartParameters.Add(AMOUNT_LCY_DISPLAY, transporterModel.WorkflowMetadata.AmountLCY.ToString("N", CultureInfo.CurrentCulture.NumberFormat));
            start.StartParameters.Add(CURRENCY_CODE, transporterModel.WorkflowMetadata.CurrencyCode);
            start.StartParameters.Add(IS_INTERNAL_PO, transporterModel.WorkflowMetadata.IsInternalPO == 1);
            start.StartParameters.Add(POR_RESPONSIBLE_1, transporterModel.WorkflowMetadata.POResponsibleLevel1);
            start.StartParameters.Add(POR_RESPONSIBLE_2, transporterModel.WorkflowMetadata.POResponsibleLevel2);
            start.StartParameters.Add(POR_RESPONSIBLE_3, transporterModel.WorkflowMetadata.POResponsibleLevel3);
            start.StartParameters.Add(POR_RESPONSIBLE_SPECIAL_CATEGORY, transporterModel.WorkflowMetadata.POResponsibleSpecialCategory);
            start.StartParameters.Add(IS_LEGAL_CHECKED, transporterModel.WorkflowMetadata.IsLegalChecked == 1);
            start.StartParameters.Add(NOTE1, transporterModel.WorkflowMetadata.Note1);
            start.StartParameters.Add(NOTE2, transporterModel.WorkflowMetadata.Note2);
            start.StartParameters.Add(WORKFLOW_LABEL, workflowName);
            start.StartParameters.Add(LIMIT_PCLCY, _moduleConfiguration.LimitPCLCY);
            start.StartParameters.Add(LIMIT_LCY, _moduleConfiguration.LimitLCY);
            start.StartParameters.Add(WORKFLOW_AUTOAPPROVAL, transporterModel.WorkflowMetadata.IsWorkflowAutoapproval == 1);

            string specialCategoryEmails = "<nessuna>";
            int specialRoleId = -1;
            if (transporterModel.SpecialCategory != null)
            {
                specialCategoryEmails = transporterModel.SpecialCategory.Email;
                specialRoleId = transporterModel.SpecialCategory.DocSuiteRole;
            }
            start.StartParameters.Add(EMAIL_SPECIAL_CATEGORY, specialCategoryEmails);
            start.StartParameters.Add(ROLE_SPECIAL_CATEGORY, specialRoleId);

            string companyCeoEmails = "<nessuna>";
            int companyCeoRoleId = -1;
            string procurementEmails = "<nessuna>";
            int procurementRoleId = -1;
            if (transporterModel.ProcurementRight != null)
            {
                companyCeoEmails = transporterModel.ProcurementRight.DocSuiteCeoEmails;
                companyCeoRoleId = transporterModel.ProcurementRight.DocSuiteCeoRole;
                procurementEmails = transporterModel.ProcurementRight.DocSuiteProcurementEmails;
                procurementRoleId = transporterModel.ProcurementRight.DocSuiteProcurementRole;

                start.StartParameters.Add(LIMIT_PROCUREMENT_RESPONSIBLE_LCY, transporterModel.ProcurementRight.LimitProcurementResponsibleLCY);
                start.StartParameters.Add(LIMIT_EMPOWERED_RESPONSIBLE_LCY, transporterModel.ProcurementRight.LimitEmpoweredResponsibleLCY);
            }
            start.StartParameters.Add(EMAIL_COMPANY_CEO, companyCeoEmails);
            start.StartParameters.Add(ROLE_COMPANY_CEO, companyCeoRoleId);
            start.StartParameters.Add(EMAIL_COMPANY_PROCUREMENT, procurementEmails);
            start.StartParameters.Add(ROLE_COMPANY_PROCUREMENT, procurementRoleId);

            WorkflowResult result = _webApiClient.StartWorkflow(start).Result;
            _logger.WriteInfo(new LogMessage(JsonConvert.SerializeObject(result)), LogCategories);
            if (!result.IsValid)
            {
                foreach (TranslationError wfError in result.Errors)
                {
                    _logger.WriteError(new LogMessage(string.Format("WorkflowService.StartWorkflow -> workflow result error: {0}", wfError.Message)), LogCategories);
                }
                throw new Exception("workflow start error");
            }

            return result.InstanceId.Value;
        }
        #endregion
    }
}
