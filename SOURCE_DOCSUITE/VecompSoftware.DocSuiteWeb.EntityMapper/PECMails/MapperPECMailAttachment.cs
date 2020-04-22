using DSW = VecompSoftware.DocSuiteWeb.Data;
using NHibernate;
using System;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.PECMails
{
    public class MapperPECMailAttachment : BaseEntityMapper<DSW.PECMailAttachment, PECMailAttachment>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public MapperPECMailAttachment()
        {
            
        }
        #endregion

        #region [ Methods ]
        protected override IQueryOver<DSW.PECMailAttachment, DSW.PECMailAttachment> MappingProjection(IQueryOver<DSW.PECMailAttachment, DSW.PECMailAttachment> queryOver)
        {
            throw new System.NotImplementedException();
        }

        protected override PECMailAttachment TransformDTO(DSW.PECMailAttachment entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("Impossibile trasformare PECMailAttachment se l'entità non è inizializzata");
            }

            PECMailAttachment apiPECMailAttachment = new PECMailAttachment();
            apiPECMailAttachment.AttachmentName = entity.AttachmentName;
            apiPECMailAttachment.EntityId = entity.Id;
            apiPECMailAttachment.IDDocument = entity.IDDocument;
            apiPECMailAttachment.IsMain = entity.IsMain;
            apiPECMailAttachment.LastChangedDate = entity.LastChangedDate;
            apiPECMailAttachment.LastChangedUser = entity.LastChangedUser;
            apiPECMailAttachment.Size = entity.Size;
            apiPECMailAttachment.RegistrationDate = entity.RegistrationDate;
            apiPECMailAttachment.RegistrationUser = entity.RegistrationUser;
            apiPECMailAttachment.UniqueId = entity.UniqueId;
            return apiPECMailAttachment;
        }
        #endregion

    }
}
