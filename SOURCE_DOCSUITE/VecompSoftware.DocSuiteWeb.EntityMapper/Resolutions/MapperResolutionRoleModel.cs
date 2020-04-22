using System;
using NHibernate;
using VecompSoftware.DocSuiteWeb.Model.Entities.Resolutions;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.EntityMapper.Commons;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Resolutions
{
    public class MapperResolutionRoleModel : BaseEntityMapper<ResolutionRole, ResolutionRoleModel>
    {
        #region [ Fields ]
        private readonly MapperRoleModel _mapperRoleModel; 
        #endregion

        #region [ Constructor ]

        public MapperResolutionRoleModel() : base()
        {
            _mapperRoleModel = new MapperRoleModel();
        }
        #endregion

        #region [ Methods ]

        protected override IQueryOver<ResolutionRole, ResolutionRole> MappingProjection(IQueryOver<ResolutionRole, ResolutionRole> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override ResolutionRoleModel TransformDTO(ResolutionRole entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare ResolutionRole se l'entità non è inizializzata");

            ResolutionRoleModel model = new ResolutionRoleModel(entity.UniqueId)
            {
                IdResolution = entity.Id.IdResolution,
                IdResolutionRoleType = entity.Id.IdResolutionRoleType,
                Role = _mapperRoleModel.MappingDTO(entity.Role),
                RegistrationUser = entity.RegistrationUser,
                RegistrationDate = entity.RegistrationDate,
                LastChangedDate = entity.LastChangedDate,
                LastChangedUser = entity.LastChangedUser
            };

            return model;
        }

        #endregion
    }
}
