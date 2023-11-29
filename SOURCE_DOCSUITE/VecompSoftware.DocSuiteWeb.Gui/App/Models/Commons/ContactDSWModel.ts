import ContactTypeDSWModel = require("App/Models/Commons/ContactTypeDSWModel");
import ContactAddressDSWModel = require("App/Models/Commons/ContactAddressDSWModel");

interface ContactDSWModel {
    Id?: number;
    IsActive?: boolean;
    BirthDate?: Date;
    BirthPlace: string;
    Description: string;
    Code: string;
    FiscalCode: string;
    TelephoneNumber: string;
    FaxNumber: string;
    EmailAddress: string;
    CertifiedMail: string;
    Note: string;
    FirstName: string;
    LastName: string;
    SearchCode: string;
    ContactType: ContactTypeDSWModel;
    Address: ContactAddressDSWModel;
}

export = ContactDSWModel;