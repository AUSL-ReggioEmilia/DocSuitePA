import { Component, OnInit, PLATFORM_ID, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { isPlatformBrowser } from '@angular/common';

import { UserModel } from '@app-models/_index';
import { StartWorkflowModel } from '@app-models/workflows/start-workflow.model';
import { StartWorkflowMapper } from '@app-mappers/workflows/start-workflow.mapper';
import { NewRequestViewModel } from '@app-viewmodels/requests/new-request.viewmodel';
import { AccessType } from '@app-viewmodels/requests/access-type.enum';
import { UserType } from '@app-viewmodels/requests/user-type.enum';
import { ReturnType } from '@app-viewmodels/requests/return-type.enum';
import { WorkflowService } from '@app-services/workflows/workflow.service';
import { UserService } from '@app-services/shared/user.service';

@Component({
    selector: 'app-new-request',
    templateUrl: './new-request.component.html',
    styleUrls: ['./new-request.component.css']
})
export class NewRequestComponent implements OnInit {
    model: NewRequestViewModel = <NewRequestViewModel>{};
    isBusy: boolean;

    constructor(private workflowService: WorkflowService,
        private startWorkflowMapper: StartWorkflowMapper,
        private router: Router, private userService: UserService, @Inject(PLATFORM_ID) private platformId: Object) { }

    ngOnInit() {
        this.model.AccessType = AccessType.Vision;
        this.model.UserType = UserType.Interested;
        this.model.ReturnType = ReturnType.PEC;
        this.model.DateOfBirth = new Date();
        this.isBusy = false;

        let userToken: UserModel | null = this.userService.currentUser;
        if (userToken) {
            this.model.ExternalCode = userToken.Id;
            if (userToken.DateOfBirth) {
                this.model.DateOfBirth = new Date(userToken.DateOfBirth);
            }
            this.model.PlaceOfBirth = userToken.PlaceOfBirth;
            this.model.Name = userToken.Name;
            this.model.Surname = userToken.Surname;
            this.model.Address = userToken.Address;
            this.model.City = userToken.City;
            this.model.CivicNumber = userToken.CivicNumber;
            this.model.Email = userToken.Email;
            this.model.PEC = userToken.PEC;
        }
    }

    onClick() {
        this.isBusy = true;
        let commandModel: StartWorkflowModel = this.startWorkflowMapper.map(this.model);
        this.workflowService.startNewWorkflow(commandModel).subscribe(data => { this.router.navigate(['/le-mie-richieste']); });
    }
}