using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSW = VecompSoftware.DocSuiteWeb.Data;
using APISeriesItem = VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using NHibernate;
using VecompSoftware.DocSuiteWeb.EntityMapper.Commons;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.DocumentSeries
{
    public class MapperDocumentSeriesItemRoleEntity : BaseEntityMapper<DSW.DocumentSeriesItemRole, APISeriesItem.DocumentSeriesItemRole>
    {
        #region [ Fields ]
        private readonly MapperRoleEntity _mapperRoleEntity;
        #endregion

        #region [ Constructor ]
        public MapperDocumentSeriesItemRoleEntity()
        {
            _mapperRoleEntity = new MapperRoleEntity();
        }
        #endregion

        #region [ Methods ]
        public static APISeriesItem.DocumentSeriesItemRoleLinkType RoleLinkConverter(DSW.DocumentSeriesItemRoleLinkType status)
        {
            switch (status)
            {
                case DSW.DocumentSeriesItemRoleLinkType.Authorization:
                    return APISeriesItem.DocumentSeriesItemRoleLinkType.Authorization;
                case DSW.DocumentSeriesItemRoleLinkType.Knowledge:
                    return APISeriesItem.DocumentSeriesItemRoleLinkType.Knowledge;
                default:
                    return APISeriesItem.DocumentSeriesItemRoleLinkType.Owner;
            }
        }

        protected override IQueryOver<DSW.DocumentSeriesItemRole, DSW.DocumentSeriesItemRole> MappingProjection(IQueryOver<DSW.DocumentSeriesItemRole, DSW.DocumentSeriesItemRole> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override APISeriesItem.DocumentSeriesItemRole TransformDTO(DSW.DocumentSeriesItemRole entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare DocumentSeriesItemRole se l'entità non è inizializzata");

            APISeriesItem.DocumentSeriesItemRole apiDocumentSeriesItemRole = new APISeriesItem.DocumentSeriesItemRole();
            apiDocumentSeriesItemRole.LinkType = RoleLinkConverter(entity.LinkType);
            apiDocumentSeriesItemRole.Role = _mapperRoleEntity.MappingDTO(entity.Role);

            return apiDocumentSeriesItemRole;
        }
        #endregion
    }
}
