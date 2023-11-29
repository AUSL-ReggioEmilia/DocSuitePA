using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Collaborations
{
    public class CollaborationTableValuedModelMapper : BaseModelMapper<CollaborationTableValuedModel, CollaborationModel>, ICollaborationTableValuedModelMapper
    {

        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        public CollaborationTableValuedModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }
        public override CollaborationModel Map(CollaborationTableValuedModel model, CollaborationModel modelTransformed)
        {
            modelTransformed.IdCollaboration = model.IdCollaboration;
            modelTransformed.AlertDate = model.AlertDate;
            modelTransformed.DocumentType = model.DocumentType;
            modelTransformed.IdPriority = model.IdPriority;
            modelTransformed.IdStatus = model.IdStatus;
            modelTransformed.MemorandumDate = model.MemorandumDate;
            modelTransformed.Note = model.Note;
            modelTransformed.Number = model.Number;
            modelTransformed.PublicationDate = model.PublicationDate;
            modelTransformed.PublicationUser = model.PublicationUser;
            modelTransformed.RegistrationName = model.RegistrationName;
            modelTransformed.RegistrationUser = model.RegistrationUser;
            modelTransformed.RegistrationDate = model.RegistrationDate;
            modelTransformed.LastChangedDate = model.LastChangedDate;
            modelTransformed.SignCount = model.SignCount;
            modelTransformed.Subject = model.Subject;
            modelTransformed.Year = model.Year;
            modelTransformed.TemplateName = model.TemplateName;

            modelTransformed.DocumentUnit = _mapperUnitOfWork.Repository<IDomainMapper<CollaborationTableValuedModel, DocumentUnitModel>>().Map(model, null);
            
            return modelTransformed;
        }
        public override ICollection<CollaborationModel> MapCollection(ICollection<CollaborationTableValuedModel> model)
        {
            if (model == null)
            {
                return new List<CollaborationModel>();
            }

            List<CollaborationModel> modelsTransformed = new List<CollaborationModel>();
            CollaborationModel modelTransformed = null;
            ICollection<CollaborationTableValuedModel> collaborationSigns;
            ICollection<CollaborationTableValuedModel> collaborationUsers;
            ICollection<CollaborationTableValuedModel> collaborationVersionings;
            foreach (IGrouping<int, CollaborationTableValuedModel> collaborationLookup in model.ToLookup(x => x.IdCollaboration))
            {
                modelTransformed = Map(collaborationLookup.First(), new CollaborationModel());
                collaborationSigns = collaborationLookup.ToLookup(x => x.CollaborationSign_IdCollaborationSign)
                    .Select(f => f.First())
                    .ToList();
                collaborationUsers = collaborationLookup.ToLookup(x => x.CollaborationUser_IdCollaborationUser)
                    .Select(f => f.First())
                    .ToList();
                collaborationVersionings = collaborationLookup.ToLookup(x => x.CollaborationVersioning_IdCollaborationVersioning)
                    .Select(f => f.First())
                    .ToList();

                modelTransformed.CollaborationSigns = _mapperUnitOfWork.Repository<IDomainMapper<CollaborationTableValuedModel, CollaborationSignModel>>().MapCollection(collaborationSigns);
                modelTransformed.CollaborationUsers = _mapperUnitOfWork.Repository<IDomainMapper<CollaborationTableValuedModel, CollaborationUserModel>>().MapCollection(collaborationUsers);
                modelTransformed.CollaborationVersionings = _mapperUnitOfWork.Repository<IDomainMapper<CollaborationTableValuedModel, CollaborationVersioningModel>>().MapCollection(collaborationVersionings);

                modelsTransformed.Add(modelTransformed);
            }
            return modelsTransformed;
        }

    }
}
