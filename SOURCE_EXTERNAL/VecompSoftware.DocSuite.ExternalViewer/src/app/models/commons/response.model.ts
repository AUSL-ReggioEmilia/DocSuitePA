import { Observable } from 'rxjs';

import { ErrorLogModel } from './error-log.model';
import { BaseServiceModel } from '../base-service.model';

export class ResponseModel<T extends BaseServiceModel> {
    results: T[];
    count?: number;
    error: ErrorLogModel;

    constructor() {
    }
}