using VecompSoftware.DocSuiteWeb.Entity.Conservations;
using VecompSoftware.DocSuiteWeb.Model.Entities.Conservations;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Conservations
{
    public class ConservationModelMapper : BaseModelMapper<Conservation, ConservationModel>, IConservationModelMapper
    {
        #region [ Fields ]
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        #endregion

        #region [ Constructor ]
        public ConservationModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }
        #endregion

        #region [ Methods ]

        public override ConservationModel Map(Conservation entity, ConservationModel modelTransformed)
        {
            modelTransformed.EntityType = entity.EntityType;
            modelTransformed.Message = entity.Message;
            modelTransformed.SendDate = entity.SendDate;
            modelTransformed.Status = (DocSuiteWeb.Model.Conservations.ConservationStatus)entity.Status;
            modelTransformed.Type = (DocSuiteWeb.Model.Conservations.ConservationType)entity.Type;
            modelTransformed.Uri = entity.Uri;
            modelTransformed.UniqueId = entity.UniqueId;
            modelTransformed.RegistrationDate = entity.RegistrationDate;
            modelTransformed.RegistrationUser = entity.RegistrationUser;
            modelTransformed.LastChangedDate = entity.LastChangedDate;
            modelTransformed.LastChangedUser = entity.LastChangedUser;

            return modelTransformed;
        }
        #endregion
    }
}
