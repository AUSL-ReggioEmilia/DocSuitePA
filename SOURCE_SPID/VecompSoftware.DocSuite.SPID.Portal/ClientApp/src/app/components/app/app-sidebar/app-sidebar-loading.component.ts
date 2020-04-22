import { Component, OnInit, Input } from '@angular/core';

@Component({
    selector: 'app-sidebar-loading',
    templateUrl: './app-sidebar-loading.component.html'
})
export class AppSidebarLoadingComponent implements OnInit {

    @Input()
    show: boolean;

    constructor() { }

    ngOnInit() {
    }

}
