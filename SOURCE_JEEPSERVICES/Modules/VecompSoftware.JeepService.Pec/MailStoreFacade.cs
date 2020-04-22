using Newtonsoft.Json;
using Org.BouncyCastle.Cms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Events.Entities.PECMails;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Data.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Data.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.EntityMapper.PECMails;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.DocSuiteWeb.Facade.NHibernate.Messages;
using VecompSoftware.DocSuiteWeb.Facade.NHibernate.PECMails;
using VecompSoftware.DocSuiteWeb.Model.DocumentGenerator;
using VecompSoftware.DocSuiteWeb.Model.Rulesets;
using VecompSoftware.Helpers;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.Helpers.LimilabsMail;
using VecompSoftware.Helpers.PEC.PA;
using VecompSoftware.Helpers.PEC.PA.Models;
using VecompSoftware.Helpers.WebAPI;
using VecompSoftware.Helpers.XML.Converters.Factory;
using VecompSoftware.JeepService.Pec.IterationTrackerFiles;
using VecompSoftware.MailManager;
using VecompSoftware.NHibernateManager;
using VecompSoftware.Services.Biblos.Models;
using VecompSoftware.Services.Command.CQRS.Events.Entities.PECMails;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.JeepService.Pec
{
    public class MailStoreFacade : IMailStoreFacade
    {
        private static FacadeFactory _factory;
        private static SMSNotificationFacade _smsFacade;
        private static PECMailBoxUserFacade _pecMailBocUserFacade;
        private IWebAPIHelper _webAPIHelper;

        public static FacadeFactory Factory
        {
            get { return _factory ?? (_factory = new FacadeFactory()); }
        }

        public static SMSNotificationFacade NotificationFacade
        {
            get { return _smsFacade ?? (_smsFacade = new SMSNotificationFacade(WindowsIdentity.GetCurrent().Name)); }
        }
        public static PECMailBoxUserFacade MailBoxUserFacade
        {
            get { return _pecMailBocUserFacade ?? (_pecMailBocUserFacade = new PECMailBoxUserFacade(WindowsIdentity.GetCurrent().Name)); }
        }

        public IWebAPIHelper WebAPIHelper
        {
            get { return _webAPIHelper ?? (_webAPIHelper = new WebAPIHelper()); }
        }

        private readonly Action<string> _sendMessage;
        private readonly MapperPECMailEntity _mapperPECMailEntity;
        private readonly MapperPECMailReceipt _mapperPECMailReceipt;

        public string Name { get; private set; }


        public MailStoreFacade(string moduleName, Action<string> sendMessage)
        {
            Name = moduleName;
            _sendMessage = sendMessage;
            _mapperPECMailEntity = new MapperPECMailEntity();
            _mapperPECMailReceipt = new MapperPECMailReceipt();
        }

        public PECMailBox GetMailBox(short idBox)
        {
            return Factory.PECMailboxFacade.GetById(idBox);
        }

        public List<PECMailBox> GetMailBoxes()
        {
            List<PECMailBox> list = new List<PECMailBox>();

            list.AddRange(
              Factory.PECMailboxFacade.GetIfIsInterop(true).Where(box => !string.IsNullOrEmpty(box.IncomingServerName))
            );

            list.AddRange(
              Factory.PECMailboxFacade.GetIfIsInterop(false).Where(box => !string.IsNullOrEmpty(box.IncomingServerName))
            );

            return list;
        }

        public IList<PECMailBox> GetIngomingMailBoxesByHost(Guid idHost, bool isDefault)
        {
            return Factory.PECMailboxFacade.GetIncomingMailBoxByIdHost(idHost, isDefault) ?? new List<PECMailBox>();
        }

        public IList<PECMailBox> GetOutgoingMailBoxesByHost(Guid idHost, bool isDefault)
        {
            return Factory.PECMailboxFacade.GetOutgoingMailBoxByIdHost(idHost, isDefault) ?? new List<PECMailBox>();
        }

        public PECMail GetMail(int idMail)
        {
            return Factory.PECMailFacade.GetById(idMail);
        }

        public bool HeaderHashExists(string hash, string recipient)
        {
            return Factory.PECMailFacade.HeaderChecksumExists(hash, recipient);
        }

        public bool ArchiveMail(string dropFolder, string mailInfoFilename, bool debugMode, bool saveSegnatura, out PECMail pecMail, out string errMsg, IterationDescriptor currentIterationInfo)
        {
            String step = string.Empty;
            errMsg = string.Empty;
            MailInfo mailInfo = null;
            pecMail = null;
            bool isInvoicePA = false;

            try
            {
                step = string.Concat("Caricamento:", mailInfoFilename);
                currentIterationInfo.Status = StatusAttempt.StartedLoadingMailInfo;
                mailInfo = MailInfo.Load(mailInfoFilename);
                currentIterationInfo.Status = StatusAttempt.EndedLoadingMailInfo;

                //E' un doppione
                currentIterationInfo.Status = StatusAttempt.StartedDuplicateCheck;
                if (HeaderHashExists(mailInfo.HeaderHash, mailInfo.MailBoxRecipient))
                {
                    currentIterationInfo.Status = StatusAttempt.ConfirmedDuplicate;
                    mailInfo.UpdateStatus(MailInfo.ProcessStatus.Archived);
                    return true;
                }
                currentIterationInfo.Status = StatusAttempt.EndedDuplicateCheck;

                //Non è ancora possibile archiviarla
                if (mailInfo.Status != MailInfo.ProcessStatus.Processed)
                {
                    currentIterationInfo.Status = StatusAttempt.NotProcessed;
                    return true;
                }


                //salva body della mail e la pecmail su db affinchè sia visibile
                currentIterationInfo.Status = StatusAttempt.StartedGettingPecMailBox;
                PECMailBox box = Factory.PECMailboxFacade.GetById(mailInfo.IDPECMailBox);
                currentIterationInfo.Status = StatusAttempt.EndedGettingPecMailBox;

                //carica indice degli allegati
                step = string.Concat("Caricamento:", mailInfo.fileIndex);
                MailIndexFile index = new MailIndexFile();
                if (File.Exists(mailInfo.fileIndex))
                {
                    currentIterationInfo.Status = StatusAttempt.StartedLoadingAttachmentIndexes;
                    index = MailIndexFile.Load(mailInfo.fileIndex);
                    currentIterationInfo.Status = StatusAttempt.EndedLoadingAttachmentIndexes;
                }


                if (mailInfo.IDPECMail > 0)
                {
                    currentIterationInfo.Status = StatusAttempt.StartedLoadingPecMail;
                    pecMail = Factory.PECMailFacade.GetById(mailInfo.IDPECMail);
                    currentIterationInfo.Status = StatusAttempt.EndedLoadingPecMail;
                }
                else
                {
                    PECMail lambdaProblemPec = null;
                    HibernateSessionHelper.TryOrRollback(() =>
                    {
                        lambdaProblemPec = new PECMail
                        {
                            Location = box.Location,
                            Attachments = new List<PECMailAttachment>(),
                            Direction = (byte)PECMailDirection.Ingoing,
                            IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Processing),
                            MailUID = mailInfo.MailUID,
                            MailBox = box,
                            MailBody = mailInfo.Body.ToString(),
                            MailSubject = mailInfo.Subject ?? box.Configuration.NoSubjectDefaultText,
                            MailSenders = mailInfo.Sender,
                            MailRecipients = mailInfo.Recipients,
                            MailRecipientsCc = mailInfo.RecipientsCc,
                            PECType = PECMailType.Anomalia,
                            MailDate = mailInfo.Date,
                            XTrasporto = mailInfo.XTrasporto,
                            MessageID = mailInfo.MessageID,
                            XRiferimentoMessageID = mailInfo.XRiferimentoMessageID,
                            MailPriority = mailInfo.Priority,
                            Checksum = string.Empty,
                            HeaderChecksum = string.Empty,
                            OriginalRecipient = mailInfo.MailBoxRecipient,
                            ReceivedAsCc = mailInfo.RecipientsCc.Contains(box.MailBoxName),
                            InvoiceStatus = InvoiceStatus.None
                        };
                        // Salvo immediatatamente il record su DB e anche su xml in modo tale da non creare nuovi record in caso di errore
                        currentIterationInfo.Status = StatusAttempt.StartedPreemptiveSaveAttempt;
                        Factory.PECMailFacade.SaveWithoutTransaction(ref lambdaProblemPec);
                        currentIterationInfo.Status = StatusAttempt.EndedPreemptiveSaveAttempt;

                        mailInfo.IDPECMail = lambdaProblemPec.Id;
                        currentIterationInfo.Status = StatusAttempt.StartedMainSaveAttempt;
                        mailInfo.Save();
                        currentIterationInfo.Status = StatusAttempt.EndedMainSaveAttempt;
                        // log di inserimento
                    }, Name, "Impossibile inserire il record in DB. Possibile errore SQL. Transazione annullata.", true);
                    pecMail = lambdaProblemPec;

                    try
                    {
                        Factory.PECMailLogFacade.Created(ref lambdaProblemPec, needTransaction: true);
                    }
                    catch (Exception ex)
                    {
                        FileLogger.Error(Name, string.Format("Errore durante la creazione del log di inserimento della PEC. La PEC ({0}) non ha subito problematiche.{1}", pecMail.Id, ex.Message), ex);
                        _sendMessage(string.Format("Errore durante la creazione del log di inserimento della PEC. La PEC ({0}) non ha subito problematiche.{0}", pecMail.Id, ex.Message));
                    }
                }

                // Salvo subito il blobbone su Biblos in modo da poterne usufruire nel caso in cui qualcosa poi andasse storto
                if (pecMail.IDMailContent == Guid.Empty)
                {
                    step = "PECMailFacade.ArchiveInConservation";
                    PECMail lambdaProblemPec = pecMail;
                    HibernateSessionHelper.TryOrRollback(() =>
                    {
                        currentIterationInfo.Status = StatusAttempt.StartedArchiveInConservation;
                        Factory.PECMailFacade.ArchiveInConservation(ref lambdaProblemPec, File.ReadAllBytes(mailInfo.EmlFilename), "corpo_del_messaggio.eml");
                        currentIterationInfo.Status = StatusAttempt.EndedArchiveInConservation;

                        // Ulteriore check di coerenza
                        if (lambdaProblemPec.IDMailContent == Guid.Empty)
                        {
                            // Significa che l'archiviazione in Conservazione non è avvuta correttamente.
                            // Blocco tutto e lancio eccezione
                            throw new Exception("Non è stato possibile memorizzazione il blob in conservazione. Verificare di avere impostato un archivio di conservazione.");
                        }
                    }, Name, "Impossibile memorizzare la PEC in conservazione. Possibile errore Biblos. Transazione annullata.", true);
                }

                //body del messaggio
                if (!string.IsNullOrEmpty(mailInfo.fileBody))
                {
                    MailAttachmentFile bodyFile = index.Find(mailInfo.fileBody);
                    if (!bodyFile.IsStored())
                    {
                        step = "ArchiveEnvelope:" + mailInfo.fileBody;
                        Factory.PECMailFacade.ArchiveEnvelope(ref pecMail, Encoding.GetEncoding(1252).GetBytes(File.ReadAllText(mailInfo.fileBody)), bodyFile.sourceName);

                        //salva idDocumento nell'indice
                        bodyFile.idStored = pecMail.IDEnvelope.ToString();
                        index.Save();
                    }
                }

                //segnatura.xml
                if (saveSegnatura && !string.IsNullOrEmpty(mailInfo.fileSegnatura))
                {
                    currentIterationInfo.Status = StatusAttempt.StartedSearchingFileSignature;
                    MailAttachmentFile segnatureFile = index.Find(mailInfo.fileSegnatura);
                    currentIterationInfo.Status = StatusAttempt.StartedSearchingFileSignature;
                    if (!segnatureFile.IsStored())
                    {
                        pecMail.Segnatura = Regex.Replace(Encoding.GetEncoding(1252).GetString(File.ReadAllBytes(mailInfo.fileSegnatura)), @"[^\u0000-\u007F]", string.Empty, RegexOptions.None);

                        step = "ArchiveSegnatura:" + mailInfo.fileSegnatura;
                        currentIterationInfo.Status = StatusAttempt.StartedArchivingSignature;
                        Factory.PECMailFacade.ArchiveSegnatura(ref pecMail, Encoding.GetEncoding(1252).GetBytes(File.ReadAllText(mailInfo.fileSegnatura)), segnatureFile.sourceName);
                        currentIterationInfo.Status = StatusAttempt.EndedArchivingSignature;

                        //salva idDocumento nell'indice
                        segnatureFile.idStored = pecMail.IDSegnatura.ToString();
                        currentIterationInfo.Status = StatusAttempt.StartedSavingSignatureId;
                        index.Save();
                        currentIterationInfo.Status = StatusAttempt.EndedSavingSignatureId;
                    }
                }

                step = "PECMailFacade.Save";
                currentIterationInfo.Status = StatusAttempt.StartedUpdatingPecMailAfterSignatureManagement;
                Factory.PECMailFacade.Update(ref pecMail);
                currentIterationInfo.Status = StatusAttempt.EndedUpdatingPecMailAfterSignatureManagement;

                PECMailReceipt receipt = null;
                //Daticert.xml
                if (!string.IsNullOrEmpty(mailInfo.fileDaticert))
                {
                    currentIterationInfo.Status = StatusAttempt.StartedSearchingDatiCertificati;
                    MailAttachmentFile daticertFile = index.Find(mailInfo.fileDaticert);
                    currentIterationInfo.Status = StatusAttempt.EndedSearchingDatiCertificati;
                    if (!daticertFile.IsStored())
                    {
                        step = "ArchiveDaticert:" + mailInfo.fileDaticert;
                        currentIterationInfo.Status = StatusAttempt.ArchivingDatiCertificati;
                        Factory.PECMailFacade.ArchiveDaticert(ref pecMail, Encoding.GetEncoding(1252).GetBytes(File.ReadAllText(mailInfo.fileDaticert)), daticertFile.sourceName);

                        step = "PECMailReceiptFacade.Save";
                        currentIterationInfo.Status = StatusAttempt.ParsingDatiCertificati;
                        XmlDocument doc = ParseDaticert(pecMail, mailInfo.fileDaticert);

                        currentIterationInfo.Status = StatusAttempt.CreatingReceiptFromDatiCertificati;
                        receipt = Factory.PECMailReceiptFacade.CreateFromDaticert(pecMail, doc);

                        currentIterationInfo.Status = StatusAttempt.SavingPecMailReceipt;
                        Factory.PECMailReceiptFacade.Save(ref receipt);

                        //salva idDocumento nell'indice
                        daticertFile.idStored = pecMail.IDDaticert.ToString();
                        currentIterationInfo.Status = StatusAttempt.StartedSavingIdDatiCertificati;
                        index.Save();
                        currentIterationInfo.Status = StatusAttempt.EndedSavingIdDatiCertificati;
                        if (pecMail.IsActive != ActiveType.Cast(ActiveType.PECMailActiveType.Active))
                        {
                            try
                            {
                                pecMail.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Active);
                                currentIterationInfo.Status = StatusAttempt.StartedUpdatingPecMailAfterDatiCertManagement;
                                Factory.PECMailFacade.UpdateOnly(ref pecMail);
                                currentIterationInfo.Status = StatusAttempt.EndedUpdatingPecMailAfterDatiCertManagement;
                            }
                            catch (Exception ex)
                            {
                                FileLogger.Error(Name, string.Format("Errore durante la attivazione della PEC inviata con stato diverso da 1 {0}IDPecMail:{1},{0}MailUID:{2}", Environment.NewLine, mailInfo.IDPECMail, mailInfo.MailUID), ex);
                                _sendMessage(string.Format("Errore durante la attivazione della PEC inviata con stato diverso da 1. La PEC è stata comunque processata correttamente.{0}Exception: {1}{0}Stacktrace: {2}", Environment.NewLine, ex.Message, ex.StackTrace));
                            }
                        }
                    }
                }

                //Postacert.eml
                if (!string.IsNullOrEmpty(mailInfo.filePostacert))
                {
                    currentIterationInfo.Status = StatusAttempt.StartedFindingPostacert;
                    MailAttachmentFile postacertFile = index.Find(mailInfo.filePostacert);
                    currentIterationInfo.Status = StatusAttempt.EndedFindingPostacert;

                    if (!postacertFile.IsStored())
                    {
                        step = "ArchivePostacert:" + mailInfo.filePostacert;
                        currentIterationInfo.Status = StatusAttempt.StartedReadingPostacertContent;
                        byte[] postacertContent = Encoding.GetEncoding(1252).GetBytes(File.ReadAllText(mailInfo.filePostacert));
                        currentIterationInfo.Status = StatusAttempt.EndedReadingPostacertContent;

                        if (DocSuiteContext.Current.ProtocolEnv.PECMainDocumentConservationDownload)
                        {
                            postacertContent = LimilabsMailHelper.RemoveAttachments(postacertContent);
                        }
                        currentIterationInfo.Status = StatusAttempt.ArchivingPostacert;
                        Factory.PECMailFacade.ArchivePostacert(ref pecMail, postacertContent, postacertFile.sourceName);

                        //salva idDocumento nell'indice
                        postacertFile.idStored = pecMail.IDPostacert.ToString();
                        currentIterationInfo.Status = StatusAttempt.SavingIdPostaCert;
                        index.Save();
                        currentIterationInfo.Status = StatusAttempt.EndedSavingIdPostaCert;
                    }
                }

                //smime.p7s 
                if (!string.IsNullOrEmpty(mailInfo.fileSMime))
                {
                    currentIterationInfo.Status = StatusAttempt.StartFindingFileSMime;
                    MailAttachmentFile smimeFile = index.Find(mailInfo.fileSMime);
                    currentIterationInfo.Status = StatusAttempt.EndedFindingFileSMime;
                    if (!smimeFile.IsStored())
                    {
                        step = "ArchiveSmime:" + mailInfo.fileSMime;
                        currentIterationInfo.Status = StatusAttempt.StartedArchivingSmime;
                        Factory.PECMailFacade.ArchiveSmime(ref pecMail, Encoding.GetEncoding(1252).GetBytes(File.ReadAllText(mailInfo.fileSMime)), smimeFile.sourceName);
                        currentIterationInfo.Status = StatusAttempt.EndedArchivingSmime;


                        //salva idDocumento nell'indice
                        smimeFile.idStored = pecMail.IDSmime.ToString();
                        currentIterationInfo.Status = StatusAttempt.StartedSavingSmimeId;
                        index.Save();
                        currentIterationInfo.Status = StatusAttempt.EndedSavingSmimeId;
                    }
                }

                //allegati
                currentIterationInfo.Status = StatusAttempt.StartedManagingAttachments;
                if (!string.IsNullOrEmpty(mailInfo.fileIndex))
                {
                    MailAttachmentFile[] attachments = index.files;

                    //se esiste un postacert.eml allora devo prendere solo gli attachment di livello superiore allo 0 (quelli inclusi nel postacert appunto)
                    if (!string.IsNullOrEmpty(mailInfo.filePostacert))
                    {
                        currentIterationInfo.Status = StatusAttempt.StartedFindingFilePostaCert;
                        MailAttachmentFile postacertFile = index.Find(mailInfo.filePostacert);
                        currentIterationInfo.Status = StatusAttempt.EndedFindingFilePostaCert;
                        if (postacertFile != null)
                        {
                            attachments = index.files.Where(p => p.idParent >= postacertFile.id && p.isBody == false).ToArray();
                        }
                    }

                    Dictionary<int, int> dict = new Dictionary<int, int>();

                    foreach (MailAttachmentFile attachment in attachments)
                    {
                        currentIterationInfo.Status = StatusAttempt.StartedFindingAttachmentId;
                        MailAttachmentFile attachmentFile = index.Find(attachment.id);
                        currentIterationInfo.Status = StatusAttempt.EndedFindingAttachmentId;
                        if (attachmentFile.IsStored())
                        {
                            continue;
                        }

                        //skip file vuoti
                        FileInfo fInfo = new FileInfo(attachment.Filename);
                        if (fInfo.Length == 0)
                        {
                            attachmentFile.idStored = Guid.Empty.ToString();
                            continue;
                        }

                        step = "FindAttachment:" + attachment.Filename;
                        PECMailAttachment parent = null;
                        if (attachment.idParent > 0 && dict.ContainsKey(attachment.idParent))
                        {
                            currentIterationInfo.Status = StatusAttempt.StartedFindingAttachmentByIdParent;
                            parent = Factory.PECMailAttachmentFacade.GetById(dict[attachment.idParent]);
                            currentIterationInfo.Status = StatusAttempt.EndedFindingAttachmentByIdParent;
                        }

                        //archivia allegato
                        step = "ArchiveAttachment:" + attachment.Filename;
                        FileDocumentInfo attachmentFileDocumentInfo = new FileDocumentInfo(attachment.sourceName, new FileInfo(attachment.Filename));
                        currentIterationInfo.Status = StatusAttempt.StartedArchivingAttachment;
                        PECMailAttachment res = Factory.PECMailFacade.ArchiveAttachment(ref pecMail, attachmentFileDocumentInfo, attachment.sourceName, false, parent);
                        currentIterationInfo.Status = StatusAttempt.EndedArchivingAttachment;
                        if (res != null)
                        {
                            dict.Add(attachment.id, res.Id);

                            //salva indice
                            attachmentFile.idStored = res.IDDocument.ToString();
                            currentIterationInfo.Status = StatusAttempt.StartedSavingIndexAfterArchiveAttachment;
                            index.Save();
                            currentIterationInfo.Status = StatusAttempt.EndedSavingIndexAfterArchiveAttachment;
                        }

                        currentIterationInfo.Status = StatusAttempt.StartedUpdatingPecMailAfterArchivingAttachment;
                        Factory.PECMailFacade.UpdateNoLastChange(ref pecMail);
                        currentIterationInfo.Status = StatusAttempt.EndedUpdatingPecMailAfterArchivingAttachment;
                    }
                }
                currentIterationInfo.Status = StatusAttempt.EndedManagingAttachments;


                step = "PEC Fattura PA";
                try
                {
                    currentIterationInfo.Status = StatusAttempt.StartedManagingPecFatturaPa;

                    if (!string.IsNullOrEmpty(mailInfo.filePostacert))
                    {
                        currentIterationInfo.Status = StatusAttempt.StartedFindingFilePostaCertPerPecFatturaPa;
                        MailAttachmentFile postacertFile = index.Find(mailInfo.filePostacert);
                        currentIterationInfo.Status = StatusAttempt.EndedFindingFilePostaCertPerPecFatturaPa;

                        if (postacertFile != null)
                        {
                            FileLogger.Debug(Name, string.Concat("Fattura PA: postacertFile != null -> ", postacertFile != null));
                            ICollection<MailAttachmentFile> postaCertAttachments = index.files.Where(p => p.idParent >= postacertFile.id && FileHelper.MatchExtension(p.Filename, FileHelper.XML)).ToList();

                            SDIMessage sdiMessage = new SDIMessage();
                            object sdiMessageObj;
                            currentIterationInfo.Status = StatusAttempt.StartedExtractingMessagesFromPostaCertAttachment;
                            foreach (MailAttachmentFile postaCertAttachment in postaCertAttachments)
                            {
                                FileLogger.Debug(Name, string.Concat("Fattura PA: Looking sdiMessage -> ", postaCertAttachment.Filename));
                                using (MemoryStream stream = new MemoryStream(File.ReadAllBytes(postaCertAttachment.Filename)))
                                {
                                    sdiMessage = PAMessageHelper.GetSIDMessage(stream, out sdiMessageObj);
                                    if (sdiMessage.Status != MessageStatus.None)
                                    {
                                        break;
                                    }
                                }
                            }

                            currentIterationInfo.Status = StatusAttempt.EndedExtractingMessagesFromPostaCertAttachment;
                            FileLogger.Debug(Name, string.Concat("Fattura PA: sdiMessage status -> ", sdiMessage.Status));

                            if (sdiMessage.Status != MessageStatus.None)
                            {
                                InvoiceStatus invoiceStatus = InvoiceStatus.None;
                                int? protocolIdStatus = null;

                                switch (sdiMessage.Status)
                                {
                                    case MessageStatus.PAInvoiceSent:
                                        {
                                            protocolIdStatus = (int)ProtocolStatusId.PAInvoiceSent;
                                            invoiceStatus = InvoiceStatus.PAInvoiceSent;

                                            break;
                                        }
                                    case MessageStatus.PAInvoiceAccepted:
                                        {
                                            protocolIdStatus = (int)ProtocolStatusId.PAInvoiceAccepted;
                                            invoiceStatus = InvoiceStatus.PAInvoiceAccepted;

                                            break;
                                        }
                                    case MessageStatus.PAInvoiceNotified:
                                    case MessageStatus.PAInvoiceReceipt:
                                    case MessageStatus.PAInvoiceFailedDelivery:
                                        {
                                            protocolIdStatus = (int)ProtocolStatusId.PAInvoiceNotified;
                                            invoiceStatus = InvoiceStatus.PAInvoiceNotified;

                                            break;
                                        }
                                    case MessageStatus.PAInvoiceRefused:
                                        {
                                            protocolIdStatus = (int)ProtocolStatusId.PAInvoiceRefused;
                                            invoiceStatus = InvoiceStatus.PAInvoiceRefused;

                                            break;
                                        }
                                    case MessageStatus.PAInvoiceSdiRefused:
                                        {
                                            protocolIdStatus = (int)ProtocolStatusId.PAInvoiceSdiRefused;
                                            invoiceStatus = InvoiceStatus.PAInvoiceSdiRefused;

                                            break;
                                        }
                                    default:
                                        {
                                            break;
                                        }
                                }
                                FileLogger.Debug(Name, $"Invoice PA: SDIIdentification {sdiMessage.SDIIdentification} with InvoiceStatus {invoiceStatus} and ProtocolIdStatus {protocolIdStatus}");
                                pecMail.InvoiceStatus = invoiceStatus;

                                if (!string.IsNullOrEmpty(sdiMessage.LogDescription))
                                {
                                    currentIterationInfo.Status = StatusAttempt.StartedInsertLogForResultNotification;
                                    Factory.PECMailLogFacade.InsertLog(ref pecMail, sdiMessage.LogDescription, PECMailLogType.Warning);
                                    currentIterationInfo.Status = StatusAttempt.EndedInsertLogForResultNotification;
                                }
                                isInvoicePA = sdiMessage.Status != MessageStatus.None;
                                FileLogger.Debug(Name, $"Fattura PA: IsInvoicePA -> {isInvoicePA}");

                                FileLogger.Debug(Name, string.Concat("Fattura PA: PAMessageType -> ", sdiMessage.MessageType));
                                currentIterationInfo.Status = StatusAttempt.StartedGettingOriginalPecFromAttachment;
                                FileLogger.Debug(Name, string.Concat("Fattura PA: Looking sended PECMail with SDI PECMessageId -> '", sdiMessage.ReferenceToPECMessageId, "'"));
                                PECMail originalPecMail = Factory.PECMailFacade.GetOriginalPECFromReferenceToSDIIdentification(sdiMessage.ReferenceToPECMessageId);
                                if (originalPecMail == null)
                                {
                                    FileLogger.Debug(Name, string.Concat("Fattura PA: Looking sended PECMail with InvoiceFilename -> '", sdiMessage.InvoiceFilename, "'"));
                                    originalPecMail = Factory.PECMailFacade.GetOriginalPECFromPAAttachmentFileName(sdiMessage.InvoiceFilename);
                                }
                                currentIterationInfo.Status = StatusAttempt.EndedGettingOriginalPecFromAttachment;
                                FileLogger.Debug(Name, string.Concat("Fattura PA: sdiMessage PECMessageId -> '", sdiMessage.ReferenceToPECMessageId, "'"));
                                FileLogger.Debug(Name, string.Concat("Fattura PA: sdiMessage InvoiceFilename -> '", sdiMessage.InvoiceFilename, "'"));
                                FileLogger.Debug(Name, string.Concat("Fattura PA: originalPecMail has been found? -> ", originalPecMail != null));
                                FileLogger.Debug(Name, string.Concat("Fattura PA: pecMail XRiferimentoMessageID -> ", pecMail.XRiferimentoMessageID));
                                FileLogger.Debug(Name, string.Concat("Fattura PA: originalPecMail XRiferimentoMessageID -> ", originalPecMail?.XRiferimentoMessageID));

                                Protocol currentProtocol = null;
                                if (originalPecMail != null)
                                {
                                    FileLogger.Debug(Name, string.Concat("Fattura PA: Looking Protocol By PECMailId -> ", originalPecMail.Id));
                                    currentIterationInfo.Status = StatusAttempt.StartedGettingPecMailProtocol;
                                    currentProtocol = Factory.PECMailFacade.GetProtocol(originalPecMail);
                                    currentIterationInfo.Status = StatusAttempt.EndedGettingPecMailProtocol;
                                    FileLogger.Debug(Name, string.Concat("Fattura PA: currentProtocol has been found? -> ", currentProtocol != null));
                                    FileLogger.Debug(Name, string.Concat("Fattura PA: Protocol Identifications -> ", currentProtocol?.Year, "/", currentProtocol?.Number, "/", currentProtocol?.UniqueId));
                                }

                                if (currentProtocol != null)
                                {
                                    if (currentProtocol.IdStatus.Value != (int)ProtocolStatusId.PAInvoiceAccepted && protocolIdStatus.HasValue)
                                    {
                                        currentProtocol.IdStatus = protocolIdStatus;
                                    }
                                    
                                    currentIterationInfo.Status = StatusAttempt.EndedUpdatingCurrentProtocol;
                                    currentProtocol.AdvanceProtocol.IdentificationSDI = currentProtocol.AdvanceProtocol.IdentificationSDI != sdiMessage.SDIIdentification ? sdiMessage.SDIIdentification : currentProtocol.AdvanceProtocol.IdentificationSDI;

                                    currentIterationInfo.Status = StatusAttempt.StartedUpdatingCurrentProtocol;
                                    Factory.ProtocolFacade.UpdateOnly(ref currentProtocol);

                                    FileLogger.Debug(Name, $"Fattura PA: Linking PECMail {pecMail.Id} with Protocol {currentProtocol.Id.Year}/{currentProtocol.Id.Number}");
                                    pecMail.Year = currentProtocol.Year;
                                    pecMail.Number = currentProtocol.Number;
                                    pecMail.DocumentUnitType = DSWEnvironment.Protocol;
                                    pecMail.RecordedInDocSuite = 1;
                                    if (DocSuiteContext.Current.ProtocolEnv.IsLogEnabled)
                                    {
                                        currentIterationInfo.Status = StatusAttempt.StartedInsertLogForPecMailForLinking;
                                        Factory.PECMailLogFacade.InsertLog(ref pecMail, string.Format("Pec collegata al protocollo {0}", currentProtocol.Id), PECMailLogType.Linked);
                                        currentIterationInfo.Status = StatusAttempt.EndedInsertLogForPecMailForLinking;

                                        currentIterationInfo.Status = StatusAttempt.StartedInsertLogForCurrentProtocolForLinking;
                                        Factory.ProtocolLogFacade.Insert(ref currentProtocol, ProtocolLogEvent.PM, string.Format("Collegata PEC n.{0} del {1} con oggetto '{2}'", pecMail.Id, pecMail.RegistrationDate.Date.ToShortDateString(), pecMail.MailSubject));
                                        currentIterationInfo.Status = StatusAttempt.EndedInsertLogForCurrentProtocolForLinking;

                                        FileLogger.Debug(Name, string.Format("Fattura PA: Pec n.{0} collegata al protocollo {1}", pecMail.Id, currentProtocol.Id));
                                    }
                                }

                                if (originalPecMail != null && originalPecMail.InvoiceStatus.HasValue && originalPecMail.InvoiceStatus.Value != InvoiceStatus.PAInvoiceAccepted)
                                {
                                    originalPecMail.InvoiceStatus = invoiceStatus;
                                    currentIterationInfo.Status = StatusAttempt.StartedUpdatingCurrentOriginalPECMail;
                                    Factory.PECMailFacade.UpdateOnly(ref originalPecMail);
                                    currentIterationInfo.Status = StatusAttempt.EndedUpdatingCurrentOriginalPECMail;
                                }
                            }
                        }
                    }

                }
                catch (Exception ex_pa)
                {
                    FileLogger.Error(Name, string.Format("Errore durante la ricerca del protocollo per la PA. La PEC è stata comunque processata correttamente {0}IDPecMail:{1},{0}MailUID:{2}", Environment.NewLine, mailInfo.IDPECMail, mailInfo.MailUID), ex_pa);
                    _sendMessage(string.Format("Errore durante la ricerca del protocollo per la PA. La PEC è stata comunque processata correttamente.{0}Exception: {1}{0}Stacktrace: {2}", Environment.NewLine, ex_pa.Message, ex_pa.StackTrace));
                }

                if (!string.IsNullOrEmpty(box.RulesetDefinition))
                {
                    PECMailBoxRuleset ruleset = JsonConvert.DeserializeObject<PECMailBoxRuleset>(box.RulesetDefinition);

                    bool pecMailRulesetCondition = false;
                    PECMailBox boxDestination = null;

                    if (ruleset.Rule == RulesetType.PECMailBoxMoveTo && pecMail.MailSenders.Equals(ruleset.Condition, StringComparison.InvariantCultureIgnoreCase))
                    {
                        boxDestination = JsonConvert.DeserializeObject<PECMailBox>(ruleset.Reference);
                        pecMailRulesetCondition = true;
                    }

                    if (pecMailRulesetCondition && (boxDestination = Factory.PECMailboxFacade.GetById(boxDestination.Id)) != null)
                    {
                        pecMail.MailBox = boxDestination;
                        Factory.PECMailboxFacade.Update(ref boxDestination);

                        Factory.PECMailLogFacade.InsertLog(ref pecMail, string.Format("PEC spostata automaticamente dalla casella {0} in quanto verificata la regola automatica di spostamento '{1}'", boxDestination.MailBoxName, ruleset.Name), PECMailLogType.RulsetActivated);
                    }
                }

                //activate
                step = "ActivatePec";
                currentIterationInfo.Status = StatusAttempt.StartedActivatingPecMail;
                Factory.PECMailFacade.ActivatePec(pecMail);
                currentIterationInfo.Status = StatusAttempt.EndedActivatingPecMail;

                //ok terminato
                pecMail.Checksum = mailInfo.EmlHash;
                pecMail.HeaderChecksum = mailInfo.HeaderHash;
                pecMail.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Active);
                
                currentIterationInfo.Status = StatusAttempt.StartedUpdatingPecMailAfterActivating;
                Factory.PECMailFacade.Update(ref pecMail);
                currentIterationInfo.Status = StatusAttempt.EndedUpdatingPecMailAfterActivating;

                if (pecMail.PECType == PECMailType.Receipt && receipt != null && (receipt.ReceiptType.Eq(PECMailTypes.ErroreConsegna) || receipt.ReceiptType.Eq(PECMailTypes.PreavvisoErroreConsegna)))
                {
                    try
                    {
                        SetPECMailTaskError(pecMail);
                    }
                    catch (Exception ex)
                    {
                        FileLogger.Error(Name, string.Format("Errore durante la modifica dello stato attività relativa alla ricevuta con IdPECMail: {0}. La PEC è stata comunque processata correttamente.", mailInfo.IDPECMail), ex);
                        _sendMessage(string.Format("Errore durante la modifica dello stato attività relativa alla ricevuta con IdPECMail: {0}. La PEC è stata comunque processata correttamente.{1}Exception: {2}{1}Stacktrace: {3}", mailInfo.IDPECMail, Environment.NewLine, ex.Message, ex.StackTrace));
                    }                    
                }

                #region send receipt event
                if (pecMail.PECType == PECMailType.Receipt && receipt != null)
                {
                    try
                    {
                        FileLogger.Info(Name, string.Format("PEC {0} di tipo ricevuta. Gestisco l'invio dell'evento EventReceivedReceiptPECMail", pecMail.Id));
                        PECMail originalPECMail = receipt.PECMail;
                        if (originalPECMail == null)
                        {
                            throw new ArgumentNullException(nameof(originalPECMail), string.Format("Nessuna PEC trovata con XRiferimentoMessageID {0}", pecMail.XRiferimentoMessageID));
                        }
                        isInvoicePA = isInvoicePA || (originalPECMail.InvoiceStatus.HasValue && originalPECMail.InvoiceStatus.Value != InvoiceStatus.None) ||
                            (pecMail.MailSenders.StartsWith("sdi", StringComparison.InvariantCultureIgnoreCase) && pecMail.MailSenders.Contains("fatturapa.it"));
                        
                        FileLogger.Debug(Name, string.Format("PEC {0}, trovato messaggio di invio con IdPECMail {1}", pecMail.Id, originalPECMail.Id));

                        EventHelper eventParameters = GetEventParameters(currentIterationInfo, null, pecMail, receipt);

                        if (originalPECMail.DocumentUnitType == DSWEnvironment.Protocol && originalPECMail.Year.HasValue && originalPECMail.Number.HasValue)
                        {
                            eventParameters = GetEventParameters(currentIterationInfo, originalPECMail, pecMail, receipt);
                        }

                        IEventReceivedReceiptPECMail @event = new EventReceivedReceiptPECMail(DocSuiteContext.Current.CurrentTenant.TenantName, DocSuiteContext.Current.CurrentTenant.TenantId,
                            eventParameters.CollaborationUniqueId, eventParameters.CollaborationId, eventParameters.CollaborationTemplateName, eventParameters.ProtocolUniqueId,
                            eventParameters.ProtocolYear, eventParameters.ProtocolNumber, isInvoicePA, eventParameters.Identity, eventParameters.PECMail, eventParameters.PECMailReceipt, null);

                        FileLogger.Info(Name, string.Format("PEC {0}, spedizione dell'evento alle WebAPI con ID {1}", pecMail.Id, @event.Id));
                        currentIterationInfo.Status = StatusAttempt.StartedSendingWebApiRequest;
                        bool sended = WebAPIHelper.SendRequest(DocSuiteContext.Current.CurrentTenant.WebApiClientConfig, DocSuiteContext.Current.CurrentTenant.WebApiClientConfig, @event, string.Empty);
                        currentIterationInfo.Status = StatusAttempt.EndedSendingWebApiRequest;
                        if (!sended)
                        {
                            FileLogger.Warn(Name, string.Format("PEC {0}, la fase di invio dell'evento alle Web API non è avvenuta correttamente. Vedere log specifico per maggiori dettagli", pecMail.Id));
                            _sendMessage(string.Concat("E' avvenuto un errore durante la fase di invio dell'evento EventReceivedReceiptPECMail alle WebAPI per la ricevuta PEC ", pecMail.Id, ". La PEC è stata comunque processata correttamente"));
                        }
                        else
                        {
                            FileLogger.Info(Name, string.Format("PEC {0}, spedizione dell'evento alle WebAPI con ID {1} avvenuto correttamente", pecMail.Id, @event.Id));
                            if (isInvoicePA)
                            {
                                pecMail.InvoiceStatus = InvoiceStatus.InvoiceWorkflowStarted;
                                currentIterationInfo.Status = StatusAttempt.StartedUpdatingPecMailAfterActivating;
                                Factory.PECMailFacade.Update(ref pecMail);
                                currentIterationInfo.Status = StatusAttempt.EndedUpdatingPecMailAfterActivating;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        FileLogger.Error(Name, string.Format("Errore durante l'invio dell'evento di gestione della ricevuta con IdPECMail: {0}. La PEC è stata comunque processata correttamente.", mailInfo.IDPECMail), ex);
                        _sendMessage(string.Format("Errore durante l'invio dell'evento di gestione della ricevuta con IdPECMail: {0}. La PEC è stata comunque processata correttamente.{1}Exception: {2}{1}Stacktrace: {3}", mailInfo.IDPECMail, Environment.NewLine, ex.Message, ex.StackTrace));
                    }
                }
                #endregion

                #region send create event
                if (pecMail.PECType == PECMailType.PEC)
                {
                    try
                    {
                        FileLogger.Info(Name, string.Format("PEC {0} di tipo creato. Gestisco l'invio dell'evento EventCreatePECMail", pecMail.Id));
                        EventHelper eventParameters = GetEventParameters(currentIterationInfo, null, pecMail, receipt);
                        MailAttachmentFile postacertFile = index.Find(mailInfo.filePostacert);

                        try
                        {
                            if (postacertFile != null)
                            {
                                isInvoicePA |= CheckIsInvoicePA(index.files.Where(p => p.idParent >= postacertFile.id && FileHelper.MatchExtension(p.Filename, FileHelper.XML)));
                                if (!isInvoicePA)
                                {
                                    isInvoicePA |= CheckIsInvoicePA(index.files.Where(p => p.idParent >= postacertFile.id && FileHelper.MatchExtension(p.Filename, FileHelper.P7M)));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            FileLogger.Error(Name, string.Format("Errore durante la lettura del file xml della creazione con IdPECMail: {0}. La PEC è stata comunque processata correttamente.", mailInfo.IDPECMail), ex);
                            _sendMessage(string.Format("Errore durante la lettura del file xml della creazione con IdPECMail: {0}. La PEC è stata comunque processata correttamente.{1}Exception: {2}{1}Stacktrace: {3}", mailInfo.IDPECMail, Environment.NewLine, ex.Message, ex.StackTrace));
                        }

                        isInvoicePA = isInvoicePA || (pecMail.MailSenders.StartsWith("sdi", StringComparison.InvariantCultureIgnoreCase) && pecMail.MailSenders.Contains("fatturapa.it"));

                        IEventCreatePECMail @event = new EventCreatePECMail(DocSuiteContext.Current.CurrentTenant.TenantName, DocSuiteContext.Current.CurrentTenant.TenantId,
                            eventParameters.CollaborationUniqueId, eventParameters.CollaborationId, eventParameters.CollaborationTemplateName, eventParameters.ProtocolUniqueId,
                            eventParameters.ProtocolYear, eventParameters.ProtocolNumber, isInvoicePA, eventParameters.Identity, eventParameters.PECMail, null);

                        FileLogger.Info(Name, string.Format("PEC {0}, spedizione dell'evento alle WebAPI con ID {1}", pecMail.Id, @event.Id));
                        currentIterationInfo.Status = StatusAttempt.StartedSendingWebApiRequest;
                        bool sended = WebAPIHelper.SendRequest(DocSuiteContext.Current.CurrentTenant.WebApiClientConfig, DocSuiteContext.Current.CurrentTenant.WebApiClientConfig, @event, string.Empty);
                        currentIterationInfo.Status = StatusAttempt.EndedSendingWebApiRequest;
                        if (!sended)
                        {
                            FileLogger.Warn(Name, string.Format("PEC {0}, la fase di invio dell'evento alle Web API non è avvenuta correttamente. Vedere log specifico per maggiori dettagli", pecMail.Id));
                            _sendMessage(string.Concat("E' avvenuto un errore durante la fase di invio dell'evento EventCreatePECMail alle WebAPI per la ricevuta PEC ", pecMail.Id, ". La PEC è stata comunque processata correttamente"));
                        }
                        else
                        {
                            FileLogger.Info(Name, string.Format("PEC {0}, spedizione dell'evento alle WebAPI con ID {1} avvenuto correttamente", pecMail.Id, @event.Id));
                        }
                    }
                    catch (Exception ex)
                    {
                        FileLogger.Error(Name, string.Format("Errore durante l'invio dell'evento di gestione della creazione con IdPECMail: {0}. La PEC è stata comunque processata correttamente.", mailInfo.IDPECMail), ex);
                        _sendMessage(string.Format("Errore durante l'invio dell'evento di gestione della creazione con IdPECMail: {0}. La PEC è stata comunque processata correttamente.{1}Exception: {2}{1}Stacktrace: {3}", mailInfo.IDPECMail, Environment.NewLine, ex.Message, ex.StackTrace));
                    }
                }
                #endregion

                if (pecMail.PECType != PECMailType.Receipt)
                {
                    try
                    {
                        currentIterationInfo.Status = StatusAttempt.StartedGettingUsersForSmsNotifications;
                        ICollection<PECMailBoxUser> results = MailBoxUserFacade.GetUsers(box.Id);
                        currentIterationInfo.Status = StatusAttempt.EndedGettingUsersForSmsNotifications;
                        if (results != null)
                        {
                            SMSNotification notification;
                            foreach (PECMailBoxUser user in results)
                            {
                                if (!NotificationFacade.ExistPecNotificationForAccountName(pecMail.Id, user.AccountName))
                                {
                                    notification = new SMSNotification(WindowsIdentity.GetCurrent().Name)
                                    {
                                        AccountName = user.AccountName,
                                        LogicalState = LogicalStateType.Active,
                                        NotificationType = SMSNotificationType.PEC,
                                        PECMail = pecMail
                                    };
                                    NotificationFacade.Save(ref notification);
                                    FileLogger.Info(Name, string.Format("Notifica SMS per la PEC ({0}) attivata per l'utente {1}", pecMail.Id, user.AccountName));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        FileLogger.Error(Name, string.Format("Errore durante la creazione delle notifice SMS. La PEC ({0}) non ha subito problematiche.{1}", pecMail.Id, ex.Message), ex);
                        _sendMessage(string.Format("Errore durante la creazione delle notifice SMS. La PEC ({0}) non ha subito problematiche.{0}", pecMail.Id, ex.Message));
                    }

                }
                //Log su casella di importazione effettivamente andata a buon fine
                step = "PECMailboxLogFacade.ImportedMail";
                currentIterationInfo.Status = StatusAttempt.StartedImportedMailProcedure;
                Factory.PECMailboxLogFacade.ImportedMail(ref pecMail);
                currentIterationInfo.Status = StatusAttempt.EndedImportedMailProcedure;

                mailInfo.UpdateStatus(MailInfo.ProcessStatus.Archived);
                currentIterationInfo.Status = StatusAttempt.StartedSavingMailInfo;
                mailInfo.Save();
                currentIterationInfo.Status = StatusAttempt.EndedSavingMailInfo;
                return true;
            }
            catch (Exception ex)
            {
                currentIterationInfo.Message.AppendLine(ex.Message);

                errMsg = string.Format("Errore in ArchiveMail:'{1}'{0}{0}Step:{2},{0}IDPecMail:{3},{0}MailUID:{4},{0}{0}Exception:{5}", Environment.NewLine, mailInfo.EmlFilename, step, mailInfo.IDPECMail, mailInfo.MailUID, ex.Message);
                if (mailInfo != null)
                {
                    mailInfo.AddError(errMsg, ex);
                    mailInfo.Save();
                }

                // Provo a salvare su SQL l'errore ottenuto
                // ma potrebbe essere SQL il problema
                try
                {
                    if (pecMail != null && mailInfo.IDPECMail > 0)
                    {
                        Factory.PECMailLogFacade.Warning(ref pecMail, errMsg);
                    }
                }
                catch (Exception sqlException)
                {
                    if (mailInfo != null)
                    {
                        mailInfo.AddError("Errore in fase di memorizzazione errore su SQL", sqlException);
                        mailInfo.Save();
                        NHibernateSessionManager.Instance.CloseTransactionAndSessions();
                    }
                    return false;
                }

                return false;
            }
        }

        private bool CheckIsInvoicePA(IEnumerable<MailAttachmentFile> results)
        {
            foreach (MailAttachmentFile file in results)
            {
                string pathFileSource = File.ReadAllText(file.Filename);
                if (!string.IsNullOrEmpty(pathFileSource))
                {
                    if (IsXmlValid(pathFileSource))
                    {
                        XmlFactory xmlFactory = new XmlFactory();
                        XMLConverterModel xmlConvertedModel = xmlFactory.BuildXmlModel(pathFileSource);

                        if (xmlConvertedModel.ModelKind == XMLModelKind.InvoicePA_V10 || xmlConvertedModel.ModelKind == XMLModelKind.InvoicePA_V11 ||
                            xmlConvertedModel.ModelKind == XMLModelKind.InvoicePA_V12 || xmlConvertedModel.ModelKind == XMLModelKind.InvoicePR_V12)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private string GetSignedContent(string data)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(data);

                CmsSignedData sig = new CmsSignedData(bytes);
                bytes = sig.SignedContent.GetContent() as byte[];

                return Encoding.Default.GetString(bytes);
            }
            catch (Exception){}
            return string.Empty;
        }

        private XmlDocument ParseDaticert(PECMail pecMail, string filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            XmlNode xmlnode = doc.GetElementsByTagName("postacert").Item(0);
            if (xmlnode != null)
            {
                if (xmlnode.Attributes != null)
                {
                    pecMail.MailType = xmlnode.Attributes["tipo"].Value;
                    pecMail.MailError = xmlnode.Attributes["errore"].Value;
                }
                pecMail.PECType = xmlnode.Attributes != null && xmlnode.Attributes["tipo"].Value.ToLower() == "posta-certificata" ? PECMailType.PEC : PECMailType.Receipt;
            }
            return doc;
        }

        private byte? GetMailStatusEnum(string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return null;
            }

            switch (status)
            {
                case "accettazione":
                    {
                        return (byte)PECMessageStatus.Accettazione;
                    }
                // Le ricevute di avvenuta consegna sono costituite da un messaggio di posta elettronica inviato al mittente che riporta la data e l’ora di avvenuta consegna
                case "avvenuta-consegna":
                    {
                        return (byte)PECMessageStatus.AvvenutaConsegna;
                    }
                // Al momento dell’accettazione del messaggio il punto di accesso deve garantirne la correttezza formale. 
                // Qualora il messaggio non superi i controlli, il punto di accesso non dovrà accettare il messaggio...
                case "non-accettazione":
                    {
                        return (byte)PECMessageStatus.NonAccettazione;
                    }
                // Qualora il gestore del mittente non abbia ricevuto dal gestore del destinatario, nelle dodici ore successive all’inoltro del messaggio, 
                // la ricevuta di presa in carico o di avvenuta consegna del messaggio inviato, comunica al mittente che il gestore del destinatario potrebbe non essere in grado di effettuare la consegna del messaggio. 
                // Tale comunicazione è effettuata mediante un avviso di mancata consegna per superamento dei tempi massimi 
                case "preavviso-errore-consegna":
                    {
                        return (byte)PECMessageStatus.PreavvisoErroreConsegna;
                    }
                // Nel caso si verifichi un errore nella fase di consegna del messaggio, il sistema genera un avviso di mancata consegna da restituire al mittente con l’indicazione dell’errore riscontrato.
                case "errore-consegna":
                    {
                        return (byte)PECMessageStatus.ErroreConsegna;
                    }
                // Non arriva mai al client, inviata tra i servers gestore mittente e gestore ricevente
                case "presa-in-carico":
                    {
                        return (byte)PECMessageStatus.PresaInCarico;
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        private static bool IsXmlValid(string xml)
        {
            try
            {
                XDocument.Parse(xml);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private EventHelper GetEventParameters(IterationDescriptor currentIterationInfo, PECMail originalPECMail, PECMail pecMail, PECMailReceipt pecMailReceipt)
        {
            EventHelper eventHelper = new EventHelper();

            currentIterationInfo.Status = StatusAttempt.StartedGettingProtocolForSendReceiptEvent;
            Protocol protocol = null;
            if (originalPECMail != null)
            {
                protocol = Factory.ProtocolFacade.GetById(originalPECMail.Year.Value, originalPECMail.Number.Value);
            }
            currentIterationInfo.Status = StatusAttempt.EndedGettingProtocolForSendReceiptEvent;

            if (protocol != null)
            {
                FileLogger.Debug(Name, string.Format("PEC {0}, trovato Protocollo associato al messaggio di invio {1} - {2:0000}/{3:0000000}", pecMail.Id, originalPECMail.Id, protocol.Year, protocol.Number));
                eventHelper.ProtocolUniqueId = protocol.UniqueId;
                eventHelper.ProtocolYear = protocol.Year;
                eventHelper.ProtocolNumber = protocol.Number;

                currentIterationInfo.Status = StatusAttempt.StartedGettingCollaborationByProtocol;
                Collaboration collaboration = Factory.CollaborationFacade.GetByProtocol(protocol);
                currentIterationInfo.Status = StatusAttempt.EndedGettingCollaborationByProtocol;

                if (collaboration != null)
                {
                    FileLogger.Debug(Name, string.Format("PEC {0}, trovata Collaborazione {1} associata al Protocollo {2:0000}/{3:0000000}", pecMail.Id, collaboration.Id, protocol.Year, protocol.Number));
                    eventHelper.CollaborationUniqueId = collaboration.UniqueId;
                    eventHelper.CollaborationId = collaboration.Id;
                    eventHelper.CollaborationTemplateName = collaboration.TemplateName;
                }
            }

            eventHelper.Identity = new IdentityContext(DocSuiteContext.Current.User.FullUserName);
            currentIterationInfo.Status = StatusAttempt.StartedMappingPecMailDto;
            eventHelper.PECMail = _mapperPECMailEntity.MappingDTO(pecMail);

            if (pecMailReceipt != null)
            {
                eventHelper.PECMailReceipt = _mapperPECMailReceipt.MappingDTO(pecMailReceipt);
            }
            currentIterationInfo.Status = StatusAttempt.EndedMappingPecMailDto;

            return eventHelper;
        }

        public static void SetPECMailTaskError(PECMail pecMail)
        {
            TaskHeader taskHeader = FacadeFactory.Instance.TaskHeaderFacade.GetByPEC(pecMail);
            if (taskHeader != null)
            {
                taskHeader.SendedStatus = TaskHeaderSendedStatus.Errors;
                FacadeFactory.Instance.TaskHeaderFacade.Update(ref taskHeader);
            }
        }

        public static void CompletePECMailTask(PECMail pecMail)
        {
            TaskHeader taskHeader = FacadeFactory.Instance.TaskHeaderFacade.GetByPEC(pecMail);
            if (taskHeader != null)
            {
                FacadeFactory.Instance.TaskHeaderFacade.CompleteTaskProcess(taskHeader);
            }
        }
    }
}
