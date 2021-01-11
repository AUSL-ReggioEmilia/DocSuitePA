import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs/observable';

@Injectable()
export class CacheInterceptor implements HttpInterceptor {
    constructor() { }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        if (req.url.includes("/ProtocolAuthorizedService") && req.method === "GET") {
            let noCacheReq: HttpRequest<any> = req.clone({
                setHeaders: {
                    ['Cache-Control']: 'no-cache',
                    ['Pragma']: 'no-cache',
                    ['Expires']: 'Sat, 01 Jan 2000 00:00:00 GMT'
                }
            });
            return next.handle(noCacheReq);
        }
        return next.handle(req);
    }
}
