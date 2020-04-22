using System;
using System.Collections.Generic;
using System.Linq;
using DSW = VecompSoftware.DocSuiteWeb.Data;
using APICommon = VecompSoftware.DocSuiteWeb.Entity.Commons;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Data;
using NHibernate;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Commons
{
    public class MapperRoleUserEntity:BaseEntityMapper<DSW.RoleUser, APICommon.RoleUser>
    {
        #region [ Contructor ]

        #endregion
        public MapperRoleUserEntity()
        {

        }

        #region [ Methods ]

        #endregion
        protected override IQueryOver<DSW.RoleUser, DSW.RoleUser> MappingProjection(IQueryOver<DSW.RoleUser, DSW.RoleUser> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override APICommon.RoleUser TransformDTO(DSW.RoleUser entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare RoleUser se l'entità non è inizializzata");

            APICommon.RoleUser roleUser = new APICommon.RoleUser();
            roleUser.UniqueId = entity.UniqueId;
            roleUser.EntityShortId = Convert.ToInt16(entity.Id);
            roleUser.Account = entity.Account;
            roleUser.Type = entity.Type;
            roleUser.Email = entity.Email;
            roleUser.Enabled = entity.Enabled;
            roleUser.IsMainRole = entity.IsMainRole;

            return roleUser;
        }
    }
}
