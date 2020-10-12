using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.UDS;
using VecompSoftware.DocSuiteWeb.DTO.UDS;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Facade.Common.WebAPI;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Model.WebAPI.Client;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.Helpers.UDS;
using VecompSoftware.Helpers.WebAPI;
using VecompSoftware.Services.Biblos.Models;

namespace VecompSoftware.DocSuiteWeb.Facade.Common.UDS
{
    public class UDSFacade
    {
        #region [ Fields ]
        private const string CTL_TITLE = "TitleField";
        private const string CTL_HEADER = "HeaderField";
        private const string CTL_ENUM = "EnumField";
        private const string CTL_TEXT = "TextField";
        private const string CTL_NUMBER = "NumberField";
        private const string CTL_DATE = "DateField";
        private const string CTL_CHECKBOX = "BoolField";
        private const string DOCUMENTS_FIELD = "Documents";
        private const string CONTACTS_FIELD = "Contacts";
        private const string AUTHORIZATIONS_FIELD = "Authorizations";
        public const string UDS_ADDRESS_NAME = "API-UDSAddress";
        private IWebAPIHelper _webAPIHelper;
        private UDSRoleFinder _udsRoleFinder;
        private UDSContactFinder _udsContactFinder;
        private UDSMessageFinder _udsMessageFinder;
        private UDSPECMailFinder _udsPECMailFinder;
        private UDSDocumentUnitFinder _udsDocumentUnitFinder;
        private UDSCollaborationFinder _udsCollaborationFinder;
        #endregion

        #region [ Properties ]

        protected IWebAPIHelper WebAPIHelper
        {
            get
            {
                if (_webAPIHelper == null)
                {
                    _webAPIHelper = new WebAPIHelper();
                }
                return _webAPIHelper;
            }
        }


        protected UDSRoleFinder UDSRoleFinder
        {
            get
            {
                if (_udsRoleFinder == null)
                {
                    _udsRoleFinder = new UDSRoleFinder(DocSuiteContext.Current.Tenants);
                }
                return _udsRoleFinder;
            }
        }

        protected UDSContactFinder UDSContactFinder
        {
            get
            {
                if (_udsContactFinder == null)
                {
                    _udsContactFinder = new UDSContactFinder(DocSuiteContext.Current.Tenants);
                }
                return _udsContactFinder;
            }
        }

        protected UDSMessageFinder UDSMessageFinder
        {
            get
            {
                if (_udsMessageFinder == null)
                {
                    _udsMessageFinder = new UDSMessageFinder(DocSuiteContext.Current.Tenants);
                }
                return _udsMessageFinder;
            }
        }

        protected UDSPECMailFinder UDSPECMailFinder
        {
            get
            {
                if (_udsPECMailFinder == null)
                {
                    _udsPECMailFinder = new UDSPECMailFinder(DocSuiteContext.Current.Tenants);
                }
                return _udsPECMailFinder;
            }
        }

        protected UDSDocumentUnitFinder UDSDocumentUnitFinder
        {
            get
            {
                if (_udsDocumentUnitFinder == null)
                {
                    _udsDocumentUnitFinder = new UDSDocumentUnitFinder(DocSuiteContext.Current.Tenants);
                }
                return _udsDocumentUnitFinder;
            }
        }

        protected UDSCollaborationFinder UDSCollaborationFinder
        {
            get
            {
                if (_udsCollaborationFinder == null)
                {
                    _udsCollaborationFinder = new UDSCollaborationFinder(DocSuiteContext.Current.Tenants);
                }
                return _udsCollaborationFinder;
            }
        }
        #endregion

        #region [ Construcor ]
        public UDSFacade()
        {

        }
        #endregion

        #region [ Methods ]

