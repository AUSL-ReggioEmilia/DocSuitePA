using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Dossiers
{
    public class DossierFolderTableValuedModelMapper : BaseModelMapper<DossierFolderTableValuedModel, DossierFolderModel>, IDossierFolderTableValuedModelMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        public DossierFolderTableValuedModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        public override DossierFolderModel Map(DossierFolderTableValuedModel model, DossierFolderModel modelTransformed)
        {
            modelTransformed.UniqueId = model.IdDossierFolder;
            modelTransformed.Name = model.Name;
            modelTransformed.Status = model.Status;
            modelTransformed.JsonMetadata = model.JsonMetadata;
            modelTransformed.IdDossier = model.Dossier_IdDossier;
            modelTransformed.IdFascicle = model.Fascicle_IdFascicle;
            modelTransformed.IdCategory = model.Category_IdCategory;
            modelTransformed.IdRole = model.Role_IdRole;
            modelTransformed.DossierFolderLevel = model.DossierFolderLevel;
            modelTransformed.DossierFolderPath = model.DossierFolderPath;

            return modelTransformed;
        }

        public override ICollection<DossierFolderModel> MapCollection(ICollection<DossierFolderTableValuedModel> model)
        {
            if (model == null)
            {
                return new List<DossierFolderModel>();
            }

            List<DossierFolderModel> modelsTransformed = new List<DossierFolderModel>();
            DossierFolderModel modelTransformed = null;
            foreach (IGrouping<Guid, DossierFolderTableValuedModel> dossierFolderLookup in model.ToLookup(x => x.IdDossierFolder))
            {
                modelTransformed = Map(dossierFolderLookup.First(), new DossierFolderModel());
                modelsTransformed.Add(modelTransformed);
            }
            return modelsTransformed;
        }
    }
}
