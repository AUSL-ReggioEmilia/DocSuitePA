import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { WorkflowStatusModel } from '@app-models/workflows/_index';
import { ODataResultModel } from '@app-models/shared/_index';
import { BaseWebapiHttpService } from '@app-services/shared/base-webapi-http.service';
import { ConfigurationService } from '@app-services/shared/configuration.service';

@Injectable()
export class WorkflowStatusService {

    private get controllerName(): string {
        return 'Workflows';
    }

    constructor(private http: BaseWebapiHttpService, private configurationService: ConfigurationService) { }

    getMyRequests(id: string): Observable<ODataResultModel<WorkflowStatusModel>> {
        return this.http.get(this.controllerName.concat("/WorkflowService.MyInstances(workflowName='", this.configurationService.config.WorkflowName ,"', identifier='", id, "')"))
            .pipe(map(response => {
                return response as ODataResultModel<WorkflowStatusModel>;
            }));
    }
}
