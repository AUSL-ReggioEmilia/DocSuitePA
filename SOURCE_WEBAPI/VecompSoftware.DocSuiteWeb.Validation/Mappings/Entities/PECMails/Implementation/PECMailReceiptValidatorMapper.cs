using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.PECMails;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.PECMails
{
    public class PECMailReceiptValidatorMapper : BaseMapper<PECMailReceipt, PECMailReceiptValidator>, IPECMailReceiptValidatorMapper
    {
        public PECMailReceiptValidatorMapper() { }

        public override PECMailReceiptValidator Map(PECMailReceipt entity, PECMailReceiptValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.EntityId = entity.EntityId;
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
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.PECMail = entity.PECMail;
            entityTransformed.PECMailParent = entity.PECMailParent;
            #endregion



            return entityTransformed;
        }

    }
}
