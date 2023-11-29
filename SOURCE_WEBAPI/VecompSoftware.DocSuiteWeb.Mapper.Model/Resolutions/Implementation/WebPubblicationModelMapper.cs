using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Model.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Resolutions
{
    public class WebPublicationModelMapper : BaseModelMapper<WebPublication, WebPublicationModel>, IWebPublicationModelMapper
    {
        #region [ Fields ]
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        #endregion

        #region [ Properties ]
        public Resolution Resolution { get; set; }
        #endregion

        #region [ Constructor ]
        public WebPublicationModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }
        #endregion

        #region [ Methods ]
        public override WebPublicationModel Map(WebPublication entity, WebPublicationModel modelTransformed)
        {
            modelTransformed.Id = entity.EntityId;
            modelTransformed.ExternalKey = entity.ExternalKey;
            modelTransformed.Status = entity.Status;
            modelTransformed.IDLocation = entity.IDLocation;
            modelTransformed.IDDocument = entity.IDDocument;
            modelTransformed.EnumDocument = entity.EnumDocument;
            modelTransformed.Descrizione = entity.Descrizione;
            modelTransformed.LastChangedDate = entity.LastChangedDate;
            modelTransformed.LastChangedUser = entity.LastChangedUser;
            modelTransformed.RegistrationUser = entity.RegistrationUser;
            modelTransformed.RegistrationDate = entity.RegistrationDate;
            modelTransformed.IsPrivacy = entity.IsPrivacy;

            if (Resolution != null)
            {
                modelTransformed.Resolution = _mapperUnitOfWork.Repository<IDomainMapper<Resolution, ResolutionModel>>().Map(Resolution, new ResolutionModel());
            }

            return modelTransformed;
        }
        #endregion
    }
}
