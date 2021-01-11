using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Entities.Tenants;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Protocols
{
    public class ProtocolTableValuedModelMapper : BaseModelMapper<ProtocolTableValuedModel, ProtocolModel>, IProtocolTableValuedModelMapper
    {

        private readonly IMapperUnitOfWork _mapperUnitOfWork;

        public ProtocolTableValuedModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        public override ProtocolModel Map(ProtocolTableValuedModel model, ProtocolModel modelTransformed)
        {
            modelTransformed.Year = model.Year;
            modelTransformed.Number = model.Number;
            modelTransformed.UniqueId = model.UniqueId;
            modelTransformed.DocumentCode = model.DocumentCode;
            modelTransformed.IdDocument = model.IdDocument;
            modelTransformed.IdStatus = model.IdStatus;
            modelTransformed.RegistrationDate = model.RegistrationDate;
            modelTransformed.Object = model.Object;
            modelTransformed.DocumentProtocol = model.DocumentProtocol;
            modelTransformed.DocumentDate = model.DocumentDate;
            modelTransformed.Note = model.Note;

            modelTransformed.Category = _mapperUnitOfWork.Repository<IDomainMapper<ICategoryTableValuedModel, CategoryModel>>().Map(model, null);
            modelTransformed.Container = _mapperUnitOfWork.Repository<IDomainMapper<IContainerTableValuedModel, ContainerModel>>().Map(model, null);
            modelTransformed.ProtocolType = _mapperUnitOfWork.Repository<IDomainMapper<ProtocolTableValuedModel, ProtocolTypeModel>>().Map(model, null);
            modelTransformed.TenantAOO = _mapperUnitOfWork.Repository<IDomainMapper<ITenantAOOTableValuedModel, TenantAOOModel>>().Map(model, null);
            return modelTransformed;
        }

        public override ICollection<ProtocolModel> MapCollection(ICollection<ProtocolTableValuedModel> model)
        {
            if (model == null)
            {
                return new List<ProtocolModel>();
            }

            List<ProtocolModel> modelsTransformed = new List<ProtocolModel>();
            ICollection<ProtocolTableValuedModel> protocolContacts;
            ICollection<ProtocolTableValuedModel> protocolContactManuals;
            ProtocolModel modelTransformed = null;
            foreach (IGrouping<Guid, ProtocolTableValuedModel> protocolLookup in model.ToLookup(x => x.UniqueId))
            {
                modelTransformed = Map(protocolLookup.First(), new ProtocolModel());

                protocolContacts = protocolLookup.ToLookup(x => x.ProtocolContact_IDContact)
                    .Select(f => f.First())
                    .ToList();

                protocolContactManuals = protocolLookup.ToLookup(x => x.ProtocolContactManual_Incremental)
                    .Select(f => f.First())
                    .ToList();

                modelTransformed.Contacts = _mapperUnitOfWork.Repository<IDomainMapper<ProtocolTableValuedModel, ProtocolContactModel>>().MapCollection(protocolContacts);
                modelTransformed.ContactManuals = _mapperUnitOfWork.Repository<IDomainMapper<ProtocolTableValuedModel, ProtocolContactManualModel>>().MapCollection(protocolContactManuals);
                modelsTransformed.Add(modelTransformed);
            }

            return modelsTransformed;
        }

    }
}
