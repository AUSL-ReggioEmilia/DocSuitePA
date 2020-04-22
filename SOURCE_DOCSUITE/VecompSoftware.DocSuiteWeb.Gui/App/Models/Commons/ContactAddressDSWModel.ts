import ContactPlaceNameDSWModel = require("App/Models/Commons/ContactPlaceNameDSWModel");

interface ContactAddressDSWModel {
    PlaceName: ContactPlaceNameDSWModel;
    Address: string;
    CivicNumber: string;
    ZipCode: string;
    City: string;
    CityCode: string;
    Nationality: string;
    Language: string;
}

export = ContactAddressDSWModel;