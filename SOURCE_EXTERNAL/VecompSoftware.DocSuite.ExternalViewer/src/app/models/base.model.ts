import { IdentifierModel } from './identifier.model';

export interface BaseModel extends IdentifierModel {

    year: number;
    number: number;
    registrationUser: string;
    registrationDate: Date;
    lastChangedUser: string;
    lastChangedDate?: Date;
    subject: string;

}