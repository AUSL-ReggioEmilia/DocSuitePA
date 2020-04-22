using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.Processes;
using VecompSoftwareFascicle = VecompSoftware.DocSuiteWeb.Model.Entities.Processes;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Processes
{
    public class ProcessModelMapper : BaseModelMapper<Process, ProcessModel>, IProcessModelMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;

        public ProcessModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        #region [ Methods ]

        public override ProcessModel Map(Process model, ProcessModel modelTransformed)
        {
            modelTransformed.UniqueId = model.UniqueId;
            modelTransformed.StartDate = model.StartDate;
            modelTransformed.EndDate = model.EndDate;
            modelTransformed.Note = model.Note;
            modelTransformed.Name = model.Name;
            modelTransformed.FascicleType = (FascicleType)model.FascicleType;
            modelTransformed.RegistrationDate = model.RegistrationDate;
            modelTransformed.RegistrationUser = model.RegistrationUser;
            modelTransformed.ProcessType = (VecompSoftwareFascicle.ProcessType)model.ProcessType;

            return modelTransformed;
        }

        #endregion
    }
}
