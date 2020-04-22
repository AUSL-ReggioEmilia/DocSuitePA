using NHibernate;
using System;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.DTO.Commons;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Commons
{
    public class MapperCategoryUserProcedure : BaseEntityMapper<Role, CategoryUserProcedure>
    {
        #region [ Constructor ]
        public MapperCategoryUserProcedure() : base() { }
        #endregion

        #region [ Methods ]

        protected override CategoryUserProcedure TransformDTO(Role entity)
        {
            throw new ArgumentException("Impossibile trasformare CategoryUserProcedure");
        }

        /// <summary>
        /// Mappo gli oggetti di DeskDocumentEndorsement su DeskEndorsement.
        /// <see cref="IsApprove">IsApprove viene settato intero poichè in una PivotGrid ho la necessità di aggregare questa informazione</see>
        /// </summary>
        /// <param name="queryOver"></param>
        /// <returns></returns>
        protected override IQueryOver<Role, Role> MappingProjection(IQueryOver<Role, Role> queryOver)
        {
            CategoryUserProcedure categoryUserProcedure = null;
            RoleUser roleUser = null;

            queryOver
                .SelectList(list => list
                .SelectGroup(s => s.Name).WithAlias(() => categoryUserProcedure.RoleName)
                .SelectGroup(() => roleUser.Description).WithAlias(() => categoryUserProcedure.ProcedureUserName));

            return queryOver;
        }

        #endregion        
    }
}
