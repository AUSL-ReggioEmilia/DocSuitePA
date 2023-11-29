using System;
using NHibernate;
using DSW = VecompSoftware.DocSuiteWeb.Data;
using APIResolution = VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.EntityMapper.Commons;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Resolutions
{
    public class MapperResolutionContact : BaseEntityMapper<DSW.ResolutionContact, APIResolution.ResolutionContact>
    {
        #region [ Fields ]
        private readonly MapperContactEntity _mapperContactEntity;
        #endregion

        #region [ Constructor ]

        public MapperResolutionContact() : base()
        {
            _mapperContactEntity = new MapperContactEntity();
        }
        #endregion

        #region [ Methods ]

        protected override IQueryOver<DSW.ResolutionContact, DSW.ResolutionContact> MappingProjection(IQueryOver<DSW.ResolutionContact, DSW.ResolutionContact> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override APIResolution.ResolutionContact TransformDTO(DSW.ResolutionContact entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("Impossibile trasformare ResolutionContact se l'entità non è inizializzata");
            }

            APIResolution.ResolutionContact model = new APIResolution.ResolutionContact(entity.UniqueId);
            model.Contact = _mapperContactEntity.MappingDTO(entity.Contact);
            model.ComunicationType = entity.ComunicationType;
            model.Incremental = entity.Incremental;
            model.IdResolution = entity.Id.IdResolution;
            model.RegistrationUser = entity.RegistrationUser;
            model.RegistrationDate = entity.RegistrationDate;
            model.LastChangedDate = entity.LastChangedDate;
            model.LastChangedUser = entity.LastChangedUser;
            model.UniqueId = entity.UniqueId;

            return model;
        }

        #endregion
    }
}
