import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpResponse, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';



import { BaseServiceModel } from '../models/base-service.model';
import { GlobalSetting } from '../settings/global.setting';
import { ResponseModel } from '../models/commons/response.model';
import { BaseModelType } from '../globals';
import { ExceptionHandlerHelper } from '../helpers/exception-handler.helper';
import { ErrorLogModel } from '../models/commons/error-log.model';
import { BaseMapper } from '../mappers/base.mapper';
import { OdataModel } from 'src/app/models/commons/odata.model';
import { RequestOptionsArgs } from '@angular/http';

export class BaseService<T extends BaseServiceModel> {

    mapper: BaseMapper;
    modelInstance: BaseModelType;

    constructor(private http: HttpClient, private exceptionHandlerHelper: ExceptionHandlerHelper, protected globalSetting: GlobalSetting, instance: BaseModelType, mapper: BaseMapper) {
        this.mapper = mapper;
        this.modelInstance = instance;
    }

    private getAuthRequest(url: string): Observable<ResponseModel<T>> {
        let resultModel: Observable<ResponseModel<T>> = new Observable<ResponseModel<T>>();

        let token: string = sessionStorage.getItem('access_token');
        let headers = new HttpHeaders({
            'Content-Type': 'application/x-www-form-urlencoded',
            'Authorization': 'Bearer '.concat(token)
        });
        let options = { headers: headers };

        return this.http.get<OdataModel<any>>(url, options)
            .pipe(
                map(this.mapResultModel),
                catchError(this.exceptionHandlerHelper.handleError)
            ) as Observable<ResponseModel<T>>;
    }

    private getAuthCountRequest(url: string): Observable<number> {
        let resultModel: Observable<number> = new Observable<number>();

        let token: string = sessionStorage.getItem('access_token');
        let headers = new HttpHeaders({
            'Content-Type': 'application/x-www-form-urlencoded',
            'Authorization': 'Bearer '.concat(token)
        });
        let options = { headers: headers };

        return this.http.get<any>(url, options)
            .pipe(
                map(result => result.value),
                catchError(this.exceptionHandlerHelper.handleError)
            ) as Observable<number>;
    }

    private getRequest(url: string, withCredentials: boolean): Observable<ResponseModel<T>> {
        let resultModel: Observable<ResponseModel<T>> = new Observable<ResponseModel<T>>();
        return this.http.get<OdataModel<any>>(url, { withCredentials: withCredentials })
            .pipe(
                map(this.mapResultModel),
                catchError(this.exceptionHandlerHelper.handleError)
            ) as Observable<ResponseModel<T>>;
    }

    private getCountRequest(url: string, withCredentials: boolean): Observable<number> {
        let resultModel: Observable<number> = new Observable<number>();
        return this.http.get<any>(url, { withCredentials: withCredentials })
            .pipe(
                map(result => result.value),
                catchError(this.exceptionHandlerHelper.handleError)
            ) as Observable<number>;
    }

    private getBoolRequest(url: string, withCredentials: boolean): Observable<boolean> {
        let resultModel: Observable<boolean> = new Observable<boolean>();
        return this.http.get<any>(url, { withCredentials: withCredentials })
            .pipe(
                map(result => result.value),
                catchError(this.exceptionHandlerHelper.handleError)
            ) as Observable<boolean>;
    }

    private getAnyRequest(url: string, withCredentials: boolean): Observable<ResponseModel<any>> {
        let resultModel: Observable<ResponseModel<T>> = new Observable<ResponseModel<any>>();
        return this.http.get<OdataModel<any>>(url, { withCredentials: withCredentials })
            .pipe(
                map(result => result.value),
                catchError(this.exceptionHandlerHelper.handleError)
            ) as Observable<ResponseModel<T>>;
    }

    private postRequest(body: any, url: string, withCredentials: boolean): Observable<ResponseModel<T>> {
        let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        let options = {
            headers: headers,
            withCredentials: withCredentials
        };

        return this.http.post(url, body, options)
            .pipe(
                map(this.mapResultModel),
                catchError(this.exceptionHandlerHelper.handleError)
            ) as Observable<ResponseModel<T>>;
    }

    protected getResults(odataFunction: string, withCredentials: boolean = false): Observable<ResponseModel<T>> {
        let url: string = this.composeODataWebAPIUrl(odataFunction);
        return this.getRequest(url, withCredentials);
    }

    protected getAnyResults(odataFunction: string, withCredentials: boolean = false): Observable<ResponseModel<any>> {
        let url: string = this.composeODataWebAPIUrl(odataFunction);
        return this.getAnyRequest(url, withCredentials);
    }

    protected getAuthResults(odataFunction: string): Observable<ResponseModel<T>> {
        let url: string = this.composeODataWebAPIUrl(odataFunction);
        return this.getAuthRequest(url);
    }

    protected getCountResults(odataFunction: string, withCredentials: boolean = false): Observable<number> {
        let url: string = this.composeODataWebAPIUrl(odataFunction);
        return this.getCountRequest(url, withCredentials);
    }

    protected getBoolResult(odataFunction: string, withCredentials: boolean = false): Observable<boolean> {
        let url: string = this.composeODataWebAPIUrl(odataFunction);
        return this.getBoolRequest(url, withCredentials);
    }

    protected getAuthCountResults(odataFunction: string): Observable<number> {
        let url: string = this.composeODataWebAPIUrl(odataFunction);
        return this.getAuthCountRequest(url);
    }

    protected postResults(body: any, withCredentials: boolean = false): Observable<ResponseModel<T>> {
        let url: string = this.composeRestAPIUrl();
        return this.postRequest(body, url, withCredentials);
    }

    protected postAuthResults(body: any): Observable<ResponseModel<T>> {
        let url: string = this.globalSetting.apiAuthAddress;
        let headers = new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' });
        let options = { headers: headers };
        return this.http.post(url, body, options)
            .pipe(
                map(this.mapAuthResultModel),
                catchError(this.exceptionHandlerHelper.handleError)
            ) as Observable<ResponseModel<T>>;
    }

    mapAuthResultModel = (result: any): ResponseModel<T> => {
        let responseModel: ResponseModel<T> = new ResponseModel<T>();
        if (result) {
            responseModel.results = [this.mapper.mapFromJson(result)];
        }
        return responseModel;
    }

    mapResultModel = (result: OdataModel<any>): ResponseModel<T> => {
        let responseModel: ResponseModel<T> = new ResponseModel<T>();
        if (result && result.value) {
            responseModel.results = [];
            for (let item of result.value) {
                responseModel.results.push(this.mapper.mapFromJson(item));
            }
            if (result["@odata.count"]) {
                responseModel.count = result["@odata.count"];
            }
        }
        return responseModel;
    }

    composeODataWebAPIUrl(odataFunction: string): string {
        let address: string = this.globalSetting.apiOdataAddress;
        let controller: string = this.globalSetting.getController<T>(this.modelInstance);
        let baseUrl: string = address.concat(controller);

        if (odataFunction) {
            baseUrl = baseUrl.concat(odataFunction);
        }

        return baseUrl;
    }

    composeRestAPIUrl(): string {
        let address: string = this.globalSetting.apiRestAddress;
        let controller: string = this.globalSetting.getController<T>(this.modelInstance);
        let baseUrl: string = address.concat(controller);
        return baseUrl;
    }
}