using System.Xml;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Common.Securities;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Mapper;

namespace VecompSoftware.DocSuite.Service.Models.Parameters
{
    public class EncryptedParameterEnvService : ParameterEnvBaseService, IEncryptedParameterEnvService
    {
        #region [ Fields ]
        private static string _collaborationLocationId;
        private static string _workflowLocationId;
        private static string _messageLocationId;
        private static string _UDSLocationId;
        private static string _templateReportLocationId;
        private static string _processContainerId;
        private static string _processRoleId;
        private static string _archiveSecurityGroupsGenerationEnabled;
        private static string _secureDocumentSignatureEnabled;
        private static string _securePaperServiceId;
        private static string _signatureProtocolType;
        private static string _signatureProtocolString;
        private static string _signatureProtocolMainFormat;
        private static string _signatureProtocolAttachmentFormat;
        private static string _signatureProtocolAnnexedFormat;
        private static string _corporateAcronym;
        private static string _corporateName;
        private static string _fascicleContainerEnabled;
        private static string _roleContactEnabled;
        private static string _groupTblContact;
        private static string _processEnabled;
        private static string _roleGroupPECRightEnabled;
        private static string _shibbolethEnabled;
        private static string _contactAOOParentId;
        private static string _signatureTemplate;
        private static string _forceDescendingOrderElements;
        private static string _fascicleAutoCloseThresholdDays;
        private static string _fascicleContactId;
        private static string _fascicleMiscellaneaLocation;
        private static string _multiAOOFascicleEnabled;
        private static string _avcpDefaultCategoryId;
        private static string _avcpDatasetUrlMask;
        private static string _avcpEntePubblicatore;
        private static string _avcpLicenza;
        private static string _avcpDocumentSeriesId;
        private static string _avcpResolutionType;
        private static string _avcpInclusiveNumberMask;
        private static string _basicPersonSearcherKey;
        private static string _docSuiteServiceAccounts;
        private static string _absentManagersCertificates;
        private static string _tenantModels;
        private static string _forceProsecutableEnabled;
        private static string _collaborationMailEnabled;
        private static string _collaborationSignatureType;
        #endregion

        #region [ Properties ]
        public string CollaborationLocationId
        {
            get
            {
                if (string.IsNullOrEmpty(_collaborationLocationId))
                {
                    _collaborationLocationId = GetParameter(COLLABORATION_LOCATION) ?? EncryptParameterToken(99.ToString());
                }
                return _collaborationLocationId;
            }
        }

        public string WorkflowLocationId
        {
            get
            {
                if (string.IsNullOrEmpty(_workflowLocationId))
                {
                    _workflowLocationId = GetParameter(WORKFLOW_LOCATION) ?? EncryptParameterToken((-1).ToString());
                }
                return _workflowLocationId;
            }
        }

        public string MessageLocationId
        {
            get
            {
                if (string.IsNullOrEmpty(_messageLocationId))
                {
                    _messageLocationId = GetParameter(MESSAGE_LOCATION) ?? EncryptParameterToken((-1).ToString());
                }
                return _messageLocationId;
            }
        }

        public string UDSLocationId
        {
            get
            {
                if (string.IsNullOrEmpty(_UDSLocationId))
                {
                    _UDSLocationId = GetParameter(UDS_LOCATION) ?? EncryptParameterToken((-1).ToString());
                }
                return _UDSLocationId;
            }
        }

        public string TemplateReportLocationId
        {
            get
            {
                if (string.IsNullOrEmpty(_templateReportLocationId))
                {
                    _templateReportLocationId = GetParameter(TEMPLATE_REPORT_LOCATION) ?? EncryptParameterToken((-1).ToString());
                }
                return _templateReportLocationId;
            }
        }

        public string ProcessContainerId
        {
            get
            {
                if (string.IsNullOrEmpty(_processContainerId))
                {
                    _processContainerId = GetParameter(PROCESS_CONTAINER) ?? EncryptParameterToken((-1).ToString());
                }
                return _processContainerId;
            }
        }

        public string ProcessRoleId
        {
            get
            {
                if (string.IsNullOrEmpty(_processRoleId))
                {
                    _processRoleId = GetParameter(PROCESS_ROLE) ?? EncryptParameterToken((-1).ToString());
                }
                return _processRoleId;
            }
        }

        public string TenantModels
        {
            get
            {
                if (string.IsNullOrEmpty(_tenantModels))
                {
                    _tenantModels = GetParameter(TENANT_MODEL);
                }
                return _tenantModels;
            }
        }

        public string AbsentManagersCertificates
        {
            get
            {
                if (string.IsNullOrEmpty(_absentManagersCertificates))
                {
                    _absentManagersCertificates = GetParameter(ABSENT_MANAGERS_CERTIFICATES);
                }
                return _absentManagersCertificates;
            }
        }

        public string ArchiveSecurityGroupsGenerationEnabled
        {
            get
            {
                if (string.IsNullOrEmpty(_archiveSecurityGroupsGenerationEnabled))
                {
                    _archiveSecurityGroupsGenerationEnabled = GetParameter(ARCHIVE_SECURITYGROUPS_GENERATION_ENABLED) ?? EncryptParameterToken(true.ToString());
                }
                return _archiveSecurityGroupsGenerationEnabled;
            }
        }