        /// <summary>
        /// Deserializza una struttura JSON dinamica recuperata da WebAPI.
        /// </summary>
        /// <returns>oggetto UDSDto</returns>
        public UDSDto ReadUDSJson(string udsJson, Data.Entity.UDS.UDSRepository repository)
        {
            if (string.IsNullOrEmpty(udsJson))
            {
                throw new ArgumentNullException("udsJson");
            }

            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            UDSModel udsModel = UDSModel.LoadXml(repository.ModuleXML);
            string jsonParsed = ParseJson(udsJson);
            UDSEntityDto entityDto = JsonConvert.DeserializeObject<IList<UDSEntityDto>>(jsonParsed, DocSuiteContext.DefaultUDSJsonSerializerSettings).FirstOrDefault();
            if (entityDto == null)
            {
                return null;
            }
            FillBaseData(entityDto, udsModel);
            FillMetadata(jsonParsed, udsModel);
            FillDocuments(entityDto, udsModel);
            FillContacts(entityDto, udsModel);
            FillAuthorization(entityDto, udsModel);
            FillMessages(entityDto, udsModel);
            FillPECMails(entityDto, udsModel);
            FillProtocols(entityDto, udsModel);
            FillCollaborations(entityDto, udsModel);

            UDSDto dto = new UDSDto()
            {
                Id = entityDto.Id.Value,
                Year = entityDto.Year.Value,
                Number = entityDto.Number.Value,
                Status = entityDto.Status,
                CancelMotivation = entityDto.CancelMotivation,
                RegistrationDate = entityDto.RegistrationDate.Value,
                RegistrationUser = entityDto.RegistrationUser,
                LastChangedDate = entityDto.LastChangedDate,
                LastChangedUser = entityDto.LastChangedUser,
                Subject = entityDto.Subject,
                Category = entityDto.Category,
                UDSRepository = entityDto.UDSRepository,
                Authorizations = entityDto.Authorizations,
                Contacts = entityDto.Contacts,
                Documents = entityDto.Documents,
                Messages = entityDto.Messages,
                PecMails = entityDto.PecMails,
                Collaborations = entityDto.Collaborations,
                DocumentUnits = entityDto.DocumentUnits,                
                UDSModel = udsModel
            };

            return dto;
        }

        private string ParseJson(string udsJson)
        {
            JToken rootToken = JToken.Parse(udsJson);
            if (rootToken == null)
            {
                throw new Exception("errore nella deserializzazione della stringa Json");
            }

            IDictionary<string, JToken> rootProperties = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(udsJson, DocSuiteContext.DefaultUDSJsonSerializerSettings);
            if (rootProperties.Count == 0)
            {
                throw new Exception("Nessun root valido presente nella stringa Json");
            }

            return rootProperties.First().Value.ToString();
        }

        private void FillBaseData(UDSEntityDto entityDto, UDSModel model)
        {
            model.Model.Title = entityDto.UDSRepository?.Name;
            model.Model.Subject.Value = entityDto.Subject;
            model.Model.Category.IdCategory = entityDto.IdCategory?.ToString();
        }

        private void FillMetadata(string jsModel, UDSModel model)
        {
            Dictionary<string, JToken> rootElements = JsonConvert.DeserializeObject<List<Dictionary<string, JToken>>>(jsModel, DocSuiteContext.DefaultUDSJsonSerializerSettings).First();
            foreach (Section metadata in model.Model.Metadata)
            {
                foreach (FieldBaseType item in metadata.Items)
                {
                    UDSModelField udsField = new UDSModelField(item);
                    JToken selectedProperty = rootElements.ContainsKey(udsField.ColumnName) ? rootElements[udsField.ColumnName] : null;
                    if (selectedProperty == null)
                    {
                        continue;
                    }

                    udsField.Value = selectedProperty.ToObject<object>();
                }
            }
        }

