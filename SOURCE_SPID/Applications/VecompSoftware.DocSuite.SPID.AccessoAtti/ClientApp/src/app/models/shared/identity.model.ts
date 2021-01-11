import { AuthorizationType } from './authorization-type.enum';

export interface IdentityModel {
    Name: string;
    Account: string;
    AuthorizationType: AuthorizationType;
}