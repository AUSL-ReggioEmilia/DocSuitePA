using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Model.Entities.Monitors;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Monitors
{
    public class TransparentAdministrationMonitorLogTableValuedModelMapper : BaseModelMapper<TransparentAdministrationMonitorLogTableValuedModel, TransparentAdministrationMonitorLogModel>, ITransparentAdministrationMonitorLogTableValuedModelMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        public TransparentAdministrationMonitorLogTableValuedModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }
        public override TransparentAdministrationMonitorLogModel Map(TransparentAdministrationMonitorLogTableValuedModel model, TransparentAdministrationMonitorLogModel modelTransformed)
        {
            modelTransformed.Date = model.Date;
            modelTransformed.UniqueId = model.UniqueId;
            modelTransformed.Note = model.Note;
            modelTransformed.RegistrationUser = model.RegistrationUser;
            modelTransformed.RegistrationDate = model.RegistrationDate;
            modelTransformed.Rating = model.Rating;

            return modelTransformed;
        }

        public override ICollection<TransparentAdministrationMonitorLogModel> MapCollection(ICollection<TransparentAdministrationMonitorLogTableValuedModel> model)
        {
            if (model == null)
            {
                return new List<TransparentAdministrationMonitorLogModel>();
            }
            List<TransparentAdministrationMonitorLogModel> modelsTransformed = new List<TransparentAdministrationMonitorLogModel>();
            TransparentAdministrationMonitorLogModel modelTransformed = null;
            foreach (IGrouping<int, TransparentAdministrationMonitorLogTableValuedModel> transparentAdministrationMonitorLookup in model.ToLookup(x => x.UniqueId))
            {
                modelTransformed = Map(transparentAdministrationMonitorLookup.First(), new TransparentAdministrationMonitorLogModel());
                modelsTransformed.Add(modelTransformed);
            }
            return modelsTransformed;
        }
    }
}
