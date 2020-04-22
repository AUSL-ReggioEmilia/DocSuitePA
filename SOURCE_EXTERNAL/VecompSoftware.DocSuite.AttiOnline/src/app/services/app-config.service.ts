import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { environment } from '../../environments/environment';
import { AppConfig } from '../models/app-config.model';

@Injectable()
export class AppConfigService {

    static settings: AppConfig;

    constructor(private http: Http) { }

    load() {
        const jsonFile = `app/config/config.${environment.production ? 'prod' : 'dev'}.json`;
        return new Promise<void>((resolve, reject) => {
            this.http.get(jsonFile).toPromise().then((response: Response) => {
                AppConfigService.settings = <AppConfig>response.json();
                resolve();
            }).catch((response: any) => {
                reject(`Could not load file '${jsonFile}': ${JSON.stringify(response)}`);
            });
        });
    }
}