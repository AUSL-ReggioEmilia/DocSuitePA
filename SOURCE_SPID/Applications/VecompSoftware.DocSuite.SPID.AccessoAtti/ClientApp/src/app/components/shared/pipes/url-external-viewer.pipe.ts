import { Pipe, PipeTransform } from '@angular/core';

import { FascicleModel } from '@app-models/fascicles/_index';
import { ConfigurationService } from '@app-services/shared/configuration.service';

@Pipe({
    name: 'urlExternalViewer'
})
export class UrlExternalViewerPipe implements PipeTransform {

    constructor(private configurationService: ConfigurationService) {}

    transform(fascicleModel: FascicleModel, args?: any): string {
        if (fascicleModel && fascicleModel.Id) {
            return this.configurationService.config.ExternalViewerBaseUrl.concat('/', fascicleModel.Id);
        }
        return '';
    }

}
