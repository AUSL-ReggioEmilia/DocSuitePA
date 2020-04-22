import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { ConfigurationModel } from '@app-models/config/configuration.model';

@Injectable()
export class ConfigurationService {
    private readonly configUrlPath: string = 'api/ClientConfiguration';
    private configData: ConfigurationModel;

    constructor(private http: HttpClient, @Inject('BASE_URL') private originUrl: string) { }

    loadConfigurationData(): Promise<ConfigurationModel> {
        return this.http.get(`${this.originUrl}${this.configUrlPath}`)
            .toPromise()
            .then(config => {
                this.configData = config as ConfigurationModel;
                return this.configData;
            })
            .catch(err => { return Promise.reject(err) });
    }

    get config(): ConfigurationModel {
        return this.configData;
    }
}
