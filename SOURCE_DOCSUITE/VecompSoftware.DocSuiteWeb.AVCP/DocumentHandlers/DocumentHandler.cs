
using System;
using System.Collections.Generic;
using System.Linq;

namespace VecompSoftware.DocSuiteWeb.AVCP.Import
{
    public class DocumentHandler : IAVCPDocumentHandler
    {
        #region [ Field ]
        protected List<Notification> listNotify = null;
        private Dictionary<string, string> dictCodiceServizio = null;
        private Dictionary<string, string> dictContraente = null;
        protected string CfEstero = ">";

        private char[] mainSeparators = { '/', ';', ',', '\n' };
        private char[] secondarySeparators = { '#' };

        #endregion


        #region [ Virtuals ]
        protected virtual List<ImportRowGara> GaraRowConverter(List<DocumentRow> rows)
        {
            return null;
        }

        protected virtual List<ImportRowPagamento> PagamentoRowConverter(List<DocumentRow> rows)
        {
            return null;
        }
        #endregion


        public DocumentHandler(string dictServizioFilename, string dictContraenteFilename)
        {
            if (!string.IsNullOrEmpty(dictServizioFilename))
                this.dictCodiceServizio = CsvDictionary.Get(dictServizioFilename, false);

            if (!string.IsNullOrEmpty(dictContraenteFilename))
                this.dictContraente = CsvDictionary.Get(dictContraenteFilename);
        }


        public List<DocumentData> BuildDocuments(List<DocumentRow> rows, out List<Notification> errors)
        {
            this.listNotify = new List<Notification>();
            errors = listNotify;

            List<DocumentData> docs = new List<DocumentData>();

            try
            {
                List<ImportRowGara> gareRows = GaraRowConverter(rows);

                //gruppi di record per document id
                var groups = gareRows.Where(gara => gara.IsValid).GroupBy(p => p.DocumentKey,
                  (key, items) => new { DocumentKey = key, Rows = items.ToList() });

                foreach (var grp in groups)
                {
                    //Crea pubblicazioni per 
                    docs.Add(
                      CreateDoc(grp.DocumentKey, grp.Rows)
                    );
                }

                return docs;
            }
            catch (Exception ex)
            {
                this.listNotify.Add(new Notification
                {
                    ErrorID = (int)Notification.ND_Error.ER_IMPORT_ERROR,
                    Message = "Errore durante la creazione delle pubblicazioni",
                    ExceptionMessage = ex.Message
                });
                return docs;
            }
        }


        public List<ImportRowPagamento> AggregatePagamenti(List<DocumentRow> rows)
        {
            this.listNotify = new List<Notification>();

            List<ImportRowPagamento> pagamenti = PagamentoRowConverter(rows);
            return (from p in pagamenti
                    group p by new { p.CIG, p.DocumentKey }
                    into grp
                    select new ImportRowPagamento
                    {
                        CIG = grp.Key.CIG,
                        DocumentKey = grp.Key.DocumentKey,
                        ImportoLiquidato = grp.Sum(p => p.ImportoLiquidato)
                    }).ToList();
        }


