using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Parameters;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.UnitOfWork;

namespace VecompSoftware.DocSuite.Service.Models.Parameters
{
    public class ParameterEnvService : BaseModelService<ParameterEnv>, IParameterEnvService
    {
        #region [ Fields ]

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        private const string WORKFLOW_LOCATION = "WorkflowLocation";
        private const string COLLABORATION_LOCATION = "CollaborationLocation";
        private const string MESSAGE_LOCATION = "MessageLocation";
        private const string TENANT_MODEL = "TenantModel";
        private const string ABSENT_MANAGERS_CERTIFICATES = "AbsentManagersCertificates";
        private const string UDS_LOCATION = "UDSLocation";
        private const string TEMPLATE_REPORT_LOCATION = "TemplateReportLocation";
        private const string PROCESS_CONTAINER = "ProcessContainer";
        private const string PROCESS_ROLE = "ProcessRole";
        private const string PROCESS_ENABLED = "ProcessEnabled";
        private const string ARCHIVE_SECURITYGROUPS_GENERATION_ENABLED = "ArchiveSecurityGroupsGenerationEnabled";
        private const string SECURE_DOCUMENT_SIGNATURE_ENABLED = "SecureDocumentSignatureEnabled";
        private const string SECURE_PAPER_ID = "SecurePaperServiceId";
        private const string SECURE_PAPER_CERTIFICATE_THUMBPRINT = "SecurePaperCertificateThumbprint";
        private const string SECURE_PAPER_SERVICE_URL = "SecurePaperServiceUrl";
        private const string SIGNATURE_PROTOCOL_TYPE = "SignatureType";
        private const string SIGNATURE_PROTOCOL_STRING = "SignatureString";
        private const string SIGNATURE_PROTOCOL_MAIN_FORMAT = "ProtocolSignatureFormat";
        private const string SIGNATURE_PROTOCOL_ATTACHMENT_FORMAT = "AttachmentSignatureFormat";
        private const string SIGNATURE_PROTOCOL_ANNEXED_FORMAT = "AnnexedSignatureFormat";
        private const string CORPORATE_ACRONYM = "CorporateAcronym";
        private const string CORPORATE_NAME = "CorporateName";
        private const string FASCICLE_CONTAINER_ENABLED = "FascicleContainerEnabled";
        private const string ROLE_CONTACT_ENABLED = "RoleContactEnabled";
        private const string GROUP_TBL_CONTACT = "GroupTblContact";
        private static short? _collaborationLocationId = null;
        private static short? _workflowLocationId = null;
        private static short? _messageLocationId = null;
        private static short? _UDSLocationId = null;
        private static short? _templateReportLocationId = null;
        private static short? _processContainerId = null;
        private static short? _processRoleId = null;
        private static bool? _archiveSecurityGroupsGenerationEnabled = null;
        private static bool? _secureDocumentSignatureEnabled;
        private static short? _securePaperServiceId;
        private static short? _signatureProtocolType;
        private static string _signatureProtocolString;
        private static string _signatureProtocolMainFormat;
        private static string _signatureProtocolAttachmentFormat;
        private static string _signatureProtocolAnnexedFormat;
        private static string _securePaperCertificateThumbprint;
        private static string _securePaperServiceUrl;
        private static string _corporateAcronym;
        private static string _corporateName;
        private static bool? _fascicleContainerEnabled;
        private static bool? _roleContactEnabled;
        private static string _groupTblContact;
        private static bool? _processEnabled;

        private static AbsentManagerCertificateModel _absentManagersCertificates = null;
        private static List<TenantModel> _tenantModels = null;
        private static TenantModel _currentTenantModel = null;        


        #endregion

        #region [ Properties ]
        private IList<TenantModel> InternalTenantModels
        {
            get
            {
                if (_tenantModels == null)
                {
                    string value = GetParameter(TENANT_MODEL);
                    _tenantModels = _mapperUnitOfWork.Repository<IMapper<string, List<TenantModel>>>().Map(value, _tenantModels);
                }
                return _tenantModels;
            }
        }

        public string CustomInstanceName { get; set; }

