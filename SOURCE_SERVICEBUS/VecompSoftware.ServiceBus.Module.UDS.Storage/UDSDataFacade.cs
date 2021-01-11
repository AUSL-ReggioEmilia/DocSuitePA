using PetaPoco;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.Helpers.UDS;
using VecompSoftware.Helpers.UDS.Exceptions;
using VecompSoftware.ServiceBus.BiblosDS;
using VecompSoftware.ServiceBus.Module.UDS.Storage.Relations;

namespace VecompSoftware.ServiceBus.Module.UDS.Storage
{
    /// <summary>
    /// Offre le funzioni di CRUD sulle entità di storage create per ciascun tipo di UDS
    /// </summary>
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public class UDSDataFacade
    {

        #region [ Fields ]
        private const int _retry_tentative = 5;
        private readonly TimeSpan _threadWaiting = TimeSpan.FromSeconds(2);

        private const string UDS_LOG_INSERT = "Inserimento";
        private const string UDS_LOG_MODIFY = "Modifica";
        private const string UDS_LOG_OBJECT_MODIFY = "Modifica oggetto";
        private const string UDS_LOG_DELETE = "Annullamento Archivio: {0} n. {1}/{2:0000000}";
        private const string UDS_INFO = " Archivio: {0} n. {1}";

        private static readonly string _q_update_change_year = string.Concat(
            "UPDATE {0}.[", UDSTableBuilder.UDSRepositoriesTableName, "] ",
            "SET [", UDSTableBuilder.UDSRepositoriesSequenceCurrentNumberField, "] = 0, ",
            "    [", UDSTableBuilder.UDSRepositoriesSequenceCurrentYearField, "] = {1} ",
            "WHERE [", UDSTableBuilder.UDSRepositoriesPK, "] = '{2}'");

        private static readonly string _q_set_next_sequenceNumber = string.Concat(
            "UPDATE {0}.[", UDSTableBuilder.UDSRepositoriesTableName, "] ",
            "SET [", UDSTableBuilder.UDSRepositoriesSequenceCurrentNumberField, "] = ",
            "    [", UDSTableBuilder.UDSRepositoriesSequenceCurrentNumberField, "] + 1 ",
            "WHERE [", UDSTableBuilder.UDSRepositoriesPK, "] = '{1}'");

        private static readonly string _q_rollback_sequenceNumber = string.Concat(
            "UPDATE {0}.[", UDSTableBuilder.UDSRepositoriesTableName,
            "] SET [", UDSTableBuilder.UDSRepositoriesSequenceCurrentNumberField,
            "] = [", UDSTableBuilder.UDSRepositoriesSequenceCurrentNumberField,
            "] - 1 WHERE [", UDSTableBuilder.UDSRepositoriesPK,
            "] = '{1}'");

        private static readonly string _q_get_sequenceNumber = string.Concat(
            "SELECT TOP 1 [", UDSTableBuilder.UDSRepositoriesSequenceCurrentNumberField,
            "] FROM {0}.[", UDSTableBuilder.UDSRepositoriesTableName,
            "] WHERE [", UDSTableBuilder.UDSRepositoriesPK,
            "] = '{1}'");

        private static readonly string _q_get_sequenceYear = string.Concat(
            "SELECT TOP 1 [", UDSTableBuilder.UDSRepositoriesSequenceCurrentYearField,
            "] FROM {0}.[", UDSTableBuilder.UDSRepositoriesTableName,
            "] WHERE [", UDSTableBuilder.UDSRepositoriesPK,
            "] = '{1}'");

        private static readonly string _q_get_uds = "SELECT TOP 1 * FROM {0}.{1} WHERE UDSId = '{2}'";

        private static readonly string _q_count_uds = "SELECT SUM(1) FROM {0}.{1} WHERE UDSId = '{2}'";

        private static readonly string _q_get_udsYearNumber = "SELECT TOP 1 _year as Year, _number as Number, _subject As Subject, RegistrationDate FROM {0}.{1} WHERE UDSId = '{2}'";

        private readonly UDSModel _uds;
        private readonly UDSTableBuilder _builder;
        private readonly ILogger _logger;
        private readonly BiblosDS.BiblosDS.DocumentsClient _biblosDocumentClient;
        private readonly BiblosDS.BiblosDSManagement.AdministrationClient _biblosAdministratorClient;

        protected static IEnumerable<LogCategory> _logCategories = null;

        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(UDSDataFacade));
                }
                return _logCategories;
            }
        }

        public UDSTableBuilder Builder => _builder;

        #endregion

        #region [ Constructor ]
        public UDSDataFacade(ILogger logger, BiblosClient biblosClient, string xml, string xmlSchema, string dbSchema = "dbo")
        {
            bool validate = UDSModel.ValidateXml(xml, xmlSchema, out List<string> validationErrors);
            if (!validate)
            {
                throw new UDSDataException(string.Format("UdsDataFacade - Errori di validazione Xml: {0}", string.Join("\n", validationErrors)));
            }

            _uds = UDSModel.LoadXml(xml);
            _builder = new UDSTableBuilder(_uds, dbSchema);
            _logger = logger;
            _biblosDocumentClient = biblosClient.Document;
            _biblosAdministratorClient = biblosClient.Administration;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Inserisce nel DB una nuova Uds e tutte le relazioni contenuto nel file Xml
        /// </summary>
        /// <param name="connStr">String di connessione</param>
        /// <param name="commitHook">Function Delegate che consente passare una funzione esterna da eseguire prima del commit su DB. Se ritorna False viene fatto rollback della transazione</param>
        /// <returns>Ritorna UDSId primary key della Uds inserita</returns>
        public UDSEntityModel AddUDS(string connStr, Guid IdUDSRepository, string UDSRepositoryName, IEnumerable<AuthorizationInstance> buildModelRoles, string userName, DateTimeOffset creationTime, Func<bool, bool> commitHook = null, short? year = null, int? number = null)
        {
            try
            {
                //validazione
                bool validate = _uds.ValidateValues(out List<string> validationErrors);
                if (!validate)
                {
                    throw new UDSDataException(string.Format("AddUds - Errore di validazione dei valori: {0}", string.Join("\n", validationErrors)));
                }

                //costruisce l'oggetto dinamico
                IDictionary<string, object> metadatas = new ExpandoObject() as IDictionary<string, object>;
                Guid udsId = Guid.NewGuid();
                if (!string.IsNullOrEmpty(_uds.Model.UDSId) && Guid.TryParse(_uds.Model.UDSId, out udsId))
                {
                    _logger.WriteDebug(new LogMessage($"UniqueId {udsId} has been readed from UDSModel"), LogCategories);
                }
                //dati statici
                metadatas.Add(UDSTableBuilder.UDSPK, udsId);
                metadatas.Add(UDSTableBuilder.UDSRepositoryFK, IdUDSRepository);
                metadatas.Add(UDSTableBuilder.UDSRegistrationUserField, userName);
                metadatas.Add(UDSTableBuilder.UDSRegistrationDateField, DateTimeOffset.UtcNow);
                metadatas.Add(UDSTableBuilder.UDSSubjectField, _uds.Model.Subject.Value);
                metadatas.Add(UDSTableBuilder.UDSIdCategoryFK, _uds.Model.Category.IdCategory);
                metadatas.Add(UDSTableBuilder.UDSStatusField, 1);

                string valueSignature = string.Empty;
                object value;
                UDSModelField uField;
                NumberField numberField;
                //dati della UDS
                foreach (Section section in _uds.Model.Metadata)
                {
                    foreach (FieldBaseType field in section.Items)
                    {
                        uField = new UDSModelField(field);
                        if (!(field is DateField) ||
                            (field is DateField && !string.IsNullOrEmpty(uField.Value as string) && !DateTime.MinValue.ToString().Equals(uField.Value as string)))
                        {
                            value = uField.Value;
                            if (field is NumberField)
                            {
                                numberField = field as NumberField;
                                value = null;
                                if (numberField.ValueSpecified)
                                {
                                    value = numberField.Value;
                                };
                            }

                            _logger.WriteDebug(new LogMessage(string.Concat("Filling database column ", uField.ColumnName, " with value ", value)), LogCategories);
                            metadatas.Add(uField.ColumnName, value);
                        }

                        if (_uds.Model.SignatureMetadataEnabled && (field is TextField) && (field as TextField).IsSignature)
                        {
                            valueSignature = uField.Value.ToString();
                        }
                    }
                }

                using (Database db = new Database(connStr))
                using (ITransaction tr = db.GetTransaction())
                {
                    int? alreadyExistingUDS = db.Fetch<int?>(string.Format(_q_count_uds, Builder.DbSchema, Builder.UDSTableName, udsId)).SingleOrDefault();
                    if (alreadyExistingUDS.HasValue && alreadyExistingUDS.Value > 0)
                    {
                        throw new UDSDataException($"AddUds - UniqueId {udsId} already exist in {Builder.UDSTableName}");
                    }
                    int i = db.UpdateROW(string.Format(_q_set_next_sequenceNumber, Builder.DbSchema, IdUDSRepository));
                    short currentYear;
                    int currentNumber;

                    if (_uds.Model.IncrementalIdentityEnabled)
                    {
                        currentYear = db.Single<short>(string.Format(_q_get_sequenceYear, Builder.DbSchema, IdUDSRepository));
                        if (currentYear < DateTime.Now.Year)
                        {
                            i = db.UpdateROW(string.Format(_q_update_change_year, Builder.DbSchema, DateTime.Now.Year, IdUDSRepository));
                            _logger.WriteInfo(new LogMessage($"Automatically changed year from {currentYear} to {DateTime.Now.Year}"), LogCategories);
                            currentYear = (short)DateTime.Now.Year;
                            i = db.UpdateROW(string.Format(_q_set_next_sequenceNumber, Builder.DbSchema, IdUDSRepository));
                        }
                        currentNumber = db.Single<int>(string.Format(_q_get_sequenceNumber, Builder.DbSchema, IdUDSRepository));
                    }
                    else
                    {
                        if (!year.HasValue || !number.HasValue)
                        {
                            throw new ArgumentException("Modello non valido. Valori anno e numero non passati");
                        }

                        currentYear = year.Value;
                        currentNumber = number.Value;
                    }

                    _logger.WriteInfo(new LogMessage(string.Concat("inserting new UDS ", _uds.Model.Title, " - ", currentYear, "/", currentNumber.ToString("0000000"))), LogCategories);
                    metadatas.Add(UDSTableBuilder.UDSYearField, currentYear);
                    metadatas.Add(UDSTableBuilder.UDSNumberField, currentNumber);

                    if (_uds.Model.Documents != null)
                    {
                        try
                        {
                            _uds.Model = ArchiveBiblosDocuments(_uds.Model, metadatas, UDSRepositoryName, signature: valueSignature);
                        }
                        catch (Exception ex)
                        {
                            int y = db.UpdateROW(string.Format(_q_rollback_sequenceNumber, Builder.DbSchema, IdUDSRepository));
                            _logger.WriteError(ex, LogCategories);
                            throw ex;
                        }
                    }

                    udsId = (Guid)db.Insert(_builder.DbSchema, _builder.UDSTableName, UDSTableBuilder.UDSPK, false, metadatas);

                    UDSEntityModel model = new UDSEntityModel
                    {
                        IdUDS = udsId,
                        Year = currentYear,
                        Number = currentNumber,
                        RegistrationDate = DateTimeOffset.UtcNow,
                        Subject = _uds.Model.Subject.Value,
                        Title = _uds.Model.Title,
                        IdCategory = Convert.ToInt16(_uds.Model.Category.IdCategory),
                        IdContainer = Convert.ToInt16(_uds.Model.Container.IdContainer),
                        DocumentUnitSynchronizeEnabled = _uds.Model.DocumentUnitSynchronizeEnabled
                    };

                    UDSRelationFacade relation = new UDSRelationFacade(_uds, Builder.DbSchema);
                    model.Relations = new UDSRelations
                    {
                        Documents = relation.AddDocuments(db, udsId),
                        Contacts = relation.AddContacts(udsId),
                        Authorizations = relation.AddAuthorizations(udsId, buildModelRoles),
                        Messages = relation.AddMessages(udsId),
                        PECMails = relation.AddPECMails(udsId),
                        Protocols = relation.AddProtocols(udsId),
                        Resolutions = relation.AddResolutions(udsId),
                        Collaborations = relation.AddCollaborations(udsId)
                    };
                    model.Users = relation.AddUsers(udsId);

                    string logMessage = string.Concat(UDS_LOG_INSERT, string.Format(UDS_INFO, model.Title, model.FullNumber));
                    model.Logs.Add(CreateNewLog(udsId, UDSLogType.Insert, logMessage, userName, creationTime));

                    if (commitHook != null && !commitHook(true))
                    {
                        return new UDSEntityModel();
                    }

                    tr.Complete();
                    _logger.WriteInfo(new LogMessage(string.Concat("successful UDS ", _uds.Model.Title, " - ", currentYear, "/", currentNumber.ToString("0000000"))), LogCategories);
                    return model;
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw new UDSDataException(string.Format("AddUDS - Errore durante l'esecuzione: {0}", ex.Message), ex);
            }
        }

        private UnitaDocumentariaSpecifica ArchiveBiblosDocuments(UnitaDocumentariaSpecifica udsModel, IDictionary<string, object> metadatas, string UDSRepositoryName, bool isMainDoc = false, string signature = "")
        {
            if (udsModel.Documents == null)
            {
                return udsModel;
            }

            if (udsModel.Documents.Document != null && udsModel.Documents.Document.Required && !udsModel.Documents.Document.Instances.Any())
            {
                _logger.WriteWarning(new LogMessage("The UDS has not declared main document"), LogCategories);
                throw new UDSDataException("The UDS has not declared main document");
            }

            if (udsModel.Documents.DocumentAttachment != null && udsModel.Documents.DocumentAttachment.Required && !udsModel.Documents.DocumentAttachment.Instances.Any())
            {
                _logger.WriteWarning(new LogMessage("The UDS has not declared attachment document"), LogCategories);
                throw new UDSDataException("The UDS has not declared attachment document");
            }

            if (udsModel.Documents.DocumentAnnexed != null && udsModel.Documents.DocumentAnnexed.Required && !udsModel.Documents.DocumentAnnexed.Instances.Any())
            {
                _logger.WriteWarning(new LogMessage("The UDS has not declared annexed document"), LogCategories);
                throw new UDSDataException("The UDS has not declared annexed document");
            }

            _logger.WriteInfo(new LogMessage("uds.Model.Documents found"), LogCategories);
            List<BiblosDS.BiblosDS.Archive> archives = _biblosDocumentClient.GetArchives();

            if (udsModel.Documents.Document != null && udsModel.Documents.Document.Instances != null && udsModel.Documents.Document.Instances.Any())
            {
                BiblosDS.BiblosDS.Archive mainArchive = archives.Single(f => f.Name.Equals(udsModel.Documents.Document.BiblosArchive, StringComparison.InvariantCultureIgnoreCase));
                List<BiblosDS.BiblosDS.Attribute> mainAttributes = _biblosDocumentClient.GetAttributesDefinition(mainArchive.Name);
                _logger.WriteDebug(new LogMessage($"biblos main archive name is {mainArchive.Name}"), LogCategories);

                _logger.WriteInfo(new LogMessage("uds.Model.Documents.Document found"), LogCategories);
                udsModel.Documents.Document.Instances = ArchiveInBiblosDS(_biblosDocumentClient, mainArchive, mainAttributes, udsModel.Documents.Document.Instances, metadatas, UDSRepositoryName, isMainDoc: true, signatureValue: signature);
            }
            if (udsModel.Documents.DocumentAttachment != null && udsModel.Documents.DocumentAttachment.Instances != null && udsModel.Documents.DocumentAttachment.Instances.Any())
            {
                BiblosDS.BiblosDS.Archive attachmentArchive = archives.Single(f => f.Name.Equals(udsModel.Documents.DocumentAttachment.BiblosArchive, StringComparison.InvariantCultureIgnoreCase));
                List<BiblosDS.BiblosDS.Attribute> attachmentAttributes = _biblosDocumentClient.GetAttributesDefinition(attachmentArchive.Name);
                _logger.WriteDebug(new LogMessage($"biblos attachment archive name is {attachmentArchive.Name}"), LogCategories);

                _logger.WriteInfo(new LogMessage("uds.Model.Documents.DocumentAttachment found"), LogCategories);
                udsModel.Documents.DocumentAttachment.Instances = ArchiveInBiblosDS(_biblosDocumentClient, attachmentArchive, attachmentAttributes, udsModel.Documents.DocumentAttachment.Instances, metadatas, UDSRepositoryName, signatureValue: signature);
            }
            if (udsModel.Documents.DocumentAnnexed != null && udsModel.Documents.DocumentAnnexed.Instances != null && udsModel.Documents.DocumentAnnexed.Instances.Any())
            {
                BiblosDS.BiblosDS.Archive annexedArchive = archives.Single(f => f.Name.Equals(udsModel.Documents.DocumentAnnexed.BiblosArchive, StringComparison.InvariantCultureIgnoreCase));
                List<BiblosDS.BiblosDS.Attribute> annexedAttributes = _biblosDocumentClient.GetAttributesDefinition(annexedArchive.Name);
                _logger.WriteDebug(new LogMessage($"biblos annexed archive name is {annexedArchive.Name}"), LogCategories);

                _logger.WriteInfo(new LogMessage("uds.Model.Documents.DocumentAnnexed found"), LogCategories);
                udsModel.Documents.DocumentAnnexed.Instances = ArchiveInBiblosDS(_biblosDocumentClient, annexedArchive, annexedAttributes, udsModel.Documents.DocumentAnnexed.Instances, metadatas, UDSRepositoryName, signatureValue: signature);
            }
            if (udsModel.Documents.DocumentDematerialisation != null && udsModel.Documents.DocumentDematerialisation.Instances != null && udsModel.Documents.DocumentDematerialisation.Instances.Any())
            {
                BiblosDS.BiblosDS.Archive documentDematerialisationArchive = archives.Single(f => f.Name.Equals(udsModel.Documents.DocumentDematerialisation.BiblosArchive, StringComparison.InvariantCultureIgnoreCase));
                List<BiblosDS.BiblosDS.Attribute> dematerialisationAttributes = _biblosDocumentClient.GetAttributesDefinition(documentDematerialisationArchive.Name);
                _logger.WriteDebug(new LogMessage($"biblos dematerialisation archive name is {documentDematerialisationArchive.Name}"), LogCategories);

                _logger.WriteInfo(new LogMessage("uds.Model.Documents.DematerialisationDocument found"), LogCategories);
                udsModel.Documents.DocumentDematerialisation.Instances = ArchiveInBiblosDS(_biblosDocumentClient, documentDematerialisationArchive, dematerialisationAttributes, udsModel.Documents.DocumentDematerialisation.Instances, metadatas, UDSRepositoryName, signatureValue: signature);
            }

            return udsModel;
        }

        /// <summary>
        /// Crea l'attributo della segnatura per la chain
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="metadatas"></param>
        /// <param name="UDSRepositoryName"></param>
        /// <returns></returns>
        private BiblosDS.BiblosDS.AttributeValue CreateSignatureAttribute(List<BiblosDS.BiblosDS.Attribute> attributes, IDictionary<string, object> metadatas, string UDSRepositoryName, string signatureValue = "")
        {
            string signature = signatureValue;
            if (string.IsNullOrEmpty(signatureValue))
            {
                signature = string.Concat("Archivio ", UDSRepositoryName, " n. ", metadatas[UDSTableBuilder.UDSYearField], "/",
                                    ((int)metadatas[UDSTableBuilder.UDSNumberField]).ToString("0000000"), " del ",
                                    ((DateTimeOffset)metadatas[UDSTableBuilder.UDSRegistrationDateField]).ToLocalTime().Date.ToString("dd/MM/yyyy"));

            }

            return CreateBiblosDSAttribute((x) => x.Single(f => f.Name.Equals(AttributeHelper.AttributeName_Signature, StringComparison.InvariantCultureIgnoreCase)),
                            attributes, AttributeHelper.AttributeName_Signature, signature);
        }

        /// <summary>
        /// Genera la lista degli attributi biblos da associare ad una nuova chain
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="metadatas"></param>
        /// <param name="UDSRepositoryName"></param>
        /// <returns></returns>
        private List<BiblosDS.BiblosDS.AttributeValue> GetChainAttributes(List<BiblosDS.BiblosDS.Attribute> attributes, IDictionary<string, object> metadatas, string UDSRepositoryName, bool isMainDoc = false, string signature = "")
        {
            List<BiblosDS.BiblosDS.AttributeValue> attributeValues = new List<BiblosDS.BiblosDS.AttributeValue>()
            {
                CreateBiblosDSAttribute((x) => x.Single(f=> f.Name.Equals(AttributeHelper.AttributeName_Filename, StringComparison.InvariantCultureIgnoreCase)),
                            attributes, AttributeHelper.AttributeName_Filename, string.Format("{0}_{1}.xml", metadatas[UDSTableBuilder.UDSYearField], metadatas[UDSTableBuilder.UDSNumberField])),
                CreateSignatureAttribute(attributes, metadatas,UDSRepositoryName, signatureValue: signature)
            };

            if (isMainDoc)
            {
                attributeValues.AddRange(new List<BiblosDS.BiblosDS.AttributeValue>()
                {
                    CreateBiblosDSAttribute((x) => x.Single(f => f.Name.Equals(AttributeHelper.AttributeName_UDSYear, StringComparison.InvariantCultureIgnoreCase)),
                            attributes, AttributeHelper.AttributeName_UDSYear, metadatas[UDSTableBuilder.UDSYearField]),
                    CreateBiblosDSAttribute((x) => x.Single(f => f.Name.Equals(AttributeHelper.AttributeName_UDSNumber, StringComparison.InvariantCultureIgnoreCase)),
                            attributes, AttributeHelper.AttributeName_UDSNumber, metadatas[UDSTableBuilder.UDSNumberField]),
                    CreateBiblosDSAttribute((x) => x.Single(f => f.Name.Equals(AttributeHelper.AttributeName_UDSSubject, StringComparison.InvariantCultureIgnoreCase)),
                            attributes, AttributeHelper.AttributeName_UDSSubject, metadatas[UDSTableBuilder.UDSSubjectField]),
                    CreateBiblosDSAttribute((x) => x.Single(f => f.Name.Equals(AttributeHelper.AttributeName_Date, StringComparison.InvariantCultureIgnoreCase)),
                            attributes, AttributeHelper.AttributeName_Date, metadatas[UDSTableBuilder.UDSRegistrationDateField])
                });
            }

            BiblosDS.BiblosDS.AttributeValue attributeValue = null;
            LogMessage logMessage;
            foreach (KeyValuePair<string, object> metadata in metadatas)
            {
                _logger.WriteDebug(new LogMessage(string.Concat("Filling biblos attribute ", metadata.Key)), LogCategories);
                logMessage = new LogMessage(string.Concat("skip attribute ", metadata.Key, " with null value"));

                attributeValue = CreateBiblosDSAttribute((x) => x.SingleOrDefault(f => f.Name.Equals(metadata.Key, StringComparison.InvariantCultureIgnoreCase)),
                        attributes, metadata.Key, metadata.Value);
                if (attributeValue != null)
                {
                    logMessage = new LogMessage(string.Concat("set metadata ", metadata.Key, " -> ", metadata.Value));
                    attributeValues.Add(attributeValue);
                }
                _logger.WriteDebug(logMessage, LogCategories);
            }

            return attributeValues;
        }

        private BiblosDS.BiblosDS.AttributeValue CreateBiblosDSAttribute(Func<List<BiblosDS.BiblosDS.Attribute>, BiblosDS.BiblosDS.Attribute> lambda,
            List<BiblosDS.BiblosDS.Attribute> attributes, string attributeName, object udsValue)
        {
            BiblosDS.BiblosDS.Attribute attribute = null;
            try
            {
                attribute = lambda(attributes);
            }
            catch (Exception)
            {
                throw new ArgumentNullException(string.Concat("AttributeName '", attributeName, "' is not defined in selected archive"));
            }
            BiblosDS.BiblosDS.AttributeValue r_attributeValue = null;
            if (attribute != null)
            {
                if (attribute.IsRequired && udsValue == null)
                {
                    throw new ArgumentNullException(string.Concat("Attribute '", attribute.Name, "' in archive is set to be required but value is not defined"));
                }
                if (udsValue is DateTimeOffset)
                {
                    DateTimeOffset item = (DateTimeOffset)udsValue;
                    udsValue = item.DateTime;
                }
                r_attributeValue = new BiblosDS.BiblosDS.AttributeValue()
                {
                    Attribute = attribute,
                    Value = udsValue
                };
            }
            return r_attributeValue;
        }

        private DocumentInstance[] ArchiveInBiblosDS(BiblosDS.BiblosDS.DocumentsClient documentsClient, BiblosDS.BiblosDS.Archive archive,
            List<BiblosDS.BiblosDS.Attribute> attributes, DocumentInstance[] documents, IDictionary<string, object> metadatas, string UDSRepositoryName, bool isMainDoc = false, string signatureValue = "")
        {
            BiblosDS.BiblosDS.Document biblosDoc = null;
            if (documents == null || !documents.Any())
            {
                return documents;
            }

            Guid? chainId;
            BiblosDS.BiblosDS.Content documentContent;
            List<BiblosDS.BiblosDS.AttributeValue> attributeValues = GetChainAttributes(attributes, metadatas, UDSRepositoryName, isMainDoc, signature: signatureValue);
            chainId = RetryingPolicyAction(() => documentsClient.CreateDocumentChain(archive.Name, attributeValues));
            _logger.WriteInfo(new LogMessage($"inserted document chain {chainId} in archive {archive.IdArchive}"), LogCategories);
            Guid idDocumentToStore;
            BiblosDS.BiblosDS.Document documentToStore;
            BiblosDS.BiblosDS.AttributeValue documentToStoreAttribute;
            foreach (DocumentInstance documentInstance in documents)
            {

                idDocumentToStore = Guid.Parse(documentInstance.IdDocumentToStore);
                _logger.WriteInfo(new LogMessage($"reading document content {documentInstance.IdDocumentToStore} ..."), LogCategories);
                documentContent = RetryingPolicyAction(() => documentsClient.GetDocumentContentById(idDocumentToStore));
                _logger.WriteInfo(new LogMessage($"reading document Info {documentInstance.IdDocumentToStore} ..."), LogCategories);
                documentToStore = RetryingPolicyAction(() => documentsClient.GetDocumentInfoById(idDocumentToStore));

                biblosDoc = new BiblosDS.BiblosDS.Document
                {
                    Archive = archive,
                    Content = new BiblosDS.BiblosDS.Content { Blob = documentContent.Blob },
                    Name = documentInstance.DocumentName,
                    IsVisible = true,
                    AttributeValues = new List<BiblosDS.BiblosDS.AttributeValue>()
                    {
                        CreateBiblosDSAttribute((x) => x.Single(f=> f.Name.Equals(AttributeHelper.AttributeName_Filename, StringComparison.InvariantCultureIgnoreCase)),
                            attributes, AttributeHelper.AttributeName_Filename, documentInstance.DocumentName),

                       CreateSignatureAttribute(attributes, metadatas, UDSRepositoryName, signatureValue: signatureValue)
                    }
                };

                documentToStoreAttribute = documentToStore.AttributeValues.SingleOrDefault((x) => x.Attribute.Name == AttributeHelper.AttributeName_SignModels);
                if (documentToStoreAttribute != null)
                {
                    BiblosDS.BiblosDS.AttributeValue attributeValue = null;
                    attributeValue = CreateBiblosDSAttribute((x) => x.SingleOrDefault(f => f.Name.Equals(AttributeHelper.AttributeName_SignModels, StringComparison.InvariantCultureIgnoreCase)),
                           attributes, AttributeHelper.AttributeName_SignModels, documentToStoreAttribute.Value);
                    biblosDoc.AttributeValues.Add(attributeValue);
                }
                biblosDoc = RetryingPolicyAction(() => documentsClient.AddDocumentToChain(biblosDoc, chainId, BiblosDS.BiblosDS.ContentFormat.Binary));
                _logger.WriteInfo(new LogMessage($"inserted document {biblosDoc.IdDocument} in archive { archive.IdArchive }"), LogCategories);
                documentInstance.StoredChainId = biblosDoc.DocumentParent.IdDocument.ToString();
                _logger.WriteInfo(new LogMessage($"detaching workflow document {documentInstance.IdDocumentToStore} ..."), LogCategories);
                RetryingPolicyAction(() => documentsClient.DocumentDetach(new BiblosDS.BiblosDS.Document() { IdDocument = idDocumentToStore }));
            }
            return documents;
        }

        private UnitaDocumentariaSpecifica UpdateBiblosDocuments(UnitaDocumentariaSpecifica udsModel, IDictionary<string, object> metadatas, string UDSRepositoryName, UDSRelations existingRels, string userName)
        {
            _logger.WriteInfo(new LogMessage("uds.Model.Documents found"), LogCategories);

            List<BiblosDS.BiblosDS.Archive> archives = _biblosDocumentClient.GetArchives();

            if (udsModel.Documents.Document != null && udsModel.Documents.Document.Instances != null && udsModel.Documents.Document.Instances.Any())
            {
                BiblosDS.BiblosDS.Archive mainArchive = archives.Single(f => f.Name.Equals(udsModel.Documents.Document.BiblosArchive, StringComparison.InvariantCultureIgnoreCase));
                List<BiblosDS.BiblosDS.Attribute> mainAttributes = _biblosDocumentClient.GetAttributesDefinition(mainArchive.Name);
                _logger.WriteDebug(new LogMessage(string.Concat("biblos main archive name is ", mainArchive.Name)), LogCategories);

                _logger.WriteInfo(new LogMessage("uds.Model.Documents.Document found"), LogCategories);
                bool checkoutOptimization = !udsModel.Documents.Document.AllowMultiFile;
                udsModel.Documents.Document.Instances = UpdateInBiblosDS(_biblosDocumentClient, mainArchive, mainAttributes, udsModel.Documents.Document.Instances,
                    metadatas, UDSRepositoryName, existingRels, Relations.UDSDocumentType.Document, userName, checkoutOptimization, true);
            }
            if (udsModel.Documents.DocumentAnnexed != null && udsModel.Documents.DocumentAnnexed.Instances != null && udsModel.Documents.DocumentAnnexed.Instances.Any())
            {
                BiblosDS.BiblosDS.Archive annexedArchive = archives.Single(f => f.Name.Equals(udsModel.Documents.DocumentAnnexed.BiblosArchive, StringComparison.InvariantCultureIgnoreCase));
                List<BiblosDS.BiblosDS.Attribute> annexedAttributes = _biblosDocumentClient.GetAttributesDefinition(annexedArchive.Name);
                _logger.WriteDebug(new LogMessage(string.Concat("biblos annexed archive name is ", annexedArchive.Name)), LogCategories);


                _logger.WriteInfo(new LogMessage("uds.Model.Documents.DocumentAnnexed found"), LogCategories);
                udsModel.Documents.DocumentAnnexed.Instances = UpdateInBiblosDS(_biblosDocumentClient, annexedArchive, annexedAttributes, udsModel.Documents.DocumentAnnexed.Instances,
                    metadatas, UDSRepositoryName, existingRels, Relations.UDSDocumentType.DocumentAnnexed, userName);
            }
            if (udsModel.Documents.DocumentAttachment != null && udsModel.Documents.DocumentAttachment.Instances != null && udsModel.Documents.DocumentAttachment.Instances.Any())
            {
                BiblosDS.BiblosDS.Archive attachmentArchive = archives.Single(f => f.Name.Equals(udsModel.Documents.DocumentAttachment.BiblosArchive, StringComparison.InvariantCultureIgnoreCase));
                List<BiblosDS.BiblosDS.Attribute> attachmentAttributes = _biblosDocumentClient.GetAttributesDefinition(attachmentArchive.Name);
                _logger.WriteDebug(new LogMessage(string.Concat("biblos attachment archive name is ", attachmentArchive.Name)), LogCategories);

                _logger.WriteInfo(new LogMessage("uds.Model.Documents.DocumentAttachment found"), LogCategories);
                udsModel.Documents.DocumentAttachment.Instances = UpdateInBiblosDS(_biblosDocumentClient, attachmentArchive, attachmentAttributes, udsModel.Documents.DocumentAttachment.Instances,
                    metadatas, UDSRepositoryName, existingRels, Relations.UDSDocumentType.DocumentAttachment, userName);
            }
            if (udsModel.Documents.DocumentDematerialisation != null && udsModel.Documents.DocumentDematerialisation.Instances != null && udsModel.Documents.DocumentDematerialisation.Instances.Any())
            {
                BiblosDS.BiblosDS.Archive documentDematerialisationArchive = archives.Single(f => f.Name.Equals(udsModel.Documents.DocumentDematerialisation.BiblosArchive, StringComparison.InvariantCultureIgnoreCase));
                List<BiblosDS.BiblosDS.Attribute> attachmentAttributes = _biblosDocumentClient.GetAttributesDefinition(documentDematerialisationArchive.Name);
                _logger.WriteDebug(new LogMessage(string.Concat("biblos dematerialisation archive name is ", documentDematerialisationArchive.Name)), LogCategories);

                _logger.WriteInfo(new LogMessage("uds.Model.Documents.DematerialisationDocument found"), LogCategories);
                udsModel.Documents.DocumentDematerialisation.Instances = UpdateInBiblosDS(_biblosDocumentClient, documentDematerialisationArchive, attachmentAttributes, udsModel.Documents.DocumentDematerialisation.Instances,
                    metadatas, UDSRepositoryName, existingRels, Relations.UDSDocumentType.Dematerialisation, userName);
            }
            return udsModel;
        }

        private DocumentInstance[] UpdateInBiblosDS(BiblosDS.BiblosDS.DocumentsClient documentsClient, BiblosDS.BiblosDS.Archive archive,
            List<BiblosDS.BiblosDS.Attribute> attributes, DocumentInstance[] documents, IDictionary<string, object> metadatas,
            string UDSRepositoryName, UDSRelations existingRels, Relations.UDSDocumentType docType, string userName, bool checkoutOptimization = false, bool isMainDoc = false)
        {
            BiblosDS.BiblosDS.Document biblosDoc = null;
            Guid chainId = Guid.Empty;

            //Creo la lista dei chain attributesvalue
            List<BiblosDS.BiblosDS.AttributeValue> attributeValues = GetChainAttributes(attributes, metadatas, UDSRepositoryName, isMainDoc);

            bool existingType = existingRels.Documents.Any(p => p.DocumentType == (short)docType);
            if (!existingType)
            {
                chainId = documentsClient.CreateDocumentChain(archive.Name, attributeValues);
                checkoutOptimization = false;
                _logger.WriteInfo(new LogMessage(string.Concat("inserted document chain ", chainId, " in archive ", archive.IdArchive.ToString())), LogCategories);
            }
            else
            {
                chainId = existingRels.Documents.FirstOrDefault(p => p.DocumentType == (short)docType).IdDocument;
                _logger.WriteInfo(new LogMessage(string.Concat("found document chain ", chainId)), LogCategories);
                if (docType == Relations.UDSDocumentType.Document)
                {
                    _logger.WriteInfo(new LogMessage("update document chain"), LogCategories);
                    UpdateChain(_biblosDocumentClient, archive, chainId, attributes, metadatas, userName, UDSRepositoryName);
                }
            }

            BiblosDS.BiblosDS.Content documentContent;
            Guid idDocumentToStore;
            foreach (DocumentInstance documentInstance in documents)
            {
                if (!string.IsNullOrEmpty(documentInstance.StoredChainId))
                {
                    continue;
                }
                _logger.WriteInfo(new LogMessage($"reading document content {documentInstance.IdDocumentToStore} ..."), LogCategories);
                idDocumentToStore = Guid.Parse(documentInstance.IdDocumentToStore);
                documentContent = RetryingPolicyAction(() => documentsClient.GetDocumentContentById(idDocumentToStore));

                biblosDoc = new BiblosDS.BiblosDS.Document()
                {
                    Archive = archive,
                    Content = new BiblosDS.BiblosDS.Content { Blob = documentContent.Blob },
                    Name = documentInstance.DocumentName,
                    IsVisible = true,
                    AttributeValues = new List<BiblosDS.BiblosDS.AttributeValue>()
                        {
                            CreateBiblosDSAttribute((x) => x.Single(f=> f.Name.Equals(AttributeHelper.AttributeName_Filename, StringComparison.InvariantCultureIgnoreCase)),
                                attributes, AttributeHelper.AttributeName_Filename, documentInstance.DocumentName),
                            CreateSignatureAttribute(attributes, metadatas, UDSRepositoryName)
                        }
                };

                if (checkoutOptimization)
                {
                    BiblosDS.BiblosDS.Document temp = documentsClient.GetDocumentChildren(chainId).FirstOrDefault();
                    if (temp != null)
                    {
                        attributeValues = attributeValues.Where(f => !f.Attribute.Name.Equals(AttributeHelper.AttributeName_Filename, StringComparison.InvariantCultureIgnoreCase)
                                                                              && !f.Attribute.Name.Equals(AttributeHelper.AttributeName_Signature, StringComparison.InvariantCultureIgnoreCase)).ToList();
                        biblosDoc = UpdateDocument(documentsClient, archive, temp, biblosDoc, biblosDoc.AttributeValues.Concat(attributeValues).ToList(), userName);
                    }
                    else
                    {
                        biblosDoc = documentsClient.AddDocumentToChain(biblosDoc, chainId, BiblosDS.BiblosDS.ContentFormat.Binary);
                        _logger.WriteInfo(new LogMessage(string.Concat("inserted document ", biblosDoc.IdDocument.ToString(), " in archive ", archive.IdArchive.ToString())), LogCategories);
                    }
                }
                else
                {
                    biblosDoc = documentsClient.AddDocumentToChain(biblosDoc, chainId, BiblosDS.BiblosDS.ContentFormat.Binary);
                    _logger.WriteInfo(new LogMessage(string.Concat("inserted document ", biblosDoc.IdDocument.ToString(), " in archive ", archive.IdArchive.ToString())), LogCategories);
                }
                documentInstance.StoredChainId = biblosDoc.DocumentParent.IdDocument.ToString();
                _logger.WriteInfo(new LogMessage($"detaching workflow document {documentInstance.IdDocumentToStore} ..."), LogCategories);
                RetryingPolicyAction(() => documentsClient.DocumentDetach(new BiblosDS.BiblosDS.Document() { IdDocument = idDocumentToStore }));
            }
            return documents;
        }

        private BiblosDS.BiblosDS.Document UpdateDocument(BiblosDS.BiblosDS.DocumentsClient client, BiblosDS.BiblosDS.Archive archive, BiblosDS.BiblosDS.Document extractedDoc,
            BiblosDS.BiblosDS.Document biblosDoc, IList<BiblosDS.BiblosDS.AttributeValue> attributeValues, string userName)
        {
            try
            {
                BiblosDS.BiblosDS.Document confirmedDoc;

                client.CheckOutDocument(extractedDoc.IdDocument, userName, BiblosDS.BiblosDS.ContentFormat.Binary, false);
                _logger.WriteInfo(new LogMessage(string.Concat("checked out document ", extractedDoc.IdDocument.ToString(), " in archive ", archive.IdArchive.ToString())), LogCategories);

                extractedDoc.Content = biblosDoc.Content;
                extractedDoc.Name = biblosDoc.Name;
                extractedDoc.AttributeValues = attributeValues.ToList();

                confirmedDoc = client.CheckInDocument(extractedDoc, userName, BiblosDS.BiblosDS.ContentFormat.Binary, null);
                client.ConfirmDocument(extractedDoc.IdDocument);
                _logger.WriteInfo(new LogMessage(string.Concat("checked in document ", extractedDoc.IdDocument.ToString(), " in archive ", archive.IdArchive.ToString())), LogCategories);
                return confirmedDoc;
            }
            catch (Exception ex)
            {
                client.UndoCheckOutDocument(extractedDoc.IdDocument, userName);
                _logger.WriteError(new LogMessage(string.Concat("error check in/out document", extractedDoc.IdDocument.ToString(), " in archive ", archive.IdArchive.ToString())), LogCategories);
                throw ex;
            }
        }

        private BiblosDS.BiblosDS.Document UpdateChain(BiblosDS.BiblosDS.DocumentsClient client, BiblosDS.BiblosDS.Archive archive, Guid idChain,
            List<BiblosDS.BiblosDS.Attribute> attributes, IDictionary<string, object> metadatas, string userName, string UDSRepositoryName)
        {
            //Creo la lista dei chain attributesvalue
            List<BiblosDS.BiblosDS.AttributeValue> attributeValues = GetChainAttributes(attributes, metadatas, UDSRepositoryName, true);

            try
            {
                BiblosDS.BiblosDS.Document confirmedChain;
                BiblosDS.BiblosDS.Document extractedChain;
                extractedChain = client.CheckOutDocument(idChain, userName, BiblosDS.BiblosDS.ContentFormat.Binary, false);
                _logger.WriteInfo(new LogMessage(string.Concat("checked out chain ", idChain.ToString(), " in archive ", archive.IdArchive.ToString())), LogCategories);

                extractedChain.AttributeValues = attributeValues;

                confirmedChain = client.CheckInDocument(extractedChain, userName, BiblosDS.BiblosDS.ContentFormat.Binary, null);
                client.ConfirmDocument(extractedChain.IdDocument);
                _logger.WriteInfo(new LogMessage(string.Concat("checked in chain ", idChain.ToString(), " in archive ", archive.IdArchive.ToString())), LogCategories);
                return confirmedChain;
            }
            catch (Exception ex)
            {
                client.UndoCheckOutDocument(idChain, userName);
                _logger.WriteError(new LogMessage(string.Concat("error check in/out chain", idChain.ToString(), " in archive ", archive.IdArchive.ToString())), LogCategories);
                throw ex;
            }
        }

        private static string ToStringFormat(string format, object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(format))
            {
                return value.ToString();
            }

            return string.Format(format, value);
        }
        public static object GetDynamicProperty(object target, string name)
        {
            CallSite<Func<CallSite, object, object>> site = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, name, target.GetType(), new[] { Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(0, null) }));
            return site.Target(site, target);
        }

        /// <summary>
        /// Aggiorna l'Uds e tutte le relazioni contenuto nel file Xml
        /// </summary>
        /// <param name="connStr">String di connessione</param>
        /// <param name="udsId">Primary Key della Uds da modificare</param>
        /// <param name="commitHook">Function Delegate che consente passare una funzione esterna da eseguire prima del commit su DB. Se ritorna False viene fatto rollback della transazione</param>
        /// <returns>Ritorna 1 se il record della Uds è stato modificato</returns>
        public UDSEntityModel UpdateUDS(string connStr, Guid udsId, IEnumerable<AuthorizationInstance> buildModelRoles, string userName, DateTimeOffset creationTime, Func<bool, bool> commitHook = null)
        {
            try
            {
                //validazione
                List<string> validationErrors = new List<string>();
                bool validate = _uds.ValidateValues(out validationErrors);
                if (!validate)
                {
                    throw new UDSDataException(string.Format("UpdateUDS - Errore di validazione dei valori: {0}", string.Join("\n", validationErrors)));
                }
                //costruisce l'oggetto dinamico
                IDictionary<string, object> metadatas = new ExpandoObject() as IDictionary<string, object>;

                //dati statici
                metadatas.Add(UDSTableBuilder.UDSPK, udsId);
                metadatas.Add(UDSTableBuilder.UDSSubjectField, _uds.Model.Subject.Value);
                metadatas.Add(UDSTableBuilder.UDSIdCategoryFK, _uds.Model.Category.IdCategory);
                metadatas.Add(UDSTableBuilder.UDSLastChangedUserField, userName);
                metadatas.Add(UDSTableBuilder.UDSLastChangedDateField, DateTimeOffset.UtcNow);

                using (Database db = new Database(connStr))
                {
                    UDSRelationFacade relation = new UDSRelationFacade(_uds, Builder.DbSchema);
                    UDSRelations existing = relation.GetRelations(db, udsId);
                    dynamic persited_uds = GetUDS(db, udsId);
                    UDSEntityModel existingEntity = db.Fetch<UDSEntityModel>(string.Format(_q_get_udsYearNumber, Builder.DbSchema, Builder.UDSTableName, udsId)).SingleOrDefault();
                    metadatas.Add(UDSTableBuilder.UDSYearField, existingEntity.Year);
                    metadatas.Add(UDSTableBuilder.UDSNumberField, existingEntity.Number);
                    metadatas.Add(UDSTableBuilder.UDSRegistrationDateField, existingEntity.RegistrationDate);
                    object value;
                    UDSModelField uField;
                    NumberField numberField;
                    //dati della UDS
                    foreach (Section section in _uds.Model.Metadata)
                    {
                        foreach (FieldBaseType field in section.Items)
                        {
                            uField = new UDSModelField(field);
                            if (!(field is DateField) ||
                                (field is DateField && !string.IsNullOrEmpty(uField.Value as string) && !DateTime.MinValue.ToString().Equals(uField.Value as string)))
                            {
                                value = uField.Value;
                                if (field is NumberField)
                                {
                                    numberField = field as NumberField;
                                    value = null;
                                    if (numberField.ValueSpecified)
                                    {
                                        value = numberField.Value;
                                    };
                                }

                                _logger.WriteDebug(new LogMessage(string.Concat("Filling database column ", uField.ColumnName, " with value ", value)), LogCategories);
                                metadatas.Add(uField.ColumnName, uField.IsModifyEnabled ? value : GetDynamicProperty(persited_uds, uField.ColumnName));
                            }
                        }
                    }

                    if (_uds.Model.Documents != null)
                    {
                        try
                        {
                            _uds.Model = UpdateBiblosDocuments(_uds.Model, metadatas, _uds.Model.Title, existing, userName);
                        }
                        catch (Exception ex)
                        {
                            _logger.WriteError(ex, LogCategories);
                            throw ex;
                        }
                    }

                    using (ITransaction tr = db.GetTransaction())
                    {
                        db.Update(_builder.DbSchema, _builder.UDSTableName, UDSTableBuilder.UDSPK, metadatas, udsId, null);

                        string logPartialMessage = UDS_LOG_MODIFY;
                        UDSLogType logType = UDSLogType.Modify;

                        UDSEntityModel model = new UDSEntityModel
                        {
                            IdUDS = udsId,
                            Year = existingEntity.Year,
                            Number = existingEntity.Number,
                            RegistrationDate = existingEntity.RegistrationDate,
                            LastChangedDate = DateTimeOffset.UtcNow,
                            LastChangedUser = userName,
                            Subject = _uds.Model.Subject.Value,
                            Title = _uds.Model.Title,
                            IdCategory = Convert.ToInt16(_uds.Model.Category.IdCategory),
                            IdContainer = Convert.ToInt16(_uds.Model.Container.IdContainer),
                            DocumentUnitSynchronizeEnabled = _uds.Model.DocumentUnitSynchronizeEnabled
                        };

                        if (!existingEntity.Subject.Equals(_uds.Model.Subject.Value))
                        {
                            logPartialMessage = UDS_LOG_OBJECT_MODIFY;
                            logType = UDSLogType.ObjectModify;
                            string logSubjectMessage = string.Concat(logPartialMessage, string.Format(UDS_INFO, model.Title, model.FullNumber));
                            model.Logs.Add(CreateNewLog(udsId, logType, logSubjectMessage, userName, creationTime));
                        }

                        model.Relations = new UDSRelations
                        {
                            Documents = relation.UpdateDocuments(db, udsId, existing, (chainId) =>
                            {
                                //Eseguo il detach dei documenti
                                BiblosDS.BiblosDS.Document toDelete = _biblosDocumentClient.GetDocumentInfoById(chainId);
                                if (toDelete != null)
                                {
                                    _biblosDocumentClient.DocumentDetach(toDelete);
                                    _logger.WriteDebug(new LogMessage($"UpdateUDS - Detached document Id: {toDelete.IdDocument}"), LogCategories);
                                }
                            }),
                            Authorizations = relation.AddAuthorizations(udsId, buildModelRoles),
                            Contacts = relation.AddContacts(udsId),
                            Messages = relation.AddMessages(udsId),
                            PECMails = relation.AddPECMails(udsId),
                            Protocols = relation.AddProtocols(udsId),
                            Resolutions = relation.AddResolutions(udsId),
                            Collaborations = relation.AddCollaborations(udsId)
                        };
                        model.Users = relation.AddUsers(udsId);

                        if (logType == UDSLogType.Modify)
                        {
                            string logMessage = string.Concat(logPartialMessage, string.Format(UDS_INFO, model.Title, model.FullNumber));
                            model.Logs.Add(CreateNewLog(udsId, logType, logMessage, userName, creationTime));
                        }

                        if (commitHook != null && !commitHook(true))
                        {
                            return new UDSEntityModel();
                        }

                        tr.Complete();
                        return model;
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw new UDSDataException(string.Format("UpdateUDS - Errore durante l'esecuzione: {0}", ex.Message), ex);
            }
        }

        /// <summary>
        /// Esegue l'annullamento di una UDS
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="udsId"></param>
        /// <param name="userName"></param>
        public UDSEntityModel CancelUDS(string connStr, Guid udsId, string userName, DateTimeOffset creationTime, string cancelMotivation)
        {
            try
            {
                IDictionary<string, object> metadatas = new ExpandoObject() as IDictionary<string, object>;
                metadatas.Add(UDSTableBuilder.UDSPK, udsId);
                metadatas.Add(UDSTableBuilder.UDSStatusField, 0);
                metadatas.Add(UDSTableBuilder.UDSCancelMotivationField, cancelMotivation);
                UDSEntityModel existingEntity;

                using (Database db = new Database(connStr))
                {
                    using (ITransaction tr = db.GetTransaction())
                    {
                        int count = db.Update(_builder.DbSchema, _builder.UDSTableName, UDSTableBuilder.UDSPK, metadatas, udsId, null);
                        UDSRelationFacade relation = new UDSRelationFacade(_uds, Builder.DbSchema);
                        UDSRelations existing = relation.GetRelations(db, udsId);
                        BiblosDS.BiblosDS.Document toDelete;
                        foreach (UDSDocument document in existing.Documents)
                        {
                            toDelete = _biblosDocumentClient.GetDocumentInfoIgnoreState(document.IdDocument);
                            if (toDelete != null && !toDelete.IsRemoved)
                            {
                                _biblosDocumentClient.DocumentDetach(toDelete);
                                _logger.WriteDebug(new LogMessage($"CancelUDS - Detach document Id: {toDelete.IdDocument}"), LogCategories);
                            }
                        }

                        tr.Complete();

                        existingEntity = db.Fetch<UDSEntityModel>(string.Format(_q_get_udsYearNumber, Builder.DbSchema, Builder.UDSTableName, udsId)).SingleOrDefault();
                        string logMessage = string.Format(UDS_LOG_DELETE, _uds.Model.Title, existingEntity.Year, existingEntity.Number);
                        existingEntity.Logs.Add(CreateNewLog(udsId, UDSLogType.Delete, logMessage, userName, creationTime));
                    }
                }
                return existingEntity;
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw new UDSDataException(string.Format("CancelUDS - Errore durante l'esecuzione: {0}", ex.Message), ex);
            }
        }

        private dynamic GetUDS(Database db, Guid udsId)
        {
            return db.Query<dynamic>(string.Format(_q_get_uds, Builder.DbSchema, Builder.UDSTableName, udsId)).SingleOrDefault();
        }

        private UDSLogModel CreateNewLog(Guid udsId, UDSLogType logType, string logDescription, string username, DateTimeOffset creationTime)
        {
            UDSLogModel log = new UDSLogModel
            {
                UDSLogId = Guid.NewGuid(),
                UDSId = udsId,
                LogDate = creationTime,
                LogDescription = logDescription,
                SystemComputer = Environment.MachineName,
                SystemUser = username,
                LogType = logType
            };

            return log;
        }

        private T RetryingPolicyAction<T>(Func<T> func, int step = 1)
        {
            _logger.WriteDebug(new LogMessage($"RetryingPolicyAction : tentative {step}/{_retry_tentative} in progress..."), LogCategories);
            if (step >= _retry_tentative)
            {
                _logger.WriteError(new LogMessage("VecompSoftware.ServiceBus.Module.UDS.Storage.UDSDataFacade.RetryingPolicyAction: retry policy expired maximum tentatives"), LogCategories);
                throw new Exception("UDSDataFacade retry policy expired maximum tentatives");
            }
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                _logger.WriteWarning(new LogMessage($"SafeActionWithRetryPolicy : tentative {step}/{_retry_tentative} faild. Waiting {_threadWaiting} second before retrying action"), ex, LogCategories);
                Task.Delay(_threadWaiting).Wait();
                return RetryingPolicyAction(func, ++step);
            }
        }
        #endregion

    }

}
