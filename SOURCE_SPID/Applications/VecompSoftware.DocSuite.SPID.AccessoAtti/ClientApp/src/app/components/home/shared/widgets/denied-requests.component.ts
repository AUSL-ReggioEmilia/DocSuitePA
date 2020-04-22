import { Component, OnInit, Input } from '@angular/core';

import { WorkflowStatusModel } from '@app-models/_index';

@Component({
    selector: 'app-denied-requests',
    templateUrl: './denied-requests.component.html'
})
export class DeniedRequestsComponent implements OnInit {

    @Input()
    requests: WorkflowStatusModel[];

    @Input()
    isBusy: boolean;

    constructor() { }

    ngOnInit() {
    }

}
