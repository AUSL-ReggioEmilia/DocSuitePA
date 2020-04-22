using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftwareDossier = VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Dossiers
{
    public class DossierFolderModelMapper : BaseModelMapper<DossierFolder, VecompSoftwareDossier.DossierFolderModel>, IDossierFolderModelMapper
    {
        #region [ Fields ]
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        #endregion

        #region [ Constructor ]
        public DossierFolderModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }
        #endregion

        public override VecompSoftwareDossier.DossierFolderModel Map(DossierFolder entity, VecompSoftwareDossier.DossierFolderModel modelTransformed)
        {
            modelTransformed.UniqueId = entity.UniqueId;
            modelTransformed.Name = entity.Name;
            modelTransformed.JsonMetadata = entity.JsonMetadata;
            modelTransformed.Status = (VecompSoftwareDossier.DossierFolderStatus)entity.Status;

            return modelTransformed;
        }
    }
}
