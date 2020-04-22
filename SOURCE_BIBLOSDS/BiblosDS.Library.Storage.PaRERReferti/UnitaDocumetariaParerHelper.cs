using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

using BiblosDS.Library.Storage.PaRERReferti.XSD;
using BiblosDS.Library.Storage.PaRERReferti.XSD.MOD770;
using BiblosDS.Library.Storage.PaRERReferti.XSD.MODF24;
using BiblosDS.Library.Storage.PaRERReferti.XSD.CUD;
using BiblosDS.Library.Storage.PaRERReferti.XSD.Cedolino;

using BiblosDS.Library.Storage.PaRERReferti.Entities;
using BiblosDS.Library.Storage.PaRERReferti.Properties;

namespace BiblosDS.Library.Storage.PaRERReferti.Util
{
    /// <summary>
    /// classe Helper per la costruzione della unità documentaria per il Parer 
    /// </summary>
    public static class UnitaDocumetariaParerHelper
    {
        public static UnitaDocumentaria GetIntestazione(UnitaDocumentaria thisParerDoc, Document thisDocument, ParerContext thisContext, string Ambiente)
        {
            thisParerDoc.Intestazione = new IntestazioneType();

            thisParerDoc.Intestazione.Chiave = new ChiaveType();
            thisParerDoc.Intestazione.Chiave.Anno = thisDocument.Anno;
            thisParerDoc.Intestazione.Chiave.Numero = thisDocument.Numero;

            thisParerDoc.Intestazione.Chiave.TipoRegistro = thisDocument.TipoRegistro;

            thisParerDoc.Intestazione.Versione = Settings.Default.XSDParerVersion;

            thisParerDoc.Intestazione.Versatore = new VersatoreType();
            thisParerDoc.Intestazione.Versatore.Ambiente = Ambiente;
            thisParerDoc.Intestazione.Versatore.Ente = thisContext.Organizzazione;
            thisParerDoc.Intestazione.Versatore.Struttura = thisContext.Struttura;
            thisParerDoc.Intestazione.Versatore.UserID = thisContext.IdCliente;

            thisParerDoc.Intestazione.TipologiaUnitaDocumentaria = thisDocument.Serie;

            return thisParerDoc;
        }

        public static UnitaDocumentaria GetConfigurazione(UnitaDocumentaria thisParerDoc, Document thisDocument, ParerContext thisContext, ParerConfig thisConfig)
        {
            thisParerDoc.Configurazione = new ConfigType();
            thisParerDoc.Configurazione.TipoConservazione = TipoConservazioneType.SOSTITUTIVA;

            thisParerDoc.Configurazione.ForzaConservazione = thisConfig.ForzaConservazione;

            thisParerDoc.Configurazione.ForzaAccettazione = thisConfig.ForzaAccettazione;

            thisParerDoc.Configurazione.ForzaCollegamento = thisConfig.ForzaCollegamento;

            thisParerDoc.Configurazione.SimulaSalvataggioDatiInDB = thisConfig.SimulaVersamento;

            thisParerDoc.Configurazione.TipoConservazioneSpecified = true;
            thisParerDoc.Configurazione.ForzaConservazioneSpecified = true;
            thisParerDoc.Configurazione.ForzaAccettazioneSpecified = true;
            thisParerDoc.Configurazione.ForzaCollegamentoSpecified = true;
            thisParerDoc.Configurazione.SimulaSalvataggioDatiInDBSpecified = true;

            return thisParerDoc;
        }

        public static UnitaDocumentaria GetFascicolo(UnitaDocumentaria thisParerDoc, Document thisDocument)
        {
            thisParerDoc.ProfiloArchivistico = new ProfiloArchivisticoType();

            thisParerDoc.ProfiloArchivistico.FascicoloPrincipale = new CamiciaFascicoloType();
            thisParerDoc.ProfiloArchivistico.FascicoloPrincipale.Classifica = "1.1.1";

            return thisParerDoc;
        }

        public static UnitaDocumentaria GetProfilo(UnitaDocumentaria thisParerDoc, Document thisDocument)
        {
            thisParerDoc.ProfiloUnitaDocumentaria = new ProfiloUnitaDocumentariaType();
            thisParerDoc.ProfiloUnitaDocumentaria.Data = thisDocument.Data;
            thisParerDoc.ProfiloUnitaDocumentaria.Oggetto = thisDocument.Oggetto;

            // allegati e annessi e annotazioni non previsti 
            thisParerDoc.NumeroAllegati = "0";
            thisParerDoc.NumeroAnnessi = "0";
            thisParerDoc.NumeroAnnotazioni = "0";

            // TODO definire se cartaceo o meno
            // thisParerDoc.ProfiloUnitaDocumentaria.Cartaceo 

            return thisParerDoc;
        }

        public static UnitaDocumentaria GetProfiloCollegate(UnitaDocumentaria thisParerDoc, Document thisDocument)
        {
            int nCollegati = thisDocument._Collegati.Count;

            if (nCollegati > 0)
            {
                thisParerDoc.DocumentiCollegati = new DocumentoCollegatoTypeDocumentoCollegato[nCollegati];
                for (int iLinkDocument = 0; iLinkDocument < nCollegati; iLinkDocument++)
                {
                    thisParerDoc.DocumentiCollegati[iLinkDocument].ChiaveCollegamento = new ChiaveType();
                    thisParerDoc.DocumentiCollegati[iLinkDocument].ChiaveCollegamento.Anno = thisDocument._Collegati[iLinkDocument].Anno;
                    thisParerDoc.DocumentiCollegati[iLinkDocument].ChiaveCollegamento.Numero = thisDocument._Collegati[iLinkDocument].Numero;
                    thisParerDoc.DocumentiCollegati[iLinkDocument].ChiaveCollegamento.TipoRegistro = thisDocument._Collegati[iLinkDocument].TipoRegistro;

                    thisParerDoc.DocumentiCollegati[iLinkDocument].DescrizioneCollegamento = thisDocument._Collegati[iLinkDocument].Description;
                }
            }

            return thisParerDoc;
        }

