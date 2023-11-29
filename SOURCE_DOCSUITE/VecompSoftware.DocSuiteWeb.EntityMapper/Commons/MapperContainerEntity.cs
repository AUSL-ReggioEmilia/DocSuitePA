using System;
using DSW = VecompSoftware.DocSuiteWeb.Data;
using APICommon = VecompSoftware.DocSuiteWeb.Entity.Commons;
using NHibernate;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Commons
{
    public class MapperContainerEntity : BaseEntityMapper<DSW.Container, APICommon.Container>
    {
        private readonly MapperLocationEntity _mapperLocationEntity;

        #region [ Constructor ]
        public MapperContainerEntity()
        {
            _mapperLocationEntity = new MapperLocationEntity();
        }
        #endregion

        #region [ Methods ]
        protected override IQueryOver<DSW.Container, DSW.Container> MappingProjection(IQueryOver<DSW.Container, DSW.Container> queryOver)
        {
            throw new NotImplementedException();
        }

        private APICommon.Location TryMappingLocation(DSW.Location location)
        {
            APICommon.Location entity = null;
            try
            {
                entity = _mapperLocationEntity.MappingDTO(location);
            }
            catch (Exception) { }
            return entity;
        }

        protected override APICommon.Container TransformDTO(DSW.Container entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("Impossibile trasformare Container se l'entità non è inizializzata");
            }

            APICommon.Container apiContainer = new APICommon.Container();
            apiContainer.UniqueId = entity.UniqueId;
            apiContainer.EntityShortId = Convert.ToInt16(entity.Id);
            apiContainer.Name = entity.Name;
            apiContainer.isActive = entity.IsActive;
            apiContainer.Note = entity.Note;
            apiContainer.HeadingFrontalino = entity.HeadingFrontalino;
            apiContainer.HeadingLetter = entity.HeadingLetter;
            apiContainer.RegistrationDate = entity.RegistrationDate;
            apiContainer.RegistrationUser = entity.RegistrationUser;
            apiContainer.ReslLocation = TryMappingLocation(entity.ReslLocation);
            apiContainer.ProtLocation = TryMappingLocation(entity.ProtLocation);
            apiContainer.DocumentSeriesLocation = TryMappingLocation(entity.DocumentSeriesLocation);
            apiContainer.DocumentSeriesAnnexedLocation = TryMappingLocation(entity.DocumentSeriesAnnexedLocation);
            apiContainer.DocumentSeriesUnpublishedAnnexedLocation = TryMappingLocation(entity.DocumentSeriesUnpublishedAnnexedLocation); 

            return apiContainer;
        }
        #endregion
    }
}
