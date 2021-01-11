import { IdentifierModel } from '../identifier.model';
import { SectorModel } from '../commons/sector.model';
import { SectorUserModel } from '../commons/sector-user.model';

export class ProtocolSectorModel implements IdentifierModel, SectorModel<ProtocolSectorModel>{

    id: string;
    name: string;
    active: boolean;
    archiveSection: string;
    uniqueIdFather: string;   

    ccType: string; 
    distributionType: string;
    authorized: boolean;

    children: ProtocolSectorModel[];
    sectorUsers: SectorUserModel[];

}