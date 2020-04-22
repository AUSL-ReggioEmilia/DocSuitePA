using DSW = VecompSoftware.DocSuiteWeb.Data;
using NHibernate;
using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.EntityMapper.Commons;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.PECMails
{
    public class MapperPECMailEntity : BaseEntityMapper<DSW.PECMail, PECMail>
    {
        #region [ Fields ]
        private readonly MapperLocationEntity _mapperLocation;
        private readonly MapperPECMailBoxEntity _mapperPECMailBox;
        private readonly MapperPECMailAttachment _mapperPECMailAttachment;
        #endregion

        #region [ Constructor ]
        public MapperPECMailEntity()
        {
            _mapperLocation = new MapperLocationEntity();
            _mapperPECMailBox = new MapperPECMailBoxEntity();
            _mapperPECMailAttachment = new MapperPECMailAttachment();
        }
        #endregion

        #region [ Methods ]
        protected override IQueryOver<DSW.PECMail, DSW.PECMail> MappingProjection(IQueryOver<DSW.PECMail, DSW.PECMail> queryOver)
        {
            throw new System.NotImplementedException();
        }

        protected override PECMail TransformDTO(DSW.PECMail entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("Impossibile trasformare PECMail se l'entità non è inizializzata");
            }

            PECMail apiPECMail = new PECMail();
            apiPECMail.Checksum = entity.Checksum;
            apiPECMail.DestinationNote = entity.DestinationNote;
            apiPECMail.Direction = (PECMailDirection)entity.Direction;
            apiPECMail.EntityId = entity.Id;
            apiPECMail.Handler = entity.Handler;
            apiPECMail.HeaderChecksum = entity.HeaderChecksum;
            apiPECMail.IDAttachments = entity.IDAttachments;
            apiPECMail.IDDaticert = entity.IDDaticert;
            apiPECMail.IDEnvelope = entity.IDEnvelope;
            apiPECMail.IDMailContent = entity.IDMailContent;
            apiPECMail.IDPostacert = entity.IDPostacert;
            apiPECMail.IDSegnatura = entity.IDSegnatura;
            apiPECMail.IDSmime = entity.IDSmime;
            apiPECMail.IdUDS = entity.IdUDS;
            apiPECMail.IsActive = (PECMailActiveType)entity.IsActive;
            apiPECMail.IsDestinated = entity.IsDestinated;
            apiPECMail.IsToForward = entity.IsToForward;
            apiPECMail.IsValidForInterop = entity.IsValidForInterop;
            apiPECMail.LastChangedDate = entity.LastChangedDate;
            apiPECMail.LastChangedUser = entity.LastChangedUser;
            apiPECMail.MailBody = entity.MailBody;
            apiPECMail.MailContent = entity.MailContent;
            apiPECMail.MailDate = entity.MailDate;
            apiPECMail.MailError = entity.MailError;
            apiPECMail.MailPriority = (PECMailPriority?)entity.MailPriority;
            apiPECMail.MailRecipients = entity.MailRecipients;
            apiPECMail.MailRecipientsCc = entity.MailRecipientsCc;
            apiPECMail.MailSenders = entity.MailSenders;
            apiPECMail.MailStatus = entity.MailStatus;
            apiPECMail.MailSubject = entity.MailSubject;
            apiPECMail.MailType = entity.MailType;
            apiPECMail.MailUID = entity.MailUID;
            apiPECMail.MessageID = entity.MessageID;
            apiPECMail.MessaggioRitornoName = entity.MessaggioRitornoName;
            apiPECMail.MessaggioRitornoStream = entity.MessaggioRitornoStream;
            apiPECMail.Multiple = entity.Multiple;
            apiPECMail.MultipleType = (PECMailMultipleType?)entity.MultipleType;
            apiPECMail.Number = entity.Number;
            apiPECMail.OriginalRecipient = entity.OriginalRecipient;
            apiPECMail.PECType = (PECType?)entity.PECType;
            apiPECMail.ProcessStatus = (PECMailProcessStatus?)entity.ProcessStatus;
            apiPECMail.ReceivedAsCc = entity.ReceivedAsCc;
            apiPECMail.RecordedInDocSuite = Convert.ToByte(entity.RecordedInDocSuite);
            apiPECMail.RegistrationDate = entity.RegistrationDate;
            apiPECMail.RegistrationUser = entity.RegistrationUser;
            apiPECMail.Segnatura = entity.Segnatura;
            apiPECMail.Size = entity.Size;
            apiPECMail.SplittedFrom = entity.SplittedFrom;
            apiPECMail.Timestamp = entity.Timestamp;
            apiPECMail.UniqueId = entity.UniqueId;
            apiPECMail.XRiferimentoMessageID = entity.XRiferimentoMessageID;
            apiPECMail.XTrasporto = entity.XTrasporto;
            apiPECMail.Year = entity.Year;
            apiPECMail.InvoiceStatus = (InvoiceStatus?)entity.InvoiceStatus;
            apiPECMail.Location = entity.Location != null ? _mapperLocation.MappingDTO(entity.Location) : null;
            apiPECMail.PECMailBox = entity.MailBox != null ? _mapperPECMailBox.MappingDTO(entity.MailBox) : null;
            apiPECMail.Attachments = entity.Attachments.Select(f => _mapperPECMailAttachment.MappingDTO(f)).ToList();
            return apiPECMail;
        }
        #endregion

    }
}
