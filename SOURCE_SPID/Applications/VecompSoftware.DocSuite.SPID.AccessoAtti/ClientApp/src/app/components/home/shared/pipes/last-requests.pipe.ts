import { Pipe, PipeTransform } from '@angular/core';
import { process } from '@progress/kendo-data-query';

import { WorkflowStatusModel } from '@app-models/_index';

@Pipe({
    name: 'lastRequests'
})
export class LastRequestsPipe implements PipeTransform {

    transform(allRequests: WorkflowStatusModel[], args?: any): WorkflowStatusModel[] {
        if (allRequests) {
            const filteredResults = process(allRequests, {
                skip: 0,
                take: 3,
                sort: [{ field: "Date", dir: "desc" }]
            });
            return filteredResults.data as WorkflowStatusModel[];
        }
        return [] as WorkflowStatusModel[];
    }

}
