using System;
using DSW = VecompSoftware.DocSuiteWeb.Data;
using APICommon = VecompSoftware.DocSuiteWeb.Entity.Commons;
using NHibernate;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Commons
{
    public class MapperContactEntity : BaseEntityMapper<DSW.Contact, APICommon.Contact>
    {
        #region
        public MapperContactEntity()
        {

        }
        #endregion

        #region
        protected override IQueryOver<DSW.Contact, DSW.Contact> MappingProjection(IQueryOver<DSW.Contact, DSW.Contact> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override APICommon.Contact TransformDTO(DSW.Contact entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare Contact se l'entità non è inizializzata");

            APICommon.Contact apiContact = new APICommon.Contact();
            apiContact.UniqueId = entity.UniqueId;
            apiContact.EntityId = entity.Id;
            apiContact.Address = entity.Address?.Address;
            apiContact.City = entity.Address?.City;
            apiContact.CityCode = entity.Address?.CityCode;
            apiContact.CivicNumber = entity.Address?.CivicNumber;
            apiContact.Language = (APICommon.LanguageType?)entity.Address?.Language;
            apiContact.Nationality = entity.Address?.Nationality;
            apiContact.ZipCode = entity.Address?.ZipCode;
            apiContact.BirthDate = entity.BirthDate;
            apiContact.BirthPlace = entity.BirthPlace;
            apiContact.CertifiedMail = entity.CertifiedMail;
            apiContact.Code = entity.Code;
            apiContact.IdContactType = entity.ContactType?.Id.ToString();
            apiContact.FiscalCode = entity.FiscalCode;
            apiContact.FullIncrementalPath = entity.FullIncrementalPath;
            apiContact.IsActive = entity.IsActive;
            apiContact.IsLocked = (byte?)entity.isLocked;
            apiContact.IsNotExpandable = (byte?)entity.isNotExpandable;
            apiContact.LastChangedDate = entity.LastChangedDate;
            apiContact.LastChangedUser = entity.LastChangedUser;
            apiContact.Note = entity.Note;
            apiContact.RegistrationDate = entity.RegistrationDate;
            apiContact.RegistrationUser = entity.RegistrationUser;
            apiContact.SDIIdentification = entity.SDIIdentification;
            apiContact.SearchCode = entity.SearchCode;
            apiContact.TelephoneNumber = entity.TelephoneNumber;
            apiContact.Description = entity.Description;

            return apiContact;
        }
        #endregion
    }
}