        private void FillDocuments(UDSEntityDto entityDto, UDSModel model)
        {
            if (model.Model.Documents == null)
            {
                return;
            }

            ICollection<Guid> mainDocuments = entityDto.Documents.Where(x => x.DocumentType == Helpers.UDS.UDSDocumentType.Main && x.IdDocument.HasValue).Select(s => s.IdDocument.Value).ToList();
            ICollection<Guid> attachementDocuments = entityDto.Documents.Where(x => x.DocumentType == Helpers.UDS.UDSDocumentType.Attachment && x.IdDocument.HasValue).Select(s => s.IdDocument.Value).ToList();
            ICollection<Guid> annexedDocuments = entityDto.Documents.Where(x => x.DocumentType == Helpers.UDS.UDSDocumentType.Annexed && x.IdDocument.HasValue).Select(s => s.IdDocument.Value).ToList();
            ICollection<Guid> dematerialisationDocuments = entityDto.Documents.Where(x => x.DocumentType == Helpers.UDS.UDSDocumentType.Dematerialisation && x.IdDocument.HasValue).Select(s => s.IdDocument.Value).ToList();

            model.FillDocuments(mainDocuments);
            model.FillDocumentAttachments(attachementDocuments);
            model.FillDocumentAnnexed(annexedDocuments);
            model.FillDocumentDematerialisation(dematerialisationDocuments);
        }

        private void FillContacts(UDSEntityDto entityDto, UDSModel model)
        {
            if (model.Model.Contacts.IsNullOrEmpty())
            {
                return;
            }

            ICollection<WebAPIDto<UDSContact>> result = WebAPIImpersonatorFacade.ImpersonateFinder(UDSContactFinder,
                    (impersonationType, finder) =>
                    {
                        finder.ResetDecoration();
                        finder.IdUDS = entityDto.Id;
                        finder.EnablePaging = false;
                        finder.ExpandRelation = true;
                        return finder.DoSearch();
                    });

            if (result == null || !result.Select(s => s.Entity).Any())
            {
                return;
            }

            entityDto.Contacts = result.Select(s => s.Entity).Select(s => new UDSEntityContactDto()
            {
                ContactManual = s.ContactManual,
                ContactType = (Helpers.UDS.UDSContactType)s.ContactType,
                IdContact = s.Relation?.EntityId,
                UDSContactId = s.UniqueId,
                Label = s.ContactLabel
            }).ToList();

            foreach (Contacts modelContacts in model.Model.Contacts)
            {
                IList<UDSEntityContactDto> contacts = entityDto.Contacts.Where(x => x.Label.Eq(modelContacts.Label)).ToList();
                if (contacts == null || !contacts.Any())
                    continue;

                foreach (UDSEntityContactDto contact in contacts)
                {
                    if (contact.ContactType == Helpers.UDS.UDSContactType.Contact)
                    {
                        if (contact.IdContact.HasValue)
                        {
                            ContactInstance newInstance = new ContactInstance() { IdContact = contact.IdContact.Value };
                            modelContacts.ContactInstances = (modelContacts.ContactInstances ?? Enumerable.Empty<ContactInstance>()).Concat(new ContactInstance[] { newInstance }).ToArray();
                        }
                    }
                    else
                    {
                        ContactManualInstance newManualInstance = new ContactManualInstance() { ContactDescription = contact.ContactManual };
                        modelContacts.ContactManualInstances = (modelContacts.ContactManualInstances ?? Enumerable.Empty<ContactManualInstance>()).Concat(new ContactManualInstance[] { newManualInstance }).ToArray();
                    }
                }
            }
        }

        private void FillAuthorization(UDSEntityDto entityDto, UDSModel model)
        {
            ICollection<UDSRole> roles = new List<UDSRole>();

            ICollection<WebAPIDto<UDSRole>> result = WebAPIImpersonatorFacade.ImpersonateFinder(UDSRoleFinder,
                    (impersonationType, finder) =>
                    {
                        finder.ResetDecoration();
                        finder.IdUDS = entityDto.Id;
                        finder.EnablePaging = false;
                        finder.ExpandRelation = true;
                        return finder.DoSearch();
                    });

            if (result == null)
            {
                return;
            }

            roles = result.Select(f => f.Entity).ToList();

            if (roles == null || roles.Count() < 1)
            {
                return;
            }

            IEnumerable<ReferenceModel> referenceModels = roles.Select(s => new ReferenceModel() { EntityId = s.Relation.EntityShortId, UniqueId = s.UniqueId, AuthorizationType = (AuthorizationType)s.AuthorizationType});
            model.FillAuthorizations(referenceModels, model.Model.Authorizations.Label);
            entityDto.Authorizations = roles.Select(s => new UDSEntityRoleDto() { IdRole = s.Relation.EntityShortId, UniqueId = s.UniqueId, AuthorizationType = (AuthorizationType)s.AuthorizationType }).ToArray();
        }


