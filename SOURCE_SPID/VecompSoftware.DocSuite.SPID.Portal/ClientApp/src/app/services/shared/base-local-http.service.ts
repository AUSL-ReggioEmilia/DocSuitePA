import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { LocalHttpParams } from '@app-shared/params/local-http.params';

@Injectable()
export class BaseLocalHttpService {

    private get params(): LocalHttpParams
    {
        return new LocalHttpParams();
    }

    get baseHttpUrl(): string {
        return this.baseUrl.concat('api/');
    }

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

    get<T>(url: string): Observable<T> {
        return <Observable<T>>this.http.get(this.baseHttpUrl.concat(url), {params: this.params});
    }

    find<T>(url: string, params: LocalHttpParams): Observable<T> {
        return <Observable<T>>this.http.get(this.baseHttpUrl.concat(url), { params: params });
    }

    post(url: string, data: any): Observable<any> {
        return <Observable<any>>this.http.post(this.baseHttpUrl.concat(url), data, { params: this.params });
    }

    postToText(url: string, data: any): Observable<any> {
        return <Observable<any>>this.http.post(this.baseHttpUrl.concat(url), data, { params: this.params, responseType: 'text' });
    }
}
