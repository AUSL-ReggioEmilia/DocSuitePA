using System;
using DSW = VecompSoftware.DocSuiteWeb.Data;
using APICommon = VecompSoftware.DocSuiteWeb.Entity.Commons;
using NHibernate;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Commons
{
    public class MapperSecurityGroupEntity : BaseEntityMapper<DSW.SecurityGroups, APICommon.SecurityGroup>
    {
        #region [ Contructor ]        
        public MapperSecurityGroupEntity()
        {

        }
        #endregion

        #region [ Methods ]        
        protected override IQueryOver<DSW.SecurityGroups, DSW.SecurityGroups> MappingProjection(IQueryOver<DSW.SecurityGroups, DSW.SecurityGroups> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override APICommon.SecurityGroup TransformDTO(DSW.SecurityGroups entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare SecurityGroup se l'entità non è inizializzata");

            APICommon.SecurityGroup securityGroup = new APICommon.SecurityGroup();
            securityGroup.EntityId = entity.Id;
            securityGroup.UniqueId = entity.UniqueId;
            securityGroup.GroupName = entity.GroupName;
            securityGroup.LogDescription = entity.LogDescription;
            securityGroup.IsAllUsers = entity.HasAllUsers;
            securityGroup.RegistrationDate = entity.RegistrationDate;
            securityGroup.RegistrationUser = entity.RegistrationUser;
            securityGroup.LastChangedDate = entity.LastChangedDate;
            securityGroup.LastChangedUser = entity.LastChangedUser;

            return securityGroup;
        }
        #endregion
    }
}
