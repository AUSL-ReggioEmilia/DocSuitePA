import { Component, OnInit, Input } from '@angular/core';

import { WorkflowStatusModel, WorkflowRequestState, FascicleModel } from '@app-models/_index';

@Component({
    selector: 'app-request-grid',
    templateUrl: './request-grid.component.html',
    styleUrls: ['./request-grid.component.css']
})
export class RequestGridComponent {

    @Input()
    requests: WorkflowStatusModel[];

    constructor() { }

    deserializeModel(model: string): FascicleModel | null {
        return (!!model) ? JSON.parse(model) : null;
    }

    getStatusClass(state: string): string {
        switch (state) {
            case 'Evaluating':
            case 'Requested':
            case 'Processing':
                return 'fa fa-2x fa-clock-o text-warning';
            case 'Confirmed':
                return 'fa fa-2x fa-check-circle-o text-success';
            case 'Rejected':
                return 'fa fa-2x fa-ban text-danger';
            default:
                return '';
        }
    }

    getStatusDescription(state: string): string {
        switch (state) {
            case 'Evaluating':
                return 'in valutazione';
            case 'Requested':
                return 'richiesta';
            case 'Processing':
                return 'in lavorazione';
            case 'Confirmed':
                return 'esibita';
            case 'Rejected':
                return 'diniego';
            default:
                return '';
        }
    }
}
