import { Component, OnInit, Input } from '@angular/core';

import { WorkflowStatusModel } from '@app-models/_index';

@Component({
    selector: 'app-requests',
    templateUrl: './requests.component.html',
    styleUrls: ['./requests.component.css']
})
export class RequestsComponent implements OnInit {

    @Input()
    isBusy: boolean;

    @Input()
    requests: WorkflowStatusModel[];

    @Input()
    titleDescription: string;

    constructor() { }

    ngOnInit() {
    }

}
