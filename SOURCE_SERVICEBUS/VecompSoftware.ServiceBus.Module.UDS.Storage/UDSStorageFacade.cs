using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.Helpers.UDS;
using VecompSoftware.Helpers.UDS.Exceptions;
using VecompSoftware.ServiceBus.Module.UDS.Storage.Smo;

namespace VecompSoftware.ServiceBus.Module.UDS.Storage
{
    /// <summary>
    /// Gestisce le operazioni di creazione, modifica e cancellazione delle table di supporto all'UDS
    /// Tabella principale e tabelle secondarie di gestione delle relazioni
    /// </summary>
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public class UDSStorageFacade
    {

        #region [ Fields ]

        private readonly UDSModel _udsModel;
        private readonly UDSTableBuilder _udsTableBuilder;
        private readonly ILogger _logger;
        protected static IEnumerable<LogCategory> _logCategories = null;
        private readonly BiblosDS.BiblosDSManagement.AdministrationClient _administrationClient;

        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(UDSStorageFacade));
                }
                return _logCategories;
            }
        }

        public UDSModel UDS => _udsModel;

        public UDSTableBuilder Builder => _udsTableBuilder;

        public string DbSchema => _udsTableBuilder.DbSchema;

        #endregion

        public UDSStorageFacade(ILogger logger, string xml, string xmlSchema, BiblosDS.BiblosDSManagement.AdministrationClient administrationClient, string dbSchema = "")
        {
            _logger = logger;
            bool validate = UDSModel.ValidateXml(xml, xmlSchema, out List<string> validationErrors);
            if (!validate)
            {
                throw new UDSStorageException(string.Format("UDSStorageFacade - Errore di validazione: {0}", string.Join("\n", validationErrors)));
            }

            _udsModel = UDSModel.LoadXml(xml);
            _udsTableBuilder = new UDSTableBuilder(_udsModel, dbSchema);
            _administrationClient = administrationClient;
        }

        #region Methods

        /// <summary>
        /// Crea la tabella di storage dell'UDS e tutte le tabelle per contenere le relazioni esterne (Documents, Contacts ecc...)
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="commitHookAsync">Function Delegate che consente passare una funzione esterna da eseguire prima del commit su DB. Se ritorna False viene fatto rollback della transazione</param>
        public async Task CreateStorageAsync(string connStr, Func<SmoContext, Task<bool>> commitHookAsync = null)
        {
            try
            {
                using (SmoContext ctx = new SmoContext(connStr, DbSchema))
                //using (SqlTransaction trans = ctx.BeginTransaction())
                {
                    try
                    {
                        _logger.WriteDebug(new LogMessage("BeginTransaction"), LogCategories);

                        if (!ctx.TableExist(_udsTableBuilder.UDSTableName))
                        {
                            _logger.WriteDebug(new LogMessage($"creating {_udsTableBuilder.UDSTableName} table"), LogCategories);
                            _udsTableBuilder.CreateUDSTable(ctx);
                        }
                        else
                        {
                            _logger.WriteDebug(new LogMessage($"{_udsTableBuilder.UDSTableName} already exist"), LogCategories);
                        }
                        if (!ctx.TableExist(_udsTableBuilder.UDSDocumentsTableName))
                        {
                            _logger.WriteDebug(new LogMessage($"creating {_udsTableBuilder.UDSDocumentsTableName} table"), LogCategories);
                            _udsTableBuilder.CreateUDSDocumentsTable(ctx);
                        }
                        else
                        {
                            _logger.WriteDebug(new LogMessage($"{_udsTableBuilder.UDSDocumentsTableName} already exist"), LogCategories);
                        }

                        if (commitHookAsync != null)
                        {
                            _logger.WriteDebug(new LogMessage("invoke commitHook"), LogCategories);
                            if (!await commitHookAsync(ctx))
                            {
                                _logger.WriteDebug(new LogMessage("RollbackTransaction"), LogCategories);
                                //trans.Rollback();
                                return;
                            }
                        }
                        _logger.WriteDebug(new LogMessage("CommitTransaction"), LogCategories);
                        //trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteDebug(new LogMessage("RollbackTransaction"), LogCategories);
                        _logger.WriteError(ex, LogCategories);
                        //trans.Rollback();
                        throw new UDSStorageException(string.Format("CreateStorage - Errore durante l'esecuzione: {0}", ex.Message), ex);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw;
            }
        }


        /// <summary>
        /// Aggiorna la tabella di storage dell'UDS. Vengono solamente aggiunti i campi mancanti rispetto l'esistente. Qualsiasi altra variazione non viene considerata
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="commitHook">Function Delegate che consente passare una funzione esterna da eseguire prima del commit su DB. Se ritorna False viene fatto rollback della transazione</param>
        public async Task UpdateStorageAsync(string connStr, Func<Task<bool>> commitHookAsync = null)
        {
            try
            {
                using (SmoContext ctx = new SmoContext(connStr, DbSchema))
                //using (SqlTransaction trans = ctx.BeginTransaction())
                {
                    try
                    {
                        _logger.WriteDebug(new LogMessage("BeginTransaction"), LogCategories);
                        //ctx.BeginTransaction();

                        _logger.WriteDebug(new LogMessage("UpdateUDSTable"), LogCategories);
                        _udsTableBuilder.UpdateUDSTable(ctx);

                        if (commitHookAsync != null)
                        {
                            _logger.WriteDebug(new LogMessage("invoke commitHook"), LogCategories);
                            if (!(await commitHookAsync()))
                            {
                                _logger.WriteDebug(new LogMessage("RollbackTransaction"), LogCategories);
                                //trans.Rollback();
                                return;
                            }
                        }
                        _logger.WriteDebug(new LogMessage("CommitTransaction"), LogCategories);
                        //trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteError(ex, LogCategories);
                        _logger.WriteDebug(new LogMessage("RollbackTransaction"), LogCategories);
                        //trans.Rollback();
                        throw new UDSStorageException(string.Format("UpdateStorage - Errore durante l'esecuzione: {0}", ex.Message), ex);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw;
            }
        }


        /// <summary>
        /// Elimina la tabella di storage dell'UDS e tutte le tabelle di relazione relativa la specifica UDS.
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="commitHook">Function Delegate che consente passare una funzione esterna da eseguire prima del commit su DB. Se ritorna False viene fatto rollback della transazione</param>
        public void DropStorage(string connStr)
        {
            try
            {
                using (SmoContext ctx = new SmoContext(connStr, DbSchema))
                using (SqlTransaction trans = ctx.BeginTransaction())
                {
                    try
                    {

                        //prima le tabelle esterne
                        ctx.DropTable(_udsTableBuilder.UDSDocumentsTableName);

                        //ultima la uds perchè ci sono le FK
                        ctx.DropTable(_udsTableBuilder.UDSTableName);

                        _logger.WriteDebug(new LogMessage("CommitTransaction"), LogCategories);
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteError(ex, LogCategories);
                        _logger.WriteDebug(new LogMessage("RollbackTransaction"), LogCategories);
                        trans.Rollback();
                        throw new UDSStorageException(string.Format("DropStorage - Errore durante l'esecuzione: {0}", ex.Message), ex);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw;
            }
        }

        public async Task GenerateStorageAsync(Func<BiblosDS.BiblosDSManagement.Archive, BiblosDS.BiblosDSManagement.AttributeGroup[], BiblosDS.BiblosDSManagement.AttributeMode[], Task> funcMapBiblosAttributes,
            string StorageMainPath, string StorageType, bool isMainDocument = false)
        {
            try
            {
                _logger.WriteInfo(new LogMessage("Creating biblosds archives"), LogCategories);
                Task<BiblosDS.BiblosDSManagement.StorageType[]> task_storagesTypes = _administrationClient.GetStoragesTypeAsync();
                Task<BiblosDS.BiblosDSManagement.AttributeMode[]> task_attributeModes = _administrationClient.GetAttributeModesAsync();
                BiblosDS.BiblosDSManagement.Storage storage = new BiblosDS.BiblosDSManagement.Storage();
                BiblosDS.BiblosDSManagement.StorageArea storageArea = new BiblosDS.BiblosDSManagement.StorageArea();
                BiblosDS.BiblosDSManagement.Archive archive = new BiblosDS.BiblosDSManagement.Archive();
                BiblosDS.BiblosDSManagement.AttributeGroup attributeGroup_default = new BiblosDS.BiblosDSManagement.AttributeGroup();
                BiblosDS.BiblosDSManagement.AttributeGroup attributeGroup_chain = new BiblosDS.BiblosDSManagement.AttributeGroup();

                archive.IdArchive = Guid.NewGuid();
                archive.Name = isMainDocument ? string.Concat("Archive_UDS_", Builder.UDSName) : string.Concat("Archive_UDS_Miscellaneous_", Builder.UDSName);
                archive.AuthorizationAssembly = string.Empty;
                archive.AuthorizationClassName = string.Empty;
                archive.AutoVersion = false;
                archive.EnableSecurity = false;
                archive.FiscalDocumentType = string.Empty;
                archive.FullSignEnabled = false;
                archive.IsLegal = false;
                archive.LastIdBiblos = 0;
                archive.LowerCache = 512 * (1024 * 1024);
                archive.MaxCache = 1024 * (1024 * 1024);
                archive.PathCache = string.Empty;
                archive.PathPreservation = string.Empty;
                archive.PathTransito = string.Empty;
                archive.PdfConversionEmabled = false;
                archive.ThumbnailEmabled = false;
                archive.TransitoEnabled = false;
                archive.UpperCache = 768 * (1024 * 1024);
                _logger.WriteInfo(new LogMessage(string.Concat("Creating biblosds archive ", archive.Name)), LogCategories);
                archive.IdArchive = await _administrationClient.AddArchiveAsync(archive);

                storage.IdStorage = Guid.NewGuid();
                storage.IsVisible = true;
                storage.AuthenticationKey = string.Empty;
                storage.AuthenticationPassword = string.Empty;
                storage.EnableFulText = false;
                storage.MainPath = Path.Combine(StorageMainPath, string.Concat("UDS", Builder.UDSName.Replace(" ", ""), isMainDocument ? null : "Miscellaneous"));
                storage.Name = isMainDocument ? string.Concat("Storage_UDS_", Builder.UDSName) : string.Concat("Storage_UDS_Miscellaneous_", Builder.UDSName);
                storage.StorageRuleAssembly = string.Empty;
                storage.StorageRuleClassName = string.Empty;
                BiblosDS.BiblosDSManagement.StorageType[] storagesTypes = await task_storagesTypes;
                storage.StorageType = storagesTypes.Single(f => f.Type.Equals(StorageType, StringComparison.InvariantCultureIgnoreCase));
                _logger.WriteInfo(new LogMessage(string.Concat("Creating biblosds storage name ", storage.Name)), LogCategories);
                await _administrationClient.AddStorageAsync(storage);

                BiblosDS.BiblosDSManagement.Archive[] archives = _administrationClient.GetArchives();
                BiblosDS.BiblosDSManagement.Storage[] storages = _administrationClient.GetAllStorages();
                BiblosDS.BiblosDSManagement.Archive udsArchive = archives.SingleOrDefault(a => a.Name.Equals(archive.Name));
                BiblosDS.BiblosDSManagement.Storage udsStorage = storages.SingleOrDefault(a => a.Name.Equals(storage.Name));
                if (udsArchive == null)
                {
                    throw new Exception(string.Format("Archive not found. Archive Name: {0}", archive.Name));
                }
                if (udsStorage == null)
                {
                    throw new Exception(string.Format("Storage not found. Storage Name: {0}", storage.Name));
                }
                storageArea.IdStorageArea = Guid.NewGuid();
                storageArea.Enable = true;
                storageArea.MaxFileNumber = 4096;
                storageArea.MaxSize = Convert.ToInt64(4096) * (1024 * 1024);
                storageArea.Name = isMainDocument ? string.Concat("StorageArea_UDS_", Builder.UDSName) : string.Concat("StorageArea_UDS_Miscellaneous_", Builder.UDSName);
                storageArea.Path = "DVD";
                storageArea.Priority = 0;
                storageArea.Storage = udsStorage;
                storageArea.Archive = udsArchive;
                _logger.WriteInfo(new LogMessage(string.Concat("Creating biblosds storage area", storageArea.Name)), LogCategories);
                await _administrationClient.AddStorageAreaAsync(storageArea);

                BiblosDS.BiblosDSManagement.ArchiveStorage archiveStorage = new BiblosDS.BiblosDSManagement.ArchiveStorage
                {
                    Active = true,
                    Archive = udsArchive,
                    Storage = udsStorage
                };
                _logger.WriteInfo(new LogMessage("Creating biblosds archive_storage"), LogCategories);
                await _administrationClient.AddArchiveStorageAsync(archiveStorage);

                attributeGroup_default.IdAttributeGroup = Guid.NewGuid();
                attributeGroup_default.Description = "default document attributes";
                attributeGroup_default.GroupType = BiblosDS.BiblosDSManagement.AttributeGroupType.Undefined;
                attributeGroup_default.IdArchive = udsArchive.IdArchive;
                attributeGroup_default.IsVisible = true;
                _logger.WriteInfo(new LogMessage("Creating biblosds attributeGroup_default"), LogCategories);
                await _administrationClient.AddAttributeGroupAsync(attributeGroup_default);

                attributeGroup_chain.IdAttributeGroup = Guid.NewGuid();
                attributeGroup_chain.Description = "UDS chain attributes";
                attributeGroup_chain.GroupType = BiblosDS.BiblosDSManagement.AttributeGroupType.Chain;
                attributeGroup_chain.IdArchive = udsArchive.IdArchive;
                attributeGroup_chain.IsVisible = true;
                _logger.WriteInfo(new LogMessage("Creating biblosds attributeGroup_chain"), LogCategories);
                await _administrationClient.AddAttributeGroupAsync(attributeGroup_chain);

                BiblosDS.BiblosDSManagement.AttributeMode[] attributeModes = await task_attributeModes;
                BiblosDS.BiblosDSManagement.AttributeGroup[] attributeGroups = { attributeGroup_default, attributeGroup_chain };
                await funcMapBiblosAttributes(archive, attributeGroups, attributeModes);

                if (isMainDocument)
                {
                    UDS.Model.Documents.Document.BiblosArchive = archive.Name;
                    return;
                }

                if (UDS.Model.Documents.DocumentAttachment != null && UDS.Model.Documents.DocumentAttachment.CreateBiblosArchive)
                {
                    UDS.Model.Documents.DocumentAttachment.BiblosArchive = archive.Name;
                }

                if (UDS.Model.Documents.DocumentAnnexed != null && UDS.Model.Documents.DocumentAnnexed.CreateBiblosArchive)
                {
                    UDS.Model.Documents.DocumentAnnexed.BiblosArchive = archive.Name;
                }

                //aggiungo il blocco riguardante la Dematerializzazione se questa è prevista
                if ((UDS.Model.Documents.Document != null && UDS.Model.Documents.Document.DematerialisationEnabled) ||
                    (UDS.Model.Documents.DocumentAttachment != null && UDS.Model.Documents.DocumentAttachment.DematerialisationEnabled) ||
                    (UDS.Model.Documents.DocumentAnnexed != null && UDS.Model.Documents.DocumentAnnexed.DematerialisationEnabled))
                {
                    AddDocumentDematerialisation(UDS.Model.Documents, archive.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        public void AddDocumentDematerialisation(Documents udsDocuments, string archiveName)
        {
            udsDocuments.DocumentDematerialisation = new Document
            {
                BiblosArchive = archiveName,
                Label = "Attestazione di conformità",
                ReadOnly = true,
                AllowMultiFile = true,
                SignRequired = true,
                ClientId = "uds_doc_dematerialisation_1"
            };
        }


        public async Task<BiblosDS.BiblosDSManagement.Attribute> GenerateBiblosDSAttribute(BiblosDS.BiblosDSManagement.Archive archive,
           BiblosDS.BiblosDSManagement.AttributeGroup attributeGroup, BiblosDS.BiblosDSManagement.AttributeMode attributeMode,
           string attributeName, string attributeType, bool isRequired, bool isMainDate = false)
        {
            BiblosDS.BiblosDSManagement.Attribute attribute = new BiblosDS.BiblosDSManagement.Attribute
            {
                IdAttribute = Guid.NewGuid(),
                Archive = archive,
                AttributeGroup = attributeGroup,
                AttributeType = attributeType,
                Disabled = false,
                IsAutoInc = false,
                IsChainAttribute = attributeGroup.GroupType == BiblosDS.BiblosDSManagement.AttributeGroupType.Chain,
                IsEnumerator = false,
                IsMainDate = isMainDate,
                IsRequired = isRequired,
                IsRequiredForPreservation = false,
                IsSectional = false,
                IsUnique = false,
                IsVisible = true,
                IsVisibleForUser = true,
                Name = attributeName,
                Mode = attributeMode
            };
            _logger.WriteInfo(new LogMessage(string.Concat("Creating biblosds attribute ", attribute.Name)), LogCategories);
            await _administrationClient.AddAttributeAsync(attribute);
            return attribute;
        }



        #endregion

    }
}
