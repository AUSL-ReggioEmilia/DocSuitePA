import TreeFlatNode = require('App/Core/TreeStructures/TreeFlatNode');

class TreeFlat<TNodeContent> {

    private _lazyLoadedFlatNodes: TreeFlatNode<TNodeContent>[] = [];

    constructor(private idGrabber: (content: TNodeContent) => string) { }

    public AddFirstLevelNode(model: TNodeContent): void {
        // removing if already exists
        this.RemoveNode(this.idGrabber(model));
        this._lazyLoadedFlatNodes.push(new TreeFlatNode('', model));
    }

    public GetFirstLevelNodes(): TreeFlatNode<TNodeContent>[] {
        let nodes = [];
        for (let node of this._lazyLoadedFlatNodes) {
            if (node.ParentId === '') {
                nodes.push(node);
            }
        }
        return nodes;
    }

    public GetChildNodes(parentNode: TreeFlatNode<TNodeContent>): TreeFlatNode<TNodeContent>[] {
        let nodes = [];
        let parentNodeId: string = this.idGrabber(parentNode.Model);
        for (let node of this._lazyLoadedFlatNodes) {
            if (node.ParentId === parentNodeId) {
                nodes.push(node);
            }            
        }
        return nodes;
    }

    public FindParent(node: TreeFlatNode<TNodeContent>): TreeFlatNode<TNodeContent> {
        return this._lazyLoadedFlatNodes.filter(r => this.idGrabber(r.Model).toLowerCase() == node.ParentId.toLowerCase())[0];
    }

    public FindNode(uniqueId: string): TreeFlatNode<TNodeContent> {
        return this._lazyLoadedFlatNodes.filter(r => this.idGrabber(r.Model).toLowerCase() == uniqueId.toLowerCase())[0];
    }

    public AddChildNodes(parentNode: TreeFlatNode<TNodeContent>, children: TNodeContent[]) {
        //TODO: remove parent child before adding
        for (let child of children) {
            this.AddChildNode(parentNode, child);
        }
    }

    public AddChildNode(parentNode: TreeFlatNode<TNodeContent>, child: TNodeContent) {
        this._lazyLoadedFlatNodes.push(new TreeFlatNode<TNodeContent>(this.idGrabber(parentNode.Model), child));
    }

    private RemoveNode(id: string): void {
        let existing = this._lazyLoadedFlatNodes.filter(r => this.idGrabber(r.Model) === id)[0];
        if (existing !== null && existing !== undefined) {
            const index = this._lazyLoadedFlatNodes.indexOf(existing);
            this._lazyLoadedFlatNodes.splice(index, 1);
        }
    }

    public Clear(): void {
        this._lazyLoadedFlatNodes = [];
    }
}

export = TreeFlat;