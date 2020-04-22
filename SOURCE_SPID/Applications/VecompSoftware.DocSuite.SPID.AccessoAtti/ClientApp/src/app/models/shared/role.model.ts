import { AuthorizationType } from './authorization-type.enum';

export interface RoleModel {
    AuthorizationType: AuthorizationType;
    Name: string;
    RoleUniqueId: string;
    ExternalTagIdentifier: string;
}