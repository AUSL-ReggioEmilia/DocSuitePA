import { Injectable } from '@angular/core'; 
 
import { GenericContactModel } from '../../models/generic-contact.model'; 
import { TreeListModel } from '../../models/commons/tree-list.model'; 
import { BaseHelper } from '../../helpers/base.helper'; 

@Injectable()
export class ContactTreeListMapper<T extends GenericContactModel<T>> {

    constructor(private baseHelper: BaseHelper) { }

    mapToTreeList(model: T): TreeListModel {

        let treeList: TreeListModel = new TreeListModel();
        treeList.id = model.id;
        treeList.name = model.name;
        treeList.hasChildren = !!model.children && model.children.length > 0;
        treeList.isSelected = model.isSelected ? true : false;
        if (treeList.hasChildren) {
            treeList.children = model.children.map(item => this.mapToTreeList(item));
        }
        treeList.imageUrl = this.baseHelper.setContactIconUrl(model.type);

        return treeList;
    }

}