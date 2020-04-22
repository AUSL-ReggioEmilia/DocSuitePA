import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs/observable';

@Injectable()
export class CacheInterceptorService implements HttpInterceptor {
    constructor() { }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        let authReq: HttpRequest<any> = req.clone();
        authReq.headers.set('Cache-Control', 'no-cache');
        authReq.headers.set('Pragma', 'no-cache');
        authReq.headers.set('Expires', 'Sat, 01 Jan 2000 00:00:00 GMT');
        return next.handle(authReq);
    }
}
