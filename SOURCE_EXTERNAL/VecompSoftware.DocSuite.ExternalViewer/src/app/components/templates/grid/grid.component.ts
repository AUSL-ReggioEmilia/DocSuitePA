import { Component, OnInit, Input, Output, EventEmitter} from '@angular/core';
import { PageChangeEvent, GridDataResult, } from '@progress/kendo-angular-grid';
import { GroupDescriptor, process } from '@progress/kendo-data-query';

@Component({
    moduleId: module.id,
    selector: 'grid',
    templateUrl: 'grid.component.html',
    styleUrls: ['grid.component.css']
})
export class GridComponent implements OnInit {
    @Input() gridData: GridDataResult
    @Input() groups: GroupDescriptor[]
    @Input() paginationEnabled: boolean
    @Input() skip: number
    @Input() pageSize: number
    @Input() groupable: boolean
    @Output() onPageChange: EventEmitter<any> = new EventEmitter<any>()

    private groupableEnabled: boolean;

    constructor() {
    }

    ngOnInit() { }

    pageChange(event: PageChangeEvent) {
        this.onPageChange.emit(event);
    }
}