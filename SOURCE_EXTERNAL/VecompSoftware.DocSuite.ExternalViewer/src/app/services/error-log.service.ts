import { HttpClient, HttpResponse, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from 'rxjs';

import { ErrorLogModel } from '../models/commons/error-log.model';
import { ExceptionHandlerHelper } from '../helpers/exception-handler.helper';
import { GlobalSetting } from '../settings/global.setting';


@Injectable()
export class ErrorLogService{

    url: string;
    error: ErrorLogModel;

    constructor(private http: HttpClient, private globalSetting: GlobalSetting) {     
        try {
            let address: string = globalSetting.apiOdataAddress;
            let controller: string = globalSetting.getController<ErrorLogModel>(new ErrorLogModel());
            this.url = address.concat(controller);
        }
        catch (ex) {
            console.log(ex);
        }   
    }

    public sendToApi = (error: any): void => {
        try {
            let body: string = JSON.stringify(error);
            let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
            let options = { headers: headers };
            this.http.post<ErrorLogModel>(this.url, body, options)
                .subscribe(
                    this.successCallback,
                    this.errorCallback
                );
        }
        catch (ex) {
            console.log(ex);
        }       
    }

    private successCallback(res: ErrorLogModel) {
        console.log("Post Success:".concat(res.toString()));
    }

    private errorCallback(error: Response) {
        console.error("Post error:".concat(error.toString()));
    }
}