import { Pipe, PipeTransform } from '@angular/core';
import { filterBy } from '@progress/kendo-data-query';

import { WorkflowStatusModel, WorkflowRequestState } from '@app-models/_index';

@Pipe({
    name: 'pendingRequestsCount'
})
export class PendingRequestsCountPipe implements PipeTransform {

    transform(allRequests: WorkflowStatusModel[], args?: any): number {
        if (allRequests) {
            let results: WorkflowStatusModel[] = filterBy(allRequests, {
                logic: 'or',
                filters: [
                    { field: "Status", operator: "eq", value: 'Processing' },
                    { field: "Status", operator: "eq", value: 'Evaluating' },
                    { field: "Status", operator: "eq", value: 'Requested' }
                ]
            });
            return results.length;
        }
        return 0;
    }

}
