import { Injectable, Inject } from '@angular/core';

import { BaseLocalHttpService } from '@app-services/shared/base-local-http.service';
import { LogItem } from '@app-models/log.model';
import { LogLevelEnum } from '@app-models/log-level.enum';

@Injectable()
export class LoggingService {

    private get controllerName(): string {
        return 'spid.logging';
    }

    constructor(private http: BaseLocalHttpService) { }

    debug(message: string): void {
        this.log(LogLevelEnum.Debug, message);
    }

    info(message: string): void {
        this.log(LogLevelEnum.Info, message);
    }

    warn(message: string): void {
        this.log(LogLevelEnum.Warn, message);
    }

    error(message: string): void {
        this.log(LogLevelEnum.Error, message);
    }

    fatal(message: string): void {
        this.log(LogLevelEnum.Critical, message);
    }

    private log(level: LogLevelEnum, message: string): void {
        let jsnlog: LogItem = new LogItem(level, 'Portal', message);
        this.http.postToText(this.controllerName, JSON.stringify(jsnlog))
            .subscribe(undefined,
            (error) => { console.error(error); });
    }
}
