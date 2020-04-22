import { IdentifierModel } from '../identifier.model';
import { ProtocolModel } from '../protocols/protocol.model';
import { FascicleReferenceType } from './fascicle-reference.type';
import { FascicleModel } from './fascicle.model';

export interface FascicleProtocolModel extends IdentifierModel{

    referenceType: FascicleReferenceType;

    fascicle: FascicleModel;
    protocol: ProtocolModel;
}