using VecompSoftware.BPM.Integrations.Modules.AUSLPC.COVID19.Models;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLPC.COVID19.Mappers
{
    public class DocumentUnitModelMapper
    {
        public static DocumentUnitModel Map(DocumentUnit documentUnit)
        {
            return new DocumentUnitModel(documentUnit.UniqueId)
            {
                Year = documentUnit.Year,
                Number = documentUnit.Number,
                Title = documentUnit.Title,
                RegistrationDate = documentUnit.RegistrationDate,
                RegistrationUser = documentUnit.RegistrationUser,
                Subject = documentUnit.Subject,
                Environment = documentUnit.Environment,
                IdUDSRepository = documentUnit.UDSRepository?.UniqueId
            };
        }
    }
}
