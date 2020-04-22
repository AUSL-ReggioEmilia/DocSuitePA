import { ItemType } from './item-type.helper';
import { BFSTreeNode } from './tree-node.helper';

export class AdaptedBFSTree {
    text: string;
    value: string;
    fascicleId: string;
    path: number[];
    itemType: ItemType;
    imageUrl: string;
    searchedNodeFont: string;
    items: BFSTreeNode[];

    constructor(text: string, value: string, path: number[], itemType: ItemType, fascicleId: string = "") {
        this.text = text;
        this.value = value;
        this.fascicleId = fascicleId;
        this.path = path;
        this.itemType = itemType;
        this.items = [];
    }

    traverseBF(childNode: BFSTreeNode, parentPath: number[]): BFSTreeNode {
        let collection: any[] = [
            {
                text: this.text,
                value: this.value,
                fascicleId: this.fascicleId,
                path: this.path,
                itemType: this.itemType,
                items: this.items
            }
        ];
        while (collection.length) {
            let node: BFSTreeNode = collection.shift();
            //if parent exists
            if (JSON.stringify(node.path) === JSON.stringify(parentPath)) {
                //update child psition
                childNode.path[childNode.path.length - 1] = node.items.length + 1;
                //add child to parent
                node.items.push(childNode);
                return childNode;
            }
            else {
                //go to the next node
                collection.push(...node.items);
            }
        }
        return childNode;
    }
}