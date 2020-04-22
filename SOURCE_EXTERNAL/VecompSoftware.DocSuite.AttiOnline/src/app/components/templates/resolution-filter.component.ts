import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { GroupDescriptor, process } from '@progress/kendo-data-query';

import { SearchModel } from '../../models/search.model'; 

@Component({
    moduleId: module.id,
    selector: 'resolution-filter',
    templateUrl: 'resolution-filter.component.html',
    styleUrls: [ 'resolution-filter.component.css' ]
})
export class ResolutionFilterComponent implements OnInit{
    @Input() model: SearchModel
    @Output() onPanelChange: EventEmitter<any> = new EventEmitter<any>()
    @Output() onSubmitSearch: EventEmitter<any> = new EventEmitter<any>()

    ngOnInit() {
        //this.gridView = process(this.gridData, { group: this.groups });
    }

    panelChange(event: any) {
        this.onPanelChange.emit(event);
    }

    submitSearch(event: SearchModel) {
        this.onSubmitSearch.emit(event);
    }
}