import { BaseServiceModel } from '../base-service.model';

export class DossierFolderModel implements BaseServiceModel {

    readonly serviceModelName: string;

    id: string;
    name: string;
    status: string;
    jsonMetadata?: string;
    idFascicle: string;
    idDossier: string;
    idCategory: number;
    idRole: number;

    constructor() {
        this.serviceModelName = "DossierFolderModel";
    }
}