import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { BaseLocalHttpService } from '@app-services/shared/base-local-http.service';
import { ApplicationModel } from '@app-models/application.model';
import { ODataResultModel } from '@app-models/odata-result.model';

@Injectable()
export class SpidApplicationService {

    private get controllerName(): string {
        return 'Application';
    }

    constructor(private http: BaseLocalHttpService) { }

    getApplications(): Observable<ODataResultModel<ApplicationModel>> {
        return this.http.get<ODataResultModel<ApplicationModel>>(this.controllerName);
    }

    getApplication(id: string): Observable<ODataResultModel<ApplicationModel>> {
        return this.http.get<ODataResultModel<ApplicationModel>>(this.controllerName.concat('/', id));
    }
}
