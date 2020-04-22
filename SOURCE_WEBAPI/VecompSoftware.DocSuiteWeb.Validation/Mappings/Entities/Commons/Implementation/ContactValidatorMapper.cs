using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons
{
    public class ContactValidatorMapper : BaseMapper<Contact, ContactValidator>, IContactValidatorMapper
    {
        public ContactValidatorMapper() { }

        public override ContactValidator Map(Contact entity, ContactValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.EntityId = entity.EntityId;
            entityTransformed.IdContactType = entity.IdContactType;
            entityTransformed.ActiveFrom = entity.ActiveFrom;
            entityTransformed.ActiveTo = entity.ActiveTo;
            entityTransformed.isActive = entity.IsActive;
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
            entityTransformed.isLocked = entity.IsLocked;
            entityTransformed.isNotExpandable = entity.IsNotExpandable;
            entityTransformed.FullIncrementalPath = entity.FullIncrementalPath;
            entityTransformed.isChanged = entity.IsChanged;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Timestamp = entity.Timestamp;
            entityTransformed.Nationality = entity.Nationality;
            entityTransformed.Language = entity.Language;
            entityTransformed.SDIIdentification = entity.SDIIdentification;

            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Title = entity.Title;
            entityTransformed.PlaceName = entity.PlaceName;
            entityTransformed.Role = entity.Role;
            entityTransformed.RoleRootContact = entity.RoleRootContact;
            entityTransformed.OChartItems = entity.OChartItems;
            entityTransformed.ProtocolContacts = entity.ProtocolContacts;
            entityTransformed.ResolutionContacts = entity.ResolutionContacts;
            entityTransformed.Fascicles = entity.Fascicles;
            entityTransformed.Dossiers = entity.Dossiers;
            #endregion

            return entityTransformed;
        }

    }
}
