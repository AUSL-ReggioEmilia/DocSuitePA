class TreeFlatNode<TNodeContent> {
    constructor(public ParentId: string, public Model: TNodeContent) {
    }
}

export = TreeFlatNode;