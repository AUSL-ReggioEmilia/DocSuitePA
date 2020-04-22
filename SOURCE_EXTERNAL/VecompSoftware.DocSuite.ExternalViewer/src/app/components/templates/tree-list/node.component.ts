import { Component, Input } from '@angular/core';

import { TreeListModel } from '../../../models/commons/tree-list.model';

@Component({
    moduleId: module.id,
    selector: 'node',
    templateUrl: 'node.component.html',
    styleUrls: [ 'node.component.css']
})

export class NodeComponent {
    @Input() item: TreeListModel;
}