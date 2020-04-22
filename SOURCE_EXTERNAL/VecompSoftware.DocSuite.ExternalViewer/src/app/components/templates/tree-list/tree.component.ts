import { Component, OnInit, Input } from '@angular/core';

import { TreeListModel } from '../../../models/commons/tree-list.model';

@Component({
    moduleId: module.id,
    selector: 'tree',
    templateUrl: 'tree.component.html'
})

export class TreeComponent {
    @Input() data: TreeListModel[];

}