import { BaseServiceModel } from '../base-service.model';
import { ErrorLevel } from './error-level';

export class ErrorLogModel implements BaseServiceModel {
    userName: string;
    status: number;
    errorMessages: string[];
    level: ErrorLevel;
    readonly serviceModelName: string;

    constructor() {
        this.serviceModelName = "ErrorLogModel";
    };
}