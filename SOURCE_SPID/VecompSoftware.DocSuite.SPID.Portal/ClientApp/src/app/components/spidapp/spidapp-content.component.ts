import { Component, OnInit, Input } from '@angular/core';

import { ApplicationModel } from '@app-models/application.model';

@Component({
    selector: 'app-spidapp-content',
    templateUrl: './spidapp-content.component.html'
})
export class SpidappContentComponent implements OnInit {

    @Input()
    currentApplication: ApplicationModel;

    constructor() { }

    ngOnInit() {
    }

}
