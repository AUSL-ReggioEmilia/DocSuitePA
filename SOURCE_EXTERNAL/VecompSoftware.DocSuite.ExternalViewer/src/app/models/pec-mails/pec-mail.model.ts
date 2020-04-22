import { IdentifierModel } from '../identifier.model';
import { Environment } from '../environment';
import { ProtocolModel } from '../protocols/protocol.model';
import { PECType } from './pec.type';
import { PECActiveType } from './pec-active.type';
import { PECDirectionType } from './pec-direction.type';
import { PECMailReceiptModel } from './pec-mail-receipt.model';
import { BaseServiceModel } from '../base-service.model';

export class PECMailModel implements IdentifierModel, BaseServiceModel {
    constructor() {
        this.serviceModelName = "PECMailModel";
    }

    id: string;
    year?: number;
    number?: number;
    direction: PECDirectionType;
    subject: string;
    senders: string;
    recipients: string;
    active: PECActiveType;
    date?: Date;
    type: PECType;
    body: string;
    step: string;
    readonly serviceModelName: string;

    receipts: PECMailReceiptModel[];

}