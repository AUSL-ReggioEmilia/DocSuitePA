import { Injectable } from '@angular/core';

import { AppConfigService } from '../services/commons/app-config.service';
import { API_CONTROLLERS_VALUES } from './static.setting';
import { ProtocolModel } from '../models/protocols/protocol.model';
import { FascicleModel } from '../models/fascicles/fascicle.model';
import { BaseServiceModel } from '../models/base-service.model';
import { BaseModelType } from '../globals';

export const ERROR_PAGE_NAME: string = 'error-page';

@Injectable()
export class GlobalSetting {
    apiOdataAddress: string;
    apiAuthAddress: string;
    apiRestAddress: string;

    constructor() {
        this.apiOdataAddress = AppConfigService.settings.apiOdataAddress;
        this.apiAuthAddress = AppConfigService.settings.apiAuthAddress;
        this.apiRestAddress = AppConfigService.settings.apiRestAddress;
    }

    getController<T extends BaseServiceModel>(arg: BaseModelType): string {
        switch ((arg as any).serviceModelName) {
            case "FascicleModel": {
                if (AppConfigService.settings.apiOdataControllers["fascicle"]) {
                    return API_CONTROLLERS_VALUES.fascicle;
                }
                break;
            }
            case "PECMailModel": {
                if (AppConfigService.settings.apiOdataControllers["pecMail"]) {
                    return API_CONTROLLERS_VALUES.pecMail;
                }
                break;
            }
            case "ProtocolModel": {
                if (AppConfigService.settings.apiOdataControllers["protocol"]) {
                    return API_CONTROLLERS_VALUES.protocol;
                }
                break;
            }
            case "DocumentModel": {
                if (AppConfigService.settings.apiOdataControllers["document"]) {
                    return API_CONTROLLERS_VALUES.document;
                }
                break;
            }
            case "DocumentUnitModel": {
                if (AppConfigService.settings.apiOdataControllers["documentUnit"]) {
                    return API_CONTROLLERS_VALUES.documentUnit;
                }
                break;
            }
            case "DossierModel": {
                if (AppConfigService.settings.apiOdataControllers["dossier"]) {
                    return API_CONTROLLERS_VALUES.dossier;
                }
                break;
            }
            case "DossierFolderModel": {
                if (AppConfigService.settings.apiOdataControllers["dossierFolder"]) {
                    return API_CONTROLLERS_VALUES.dossierFolder;
                }
                break;
            }
            case "FascicleFolderModel": {
                if (AppConfigService.settings.apiOdataControllers["fascicleFolder"]) {
                    return API_CONTROLLERS_VALUES.fascicleFolder;
                }
                break;
            }
            case "ErrorLogModel": {
                if (AppConfigService.settings.apiOdataControllers["errorLog"]) {
                    return API_CONTROLLERS_VALUES.errorLog;
                }
                break;
            }
        };
    }

    getAuthUsername(appId: string): string {
        let result: string = AppConfigService.settings.oauthUsername[appId];
        return result;
    }
}