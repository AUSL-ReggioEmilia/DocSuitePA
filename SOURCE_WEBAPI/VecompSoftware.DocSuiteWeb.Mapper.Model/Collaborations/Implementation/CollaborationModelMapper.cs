using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Model.Entities.Resolutions;

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
            modelTransformed.DocumentSeriesItem = entity.DocumentSeriesItem == null ? null : _mapperUnitOfWork.Repository<IDomainMapper<DocumentSeriesItem, DocumentSeriesItemModel>>().Map(entity.DocumentSeriesItem, new DocumentSeriesItemModel());
            modelTransformed.Resolution = entity.DocumentSeriesItem == null ? null : _mapperUnitOfWork.Repository<IDomainMapper<Resolution, ResolutionModel>>().Map(entity.Resolution, new ResolutionModel());

            return modelTransformed;
        }
    }
}
