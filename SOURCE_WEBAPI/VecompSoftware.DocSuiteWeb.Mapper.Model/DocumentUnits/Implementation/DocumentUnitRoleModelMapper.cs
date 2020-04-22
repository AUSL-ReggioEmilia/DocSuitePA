using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.DocumentUnits
{
    public class DocumentUnitRoleModelMapper : BaseModelMapper<DocumentUnitRole, RoleModel>, IDocumentUnitRoleModelMapper
    {

        private readonly IMapperUnitOfWork _mapperUnitOfWork;

        public DocumentUnitRoleModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        public override RoleModel Map(DocumentUnitRole model, RoleModel modelTransformed)
        {
            modelTransformed.UniqueId = model.UniqueIdRole;

            return modelTransformed;
        }

        public override ICollection<RoleModel> MapCollection(ICollection<DocumentUnitRole> entities)
        {
            if (entities == null)
            {
                return new List<RoleModel>();
            }

            List<RoleModel> entitiesTransformed = new List<RoleModel>();
            RoleModel entityTransformed = null;
            foreach (IGrouping<Guid, DocumentUnitRole> udLookup in entities.ToLookup(x => x.UniqueId))
            {
                entityTransformed = Map(udLookup.First(), new RoleModel());
                entitiesTransformed.Add(entityTransformed);
            }

            return entitiesTransformed;
        }

    }
}
