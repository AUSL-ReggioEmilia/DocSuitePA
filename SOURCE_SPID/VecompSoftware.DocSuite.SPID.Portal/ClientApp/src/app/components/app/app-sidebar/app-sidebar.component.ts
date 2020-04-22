import { Component, OnInit, Inject, Input } from '@angular/core';

import { ApplicationModel } from '@app-models/application.model';

@Component({
    selector: 'app-sidebar',
    templateUrl: './app-sidebar.component.html',
    styleUrls: ['./app-sidebar.component.css']
})
export class AppSidebarComponent implements OnInit {

    @Input()
    applications: ApplicationModel[] = [];
    @Input()
    isBusy: boolean = false;

    constructor() { }

    ngOnInit() {        
        
    }
}
