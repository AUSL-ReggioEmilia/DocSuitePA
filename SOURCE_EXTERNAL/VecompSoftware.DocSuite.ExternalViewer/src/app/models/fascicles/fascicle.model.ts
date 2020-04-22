import { IdentifierModel } from '../identifier.model';
import { BaseModel } from '../base.model'
import { ArchiveEntityModel } from '../archive-entity.model';
import { CategoryModel } from '../commons/category.model';
import { FascicleContactModel } from './fascicle-contact.model';
import { FascicleType } from './fascicle.type';
import { DocumentUnitModel } from '../document-units/document-unit.model'; 
import { DocumentModel } from '../commons/document.model'; 
import { BaseServiceModel } from '../base-service.model';
import { FascicleFolderModel } from './fascicle-folder.model';

export class FascicleModel implements ArchiveEntityModel, BaseModel, IdentifierModel, BaseServiceModel {
    constructor() {
        this.serviceModelName = "FascicleModel";
    }

    id: string;
    year: number;
    number: number;
    registrationUser: string;
    registrationDate: Date;
    lastChangedUser: string;
    lastChangedDate?: Date;

    startDate: Date;
    endDate?: Date;
    name: string;
    subject: string; 
    note: string;  
    type: FascicleType;
    title: string;

    category: CategoryModel;
    contacts: FascicleContactModel[];
    documentUnits: DocumentUnitModel[];
    documents: DocumentModel[];
    fascicleFolders: FascicleFolderModel[];
    readonly serviceModelName: string;

}