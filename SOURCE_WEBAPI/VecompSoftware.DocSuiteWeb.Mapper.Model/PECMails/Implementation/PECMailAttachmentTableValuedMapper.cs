using VecompSoftware.DocSuiteWeb.Model.Entities.PECMails;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.PECMails
{
    public class PECMailAttachmentTableValuedMapper : BaseModelMapper<PECMailAttachmentTableValuedModel, PECMailAttachmentTableValuedModel>, IPECMailAttachmentTableValuedMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;

        public PECMailAttachmentTableValuedMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;

        }
        public override PECMailAttachmentTableValuedModel Map(PECMailAttachmentTableValuedModel model, PECMailAttachmentTableValuedModel modelTransformed)
        {
            modelTransformed.EntityId = model.EntityId;
            modelTransformed.AttachmentName = model.AttachmentName;
            modelTransformed.IsMain = model.IsMain;
            modelTransformed.IDDocument = model.IDDocument;
            modelTransformed.Size = model.Size;
            modelTransformed.UniqueId = model.UniqueId;
            modelTransformed.RegistrationUser = model.RegistrationUser;
            modelTransformed.RegistrationDate = model.RegistrationDate;
            modelTransformed.LastChangedUser = model.LastChangedUser;
            modelTransformed.LastChangedDate = model.LastChangedDate;

            return modelTransformed;
        }
    }
}
