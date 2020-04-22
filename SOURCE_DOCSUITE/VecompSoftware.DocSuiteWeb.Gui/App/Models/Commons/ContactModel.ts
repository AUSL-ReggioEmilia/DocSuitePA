import ContactPlaceNameModel = require("App/Models/Commons/ContactPlaceNameModel");
import ContactTitleModel = require("App/Models/Commons/ContactTitleModel");

interface ContactModel {
    EntityId: number;
    Code?: string;
    FiscalCode?: string;
    Address?: string;
    CivicNumber?: string;
    ZipCode?: string;
    City?: string;
    CityCode?: string;
    TelephoneNumber?: string;
    FaxNumber?: string;
    EmailAddress?: string;
    CertifiedMail?: string;
    Note?: string;
    FullIncrementalPath?: string;
    IncrementalFather?: number;
    Description?: string;
    BirthDate?: Date;
    BirthPlace: string;
    Language: string;
    Nationality: string;
    SearchCode?: string;
    isActive?: number;
    isLocked?: number;
    isNotExpandable?: number;
    ActiveFrom?: Date;
    ActiveTo?: Date;
    isChanged?: number;
    IdContactType?: string;
    UniqueId?: string;
    Title?: ContactTitleModel;
    PlaceName?: ContactPlaceNameModel;
}

export = ContactModel;