import { IdentifierModel } from './identifier.model';
import { ContactType } from '../models/commons/contact.type';

export interface GenericContactModel<T> extends IdentifierModel {

    name: string;
    type: ContactType;
    active: boolean;
    uniqueIdFather: string;
    isSelected: boolean;

    children: T[];

}