using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.AVCP.Entities;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.Helpers;
using VecompSoftware.Helpers.ExtensionMethods;
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
                if (pub.metadata.dataPubblicazioneDataset != DateTime.MinValue)
                    tor.Chain.AddAttribute(AttributeDataPubblicazione, pub.metadata.dataPubblicazioneDataset.ToString("s"));
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

        private void FlushDocuments(DocumentSeriesItem item)
        {
            // Documento presente, lo sostituisco
            var toSave = FacadeFactory.Instance.DocumentSeriesItemFacade.GetAttributes(item);
            var newChain = new BiblosChainInfo();
            newChain.AddAttributes(toSave);

            item.IdMain = newChain.ArchiveInBiblos(item.Location.ProtBiblosDSDB);
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
