import { Component, OnInit } from '@angular/core';

import { SpidApplicationService } from '@app-services/spidapp/spid-application.service';
import { ApplicationModel } from '@app-models/application.model';
import { ConfigurationService } from '@app-services/shared/configuration.service';

@Component({
    selector: 'app-full-layout',
    templateUrl: './full-layout.component.html'
})
export class FullLayoutComponent implements OnInit {

    applications: ApplicationModel[] = [];
    isBusy: boolean = false;
    applicationName: string;

    constructor(private service: SpidApplicationService, configurationService: ConfigurationService) {
        this.applicationName = configurationService.config.ApplicationName;
    }

    ngOnInit() {
        this.isBusy = true;
        this.service.getApplications().subscribe((result) => {
            if (result) {
                this.applications = result.value;
            }
            this.isBusy = false;
        });
    }

}
