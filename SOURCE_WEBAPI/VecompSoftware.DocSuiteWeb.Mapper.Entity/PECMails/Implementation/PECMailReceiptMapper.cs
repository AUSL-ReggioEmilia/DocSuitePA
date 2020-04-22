using VecompSoftware.DocSuiteWeb.Entity.PECMails;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.PECMails
{
    public class PECMailReceiptMapper : BaseEntityMapper<PECMailReceipt, PECMailReceipt>, IPECMailReceiptMapper
    {
        public override PECMailReceipt Map(PECMailReceipt entity, PECMailReceipt entityTransformed)
        {
            #region [ Base ]

            entityTransformed.ReceiptType = entity.ReceiptType;
            entityTransformed.ErrorShort = entity.ErrorShort;
            entityTransformed.ErrorDescription = entity.ErrorDescription;
            entityTransformed.DateZone = entity.DateZone;
            entityTransformed.ReceiptDate = entity.ReceiptDate;
            entityTransformed.Sender = entity.Sender;
            entityTransformed.ReceiverType = entity.ReceiverType;
            entityTransformed.Receiver = entity.Receiver;
            entityTransformed.Subject = entity.Subject;
            entityTransformed.Provider = entity.Provider;
            entityTransformed.Identification = entity.Identification;
            entityTransformed.MSGID = entity.MSGID;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.Timestamp = entity.Timestamp;

            #endregion

            return entityTransformed;
        }

    }
}