        public short WorkflowLocationId
        {
            get
            {
                if (!_workflowLocationId.HasValue)
                {
                    _workflowLocationId = -1;
                    string value = GetParameter(WORKFLOW_LOCATION);
                    _workflowLocationId = _mapperUnitOfWork.Repository<IMapper<string, short>>().Map(value, _workflowLocationId.Value);
                }
                return _workflowLocationId.Value;
            }
        }

        public short CollaborationLocationId
        {
            get
            {
                if (!_collaborationLocationId.HasValue)
                {
                    _collaborationLocationId = 99;
                    string value = GetParameter(COLLABORATION_LOCATION);
                    _collaborationLocationId = _mapperUnitOfWork.Repository<IMapper<string, short>>().Map(value, _collaborationLocationId.Value);
                }
                return _collaborationLocationId.Value;
            }
        }

        public short MessageLocationId
        {
            get
            {
                if (!_messageLocationId.HasValue)
                {
                    _messageLocationId = -1;
                    string value = GetParameter(MESSAGE_LOCATION);
                    _messageLocationId = _mapperUnitOfWork.Repository<IMapper<string, short>>().Map(value, _messageLocationId.Value);
                }
                return _messageLocationId.Value;
            }
        }

        public short UDSLocationId
        {
            get
            {
                if (!_UDSLocationId.HasValue)
                {
                    _UDSLocationId = -1;
                    string value = GetParameter(UDS_LOCATION);
                    _UDSLocationId = _mapperUnitOfWork.Repository<IMapper<string, short>>().Map(value, _UDSLocationId.Value);
                }
                return _UDSLocationId.Value;
            }
        }

        public short TemplateReportLocationId
        {
            get
            {
                if (!_templateReportLocationId.HasValue)
                {
                    _templateReportLocationId = -1;
                    string value = GetParameter(TEMPLATE_REPORT_LOCATION);
                    _templateReportLocationId = _mapperUnitOfWork.Repository<IMapper<string, short>>().Map(value, _templateReportLocationId.Value);
                }
                return _templateReportLocationId.Value;
            }
        }

        public short ProcessContainerId
        {
            get
            {
                if (!_processContainerId.HasValue)
                {
                    _processContainerId = -1;
                    string value = GetParameter(PROCESS_CONTAINER);
                    _processContainerId = _mapperUnitOfWork.Repository<IMapper<string, short>>().Map(value, _processContainerId.Value);
                }
                return _processContainerId.Value;
            }
        }

        public short ProcessRoleId
        {
            get
            {
                if (!_processRoleId.HasValue)
                {
                    _processRoleId = -1;
                    string value = GetParameter(PROCESS_ROLE);
                    _processRoleId = _mapperUnitOfWork.Repository<IMapper<string, short>>().Map(value, _processRoleId.Value);
                }
                return _processRoleId.Value;
            }
        }

        public string CurrentTenantName
        {
            get
            {
                return CurrentTenantModel.TenantName;
            }
        }

        public Guid CurrentTenantId
        {
            get
            {
                return CurrentTenantModel.TenantId;
            }
        }

        public TenantModel CurrentTenantModel
        {
            get
            {
                if (_currentTenantModel == null && InternalTenantModels != null && InternalTenantModels.Count > 0)
                {
                    _currentTenantModel = _tenantModels.Where(t => t.CurrentTenant).FirstOrDefault();
                }
                return _currentTenantModel;
            }
        }

        public IList<TenantModel> TenantModels
        {
            get
            {
                return InternalTenantModels;
            }
        }

        public AbsentManagerCertificateModel AbsentManagersCertificates
        {
            get
            {
                if (_absentManagersCertificates == null)
                {
                    string value = GetParameter(ABSENT_MANAGERS_CERTIFICATES);
                    if (!string.IsNullOrEmpty(value))
                    {
                        _absentManagersCertificates = _mapperUnitOfWork.Repository<IMapper<string, AbsentManagerCertificateModel>>().Map(value, _absentManagersCertificates);
                    }
                }
                return _absentManagersCertificates;
            }
        }

