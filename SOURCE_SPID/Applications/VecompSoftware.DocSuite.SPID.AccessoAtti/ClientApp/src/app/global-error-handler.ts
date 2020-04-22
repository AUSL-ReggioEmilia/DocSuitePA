import { ErrorHandler, Injectable, Injector, NgZone } from '@angular/core';
import { Router } from '@angular/router';

import { LoggingService } from '@app-services/shared/logging.service';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
    constructor(private injector: Injector, private loggingService: LoggingService, private zone: NgZone) { }

    handleError(error: any) {
        const message = error.message ? error.message : error.toString();
        this.loggingService.error(message);
        this.zone.run(() => this.injector.get(Router).navigate(['/error']));
        console.error(error);
    }
}