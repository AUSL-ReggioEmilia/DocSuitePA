import { IdentifierModel } from '../identifier.model';

export class CategoryModel implements IdentifierModel {

    id: string;

    name: string;
    active: boolean;
    code: number;
    hierarchyCode: string;
    hierarchyDescription: string;
    archiveSection: string;
    startDate: Date;
    endDate: Date;

    children: CategoryModel[];

}