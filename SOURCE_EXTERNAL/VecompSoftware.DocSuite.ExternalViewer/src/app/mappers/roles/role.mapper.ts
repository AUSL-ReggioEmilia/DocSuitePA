import { Injectable } from '@angular/core';
import { BaseMapper } from '../base.mapper';
import { RoleModel } from '../../models/roles/role.model';

@Injectable()
export class RoleMapper implements BaseMapper {

    constructor() { }

    mapFromJson(json: any): RoleModel {

        if (!json) {
            return null;
        }

        let model: RoleModel = new RoleModel();

        model.uniqueId = json.UniqueId;
        model.name = json.Name;
        model.idRole = json.IdRole;
        model.tenantId = json.TenantId;
        model.fullIncrementalPath = json.FullIncrementalPath;
        model.roleLabel = json.RoleLabel;
        model.authorizationType = json.AuthorizationType;

        return model;
    }

}