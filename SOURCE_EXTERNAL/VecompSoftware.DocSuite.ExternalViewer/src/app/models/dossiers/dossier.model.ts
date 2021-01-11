import { BaseServiceModel } from '../base-service.model';
import { DossierFolderModel } from './dossier-folder.model';
import { DossierRoleModel } from './dossier-role.model';

export class DossierModel implements BaseServiceModel {

    readonly serviceModelName: string;

    uniqueId: string;
    year: number;
    number: number;
    title: string;
    subject: string;
    note: string;
    registrationDate: Date;
    registrationUser: string;
    lastChangedDate?: Date;
    lastChangedUser?: string;
    startDate: Date;
    endDate?: Date;
    containerId: number;
    containerName: string;
    jsonMetadata?: string;

    dossierFolders: DossierFolderModel[];
    dossierRoles: DossierRoleModel[];

    constructor() {
        this.serviceModelName = "DossierModel";
    }
}