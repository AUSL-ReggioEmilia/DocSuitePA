using VecompSoftware.DocSuiteWeb.Model.Entities.Conservations;
using VecompSoftware.JeepService.LogConservation.Models;

namespace VecompSoftware.JeepService.LogConservation.Mappers
{
    public class LogModelMapper : IMapper<ConservationLogModel, LogModel>
    {
        public LogModel Map(ConservationLogModel source)
        {
            return Map(new LogModel(), source);
        }

        public LogModel Map(LogModel reference, ConservationLogModel source)
        {
            if (reference == null)
            {
                reference = new LogModel();
            }

            reference.Description = source.Description;
            reference.EntityName = source.ReferenceEntityName;
            reference.Hash = source.Hash;
            reference.LogType = source.LogType;
            reference.RegistrationDate = source.LogDate;
            reference.RegistrationUser = source.RegistrationUser;
            reference.ReferenceModel = new ReferenceModel()
            {
                ReferenceUniqueId = source.ReferenceUniqueId,
                Year = source.Year,
                Number = source.Number,
                Subject = source.Subject
            };
            return reference;
        }
    }
}
