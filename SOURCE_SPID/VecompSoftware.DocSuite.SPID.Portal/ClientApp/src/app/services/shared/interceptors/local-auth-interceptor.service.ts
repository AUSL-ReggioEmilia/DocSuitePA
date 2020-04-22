import { Injectable, Injector } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs/observable';

import { LocalHttpParams } from '@app-shared/params/local-http.params';
import { WebApiHttpParams } from '@app-shared/params/web-api-http.params';
import { AuthService } from '@app-services/auth/auth.service';

@Injectable()
export class LocalAuthInterceptorService implements HttpInterceptor {
    constructor(private injector: Injector) { }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const authService: AuthService = this.injector.get(AuthService);
        let authReq: HttpRequest<any>;
        if (req.params instanceof LocalHttpParams) {
            const token: string|null = authService.getToken();
            authReq = req.clone({
                headers: req.headers.set('Authorization', 'bearer '.concat(token ? token : ''))
            });
        } else {
            authReq = req;
        }        
        return next.handle(authReq);
    }
}