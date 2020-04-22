using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Processes;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Processes
{
    public class ProcessTableValuedModelMapper : BaseModelMapper<ProcessTableValuedModel, ProcessModel>, IProcessTableValuedModelMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;

        public ProcessTableValuedModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        public override ProcessModel Map(ProcessTableValuedModel entity, ProcessModel entityTransformed)
        {
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.StartDate = entity.StartDate;
            entityTransformed.EndDate = entity.EndDate;
            entityTransformed.Note = entity.Note;
            entityTransformed.Name = entity.Name;
            entityTransformed.FascicleType = entity.FascicleType;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.ProcessType = entity.ProcessType;
            
            entityTransformed.Category = new CategoryModel()
            {
                Code = entity.Category_Code,
                Name = entity.Category_Name,
                IdCategory = entity.Category_IdCategory
            };

            return entityTransformed;
        }
    }
}
