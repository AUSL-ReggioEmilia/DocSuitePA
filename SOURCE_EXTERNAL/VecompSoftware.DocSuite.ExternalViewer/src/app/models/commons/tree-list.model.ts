
export class TreeListModel {
    id: string;
    name: string;
    children: TreeListModel[];
    hasChildren: boolean;
    isSelected: boolean;
    imageUrl: string;
    isRoot: boolean;
    isUser: boolean;
}