        /*
        public static UnitaDocumentaria GetProfiloAnnessi(UnitaDocumentaria thisParerDoc, Document thisDocument)
        {
            int nAnnessi = thisDocument._Annessi.Count;

            thisParerDoc.NumeroAnnessi  = nAnnessi.ToString();

            if (nAnnessi > 0)
            {
                thisParerDoc.Annessi = new DocumentoType[nAnnessi];
                for (int iAnnesso = 0; iAnnesso < nAnnessi; iAnnesso++)
                {
                    thisParerDoc.Annessi[iAnnesso] = new DocumentoType();
                    thisParerDoc.Annessi[iAnnesso].DatiSpecifici = null;  
                    thisParerDoc.Annessi[iAnnesso].IDDocumento = thisDocument._Annessi[iAnnesso].IDDocumentKey(iAnnesso + 1);

                    // Tomasetti : 20111223 
                    // thisParerDoc.Annessi[iAnnesso].TipoDocumento = thisDocument._Annessi[iAnnesso].GetTipoDocumento();
                    thisParerDoc.Annessi[iAnnesso].TipoDocumento = "GENERICO";

                    thisParerDoc.Annessi[iAnnesso].ProfiloDocumento = new ProfiloDocumentoType();
                    thisParerDoc.Annessi[iAnnesso].ProfiloDocumento.Descrizione = thisDocument._Annessi[iAnnesso].Descrizione;
                    thisParerDoc.Annessi[iAnnesso].ProfiloDocumento.Autore = thisDocument._Annessi[iAnnesso].Autore;

                    thisParerDoc.Annessi[iAnnesso].StrutturaOriginale = new StrutturaType();
                    thisParerDoc.Annessi[iAnnesso].StrutturaOriginale.TipoStruttura = "DocumentoGenerico";

                    // Un documento = una componente 
                    thisParerDoc.Annessi[iAnnesso].StrutturaOriginale.Componenti = new ComponenteType[1];
                    thisParerDoc.Annessi[iAnnesso].StrutturaOriginale.Componenti[0] = new ComponenteType();
                    thisParerDoc.Annessi[iAnnesso].StrutturaOriginale.Componenti[0].ID = thisParerDoc.Annessi[iAnnesso].IDDocumento;
                    thisParerDoc.Annessi[iAnnesso].StrutturaOriginale.Componenti[0].OrdinePresentazione = (iAnnesso + 1).ToString();

                    // da chiarire con il Parer cfr. pag. 24 della documentazione
                    // thisParerDoc.Allegati[iAnnesso].StrutturaOriginale.Componenti[0].TipoComponente = thisDocument._Allegati[iAnnesso].GetTipoFirma;
                    thisParerDoc.Annessi[iAnnesso].StrutturaOriginale.Componenti[0].TipoComponente = "Contenuto";

                    thisParerDoc.Annessi[iAnnesso].StrutturaOriginale.Componenti[0].TipoSupportoComponente = TipoSupportoType.FILE;
                    if (thisDocument._Annessi[iAnnesso].NomeDocumento == "")
                    {
                        thisDocument._Annessi[iAnnesso].NomeDocumento = "documento.pdf";
                        thisParerDoc.Annessi[iAnnesso].StrutturaOriginale.Componenti[0].NomeComponente = thisDocument._Annessi[iAnnesso].NomeDocumento;
                    }
                    else
                        thisParerDoc.Annessi[iAnnesso].StrutturaOriginale.Componenti[0].NomeComponente = thisDocument._Annessi[iAnnesso].NomeDocumento;
                    
                    thisParerDoc.Annessi[iAnnesso].StrutturaOriginale.Componenti[0].FormatoFileVersato = thisDocument._Annessi[iAnnesso].FormatoDocumento;
                    if (thisDocument._Annessi[iAnnesso].HashDocumento != null)
                    {
                        string HashDoc = Convert.ToBase64String(thisDocument._Annessi[iAnnesso].HashDocumento); 
                        thisParerDoc.Annessi[iAnnesso].StrutturaOriginale.Componenti[0].HashVersato = HashDoc ;
                    }
                    // Parer 20120112 Tommasetti
                    // thisParerDoc.Annessi[iAnnesso].StrutturaOriginale.Componenti[0].UtilizzoDataFirmaPerRifTemp = true ;
                    
                    // Piccoli 20120119 La data di riferimento è la data di protocollo / data adozione (per i documenti con firma digitale) 
                    if (thisDocument._Annessi[iAnnesso].TipoFirma == typeFirma.FIRMATO)
                    {
                        thisParerDoc.Annessi[iAnnesso].StrutturaOriginale.Componenti[0].UtilizzoDataFirmaPerRifTemp = false;
                        thisParerDoc.Annessi[iAnnesso].StrutturaOriginale.Componenti[0].UtilizzoDataFirmaPerRifTempSpecified = true;
                        
                        DateTime rifTemporale = thisDocument.Data;
                        if (rifTemporale.Hour == 0 && rifTemporale.Minute == 0 && rifTemporale.Second == 0)
                            rifTemporale.Add(new TimeSpan(23,59,59)) ; 
                        thisParerDoc.Annessi[iAnnesso].StrutturaOriginale.Componenti[0].RiferimentoTemporale = rifTemporale ;
                        thisParerDoc.Annessi[iAnnesso].StrutturaOriginale.Componenti[0].RiferimentoTemporaleSpecified = true;
                        switch (thisDocument.Serie)
                        {
                            case typeSerie.PROTOCOLLO:
                                thisParerDoc.Annessi[iAnnesso].StrutturaOriginale.Componenti[0].DescrizioneRiferimentoTemporale = "data protocollo";
                                break;
                            case typeSerie.DELIBERA:
                                thisParerDoc.Annessi[iAnnesso].StrutturaOriginale.Componenti[0].DescrizioneRiferimentoTemporale = "data adozione";
                                break;
                            case typeSerie.DETERMINA:
                                thisParerDoc.Annessi[iAnnesso].StrutturaOriginale.Componenti[0].DescrizioneRiferimentoTemporale = "data adozione";
                                break;
                            default:
                                thisParerDoc.Annessi[iAnnesso].StrutturaOriginale.Componenti[0].DescrizioneRiferimentoTemporale = "data unita documentaria";
                                break;
                        }
                    }
                    else
                    {
                        thisParerDoc.Annessi[iAnnesso].StrutturaOriginale.Componenti[0].UtilizzoDataFirmaPerRifTemp = false;
                        thisParerDoc.Annessi[iAnnesso].StrutturaOriginale.Componenti[0].RiferimentoTemporaleSpecified = false;
                    }
                }
            }

            return thisParerDoc;
        }

        public static UnitaDocumentaria GetProfiloAllegati(UnitaDocumentaria thisParerDoc, Document thisDocument)
        {
            int nAllegati = thisDocument._Allegati.Count;

            thisParerDoc.NumeroAllegati = nAllegati.ToString(); 

            if (nAllegati > 0)
            {
                thisParerDoc.Allegati = new DocumentoType[nAllegati];
                for (int iAllegato = 0; iAllegato < nAllegati; iAllegato++)
                {
                    thisParerDoc.Allegati[iAllegato] = new DocumentoType(); 
                    thisParerDoc.Allegati[iAllegato].DatiSpecifici = null;  
                    thisParerDoc.Allegati[iAllegato].IDDocumento = thisDocument._Allegati[iAllegato].IDDocumentKey(iAllegato+1) ;

                    // Tomasetti : 20111223 
                    // thisParerDoc.Allegati[iAllegato].TipoDocumento = thisDocument._Allegati[iAllegato].GetTipoDocumento();
                    thisParerDoc.Allegati[iAllegato].TipoDocumento = "GENERICO"; 

                    thisParerDoc.Allegati[iAllegato].ProfiloDocumento = new ProfiloDocumentoType();
                    thisParerDoc.Allegati[iAllegato].ProfiloDocumento.Descrizione = thisDocument._Allegati[iAllegato].Descrizione;
                    thisParerDoc.Allegati[iAllegato].ProfiloDocumento.Autore = thisDocument._Allegati[iAllegato].Autore;

                    thisParerDoc.Allegati[iAllegato].StrutturaOriginale = new StrutturaType();
                    thisParerDoc.Allegati[iAllegato].StrutturaOriginale.TipoStruttura = "DocumentoGenerico";

                    // Un documento = una componente 
                    thisParerDoc.Allegati[iAllegato].StrutturaOriginale.Componenti = new ComponenteType[1];
                    thisParerDoc.Allegati[iAllegato].StrutturaOriginale.Componenti[0] = new ComponenteType();
                    thisParerDoc.Allegati[iAllegato].StrutturaOriginale.Componenti[0].ID = thisParerDoc.Allegati[iAllegato].IDDocumento ;
                    thisParerDoc.Allegati[iAllegato].StrutturaOriginale.Componenti[0].OrdinePresentazione = (iAllegato + 1).ToString() ;
                    
                    // da chiarire con il Parer cfr. pag. 24 della documentazione
                    // thisParerDoc.Allegati[iAllegato].StrutturaOriginale.Componenti[0].TipoComponente = thisDocument._Allegati[iAllegato].GetTipoFirma;
                    thisParerDoc.Allegati[iAllegato].StrutturaOriginale.Componenti[0].TipoComponente = "Contenuto"; 
                    
                    thisParerDoc.Allegati[iAllegato].StrutturaOriginale.Componenti[0].TipoSupportoComponente = TipoSupportoType.FILE; 
                    thisParerDoc.Allegati[iAllegato].StrutturaOriginale.Componenti[0].NomeComponente = thisDocument._Allegati[iAllegato].NomeDocumento;
                    thisParerDoc.Allegati[iAllegato].StrutturaOriginale.Componenti[0].FormatoFileVersato = thisDocument._Allegati[iAllegato].FormatoDocumento;
                    if (thisDocument._Allegati[iAllegato].HashDocumento != null)
                    {
                        string HashDoc = Convert.ToBase64String(thisDocument._Allegati[iAllegato].HashDocumento); 
                        thisParerDoc.Allegati[iAllegato].StrutturaOriginale.Componenti[0].HashVersato = HashDoc ;
                    }
                    // Parer 20120112 Tommasetti
                    // thisParerDoc.Allegati[iAllegato].StrutturaOriginale.Componenti[0].UtilizzoDataFirmaPerRifTemp = true ;

                    // Piccoli 20120119 La data di riferimento è la data per i firmati 
                    if (thisDocument._Allegati[iAllegato].TipoFirma == typeFirma.FIRMATO)
                    {
                        thisParerDoc.Allegati[iAllegato].StrutturaOriginale.Componenti[0].UtilizzoDataFirmaPerRifTemp = false;
                        thisParerDoc.Allegati[iAllegato].StrutturaOriginale.Componenti[0].UtilizzoDataFirmaPerRifTempSpecified = true; 
                        DateTime rifTemporale = thisDocument.Data;
                        if (rifTemporale.Hour == 0 && rifTemporale.Minute == 0 && rifTemporale.Second == 0)
                            rifTemporale.Add(new TimeSpan(23,59,59)) ; 

                        thisParerDoc.Allegati[iAllegato].StrutturaOriginale.Componenti[0].RiferimentoTemporale = rifTemporale ;
                        thisParerDoc.Allegati[iAllegato].StrutturaOriginale.Componenti[0].RiferimentoTemporaleSpecified = true;
                        switch (thisDocument.Serie)
                        {
                            case typeSerie.PROTOCOLLO:
                                thisParerDoc.Allegati[iAllegato].StrutturaOriginale.Componenti[0].DescrizioneRiferimentoTemporale = "data protocollo";
                                break;
                            case typeSerie.DELIBERA:
                                thisParerDoc.Allegati[iAllegato].StrutturaOriginale.Componenti[0].DescrizioneRiferimentoTemporale = "data adozione";
                                break;
                            case typeSerie.DETERMINA:
                                thisParerDoc.Allegati[iAllegato].StrutturaOriginale.Componenti[0].DescrizioneRiferimentoTemporale = "data adozione";
                                break;
                            default:
                                thisParerDoc.Allegati[iAllegato].StrutturaOriginale.Componenti[0].DescrizioneRiferimentoTemporale = "data unita documentaria";
                                break;
                        }
                    }
                    else
                    {
                        thisParerDoc.Allegati[iAllegato].StrutturaOriginale.Componenti[0].UtilizzoDataFirmaPerRifTemp = false;
                        thisParerDoc.Allegati[iAllegato].StrutturaOriginale.Componenti[0].RiferimentoTemporaleSpecified = false;
                    }
                }
            }

            return thisParerDoc;
        }
*/
        public static UnitaDocumentaria GetDatiSpecificiModelloF24(UnitaDocumentaria thisParerDoc, Document thisDocument)
        {
            thisParerDoc.DatiSpecifici = new DatiSpecificiType();
            BiblosDS.Library.Storage.PaRERReferti.XSD.MODF24.DatiSpecificiType thisParerModF24 = new XSD.MODF24.DatiSpecificiType();

            // popola i metadati
            thisParerModF24.AnnoDiCompetenza = thisDocument.GetAttributeValue("AnnoDiCompetenza");
            thisParerModF24.MeseDiCompetenza = thisDocument.GetAttributeValue("MeseDiCompetenza");

            thisParerModF24.VersioneDatiSpecifici = "1.0";
            DatiSpecificiSerializer(thisParerDoc, thisParerModF24, typeof(BiblosDS.Library.Storage.PaRERReferti.XSD.MODF24.DatiSpecificiType));

            return thisParerDoc;
        }

