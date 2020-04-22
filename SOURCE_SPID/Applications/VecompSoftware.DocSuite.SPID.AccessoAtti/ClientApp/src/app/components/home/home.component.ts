import { Component, OnInit } from '@angular/core';

import { WorkflowStatusModel } from '@app-models/_index';
import { WorkflowStatusService } from '@app-services/workflows/workflow-status.service';
import { UserService } from '@app-services/shared/user.service';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

    isBusy: boolean;
    requests: WorkflowStatusModel[];

    constructor(private service: WorkflowStatusService, private userService: UserService) { }

    ngOnInit() {
        if (this.userService.currentUser) {
            this.isBusy = true;
            this.service.getMyRequests(this.userService.currentUser.Id).subscribe(data => {
                this.requests = data.value;
                this.isBusy = false;
            });
        }
    }

}
