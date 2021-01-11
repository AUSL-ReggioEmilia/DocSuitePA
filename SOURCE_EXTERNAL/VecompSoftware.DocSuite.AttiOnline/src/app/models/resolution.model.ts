import { ResolutionType } from './resolution.type';

export class ResolutionModel {

    id: string;
    year: number;
    serviceNumber: string;
    serviceName: string;
    serviceCode: string;
    inclusiveNumber: string;
    adoptionDate: Date;
    publicationDate: Date;
    executiveDate: Date;
    subject: string;
    documentUrl: string;
    proposer: string;
    type: ResolutionType;
    service: string;
    
}