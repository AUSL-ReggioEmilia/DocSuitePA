
import { throwError as observableThrowError, Observable } from 'rxjs';
import { Response } from '@angular/http';
import { Injectable, ErrorHandler, Inject } from '@angular/core';
import { Router } from '@angular/router';


import { ErrorLogModel } from '../models/commons/error-log.model';
import { ResponseModel } from '../models/commons/response.model';
import { ErrorLogService } from '../services/error-log.service';
import { ErrorLevel } from '../models/commons/error-level';

@Injectable()
export class ExceptionHandlerHelper implements ErrorHandler {

    constructor(private errorLogService: ErrorLogService, private router: Router, ) { }

    public handleError = (error: any): any => {
        if (!error || !error.constructor) {
            return;
        }

        let errorLog: ErrorLogModel = new ErrorLogModel(); //il modello deve essere una collezione di stringhe 
        errorLog.userName = 'utente autenticato dall’integrazione';
        errorLog.level = ErrorLevel.ManagedError;
        errorLog.status = 500;

        if ('status' in error) {
            let responseError: Response = error as Response;

            if (!responseError) {
                console.log(error)
                return;
            }

            if (responseError.status) {
                errorLog.status = responseError.status;
            }            
            errorLog.errorMessages = [responseError.statusText];

            this.loggingError(errorLog);
        } else {
            let baseError: Error = error as Error;

            if (!baseError) {
                console.log(baseError)
                return;
            }
            errorLog.errorMessages = [baseError.message, baseError.stack];
            this.loggingError(errorLog);
        }
        //throw (errorLog);          
        return observableThrowError(errorLog);
    }

    private loggingError(errorLog: ErrorLogModel): void {
        if (!errorLog) {
            return;
        }

        if (console.group) {
            console.group("ErrorHandler");
            console.error(errorLog);
            console.groupEnd();
        }
        else {
            console.log(errorLog);
        }

        if (localStorage) {
            let errorKey: string = "Error - ".concat(new Date().getTime().toString());
            localStorage.setItem(errorKey, JSON.stringify(errorLog));
        }

        //TODO: decommentare
        //if (this.errorLogService)
        //    this.errorLogService.sendToApi(errorLog);

    }

}