        public static UnitaDocumentaria GetDatiSpecificiModello770(UnitaDocumentaria thisParerDoc, Document thisDocument)
        {
            thisParerDoc.DatiSpecifici = new DatiSpecificiType();
            BiblosDS.Library.Storage.PaRERReferti.XSD.MOD770.DatiSpecificiType thisParerMod770 = new XSD.MOD770.DatiSpecificiType();

            // popola i metadati
            thisParerMod770.AnnoDiCompetenza = thisDocument.GetAttributeValue("AnnoDiCompetenza");
            if (thisDocument.GetAttributeValue("TipoModello").ToString() == "1")
                thisParerMod770.TipoModello = XSD.MOD770.DatiSpecificiTypeTipoModello.ORDINARIO;
            else
                thisParerMod770.TipoModello = XSD.MOD770.DatiSpecificiTypeTipoModello.SEMPLIFICATO;

            thisParerMod770.VersioneDatiSpecifici = "1.0";
            DatiSpecificiSerializer(thisParerDoc, thisParerMod770, typeof(BiblosDS.Library.Storage.PaRERReferti.XSD.MOD770.DatiSpecificiType));

            return thisParerDoc;
        }

        public static UnitaDocumentaria GetDatiSpecificiReferto(UnitaDocumentaria thisParerDoc, Document thisDocument)
        {
            thisParerDoc.DatiSpecifici = new DatiSpecificiType();

            BiblosDS.Library.Storage.PaRERReferti.XSD.DatiSpecificiType thisParerReferto = new BiblosDS.Library.Storage.PaRERReferti.XSD.DatiSpecificiType();

            // popola i metadati 
            thisParerReferto.IDStrutturaProduttrice = thisDocument.GetAttributeValue("SettoreErogatore");
            thisParerReferto.StrutturaProduttrice = thisDocument.GetAttributeValue("SettoreErogatoreDescrizione");
            thisParerReferto.IdStrutturaPrescrittriceDestinataria = thisDocument.GetAttributeValue("Richiedente");
            thisParerReferto.StrutturaPrescrittriceDestinataria = thisDocument.GetAttributeValue("RichiedenteDescrizione");
            thisParerReferto.ContestoCura = thisDocument.GetAttributeValue("Regime");

            thisParerReferto.NominativoRefertante = thisDocument.GetAttributeValue("MedicoRefertante");
            if (thisParerReferto.NominativoRefertante == "")
            {
                // TODO
                // scrive un warning perchè non dovrebbe mai essere vuoto  
            }

            thisParerReferto.CodFiscRefertante = thisDocument.GetAttributeValue("MedicoRefertanteDescrizione");
            if (thisParerReferto.CodFiscRefertante == "")
            {
                // TODO
                // scrive un warning perchè non dovrebbe mai essere vuoto 
            }

            thisParerReferto.CognomePrescrittoreDestinatario = "";
            thisParerReferto.NomePrescrittoreDestinatario = "";

            thisParerReferto.IDPazienteAnagraficaAziendale = thisDocument.GetAttributeValue("IDAssistitoSAC");

            thisParerReferto.NumeroTesseraSanitaria = "";
            thisParerReferto.NumeroTesseraTEAM = "";
            thisParerReferto.NumeroTessereSTP = "";
            thisParerReferto.AttributiDiSpecificazione = "";

            thisParerReferto.CognomePaziente = thisDocument.GetAttributeValue("Cognome");
            thisParerReferto.NomePaziente = thisDocument.GetAttributeValue("Nome");
            thisParerReferto.SessoPaziente = thisDocument.GetAttributeValue("Sesso");
            try
            {
                thisParerReferto.DataNascitaPaziente = DateTime.Parse(thisDocument.GetAttributeValue("DataNascita"));
                thisParerReferto.DataNascitaPazienteSpecified = true;
            }
            catch
            {
                thisParerReferto.DataNascitaPazienteSpecified = false;
            }

            thisParerReferto.CodiceFiscale = thisDocument.GetAttributeValue("CodiceFiscale");

            thisParerReferto.LuogoNascitaPazienteCodiceISTAT = "";

            thisParerReferto.AnonimatoRiserbo = "NON_NOTO";

            thisParerReferto.IdRichiesta = thisDocument.GetAttributeValue("IDPrenotazione");
            thisParerReferto.CodiceEpisodio = thisDocument.GetAttributeValue("IDEpisodio");

            thisParerReferto.TipoPrestazione = "";
            thisParerReferto.TipoCodicePrestazione = "";
            thisParerReferto.DataPrestazione = "NON_NOTA";
            thisParerReferto.TipoDataPrestazione = "";
            thisParerReferto.CodiceDayService = "";
            thisParerReferto.CodicePercorsoPDTA = "";
            thisParerReferto.DataRichiesta = "NON_NOTA";

            try
            {
                thisParerReferto.DataRegistrazione = DateTime.Parse(thisDocument.GetAttributeValue("DataOraReferto"));
                thisParerReferto.DataRegistrazioneSpecified = true;
            }
            catch
            {
                thisParerReferto.DataRegistrazioneSpecified = false;
            }

            thisParerReferto.DataCustodia = thisDocument.DataInserimentoDocumento;
            thisParerReferto.DataCustodiaSpecified = true;

            thisParerReferto.IdentificazioneRepository = "";
            thisParerReferto.TempoConservazione = "";

            thisParerReferto.Consultabilita = "DATI_STATO_SALUTE";

            thisParerReferto.Validita = thisDocument.GetAttributeValue("Validita");
            if (thisParerReferto.Validita == "")
                thisParerReferto.Validita = "VALIDO";

            // serializza 
            thisParerReferto.VersioneDatiSpecifici = "1.3";
            DatiSpecificiSerializer(thisParerDoc, thisParerReferto, typeof(BiblosDS.Library.Storage.PaRERReferti.XSD.DatiSpecificiType));

            return thisParerDoc;
        }

