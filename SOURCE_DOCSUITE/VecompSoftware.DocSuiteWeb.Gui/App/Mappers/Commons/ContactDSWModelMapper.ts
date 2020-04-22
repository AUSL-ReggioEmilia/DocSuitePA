import BaseMapper = require('App/Mappers/BaseMapper');
import ContactDSWModel = require('App/Models/Commons/ContactDSWModel');
import ContactModel = require('App/Models/Commons/ContactModel');
import ContactTypeDSWModelMapper = require('./ContactTypeDSWModelMapper');
import ContactAddressDSWModelMapper = require('./ContactAddressDSWModelMapper');

class ContactDSWModelMapper extends BaseMapper<ContactDSWModel>{
    constructor() {
        super();
    }

    public Map(source: ContactModel): ContactDSWModel {
        let toMap = <ContactDSWModel>{};

        if (!source) {
            return null;
        }

        toMap.Id = source.EntityId;
        toMap.IsActive = source.isActive;
        toMap.BirthDate = source.BirthDate;
        toMap.Description = source.Description;
        toMap.Code = source.Code;
        toMap.FiscalCode = source.FiscalCode;
        toMap.TelephoneNumber = source.TelephoneNumber;
        toMap.FaxNumber = source.FaxNumber;
        toMap.EmailAddress = source.EmailAddress;
        toMap.CertifiedMail = source.CertifiedMail;
        toMap.Note = source.Note;
        if (source.Description) {
            let splittedName: string[] = source.Description.split("|");
            toMap.FirstName = splittedName[0];
            if (splittedName.length > 1) {
                toMap.LastName = splittedName[1];
            }            
        }        
        toMap.SearchCode = source.SearchCode;
        toMap.ContactType = new ContactTypeDSWModelMapper().Map(source);
        toMap.Address = new ContactAddressDSWModelMapper().Map(source);

        return toMap;
    }
}

export = ContactDSWModelMapper;