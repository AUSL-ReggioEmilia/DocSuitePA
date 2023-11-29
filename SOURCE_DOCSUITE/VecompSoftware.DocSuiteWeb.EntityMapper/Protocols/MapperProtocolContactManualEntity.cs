using System;
using DSW = VecompSoftware.DocSuiteWeb.Data;
using APIProtocol = VecompSoftware.DocSuiteWeb.Entity.Protocols;
using NHibernate;
using APICommon = VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Protocols
{
    public class MapperProtocolContactManualEntity : BaseEntityMapper<DSW.ProtocolContactManual, APIProtocol.ProtocolContactManual>
    {
        #region [ Fields ]
        #endregion

        #region [Constructor]
        public MapperProtocolContactManualEntity()
        {
        }
        #endregion

        protected override IQueryOver<DSW.ProtocolContactManual, DSW.ProtocolContactManual> MappingProjection(IQueryOver<DSW.ProtocolContactManual, DSW.ProtocolContactManual> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override APIProtocol.ProtocolContactManual TransformDTO(DSW.ProtocolContactManual entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare ProtocolContactManual se l'entità non è inizializzata");

            APIProtocol.ProtocolContactManual apiProtocolContactManual = new APIProtocol.ProtocolContactManual
            {
                Protocol = new APIProtocol.Protocol()
            };
            apiProtocolContactManual.UniqueId = entity.Id;
            apiProtocolContactManual.Protocol.Year = entity.Year;
            apiProtocolContactManual.Protocol.Number = entity.Number;
            apiProtocolContactManual.Address = entity.Contact.Address?.Address;
            apiProtocolContactManual.City = entity.Contact.Address?.City;
            apiProtocolContactManual.CityCode = entity.Contact.Address?.CityCode;
            apiProtocolContactManual.CivicNumber = entity.Contact.Address?.CivicNumber;
            apiProtocolContactManual.Language = (APICommon.LanguageType?)entity.Contact.Address?.Language;
            apiProtocolContactManual.Nationality = entity.Contact.Address?.Nationality;
            apiProtocolContactManual.ZipCode = entity.Contact.Address?.ZipCode;
            apiProtocolContactManual.BirthDate = entity.Contact.BirthDate;
            apiProtocolContactManual.BirthPlace = entity.Contact.BirthPlace;
            apiProtocolContactManual.CertifydMail = entity.Contact.CertifiedMail;
            apiProtocolContactManual.Code = entity.Contact.Code;
            apiProtocolContactManual.IdContactType = entity.Contact.ContactType?.Id.ToString();
            apiProtocolContactManual.FiscalCode = entity.Contact.FiscalCode;
            apiProtocolContactManual.FullIncrementalPath = entity.Contact.FullIncrementalPath;
            apiProtocolContactManual.LastChangedDate = entity.LastChangedDate;
            apiProtocolContactManual.LastChangedUser = entity.LastChangedUser;
            apiProtocolContactManual.Note = entity.Contact.Note;
            apiProtocolContactManual.Description = entity.Contact.Description;
            apiProtocolContactManual.RegistrationDate = entity.RegistrationDate;
            apiProtocolContactManual.RegistrationUser = entity.RegistrationUser;
            apiProtocolContactManual.SDIIdentification = entity.SDIIdentification;
            apiProtocolContactManual.TelephoneNumber = entity.Contact?.TelephoneNumber;
            apiProtocolContactManual.ComunicationType = entity.ComunicationType;
            apiProtocolContactManual.Type = entity.Type;
            apiProtocolContactManual.Description = entity.Contact?.Description;
            
            return apiProtocolContactManual;
        }
    }
}
