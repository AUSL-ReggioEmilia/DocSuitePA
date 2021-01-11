import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';

import { SpidApplicationService } from '@app-services/spidapp/spid-application.service';
import { ApplicationModel } from '@app-models/application.model';

@Component({
    selector: 'app-spidapp',
    templateUrl: './spidapp.component.html',
    styleUrls: ['./spidapp.component.css']
})
export class SpidAppComponent implements OnInit {

    private applicationId: string|null;
    private parametersObservable: any;
    isBusy: boolean = false;

    currentApplication: ApplicationModel = <ApplicationModel>{};

    constructor(private route: ActivatedRoute, private router: Router, private service: SpidApplicationService) {
        this.applicationId = route.snapshot.paramMap.get('id');
    }

    ngOnInit() {        
        if (this.applicationId) {
            this.isBusy = true;
            this.service.getApplication(this.applicationId).subscribe((result) => {
                if (result && result.value.length && result.value.length > 0) {
                    this.currentApplication = result.value[0];
                } else {
                    this.router.navigate(['/pages/404']);
                }
                this.isBusy = false;
            });
        } else {
            this.router.navigate(['/pages/404']);
        }
    }

}
