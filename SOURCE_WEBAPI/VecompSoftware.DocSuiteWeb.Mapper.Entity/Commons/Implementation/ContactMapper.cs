using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Commons
{
    public class ContactMapper : BaseEntityMapper<Contact, Contact>, IContactMapper
    {
        public ContactMapper()
        {

        }

        public override Contact Map(Contact entity, Contact entityTransformed)
        {
            #region [ Base ]
            entityTransformed.IdContactType = entity.IdContactType;
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.IncrementalFather = entity.IncrementalFather;
            entityTransformed.Description = entity.Description;
            entityTransformed.BirthDate = entity.BirthDate;
            entityTransformed.BirthPlace = entity.BirthPlace;
            entityTransformed.Code = entity.Code;
            entityTransformed.SearchCode = entity.SearchCode;
            entityTransformed.FiscalCode = entity.FiscalCode;
            entityTransformed.ZipCode = entity.ZipCode;
            entityTransformed.Address = entity.Address;
            entityTransformed.CivicNumber = entity.CivicNumber;
            entityTransformed.City = entity.City;
            entityTransformed.CityCode = entity.CityCode;
            entityTransformed.TelephoneNumber = entity.TelephoneNumber;
            entityTransformed.FaxNumber = entity.FaxNumber;
            entityTransformed.EmailAddress = entity.EmailAddress;
            entityTransformed.CertifiedMail = entity.CertifiedMail;
            entityTransformed.Note = entity.Note;
            entityTransformed.IsLocked = entity.IsLocked;
            entityTransformed.IsNotExpandable = entity.IsNotExpandable;
            entityTransformed.FullIncrementalPath = entity.FullIncrementalPath;
            entityTransformed.Nationality = entity.Nationality;
            entityTransformed.Language = entity.Language;
            entityTransformed.SDIIdentification = entity.SDIIdentification;

            #endregion

            return entityTransformed;
        }

    }
}
