using VecompSoftware.DocSuiteWeb.Entity.PECMails;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.PECMails
{
    public class PECMailAttachmentMapper : BaseEntityMapper<PECMailAttachment, PECMailAttachment>, IPECMailAttachmentMapper
    {
        public override PECMailAttachment Map(PECMailAttachment entity, PECMailAttachment entityTransformed)
        {

            #region [ Base ]
            entityTransformed.EntityId = entity.EntityId;
            entityTransformed.AttachmentName = entity.AttachmentName;
            entityTransformed.IsMain = entity.IsMain;
            entityTransformed.IDDocument = entity.IDDocument;
            entityTransformed.Size = entity.Size;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            #endregion

            return entityTransformed;

        }

    }
}
