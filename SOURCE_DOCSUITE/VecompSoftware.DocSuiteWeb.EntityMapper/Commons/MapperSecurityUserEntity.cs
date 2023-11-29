using System;
using DSW = VecompSoftware.DocSuiteWeb.Data;
using APICommon = VecompSoftware.DocSuiteWeb.Entity.Commons;
using NHibernate;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Commons
{
    public class MapperSecurityUserEntity : BaseEntityMapper<DSW.SecurityUsers, APICommon.SecurityUser>
    {
        #region [ Fields ]
        private readonly MapperSecurityGroupEntity _mapperSecurityGroup;
        #endregion

        #region [ Contructor ]        
        public MapperSecurityUserEntity()
        {
            _mapperSecurityGroup = new MapperSecurityGroupEntity();
        }
        #endregion

        #region [ Methods ]        
        protected override IQueryOver<DSW.SecurityUsers, DSW.SecurityUsers> MappingProjection(IQueryOver<DSW.SecurityUsers, DSW.SecurityUsers> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override APICommon.SecurityUser TransformDTO(DSW.SecurityUsers entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare SecurityUser se l'entità non è inizializzata");

            APICommon.SecurityUser securityUser = new APICommon.SecurityUser();
            securityUser.EntityId = entity.Id;
            securityUser.UniqueId = entity.UniqueId;
            securityUser.UserDomain = entity.UserDomain;
            securityUser.Account = entity.Account;
            securityUser.Description = entity.Description;
            securityUser.RegistrationDate = entity.RegistrationDate;
            securityUser.RegistrationUser = entity.RegistrationUser;
            securityUser.LastChangedDate = entity.LastChangedDate;
            securityUser.LastChangedUser = entity.LastChangedUser;
            securityUser.Group = _mapperSecurityGroup.MappingDTO(entity.Group);

            return securityUser;
        }
        #endregion
    }
}
