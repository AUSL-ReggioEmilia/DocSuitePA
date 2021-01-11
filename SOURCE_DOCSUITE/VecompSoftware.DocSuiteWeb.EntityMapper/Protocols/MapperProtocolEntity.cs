using System;
using DSW = VecompSoftware.DocSuiteWeb.Data;
using APIProtocol = VecompSoftware.DocSuiteWeb.Entity.Protocols;
using NHibernate;
using VecompSoftware.DocSuiteWeb.EntityMapper.Commons;
using System.Linq;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Protocols
{
    public class MapperProtocolEntity : BaseEntityMapper<DSW.Protocol, APIProtocol.Protocol>
    {
        #region [ Fields ]
        private readonly MapperContainerEntity _mapperContainer;
        private readonly MapperCategoryEntity _mapperCategory;
        private readonly MapperProtocolRoleEntity _mapperProtocolRoleEntity;
        private readonly MapperProtocolUserEntity _mapperProtocolUserEntity;
        #endregion

        #region [ Constructor ]
        public MapperProtocolEntity()
        {
            _mapperCategory = new MapperCategoryEntity();
            _mapperContainer = new MapperContainerEntity();
            _mapperProtocolRoleEntity = new MapperProtocolRoleEntity();
            _mapperProtocolUserEntity = new MapperProtocolUserEntity();
        }
        #endregion

        #region [ Methods ]
        protected override IQueryOver<DSW.Protocol, DSW.Protocol> MappingProjection(IQueryOver<DSW.Protocol, DSW.Protocol> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override APIProtocol.Protocol TransformDTO(DSW.Protocol entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare Protocol se l'entità non è inizializzata");

            APIProtocol.Protocol apiProtocol = new APIProtocol.Protocol();
            apiProtocol.UniqueId = entity.Id;
            apiProtocol.Year = entity.Year;
            apiProtocol.Number = entity.Number;
            apiProtocol.Object = entity.ProtocolObject;
            apiProtocol.RegistrationDate = entity.RegistrationDate;
            apiProtocol.RegistrationUser = entity.RegistrationUser;
            apiProtocol.LastChangedDate = entity.LastChangedDate;
            apiProtocol.LastChangedUser = entity.LastChangedUser;
            apiProtocol.ObjectChangeReason = entity.ObjectChangeReason;
            apiProtocol.DocumentDate = entity.DocumentDate;
            apiProtocol.DocumentProtocol = entity.DocumentProtocol;
            apiProtocol.IdDocument = entity.IdDocument;
            apiProtocol.IdAttachments = entity.IdAttachments;
            apiProtocol.DocumentCode = entity.DocumentCode;
            apiProtocol.IdStatus = Convert.ToInt16(entity.IdStatus);
            apiProtocol.LastChangedReason = entity.LastChangedReason;
            apiProtocol.AlternativeRecipient = entity.AlternativeRecipient;
            apiProtocol.JournalDate = entity.JournalDate;
            apiProtocol.LastConservationDate = entity.LastConservationDate;
            apiProtocol.HasConservatedDocs = entity.HasConservatedDocs;
            apiProtocol.IdAnnexed = entity.IdAnnexed != Guid.Empty ? entity.IdAnnexed : (Guid?)null;
            apiProtocol.HandlerDate = entity.HandlerDate;
            apiProtocol.TDError = entity.TDError;
            apiProtocol.IdProtocolKind = entity.IdProtocolKind;
            apiProtocol.DematerialisationChainId = entity.DematerialisationChainId;
            apiProtocol.Category = _mapperCategory.MappingDTO(entity.Category);
            apiProtocol.Container = _mapperContainer.MappingDTO(entity.Container);
            apiProtocol.ProtocolRoles = entity.Roles.Select(s => _mapperProtocolRoleEntity.MappingDTO(s)).ToList();
            apiProtocol.ProtocolUsers = entity.Users.Select(u => _mapperProtocolUserEntity.MappingDTO(u)).ToList();
            return apiProtocol;
        }
        #endregion
    }
}
