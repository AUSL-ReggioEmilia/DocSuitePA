import { BaseServiceModel } from '../base-service.model';

export class RoleModel implements BaseServiceModel {

    readonly serviceModelName: string;

    idRole: number;
    uniqueId: string;
    tenantId: string;
    name: string;
    fullIncrementalPath: string;
    roleLabel: string;
    authorizationType: string;

    constructor() {
        this.serviceModelName = "RoleModel";
    }
}