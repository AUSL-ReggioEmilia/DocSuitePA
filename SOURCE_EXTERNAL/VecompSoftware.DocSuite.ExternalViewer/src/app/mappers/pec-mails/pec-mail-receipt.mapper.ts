import { Injectable } from '@angular/core';

import { BaseMapper } from '../base.mapper';
import { PECMailReceiptModel } from '../../models/pec-mails/pec-mail-receipt.model';
import { PECDirectionType } from '../../models/pec-mails/pec-direction.type';
import { PECType } from '../../models/pec-mails/pec.type';

@Injectable()
export class PECMailReceiptMapper implements BaseMapper {

    constructor() { }

    mapFromJson(json: any): PECMailReceiptModel {

        if (!json) {
            return null;
        }

        let model: PECMailReceiptModel = new PECMailReceiptModel();

        model.step = json.Step;
        model.date = json.Date;

        return model;
    }

}