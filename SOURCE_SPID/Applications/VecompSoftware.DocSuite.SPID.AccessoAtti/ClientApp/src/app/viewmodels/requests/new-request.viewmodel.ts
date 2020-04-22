import { UserType } from './user-type.enum';
import { ReturnType } from './return-type.enum';
import { AccessType } from './access-type.enum';

export interface NewRequestViewModel {
    ArchiveName: string;
    Name: string;
    Surname: string;
    City: string;
    Address: string;
    ZipCode: string;
    CivicNumber: string;
    TelephoneNumber: string;
    FiscalCode: string;
    DateOfBirth: Date;
    PlaceOfBirth: string;
    UserType: UserType;
    Documents: string;
    Motivations: string;
    ReturnType: ReturnType;
    AccessType: AccessType;
    Email: string;
    PEC: string;
    ExternalCode: string;
}