        public static UnitaDocumentaria GetDatiSpecificiCUD(UnitaDocumentaria thisParerDoc, Document thisDocument)
        {
            thisParerDoc.DatiSpecifici = new DatiSpecificiType();
            BiblosDS.Library.Storage.PaRERReferti.XSD.CUD.DatiSpecificiType thisParerCUD = new XSD.CUD.DatiSpecificiType();

            // popola i metadati 
            thisParerCUD.AnnoDiCompetenza = thisDocument.GetAttributeValue("AnnoDiCompetenza");
            thisParerCUD.CodiceFiscale = thisDocument.GetAttributeValue("CodiceFiscale");

            thisParerCUD.Nominativo = thisDocument.GetAttributeValue("Nominativo");
            // thisParerCUD.Nome = thisDocument.GetAttributeValue("Nome");
            // thisParerCUD.Cognome = thisDocument.GetAttributeValue("Cognome");

            if (thisDocument.GetAttributeValue("DataDiNascita") != "")
            {
                thisParerCUD.DataDiNascita = DateTime.Parse(thisDocument.GetAttributeValue("DataDiNascita"));
                thisParerCUD.DataDiNascitaSpecified = true;
            }
            else
                thisParerCUD.DataDiNascitaSpecified = false;

            thisParerCUD.Matricola = thisDocument.GetAttributeValue("Matricola");
            if (thisParerCUD.Matricola == "")
                thisParerCUD.Matricola = "Non Disponibile";

            thisParerCUD.VersioneDatiSpecifici = "1.1";
            DatiSpecificiSerializer(thisParerDoc, thisParerCUD, typeof(BiblosDS.Library.Storage.PaRERReferti.XSD.CUD.DatiSpecificiType));

            return thisParerDoc;
        }

        public static UnitaDocumentaria GetDatiSpecificiCedolinoComulativo(UnitaDocumentaria thisParerDoc, Document thisDocument)
        {
            thisParerDoc.DatiSpecifici = new DatiSpecificiType();
            BiblosDS.Library.Storage.PaRERReferti.XSD.CedolinoComulativo.DatiSpecificiType thisParerCedolino = new XSD.CedolinoComulativo.DatiSpecificiType();

            // popola i metadati 
            thisParerCedolino.AnnoDiCompetenza = thisDocument.GetAttributeValue("Anno");
            thisParerCedolino.MeseDiCompetenza = thisDocument.GetAttributeValue("Mese");

            thisParerCedolino.VersioneDatiSpecifici = "1.0";

            DatiSpecificiSerializer(thisParerDoc, thisParerCedolino, typeof(BiblosDS.Library.Storage.PaRERReferti.XSD.CedolinoComulativo.DatiSpecificiType));

            return thisParerDoc;
        }

