using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftwareFascicle = VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Fascicles
{
    public class FascicleModelMapper : BaseModelMapper<Fascicle, VecompSoftwareFascicle.FascicleModel>, IFascicleModelMapper
    {
        #region [ Fields ]
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        #endregion

        #region [ Constructor ]
        public FascicleModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }
        #endregion

        #region [ Methods ]
        public override VecompSoftwareFascicle.FascicleModel Map(Fascicle entity, VecompSoftwareFascicle.FascicleModel modelTransformed)
        {
            modelTransformed.UniqueId = entity.UniqueId;
            modelTransformed.Year = entity.Year;
            modelTransformed.Number = entity.Number;
            modelTransformed.Conservation = entity.Conservation;
            modelTransformed.StartDate = entity.StartDate;
            modelTransformed.EndDate = entity.EndDate;
            modelTransformed.Name = entity.Name;
            modelTransformed.Title = entity.Title;
            modelTransformed.FascicleObject = entity.FascicleObject;
            modelTransformed.RegistrationDate = entity.RegistrationDate;
            modelTransformed.RegistrationUser = entity.RegistrationUser;
            modelTransformed.Manager = entity.Manager;
            modelTransformed.Rack = entity.Rack;
            modelTransformed.Note = entity.Note;
            modelTransformed.FascicleType = (VecompSoftwareFascicle.FascicleType)entity.FascicleType;
            modelTransformed.VisibilityType = (VecompSoftwareFascicle.VisibilityType)entity.VisibilityType;
            modelTransformed.MetadataValues = entity.MetadataValues;
            modelTransformed.FascicleDocumentUnits = _mapperUnitOfWork.Repository<IDomainMapper<FascicleDocumentUnit, VecompSoftwareFascicle.FascicleDocumentUnitModel>>().MapCollection(entity.FascicleDocumentUnits);
            modelTransformed.FascicleDocuments = _mapperUnitOfWork.Repository<IDomainMapper<FascicleDocument, VecompSoftwareFascicle.FascicleDocumentModel>>().MapCollection(entity.FascicleDocuments);
            modelTransformed.Category = entity.Category == null ? null : _mapperUnitOfWork.Repository<IDomainMapper<Category, CategoryModel>>().Map(entity.Category, new CategoryModel());
            modelTransformed.Container = entity.Container == null ? null : _mapperUnitOfWork.Repository<IDomainMapper<Container, ContainerModel>>().Map(entity.Container, new ContainerModel());
            return modelTransformed;
        }
        #endregion
    }
}
