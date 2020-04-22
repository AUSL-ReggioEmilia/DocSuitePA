import { Pipe, PipeTransform } from '@angular/core';
import { filterBy } from '@progress/kendo-data-query';

import { WorkflowStatusModel, WorkflowRequestState } from '@app-models/_index';

@Pipe({
  name: 'deniedRequestsCount'
})
export class DeniedRequestsCountPipe implements PipeTransform {

    transform(allRequests: WorkflowStatusModel[], args?: any): number {
        if (allRequests) {
            let results: WorkflowStatusModel[] = filterBy(allRequests, {
                logic: 'and',
                filters: [
                    { field: "Status", operator: "eq", value: 'Rejected' }
                ]
            });
            return results.length;
        }
        return 0;
    }

}
