using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Protocols
{
    public class ProtocolContactManualValidatorMapper : BaseMapper<ProtocolContactManual, ProtocolContactManualValidator>, IProtocolContactManualValidatorMapper
    {
        public ProtocolContactManualValidatorMapper() { }

        public override ProtocolContactManualValidator Map(ProtocolContactManual entity, ProtocolContactManualValidator entityTransformed)
        {

            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Year = entity.Year;
            entityTransformed.Number = entity.Number;
            entityTransformed.ComunicationType = entity.ComunicationType;
            entityTransformed.Type = entity.Type;
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
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Timestamp = entity.Timestamp;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.IdContactType = entity.IdContactType;
            entityTransformed.Language = entity.Language;
            entityTransformed.Nationality = entity.Nationality;
            entityTransformed.SDIIdentification = entity.SDIIdentification;

            #endregion

            #region [ Navigation Properties ]

            entityTransformed.Protocol = entity.Protocol;
            entityTransformed.ContactPlaceName = entity.ContactPlaceName;
            entityTransformed.ContactTitle = entity.ContactTitle;

            #endregion

            return entityTransformed;
        }

    }
}
