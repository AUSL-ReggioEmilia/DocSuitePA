import { IdentifierModel } from '../identifier.model';
import { ContactModel } from '../commons/contact.model';
import { ContactType } from '../commons/contact.type'; 
import { GenericContactModel } from '../generic-contact.model'; 

export class FascicleContactModel implements IdentifierModel, GenericContactModel<FascicleContactModel>, ContactModel<FascicleContactModel>{

    id: string;
    name: string;
    type: ContactType;
    active: boolean;
    uniqueIdFather: string;

    isSelected: boolean;

    children: FascicleContactModel[];

}