using System;
using DSW = VecompSoftware.DocSuiteWeb.Data;
using APIProtocol = VecompSoftware.DocSuiteWeb.Entity.Protocols;
using NHibernate;
using VecompSoftware.DocSuiteWeb.EntityMapper.Commons;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Protocols
{
    public class MapperProtocolRoleEntity : BaseEntityMapper<DSW.ProtocolRole, APIProtocol.ProtocolRole>
    {
        #region [ Fields ]
        private MapperRoleEntity _mapperRoleEntity;
        #endregion

        #region [Constructor]
        public MapperProtocolRoleEntity()
        {
            _mapperRoleEntity = new MapperRoleEntity();
        }
        #endregion

        protected override IQueryOver<DSW.ProtocolRole, DSW.ProtocolRole> MappingProjection(IQueryOver<DSW.ProtocolRole, DSW.ProtocolRole> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override APIProtocol.ProtocolRole TransformDTO(DSW.ProtocolRole entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare ProtocolRole se l'entità non è inizializzata");

            APIProtocol.ProtocolRole apiProtocolRole = new APIProtocol.ProtocolRole();
            apiProtocolRole.Role = _mapperRoleEntity.MappingDTO(entity.Role);
            apiProtocolRole.Protocol = new APIProtocol.Protocol();
            apiProtocolRole.Protocol.Year = entity.Year;
            apiProtocolRole.Protocol.Number = entity.Number;
            apiProtocolRole.RegistrationUser = entity.RegistrationUser;
            apiProtocolRole.RegistrationDate = entity.RegistrationDate;

            apiProtocolRole.Rights = entity.Rights;
            apiProtocolRole.Note = entity.Note;
            apiProtocolRole.Type = entity.Type;
            apiProtocolRole.DistributionType = entity.DistributionType;

            return apiProtocolRole;
        }
    }
}
