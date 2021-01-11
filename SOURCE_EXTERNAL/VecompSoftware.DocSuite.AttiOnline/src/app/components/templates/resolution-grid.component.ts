import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { PageChangeEvent, GridDataResult } from '@progress/kendo-angular-grid';
import { GroupDescriptor, process } from '@progress/kendo-data-query';

@Component({
    moduleId: module.id,
    selector: 'resolution-grid',
    templateUrl: 'resolution-grid.component.html',
    styleUrls: [ 'resolution-grid.component.css' ]
})
export class ResolutionGridComponent implements OnInit{
    @Input() gridData: GridDataResult
    @Input() groups: GroupDescriptor[]
    @Input() paginationEnabled: boolean
    @Input() skip: number
    @Input() pageSize: number
    @Input() publicationDateVisible: boolean
    @Input() executiveDateVisible: boolean
    @Input() serviceNumberVisible: boolean
    @Input() proposerColumnVisible: boolean
    @Input() groupable: boolean
    @Output() onPageChange: EventEmitter<any> = new EventEmitter<any>()

    private groupableEnabled: boolean;

    ngOnInit() {    }

    pageChange(event: PageChangeEvent) {
        this.onPageChange.emit(event);
    }
}