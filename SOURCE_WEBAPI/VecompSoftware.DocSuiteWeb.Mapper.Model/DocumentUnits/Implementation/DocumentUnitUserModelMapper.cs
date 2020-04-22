using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.DocumentUnits
{
    public class DocumentUnitUserModelMapper : BaseModelMapper<DocumentUnitUser, UserModel>, IDocumentUnitUserModelMapper
    {

        private readonly IMapperUnitOfWork _mapperUnitOfWork;

        public DocumentUnitUserModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }


        public override UserModel Map(DocumentUnitUser model, UserModel modelTransformed)
        {
            modelTransformed.UniqueId = model.UniqueId;
            modelTransformed.Account = model.Account;

            return modelTransformed;
        }

        public override ICollection<UserModel> MapCollection(ICollection<DocumentUnitUser> entities)
        {
            if (entities == null)
            {
                return new List<UserModel>();
            }

            List<UserModel> entitiesTransformed = new List<UserModel>();
            UserModel entityTransformed = null;
            foreach (IGrouping<Guid, DocumentUnitUser> udLookup in entities.ToLookup(x => x.UniqueId))
            {
                entityTransformed = Map(udLookup.First(), new UserModel());
                entitiesTransformed.Add(entityTransformed);
            }

            return entitiesTransformed;
        }

    }
}
