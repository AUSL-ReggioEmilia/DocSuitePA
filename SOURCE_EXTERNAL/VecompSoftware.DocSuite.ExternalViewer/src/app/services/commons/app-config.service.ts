import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { AppConfig } from '../../models/config/app-config-model';

@Injectable()
export class AppConfigService {

    static settings: AppConfig;

    constructor(private http: HttpClient) { }

    load() {
        const jsonFile = `app/config/config.${environment.production ? 'prod' : 'dev'}.json`;
        return new Promise<void>((resolve, reject) => {
            this.http.get<AppConfig>(jsonFile)
                .toPromise().then((response: AppConfig) => {
                    AppConfigService.settings = response;
                    resolve();
                })
                .catch((response: any) => {
                    reject(`Could not load file '${jsonFile}': ${JSON.stringify(response)}`);
                });
        });
    }
}