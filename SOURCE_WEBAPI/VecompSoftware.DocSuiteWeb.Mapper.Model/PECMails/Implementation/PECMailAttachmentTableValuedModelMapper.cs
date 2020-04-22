using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Model.Entities.PECMails;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.PECMails
{
    public class PECMailAttachmentTableValuedModelMapper : BaseModelMapper<PECMailAttachmentTableValuedModel, PECMailAttachmentModel>, IPECMailAttachmentTableValuedModelMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        public PECMailAttachmentTableValuedModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        public override PECMailAttachmentModel Map(PECMailAttachmentTableValuedModel model, PECMailAttachmentModel modelTransformed)
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

        public override ICollection<PECMailAttachmentModel> MapCollection(ICollection<PECMailAttachmentTableValuedModel> model)
        {
            if (model == null)
            {
                return new List<PECMailAttachmentModel>();
            }
            List<PECMailAttachmentModel> modelsTransformed = new List<PECMailAttachmentModel>();
            PECMailAttachmentModel modelTransformed = null;
            foreach (IGrouping<int, PECMailAttachmentTableValuedModel> transparentAdministrationMonitorLookup in model.ToLookup(x => x.UniqueId))
            {
                modelTransformed = Map(transparentAdministrationMonitorLookup.First(), new PECMailAttachmentModel());
                modelsTransformed.Add(modelTransformed);
            }
            return modelsTransformed;
        }
    }
}
