import { Injectable } from '@angular/core';
import { Response } from '@angular/http';
import { Observable } from 'rxjs';

import { StartWorkflowModel } from '@app-models/_index';
import { BaseWebapiHttpService } from '@app-services/shared/base-webapi-http.service';

@Injectable()
export class WorkflowService {
   
    private get controllerName(): string {
        return 'StartWorkflow';
    }

    constructor(private http: BaseWebapiHttpService) { }

    /**
     * Start new workflow instance
     * @param model
     */
    startNewWorkflow(model: StartWorkflowModel): Observable<Response> {
        return this.http.post(this.controllerName, model);
    }
}
