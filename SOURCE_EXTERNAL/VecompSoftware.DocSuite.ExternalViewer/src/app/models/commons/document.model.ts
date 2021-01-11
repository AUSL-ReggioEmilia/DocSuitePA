import { IdentifierModel } from '../identifier.model';
import { DocumentType } from './document.type';
import { BaseServiceModel } from '../base-service.model';

export class DocumentModel implements IdentifierModel, BaseServiceModel{
    constructor() {
        this.serviceModelName = "DocumentModel";
    }

    id: string;
    chainId: string;
    documentName: string;
    documentType: DocumentType;
    archiveSection: string;
    imageUrl: string;
    readonly serviceModelName: string;
}