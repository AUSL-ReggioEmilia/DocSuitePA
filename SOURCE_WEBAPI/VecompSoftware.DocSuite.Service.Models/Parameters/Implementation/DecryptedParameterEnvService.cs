using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Common.Securities;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Model.Parameters;

namespace VecompSoftware.DocSuite.Service.Models.Parameters
{
    public class DecryptedParameterEnvService : ParameterEnvBaseService, IDecryptedParameterEnvService
    {
        #region [ Fields ]
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
        private static string _corporateAcronym;
        private static string _corporateName;
        private static bool? _fascicleContainerEnabled;
        private static bool? _roleContactEnabled;
        private static string _groupTblContact;
        private static bool? _processEnabled;
        private static bool? _roleGroupPECRightEnabled;
        private static bool? _shibbolethEnabled;
        private static short? _contactAOOParentId = null;
        private static string _signatureTemplate;
        private static bool? _forceDescendingOrderElements;
        private static int? _fascicleAutoCloseThresholdDays;
        private static int? _fascicleContactId;
        private static int? _fascicleMiscellaneaLocation;
        private static bool? _multiAOOFascicleEnabled;
        private static int? _avcpDefaultCategoryId;
        private static string _avcpDatasetUrlMask;
        private static string _avcpEntePubblicatore;
        private static string _avcpLicenza;
        private static int? _avcpDocumentSeriesId;
        private static int? _avcpResolutionType;
        private static string _avcpInclusiveNumberMask;
        private static string _basicPersonSearcherKey;
        private static bool? _forceProsecutableEnabled;
        private static bool? _collaborationMailEnabled;
        private static int? _collaborationSignatureType;

        private static ICollection<string> _docSuiteServiceAccounts;
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

        public bool RoleGroupPECRightEnabled
        {
            get
            {
                if (!_roleGroupPECRightEnabled.HasValue)
                {
                    _roleGroupPECRightEnabled = false;
                    string value = GetParameter(ROLEGROUPPECRIGHT_ENABLED);
                    _roleGroupPECRightEnabled = _mapperUnitOfWork.Repository<IMapper<string, bool>>().Map(value, _roleGroupPECRightEnabled.Value);
                }
                return _roleGroupPECRightEnabled.Value;
            }
        }

        public bool ShibbolethEnabled
        {
            get
            {
                if (!_shibbolethEnabled.HasValue)
                {
                    _shibbolethEnabled = false;
                    string value = GetParameter(SHIBBOLETH_ENABLED);
                    _shibbolethEnabled = _mapperUnitOfWork.Repository<IMapper<string, bool>>().Map(value, _shibbolethEnabled.Value);
                }
                return _shibbolethEnabled.Value;
            }
        }

        public short ContactAOOParentId
        {
            get
            {
                if (!_contactAOOParentId.HasValue)
                {
                    _contactAOOParentId = -1;
                    string value = GetParameter(CONTACT_AOO_PARENT);
                    _contactAOOParentId = _mapperUnitOfWork.Repository<IMapper<string, short>>().Map(value, _contactAOOParentId.Value);
                }
                return _contactAOOParentId.Value;
            }
        }

        public string SignatureTemplate
        {
            get
            {
                if (string.IsNullOrEmpty(_signatureTemplate))
                {
                    _signatureTemplate = GetParameter(SIGNATURE_TEMPLATE);
                }
                if (string.IsNullOrEmpty(_signatureTemplate))
                {
                    _signatureTemplate = "<Label><Text>(SIGNATURE) Pagina (pagina) di (pagine)</Text><Font Face=\"Arial\" Size=\"10\" Style=\"Bold\" /></Label>";
                }
                if (!string.IsNullOrEmpty(_signatureTemplate))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(_signatureTemplate);
                    _signatureTemplate = xmlDocument.OuterXml;
                }
                return _signatureTemplate;
            }
        }

