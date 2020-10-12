using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.SM.ACATA.Configuration;
using VecompSoftware.BPM.Integrations.Modules.SM.ACATA.DocumentService;
using VecompSoftware.BPM.Integrations.Modules.SM.ACATA.Helpers;
using VecompSoftware.BPM.Integrations.Modules.SM.ACATA.Models;
using VecompSoftware.BPM.Integrations.Modules.SM.ACATA.OfficialBookService;
using VecompSoftware.BPM.Integrations.Modules.SM.ACATA.Services;
using VecompSoftware.BPM.Integrations.Modules.SM.ACATA.SubjectRegistryService;
using VecompSoftware.BPM.Integrations.Services.BiblosDS;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.StampaConforme;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.Helpers.Workflow;
using VecompSoftware.Services.Command.CQRS.Events.Entities.DocumentUnits;
using BiblosDocument = VecompSoftware.BPM.Integrations.Services.BiblosDS.DocumentService.Document;
using Content = VecompSoftware.BPM.Integrations.Services.BiblosDS.DocumentService.Content;
using BiblosAttributeValue = VecompSoftware.BPM.Integrations.Services.BiblosDS.DocumentService.AttributeValue;
using System.Runtime.CompilerServices;

namespace VecompSoftware.BPM.Integrations.Modules.SM.ACATA
{

    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IServiceBusClient _serviceBusClient;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly IWebAPIClient _webApiClient;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly IList<Guid> _subscriptions = new List<Guid>();
        private readonly IDocumentClient _documentClient;
        private readonly IStampaConformeClient _stampaConformeClient;
        private readonly IDictionary<string, enumMimeTypeType> _extensionMimeTypes;
        private bool _needInitializeModule = false;

        private readonly AcarisService _acarisService;
        private readonly RepositoryService.ObjectIdType _repositoryId;
        private BackOfficeService.PrincipalIdType _principalId;
        private QueryService _queryService;
        private DocumentContentService _documentService;

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
        public EvaluationModel RetryPolicyEvaluation { get; set; }
        #endregion