        public static UnitaDocumentaria GetDatiSpecificiCedolino(UnitaDocumentaria thisParerDoc, Document thisDocument)
        {
            thisParerDoc.DatiSpecifici = new DatiSpecificiType();
            BiblosDS.Library.Storage.PaRERReferti.XSD.Cedolino.DatiSpecificiType thisParerCedolino = new XSD.Cedolino.DatiSpecificiType();

            // popola i metadati 
            thisParerCedolino.AnnoDiCompetenza = thisDocument.GetAttributeValue("Anno");
            thisParerCedolino.MeseDiCompetenza = thisDocument.GetAttributeValue("Mese");
            thisParerCedolino.CodiceFiscale = thisDocument.GetAttributeValue("CodiceFiscale");
            thisParerCedolino.Nome = thisDocument.GetAttributeValue("Nome");
            thisParerCedolino.Cognome = thisDocument.GetAttributeValue("Cognome");
            thisParerCedolino.DataDiNascita = DateTime.Parse(thisDocument.GetAttributeValue("DataDiNascita"));
            thisParerCedolino.Matricola = thisDocument.GetAttributeValue("Matricola");
            thisParerCedolino.StrutturaDiAppartenenza = thisDocument.GetAttributeValue("StrutturaDiAppartenenza");

            thisParerCedolino.VersioneDatiSpecifici = "1.0";
            DatiSpecificiSerializer(thisParerDoc, thisParerCedolino, typeof(BiblosDS.Library.Storage.PaRERReferti.XSD.Cedolino.DatiSpecificiType));

            return thisParerDoc;

        }

        private static void DatiSpecificiSerializer(UnitaDocumentaria thisParerDoc, object thisDatiSpecifici, System.Type thisDatiSpecificiType)
        {
            XmlSerializer serializerXml = new XmlSerializer(thisDatiSpecificiType);
            StringWriter writerXml = new StringWriter();
            serializerXml.Serialize(writerXml, thisDatiSpecifici);
            writerXml.Close();
            XmlDocument xmlDocParer = new XmlDocument();
            xmlDocParer.LoadXml(writerXml.ToString());
            writerXml.Dispose();

            thisParerDoc.DatiSpecifici.Any = new XmlElement[1];
            thisParerDoc.DatiSpecifici.Any[0] = xmlDocParer.DocumentElement;
        }

        /*
                public static UnitaDocumentaria GetDatiSpecificiDelibere(UnitaDocumentaria thisParerDoc, Document thisDocument)
                {
                    thisParerDoc.DocumentoPrincipale.DatiSpecifici = new DatiSpecificiType() ;

                    DocSuiteWeb_BiblosDS2010_ClientForParer.XSD.Delibere.DatiSpecificiType thisParerDelibera = new XSD.Delibere.DatiSpecificiType();
                    if (thisDocument.DataEsecutiva == null)
                        throw new Exception("DataEsecutiva obbligatoria per documenti di tipo Delibera"); 
                    else 
                        thisParerDelibera.DataEsecutiva = thisDocument.DataEsecutiva.Value;

                    thisParerDelibera.DataRegistrProt = thisDocument.DataEsecutiva.Value;

                    // In AUSL-RE e ASMN-RE la data di fine pubblicazione coincide con la data di esecutività
                    if (thisDocument.DataPubblicazione == null)
                    {
                        thisParerDelibera.DataInizioPubblicazioneSpecified = false; 
                    }
                    else
                    {
                        thisParerDelibera.DataInizioPubblicazioneSpecified = true;
                        thisParerDelibera.DataInizioPubblicazione = thisDocument.DataPubblicazione.Value;
                        if (thisDocument.DataRitiro != null)
                        {
                            thisParerDelibera.DataFinePubblicazioneSpecified = true;
                            thisParerDelibera.DataFinePubblicazione = thisDocument.DataRitiro.Value;
                        }
                    }

                    thisParerDelibera.VicarioFirmatario1Specified = false;
                    thisParerDelibera.VicarioFirmatario2Specified = false;
                    thisParerDelibera.VicarioFirmatario3Specified = false;
                    thisParerDelibera.VicarioFirmatario4Specified = false;
                    thisParerDelibera.VicarioFirmatario5Specified = false;

                    if (thisDocument.Documento.TipoFirma == typeFirma.NON_FIRMATO)
                    {
                        thisParerDelibera.Firmatario1 = "vedi firme autografe";
                        thisParerDelibera.VicarioFirmatario1Specified = false; 
                    }
                    else
                    {
                        Sign[] thoseSigns = BiblosDS2010Helper.GetDocumentSignatures(thisDocument.Documento.IdDocument);
                        for (int iSign = 0; iSign < thoseSigns.Length; iSign++)
                        {
                            switch (iSign)
                            {
                                case 0:
                                    thisParerDelibera.Firmatario1 = thoseSigns[iSign].Signature;
                                    if (thoseSigns[iSign].IsVicariale == true)
                                        thisParerDelibera.VicarioFirmatario1 = XSD.Delibere.DatiSpecificiTypeVicarioFirmatario1.SI;
                                    else
                                        thisParerDelibera.VicarioFirmatario1 = XSD.Delibere.DatiSpecificiTypeVicarioFirmatario1.NO;
                                    thisParerDelibera.VicarioFirmatario1Specified = true;
                                    break;
                                case 1:
                                    thisParerDelibera.Firmatario2 = thoseSigns[iSign].Signature;
                                    if (thoseSigns[iSign].IsVicariale == true)
                                        thisParerDelibera.VicarioFirmatario2 = XSD.Delibere.DatiSpecificiTypeVicarioFirmatario2.SI;
                                    else
                                        thisParerDelibera.VicarioFirmatario2 = XSD.Delibere.DatiSpecificiTypeVicarioFirmatario2.NO;
                                    thisParerDelibera.VicarioFirmatario2Specified = true;
                                    break;
                                case 2:
                                    thisParerDelibera.Firmatario3 = thoseSigns[iSign].Signature;
                                    if (thoseSigns[iSign].IsVicariale == true)
                                        thisParerDelibera.VicarioFirmatario3 = XSD.Delibere.DatiSpecificiTypeVicarioFirmatario3.SI;
                                    else
                                        thisParerDelibera.VicarioFirmatario3 = XSD.Delibere.DatiSpecificiTypeVicarioFirmatario3.NO;
                                    thisParerDelibera.VicarioFirmatario3Specified = true;
                                    break;
                                case 3:
                                    thisParerDelibera.Firmatario4 = thoseSigns[iSign].Signature;
                                    if (thoseSigns[iSign].IsVicariale == true)
                                        thisParerDelibera.VicarioFirmatario4 = XSD.Delibere.DatiSpecificiTypeVicarioFirmatario4.SI;
                                    else
                                        thisParerDelibera.VicarioFirmatario4 = XSD.Delibere.DatiSpecificiTypeVicarioFirmatario4.NO;
                                    thisParerDelibera.VicarioFirmatario4Specified = true;
                                    break;
                                case 4:
                                    thisParerDelibera.Firmatario5 = thoseSigns[iSign].Signature;
                                    if (thoseSigns[iSign].IsVicariale == true)
                                        thisParerDelibera.VicarioFirmatario5 = XSD.Delibere.DatiSpecificiTypeVicarioFirmatario5.SI;
                                    else
                                        thisParerDelibera.VicarioFirmatario5 = XSD.Delibere.DatiSpecificiTypeVicarioFirmatario5.NO;
                                    thisParerDelibera.VicarioFirmatario5Specified = true;
                                    break;
                            }
                        }
                    }

                    // dati XSD Parer Dicembre 2011 
                    // thisParerDelibera.ParereDirAmm = "vedi firme digitali";
                    // thisParerDelibera.ParereDirSanit = "vedi firme digitali";
                    // thisParerDelibera.Firmatario = "vedi firme digitali";
                    // thisParerDelibera.DataRegistrProtSpecified = false;
                    // thisParerDelibera.RuoloDirAmm = XSD.Delibere.DatiSpecificiTypeRuoloDirAmm.TITOLARE;
                    // thisParerDelibera.RuoloDirSanit = XSD.Delibere.DatiSpecificiTypeRuoloDirSanit.TITOLARE; 

                    if (Settings.Default.UseNumeroPubblicazione == true)
                        thisParerDelibera.NumeroPubblicazione = thisDocument.NumeroPubblicazione; 
                    else
                        thisParerDelibera.NumeroPubblicazione = "";

                    thisParerDelibera.RegistroPubblicazione = "albo online";

                    // da gestire in docsuiteweb
                    thisParerDelibera.Riservatezza = XSD.Delibere.DatiSpecificiTypeRiservatezza.NON_NOTO;        
                    thisParerDelibera.TipoDatoPersonale = XSD.Delibere.DatiSpecificiTypeTipoDatoPersonale.NESSUNO;
                    thisParerDelibera.RiservatezzaSpecified = true;
                    thisParerDelibera.TipoDatoPersonaleSpecified = true; 

                    thisParerDelibera.UnitaProponente = thisDocument.Proponente; 

                    thisParerDelibera.VersioneDatiSpecifici = "1.0";

                    XmlSerializer serializerProtocol = new XmlSerializer(typeof(XSD.Delibere.DatiSpecificiType));
                    StringWriter writerProtocol = new StringWriter();
                    serializerProtocol.Serialize(writerProtocol, thisParerDelibera);
                    writerProtocol.Close();
                    XmlDocument xmlProtocol = new XmlDocument();
                    xmlProtocol.LoadXml(writerProtocol.ToString());

                    thisParerDoc.DocumentoPrincipale.DatiSpecifici.Any = new XmlElement[1];
                    thisParerDoc.DocumentoPrincipale.DatiSpecifici.Any[0] = xmlProtocol.DocumentElement;

                    writerProtocol.Dispose();

                    return thisParerDoc; 
                }
        */

