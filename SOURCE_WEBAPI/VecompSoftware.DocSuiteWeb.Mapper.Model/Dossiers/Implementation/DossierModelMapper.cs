using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers;
using DossierStatus = VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers.DossierStatus;
using DossierType = VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers.DossierType;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Dossiers
{
    public class DossierModelMapper : BaseModelMapper<Dossier, DossierModel>, IDossierModelMapper
    {
        #region [ Fields ]
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        #endregion

        #region [ Constructor ]
        public DossierModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }
        #endregion

        #region [ Methods ]

        public override DossierModel Map(Dossier entity, DossierModel modelTransformed)
        {
            modelTransformed.UniqueId = entity.UniqueId;
            modelTransformed.Year = entity.Year;
            modelTransformed.Number = entity.Number;
            modelTransformed.Title = string.Format("{0}/{1:0000000}", entity.Year, entity.Number);
            modelTransformed.Subject = entity.Subject;
            modelTransformed.Note = entity.Note;
            modelTransformed.RegistrationDate = entity.RegistrationDate;
            modelTransformed.RegistrationUser = entity.RegistrationUser;
            modelTransformed.LastChangedDate = entity.LastChangedDate;
            modelTransformed.LastChangedUser = entity.LastChangedUser;
            modelTransformed.StartDate = entity.StartDate;
            modelTransformed.EndDate = entity.EndDate;
            modelTransformed.MetadataDesigner = entity.MetadataDesigner;
            modelTransformed.MetadataValues = entity.MetadataValues;
            modelTransformed.ContainerName = entity.Container == null ? null : entity.Container.Name;
            modelTransformed.ContainerId = entity.Container == null ? short.Parse("0") : entity.Container.EntityShortId;
            modelTransformed.Category = entity.Category == null ? null : _mapperUnitOfWork.Repository<IDomainMapper<Category, CategoryModel>>().Map(entity.Category, new CategoryModel());
            modelTransformed.DossierType = (DossierType)entity.DossierType;
            modelTransformed.Status = (DossierStatus)entity.Status;

            return modelTransformed;
        }
        #endregion
    }
}
