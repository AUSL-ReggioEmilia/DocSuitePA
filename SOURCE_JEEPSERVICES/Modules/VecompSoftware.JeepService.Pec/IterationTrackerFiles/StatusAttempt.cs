namespace VecompSoftware.JeepService.Pec.IterationTrackerFiles
{
    /// <summary>
    /// Implementazione del Safe Type Enum Pattern. Per avere piu flessibilita, usiamo una classe invece del classico Enum.
    /// Se ci fosse bisogno possiamo aggiungere anche un id per ogni voce
    /// Enum usato per descrivere lo stato del tentativo.
    /// Nel caso in cui si ferma durante il tentativo vogliamo avere informazioni sul punto in cui si e' fermato.
    /// </summary>
    public sealed class StatusAttempt
    {
        private readonly string _name;

        #region List of possible statuses

        public static readonly StatusAttempt Started = new StatusAttempt("Started");
        public static readonly StatusAttempt StartedLoadingMailInfo = new StatusAttempt("Started loading mail info");
        public static readonly StatusAttempt EndedLoadingMailInfo = new StatusAttempt("Ended loading mail info");
        public static readonly StatusAttempt StartedDuplicateCheck = new StatusAttempt("Started duplicate check");
        public static readonly StatusAttempt ConfirmedDuplicate = new StatusAttempt("Confirmed duplicate");
        public static readonly StatusAttempt EndedDuplicateCheck = new StatusAttempt("Ended duplicate check");
        public static readonly StatusAttempt NotProcessed = new StatusAttempt("Cannot archive because the status is not processed");
        public static readonly StatusAttempt StartedGettingPecMailBox = new StatusAttempt("Started getting the pec mail box");
        public static readonly StatusAttempt EndedGettingPecMailBox = new StatusAttempt("Ended getting the pec mail box");
        public static readonly StatusAttempt StartedLoadingAttachmentIndexes = new StatusAttempt("Started loading attachment indexes");
        public static readonly StatusAttempt EndedLoadingAttachmentIndexes = new StatusAttempt("Ended loading attachment indexes");
        public static readonly StatusAttempt StartedLoadingPecMail = new StatusAttempt("Started loading pec mail");
        public static readonly StatusAttempt EndedLoadingPecMail = new StatusAttempt("Ended loading pec mail");
        public static readonly StatusAttempt StartedPreemptiveSaveAttempt = new StatusAttempt("Started saving pec mail on db and on xml");
        public static readonly StatusAttempt EndedPreemptiveSaveAttempt = new StatusAttempt("Ended saving pec mail on db and on xml");
        public static readonly StatusAttempt StartedMainSaveAttempt = new StatusAttempt("Started saving pec mail in transaction");
        public static readonly StatusAttempt EndedMainSaveAttempt = new StatusAttempt("Ended saving pec mail in transaction");
        public static readonly StatusAttempt StartedGettingUsersForSmsNotifications = new StatusAttempt("Started getting ussers for sms notifications");
        public static readonly StatusAttempt EndedGettingUsersForSmsNotifications = new StatusAttempt("Ended getting ussers for sms notifications");
        public static readonly StatusAttempt StartedArchiveInConservation = new StatusAttempt("Started archive in conservation of pec");
        public static readonly StatusAttempt EndedArchiveInConservation = new StatusAttempt("Ended archive in conservation of pec");
        public static readonly StatusAttempt StartedSearchingFileSignature = new StatusAttempt("Started searching file signature");
        public static readonly StatusAttempt EndedSearchingFileSignature = new StatusAttempt("Ended searching file signature");
        public static readonly StatusAttempt StartedArchivingSignature = new StatusAttempt("Started archiving file signature");
        public static readonly StatusAttempt EndedArchivingSignature = new StatusAttempt("Ended archiving file signature");
        public static readonly StatusAttempt StartedSavingSignatureId = new StatusAttempt("Started saving signature id");
        public static readonly StatusAttempt EndedSavingSignatureId = new StatusAttempt("Ended saving signature id");
        public static readonly StatusAttempt StartedUpdatingPecMailAfterSignatureManagement = new StatusAttempt("Started updating pec mail after signature management");
        public static readonly StatusAttempt EndedUpdatingPecMailAfterSignatureManagement = new StatusAttempt("Ended updating pec mail after signature management");
        public static readonly StatusAttempt StartedSearchingDatiCertificati = new StatusAttempt("Started searching for dati certificati");
        public static readonly StatusAttempt EndedSearchingDatiCertificati = new StatusAttempt("Ended searching for dati certificati");
        public static readonly StatusAttempt ArchivingDatiCertificati = new StatusAttempt("Attempting to archive dati certificati");
        public static readonly StatusAttempt ParsingDatiCertificati = new StatusAttempt("Parsing dati certificati");
        public static readonly StatusAttempt CreatingReceiptFromDatiCertificati = new StatusAttempt("Creating receipt from dati certificati");
        public static readonly StatusAttempt SavingPecMailReceipt = new StatusAttempt("Saving pec mail receipt");
        public static readonly StatusAttempt StartedSavingIdDatiCertificati = new StatusAttempt("Started saving id dati certificati");
        public static readonly StatusAttempt EndedSavingIdDatiCertificati = new StatusAttempt("Ended saving id dati certificati");
        public static readonly StatusAttempt StartedUpdatingPecMailAfterDatiCertManagement = new StatusAttempt("Started updating pec mail after dati cert. management");
        public static readonly StatusAttempt EndedUpdatingPecMailAfterDatiCertManagement = new StatusAttempt("Ended updating pec mail after dati cert. management");
        public static readonly StatusAttempt StartedFindingPostacert = new StatusAttempt("Started finding posta cert.");
        public static readonly StatusAttempt EndedFindingPostacert = new StatusAttempt("Ended finding posta cert");
        public static readonly StatusAttempt StartedReadingPostacertContent = new StatusAttempt("Started reading posta cert. content");
        public static readonly StatusAttempt EndedReadingPostacertContent = new StatusAttempt("Ended reading posta cert. content");
        public static readonly StatusAttempt ArchivingPostacert = new StatusAttempt("Archiving posta cert");
        public static readonly StatusAttempt SavingIdPostaCert = new StatusAttempt("Saving id of posta cert archived");
        public static readonly StatusAttempt EndedSavingIdPostaCert = new StatusAttempt("Ended saving id of posta cert. archived");
        public static readonly StatusAttempt StartFindingFileSMime = new StatusAttempt("Started finding smime file");
        public static readonly StatusAttempt EndedFindingFileSMime = new StatusAttempt("Ended finding smime file");
        public static readonly StatusAttempt StartedArchivingSmime = new StatusAttempt("Started archiving Smime");
        public static readonly StatusAttempt EndedArchivingSmime = new StatusAttempt("Ended archiving Smime");
        public static readonly StatusAttempt StartedSavingSmimeId = new StatusAttempt("Started saving smime id");
        public static readonly StatusAttempt EndedSavingSmimeId = new StatusAttempt("Ended saving smime id");
        public static readonly StatusAttempt StartedFindingFilePostaCert = new StatusAttempt("Started finding file posta cert");
        public static readonly StatusAttempt EndedFindingFilePostaCert = new StatusAttempt("Ended finding file posta cert");
        public static readonly StatusAttempt StartedManagingAttachments = new StatusAttempt("Started managing attachments");
        public static readonly StatusAttempt EndedManagingAttachments = new StatusAttempt("Ended managing attachments");
        public static readonly StatusAttempt StartedFindingAttachmentId = new StatusAttempt("Started finding attachment id");
        public static readonly StatusAttempt EndedFindingAttachmentId = new StatusAttempt("Ended finding attachment id");
        public static readonly StatusAttempt StartedFindingAttachmentByIdParent  = new StatusAttempt("Started finding attachment by id parent");
        public static readonly StatusAttempt EndedFindingAttachmentByIdParent  = new StatusAttempt("Ended finding attachment by id parent");
        public static readonly StatusAttempt StartedArchivingAttachment = new StatusAttempt("Started archiving the attachment");
        public static readonly StatusAttempt EndedArchivingAttachment = new StatusAttempt("Ended archiving the attachment");
        public static readonly StatusAttempt StartedSavingIndexAfterArchiveAttachment = new StatusAttempt("Started saving index after archive attachment");
        public static readonly StatusAttempt EndedSavingIndexAfterArchiveAttachment = new StatusAttempt("Ended saving index after archive attachment");
        public static readonly StatusAttempt StartedUpdatingPecMailAfterArchivingAttachment = new StatusAttempt("Started updating pec mail after archiving attachment");
        public static readonly StatusAttempt EndedUpdatingPecMailAfterArchivingAttachment = new StatusAttempt("Ended updating pec mail after archiving attachment");
        public static readonly StatusAttempt StartedManagingPecFatturaPa = new StatusAttempt("Started managing pec fattura pa");
        public static readonly StatusAttempt StartedFindingFilePostaCertPerPecFatturaPa = new StatusAttempt("Started finding file posta cert. per pec fattura pa");
        public static readonly StatusAttempt EndedFindingFilePostaCertPerPecFatturaPa = new StatusAttempt("Ended finding file posta cert. per pec fattura pa");
        public static readonly StatusAttempt StartedExtractingMessagesFromPostaCertAttachment = new StatusAttempt("Started extracting messages from posta cert attachment");
        public static readonly StatusAttempt EndedExtractingMessagesFromPostaCertAttachment = new StatusAttempt("Ended extracting messages from posta cert attachment");
        public static readonly StatusAttempt StartedGettingOriginalPecFromAttachment = new StatusAttempt("Started getting original pec from attachmnet");
        public static readonly StatusAttempt EndedGettingOriginalPecFromAttachment = new StatusAttempt("Ended getting original pec from attachmnet");
        public static readonly StatusAttempt StartedGettingPecMailProtocol = new StatusAttempt("Started getting pec mail protocol");
        public static readonly StatusAttempt EndedGettingPecMailProtocol = new StatusAttempt("Ended getting pec mail protocol");
        public static readonly StatusAttempt StartInsertLogForCommencementNotification = new StatusAttempt("Started inserting log for commencementNotification");
        public static readonly StatusAttempt EndInsertLogForCommencementNotification = new StatusAttempt("Started inserting log for commencement notification");
        public static readonly StatusAttempt StartedInsertLogForDeliveryFailureNotification  = new StatusAttempt("Started inserting log for delivery failure notification");
        public static readonly StatusAttempt EndedInsertLogForDeliveryFailureNotification  = new StatusAttempt("Ended inserting log for delivery failure notification");
        public static readonly StatusAttempt StartedInsertLogForDeliveryReceipt = new StatusAttempt("Started inserting log for delivery receipt");
        public static readonly StatusAttempt EndedInsertLogForDeliveryReceipt = new StatusAttempt("Ended inserting log for delivery receipt");
        public static readonly StatusAttempt StartedInsertLogForInvoiceCertification = new StatusAttempt("Started inserting log for invoice certification");
        public static readonly StatusAttempt EndedInsertLogForInvoiceCertification = new StatusAttempt("Ended inserting log for invoice certification");
        public static readonly StatusAttempt StartedInsertLogForMetadataSendFileNotification = new StatusAttempt("Started inserting log for metadata send file notification");
        public static readonly StatusAttempt EndedInsertLogForMetadataSendFileNotification = new StatusAttempt("Ended inserting log for metadata send file notification");
        public static readonly StatusAttempt StartedInsertLogForResultNotification = new StatusAttempt("Started inserting log for result notification");
        public static readonly StatusAttempt EndedInsertLogForResultNotification = new StatusAttempt("Ended inserting log for result notification");
        public static readonly StatusAttempt StartedInsertLogForResultOutcomeNotification = new StatusAttempt("Started inserting log for result outcome notification");
        public static readonly StatusAttempt EndedInsertLogForResultOutcomeNotification = new StatusAttempt("Ended inserting log for result outcome notification");
        public static readonly StatusAttempt StartedInsertLogForScrapOutcomeNotification = new StatusAttempt("Started inserting log for scrap outcome notification");
        public static readonly StatusAttempt EndedInsertLogForScrapOutcomeNotification = new StatusAttempt("Ended inserting log for scrap outcome notification");
        public static readonly StatusAttempt StartedInsertLogForWasteNotification = new StatusAttempt("Started inserting log for waste notification");
        public static readonly StatusAttempt EndedInsertLogForWasteNotification = new StatusAttempt("Ended inserting log for waste notification");
        public static readonly StatusAttempt StartedInsertLogForPecMailForLinking = new StatusAttempt("Started inserting log for pec mail di avvenuto for linking with protocol");
        public static readonly StatusAttempt EndedInsertLogForPecMailForLinking = new StatusAttempt("Ended inserting log for pec mail for linking with protocol");
        public static readonly StatusAttempt StartedInsertLogForCurrentProtocolForLinking = new StatusAttempt("Started insert log for current protocol for linking with pec mail");
        public static readonly StatusAttempt EndedInsertLogForCurrentProtocolForLinking = new StatusAttempt("Ended insert log for current protocol for linking with pec mail");
        public static readonly StatusAttempt StartedUpdatingCurrentProtocol = new StatusAttempt("Started updating current protocol");
        public static readonly StatusAttempt EndedUpdatingCurrentProtocol = new StatusAttempt("Ended updating current protocol");
        public static readonly StatusAttempt StartedUpdatingCurrentOriginalPECMail = new StatusAttempt("Started updating current original pec mail");
        public static readonly StatusAttempt EndedUpdatingCurrentOriginalPECMail = new StatusAttempt("Ended updating current original pec mail");
        public static readonly StatusAttempt StartedActivatingPecMail = new StatusAttempt("Started activating pec mail");
        public static readonly StatusAttempt EndedActivatingPecMail = new StatusAttempt("Ended activating pec mail");
        public static readonly StatusAttempt StartedUpdatingPecMailAfterActivating = new StatusAttempt("Started updating pec mail after activating it");
        public static readonly StatusAttempt EndedUpdatingPecMailAfterActivating = new StatusAttempt("Ended updating pec mail after activating it");
        public static readonly StatusAttempt StartedGettingProtocolForSendReceiptEvent = new StatusAttempt("Started getting protocol for send receipt event");
        public static readonly StatusAttempt EndedGettingProtocolForSendReceiptEvent = new StatusAttempt("Ended getting protocol for send receipt event");
        public static readonly StatusAttempt StartedGettingCollaborationByProtocol = new StatusAttempt("Started getting collaboration by protocol");
        public static readonly StatusAttempt EndedGettingCollaborationByProtocol = new StatusAttempt("Ended getting collaboration by protocol");
        public static readonly StatusAttempt StartedMappingPecMailDto = new StatusAttempt("Started mapping pec mail dto");
        public static readonly StatusAttempt EndedMappingPecMailDto = new StatusAttempt("Ended mapping pec mail dto");
        public static readonly StatusAttempt StartedSendingWebApiRequest = new StatusAttempt("Started sending web api request");
        public static readonly StatusAttempt EndedSendingWebApiRequest = new StatusAttempt("Ended sending web api request");
        public static readonly StatusAttempt StartedImportedMailProcedure = new StatusAttempt("Started imported mail procedure");
        public static readonly StatusAttempt EndedImportedMailProcedure = new StatusAttempt("Ended imported mail procedure");
        public static readonly StatusAttempt StartedSavingMailInfo = new StatusAttempt("Started saving mail info");
        public static readonly StatusAttempt EndedSavingMailInfo = new StatusAttempt("Ended saving mail info");
        public static readonly StatusAttempt StartedUpdatingInvoicePecMail = new StatusAttempt("Started updating Invoice pec mail InvoiceWorkflowStarted status");
        public static readonly StatusAttempt EndedUpdatingInvoicePecMail = new StatusAttempt("Ended updating Invoice pec mail InvoiceWorkflowStarted status");

        #endregion

        public override string ToString()
        {
            return _name;
        }

        public StatusAttempt(string name)
        {
            _name = name;
        }
    }
}