using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.PECMails;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.PECMails
{
    public class PECMailAttachmentValidatorMapper : BaseMapper<PECMailAttachment, PECMailAttachmentValidator>, IPECMailAttachmentValidatorMapper
    {
        public PECMailAttachmentValidatorMapper() { }

        public override PECMailAttachmentValidator Map(PECMailAttachment entity, PECMailAttachmentValidator entityTransformed)
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

            #region [ Navigation Properties ]
            entityTransformed.PECMail = entity.PECMail;
            entityTransformed.Parent = entity.Parent;
            #endregion

            return entityTransformed;
        }

    }
}
