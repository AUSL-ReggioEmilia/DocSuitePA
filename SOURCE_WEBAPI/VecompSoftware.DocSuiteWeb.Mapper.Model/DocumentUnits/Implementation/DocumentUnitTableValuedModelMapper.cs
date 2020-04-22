using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.DocumentUnits
{
    public class DocumentUnitTableValuedModelMapper : BaseModelMapper<DocumentUnitTableValuedModel, DocumentUnitModel>, IDocumentUnitTableValuedModelMapper
    {

        private readonly IMapperUnitOfWork _mapperUnitOfWork;

        public DocumentUnitTableValuedModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        public override DocumentUnitModel Map(DocumentUnitTableValuedModel model, DocumentUnitModel modelTransformed)
        {
            modelTransformed.UniqueId = model.UniqueId;
            modelTransformed.EntityId = model.EntityId;
            modelTransformed.Environment = model.Environment;
            modelTransformed.DocumentUnitName = model.DocumentUnitName;
            modelTransformed.Year = model.Year;
            modelTransformed.Number = model.Number;
            modelTransformed.Title = model.Title;
            modelTransformed.ReferenceType = model.ReferenceType;
            modelTransformed.RegistrationDate = model.RegistrationDate;
            modelTransformed.RegistrationUser = model.RegistrationUser;
            modelTransformed.Subject = model.Subject;
            modelTransformed.IdUDSRepository = model.IdUDSRepository;
            modelTransformed.IdFascicle = model.IdFascicle;
            modelTransformed.IsFascicolable = model.IsFascicolable;
            modelTransformed.MainDocumentName = model.DocumentUnitChain_DocumentName;

            modelTransformed.Category = _mapperUnitOfWork.Repository<IDomainMapper<ICategoryTableValuedModel, CategoryModel>>().Map(model, null);
            modelTransformed.Container = _mapperUnitOfWork.Repository<IDomainMapper<IContainerTableValuedModel, ContainerModel>>().Map(model, null);
            modelTransformed.DocumentUnitChain = _mapperUnitOfWork.Repository<IDomainMapper<DocumentUnitTableValuedModel, DocumentUnitChainModel>>().Map(model, null);

            return modelTransformed;
        }

        public override ICollection<DocumentUnitModel> MapCollection(ICollection<DocumentUnitTableValuedModel> entities)
        {
            if (entities == null)
            {
                return new List<DocumentUnitModel>();
            }

            List<DocumentUnitModel> entitiesTransformed = new List<DocumentUnitModel>();
            DocumentUnitModel entityTransformed = null;
            foreach (IGrouping<Guid, DocumentUnitTableValuedModel> udLookup in entities.ToLookup(x => x.UniqueId))
            {
                entityTransformed = Map(udLookup.First(), new DocumentUnitModel());
                entitiesTransformed.Add(entityTransformed);
            }

            return entitiesTransformed;
        }

    }
}
