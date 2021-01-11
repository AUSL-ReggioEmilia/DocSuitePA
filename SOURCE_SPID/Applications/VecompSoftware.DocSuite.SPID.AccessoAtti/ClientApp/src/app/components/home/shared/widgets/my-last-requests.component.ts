import { Component, OnInit, Input } from '@angular/core';

import { WorkflowStatusModel } from '@app-models/_index';

@Component({
    selector: 'app-my-last-requests',
    templateUrl: './my-last-requests.component.html'
})
export class MyLastRequestsComponent implements OnInit {

    title: string = "Le mie ultime richieste";
    @Input()
    isBusy: boolean = true;

    @Input()
    requests: WorkflowStatusModel[];

    constructor() { }

    ngOnInit() {
        
    }

}