        public List<DocumentData> UpdateDocuments(List<DocumentRow> rows, ResolveDocumentKey resolveHandler, GetDocumentKeyXml getXmlHandler, out List<Notification> errors)
        {
            List<DocumentData> docs = new List<DocumentData>();
            this.listNotify = new List<Notification>();
            errors = listNotify;

            try
            {
                List<ImportRowPagamento> pagamenti = PagamentoRowConverter(rows);
                if (this.listNotify.Count > 0)
                    return docs;

                var lotti = pagamenti.GroupBy(p => p.CIG,
                  (key, items) => new { CIG = key, Rows = items.ToList() });

                //ottiene il dizionario DocumentKey per CIG
                string[] cigList = lotti.Select(p => p.CIG).ToArray();
                Dictionary<string, string> keyDict = resolveHandler(cigList);

                //aggiorna nelle righe il DocumentKey
                foreach (var item in pagamenti)
                {
                    if (keyDict.ContainsKey(item.CIG) && !string.IsNullOrEmpty(keyDict[item.CIG]))
                        item.DocumentKey = keyDict[item.CIG];
                    else
                    {
                        this.listNotify.Add(new Notification
                        {
                            ErrorID = (int)Notification.ND_Error.ER_LOTTO_NOT_FOUND,
                            Message = string.Format("Lotto CIG:'{1}', nessuna gara individuata per questo CIG.", item.CIG)
                        });
                    }
                }

                //gruppi di record per provvedimento
                var groups = pagamenti.GroupBy(p => p.DocumentKey,
                  (key, items) => new { DocumentKey = key, Rows = items.ToList() });

                foreach (var grp in groups)
                {
                    if (grp.Rows.Count == 0)
                        continue;

                    //aggiorna l'xml di ciascun provvedimento
                    string xml = String.Empty;
                    try
                    {
                        xml = getXmlHandler(grp.DocumentKey);
                    }
                    catch (Exception ex)
                    {
                        this.listNotify.Add(new Notification
                        {
                            ErrorID = (int)Notification.ND_Error.ER_STORAGE_GET_ERROR,
                            Message = string.Format("DocumentKey {0}: Errore in lettura dell'Xml da storage", grp.DocumentKey),
                            ExceptionMessage = ex.Message
                        });
                        continue;
                    }

                    pubblicazione pub = XmlUtil.Deserialize<AVCP.pubblicazione>(xml);

                    //aggiorna la pubblicazione
                    foreach (var lotto in lotti)
                    {
                        var lottoPub = pub.data.Where(p => p.cig.ToLower() == ((string)lotto.CIG).ToLower()).SingleOrDefault();
                        if (lottoPub == null)
                        {
                            this.listNotify.Add(new Notification
                            {
                                ErrorID = (int)Notification.ND_Error.ER_LOTTO_NOT_FOUND,
                                Message = string.Format("DocumentKey {0}: Lotto CIG:'{1}' mancante", grp.DocumentKey, lotto.CIG)
                            });
                            continue;
                        }
                        lottoPub.importoSommeLiquidate = lotto.Rows.Sum(p => p.ImportoLiquidato);
                    }

                    if (lotti.Count() > 0 && errors.Count == 0)
                    {
                        docs.Add(new DocumentData
                        {
                            DocumentKey = grp.DocumentKey,
                            Pubblicazione = pub
                        });
                    }
                }
                return docs;
            }
            catch (Exception ex)
            {
                this.listNotify.Add(new Notification
                {
                    ErrorID = (int)Notification.ND_Error.ER_IMPORT_ERROR,
                    Message = "Errore durante l'aggiornamento delle pubblicazioni",
                    ExceptionMessage = ex.Message
                });

                return docs;
            }
        }


        protected bool ParseProvvedimento(string provvedimento, out int anno, out string codice, out int numero)
        {
            anno = 0;
            codice = string.Empty;
            numero = 0;

            if (string.IsNullOrEmpty(provvedimento))
                return false;

            string[] tokens = provvedimento.Split('/');
            if (tokens.Length > 3 || tokens.Length < 2)
                return false;

            if (!Int32.TryParse(tokens[0], out anno))
                return false;

            if (tokens.Length == 3)
            {
                codice = tokens[1].Trim();
                if (string.IsNullOrEmpty(codice))
                    return false;

                if (!Int32.TryParse(tokens[2], out numero))
                    return false;
            }
            else
            {
                if (!Int32.TryParse(tokens[1], out numero))
                    return false;
            }

            return true;
        }


