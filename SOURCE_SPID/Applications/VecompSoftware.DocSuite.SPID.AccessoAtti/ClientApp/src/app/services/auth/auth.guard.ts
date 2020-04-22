import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Router, CanActivate } from '@angular/router';

import { UserService } from '@app-services/shared/user.service';
import { LoggingService } from '@app-services/shared/logging.service';

@Injectable()
export class AuthGuard implements CanActivate {

    constructor(private router: Router, private userService: UserService, private loggingService: LoggingService, @Inject(PLATFORM_ID) private platformId: Object) { }

    canActivate() {
        if (isPlatformBrowser(this.platformId)) {
            if (this.userService.currentUser) {
                return true;
            }

            this.loggingService.error('Nessuna utenza definita');
            console.warn('Nessuna utenza definita');
            this.router.navigate(['/not-authorized']);
        }
        return false;
    }
}
