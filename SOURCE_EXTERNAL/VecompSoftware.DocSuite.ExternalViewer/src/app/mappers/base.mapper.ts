import { BaseModelType } from '../globals';
import { IdentifierModel } from '../models/identifier.model';

export interface BaseMapper {

    mapFromJson(json: any): any;
}