        private void FillMessages(UDSEntityDto entityDto, UDSModel model)
        {
            ICollection<WebAPIDto<UDSMessage>> result = WebAPIImpersonatorFacade.ImpersonateFinder(UDSMessageFinder,
                    (impersonationType, finder) =>
                    {
                        finder.ResetDecoration();
                        finder.IdUDS = entityDto.Id;
                        finder.EnablePaging = false;
                        finder.ExpandRelation = true;
                        return finder.DoSearch();
                    });

            if (result == null || !result.Select(s => s.Entity).Any())
            {
                return;
            }

            ICollection<UDSMessage> messages = result.Select(s => s.Entity).ToList();
            IEnumerable<ReferenceModel> referenceModels = messages.Select(s => new ReferenceModel() { EntityId = s.Relation.EntityId, UniqueId = s.UniqueId });
            model.FillMessages(referenceModels);
            entityDto.Messages = result.Select(s => s.Entity).Select(s => new UDSEntityMessageDto() { IdMessage = s.Relation.EntityId, UniqueId = s.UniqueId }).ToArray();
        }

        private void FillPECMails(UDSEntityDto entityDto, UDSModel model)
        {
            ICollection<WebAPIDto<UDSPECMail>> result = WebAPIImpersonatorFacade.ImpersonateFinder(UDSPECMailFinder,
                    (impersonationType, finder) =>
                    {
                        finder.ResetDecoration();
                        finder.IdUDS = entityDto.Id;
                        finder.EnablePaging = false;
                        finder.ExpandRelation = true;
                        return finder.DoSearch();
                    });

            if (result == null || !result.Select(s => s.Entity).Any())
            {
                return;
            }

            ICollection<UDSPECMail> pecMails = result.Select(s => s.Entity).ToList();
            IEnumerable<ReferenceModel> referenceModels = pecMails.Select(s => new ReferenceModel() { EntityId = s.Relation.EntityId, UniqueId = s.UniqueId });
            model.FillPECMails(referenceModels);
            entityDto.PecMails = result.Select(s => s.Entity).Select(s => new UDSEntityPECMailDto() { IdPECMail = s.Relation.EntityId, UniqueId = s.UniqueId }).ToArray();
        }

        private void FillProtocols(UDSEntityDto entityDto, UDSModel model)
        {
            ICollection<WebAPIDto<UDSDocumentUnit>> result = WebAPIImpersonatorFacade.ImpersonateFinder(UDSDocumentUnitFinder,
                    (impersonationType, finder) =>
                    {
                        finder.ResetDecoration();
                        finder.IdUDS = entityDto.Id;
                        finder.EnablePaging = false;
                        finder.ExpandRelation = true;
                        finder.DocumentUnitTypes = new List<Entity.UDS.UDSRelationType>() { Entity.UDS.UDSRelationType.Protocol, Entity.UDS.UDSRelationType.ArchiveProtocol, Entity.UDS.UDSRelationType.ProtocolArchived };
                        return finder.DoSearch();
                    });

            if (result == null || !result.Select(s => s.Entity).Any())
            {
                return;
            }

            ICollection<UDSDocumentUnit> documentUnits = result.Select(s => s.Entity).ToList();
            IEnumerable<ReferenceModel> referenceModels = documentUnits.Select(s => new ReferenceModel() { UniqueId = s.Relation.UniqueId });
            model.FillProtocols(referenceModels);
            entityDto.DocumentUnits = entityDto.DocumentUnits.Concat(documentUnits.Select(s => new UDSEntityDocumentUnitDto() { UniqueId = s.Relation.UniqueId, UDSDocumentUnitId = s.UniqueId, RelationType = Model.Entities.UDS.UDSRelationType.Protocol })).ToList();
        }
        private void FillCollaborations(UDSEntityDto entityDto, UDSModel model)
        {
            ICollection<WebAPIDto<UDSCollaboration>> result = WebAPIImpersonatorFacade.ImpersonateFinder(UDSCollaborationFinder,
                    (impersonationType, finder) =>
                    {
                        finder.ResetDecoration();
                        finder.IdUDS = entityDto.Id;
                        finder.EnablePaging = false;
                        finder.ExpandRelation = true;
                        return finder.DoSearch();
                    });

            if (result == null || !result.Select(s => s.Entity).Any())
            {
                return;
            }

            ICollection<UDSCollaboration> collaborations = result.Select(s => s.Entity).ToList();
            IEnumerable<ReferenceModel> referenceModels = collaborations.Select(s => new ReferenceModel() { EntityId = s.Relation.EntityId, UniqueId = s.Relation.UniqueId });
            model.FillCollaborations(referenceModels);
            entityDto.Collaborations = result.Select(s => s.Entity).Select(s => new UDSEntityCollaborationDto() { IdCollaboration = s.Relation.EntityId, UniqueId = s.UniqueId }).ToArray();
        }

