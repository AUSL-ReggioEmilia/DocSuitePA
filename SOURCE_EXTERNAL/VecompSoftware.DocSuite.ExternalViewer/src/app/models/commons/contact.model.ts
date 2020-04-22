import { IdentifierModel } from '../identifier.model';
import { ContactType } from './contact.type';

export interface ContactModel<T> extends IdentifierModel{

    name: string;
    type: ContactType;
    active: boolean;
    uniqueIdFather: string;

    children: T[];

}