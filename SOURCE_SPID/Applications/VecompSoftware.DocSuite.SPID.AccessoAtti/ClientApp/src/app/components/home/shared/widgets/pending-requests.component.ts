import { Component, OnInit, Input } from '@angular/core';

import { WorkflowStatusModel } from '@app-models/_index';

@Component({
    selector: 'app-pending-requests',
    templateUrl: './pending-requests.component.html'
})
export class PendingRequestsComponent implements OnInit {

    @Input()
    requests: WorkflowStatusModel[];

    @Input()
    isBusy: boolean;

    constructor() { }

    ngOnInit() {
    }

}
