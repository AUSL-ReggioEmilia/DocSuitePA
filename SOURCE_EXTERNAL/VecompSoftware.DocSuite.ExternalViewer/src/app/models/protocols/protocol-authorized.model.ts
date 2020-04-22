import { IdentifierModel } from '../identifier.model';
import { BaseModel } from '../base.model';
import { BaseServiceModel } from '../base-service.model';
import { DocumentUnitModel } from '../document-unit.model';
import { ContainerModel } from '../commons/container.model';
import { CategoryModel } from '../commons/category.model';
import { PECMailModel } from '../pec-mails/pec-mail.model';
import { ProtocolContactModel } from './protocol-contact.model';
import { ProtocolLinkModel } from './protocol-link.model';
import { ProtocolSectorModel } from './protocol-sector.model';
import { ProtocolType } from './protocol.type';
import { DocumentModel } from '../commons/document.model';
import { DocumentUnitType } from '../document-unit.type';
import { ProtocolStatusType } from './protocol-status.type';


export class ProtocolAuthorizedModel implements DocumentUnitModel, BaseModel, IdentifierModel, BaseServiceModel {
    constructor() {
        this.serviceModelName = "ProtocolAuthorizedModel";
    }

    id: string;
    year: number;
    number: number;
    registrationUser: string;
    registrationDate: Date;
    lastChangedUser: string;
    lastChangedDate: Date;
    subject: string;
    documentUnitName: string;
    title: string;
    documentUnitType: DocumentUnitType;

    note: string;
    type: ProtocolType;
    location: string;
    status: ProtocolStatusType;

    annulmentReason: string;
    isRejected: boolean;

    //valore di Subject di AdvancedProtocol, se il protocollo è in entrata
    assignee: string;

    //valore di Subject di AdvancedProtocol, se il protocollo è in uscita
    addressee: string;

    serviceCategory: string;
    //corrisponde a CheckPublication in Protocol
    onlinePublication: boolean;

    documents: DocumentModel[];
    container: ContainerModel;
    category: CategoryModel;
    sectors: ProtocolSectorModel[];
    pecMails: PECMailModel[];
    contacts: ProtocolContactModel[];
    links: ProtocolLinkModel[];
    linkedProtocols: ProtocolLinkModel[];
    readonly serviceModelName: string;
}