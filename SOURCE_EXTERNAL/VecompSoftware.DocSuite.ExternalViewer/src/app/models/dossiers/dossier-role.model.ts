import { BaseServiceModel } from '../base-service.model';
import { RoleModel } from '../roles/role.model';

export class DossierRoleModel implements BaseServiceModel {

    readonly serviceModelName: string;

    uniqueId: string;
    type: string;
    role: RoleModel;

    constructor() {
        this.serviceModelName = "DossierRoleModel";
    }
}