import { Injectable } from '@angular/core'; 

import { BaseMapper } from './base.mapper'; 
import { ResolutionModel } from '../models/resolution.model';
import { ResolutionType } from '../models/resolution.type';  


@Injectable()
export class ResolutionMapper implements BaseMapper {

    mapFromJson(json: any): ResolutionModel {
        if (!json) {
            return null;
        }

        let model: ResolutionModel = new ResolutionModel();
        model.id = json.Id;
        model.year = json.Year;
        model.inclusiveNumber = json.InclusiveNumber;
        model.subject = json.Subject;
        model.adoptionDate = json.AdoptionDate;
        model.executiveDate = json.EffectivenessDate;
        model.publicationDate = json.PublishingDate;
        model.proposer = json.Proposer;
        model.type = ResolutionType[json.IdType as string];
        model.serviceNumber = json.ServiceNumber;
        model.serviceCode = json.ServiceNumber ? json.ServiceNumber.split('/')[0] : null;

        return model;

    }
}