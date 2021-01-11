
import {map, catchError} from 'rxjs/operators';
import { Http, Response, Headers } from '@angular/http'; 
import { Observable, throwError } from 'rxjs';



import { AppConfigService } from '../services/app-config.service';
import { BaseMapper } from '../mappers/base.mapper';
import { ODataResultModel } from '../models/odata-result.model'; 

export class BaseService<T> {

    mapper: BaseMapper;

    constructor(private http: Http, mapper: BaseMapper) {
        this.mapper = mapper;
    }

    private getRequest(url: string): Observable<ODataResultModel> {
        let resultModel: Observable<ODataResultModel> = new Observable<ODataResultModel>();
        let response: Observable<Response> = this.http.get(url);
        resultModel = response.pipe(map(this.mapResultModel),catchError(this.errorHandler)) as Observable<ODataResultModel>;
        return resultModel;
    }

    private getCountRequest(url: string): Observable<number> {
        let resultModel: Observable<number> = new Observable<number>();
        let response: Observable<Response> = this.http.get(url);
        resultModel = response.pipe(map(result => result.json().value),catchError(this.errorHandler)) as Observable<number>;
        return resultModel;
    }

    protected getResults(odataFilter: string): Observable<ODataResultModel> {
        let url: string = AppConfigService.settings.apiOdataAddress.concat(odataFilter);
        return this.getRequest(url);
    }

    protected getCountResults(odataFilter: string): Observable<number> {
        let url: string = AppConfigService.settings.apiOdataAddress.concat(odataFilter);
        return this.getCountRequest(url);
    }

    private mapResultModel = (result: Response): ODataResultModel => {
        let odataModel: ODataResultModel = new ODataResultModel();
        odataModel.totalCount = result.json()['@odata.count'];
        odataModel.results = result.json().value.map(data => this.mapper.mapFromJson(data));
        return odataModel;
    }

    private errorHandler = (error:any): any => {
        console.error(error);
        return throwError(error);
    }

    
}