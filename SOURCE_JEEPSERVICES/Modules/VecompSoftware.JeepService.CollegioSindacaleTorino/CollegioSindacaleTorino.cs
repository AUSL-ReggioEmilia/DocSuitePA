using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.Helpers;
using VecompSoftware.Helpers.Compress;
using VecompSoftware.JeepService.Common;
using VecompSoftware.Services.Biblos.Models;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.JeepService.CollegioSindacaleTorino
{
    public class CollegioSindacaleTorino : JeepModuleBase<CollegioSindacaleTorinoParameters>
    {
        #region [ Methods ]

        public override void SingleWork()
        {
            FileLogger.Info(Name, "Avvio ciclo di lavoro.");

            IList<PECOC> listToElaborate = Tools.Factory.PECOCFacade.GetByStatus(PECOCStatus.Aggiunto);
            foreach (var item in listToElaborate)
            {
                if (Cancel)
                {
                    FileLogger.Info(Name, "Chiusura modulo invocata dall'utente.");
                    return;
                }

                try
                {
                    ElaboratePecOc(item);
                }
                catch (Exception exception)
                {
                    SetPecOcStatus(item, PECOCStatus.Errore, exception.Message);
                    FileLogger.Warn(Name, string.Format("Errore bloccante in elaborazione PECOC [{0}].", item.Id), exception);
                }
            }

            SetCompleteToSended(Tools.Factory.PECOCFacade.GetByStatus(PECOCStatus.Completo));
        }

        private void ElaboratePecOc(PECOC item)
        {
            FileLogger.Info(Name, string.Format("Inizio elaborazione PECOC [{0}].", item.Id));

            SetPecOcStatus(item, PECOCStatus.Elaborazione, "");

            FileInfo attachmentFile = Extract(item);
            attachmentFile.Refresh();
            if (!attachmentFile.Exists)
            {
                SetPecOcStatus(item, PECOCStatus.Vuoto, "");
                return;
            }

            // Creazione PEC e invio
            var pecMail = Tools.Factory.PECMailFacade.InstantiateOutgoing(
            DocSuiteContext.Current.ResolutionEnv.MailBoxCollegioSindacale.MailBoxName,
            DocSuiteContext.Current.ResolutionEnv.EmailCollegioSindacale,
            DocSuiteContext.Current.ResolutionEnv.OggettoCollegioSindacale,
            DocSuiteContext.Current.ResolutionEnv.TestoCollegioSindacale,
            DocSuiteContext.Current.ResolutionEnv.MailBoxBozze);

            Tools.Factory.PECMailFacade.Save(ref pecMail);

            var document = new FileDocumentInfo(attachmentFile);

            Tools.Factory.PECMailFacade.ArchiveAttachment(ref pecMail, document.Stream, document.Name, false);
            Tools.Factory.PECMailFacade.ActivatePec(pecMail);
            Tools.Factory.PECMailLogFacade.Created(ref pecMail);

            // Lego la mail al pecOc e salvo col nuovo stato
            item.IdMail = pecMail.Id;
            SetPecOcStatus(item, PECOCStatus.Completo, "");

            FileLogger.Info(Name, string.Format("Fine elaborazione PECOC [{0}].", item.Id));
        }

        /// <summary>Controlla tutte le PECOC complete e le mette in invio.</summary>
        /// <remarks>Se sono nella mailbox adibita all'invio imposta lo stato successivo.</remarks>
        private void SetCompleteToSended(IList<PECOC> pecOcList)
        {
            int sentMails = 0;
            foreach (var pecOc in pecOcList)
            {
                if (Cancel)
                {
                    FileLogger.Info(Name, "Chiusura modulo invocata dall'utente.");
                    return;
                }
                if (!pecOc.IdMail.HasValue)
                {
                    FileLogger.Warn(Name, string.Format("Il registro [{0}] anche se completo non ha PEC.", pecOc.Id));
                    continue;
                }

                // Controllo che la mail del pecoc è nella box corretta per l'invio
                PECMail pec = Tools.Factory.PECMailFacade.GetById(pecOc.IdMail.Value);
                if (pec == null)
                {
                    FileLogger.Warn(Name, string.Format("Impossibile trovare la PEC [{0}] del registro [{1}].", pecOc.IdMail.Value, pecOc.Id));
                    continue;
                }
                if (pec.MailBox.Id != DocSuiteContext.Current.ResolutionEnv.MailBoxCollegioSindacale.Id)
                {
                    FileLogger.Warn(Name, string.Format("Pec del registro [{0}] completa ma nella mailbox errata: spostare la pec o eliminare il registro.", pecOc.Id));
                    continue;
                }

                SetPecOcStatus(pecOc, PECOCStatus.Spedito, "");
                sentMails++;
            }
            FileLogger.Info(Name, string.Format("Trovati [{0}] registri completati, impostati [{1}] a spedito.", pecOcList.Count, sentMails));
        }

        /// <summary> Estrae gli atti inerenti una ricerca specificata nell'oggetto di tipo <see cref="PECOC"/> e crea uno zip. </summary>
        /// <returns><see cref="FileInfo"/> rappresentante il file zip con i documenti richiesti.</returns>
        private FileInfo Extract(PECOC pecOc)
        {
            FileLogger.Info(Name, string.Format("Estrazione PECOC [{0}] iniziata.", pecOc.Id));

            // Mi assicuro che la directory di destinazione sia vuota
            var extractFolder = new DirectoryInfo(Parameters.ExtractFolder);
            if (extractFolder.Exists)
            {
                extractFolder.Delete(true);
            }
            extractFolder.Refresh();
            extractFolder.Create();
            extractFolder.Refresh();

            // Directory temporanea per l'estrazione
            var temporaryFolder = new DirectoryInfo(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
            if (temporaryFolder.Exists)
            {
                temporaryFolder.Delete(true);
            }
            temporaryFolder.Create();
            temporaryFolder.Refresh();

            // Ritiro tutti gli atti richiesti
            IList<Resolution> resolutions = Tools.Factory.ResolutionFacade.GetBySupervisoryBoardDate(pecOc.FromDate, pecOc.ToDate, pecOc.ResolutionType);

            if (resolutions.Count == 0)
            {
                var message = new StringBuilder();
                message.AppendFormat("Nessun atto di tipo [{0}] trovato ", pecOc.ResolutionType.Description);
                if (pecOc.ToDate.HasValue)
                {
                    message.AppendFormat("nel periodo dal {0} al {1}", pecOc.FromDate, pecOc.ToDate.Value);
                }
                else
                {
                    message.AppendFormat("il giorno {0}", pecOc.FromDate);
                }
                FileLogger.Warn(Name, message.ToString());
            }
            else
            {
                var message = new StringBuilder();
                message.AppendFormat("{0} [{1}] atti di tipo [{2}] ",
                    resolutions.Count == 1 ? "Trovato" : "Trovati",
                    resolutions.Count,
                    pecOc.ResolutionType.Description);
                if (pecOc.ToDate.HasValue)
                {
                    message.AppendFormat("nel periodo dal {0} al {1}", pecOc.FromDate, pecOc.ToDate.Value);
                }
                else
                {
                    message.AppendFormat("il giorno {0}", pecOc.FromDate);
                }
                FileLogger.Info(Name, message.ToString());
            }

            // Estraggo tutti gli atti
            int successfullyExtracted = 0;
            foreach (var resolution in resolutions)
            {
                try
                {
                    if (ExtractResolutionFiles(resolution, temporaryFolder, pecOc.ExtractAttachments))
                    {
                        successfullyExtracted++;
                    }
                }
                catch (Exception exception)
                {
                    // Errori non previsti
                    FileLogger.Warn(Name, string.Format("Problema di estrazione atto numero {0}.", resolution.Id), exception);
                }
            }

            string toDate = "";
            if (pecOc.ResolutionType.Id == ResolutionType.IdentifierDetermina)
            {
                if (!pecOc.ToDate.HasValue)
                {
                    throw new CollegioSindacaleTorinoException("Determina relativa al registro [{0}] non conforme.", pecOc.Id);
                }
                toDate = "_" + pecOc.ToDate.Value.ToString("dd.MM.yyyy");
            }
            // Risolvo il nome dello zip
            string fileName = string.Format("{0}_{1:dd.MM.yyyy}{2}{3}",
                Tools.ResolutionTypeCaptionPlural(pecOc.ResolutionType.Id).ToLower(), pecOc.FromDate, toDate, FileHelper.ZIP);

            var zipped = new FileInfo(Path.Combine(extractFolder.FullName, fileName));

            // Comprimo i documenti in uno zip cancellando il file se esiste già
            if (successfullyExtracted != 0)
            {
                ZipCompress zip = new ZipCompress();
                zip.Compress(temporaryFolder.FullName, zipped.FullName);

                zipped.Refresh();
                if (zipped.Exists)
                {
                    FileLogger.Info(Name, string.Format("Creato il file [{0}].", zipped.FullName));
                }
                else
                {
                    FileLogger.Warn(Name, "Nessun file creato.");
                }
            }

            int possibleErrors = resolutions.Count - successfullyExtracted;
            if (possibleErrors > 0)
            {
                FileLogger.Warn(Name, string.Format("Trovati [{0}] atti senza documenti allegati o con errori.", possibleErrors));
            }
            else
            {
                FileLogger.Info(Name, string.Format("Estrazione di [{0}] atti completata.", resolutions.Count));
            }

            return zipped;
        }

        /// <summary> Estrae da un singolo atto i documenti depositandoli nella directory. </summary>
        /// <returns> True se l'atto è stato processato correttamente (se aveva file e sono stati estratti o se non aveva file). </returns>
        private bool ExtractResolutionFiles(Resolution resolution, DirectoryInfo temporaryFolder, bool extractAttachments)
        {
            bool success = false;

            FileLogger.Info(Name, string.Format("Estrazione atto id: {0}", resolution.Id));

            // Recupero l'id del file principale dell'atto
            int? idResolutionFile = Tools.Factory.FileResolutionFacade.GetByResolution(resolution)[0].IdResolutionFile;
            if (idResolutionFile.HasValue)
            {
                var caption = Tools.ResolutionTypeCaption(resolution.Type.Id);
                // Recupero il documento dell'atto
                string fileName = string.Format("{0}_{1:000}_{2}_{3}",
                    caption,
                    resolution.Number.GetValueOrDefault(0),
                    resolution.ResolutionContactProposers[0].Contact.Code,
                    resolution.Year);

                var chain = new BiblosChainInfo(resolution.Container.ReslLocation.DocumentServer, resolution.Container.ReslLocation.ReslBiblosDSDB, idResolutionFile.Value);

                if (SaveDocuments(chain.ArchivedDocuments, fileName, temporaryFolder))
                {
                    success = true;
                    FileLogger.Info(Name, string.Format("Salvata {0} numero [{1}].", caption, resolution.Number));
                }
            }
            else
            {
                // Se l'atto non ha files l'operazione è da considerarsi andata a buon fine
                success = true;
            }

            if (!extractAttachments)
            {
                return success;
            }

            // recupero il documento del protocollo legato al collegio sindacale nell'atto
            Protocol protocol;
            if (string.IsNullOrEmpty(resolution.SupervisoryBoardProtocolLink))
            {
                protocol = null;
            }
            else
            {
                // recupero il documento del protocollo
                short protocolYear;
                int protocolNumber;
                if (!short.TryParse(Resolution.FormatProtocolLink(resolution.SupervisoryBoardProtocolLink, "Y"), out protocolYear) ||
                    !int.TryParse(Resolution.FormatProtocolLink(resolution.SupervisoryBoardProtocolLink, "N"), out protocolNumber))
                {
                    throw new CollegioSindacaleTorinoException("Il protocollo [{0}] non sembra essere formattato correttamente.", resolution.SupervisoryBoardProtocolLink);
                }

                protocol = Tools.Factory.ProtocolFacade.GetById(protocolYear, protocolNumber);
            }

            if (protocol == null)
            {
                return success;
            }

            // Recupero la lettera di trasmissione al collegio sindacale
            string transmissionFileName = string.Format("lettera_trasmissione_{0}_{1}", protocol.Number, protocol.Year);
            var document = ProtocolFacade.GetDocument(protocol);
            if (SaveDocuments(new List<BiblosDocumentInfo> { document }, transmissionFileName, temporaryFolder))
            {
                FileLogger.Info(Name, string.Format("Salvata lettera di trasmissione numero [{0}].", protocol.FullNumber));
                return true;
            }

            // Se il protocollo esiste ma non riesco ad estrarre il file lo considero un errore
            return false;
        }

        public bool SaveDocuments(List<BiblosDocumentInfo> documents, string filename, DirectoryInfo destination)
        {
            bool somethingSaved = false;
            int index = 1;
            foreach (var documentInfo in documents)
            {
                string completeFileName = string.Format("{0}{1}{2}", filename, documents.Count > 1 ? index.ToString(CultureInfo.InvariantCulture) : "", FileHelper.PDF);

                var newFile = documentInfo.SavePdf(destination, completeFileName, "");

                if (newFile.Exists)
                {
                    FileLogger.Info(Name, string.Format("Estratto documento [{0}] dal documento biblos [{1}].", completeFileName, documentInfo));

                    somethingSaved = true;
                }
                index++;
            }
            return somethingSaved;
        }

        /// <summary> Cambia lo status e salva l'oggetto. </summary>
        /// <param name="item">Oggetto da salvare</param>
        /// <param name="newStatus">Status da mettere nell'oggetto e da notificare nel messaggio del log</param>
        /// <param name="aggiuntiveMessage">Messaggio da aggiungere a quello default dello status</param>
        private static void SetPecOcStatus(PECOC item, PECOCStatus newStatus, string aggiuntiveMessage)
        {
            item.Status = newStatus;

            Tools.Factory.PECOCFacade.Update(ref item);

            Tools.Factory.PECOCLogFacade.InsertLog(item, aggiuntiveMessage);
        }

        #endregion
    }
}
