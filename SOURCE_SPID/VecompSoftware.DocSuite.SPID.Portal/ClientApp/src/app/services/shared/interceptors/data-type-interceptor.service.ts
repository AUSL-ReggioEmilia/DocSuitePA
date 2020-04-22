import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs/observable';

@Injectable()
export class DataTypeInterceptorService implements HttpInterceptor {
    constructor() { }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        if (req.method === 'POST') {
            let authReq: HttpRequest<any> = req.clone({
                headers: req.headers.set('Content-Type', 'application/json')
            });
            return next.handle(authReq);
        }
        return next.handle(req);
    }

}