        public bool ArchiveSecurityGroupsGenerationEnabled
        {
            get
            {
                if (_archiveSecurityGroupsGenerationEnabled == null)
                {
                    _archiveSecurityGroupsGenerationEnabled = true;
                    string value = GetParameter(ARCHIVE_SECURITYGROUPS_GENERATION_ENABLED);
                    _archiveSecurityGroupsGenerationEnabled = _mapperUnitOfWork.Repository<IMapper<string, bool>>().Map(value, _archiveSecurityGroupsGenerationEnabled.Value);
                }
                return _archiveSecurityGroupsGenerationEnabled.Value;
            }
        }

        public bool SecureDocumentSignatureEnabled
        {
            get
            {
                if (!_secureDocumentSignatureEnabled.HasValue)
                {
                    _secureDocumentSignatureEnabled = false;
                    string value = GetParameter(SECURE_DOCUMENT_SIGNATURE_ENABLED);
                    _secureDocumentSignatureEnabled = _mapperUnitOfWork.Repository<IMapper<string, bool>>().Map(value, _secureDocumentSignatureEnabled.Value);
                }
                return _secureDocumentSignatureEnabled.Value;
            }
        }

        public short SecurePaperServiceId
        {
            get
            {
                if (!_securePaperServiceId.HasValue)
                {
                    _securePaperServiceId = -1;
                    string value = GetParameter(SECURE_PAPER_ID);
                    _securePaperServiceId = _mapperUnitOfWork.Repository<IMapper<string, short>>().Map(value, _securePaperServiceId.Value);
                }
                return _securePaperServiceId.Value;
            }
        }

        public string SecurePaperCertificateThumbprint
        {
            get
            {
                if (string.IsNullOrEmpty(_securePaperCertificateThumbprint))
                {
                    _securePaperCertificateThumbprint = GetParameter(SECURE_PAPER_CERTIFICATE_THUMBPRINT) ?? string.Empty;
                }

                return _securePaperCertificateThumbprint;
            }
        }

