import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class BaseLocalHttpService {

    private get params(): HttpParams {
        return new HttpParams();
    }

    get baseHttpUrl(): string {
        return this.baseUrl.concat('api/');
    }

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

    postToText(url: string, data: any): Observable<any> {
        return <Observable<any>>this.http.post(this.baseHttpUrl.concat(url), data, { params: this.params, responseType: 'text' });
    }
}
