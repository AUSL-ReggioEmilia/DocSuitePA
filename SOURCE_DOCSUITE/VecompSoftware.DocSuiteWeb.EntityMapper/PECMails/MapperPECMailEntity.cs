using NHibernate;
using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.EntityMapper.Commons;
using VecompSoftware.DocSuiteWeb.EntityMapper.DocumentUnits;
using DSW = VecompSoftware.DocSuiteWeb.Data;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.PECMails
{
    public class MapperPECMailEntity : BaseEntityMapper<DSW.PECMail, PECMail>
    {
        #region [ Fields ]
        private readonly MapperLocationEntity _mapperLocation;
        private readonly MapperPECMailBoxEntity _mapperPECMailBox;
        private readonly MapperPECMailAttachment _mapperPECMailAttachment;
        private readonly MapperDocumentUnit _mapperDocumentUnit;
        #endregion

        #region [ Constructor ]
        public MapperPECMailEntity()
        {
            _mapperLocation = new MapperLocationEntity();
            _mapperPECMailBox = new MapperPECMailBoxEntity();
            _mapperPECMailAttachment = new MapperPECMailAttachment();
            _mapperDocumentUnit = new MapperDocumentUnit();
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

            PECMail apiPECMail = new PECMail
            {
                Checksum = entity.Checksum,
                DestinationNote = entity.DestinationNote,
                Direction = (PECMailDirection)entity.Direction,
                EntityId = entity.Id,
                Handler = entity.Handler,
                HeaderChecksum = entity.HeaderChecksum,
                IDAttachments = entity.IDAttachments,
                IDDaticert = entity.IDDaticert,
                IDEnvelope = entity.IDEnvelope,
                IDMailContent = entity.IDMailContent,
                IDPostacert = entity.IDPostacert,
                IDSegnatura = entity.IDSegnatura,
                IDSmime = entity.IDSmime,
                IsActive = (PECMailActiveType)entity.IsActive,
                IsDestinated = entity.IsDestinated,
                IsToForward = entity.IsToForward,
                IsValidForInterop = entity.IsValidForInterop,
                LastChangedDate = entity.LastChangedDate,
                LastChangedUser = entity.LastChangedUser,
                MailBody = entity.MailBody,
                MailContent = entity.MailContent,
                MailDate = entity.MailDate,
                MailError = entity.MailError,
                MailPriority = (PECMailPriority?)entity.MailPriority,
                MailRecipients = entity.MailRecipients,
                MailRecipientsCc = entity.MailRecipientsCc,
                MailSenders = entity.MailSenders,
                MailStatus = entity.MailStatus,
                MailSubject = entity.MailSubject,
                MailType = entity.MailType,
                MailUID = entity.MailUID,
                MessageID = entity.MessageID,
                MessaggioRitornoName = entity.MessaggioRitornoName,
                MessaggioRitornoStream = entity.MessaggioRitornoStream,
                Multiple = entity.Multiple,
                MultipleType = (PECMailMultipleType?)entity.MultipleType,
                Number = entity.Number,
                OriginalRecipient = entity.OriginalRecipient,
                PECType = (PECType?)entity.PECType,
                ProcessStatus = (PECMailProcessStatus?)entity.ProcessStatus,
                ReceivedAsCc = entity.ReceivedAsCc,
                RecordedInDocSuite = Convert.ToByte(entity.RecordedInDocSuite),
                RegistrationDate = entity.RegistrationDate,
                RegistrationUser = entity.RegistrationUser,
                Segnatura = entity.Segnatura,
                Size = entity.Size,
                SplittedFrom = entity.SplittedFrom,
                Timestamp = entity.Timestamp,
                UniqueId = entity.UniqueId,
                XRiferimentoMessageID = entity.XRiferimentoMessageID,
                XTrasporto = entity.XTrasporto,
                Year = entity.Year,
                InvoiceStatus = (InvoiceStatus?)entity.InvoiceStatus,
                Location = entity.Location != null ? _mapperLocation.MappingDTO(entity.Location) : null,
                DocumentUnit = entity.DocumentUnit != null ? _mapperDocumentUnit.MappingDTO(entity.DocumentUnit) : null,
                PECMailBox = entity.MailBox != null ? _mapperPECMailBox.MappingDTO(entity.MailBox) : null,
                Attachments = entity.Attachments.Select(f => _mapperPECMailAttachment.MappingDTO(f)).ToList()
            };
            return apiPECMail;
        }
        #endregion

    }
}
