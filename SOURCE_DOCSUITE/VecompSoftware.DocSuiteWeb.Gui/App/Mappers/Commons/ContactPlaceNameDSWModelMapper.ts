import BaseMapper = require('App/Mappers/BaseMapper');
import ContactPlaceNameDSWModel = require('App/Models/Commons/ContactPlaceNameDSWModel');
import ContactModel = require('App/Models/Commons/ContactModel');

class ContactPlaceNameDSWModelMapper extends BaseMapper<ContactPlaceNameDSWModel>{
    constructor() {
        super();
    }

    public Map(source: ContactModel): ContactPlaceNameDSWModel {
        let toMap = <ContactPlaceNameDSWModel>{};

        if (!source || !source.PlaceName) {
            return null;
        }

        toMap.Id = source.PlaceName.EntityShortId;
        toMap.Description = source.PlaceName.Description;

        return toMap;
    }
}

export = ContactPlaceNameDSWModelMapper;
