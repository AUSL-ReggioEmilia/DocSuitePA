import { Component, OnInit } from '@angular/core';

import { SpidApplicationService } from '@app-services/spidapp/spid-application.service';
import { ApplicationModel } from '@app-models/application.model';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

    isBusy: boolean = false;
    applications: ApplicationModel[] = [];

    constructor(private service: SpidApplicationService) { 
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