        /*
                public static UnitaDocumentaria GetDatiSpecificiDetermine(UnitaDocumentaria thisParerDoc, Document thisDocument)
                {
                    thisParerDoc.DocumentoPrincipale.DatiSpecifici = new DatiSpecificiType();

                    DocSuiteWeb_BiblosDS2010_ClientForParer.XSD.Determine.DatiSpecificiType  thisParerDetermina = new XSD.Determine.DatiSpecificiType();
                    if (thisDocument.DataEsecutiva == null)
                        throw new Exception("DataEsecutiva obbligatoria per documenti di tipo Delibera");
                    else
                        thisParerDetermina.DataEsecutiva = thisDocument.DataEsecutiva.Value;

                    thisParerDetermina.DataRegistrProt = thisDocument.DataEsecutiva.Value; 

                    // In AUSL-RE e ASMN-RE la data di fine pubblicazione coincide con la data di esecutività
                    if (thisDocument.DataPubblicazione == null)
                    {
                        thisParerDetermina.DataInizioPubblicazioneSpecified = false;
                    }
                    else
                    {
                        thisParerDetermina.DataInizioPubblicazioneSpecified = true;
                        thisParerDetermina.DataInizioPubblicazione = thisDocument.DataPubblicazione.Value;
                        if (thisDocument.DataRitiro != null)
                        {
                            thisParerDetermina.DataFinePubblicazioneSpecified = true;
                            thisParerDetermina.DataFinePubblicazione = thisDocument.DataRitiro.Value;
                        }
                    }

                    // thisParerDetermina.FirmatarioVistoContabile = "vedi firme digitali"; 
                    // thisParerDetermina.Firmatario = "vedi firme digitali";
                    // thisParerDetermina.RuoloFirmatario = XSD.Determine.DatiSpecificiTypeRuoloFirmatario.TITOLARE;
                    // thisParerDetermina.SoggettoAdottante = thisDocument.Adottante;  
                    // thisParerDetermina.DataRegistrProtSpecified = false;
                    // thisParerDetermina.DataVistoContabile = thisDocument.Data; // si presume che il visto contabile sia stato dato alla data di adozione

                    thisParerDetermina.VicarioFirmatario1Specified = false;
                    thisParerDetermina.VicarioFirmatario2Specified = false;
                    thisParerDetermina.VicarioFirmatario3Specified = false;
                    thisParerDetermina.VicarioFirmatario4Specified = false;
                    thisParerDetermina.VicarioFirmatario5Specified = false;

                    if (thisDocument.Documento.TipoFirma == typeFirma.NON_FIRMATO)
                    {
                        thisParerDetermina.Firmatario1 = "vedi firme autografe";
                        thisParerDetermina.VicarioFirmatario1Specified = false;
                    }
                    else
                    {
                        Sign[] thoseSigns = BiblosDS2010Helper.GetDocumentSignatures(thisDocument.Documento.IdDocument);
                        for (int iSign = 0; iSign < thoseSigns.Length; iSign++)
                        {
                            switch (iSign)
                            {
                                case 0:
                                    thisParerDetermina.Firmatario1 = thoseSigns[iSign].Signature;
                                    if (thoseSigns[iSign].IsVicariale == true)
                                        thisParerDetermina.VicarioFirmatario1 = XSD.Determine.DatiSpecificiTypeVicarioFirmatario1.SI;
                                    else
                                        thisParerDetermina.VicarioFirmatario1 = XSD.Determine.DatiSpecificiTypeVicarioFirmatario1.NO;
                                    thisParerDetermina.VicarioFirmatario1Specified = true;
                                    break;
                                case 1:
                                    thisParerDetermina.Firmatario2 = thoseSigns[iSign].Signature;
                                    if (thoseSigns[iSign].IsVicariale == true)
                                        thisParerDetermina.VicarioFirmatario2 = XSD.Determine.DatiSpecificiTypeVicarioFirmatario2.SI;
                                    else
                                        thisParerDetermina.VicarioFirmatario2 = XSD.Determine.DatiSpecificiTypeVicarioFirmatario2.NO;
                                    thisParerDetermina.VicarioFirmatario2Specified = true;
                                    break;
                                case 2:
                                    thisParerDetermina.Firmatario3 = thoseSigns[iSign].Signature;
                                    if (thoseSigns[iSign].IsVicariale == true)
                                        thisParerDetermina.VicarioFirmatario3 = XSD.Determine.DatiSpecificiTypeVicarioFirmatario3.SI;
                                    else
                                        thisParerDetermina.VicarioFirmatario3 = XSD.Determine.DatiSpecificiTypeVicarioFirmatario3.NO;
                                    thisParerDetermina.VicarioFirmatario3Specified = true;
                                    break;
                                case 3:
                                    thisParerDetermina.Firmatario4 = thoseSigns[iSign].Signature;
                                    if (thoseSigns[iSign].IsVicariale == true)
                                        thisParerDetermina.VicarioFirmatario4 = XSD.Determine.DatiSpecificiTypeVicarioFirmatario4.SI;
                                    else
                                        thisParerDetermina.VicarioFirmatario4 = XSD.Determine.DatiSpecificiTypeVicarioFirmatario4.NO;
                                    thisParerDetermina.VicarioFirmatario4Specified = true;
                                    break;
                                case 4:
                                    thisParerDetermina.Firmatario5 = thoseSigns[iSign].Signature;
                                    if (thoseSigns[iSign].IsVicariale == true)
                                        thisParerDetermina.VicarioFirmatario5 = XSD.Determine.DatiSpecificiTypeVicarioFirmatario5.SI;
                                    else
                                        thisParerDetermina.VicarioFirmatario5 = XSD.Determine.DatiSpecificiTypeVicarioFirmatario5.NO;
                                    thisParerDetermina.VicarioFirmatario5Specified = true;
                                    break;
                            }
                        }
                    }

                    if (Settings.Default.UseNumeroPubblicazione == true)
                        thisParerDetermina.NumeroPubblicazione = thisDocument.NumeroPubblicazione; 
                    else
                        thisParerDetermina.NumeroPubblicazione = "";

                    thisParerDetermina.RegistroPubblicazione = "albo online";


                    // da gestire in docsuiteweb
                    thisParerDetermina.Riservatezza = XSD.Determine.DatiSpecificiTypeRiservatezza.NON_NOTO;
                    thisParerDetermina.TipoDatoPersonale = XSD.Determine.DatiSpecificiTypeTipoDatoPersonale.NESSUNO;

                    thisParerDetermina.UnitaProponente = thisDocument.Proponente; 

                    thisParerDetermina.VersioneDatiSpecifici = "1.0";

                    XmlSerializer serializerProtocol = new XmlSerializer(typeof(XSD.Determine.DatiSpecificiType));
                    StringWriter writerProtocol = new StringWriter();
                    serializerProtocol.Serialize(writerProtocol, thisParerDetermina);
                    writerProtocol.Close();
                    XmlDocument xmlProtocol = new XmlDocument();
                    xmlProtocol.LoadXml(writerProtocol.ToString());

                    thisParerDoc.DocumentoPrincipale.DatiSpecifici.Any = new XmlElement[1];
                    thisParerDoc.DocumentoPrincipale.DatiSpecifici.Any[0] = xmlProtocol.DocumentElement;

                    writerProtocol.Dispose(); 

                    return thisParerDoc;
                }

                public static UnitaDocumentaria GetDatiSpecificiProtocollo(UnitaDocumentaria thisParerDoc, Document thisDocument)
                {
                    thisParerDoc.DocumentoPrincipale.DatiSpecifici = new DatiSpecificiType();

                    DocSuiteWeb_BiblosDS2010_ClientForParer.XSD.Protocollo.DatiSpecificiType thisParerProtocol = new XSD.Protocollo.DatiSpecificiType();
                    switch (thisDocument._TipoProtocollo) 
                    {
                        case typeProtocollo.INGRESSO :
                            thisParerProtocol.Movimento = XSD.Protocollo.DatiSpecificiTypeMovimento.ENTRATA;
                            thisParerProtocol.Mittente = thisDocument.Riferimenti;
                            thisParerProtocol.NumProtMittente = thisDocument.NumProtocolloMittente;
                            if (thisDocument.DataProtocolloMittente == null)
                            {
                                thisParerProtocol.DataProtMittenteSpecified = false; 
                            }
                            else
                            {
                                thisParerProtocol.DataProtMittenteSpecified = true; 
                                thisParerProtocol.DataProtMittente = thisDocument.DataProtocolloMittente.Value ; 
                            }                    
                            break ; 
                        case typeProtocollo.USCITA :
                            thisParerProtocol.Movimento = XSD.Protocollo.DatiSpecificiTypeMovimento.USCITA;
                            thisParerProtocol.Destinatario = thisDocument.Riferimenti; 
                            break ; 
                        // nella versione Gennaio 2012 è stato tolto il protocollo interno
                        case typeProtocollo.TRA_UFFICI :
                            throw new Exception("La gestione dei protocolli tra uffici non è supportata in questa versione"); 
                         default : 
                            // non succede mai 
                            break; 
                    }

                    // TODO da gestire in DocSuiteWeb
                    thisParerProtocol.Riservatezza = XSD.Protocollo.DatiSpecificiTypeRiservatezza.NON_NOTO;
                    thisParerProtocol.TipoDatoPersonale = XSD.Protocollo.DatiSpecificiTypeTipoDatoPersonale.NESSUNO;

                    thisParerProtocol.VicarioFirmatario1Specified = false;
                    thisParerProtocol.VicarioFirmatario2Specified = false;
                    thisParerProtocol.VicarioFirmatario3Specified = false;
                    thisParerProtocol.VicarioFirmatario4Specified = false;
                    thisParerProtocol.VicarioFirmatario5Specified = false;

                    if (thisDocument.Documento.TipoFirma == typeFirma.NON_FIRMATO)
                    {
                        thisParerProtocol.Firmatario1 = "vedi firme autografe";
                        thisParerProtocol.VicarioFirmatario1Specified = false;
                    }
                    else
                    {
                        Sign[] thoseSigns = BiblosDS2010Helper.GetDocumentSignatures(thisDocument.Documento.IdDocument);
                        for (int iSign = 0; iSign < thoseSigns.Length; iSign++)
                        {
                            switch (iSign)
                            {
                                case 0:
                                    thisParerProtocol.Firmatario1 = thoseSigns[iSign].Signature;
                                    if (thoseSigns[iSign].IsVicariale == true)
                                        thisParerProtocol.VicarioFirmatario1 = XSD.Protocollo.DatiSpecificiTypeVicarioFirmatario1.SI;
                                    else
                                        thisParerProtocol.VicarioFirmatario1 = XSD.Protocollo.DatiSpecificiTypeVicarioFirmatario1.NO;
                                    thisParerProtocol.VicarioFirmatario1Specified = true;
                                    break;
                                case 1:
                                    thisParerProtocol.Firmatario2 = thoseSigns[iSign].Signature;
                                    if (thoseSigns[iSign].IsVicariale == true)
                                        thisParerProtocol.VicarioFirmatario2 = XSD.Protocollo.DatiSpecificiTypeVicarioFirmatario2.SI;
                                    else
                                        thisParerProtocol.VicarioFirmatario2 = XSD.Protocollo.DatiSpecificiTypeVicarioFirmatario2.NO;
                                    thisParerProtocol.VicarioFirmatario2Specified = true;
                                    break;
                                case 2:
                                    thisParerProtocol.Firmatario3 = thoseSigns[iSign].Signature;
                                    if (thoseSigns[iSign].IsVicariale == true)
                                        thisParerProtocol.VicarioFirmatario3 = XSD.Protocollo.DatiSpecificiTypeVicarioFirmatario3.SI;
                                    else
                                        thisParerProtocol.VicarioFirmatario3 = XSD.Protocollo.DatiSpecificiTypeVicarioFirmatario3.NO;
                                    thisParerProtocol.VicarioFirmatario3Specified = true;
                                    break;
                                case 3:
                                    thisParerProtocol.Firmatario4 = thoseSigns[iSign].Signature;
                                    if (thoseSigns[iSign].IsVicariale == true)
                                        thisParerProtocol.VicarioFirmatario4 = XSD.Protocollo.DatiSpecificiTypeVicarioFirmatario4.SI;
                                    else
                                        thisParerProtocol.VicarioFirmatario4 = XSD.Protocollo.DatiSpecificiTypeVicarioFirmatario4.NO;
                                    thisParerProtocol.VicarioFirmatario4Specified = true;
                                    break;
                                case 4:
                                    thisParerProtocol.Firmatario5 = thoseSigns[iSign].Signature;
                                    if (thoseSigns[iSign].IsVicariale == true)
                                        thisParerProtocol.VicarioFirmatario5 = XSD.Protocollo.DatiSpecificiTypeVicarioFirmatario5.SI;
                                    else
                                        thisParerProtocol.VicarioFirmatario5 = XSD.Protocollo.DatiSpecificiTypeVicarioFirmatario5.NO;
                                    thisParerProtocol.VicarioFirmatario5Specified = true;
                                    break;
                            }
                        }
                    }

                    thisParerProtocol.VersioneDatiSpecifici = "1.0"; 

                    XmlSerializer serializerProtocol = new XmlSerializer(typeof(XSD.Protocollo.DatiSpecificiType));
                    StringWriter writerProtocol = new StringWriter() ;
                    serializerProtocol.Serialize(writerProtocol, thisParerProtocol) ; 
                    writerProtocol.Close() ;
                    XmlDocument xmlProtocol = new XmlDocument() ; 
                    xmlProtocol.LoadXml(writerProtocol.ToString()) ;

                    thisParerDoc.DocumentoPrincipale.DatiSpecifici.Any = new XmlElement[1] ;
                    thisParerDoc.DocumentoPrincipale.DatiSpecifici.Any[0] = xmlProtocol.DocumentElement ;

                    writerProtocol.Dispose(); 

                    return thisParerDoc; 
                }
        */