        protected bool TryParseAzienda(string field, out List<ImportAzienda> list)
        {
            list = new List<ImportAzienda>();
            if (string.IsNullOrEmpty(field))
                return true;

            string[] aziende = field.Split(mainSeparators, StringSplitOptions.RemoveEmptyEntries);
            foreach (var azienda in aziende)
            {
                var item = new ImportAzienda();

                string[] tokens = azienda.Split(secondarySeparators, StringSplitOptions.RemoveEmptyEntries);
                switch (tokens.Length)
                {
                    case 0:
                        item.RagioneSociale = azienda;
                        break;

                    case 1:
                        item.RagioneSociale = azienda;
                        break;

                    case 2:
                        item.RagioneSociale = tokens[0];

                        if (tokens[1].StartsWith(CfEstero))
                            item.CodiceEstero = tokens[1].Substring(1);
                        else
                            item.Codice = tokens[1];
                        break;

                    case 3:
                        item.RagioneSociale = tokens[0];

                        if (tokens[1].StartsWith(CfEstero))
                            item.CodiceEstero = tokens[1].Substring(1);
                        else
                            item.Codice = tokens[1];

                        if (tokens[2].StartsWith(CfEstero))
                            item.CodiceEstero = tokens[2].Substring(1);
                        else
                            item.Codice = tokens[2];
                        break;

                    default:
                        return false;
                }

                item.Codice = item.Codice.Trim();
                item.CodiceEstero = item.CodiceEstero.Trim();
                if (string.IsNullOrEmpty(item.Codice) && string.IsNullOrEmpty(item.CodiceEstero))
                {
                    item.Codice = "00000000000";
                }
                item.RagioneSociale = item.RagioneSociale.Trim();

                list.Add(item);
            }

            return true;
        }


        protected DocumentData CreateDoc(string DocumentKey, List<ImportRowGara> rows)
        {
            AVCP.pubblicazione pub = new AVCP.pubblicazione();

            var row = rows[0];
            pub.metadata = new pubblicazioneMetadata();
            pub.metadata.annoRiferimento = row.AnnoRiferimento;
            pub.metadata.dataPubbicazioneDataset = DateTime.Now;
            pub.metadata.dataUltimoAggiornamentoDataset = rows.Max(p => p.DataAggiornamento);

            pub.metadata.titolo = AVCPHelper.GetDocumentTitle(
              row.AnnoProvvedimento,
              row.CodiceProvvedimento,
              row.NumeroProvvedimento
            );

            pub.metadata.@abstract = pub.metadata.titolo;

            var lotti = rows.Where(x => x.IsValid).GroupBy(p => p.CIG,
              (key, items) => new { CIG = key, Rows = items.ToList() });

            List<AVCP.pubblicazioneLotto> lottiList = new List<pubblicazioneLotto>();

            foreach (var lotto in lotti)
            {
                if (lotto.Rows == null || lotto.Rows.Count == 0)
                    continue;

                if (string.IsNullOrEmpty(lotto.CIG))
                {
                    this.listNotify.Add(new Notification
                    {
                        ErrorID = (int)Notification.ND_Error.ER_LOTTO_NOT_FOUND,
                        Message = string.Format("Documento:'{0}' - Attenzione codice CIG mancante.", DocumentKey)
                    });
                    continue;
                }

                //attenzione Errore - vi sono più righe per singolo lotto (CIG) 
                if (lotto.Rows.Count > 1)
                {
                    this.listNotify.Add(new Notification
                    {
                        ErrorID = (int)Notification.ND_Error.ER_LOTTO_MULTIPLE_FOUND,
                        Message = string.Format("Documento:'{0}' - Attenzione vi sono più righe per lo stesso CIG:{1}", DocumentKey, lotto.CIG)
                    });
                    continue;
                }

                row = lotto.Rows[0];

                AVCP.pubblicazioneLotto lottoPub = new pubblicazioneLotto();
                lottoPub.cig = lotto.CIG;
                lottoPub.strutturaProponente = new pubblicazioneLottoStrutturaProponente
                {
                    codiceFiscaleProp = row.CodiceFiscaleProponente ?? "",
                    denominazione = row.DenominazioneProponente ?? ""
                };

                lottoPub.oggetto = row.Oggetto;
                lottoPub.sceltaContraente = row.SceltaContraente;

                //partecipanti
                if (!string.IsNullOrEmpty(row.Partecipante))
                {
                    List<ImportAzienda> aziendeList;
                    if (TryParseAzienda(row.Partecipante, out aziendeList))
                    {

                        lottoPub.partecipanti = new pubblicazioneLottoPartecipanti();
                        lottoPub.partecipanti.partecipante = aziendeList.Select(p => new singoloType
                        {
                            Item = p.Codice,
                            ItemElementName = ItemChoiceType1.codiceFiscale,
                            ragioneSociale = p.RagioneSociale
                        }).ToArray();

                    }
                }

                //aggiudicatari
                if (!string.IsNullOrEmpty(row.Aggiudicatario))
                {
                    List<ImportAzienda> aziendeList;
                    List<singoloType> stList = new List<singoloType>();

                    if (TryParseAzienda(row.Aggiudicatario, out aziendeList))
                    {
                        lottoPub.aggiudicatari = new pubblicazioneLottoAggiudicatari();

                        foreach (var azienda in aziendeList)
                        {
                            var st = new singoloType();
                            st.ragioneSociale = azienda.RagioneSociale;

                            if (!string.IsNullOrEmpty(azienda.Codice))
                            {
                                st.Item = azienda.Codice;
                                st.ItemElementName = ItemChoiceType1.codiceFiscale;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(azienda.CodiceEstero))
                                {
                                    st.Item = azienda.CodiceEstero;
                                    st.ItemElementName = ItemChoiceType1.identificativoFiscaleEstero;
                                }
                            }

                            stList.Add(st);
                        }
                        lottoPub.aggiudicatari.aggiudicatario = stList.ToArray();
                    }
                }

                lottoPub.importoAggiudicazione = lotto.Rows.Sum(p => p.ImportoAggiudicazione);

                if (row.DurataDal != DateTime.MinValue && row.DurataAl != DateTime.MinValue)
                {
                    lottoPub.tempiCompletamento = new pubblicazioneLottoTempiCompletamento
                    {
                        dataInizio = row.DurataDal,
                        dataUltimazione = row.DurataAl,
                        dataInizioSpecified = true,
                        dataUltimazioneSpecified = true
                    };
                }

                lottiList.Add(lottoPub);
            }

