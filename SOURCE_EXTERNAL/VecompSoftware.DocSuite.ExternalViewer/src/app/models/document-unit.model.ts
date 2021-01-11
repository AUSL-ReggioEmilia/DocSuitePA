import { BaseModel } from './base.model';
import { IdentifierModel } from './identifier.model';
import { DocumentUnitType } from './document-unit.type';
import { ContainerModel } from './commons/container.model';
import { CategoryModel } from './commons/category.model';

export interface DocumentUnitModel extends BaseModel, IdentifierModel {

    documentUnitName: string;
    title: string;
    documentUnitType: DocumentUnitType;
    container: ContainerModel;
    category: CategoryModel;
    location: string;
}