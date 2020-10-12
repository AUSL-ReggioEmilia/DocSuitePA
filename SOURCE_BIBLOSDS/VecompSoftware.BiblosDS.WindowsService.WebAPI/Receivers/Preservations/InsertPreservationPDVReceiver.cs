using BiblosDS.Library.Common.Enums;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Preservation.Services;
using BiblosDS.Library.Common.Services;
using BiblosDS.Library.Common.Utility;
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
using VecompSoftware.BiblosDS.WCF.Common;
using VecompSoftware.ServiceContract.BiblosDS.Documents;

namespace VecompSoftware.BiblosDS.WindowsService.WebAPI.Receivers.Preservations
{
    public class InsertPreservationPDVReceiver : Receiver
    {
        #region [ Fields ]
        private static readonly ILog _logger = LogManager.GetLogger(typeof(InsertPreservationPDVReceiver));
        private readonly PreservationService _preservationService;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public InsertPreservationPDVReceiver(IReceiverMediator mediator)
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
                if (!(commandModel is CommandInsertPreservationPDV))
                {
                    _logger.Error($"Command is not of type {nameof(CommandInsertPreservationPDV)}");
                    return;
                }
                
                CommandInsertPreservationPDV @command = commandModel as CommandInsertPreservationPDV;
                await SendNotification(command.ReferenceId, $"Inizio salvataggio PDV per il lotto di versamento con id {command.IdAwardBatch}", NotifyLevel.Info);
                DocumentArchive pdvArchive = ArchiveService.GetArchiveByName(command.PDVArchive);
                if (pdvArchive == null)
                {
                    _logger.Error($"Archive with name {command.PDVArchive} not found");
                    throw new Exception($"Non è stato trovato un archivio con nome {command.PDVArchive}");
                }

                ICollection<DocumentAttribute> attributes = AttributeService.GetAttributesFromArchive(pdvArchive.IdArchive);
                AwardBatch awardBatch = _preservationService.GetAwardBatch(command.IdAwardBatch);
                if (awardBatch == null)
                {
                    _logger.Error($"Award batch with id {command.IdAwardBatch} not found");
                    throw new Exception($"Non è stato trovato un lotto di versamento con id {command.IdAwardBatch}");
                }

                Document document = new Document
                {
                    Content = new DocumentContent() { Blob = Convert.FromBase64String(command.Content) },
                    Name = string.Concat(UtilityService.GetSafeFileName(awardBatch.Name), ".xml"),
                    Archive = pdvArchive,
                    AttributeValues = new BindingList<DocumentAttributeValue>()
                };
                document.AttributeValues.Add(new DocumentAttributeValue()
                {
                    Value = document.Name,
                    Attribute = attributes.Single(f => f.Name.Equals("Filename", StringComparison.InvariantCultureIgnoreCase))
                });
                document.AttributeValues.Add(new DocumentAttributeValue()
                {
                    Value = awardBatch.Name,
                    Attribute = attributes.Single(f => f.Name.Equals("Signature", StringComparison.InvariantCultureIgnoreCase))
                });

                using (var clientChannel = WCFUtility.GetClientConfigChannel<IDocuments>(ServerService.WCF_Document_HostName))
                {
                    document = (clientChannel as IDocuments).AddDocumentToChain(document, null, DocumentContentFormat.Binary);
                }
                awardBatch.IdPDVDocument = document.IdDocument;
                _preservationService.UpdateAwardBatch(awardBatch);
                _logger.Info($"Saved PDV with id {awardBatch.IdPDVDocument} for awardbatch {awardBatch.IdAwardBatch}");
                await SendNotification(command.ReferenceId, $"PDV salvato con id {awardBatch.IdPDVDocument} per il lotto di versamento con id {awardBatch.IdAwardBatch}", NotifyLevel.Info);
            }
            catch (Exception ex)
            {
                _logger.Error("Error on insert PDV", ex);
                await SendNotification(commandModel.ReferenceId, $"Errore nella fase di inserimento PDV", NotifyLevel.Error);
            }            
        }

        private async Task SendNotification(string referenceId, string message, NotifyLevel level)
        {
            CommandPreservationNotify command = new CommandPreservationNotify();
            command.Message = message;
            command.NotifyLevel = level;
            command.ReferenceId = referenceId;

            await Mediator.Send(command);
        }
        #endregion
    }
}
