using VecompSoftware.DocSuiteWeb.Entity.Conservations;
using VecompSoftware.DocSuiteWeb.Model.Entities.Conservations;

namespace VecompSoftware.JeepService.LogConservation.Mappers
{
    public class ConservationMapper : IMapper<ConservationLogModel, Conservation>
    {
        public Conservation Map(ConservationLogModel source)
        {
            return Map(new Conservation(), source);
        }

        public Conservation Map(Conservation reference, ConservationLogModel source)
        {
            if (reference == null)
            {
                reference = new Conservation();
            }

            reference.UniqueId = source.Id;
            reference.EntityType = source.EntityName;
            reference.SendDate = source.LogDate;
            reference.Status = ConservationStatus.Error;
            reference.Type = ConservationType.Biblos;
            return reference;
        }
    }
}