        #region [ Constructor ]
        [ImportingConstructor]
        public Execution(ILogger logger, IServiceBusClient serviceBusClient, IWebAPIClient webAPIClient, IDocumentClient documentClient,
             IStampaConformeClient stampaConformeClient) : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                _serviceBusClient = serviceBusClient;
                _webApiClient = webAPIClient;
                _documentClient = documentClient;
                _stampaConformeClient = stampaConformeClient;
                _acarisService = new AcarisService(_moduleConfiguration.Endpoints, _logger, _moduleConfiguration.AcarisParameters.AppKey);
                _repositoryId = _acarisService.GetRepositoryId(_moduleConfiguration.AcarisParameters.RepositoryName);
                _extensionMimeTypes = ExtensionsMimeTypeSupported();
                _needInitializeModule = true;
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("SM.ACATA -> Critical error in construction module"), ex, LogCategories);
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
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("SM.ACATA -> Execute critical error"), ex, LogCategories);
                throw;
            }
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _subscriptions.Add(_serviceBusClient.StartListening<IEventShareDocumentUnit>(ModuleConfigurationHelper.MODULE_NAME,
                    _moduleConfiguration.TopicWorkflowIntegration, _moduleConfiguration.WorkflowStartActaShareProtocolSubscription, EventStartACTAShareCallback));
                _needInitializeModule = false;
            }
        }

        private async Task EventStartACTAShareCallback(IEventShareDocumentUnit evt, IDictionary<string, object> properties)
        {
            _logger.WriteInfo(new LogMessage($"EventStartACTAShareCallback -> received callback with event id {evt.Id}"), LogCategories);
            IdentificazioneRegistrazione registrationResult = null;

            try
            {
                DocumentUnit documentUnit = evt.ContentType.ContentValue as DocumentUnit;
                ICollection<DocSuiteWeb.Entity.Workflows.WorkflowProperty> workflowProperties = await _webApiClient.GetWorkflowPropertiesAsync(Guid.Parse(evt.CustomProperties["WorkflowActivityId"].ToString()));
                DocSuiteWeb.Entity.Workflows.WorkflowProperty tenantIdProp = workflowProperties.First(x => x.Name == WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID);
                DocSuiteWeb.Entity.Tenants.Tenant tenant = await _webApiClient.GetTenantAsync(tenantIdProp.ValueGuid.Value);

                string companyName = tenant.CompanyName;
                string registrationSubject = documentUnit.Subject;

                ICollection<DocumentUnitChain> documentUnitChains = await _webApiClient.GetDocumentUnitChainsAsync(documentUnit.UniqueId);

                DocumentUnitChain docChain = documentUnitChains.Where(x => x.ChainType == ChainType.MainChain).FirstOrDefault();
                DocumentUnitChain docChainAttachments = documentUnitChains.Where(x => x.ChainType == ChainType.AttachmentsChain).FirstOrDefault();
                DocumentUnitChain docChainAnnexed = documentUnitChains.Where(x => x.ChainType == ChainType.AnnexedChain).FirstOrDefault();

                List<BiblosDocument> mainDocumentsToAttach = await _documentClient.GetDocumentChildrenAsync(docChain.IdArchiveChain);
                List<BiblosDocument> attachmentsToAttach = docChainAttachments != null ? await _documentClient.GetDocumentChildrenAsync(docChainAttachments.IdArchiveChain) : new List<BiblosDocument>();
                List<BiblosDocument> annexedToAttach = docChainAnnexed != null ? await _documentClient.GetDocumentChildrenAsync(docChainAnnexed.IdArchiveChain) : new List<BiblosDocument>();

                IEnumerable<string> attachmentNames = attachmentsToAttach.Select(x => x.Name);
                IEnumerable<string> annexedNames = annexedToAttach.Select(x => x.Name);

                string[] attachementsAndAnnexedNames = attachmentNames.Concat(annexedNames).ToArray();

                //some attachements have media types that are not supported in acta. We will make them pdf's and we have 
                //to also change the names of the attachements/annexes that are passed in protocol definition
                string[] convertedNames = GetFileConvertedNames(attachementsAndAnnexedNames);

                if (RetryPolicyEvaluation != null && !string.IsNullOrEmpty(RetryPolicyEvaluation.ReferenceModel))
                {
                    _logger.WriteDebug(new LogMessage("Load reference model from RetryPolicyEvaluation"), LogCategories);
                    registrationResult = JsonConvert.DeserializeObject<IdentificazioneRegistrazione>(RetryPolicyEvaluation.ReferenceModel, ModuleConfigurationHelper.JsonSerializerSettings);
                }
                else
                {
                    _logger.WriteDebug(new LogMessage("Generate new RetryPolicyEvaluation model"), LogCategories);
                    RetryPolicyEvaluation = new EvaluationModel();
                }

                #region [ Create protocol registration ]

                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "REGISTRATION_CREATED"))
                {
                    _logger.WriteDebug(new LogMessage($"SM.ACATA -> Creating new protocol registration."), LogCategories);

                    registrationResult = CreateRegistration(documentUnit, registrationSubject, companyName, convertedNames);

                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = "REGISTRATION_CREATED",
                        LocalReference = JsonConvert.SerializeObject(registrationResult, ModuleConfigurationHelper.JsonSerializerSettings)
                    });
                    _logger.WriteDebug(new LogMessage("Set REGISTRATION_CREATED RetryPolicyEvaluation"), LogCategories);
                    _logger.WriteInfo(new LogMessage($"SM.ACATA -> Protocol successfully created {registrationResult.numero} {registrationResult.dataUltimoAggiornamento.value}"), LogCategories);
                }
                else
                {
                    _logger.WriteDebug(new LogMessage("Load protocol entity from RetryPolicyEvaluation REGISTRATION_CREATED"), LogCategories);
                    StepModel registrationStatus = RetryPolicyEvaluation.Steps.First(f => f.Name == "REGISTRATION_CREATED");
                    registrationResult = JsonConvert.DeserializeObject<IdentificazioneRegistrazione>(registrationStatus.LocalReference);
                }

                #endregion

                List<DocumentClassification> attachmentsList = _queryService.SearchAttachmentsRegisteredClassification(registrationResult.classificazioneId);

                #region [ Create MAIN DOCUMENT ] 

                _logger.WriteDebug(new LogMessage($"SM.ACATA -> Creating main document to registration {registrationResult.numero} {registrationResult.dataUltimoAggiornamento.value}"), LogCategories);
                ContentInfo mainDocContentInfo = await GetContentInfoAsync(mainDocumentsToAttach.First());

                DocumentStatusModel documentSigned = new DocumentStatusModel()
                {
                    StatusEfficacyId = Convert.ToInt32(_queryService.QueryObjectServiceFor("StatoDiEfficaciaDecodifica", "descrizione", ACTAHelper.STATUS_EFFICACY_ID_SIGNED, "dbKey")),
                    PhysicalDocTypeId = Convert.ToInt32(_queryService.QueryObjectServiceFor("TipoDocFisicoView", "descTipoDocFisico", ACTAHelper.PHYSICAL_DOCUMENT_TYPE_ID_SIGNED, "dbKey")),
                    DocumentCompositionId = Convert.ToInt32(_queryService.QueryObjectServiceFor("ComposizioneDocumentoView", "descComposizioneDocumento", ACTAHelper.DOCUMENT_COMPOSITION_ID, "dbKey"))
                };

                DocumentStatusModel documentUnsigned = new DocumentStatusModel
                {
                    StatusEfficacyId = Convert.ToInt32(_queryService.QueryObjectServiceFor("StatoDiEfficaciaDecodifica", "descrizione", ACTAHelper.STATUS_EFFICACY_ID_UNSIGNED, "dbKey")),
                    PhysicalDocTypeId = Convert.ToInt32(_queryService.QueryObjectServiceFor("TipoDocFisicoView", "descTipoDocFisico", ACTAHelper.PHYSICAL_DOCUMENT_TYPE_ID_UNSIGNED, "dbKey")),
                    DocumentCompositionId = Convert.ToInt32(_queryService.QueryObjectServiceFor("ComposizioneDocumentoView", "descComposizioneDocumento", ACTAHelper.DOCUMENT_COMPOSITION_ID, "dbKey"))
                };

                DocumentStatusModel dsm = DocumentSignatureFactory(mainDocContentInfo.DocumentName, documentSigned, documentUnsigned);
                _logger.WriteWarning(new LogMessage("- create main doc"), LogCategories);

                DocumentoFisicoIRC[] documents = new List<DocumentoFisicoIRC>()
                {
                    _documentService.CreatePhysicalDocument(mainDocContentInfo.DocumentName, mainDocContentInfo.Extension, mainDocContentInfo.Content,
                        mainDocContentInfo.ActaMediaType, enumStreamId.primary)
                }.ToArray();

                _documentService.TransformDocumentPlaceholder(registrationResult.classificazioneId, registrationResult.registrazioneId,
                    documents, dsm.StatusEfficacyId, dsm.PhysicalDocTypeId, dsm.DocumentCompositionId);

                _logger.WriteWarning(new LogMessage("- create main ok"), LogCategories);

                #endregion

                #region [ Create ATTACHMENTS DOCUMENTS]

                if (attachmentsToAttach.Count > 0)
                {
                    _logger.WriteDebug(new LogMessage($"SM.ACATA -> Creating attachments documents to registration {registrationResult.numero} {registrationResult.dataUltimoAggiornamento.value}"), LogCategories);

                    DocumentClassification documentClassification = null;
                    DocumentoFisicoIRC[] attachmentsDocs = new DocumentoFisicoIRC[1];
                    OfficialBookService.ObjectIdType classificationId = null;

                    foreach (BiblosDocument attachment in attachmentsToAttach)
                    {
                        ContentInfo attachementContentInfo = await GetContentInfoAsync(attachment);

                        dsm = DocumentSignatureFactory(attachment.Name, documentSigned, documentUnsigned);
                        documentClassification = attachmentsList.First(x => x.OggettoDocumento.Equals(attachment.Name));
                        classificationId = new OfficialBookService.ObjectIdType
                        {
                            value = documentClassification.ObjectIdClassificazione
                        };

                        attachmentsDocs[0] = _documentService.CreatePhysicalDocument(attachementContentInfo.DocumentName, attachementContentInfo.Extension,
                            attachementContentInfo.Content, attachementContentInfo.ActaMediaType, enumStreamId.renditionDocument);
                        _documentService.TransformDocumentPlaceholder(classificationId, registrationResult.registrazioneId, attachmentsDocs,
                            dsm.StatusEfficacyId, dsm.PhysicalDocTypeId, dsm.DocumentCompositionId);
                    }

                    _logger.WriteDebug(new LogMessage($"SM.ACATA -> Sucesfully added document attachments to registration."), LogCategories);

                }
                #endregion

                #region [ Create ANNEXED DOCUMENTS]

                if (annexedToAttach.Count > 0)
                {
                    _logger.WriteDebug(new LogMessage($"SM.ACATA -> Creating annexed documents to registration {registrationResult.numero} {registrationResult.dataUltimoAggiornamento.value}"), LogCategories);


                    DocumentoFisicoIRC[] annexedDocs = new DocumentoFisicoIRC[1];
                    DocumentClassification documentClassification = null;
                    OfficialBookService.ObjectIdType classicationId = new OfficialBookService.ObjectIdType();

                    foreach (BiblosDocument annexed in annexedToAttach)
                    {
                        ContentInfo annexedContentInfo = await GetContentInfoAsync(annexed);

                        dsm = DocumentSignatureFactory(annexed.Name, documentSigned, documentUnsigned);
                        documentClassification = attachmentsList.First(x => x.OggettoDocumento.Equals(annexed.Name));
                        classicationId.value = documentClassification.ObjectIdClassificazione;

                        annexedDocs[0] = _documentService.CreatePhysicalDocument(annexedContentInfo.DocumentName, annexedContentInfo.Extension,
                            annexedContentInfo.Content, annexedContentInfo.ActaMediaType, enumStreamId.renditionDocument);
                        _documentService.TransformDocumentPlaceholder(classicationId, registrationResult.registrazioneId, annexedDocs,
                            dsm.StatusEfficacyId, dsm.PhysicalDocTypeId, dsm.DocumentCompositionId);
                    }
                    _logger.WriteDebug(new LogMessage($"SM.ACATA -> Sucesfully added annexed documents to registration."), LogCategories);
                }
                #endregion

            }
            catch (Exception ex)
            {
                RetryPolicyEvaluation.ReferenceModel = JsonConvert.SerializeObject(registrationResult, ModuleConfigurationHelper.JsonSerializerSettings);
                _logger.WriteError(ex, LogCategories);
                _logger.WriteError(new LogMessage(ex.StackTrace), LogCategories);
                throw new ServiceBusEvaluationException(RetryPolicyEvaluation);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetExtension(string documentName)
        {
            return Path.GetExtension(documentName);
        }

        private DocumentStatusModel DocumentSignatureFactory(string documentName, DocumentStatusModel documentSigned, DocumentStatusModel documentUnsigned)
        {
            if (!Path.GetExtension(documentName).Equals(".p7m"))
            {
                return documentUnsigned;
            }
            return documentSigned;
        }

        private IdentificazioneRegistrazione CreateRegistration(DocumentUnit documentUnit, string registrationSubject, string companyName, string[] attachments)
        {
            //principal id is expiring
            _principalId = _acarisService.GetPrincipalIdType(_repositoryId, _moduleConfiguration.AcarisParameters.IdAOO, _moduleConfiguration.AcarisParameters.FiscalCode, _moduleConfiguration.AcarisParameters.IdStructure, _moduleConfiguration.AcarisParameters.IdNode); ;
            _queryService = new QueryService(_acarisService, _repositoryId, _principalId, _logger);
            _documentService = new DocumentContentService(_acarisService, _repositoryId, _principalId, _logger);

            // type of registration: incoming
            RegistrazioneArrivo incomingRegistration = new RegistrazioneArrivo
            {
                tipoRegistrazione = enumTipoAPI.Arrivo,
            };

            // oggetto - contains all the data to set up logging
            InfoCreazioneRegistrazione registrationData = new InfoCreazioneRegistrazione
            {
                forzareSePresenzaInviti = true,
                forzareSePresenzaDaInoltrare = true,
                forzareSeRegistrazioneSimile = true,
                descrizioneAllegato = attachments
            };
            // enhancement of registration - protocol
            OfficialBookService.ObjectIdType objectIdStructure = new OfficialBookService.ObjectIdType
            {
                value = _queryService.QueryBackofficeServiceFor(DocumentService.enumObjectType.StrutturaPropertiesType.ToString(), "dbKey", _moduleConfiguration.AcarisParameters.IdStructure.ToString(), "objectId")
            };

            OfficialBookService.ObjectIdType objectIdNode = new OfficialBookService.ObjectIdType
            {
                value = _queryService.QueryBackofficeServiceFor(DocumentService.enumObjectType.NodoPropertiesType.ToString(), "dbKey", _moduleConfiguration.AcarisParameters.IdNode.ToString(), "objectId")
            };

            IdentificazioneProtocollante idProtocol = new IdentificazioneProtocollante()
            {
                strutturaId = objectIdStructure,
                nodoId = objectIdNode
            };

            registrationData.protocollante = idProtocol;

            registrationData.oggetto = registrationSubject;

            // SENDER - external sender: legal entity (external AOO)
            InfoComuniCreazioneSoggetto createSubject = new InfoComuniCreazioneSoggetto()
            {
                codiceTipoSoggetto = SubjectRegistryService.enumPFPGUL.PG,
                denominazione = companyName,
                forzareSePresentiSoggettiSimili = true
            };

            SoggettoDefinitivo definitiveSubject = new SoggettoDefinitivo()
            {
                dataFineValidita = null,
                infoComuniCreazioneSoggetto = createSubject
            };

            string actaSubjectValue = _queryService.QuerySubjectRegistryServiceFor(enumRegistryObjectType.SoggettoPropertiesType.ToString(), "denominazione", companyName, "objectId");

            if (string.IsNullOrWhiteSpace(actaSubjectValue))
            {
                _logger.WriteInfo(new LogMessage($"SM.ACATA -> Protocol registration contact not found. Creating process initiated."), LogCategories);
                createSubject.idTipoSoggettoAppartenenza = _queryService.QuerySubjectMembershipId(_repositoryId, _principalId, _moduleConfiguration.AcarisParameters.InstitutionCode);
                actaSubjectValue = _acarisService.CreateSubject(_repositoryId, _principalId, definitiveSubject);
            }

            OfficialBookService.ObjectIdType actaSubject = new OfficialBookService.ObjectIdType
            {
                value = actaSubjectValue
            };

            RiferimentoSoggettoEsistente senderSubject = new RiferimentoSoggettoEsistente
            {
                soggettoId = actaSubject,
                idPFPGUL = OfficialBookService.enumPFPGUL.PG,
                tipologia = enumTipologiaSoggettoAssociato.SoggettoActa
            };

            InfoCreazioneCorrispondente correspondingSender = new InfoCreazioneCorrispondente()
            {
                denominazione = companyName,
                infoSoggettoAssociato = senderSubject
            };

            List<MittenteEsterno> externalSenders = new List<MittenteEsterno>()
            {
                new MittenteEsterno()
                {
                    corrispondente = correspondingSender
                }
            };
            incomingRegistration.mittenteEsterno = externalSenders.ToArray();

            //RECEIVER
            RiferimentoSoggettoEsistente receiverSubject = new RiferimentoSoggettoEsistente
            {
                tipologia = enumTipologiaSoggettoAssociato.Nodo,
                soggettoId = _queryService.QueryNodeByCode(_moduleConfiguration.AcarisParameters.DestinatarioInternoCodiceNodo)
            };

            InfoCreazioneCorrispondente correspondingReceiver = new InfoCreazioneCorrispondente()
            {
                infoSoggettoAssociato = receiverSubject,
                denominazione = _moduleConfiguration.AcarisParameters.DestinatarioInternoDescrizioneNodo
            };

            List<DestinatarioInterno> internalReceivers = new List<DestinatarioInterno>()
            {
                new DestinatarioInterno()
                {
                    corrispondente = correspondingReceiver,
                    idRuoloCorrispondente = 1 // 1 - per competenza; 2 - per quanto di competenza; 3 - per conoscenza
                }
            };

            registrationData.destinatarioInterno = internalReceivers.ToArray();
            incomingRegistration.infoCreazione = registrationData;
            incomingRegistration.infoProtocolloMittente = new InfoProtocolloMittente()
            {
                anno = documentUnit.Year.ToString(),
                numero = documentUnit.Number.ToString(),
                data = documentUnit.RegistrationDate.UtcDateTime
            };

            enumTipoRegistrazioneDaCreare typologyCreation = enumTipoRegistrazioneDaCreare.Protocollazione;

            OfficialBookService.ObjectIdType objectIdAoo = new OfficialBookService.ObjectIdType()
            {
                value = _queryService.QueryBackofficeServiceFor(DocumentService.enumObjectType.AOOPropertiesType.ToString(), "dbKey", _moduleConfiguration.AcarisParameters.IdAOO.ToString(), "objectId")
            };

            Protocollazione protocol = new Protocollazione
            {
                aooProtocollanteId = objectIdAoo,
                senzaCreazioneSoggettiEsterni = true,
                registrazioneAPI = incomingRegistration
            };

            try
            {
                return _acarisService.CreateRegistration(_repositoryId, _principalId, typologyCreation, protocol);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("SM.ACATA -> Error creating a protocol"), ex, LogCategories);
                throw;
            }
        }

        private string[] GetFileConvertedNames(string[] names)
        {
            List<string> convertedFileNames = new List<string>();

            foreach (string fileName in names)
            {
                string extension = GetExtension(fileName);
                bool hasMimeType = _extensionMimeTypes.TryGetValue(extension, out enumMimeTypeType originalMediaType);

                if (!hasMimeType)
                {
                    string pdfName = CreateConvertedPdfName(fileName, extension);

                    convertedFileNames.Add(pdfName);
                }
                else
                {
                    convertedFileNames.Add(fileName);
                }
            }

            return convertedFileNames.ToArray();
        }

        private async Task<ContentInfo> GetContentInfoAsync(BiblosDocument biblosDocument)
        {
            if (biblosDocument is null)
            {
                throw new ArgumentNullException(nameof(biblosDocument));
            }

            string extension = GetExtension(biblosDocument.Name);
            bool hasMimeType = _extensionMimeTypes.TryGetValue(extension, out enumMimeTypeType originalMediaType);
          
            Content contentStream = await _documentClient.GetDocumentContentByIdAsync(biblosDocument.IdDocument);
            BiblosAttributeValue signatureAttribute = biblosDocument.AttributeValues.First(x => x.Attribute.Name == _documentClient.ATTRIBUTE_SIGNATURE);

            _logger.WriteInfo(new LogMessage((signatureAttribute is null).ToString()), LogCategories);
            _logger.WriteInfo(new LogMessage(signatureAttribute.Value.ToString()), LogCategories);

            if (!hasMimeType)
            {
                //if the document does not have a media type allowed for acta, we convert it to a pdf
                string pdfName = CreateConvertedPdfName(biblosDocument.Name, extension);

                _logger.WriteInfo(new LogMessage($"Converting document \'{biblosDocument.Name}\' to {pdfName}."), LogCategories);

                byte[] contentPDFA = await _stampaConformeClient.ConvertToPDFAAsync(
                   source: contentStream.Blob,
                   fileExtension: extension,
                   signature: signatureAttribute.Value.ToString()).ConfigureAwait(false);

                return new ContentInfo
                {
                    ActaMediaType = enumMimeTypeType.applicationpdf,
                    Content = contentPDFA,
                    Extension = ".pdf",
                    DocumentName = pdfName
                };
            }
            else
            {
                _logger.WriteInfo(new LogMessage($"Returning original content for \'{biblosDocument.Name}\'. "), LogCategories);

                return new ContentInfo
                {
                    ActaMediaType = originalMediaType,
                    Extension = extension,
                    Content = contentStream.Blob,
                    DocumentName = biblosDocument.Name
                };
            }
        }

        private static string CreateConvertedPdfName(string fileName, string extension)
        {
            if (fileName.EndsWith(extension))
            {
                fileName = fileName.Substring(0, fileName.Length - extension.Length);
            }

            string pdfName = $"CC_{fileName}.pdf";
            return pdfName;
        }

        private Dictionary<string, enumMimeTypeType> ExtensionsMimeTypeSupported()
        {
            return new Dictionary<string, enumMimeTypeType>()
                {
                    [".pdf"] = enumMimeTypeType.applicationpdf 
                };
        }

        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> SM.ACATA"), LogCategories);
        }

        private void CleanSubscriptions()
        {
            foreach (Guid item in _subscriptions)
            {
                _serviceBusClient.CloseListeningAsync(item).Wait();
            }
            _subscriptions.Clear();
            _needInitializeModule = true;
        }

        #endregion
    }
}
