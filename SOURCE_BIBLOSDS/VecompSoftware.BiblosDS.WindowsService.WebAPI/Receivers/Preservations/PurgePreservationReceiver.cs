using BiblosDS.Library.Common;
using BiblosDS.Library.Common.DB;
using BiblosDS.Library.Common.Enums;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Preservation.Services;
using BiblosDS.Library.Common.Services;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using VecompSoftware.BiblosDS.Model.CQRS;
using VecompSoftware.BiblosDS.Model.CQRS.Notifications;
using VecompSoftware.BiblosDS.Model.CQRS.Notifications.Preservations;
using VecompSoftware.BiblosDS.Model.CQRS.Preservations;
using DocumentStorage = VecompSoftware.BiblosDS.Service.Storage;

namespace VecompSoftware.BiblosDS.WindowsService.WebAPI.Receivers.Preservations
{
    public class PurgePreservationReceiver : Receiver
    {
        #region [ Fields ]
        private static readonly ILog _logger = LogManager.GetLogger(typeof(PurgePreservationReceiver));
        private readonly PreservationService _preservationService;
        private readonly DocumentStorage.StorageService _storageService;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public PurgePreservationReceiver(IReceiverMediator mediator)
            : base(mediator)
        {
            _preservationService = new PreservationService();
            _storageService = new DocumentStorage.StorageService();
        }
        #endregion

