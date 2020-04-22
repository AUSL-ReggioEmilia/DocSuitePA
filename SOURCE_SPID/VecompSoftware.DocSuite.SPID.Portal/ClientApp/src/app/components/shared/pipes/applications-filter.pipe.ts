import { Pipe, PipeTransform } from '@angular/core';

import { ApplicationModel } from '@app-models/application.model';

@Pipe({
    name: 'applicationsFilter'
})
export class ApplicationsFilterPipe implements PipeTransform {

    transform(items: ApplicationModel[], args?: any): any {
        if (args && items) {
            return items.filter(item => item.Name.toLowerCase().indexOf(args.toLowerCase()) !== -1);
        }
        return items;
    }

}
