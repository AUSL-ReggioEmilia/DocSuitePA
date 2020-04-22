import { IdentifierModel } from '../identifier.model';
import { SectorModel } from '../commons/sector.model';
import { SectorUserModel } from '../commons/sector-user.model';

export class ProtocolUserModel implements IdentifierModel{

    id: string;
    account: string;
    type: number; 

}