        #region [ Methods ]
        public override async Task Execute(CommandModel commandModel)
        {
            try
            {
                if (!(commandModel is CommandPurgePreservation))
                {
                    _logger.Error($"Command is not of type {nameof(CommandPurgePreservation)}");
                    await SendNotification(commandModel.ReferenceId, "E' avvenuto un errore durante la gestione del comando di pulizia storage", NotifyLevel.Error, true);
                    return;
                }

                CommandPurgePreservation @command = commandModel as CommandPurgePreservation;
                _logger.Info($"Process command {command.Id} with preservation id {command.IdPreservation}");
                await SendNotification(command.ReferenceId, "Inizio attività di pulizia archivio corrente degli elementi conservati", NotifyLevel.Info);
                if (command.IdPreservation == Guid.Empty)
                {
                    _logger.Error($"Preservation reference not setted for command {command.Id}");
                    await SendNotification(command.ReferenceId, "E' avvenuto un errore durante la gestione del comando di pulizia archivio corrente", NotifyLevel.Error, true);
                    return;
                }

                if (string.IsNullOrEmpty(command.Executor))
                {
                    _logger.Error($"Process executor not setted for command {command.Id}");
                    await SendNotification(command.ReferenceId, "E' avvenuto un errore durante la gestione del comando di pulizia archivio corrente", NotifyLevel.Error, true);
                    return;
                }

                Preservation preservation = _preservationService.GetPreservation(command.IdPreservation, false);
                if (preservation == null)
                {
                    _logger.Error($"Preservation {command.IdPreservation} not found");
                    await SendNotification(command.ReferenceId, $"Nessuna conservazione trovata con id {command.IdPreservation}", NotifyLevel.Error, true);
                    return;
                }

                if (!preservation.CloseDate.HasValue)
                {
                    _logger.Error($"Preservation {command.IdPreservation} is not closed. Please close preservation before execute this task.");
                    await SendNotification(command.ReferenceId, $"Non è possibile procedere alla pulizia dell'archivio corrente di una conservazione non chiusa", NotifyLevel.Error, true);
                    return;
                }

                Logging.AddLog(LoggingOperationType.BiblosDS_General, preservation.IdArchive,
                    Guid.Empty, preservation.IdPreservation,
                    $"Attività di pulizia storage per la conservazione {preservation.IdPreservation} richiesta dall'utente {command.Executor}",
                    command.Executor);

                int preservedDocumentsCount = _preservationService.CountPreservationDocumentsToPurge(command.IdPreservation);
                _logger.Info($"Found {preservedDocumentsCount} to process");
                if (preservedDocumentsCount == 0)
                {
                    await SendNotification(command.ReferenceId, $"Nessun documento trovato per la conservazione {command.IdPreservation}", NotifyLevel.Info, true);
                    return;
                }
                await SendNotification(command.ReferenceId, $"Trovati {preservedDocumentsCount} documenti da eliminare dall'archivio corrente", NotifyLevel.Info);

                int processedItems = 0;
                decimal pulsePercentageLimit = 20;
                decimal percentage;
                EntityProvider entityProvider;
                await SendNotification(command.ReferenceId, $"Inizio cancellazione documenti, l'attività può richiedere alcuni minuti", NotifyLevel.Info);
                Status statusToChange = new Status(DocumentStatus.MovedToPreservation);

                ICollection<Document> preservedDocuments = _preservationService.GetPreservationDocumentsToPurge(command.IdPreservation);
                string documentPath = string.Empty;
                bool hasMoveErrors = false;
                foreach (Document document in preservedDocuments)
                {
                    processedItems++;
                    _logger.Info($"Process document {document.IdDocument}");
                    documentPath = DocumentService.GetPreservationDocumentPath(document);
                    _logger.Info($"Document {document.IdDocument} with preservation path: {documentPath}");
                    if (string.IsNullOrEmpty(documentPath))
                    {
                        _logger.Info($"Document {document.IdDocument} hasn't preservation path defined. Document skipped");
                        await SendNotification(command.ReferenceId, $"Il documento {document.IdDocument} non ha l'indicazione del percorso di conservazione e pertanto non può essere eliminato dall'archivio corrente", NotifyLevel.Error);
                        continue;
                    }

                    if (!File.Exists(documentPath))
                    {
                        _logger.Info($"File {documentPath} not found on disk for document {document.IdDocument}");
                        await SendNotification(command.ReferenceId, $"Il documento {document.IdDocument} non esiste nel percorso di conservazione {documentPath}", NotifyLevel.Error);
                        continue;
                    }

                    entityProvider = new EntityProvider();
                    using (IDbTransaction transaction = entityProvider.BeginNoSave())
                    {
                        try
                        {
                            _logger.Info($"Document {document.IdDocument} -> change document state to {DocumentStatus.MovedToPreservation.ToString()}");
                            entityProvider.UpdateDocumentStatus(document, statusToChange);
                            entityProvider.SaveChanges();

                            _logger.Info($"Document {document.IdDocument} -> delete document from storage");
                            _storageService.DeleteDocument(document, false);
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            _logger.Info($"Error on delete document {document.IdDocument} from storage. Document skipped.", ex);
                            hasMoveErrors = true;                            
                        }
                    }

                    percentage = ((decimal)processedItems / preservedDocumentsCount) * 100.0m;
                    if (Math.Ceiling(percentage) > pulsePercentageLimit)
                    {
                        await SendNotification(command.ReferenceId, $"Pulizia documenti nell'archivio corrente ({pulsePercentageLimit}%).", NotifyLevel.Info);
                        pulsePercentageLimit += 20;
                    }
                }

                string completeMessage = $"Il processo di pulizia archivio corrente per la conservazione {command.IdPreservation} è terminato correttamente.";
                if (hasMoveErrors)
                {
                    completeMessage = $"Il processo di pulizia archivio corrente per la conservazione {command.IdPreservation} è terminato con errori. Verificare il file di log per ulteriori dettagli.";
                }
                await SendNotification(command.ReferenceId, completeMessage, hasMoveErrors ? NotifyLevel.Error : NotifyLevel.Info, true);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error on purge preservation", ex);
                await SendNotification(commandModel.ReferenceId, $"E' avvenuto un errore durante l'esecuzione dell'attività corrente", NotifyLevel.Error, true);
            }
        }

        private async Task SendNotification(string referenceId, string message, NotifyLevel level, bool isComplete = false)
        {
            CommandPreservationNotify command = new CommandPreservationNotify();
            command.Message = message;
            command.NotifyLevel = level;
            command.ReferenceId = referenceId;
            command.Complete = isComplete;

            await Mediator.Send(command);
        }
        #endregion
    }
}
