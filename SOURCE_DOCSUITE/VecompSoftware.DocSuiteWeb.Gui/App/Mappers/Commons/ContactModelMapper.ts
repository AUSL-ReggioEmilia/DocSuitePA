import BaseMapper = require('App/Mappers/BaseMapper');
import ContactModel = require('App/Models/Commons/ContactModel');

class ContactModelMapper extends BaseMapper<ContactModel>{
    constructor() {
        super();
    }

    public Map(source: any): ContactModel {
        let toMap = <ContactModel>{};

        if (!source) {
            return null;
        }

        toMap.Address = source.Address;
        toMap.BirthDate = source.BirthDate;
        toMap.CertifiedMail = source.CertifiedMail;
        toMap.City = source.City;
        toMap.CityCode = source.CityCode;
        toMap.CivicNumber = source.CivicNumber;
        toMap.Code = source.Code;
        toMap.Description = source.Description;
        toMap.EmailAddress = source.EmailAddress;
        toMap.EntityId = source.EntityId;
        toMap.FaxNumber = source.FaxNumber;
        toMap.FiscalCode = source.FiscalCode;
        toMap.FullIncrementalPath = source.FullIncrementalPath;
        toMap.IdContactType = source.IdContactType;
        toMap.IncrementalFather = source.IncrementalFather;
        toMap.isActive = source.isActive;
        toMap.isLocked = source.isLocked;
        toMap.isNotExpandable = source.isNotExpandable;
        toMap.Note = source.Note;
        toMap.SearchCode = source.SearchCode;
        toMap.TelephoneNumber = source.TelephoneNumber;
        toMap.UniqueId = source.UniqueId;
        toMap.ZipCode = source.ZipCode;
        toMap.Title = source.Title;
        toMap.PlaceName = source.PlaceName;

        return toMap;
    }
}

export = ContactModelMapper;