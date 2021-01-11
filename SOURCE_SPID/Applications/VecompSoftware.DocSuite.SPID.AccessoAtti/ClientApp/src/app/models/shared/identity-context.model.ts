import { IdentityModel } from './identity.model';
import { RoleModel } from './role.model';

export interface IdentityContextModel {
    Identity: IdentityModel;
    Roles: RoleModel[];
}