using System;
using DSW = VecompSoftware.DocSuiteWeb.Data;
using APIProtocol = VecompSoftware.DocSuiteWeb.Entity.Protocols;
using NHibernate;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Protocols
{
    public class MapperProtocolUserEntity : BaseEntityMapper<DSW.ProtocolUser, APIProtocol.ProtocolUser>
    {
        #region [ Fields ]
        #endregion

        #region [Constructor]
        public MapperProtocolUserEntity()
        {
        }
        #endregion

        protected override IQueryOver<DSW.ProtocolUser, DSW.ProtocolUser> MappingProjection(IQueryOver<DSW.ProtocolUser, DSW.ProtocolUser> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override APIProtocol.ProtocolUser TransformDTO(DSW.ProtocolUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("Impossibile trasformare ProtocolRoleUser se l'entità non è inizializzata");
            }
            APIProtocol.ProtocolUser apiProtocolUser = new APIProtocol.ProtocolUser();
            apiProtocolUser.Protocol = new APIProtocol.Protocol();
            apiProtocolUser.Protocol.Year = entity.Year;
            apiProtocolUser.Protocol.Number = entity.Number;
            apiProtocolUser.RegistrationUser = entity.RegistrationUser;
            apiProtocolUser.RegistrationDate = entity.RegistrationDate;
            apiProtocolUser.Account = entity.Account;
            apiProtocolUser.Note = entity.Note;

            return apiProtocolUser;
        }
    }
}
