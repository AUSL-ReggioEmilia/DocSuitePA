import { IdentifierModel } from '../identifier.model';
import { BaseServiceModel } from '../base-service.model';

export class DocumentUnitModel implements IdentifierModel, BaseServiceModel{
    constructor() {
        this.serviceModelName = "DocumentUnitModel";
    }

    id: string;
    title: string;
    subject: string;
    name: string;
    documentUnitName: string;
    environment: number;
    year: number;
    number: number;
    readonly serviceModelName: string;
}