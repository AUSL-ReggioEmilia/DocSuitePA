import { Injectable } from '@angular/core';
import { BaseMapper } from '../base.mapper';
import { DossierRoleModel } from '../../models/dossiers/dossier-role.model';
import { RoleMapper } from '../roles/role.mapper';

@Injectable()
export class DossierRoleMapper implements BaseMapper {

    constructor(private roleMapper: RoleMapper) { }

    mapFromJson(json: any): DossierRoleModel {

        if (!json) {
            return null;
        }

        let model: DossierRoleModel = new DossierRoleModel();

        model.uniqueId = json.UniqueId;
        model.type = json.Type;
        model.role = json.Role ? this.roleMapper.mapFromJson(json.Role) : null;

        return model;
    }

}