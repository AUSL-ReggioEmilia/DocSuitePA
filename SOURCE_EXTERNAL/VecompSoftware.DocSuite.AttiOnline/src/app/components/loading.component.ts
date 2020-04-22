import { Component } from '@angular/core';

import { LoadingSpinnerService } from '../services/loading-spinner.service';

import { Router, Event, NavigationStart, NavigationEnd, NavigationCancel, NavigationError } from '@angular/router';

@Component({
    moduleId: module.id,
    selector: 'loading-spinner',
    templateUrl: 'loading.component.html'
})
export class LoadingComponent {
    display: boolean;

    constructor(private router: Router, private loadingSpinnerService: LoadingSpinnerService) {
        loadingSpinnerService.status.subscribe((status: boolean) => {
            this.display = status;
        });
    }
}