            pub.data = lottiList.ToArray();
            return new DocumentData
            {
                Anno = row.AnnoProvvedimento,
                CodiceServizio = row.CodiceProvvedimento,
                Numero = row.NumeroProvvedimento,
                CigList = lottiList.Select(p => p.cig).ToArray(),
                DocumentKey = DocumentKey,
                Pubblicazione = pub
            };
        }


        protected bool TryConvertCodiceProvvedimento(string codice, out string converted)
        {
            converted = codice;
            if (dictCodiceServizio != null)
            {
                string normCodice = CsvDictionary.NormalizeString(codice);
                if (dictCodiceServizio.ContainsKey(normCodice))
                    converted = dictCodiceServizio[normCodice];
                else
                    return false;
            }
            return true;
        }


        protected bool TryConvertContraente(string contraente, out sceltaContraenteType val)
        {
            val = sceltaContraenteType.Item01PROCEDURAAPERTA;

            //verifica presenza nel dizionario per conversione
            if (dictContraente != null)
            {
                contraente = CsvDictionary.NormalizeString(contraente);
                if (!dictContraente.ContainsKey(contraente))
                    return false;

                contraente = dictContraente[contraente];
            }
            try
            {
                val = (sceltaContraenteType)Enum.Parse(typeof(sceltaContraenteType), contraente);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected bool TryParseCIG(string cigIn, out string cigOut)
        {
            cigOut = String.Empty;
            if (string.IsNullOrEmpty(cigIn))
                return false;

            string[] tokens = cigIn.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            cigOut = tokens[0];
            return true;
        }



    }
}
