import { Injectable } from '@angular/core'; 
 
import { ProtocolUserModel } from '../../models/protocols/protocol-user.model'; 
import { TreeListModel } from '../../models/commons/tree-list.model'; 

@Injectable()
export class UserTreeListMapper {

    mapToTreeList(model: ProtocolUserModel): TreeListModel {

        let treeList: TreeListModel = new TreeListModel();
        treeList.id = model.id;
        treeList.name = model.account;
        treeList.imageUrl = 'app/images/sectors/user.png';
        treeList.isSelected = true;

        return treeList;
    }

}