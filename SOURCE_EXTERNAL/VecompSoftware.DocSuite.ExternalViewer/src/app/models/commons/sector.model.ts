import { IdentifierModel } from '../identifier.model';

export interface SectorModel<T> extends IdentifierModel {

    name: string;
    active: boolean;
    archiveSection: string;
    uniqueIdFather: string;

    children: T[];
}