import { Component, OnInit, Input } from '@angular/core';

import { ApplicationModel } from '@app-models/application.model';

@Component({
    selector: 'app-home-application',
    templateUrl: './home-application.component.html'
})
export class HomeApplicationComponent implements OnInit {

    @Input()
    currentApplication: ApplicationModel;

    constructor() { }

    ngOnInit() {
    }

}
