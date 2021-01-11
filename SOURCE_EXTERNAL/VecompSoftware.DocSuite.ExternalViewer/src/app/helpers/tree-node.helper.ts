import { ItemType } from './item-type.helper';

export class BFSTreeNode {
    text: string;
    value: string;
    fascicleId: string;
    path: number[];
    itemType: ItemType;
    imageUrl: string;
    searchedNodeFont: string;
    loadingNode: boolean;
    items: BFSTreeNode[];

    constructor(text: string, value: string, path: number[], itemType: ItemType, fascicleId: string = "") {
        this.text = text;
        this.value = value;
        this.fascicleId = fascicleId;
        this.path = path;
        this.itemType = itemType;
        this.items = [];
    }
}