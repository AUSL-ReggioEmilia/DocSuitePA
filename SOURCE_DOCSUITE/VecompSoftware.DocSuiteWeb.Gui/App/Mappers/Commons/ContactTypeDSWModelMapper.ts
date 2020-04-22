import BaseMapper = require('App/Mappers/BaseMapper');
import ContactTypeDSWModel = require('App/Models/Commons/ContactTypeDSWModel');
import ContactModel = require('App/Models/Commons/ContactModel');

class ContactTypeDSWModelMapper extends BaseMapper<ContactTypeDSWModel>{
    constructor() {
        super();
    }

    public Map(source: ContactModel): ContactTypeDSWModel {
        let toMap = <ContactTypeDSWModel>{};

        if (!source) {
            return null;
        }

        switch (source.IdContactType) {
            case "Administration":
                toMap.ContactTypeId = "M";
                break;
            case "AOO":
                toMap.ContactTypeId = "A";
                break;
            case "AO":
                toMap.ContactTypeId = "U";
                break;
            case "Role":
                toMap.ContactTypeId = "R";
                break;
            case "Group":
                toMap.ContactTypeId = "G";
                break;
            case "Sector":
                toMap.ContactTypeId = "S";
                break;
            case "Citizen":
                toMap.ContactTypeId = "P";
                break;
            case "IPA":
                toMap.ContactTypeId = "I";
                break;
            default:
                toMap.ContactTypeId = source.IdContactType;
        }

        return toMap;
    }
}

export = ContactTypeDSWModelMapper;
