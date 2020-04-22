using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Fascicles
{
    public class FascicleTableValuedModelMapper : BaseModelMapper<FascicleTableValuedModel, FascicleModel>, IFascicleTableValuedModelMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        public FascicleTableValuedModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        public override FascicleModel Map(FascicleTableValuedModel entity, FascicleModel entityTransformed)
        {
            entityTransformed.Category = _mapperUnitOfWork.Repository<IDomainMapper<ICategoryTableValuedModel, CategoryModel>>().Map(entity, null);
            entityTransformed.Conservation = entity.Conservation;
            entityTransformed.EndDate = entity.EndDate;
            entityTransformed.FascicleObject = entity.FascicleObject;
            entityTransformed.FascicleType = entity.FascicleType;
            entityTransformed.VisibilityType = entity.VisibilityType;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.Manager = entity.Manager;
            entityTransformed.Name = entity.Name;
            entityTransformed.Note = entity.Note;
            entityTransformed.Number = entity.Number;
            entityTransformed.Rack = entity.Rack;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.StartDate = entity.StartDate;
            entityTransformed.Title = entity.Title;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Year = entity.Year;
            return entityTransformed;
        }

        public override ICollection<FascicleModel> MapCollection(ICollection<FascicleTableValuedModel> model)
        {
            if (model == null)
            {
                return new List<FascicleModel>();
            }

            List<FascicleModel> modelsTransformed = new List<FascicleModel>();
            FascicleModel modelTransformed = null;
            ICollection<FascicleTableValuedModel> fascicleContacts;
            foreach (IGrouping<Guid, FascicleTableValuedModel> fascicleLookup in model.ToLookup(x => x.UniqueId))
            {
                modelTransformed = Map(fascicleLookup.First(), new FascicleModel());
                fascicleContacts = fascicleLookup.ToLookup(x => x.Contact_Incremental)
                    .Select(f => f.First())
                    .ToList();

                modelTransformed.Contacts = _mapperUnitOfWork.Repository<IDomainMapper<IContactTableValuedModel, ContactModel>>().MapCollection(fascicleContacts);
                modelTransformed.Manager = modelTransformed.Contacts.FirstOrDefault()?.Description;
                modelsTransformed.Add(modelTransformed);
            }
            return modelsTransformed;
        }
    }
}



