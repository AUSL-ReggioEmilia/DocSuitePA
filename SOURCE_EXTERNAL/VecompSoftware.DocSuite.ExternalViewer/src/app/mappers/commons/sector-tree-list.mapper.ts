import { Injectable } from '@angular/core'; 
 
import { ProtocolSectorModel } from '../../models/protocols/protocol-sector.model'; 
import { TreeListModel } from '../../models/commons/tree-list.model'; 

@Injectable()
export class SectorTreeListMapper {

    mapToTreeList(model: ProtocolSectorModel): TreeListModel {

        let treeList: TreeListModel = new TreeListModel();
        treeList.id = model.id;
        treeList.name = model.name;
        treeList.hasChildren = !!model.children && model.children.length > 0;
        treeList.isSelected = model.authorized ? true : false;
        if (treeList.hasChildren) {
            treeList.children = model.children.map(item => this.mapToTreeList(item));
        }
        treeList.imageUrl = !model.uniqueIdFather ? 'app/images/sectors/bricks.png' : 'app/images/sectors/brick.png';

        return treeList;
    }

}