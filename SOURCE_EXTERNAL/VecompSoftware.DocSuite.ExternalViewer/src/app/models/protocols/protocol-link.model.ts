import { IdentifierModel } from '../identifier.model';
import { ProtocolModel } from './protocol.model';

export interface ProtocolLinkModel extends IdentifierModel{

    linkType?: number;

    protocol: ProtocolModel;
    protocolLinked: ProtocolModel;
}