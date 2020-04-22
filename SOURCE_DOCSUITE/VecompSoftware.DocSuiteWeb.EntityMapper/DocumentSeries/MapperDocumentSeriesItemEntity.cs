using System;
using DSW = VecompSoftware.DocSuiteWeb.Data;
using APISeriesItem = VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using NHibernate;
using VecompSoftware.DocSuiteWeb.EntityMapper.Commons;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.DocumentSeries
{
    public class MapperDocumentSeriesItemEntity : BaseEntityMapper<DSW.DocumentSeriesItem, APISeriesItem.DocumentSeriesItem>
    {
        #region [ Fields ]
        private readonly MapperCategoryEntity _mapperCategoryEntity;
        private readonly MapperDocumentSeriesEntity _mapperDocumentSeriesEntity;
        private readonly MapperDocumentSeriesItemRoleEntity _mapperDocumentSeriesItemRoleEntity;
        #endregion

        #region [ Constructor ]
        public MapperDocumentSeriesItemEntity()
        {
            _mapperCategoryEntity = new MapperCategoryEntity();
            _mapperDocumentSeriesEntity = new MapperDocumentSeriesEntity();
            _mapperDocumentSeriesItemRoleEntity = new MapperDocumentSeriesItemRoleEntity();
        }
        #endregion

        #region [ Methods ]
        public static APISeriesItem.DocumentSeriesItemStatus StatusConverter(DSW.DocumentSeriesItemStatus status)
        {
            switch (status)
            {
                case DSW.DocumentSeriesItemStatus.Active:
                    return APISeriesItem.DocumentSeriesItemStatus.Active;
                case DSW.DocumentSeriesItemStatus.Canceled:
                    return APISeriesItem.DocumentSeriesItemStatus.Canceled;
                case DSW.DocumentSeriesItemStatus.Draft:
                    return APISeriesItem.DocumentSeriesItemStatus.Draft;
                default:
                    return APISeriesItem.DocumentSeriesItemStatus.NotActive;
            }
        }

        protected override IQueryOver<DSW.DocumentSeriesItem, DSW.DocumentSeriesItem> MappingProjection(IQueryOver<DSW.DocumentSeriesItem, DSW.DocumentSeriesItem> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override APISeriesItem.DocumentSeriesItem TransformDTO(DSW.DocumentSeriesItem entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare DocumentSeriesItem se l'entità non è inizializzata");

            APISeriesItem.DocumentSeriesItem apiDocumentSeriesItem = new APISeriesItem.DocumentSeriesItem();
            apiDocumentSeriesItem.EntityId = entity.Id;
            apiDocumentSeriesItem.UniqueId = entity.UniqueId;
            apiDocumentSeriesItem.Year = entity.Year;
            apiDocumentSeriesItem.Number = entity.Number;
            apiDocumentSeriesItem.IdMain = entity.IdMain;
            apiDocumentSeriesItem.IdAnnexed = entity.IdAnnexed;
            apiDocumentSeriesItem.PublishingDate = entity.PublishingDate;
            apiDocumentSeriesItem.RetireDate = entity.RetireDate;
            apiDocumentSeriesItem.Subject = entity.Subject;
            apiDocumentSeriesItem.Status = StatusConverter(entity.Status);
            apiDocumentSeriesItem.IdUnpublishedAnnexed = entity.IdUnpublishedAnnexed;
            apiDocumentSeriesItem.DematerialisationChainId = entity.DematerialisationChainId;
            apiDocumentSeriesItem.Priority = entity.Priority;
            apiDocumentSeriesItem.RegistrationDate = entity.RegistrationDate;
            apiDocumentSeriesItem.RegistrationUser = entity.RegistrationUser;
            apiDocumentSeriesItem.LastChangedDate = entity.LastChangedDate;
            apiDocumentSeriesItem.LastChangedUser = entity.LastChangedUser;
            apiDocumentSeriesItem.Category = entity.SubCategory == null ? _mapperCategoryEntity.MappingDTO(entity.Category) : _mapperCategoryEntity.MappingDTO(entity.SubCategory);
            apiDocumentSeriesItem.DocumentSeries = _mapperDocumentSeriesEntity.MappingDTO(entity.DocumentSeries);
            apiDocumentSeriesItem.DocumentSeriesItemRoles = _mapperDocumentSeriesItemRoleEntity.MappingDTO(entity.DocumentSeriesItemRoles);

            return apiDocumentSeriesItem;
        }
        #endregion
    }
}
