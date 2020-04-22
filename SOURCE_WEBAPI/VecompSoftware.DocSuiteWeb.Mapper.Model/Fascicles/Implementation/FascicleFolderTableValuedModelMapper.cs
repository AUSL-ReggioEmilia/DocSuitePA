using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Fascicles
{
    public class FascicleFolderTableValuedModelMapper : BaseModelMapper<FascicleFolderTableValuedModel, FascicleFolderModel>, IFascicleFolderTableValuedModelMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        public FascicleFolderTableValuedModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        public override FascicleFolderModel Map(FascicleFolderTableValuedModel model, FascicleFolderModel modelTransformed)
        {
            modelTransformed.UniqueId = model.IdFascicleFolder;
            modelTransformed.Name = model.Name;
            modelTransformed.Status = model.Status;
            modelTransformed.Typology = model.Typology;
            modelTransformed.IdFascicle = model.Fascicle_IdFascicle;
            modelTransformed.IdCategory = model.Category_IdCategory;
            modelTransformed.HasChildren = model.HasChildren;
            modelTransformed.HasDocuments = model.HasDocuments;
            modelTransformed.FascicleFolderLevel = model.FascicleFolderLevel;

            return modelTransformed;
        }

        public override ICollection<FascicleFolderModel> MapCollection(ICollection<FascicleFolderTableValuedModel> model)
        {
            if (model == null)
            {
                return new List<FascicleFolderModel>();
            }

            List<FascicleFolderModel> modelsTransformed = new List<FascicleFolderModel>();
            FascicleFolderModel modelTransformed = null;
            foreach (IGrouping<Guid, FascicleFolderTableValuedModel> fascicleFolderLookup in model.ToLookup(x => x.IdFascicleFolder))
            {
                modelTransformed = Map(fascicleFolderLookup.First(), new FascicleFolderModel());
                modelsTransformed.Add(modelTransformed);
            }
            return modelsTransformed;
        }
    }
}
