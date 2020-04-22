using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Protocols
{
    public class ProtocolContactManualMapper : BaseEntityMapper<ProtocolContactManual, ProtocolContactManual>, IProtocolContactManualMapper
    {
        public override ProtocolContactManual Map(ProtocolContactManual entity, ProtocolContactManual entityTransformed)
        {
            #region [ Base ]
            //entityTransformed.Year = entity.Year;
            //entityTransformed.Number = entity.Number;
            entityTransformed.ComunicationType = entity.ComunicationType;
            entityTransformed.IdContactType = entity.IdContactType;
            entityTransformed.Type = entity.Type;
            entityTransformed.IdContactType = entity.IdContactType;
            entityTransformed.Description = entity.Description;
            entityTransformed.BirthDate = entity.BirthDate;
            entityTransformed.BirthPlace = entity.BirthPlace;
            entityTransformed.Code = entity.Code;
            entityTransformed.FiscalCode = entity.FiscalCode;
            entityTransformed.Address = entity.Address;
            entityTransformed.CivicNumber = entity.CivicNumber;
            entityTransformed.ZipCode = entity.ZipCode;
            entityTransformed.City = entity.City;
            entityTransformed.CityCode = entity.CityCode;
            entityTransformed.TelephoneNumber = entity.TelephoneNumber;
            entityTransformed.FaxNumber = entity.FaxNumber;
            entityTransformed.Nationality = entity.Nationality;
            entityTransformed.Language = entity.Language;
            entityTransformed.SDIIdentification = entity.SDIIdentification;
            #endregion

            return entityTransformed;
        }

    }
}
