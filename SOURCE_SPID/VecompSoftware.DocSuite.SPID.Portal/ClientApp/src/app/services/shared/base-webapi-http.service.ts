import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { WebApiHttpParams } from '@app-shared/params/web-api-http.params';

@Injectable()
export class BaseWebapiHttpService {

    private get params(): WebApiHttpParams {
        return new WebApiHttpParams();
    }

    get baseHttpUrl(): string {
        return '';
    }

    constructor(private http: HttpClient) { }

}