        public bool ForceDescendingOrderElements
        {
            get
            {
                if (!_forceDescendingOrderElements.HasValue)
                {
                    _forceDescendingOrderElements = false;
                    string value = GetParameter(FORCE_DESCENDING_ORDER_ELEMENTS);
                    _forceDescendingOrderElements = _mapperUnitOfWork.Repository<IMapper<string, bool>>().Map(value, _forceDescendingOrderElements.Value);
                }
                return _forceDescendingOrderElements.Value;
            }
        }

        public int FascicleAutoCloseThresholdDays
        {
            get
            {
                if (!_fascicleAutoCloseThresholdDays.HasValue)
                {
                    string value = GetParameter(FASCICLE_AUTOCLOSE_THRESHOLDDAYS);
                    _fascicleAutoCloseThresholdDays = 60;
                    _fascicleAutoCloseThresholdDays = _mapperUnitOfWork.Repository<IMapper<string, int>>().Map(value, _fascicleAutoCloseThresholdDays.Value);
                }
                return _fascicleAutoCloseThresholdDays.Value;
            }
        }

        public int FascicleContactId
        {
            get
            {
                if (!_fascicleContactId.HasValue)
                {
                    string value = GetParameter(FASCICLE_CONTACT_ID);
                    _fascicleContactId = -1;
                    _fascicleContactId = _mapperUnitOfWork.Repository<IMapper<string, int>>().Map(value, _fascicleContactId.Value);
                }
                return _fascicleContactId.Value;
            }
        }

        public int FascicleMiscellaneaLocation
        {
            get
            {
                if (!_fascicleMiscellaneaLocation.HasValue)
                {
                    string value = GetParameter(FASCICLE_MISCELLANEA_LOCATION);
                    _fascicleMiscellaneaLocation = -1;
                    _fascicleMiscellaneaLocation = _mapperUnitOfWork.Repository<IMapper<string, int>>().Map(value, _fascicleMiscellaneaLocation.Value);
                }
                return _fascicleMiscellaneaLocation.Value;
            }
        }

        public bool MultiAOOFascicleEnabled
        {
            get
            {
                if (!_multiAOOFascicleEnabled.HasValue)
                {
                    _multiAOOFascicleEnabled = false;
                    string value = GetParameter(MULTI_AOO_FASCICLE_ENABLED);
                    _multiAOOFascicleEnabled = _mapperUnitOfWork.Repository<IMapper<string, bool>>().Map(value, _multiAOOFascicleEnabled.Value);
                }
                return _multiAOOFascicleEnabled.Value;
            }
        }

        public int AVCPDefaultCategoryId
        {
            get
            {
                if (!_avcpDefaultCategoryId.HasValue)
                {
                    string value = GetParameter(AVCP_DEFAULT_CATEGORY_ID);
                    _avcpDefaultCategoryId = -1;
                    _avcpDefaultCategoryId = _mapperUnitOfWork.Repository<IMapper<string, int>>().Map(value, _avcpDefaultCategoryId.Value);
                }
                return _avcpDefaultCategoryId.Value;
            }
        }

        public string AVCPDatasetUrlMask
        {
            get
            {
                if (string.IsNullOrEmpty(_avcpDatasetUrlMask))
                {
                    _avcpDatasetUrlMask = GetParameter(AVCP_DATASET_URL_MASK);
                }
                return _avcpDatasetUrlMask;
            }
        }

        public string AVCPEntePubblicatore
        {
            get
            {
                if (string.IsNullOrEmpty(_avcpEntePubblicatore))
                {
                    _avcpEntePubblicatore = GetParameter(AVCP_ENTE_PUBBLICATORE);
                }
                return _avcpEntePubblicatore;
            }
        }

        public string AVCPLicenza
        {
            get
            {
                if (string.IsNullOrEmpty(_avcpLicenza))
                {
                    _avcpLicenza = GetParameter(AVCP_LICENZA);
                }
                return _avcpLicenza;
            }
        }

