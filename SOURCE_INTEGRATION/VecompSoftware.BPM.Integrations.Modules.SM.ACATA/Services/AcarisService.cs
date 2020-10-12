using System;
using System.Collections.Generic;
using VecompSoftware.BPM.Integrations.Modules.SM.ACATA.Adaptors;
using VecompSoftware.BPM.Integrations.Modules.SM.ACATA.BackOfficeService;
using VecompSoftware.BPM.Integrations.Modules.SM.ACATA.DocumentService;
using VecompSoftware.BPM.Integrations.Modules.SM.ACATA.Models;
using VecompSoftware.BPM.Integrations.Modules.SM.ACATA.OfficialBookService;
using VecompSoftware.BPM.Integrations.Modules.SM.ACATA.SubjectRegistryService;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.BPM.Integrations.Modules.SM.ACATA.Services
{
    public class AcarisService
    {
        #region [ Fields ]
        private readonly RepositoryService.RepositoryService _repositoryService;
        private readonly BackOfficeService.BackOfficeService _backOfficeService;
        private readonly OfficialBookService.OfficialBookService _officialBookService;
        private readonly SubjectRegistryService.SubjectRegistryService _subjectRegistryService;
        private readonly ObjectService.ObjectService _objectService;
        private readonly DocumentService.DocumentService _documentService;
        private readonly ILogger _logger;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly string _smartKey = string.Empty;
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
        public AcarisService(Endpoints endpoints, ILogger logger, string smartKey)
        {
            _repositoryService = new RepositoryService.RepositoryService() { Url = endpoints.RepositoryWS };
            _backOfficeService = new BackOfficeService.BackOfficeService() { Url = endpoints.BackOfficeWS };
            _officialBookService = new OfficialBookService.OfficialBookService() { Url = endpoints.OfficialBookWS };
            _subjectRegistryService = new SubjectRegistryService.SubjectRegistryService() { Url = endpoints.SubjectRegistryWS };
            _objectService = new ObjectService.ObjectService() { Url = endpoints.ObjectWS };
            _documentService = new DocumentService.DocumentService() { Url = endpoints.DocumentWS };
            _logger = logger;
            _smartKey = smartKey;
        }
        #endregion

        #region [ Methods ]
        public RepositoryService.ObjectIdType GetRepositoryId(string repositoryName)
        {
            RepositoryService.ObjectIdType repositoryId = null;
            RepositoryService.acarisRepositoryEntryType[] repEntries;
            try
            {
                repEntries = _repositoryService.getRepositories(new RepositoryService.getRepositories());
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("[SM.ACATA] --> Fatal application exception"), ex, LogCategories);
                throw;
            }
            foreach (RepositoryService.acarisRepositoryEntryType entry in repEntries)
            {
                if (entry.repositoryName != null && entry.repositoryName.Contains(repositoryName))
                {
                    repositoryId = entry.repositoryId;
                    break;
                }
            }
            if (repositoryId == null)
            {
                _logger.WriteError(new LogMessage("[SM.ACATA] --> Error in loading the repository id"), LogCategories);
                throw new Exception("Error in loading the repository id");
            }
            return repositoryId;
        }
        public BackOfficeService.PrincipalIdType GetPrincipalIdType(RepositoryService.ObjectIdType repositoryId, long idAOO, string fiscalCode, long idStructure, long idNode)
        {
            BackOfficeService.IdAOOType aoo = new BackOfficeService.IdAOOType() { value = idAOO };
            BackOfficeService.CodiceFiscaleType fc = new BackOfficeService.CodiceFiscaleType { value = fiscalCode };

            BackOfficeService.PrincipalIdType principalId;
            try
            {
                if (!string.IsNullOrEmpty(_smartKey))
                {
                    BackOfficeService.IdStrutturaType idStructureType = new BackOfficeService.IdStrutturaType { value = idStructure };
                    BackOfficeService.IdNodoType node = new BackOfficeService.IdNodoType { value = idNode };
                    ClientApplicationInfo cai = new ClientApplicationInfo { appKey = _smartKey };
                    getPrincipalExt principal = new getPrincipalExt()
                    {
                        idAOO = aoo,
                        repositoryId = repositoryId.ConvertToBackOfficeService(),
                        idUtente = fc,
                        idNodo = node,
                        idStruttura = idStructureType,
                        clientApplicationInfo = cai
                    };
                    PrincipalExtResponseType[] principalArr = _backOfficeService.getPrincipalExt(principal);
                    principalId = principalArr[0].principalId;
                }
                else
                {
                    getPrincipal principal = new getPrincipal()
                    {
                        repositoryId = repositoryId.ConvertToBackOfficeService(),
                        idAOO = aoo,
                        idUtente = fc
                    };
                    PrincipalResponseType[] principalArr = _backOfficeService.getPrincipal(principal);
                    principalId = principalArr[0].idPrincipal;
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("[SM.ACATA] --> Error in loading the principal id "), ex, LogCategories);
                throw;
            }
            return principalId;
        }
        public IdentificazioneRegistrazione CreateRegistration(RepositoryService.ObjectIdType repositoryId, BackOfficeService.PrincipalIdType principalId, enumTipoRegistrazioneDaCreare typology, Protocollazione protocol)
        {
            creaRegistrazione createRegistration = new creaRegistrazione()
            {
                tipologiaCreazione = typology,
                principalId = principalId.ConvertToOfficialBookService(),
                repositoryId = repositoryId.ConvertToOfficialBookService(),
                infoRichiestaCreazione = protocol
            };
            creaRegistrazioneResponse result = _officialBookService.creaRegistrazione(createRegistration);

            return result.identificazioneCreazione;
        }
        public BackOfficeService.PagingResponseType QuerySubjectRegistry(RepositoryService.ObjectIdType repositoryId, BackOfficeService.PrincipalIdType principalId, SubjectRegistryService.QueryableObjectType target,
          SubjectRegistryService.PropertyFilterType filter, SubjectRegistryService.QueryConditionType[] criteria, SubjectRegistryService.NavigationConditionInfoType navigationConditionInfo = null,
          int maxItems = 0, bool maxItemsField = false, int skipCount = 0, bool skipCountField = false)
        {
            SubjectRegistryService.query subjectRegistryService = new SubjectRegistryService.query()
            {
                repositoryId = repositoryId.ConvertToSubjectRegistryService(),
                principalId = principalId.ConvertToSubjectRegistryService(),
                target = target,
                filter = filter,
                criteria = criteria
            };
            SubjectRegistryService.queryResponse response = _subjectRegistryService.query(subjectRegistryService);
            if (response.@object != null && response.@object.objects != null && response.@object.objects.Length > 0)
            {
                return response.ConvertSubjectToBackOfficeResponseType();
            }
            return null;
        }
        public BackOfficeService.PagingResponseType Query(RepositoryService.ObjectIdType repositoryId, BackOfficeService.PrincipalIdType principalId, BackOfficeService.QueryableObjectType target,
            BackOfficeService.PropertyFilterType filter, BackOfficeService.QueryConditionType[] criteria, BackOfficeService.NavigationConditionInfoType navigationConditionInfoType,
            int maxItemsField = 0, bool maxItemsFieldSpecified = false, int skipCountField = 0, bool skipCountFieldSpecified = false)
        {
            BackOfficeService.query query = new BackOfficeService.query()
            {
                repositoryId = repositoryId.ConvertToBackOfficeService(),
                principalId = principalId,
                target = target,
                filter = filter,
                criteria = criteria
            };
            BackOfficeService.queryResponse response = _backOfficeService.query(query);

            return response.@object;
        }
        public BackOfficeService.PagingResponseType Query(RepositoryService.ObjectIdType repositoryId, BackOfficeService.PrincipalIdType principalId, ObjectService.QueryableObjectType target,
           ObjectService.PropertyFilterType filter, ObjectService.QueryConditionType[] criteria, ObjectService.NavigationConditionInfoType navigationConditionInfoType,
           int maxItemsField = 0, bool maxItemsFieldSpecified = false, int skipCountField = 0, bool skipCountFieldSpecified = false)
        {
            ObjectService.query query = new ObjectService.query()
            {
                repositoryId = repositoryId.ConvertToObjectService(),
                principalId = principalId.ConvertToObjectService(),
                target = target,
                filter = filter,
                criteria = criteria
            };
            ObjectService.queryResponse response = _objectService.query(query);

            return response.ConvertObjectToBackOfficeResponseType();
        }

        public string CreateSubject(RepositoryService.ObjectIdType repositoryId, BackOfficeService.PrincipalIdType principalId, SoggettoDefinitivo definitiveSubject)
        {
            creaSoggetto createSubject = new creaSoggetto()
            {
                tipologiaCreazione = enumTipologiaCreazioneSoggetto.SoggettoDefinitivo,
                principalId = principalId.ConvertToSubjectRegistryService(),
                repositoryId = repositoryId.ConvertToSubjectRegistryService(),
                infoRichiestaCreazione = definitiveSubject
            };

            creaSoggettoResponse response = _subjectRegistryService.creaSoggetto(createSubject);
            if (response == null)
            {
                _logger.WriteError(new LogMessage("[SM.ACATA] --> Error creating a subject"), LogCategories);
                throw new Exception("[SM.ACATA] --> Couldn't create a subject!");
            }
            return response.identificazioneSoggetto.soggettoId.value;
        }

        public IdentificazioneTrasformazione[] TransformDocumentPlaceholderInElectronicDocument(RepositoryService.ObjectIdType repositoryId, BackOfficeService.PrincipalIdType principalId, DocumentService.ObjectIdType classificationId,
            DocumentService.ObjectIdType regProtId, InfoRichiestaTrasformazione requestTransformationInfo, DocumentoFisicoIRC[] documents)
        {
            trasformaDocumentoPlaceholderInDocumentoElettronico tdpide = new trasformaDocumentoPlaceholderInDocumentoElettronico()
            {
                repositoryId = repositoryId.ConvertToDocumentService(),
                principalId = principalId.ConvertToDocumentService(),
                classificazioneId = classificationId,
                infoRichiesta = requestTransformationInfo,
                registrazioneId = regProtId,
                documentoFisico = documents
            };
            IdentificazioneTrasformazione[] response = _documentService.trasformaDocumentoPlaceholderInDocumentoElettronico(tdpide);

            return response;
        }

        public BackOfficeService.PagingResponseType QueryObjectService(RepositoryService.ObjectIdType repositoryId, BackOfficeService.PrincipalIdType principalId, ObjectService.QueryableObjectType target, ObjectService.PropertyFilterType filter, ObjectService.QueryConditionType[] criteria, ObjectService.NavigationConditionInfoType navigationConditionInfoType, int maxItems)
        {
            ObjectService.query q = new ObjectService.query()
            {
                repositoryId = repositoryId.ConvertToObjectService(),
                principalId = principalId.ConvertToObjectService(),
                filter = filter,
                criteria = criteria,
                maxItems = maxItems,
                target = target
            };

            ObjectService.queryResponse response = _objectService.query(q);

            if (response != null && response.@object != null && response.@object.objects != null)
            {
                return response.ConvertObjectToBackOfficeResponseType();
            }
            return null;
        }
        #endregion
    }
}
