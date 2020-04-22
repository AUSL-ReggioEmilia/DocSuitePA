import { Component, OnInit, Inject } from '@angular/core';
import { orderBy } from '@progress/kendo-data-query';

import { WorkflowStatusModel } from '@app-models/_index';
import { WorkflowStatusService } from '@app-services/workflows/workflow-status.service';
import { UserService } from '@app-services/shared/user.service';

@Component({
    selector: 'app-my-requests',
    templateUrl: './my-requests.component.html',
    styleUrls: ['./my-requests.component.css']
})
export class MyRequestsComponent implements OnInit {

    title: string= "Le mie richieste";
    isBusy: boolean = false;
    requests: WorkflowStatusModel[];

    constructor(private service: WorkflowStatusService, private userService: UserService) {
        
    }

    ngOnInit() {
        if (this.userService.currentUser) {
            this.isBusy = true;
            this.service.getMyRequests(this.userService.currentUser.Id).subscribe(data => {
                if (data.value) {
                    const sortedResults = orderBy(data.value, [{ field: "Date", dir: "desc" }]);
                    this.requests = sortedResults;
                }
                this.isBusy = false;
            });
        }
    }
}
