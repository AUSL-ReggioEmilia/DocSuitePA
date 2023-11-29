using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Common.Securities;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Parameters;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.UnitOfWork;
using VecompSoftware.Helpers.Security;

namespace VecompSoftware.DocSuite.Service.Models.Parameters
{
    public abstract class ParameterEnvBaseService : BaseModelService<ParameterEnv>, IParameterEnvBaseService
    {
        #region [ Fields ]
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ILogger _logger;
        protected readonly IMapperUnitOfWork _mapperUnitOfWork;
        protected readonly string _passwordEncryptionKey;
        protected readonly bool _useEncryption;

        protected string WORKFLOW_LOCATION = "WorkflowLocation";
        protected string COLLABORATION_LOCATION = "CollaborationLocation";
        protected string MESSAGE_LOCATION = "MessageLocation";
        protected string TENANT_MODEL = "TenantModel";
        protected string ABSENT_MANAGERS_CERTIFICATES = "AbsentManagersCertificates";
        protected string UDS_LOCATION = "UDSLocation";
        protected string TEMPLATE_REPORT_LOCATION = "TemplateReportLocation";
        protected string PROCESS_CONTAINER = "ProcessContainer";
        protected string PROCESS_ROLE = "ProcessRole";
        protected string PROCESS_ENABLED = "ProcessEnabled";
        protected string ROLEGROUPPECRIGHT_ENABLED = "RoleGroupPECRightEnabled";
        protected string ARCHIVE_SECURITYGROUPS_GENERATION_ENABLED = "ArchiveSecurityGroupsGenerationEnabled";
        protected string SECURE_DOCUMENT_SIGNATURE_ENABLED = "SecureDocumentSignatureEnabled";
        protected string SECURE_PAPER_ID = "SecurePaperServiceId";
        protected string SECURE_PAPER_CERTIFICATE_THUMBPRINT = "SecurePaperCertificateThumbprint";
        protected string SECURE_PAPER_SERVICE_URL = "SecurePaperServiceUrl";
        protected string SIGNATURE_PROTOCOL_TYPE = "SignatureType";
        protected string SIGNATURE_PROTOCOL_STRING = "SignatureString";
        protected string SIGNATURE_PROTOCOL_MAIN_FORMAT = "ProtocolSignatureFormat";
        protected string SIGNATURE_PROTOCOL_ATTACHMENT_FORMAT = "AttachmentSignatureFormat";
        protected string SIGNATURE_PROTOCOL_ANNEXED_FORMAT = "AnnexedSignatureFormat";
        protected string CORPORATE_ACRONYM = "CorporateAcronym";
        protected string CORPORATE_NAME = "CorporateName";
        protected string FASCICLE_CONTAINER_ENABLED = "FascicleContainerEnabled";
        protected string ROLE_CONTACT_ENABLED = "RoleContactEnabled";
        protected string GROUP_TBL_CONTACT = "GroupTblContact";
        protected string SHIBBOLETH_ENABLED = "ShibbolethEnabled";
        protected string CONTACT_AOO_PARENT = "ContactAOOParentId";
        protected string SIGNATURE_TEMPLATE = "SignatureTemplate";
        protected string FORCE_DESCENDING_ORDER_ELEMENTS = "ForceDescendingOrderElements";
        protected string FASCICLE_AUTOCLOSE_THRESHOLDDAYS = "FascicleAutoCloseThresholdDays";
        protected string FASCICLE_CONTACT_ID = "FascicleContactId";
        protected string FASCICLE_MISCELLANEA_LOCATION = "FascicleMiscellaneaLocation";
        protected string MULTI_AOO_FASCICLE_ENABLED = "MultiAOOFascicleEnabled";
        protected string AVCP_DEFAULT_CATEGORY_ID = "AVCPDefaultCategoryId";
        protected string AVCP_DATASET_URL_MASK = "AVCPDatasetUrlMask";
        protected string AVCP_ENTE_PUBBLICATORE = "AVCPEntePubblicatore";
        protected string AVCP_LICENZA = "AVCPLicenza";
        protected string AVCP_DOCUMENT_SERIES_ID = "AvcpDocumentSeriesId";
        protected string AVCP_RESOLUTION_TYPE = "AVCPResolutionType";
        protected string AVCP_INCLUSIVE_NUMBER_MASK = "AVCPInclusiveNumberMask";
        protected string DOCSUITE_SERVICE_ACCOUNTS = "DocSuiteServiceAccounts";
        protected string BASIC_PERSON_SEARCH_KEY = "BasicPersonSearcherKey";
        protected string PASSWORD_ENCRYPTION_KEY = "VecompSoftware.DocSuiteWeb.PasswordEncryptionKey";
        protected string FORCE_PROSECUTABLE = "ForceProsecutable";
        protected string COLLABORATION_MAIL = "CollaborationMail";
        protected string COLLABORATION_SIGNATURE_TYPE = "CollaborationSignatureType";
        #endregion

        #region [ Properties ]
        public string CustomInstanceName { get; set; }
        #endregion

        #region [ Constructor ]
        public ParameterEnvBaseService(IDataUnitOfWork unitOfWork, ILogger logger, IMapperUnitOfWork mapperUnitOfWork, IEncryptionKey encryptionKey, bool useEncryption)
            : base(unitOfWork, logger, mapperUnitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapperUnitOfWork = mapperUnitOfWork;
            _passwordEncryptionKey = encryptionKey.Value;
            _useEncryption = useEncryption;
        }
        #endregion

        #region [ Methods ]
        protected string GetParameter(string name)
        {
            ParameterEnv parameter = null;
            string parameterName = !string.IsNullOrEmpty(CustomInstanceName)
                ? $"{CustomInstanceName}{name}"
                : name;
            parameter = _unitOfWork.Repository<ParameterEnv>().GetParameter(parameterName).FirstOrDefault();
            if (parameter == null)
            {
                parameter = _unitOfWork.Repository<ParameterEnv>().GetParameter(name).FirstOrDefault();
            }
            _logger.WriteDebug(new LogMessage($"ParemeterENV service - get {parameterName} with value {(parameterName.EndsWith(TENANT_MODEL) ? "TENANT_MODEL IS HIDDEN" : parameter?.Value)}"), LogCategories);
            return _useEncryption
                ? parameter?.Value
                : DecryptParameterToken(parameter?.Value);
        }

        protected string EncryptParameterToken(string parameterToken)
        {
            return string.IsNullOrEmpty(parameterToken)
                ? parameterToken
                : EncryptionHelper.EncryptString(parameterToken, _passwordEncryptionKey);
        }

        protected string DecryptParameterToken(string parameterToken)
        {
            return string.IsNullOrEmpty(parameterToken)
                ? parameterToken
                : EncryptionHelper.DecryptString(parameterToken, _passwordEncryptionKey);
        }
        #endregion
    }
}
