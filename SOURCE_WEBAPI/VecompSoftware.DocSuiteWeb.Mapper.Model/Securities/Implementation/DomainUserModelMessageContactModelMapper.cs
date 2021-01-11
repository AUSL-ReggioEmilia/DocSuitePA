using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Entities.Messages;
using VecompSoftware.DocSuiteWeb.Model.Securities;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Securities
{
    public class DomainUserModelMessageContactModelMapper : BaseModelMapper<DomainUserModel, MessageContactModel>, IDomainUserModelMessageContactModelMapper
    {
        private readonly IMapperUnitOfWork _mapper;
        public DomainUserModelMessageContactModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapper = mapperUnitOfWork;
        }

        public override MessageContactModel Map(DomainUserModel entity, MessageContactModel entityTransformed)
        {
            #region [ Base ]
            entityTransformed.ContactPosition = MessageContantTypology.Recipient;
            entityTransformed.ContactType = MessageContactType.User;
            entityTransformed.Description = entity.DisplayName;
            entityTransformed.MessageContactEmail = _mapper.Repository<IDomainUserModelMessageContactEmailModelMapper>().MapCollection(new List<DomainUserModel>() { entity });
            #endregion

            return entityTransformed;
        }
    }
}
