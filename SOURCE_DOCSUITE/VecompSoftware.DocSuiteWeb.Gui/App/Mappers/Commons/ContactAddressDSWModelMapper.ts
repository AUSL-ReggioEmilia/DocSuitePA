import BaseMapper = require('App/Mappers/BaseMapper');
import ContactAddressDSWModel = require('App/Models/Commons/ContactAddressDSWModel');
import ContactModel = require('App/Models/Commons/ContactModel');
import ContactPlaceNameDSWModelMapper = require('./ContactPlaceNameDSWModelMapper');

class ContactAddressDSWModelMapper extends BaseMapper<ContactAddressDSWModel>{
    constructor() {
        super();
    }

    public Map(source: ContactModel): ContactAddressDSWModel {
        let toMap = <ContactAddressDSWModel>{};

        if (!source) {
            return null;
        }

        toMap.Address = source.Address;
        toMap.CivicNumber = source.CivicNumber;
        toMap.ZipCode = source.ZipCode;
        toMap.City = source.City;
        toMap.CityCode = source.CityCode;
        toMap.PlaceName = new ContactPlaceNameDSWModelMapper().Map(source);

        return toMap;
    }
}

export = ContactAddressDSWModelMapper;
