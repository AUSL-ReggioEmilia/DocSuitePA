using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftwareFascicle = VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Fascicles
{
    public class FascicleFolderModelMapper : BaseModelMapper<FascicleFolder, VecompSoftwareFascicle.FascicleFolderModel>, IFascicleFolderModelMapper
    {
        #region [ Fields ]
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        #endregion

        #region [ Constructor ]
        public FascicleFolderModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }
        #endregion

        public override VecompSoftwareFascicle.FascicleFolderModel Map(FascicleFolder entity, VecompSoftwareFascicle.FascicleFolderModel modelTransformed)
        {
            modelTransformed.UniqueId = entity.UniqueId;
            modelTransformed.Name = entity.Name;
            modelTransformed.Typology = (VecompSoftwareFascicle.FascicleFolderTypology)entity.Typology;
            modelTransformed.Status = (VecompSoftwareFascicle.FascicleFolderStatus)entity.Status;

            return modelTransformed;
        }
    }
}
