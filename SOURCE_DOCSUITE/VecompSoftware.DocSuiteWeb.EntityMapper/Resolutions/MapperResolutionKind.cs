using System;
using NHibernate;
using APIResolution = VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using DSW = VecompSoftware.DocSuiteWeb.Data;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Resolutions
{
    public class MapperResolutionKind : BaseEntityMapper<DSW.ResolutionKind, APIResolution.ResolutionKind>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]

        public MapperResolutionKind() : base()
        {
        }
        #endregion

        #region [ Methods ]

        protected override IQueryOver<DSW.ResolutionKind, DSW.ResolutionKind> MappingProjection(IQueryOver<DSW.ResolutionKind, DSW.ResolutionKind> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override APIResolution.ResolutionKind TransformDTO(DSW.ResolutionKind entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("Impossibile trasformare ResolutionKind se l'entità non è inizializzata");
            }

            APIResolution.ResolutionKind model = new APIResolution.ResolutionKind(entity.UniqueId)
            {
                AmountEnabled = entity.AmountEnabled,
                IsActive = entity.IsActive,
                Name = entity.Name,                
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
