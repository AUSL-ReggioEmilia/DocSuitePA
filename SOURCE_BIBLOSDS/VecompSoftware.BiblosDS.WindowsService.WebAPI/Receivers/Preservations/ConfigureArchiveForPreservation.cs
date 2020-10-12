using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Preservation.Services;
using BiblosDS.Library.Common.Services;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.BiblosDS.Model.CQRS;
using VecompSoftware.BiblosDS.Model.CQRS.Notifications;
using VecompSoftware.BiblosDS.Model.CQRS.Notifications.Preservations;
using VecompSoftware.BiblosDS.Model.CQRS.Preservations;

namespace VecompSoftware.BiblosDS.WindowsService.WebAPI.Receivers.Preservations
{
    public class ConfigureArchiveForPreservation : Receiver
    {
        #region [ Fields ]
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ConfigureArchiveForPreservation));
        private readonly PreservationService _preservationService;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public ConfigureArchiveForPreservation(IReceiverMediator mediator)
            : base(mediator)
        {
            _preservationService = new PreservationService();
        }
        #endregion

        #region [ Methods ]
        public override async Task Execute(CommandModel commandModel)
        {
            try
            {
                if (!(commandModel is CommandConfigureArchiveForPreservation))
                {
                    _logger.Error($"Command is not of type {nameof(CommandConfigureArchiveForPreservation)}");
                    return;
                }

                CommandConfigureArchiveForPreservation @command = commandModel as CommandConfigureArchiveForPreservation;
                await SendNotification(command.ReferenceId, $"Inizio attività di configurazione per l'archivio con id {command.IdArchive}", NotifyLevel.Info);

                DocumentArchive currentArchive = ArchiveService.GetArchive(command.IdArchive);
                if (currentArchive == null)
                {
                    _logger.Error($"Archive with Id {command.IdArchive} not found");
                    throw new Exception($"Non è stato trovato un archivio con ID {command.IdArchive}");
                }

                Company currentCompany = ArchiveService.GetCompany(command.IdCompany);
                if (currentCompany == null)
                {
                    _logger.Error($"Company with Id {command.IdCompany} not found");
                    throw new Exception($"Non è stata trovata una azienda con ID {command.IdCompany}");
                }

                BindingList<DocumentAttribute> archiveAttributes = AttributeService.GetAttributesFromArchive(command.IdArchive);
                if (archiveAttributes == null || archiveAttributes.Count == 0)
                {
                    _logger.Error($"There aren't attributes for archive with Id {command.IdArchive}");
                    throw new Exception($"Non sono stati trovati attributi per l'archivio con ID {command.IdArchive}");
                }

                if (string.IsNullOrEmpty(currentArchive.PathPreservation))
                {
                    _logger.Error($"PathPreservation is null for archive with Id {command.IdArchive}");
                    throw new Exception($"Non è stato definito il percorso di conservazione per l'archivio con ID {command.IdArchive}");
                }

                DocumentAttribute mainDateAttribute = archiveAttributes.SingleOrDefault(x => x.IsMainDate == true);
                if (mainDateAttribute == null)
                {
                    _logger.Error($"MainDate attribute is not defined for archive with Id {command.IdArchive}");
                    throw new Exception($"Non è stato definito l'attributo di tipo MainDate per l'archivio con ID {command.IdArchive}");
                }

                ICollection<DocumentAttribute> preservationAttributes = archiveAttributes.Where(x => x.ConservationPosition > 0).ToList();
                if (preservationAttributes.Count == 0)
                {
                    _logger.Error($"There aren't conservation attributes for archive with Id {command.IdArchive}");
                    throw new Exception($"Non sono stati definiti gli attributi di conservazione per l'archivio con ID {command.IdArchive}");
                }

                if (_preservationService.ExistPreservationsByArchive(currentArchive))
                {
                    _logger.Error($"The archive {command.IdArchive} already has some conservation done");
                    throw new Exception($"L'archivio {command.IdArchive} presenta già delle conservazioni eseguite");
                }

                _logger.Info($"Enable archive {currentArchive.Name} to conservation");
                await SendNotification(commandModel.ReferenceId, $"Abilitazione archivio {currentArchive.Name}({currentArchive.IdArchive}) alla conservazione", NotifyLevel.Info);
                currentArchive.IsLegal = true;               
                ArchiveService.UpdateArchive(currentArchive, false);

                ICollection<Document> documents = DocumentService.GetDocumentsFromArchive(currentArchive);
                _logger.Debug($"Found {documents.Count} documents to process");
                await SendNotification(commandModel.ReferenceId, $"Trovati {documents.Count} documenti da processare. L'attività può richiedere diversi minuti...", NotifyLevel.Info);
                BindingList<DocumentAttributeValue> documentAttributeValues;
                DateTime? mainDate;
                string primaryKey;
                decimal percentage,
                    pulsePercentageLimit = 20;
                int progress = 1,
                    totalItems = documents.Count,
                    totalErrorDocuments = 0;
                ICollection<NotificationDetailModel> errorDetails = new List<NotificationDetailModel>();
                foreach (Document document in documents)
                {
                    try
                    {
                        _logger.Debug($"Process document {document.IdDocument}({progress} di {totalItems})");                        
                        documentAttributeValues = AttributeService.GetFullDocumentAttributeValues(document.IdDocument);
                        mainDate = document.DateCreated;
                        primaryKey = AttributeService.ParseAttributeValues(documentAttributeValues, archiveAttributes, out mainDate);
                        DocumentService.UpdatePrimaryKeyAndDateMainDocument(document, mainDate, primaryKey);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Error on update document {document.IdDocument}", ex);
                        errorDetails.Add(new NotificationDetailModel()
                        {
                            Id = document.IdDocument,
                            ActivityName = $"Modifica primary key e maindate del documento {document.IdDocument}",
                            Detail = $"{ex.Message}\r\n{ex.StackTrace}"
                        });
                        totalErrorDocuments++;
                    }

                    percentage = ((decimal)progress / totalItems) * 100.0m;
                    if (Math.Ceiling(percentage) > pulsePercentageLimit)
                    {
                        await SendNotification(commandModel.ReferenceId, $"Migrazione documenti ({pulsePercentageLimit}%, {totalErrorDocuments} in errore)", (totalErrorDocuments > 0 ? NotifyLevel.Warning : NotifyLevel.Info), errorDetails);
                        pulsePercentageLimit += 20;
                    }
                    progress++;
                }
                await SendNotification(commandModel.ReferenceId, $"Migrazione documenti (100%, {totalErrorDocuments} in errore)", (totalErrorDocuments > 0 ? NotifyLevel.Warning : NotifyLevel.Info));

                _logger.Info("Build Index and AwardBatch style files");
                await SendNotification(commandModel.ReferenceId, "Creazione fogli di stile per visualizzazione file di Indice e Lotti", NotifyLevel.Info);
                string indexXsl = _preservationService.BuildPreservationIndexXSL(currentArchive);
                string awardBatchXsl = _preservationService.GetAwardBatchXSL();

                ArchiveCompany archiveCompany = _preservationService.GetArchiveCompanies(currentArchive.IdArchive).SingleOrDefault();
                if (archiveCompany == null)
                {
                    archiveCompany = new ArchiveCompany()
                    {
                        IdArchive = currentArchive.IdArchive,
                        IdCompany = currentCompany.IdCompany,
                        WorkingDir = $@"{currentArchive.PathPreservation}\{currentArchive.Name}\Comunicazioni",
                        XmlFileTemplatePath = $@"{currentArchive.PathPreservation}\{currentArchive.Name}\Comunicazioni\Template.xml"
                    };
                    archiveCompany = ArchiveService.AddArchiveCompany(archiveCompany);
                }
                archiveCompany.AwardBatchXSLTFile = awardBatchXsl;
                archiveCompany.TemplateXSLTFile = indexXsl;
                ArchiveService.UpdateArchiveCompany(archiveCompany);

                StringBuilder resultBuilder = new StringBuilder();
                resultBuilder.Append($"Archivio {currentArchive.Name} configurato correttamente per la conservazione");
                if (totalErrorDocuments > 0)
                {
                    resultBuilder.Append(". Alcuni documenti non sono stati migrati e potrebbero dare errore in fase di conservazione");
                }
                await SendNotification(commandModel.ReferenceId, resultBuilder.ToString(), NotifyLevel.Info, true);
            }
            catch (Exception ex)
            {
                _logger.Error("Error on configure archive for preservation", ex);                
                await SendNotification(commandModel.ReferenceId, $"Errore nella configurazione dell'archivio per la conservazione", NotifyLevel.Error, new List<NotificationDetailModel>()                
                {
                    new NotificationDetailModel()
                    {
                        Id = Guid.NewGuid(),
                        ActivityName = $"Errore configurazione archivo",
                        Detail = $"{ex.Message}\r\n{ex.StackTrace}"
                    }
                }, true);
            }
        }

        private async Task SendNotification(string referenceId, string message, NotifyLevel level, bool isComplete = false)
        {
            await SendNotification(referenceId, message, level, null, isComplete);
        }

        private async Task SendNotification(string referenceId, string message, NotifyLevel level, ICollection<NotificationDetailModel> details, bool isComplete = false)
        {
            CommandPreservationNotify command = new CommandPreservationNotify();
            command.Message = message;
            command.NotifyLevel = level;
            command.ReferenceId = referenceId;
            command.Complete = isComplete;
            command.Details = details;

            await Mediator.Send(command);
        }
        #endregion
    }
}
