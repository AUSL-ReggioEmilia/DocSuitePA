using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.PECMails;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.PECMails
{
    public class PECMailValidatorMapper : BaseMapper<PECMail, PECMailValidator>, IPECMailValidatorMapper
    {
        public PECMailValidatorMapper() { }

        public override PECMailValidator Map(PECMail entity, PECMailValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.EntityId = entity.EntityId;
            entityTransformed.Direction = entity.Direction;
            entityTransformed.Year = entity.Year;
            entityTransformed.Number = entity.Number;
            entityTransformed.MailUID = entity.MailUID;
            entityTransformed.MailContent = entity.MailContent;
            entityTransformed.MailSenders = entity.MailSenders;
            entityTransformed.MailSubject = entity.MailSubject;
            entityTransformed.MailRecipients = entity.MailRecipients;
            entityTransformed.MailDate = entity.MailDate;
            entityTransformed.MailType = entity.MailType;
            entityTransformed.MailError = entity.MailError;
            entityTransformed.MailPriority = entity.MailPriority;
            entityTransformed.XTrasporto = entity.XTrasporto;
            entityTransformed.MessageID = entity.MessageID;
            entityTransformed.XRiferimentoMessageID = entity.XRiferimentoMessageID;
            entityTransformed.Segnatura = entity.Segnatura;
            entityTransformed.MessaggioRitornoName = entity.MessaggioRitornoName;
            entityTransformed.MessaggioRitornoStream = entity.MessaggioRitornoStream;
            entityTransformed.MailBody = entity.MailBody;
            entityTransformed.RecordedInDocSuite = entity.RecordedInDocSuite;
            entityTransformed.ContentLength = entity.ContentLength;
            entityTransformed.IsToForward = entity.IsToForward;
            entityTransformed.IsValidForInterop = entity.IsValidForInterop;
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.MailStatus = entity.MailStatus;
            entityTransformed.DestinationNote = entity.DestinationNote;
            entityTransformed.IsDestinated = entity.IsDestinated;
            entityTransformed.Handler = entity.Handler;
            entityTransformed.IDAttachments = entity.IDAttachments;
            entityTransformed.IDDaticert = entity.IDDaticert;
            entityTransformed.IDEnvelope = entity.IDEnvelope;
            entityTransformed.IDMailContent = entity.IDMailContent;
            entityTransformed.IDPostacert = entity.IDPostacert;
            entityTransformed.IDSegnatura = entity.IDSegnatura;
            entityTransformed.IDSmime = entity.IDSmime;
            entityTransformed.PECType = entity.PECType;
            entityTransformed.Checksum = entity.Checksum;
            entityTransformed.Multiple = entity.Multiple;
            entityTransformed.SplittedFrom = entity.SplittedFrom;
            entityTransformed.OriginalRecipient = entity.OriginalRecipient;
            entityTransformed.HeaderChecksum = entity.HeaderChecksum;
            entityTransformed.ProcessStatus = entity.ProcessStatus;
            entityTransformed.MailRecipientsCc = entity.MailRecipientsCc;
            entityTransformed.ReceivedAsCc = entity.ReceivedAsCc;
            entityTransformed.Size = entity.Size;
            entityTransformed.MultipleType = entity.MultipleType;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.IdUDS = entity.IdUDS;
            entityTransformed.DocumentUnitType = entity.DocumentUnitType;
            entityTransformed.InvoiceStatus = entity.InvoiceStatus;

            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Location = entity.Location;
            entityTransformed.PECMailBox = entity.PECMailBox;
            entityTransformed.UDSRepository = entity.UDSRepository;
            entityTransformed.PECMailReceipts = entity.PECMailReceipts;
            entityTransformed.PECMailChildrenReceipts = entity.PECMailChildrenReceipts;
            entityTransformed.Attachments = entity.Attachments;
            #endregion



            return entityTransformed;
        }

    }
}
