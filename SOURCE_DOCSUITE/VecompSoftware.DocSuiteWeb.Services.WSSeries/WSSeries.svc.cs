using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using HibernatingRhinos.Profiler.Appender.NHibernate;
using VecompSoftware.DocSuiteWeb.AVCP;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.DTO.WSSeries;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.DocSuiteWeb.Services.WSSeries.ErrorHandler;
using VecompSoftware.Helpers;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.NHibernateManager;
using VecompSoftware.Services.Biblos;
using VecompSoftware.Services.Biblos.Models;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.DocSuiteWeb.Services.WSSeries
{

    [GlobalExceptionHandlerBehaviourAttribute(typeof(GlobalExceptionHandler))]
    public class WSSeries : IWSSeries
    {
        #region [ Constants ]

        private const string LoggerName = "WSSeriesLog";

        private const string AppSettingsExcludedExtensions = "ExcludedExtensions";
        private const string AppSettingsConsultationIncludeDocumentStream = "ConsultationIncludeDocumentStream";
        private const string AppSettingsHistoryConsultationEnable = "HistoryConsultationEnable";

        #endregion

        #region [ Properties ]

        private FacadeFactory _facade;

        public FacadeFactory Facade
        {
            get { return _facade ?? (_facade = new FacadeFactory("ProtDB")); }
        }

        /// <summary>
        /// Ritorna una lista contenente tutte le estensioni per le quali non si deve procedere con la conversione in pdf
        /// </summary>
        /// <returns></returns>
        private static List<string> _excludedExtensions;
        public static IEnumerable<string> ExcludedExtensions
        {
            get
            {
                var list = ConfigurationManager.AppSettings[AppSettingsExcludedExtensions];

                if (string.IsNullOrEmpty(list)) return new List<string>();

                if (_excludedExtensions.IsNullOrEmpty())
                {
                    _excludedExtensions = new List<string>(list.Split('|'));
                }

                return _excludedExtensions;
            }
        }

        #endregion

        #region [ Remote Methods ]

        public bool IsAlive()
        {
            return true;
        }

        public string Insert(string xmlSeries)
        {
            // Validazione
            if (string.IsNullOrWhiteSpace(xmlSeries))
                throw new ArgumentException("xml non valorizzato", "xmlSeries");

            FileLogger.Debug(LoggerName, string.Format("Insert - xmlSeries: {0}", xmlSeries));

            // Deserializzazione
            DocumentSeriesItemWSO documentSeriesItemWSO = SerializationHelper.SerializeFromString<DocumentSeriesItemWSO>(xmlSeries);

            DocumentSeries documentSeries = Facade.DocumentSeriesFacade.GetById(documentSeriesItemWSO.IdDocumentSeries);
            if (documentSeries == null)
                throw new ArgumentException("DocumentSeries non presente, verificare l'identificativo", "IdDocumentSeries");

            DocumentSeriesSubsection documentSeriesSubsection = null;
            if (documentSeriesItemWSO.IdDocumentSeriesSubsection.HasValue)
            {
                documentSeriesSubsection = Facade.DocumentSeriesSubsectionFacade.GetById(documentSeriesItemWSO.IdDocumentSeriesSubsection.Value);
                if (documentSeriesSubsection == null)
                    throw new ArgumentException("DocumentSeriesSubsection non presente, verificare l'identificativo", "IdDocumentSeriesSubsection");
            }

            if (documentSeriesItemWSO.MainDocs == null || documentSeriesItemWSO.MainDocs.Count < 1)
                throw new ArgumentException("Attenzione inserire almeno un documento principale", "MainDocs");

            // Preparazione dati per inserimento

            // MainDocuments
            List<DocumentInfo> mainDocuments = documentSeriesItemWSO.MainDocs.Select(item => new MemoryDocumentInfo(Convert.FromBase64String(item.Stream), item.Name, string.Empty)).Cast<DocumentInfo>().ToList();

            // Metadati
            BiblosChainInfo chain = new BiblosChainInfo();
            chain.AddDocuments(mainDocuments);
            foreach (AttributeWSO item in documentSeriesItemWSO.DynamicData)
            {
                chain.AddAttribute(item.Key, item.Value);
            }

            // Annessi
            List<DocumentInfo> lstAnnexed = new List<DocumentInfo>();
            if (documentSeriesItemWSO.AnnexedDocs != null && documentSeriesItemWSO.AnnexedDocs.Count > 0)
            {
                foreach (DocWSO item in documentSeriesItemWSO.AnnexedDocs)
                {
                    lstAnnexed.Add(new MemoryDocumentInfo(Convert.FromBase64String(item.Stream), item.Name, string.Empty));
                }
            }

            //Non pubblicati 
            List<DocumentInfo> lstUnPublished = new List<DocumentInfo>();
            if (documentSeriesItemWSO.UnPublishedDocs != null && documentSeriesItemWSO.UnPublishedDocs.Count > 0)
            {
                foreach (DocWSO item in documentSeriesItemWSO.UnPublishedDocs)
                {
                    lstUnPublished.Add(new MemoryDocumentInfo(Convert.FromBase64String(item.Stream), item.Name, string.Empty));
                }
            }

            // Mi ricavo le Categorie
            Category category = null;
            if (documentSeriesItemWSO.IdCategory.HasValue)
                category = Facade.CategoryFacade.GetById(documentSeriesItemWSO.IdCategory.Value);
            Category subCategory = null;
            if (documentSeriesItemWSO.IdSubCategory.HasValue)
                subCategory = Facade.CategoryFacade.GetById(documentSeriesItemWSO.IdSubCategory.Value);

            // Inserisco la DocumentSeriesItem nel ecosistema Docsuite
            var documentSeriesItem = new DocumentSeriesItem
            {
                DocumentSeries = documentSeries,
                DocumentSeriesSubsection = documentSeriesSubsection,
                PublishingDate = documentSeriesItemWSO.PublishingDate,
                RegistrationUser = documentSeriesItemWSO.RegistrationUser,
                RetireDate = documentSeriesItemWSO.RetireDate,
                Subject = documentSeriesItemWSO.Subject,
                Category = category,
                SubCategory = subCategory
            };

            Facade.DocumentSeriesItemFacade.SaveDocumentSeriesItem(documentSeriesItem, chain, lstAnnexed, null, documentSeriesItemWSO.RegistrationUser);

            return SerializationHelper.SerializeToStringWithoutNamespace(new DocumentSeriesItemWSO
            {
                Id = documentSeriesItem.Id,
                IdDocumentSeries = documentSeriesItem.DocumentSeries.Id,
                IdDocumentSeriesSubsection = documentSeriesItem.DocumentSeriesSubsection != null ? documentSeriesItem.DocumentSeriesSubsection.Id : (int?)null,
                Year = documentSeriesItem.Year,
                Number = documentSeriesItem.Number,
                IdLocation = documentSeriesItem.DocumentSeries.Container.DocumentSeriesLocation.Id,
                IdLocationAnnexed = documentSeriesItem.DocumentSeries.Container.DocumentSeriesAnnexedLocation.Id,
                RegistrationUser = documentSeriesItem.RegistrationUser,
                RegistrationDate = documentSeriesItem.RegistrationDate.DateTime,
                PublishingDate = documentSeriesItemWSO.PublishingDate,
                RetireDate = documentSeriesItemWSO.RetireDate,
                Status = (short)documentSeriesItem.Status,
                Subject = documentSeriesItem.Subject,
                IdCategory = documentSeriesItem.Category != null ? documentSeriesItem.Category.Id : (int?)null,
                IdSubCategory = documentSeriesItem.SubCategory != null ? documentSeriesItem.SubCategory.Id : (int?)null
            });
        }

        public string GetSeriesItem(int idDocumentSeriesItem, bool includeDocuments, bool includeAnnexedDocuments, bool includeUnPublishedDocuments, bool includeStream, bool pdf)
        {
            return GetSeriesItem(idDocumentSeriesItem, includeDocuments, includeAnnexedDocuments, includeUnPublishedDocuments, includeStream, pdf, true);
        }

        public string GetSeriesItem(int idDocumentSeriesItem, bool includeDocuments, bool includeAnnexedDocuments, bool includeUnPublishedDocuments, bool includeStream, bool pdf, bool onlyPublished)
        {
#if DEBUG
            var stopWatch = new Stopwatch();
            stopWatch.Start();
#endif

            // Recupero l'Item da elaborare
            DocumentSeriesItem item = Facade.DocumentSeriesItemFacade.GetById(idDocumentSeriesItem);
            DateTime? datetimeNull = null;
            if (item == null)
                throw new ArgumentException(string.Format("DocumentSerieItem con id {0} non presente", idDocumentSeriesItem), "idDocumentSeriesItem");

            if (onlyPublished && !item.IsPublished())
                throw new InvalidOperationException(string.Format("Attenzione DocumentSeriesItem con id {0} non pubblicata, verificare identificativo passato", idDocumentSeriesItem));

            FileLogger.Debug(LoggerName, string.Format("Consultation - id: {0} withDocument: {1} withAnnexed: {2}", idDocumentSeriesItem, includeDocuments, includeAnnexedDocuments));

            DocumentSeriesItemWSO tor = new DocumentSeriesItemWSO
            {
                Id = item.Id,
                IdDocumentSeries = item.DocumentSeries.Id,
                Year = item.Year,
                Number = item.Number,
                IdLocation = item.DocumentSeries.Container.DocumentSeriesLocation.Id,
                RegistrationUser = item.RegistrationUser,
                RegistrationDate = item.RegistrationDate.DateTime,
                PublishingDate = item.PublishingDate,
                LastChangedDate = item.LastChangedDate.HasValue ? item.LastChangedDate.Value.DateTime : datetimeNull,
                LastChangedUser = item.LastChangedUser,
                RetireDate = item.RetireDate,
                Status = (short)item.Status,
                Subject = item.Subject
            };

            // Location per gli annessi
            if (item.DocumentSeries.Container.DocumentSeriesAnnexedLocation != null)
            {
                tor.IdLocationAnnexed = item.DocumentSeries.Container.DocumentSeriesAnnexedLocation.Id;
            }

            // Contenitore
            tor.Container = new ContainerWSO
            {
                Id = item.DocumentSeries.Container.Id,
                Name = item.DocumentSeries.Container.Name
            };

            // Subsection. Se attiva e valorizzata
            if (item.DocumentSeries.SubsectionEnabled.GetValueOrDefault(false)
                && item.DocumentSeriesSubsection != null)
            {
                tor.IdDocumentSeriesSubsection = item.DocumentSeriesSubsection.Id;
            }
            // Classificazione
            if (item.Category != null)
            {
                tor.IdCategory = item.Category.Id;
            }
            // Sottoclassificazione
            if (item.SubCategory != null)
            {
                tor.IdSubCategory = item.SubCategory.Id;
            }
            // Documenti della catena principale
            if (includeDocuments)
            {
                var chain = Facade.DocumentSeriesItemFacade.GetMainChainInfo(item);
                if (!chain.Documents.IsNullOrEmpty())
                {
                    tor.MainDocs = BiblosChainToWSO(chain, pdf, includeStream);
                }
            }

            // Documenti annessi
            if (includeAnnexedDocuments)
            {
                var chain = Facade.DocumentSeriesItemFacade.GetAnnexedChainInfo(item);
                if (!chain.Documents.IsNullOrEmpty())
                {
                    tor.AnnexedDocs = BiblosChainToWSO(chain, pdf, includeStream);
                }
            }

            // Documenti non pubblicati 
            if (includeUnPublishedDocuments)
            {
                var chain = Facade.DocumentSeriesItemFacade.GetAnnexedUnpublishedChainInfo(item);
                if (!chain.Documents.IsNullOrEmpty())
                {
                    tor.UnPublishedDocs = BiblosChainToWSO(chain, pdf, includeStream);
                }
            }

            // Dati dinamici
            ArchiveInfo archive = DocumentSeriesFacade.GetArchiveInfo(item.DocumentSeries);
            IDictionary<string, string> dynamicData = Facade.DocumentSeriesItemFacade.GetAttributes(item) ?? new Dictionary<string, string>();

            tor.DynamicData = new List<AttributeWSO>();
            foreach (ArchiveAttribute archiveAttribute in archive.VisibleChainAttributes)
            {
                var a = new AttributeWSO { Key = archiveAttribute.Name };
                // cerco il valore
                if (dynamicData.Keys.Contains(archiveAttribute.Name))
                {
                    a.Value = dynamicData[archiveAttribute.Name];
                }
                tor.DynamicData.Add(a);
            }

            string x = SerializationHelper.SerializeToStringWithoutNamespace(tor);

#if DEBUG
            stopWatch.Stop();
            FileLogger.Debug(LoggerName, string.Format("GetDocumentSeriesItem - Time elapsed: {0} ", stopWatch.Elapsed));
#endif

            return x;
        }

        public string ConsultationItem(int idDocumentSeriesItem, bool includeDocuments, bool includeAnnexedDocuments, bool includeUnPublishedDocuments, bool pdf)
        {
            return ConsultationItem(idDocumentSeriesItem, includeDocuments, includeAnnexedDocuments, includeUnPublishedDocuments, pdf, true);
        }

        public string ConsultationItem(int idDocumentSeriesItem, bool includeDocuments, bool includeAnnexedDocuments, bool includeUnPublishedDocuments, bool pdf, bool onlyPublished)
        {
            try
            {
#if DEBUG
                var stopWatch = new Stopwatch();
                stopWatch.Start();
#endif
                bool includeStream;
                if (!bool.TryParse(ConfigurationManager.AppSettings[AppSettingsConsultationIncludeDocumentStream], out includeStream))
                {
                    includeStream = false;
                }

                string tor = string.Empty;
                if (onlyPublished)
                    tor = GetSeriesItem(idDocumentSeriesItem, includeDocuments, includeAnnexedDocuments, includeUnPublishedDocuments, includeStream, pdf);
                else
                    tor = GetSeriesItem(idDocumentSeriesItem, includeDocuments, includeAnnexedDocuments, includeUnPublishedDocuments, includeStream, pdf, false);

#if DEBUG
                stopWatch.Stop();
                FileLogger.Debug(LoggerName, string.Format("Consultation - Time elapsed: {0} ", stopWatch.Elapsed));
#endif
                return tor;
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public void Update(string xmlSeries)
        {
            //Validazione
            if (string.IsNullOrEmpty(xmlSeries))
                throw new ArgumentException("Xml non valorizzato", "xmlSeries");

            FileLogger.Debug(LoggerName, string.Format("Update - xmlSeries: {0} ", xmlSeries));

            DocumentSeriesItemWSO itemDTO = SerializationHelper.SerializeFromString<DocumentSeriesItemWSO>(xmlSeries);
            DocumentSeriesItem item = Facade.DocumentSeriesItemFacade.GetById(itemDTO.Id);
            if (item == null)
                throw new ArgumentException("Non è stata trovata alcuna documentSeriesItem, verificare identificativo passato", "xmlSeries");

            //Update Static Property
            if (itemDTO.RetireDate.HasValue)
                item.RetireDate = itemDTO.RetireDate;
            if (itemDTO.PublishingDate.HasValue)
                item.PublishingDate = itemDTO.PublishingDate;
            if (itemDTO.Status.HasValue)
                item.Status = (DocumentSeriesItemStatus)itemDTO.Status.Value;
            if (itemDTO.IdDocumentSeries > 0)
                item.DocumentSeries = Facade.DocumentSeriesFacade.GetById(itemDTO.IdDocumentSeries);
            if (itemDTO.IdCategory.HasValue)
                item.Category = Facade.CategoryFacade.GetById(itemDTO.IdCategory.Value);
            if (itemDTO.IdSubCategory.HasValue)
                item.SubCategory = Facade.CategoryFacade.GetById(itemDTO.IdSubCategory.Value);
            if (itemDTO.Subject != null)
                item.Subject = itemDTO.Subject;
            if (itemDTO.LastChangedUser != null)
                item.LastChangedUser = itemDTO.LastChangedUser;
            if (itemDTO.IdDocumentSeriesSubsection.HasValue)
                item.DocumentSeriesSubsection = Facade.DocumentSeriesSubsectionFacade.GetById(itemDTO.IdDocumentSeriesSubsection.Value);

            //Update Dynamic Property
            BiblosChainInfo chain = Facade.DocumentSeriesItemFacade.GetMainChainInfo(item);
            foreach (var attribute in itemDTO.DynamicData)
            {
                chain.AddAttribute(attribute.Key, attribute.Value);
            }

            // Aggiorno l'istanza di documentSeriesItem e il rispettivo mainDocument
            Facade.DocumentSeriesItemFacade.UpdateDocumentSeriesItem(item, chain, $"Registrazione {item.Year:0000}/{item.Number:0000000} modificata da Web Service");

            // é ridondante, ma la inserisco per sicurezza
            Facade.DocumentSeriesItemFacade.ImpersonificatedUpdate(item, itemDTO.LastChangedUser);

            // Invio comando di update alle WebApi
            if (item.Status == DocumentSeriesItemStatus.Active)
            {
                Facade.DocumentSeriesItemFacade.SendUpdateDocumentSeriesItemCommand(item);
            }
        }

        public void AddAnnexed(int id, string nameDocument, string base64DocumentStream)
        {
            // Validazione
            DocumentSeriesItem documentSeriesItem = Facade.DocumentSeriesItemFacade.GetById(id);
            if (documentSeriesItem == null)
                throw new ArgumentException(string.Format("Attenzione DocumentSerieItem con id {0} non presente", id), "id");

            if (string.IsNullOrEmpty(nameDocument))
                throw new ArgumentException("Nome del documento non valorizzato", "nameDocument");

            if (string.IsNullOrEmpty(base64DocumentStream))
                throw new ArgumentException("Stream in base 64 del documento non valorizzato", "base64DocumentStream");

            FileLogger.Debug(LoggerName, string.Format("AddAnnexed - id: {0} nameDocument: {1} base64DocumentStream: {2}", id, nameDocument, base64DocumentStream));

            // Inserimento del documento nella catena degli annessi
            Facade.DocumentSeriesItemFacade.AddAnnexed(documentSeriesItem, new MemoryDocumentInfo
            {
                Name = nameDocument,
                Signature = string.Empty,
                Stream = Convert.FromBase64String(base64DocumentStream)
            });

            Facade.DocumentSeriesItemFacade.Update(ref documentSeriesItem);
            Facade.DocumentSeriesItemLogFacade.AddLog(documentSeriesItem, DocumentSeriesItemLogType.Retire, $"Registrazione {documentSeriesItem.Year:0000}/{documentSeriesItem.Number:0000000} modificata da Web Service");

            // Invio comando di update alle WebApi
            Facade.DocumentSeriesItemFacade.SendUpdateDocumentSeriesItemCommand(documentSeriesItem);
        }

        public string Search(string xmlFinder, bool pdf)
        {
            try
            {
#if DEBUG
                var stopWatch = new Stopwatch();
                stopWatch.Start();
#endif

                var finder = GetFinderFromString(xmlFinder);
                finder.IsPublished = true;

                // Effettuo la ricerca e il calcolo delle tuple totali
                List<DocumentSeriesItemWSO> lstDocumentSeriesItemWSO = new List<DocumentSeriesItemWSO>();

#if DEBUG
                FileLogger.Debug(LoggerName, string.Format("Search - Count - Time elapsed: {0} ", stopWatch.Elapsed));
#endif

                List<DocumentSeriesItemDTO<BiblosDocumentInfo>> lstDocumentSeriesItem = finder.DoSearchAttributesOnly() as List<DocumentSeriesItemDTO<BiblosDocumentInfo>>;
#if DEBUG
                FileLogger.Debug(LoggerName, string.Format("Search - DoSearchAttributesOnly - Time elapsed: {0} ", stopWatch.Elapsed));
#endif
                if (lstDocumentSeriesItem != null)
                {
                    lstDocumentSeriesItemWSO = lstDocumentSeriesItem.Select(x => GetByDTO(x, pdf)).ToList();
                }
                if (DocSuiteContext.Current.ProtocolEnv.SeriesConfigurationOrders.Any(f => lstDocumentSeriesItemWSO.Any(x => x.IdDocumentSeries == f.IdDocumentSeries)))
                {
                    DocumentSeriesItemWSO wso = lstDocumentSeriesItemWSO.FirstOrDefault();
                    DocumentSeriesConfigurationOrder traspConfig = DocSuiteContext.Current.ProtocolEnv.SeriesConfigurationOrders.FirstOrDefault(f => f.IdDocumentSeries == wso.IdDocumentSeries);

                    if (!traspConfig.DynamicColumns.IsNullOrEmpty())
                    {
                        //Presumo che la lista di elementi sia per singola DocumentSeries

                        if (wso.IdDocumentSeries == traspConfig.IdDocumentSeries)
                        {
                            foreach (ColumnOrder column in traspConfig.DynamicColumns)
                            {
                                if (column.Order == System.Data.SqlClient.SortOrder.Ascending)
                                {
                                    lstDocumentSeriesItemWSO = lstDocumentSeriesItemWSO
                                        .OrderBy(o => o.DynamicData.Any(x => x.Key.Eq(column.ColumnName))
                                                ? o.DynamicData.Single(x => x.Key.Eq(column.ColumnName)).Value.ToUpper()
                                                : string.Empty).ToList();
                                }
                                else
                                {
                                    lstDocumentSeriesItemWSO = lstDocumentSeriesItemWSO
                                        .OrderByDescending(o => o.DynamicData.Any(x => x.Key.Eq(column.ColumnName))
                                                ? o.DynamicData.Single(x => x.Key.Eq(column.ColumnName)).Value.ToUpper()
                                                : string.Empty).ToList();
                                }
                            }
                        }
                    }
                }
#if DEBUG
                FileLogger.Debug(LoggerName, string.Format("Search - GetByDTO - Time elapsed: {0} ", stopWatch.Elapsed));
#endif

                // Serializzo i DocumentSeriesItem trovati e li restituisco al chiamante
                string tor = SerializationHelper.SerializeToStringWithoutNamespace(new DocumentSeriesItemResultWSO
                {
                    TotalRowCount = lstDocumentSeriesItemWSO.Count,
                    DocumentSeriesItems = lstDocumentSeriesItemWSO
                });

#if DEBUG
                FileLogger.Debug(LoggerName, string.Format("Search - SerializeToStringWithoutNamespace - Time elapsed: {0} ", stopWatch.Elapsed));

                stopWatch.Stop();
                FileLogger.Debug(LoggerName, string.Format("Search - Time elapsed: {0} ", stopWatch.Elapsed));
#endif

                return tor;
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string SearchRetired(string xmlFinder, bool pdf)
        {
            try
            {
#if DEBUG
                NHibernateProfiler.Initialize();
                var stopWatch = new Stopwatch();
                stopWatch.Start();
#endif

                DocumentSeriesItemFinder finder = GetFinderFromString(xmlFinder);
                finder.IsPublished = null;
                finder.IsRetired = true;

                // Effettuo la ricerca e il calcolo delle tuple totali
                List<DocumentSeriesItemWSO> lstDocumentSeriesItemWSO = new List<DocumentSeriesItemWSO>();

#if DEBUG
                FileLogger.Debug(LoggerName, string.Format("Search - Count - Time elapsed: {0} ", stopWatch.Elapsed));
#endif
                List<DocumentSeriesItemDTO<BiblosDocumentInfo>> lstDocumentSeriesItem = finder.DoSearchAttributesOnly() as List<DocumentSeriesItemDTO<BiblosDocumentInfo>>;
#if DEBUG
                FileLogger.Debug(LoggerName, string.Format("Search - DoSearchAttributesOnly - Time elapsed: {0} ", stopWatch.Elapsed));
#endif
                if (lstDocumentSeriesItem != null)
                {
                    lstDocumentSeriesItemWSO = lstDocumentSeriesItem.Select(x => GetByDTO(x, pdf)).ToList();
                }
#if DEBUG
                FileLogger.Debug(LoggerName, string.Format("Search - GetByDTO - Time elapsed: {0} ", stopWatch.Elapsed));
#endif

                // Serializzo i DocumentSeriesItem trovati e li restituisco al chiamante
                string tor = SerializationHelper.SerializeToStringWithoutNamespace(new DocumentSeriesItemResultWSO
                {
                    TotalRowCount = lstDocumentSeriesItemWSO.Count,
                    DocumentSeriesItems = lstDocumentSeriesItemWSO
                });

#if DEBUG
                FileLogger.Debug(LoggerName, string.Format("Search - SerializeToStringWithoutNamespace - Time elapsed: {0} ", stopWatch.Elapsed));

                stopWatch.Stop();
                FileLogger.Debug(LoggerName, string.Format("Search - Time elapsed: {0} ", stopWatch.Elapsed));
#endif

                return tor;
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public int SearchCount(string xmlFinder)
        {
            try
            {
#if DEBUG
                var stopWatch = new Stopwatch();
                stopWatch.Start();
#endif

                DocumentSeriesItemFinder finder = GetFinderFromString(xmlFinder);
                int tor = finder.Count();

#if DEBUG
                stopWatch.Stop();
                FileLogger.Debug(LoggerName, string.Format("SearchCount - Time elapsed: {0} ", stopWatch.Elapsed));
#endif

                return tor;
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public int SearchCountRetired(string xmlFinder)
        {
            try
            {
#if DEBUG
                var stopWatch = new Stopwatch();
                stopWatch.Start();
#endif

                DocumentSeriesItemFinder finder = GetFinderFromString(xmlFinder);
                finder.IsPublished = null;
                finder.IsRetired = true;
                int tor = finder.Count();

#if DEBUG
                stopWatch.Stop();
                FileLogger.Debug(LoggerName, string.Format("SearchCount - Time elapsed: {0} ", stopWatch.Elapsed));
#endif

                return tor;
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string SearchConstraints(string xmlFinder)
        {
            try
            {
#if DEBUG
                var stopWatch = new Stopwatch();
                stopWatch.Start();
#endif

                DocumentSeriesItemFinder finder = GetFinderFromString(xmlFinder);
                ICollection<string> tor = finder.DoSearchConstraints();

#if DEBUG
                stopWatch.Stop();
                FileLogger.Debug(LoggerName, string.Format("SearchCount - Time elapsed: {0} ", stopWatch.Elapsed));
#endif

                return SerializationHelper.SerializeToStringWithoutNamespace(tor);
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string SearchConstraintsRetired(string xmlFinder)
        {
            try
            {
#if DEBUG
                var stopWatch = new Stopwatch();
                stopWatch.Start();
#endif

                DocumentSeriesItemFinder finder = GetFinderFromString(xmlFinder);
                finder.IsPublished = null;
                finder.IsRetired = true;
                ICollection<string> tor = finder.DoSearchConstraints();

#if DEBUG
                stopWatch.Stop();
                FileLogger.Debug(LoggerName, string.Format("SearchCount - Time elapsed: {0} ", stopWatch.Elapsed));
#endif

                return SerializationHelper.SerializeToStringWithoutNamespace(tor);
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string GetPublicationEnabledContainers()
        {
            try
            {
#if DEBUG
                var stopWatch = new Stopwatch();
                stopWatch.Start();
#endif

                List<ContainerWSO> lstContainerWSO = new List<ContainerWSO>();

                foreach (DocumentSeries series in Facade.DocumentSeriesFacade.GetPublicationEnabledDocumentSeries())
                {
                    //var documentSeries = Facade.DocumentSeriesFacade.GetDocumentSeries(container);
                    IList<DocumentSeriesSubsection> documentSeriesSubsections = Facade.DocumentSeriesSubsectionFacade.GetByDocumentSeries(series) ?? new List<DocumentSeriesSubsection>();

                    lstContainerWSO.Add(new ContainerWSO
                    {
                        Id = series.Container.Id,
                        Name = series.Container.Name,
                        IdDocumentSeries = series.Id,
                        DocumentSeriesSubsections = documentSeriesSubsections.Select(x => new DocumentSeriesSubsectionWSO
                        {
                            Id = x.Id,
                            Description = x.Description,
                            Notes = x.Notes,
                            SortOrder = x.SortOrder
                        }).ToList()
                    });
                }

                string tor = SerializationHelper.SerializeToStringWithoutNamespace(new ResultContainerWSO
                {
                    Containers = lstContainerWSO
                });

#if DEBUG
                stopWatch.Stop();
                FileLogger.Debug(LoggerName, string.Format("Search - Time elapsed: {0} ", stopWatch.Elapsed));
#endif

                return tor;
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string GetFamily(int idFamily, bool onlyPublicationEnabled, bool includeSubsections, int? idArchive)
        {
            try
            {
#if DEBUG
                var stopWatch = new Stopwatch();
                stopWatch.Start();
#endif
                DocumentSeriesFamily family = Facade.DocumentSeriesFamilyFacade.GetById(idFamily);
                DocumentSeriesFamilyWSO wso = FamilyToWSO(family, onlyPublicationEnabled, includeSubsections, idArchive);
                string tor = SerializationHelper.SerializeToStringWithoutNamespace(wso);

#if DEBUG
                stopWatch.Stop();
                FileLogger.Debug(LoggerName, string.Format("GetFamily - Time elapsed: {0} ", stopWatch.Elapsed));
#endif

                return tor;
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string GetDocumentSeries(int idSeries, bool includeSubsections, int? idArchive)
        {
            try
            {
#if DEBUG
                var stopWatch = new Stopwatch();
                stopWatch.Start();
#endif
                DocumentSeries series = null;
                if (idArchive.HasValue)
                    series = Facade.DocumentSeriesFacade.GetSeriesByArchive(idSeries, idArchive.Value);
                else
                    series = Facade.DocumentSeriesFacade.GetById(idSeries);

                if (series == null)
                    return string.Empty;

                DocumentSeriesWSO seriesWSO = SeriesToWSO(
                    series,
                    includeSubsections);
                string tor = SerializationHelper.SerializeToStringWithoutNamespace(seriesWSO);

#if DEBUG
                stopWatch.Stop();
                FileLogger.Debug(LoggerName, string.Format("GetDocumentSeries - Time elapsed: {0} ", stopWatch.Elapsed));
#endif

                return tor;
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string GetFamilies(bool onlyPublicationEnabled, bool includeSubsections, bool includeEmptyFamilies, int? idArchive)
        {
            try
            {
#if DEBUG
                var stopWatch = new Stopwatch();
                stopWatch.Start();
#endif
                // Elenco delle Family da restituire
                List<DocumentSeriesFamilyWSO> tor = new List<DocumentSeriesFamilyWSO>();
                // Recupero l'elenco di tutte le Family da DB
                IList<DocumentSeriesFamily> families = new List<DocumentSeriesFamily>();
                if (idArchive.HasValue)
                    families = Facade.DocumentSeriesFamilyFacade.GetFamiliesByArchive(idArchive.Value);
                else
                    families = Facade.DocumentSeriesFamilyFacade.GetAll();

                foreach (DocumentSeriesFamily family in families)
                {
                    // Credo l'oggetto WSO che rappresenta la Family
                    DocumentSeriesFamilyWSO familyWSO = FamilyToWSO(family, onlyPublicationEnabled, includeSubsections, idArchive);

                    // Se non si vogliono le famiglie vuote 
                    if (!includeEmptyFamilies && familyWSO.DocumentSeries.IsNullOrEmpty()) continue;

                    tor.Add(familyWSO);
                }

                // Restituisco l'elenco delle Famiglie serializzate
                string x = SerializationHelper.SerializeToStringWithoutNamespace(new ResultDocumentSeriesFamilyWSO()
                {
                    DocumentSeriesFamilies = tor
                });

#if DEBUG
                stopWatch.Stop();
                FileLogger.Debug(LoggerName, string.Format("GetFamilies - Time elapsed: {0} ", stopWatch.Elapsed));
#endif

                return x;
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string GetDynamicData(int idDocumentSeries)
        {
            try
            {
#if DEBUG
                var stopWatch = new Stopwatch();
                stopWatch.Start();
#endif

                FileLogger.Debug(LoggerName, string.Format("GetArchiveAttribute - idDocumentSeries: {0} ", idDocumentSeries));

                DocumentSeries currentSeries = Facade.DocumentSeriesFacade.GetById(idDocumentSeries);

                if (currentSeries == null)
                    throw new ArgumentException("Attenzione nessuna DocumentSeries possiede il contenitore indicato", "idDocumentSeries");

                ArchiveInfo archive = DocumentSeriesFacade.GetArchiveInfo(currentSeries);
                List<ArchiveAttributeWSO> result = archive.VisibleChainAttributes.Select(x => new ArchiveAttributeWSO
                {
                    Format = x.Format,
                    Name = x.Name,
                    Required = x.Required,
                    DataType = x.DataType,
                    DefaultValue = x.DefaultValue,
                    Description = x.Description,
                    Disabled = x.Disabled,
                    Id = x.Id,
                    AutoIncremental = x.AutoIncremental,
                    MaxLength = x.MaxLength
                }).ToList();

                string tor = SerializationHelper.SerializeToStringWithoutNamespace(new ResultArchiveAttributeWSO
                {
                    ArchiveAttributes = result
                });

#if DEBUG
                stopWatch.Stop();
                FileLogger.Debug(LoggerName, string.Format("GetDynamicData - Time elapsed: {0} ", stopWatch.Elapsed));
#endif

                return tor;
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string GetMainDocumentWithSignature(int idDocumentSeriesItem, Guid idDoc, string signature)
        {
            try
            {
#if DEBUG
                var stopWatch = new Stopwatch();
                stopWatch.Start();
#endif

                string val = GetSeriesItemDocumentSerialized(idDocumentSeriesItem, idDoc, signature, true, (seriesItem) =>
                {
                    List<BiblosDocumentInfo> docs = Facade.DocumentSeriesItemFacade.GetMainDocuments(seriesItem).ToList();
                    if (docs == null || docs.Count == 0)
                        throw new ArgumentException("Attenzione nessun documento presente con i parametri passati", "idDocumentSeriesItem");
                    return docs;
                });
                
#if DEBUG
                stopWatch.Stop();
                FileLogger.Debug(LoggerName, string.Format("GetMainDocumentWithSignature - Time elapsed: {0} ", stopWatch.Elapsed));
#endif

                return val;
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string GetMainDocument(int idDocumentSeriesItem, Guid idDoc, bool pdf)
        {
            try
            {
#if DEBUG
                var stopWatch = new Stopwatch();
                stopWatch.Start();
#endif

                string val = GetSeriesItemDocumentSerialized(idDocumentSeriesItem, idDoc, string.Empty, pdf, (seriesItem) =>
                {
                    List<BiblosDocumentInfo> docs = Facade.DocumentSeriesItemFacade.GetMainDocuments(seriesItem).ToList();
                    if (docs == null || docs.Count == 0)
                        throw new ArgumentException("Attenzione nessun documento presente con i parametri passati", "idDocumentSeriesItem");
                    return docs;
                });

#if DEBUG
                stopWatch.Stop();
                FileLogger.Debug(LoggerName, string.Format("GetMainDocument - Time elapsed: {0} ", stopWatch.Elapsed));
#endif

                return val;
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string GetAnnexedWithSignature(int idDocumentSeriesItem, Guid idAnnexed, string signature)
        {
            try
            {
#if DEBUG
                var stopWatch = new Stopwatch();
                stopWatch.Start();
#endif

                string val = GetSeriesItemDocumentSerialized(idDocumentSeriesItem, idAnnexed, signature, true, (seriesItem) =>
                {
                    List<BiblosDocumentInfo> docs = Facade.DocumentSeriesItemFacade.GetAnnexedDocuments(seriesItem);
                    if (docs == null || docs.Count == 0)
                        throw new ArgumentException("Attenzione nessun annesso presente con i parametri passati", "idDocumentSeriesItem");
                    return docs;
                });

#if DEBUG
                stopWatch.Stop();
                FileLogger.Debug(LoggerName, string.Format("GetAnnexedWithSignature - Time elapsed: {0} ", stopWatch.Elapsed));
#endif

                return val;
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string GetAnnexed(int idDocumentSeriesItem, Guid idAnnexed, bool pdf)
        {
            try
            {
#if DEBUG
                var stopWatch = new Stopwatch();
                stopWatch.Start();
#endif

                string val = GetSeriesItemDocumentSerialized(idDocumentSeriesItem, idAnnexed, string.Empty, pdf, (seriesItem) =>
                {
                    List<BiblosDocumentInfo> docs = Facade.DocumentSeriesItemFacade.GetAnnexedDocuments(seriesItem);
                    if (docs == null || docs.Count == 0)
                        throw new ArgumentException("Attenzione nessun annesso presente con i parametri passati", "idDocumentSeriesItem");
                    return docs;
                });

#if DEBUG
                stopWatch.Stop();
                FileLogger.Debug(LoggerName, string.Format("GetAnnexed - Time elapsed: {0} ", stopWatch.Elapsed));
#endif

                return val;
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string GetUnPublished(int idDosucmentSeriesItem, Guid idUnpublished, bool pdf)
        {
            try
            {
#if DEBUG
                var stopWatch = new Stopwatch();
                stopWatch.Start();
#endif

                string result = GetSeriesItemDocumentSerialized(idDosucmentSeriesItem, idUnpublished, string.Empty, pdf, (seriesItem) =>
                {
                    List<BiblosDocumentInfo> docs = Facade.DocumentSeriesItemFacade.GetUnpublishedAnnexedDocuments(seriesItem);
                    if (docs == null || docs.Count == 0)
                        throw new ArgumentException("Attenzione nessun annesso presente con i parametri passati", "idDocumentSeriesItem");
                    return docs;
                });

#if DEBUG
                stopWatch.Stop();
                FileLogger.Debug(LoggerName, string.Format("GetUnPublished - Time elapsed: {0} ", stopWatch.Elapsed));
#endif

                return result;
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }

        }

        public string GetUnPublishedWithSignature(int idDocumentSeriesItem, Guid idUnPublished, string signature)
        {
            try
            {
#if DEBUG
                var stopWatch = new Stopwatch();
                stopWatch.Start();
#endif

                string val = GetSeriesItemDocumentSerialized(idDocumentSeriesItem, idUnPublished, signature, true, (seriesItem) =>
                {
                    List<BiblosDocumentInfo> docs = Facade.DocumentSeriesItemFacade.GetUnpublishedAnnexedDocuments(seriesItem);
                    if (docs == null || docs.Count == 0)
                        throw new ArgumentException("Attenzione nessun annesso presente con i parametri passati", "idDocumentSeriesItem");
                    return docs;
                });

#if DEBUG
                stopWatch.Stop();
                FileLogger.Debug(LoggerName, string.Format("GetUnPublishedWithSignature - Time elapsed: {0} ", stopWatch.Elapsed));
#endif

                return val;

            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string GetLatestDocumentSeriesItemByArchive(int idArchive, int topResults)
        {
            try
            {
#if DEBUG
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
#endif
                DateTime? datetimeNull = null;
                List<DocumentSeriesItemWSO> tor = new List<DocumentSeriesItemWSO>();
                IList<DocumentSeriesItem> documentSeriesItems = Facade.DocumentSeriesItemFacade.GetByArchive(idArchive, topResults);
                documentSeriesItems = documentSeriesItems.OrderByDescending(x => x.PublishingDate).ToList();
                if (documentSeriesItems == null || documentSeriesItems.Count == 0)
                    throw new ArgumentException("Attenzione non è stata trovata alcuna DocumentSeriesItem, verificare l'identificativo dell'archivio passato", "idArchive");

                foreach (DocumentSeriesItem item in documentSeriesItems)
                {

                    DocumentSeriesItemWSO torItem = new DocumentSeriesItemWSO
                    {
                        Id = item.Id,
                        IdDocumentSeries = item.DocumentSeries.Id,
                        Year = item.Year,
                        Number = item.Number,
                        IdLocation = item.DocumentSeries.Container.DocumentSeriesLocation.Id,
                        RegistrationUser = item.RegistrationUser,
                        RegistrationDate = item.RegistrationDate.DateTime,
                        PublishingDate = item.PublishingDate,
                        LastChangedDate = item.LastChangedDate.HasValue ? item.LastChangedDate.Value.DateTime : datetimeNull,
                        LastChangedUser = item.LastChangedUser,
                        RetireDate = item.RetireDate,
                        Status = (short)item.Status,
                        Subject = item.Subject
                    };

                    // Location per gli annessi
                    if (item.DocumentSeries.Container.DocumentSeriesAnnexedLocation != null)
                    {
                        torItem.IdLocationAnnexed = item.DocumentSeries.Container.DocumentSeriesAnnexedLocation.Id;
                    }

                    // Contenitore
                    torItem.Container = new ContainerWSO
                    {
                        Id = item.DocumentSeries.Container.Id,
                        Name = item.DocumentSeries.Container.Name
                    };

                    tor.Add(torItem);
                }

#if DEBUG
                stopWatch.Stop();
                FileLogger.Debug(LoggerName, string.Format("GetDocumentSeriesByArchive- Time elapsed: {0} ", stopWatch.Elapsed));
#endif
                return SerializationHelper.SerializeToStringWithoutNamespace(new DocumentSeriesItemResultWSO
                {
                    TotalRowCount = tor.Count(),
                    DocumentSeriesItems = tor
                });

            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string GetArchivesNameByContainerArchiveId(int idArchive)
        {
            try
            {


#if DEBUG
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
#endif
                IList<string> archiveNames = Facade.ContainerFacade.GetArchiveNames(idArchive);
                if (archiveNames == null || archiveNames.Count == 0)
                    throw new ArgumentException("Attenzione non è stato trovato nessun archivio corrispondente al ContainerArchive passato", "idArchive");



#if DEBUG
                stopWatch.Stop();
                FileLogger.Debug(LoggerName, string.Format("GetArchiveNamesByContainerArchiveId - Tipe elapsed: {0}", stopWatch.Elapsed));
#endif
                return SerializationHelper.SerializeToStringWithoutNamespace(archiveNames);
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }


        public string GetDocumentSeriesByDocumentId(Guid idDocument)
        {
            try
            {
#if DEBUG
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
#endif
                IList<DocumentSeriesItem> list = Facade.DocumentSeriesItemFacade.GetByDocumentId(idDocument);
                if (list == null || list.Count == 0 || list.Count > 1)
                    throw new ArgumentException("Attenzione non è stata trovata alcuna DocumentSeriesItem, verificare l'identificativo del documento passato", "idDocument");

                int idSeriesItem = list.First().Id;

#if DEBUG
                stopWatch.Stop();
                FileLogger.Debug(LoggerName, string.Format("GetDocumentSeriesByDocumentId - Time elapsed: {0} ", stopWatch.Elapsed));
#endif

                return idSeriesItem.ToString();
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string GetDocumentSeriesItemsByResolution(int idResolution, bool pdf)
        {
            try
            {
#if DEBUG
                var stopWatch = new Stopwatch();
                stopWatch.Start();
#endif

                Resolution resolution = Facade.ResolutionFacade.GetById(idResolution);
                if (resolution == null)
                    throw new ArgumentException("Attenzione Resolution non presente", "idResolution");

                // Ottengo la lista di DocumentSeriesItem dato una Resolutin
                IList<DocumentSeriesItem> lstDocumentSeriesItem = Facade.ResolutionDocumentSeriesItemFacade.GetDocumentSeriesItems(resolution);

                // Mapping delle DocumentSeriesItem nelle rispettive DocumentSeriesItemWSO
                List<DocumentSeriesItemWSO> result = lstDocumentSeriesItem.Select(x => GetByDSI(x, pdf)).ToList();

#if DEBUG
                stopWatch.Stop();
                FileLogger.Debug(LoggerName, string.Format("GetDocumentSeriesItemByResolution - Time elapsed: {0} ", stopWatch.Elapsed));
#endif

                return SerializationHelper.SerializeToStringWithoutNamespace(new DocumentSeriesItemResultWSO
                {
                    TotalRowCount = result.Count(),
                    DocumentSeriesItems = result
                });
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string GetIndex(int idSeries, string impersonatingUser, string urlFile, string titolo, string @abstract, string entePubblicatore, string licenza, string urlMask, int? year = null, bool checkPublished = true)
        {
            try
            {
#if DEBUG
                var stopWatch = new Stopwatch();
                stopWatch.Start();
#endif

                DocumentSeriesItemFinder finder = new DocumentSeriesItemFinder(true, impersonatingUser);
                finder.EnablePaging = false;
                finder.IdDocumentSeriesIn = new List<int> { idSeries };
                finder.ItemStatusIn = new List<DocumentSeriesItemStatus> { DocumentSeriesItemStatus.Active };
                if (checkPublished)
                {
                    finder.IsPublished = true;
                }
                finder.Year = year;

                IList<DocumentSeriesItem> items = finder.DoSearch();
                string index = AVCPHelper.GetIndexFile(urlFile, titolo, @abstract, entePubblicatore, licenza, items, urlMask, year);

#if DEBUG
                stopWatch.Stop();
                FileLogger.Debug(LoggerName, string.Format("GetIndex - Time elapsed: {0} ", stopWatch.Elapsed));
#endif

                return index;
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        #endregion

        #region [ Methods ]
        private DocumentSeriesWSO SeriesToWSO(DocumentSeries series, bool includeSubsections)
        {
            // Creo l'oggetto DocumentSeries per la risposta
            DocumentSeriesWSO seriesWSO = new DocumentSeriesWSO
            {
                Id = series.Id,
                Name = series.Container.Name,
                IdContainer = series.Container.Id,
                PublicationEnabled = series.PublicationEnabled
            };

            // Se le sottosezioni sono attive new restituisco l'elenco
            if (includeSubsections && series.SubsectionEnabled.GetValueOrDefault(false))
            {
                IList<DocumentSeriesSubsection> subsections = Facade.DocumentSeriesSubsectionFacade.GetByDocumentSeries(series);
                seriesWSO.DocumentSeriesSubsections = subsections.Select(x => new DocumentSeriesSubsectionWSO
                {
                    Id = x.Id,
                    Description = x.Description,
                    Notes = x.Notes,
                    SortOrder = x.SortOrder
                }).ToList();
            }
            return seriesWSO;
        }

        private DocumentSeriesFamilyWSO FamilyToWSO(DocumentSeriesFamily family, bool onlyPublicationEnabled, bool includeSubsections, int? idArchive)
        {
            // Credo l'oggetto WSO che rappresenta la Family
            DocumentSeriesFamilyWSO familyWSO = new DocumentSeriesFamilyWSO
            {
                Id = family.Id,
                Name = family.Name,
                Description = family.Description,
                DocumentSeries = new List<DocumentSeriesWSO>()
            };

            IList<DocumentSeries> documentSeries = new List<DocumentSeries>();
            if (idArchive.HasValue)
                documentSeries = Facade.DocumentSeriesFacade.GetSeriesByFamilyAndArchive(family.Id, idArchive.Value);
            else
                documentSeries = family.DocumentSeries;

            // Eseguo un ciclo per ogni Serie Documentale della Famiglia
            foreach (DocumentSeries series in documentSeries.OrderBy(x => x.SortOrder))
            {
                if (onlyPublicationEnabled && !series.PublicationEnabled.GetValueOrDefault(false)) continue;

                // Creo l'oggetto DocumentSeries per la risposta
                DocumentSeriesWSO seriesWSO = SeriesToWSO(series, includeSubsections);
                // Aggiungo la Serie alla Famiglia
                familyWSO.DocumentSeries.Add(seriesWSO);
            }

            return familyWSO;
        }

        private DocumentSeriesItemFinder GetFinderFromString(string xmlFinder)
        {
            if (string.IsNullOrEmpty(xmlFinder))
                throw new ArgumentException("Xml non valorizzato", "xmlFinder");

            FileLogger.Debug(LoggerName, string.Format("Search - xmlFinder: {0}", xmlFinder));

            // Parsing del finder
            DocumentSeriesItemFinderWSO finderWSO = SerializationHelper.SerializeFromString<DocumentSeriesItemFinderWSO>(xmlFinder);

            DocumentSeries currentSeries = Facade.DocumentSeriesFacade.GetById(finderWSO.IdDocumentSeries);

            // Se non specifico l'id della documentSeries e ho dei metadati lancio eccezione
            if (currentSeries == null && finderWSO.DynamicData != null && finderWSO.DynamicData.Count > 0)
                throw new DocSuiteException("Errore dati", string.Format("È obbligatorio indicare una {0} valida se si eseguono ricerche per Attributo", DocSuiteContext.Current.ProtocolEnv.DocumentSeriesName));

            // Ricostruisco il Finder
            DocumentSeriesItemFinder finder = GetFinder(finderWSO, currentSeries);

            // Filtro per attributi di Biblos
            if (!finderWSO.DynamicData.IsNullOrEmpty())
            {
                List<SearchCondition> conditions = new List<SearchCondition>();
                foreach (var attributeWso in finderWSO.DynamicData)
                {
                    SearchConditionOperator operatorCondition;
                    // Tento di recuperare l'operatore. Se non ci riesco imposto Contains di default
                    if (!Enum.TryParse(attributeWso.Operator, true, out operatorCondition))
                    {
                        operatorCondition = SearchConditionOperator.Contains;
                    }

                    SearchCondition condition = new SearchCondition
                    {
                        AttributeName = attributeWso.Key,
                        AttributeValue = attributeWso.Value,
                        Operator = operatorCondition
                    };
                    conditions.Add(condition);
                }
                Facade.DocumentSeriesFacade.FillFinder(finder, conditions);
            }

            return finder;
        }

        private static DocWSO ConvertToDocWSO(BiblosDocumentInfo document, bool pdf, bool includeStream, string signature)
        {
            DocWSO tor = new DocWSO
            {
                Id = document.DocumentId,
                Server = document.Server
            };

            if (pdf && !ExcludedExtensions.Any(x => x.Eq(document.Extension)))
            {
                tor.Name = document.PDFName;
                tor.Stream = includeStream ? Convert.ToBase64String(document.GetPdfStream(signature)) : null;
                return tor;
            }

            tor.Name = document.Name;
            tor.Stream = includeStream ? Convert.ToBase64String(document.Stream) : null;
            return tor;
        }

        private static List<DocWSO> ConvertToDocWSOList(List<BiblosDocumentInfo> documents, bool pdf, bool includeStream)
        {
            if (DocSuiteContext.Current.ProtocolEnv.DocumentSeriesReorderDocumentEnabled)
            {
                documents = BiblosFacade.SortDocuments(documents);
            }
            return documents.Select(d => ConvertToDocWSO(d, pdf, includeStream, string.Empty)).ToList();
        }

        /// <summary>
        /// Dato un DocumentSeriesItemDTO ritornato dal finder ritorno un oggetto DocumentSeriesItemWSO opportunamente formattato 
        /// ( da utilizzare per la creazione dell'xml di ritorno )
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="pdf"> </param>
        /// <returns></returns>
        private static DocumentSeriesItemWSO GetByDTO(DocumentSeriesItemDTO<BiblosDocumentInfo> dto, bool pdf)
        {
            DocumentSeriesItemWSO wso = new DocumentSeriesItemWSO();
            wso.Id = dto.Id;
            wso.IdDocumentSeries = dto.IdDocumentSeries;
            wso.IdDocumentSeriesSubsection = dto.IdDocumentSeriesSubsection;
            wso.Year = dto.Year;
            wso.Number = dto.Number;
            wso.IdLocation = dto.IdLocation;
            wso.IdLocationAnnexed = dto.IdLocationAnnexed;
            wso.RegistrationUser = dto.RegistrationUser;
            wso.RegistrationDate = dto.RegistrationDate.HasValue ? dto.RegistrationDate.Value.DateTime : default(DateTime?);
            wso.PublishingDate = dto.PublishingDate;
            wso.LastChangedDate = dto.LastChangedDate.HasValue ? dto.LastChangedDate.Value.DateTime : default(DateTime?);
            wso.LastChangedUser = dto.LastChangedUser;
            wso.RetireDate = dto.RetireDate;
            wso.Status = (short?)dto.Status;
            wso.Subject = dto.Subject;
            wso.Container = new ContainerWSO();
            wso.Container.Id = dto.IdContainer;
            wso.Container.Name = dto.ContainerName;
            wso.Priority = dto.Priority;

            if (dto.Category != null)
                wso.IdCategory = dto.Category.Id;
            if (dto.SubCategory != null)
                wso.IdSubCategory = dto.SubCategory.Id;

            if (dto.Attributes != null)
                wso.DynamicData = dto.Attributes.Select(a => new AttributeWSO { Key = a.Key, Value = a.Value }).ToList();

            // Carico l'identificativo DocumentSeriesSubsection
            if (dto.DocumentSeriesSubsection != null) wso.IdDocumentSeriesSubsection = dto.DocumentSeriesSubsection.Id;

            // Documento principale
            if (!dto.MainDocuments.IsNullOrEmpty())
                wso.MainDocs = ConvertToDocWSOList(dto.MainDocuments, pdf, false);

            //Annessi
            if (!dto.Annexed.IsNullOrEmpty())
                wso.AnnexedDocs = ConvertToDocWSOList(dto.Annexed, pdf, false);

            //Annessi non pubblicati
            if (!dto.UnPublishedAnnexed.IsNullOrEmpty())
                wso.UnPublishedDocs = ConvertToDocWSOList(dto.UnPublishedAnnexed, pdf, false);


            return wso;
        }

        /// <summary>
        /// Dato un DocumentSeriesItem mi restituisce il rispettivo wrapper WSO con valorizzati i rispettivi MainDocument e Annessi
        /// </summary>
        /// <param name="dsi"></param>
        /// <param name="pdf"></param>
        /// <returns></returns>
        private DocumentSeriesItemWSO GetByDSI(DocumentSeriesItem dsi, bool pdf)
        {
            DateTime? datetimeNull = null;
            // Carico il rispettivo documento principale sse withDocument valorizzato a True
            List<BiblosDocumentInfo> docs = Facade.DocumentSeriesItemFacade.GetMainDocuments(dsi).ToList();
            if (docs.IsNullOrEmpty())
                throw new InvalidOperationException("Attenzione la documentSeries non possiede documenti principali");

            // Carico i Documenti Proncipali
            List<DocWSO> mainDocs = ConvertToDocWSOList(docs, pdf, false);

            // Carico gli Annessi
            List<BiblosDocumentInfo> annexed = Facade.DocumentSeriesItemFacade.GetAnnexedDocuments(dsi);
            List<DocWSO> annexedDocs = null;
            if (!annexed.IsNullOrEmpty())
                annexedDocs = ConvertToDocWSOList(annexed, pdf, false);

            // Recupero i dynamicData
            List<AttributeWSO> dynamicData = Facade.DocumentSeriesItemFacade.GetAttributes(dsi)
                .Select(x => new AttributeWSO { Key = x.Key, Value = x.Value })
                .ToList();

            // DocumentSeriesSubsection
            int? idSubsection = dsi.DocumentSeriesSubsection != null
                                    ? dsi.DocumentSeriesSubsection.Id
                                    : (int?)null;

            return new DocumentSeriesItemWSO
            {
                Id = dsi.Id,
                IdDocumentSeries = dsi.DocumentSeries.Id,
                Year = dsi.Year,
                Number = dsi.Number,
                IdLocation = dsi.DocumentSeries.Container.DocumentSeriesLocation.Id,
                IdLocationAnnexed = dsi.DocumentSeries.Container.DocumentSeriesAnnexedLocation.Id,
                RegistrationUser = dsi.RegistrationUser,
                RegistrationDate = dsi.RegistrationDate.DateTime,
                PublishingDate = dsi.PublishingDate,
                LastChangedDate = dsi.LastChangedDate.HasValue ? dsi.LastChangedDate.Value.DateTime : datetimeNull,
                LastChangedUser = dsi.LastChangedUser,
                RetireDate = dsi.RetireDate,
                Status = (short)dsi.Status,
                Subject = dsi.Subject,
                IdDocumentSeriesSubsection = idSubsection,
                Container = new ContainerWSO
                {
                    Id = dsi.DocumentSeries.Container.Id,
                    Name = dsi.DocumentSeries.Container.Name
                },
                IdCategory = dsi.Category != null ? dsi.Category.Id : (int?)null,
                IdSubCategory = dsi.SubCategory != null ? dsi.SubCategory.Id : (int?)null,
                DynamicData = dynamicData,
                MainDocs = mainDocs,
                AnnexedDocs = annexedDocs

            };
        }

        private DocumentSeriesItemWSO copySeriesItemToWso(DocumentSeriesItem item)
        {
            // DocumentSeriesSubsection
            int? idSubsection = item.DocumentSeriesSubsection != null
                                    ? item.DocumentSeriesSubsection.Id
                                    : (int?)null;

            return new DocumentSeriesItemWSO
            {
                Id = item.Id,
                IdDocumentSeries = item.DocumentSeries.Id,
                Year = item.Year,
                Number = item.Number,
                IdLocation = item.DocumentSeries.Container.DocumentSeriesLocation.Id,
                IdLocationAnnexed = item.DocumentSeries.Container.DocumentSeriesAnnexedLocation.Id,
                RegistrationUser = item.RegistrationUser,
                RegistrationDate = item.RegistrationDate.DateTime,
                PublishingDate = item.PublishingDate,
                LastChangedDate = item.LastChangedDate.HasValue ? item.LastChangedDate.Value.DateTime : DateTime.Now,
                LastChangedUser = item.LastChangedUser,
                RetireDate = item.RetireDate,
                Status = (short)item.Status,
                Subject = item.Subject,
                IdDocumentSeriesSubsection = idSubsection,
                Container = new ContainerWSO
                {
                    Id = item.DocumentSeries.Container.Id,
                    Name = item.DocumentSeries.Container.Name
                },
                IdCategory = item.Category != null ? item.Category.Id : (int?)null,
                IdSubCategory = item.SubCategory != null ? item.SubCategory.Id : (int?)null,
            };
        }

        /// <summary>
        /// Crea il finder a partire dal rispettivo oggetto WSO
        /// </summary>
        /// <param name="finderWSO"></param>
        /// <param name="currentSeries"></param>
        /// <returns>finder per Oggetti del tipo DocumentSeriesItem</returns>
        private DocumentSeriesItemFinder GetFinder(DocumentSeriesItemFinderWSO finderWSO, DocumentSeries currentSeries)
        {
            List<int> idDocumentSeriesIn = new List<int>();
            if (currentSeries != null)
                idDocumentSeriesIn.Add(currentSeries.Id);

            DocumentSeriesItemFinder finder = new DocumentSeriesItemFinder(false, finderWSO.ImpersonatingUser)
            {
                IdDocumentSeriesIn = idDocumentSeriesIn,
                IdSubsectionIn = finderWSO.IdDocumentSeriesSubsections,
                Year = finderWSO.Year,
                NumberFrom = finderWSO.NumberFrom,
                NumberTo = finderWSO.NumberTo,
                RegistrationDateFrom = finderWSO.RegistrationDateFrom,
                RegistrationDateTo = finderWSO.RegistrationDateTo,
                RetireDateFrom = finderWSO.RetireDateFrom,
                RetireDateTo = finderWSO.RetireDateTo,
                EnablePaging = finderWSO.EnablePaging.GetValueOrDefault(true),
                IsPublished = finderWSO.IsPublished,
                IsRetired = finderWSO.IsRetired,
                IsPriority = finderWSO.IsPriority,
                SubjectContains = finderWSO.SubjectContains,
                SubjectStartsWith = finderWSO.SubjectStartsWith,
                CategoryPath = finderWSO.CategoryPath,
                IncludeSubsections = finderWSO.IncludeSubsections
            };

            if (finderWSO.PublishingDateFrom.HasValue)
            {
                finder.PublishingDateFrom = finderWSO.PublishingDateFrom.Value.Date;
            }

            if (finderWSO.PublishingDateTo.HasValue)
            {
                finder.PublishingDateTo = finderWSO.PublishingDateTo.Value.Date;
            }

            if (finderWSO.PublishingYear.HasValue)
            {
                finder.PublishingDateFrom = new DateTime(finderWSO.PublishingYear.Value, 1, 1);
                finder.PublishingDateTo = new DateTime(finderWSO.PublishingYear.Value, 12, 31);
            }

            // Gestione della paginazione
            if (finder.EnablePaging)
            {
                if (finderWSO.Skip.HasValue && finderWSO.Take.HasValue)
                {
                    finder.PageIndex = finderWSO.Skip.Value;
                    finder.PageSize = finderWSO.Take.Value;
                }
                else
                    throw new InvalidOperationException("Identificatori di pagina mancanti o non validi.");
            }

            // Solo Item ATTIVI
            finder.ItemStatusIn = new List<DocumentSeriesItemStatus> { DocumentSeriesItemStatus.Active };

            // Ordinamento di default per Pubblicazione Web
            finder.SortExpressions.Add("DSI.PublishingDate", "DESC");
            finder.SortExpressions.Add("DSI.Year", "DESC");
            finder.SortExpressions.Add("DSI.Number", "DESC");

            finder.LastModifiedSortingView = finderWSO.LastModifiedSortingView;
            finder.FindByConstraints = finderWSO.FindByConstraints;
            finder.Constraint = finderWSO.Constraint;
            return finder;
        }

        private List<DocWSO> BiblosChainToWSO(BiblosChainInfo chain, bool pdf, bool includeStream)
        {
            if (!chain.ArchivedDocuments.IsNullOrEmpty())
                return ConvertToDocWSOList(chain.ArchivedDocuments, pdf, includeStream);

            return null;
        }

        private string GetSeriesItemDocumentSerialized(int idDocumentSeriesItem, Guid idDocument, string signature, bool pdf, Func<DocumentSeriesItem, ICollection<BiblosDocumentInfo>> findDocumentsAction)
        {
            DocumentSeriesItem seriesItem = Facade.DocumentSeriesItemFacade.GetById(idDocumentSeriesItem);
            bool consultationHistoryEnable;
            if (!bool.TryParse(ConfigurationManager.AppSettings[AppSettingsHistoryConsultationEnable], out consultationHistoryEnable) || !consultationHistoryEnable)
            {
                if (!seriesItem.IsPublished())
                    throw new InvalidOperationException("Attenzione DocumentSeriesItem non pubblicata, verificare identificativo passato");
            }

            ICollection<BiblosDocumentInfo> list = findDocumentsAction(seriesItem);

            //Verifico cheil documento esista
            int countDocs = list.Count(x => x.DocumentId == idDocument);
            if (countDocs == 0 || countDocs > 1)
                throw new InvalidOperationException("Attenzione nessun documento della DocumentSeriesItem possiede l'identificativo passato");

            BiblosDocumentInfo doc = list.First(x => x.DocumentId == idDocument);

            DocumentSeriesItemWSO wso = copySeriesItemToWso(seriesItem);
            string formattedSignature = !string.IsNullOrEmpty(signature) ? string.Format(new StringCustomFormatInfo<DocumentSeriesItemWSO>(), signature, wso) : string.Empty;
            DocWSO tor = ConvertToDocWSO(doc, pdf, true, formattedSignature);

            return SerializationHelper.SerializeToStringWithoutNamespace(new DocResultWSO
            {
                Docs = new List<DocWSO> { tor }
            });
        }
#endregion

    }
}
