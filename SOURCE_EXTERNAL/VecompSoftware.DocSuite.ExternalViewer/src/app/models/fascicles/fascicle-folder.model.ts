import { BaseServiceModel } from '../base-service.model';

export class FascicleFolderModel implements BaseServiceModel {
    readonly serviceModelName: string;

    constructor() {
        this.serviceModelName = "FascicleFolderModel";
    }

    id: string;
    status: string;
    typology: string;
    name: string;
    idFascicle: string;
    idCategory: string;
    hasDocuments: boolean;
    hasChildren: boolean;
    fascicleFolderLevel: number;
}