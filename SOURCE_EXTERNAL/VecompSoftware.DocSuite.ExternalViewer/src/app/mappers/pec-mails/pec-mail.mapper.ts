import { Injectable } from '@angular/core';

import { BaseMapper } from '../base.mapper';
import { PECMailModel } from '../../models/pec-mails/pec-mail.model';
import { PECMailReceiptMapper } from '../../mappers/pec-mails/pec-mail-receipt.mapper';
import { PECDirectionType } from '../../models/pec-mails/pec-direction.type';
import { PECActiveType } from '../../models/pec-mails/pec-active.type';
import { PECType } from '../../models/pec-mails/pec.type';

@Injectable()
export class PECMailMapper implements BaseMapper {

    constructor(private pecMailReceiptMapper: PECMailReceiptMapper) { }

    mapFromJson(json: any): PECMailModel {

        if (!json) {
            return null;
        }

        let model: PECMailModel = new PECMailModel();

        model.id = json.Id;
        model.year = json.Year;
        model.number = json.Number;
        model.subject = json.Subject;
        model.direction = PECDirectionType[json.Direction as string];
        model.senders = json.Senders;
        model.recipients = json.Recipients;
        model.date = json.Date;
        model.body = json.Body;
        model.step = json.Step;
        model.active = PECActiveType[json.IsActive as string];
        model.type = PECType[json.PECType as string];
        model.receipts = json.Receipts ? json.Receipts.map((item) => this.pecMailReceiptMapper.mapFromJson(item)) : null;

        return model;
    }

}