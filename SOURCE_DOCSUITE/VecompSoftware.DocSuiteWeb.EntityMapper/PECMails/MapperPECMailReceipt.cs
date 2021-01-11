using DSW = VecompSoftware.DocSuiteWeb.Data;
using NHibernate;
using System;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.PECMails
{
    public class MapperPECMailReceipt : BaseEntityMapper<DSW.PECMailReceipt, PECMailReceipt>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public MapperPECMailReceipt()
        {
        }
        #endregion

        #region [ Methods ]
        protected override IQueryOver<DSW.PECMailReceipt, DSW.PECMailReceipt> MappingProjection(IQueryOver<DSW.PECMailReceipt, DSW.PECMailReceipt> queryOver)
        {
            throw new System.NotImplementedException();
        }

        protected override PECMailReceipt TransformDTO(DSW.PECMailReceipt entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("Impossibile trasformare PECMailReceipt se l'entità non è inizializzata");
            }

            PECMailReceipt apiPECMail = new PECMailReceipt();
            apiPECMail.DateZone = entity.DateZone;
            apiPECMail.EntityId = entity.Id;
            apiPECMail.ErrorDescription = entity.ErrorDescription;
            apiPECMail.ErrorShort = entity.ErrorShort;
            apiPECMail.Identification = entity.Identification;
            apiPECMail.LastChangedDate = entity.LastChangedDate;
            apiPECMail.LastChangedUser = entity.LastChangedUser;
            apiPECMail.MSGID = entity.MSGID;
            apiPECMail.Provider = entity.Provider;
            apiPECMail.ReceiptDate = entity.ReceiptDate;
            apiPECMail.ReceiptType = entity.ReceiptType;
            apiPECMail.Receiver = entity.Receiver;
            apiPECMail.ReceiverType = entity.ReceiverType;
            apiPECMail.RegistrationDate = entity.RegistrationDate;
            apiPECMail.RegistrationUser = entity.RegistrationUser;
            apiPECMail.Sender = entity.Sender;
            apiPECMail.Subject = entity.Subject;
            apiPECMail.Timestamp = entity.Timestamp;
            apiPECMail.UniqueId = entity.UniqueId;
            return apiPECMail;
        }
        #endregion

    }
}