        /// <summary>
        /// Deserializza una struttura JSON di tipo UDSWorkflowModel recuperata da Workflow (property).
        /// </summary>
        /// <returns>oggetto UDSDto</returns>
        public UDSDto ReadUDSWorkflowJson(string udsWorkflowJson, Data.Entity.UDS.UDSRepository repository)
        {
            if (string.IsNullOrEmpty(udsWorkflowJson))
            {
                throw new ArgumentNullException("udsWorkflowJson");
            }

            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            UDSModel udsModel = UDSModel.LoadXml(repository.ModuleXML);
            UDSWorkflowModel model = JsonConvert.DeserializeObject<UDSWorkflowModel>(udsWorkflowJson, DocSuiteContext.DefaultWebAPIJsonSerializerSettings);
            UDSModelField udsField;
            foreach (Section metadata in udsModel.Model.Metadata)
            {
                foreach (FieldBaseType item in metadata.Items)
                {
                    udsField = new UDSModelField(item);
                    foreach (KeyValuePair<string, string> data in model.DynamicDatas)
                    {
                        if (data.Key.Eq(udsField.ColumnName))
                        {
                            udsField.Value = data.Value;
                        }
                    }
                }
            }

            if (udsModel.Model.Contacts != null && model.Contact != null)
            {
                //todo: sarà sempre e solo un singolo contatto?
                Contacts contact = udsModel.Model.Contacts.FirstOrDefault();
                if (model.Contact.HasId())
                {
                    ContactInstance newInstance = new ContactInstance() { IdContact = model.Contact.Id.Value };
                    contact.ContactInstances = (contact.ContactInstances ?? Enumerable.Empty<ContactInstance>()).Concat(new ContactInstance[] { newInstance }).ToArray();
                }
                else if (!string.IsNullOrEmpty(model.Contact.Description))
                {
                    ContactManualInstance newManualInstance = new ContactManualInstance() { ContactDescription = model.Contact.Description };
                    contact.ContactManualInstances = (contact.ContactManualInstances ?? Enumerable.Empty<ContactManualInstance>()).Concat(new ContactManualInstance[] { newManualInstance }).ToArray();
                }
            }

            UDSDto udsDto = new UDSDto()
            {
                UDSModel = udsModel
            };

            return udsDto;
        }

        public UDSDto GetUDSSource(Data.Entity.UDS.UDSRepository udsRepository, string odataFilter)
        {
            string controllerName = Utils.GetWebAPIControllerName(udsRepository.Name);
            IBaseAddress webApiAddress = DocSuiteContext.Current.CurrentTenant.WebApiClientConfig.Addresses.FirstOrDefault(x => x.AddressName.Eq(UDS_ADDRESS_NAME));
            WebApiControllerEndpoint udsEndpoint = new WebApiControllerEndpoint
            {
                AddressName = UDS_ADDRESS_NAME,
                ControllerName = controllerName,
                EndpointName = "UDSModel"
            };

            HttpClientConfiguration customHttpConfiguration = new HttpClientConfiguration();
            customHttpConfiguration.Addresses.Add(webApiAddress);
            customHttpConfiguration.EndPoints.Add(udsEndpoint);

            odataFilter = string.Concat("?", odataFilter, "&applySecurity='0'");
            string jsonSource = WebAPIImpersonatorFacade.ImpersonateRawRequest<UDSModel, string>(WebAPIHelper, odataFilter, customHttpConfiguration);
            if (string.IsNullOrEmpty(jsonSource))
            {
                return null;
            }

            return ReadUDSJson(jsonSource, udsRepository);
        }

