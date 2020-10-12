using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Objects.Enums;
using BiblosDS.Library.Common.Objects.Response;
using BiblosDS.Library.Common.Preservation.Services;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.BiblosDS.Model.CQRS;
using VecompSoftware.BiblosDS.Model.CQRS.Notifications;
using VecompSoftware.BiblosDS.Model.CQRS.Notifications.Preservations;
using VecompSoftware.BiblosDS.Model.CQRS.Preservations;

namespace VecompSoftware.BiblosDS.WindowsService.WebAPI.Receivers.Preservations
{
    public class ExecutePreservationReceiver : Receiver
    {
        #region [ Fields ]
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ExecutePreservationReceiver));
        private readonly PreservationService _preservationService;
        private CommandModel _commandModel;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public ExecutePreservationReceiver(IReceiverMediator mediator)
            : base(mediator)
        {
            _preservationService = new PreservationService();
            _preservationService.PulseHighFrequencyEnabled = true;
            _preservationService.OnPulse += PreservationService_OnPulse;
        }
        #endregion

        #region [ Methods ]
        private void PreservationService_OnPulse(object sender, PreservationService.PulseEventArgs e)
        {
            SendNotification(_commandModel.ReferenceId, e.Message, NotifyLevel.Info).Wait();
        }

        public override async Task Execute(CommandModel commandModel)
        {
            PreservationTask preservationTask = null;
            try
            {
                _commandModel = commandModel;
                if (!(commandModel is CommandExecutePreservation))
                {
                    _logger.Error($"Command is not of type {nameof(CommandExecutePreservation)}");
                    await SendNotification(commandModel.ReferenceId, "E' avvenuto un errore durante la gestione del comando di esecuzione conservazione", NotifyLevel.Error, true);
                    return;
                }
                
                CommandExecutePreservation @command = commandModel as CommandExecutePreservation;
                if (command.IdTask == Guid.Empty)
                {
                    _logger.Error($"Command with idTask not defined");
                    await SendNotification(command.ReferenceId, "Non è stato definito un ID Task per l'esecuzione della conservazione", NotifyLevel.Error, true);
                    return;
                }

                preservationTask = _preservationService.GetPreservationTask(command.IdTask);
                if (preservationTask == null)
                {
                    _logger.Error($"Task {command.IdTask} not found");
                    await SendNotification(command.ReferenceId, $"Non è stato trovato un Task con id {command.IdTask}", NotifyLevel.Error, true);
                    return;
                }
                PreservationInfoResponse response = _preservationService.CreatePreservation(preservationTask);
                if (response.HasErros)
                {
                    await SendNotification(command.ReferenceId, $"Errore nell'attività di conservazione del task con id {command.IdTask}: {response.Error.Message}", NotifyLevel.Error, true);
                    return;
                }                

                if (preservationTask.TaskType.Type == PreservationTaskTypes.Preservation && command.AutoGenerateNextTask)
                {
                    try
                    {
                        _preservationService.AutoGenerateNextTask(preservationTask);
                    }
                    catch (Exception ex)
                    {
                        _logger.Warn($"E' avvenuto un errore durante la generazione del task di conservazione successivo al task {command.IdTask}", ex);
                        await SendNotification(command.ReferenceId, $"Non è stato possibile generare il task di conservazione successivo al task {command.IdTask}", NotifyLevel.Warning);
                    }                    
                }

                if (response.AwardBatchesXml != null && response.AwardBatchesXml.Count > 0)
                {
                    foreach (KeyValuePair<Guid, string> awardBatchXml in response.AwardBatchesXml)
                    {
                        string serializedContent = Convert.ToBase64String(Encoding.UTF8.GetBytes(awardBatchXml.Value));
                        if (!string.IsNullOrEmpty(command.PDVArchive))
                        {
                            CommandInsertPreservationPDV pdvCommand = new CommandInsertPreservationPDV()
                            {
                                PDVArchive = command.PDVArchive,
                                ReferenceId = command.ReferenceId,
                                IdAwardBatch = awardBatchXml.Key,
                                Content = serializedContent
                            };
                            await Mediator.Send(pdvCommand);
                        }

                        if (!string.IsNullOrEmpty(command.RDVArchive))
                        {
                            CommandInsertPreservationRDV rdvCommand = new CommandInsertPreservationRDV()
                            {
                                RDVArchive = command.RDVArchive,
                                ReferenceId = command.ReferenceId,
                                IdAwardBatch = awardBatchXml.Key,
                                Content = serializedContent
                            };
                            await Mediator.Send(rdvCommand);
                        }
                    }
                }
                await SendNotification(command.ReferenceId, $"Processo di conservazione terminato", NotifyLevel.Info, true);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error on execute preservation task", ex);
                await SendNotification(commandModel.ReferenceId, $"E' avvenuto un errore durante l'attività richiesta", NotifyLevel.Error, true);
            }
            finally
            {
                if (preservationTask != null)
                {
                    _preservationService.UnlockTask(preservationTask);
                }
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
