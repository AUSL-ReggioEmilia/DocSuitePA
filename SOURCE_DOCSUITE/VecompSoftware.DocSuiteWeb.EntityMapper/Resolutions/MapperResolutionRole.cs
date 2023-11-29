using System;
using NHibernate;
using DSW = VecompSoftware.DocSuiteWeb.Data;
using APIResolution = VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.EntityMapper.Commons;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Resolutions
{
    public class MapperResolutionRole : BaseEntityMapper<DSW.ResolutionRole, APIResolution.ResolutionRole>
    {
        #region [ Fields ]
        private readonly MapperRoleEntity _mapperRoleEntity; 
        #endregion

        #region [ Constructor ]

        public MapperResolutionRole() : base()
        {
            _mapperRoleEntity = new MapperRoleEntity();
        }
        #endregion

        #region [ Methods ]

        protected override IQueryOver<DSW.ResolutionRole, DSW.ResolutionRole> MappingProjection(IQueryOver<DSW.ResolutionRole, DSW.ResolutionRole> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override APIResolution.ResolutionRole TransformDTO(DSW.ResolutionRole entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("Impossibile trasformare ResolutionRole se l'entità non è inizializzata");
            }

            APIResolution.ResolutionRole model = new APIResolution.ResolutionRole(entity.UniqueId)
            {
                IdResolutionRoleType = entity.Id.IdResolutionRoleType,
                Role = _mapperRoleEntity.MappingDTO(entity.Role),
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