        /// <summary>
        /// compila la sezione del documento principale
        /// </summary>
        /// <param name="thisParerDoc">Istanza del documento Parer</param>
        /// <param name="thisDocument">Istanza del documento DocSuiteWeb</param>
        /// <returns>Istanza modificata del documento parer</returns>
        public static UnitaDocumentaria GetDocumentoPrincipale(UnitaDocumentaria thisParerDoc, Document thisDocument)
        {
            thisParerDoc.NumeroAnnessi = "0";
            thisParerDoc.NumeroAnnotazioni = "0";

            thisParerDoc.DocumentoPrincipale = new DocumentoType();

            thisParerDoc.DocumentoPrincipale.IDDocumento = thisDocument.Documento.IDDocumentKey(0);
            thisParerDoc.DocumentoPrincipale.TipoDocumento = thisDocument.Documento.TipoDocumento;

            thisParerDoc.DocumentoPrincipale.ProfiloDocumento = new ProfiloDocumentoType();
            thisParerDoc.DocumentoPrincipale.ProfiloDocumento.Descrizione = thisDocument.Documento.Descrizione;
            thisParerDoc.DocumentoPrincipale.ProfiloDocumento.Autore = thisDocument.Documento.Autore;

            // thisParerDoc.DocumentoPrincipale.StrutturaOriginale = new StrutturaType();
            thisParerDoc.DocumentoPrincipale.StrutturaOriginale = new StrutturaType();
            thisParerDoc.DocumentoPrincipale.StrutturaOriginale.TipoStruttura = "DocumentoGenerico";

            // Un documento = una componente 
            thisParerDoc.DocumentoPrincipale.StrutturaOriginale.Componenti = new ComponenteType[1];
            thisParerDoc.DocumentoPrincipale.StrutturaOriginale.Componenti[0] = new ComponenteType();
            thisParerDoc.DocumentoPrincipale.StrutturaOriginale.Componenti[0].ID = thisParerDoc.DocumentoPrincipale.IDDocumento;
            thisParerDoc.DocumentoPrincipale.StrutturaOriginale.Componenti[0].OrdinePresentazione = "1";

            // da chiarire con il Parer cfr. pag. 24 della documentazione
            // thisParerDoc.DocumentoPrincipale.StrutturaOriginale.Componenti[0].TipoComponente = thisDocument.Documento.GetTipoFirma;
            thisParerDoc.DocumentoPrincipale.StrutturaOriginale.Componenti[0].TipoComponente = "Contenuto";

            thisParerDoc.DocumentoPrincipale.StrutturaOriginale.Componenti[0].TipoSupportoComponente = TipoSupportoType.FILE;
            thisParerDoc.DocumentoPrincipale.StrutturaOriginale.Componenti[0].NomeComponente = thisDocument.Documento.NomeDocumento;
            thisParerDoc.DocumentoPrincipale.StrutturaOriginale.Componenti[0].FormatoFileVersato = thisDocument.Documento.FormatoDocumento;
            if (thisDocument.Documento.HashDocumento != null)
            {
                string HashDoc = Convert.ToBase64String(thisDocument.Documento.HashDocumento);
                thisParerDoc.DocumentoPrincipale.StrutturaOriginale.Componenti[0].HashVersato = HashDoc;
            }
            // Parer Tommasetti 20120112
            // thisParerDoc.DocumentoPrincipale.StrutturaOriginale.Componenti[0].UtilizzoDataFirmaPerRifTemp = true;

            // Piccoli 20120119 La data di riferimento è la data per i firmati 
            if (thisDocument.Documento.TipoFirma == typeFirma.FIRMATO)
            {
                thisParerDoc.DocumentoPrincipale.StrutturaOriginale.Componenti[0].UtilizzoDataFirmaPerRifTemp = false;
                thisParerDoc.DocumentoPrincipale.StrutturaOriginale.Componenti[0].UtilizzoDataFirmaPerRifTempSpecified = true;

                DateTime rifTemporale = thisDocument.Data;
                if (rifTemporale.Hour == 0 && rifTemporale.Minute == 0 && rifTemporale.Second == 0)
                    rifTemporale.Add(new TimeSpan(23, 59, 59));

                thisParerDoc.DocumentoPrincipale.StrutturaOriginale.Componenti[0].RiferimentoTemporale = rifTemporale;
                thisParerDoc.DocumentoPrincipale.StrutturaOriginale.Componenti[0].RiferimentoTemporaleSpecified = true;

                thisParerDoc.DocumentoPrincipale.StrutturaOriginale.Componenti[0].DescrizioneRiferimentoTemporale = "data firma";
            }
            else
            {
                thisParerDoc.DocumentoPrincipale.StrutturaOriginale.Componenti[0].UtilizzoDataFirmaPerRifTemp = false;
                thisParerDoc.DocumentoPrincipale.StrutturaOriginale.Componenti[0].RiferimentoTemporaleSpecified = false;
            }

            return thisParerDoc;
        }
    }
}
