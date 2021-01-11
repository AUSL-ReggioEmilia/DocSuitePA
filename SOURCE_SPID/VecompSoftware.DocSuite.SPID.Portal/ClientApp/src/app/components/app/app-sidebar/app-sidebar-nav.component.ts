import { Component, OnInit, Input } from '@angular/core';

import { ApplicationModel } from '@app-models/application.model';

@Component({
    selector: 'app-sidebar-nav',
    templateUrl: './app-sidebar-nav.component.html'
})
export class AppSidebarNavComponent implements OnInit {

    @Input()
    currentApplication: ApplicationModel;

    constructor() { }

    ngOnInit() {
        
    }

}
