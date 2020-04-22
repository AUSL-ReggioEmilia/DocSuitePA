import { ErrorHandler, Injectable, Injector } from '@angular/core';
import { LocationStrategy, PathLocationStrategy } from '@angular/common';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';

import { LoggingService } from '@app-services/shared/logging.service';
import { AuthService } from '@app-services/auth/auth.service';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
    constructor(private injector: Injector) { }

    handleError(error: any) {
        const loggingService = this.injector.get(LoggingService);
        const authService = this.injector.get(AuthService);
        const location = this.injector.get(LocationStrategy);

        const url = location instanceof PathLocationStrategy ? location.path() : '';
        const message = error.message ? error.message : error.toString();
        
        if (error instanceof HttpErrorResponse) {
            if (error.status === 401 || error.status === 403) {
                loggingService.warn(url.concat(' - unauthorized exception - ', message));
                authService.expireToken();
                this.injector.get(Router).navigate(['/pages/login']);
            } else if (error.status === 404) {
                loggingService.warn(url.concat(' - not found exception - ', message));
                this.injector.get(Router).navigate(['/pages/404']);
            } else {
                loggingService.error(url.concat(' - ', message));
                this.injector.get(Router).navigate(['/pages/500']);
            }
        } else {
            loggingService.error(url.concat(' - ', message));
            this.injector.get(Router).navigate(['/pages/500']);
        }
        
        console.error(error);
    }
}