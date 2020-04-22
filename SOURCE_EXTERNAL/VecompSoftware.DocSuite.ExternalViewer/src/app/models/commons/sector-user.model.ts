import { IdentifierModel } from '../identifier.model';
import { Environment } from '../environment';

export interface SectorUserModel extends IdentifierModel{

    name: string;
    userType: string;
    active: boolean;
    account: string;
    environment: Environment;

}