        public static DocumentInstance[] GetDocumentInstances(ICollection<DocumentInfo> documents)
        {
            IList<DocumentInstance> documentInstances = new List<DocumentInstance>();
            if (documents.Any())
            {
                BiblosDocumentInfo documentStored = null;
                foreach (DocumentInfo document in documents)
                {
                    if (document is BiblosPdfDocumentInfo)
                    {
                        documentStored = document.ArchiveInBiblos(CommonShared.CurrentWorkflowLocation.ProtBiblosDSDB, Guid.Empty);
                        documentInstances.Add(new DocumentInstance
                        {
                            IdDocumentToStore = documentStored.DocumentId.ToString(),
                            DocumentName = document.Name,
                        });
                    }
                    else
                    {
                        documentInstances.Add(new DocumentInstance { StoredChainId = ((BiblosDocumentInfo)document).DocumentId.ToString() });
                    }
                }
            }
            return documentInstances.ToArray();
        }

        public static ContactInstance[] GetContactInstances(ICollection<ContactDTO> contacts)
        {
            IList<ContactInstance> contactInstances = new List<ContactInstance>();
            IList<ContactDTO> addressContacts = contacts.Where(x => x.Type == ContactDTO.ContactType.Address).ToList();
            foreach (ContactDTO addressContact in addressContacts)
            {
                ContactInstance instance = new ContactInstance { IdContact = addressContact.Contact.Id };
                contactInstances.Add(instance);
            }

            return contactInstances.ToArray();
        }

        public static ContactManualInstance[] GetManualContactInstances(ICollection<ContactDTO> contacts)
        {
            IList<ContactManualInstance> contactInstances = new List<ContactManualInstance>();

            IList<ContactDTO> manualContacts = contacts.Where(x => x.Type == ContactDTO.ContactType.Manual).ToList();
            foreach (ContactDTO manualContact in manualContacts)
            {
                ContactManualInstance instance = new ContactManualInstance();
                manualContact.IdManualContact = null;
                manualContact.Contact = manualContact.Contact.Duplicate();
                manualContact.Contact.UniqueId = Guid.Empty;
                instance.ContactDescription = JsonConvert.SerializeObject(manualContact, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Serialize });
                contactInstances.Add(instance);
            }

            return contactInstances.ToArray();
        }

        public static BiblosDocumentInfo[] FillUDSDocuments(Helpers.UDS.Document document)
        {
            if (document == null || document.Instances == null)
            {
                return new BiblosDocumentInfo[] { };
            }

            IList<BiblosDocumentInfo> docInfos = new List<BiblosDocumentInfo>();
            IList<BiblosDocumentInfo> bibDocs;
            foreach (DocumentInstance instance in document.Instances)
            {
                bibDocs = BiblosDocumentInfo.GetDocumentsLatestVersion(Guid.Parse(instance.StoredChainId));
                foreach (BiblosDocumentInfo doc in bibDocs)
                {
                    docInfos.Add(doc);
                }
            }
            return docInfos.ToArray();
        }

        public static IList<DocumentInfo> GetAllDocuments(UDSModel udsModel)
        {
            List<DocumentInfo> list = new List<DocumentInfo>();
            list.AddRange(FillUDSDocuments(udsModel.Model.Documents.Document));
            list.AddRange(FillUDSDocuments(udsModel.Model.Documents.DocumentAttachment));
            list.AddRange(FillUDSDocuments(udsModel.Model.Documents.DocumentAnnexed));
            return list;
        }

