import { Component, OnInit, Input } from '@angular/core';

import { WorkflowStatusModel } from '@app-models/_index';

@Component({
    selector: 'app-confirmed-requests',
    templateUrl: './confirmed-requests.component.html'
})
export class ConfirmedRequestsComponent implements OnInit {

    @Input()
    requests: WorkflowStatusModel[];

    @Input()
    isBusy: boolean;

    constructor() { }

    ngOnInit() {
    }

}
