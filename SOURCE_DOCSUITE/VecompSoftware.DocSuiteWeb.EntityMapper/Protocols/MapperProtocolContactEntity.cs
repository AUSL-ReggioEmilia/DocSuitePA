using System;
using DSW = VecompSoftware.DocSuiteWeb.Data;
using APIProtocol = VecompSoftware.DocSuiteWeb.Entity.Protocols;
using NHibernate;
using VecompSoftware.DocSuiteWeb.EntityMapper.Commons;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Protocols
{
    public class MapperProtocolContactEntity : BaseEntityMapper<DSW.ProtocolContact, APIProtocol.ProtocolContact>
    {
        #region [ Fields ]
        private MapperContactEntity _mapperContactEntity;
        #endregion

        #region [Constructor]
        public MapperProtocolContactEntity()
        {
            _mapperContactEntity = new MapperContactEntity();
        }
        #endregion

        protected override IQueryOver<DSW.ProtocolContact, DSW.ProtocolContact> MappingProjection(IQueryOver<DSW.ProtocolContact, DSW.ProtocolContact> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override APIProtocol.ProtocolContact TransformDTO(DSW.ProtocolContact entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare ProtocolContact se l'entità non è inizializzata");

            APIProtocol.ProtocolContact apiProtocolContact = new APIProtocol.ProtocolContact
            {
                Contact = _mapperContactEntity.MappingDTO(entity.Contact),
                Protocol = new APIProtocol.Protocol()
            };
            apiProtocolContact.UniqueId = entity.Id;
            apiProtocolContact.Protocol.Year = entity.Year;
            apiProtocolContact.Protocol.Number = entity.Number;
            apiProtocolContact.RegistrationUser = entity.RegistrationUser;
            apiProtocolContact.RegistrationDate = entity.RegistrationDate;
            apiProtocolContact.ComunicationType = entity.ComunicationType;
            apiProtocolContact.Type = entity.Type;
            apiProtocolContact.LastChangedDate = entity.LastChangedDate;
            apiProtocolContact.LastChangedUser = entity.LastChangedUser;

            return apiProtocolContact;
        }
    }
}