        public string SecureDocumentSignatureEnabled
        {
            get
            {
                if (string.IsNullOrEmpty(_secureDocumentSignatureEnabled))
                {
                    _secureDocumentSignatureEnabled = GetParameter(SECURE_DOCUMENT_SIGNATURE_ENABLED) ?? EncryptParameterToken(false.ToString());
                }
                return _secureDocumentSignatureEnabled;
            }
        }

        public string SecurePaperServiceId
        {
            get
            {
                if (string.IsNullOrEmpty(_securePaperServiceId))
                {
                    _securePaperServiceId = GetParameter(SECURE_PAPER_ID) ?? EncryptParameterToken((-1).ToString());
                }
                return _archiveSecurityGroupsGenerationEnabled;
            }
        }

        public string SignatureProtocolType
        {
            get
            {
                if (string.IsNullOrEmpty(_signatureProtocolType))
                {
                    _signatureProtocolType = GetParameter(SIGNATURE_PROTOCOL_TYPE) ?? EncryptParameterToken(0.ToString());
                }
                return _signatureProtocolType;
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
                    _signatureProtocolMainFormat = GetParameter(SIGNATURE_PROTOCOL_MAIN_FORMAT)
                        ?? EncryptParameterToken("{0:Short}/{1:Number:0000000}/{1:Date:dd/MM/yyyy}/{1:Direction:Short}/{1:Container:Name}({2:DocumentType:Short}/AA.{2:AttachmentsCount});");
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
                    _signatureProtocolAttachmentFormat = GetParameter(SIGNATURE_PROTOCOL_ATTACHMENT_FORMAT)
                        ?? EncryptParameterToken("{0:Short}/{1:Number:0000000}/{1:Date:dd/MM/yyyy}/{1:Direction:Short}/{1:Container:Name}({2:DocumentType:Short}.{2:DocumentNumber})");
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
                    _signatureProtocolAnnexedFormat = GetParameter(SIGNATURE_PROTOCOL_ANNEXED_FORMAT)
                        ?? EncryptParameterToken("{0:Short}/{1:Number:0000000}/{1:Date:dd/MM/yyyy}/{1:Direction:Short}/{1:Container:Name}");
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

        public string FascicleContainerEnabled
        {
            get
            {
                if (string.IsNullOrEmpty(_fascicleContainerEnabled))
                {
                    _fascicleContainerEnabled = GetParameter(FASCICLE_CONTAINER_ENABLED) ?? EncryptParameterToken(false.ToString());
                }
                return _fascicleContainerEnabled;
            }
        }

        public string RoleContactEnabled
        {
            get
            {
                if (string.IsNullOrEmpty(_roleContactEnabled))
                {
                    _roleContactEnabled = GetParameter(ROLE_CONTACT_ENABLED) ?? EncryptParameterToken(false.ToString());
                }
                return _roleContactEnabled;
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

        public string ProcessEnabled
        {
            get
            {
                if (string.IsNullOrEmpty(_processEnabled))
                {
                    _processEnabled = GetParameter(PROCESS_ENABLED) ?? EncryptParameterToken(false.ToString());
                }
                return _processEnabled;
            }
        }

        public string ShibbolethEnabled
        {
            get
            {
                if (string.IsNullOrEmpty(_shibbolethEnabled))
                {
                    _shibbolethEnabled = GetParameter(SHIBBOLETH_ENABLED) ?? EncryptParameterToken(false.ToString());
                }
                return _shibbolethEnabled;
            }
        }

        public string ContactAOOParentId
        {
            get
            {
                if (string.IsNullOrEmpty(_contactAOOParentId))
                {
                    _contactAOOParentId = GetParameter(CONTACT_AOO_PARENT) ?? EncryptParameterToken((-1).ToString());
                }
                return _contactAOOParentId;
            }
        }

        public string SignatureTemplate
        {
            get
            {
                if (string.IsNullOrEmpty(_signatureTemplate))
                {
                    string signatureTemplateParameter = GetParameter(SIGNATURE_TEMPLATE);
                    _signatureTemplate = !string.IsNullOrEmpty(signatureTemplateParameter)
                        ? DecryptParameterToken(signatureTemplateParameter)
                        : "<Label><Text>(SIGNATURE) Pagina (pagina) di (pagine)</Text><Font Face=\"Arial\" Size=\"10\" Style=\"Bold\" /></Label>";
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(_signatureTemplate);
                    _signatureTemplate = EncryptParameterToken(xmlDocument.OuterXml);
                }
                return _signatureTemplate;
            }
        }

        public string RoleGroupPECRightEnabled
        {
            get
            {
                if (string.IsNullOrEmpty(_roleGroupPECRightEnabled))
                {
                    _roleGroupPECRightEnabled = GetParameter(ROLEGROUPPECRIGHT_ENABLED) ?? EncryptParameterToken(false.ToString());
                }
                return _roleGroupPECRightEnabled;
            }
        }

        public string ForceDescendingOrderElements
        {
            get
            {
                if (string.IsNullOrEmpty(_forceDescendingOrderElements))
                {
                    _forceDescendingOrderElements = GetParameter(FORCE_DESCENDING_ORDER_ELEMENTS) ?? EncryptParameterToken(false.ToString());
                }
                return _forceDescendingOrderElements;
            }
        }

        public string FascicleAutoCloseThresholdDays
        {
            get
            {
                if (string.IsNullOrEmpty(_fascicleAutoCloseThresholdDays))
                {
                    _fascicleAutoCloseThresholdDays = GetParameter(FASCICLE_AUTOCLOSE_THRESHOLDDAYS) ?? EncryptParameterToken(60.ToString());
                }
                return _fascicleAutoCloseThresholdDays;
            }
        }

        public string FascicleContactId
        {
            get
            {
                if (string.IsNullOrEmpty(_fascicleContactId))
                {
                    _fascicleContactId = GetParameter(FASCICLE_CONTACT_ID) ?? EncryptParameterToken((-1).ToString());
                }
                return _fascicleContactId;
            }
        }

        public string FascicleMiscellaneaLocation
        {
            get
            {
                if (string.IsNullOrEmpty(_fascicleMiscellaneaLocation))
                {
                    _fascicleMiscellaneaLocation = GetParameter(FASCICLE_MISCELLANEA_LOCATION) ?? EncryptParameterToken((-1).ToString());
                }
                return _fascicleMiscellaneaLocation;
            }
        }

        public string MultiAOOFascicleEnabled
        {
            get
            {
                if (string.IsNullOrEmpty(_multiAOOFascicleEnabled))
                {
                    _multiAOOFascicleEnabled = GetParameter(MULTI_AOO_FASCICLE_ENABLED) ?? EncryptParameterToken(false.ToString());
                }
                return _multiAOOFascicleEnabled;
            }
        }

        public string AVCPDefaultCategoryId
        {
            get
            {
                if (string.IsNullOrEmpty(_avcpDefaultCategoryId))
                {
                    _avcpDefaultCategoryId = GetParameter(AVCP_DEFAULT_CATEGORY_ID) ?? EncryptParameterToken((-1).ToString());
                }
                return _avcpDefaultCategoryId;
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

        public string AvcpDocumentSeriesId
        {
            get
            {
                if (string.IsNullOrEmpty(_avcpDocumentSeriesId))
                {
                    _avcpDocumentSeriesId = GetParameter(AVCP_DOCUMENT_SERIES_ID) ?? EncryptParameterToken((-1).ToString());
                }
                return _avcpDocumentSeriesId;
            }
        }

        public string AVCPResolutionType
        {
            get
            {
                if (string.IsNullOrEmpty(_avcpResolutionType))
                {
                    _avcpResolutionType = GetParameter(AVCP_RESOLUTION_TYPE) ?? EncryptParameterToken((-1).ToString());
                }
                return _avcpResolutionType;
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

        public string DocSuiteServiceAccounts
        {
            get
            {
                if (string.IsNullOrEmpty(_docSuiteServiceAccounts))
                {
                    _docSuiteServiceAccounts = GetParameter(DOCSUITE_SERVICE_ACCOUNTS);
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
                    _basicPersonSearcherKey = GetParameter(BASIC_PERSON_SEARCH_KEY)
                        ?? EncryptParameterToken("(&(&(objectCategory=person)(objectClass=user)(!(userAccountControl:1.2.840.113556.1.4.803:=2)))(|(cn=*{0}*)(sAMAccountName={0})))");
                }
                return _basicPersonSearcherKey;
            }
        }

        public string ForceProsecutableEnabled {
            get {
                if (string.IsNullOrEmpty(_forceProsecutableEnabled))
                {
                    _forceProsecutableEnabled = GetParameter(FORCE_PROSECUTABLE) ?? EncryptParameterToken(false.ToString());
                }
                return _forceProsecutableEnabled;
            }
        }

        public string CollaborationMailEnabled {
            get {
                if (string.IsNullOrEmpty(_collaborationMailEnabled))
                {
                    _collaborationMailEnabled = GetParameter(COLLABORATION_MAIL) ?? EncryptParameterToken(false.ToString());
                }
                return _collaborationMailEnabled;
            }
        }

        public string CollaborationSignatureType {
            get {
                if (string.IsNullOrEmpty(_collaborationSignatureType))
                {
                    _collaborationSignatureType = GetParameter(COLLABORATION_SIGNATURE_TYPE) ?? EncryptParameterToken(0.ToString());
                }
                return _collaborationSignatureType;
            }
        }

        #endregion

        #region [ Constructor ]
        public EncryptedParameterEnvService(IDataUnitOfWork unitOfWork, ILogger logger, IMapperUnitOfWork mapperUnitOfWork, IEncryptionKey encryptionKey)
            : base(unitOfWork, logger, mapperUnitOfWork, encryptionKey, useEncryption: true)
        {
        }
        #endregion

        #region [ Methods ]
        #endregion
    }
}