        public int AvcpDocumentSeriesId
        {
            get
            {
                if (!_avcpDocumentSeriesId.HasValue)
                {
                    string value = GetParameter(AVCP_DOCUMENT_SERIES_ID);
                    _avcpDocumentSeriesId = -1;
                    _avcpDocumentSeriesId = _mapperUnitOfWork.Repository<IMapper<string, int>>().Map(value, _avcpDocumentSeriesId.Value);
                }
                return _avcpDocumentSeriesId.Value;
            }
        }

        public int AVCPResolutionType
        {
            get
            {
                if (!_avcpResolutionType.HasValue)
                {
                    string value = GetParameter(AVCP_RESOLUTION_TYPE);
                    _avcpResolutionType = -1;
                    _avcpResolutionType = _mapperUnitOfWork.Repository<IMapper<string, int>>().Map(value, _avcpResolutionType.Value);
                }
                return _avcpResolutionType.Value;
            }
        }

        public string AVCPInclusiveNumberMask
        {
            get
            {
                if (string.IsNullOrEmpty(_avcpInclusiveNumberMask))
                {
                    _avcpInclusiveNumberMask = GetParameter(AVCP_INCLUSIVE_NUMBER_MASK);
                }
                return _avcpInclusiveNumberMask;
            }
        }

        public ICollection<string> DocSuiteServiceAccounts
        {
            get
            {
                if (_docSuiteServiceAccounts == null)
                {
                    _docSuiteServiceAccounts = new List<string>();
                    string tmp = GetParameter(DOCSUITE_SERVICE_ACCOUNTS);
                    _docSuiteServiceAccounts = tmp.Split('|').ToList();
                }
                return _docSuiteServiceAccounts;
            }
        }

        public string BasicPersonSearcherKey
        {
            get
            {
                if (string.IsNullOrEmpty(_basicPersonSearcherKey))
                {
                    string basicPersonSearcherKeyParam = GetParameter(BASIC_PERSON_SEARCH_KEY);

                    _basicPersonSearcherKey = string.IsNullOrEmpty(basicPersonSearcherKeyParam)
                        ? "(&(&(objectCategory=person)(objectClass=user)(!(userAccountControl:1.2.840.113556.1.4.803:=2)))(|(cn=*{0}*)(sAMAccountName={0})))"
                        : basicPersonSearcherKeyParam;
                }

                return _basicPersonSearcherKey;
            }
        }

        public bool ForceProsecutableEnabled {
            get {
                if (!_forceProsecutableEnabled.HasValue)
                {
                    _forceProsecutableEnabled = false;
                    string value = GetParameter(FORCE_PROSECUTABLE);
                    _forceProsecutableEnabled = _mapperUnitOfWork.Repository<IMapper<string, bool>>().Map(value, _forceProsecutableEnabled.Value);
                }
                return _forceProsecutableEnabled.Value;
            }
        }

        public bool CollaborationMailEnabled {
            get {
                if (!_collaborationMailEnabled.HasValue)
                {
                    _collaborationMailEnabled = false;
                    string value = GetParameter(COLLABORATION_MAIL);
                    _collaborationMailEnabled = _mapperUnitOfWork.Repository<IMapper<string, bool>>().Map(value, _collaborationMailEnabled.Value);
                }
                return _collaborationMailEnabled.Value;
            }
        }

        public int CollaborationSignatureType {
            get {
                if (!_collaborationSignatureType.HasValue)
                {
                    string value = GetParameter(COLLABORATION_SIGNATURE_TYPE);
                    _collaborationSignatureType = 0;
                    _collaborationSignatureType = _mapperUnitOfWork.Repository<IMapper<string, int>>().Map(value, _collaborationSignatureType.Value);
                }
                return _collaborationSignatureType.Value;
            }
        }

        #endregion

        #region [ Constructor ]
        public DecryptedParameterEnvService(IDataUnitOfWork unitOfWork, ILogger logger, IMapperUnitOfWork mapperUnitOfWork, IEncryptionKey encryptionKey)
            : base(unitOfWork, logger, mapperUnitOfWork, encryptionKey, useEncryption: false)
        {
        }
        #endregion

        #region [ Methods ]
        #endregion
    }
}
