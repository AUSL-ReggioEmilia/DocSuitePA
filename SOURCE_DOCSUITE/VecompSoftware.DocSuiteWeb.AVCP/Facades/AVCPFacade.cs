using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.AVCP.Entities;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.Helpers;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.Services.Biblos;
using VecompSoftware.Services.Biblos.Models;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.DocSuiteWeb.AVCP
{
    public class AVCPFacade
    {
        #region Consts
        public const string LoggerName = "AVCPFacade";

        private const string AttributeIdentifier = "Identifier";
        /// <summary> int64 </summary>
        private const string AttributeAnno = "Anno";
        /// <summary> stringa </summary>
        private const string AttributeCodiceSettore = "CodiceSettore";
        /// <summary> int64 </summary>
        private const string AttributeNumero = "Numero";
        /// <summary> int64  0 false 1 true </summary>
        private const string AttributeChiusa = "Chiusa";
        /// <summary> datetime nullable </summary>

        public const string XSDPath = "Config\\AVCP\\datasetAppaltiL190.xsd";

        private const string AttributeDataUltimoAggiornamento = "DataUltimoAggiornamento";
        private const string AttributeUrlFile = "UrlFile";
        private const string AttributeLicenza = "Licenza";
        private const string AttributeAbstract = "Abstract";
        private const string AttributeAnnoRiferimento = "AnnoRiferimento";
        private const string AttributeTitolo = "Titolo";
        private const string AttributeDataPubblicazione = "DataPubblicazione";
        private const string AttributeEntePubblicatore = "EntePubblicatore";


        private const string FieldSeparator = "; ";
        private const string WordSeparator = " - ";

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Percorso del XSD di validazione dell'XML di pubblicazione
        /// </summary>
        public string XSDFullPath
        {
            get { return AppDomain.CurrentDomain.BaseDirectory + XSDPath; }
        }

        public SetDataSetResult SetDataSetPub(pubblicazione pub, DocumentSeriesItem item, string username)
        {
            return SetDataSetPub(pub, item, username, false);
        }

        /// <summary>
        /// Metodo per il salvataggio di un DataSet in Serie AVCP
        /// </summary>
        /// <param name="pub">Oggetto da salvare</param>
        /// <param name="item">DocumentSeriesItem in cui deve essere salvato</param>
        /// <param name="username">Nome dell'operatore che esegue il savataggio</param>
        /// <returns>Restituisce l'oggetto contenente tutte le informazioni sull'importazione eseguita</returns>
        public SetDataSetResult SetDataSetPub(pubblicazione pub, DocumentSeriesItem item, string username, bool saveAlways)
        {
            if (pub == null)
                throw new ArgumentNullException("pub", "Oggetto [pubblicazione] nullo");

            // Creo l'oggetto di ritorno
            var tor = new SetDataSetResult();
            tor.Item = item;

            try
            {
                // Recupero il documento principale
                tor.Chain = FacadeFactory.Instance.DocumentSeriesItemFacade.GetMainChainInfo(tor.Item);
            }
            catch (Exception ex)
            {
                throw new SetDataSetResultException(tor, "Errore in fase di recupero documento catena principale.", ex);
            }

            try
            {
                // Verifico data ultimo aggiornamento
                tor.Step = 100;

                if (!saveAlways && !tor.Chain.Documents.IsNullOrEmpty() && tor.Chain.Attributes.ContainsKey(AttributeDataUltimoAggiornamento))
                {
                    DateTime lastUpdate = DateTime.MinValue;
                    if (!DateTime.TryParse(tor.Chain.Attributes[AttributeDataUltimoAggiornamento], out lastUpdate))
                    {
                        tor.Chain.AddAttribute(AttributeDataUltimoAggiornamento, pub.metadata.dataUltimoAggiornamentoDataset.ToString("s"));
                    }
                    tor.LastUpdate = lastUpdate;
                    FileLogger.Debug(LoggerName, string.Format("tor.LastUpdate = {0}", tor.LastUpdate.Date));
                    FileLogger.Debug(LoggerName, string.Format("pub.metadata.dataUltimoAggiornamentoDataset.Date = {0}", pub.metadata.dataUltimoAggiornamentoDataset.Date));

                    // Aggiorno solo se la data in arrivo è maggiore di quella salvata
                    if (pub.metadata.dataUltimoAggiornamentoDataset.Date <= tor.LastUpdate.Date)
                    {
                        tor.Updated = false;
                        return tor;
                    }
                }
                tor.Updated = true;
            }
            catch (Exception ex)
            {
                throw new SetDataSetResultException(tor, "Errore in fase di verifica data ultimo aggiornamento.", ex);
            }

            try
            {
                // Aggiornamento necessario, eseguo eventaule Flush del documento
                tor.Step = 200;
                if (!tor.Chain.Documents.IsNullOrEmpty())
                {
                    FlushDocuments(tor.Item);
                    tor.Flushed = true;
                    // Ricarico la nuova catena creata
                    tor.Chain = FacadeFactory.Instance.DocumentSeriesItemFacade.GetMainChainInfo(tor.Item);
                }
            }
            catch (Exception ex)
            {
                throw new SetDataSetResultException(tor, "Errore in fase di Flush del documento.", ex);
            }

            try
            {
                // Salvo il documento nella catena
                tor.Step = 300;
                tor.SerializedDataSet = AVCPHelper.Serialize(pub);
                var doc = new MemoryDocumentInfo(tor.SerializedDataSet.ToBytes(), "dataset.xml", "");
                tor.Chain.AddDocument(doc);
            }
            catch (Exception ex)
            {
                throw new SetDataSetResultException(tor, "Errore in fase di salvataggio del documento in catena.", ex);
            }

            try
            {
                // Riporto i Metadati in Serie Documentale
                tor.Step = 400;

                tor.Chain.AddAttribute(AttributeUrlFile, string.Format(DocSuiteContext.Current.ProtocolEnv.AVCPDatasetUrlMask, tor.Item.Id));
                if (pub.metadata.dataUltimoAggiornamentoDataset != DateTime.MinValue)
                    tor.Chain.AddAttribute(AttributeDataUltimoAggiornamento, pub.metadata.dataUltimoAggiornamentoDataset.ToString("s"));
                if (pub.metadata.licenza != null)
                    tor.Chain.AddAttribute(AttributeLicenza, pub.metadata.licenza.ToString());
                tor.Chain.AddAttribute(AttributeAbstract, pub.metadata.@abstract);
                tor.Chain.AddAttribute(AttributeAnnoRiferimento, pub.metadata.annoRiferimento.ToString());
                tor.Chain.AddAttribute(AttributeTitolo, pub.metadata.titolo);
                if (pub.metadata.dataPubbicazioneDataset != DateTime.MinValue)
                    tor.Chain.AddAttribute(AttributeDataPubblicazione, pub.metadata.dataPubbicazioneDataset.ToString("s"));
                tor.Chain.AddAttribute(AttributeEntePubblicatore, pub.metadata.entePubblicatore);
            }
            catch (Exception ex)
            {
                throw new SetDataSetResultException(tor, "Errore in fase di impostazione attributi in catena.", ex);
            }

            try
            {
                // Eseguo il salvataggio dell'Item
                tor.Step = 500;
                FacadeFactory.Instance.DocumentSeriesItemFacade.UpdateDocumentSeriesItem(tor.Item, tor.Chain, username, $"Modificata registrazione AVCP {tor.Item.Year:0000}/{tor.Item.Number:0000000}");

                // Invio comando di update alle WebApi
                if (tor.Item.Status == DocumentSeriesItemStatus.Active)
                {
                    FacadeFactory.Instance.DocumentSeriesItemFacade.SendUpdateDocumentSeriesItemCommand(tor.Item);
                }

                tor.Saved = true;
            }
            catch (Exception ex)
            {
                throw new SetDataSetResultException(tor, "Errore in fase di salvataggio finale.", ex);
            }

            return tor;
        }

        public SetDataSetResult SetDataSetPub(pubblicazione pub, DocumentSeriesItem item)
        {
            return SetDataSetPub(pub, item, DocSuiteContext.Current.User.FullUserName);
        }

        /// <summary>
        /// Crea o aggiorna la serie documentale bandi di gara e contratti associata al provvedimento
        /// passato per parametro
        /// </summary>
        /// <param name="pub"></param>
        /// <param name="resolution"></param>
        public void CreateOrUpdateBandiDiGaraSeriesItem(pubblicazione pub, Resolution resolution)
        {
            if (pub == null)
            {
                FileLogger.Debug(LoggerName, "CreateOrUpdateBandiDiGaraSeries => parametro pub è nullo");
                return;
            }

            if (resolution == null)
            {
                FileLogger.Debug(LoggerName, "CreateOrUpdateBandiDiGaraSeries => parametro resolution è nullo");
                return;
            }

            if (!DocSuiteContext.Current.ProtocolEnv.BandiGaraDocumentSeriesId.HasValue)
            {
                FileLogger.Debug(LoggerName, "CreateOrUpdateBandiDiGaraSeries => parametro ProtocolEnv.BandiGaraDocumentSeriesId non definito");
                return;
            }

            try
            {
                //Recupero le documentseriesitem associate al provvedimento
                ICollection<DocumentSeriesItem> docSeriesLikedToResl = FacadeFactory.Instance.ResolutionFacade.GetSeriesByResolution(resolution);
                DocumentSeriesItem bandiGaraDocumentSeriesItem = docSeriesLikedToResl.Where(x => x.DocumentSeries.Id == DocSuiteContext.Current.ProtocolEnv.BandiGaraDocumentSeriesId.Value).FirstOrDefault();
                if (bandiGaraDocumentSeriesItem == null)
                {
                    //Se non esiste nessuna documentseriesitem bandi di gara e contratti associata al provvedimento allora la genero.
                    bandiGaraDocumentSeriesItem = new DocumentSeriesItem()
                    {
                        Status = DocumentSeriesItemStatus.Active,
                        Subject = resolution.ResolutionObject
                    };

                    DocumentSeries bandiGaraDocumentSeries = FacadeFactory.Instance.DocumentSeriesFacade.GetById(DocSuiteContext.Current.ProtocolEnv.BandiGaraDocumentSeriesId.Value);
                    bandiGaraDocumentSeriesItem.DocumentSeries = bandiGaraDocumentSeries;

                    //Se esiste recupero il classificatore di default associato al contenitore
                    ContainerBehaviour behaviour = FacadeFactory.Instance.ContainerBehaviourFacade.GetBehaviours(bandiGaraDocumentSeries.Container, ContainerBehaviourAction.Insert, "#uscClassificatori").FirstOrDefault();
                    Category category = null;
                    if (behaviour != null)
                    {
                        category = FacadeFactory.Instance.CategoryFacade.GetById(int.Parse(behaviour.AttributeValue));
                    }

                    //Se non ho trovato il classificatore di default uso quello del provvedimento
                    if (category == null)
                    {
                        category = resolution.SubCategory != null ? resolution.SubCategory : resolution.Category;
                    }

                    if (category.Root == category)
                    {
                        bandiGaraDocumentSeriesItem.Category = category;
                    }
                    else
                    {
                        bandiGaraDocumentSeriesItem.Category = category.Root;
                        bandiGaraDocumentSeriesItem.SubCategory = category;
                    }

                    BiblosChainInfo newChain = new BiblosChainInfo();

                    //Salvo la documentseriesitem
                    bandiGaraDocumentSeriesItem = FacadeFactory.Instance.DocumentSeriesItemFacade.SaveDocumentSeriesItem(bandiGaraDocumentSeriesItem, newChain, null, null, DocSuiteContext.Current.User.FullUserName);
                    //La collego al provvedimento che sto gestendo
                    LinkToResolution(bandiGaraDocumentSeriesItem, resolution);
                }

                ArchiveInfo bandiGaraArchiveInfo = DocumentSeriesFacade.GetArchiveInfo(bandiGaraDocumentSeriesItem.DocumentSeries);
                BiblosChainInfo chain = FacadeFactory.Instance.DocumentSeriesItemFacade.GetMainChainInfo(bandiGaraDocumentSeriesItem);
                // update campi dinamici DocumentSeriesItem
                chain = UpdateAttributeBandiDiGara(bandiGaraDocumentSeriesItem, pub, bandiGaraArchiveInfo, chain);
                // salvo il DocumentSeriesItem e aggiorno gli attributi della serie documentale
                FacadeFactory.Instance.DocumentSeriesItemFacade.UpdateDocumentSeriesItem(bandiGaraDocumentSeriesItem, chain, $"Aggiornamento metadati della serie bandi di gara e contratti {bandiGaraDocumentSeriesItem.Year:0000}/{bandiGaraDocumentSeriesItem.Number:0000000} da pubblicazione AVCP");
                FileLogger.Info(LoggerName, string.Format("CreateOrUpdateBandiDiGaraSeries => modificata BandiGaraDocumentSeries con ID {0}", bandiGaraDocumentSeriesItem.Id));
                if (bandiGaraDocumentSeriesItem.Status == DocumentSeriesItemStatus.Active)
                {
                    FacadeFactory.Instance.DocumentSeriesItemFacade.SendUpdateDocumentSeriesItemCommand(bandiGaraDocumentSeriesItem);
                }
            }
            catch (Exception ex)
            {
                FileLogger.Error(LoggerName, string.Format("CreateOrUpdateBandiDiGaraSeriesItem => Errore nella generazione della serie Bandi di Gara e Contratti. {0}", ex.StackTrace));
            }
        }

        /// <summary>
        /// Aggiorna gli attributi della serie documentale per bandi di gara e contratti
        /// </summary>
        /// <param name="item"></param>
        /// <param name="pub"></param>
        /// <param name="archiveInfo"></param>
        /// <param name="chain"></param>
        /// <returns></returns>
        public BiblosChainInfo UpdateAttributeBandiDiGara(DocumentSeriesItem item, pubblicazione pub, ArchiveInfo archiveInfo, BiblosChainInfo chain)
        {
            foreach (ArchiveAttribute attribute in archiveInfo.Attributes)
            {
                string valueString = string.Empty;
                DynamicAttribute attr;

                try
                {
                    attr = EnumHelper.ParseDescriptionToEnum<DynamicAttribute>(attribute.Name);
                }
                catch (Exception)
                {
                    continue;
                }

                switch (attr)
                {
                    case DynamicAttribute.Aggiudicatario:
                        valueString = this.GetAziendeAggiudicatarie(pub);
                        break;
                    case DynamicAttribute.Lotti:
                        valueString = this.GetLotti(pub);
                        break;
                    case DynamicAttribute.Liquidato:
                        valueString = this.GetImportoSommeLiquidate(pub).ToString();
                        break;
                    case DynamicAttribute.DitteInvitate:
                        valueString = this.GetAziendeInvitate(pub);
                        break;
                    case DynamicAttribute.DittePartecipanti:
                        valueString = this.GetAziendePartecipanti(pub);
                        break;
                    case DynamicAttribute.ProceduraAggiudicazione:
                        valueString = this.GetSceltaContraente(pub);
                        break;
                    case DynamicAttribute.ImportoComplessivo:
                        valueString = this.GetImportoAggiudicazione(pub).ToString();
                        break;
                    case DynamicAttribute.StrutturaProponente:
                        valueString = this.GetStrutturaProponente(pub);
                        break;
                }

                if (!string.IsNullOrEmpty(valueString))
                {
                    chain.AddAttribute(attribute.Name, valueString);
                    continue;
                }

                DateTime valueDatetime = DateTime.MinValue;
                switch (EnumHelper.ParseDescriptionToEnum<DynamicAttribute>(attribute.Name))
                {
                    case DynamicAttribute.DurataAl:
                        DateTime? endDate = this.GetDataFineLavori(pub);
                        if ((endDate.HasValue))
                        {
                            valueDatetime = endDate.Value;
                        }
                        break;
                    case DynamicAttribute.DurataDal:
                        DateTime? startDate = this.GetDataInizioLavori(pub);
                        if ((startDate.HasValue))
                        {
                            valueDatetime = startDate.Value;
                        }
                        break;
                }

                if (valueDatetime != DateTime.MinValue)
                {
                    chain.AddAttribute(attribute.Name, valueDatetime.ToString());
                }
            }
            return chain;
        }

        /// <summary>
        /// Metodo per il salvataggio di un DataSet in Serie AVCP
        /// </summary>
        /// <param name="xml">Oggetto serializzato da salvare</param>
        /// <param name="item">DocumentSeriesItem in cui deve essere salvato</param>
        /// <param name="username">Nome dell'operatore che esegue il savataggio</param>
        /// <returns>Restituisce l'oggetto contenente tutte le informazioni sull'importazione eseguita</returns>
        public SetDataSetResult SetDataSetXML(string xml, DocumentSeriesItem item, string username)
        {
            pubblicazione pub = XmlUtil.Deserialize<AVCP.pubblicazione>(xml);

            return SetDataSetPub(pub, item, username);
        }

        public SetDataSetResult SetDataSetXML(string xml, DocumentSeriesItem item)
        {
            return SetDataSetXML(xml, item, DocSuiteContext.Current.User.FullUserName);
        }

        /// <summary>
        /// Recupera o Crea il DocumentSeriesItem a partire dell'Identificatore
        /// </summary>
        /// <param name="identifier">Identificatore della registrazione</param>
        /// <param name="titolo">Titolo per l'eventuale creazione</param>
        /// <param name="idCategory">Identificativo della Category per eventuale creazione</param>
        /// <param name="username">Nome dell'operatore che esegue il savataggio</param>
        /// <returns></returns>
        public DocumentSeriesItem GetItemOrCreate(string identifier, string titolo, int idCategory, string username)
        {
            FileLogger.Debug(LoggerName, string.Format("GetItemOrCreate {0}, {1}, {2}, {3} ", identifier, titolo, idCategory, username));

            // Recupero il DocumentSeriesItem oppure lo creo
            var doc = GetDocumentSerieItem(identifier, username);
            if (doc == null)
            {
                FileLogger.Debug(LoggerName, "Documento non trovato.");
                doc = CreateDataSet(identifier, titolo, idCategory, username);
            }
            return doc;
        }

        public DocumentSeriesItem GetItemOrCreate(Resolution resl, string username)
        {
            var item = GetAVCPDocumentSeriesItem(resl);
            if (item != null)
                return item;

            return GetItemOrCreate(resl.InclusiveNumber, resl.ResolutionObject, resl.MyCategoryId(), username);
        }

        public DocumentSeriesItem GetItemOrCreate(Resolution resl)
        {
            return GetItemOrCreate(resl.InclusiveNumber, resl.ResolutionObject, resl.MyCategoryId(), DocSuiteContext.Current.User.FullUserName);
        }

        /// <summary>
        /// Restituisce l'oggetto serializzato a partire dal suo identificatore
        /// </summary>
        /// <param name="identifier">Identificatore della registrazione</param>
        /// <returns>Restituisce la serializzazione dell'oggetto trovato. Stringa vuota nel caso non lo trovi.</returns>
        //public string GetDataSetXML(string identifier)
        //{
        //    var item = GetDocumentSerieItem(identifier);

        //    if (item == null)
        //    {
        //        return String.Empty;
        //    }

        //    return GetAVCPXml(item);
        //}

        public string GetIdentifier(DocumentSeriesItem item)
        {
            var attributes = FacadeFactory.Instance.DocumentSeriesItemFacade.GetAttributes(item);
            return attributes[AttributeIdentifier];
        }

        public bool ParseProvvedimento(string provvedimento, out int anno, out string codiceServizio, out int numeroAtto)
        {
            anno = 0;
            codiceServizio = String.Empty;
            numeroAtto = 0;

            if (string.IsNullOrEmpty(provvedimento))
                return false;

            string[] tokens = provvedimento.Split('/');
            if (tokens.Length != 3)
                return false;

            if (!Int32.TryParse(tokens[0], out numeroAtto))
                return false;

            codiceServizio = tokens[1].Trim();
            if (string.IsNullOrEmpty(codiceServizio))
                return false;

            if (!Int32.TryParse(tokens[2], out anno))
                return false;

            return true;
        }

        /// <summary>
        /// Restituisce l'oggetto Pubblicazione a partire dal suo Identificatore
        /// </summary>
        /// <param name="identifier">Identificatore della registrazione</param>
        /// <returns>Restituisce l'oggetto trovato. NULL nel caso non lo trovi</returns>
        //public pubblicazione GetDataSetPub(string identifier)
        //{
        //    var temp = GetDataSetXML(identifier);

        //    if (string.IsNullOrEmpty(temp))
        //    {
        //        return null;
        //    }

        //    return XmlUtil.Deserialize<AVCP.pubblicazione>(temp);
        //}

        /// <summary>
        /// Restituisce il DocumentSeriesItem a partire dal suo Identificatore
        /// </summary>
        /// <param name="identifier">Identificatore della registrazione</param>
        /// <returns>Restituisce l'oggetto trovato. NULL nel caso non lo trovi.</returns>
        public DocumentSeriesItem GetDocumentSerieItem(string identifier, string username)
        {
            DocumentSeriesItemFinder finder = new DocumentSeriesItemFinder(false, username);

            finder.IdDocumentSeriesIn = new List<int> { DocSuiteContext.Current.ProtocolEnv.AvcpDocumentSeriesId.Value };
            finder.OnlyActive = true;
            List<SearchCondition> conditions = new List<SearchCondition>() { new SearchCondition { AttributeName = AttributeIdentifier, AttributeValue = string.Format("'{0}'", identifier), Operator = SearchConditionOperator.IsEqualTo } };

            FacadeFactory.Instance.DocumentSeriesFacade.FillFinder(finder, conditions);

            List<DocumentSeriesItem> list = finder.DoSearch().ToList();

            if (list.Count > 1)
            {
                throw new DocSuiteException(string.Format("Ricerca per identifier [{0}] ha restituito {1} risultati.", identifier, list.Count));
            }

            if (list.Count == 0)
            {
                return null;
            }

            return list[0];
        }

        /// <summary>
        /// Esegue il collegamento tra la Resolution e il Tender.
        /// </summary>
        /// <param name="pub">Pubblicazione usata per generare il Tender</param>
        /// <param name="resl">Resolution di riferimento</param>
        /// <returns>Restituisce il Tender creato.</returns>
        public TenderHeader LinkToTender(pubblicazione pub, Resolution resl, DocumentSeriesItem item)
        {
            TenderHeader tender = null;
            // Cerco per Resolution
            if (resl != null)
                tender = FacadeFactory.Instance.TenderHeaderFacade.GetByResolution(resl);
            // Cerco per DocumentSeriesItem
            if (tender == null)
            {
                tender = FacadeFactory.Instance.TenderHeaderFacade.GetByDocumentSeriesItem(item.Id);
            }
            // Creo nuova registrazione
            if (tender == null)
            {
                tender = new TenderHeader();
                FileLogger.Debug(LoggerName, "Tender non trovato.");
            }

            tender.Title = pub.metadata.titolo;
            tender.Abstract = pub.metadata.@abstract;
            tender.Year = pub.metadata.annoRiferimento;

            tender.DocumentSeriesItem = item;

            if (resl == null)
                tender.IdResolution = null;
            else
                tender.IdResolution = resl.Id;

            // Aggiungo i lot (solo se non presenti)
            foreach (var data in pub.data)
            {
                var lot = FacadeFactory.Instance.TenderLotFacade.GetByCIG(data.cig);
                if (lot == null)
                {
                    // Lotto non presente
                    lot = new TenderLot();
                    lot.Payments = new List<TenderLotPayment>();
                    lot.CIG = data.cig;
                    lot.RegistrationUser = DocSuiteContext.Current.User.FullUserName;
                    lot.RegistrationDate = DateTimeOffset.UtcNow;
                }

                // Verifico che non sia già associato ad altra Gara
                if (lot.Tender != null && lot.Tender != tender)
                {
                    throw new InvalidOperationException(string.Format("CIG [{0}] associato a GARA [{1}]", data.cig, lot.Tender.Id));
                }

                // Aggiungo solo se Lotto orfano
                if (lot.Tender == null)
                {
                    // Lotto orfano
                    tender.AddLot(lot);
                }
            }

            FacadeFactory.Instance.TenderHeaderFacade.Update(ref tender);

            return tender;
        }

        /// <summary>
        /// Esegue il collegamento tra il DocumentSeriesItem e la Resolution
        /// </summary>
        public void LinkToResolution(DocumentSeriesItem item, Resolution resl)
        {
            // mette in collegamento la resolution e le serie documentali associate tramite la resolutionKind
            IList<DocumentSeriesItem> existingItems = FacadeFactory.Instance.ResolutionDocumentSeriesItemFacade.GetDocumentSeriesItems(resl);
            if (!existingItems.IsNullOrEmpty() && existingItems.Any(x => x.Id == item.Id))
            {
                return;
            }

            // aggiungere collegamento alla resolution
            FacadeFactory.Instance.ResolutionDocumentSeriesItemFacade.LinkResolutionToDocumentSeriesItem(resl, item);
            // Aggiungere log in Resolution
            FacadeFactory.Instance.ResolutionLogFacade.Log(resl, ResolutionLogType.SD, string.Format("Inserimento in Serie Documentale [{0}] da [{3}] : {1}/{2:000000}", item.DocumentSeries.Container.Name, item.Year, item.Number, DocSuiteContext.Current.User.FullUserName));
        }

        public bool ValidateAVCP(pubblicazione pub, out List<string> errors)
        {
            var xml = AVCPHelper.Serialize(pub);
            XmlValidator validator = new XmlValidator();
            var valid = validator.ValidateXml(xml, XSDFullPath, AVCPHelper.AvcpNamespace);
            errors = validator.Errors;
            return valid;
        }

        public pubblicazione GetAVCPStructure(DocumentSeriesItem item)
        {
            var docs = FacadeFactory.Instance.DocumentSeriesItemFacade.GetMainDocuments(item);
            if (docs.Count > 1)
            {
                throw new DocSuiteException(string.Format("Trovati {0} documenti.", docs.Count));
            }
            if (docs == null || docs.Count == 0)
            {
                throw new DocSuiteException("Nessun documento trovato.");
            }

            var doc = docs[0];
            string ss;
            pubblicazione result;
            try
            {
                ss = System.Text.Encoding.Unicode.GetString(doc.Stream);
                result = XmlUtil.Deserialize<pubblicazione>(ss);
            }
            catch (Exception)
            {
                ss = System.Text.Encoding.UTF8.GetString(doc.Stream);
                result = XmlUtil.Deserialize<pubblicazione>(ss);
            }

            return result;
        }

        public DocumentSeriesItem GetAVCPDocumentSeriesItem(Resolution resl)
        {
            if (!DocSuiteContext.Current.ProtocolEnv.AvcpDocumentSeriesId.HasValue)
                throw new DocSuiteException("Valore parametro [ProtocolEnv.AvcpDocumentSeriesId] non impostato.");

            var items = FacadeFactory.Instance.ResolutionDocumentSeriesItemFacade.GetByResolution(resl);
            var ids = items.Where(ri => ri.IdDocumentSeriesItem.HasValue).Select(ri => ri.IdDocumentSeriesItem.Value).ToList<int>();
            var dsi = FacadeFactory.Instance.DocumentSeriesItemFacade.GetByIdentifiers(ids);
            var item = dsi.FirstOrDefault<DocumentSeriesItem>(i => i.DocumentSeries.Id == DocSuiteContext.Current.ProtocolEnv.AvcpDocumentSeriesId.Value);
            return item;
        }

        public pubblicazione GetAVCPPub(DocumentSeriesItem item)
        {
            return GetAVCPStructure(item);
        }

        public SetDataSetResult UpdateAVCPPayments(TenderHeader _tenderHeader, string username)
        {
            // Recupero Item
            DocumentSeriesItem item = FacadeFactory.Instance.DocumentSeriesItemFacade.GetById(_tenderHeader.DocumentSeriesItem.Id);

            pubblicazione pub = GetAVCPPub(item);

            pub = SetAVCPPayments(item, pub, _tenderHeader);

            return SetDataSetPub(pub, item, username);
        }

        public pubblicazione SetAVCPPayments(DocumentSeriesItem item, pubblicazione pub, TenderHeader _tenderHeader)
        {
            try
            {
                _tenderHeader = FacadeFactory.Instance.TenderHeaderFacade.GetById(_tenderHeader.Id);//.GetByDocumentSeriesItem(item.Id);

                foreach (var lot in _tenderHeader.Lots)
                {
                    FileLogger.Debug(LoggerName, String.Concat("lot ", lot.Id));
                    FileLogger.Debug(LoggerName, String.Concat("lot.Payments is null ", (lot.Payments == null)));

                    // recupero il relativo Lotto in pub
                    pubblicazioneLotto lotto = pub.data.FirstOrDefault(l => l.cig.Trim().Eq(lot.CIG.Trim()));
                    FileLogger.Debug(LoggerName, String.Concat("lotto trovato ", (lotto != null), " con valore", (lotto != null) ? lotto.importoSommeLiquidate : 0));
                    // somma liquidato
                    if (!lot.Payments.IsNullOrEmpty())
                    {
                        double tot = lot.Payments.Sum(p => p.Amount);
                        FileLogger.Debug(LoggerName, String.Concat("tot ", tot));
                        lotto.importoSommeLiquidate = Convert.ToDecimal(tot);
                        FileLogger.Debug(LoggerName, String.Concat("lotto aggiornato ", (lotto != null), " con valore", (lotto != null) ? lotto.importoSommeLiquidate : 0));
                    }
                }

                if (_tenderHeader.Lots.Count > 0)
                {
                    pub.metadata.dataUltimoAggiornamentoDataset = DateTime.Now;
                }

                return pub;
            }
            catch (Exception ex)
            {
                FileLogger.Warn(LoggerName, String.Concat("Errore generico in SetAVCPPayments per item ", item.Id), ex);
                throw ex;
            }


        }

        public Resolution GetProvvedimento(string identifier, bool enableFilter = true, bool findAllResolutionTypes = false)
        {
            var finder = new NHibernateResolutionFinder("ReslDB", enableFilter);
            //finder.Year = year.ToString();
            finder.InclusiveNumbers = new List<string>() { identifier };
            if (!findAllResolutionTypes)
            {
                finder.ResolutionType = FacadeFactory.Instance.ResolutionTypeFacade.GetById((short)DocSuiteContext.Current.ProtocolEnv.AVCPResolutionType);
            }
            finder.EagerLog = false;

            var res = finder.DoSearch();

            if (res.Count != 1)
            {
                throw new DocSuiteException(string.Format("Risultato della ricerca per [{0}] ha restituito {1} righe.", identifier, res.Count));
            }

            return res[0];
        }

        public Resolution GetProvvedimento(int year, string codiceServizio, int number, bool enableFilter = true, bool findAllResolutionTypes = false)
        {
            string incNum = string.Format(DocSuiteContext.Current.ProtocolEnv.AVCPInclusiveNumberMask, year, codiceServizio, number);
            FileLogger.Debug(LoggerName, string.Format("Cerco il provvedimento per inclusivenumber = [{0}]", incNum));

            string[] splittedNumbers = incNum.Split('/');
            if (splittedNumbers.Length == 3 && string.IsNullOrEmpty(splittedNumbers[1]))
            {
                incNum = string.Format("{0}/{1}", splittedNumbers[0], splittedNumbers[2]);
                FileLogger.Debug(LoggerName, string.Format("Codice servizio non presente. Modifico inclusivenumber in [{0}]", incNum));
            }
            return GetProvvedimento(incNum, enableFilter, findAllResolutionTypes);
        }

        private DocumentSeriesItem CreateDataSet(string identifier, string subject, int idCategory, string username)
        {
            var ds = FacadeFactory.Instance.DocumentSeriesFacade.GetById(DocSuiteContext.Current.ProtocolEnv.AvcpDocumentSeriesId.Value);

            FileLogger.Info(LoggerName, "DocumentSeries " + ds.Id);

            var chain = new BiblosChainInfo();
            chain.AddAttribute(AttributeIdentifier, identifier);

            // Chiusa = false = 0 stato impostato da amministrazione trasparente o da altro modulo
            chain.AddAttribute(AttributeChiusa, "0");

            var item = new DocumentSeriesItem();

            Category selCategory = FacadeFactory.Instance.CategoryFacade.GetById(idCategory);
            FileLogger.Info(LoggerName, "selCategory " + selCategory.Id);

            // Se la Category è di tipo radice
            if (selCategory.Root == selCategory)
            {
                item.Category = selCategory;
            }
            else
            {
                item.Category = selCategory.Root;
                item.SubCategory = selCategory;
            }

            item.DocumentSeries = ds;
            item.Subject = subject;
            FileLogger.Info(LoggerName, "saving item");
            FacadeFactory.Instance.DocumentSeriesItemFacade.SaveDocumentSeriesItem(item, chain, null, null, DocumentSeriesItemStatus.Active, string.Format("Importato da [{0}].", username));
            FileLogger.Debug(LoggerName, "item " + item.Id);

            return item;

        }

        private void FlushDocuments(DocumentSeriesItem item)
        {
            // Documento presente, lo sostituisco
            var toSave = FacadeFactory.Instance.DocumentSeriesItemFacade.GetAttributes(item);
            var newChain = new BiblosChainInfo();
            newChain.AddAttributes(toSave);

            item.IdMain = newChain.ArchiveInBiblos(item.Location.DocumentServer, item.Location.ProtBiblosDSDB);
        }

        public bool CheckCIGExists(string cig)
        {
            if (FacadeFactory.Instance.TenderLotFacade.GetByCIG(cig) != null)
                return true;
            return false;
        }

        /// <summary>
        /// Ritorna la struttura proponente data una pubblicazione AVCP
        /// </summary>
        /// <param name="pub"></param>
        /// <returns></returns>
        public string GetStrutturaProponente(pubblicazione pub)
        {
            if (pub.data == null || !pub.data.Any())
            {
                return string.Empty;
            }
            return pub.data.Select(x => string.Concat(x.strutturaProponente.denominazione, WordSeparator, x.strutturaProponente.codiceFiscaleProp))
                           .Aggregate((current, next) => string.Concat(current, FieldSeparator, next));
        }

        /// <summary>
        /// Ritorna le aziende invitate.
        /// </summary>
        /// <param name="pub"></param>
        /// <returns>String vuota per future implementazioni</returns>
        public string GetAziendeInvitate(pubblicazione pub)
        {
            return GetAziendePartecipanti(pub);
        }

        /// <summary>
        /// Ritorna le aziende partecipanti nel formato: {codice fiscale} - {ragione sociale};{codice fiscale} - {ragione sociale}
        /// </summary>
        /// <param name="pub"></param>
        /// <returns></returns>
        public string GetAziendePartecipanti(pubblicazione pub)
        {
            List<singoloType> partecipantiList = new List<singoloType>();
            if (pub.data != null && pub.data.Any(x => x.partecipanti != null))
            {
                foreach (pubblicazioneLottoPartecipanti partecipanti in pub.data
                    .Where(x => x.partecipanti != null)
                    .Select(x => x.partecipanti))
                {
                    foreach (singoloType partecipante in partecipanti.partecipante)
                    {
                        if (!partecipantiList.Any(f => f.Item.Eq(partecipante.Item) && f.ragioneSociale.Eq(partecipante.ragioneSociale)))
                        {
                            partecipantiList.Add(partecipante);
                        }
                    }
                }
                IEnumerable<string> partecipantiResult = partecipantiList
                    .Select(y => string.Concat(y.Item, WordSeparator, y.ragioneSociale))
                    .ToList<string>();
                return string.Join(FieldSeparator, partecipantiResult.ToArray());
            }
            return string.Empty;
        }

        /// <summary>
        /// Ritorna le aziende aggiudicatarie nel formato: {codice fiscale} - {ragione sociale};{codice fiscale} - {ragione sociale}
        /// </summary>
        /// <param name="pub"></param>
        /// <returns></returns>
        public string GetAziendeAggiudicatarie(pubblicazione pub)
        {
            List<singoloType> aggiudicatariList = new List<singoloType>();
            if (pub.data != null && pub.data.Any(x => x.aggiudicatari != null))
            {
                foreach (pubblicazioneLottoAggiudicatari aggiudicatari in pub.data
                    .Where(x => x.partecipanti != null)
                    .Select(x => x.aggiudicatari))
                {
                    foreach (singoloType aggiudicatario in aggiudicatari.aggiudicatario)
                    {
                        if (!aggiudicatariList.Any(f => f.Item.Eq(aggiudicatario.Item) && f.ragioneSociale.Eq(aggiudicatario.ragioneSociale)))
                        {
                            aggiudicatariList.Add(aggiudicatario);
                        }
                    }
                }
                IEnumerable<string> aggiudicatariResult = aggiudicatariList
                    .Select(y => string.Concat(y.Item, WordSeparator, y.ragioneSociale))
                    .ToList<string>();
                return string.Join(FieldSeparator, aggiudicatariResult.ToArray());
            }
            return string.Empty;
        }

        /// <summary>
        /// Ritorna le date di inizio lavori nel formato {0:dd/MM/yyyy};{0:dd/MM/yyyy}
        /// </summary>
        /// <param name="pub"></param>
        /// <returns></returns>
        public Nullable<DateTime> GetDataInizioLavori(pubblicazione pub)
        {
            if (pub.data != null && pub.data.Any(x => x.tempiCompletamento != null))
            {
                return pub.data
                    .Where(x => x.tempiCompletamento != null)
                    .Min(x => x.tempiCompletamento.dataInizio);
            }
            return null;
        }

        /// <summary>
        /// Ritorna le date di fine lavori nel formato {0:dd/MM/yyyy};{0:dd/MM/yyyy}
        /// </summary>
        /// <param name="pub"></param>
        /// <returns></returns>
        public Nullable<DateTime> GetDataFineLavori(pubblicazione pub)
        {
            if (pub.data != null && pub.data.Any(x => x.tempiCompletamento != null))
            {
                return pub.data
                    .Where(x => x.tempiCompletamento != null)
                    .Min(x => x.tempiCompletamento.dataUltimazione);
            }
            return null;
        }

        /// <summary>
        /// Ritorna gli importi di aggiudicazione suddivisi per lotto nel formato {0:0.00};{0:0.00}
        /// </summary>
        /// <param name="pub"></param>
        /// <returns></returns>
        public decimal GetImportoAggiudicazione(pubblicazione pub)
        {
            if (pub.data != null && pub.data.Any())
            {
                return pub.data.Sum(x => x.importoAggiudicazione);
            }
            return 0;
        }
        /// <summary>
        /// Ritorna gli importi di aggiudicazione suddivisi per lotto nel formato {0:0.00};{0:0.00}
        /// </summary>
        /// <param name="pub"></param>
        /// <returns></returns>
        public decimal GetImportoSommeLiquidate(pubblicazione pub)
        {
            if (pub.data != null && pub.data.Any())
            {
                return pub.data.Sum(x => x.importoSommeLiquidate);
            }
            return 0;
        }

        /// <summary>
        /// Ritorna i CIG dei lotti nel formato {CIG};{CIG}
        /// </summary>
        /// <param name="pub"></param>
        /// <returns></returns>
        public string GetLotti(pubblicazione pub)
        {
            if (pub.data != null && pub.data.Any())
            {
                return pub.data
                    .Select(x => x.cig)
                    .Aggregate((current, next) => string.Concat(current, FieldSeparator, next));
            }
            return String.Empty;
        }

        public string GetSceltaContraente(pubblicazione pub)
        {
            if (pub.data != null && pub.data.Any())
            {
                return pub.data
                   .Select(x => x.sceltaContraente.GetXmlName().ToString())
                   .Aggregate((current, next) => string.Concat(current, FieldSeparator, next));
            }
            return String.Empty;
        }
        #endregion
    }

}
