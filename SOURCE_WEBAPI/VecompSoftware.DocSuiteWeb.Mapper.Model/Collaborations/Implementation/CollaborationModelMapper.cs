using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Collaborations
{
    public class CollaborationModelMapper : BaseModelMapper<Collaboration, CollaborationModel>, ICollaborationModelMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;

        public CollaborationModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }
        public override CollaborationModel Map(Collaboration entity, CollaborationModel modelTransformed)
        {
            modelTransformed.IdCollaboration = entity.EntityId;
            modelTransformed.AlertDate = entity.AlertDate;
            modelTransformed.DocumentType = entity.DocumentType;
            modelTransformed.IdPriority = entity.IdPriority;
            modelTransformed.IdStatus = entity.IdStatus;
            modelTransformed.MemorandumDate = entity.MemorandumDate;
            modelTransformed.Note = entity.Note;
            modelTransformed.Number = entity.Number;
            modelTransformed.PublicationDate = entity.PublicationDate;
            modelTransformed.PublicationUser = entity.PublicationUser;
            modelTransformed.LastChangedDate = entity.LastChangedDate;
            modelTransformed.RegistrationName = entity.RegistrationName;
            modelTransformed.RegistrationUser = entity.RegistrationUser;
            modelTransformed.SignCount = entity.SignCount;
            modelTransformed.Subject = entity.Subject;
            modelTransformed.Year = entity.Year;
            modelTransformed.TemplateName = entity.TemplateName;

            modelTransformed.CollaborationVersionings = _mapperUnitOfWork.Repository<IDomainMapper<CollaborationVersioning, CollaborationVersioningModel>>().MapCollection(entity.CollaborationVersionings).ToList();
            modelTransformed.CollaborationUsers = _mapperUnitOfWork.Repository<IDomainMapper<CollaborationUser, CollaborationUserModel>>().MapCollection(entity.CollaborationUsers).ToList();
            modelTransformed.CollaborationSigns = _mapperUnitOfWork.Repository<IDomainMapper<CollaborationSign, CollaborationSignModel>>().MapCollection(entity.CollaborationSigns).ToList();
            modelTransformed.DocumentUnit = entity.DocumentUnit == null ? null : _mapperUnitOfWork.Repository<IDomainMapper<DocumentUnit, DocumentUnitModel>>().Map(entity.DocumentUnit, new DocumentUnitModel());
            
            return modelTransformed;
        }
    }
}
