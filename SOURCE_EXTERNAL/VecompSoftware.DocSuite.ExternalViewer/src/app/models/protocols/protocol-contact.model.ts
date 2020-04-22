import { IdentifierModel } from '../identifier.model';
import { ContactModel } from '../commons/contact.model';
import { ComunicationType } from './comunication.type';
import { ContactType } from '../commons/contact.type'; 
import { GenericContactModel } from '../generic-contact.model'; 

export class ProtocolContactModel implements IdentifierModel, GenericContactModel<ProtocolContactModel>, ContactModel<ProtocolContactModel>{

    id: string;
    name: string;
    type: ContactType;
    active: boolean;
    uniqueIdFather: string;

    comunicationType: ComunicationType;
    isCC: string;
    isSelected: boolean;

    children: ProtocolContactModel[];

}