        public string SecurePaperServiceUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_securePaperServiceUrl))
                {
                    _securePaperServiceUrl = GetParameter(SECURE_PAPER_SERVICE_URL) ?? string.Empty;
                }

                return _securePaperServiceUrl;
            }
        }

        public short SignatureProtocolType
        {
            get
            {
                if (!_signatureProtocolType.HasValue)
                {
                    _signatureProtocolType = 0;
                    string value = GetParameter(SIGNATURE_PROTOCOL_TYPE);
                    _signatureProtocolType = _mapperUnitOfWork.Repository<IMapper<string, short>>().Map(value, _signatureProtocolType.Value);
                }
                return _signatureProtocolType.Value;
            }
        }

        public string SignatureProtocolString
        {
            get
            {
                if (string.IsNullOrEmpty(_signatureProtocolString))
                {
                    _signatureProtocolString = GetParameter(SIGNATURE_PROTOCOL_STRING);
                }
                return _signatureProtocolString;
            }
        }

        public string SignatureProtocolMainFormat
        {
            get
            {
                if (string.IsNullOrEmpty(_signatureProtocolMainFormat))
                {
                    _signatureProtocolMainFormat = GetParameter(SIGNATURE_PROTOCOL_MAIN_FORMAT);
                }
                if (string.IsNullOrEmpty(_signatureProtocolMainFormat))
                {
                    _signatureProtocolMainFormat = "{0:Short}/{1:Number:0000000}/{1:Date:dd/MM/yyyy}/{1:Direction:Short}/{1:Container:Name}({2:DocumentType:Short}/AA.{2:AttachmentsCount});";
                }
                return _signatureProtocolMainFormat;
            }
        }

        public string SignatureProtocolAttachmentFormat
        {
            get
            {
                if (string.IsNullOrEmpty(_signatureProtocolAttachmentFormat))
                {
                    _signatureProtocolAttachmentFormat = GetParameter(SIGNATURE_PROTOCOL_ATTACHMENT_FORMAT);
                }
                if (string.IsNullOrEmpty(_signatureProtocolAttachmentFormat))
                {
                    _signatureProtocolAttachmentFormat = "{0:Short}/{1:Number:0000000}/{1:Date:dd/MM/yyyy}/{1:Direction:Short}/{1:Container:Name}({2:DocumentType:Short}.{2:DocumentNumber})";
                }
                return _signatureProtocolAttachmentFormat;
            }
        }

        public string SignatureProtocolAnnexedFormat
        {
            get
            {
                if (string.IsNullOrEmpty(_signatureProtocolAnnexedFormat))
                {
                    _signatureProtocolAnnexedFormat = GetParameter(SIGNATURE_PROTOCOL_ANNEXED_FORMAT);
                }
                if (string.IsNullOrEmpty(_signatureProtocolAnnexedFormat))
                {
                    _signatureProtocolAnnexedFormat = "{0:Short}/{1:Number:0000000}/{1:Date:dd/MM/yyyy}/{1:Direction:Short}/{1:Container:Name}";
                }
                return _signatureProtocolAnnexedFormat;
            }
        }

        public string CorporateAcronym
        {
            get
            {
                if (string.IsNullOrEmpty(_corporateAcronym))
                {
                    _corporateAcronym = GetParameter(CORPORATE_ACRONYM);
                }
                return _corporateAcronym;
            }
        }
        public string CorporateName
        {
            get
            {
                if (string.IsNullOrEmpty(_corporateName))
                {
                    _corporateName = GetParameter(CORPORATE_NAME);
                }
                return _corporateName;
            }
        }

        public bool FascicleContainerEnabled
        {
            get
            {
                if (!_fascicleContainerEnabled.HasValue)
                {
                    _fascicleContainerEnabled = false;
                    string value = GetParameter(FASCICLE_CONTAINER_ENABLED);
                    _fascicleContainerEnabled = _mapperUnitOfWork.Repository<IMapper<string, bool>>().Map(value, _fascicleContainerEnabled.Value);
                }
                return _fascicleContainerEnabled.Value;
            }
        }
        public bool RoleContactEnabled
        {
            get
            {
                if (!_roleContactEnabled.HasValue)
                {
                    _roleContactEnabled = false;
                    string value = GetParameter(ROLE_CONTACT_ENABLED);
                    _roleContactEnabled = _mapperUnitOfWork.Repository<IMapper<string, bool>>().Map(value, _roleContactEnabled.Value);
                }
                return _roleContactEnabled.Value;
            }
        }

        public string GroupTblContact
        {
            get
            {
                if (string.IsNullOrEmpty(_groupTblContact))
                {
                    _groupTblContact = GetParameter(GROUP_TBL_CONTACT);
                }
                return _groupTblContact;
            }
        }

        public bool ProcessEnabled
        {
            get
            {
                if (!_processEnabled.HasValue)
                {
                    _processEnabled = false;
                    string value = GetParameter(PROCESS_ENABLED);
                    _processEnabled = _mapperUnitOfWork.Repository<IMapper<string, bool>>().Map(value, _processEnabled.Value);
                }
                return _processEnabled.Value;
            }
        }
        #endregion

        #region [ Constructor ]

        public ParameterEnvService(IDataUnitOfWork unitOfWork, ILogger logger, IMapperUnitOfWork mapperUnitOfWork)
            : base(unitOfWork, logger, mapperUnitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        #endregion

        #region [ Methods ]

        private string GetParameter(string name)
        {
            ParameterEnv parameter = null;
            string parameterName = name;
            if (!string.IsNullOrEmpty(CustomInstanceName))
            {
                parameterName = string.Concat(CustomInstanceName, name);
            }

            parameter = _unitOfWork.Repository<ParameterEnv>().GetParameter(parameterName).FirstOrDefault();
            if (parameter == null)
            {
                parameter = _unitOfWork.Repository<ParameterEnv>().GetParameter(name).FirstOrDefault();
            }
            _logger.WriteDebug(new LogMessage($"ParemeterENV service - get  {parameterName} with value {parameter?.Value}"), LogCategories);
            return parameter?.Value;
        }


        #endregion

    }
}