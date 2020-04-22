import { ContactType } from './contact-type.enum';

export interface ContactModel {
    $type: string;
    ContactType: ContactType;
    Description: string;
    ArchiveSection: string;
    LanguageCode: string;
    City: string;
    Address: string;
    ZipCode: string;
    CivicNumber: string;
    TelephoneNumber: string;
    FiscalCode: string;
    BirthDate: Date;
    BirthPlace: string;
    EmailAddress: string;
    PECAddress: string;
    ExternalCode: string;
}