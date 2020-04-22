import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { ConfigurationModel } from '@app-models/configuration.model';

@Injectable()
export class ConfigurationService {
    private readonly configUrlPath: string = 'api/ClientConfiguration';
    private configData: ConfigurationModel;

    constructor(private http: HttpClient, @Inject('BASE_URL') private originUrl: string,
        @Inject(PLATFORM_ID) private platformId: Object) { }

    loadConfigurationData(): Promise<ConfigurationModel> {
        if (isPlatformBrowser(this.platformId)) {
            return this.http.get(`${this.originUrl}${this.configUrlPath}`, { responseType: 'json' })
                .toPromise().then(config => {
                    this.configData = config as ConfigurationModel;
                    return this.configData;
                })
                .catch(err => { return Promise.reject(err) });
        }
        return Promise.resolve(this.configData);
    }

    get config(): ConfigurationModel {
        return this.configData;
    }
}
