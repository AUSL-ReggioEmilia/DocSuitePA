import { Injectable } from '@angular/core';
import { Router, CanActivate } from '@angular/router';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

import { AuthService } from './auth.service';
import { LoggingService } from '@app-services/shared/logging.service';

@Injectable()
export class AuthGuard implements CanActivate {

    constructor(private router: Router, private authService: AuthService, private loggingService: LoggingService) { }

    canActivate(): Observable<boolean> {
        if (!this.authService.isTokenExpired()) {
            return of(true);
        }

        return this.authService.refreshToken().pipe(
            map(result => {
                if (!result) {
                    this.authService.expireToken();
                    this.router.navigate(['/pages/login']);
                    return false;
                }
                let refreshedToken: string = result;
                this.authService.setToken(refreshedToken);
                return true;
            }),
            catchError((error: any) => {
                this.authService.expireToken();
                this.loggingService.warn('User is not authorized or cannot refresh token');
                console.error(error);
                this.router.navigate(['/pages/login']);
                return of(false);
            })
        );
    }
}