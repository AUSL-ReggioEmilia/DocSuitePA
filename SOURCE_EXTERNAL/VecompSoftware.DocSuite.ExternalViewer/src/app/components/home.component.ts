import { Component } from '@angular/core';

import { AppConfigService } from '../services/commons/app-config.service';
import { LoadingSpinnerService } from '../services/commons/loading-spinner.service';

@Component({
    moduleId: module.id,
    templateUrl: 'home.component.html',
    styleUrls: ['home.component.css']
})


export class HomeComponent {

    intro: string = `Per visualizzare un protocollo, comporre l'URL indicando l'identificativo o la coppia anno e numero.`;
    titleIntroduction: string = `External Viewer`;
    appLogo: string = `app/images/app-logo.png`;

    constructor(private spinnerService: LoadingSpinnerService) {
        this.spinnerService.stop();
    }

    ngOnInit() {
        if (AppConfigService.settings.appLogo) {
            this.appLogo = AppConfigService.settings.appLogo;
        }
        if (AppConfigService.settings.introduction) {
            this.intro = AppConfigService.settings.introduction;
        }

        if (AppConfigService.settings.titleIntroduction) {
            this.titleIntroduction = AppConfigService.settings.titleIntroduction;
        }        
    }
} 