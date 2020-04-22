import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { ConfigurationService } from './configuration.service';

@Injectable()
export class BaseWebapiHttpService {

    private get params(): HttpParams {
        return new HttpParams();
    }

    private getODATAFullUrl(url: string): string {
        return this.configurationService.config.ODATAAddress.concat('/', url);
    }

    private getRestFullUrl(url: string): string {
        return this.configurationService.config.APIAddress.concat('/', url);
    }

    constructor(private http: HttpClient, private configurationService: ConfigurationService) { }

    get<T>(url: string): Observable<T> {
        return <Observable<T>>this.http.get(this.getODATAFullUrl(url), { params: this.params });
    }

    find<T>(url: string, params: HttpParams): Observable<T> {
        return <Observable<T>>this.http.get(this.getODATAFullUrl(url), { params: params });
    }

    post(url: string, data: any): Observable<any> {
        return <Observable<any>>this.http.post(this.getRestFullUrl(url), data, { params: this.params });
    }

    postToText(url: string, data: any): Observable<any> {
        return <Observable<any>>this.http.post(this.getRestFullUrl(url), data, { params: this.params, responseType: 'text' });
    }
}