        public static string GetUDSMailSubject(UDSDto uds)
        {
            return String.Format("Archivio: {0} n. {1}/{2:0000000} del {3:dd/MM/yyyy} - {4}",
                                 uds.UDSRepository.Name, uds.Year, uds.Number, uds.RegistrationDate, uds.UDSModel.Model.Subject.Value);
        }

        public static string GetUDSMailBody(UDSDto uds)
        {
            string tempBody = GetUDSMailSubject(uds);
            string link = string.Format("<a href=\"{0}?Tipo=UDS&Azione=Apri&IdUDSRepository={1}&IdUDS={2}\">{3}</a>", DocSuiteContext.Current.CurrentTenant.DSWUrl, uds.UDSRepository.UniqueId, uds.Id, GetUDSMailSubject(uds));
            tempBody = string.Concat(tempBody, string.Format("<br>Contenitore: {0} <br> Oggetto: {1} <br> Classificazione: {2} <br><br> {3} <br>", uds.UDSRepository.Container.Name, uds.UDSModel.Model.Subject.Value, uds.Category.Name, link));
            return tempBody;

        }

        public static DocumentInfo GetUDSTreeDocuments(UDSDto udsSource, Action<BiblosDocumentInfo, Helpers.UDS.UDSDocumentType> documentOptionsAction)
        {
            UDSModel udsModel = udsSource.UDSModel;
            FolderInfo mainFolder = new FolderInfo() { Name = string.Concat(udsModel.Model.Title, " ", udsSource.FullNumber), ID = JsonConvert.SerializeObject(new KeyValuePair<string, Guid>(udsModel.Model.Title, udsSource.Id)) };

            // Documento principale
            BiblosDocumentInfo[] docs = FillUDSDocuments(udsModel.Model.Documents.Document);
            if (documentOptionsAction != null)
            {
                docs.ToList().ForEach(f => documentOptionsAction(f, Helpers.UDS.UDSDocumentType.Main));
            }
            
            if (docs.Length > 0)
            {
                FolderInfo folderDoc = new FolderInfo() { Name = udsModel.Model.Documents.Document.Label, Parent = mainFolder };
                folderDoc.AddChildren(docs);
            }

            // Allegati
            BiblosDocumentInfo[] attachments = FillUDSDocuments(udsModel.Model.Documents.DocumentAttachment);
            if (documentOptionsAction != null)
            {
                attachments.ToList().ForEach(f => documentOptionsAction(f, Helpers.UDS.UDSDocumentType.Attachment));
            }      
            
            if (attachments.Length > 0)
            {
                FolderInfo folderAtt = new FolderInfo() { Name = udsModel.Model.Documents.DocumentAttachment.Label, Parent = mainFolder };
                folderAtt.AddChildren(attachments);
            }

            // Allegati non parte integrante (Annessi)
            BiblosDocumentInfo[] annexes = FillUDSDocuments(udsModel.Model.Documents.DocumentAnnexed);
            if (documentOptionsAction != null)
            {
                annexes.ToList().ForEach(f => documentOptionsAction(f, Helpers.UDS.UDSDocumentType.Annexed));
            }
            
            if (annexes.Length > 0)
            {
                FolderInfo folderAnnexed = new FolderInfo() { Name = udsModel.Model.Documents.DocumentAnnexed.Label, Parent = mainFolder };
                folderAnnexed.AddChildren(annexes);
            }

            // Dematerializzazione (Attestazione di conformità)
            BiblosDocumentInfo[] dematerialisation = FillUDSDocuments(udsModel.Model.Documents.DocumentDematerialisation);
            if (documentOptionsAction != null)
            {
                dematerialisation.ToList().ForEach(f => documentOptionsAction(f, Helpers.UDS.UDSDocumentType.Dematerialisation));
            }
            
            if (dematerialisation.Length > 0)
            {
                FolderInfo folderDematerialisation = new FolderInfo() { Name = udsModel.Model.Documents.DocumentDematerialisation.Label, Parent = mainFolder };
                folderDematerialisation.AddChildren(dematerialisation);
            }

            return mainFolder;
        }
        #endregion
    }
}
